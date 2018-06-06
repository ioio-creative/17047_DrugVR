using System;
using UnityEngine;
using UnityEngine.UI;

public class ReplyModeBroadcast : MonoBehaviour
{
    public enum ReplyMode
    {
        Reply,
        NotReply,
        Null
    }


    public event Action<ReplyMode> OnReplyModeIndicated;
    public bool IsReplyModeIndicated { get { return m_IsReplyModeIndicated; } }

    [SerializeField]
    private AudioSource m_Audio;
    [SerializeField]
    private AudioClip m_OnReplyClip;
    [SerializeField]
    private AudioClip m_OnIgnoreMsgClip;

    [SerializeField]
    private float m_XDisplacementThreshold;
    [SerializeField]
    private float m_YDisplacementThreshold;
    [SerializeField]
    private RectTransform m_RectTransformToListen;
    [SerializeField]
    private Image m_PopupMsgImage;
    private bool m_IsReplyModeIndicated = false;
    private Vector3 m_OriginalRectTransformPos;


    /* MonoBahaviour */

    private void Start()
    {
        m_OriginalRectTransformPos = m_RectTransformToListen.position;
    }

    private void LateUpdate()
    {
        //Debug.Log("m_RectTransformToListen.position.x: " + m_RectTransformToListen.position.x);
        //Debug.Log("m_RectTransformToListen.position.y: " + m_RectTransformToListen.position.y);

        ReplyMode replyMode = ReplyMode.Null;

        // set replyMode based on where m_RectTransformToListen is
        // "swiped" to
        if (m_RectTransformToListen.position.x > m_XDisplacementThreshold)
        {
            replyMode = ReplyMode.NotReply;
        }
        else if (m_RectTransformToListen.position.y > m_YDisplacementThreshold)
        {
            replyMode = ReplyMode.Reply;
        }

        // broadcast replyMode if m_RectTransformToListen is "swiped"
        // beyond certain thresholds
        if (replyMode != ReplyMode.Null && !m_IsReplyModeIndicated)
        {
            m_IsReplyModeIndicated = true;

            PlayOnReplyModeIndicatedClip(replyMode);

            // make m_PopupMsgImage transparent
            Color popupMsgImageOriginalColor = m_PopupMsgImage.color;
            popupMsgImageOriginalColor.a = 0;
            m_PopupMsgImage.color = popupMsgImageOriginalColor;            

            // TODO: Maybe we can play some sound here!

            if (OnReplyModeIndicated != null)
            {
                OnReplyModeIndicated(replyMode);
            }
        }
    }

    /* end of MonoBahaviour */


    /* audios */

    private void PlayOnReplyModeIndicatedClip(ReplyMode _replyMode)
    {
        switch (_replyMode)
        {
            case ReplyMode.Reply:
                PlayAudioClip(m_OnReplyClip);
                break;
            case ReplyMode.NotReply:
                PlayAudioClip(m_OnIgnoreMsgClip);
                break;
            default:
                break;
        }
    }

    private void PlayAudioClip(AudioClip aClip)
    {
        m_Audio.clip = aClip;
        if (m_Audio.clip != null)
        {
            m_Audio.Play();
        }
    }

    /* end of audios */


    public void Reset()
    {
        m_RectTransformToListen.position = m_OriginalRectTransformPos;
    }
}
