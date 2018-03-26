using System;
using System.Collections;
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


    [SerializeField]
    private ReplyMode m_ReplyModeToBroadcast;
    [SerializeField]
    private Scrollbar m_ScrollBarToListen;


    /* MonoBahaviour */

    private void Start()
    {
        StartCoroutine(WaitForScrollBarFilledThenBroadcast());
    }

    /* end of MonoBahaviour */


    private IEnumerator WaitForScrollBarFilledThenBroadcast()
    {
        yield return new WaitUntil(() => m_ScrollBarToListen.value == 1);

		if (OnReplyModeIndicated != null)
        {
            OnReplyModeIndicated(m_ReplyModeToBroadcast);
        }
	}
}
