using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Scene2SControlNew : MonoBehaviour
{
    [SerializeField]
    private UIFader m_ReplyInstructionFader;
    [SerializeField]
    private UIFader m_PopupMsgOnPhoneFader;
    [SerializeField]
    private UIFader m_MessengerOnPhoneFader;
    [SerializeField]
    private UIFader m_CoverOnPhoneFader;
    [SerializeField]
    private ReplyModeBroadcastNew m_Reply;


    /* MonoBehaviour */

    private void OnEnable()
    {
        m_Reply.OnReplyModeIndicated += HandleReplyModeIndicated;
    }

    private void OnDisable()
    {
        m_Reply.OnReplyModeIndicated -= HandleReplyModeIndicated;
    }

    private void Start()
    {
        StartCoroutine(m_PopupMsgOnPhoneFader.InteruptAndFadeIn());
        StartCoroutine(m_ReplyInstructionFader.InteruptAndFadeIn());
    }

    /* end of MonoBehaviour */


    private void HandleReplyModeIndicated(ReplyModeBroadcastNew.ReplyMode replyMode)
    {
        StartCoroutine(m_ReplyInstructionFader.InteruptAndFadeOut());

        switch (replyMode)
        {
            case ReplyModeBroadcastNew.ReplyMode.NotReply:
                StartCoroutine(m_CoverOnPhoneFader.InteruptAndFadeIn());
                break;
            case ReplyModeBroadcastNew.ReplyMode.Reply:
            default:
                StartCoroutine(m_MessengerOnPhoneFader.InteruptAndFadeIn());
                break;
        }
    }

    public void Reset()
    {
        StartCoroutine(m_ReplyInstructionFader.InteruptAndFadeIn());
        StartCoroutine(m_PopupMsgOnPhoneFader.InteruptAndFadeIn());
        StartCoroutine(m_MessengerOnPhoneFader.InteruptAndFadeOut());
        StartCoroutine(m_CoverOnPhoneFader.InteruptAndFadeOut());

        m_Reply.Reset();
    }
}
