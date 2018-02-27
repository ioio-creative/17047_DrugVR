using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This class works similarly to the SelectionRadial class except
// it has a physical manifestation in the scene.  This can be
// either a UI slider or a mesh with the SlidingUV shader.  The
// functions as a bar that fills up whilst the user looks at it
// and holds down the Fire1 button.
public class MySelectionSlider : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler
{
    public event Action OnBarFilled;


    [SerializeField] private float m_Duration;                          // The length of time it takes for the bar to fill.
    [SerializeField] private AudioSource m_Audio;                       // Reference to the audio source that will play effects when the user looks at it and when it fills.
    [SerializeField] private AudioClip m_OnOverClip;                    // The clip to play when the user looks at the bar.
    [SerializeField] public AudioClip m_OnFilledClip;                  // The clip to play when the bar finishes filling.
    [SerializeField] private Slider m_Slider;                           // Optional reference to the UI slider (unnecessary if using a standard Renderer).
    [SerializeField] private VRInteractiveItem m_InteractiveItem;       // Reference to the VRInteractiveItem to determine when to fill the bar.
    [SerializeField] public VRInput m_VRInput;                         // Reference to the VRInput to detect button presses.
    [SerializeField] private GameObject m_BarCanvas;                    // Optional reference to the GameObject that holds the slider (only necessary if DisappearOnBarFill is true).
    [SerializeField] private Renderer m_Renderer;                       // Optional reference to a renderer (unnecessary if using a UI slider).
    [SerializeField] private SelectionRadial m_SelectionRadial;         // Optional reference to the SelectionRadial, if non-null the duration of the SelectionRadial will be used instead.
    [SerializeField] private UIFader m_UIFader;                         // Optional reference to a UIFader, used if the SelectionSlider needs to fade out.
    [SerializeField] private Collider m_Collider;                       // Optional reference to the Collider used to detect the user's gaze, turned off when the UIFader is not visible.
    [SerializeField] private bool m_DisableOnBarFill;                   // Whether the bar should stop reacting once it's been filled (for single use bars).
    [SerializeField] private bool m_DisappearOnBarFill;                 // Whether the bar should disappear instantly once it's been filled.


    private bool m_BarFilled;                                           // Whether the bar is currently filled.
    private bool m_GazeOver;                                            // Whether the user is currently looking at the bar.
    private float m_Timer;                                              // Used to determine how much of the bar should be filled.
    private Coroutine m_FillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.


    private const string k_SliderMaterialPropertyName = "_SliderValue"; // The name of the property on the SlidingUV shader that needs to be changed in order for it to fill.


    /* MonoBehavious interface */

    private void Start()
    {
        
    }

    private void Update()
    {
		if (!m_UIFader)
        {
            return;
        }

        // If this bar is using a UIFader turn off the collider when it's invisible.
        m_Collider.enabled = m_UIFader.Visible;
    }

    /* end of MonoBehavious interface */


    /* controlling sliders */

    public IEnumerator WaitForBarToFill()
    {

    }

    private IEnumerator FillBar()
    {

    }



    /* end of controlling sliders */


    /* UnityEngine.EventSystems pointer event handler interface */

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter: MySelectionSlider");
        HandleOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit: MySelectionSlider");
        HandleOut();
    }

    /* end of UnityEngine.EventSystems pointer event handler interface */


    /* real pointer handlers */

    private void HandleDown()
    {

    }

    private void HandleUp()
    {

    }

    private void HandleOver()
    {

    }

    private void HandleOut()
    {

    }

    /* end of real pointer handlers */
}
