using DrugVR_Scribe;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class Sc07BClient : VideoSceneClientBase
{
    private DrugVR_SceneENUM endSceneToLoad;

    [SerializeField]
    private AudioSource m_AudioSrc;
    [SerializeField]
    private AudioClip m_PoliceGroupClip;

    protected override void Awake()
    {
        base.Awake();       
    }

    protected override void HandleSystemVideoEnd(VideoPlayer source)
    {
        //Override parent HandleVideoEnd
        StartCoroutine(MethPartyEndRoutine());
    }

    public void GoToEndSceneOnChoice()
    {
        endSceneToLoad = Scribe.EndingSceneENUM();
        managerInst.GoToScene(endSceneToLoad, 3f);
    }

    private IEnumerator MethPartyEndRoutine()
    {
        m_AudioSrc.clip = m_PoliceGroupClip;
        yield return StartCoroutine(GameManager.Instance.FadeOutToBlackRoutine());
        m_AudioSrc.Play();
        yield return new WaitWhile(() => m_AudioSrc.isPlaying);
        GoToEndSceneOnChoice();
    }
}