using Doceragnum;
using SceneController;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Phase_3
{
    public class F3Manager : Singleton<F3Manager>
    {
        [SerializeField] private Bed bed;
        [SerializeField] private ComputerF3 computerF3;
        [SerializeField] private RectTransform _computerCanvasRoot;
        [SerializeField] private GameObject _computerExitButton;
        [SerializeField] private List<UnityEngine.UI.Button> _botonesOpciones;

        //Datos del dialogue
        [SerializeField] private GameObject _dialogueUI;
        [SerializeField] private Image _characterImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _dialogueText;

        // EVENTS
        public event Action OnInitCompSm;
        public event Action OnResetCompSm;

        public override void Awake()
        {
            base.Awake();
            computerF3.OnComputerF3Interact += ChangeToCompView;
            computerF3.OnComputerF3Disconnect += ChangeToPlayerView;
            bed.OnEndDay += EndDay;
            HideComputerCanvas();
        }

        private void Start()
        {
            // Same as F2Manager: run after StoryManager / DialogueManager Awake
            StoryManager.Instance.llenarBotones(_botonesOpciones);
            DialogueManager.Instance.RegistrarReferenciasUI(_dialogueUI, _dialogueText, _nameText, _characterImage);
        }

        private void OnDisable()
        {
            if (computerF3 != null)
            {
                computerF3.OnComputerF3Interact -= ChangeToCompView;
                computerF3.OnComputerF3Disconnect -= ChangeToPlayerView;
            }

            if (bed != null)
                bed.OnEndDay -= EndDay;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void ChangeToPlayerView()
        {
            if (Player.Instance != null)
                Player.Instance.IsLocked = false;

            OnResetCompSm?.Invoke();
            HideComputerCanvas();
        }

        private void ChangeToCompView()
        {
            if (Player.Instance != null)
                Player.Instance.IsLocked = true;

            ShowComputerCanvas();
            OnInitCompSm?.Invoke();
        }

        private void ShowComputerCanvas()
        {
            if (_computerCanvasRoot == null)
                return;

            _computerCanvasRoot.localScale = Vector3.one;

            if (_computerExitButton != null)
                _computerExitButton.SetActive(true);
        }

        private void HideComputerCanvas()
        {
            if (_computerCanvasRoot != null)
                _computerCanvasRoot.localScale = Vector3.zero;

            if (_computerExitButton != null)
                _computerExitButton.SetActive(false);
        }

        public void ExitComputerView()
        {
            if (Player.Instance != null)
                Player.Instance.IsLocked = false;

            computerF3?.Disconnect();
        }
 
        private void EndDay()
        {
            if (!PreConditionsPassNextDay(out StoreItemSO.BonusType failedStat))
            {
                Debug.Log("Preconditions failed");
                InGameDataManager.FaseManager.Start();
                return;
            } 

            InGameDataManager.Data.DayCount++;
            GlobalEventHUD.InvokeDayChanged(InGameDataManager.Data.DayCount);
            InGameDataManager.Data.CurrentPhase = 1;
            GlobalEventHUD.InvokePhaseChanged(InGameDataManager.Data.CurrentPhase);
            SceneController.SceneController.Instance.NewPlan()
                .Load(SceneRegistry.Slots.Gameplay, SceneRegistry.Scenes.Fase1).WithOverlay().Execute();
        }

        private bool PreConditionsPassNextDay(out StoreItemSO.BonusType failedStat)
        {
            var player = InGameDataManager.PlayerData;

            if (player.felicidadActual <= 0)
            {
                failedStat = StoreItemSO.BonusType.HAPPINESS;
                return false;
            }

            if (player.ReputacionActual <= -player.ReputacionMax)
            {
                failedStat = StoreItemSO.BonusType.REPUTATION;
                return false;
            }

            if (!InGameDataManager.CanAffordOutstandingBills())
            {
                failedStat = StoreItemSO.BonusType.MONEY;
                return false;
            }

            failedStat = StoreItemSO.BonusType.HAPPINESS;
            return true;
        }
    }
}