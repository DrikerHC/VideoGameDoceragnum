using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaConsecuenciaFelicidad", menuName = "Historia/Consecuencias/Afecto")]
    public class ConsecuenciaAfecto : BaseConsecuencia
    {
        public int cantidad;

        public override string Ejecutar(CharacterData npc)
        {
            Debug.Log("Se esta ejecutando la consecuencia de Afecto");
            npc.afectoActual += cantidad;
            if (cantidad > 0)
            {
                return "Eso ha hecho que la confianza de "+ npc.Name +" aumente ";
            }
            else
            {
                return "Eso no le ha gustado a " + npc.Name;
            }
        }
    }
}