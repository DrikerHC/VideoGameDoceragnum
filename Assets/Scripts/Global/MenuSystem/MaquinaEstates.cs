using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    public class MaquinaEstate<TState, TController> where TState : Enum
    {
        private Dictionary<TState, TController> _stateControllers = new Dictionary<TState, TController>();
        private TState _currentState;
        private TController _currentController;

        public void Initialize(TState initialState, Dictionary<TState, TController> controllers)
        {
            _stateControllers = controllers;
            ChangeState(initialState);
        }
        public void Reset()
        {
            foreach (var controller in _stateControllers)
            {
                if (controller.Value is MonoBehaviour newMonoBehaviour)
                {
                    newMonoBehaviour.gameObject.SetActive(false);
                }
            }
        }


        public void ChangeState(TState newState)
        {
            // Desactivar el controlador actual (GameObject del UI)
            Debug.Log("here");
            if (_currentController is MonoBehaviour currentMonoBehaviour)
            {
                currentMonoBehaviour.gameObject.SetActive(false); // Desactiva el GameObject
            }

            _currentState = newState;
            _currentController = _stateControllers[newState];

            // Activar el nuevo controlador (GameObject del UI)
            if (_currentController is MonoBehaviour newMonoBehaviour)
            {
                newMonoBehaviour.gameObject.SetActive(true); // Activa el GameObject
            }
        }

        public TState GetCurrentState() => _currentState;
        public TController GetCurrentController() => _currentController;
    }
}