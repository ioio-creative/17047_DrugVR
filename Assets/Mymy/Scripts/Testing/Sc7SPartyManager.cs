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

    public class PartyEnumComparer : IEqualityComparer<PartyOptionEnum>
    {
        public bool Equals(PartyOptionEnum x, PartyOptionEnum y)
        {
            return x == y;
        }

        public int GetHashCode(PartyOptionEnum x)
        {
            return (int)x;
        }
    }

    public class Sc7SPartyManager : MonoBehaviour
    {
        private int m_PartyRoundCnt = 0;
        [SerializeField]
        private GameObject[] m_PartyRoundContainer;

        [SerializeField]
        private PartyVFXAnimationControl m_partyVFXControll;

        //Only Stores ONE Round of UIFaders and PartySelections
        private PartySelection[] m_PartySelections;
        private UIFader[] m_OptionUIFaders;


        //private List<PartySelection> m_PartySelections = new List<PartySelection>();
        //private List<UIFader> m_OptionUIFaders = new List<UIFader>();

        /* MonoBehavior */

        private void Awake()
        {
            m_partyVFXControll = GetComponentInChildren<PartyVFXAnimationControl>();

            LoadPartyRound(m_PartyRoundContainer[m_PartyRoundCnt]);
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

        /* end of MonoBehaviour */

        private void LoadPartyRound(GameObject partyRound)
        {
            m_PartySelections = partyRound.GetComponentsInChildren<PartySelection>();
            m_OptionUIFaders = partyRound.GetComponentsInChildren<UIFader>();
            partyRound.SetActive(true);
        }

        public IEnumerator HandlePartyOptionSelected(PartySelection selectedPartyOption)
        {

            foreach (UIFader fader in m_OptionUIFaders)
            {
                StartCoroutine(fader.InterruptAndFadeOut());
            }

            yield return StartCoroutine(WaitUntilAllFadedOut());
            //Deactive current party round
            m_PartyRoundContainer[m_PartyRoundCnt].SetActive(false);

            if (selectedPartyOption.PartyOption == PartyOptionEnum.METH)
            {
                Scribe.Side06 = false;
                Sc07SClient.GoToSceneOnChoice();
            }
            else 
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
                //Load Next Round
                if (m_PartyRoundCnt < m_PartyRoundContainer.Length)
                {
                    LoadPartyRound(m_PartyRoundContainer[m_PartyRoundCnt++]);
                    //Fade in all the buttons for the next round
                    foreach (UIFader fader in m_OptionUIFaders)
                    {
                        StartCoroutine(fader.InterruptAndFadeIn());
                    }
                }
                else
                {
                    //Go To Next Scene if no more rounds to play
                    Sc07SClient.GoToSceneOnChoice();
                }


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
