using UnityEngine;

public abstract class BaseRequisite : ScriptableObject
{
    [Header("Requisite Title"), SerializeField] protected string NameRequisite;
    [Header("Requisite Description"),TextArea(5, 10) , SerializeField] protected string DescRequisite;
    public abstract bool Validate();
    public abstract string GetName();
    public abstract string GetDesc();
    
}
