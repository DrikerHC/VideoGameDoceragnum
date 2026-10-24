using System;
using TMPro;
using UnityEngine;

namespace Doceragnum
{
    public class ProfessorCard : DocumentUI
    {
        private string _name = "";
        [SerializeField] private TextMeshProUGUI _nameText;
        private string _highschool = "";
        [SerializeField] private TextMeshProUGUI _highschoolText;
        private string _city = "";
        [SerializeField] private TextMeshProUGUI _cityText;
        private DateTime _expiration;
        [SerializeField] private TextMeshProUGUI _expirationText;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _nameText.text = value;
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

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                _cityText.text = value;
            }
        }

        public DateTime ExpirationDate
        {
            get { return _expiration; }
            set
            {
                _expiration = value;
                _expirationText.text = "Vàlid fins al " + value.ToString("dd-MM-yyyy");
            }
        }

        public override void InitVisitorData(CharacterData visitorInfo)
        {
            Name = $"{visitorInfo.Name} {visitorInfo.Surname1} {visitorInfo.Surname2}";
            DNI = visitorInfo.DniNumber;
            Highschool = visitorInfo.CenterName;
            City = visitorInfo.CenterCity;
            ExpirationDate = visitorInfo.ValidityDate;
        }
    }
}
