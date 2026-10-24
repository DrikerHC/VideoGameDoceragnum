using System.Collections.Generic;
using Bogus;
using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "ErrorDNI", menuName = "Phase 1/Errors/DNI")]
    public class ErrorDNI : BaseErrorType
    {
        private void Awake()
        {
            AppliesTo = new List<F1Manager.DocumentType>(new F1Manager.DocumentType[] { F1Manager.DocumentType.DNI, F1Manager.DocumentType.StudentCard, F1Manager.DocumentType.ProfessorCard });
        }
        public override void Apply(DocumentUI document)
        {
            int num = Random.Range(0, 10);
            DifficultyModule diffModule = F1Manager.Instance.CurrentDiffModule;
            if (num < diffModule.minorErrorChance)
            {
                int randPos = Random.Range(0, 8);
                document.DNI = document.DNI.Insert(randPos, Random.Range(0, 10).ToString());
                document.DNI = document.DNI.Remove(randPos + 1);
            }
            if (diffModule.minorErrorChance <= num && num < (diffModule.minorErrorChance + diffModule.MediumErrorChance))
            {
                document.DNI = document.DNI.Remove(document.DNI.Length - 1);
                document.DNI += (char)Random.Range('A', 'Z');
            }
            else
            {
                var faker = new Faker("es");
                document.DNI = faker.Random.Replace("########?").ToUpper();
            }
        }
    }
}