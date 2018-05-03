using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRStandardAssets.Utils;
using wvr;

namespace Scene07Party
{
    public class PartySelection : VrButtonBase
    {

        [SerializeField]
        public PartyOptionEnum PartyOption;
        [SerializeField]
        private Sc7SPartyManager m_PartyManager;

        public event Func<PartySelection, IEnumerator> OnSelected;
        public AudioClip PartySFXAudioClip { get { return m_PartySFXAudioClip; } }

        [SerializeField]
        private AudioClip m_PartySFXAudioClip;


        /* MonoBehaviour */

        private void OnEnable()
        {
            if (!m_PartyManager)
            {
                m_PartyManager = GetComponentInParent<Sc7SPartyManager>();
            }

            OnSelected += m_PartyManager.HandlePartyOptionSelected;
        }

        protected override void Update()
        {
            base.Update();
        }

        /* end of MonoBehaviour */

        private void RaiseOnSelectedEvent()
        {
            if (OnSelected != null)
            {
                StartCoroutine(OnSelected(this));
            }
        }


        /* IHandleUiButton interfaces */

        public override void HandleDown()
        {
            base.HandleDown();
            if (m_GazeOver)
            {
                RaiseOnSelectedEvent();
            }
        }

        public override void HandleEnter()
        {
            base.HandleEnter();
        }

        public override void HandleExit()
        {
            base.HandleExit();
        }

        public override void HandleUp()
        {
            base.HandleUp();
        }

        /* end of IHandleUiButton interfaces */
    }
}
