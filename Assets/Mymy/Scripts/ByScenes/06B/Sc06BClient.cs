using DrugVR_Scribe;

public class Sc06BClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc07;

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }
}
