using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Doceragnum
{
    public class F1Manager : MonoBehaviour
    {
        public static F1Manager Instance { get; private set; }

        public enum DocumentType { DNI, StudentCard, ProfessorCard }

        [Header("Difficulty Modules"), SerializeField] private DifficultyModule _normalDiffModule;
        [SerializeField] private DifficultyModule _hardDiffModule;
        public DifficultyModule CurrentDiffModule { get; private set; }
        private RuleSet _todayRuleset;

        private int _visitorCount = 0;
        private int _normalNpcCount = 0;
        private int _attended = 0;
        private int _errors = 0;

        [Header("On Press Action")]
        [SerializeField] private EventPressed _onPress;

        [Header("UI Document GameObjects")]
        public GameObject StudentCardGameObject;
        public GameObject IdentityCardGameObject;
        public GameObject ProfessorCardGameObject;

        [Header("_canvas")]
        [SerializeField] private Canvas _canvas;

        [Header("Penalties"), SerializeField] private Transform _documentsParent;
        [SerializeField] private GameObject _penaltyPrefab;

        private GameObject _currentVisitor;
        public CharacterData CurrentVisitorData { get; private set; }
        public BaseErrorType CurrentError { get; private set; }

        private bool _available = false;
        private bool _served = false;
        private bool _isVisitorAccepted = false;

        private Coroutine _spawnVisitorsCoroutine;
        [Header("End Menu Texts"), SerializeField] private GameObject _endMenu;
        [SerializeField] private TextMeshProUGUI _errorsText;
        [SerializeField] private TextMeshProUGUI _successes;
        [SerializeField] private TextMeshProUGUI _penalties;
        [SerializeField] private TextMeshProUGUI _total;

        [Header("Dialogue"), SerializeField] private TextMeshProUGUI _npcText;
        [SerializeField] private GameObject _npcWindow;
        [SerializeField] private TextMeshProUGUI _playerText;
        [SerializeField] private GameObject _playerWindow;
        private DialogueNode _dialogueNode;
        private bool _inDialogue = false;
        private bool _isNpcTalking = false;

        [SerializeField] private InputActionReference _interactRef;

        private void Awake()
        {
            Instance = this;
            CurrentDiffModule = InGameDataManager.Data.difficulty == GameData.Difficulty.Normal ? _normalDiffModule : _hardDiffModule;

            int dayIndex = InGameDataManager.Data.DayCount - 1;
            var ruleSets = InGameDataManager.Data.RuleSets;
            if (ruleSets == null || ruleSets.Length == 0)
            {
                Debug.LogError("F1Manager: no RuleSets configured on GameData.");
                return;
            }

            if (dayIndex < 0 || dayIndex >= ruleSets.Length)
            {
                Debug.LogError(
                    $"F1Manager: DayCount {InGameDataManager.Data.DayCount} is out of range for RuleSets (length {ruleSets.Length}).");
                dayIndex = Mathf.Clamp(dayIndex, 0, ruleSets.Length - 1);
            }

            _todayRuleset = ruleSets[dayIndex];
        }

        private void OnEnable()
        {
            _onPress.OnEvent += ButtonCheck;
            _interactRef.action.started += EndLine;
        }

        private void OnDisable()
        {
            _onPress.OnEvent -= ButtonCheck;
        }

        private void Start()
        {
            InGameDataManager.CharacterPool.Refill();
            InGameDataManager.CharacterPool.GenerateMainCharactersDocuments();
            //_remainingSeconds = (int)(CurrentDiffModule.CalculateRoundTime() * 60);
            //_remainingSeconds = 5;

            _spawnVisitorsCoroutine = StartCoroutine(SpawnVisitors());
        }

        private IEnumerator SpawnVisitors()
        {
            while (InGameDataManager.CharacterPool._charsIndex < InGameDataManager.CharacterPool._mainCharactersPool.Count)
            {
                yield return new WaitForSeconds(0.5f);
                _served = false;
                CreateVisitor();
                yield return new WaitUntil(() => _served);
            }
            StartCoroutine(EndRound());
        }

        private IEnumerator EndRound()
        {
            if (!_served)
            {
                yield return new WaitUntil(() => _served);
            }
            else
            {
                StopCoroutine(_spawnVisitorsCoroutine);
            }
            _errorsText.text = _errors.ToString();
            _successes.text = (_attended - _errors).ToString();
            _penalties.text = (_errors * 100).ToString();
            _total.text = ((_attended - _errors) * 100 - _errors * 100).ToString();
            _endMenu.SetActive(true);
        }

        private void CreateVisitor()
        {
            if (_normalNpcCount < 1)
            {
                var CharacterInfo = InGameDataManager.CharacterPool.GetElement(_visitorCount);

                _visitorCount++;
                CurrentVisitorData = CharacterInfo.CharacterData;
                foreach (var docScript in CurrentVisitorData.Documents)
                {
                    docScript.InitVisitorData(CurrentVisitorData);
                }

                int errorChance = CurrentDiffModule.CalculateErrorChance();
                int num = Random.Range(0, 100);
                if (num < errorChance)
                {
                    Debug.Log("Error present");
                    int randDoc = Random.Range(0, CurrentVisitorData.Documents.Count);
                    DocumentUI document = CurrentVisitorData.Documents[randDoc];
                    List<BaseErrorType> errorTypesByDocument = _todayRuleset.ErrorTypes.Where(e => e.AppliesTo.Contains(document.DocumentType)).ToList();
                    if (errorTypesByDocument.Count > 0)
                    {
                        CurrentError = errorTypesByDocument[Random.Range(0, errorTypesByDocument.Count)];
                        CurrentError.Apply(document);
                    }
                }
                _currentVisitor = CharacterInfo.GameObject;
                _currentVisitor.transform.position = new Vector3(-12, -0.8f, 0);
                _normalNpcCount++;
            }
            else
            {
                _normalNpcCount = 0;
                _currentVisitor = InGameDataManager.CharacterPool.GetMainCharacter();
                Debug.Log($"<color=green>NPC:</color> {_currentVisitor.GetComponent<NPC>() != null}, <color=blue>Data:</color> {_currentVisitor.GetComponent<NPC>()?.Data != null}");

                if (_currentVisitor.GetComponent<NPC>()?.Data != null)
                {
                    Debug.Log($"<color=yellow>Nombre:</color> {_currentVisitor.GetComponent<NPC>().Data.Name}");
                    Debug.Log($"<color=red>Docs Count:</color> {_currentVisitor.GetComponent<NPC>().Data.Documents?.Count}");
                }
                CurrentVisitorData = _currentVisitor.GetComponent<NPC>().Data;
                foreach (var docScript in CurrentVisitorData.Documents)
                {
                    docScript.InitVisitorData(CurrentVisitorData);
                }
                _currentVisitor.transform.position = new Vector3(-12, -2, 0);
                _currentVisitor.GetComponent<SpriteRenderer>().sprite = _currentVisitor.GetComponent<NPC>().Data.Sprite;
                _currentVisitor.SetActive(true);
            }

            _currentVisitor.transform.localScale = new Vector3(3.7f, 3.7f, 1);
            Tween.PositionX(_currentVisitor.transform, 0, 1).OnComplete(target: this, target => target.StartDialogue());
        }

        private void StartDialogue()
        {
            _npcWindow.SetActive(true);
            _inDialogue = true;
            _isNpcTalking = true;

            // 1. Empezamos desde el RootNode
            BaseNode nodoActual = CurrentVisitorData.RootNode;

            // 2. Iteramos mientras sea un PuertaNode para encontrar el diálogo real
            while (nodoActual is PuertaNode)
            {
                nodoActual = nodoActual.EjecutarNodo(CurrentVisitorData);
            }

            // 3. Ahora que salimos de las puertas, intentamos el cast
            _dialogueNode = nodoActual as DialogueNode;

            if (_dialogueNode != null)
            {
                _npcText.text = _dialogueNode.miTextoLocalizado.GetLocalizedString();
            }
            else
            {
                Debug.LogError($"F1Manager: Después de las puertas, no se encontró un DialogueNode en {CurrentVisitorData.Name}");
                _inDialogue = false;
                ShowDocuments();
            }
        }

        private void EndLine(InputAction.CallbackContext context)
        {
            if (_inDialogue)
            {
                if (_isNpcTalking)
                {
                    _npcWindow.SetActive(false);
                    _playerWindow.SetActive(true);
                    _npcText.text = "";
                    _playerText.text = _dialogueNode.opciones[0].MostrarTexto();
                    _isNpcTalking = false;
                }
                else
                {
                    if (_dialogueNode.opciones[0].siguienteNodo != null)
                    {
                        Debug.Log("Opcion: " + _dialogueNode.opciones[0].siguienteNodo);

                        if (_dialogueNode.opciones[0].siguienteNodo is PuertaNode)
                        {
                            var NP = _dialogueNode.opciones[0].siguienteNodo;
                            while (NP.EjecutarNodo(CurrentVisitorData) is PuertaNode)
                            {
                                NP = NP.EjecutarNodo(CurrentVisitorData);
                                Debug.Log("Nodo: " + NP);
                            }
                            Debug.Log("He salido ya");
                            _dialogueNode = (DialogueNode)NP.EjecutarNodo(CurrentVisitorData);
                        }
                        else
                        {
                            _dialogueNode = (DialogueNode)_dialogueNode.opciones[0].siguienteNodo;
                        }
                        _playerWindow.SetActive(false);
                        _npcWindow.SetActive(true);
                        _playerText.text = "";
                        _npcText.text = _dialogueNode.miTextoLocalizado.GetLocalizedString();
                        _isNpcTalking = true;
                    }
                    else
                    {
                        _inDialogue = false;
                        _playerWindow.SetActive(false);
                        _playerText.text = "";
                        ShowDocuments();
                    }
                }
            }
        }

        private void ShowDocuments()
        {
            foreach (var doc in CurrentVisitorData.Documents)
            {
                doc.DeskDocument.SetActive(true);
            }
            _available = true;
        }

        private void ButtonCheck(bool isAccepted)
        {
            if (_available)
            {
                _isVisitorAccepted = isAccepted;
                if (isAccepted)
                {

                    // 1. ¿Existe el objeto de consecuencias?
                    if (CurrentVisitorData.listaConsecuenciasPorDia != null)
                    {
                        int dayIndex = InGameDataManager.Data.DayCount - 1;
                        var todasLasListas = CurrentVisitorData.listaConsecuenciasPorDia.listas;

                        // 2. ¿El índice del día existe en este NPC? (Aquí es donde fallaba)
                        if (dayIndex >= 0 && dayIndex < todasLasListas.Count)
                        {
                            var listaDeHoy = todasLasListas[dayIndex];

                            // 3. ¿Hay consecuencias definidas para hoy?
                            if (listaDeHoy.consecuencias != null)
                            {
                                foreach (var consecuencia in listaDeHoy.consecuencias)
                                {
                                    consecuencia.Ejecutar(CurrentVisitorData);
                                }
                            }
                        }
                        else
                        {
                            // Esto te dirá exactamente qué NPC te falta configurar
                            Debug.LogError($"Al NPC {CurrentVisitorData.Name} le faltan datos para el día {InGameDataManager.Data.DayCount}");
                        }
                    }
                    InGameDataManager.CharacterPool.AllowInside(_visitorCount - 1);
                }
                if (CanAccess() && !isAccepted)
                {
                    Debug.Log("DEBERIA DE HABER SIDO ACEPTADO Y LO HAS DENEGADO");
                    InstanciatePenalty("Se ha rechazado el acceso a una persona válida");
                    _errors++;
                }
                else if (!CanAccess() && isAccepted)
                {
                    Debug.Log("DEBERIA HABER SIDO DENEGADO Y LO HAS ACEPTADO");
                    InstanciatePenalty(CurrentError.ErrorDescription);
                    _errors++;
                }
                _attended++;
                DisableCurrentVisitor();
            }
        }

        private void InstanciatePenalty(string desc)
        {
            var penaltyobj = Instantiate(_penaltyPrefab, _documentsParent);
            var penalty = penaltyobj.GetComponent<Penalty>();
            penalty.Canvas = _canvas;
            penalty.canvasRectTransform = penalty.Canvas.GetComponent<RectTransform>();
            penalty.Description = desc;
        }

        private bool CanAccess()
        {
            foreach (var req in _todayRuleset.RequisitesOfToday)
            {
                if (!req.Validate()) return false;
            }
            return true;
        }

        public void DisableCurrentVisitor()
        {
            _available = false;
            foreach (var doc in CurrentVisitorData.Documents)
            {
                doc.DisableDocument();
            }
            if (_isVisitorAccepted) Tween.PositionX(_currentVisitor.transform, 12, 1).OnComplete(target: this, target => target.NextVisitor());
            else Tween.PositionX(_currentVisitor.transform, -12, 1).OnComplete(target: this, target => target.NextVisitor());
        }

        private void NextVisitor()
        {
            _currentVisitor.transform.localScale = new Vector3(1, 1, 1);
            InGameDataManager.CharacterPool.ReturnElement(_currentVisitor);
            _served = true;
        }

        public void ChangeOfPhase()
        {
            InGameDataManager.Data.CurrentPhase = 2;
            GlobalEventHUD.InvokePhaseChanged(InGameDataManager.Data.CurrentPhase);
            PauseMenu.Instance.CanBeOpened = false;
            SceneController.SceneController.Instance.NewPlan()
                .Unload(SceneController.SceneRegistry.Slots.Gameplay)
                .Load(SceneController.SceneRegistry.Slots.Gameplay, SceneController.SceneRegistry.Scenes.Fase2)
                .WithOverlay()
                .OnComplete(() =>
                {
                    InGameDataManager.CharacterPool.InitMainCharacters();
                    RoomManager.Instance.PlaceWalkNpcs();
                    F2Manager.Instance.Npcwalker.SetNpcWalk();
                    RoomManager.Instance.Init();
                }).Execute();
        }
    }
}