using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaMision", menuName = "Historia/Misiones")]
    public class Mision : BaseMision
    {
        public string misionID;
        [TextArea(3, 10)]
        public string MisionName;
        public bool IsCumplida;
        public List<Pasos> Pasos;
        public string NameCharacterMision;

        public override bool IsMisionCumplida()
        {
            return IsCumplida;
        }
        public override int GetLastStep()
        {
            for (int i = 0; i < Pasos.Count; i++)
            {
                if (!Pasos[i].IsCumplido)
                {
                    return i;
                }
            }
            return -1;
        }

        public override void CumplirMision(int paso)
        { 
            Pasos[paso].IsCumplido = true;

            foreach (var pasoC in Pasos)
            {
                if (!pasoC.IsCumplido)
                {
                    IsCumplida = false;
                    return;
                }
                    

            }
            IsCumplida = true;
        }
    }

    [System.Serializable]
    public class Pasos
    {
        public string NamePaso;
        public bool IsCumplido;
    }
}