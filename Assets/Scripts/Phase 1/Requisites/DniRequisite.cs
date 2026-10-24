using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "DniRequisite", menuName = "Phase 1/Requisites/Dni")]
public class DniRequisite : BaseRequisite
{
    public override bool Validate()
    {
        
        var visitor = F1Manager.Instance.CurrentVisitorData;
        var documents = visitor.Documents;
        foreach (var doc in documents)
        {
            if (doc.DNI != visitor.DniNumber)
            {
                return false;
            }
        }
        return true;
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