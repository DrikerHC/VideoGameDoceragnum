using System;
using System.Collections.Generic;
using System.Linq;
using Phase_3;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public enum MenuState
    {
        Apagado,
        Login,
        Home,
        TiendaJuegos,
        VideoJoc,
        Store,
        Bills,
        StoreItem,
        Settings,
        MainMenu
    }

    [Serializable]
    public abstract class MMState : MonoBehaviour
    {
        protected MenuStateManager menuManager;

        public virtual void Initialize(MenuStateManager manager)
        {
            menuManager = manager;
        }
    }


    public class MenuStateManager : MonoBehaviour
    {
        public MenuState initialState;
        [SerializeField] private Button _computerExitButton;
        private MaquinaEstate<MenuState, MMState> stateMachine;
        private Stack<MenuState> stateHistory = new Stack<MenuState>();
        private Dictionary<MenuState, MMState> controllers;
        private MMState[] menuDocuments;
        private F3Manager _f3Manager;
        private bool _subscribedToF3Manager;

        private void Awake()
        {
            menuDocuments = GetComponentsInChildren<MMState>(true);
            controllers = new Dictionary<MenuState, MMState>();

            foreach (var doc in menuDocuments)
            {
                if (doc.gameObject.TryGetComponent(out MMState mmstate))
                {
                    mmstate.Initialize(this);
                }

                if (Enum.TryParse<MenuState>(doc.name, out var state))
                {
                    controllers[state] = doc;
                    // No manipules la visibilidad aqu�, eso lo haremos en la m�quina de estados
                    doc.gameObject.SetActive(false); // Desactiva inicialmente
                }
            }

            stateMachine = new MaquinaEstate<MenuState, MMState>();
            TrySubscribeToF3Manager();

            if (_computerExitButton != null)
                _computerExitButton.onClick.AddListener(ExitComputerView);
        }

        private void Start()
        {
            TrySubscribeToF3Manager();
        }

        private void TrySubscribeToF3Manager()
        {
            if (_subscribedToF3Manager)
                return;

            _f3Manager = F3Manager.Instance;
            if (_f3Manager == null)
                return;

            _f3Manager.OnInitCompSm += InitializeStateMachine;
            _f3Manager.OnResetCompSm += DisconnectStateMachine;
            _subscribedToF3Manager = true;
        }

        public void InitializeStateMachine()
        {
            stateMachine.Initialize(initialState, controllers);
        }

        public void DisconnectStateMachine()
        {
            LoginController loginController = menuDocuments.Where(s => s is LoginController).Cast<LoginController>().FirstOrDefault();
            loginController?.ResetUserName();
            stateMachine.Reset();
        }

        private void ExitComputerView()
        {
            F3Manager.Instance?.ExitComputerView();
        }



        public void ChangeMenu(MenuState newState)
        {
            if (stateMachine.GetCurrentState() != newState)
            {
                stateHistory.Push(stateMachine.GetCurrentState());
            }

            stateMachine.ChangeState(newState);
        }

        public void PreviousMenu()
        {
            if (stateHistory.Count > 0)
            {
                var previousState = stateHistory.Pop();
                stateMachine.ChangeState(previousState);
            }
            else
            {
                Debug.LogWarning("No previous state");
            }
        }
        
        private void OnDestroy()
        {
            if (_computerExitButton != null)
                _computerExitButton.onClick.RemoveListener(ExitComputerView);

            if (!_subscribedToF3Manager || _f3Manager == null)
                return;

            _f3Manager.OnInitCompSm -= InitializeStateMachine;
            _f3Manager.OnResetCompSm -= DisconnectStateMachine;
            _subscribedToF3Manager = false;
        }

        public void DesactivarMenu()
        {
            stateMachine.Reset();
            this.gameObject.SetActive(false);
        }

    }
}