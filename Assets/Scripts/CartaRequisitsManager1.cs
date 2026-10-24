using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace Doceragnum
{
    public class CartaRequisitsManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _Titulo;
        [SerializeField] private TextMeshProUGUI _Descripcion;
        void Start()
        {
            _Descripcion.text = "";
            SetPages();
        }

        public void OnEnable()
        {
            SetPages();
        }
        public void OnDisable()
        {
            Debug.Log("Desactivando");
        }

        private void SetPages()
        {
            foreach (var requisito in InGameDataManager.Data.RuleSets[InGameDataManager.Data.DayCount-1].RequisitesOfToday)
            {
                _Descripcion.text += ", " + requisito.GetName();
            }

            _Titulo.text = "Requisitos dia " + InGameDataManager.Data.DayCount.ToString();
            
        }

    }
}


