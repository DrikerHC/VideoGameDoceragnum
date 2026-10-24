using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class ButtonBillController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI StoreItemName;
        [SerializeField] private Button BillButton;
        [SerializeField] private Button BtnPay;
        [SerializeField] public BillSO DataBill;

        private void Awake()
        {
            BtnPay.onClick.AddListener(Pay);
        }

        public void init()
        {
            if (DataBill == null)
                return;

            StoreItemName.text = DataBill.Name;
            BillButton.GetComponent<Image>().sprite = DataBill.Icon;
        }

        private void OnDestroy()
        {
            BtnPay.onClick.RemoveListener(Pay);
        }

        private void Pay()
        {
            InGameDataManager.Instance.PayBill(DataBill);
        }
    }
}