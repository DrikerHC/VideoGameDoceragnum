using UnityEngine;

[CreateAssetMenu(fileName = "BillSO", menuName = "Scriptable Objects/BillSO")]
public class BillSO : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public string Description;
    
    public float Price;
    public int Frequency;
    [Min(1)] public int DayStartPayment;
    public int DayLastPayment;
}
