using Bogus;
using System;
using System.Collections.Generic;
using UnityEngine;
using DocumentType = Doceragnum.F1Manager.DocumentType;
using VisitorType = Doceragnum.RuleSet.VisitorType;

namespace Doceragnum
{
    public class VisitorGenerator : MonoBehaviour
    {
        [SerializeField] private PuertaNode[] _randomDialogues;

        public CharacterData GenerateVisitorInfo()
        {
            var data = ScriptableObject.CreateInstance<CharacterData>();
            var faker = new Faker("es");
            var gender = faker.PickRandom<Bogus.DataSets.Name.Gender>();
            data.Model = faker.PickRandom(InGameDataManager.Data.RuleSets[InGameDataManager.Data.DayCount - 1].VisitorModels);

            data.Name = faker.Name.FirstName(gender);
            data.Surname1 = faker.Name.LastName();
            data.Surname2 = faker.Name.LastName();
            data.Gender = gender == Bogus.DataSets.Name.Gender.Male ? "M" : "F";

            if (data.Model.Type == VisitorType.Student) data.Birthdate = faker.Date.Between(new DateTime(2006, 1, 1), new DateTime(2013, 12, 31));
            else data.Birthdate = faker.Date.Between(new DateTime(1976, 1, 1), new DateTime(2000, 12, 31));
            data.EmisionDate = faker.Date.Recent(365);
            data.ExpirationDate = data.EmisionDate.AddYears(5);

            data.DniNumber = faker.Random.Replace("########?").ToUpper();

            data.CenterName = "Institut Dominguell";
            data.CenterAddress = "C. Juantanal 3";
            data.CenterCity = "Dominguell";

            if (data.Model.Type == VisitorType.Student)
            {
                data.StudentId = long.Parse(faker.Random.Replace("##########"));
                int age = GetAge(data.Birthdate);
                switch (age)
                {
                    case 12:
                    case 13:
                        data.Grade = "1º ESO";
                        break;
                    case 14:
                        data.Grade = "2º ESO";
                        break;
                    case 15:
                        data.Grade = "3º ESO";
                        break;
                    case 16:
                        data.Grade = "4º ESO";
                        break;
                    case 17:
                        data.Grade = "1º " + faker.PickRandom(new[] { "Bachillerato", "SMX" });
                        break;
                    case 18:
                        data.Grade = "2º " + faker.PickRandom(new[] { "Bachillerato", "SMX" });
                        break;
                    case 19:
                        data.Grade = "1º " + faker.PickRandom(new[] { "DAM", "DAMvi", "DAW", "ASIX" });
                        break;
                    default:
                        data.Grade = "2º " + faker.PickRandom(new[] { "DAM", "DAMvi", "DAW", "ASIX" });
                        break;
                }
            }
            if (data.Model.Type == VisitorType.Professor) data.ValidityDate = faker.Date.Future(refDate: faker.Date.Soon(90, DateTime.Now.AddDays(30)), yearsToGoForward: 3);

            List<DocumentUI> docs = new List<DocumentUI>();
            foreach (var docType in data.Model.Documents)
            {
                switch (docType)
                {
                    case DocumentType.StudentCard:
                        StudentCardDocument studentCard = F1Manager.Instance.StudentCardGameObject.GetComponent<StudentCardDocument>();
                        docs.Add(studentCard);
                        break;
                    case DocumentType.DNI:
                        IdentityCard DNI = F1Manager.Instance.IdentityCardGameObject.GetComponent<IdentityCard>();
                        docs.Add(DNI);
                        break;
                    case DocumentType.ProfessorCard:
                        ProfessorCard professorCard = F1Manager.Instance.ProfessorCardGameObject.GetComponent<ProfessorCard>();
                        docs.Add(professorCard);
                        break;
                }
            }
            data.Documents = docs;

            if (data.RootNode == null) data.RootNode = faker.PickRandom(_randomDialogues);


            return data;
        }

        private int GetAge(DateTime birthdate)
        {
            int age = DateTime.Today.Year - birthdate.Year;
            return age = DateTime.Now < birthdate.AddYears(age) ? age - 1 : age;
        }
    }
}
