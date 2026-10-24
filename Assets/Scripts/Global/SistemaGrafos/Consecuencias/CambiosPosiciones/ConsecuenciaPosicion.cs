using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "ConsecuenciaPosicion", menuName = "Historia/Consecuencias/Posición")]
    public class ConsecuenciaPosicion : BaseConsecuencia
    {
        public Vector3 Position;

        public override string Ejecutar(CharacterData npc)
        {
            npc.Position = Position;
            return "Posición del npc cambiada.";
        }
    }
}