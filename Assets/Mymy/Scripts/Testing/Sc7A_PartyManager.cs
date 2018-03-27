using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class Sc7A_PartyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject methOptionPrefab;
    [SerializeField]
    private GameObject[] m_InitialOptionUIFadersContainers;


    private List<UIFader> m_OptionUIFaders = new List<UIFader>();
    private List<PartySelection> m_PartySelections = new List<PartySelection>();


    private void Awake()
    {       
        foreach (GameObject faderToStartContainer in m_InitialOptionUIFadersContainers)
        {
            // check if the game object contains UIFader component
            UIFader fader = faderToStartContainer.GetComponent<UIFader>();
            PartySelection partySelection = faderToStartContainer.GetComponent<PartySelection>();

            if (fader)
            {
                m_OptionUIFaders.Add(fader);
            }
            // if the game object does not contain UIFader component,
            // check the game object's children
            else
            {
                m_OptionUIFaders.AddRange(faderToStartContainer.GetComponentsInChildren<UIFader>());
            }



            if (partySelection)
            {
                m_PartySelections.Add(partySelection);
            }
            // if the game object does not contain PartySelection component,
            // check the game object's children
            else
            {
                m_PartySelections.AddRange(faderToStartContainer.GetComponentsInChildren<PartySelection>());
            }
        }

    }

    private void OnEnable()
    {
        foreach (PartySelection option in m_PartySelections)
        {
            option.OnSelected += HandlePartyOptionSelected;
        }
    }

    private void OnDisable()
    {
        foreach (PartySelection option in m_PartySelections)
        {
            option.OnSelected -= HandlePartyOptionSelected;
        }
    }

    private void Start()
    {
        foreach (UIFader fader in m_OptionUIFaders)
        {
            StartCoroutine(fader.InteruptAndFadeIn());
        }
    }

    private void Update()
    {
        
    }

    private IEnumerator HandlePartyOptionSelected(GameObject selectedPartyOption)
    {
        
        foreach (UIFader fader in m_OptionUIFaders)
        {
            StartCoroutine(fader.InteruptAndFadeOut());
        }

        yield return StartCoroutine(WaitUntilAllFadedOut());
    

        Transform selectedOptionTransform = selectedPartyOption.transform;
        GameObject newMethObject = Instantiate(methOptionPrefab, selectedOptionTransform.position, selectedOptionTransform.rotation, selectedOptionTransform.parent);
        m_PartySelections.Add(newMethObject.GetComponent<PartySelection>());
        m_OptionUIFaders.Add(newMethObject.GetComponent<UIFader>());


        m_PartySelections.Remove(selectedPartyOption.GetComponent<PartySelection>());
        m_OptionUIFaders.Remove(selectedPartyOption.GetComponent<UIFader>());
        Destroy(selectedPartyOption.gameObject);
        

        foreach (UIFader fader in m_OptionUIFaders)
        {
            StartCoroutine(fader.InteruptAndFadeIn());
        }

    }

    private IEnumerator WaitUntilAllFadedOut()
    {
        yield return new WaitUntil(() =>
        {
            foreach (UIFader fader in m_OptionUIFaders)
            {
                if (fader.Visible || fader.Fading)
                {
                    return false;
                }
            }

            return true;
        });
    }

}
