using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ArenaTest))]
public class ArenaTestEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as ArenaTest;

        myScript.Shape = (WVR_ArenaShape)EditorGUILayout.EnumPopup ("Arena Shape", myScript.Shape);
        if (myScript.Shape == WVR_ArenaShape.WVR_ArenaShape_Rectangle)
        {
            myScript.ArenaLength = (float)EditorGUILayout.Slider ("Length of Arena", myScript.ArenaLength, 0.5f, 2.0f);
            myScript.ArenaWidth = (float)EditorGUILayout.Slider ("Width of Arena", myScript.ArenaWidth, 0.5f, 2.0f);
        }
        if (myScript.Shape == WVR_ArenaShape.WVR_ArenaShape_Round)
        {
            myScript.Diameter = (float)EditorGUILayout.Slider ("Diameter of Arena", myScript.Diameter, 1.0f, 4.0f);
        }
        myScript.Text_Result = (Text)EditorGUILayout.ObjectField ("Text result", myScript.Text_Result, typeof(Text), true);
    }
}
#endif

public class ArenaTest : MonoBehaviour
{
    private const string LOG_TAG = "ArenaTest";

    public WVR_ArenaShape Shape;
    [Tooltip("Length of rectangle arena (meter)")]
    [Range(0.5f, 2.0f)]
    public float ArenaLength = 0.5f;
    [Tooltip("Width of rectangle arena (meter)")]
    [Range(0.5f, 2.0f)]
    public float ArenaWidth = 0.5f;
    [Tooltip("Diameter of round arena (meter)")]
    [Range(1.0f, 4.0f)]
    public float Diameter = 1.0f;

    public Text Text_Result; 

    private WVR_Arena_t arena;
    private WVR_ArenaArea_t arena_area;

	// Use this for initialization
	void Start ()
    {
        arena.shape = Shape;
        switch (Shape)
        {
        case WVR_ArenaShape.WVR_ArenaShape_Rectangle:
            arena_area.rectangle.length = ArenaLength;
            arena_area.rectangle.width = ArenaWidth;
            break;
        case WVR_ArenaShape.WVR_ArenaShape_Round:
            arena_area.round.diameter = Diameter;
            break;
        default:
            break;
        }
        arena.area = arena_area;

        if (WaveVR.Instance != null)
        {
            bool _ret = Interop.WVR_SetArena (ref arena);
            if (!_ret)
                Log.e (LOG_TAG, "WVR_SetArena failed.");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool _overrange = false;
        WVR_ArenaVisible _visible = WVR_ArenaVisible.WVR_ArenaVisible_Auto;

        if (WaveVR.Instance != null)
        {
            arena = Interop.WVR_GetArena ();
            _overrange = Interop.WVR_IsOverArenaRange ();
            _visible = Interop.WVR_GetArenaVisible ();

            bool _touchpad_pressed = 
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Left).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Right).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Touchpad);
            if (_touchpad_pressed)
            {
                Interop.WVR_SetArenaVisible (WVR_ArenaVisible.WVR_ArenaVisible_ForceOn);
            }

            bool _menu_pressed =
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Left).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Menu) ||
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Right).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Menu);
            if (_menu_pressed)
            {
                Interop.WVR_SetArenaVisible (WVR_ArenaVisible.WVR_ArenaVisible_ForceOff);
            }

            bool _triggered =
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Left).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Trigger) ||
                WaveVR_Controller.Input (WVR_DeviceType.WVR_DeviceType_Controller_Right).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Trigger);
            if (_triggered)
            {
                Interop.WVR_SetArenaVisible (WVR_ArenaVisible.WVR_ArenaVisible_Auto);
            }
        }

        string _content =
            "arena shape: " + arena.shape +
            "\narena length: " + arena.area.rectangle.length +
            "\narena width: " + arena.area.rectangle.width +
            "\narena diameter: " + arena.area.round.diameter +
            "\nover range: " + (_overrange ? "yes" : "no") +
            "\nvisible: " + _visible;

        Text_Result.text = _content;
	}
}
