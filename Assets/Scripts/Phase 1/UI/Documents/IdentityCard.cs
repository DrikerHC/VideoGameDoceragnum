using System;
using TMPro;
using UnityEngine;

namespace Doceragnum
{
    public class IdentityCard : DocumentUI
    {
        private string _surnames = "";
        [SerializeField] private TextMeshProUGUI _surnamesText;
        private string _name = "";
        [SerializeField] private TextMeshProUGUI _nameText;
        private string _gender = "";
        [SerializeField] private TextMeshProUGUI _genderText;
        private DateTime _birthdate;
        [SerializeField] private TextMeshProUGUI _birthdateText;
        private DateTime _emission;
        [SerializeField] private TextMeshProUGUI _emissionText;
        private DateTime _expiration;
        [SerializeField] private TextMeshProUGUI _expirationText;

        public string Surnames
        {
            get { return _surnames; }
            set
            {
                _surnames = value;
                _surnamesText.text = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _nameText.text = value;
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                _genderText.text = value;
            }
        }

        public DateTime Birthdate
        {
            get { return _birthdate; }
            set
            {
                _birthdate = value;
                _birthdateText.text = value.ToString("dd MM yyyy");
            }
        }

        public DateTime EmissionDate
        {
            get { return _emission; }
            set
            {
                _emission = value;
                _emissionText.text = value.ToString("dd MM yyyy");
            }
        }

        public DateTime ExpirationDate
        {
            get { return _expiration; }
            set
            {
                _expiration = value;
                _expirationText.text = value.ToString("dd MM yyyy");
            }
        }

        public override void InitVisitorData(CharacterData visitorInfo)
        {
            DNI = visitorInfo.DniNumber;
            Surnames = visitorInfo.Surname1 + "\n" + visitorInfo.Surname2;
            Name = visitorInfo.Name;
            Gender = visitorInfo.Gender;
            Birthdate = visitorInfo.Birthdate;
            EmissionDate = visitorInfo.EmisionDate;
            ExpirationDate = visitorInfo.ExpirationDate;
        }
    }
}