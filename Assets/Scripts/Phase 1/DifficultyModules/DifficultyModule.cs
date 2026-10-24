using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyModule", menuName = "Phase 1/Difficulty Modules/Create Module")]
public class DifficultyModule : ScriptableObject
{
    [Header("Round Time Params")]
    public float InitialMinutes;
    public float OnDecreaseDayInterval;
    public float MinusTimeOnDecrease;
    public float MinMinutes;

    [Header("Error Chance Params")]
    public float BaseChance;
    public float OnIncreaseDayInterval;
    public float PlusChanceOnIncrease;
    public float MaxChance;

    [Header("Error Severity Chances")]
    // Tiers de probabilitats dintre dels documents:
    // - Major Error: Errors clars que son fàcils de detectar. Ex: Tots els números del DNI canviats
    //   Es l'error base que es genera en el cas de no caure ni en el petit ni en el mitjà
    // - Medium Error: Errors mitjanament detectables. Ex: La lletra del DNI canviada
    // - Minor Error: Errors sútils que costen molt més de detectar. Ex: un número del DNI canviat
    // Tots en escala de 10. 1 de 10
    public float MediumErrorChance;
    public float minorErrorChance;

    public float CalculateRoundTime()
    {
        float roundTime = InitialMinutes;
        int decreaseCount = (int)(InGameDataManager.Data.DayCount / OnDecreaseDayInterval);
        float decrease = decreaseCount * MinusTimeOnDecrease;
        return Mathf.Clamp(roundTime - decrease, MinMinutes, int.MaxValue);
    }

    public int CalculateErrorChance()
    {
        float errorChance = BaseChance;
        int increaseCount = (int)(InGameDataManager.Data.DayCount / OnIncreaseDayInterval);
        float increase = increaseCount * PlusChanceOnIncrease;
        return (int)Mathf.Clamp(errorChance + increase, BaseChance, MaxChance);
    }
}
