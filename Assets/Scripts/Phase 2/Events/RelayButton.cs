using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelayButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _image;

    public bool IsActive { get; set; }
    public int ClicksToFix { get; set; }

    public Color RelayColor
    {
        set
        {
            _image.color = value;
        }
    }

    public static event Action<RelayButton> OnRelayClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClicksToFix > 1)
        {
            ClicksToFix--;
        }
        else if (ClicksToFix == 1)
        {
            {
                OnRelayClicked?.Invoke(this);
                ClicksToFix = 0;
                IsActive = false;
            }
        }
    }
}