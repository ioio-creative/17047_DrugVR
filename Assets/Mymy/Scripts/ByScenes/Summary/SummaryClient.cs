using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SummaryClient : VideoSceneClientBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.SkyVideoPlayer.isLooping = true;
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        // This scene won't go to next scene based on Video ends
    }
}
