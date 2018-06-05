using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoButton : VrButtonBase
{
    [SerializeField]
    private Sc03AClient m_3AClientRef;

    protected override void Awake()
    {
        base.Awake();
        m_3AClientRef = m_3AClientRef ?? GetComponentInParent<Sc03AClient>();
    }

    private IEnumerator ResignMemoRoutine()
    {
        yield return PlayErrorClipAndWaitWhilePlaying();
        StartCoroutine(m_3AClientRef.ResignRoutine());
    }

    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();      
        Scribe.Side03 = false;
        StartCoroutine(ResignMemoRoutine());

        ColorBlock buttonColors = m_Button.colors;
        buttonColors.highlightedColor = buttonColors.pressedColor;
        buttonColors.normalColor = buttonColors.pressedColor;
        m_Button.colors = buttonColors;

        m_Button.GetComponentInChildren<UIImageAnimationControl>().SetColor(buttonColors.pressedColor);
        ForceDisableButton();
    }

    public override void HandleEnter()
    {
        base.HandleEnter();
    }

    public override void HandleExit()
    {
        base.HandleExit();
    }

    public override void HandleUp()
    {
        base.HandleUp();
    }

    /* end of IHandleUiButton interfaces */
}
