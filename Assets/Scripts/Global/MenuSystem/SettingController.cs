using Doceragnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class SetingController : MMState
    {
        [SerializeField] private Button _ExitButton;


        private void OnEnable()
        {
            _ExitButton.onClick.AddListener(OnPlayCallback);
        }

        private void OnDisable()
        {
            _ExitButton.onClick.RemoveListener(OnPlayCallback);
        }

        private void OnPlayCallback()
        {
            menuManager.DesactivarMenu();
        }
    }
}