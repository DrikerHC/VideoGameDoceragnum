using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Historia/Data/GameInfo")]
    public class GameData : ScriptableObject
    {
        public enum Difficulty
        {
            Normal,
            Hard
        }

        public Difficulty difficulty;

        public int Seed;
        public int NpcCount;

        public int DayCount;
        public int CurrentPhase = 1;
        public RuleSet[] RuleSets;
        public InventorySO Inventory;
        public int LasDayPlayed = 0;

        [Header("New Game starting values")]
        public int diaInicial = 1;
        public int faseInicial = 1;
    }
}