using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaConsecuenciaMision", menuName = "Historia/Consecuencias/Mision")]
    public class ConsecuenciaMision : BaseConsecuencia
    {
        public int Paso;
        public Mision MisionNPC;

        public override string Ejecutar(CharacterData npc)
        {
            Debug.Log("Se esta ejecutando la consecuencia de Mision");
            MisionNPC.CumplirMision(Paso);
            return null;
        }
    }
}
