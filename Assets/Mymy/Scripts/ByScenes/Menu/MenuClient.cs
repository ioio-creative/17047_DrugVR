using DrugVR_Scribe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class MenuClient : MonoBehaviour,
    IShowAndHideSceneMenu
{
    public event Action OnShowMenu;
    public event Action OnHideMenu;

    [SerializeField]
    private GameObject[] m_FaderContainers;
    private UIFader[] m_Faders;

    [SerializeField]
    private GridLayoutGroup m_SceneBtnGrid;
    private Transform m_SceneBtnGridTransform;
    [SerializeField]
    private GameObject m_SceneBtnPrefab;

    


    /* MonoBehaviour */

    private void Awake()
    {
        m_SceneBtnGridTransform = m_SceneBtnGrid.transform;

        // destroy all of m_SceneBtnGridTransform's children
        foreach (Transform child in m_SceneBtnGridTransform)
        {
            Destroy(child.gameObject);
        }

        // assign m_SceneBtnGridTransform as parent of the SceneBtns created
        // so that their faders can be included in the List of UIFaders below
        foreach (DrugVR_SceneENUM scene in Scribe.SceneDictionary.Keys)
        {
            CreateSceneBtn(scene, m_SceneBtnGridTransform, m_SceneBtnGrid);
        }

        List<UIFader> faders = new List<UIFader>();
        foreach (GameObject faderContainer in m_FaderContainers)
        {
            // check if the game object contains UIFader component
            UIFader fader = faderContainer.GetComponent<UIFader>();

            if (fader)
            {
                faders.Add(fader);
            }
            // if the game object does not contain UIFader component,
            // check the game object's children
            else
            {
                faders.AddRange(faderContainer.GetComponentsInChildren<UIFader>());
            }
        }
        m_Faders = faders.ToArray();

        m_SceneBtnGridTransform = m_SceneBtnGrid.transform;
    }

    private void Start()
    {
        ShowSceneMenu();
    }
		
    /* end of MonoBehaviour */


    private GameObject CreateSceneBtn(DrugVR_SceneENUM scene, Transform parentTransform,
        GridLayoutGroup gridLayoutGroup)
    {
        GameObject newSceneBtn = Instantiate(m_SceneBtnPrefab, parentTransform);

        // TODO: The following assume some hierachical structure among the components
        newSceneBtn.GetComponent<GoToSceneOnSelected>().SceneToGoNext = scene;
        newSceneBtn.GetComponent<FadeSceneMenuOnSelected>().SceneMenuControl = this;
        newSceneBtn.GetComponent<MatchColliderToGrid>().Grid = gridLayoutGroup;
        newSceneBtn.GetComponentInChildren<Text>().text = scene.ToString();
        return newSceneBtn;
    }

    private IEnumerator HideSceneMenuRoutine()
    {
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
    }


    /* IShowAndHideSceneMenu interface */

    public void ShowSceneMenu()
    {
        foreach (UIFader fader in m_Faders)
        {
            StartCoroutine(fader.InterruptAndFadeIn());
        }

        if (OnShowMenu != null)
        {
            OnShowMenu();
        }
    }

    public void HideSceneMenu()
    {
        StartCoroutine(HideSceneMenuRoutine());

        if (OnHideMenu != null)
        {
            OnHideMenu();
        }
    }

    /* end of IShowAndHideSceneMenu interface */
}
