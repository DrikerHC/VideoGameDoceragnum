using Doceragnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class LoginController : MMState
    {
        [SerializeField] private Button _LoginButton;
        public TMP_InputField UserNameInput;


        private void OnEnable()
        {
            _LoginButton.onClick.AddListener(OnPlayCallback);
        }

        private void OnDisable()
        {
            _LoginButton.onClick.RemoveListener(OnPlayCallback);
        }

        private void OnPlayCallback()
        {
            menuManager.ChangeMenu(MenuState.Home);
        }
        public void ResetUserName()
        {
            UserNameInput.text = string.Empty;
        }
    }
}