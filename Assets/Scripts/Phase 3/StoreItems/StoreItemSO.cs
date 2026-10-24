using UnityEngine;

[CreateAssetMenu(fileName = "StoreItemSO", menuName = "Scriptable Objects/StoreItemSO")]
public class StoreItemSO : ScriptableObject
{
    [Header("Base Information")]
    public string ItemName;
    public Sprite ItemSprite;
    public string ItemDescription;
    public int ItemPrice;

    public enum BonusType
    {
        HAPPINESS, MONEY, REPUTATION
    };


    [Header("Statistics")] 
    
    public BonusType type;
    public int InitialBonus;
    public float Multiplier;
    public bool isBuyOnce;
    public bool isBought = false;
    public int Frequency;
    public int QuantityPerFrequency;
    public int QuantityConsumed = 0;
    public int LastUse = 0;

    [Header("Mission")] 
    public bool isMission = false;
    public string[] MissionNames;

}
