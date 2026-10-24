using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;


namespace Doceragnum
{
    public class StoryManager : MonoBehaviour
    {
        public static StoryManager Instance;

        [Header("Referencias UI")]
        [SerializeField] private List<UnityEngine.UI.Button> _botonesOpciones;
        [SerializeField] private GameObject _ConsecuenciaUI;
        [SerializeField] private TextMeshProUGUI _ConsecuenciaTextUI;

        private DialogueNode _nodoDialogoActual;
        private CharacterData _NpcData;

        private void Awake() => Instance = this;

        public void llenarBotones(List<UnityEngine.UI.Button> nuevosBotones)
        {
            _botonesOpciones = nuevosBotones;
        }
        public void ProcesarNodo(BaseNode nodo, CharacterData _data)
        {
            // Aquesta funció neteja les opcions del diàleg anterior
            LimpiarBotones();
            // Establim la informació que enviarem al node
            _NpcData = _data;
            BaseNode temp = nodo;
            // Comprovem quin tipus de node és.
            // En cas de ser un node porta, s’itera executant el node
            // fins a arribar a un node de diàleg
            while (temp is PuertaNode)
            {
            // Bucle on s’executa el node porta fins que sigui un node de diàleg
                Debug.Log("He detectado un DecisionNode/Puerta");
                temp = temp.EjecutarNodo(_data);
            }
            if (temp is DialogueNode diag)
            {
                Debug.Log("Estas en la Fase: "+InGameDataManager.Data.CurrentPhase+" Y te quedan "+InGameDataManager.Data.CurrentPhase+" acciones");
            // Aquí fem una comprovació simple per veure si el jugador
            // té suficient energia per mantenir una conversa.
            // En cas contrari, s’executarà el node preestablert per aquesta situacio
                if (InGameDataManager.Data.CurrentPhase == 2 && F2Manager.Instance.RemainingActions == 0)
                {
                 // Executa el node preestablert quan el jugador no té energi
                  diag = InGameDataManager.PlayerData.NodoSinEnergia;
                    _nodoDialogoActual = diag;
                    DialogueManager.Instance.StartDialogueDesdeNodo(diag, _data);
                } else
                {
                 // Inicia la funció de mostrar el diàleg i,
                 // en cas que el node hagi de gastar energia,
                 // resta la quantitat corresponent
                    _nodoDialogoActual = diag;
                    if (InGameDataManager.Data.CurrentPhase == 2 && F2Manager.Instance != null)
                    {
                        F2Manager.Instance.RemainingActions -= diag.ActionSpend;
                    }
                    DialogueManager.Instance.StartDialogueDesdeNodo(diag, _data);
                }
            }
            else
            {
                FinalizarHistoria();
            }
        }

        public void OnDialogueFinished()  
        {
            Debug.Log("StoryManager: Ha acaabdo de escribir el dialogo");
            if (_nodoDialogoActual != null && _nodoDialogoActual.opciones.Count > 0)
            {
                Debug.Log("En teoria va a llamas a la funcion de actualizar botones");
                ActualizarBotonesLista(_nodoDialogoActual.opciones);
            }
        }

        private void ActualizarBotonesLista(List<Option> opciones)
        {
            Debug.Log("En teoria estoy poniendo los botones");
            LimpiarBotones();

            for (int i = 0; i < opciones.Count; i++)
            {
                if (i >= _botonesOpciones.Count) break;

                Option opcionActual = opciones[i];
                UnityEngine.UI.Button boton = _botonesOpciones[i];

                boton.gameObject.SetActive(true);
                boton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = opcionActual.MostrarTexto();
                boton.onClick.RemoveAllListeners();
                boton.onClick.AddListener(() =>
                {
                    SeleccionarOpcion(opcionActual);
                });
            }
        }

        private void SeleccionarOpcion(Option opcion)
        {
            LimpiarBotones();

            Debug.Log("Nombre del character con el que estamos hablando: " + _NpcData.name);
            opcion.AplicarConsecuencias(_NpcData,_ConsecuenciaTextUI );
            if(opcion.consecuencias.Count > 0)
                StartCoroutine(MostrarConsecuencias());
            
            if (opcion.siguienteNodo != null)
                ProcesarNodo(opcion.siguienteNodo, _NpcData);
            else
                FinalizarHistoria();
        }

        private void LimpiarBotones()
        {
            foreach (var boton in _botonesOpciones)
            {
                if (boton != null)
                    boton.gameObject.SetActive(false);
            }
        }

        private void FinalizarHistoria()
        {
            _nodoDialogoActual = null;
            LimpiarBotones();

            DialogueManager.Instance.CerrarUI();

            if (Player.Instance != null)
            {
                Player.Instance.IsLocked = false;
            }
            Debug.Log("Historia finalizada: Jugador desbloqueado.");
        }

        IEnumerator MostrarConsecuencias()
        {
            _ConsecuenciaUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            _ConsecuenciaUI.SetActive(false);
            _ConsecuenciaTextUI.text = "";
        }
    }
}