using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaConsecuenciaFelicidad", menuName = "Historia/Consecuencias/Felicidad")]
    public class ConsecuenciaFelicidad : BaseConsecuencia
    {
        public int cantidad;

        public override string Ejecutar(CharacterData npc)
        {
            Debug.Log("Se esta ejecutando la consecuencia de Felicidad");
            InGameDataManager.PlayerData.felicidadActual += cantidad;
            return "Eso te ha sentado bien, se podria decir que eres un poc mas feliz";
        }
    }
}