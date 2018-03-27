using System;
using UnityEngine;
using UnityEngine.UI;

public class ReplyModeBroadcast : MonoBehaviour
{
    public enum ReplyMode
    {
        Reply,
        NotReply
    }
    

    public event Action<ReplyMode> OnReplyModeIndicated;
    public bool IsReplyModeIndicated { get { return m_IsReplyModeIndicated; } }


    [SerializeField]
    private ReplyMode m_ReplyModeToBroadcast;
    [SerializeField]
    private Scrollbar m_ScrollBarToListen;
    private bool m_IsReplyModeIndicated = false;


    /* MonoBahaviour */

    /* end of MonoBahaviour */


    public void HandleScrollBarValueChanged()
    {
        if (m_ScrollBarToListen.value == 1)
        {
            m_IsReplyModeIndicated = true;

            if (OnReplyModeIndicated != null)
            {
                OnReplyModeIndicated(m_ReplyModeToBroadcast);
            }
        }
    }
}
