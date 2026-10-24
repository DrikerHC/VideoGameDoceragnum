using SceneController;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using ZLinq;

namespace Doceragnum
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsMenu;
        [SerializeField] private TMP_Dropdown _languagesDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        private void Awake()
        {
            _fullscreenToggle.isOn = Screen.fullScreen;
        }

        private void Start()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales.AsValueEnumerable().Select(l => l.LocaleName).ToList();

            _languagesDropdown.AddOptions(locales);
            _languagesDropdown.value = locales.IndexOf(LocalizationSettings.SelectedLocale.LocaleName);
            _languagesDropdown.RefreshShownValue();

            _languagesDropdown.onValueChanged.AddListener(SwitchLanguage);
        }

        private void SwitchLanguage(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }

        private void OnDestroy()
        {
            _languagesDropdown.onValueChanged.RemoveListener(SwitchLanguage);
        }

        public void startPartida()
        {
            InGameDataManager.Instance.ResetToNewGame();
            var plan = SceneController.SceneController.Instance.NewPlan()
                .Unload(SceneRegistry.Slots.Menu)
                .Load(SceneRegistry.Slots.HUD, SceneRegistry.Scenes.HUD)
                .Load(SceneRegistry.Slots.PauseMenu, SceneRegistry.Scenes.PauseMenu, false)
                .Load(SceneRegistry.Slots.Gameplay, SceneRegistry.Scenes.Fase1)
                .WithOverlay()
                .WithClearUnusedAssets();

            plan.Execute();
        }

        public void loadPartida()
        {
            InGameDataManager.Instance.LoadGame();

            var plan = SceneController.SceneController.Instance.NewPlan()
                .Unload(SceneRegistry.Slots.Menu)
                .Load(SceneRegistry.Slots.HUD, SceneRegistry.Scenes.HUD)
                .Load(SceneRegistry.Slots.PauseMenu, SceneRegistry.Scenes.PauseMenu, false)
                .Load(SceneRegistry.Slots.Gameplay, "Fase" + InGameDataManager.Data.CurrentPhase)
                .WithOverlay()
                .WithClearUnusedAssets().OnComplete(() =>
                {
                    if (InGameDataManager.Data.CurrentPhase == 2)
                        RoomManager.Instance.Init();
                });
            plan.Execute();
        }

        public void OpenSettings()
        {
            _settingsMenu.SetActive(true);
        }

        public void CloseSettings()
        {
            _settingsMenu.SetActive(false);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}