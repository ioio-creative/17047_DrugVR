using System;
using System.Collections;
using UnityEngine;

// This class is used to control the property of another 
// GameObject ISelectionProgressable, e.g. slider value, that fills 
// up as the user gazes at it and holds down the Fire1 button. 
// When it has finished filling, it triggers an event.  It also has a
// coroutine which returns once the bar is filled.
[RequireComponent(typeof(SelectionProgressable))]
[RequireComponent(typeof(AudioSource))]
public class SelectionProgress : MonoBehaviour,
    IHandleUiButton
{
    // This event is triggered when the bar has filled.
    public event Action OnSelectionComplete;

    public float SelectionDuration { get { return m_SelectionDuration; } }


    // How long it takes for the bar to fill.
    [SerializeField]
    private float m_SelectionDuration = 2f;

    // Whether or not the bar should be visible at the start.
    [SerializeField]
    private bool m_HideOnStart = true;    

    // Reference to the GameObject ISelectionProgressable
    // whose fill amount is adjusted to display the bar.
    private SelectionProgressable m_Selection;

    // Reference to the audio source that will play effects
    // when the user looks at it and when it fills.
    private AudioSource m_Audio;

    // The clip to play when the user looks at the bar.
    [SerializeField]
    private AudioClip m_OnOverClip;

    // The clip to play when the bar finishes filling.
    [SerializeField]
    private AudioClip m_OnFilledClip;

    // Required reference to the GameObject that holds the selection UI
    // (only necessary if DisappearOnSelectionFill is true).
    [SerializeField]    
    private GameObject m_SelectionCanvas;

    // Whether the selection should disappear instantly once it's been filled.
    [SerializeField]
    private bool m_DisappearOnSelectionFill;

    // Used to start and stop the filling coroutine based on input.
    private Coroutine m_SelectionFillRoutine;

    // Whether or not the bar is currently useable.
    private bool m_IsSelectionActive;       

    // Used to allow the coroutine to WAIT for the bar to fill.
    private bool m_SelectionFilled;

    // Whether input pointer is over
    private bool m_GazeOver;

    // Whether input button is pressed
    private bool m_ButtonPressed;


    /* MonoBehavior */

    private void Awake()
    {
        m_Selection = GetComponent<SelectionProgressable>();
        m_Audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Setup m_Selection to have no fill at the start
        // and hide if necessary.
        m_Selection.SetValueToMin();

        if (m_HideOnStart)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    /* end of MonoBehavior */


    public void Show()
    {
        m_SelectionCanvas.SetActive(true);
        m_IsSelectionActive = true;
    }

    public void Hide()
    {
        m_SelectionCanvas.SetActive(false);
        m_IsSelectionActive = false;

        // This effectively resets m_Selection for
        // when it's shown again.
        m_Selection.SetValueToMin();
    }

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
            if (m_GazeOver && m_ButtonPressed)
                continue;

            // If the user is no longer looking at the selection,
            // reset the selection and leave the function.
            m_Selection.SetValueToMin();
            yield break;
        }

        // When the loop is finished set the fill amount to be full.
        m_Selection.SetValueToMax();

        // Turn off the selection so it can only be used once.
        m_IsSelectionActive = false;

        // The selection is now filled so the coroutine waiting for it can continue.
        m_SelectionFilled = true;

        // If there is anything subscribed to OnSelectionComplete call it.
        if (OnSelectionComplete != null)
            OnSelectionComplete();

        // Play the clip for when the selection is filled.        
        PlayOnFilledClip();

        // If the selection should disappear once it's filled, hide it.
        if (m_DisappearOnSelectionFill)
        {
            Hide();
        }
    }

    private void PlayOnOverClip()
    {
        m_Audio.clip = m_OnOverClip;
        m_Audio.Play();
    } 

    private void PlayOnFilledClip()
    {
        m_Audio.clip = m_OnFilledClip;
        m_Audio.Play();
    }

    private void StartSelectionFillRoutineIfActive()
    {
        // If the selection is active start filling it.
        if (m_IsSelectionActive)
        {
            m_SelectionFillRoutine =
                StartCoroutine(FillSelection());
        }
    }

    private void StopSelectionFillRoutineIfActive()
    {
        // If the selection is active,
        // stop filling it and reset its value.
        if (m_IsSelectionActive)
        {
            if (m_SelectionFillRoutine != null)
            {
                StopCoroutine(m_SelectionFillRoutine);                
            }
            m_SelectionFillRoutine = null;
            m_Selection.SetValueToMin();
        }
    }    


    /* IHandleUiButton interfaces */

    public void HandleDown()
    {
        Debug.Log("HandleDown: SelectionProgress");
        m_ButtonPressed = true;
        StartSelectionFillRoutineIfActive();
    }

    public void HandleEnter()
    {
        Debug.Log("HandleEnter: SelectionProgress");
        m_GazeOver = true;
        if (m_IsSelectionActive)
        {
            // Play the clip appropriate when the user
            // starts looking at the selection.
            PlayOnOverClip();
        }
    }

    public void HandleExit()
    {
        Debug.Log("HandleExit: SelectionProgress");
        m_GazeOver = false;
        StopSelectionFillRoutineIfActive();
    }

    public void HandleUp()
    {
        Debug.Log("HandleUp: SelectionProgress");
        m_ButtonPressed = false;
        StopSelectionFillRoutineIfActive();
    }

    /* end of IHandleUiButton interfaces */
}
