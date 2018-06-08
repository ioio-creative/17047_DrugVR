using System;
using System.Collections;
using UnityEngine;

public class SelectionStandard : VrButtonBase
{
    // This event is triggered when the bar has filled.
    public event Action OnSelectionDown;
    public event Action OnSelectionDisappear;

    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
    }

    /* end of MonoBehaviour */


    /* audios */

    /* end of audios */

    private IEnumerator WaitForButtonFadeOutAndRaiseEvent()
    {
        yield return StartCoroutine(InterruptAndFadeOut());
        if (OnSelectionDown != null)
        {
            OnSelectionDown();
        }
    }

    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();

        PlayOnSelectedClip();
        if (OnSelectionDown != null)
        {
            OnSelectionDown();
        }
        if (DisappearOnSelection)
        {
            StartCoroutine(WaitForButtonFadeOutAndRaiseEvent());
        }
        else
        {
            if (OnSelectionDisappear != null)
            {
                OnSelectionDisappear();
            }
        }
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
