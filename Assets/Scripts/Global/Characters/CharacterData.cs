using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "Historia/Data/CharacterInfo")]
    public class CharacterData : ScriptableObject
    {
        [Header("Visitor Info")]
        public string Name;
        public string Surname1;
        public string Surname2;
        public string Gender;
        public Sprite Photo;
        public DateTime Birthdate;
        public int Age => GetAge();

        [Header("DNI Fields")]
        public string DniNumber;
        public DateTime EmisionDate;
        public DateTime ExpirationDate;

        [Header("Center Fields")]
        public string CenterName;
        public string CenterAddress;
        public string CenterCity;

        [Header("Student Fields")]
        public long StudentId;
        public string Grade;

        [Header("Professor Fields")]
        public DateTime ValidityDate;

        public VisitorModel Model;
        public List<DocumentUI> Documents;

        [Header("Node Params")]
        public PuertaNode RootNode;
        //Un par de ejemplos temporales
        public int afectoActual = 0;
        public int afectoMaximo = 100;
        [Serializable]
        public class ConsecuenciasByDay
        {
            public List<BaseConsecuencia> consecuencias;
        }
        [Serializable]
        public class ListaCDia
        {
            public List<ConsecuenciasByDay> listas;
        }

        [Header("Consecuencias")]
        public ListaCDia listaConsecuenciasPorDia;

        [Header("Map Params")]
        public string LocatedInRoom;
        public Vector3 DefaultPosition;
        public Vector3 Position;

        [Header("Visuals")]
        //cosas que tocariamos pa la UI mas bien, ejemplos sin mas
        public Sprite Sprite;
        public Sprite DialogueImage;
        public Color colorDeNombre = Color.white;

        private int GetAge()
        {
            int age = DateTime.Today.Year - Birthdate.Year;
            return age = DateTime.Now < Birthdate.AddYears(age) ? age - 1 : age;
        }
        [Header("Diari Info")]
        public float IsTalkedToday = 0;

        //Aqui pondre una lista de misiones

        public float GetVariable(string nombre)
        {
            // Aquí buscas en tu diccionario o en tus variables fijas
            if (nombre == "Afecto") return afectoActual;
            if (nombre == "Felicidad") return IsTalkedToday;
            if (nombre == "Dia") return InGameDataManager.Data.DayCount;
            if (nombre == "Fase") return InGameDataManager.Data.CurrentPhase;
            return InGameDataManager.Instance.GetProgresoMision(nombre);
        }
    }
}