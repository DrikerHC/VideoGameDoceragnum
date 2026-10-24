using System;
using System.Collections.Generic;
using Bogus;
using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ErrorBirthdate", menuName = "Phase 1/Errors/Birthdate")]
public class ErrorBirthdate : BaseErrorType
{
    private void Awake()
    {
        AppliesTo = new List<F1Manager.DocumentType>(new F1Manager.DocumentType[] { F1Manager.DocumentType.StudentCard });
    }

    public override void Apply(DocumentUI document)
    {
        var faker = new Faker("es");
        switch (document)
        {
            case IdentityCard dni:
                dni.Birthdate = faker.Date.Between(new DateTime(2006, 1, 1), new DateTime(2014, 12, 31));
                break;
            case StudentCardDocument student:
                student.Birthdate = faker.Date.Between(new DateTime(2006, 1, 1), new DateTime(2014, 12, 31));
                break;
        }
    }
}
