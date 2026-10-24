using Phase_3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Doceragnum
{
    public class Player : MonoBehaviour
    {
        public static Player Instance;
        private Rigidbody2D _body;

        private Vector2 _moveInput;
        [SerializeField] private float _moveSpeed;
        public bool IsLocked;
        public bool InsideMinigame;

        private Interactable _currentInteractable;

        [SerializeField] private InputActionReference _moveRef;
        [SerializeField] private InputActionReference _interactRef;
        [SerializeField] private InputActionReference _disconnectRef;
        [SerializeField] private InputActionReference _MisionTableRef;
        [SerializeField] private GameObject _TableroMisionesUI;

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();

            Instance = this;
            _interactRef.action.started += OnInteract;
            if (_TableroMisionesUI != null)
            {
                _MisionTableRef.action.started += OnMisionTable;
            }
            _disconnectRef.action.started += OnDisconnect;

            transform.position = InGameDataManager.PlayerData.position;
        }

        private void OnDestroy()
        {
            _interactRef.action.started -= OnInteract;
            _disconnectRef.action.started -= OnDisconnect;
            if (_TableroMisionesUI != null)
            {
                _MisionTableRef.action.started -= OnMisionTable;
            }
        }

        private void Update()
        {
            _moveInput = _moveRef.action.ReadValue<Vector2>().normalized;
        }

        private void FixedUpdate()
        {
            if (!IsLocked)
            {
                _body.linearVelocity = _moveSpeed * _moveInput;
            }
            else
            {
                _body.linearVelocity = _moveInput * 0;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Interactable interactable))
            {
                _currentInteractable = interactable;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Interactable interactable))
            {
                if (_currentInteractable == interactable)
                    _currentInteractable = null;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log("Hola paco");
            if (InGameDataManager.Data.CurrentPhase == 3)
            {
                if (_currentInteractable is ComputerF3)
                    IsLocked = true;
                _currentInteractable?.Interact();
            }
            else {
                if (InsideMinigame) return;         
                if (IsLocked)
                {
                    if (DialogueManager.Instance._isTyping)
                    {
                        DialogueManager.Instance.EndLineEarly();
                    }
                    else
                    {
                        DialogueManager.Instance.DisplayDialogueLine();
                    }
                }
                else
                {
                    if (_currentInteractable is ComputerF3) IsLocked = true;
                    _currentInteractable?.Interact();
                }
            }

        }

        private void OnDisconnect(InputAction.CallbackContext obj)
        {
            if (InGameDataManager.Data.CurrentPhase == 3 && IsLocked)
                return;

            if (_currentInteractable != null && _currentInteractable is ComputerF3 newComputerF3)
            {
                print("disconnect");
                IsLocked = false;
                newComputerF3.Disconnect();
            }
        }

        public void OnMisionTable(InputAction.CallbackContext context)
        {
            if (_TableroMisionesUI.activeSelf == true)
            {
                _TableroMisionesUI.SetActive(false);
            }
            else
            {
                _TableroMisionesUI.SetActive(true);
            }
        }

    }
}