using Doceragnum;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StudentIdRequisite", menuName = "Phase 1/Requisites/StudentId")]
public class StudentIdRequisite : BaseRequisite
{
    public override bool Validate()
    {
        var visitor = F1Manager.Instance.CurrentVisitorData;
        var documents = visitor.Documents;
        if (!documents.OfType<StudentCardDocument>().Any())
        {
            return true;
        }
        foreach (var document in documents)
        {
            switch (document)
            {
                case StudentCardDocument student:
                    if (student.StudentId != visitor.StudentId)
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