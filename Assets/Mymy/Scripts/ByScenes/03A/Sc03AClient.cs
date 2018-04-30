using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc03AClient : MonoBehaviour
{
    //private DrugVR_SceneENUM nextSceneToLoad;
    private GameManager managerInst;
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

    private void GoToSceneOnChoice()
    {
        if (managerInst.Side01)
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc01A);
        }
        else
        {
            managerInst.GoToScene(DrugVR_SceneENUM.Sc01B);
        }
    }

    private void FadeOutSphere()
    {
        sphereFadeOutSwitch = true;
        m_sphereAnim.SetTrigger("FadeOutSphere");
    }

    private void SetSphereOpacity(float alpha)
    {
        introSphere.SetFloat("_Transparency", alpha);
    }


}
