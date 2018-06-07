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
        [SerializeField]
        private Sc07SClient m_Sc7SClientRef;

        private int m_PartyRoundCnt = 0;
        [SerializeField]
        private GameObject[] m_PartyRoundContainer;

        [SerializeField]
        private PartyVFXAnimationControl m_partyVFXControll;

        //Only Stores ONE Round of UIFaders and PartySelections
        private PartySelection[] m_PartySelections;
        private UIFader[] m_OptionUIFaders;

        [SerializeField]
        private AudioSource m_AudioSrc;
        [SerializeField]
        private AudioClip m_PersuadeClip;

        /* MonoBehavior */

        private void Awake()
        {
            if (m_AudioSrc == null)
            {
                m_AudioSrc = GetComponent<AudioSource>();
            }           

            m_partyVFXControll = GetComponentInChildren<PartyVFXAnimationControl>();
            
            LoadPartyRound(m_PartyRoundContainer[m_PartyRoundCnt]);
        }

        private void OnEnable()
        {
            m_partyVFXControll.OnFXPlay += HandleFXPlay;
            m_partyVFXControll.OnFXEnd += HandleFXEnd;
        }

        private void OnDisable()
        {
            m_partyVFXControll.OnFXPlay -= HandleFXPlay;
            m_partyVFXControll.OnFXEnd -= HandleFXEnd;

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
            partyRound.SetActive(true);
            m_PartySelections = partyRound.GetComponentsInChildren<PartySelection>();
            m_OptionUIFaders = partyRound.GetComponentsInChildren<UIFader>();

            foreach (PartySelection option in m_PartySelections)
            {
                option.OnSelected += HandlePartyOptionSelected;
            }
        }

        private void UnloadPartyRound(GameObject partyRound)
        {
            foreach (PartySelection option in m_PartySelections)
            {
                option.OnSelected -= HandlePartyOptionSelected;
            }
            partyRound.SetActive(false);
        }

        public IEnumerator HandlePartyOptionSelected(PartySelection selectedPartyOption)
        {
            //Load Party VFX in the background first
            if (selectedPartyOption.PartyOption != PartyOptionEnum.METH) m_partyVFXControll.PreparePartyVFX(selectedPartyOption.PartyOption);

            foreach (UIFader fader in m_OptionUIFaders)
            {
                StartCoroutine(fader.InterruptAndFadeOut());
            }

            yield return StartCoroutine(WaitUntilAllFadedOut());

            if (selectedPartyOption.PartyOption == PartyOptionEnum.METH)
            {
                Scribe.Side06 = false;
                m_Sc7SClientRef.GoToSceneOnChoice();       
            }
            else 
            {
                Scribe.Side06 = true;

                //Deactive current party round
                UnloadPartyRound(m_PartyRoundContainer[m_PartyRoundCnt++]);

                //Play corresponding VFX anim after button fade out
                m_partyVFXControll.PlayPartyVFX(selectedPartyOption.PartyOption);


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
                                         
            }
            

        }

        private void HandleFXPlay()
        {
            if (m_PartyRoundCnt < m_PartyRoundContainer.Length)
            {
                LoadPartyRound(m_PartyRoundContainer[m_PartyRoundCnt]);
            }
            
        }

        private void HandleFXEnd()
        {
            if (m_PartyRoundCnt < m_PartyRoundContainer.Length)
            {
                //Handle case before 2nd round is faded in
                if (m_PartyRoundCnt == 1)
                {
                    StartCoroutine(PersuadeMethRoutine());
                }
                else
                {
                    //Fade in all the buttons for the next round
                    foreach (UIFader fader in m_OptionUIFaders)
                    {
                        StartCoroutine(fader.InterruptAndFadeIn());
                    }
                }                                           
            }
            else
            {
                //Go To Next Scene if no more rounds to play
                m_Sc7SClientRef.GoToSceneOnChoice();
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

        private IEnumerator PersuadeMethRoutine()
        {
            if (m_PersuadeClip != null)
            {
                m_AudioSrc.clip = m_PersuadeClip;
            }
            m_AudioSrc.Play();
            yield return new WaitWhile(() => m_AudioSrc.isPlaying);
            //Fade in all the buttons for the next round
            foreach (UIFader fader in m_OptionUIFaders)
            {
                StartCoroutine(fader.InterruptAndFadeIn());
            }
            m_Sc7SClientRef.ActivateExitButton();
        }
    }
}
