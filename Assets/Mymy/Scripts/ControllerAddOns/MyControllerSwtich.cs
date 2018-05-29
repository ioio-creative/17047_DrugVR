using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaserPointer))]
public class MyControllerSwtich : MonoBehaviour
{
    public LaserPointer LaserPointerRef;
    [SerializeField]
    private GameObject m_OriginalControllerModel;

    private Transform m_ControllerTransform;
    public Transform ControllerTransform
    {
        get { return m_ControllerTransform; }
        private set { m_ControllerTransform = value; }
    }
    
    private Vector3 m_OriginalControllerModelInitialScale;

    private bool m_IsControllerVisible = true;
    public bool IsControllerVisible
    {
        get { return m_IsControllerVisible; }
    }

    
    private void Awake()
    {
        LaserPointerRef = GetComponent<LaserPointer>();

        if (m_OriginalControllerModel != null)
        {
            ControllerTransform = m_OriginalControllerModel.transform;
            m_OriginalControllerModelInitialScale = ControllerTransform.localScale;
        }
    }

    /* controller main switch*/
    public void ShowController()
    {
        ShowController(true, true);
    }

    public void ShowController(bool beam, bool reticle)
    {
        m_IsControllerVisible = true;
        LaserPointerRef.IsEnableBeam = beam;
        LaserPointerRef.IsEnableReticle = reticle;
        //We DO NOT SetActive() to enable model game object because WVR script will ensure enabling during startup
        ControllerTransform.localScale = m_OriginalControllerModelInitialScale;
    }

    public void HideController()
    {
        HideController(false, false);
    }

    public void HideController(bool beam, bool reticle)
    {
        m_IsControllerVisible = false;
        LaserPointerRef.IsEnableBeam = beam;
        LaserPointerRef.IsEnableReticle = reticle;
        //We DO NOT SetActive() to enable model game object because WVR script will ensure enabling during startup
        ControllerTransform.localScale = Vector3.zero;
    }
    /* end of controller main switch*/
}
