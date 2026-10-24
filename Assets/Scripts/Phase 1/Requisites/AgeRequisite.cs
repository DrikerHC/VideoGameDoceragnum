using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "AgeRequisite", menuName = "Phase 1/Requisites/Age")]
public class AgeRequisite : BaseRequisite
{
    public int Age; 
    public override bool Validate()
    {
        int VisitorAge = F1Manager.Instance.CurrentVisitorData.Age;
        return VisitorAge >= Age;
    }
    public override string GetName()
    {
        return  NameRequisite;
    }

    public override string GetDesc()
    {
        return  DescRequisite;
    }
}
