using System;
using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpirationRequisite", menuName = "Phase 1/Requisites/Expiration Date")]
public class ExpirationRequisite : BaseRequisite
{
    private DateTime _nowDate = DateTime.Now;
    
    public override bool Validate()
    {
        var documents = F1Manager.Instance.CurrentVisitorData.Documents;
        if (documents.Count == 1 && documents[0] is StudentCardDocument)
        {
            return true;
        }
        foreach (var doc in documents)
        {
            switch (doc)
            {
                case IdentityCard dni:
                    if (_nowDate > dni.ExpirationDate)
                    {
                        return false;
                    }
                    break;
                case ProfessorCard professor:
                    if (_nowDate > professor.ExpirationDate)
                    {
                        return false;
                    }
                    break;
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
