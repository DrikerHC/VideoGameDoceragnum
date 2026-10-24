using Doceragnum;
using SceneController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    [SerializeField] private GameObject _menuUI;

    public bool CanBeOpened { get; set; } = true;
    public bool IsOpen { get; set; } = false;

    [SerializeField] private InputActionReference _menuRef;

    private void Awake()
    {
        Instance = this;
        _menuUI.SetActive(false);
    }

    private void OnEnable()
    {
        _menuRef.action.started += InteractMenu;
    }

    private void OnDisable()
    {
        _menuRef.action.started -= InteractMenu;
    }

    private void InteractMenu(InputAction.CallbackContext context)
    {
        if (CanBeOpened)
        {
            if (!IsOpen)
            {
                IsOpen = true;
                Time.timeScale = 0;
                _menuUI.SetActive(true);
                
            }
            else if (IsOpen)
            {
                IsOpen = false;
                Time.timeScale = 1;
                _menuUI.SetActive(false);
            }
        }
    }

    public void CloseMenu()
    {
        InteractMenu(new InputAction.CallbackContext());
    }

    public void Save()
    {
        InGameDataManager.Instance.SaveGame();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneController.SceneController.Instance.NewPlan()
            .Unload(SceneRegistry.Slots.Gameplay)
            .Unload(SceneRegistry.Slots.PauseMenu)
            .Load(SceneRegistry.Slots.Menu, SceneRegistry.Scenes.MainMenu)
            .WithOverlay()
            .WithClearUnusedAssets()
            .Execute();
    }
}