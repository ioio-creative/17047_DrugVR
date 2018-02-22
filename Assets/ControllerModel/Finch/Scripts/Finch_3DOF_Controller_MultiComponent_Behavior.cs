using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using System;

public class Finch_3DOF_Controller_MultiComponent_Behavior : MonoBehaviour {
    private static string LOG_TAG = "Finch_3DOF_CTR_Behavior";

    public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_Controller_Right;
    public bool useSystemConfig = true;
    public GameObject TouchPad = null;
    public GameObject Touch_Effect = null;
    public GameObject Battery_Effect = null;
    public GameObject Trigger_Effect = null;
    public GameObject VolumeUp_Effect = null;
    public GameObject VolumeDown_Effect = null;
    public GameObject Grip_Effect = null;
    public GameObject Touch_Press = null;
    public GameObject BumperPress = null;
#pragma warning disable 0414
    private int batteryLevels = 5;
    public Texture[] textures = new Texture[5];
    [Range(0, 1.0f)]
    public float[] percents = new float[5];

    private Vector3 originPosition;
    private MeshRenderer batteryMeshRenderer = null;
    private bool getValidBattery = false;
    private Mesh toucheffectMesh = null;
    private Mesh touchpadMesh = null;
    private bool isTouchPressed = false;
    private Color materialColor = new Color (0, 179, 227, 255); // #00B3E3FF

    private Color StringToColor(string color_string)
    {
        float _color_r = (float)Convert.ToInt32 (color_string.Substring (1, 2), 16);
        float _color_g = (float)Convert.ToInt32 (color_string.Substring (3, 2), 16);
        float _color_b = (float)Convert.ToInt32 (color_string.Substring (5, 2), 16);
        float _color_a = (float)Convert.ToInt32 (color_string.Substring (7, 2), 16);

        return new Color (_color_r, _color_g, _color_b, _color_a);
    }

    private Texture2D GetTexture2D(string texture_path)
    {
        if (System.IO.File.Exists (texture_path))
        {
            var _bytes = System.IO.File.ReadAllBytes (texture_path);
            var _texture = new Texture2D (1, 1);
            _texture.LoadImage (_bytes);
            return _texture;
        }
        return null;
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col);
                tex.SetPixel(nx, ny, col);

            }
        }
        tex.Apply();
    }

    private void ReadJsonValues()
    {
        string json_values = WaveVR_Utils.OEMConfig.getControllerConfig ();
        if (!json_values.Equals (""))
        {
            SimpleJSON.JSONNode jsNodes = SimpleJSON.JSONNode.Parse (json_values);
            string node_value = "";

            node_value = jsNodes ["model"] ["touchpad_dot_use_texture"].Value;
            if (node_value.ToLower().Equals ("false"))
            {
                Log.d(LOG_TAG, "touchpad_dot_use_texture = false, create texture");
                if (Touch_Effect != null)
                {
                    // Material of touchpad dot.
                    node_value = jsNodes["model"]["touchpad_dot_color"].Value;
                    if (!node_value.Equals(""))
                    {
                        materialColor = StringToColor(node_value);

                        var texture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                        Color o = Color.clear;
                        o.r = 1f;
                        o.g = 1f;
                        o.b = 1f;
                        o.a = 0f;
                        for (int i = 0; i < 256; i++)
                        {
                            for (int j = 0; j < 256; j++)
                            {
                                texture.SetPixel(i, j, o);
                            }
                        }
                        texture.Apply();

                        Circle(texture, 128, 128, 100, materialColor);
                        MeshRenderer mshr = Touch_Effect.GetComponent<MeshRenderer>();
                        //  mshr.
                        mshr.material.mainTexture = texture;
                    }
                }
            } else
            {
                Log.d(LOG_TAG, "touchpad_dot_use_texture = true");
                node_value = jsNodes["model"]["touchpad_dot_texture_name"].Value;
                if (!node_value.Equals(""))
                {
                    if (System.IO.File.Exists(node_value))
                    {
                        var _bytes = System.IO.File.ReadAllBytes(node_value);
                        var _texture = new Texture2D(1, 1);
                        _texture.LoadImage(_bytes);

                        if (Touch_Effect != null)
                        {
                            MeshRenderer _mrdr = Touch_Effect.GetComponentInChildren<MeshRenderer>();
                            Material _mat = _mrdr.materials[0];
                            _mat.mainTexture = _texture;
                            _mat.color = materialColor;
                        }
                    }
                }
            }

            // Battery
            node_value = jsNodes ["battery"] ["battery_level_count"].Value;
            if (!node_value.Equals (""))
                batteryLevels = Convert.ToInt32 (node_value, 10);

            for (int i=0; i<5; i++)
            {
                node_value = jsNodes["battery"]["battery_levels"][i]["level_texture_name"].Value;
                if (!node_value.Equals(""))
                {
                    Texture _tex = GetTexture2D(node_value);
                    if (_tex != null)
                    {
                        textures[i] = _tex;
                        // texture has been changed and then change battery_level
                        node_value = jsNodes["battery"]["battery_levels"][i]["level_min_value"].Value;
                        if (!node_value.Equals(""))
                            percents[i] = float.Parse(node_value);
                    }
                }
            }

            Log.d (LOG_TAG,
                "percents[0]: " + percents [0] +
                ", percents[1]: " + percents [1] +
                "，percents[2]: " + percents [2] +
                ", percents[3]: " + percents [3] +
                ", percents[4]: " + percents [4]);
        }
    }

    void OnEnable()
    {
        if (useSystemConfig)
        {
            Log.d(LOG_TAG, "use system config in controller model!");
            ReadJsonValues();
        } else
        {
            Log.w(LOG_TAG, "use custom config in controller model!");
        }
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.BATTERY_STATUS_UPDATE, onBatteryStatusUpdate);
    }

    private void onBatteryStatusUpdate(params object[] args)
    {
        Log.d(LOG_TAG, "receive battery status update event");
        if (Battery_Effect != null)
        {
            getValidBattery = updateBatteryInfo();
        }
    }

    private bool updateBatteryInfo()
    {
        if (Application.isEditor)
            return false;

        float batteryPer = Interop.WVR_GetDeviceBatteryPercentage(device);
        Log.d(LOG_TAG, "BatteryPercentage device: " + device + ", percentage: " + batteryPer);
        if (batteryPer < 0)
        {
            Log.d(LOG_TAG, "device: " + device + " BatteryPercentage is negative, return false");
            return false;
        }

        if (batteryPer > percents[0])
        {
            batteryMeshRenderer.material.mainTexture = textures[0];
        }
        else if (batteryPer > percents[1])
        {
            batteryMeshRenderer.material.mainTexture = textures[1];
        }
        else if (batteryPer > percents[2])
        {
            batteryMeshRenderer.material.mainTexture = textures[2];
        }
        else if (batteryPer > percents[3])
        {
            batteryMeshRenderer.material.mainTexture = textures[3];
        }
        else
        {
            batteryMeshRenderer.material.mainTexture = textures[4];
        }
        Battery_Effect.SetActive(true);
        return true;
    }

    void Start ()
    {
        if (TouchPad != null && Touch_Effect != null)
        {
            originPosition = Touch_Effect.transform.localPosition;
            Touch_Effect.SetActive(false);
            toucheffectMesh = Touch_Effect.GetComponent<MeshFilter>().mesh;
            touchpadMesh = TouchPad.GetComponent<MeshFilter>().mesh;
        }
        if (Battery_Effect != null)
        {
            batteryMeshRenderer = Battery_Effect.GetComponent<MeshRenderer>();

            Battery_Effect.SetActive(false);
        }

        if (Grip_Effect != null)
        {
            Grip_Effect.SetActive(false);
        }

        if (Trigger_Effect != null)
        {
            Trigger_Effect.SetActive(false);
        }

        if (VolumeUp_Effect != null)
        {
            VolumeUp_Effect.SetActive(false);
        }

        if (VolumeDown_Effect != null)
        {
            VolumeDown_Effect.SetActive(false);
        }

        if (Touch_Press != null)
        {
            Touch_Press.SetActive(false);
        }

        if (BumperPress != null)
        {
            BumperPress.SetActive(false);
        }
    }

    int t = 0;

    // Update is called once per frame
    void Update () {
        if (Battery_Effect != null)
        {
            if (!getValidBattery)
            {
                if (t++ > 300)
                {
                    getValidBattery = updateBatteryInfo();

                    t = 0;
                }
            }
        }

        //WVR_InputId_Alias1_Trigger
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Trigger press down");
            if (Trigger_Effect != null)
            {
                Trigger_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            if (Trigger_Effect != null)
            {
                Trigger_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Trigger press up");
            if (Trigger_Effect != null)
            {
                Trigger_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Volume_Up
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Volume_Up))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Volume_Up press down");
            if (VolumeUp_Effect != null)
            {
                VolumeUp_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Volume_Up))
        {
            if (VolumeUp_Effect != null)
            {
                VolumeUp_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Volume_Up))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Volume_Up press up");
            if (VolumeUp_Effect != null)
            {
                VolumeUp_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Volume_Down
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Volume_Down))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Volume_Down press down");
            if (VolumeDown_Effect != null)
            {
                VolumeDown_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Volume_Down))
        {
            if (VolumeDown_Effect != null)
            {
                VolumeDown_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Volume_Down))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Volume_Down press up");
            if (VolumeDown_Effect != null)
            {
                VolumeDown_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Grip
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Grip press down");
            if (Grip_Effect != null)
            {
                Grip_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            if (Grip_Effect != null)
            {
                Grip_Effect.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Grip press up");
            if (Grip_Effect != null)
            {
                Grip_Effect.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Bumper
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Bumper))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Bumper press down");
            if (BumperPress != null)
            {
                BumperPress.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Bumper))
        {
            if (BumperPress != null)
            {
                BumperPress.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Bumper))
        {
            Log.d(LOG_TAG, "WVR_InputId_Alias1_Bumper press up");
            if (BumperPress != null)
            {
                BumperPress.SetActive(false);
            }
        }

        //WVR_InputId_Alias1_Touchpad
        if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            isTouchPressed = true;
            if (Touch_Press != null)
            {
                Touch_Press.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Press != null)
            {
                if (Touch_Effect != null)
                {
                    Touch_Effect.SetActive(false);
                }
                Touch_Press.SetActive(true);
            }
        }

        if (WaveVR_Controller.Input(device).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            isTouchPressed = false;
            if (Touch_Press != null)
            {
                Touch_Press.SetActive(false);
            }
        }
        // button touch down
        if (WaveVR_Controller.Input(device).GetTouchDown(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (!isTouchPressed && Touch_Effect != null)
            {
                Touch_Effect.SetActive(true);
            }
        }

        // button touch up
        if (WaveVR_Controller.Input(device).GetTouchUp(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (Touch_Effect != null)
            {
                Touch_Effect.SetActive(false);
            }
        }
 
        // button touched
        if (WaveVR_Controller.Input(device).GetTouch(WVR_InputId.WVR_InputId_Alias1_Touchpad))
        {
            if (!isTouchPressed)
            {
                if (Touch_Effect != null)
                {
                    Touch_Effect.SetActive(true);
                }

                if (TouchPad != null && Touch_Effect != null)
                {
                    var axis = WaveVR_Controller.Input(device).GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);

                    float xangle = axis.x * (touchpadMesh.bounds.size.x * TouchPad.transform.localScale.x - toucheffectMesh.bounds.size.x * Touch_Effect.transform.localScale.x) / 2;
                    float yangle = axis.y * (touchpadMesh.bounds.size.y * TouchPad.transform.localScale.y - toucheffectMesh.bounds.size.y * Touch_Effect.transform.localScale.y) / 2;

                    Log.d(LOG_TAG, "WVR_InputId_Alias1_Touchpad axis x: " + axis.x + ", xangle: " + xangle + " axis.y: " + axis.y + ", yangle: " + yangle);
#if DEBUG
                    Log.d(LOG_TAG, "Touch_EffectMesh.bounds.size.x: " + toucheffectMesh.bounds.size.x);
                    Log.d(LOG_TAG, "Touch_EffectMesh.bounds.size.y: " + toucheffectMesh.bounds.size.y);
                    Log.d(LOG_TAG, "Touch_EffectMesh. x scale: " + Touch_Effect.transform.localScale.x);
                    Log.d(LOG_TAG, "Touch_EffectMesh. y scale: " + Touch_Effect.transform.localScale.y);

                    Log.d(LOG_TAG, "touchpadMesh.bounds.size.x: " + touchpadMesh.bounds.size.x);
                    Log.d(LOG_TAG, "touchpadMesh.bounds.size.y: " + touchpadMesh.bounds.size.y);
                    Log.d(LOG_TAG, "touchpadMesh. x scale: " + TouchPad.transform.localScale.x);
                    Log.d(LOG_TAG, "touchpadMesh. y scale: " + TouchPad.transform.localScale.y);
#endif
                    var translateVec = new Vector3(xangle, yangle, 0);
                    Touch_Effect.transform.localPosition = originPosition + translateVec;
                }
            }
        }
    }
}
