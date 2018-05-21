using System;

public class SelectionStandard : VrButtonBase
{
    // This event is triggered when the bar has filled.
    public event Action OnSelectionComplete;

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


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();

        // If there is anything subscribed to OnSelectionComplete call it.
        if (OnSelectionComplete != null)
        { 
            OnSelectionComplete();
        }

        if (DisappearOnSelection)
        {
            StartCoroutine(InterruptAndFadeOut());
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
