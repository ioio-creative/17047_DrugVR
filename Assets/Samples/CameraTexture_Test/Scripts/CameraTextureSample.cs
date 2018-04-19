using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WaveVR_Log;

public class CameraTextureSample : MonoBehaviour {
    private bool started = false;
    Texture2D nativeTexture = null;
    private static string LOG_TAG = "CameraTextureSample";
    System.IntPtr textureid ;
    private MeshRenderer meshrenderer;
    private bool updated = true;
    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return ;
#endif
        startCamera();
    }

    private void startCamera()
    {
        WaveVR_CameraTexture.UpdateCameraCompletedDelegate += updateTextureCompleted;
        WaveVR_CameraTexture.StartCameraCompletedDelegate += onStartCameraCompleted;

        started = WaveVR_CameraTexture.instance.startCamera();
        nativeTexture = new Texture2D(1280, 400);
        textureid = nativeTexture.GetNativeTexturePtr();
        meshrenderer = GetComponent<MeshRenderer>();
        meshrenderer.material.mainTexture = nativeTexture;
    }

    void updateTextureCompleted(uint textureId)
    {
        Log.d(LOG_TAG, "updateTextureCompleted, textureid = " + textureId);

        meshrenderer.material.mainTexture = nativeTexture;
        updated = true;
    }

    void onStartCameraCompleted(bool result)
    {
        Log.d(LOG_TAG, "onStartCameraCompleted, result = " + result);
        started = result;
    }

    void OnApplicationPause(bool pauseStatus)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
#endif
        if (!pauseStatus)
        {
            if (started)
            {
                Log.d(LOG_TAG, "Pause(" + pauseStatus + ") and auto start camera");
                startCamera();
            }
        }
    }

    // Update is called once per frame
    void Update () {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
#endif
        if (started && updated)
        {
            updated = false;
            WaveVR_CameraTexture.instance.updateTexture((uint)textureid);
        }
    }
}
