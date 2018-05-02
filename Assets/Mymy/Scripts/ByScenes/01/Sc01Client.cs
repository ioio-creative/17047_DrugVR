using DrugVR_Scribe;
using UnityEngine;

public class Sc01Client : VideoSceneClientBase
{
    [SerializeField]
    DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc01S;

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
