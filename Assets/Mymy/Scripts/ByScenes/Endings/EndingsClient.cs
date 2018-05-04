using DrugVR_Scribe;
using UnityEngine;

public class EndingsClient : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Summary;

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
    }
}
