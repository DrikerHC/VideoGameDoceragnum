using UnityEngine;

namespace Doceragnum
{
    public class NPC : Interactable
    {
        public CharacterData Data { get; set; }

        public override void Interact()
        {
            Player.Instance.IsLocked = true;
            Debug.Log("Voy a ejecutar el nodo despues de haber detectado que ha interactuado");
            StoryManager.Instance.ProcesarNodo(Data.RootNode, Data);
        }
    }
}