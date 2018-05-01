using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc03AClient : MonoBehaviour
{
    //private DrugVR_SceneENUM nextSceneToLoad;
    public static GameManager managerInst;
    [SerializeField]
    private Material introSphere;
    [SerializeField]
    private Animator m_sphereAnim;
    [SerializeField]
    [Range(0,1.0f)]
    private float sphereAlpha = 1.0f;
    private bool sphereFadeOutSwitch = false;

    private void Awake()
    {
        managerInst = GameManager.Instance;
    }

    private void Start()
    {
        if (introSphere == null)
        {
            introSphere = GetComponentInChildren<MeshRenderer>().material;         
        }
        SetSphereOpacity(1.0f);
        //FadeOutSphere();
    }

    private void FixedUpdate()
    {
        if (sphereFadeOutSwitch) SetSphereOpacity(sphereAlpha);

        if (sphereAlpha <= 0)
        {
            sphereFadeOutSwitch = false;
            m_sphereAnim.ResetTrigger("FadeOutSphere");
        }
    }

    public static void GoToSceneOnChoice()
    {
        if (managerInst.Side03)
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
    }

    private void SetSphereOpacity(float alpha)
    {
        introSphere.SetFloat("_Transparency", alpha);
    }


}
