using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventPressed", menuName = "Phase 1/Events/Pressed")]
public class EventPressed : ScriptableObject
{
    public event Action<bool> OnEvent;
    public void Raise(bool state) => OnEvent?.Invoke(state);
}
