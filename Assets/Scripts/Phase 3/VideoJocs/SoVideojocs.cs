using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevoVideojuego", menuName = "Historia/Videojuego")]
    public class VideojuegoSO : ScriptableObject
    {
        [Header("Informacion Basica")]
        public string nombreDelVideojuego;
        public Sprite SpriteVideojuego;
        public string DescripcionJuego;
        public int PrecioJuego;
        public bool isBought = false;



        [Header("Estadasticas Relacionadas")]
        public int BonusInicial;
        [Range(0f, 1f)]
        public float DecreaseDiario;
        public float BonusActual;
        public int DiasJugados;
        public int DiasJugables;
    }
}
