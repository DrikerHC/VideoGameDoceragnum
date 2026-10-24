using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaConsecuenciaReloj", menuName = "Historia/Consecuencias/Reloj")]
    public class ConsecuenciaReloj : BaseConsecuencia
    {
        public int cantidad;

        public override string Ejecutar(CharacterData npc)
        {
            Debug.Log("Se esta ejecutando la consecuencia de Felicidad");
            F2Manager.Instance.RemainingActions -= cantidad;
            return "Eso te ha sentado bien, se podria decir que eres un poc mas feliz";
        }
    }
}
