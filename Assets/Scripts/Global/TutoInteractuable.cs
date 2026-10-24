using System;
using UnityEngine;

namespace Doceragnum
{
    public class TutoInteractuable : Interactable
    {
        [SerializeField] private bool IsTutoDone = false;
        [SerializeField] private BaseNode _nodoInicial;
        [SerializeField] private CharacterData _Data;
        public override void Interact()
        {
            Debug.Log("Click a la e, eso seguro");
            if (IsTutoDone || InGameDataManager.Data.DayCount!=1)
                return;
            IsTutoDone = true;
            Debug.Log("Hemos interactuado con el phone");
            Cursor.visible = true;
            if (_nodoInicial != null)
                StoryManager.Instance.ProcesarNodo(_nodoInicial, _Data);
        }
        
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsTutoDone || InGameDataManager.Data.DayCount!=1)
                return;
            base.OnTriggerEnter2D(collision);
            Player.Instance.IsLocked = true;
            Debug.Log("El jugador se acercó al tuto");
        }
    }
}


