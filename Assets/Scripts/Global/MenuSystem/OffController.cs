using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class OffController : MMState
    {
        [SerializeField] private Button _powerOnButton;

        private void OnEnable()
        {
            _powerOnButton.onClick.AddListener(OnPowerOn);
        }

        private void OnDisable()
        {
            _powerOnButton.onClick.RemoveListener(OnPowerOn);
        }

        private void OnPowerOn()
        {
            menuManager.ChangeMenu(MenuState.Login);
        }
    }
}
