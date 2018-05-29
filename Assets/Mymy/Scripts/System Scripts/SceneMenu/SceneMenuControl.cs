using DrugVR_Scribe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

// This class is essentially a copy of MenuClient
public class SceneMenuControl : MonoBehaviour,
    IShowAndHideSceneMenu
{
    public event Action OnShowMenu;
    public event Action OnHideMenu;

    [SerializeField]
    private List<UIFader> m_Faders;
    [SerializeField]
    private Image m_BackgroundImage;

    [SerializeField]
    private GridLayoutGroup m_SceneBtnGrid;
    private Transform m_SceneBtnGridTransform;
    [SerializeField]
    private GameObject m_SceneBtnPrefab;

    private MyControllerSwtich m_ControllerSwitch;
    private bool _ControllerVisible;
    private bool _BeamEnable;
    private bool _ReticleEnable;

    [SerializeField]
    private WVR_DeviceType m_DeviceToListen;
    [SerializeField]
    private WVR_InputId m_InputToListen;
    private WaveVR_Controller.Device m_WaveVrDevice;
    [SerializeField]
    private int m_EditorUseMouseButton;
    [SerializeField]
    private int m_NumOfClicksToShowMenu;
    [SerializeField]
    private float m_ConsecutiveClickTime = 0.3f;  //The max time allowed between double clicks
    private float m_LastMouseUpTime;  // The time when Fire1 was last released.
    private int m_NumOfConsecutiveClicks = 1;

    private Transform m_HeadTransform;
    private Transform m_ThisTransform;

    private bool m_IsInputPressUp;
    private bool m_IsMenuShown = false;


    /* MonoBehaviour */

    private void Start()
    {
        m_SceneBtnGridTransform = m_SceneBtnGrid.transform;

        // destroy all of m_SceneBtnGridTransform's children
        foreach (Transform child in m_SceneBtnGridTransform)
        {
            Destroy(child.gameObject);
        }

        m_BackgroundImage.enabled = false;

        m_WaveVrDevice = WaveVR_Controller.Input(m_DeviceToListen);
        m_HeadTransform = GameManager.Instance.HeadObject.transform;
        m_ThisTransform = transform;

        m_ControllerSwitch = GameManager.Instance.ControllerSwitch;
    }

    private void Update()
    {
        CheckInput();
    }

    /* end of MonoBehaviour */


    private void CheckInput()
    {
#if UNITY_EDITOR
        m_IsInputPressUp = m_IsInputPressUp = Input.GetMouseButtonUp(m_EditorUseMouseButton);
#else
        m_IsInputPressUp = m_WaveVrDevice.GetPressUp(m_InputToListen);
#endif

        // This if statement is to trigger events based on the information gathered before.
        if (m_IsInputPressUp)
        {
            // If the time between the last release of Fire1 and now is less
            // than the allowed double click time then it's a double click.
            if (Time.time - m_LastMouseUpTime < m_ConsecutiveClickTime)
            {
                m_NumOfConsecutiveClicks++;

                if (m_NumOfConsecutiveClicks == m_NumOfClicksToShowMenu)
                {
                    // If anything has subscribed to OnDoubleClick call it.
                    OnMultipleClicked();
                }                
            }
            else
            {
                m_NumOfConsecutiveClicks = 1;
            }

            // Record the time when Fire1 is released.
            m_LastMouseUpTime = Time.time;
        }
    }

    private void OnMultipleClicked()
    {
        if (!m_IsMenuShown)
        {
            SaveControllerState();
            m_ControllerSwitch.ShowController();
            ShowSceneMenu();
        }
        else
        {            
            HideSceneMenu();
            ResumeControllerState();
        }
    }

    private IEnumerator ShowSceneMenuRoutine()
    {
        m_Faders = new List<UIFader>();

        yield return CreateSceneBtnsAndAssignFaders();

        foreach (UIFader fader in m_Faders)
        {
            StartCoroutine(fader.InterruptAndFadeIn());
        }

        // wait till all faders are visible
        yield return new WaitUntil(() =>
        {            
            foreach (UIFader fader in m_Faders)
            {
                if (!fader.Visible)
                {
                    return false;
                }
            }
            return true;
        });

        m_BackgroundImage.enabled = true;
    }

    private IEnumerator HideSceneMenuRoutine()
    {
        m_BackgroundImage.enabled = false;

        foreach (UIFader fader in m_Faders)
        {
            StartCoroutine(fader.InterruptAndFadeOut());
        }

        // wait till all faders are invisible
        yield return new WaitUntil(() =>
        {
            foreach (UIFader fader in m_Faders)
            {
                if (fader.Visible)
                {
                    return false;
                }
            }
            return true;
        });

        StartCoroutine(DestroySceneBtnsAndDeassignFaders());
    }

    private IEnumerator CreateSceneBtnsAndAssignFaders()
    {       
        // assign m_SceneBtnGridTransform as parent of the SceneBtns created
        // so that their faders can be included in the List of UIFaders below
        foreach (DrugVR_SceneENUM scene in Scribe.SceneDictionary.Keys)
        {
            CreateSceneBtnAndAddToFaders(scene, m_SceneBtnGridTransform, m_SceneBtnGrid);            
            yield return null;
        }
    }

    private GameObject CreateSceneBtnAndAddToFaders(DrugVR_SceneENUM scene, 
        Transform parentTransform,
        GridLayoutGroup gridLayoutGroup)
    {
        GameObject newSceneBtn = Instantiate(m_SceneBtnPrefab, parentTransform);

        // TODO: The following assume some hierachical structure among the components
        newSceneBtn.GetComponent<GoToSceneOnSelected>().SceneToGoNext = scene;
        newSceneBtn.GetComponent<FadeSceneMenuOnSelected>().SceneMenuControl = this;
        newSceneBtn.GetComponent<MatchColliderToGrid>().Grid = gridLayoutGroup;
        newSceneBtn.GetComponentInChildren<Text>().text = scene.ToString();        
        m_Faders.Add(newSceneBtn.GetComponent<UIFader>());

        return newSceneBtn;
    }

    private IEnumerator DestroySceneBtnsAndDeassignFaders()
    {
        foreach (UIFader fader in m_Faders)
        {
            Destroy(fader.gameObject);
            yield return null;
        }

        m_Faders = null;
    }

    private void SaveControllerState()
    {
        _ControllerVisible = m_ControllerSwitch.IsControllerVisible;
        _BeamEnable = m_ControllerSwitch.LaserPointerRef.IsEnableBeam;
        _ReticleEnable = m_ControllerSwitch.LaserPointerRef.IsEnableReticle;
    }

    private void ResumeControllerState()
    {
        if (_ControllerVisible)
        {
            m_ControllerSwitch.ShowController(_BeamEnable, _ReticleEnable);
        }
        else
        {
            m_ControllerSwitch.HideController(_BeamEnable, _ReticleEnable);
        }

    }

    /* IShowAndHideSceneMenu interface */

    public void ShowSceneMenu()
    {
        m_IsMenuShown = true;

        // rotate the scene menu to face the user's head
        float signedAzimuthOfHeadForward = 0f;
        float zenithOfHeadForward = 0f;
        AngleCalculations.CalculateAzimuthAndZenithFromPointerDirection(m_HeadTransform.forward,
            Vector3.up, Vector3.forward, Vector3.zero, default(Color), default(Color),
            ref signedAzimuthOfHeadForward, ref zenithOfHeadForward);

        m_ThisTransform.localRotation = 
            Quaternion.Euler(0, -signedAzimuthOfHeadForward, 0);

        StartCoroutine(ShowSceneMenuRoutine());

        if (OnShowMenu != null)
        {
            OnShowMenu();
        }
    }

    public void HideSceneMenu()
    {
        m_IsMenuShown = false;
        StartCoroutine(HideSceneMenuRoutine());

        if (OnHideMenu != null)
        {
            OnHideMenu();
        }
    }

    /* end of IShowAndHideSceneMenu interface */
}
