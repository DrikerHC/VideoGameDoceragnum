using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private EventPressed _onPress;
    [SerializeField] private bool _doesAccept;

    public void OnPointerClick(PointerEventData eventData)
    {
        _onPress.Raise(_doesAccept);
    }
}
