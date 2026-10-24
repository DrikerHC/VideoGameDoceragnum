using System;
using UnityEngine;

namespace Doceragnum
{
    [Serializable]
    public abstract class BaseConsecuencia : ScriptableObject
    {

        //En realidad recibira al player, pero ahora mismo no me quiero complicar asi que simplemente lo hago asi
        public abstract string Ejecutar(CharacterData npc);
    }
}