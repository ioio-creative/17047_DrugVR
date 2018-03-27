using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class Scene2SControl : MonoBehaviour
{
    [SerializeField]
    private ReplyModeBroadcast m_Reply;
    [SerializeField]
    private ReplyModeBroadcast m_NotReply;
    [SerializeField]
    private UIFader m_PopupMsgOnPhoneFader;
    [SerializeField]
    private UIFader m_MessengerOnPhoneFader;
    [SerializeField]
    private UIFader m_CoverOnPhoneFader;


    /* MonoBehaviour */

    private void OnEnable()
    {
        m_Reply.OnReplyModeIndicated += HandleReplyModeIndicated;
        m_NotReply.OnReplyModeIndicated += HandleReplyModeIndicated;
    }

    private void OnDisable()
    {
        m_Reply.OnReplyModeIndicated -= HandleReplyModeIndicated;
        m_NotReply.OnReplyModeIndicated -= HandleReplyModeIndicated;
    }

    private void Start()
    {
        StartCoroutine(m_PopupMsgOnPhoneFader.InteruptAndFadeIn());
    }    

    /* end of MonoBehaviour */


    private void HandleReplyModeIndicated(ReplyModeBroadcast.ReplyMode replyMode)
    {
        switch (replyMode)
        {
            case ReplyModeBroadcast.ReplyMode.NotReply:
                StartCoroutine(m_CoverOnPhoneFader.InteruptAndFadeIn());
                break;
            case ReplyModeBroadcast.ReplyMode.Reply:
            default:
                StartCoroutine(m_MessengerOnPhoneFader.InteruptAndFadeIn());
                break;
        }
    }
}
