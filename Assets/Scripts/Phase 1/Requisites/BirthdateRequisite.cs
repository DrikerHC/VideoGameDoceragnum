using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "BirthdateRequisite", menuName = "Phase 1/Requisites/Birthdate")]
public class BirthdateRequisite : BaseRequisite
{
    public override bool Validate()
    {
        var visitor = F1Manager.Instance.CurrentVisitorData;
        var documents = visitor.Documents;
        if (documents.Count == 1 && documents[0] is ProfessorCard)
        {
            return true;
        }
        foreach (var document in documents)
        {
            switch (document)
            {
                case StudentCardDocument student:
                    if (student.Birthdate != visitor.Birthdate)
                    {
                        return false;
                    }
                    break;
                case IdentityCard dni:
                    if (dni.Birthdate != visitor.Birthdate)
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