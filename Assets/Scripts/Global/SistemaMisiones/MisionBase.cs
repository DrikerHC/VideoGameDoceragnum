using UnityEngine;

namespace Doceragnum
{
    public abstract class BaseMision : ScriptableObject
    {
        //Metodo que se heredara para ejecutar el nodo
        public abstract void CumplirMision(int paso);
        public abstract bool IsMisionCumplida();
        public abstract int GetLastStep();
    }
}