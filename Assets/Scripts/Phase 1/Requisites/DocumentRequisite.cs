using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "DocumentRequisite", menuName = "Phase 1/Requisites/Document")]
public class DocumentRequisite : BaseRequisite
{
    public override bool Validate()
    {
        var visitorDocs = F1Manager.Instance.CurrentVisitorData.Documents;
        var requiredDocs = F1Manager.Instance.CurrentVisitorData.Model.Documents;
        if (visitorDocs.Count != requiredDocs.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
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