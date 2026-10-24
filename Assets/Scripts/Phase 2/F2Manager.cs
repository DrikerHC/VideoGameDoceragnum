using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class F2Manager : MonoBehaviour
    {
        public static F2Manager Instance;

        public NpcWalker Npcwalker;
        [SerializeField] private GameObject[] Clocks;
        [SerializeField] private List<UnityEngine.UI.Button> _botonesOpciones;
        [SerializeField] private GameObject ClockPrefab;
        [SerializeField] private Transform Parent;
        [SerializeField] private float _remainingActions = 5;

        //Datos del dialogue
        [SerializeField] private GameObject _dialogueUI;
        [SerializeField] private Image _characterImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        public float RemainingActions
        {
            get { return _remainingActions; }
            set
            {
                if (Clocks != null)
                {
                    _remainingActions = value;
                    if (_remainingActions <= 0)
                    {
                        NoMoreActions();
                        Clocks[4].SetActive(false);
                        Debug.Log("Volver a casa");
                    }
                    else
                    {
                        int CloclsToDelete = (int)(Clocks.Length - _remainingActions);
                        Debug.Log("Relojes a esconder: " + CloclsToDelete);
                        for (int i = 0; i < CloclsToDelete; i++)
                        {
                            Clocks[i].SetActive(false);
                            Debug.Log("Reloj escondido");
                        }
                        Debug.Log("En teoria he acabado de esconder relojes");
                    }
                }
                }
                
        }

        private bool _gracePeriod = true;
        private RuleSet ruleSet;
        private System.Random rand;
        private int _currentChecks;

        [SerializeField] private GameObject[] _eventObjects;
        private GameObject _currentEvent;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _remainingActions = 5;
            Clocks = new GameObject[(int)_remainingActions];
            StoryManager.Instance.llenarBotones(_botonesOpciones);
            DialogueManager.Instance.RegistrarReferenciasUI(_dialogueUI, _dialogueText, _nameText, _characterImage);
            for (int i = 0; i < _remainingActions; i++)
            {
                GameObject newClock = Instantiate(ClockPrefab, Parent);

                Clocks[i] = newClock;
            }
            ruleSet = InGameDataManager.Data.RuleSets[InGameDataManager.Data.DayCount - 1];
            if (ruleSet.HasRandomEvents)
            {
                Debug.Log("Empezamos eventos");
                StartCoroutine(CheckForRandomEvent());
            }
        }

        private IEnumerator CheckForRandomEvent()
        {
            // Grace period
            if (_gracePeriod) yield return new WaitForSeconds(5);
            _gracePeriod = false;
            int characterSeed = GenerateSeed();
            rand = new System.Random(characterSeed);
            int randNum = rand.Next(100);
            if (randNum < ruleSet.RandomEventProb)
            {
                Debug.Log("Se inicia evento");
                _currentEvent = _eventObjects[rand.Next(_eventObjects.Length)];
                _currentEvent.GetComponent<IMinigame>().OnGameFinished += OnRandomEventFinished;
                _currentEvent.SetActive(true);

                yield break;
            }
            yield return new WaitForSeconds(5f);
            StartCoroutine(CheckForRandomEvent());
        }

        public void OnRandomEventFinished()
        {
            _currentEvent.SetActive(false);
            _gracePeriod = true;
            StartCoroutine(CheckForRandomEvent());
        }

        private int GenerateSeed()
        {
            var day = InGameDataManager.Data.DayCount;
            return InGameDataManager.Data.Seed.Seeded(day + (_currentChecks++));
        }

        private void NoMoreActions()
        {

        }
    }
}