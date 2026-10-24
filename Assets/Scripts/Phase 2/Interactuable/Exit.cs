using Doceragnum;
using SceneController;
using UnityEngine;

public class Exit : Interactable
{
    public override void Interact()
    {
        InGameDataManager.Data.CurrentPhase = 3;
        GlobalEventHUD.InvokePhaseChanged(InGameDataManager.Data.CurrentPhase);
        SceneController.SceneController.Instance.NewPlan()
            .Unload(SceneRegistry.Slots.Gameplay)
            .Load(SceneRegistry.Slots.Gameplay, SceneRegistry.Scenes.Fase3).WithOverlay().Execute();
    }
    
}
