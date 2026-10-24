using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class BillController : MMState
    {
        [SerializeField] private Button VideoJocsButton;
        [SerializeField] private BillSO[] _bills;

        [SerializeField] private ButtonBillController[] _IconosBills;

        private void OnEnable()
        {
            VideoJocsButton.onClick.AddListener(OnPlayCallback);

            for (int i = 0; i < _IconosBills.Length && i < _bills.Length; i++)
            {
                _IconosBills[i].DataBill = _bills[i];
                _IconosBills[i].init();
            }
        }

        private void OnDisable()
        {
            VideoJocsButton.onClick.RemoveListener(OnPlayCallback);
        }

        private void OnPlayCallback()
        {
            print("back");
            menuManager.ChangeMenu(MenuState.Home);
        }
    }
}