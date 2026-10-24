using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance;

        public PlayerData player; // Arrastramos aquí nuestro SO de Jugador
        public int diaActual;
        public List<string> flagsMundo;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Opcional: DontDestroyOnLoad(gameObject); si quieres que persista entre escenas
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /*
        public void EjecutarConsecuencia(BaseConsecuencia cons, CharacterData npcActual)
        {
            cons.Ejecutar(npcActual, player, this);
        }
        */
    }
}