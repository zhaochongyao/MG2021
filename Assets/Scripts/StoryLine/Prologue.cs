using System.Collections;
using DialogueSystem;
using UnityEngine;
using Utilities;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace StoryLine
{
    public class Prologue : LSingleton<Prologue>
    {
        [SerializeField] private float _laptopShutDelay;


        [SerializeField] private float _welcomeDialogueDelay;
        [SerializeField] private DialogueDataSO _welcomeDialogue;

        [SerializeField] private GameObject _arrow1;
        [SerializeField] private GameObject _arrow2;
        
        [SerializeField] private GameObject _left;
        [SerializeField] private GameObject _right;

        [SerializeField] private GameObject _phoneButton;


        [SerializeField] private GameObject _frontEnd;
        [SerializeField] private GameObject _office;

        [SerializeField] private float _twistDelay;
        [SerializeField] private GameObject _twistFX;
        [SerializeField] private float _twistStay;
        [SerializeField] private string _nextLevel;
        
        private void Start()
        {
            _arrow1.SetActive(true);
            _arrow2.SetActive(false);
            
            PlayerDistanceChecker.EnterEvent += OnEnterEvent;
            PlayerDistanceChecker.ExitEvent += OnExitEvent;
            
            OpenDoor.DoorOpen += () =>
            {
                Wait.Delayed(() =>
                {
                    DialoguePlayer.Instance.SendDialogue(_welcomeDialogue);
                }, _welcomeDialogueDelay);
            };

            DialoguePlayer.Instance.DialogueEvent += OnDialogueEvent;
        }

        private void OnEnterEvent(string eventName)
        {
            if (eventName == "到达前台")
            {
                DialoguePlayer.Instance.InvokeContinueEvent(eventName);
                _arrow1.SetActive(false);
            }
            else if (eventName == "睡着")
            {
                StartCoroutine(OnSleep());
            }
        }

        private void OnExitEvent(string eventName)
        {
            if (eventName == "进入办公室")
            {
                _frontEnd.SetActive(false);
                _office.SetActive(true);
            }
        }

        private IEnumerator OnSleep()
        {
            yield return Wait.Seconds(_twistDelay);
            _twistFX.SetActive(true);
            yield return Wait.Seconds(_twistStay);
            SceneLoader.LoadScene(_nextLevel);
        }

        private void OnDialogueEvent(string eventName)
        {
            if (eventName == "启用手机")
            {
                _phoneButton.SetActive(true);
            }
            else if (eventName == "离开场景")
            {
                _arrow2.SetActive(true);
            }
        }
        
        public void Shut()
        {
            Wait.Delayed(() =>
            {
                _left.SetActive(false);
                _right.SetActive(false);
            }, _laptopShutDelay);
        }
    }
}