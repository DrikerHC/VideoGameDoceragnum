using UnityEngine;

namespace Doceragnum
{
    public abstract class BaseNode : ScriptableObject
    {
        //Metode que se heredara per executar el node
        public abstract BaseNode EjecutarNodo(CharacterData npc);
    }
}