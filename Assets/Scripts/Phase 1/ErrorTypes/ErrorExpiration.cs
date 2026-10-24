using System.Collections.Generic;
using Bogus;
using Doceragnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ErrorExpiration", menuName = "Phase 1/Errors/Expiration")]
public class ErrorExpiration : BaseErrorType
{
    private void Awake()
    {
        AppliesTo = new List<F1Manager.DocumentType>(new F1Manager.DocumentType[] { F1Manager.DocumentType.DNI, F1Manager.DocumentType.ProfessorCard });
    }

    public override void Apply(DocumentUI document)
    {
        var faker = new Faker("es");
        switch (document)
        {
            case IdentityCard dni:
                dni.ExpirationDate = faker.Date.Recent(days: 5, refDate: System.DateTime.Today.AddDays(-1));
                dni.EmissionDate = dni.ExpirationDate.AddYears(-5);
                break;
            case ProfessorCard professor:
                professor.ExpirationDate = faker.Date.Recent(days: 5, refDate: System.DateTime.Today.AddDays(-1));
                break;
        }
    }
}