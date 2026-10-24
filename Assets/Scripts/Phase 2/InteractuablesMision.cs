using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    public class InteractuableMision : Interactable
    {
        [SerializeField] private Mision _Mision;
        [SerializeField] private BaseNode _nodoInicial;
        [SerializeField] private List<int> Pasos;
        [SerializeField] private Queue<int> ColaPasos;
        [SerializeField] private CharacterData _Data;

        public void Start()
        {
            ColaPasos = new Queue<int>(Pasos);
        }

        public override void Interact()
        {
            if(ColaPasos.Count > 0)
            {
                Debug.Log("Entro a la funcion heredada");
                Player.Instance.IsLocked = true;
                Debug.Log("He bloqueado al player");
                if (ColaPasos.Peek() == 0 || _Mision.Pasos[ColaPasos.Peek() - 1].IsCumplido)
                {
                    _Mision.CumplirMision(ColaPasos.Dequeue());
                    Debug.Log("He ejecutado el cumplir mision");
                }
                else
                {
                    Debug.Log("Anterior paso cumplido: "+_Mision.Pasos[ColaPasos.Peek() - 1].IsCumplido);
                    Debug.Log("Peek: " + ColaPasos.Peek() );
                    Debug.Log("Peek: " + (ColaPasos.Peek() - 1));
                    Debug.Log("No he ejecutado el cumplir mision");
                }
                    
                
                Debug.Log("Voy a ejecutar el nodo despues de haber detectado que ha interactuado");
                if (_nodoInicial != null)
                    StoryManager.Instance.ProcesarNodo(_nodoInicial, _Data);
            }
        }
    }
}