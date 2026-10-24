using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;

        private GameObject _dialogueUI;
        private Image _characterImage;
        private TextMeshProUGUI _nameText;
        private TextMeshProUGUI _dialogueText;
        private Queue<string> _dialogueLines = new Queue<string>();
        public bool _isTyping;

        [SerializeField] private float _typeSpeed;

        private Coroutine _typeLineCoroutine;
        private DialogueNode _nodoActual;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // El original sobrevive
            }
            else
            {
                // Si ya hay uno, este objeto de la escena actual se destruye inmediatamente
                Destroy(gameObject);
                return;
            }
        }
        public void RegistrarReferenciasUI(GameObject panel, TMPro.TextMeshProUGUI textoDialogo, TMPro.TextMeshProUGUI textoNombre, UnityEngine.UI.Image fotoPerfil)
        {
            // Cambia estos nombres por los nombres exactos de tus variables en tu DialogueManager
            _dialogueUI = panel;
            _dialogueText = textoDialogo;
            _nameText = textoNombre;
            _characterImage = fotoPerfil; // <--- ¡Esta es la que causó el error en la línea 92!

            Debug.Log("DialogueManager: ¡Referencias de UI actualizadas para esta escena!");
        }

        public void DisplayDialogueLine()
        {
            if (_dialogueLines.Count == 0)
            {
                Debug.Log("Ha acabado de escribir el dialogo");
                EndDialogue();
                return;
            }
            _typeLineCoroutine = StartCoroutine(TypeLineCoroutine(_dialogueLines.Dequeue()));
        }

        public void EndLineEarly()
        {
            StopCoroutine(_typeLineCoroutine);
            _dialogueText.maxVisibleCharacters = 999;
            _isTyping = false;
            EndDialogue();
        }
        private void EndDialogue()
        {
            if (_nodoActual.opciones == null || _nodoActual.opciones.Count == 0)
            {
                Debug.Log("Se acabo soy libre");
                _dialogueUI.SetActive(false);
                Player.Instance.IsLocked = false;
            }

            StoryManager.Instance.OnDialogueFinished();
        }

        private IEnumerator TypeLineCoroutine(string line)
        {
            _isTyping = true;

            int maxVisibleChars = 0;

            _dialogueText.text = line;
            _dialogueText.maxVisibleCharacters = maxVisibleChars;

            foreach (char c in line.ToCharArray())
            {
                maxVisibleChars++;
                _dialogueText.maxVisibleCharacters = maxVisibleChars;
                yield return new WaitForSeconds(0.2f / _typeSpeed);
            }
            if (_dialogueLines.Count == 0)
            {
                EndDialogue();
            }
            _isTyping = false;
        }

        public void CerrarUI()
        {
            _dialogueUI.SetActive(false);
            _dialogueText.text = string.Empty;
        }

        public void StartDialogueDesdeNodo(DialogueNode nodo, CharacterData NPC)
        {
            Debug.Log("Deberia empezar a escribir el dialogo");
            _characterImage.sprite = NPC.DialogueImage;
            _nameText.text = NPC.Name;
            if (InGameDataManager.Data.CurrentPhase == 2 && F2Manager.Instance.RemainingActions == 0)
                _nameText.text = "Mark(Tu mismo)";
            _dialogueText.text = string.Empty;
            _dialogueUI.SetActive(true);
            _dialogueLines.Clear();
            _dialogueLines.Enqueue(nodo.MostrarTexto());
            _nodoActual = nodo;
            if(_nodoActual)
            DisplayDialogueLine();
        }
    }
}