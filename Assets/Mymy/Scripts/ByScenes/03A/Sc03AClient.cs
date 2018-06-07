using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc03AClient : MonoBehaviour
{
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide03IsTrue = DrugVR_SceneENUM.Sc04;
    private const DrugVR_SceneENUM nextSceneToLoadWhenSide03IsFalse = DrugVR_SceneENUM.Sc03B;

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
    private BasinTrigger m_BasinTrigger;
    
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

    [SerializeField]
    private AudioSource m_ClientAudioSource;
    [SerializeField]
    private AudioClip m_ChefVOClip;
    [SerializeField]
    private AudioClip m_ResignClip;

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

    private void OnEnable()
    {
        m_BasinTrigger.OnProgressTrigger += HandleBasinTriggerProgress;
    }

    private void OnDisable()
    {
        m_BasinTrigger.OnProgressTrigger -= HandleBasinTriggerProgress;
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

    public IEnumerator ResignRoutine()
    {
        yield return PlayResignClipAndWaitWhilePlaying();
        GoToSceneOnChoice();
    }

    public static void GoToSceneOnChoice()
    {
        if (Scribe.Side03)
        {
            managerInst.GoToScene(nextSceneToLoadWhenSide03IsTrue);
        }
        else
        {
            managerInst.GoToScene(nextSceneToLoadWhenSide03IsFalse);
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

    public void PlayResignClip()
    {
        PlayAudioClip(m_ResignClip);
    }

    public void PlayChefClip()
    {
        PlayAudioClip(m_ChefVOClip);
    }

    private void PlayAudioClip(AudioClip aClip)
    {
        m_ClientAudioSource.clip = aClip;
        if (m_ClientAudioSource.clip != null)
        {
            m_ClientAudioSource.Play();
        }
    }

    private IEnumerator PlayResignClipAndWaitWhilePlaying()
    {
        yield return PlayAudioClipAndWaitWhilePlaying(m_ResignClip);
    }

    private IEnumerator PlayChefClipAndWaitWhilePlaying()
    {
        yield return PlayAudioClipAndWaitWhilePlaying(m_ChefVOClip);
    }

    private IEnumerator PlayAudioClipAndWaitWhilePlaying(AudioClip aClip)
    {
        PlayAudioClip(aClip);
        yield return WaitWhileAudioIsPlaying();
    }

    private IEnumerator WaitWhileAudioIsPlaying()
    {
        yield return new WaitWhile(() => m_ClientAudioSource.isPlaying);
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

    private void HandleBasinTriggerProgress()
    {
        PlayChefClip();
    }
}
