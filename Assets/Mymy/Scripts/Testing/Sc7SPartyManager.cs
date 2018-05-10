using DrugVR_Scribe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

namespace Scene07Party
{
    public enum PartyOptionEnum
    {
        METH,
        Drink,
        Dart,
        Pool
    }

    public class Sc7SPartyManager : MonoBehaviour
    {


        [SerializeField]
        private GameObject methOptionPrefab;
        [SerializeField]
        private GameObject[] m_InitialOptionUIFadersContainers;
        [SerializeField]
        private PartyVFXAnimationControl m_partyVFXControll;


        private List<UIFader> m_OptionUIFaders = new List<UIFader>();
        private List<PartySelection> m_PartySelections = new List<PartySelection>();


        private void Awake()
        {
            m_partyVFXControll = GetComponentInChildren<PartyVFXAnimationControl>();

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
                StartCoroutine(fader.InterruptAndFadeIn());
            }
        }

        private void Update()
        {

        }

        public IEnumerator HandlePartyOptionSelected(PartySelection selectedPartyOption)
        {

            foreach (UIFader fader in m_OptionUIFaders)
            {
                StartCoroutine(fader.InterruptAndFadeOut());
            }

            yield return StartCoroutine(WaitUntilAllFadedOut());

            if (selectedPartyOption.PartyOption != PartyOptionEnum.METH)
            {
                Scribe.Side06 = true;
                
                //Play corresponding VFX anim after button fade out
                yield return StartCoroutine(m_partyVFXControll.PlayPartyVFX(selectedPartyOption.PartyOption));

                //TEST SCENE METHODS
                //Save selection button transform
                //Transform selectedOptionTransform = selectedPartyOption.transform;
                //Instantiate replacement button
                //GameObject newMethObject = Instantiate(methOptionPrefab, selectedOptionTransform.position, selectedOptionTransform.rotation, selectedOptionTransform.parent);

                //Add replacement to local Lists
                //m_PartySelections.Add(newMethObject.GetComponent<PartySelection>());
                //m_OptionUIFaders.Add(newMethObject.GetComponent<UIFader>());

                //Remove Selected Button
                //m_PartySelections.Remove(selectedPartyOption.GetComponent<PartySelection>());
                //m_OptionUIFaders.Remove(selectedPartyOption.GetComponent<UIFader>());
                //Destroy(selectedPartyOption.gameObject);
                //TEST SCENE METHODS ENDS

                //Fade in all the buttons for the next round
                foreach (UIFader fader in m_OptionUIFaders)
                {
                    StartCoroutine(fader.InterruptAndFadeIn());
                }
            }
            else
            {
                Scribe.Side06 = false;
                Sc07SClient.GoToSceneOnChoice();
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
}
