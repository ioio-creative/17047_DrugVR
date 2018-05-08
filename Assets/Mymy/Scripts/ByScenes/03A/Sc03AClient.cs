using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc03AClient : MonoBehaviour
{
    //private DrugVR_SceneENUM nextSceneToLoad;
    public static GameManager managerInst;
    [SerializeField]
    private MemoButton m_MemoButton;
    [SerializeField]
    private Material introSphere;
    [SerializeField]
    private Animator m_sphereAnim;
    [SerializeField]
    [Range(0,1.0f)]
    private float sphereAlpha = 1.0f;
    private bool sphereFadeOutSwitch = false;

    private UIFader m_MemoUIFader;

    private void Awake()
    {
        managerInst = GameManager.Instance;
        if (m_MemoButton == null)
        {
            m_MemoButton = GetComponentInChildren<MemoButton>();
        }
    }

    private void Start()
    {
        if (introSphere == null)
        {
            introSphere = GetComponentInChildren<MeshRenderer>().material;         
        }
        SetSphereOpacity(1.0f);

        if (m_MemoUIFader == null)
        {
            m_MemoUIFader = m_MemoButton.gameObject.GetComponent<UIFader>();
        }
    }

    private void FixedUpdate()
    {
        
        if (sphereAlpha <= 0)
        {
            sphereFadeOutSwitch = false;
            m_sphereAnim.ResetTrigger("FadeOutSphere");
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
        m_sphereAnim.SetTrigger("FadeOutSphere");
        StartCoroutine(m_MemoUIFader.InteruptAndFadeIn());
    }

    private void SetSphereOpacity(float alpha)
    {
        introSphere.SetFloat("_Transparency", alpha);
    }
}
