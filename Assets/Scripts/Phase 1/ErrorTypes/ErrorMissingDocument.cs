using System.Collections.Generic;
using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ErrorMissingDocument", menuName = "Phase 1/Errors/MissingDocument")]
public class ErrorMissingDocument : BaseErrorType
{
    private void Awake()
    {
        AppliesTo = new List<F1Manager.DocumentType>(new F1Manager.DocumentType[] { F1Manager.DocumentType.DNI, F1Manager.DocumentType.StudentCard, F1Manager.DocumentType.ProfessorCard });
    }

    public override void Apply(DocumentUI document)
    {
        var visitorDocs = F1Manager.Instance.CurrentVisitorData.Documents;
        visitorDocs.Remove(document);
    }
}