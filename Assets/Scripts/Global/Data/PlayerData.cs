using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Historia/Data/PlayerInfo")]
    public class PlayerData : ScriptableObject
    {
        public string nombreUsuario;
        public int felicidadMax;
        public int felicidadActual;
        public int ReputacionMax;
        public int ReputacionActual;
        public int DineroActual;
        public int estamina;

        [Header("New Game starting values")]
        public int dineroInicial = 500;
        public int felicidadInicial = 50;
        public int reputacionInicial = 5;
        public int estaminaInicial = 50;
        public DialogueNode NodoSinEnergia;
        public Vector3 position = new(0,0,0);
    }
}