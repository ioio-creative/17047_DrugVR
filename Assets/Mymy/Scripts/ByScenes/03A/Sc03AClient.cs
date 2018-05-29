using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc03AClient : MonoBehaviour
{
    //private DrugVR_SceneENUM nextSceneToLoad;
    public static GameManager managerInst;
    private LaserPointer m_PickupLaser;

    [SerializeField]
    private Material introSphere;
    [SerializeField]
    private SelectionStandard m_StartGameButton;
    [SerializeField]
    private Animator m_InstructionAnim;   
    [SerializeField]
    private MemoButton m_MemoButton;
    
    [SerializeField]
    private Animator m_SphereAnim;
    [SerializeField]
    private Texture2D m_SphereTexture;
    [SerializeField]
    [Range(0,1.0f)]
    private float sphereAlpha = 1.0f;
    private bool sphereFadeOutSwitch = false;

    private UIFader m_InstructionAnimFader;
    private UIFader m_MemoUIFader;

    private void Awake()
    {
        managerInst = GameManager.Instance;
        m_PickupLaser = FindObjectOfType<LaserPointer>();

        if (m_MemoButton == null)
        {
            m_MemoButton = GetComponentInChildren<MemoButton>();
        }
        if (m_SphereAnim == null)
        {
            m_SphereAnim = GetComponent<Animator>();
        }
    }

    private void OnDisable()
    {
        m_StartGameButton.OnSelectionComplete -= HandleStartGameButtonSelectionComplete;
        m_PickupLaser.UnforceDisableGrab();
    }

    private void Start()
    {
        m_InstructionAnimFader = m_InstructionAnim.GetComponent<UIFader>();

        if (introSphere == null)
        {
            introSphere = GetComponentInChildren<MeshRenderer>().material;
        }
        
        if (m_SphereTexture != null)
        {
            introSphere.SetTexture("_MainTex", m_SphereTexture);
        }       
        SetSphereOpacity(1.0f);

        m_PickupLaser.ForceDisableGrab();
    }

    private void FixedUpdate()
    {
        
        if (sphereAlpha <= 0 && sphereFadeOutSwitch)
        {
            sphereFadeOutSwitch = false;
            m_SphereAnim.ResetTrigger("FadeOutSphere");
            m_InstructionAnim.SetTrigger("Play");
        }
    }

    private void Update()
    {
        if (sphereFadeOutSwitch) SetSphereOpacity(sphereAlpha);
    }

    public static void GoToSceneOnChoice()
    {
        if (Scribe.Side03)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc04);
        }
        else
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc03B);
        }
    }

    public void FadeOutSphere()
    {
        sphereFadeOutSwitch = true;
        m_SphereAnim.SetTrigger("FadeOutSphere");
        m_StartGameButton.OnSelectionComplete += HandleStartGameButtonSelectionComplete;
        StartCoroutine(m_StartGameButton.InterruptAndFadeIn());       
    }

    private void SetSphereOpacity(float alpha)
    {
        introSphere.SetFloat("_Transparency", alpha);
    }

    private void HandleStartGameButtonSelectionComplete()
    {
        m_InstructionAnim.SetTrigger("Stop");
        StartCoroutine(m_InstructionAnimFader.InterruptAndFadeOut());
        StartCoroutine(m_StartGameButton.InterruptAndFadeOut());
        m_StartGameButton.OnSelectionComplete -= HandleStartGameButtonSelectionComplete;
        StartCoroutine(m_MemoButton.InterruptAndFadeIn());
        m_PickupLaser.UnforceDisableGrab();
    }
}
