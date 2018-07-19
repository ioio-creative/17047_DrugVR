using DrugVR_Scribe;
using UnityEngine;

public class EndingsClient : VideoSceneClientBase
{    
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Summary;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Start()
    {
        nextSceneToLoadBase = nextSceneToLoad;

        //This is an alt. ver requested by FYG to have Sc08 appears at the end 
        //of Sc09 or 10, regardless of Player's ending
        if (GameManager.Instance.CurrentScene != DrugVR_SceneENUM.Sc08)
        {
            nextSceneToLoadBase = DrugVR_SceneENUM.Sc08;
        }
    }
}
