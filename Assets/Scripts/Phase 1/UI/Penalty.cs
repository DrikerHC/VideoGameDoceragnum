using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Penalty : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Canvas _canvas;
    public Canvas Canvas { get; set; }
    public RectTransform RectTransform { get; set; }
    public RectTransform canvasRectTransform { get; set; }

    private string _desc = "";
    [SerializeField] private TextMeshProUGUI _descText;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public string Description
    {
        get { return _desc; }
        set
        {
            _desc = value;
            _descText.text = value;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = RectTransform.anchoredPosition + eventData.delta / Canvas.scaleFactor;
        newPosition = ClampToScreen(newPosition);
        RectTransform.anchoredPosition = newPosition;
    }

    private Vector2 ClampToScreen(Vector2 anchoredPosition)
    {
        Vector2 minPosition = new Vector2(
            -canvasRectTransform.rect.width * 0.5f + RectTransform.rect.width * 0.5f,
            -canvasRectTransform.rect.height * 0.5f + RectTransform.rect.height * 0.5f
        );

        Vector2 maxPosition = new Vector2(
            canvasRectTransform.rect.width * 0.5f - RectTransform.rect.width * 0.5f,
            canvasRectTransform.rect.height * 0.5f - RectTransform.rect.height * 0.5f
        );

        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, minPosition.x, maxPosition.x);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, minPosition.y, maxPosition.y);

        return anchoredPosition;
    }
}
