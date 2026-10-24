using System;
using TMPro;
using UnityEngine;

namespace Doceragnum
{
    public class StudentCardDocument : DocumentUI
    {
        private string _name = "";
        [SerializeField] private TextMeshProUGUI _nameText;
        private DateTime _birthdate;
        [SerializeField] private TextMeshProUGUI _birthdateText;
        private long _studentId = 0;
        [SerializeField] private TextMeshProUGUI _idText;
        private string _highschool = "";
        [SerializeField] private TextMeshProUGUI _highschoolText;
        private string _address = "";
        [SerializeField] private TextMeshProUGUI _addressText;
        private string _grade = "";
        [SerializeField] private TextMeshProUGUI _gradeText;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _nameText.text = value;
            }
        }

        public DateTime Birthdate
        {
            get { return _birthdate; }
            set
            {
                _birthdate = value;
                _birthdateText.text = "Data de naixement: " + value.ToString("dd/MM/yyyy");
            }
        }

        public long StudentId
        {
            get { return _studentId; }
            set
            {
                _studentId = value;
                _idText.text = "Id. de l'alumne/a: " + value;
            }
        }

        public string Highschool
        {
            get { return _highschool; }
            set
            {
                _highschool = value;
                _highschoolText.text = value;
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                _addressText.text = value;
            }
        }

        public string Grade
        {
            get { return _grade; }
            set
            {
                _grade = value;
                _gradeText.text = "Ensenyament: " + value;
            }
        }

        public int GetAge()
        {
            int edad = DateTime.Today.Year - _birthdate.Year;

            if (DateTime.Now < _birthdate.AddYears(edad))
            {
                edad--;
            }
            return edad;
        }

        public override void InitVisitorData(CharacterData visitorInfo)
        {
            Name = $"{visitorInfo.Surname1} {visitorInfo.Surname2}, {visitorInfo.Name}";
            DNI = visitorInfo.DniNumber;
            Birthdate = visitorInfo.Birthdate;
            StudentId = visitorInfo.StudentId;
            Highschool = visitorInfo.CenterName;
            Address = $"{visitorInfo.CenterAddress}, {visitorInfo.CenterCity}";
            Grade = visitorInfo.Grade;
        }
    }
}