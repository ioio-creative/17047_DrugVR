using UnityEngine;
using UnityEngine.Video;

public class SummaryClient : VideoSceneClientBase
{
    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        managerInst.SkyVideoPlayer.isLooping = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();        
    }
    
    /* end of MonoBehaviour */


    /* event handlers */

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        // This scene won't go to next scene based on Video ends
    }    

    /* end of event handlers */
}
