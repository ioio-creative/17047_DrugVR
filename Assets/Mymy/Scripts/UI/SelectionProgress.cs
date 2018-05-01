using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// This class is used to control the property of another 
// GameObject ISelectionProgressable, e.g. slider value, that fills 
// up as the user gazes at it and holds down the Fire1 button. 
// When it has finished filling, it triggers an event.  It also has a
// coroutine which returns once the bar is filled.
[RequireComponent(typeof(SelectionProgressable))]
public class SelectionProgress : VrButtonBase    
{
    // This event is triggered when the bar has filled.
    public event Action OnSelectionComplete;

    public float SelectionDuration { get { return m_SelectionDuration; } }


    // How long it takes for the bar to fill.
    [SerializeField]
    private float m_SelectionDuration = 2f;

    [SerializeField]
    private AudioClip m_OnFilledClip;

    // Reference to the GameObject ISelectionProgressable
    // whose fill amount is adjusted to display the bar.
    private SelectionProgressable m_Selection;        

    // Whether the selection should disappear instantly once it's been filled.
    [SerializeField]
    private bool m_DisappearOnSelectionFill;

    // Used to start and stop the filling coroutine based on input.
    private Coroutine m_SelectionFillRoutine;

    // Used to allow the coroutine to WAIT for the bar to fill.
    private bool m_SelectionFilled;    


    /* MonoBehaviour */

    protected override void Awake()
    {
        base.Awake();
        m_Selection = GetComponent<SelectionProgressable>();
    }

    private void Start()
    {
        // Setup m_Selection to have no fill at the start
        // and hide if necessary.
        m_Selection.SetValueToMin();
    }

    protected override void Update()
    {
        base.Update();
    }

    /* end of MonoBehaviour */


    /* audios */

    public void PlayOnFilledClip()
    {
        base.PlayAudioClip(m_OnFilledClip);
    }

    /* end of audios */


    public IEnumerator WaitForSelectionToFill()
    {
        // Check every frame if the selection is filled.
        return new WaitUntil(() => m_SelectionFilled);
    }

    private IEnumerator FillSelection()
    {
        // At the start of the coroutine, the bar is not filled.
        m_SelectionFilled = false;

        // Create a timer and reset the fill amount.
        float timer = 0f;
        m_Selection.SetValueToMin();

        // This loop is executed once per frame until the timer exceeds the duration.
        while (timer < m_SelectionDuration)
        {
            // The selection's fill amount requires a value from 0 to 1 so we normalise the time.
            m_Selection.SetValue(timer / m_SelectionDuration);

            // Increase the timer by the time between frames and wait for the next frame.
            timer += Time.deltaTime;

            // Wait until next frame.
            yield return null;

            // The following code is just to play safe
            // if the StopCoroutine() is somehow not called

            // If the user is still looking at the selection,
            // go on to the next iteration of the loop.
            if (base.m_GazeOver && base.m_ButtonPressed)
                continue;

            // If the user is no longer looking at the selection,
            // reset the selection and leave the function.
            ResetSelectionProgress();
            yield break;
        }

        // When the loop is finished set the fill amount to be full.
        m_Selection.SetValueToMax();

        // The selection is now filled so the coroutine waiting for it can continue.
        m_SelectionFilled = true;

        RaiseOnSelectedEvent();

        // Play the clip for when the selection is filled.        
        PlayOnFilledClip();

        // If the selection should disappear once it's filled, hide it.
        if (m_DisappearOnSelectionFill)
        {
            base.InteruptAndFadeOut();
        }
    }

    private void StartSelectionFillRoutine()
    {
        // start filling it.        
        m_SelectionFillRoutine =
            StartCoroutine(FillSelection());        
    }

    private void StopSelectionFillRoutine()
    {
        // stop filling it and reset its value.    
        if (m_SelectionFillRoutine != null)
        {
            StopCoroutine(m_SelectionFillRoutine);                
        }
        ResetSelectionProgress();        
    }
    
    private void ResetSelectionProgress()
    {
        m_SelectionFillRoutine = null;
        m_Selection.SetValueToMin();
    }

    private void RaiseOnSelectedEvent()
    {
        // If there is anything subscribed to OnSelectionComplete call it.
        if (OnSelectionComplete != null)
            OnSelectionComplete();
    }


    /* IHandleUiButton interfaces */

    public override void HandleDown()
    {
        base.HandleDown();
        StartSelectionFillRoutine();
    }

    public override void HandleEnter()
    {
        base.HandleEnter();              
    }

    public override void HandleExit()
    {
        base.HandleExit();
        StopSelectionFillRoutine();
    }

    public override void HandleUp()
    {
        base.HandleUp();        
        StopSelectionFillRoutine();
    }

    /* end of IHandleUiButton interfaces */
}
