using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "NameRequisite", menuName = "Phase 1/Requisites/Name")]
public class NameRequisite : BaseRequisite
{
    
    public override bool Validate()
    {
        var visitor = F1Manager.Instance.CurrentVisitorData;
        var documents = visitor.Documents;
        foreach (var document in documents)
        {
            switch (document)
            {
                case StudentCardDocument student:
                    string visitorName = $"{visitor.Surname1} {visitor.Surname2}, {visitor.Name}";
                    if (student.Name != visitorName)
                    {
                        return false;
                    }
                    break;
                case IdentityCard dni:
                    string surnames = visitor.Surname1 + "\n" + visitor.Surname2;
                    if (dni.Surnames != surnames || dni.Name != visitor.Name)
                    {
                        return false;
                    }
                    break;
                case ProfessorCard professor:
                    string professorName = $"{visitor.Name} {visitor.Surname1} {visitor.Surname2}";
                    if (professor.Name != professorName)
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