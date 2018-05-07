using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoButton : VrButtonBase
{

    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        PlayOnSelectedClip();
        Scribe.Side03 = false;
        ColorBlock buttonColors = m_Button.colors;
        buttonColors.highlightedColor = buttonColors.pressedColor;
        buttonColors.normalColor = buttonColors.pressedColor;
        m_Button.colors = buttonColors;
        Sc03AClient.GoToSceneOnChoice();
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
