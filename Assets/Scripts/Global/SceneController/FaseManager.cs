using System;
using SceneController;
using UnityEngine;

namespace Doceragnum
{
    public class FaseManager : MonoBehaviour
    {
        public string FaseActual { get; set; } = string.Empty;

        public void Start()
        {
            SceneController.SceneController.Instance.NewPlan()
                .Load(SceneRegistry.Slots.Menu, SceneRegistry.Scenes.MainMenu)
                .Unload(SceneRegistry.Slots.Gameplay)
                .WithOverlay()
                .Execute();
        }

        public void CambiarEscena(string nombreEscena, string unloadScene = null, Action callback = null)
        {
            if (nombreEscena.Contains("Fase")) FaseActual = nombreEscena;

            // 1. Iniciamos el plan
            var plan = SceneController.SceneController.Instance.NewPlan()
                .Unload(unloadScene)
                .Load(SceneRegistry.Slots.Gameplay, nombreEscena)
                .WithOverlay()
                .OnComplete(callback)
                .WithClearUnusedAssets();

            // 2. Ejecutamos el plan
            plan.Execute();
        }
    }
}

// Cosas a guardar para la fase 2:
// - FaseManager FaseActual
// - CharacterPool ActiveNPCsProcedurales
// - RoomManager CurrentRoom
// - Player Position