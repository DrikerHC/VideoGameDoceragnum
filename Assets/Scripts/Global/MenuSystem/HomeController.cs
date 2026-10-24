using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class HomeController : MMState
    {
        [SerializeField] private Button VideoJocsButton;
        [SerializeField] private Button StoreButton;
        [SerializeField] private Button BillButton;

        [Header("Bill Due Notification")]
        [SerializeField] private GameObject _billNotificationPanel;
        [SerializeField] private TextMeshProUGUI _billNotificationText;
        [SerializeField] private Button _dismissBillNotificationButton;
        [SerializeField] private GameObject _billBadge;
        [SerializeField] private TextMeshProUGUI _billBadgeText;

        private bool _billNotificationDismissed;

        private void OnEnable()
        {
            _billNotificationDismissed = false;
            RefreshBillNotification();

            VideoJocsButton.onClick.AddListener(OnPlayCallback);
            StoreButton.onClick.AddListener(OnShowStore);
            BillButton.onClick.AddListener(OnShowBills);

            if (_dismissBillNotificationButton != null)
                _dismissBillNotificationButton.onClick.AddListener(DismissBillNotification);
        }

        private void OnDisable()
        {
            VideoJocsButton.onClick.RemoveListener(OnPlayCallback);
            StoreButton.onClick.RemoveListener(OnShowStore);
            BillButton.onClick.RemoveListener(OnShowBills);

            if (_dismissBillNotificationButton != null)
                _dismissBillNotificationButton.onClick.RemoveListener(DismissBillNotification);
        }

        private void DismissBillNotification()
        {
            _billNotificationDismissed = true;

            if (_billNotificationPanel != null)
                _billNotificationPanel.SetActive(false);
        }

        private void RefreshBillNotification()
        {
            if (_billNotificationPanel == null)
                return;

            List<BillSO> payableBills = InGameDataManager.GetPayableUnpaidBills();
            bool hasPayableBills = payableBills.Count > 0;

            _billNotificationPanel.SetActive(hasPayableBills && !_billNotificationDismissed);

            if (_billBadge != null)
                _billBadge.SetActive(hasPayableBills);

            if (!hasPayableBills)
                return;

            var message = new StringBuilder("Facturas pendientes:");
            foreach (var bill in payableBills)
            {
                int amount = InGameDataManager.GetBillPaymentAmount(bill);
                message.AppendLine().Append("• ").Append(bill.Name).Append(" — ").Append(amount).Append(" x");
            }

            if (_billNotificationText != null)
                _billNotificationText.text = message.ToString();

            if (_billBadgeText != null)
                _billBadgeText.text = payableBills.Count.ToString();
        }

        private void OnShowBills()
        {
            menuManager.ChangeMenu(MenuState.Bills);
        }

        private void OnShowStore()
        {
            print("hereee");
            menuManager.ChangeMenu(MenuState.Store);
        }

        private void OnPlayCallback()
        {
            menuManager.ChangeMenu(MenuState.TiendaJuegos);
        }
    }
}
