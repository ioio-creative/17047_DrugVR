using DrugVR_Scribe;
using System.Collections;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc02SClient : VideoSceneClientBase
{
    private const DrugVR_SceneENUM nextSceneToLoad = DrugVR_SceneENUM.Sc03;
    [SerializeField]
    private UIFader m_PopupMsgOnPhoneFader;
    [SerializeField]
    private ReplyModeBroadcast m_Reply;
    [SerializeField]
    private float m_SecondsToWaitBeforeTransitionToNext;


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
        nextSceneToLoadBase = nextSceneToLoad;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Reply.OnReplyModeIndicated += HandleReplyModeIndicated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_Reply.OnReplyModeIndicated -= HandleReplyModeIndicated;
    }    

    private void Start()
    {        
        StartCoroutine(m_PopupMsgOnPhoneFader.InterruptAndFadeIn());        
    }

    /* end of MonoBehaviour */


    /* event handlers */

    private void HandleReplyModeIndicated(ReplyModeBroadcast.ReplyMode replyMode)
    {
        StartCoroutine(m_PopupMsgOnPhoneFader.InterruptAndFadeOut());
        
        switch (replyMode)
        {
            case ReplyModeBroadcast.ReplyMode.Reply:
            default:
                Scribe.Side02 = true;
                StartCoroutine(TransitionToNextWhenIsReply());
                break;
            case ReplyModeBroadcast.ReplyMode.NotReply:
                Scribe.Side02 = false;
                StartCoroutine(TransitionToNextWhenIsNotReply());
                break;            
        }        
    }

    /* end of event handlers */


    private IEnumerator TransitionToNextWhenIsReply()
    {
        yield return new WaitForSeconds(m_SecondsToWaitBeforeTransitionToNext);
        managerInst.PlayVideo();
    }

    private IEnumerator TransitionToNextWhenIsNotReply()
    {
        yield return new WaitForSeconds(m_SecondsToWaitBeforeTransitionToNext);
        managerInst.GoToScene(nextSceneToLoadBase);
    }
}
