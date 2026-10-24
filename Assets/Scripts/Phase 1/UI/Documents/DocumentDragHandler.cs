using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Phase_1.UI
{
    [Serializable]
    public struct DocumentElement
    {
        public Transform sprite;
        public RectTransform UI;
    }

    public class DocumentDragHandler : MonoBehaviour
    {
        [SerializeField] private DocumentElement[] _documentElements;
        private Dictionary<Transform, RectTransform> _docsDict = new Dictionary<Transform, RectTransform>();

        [Header("Manual"), SerializeField] private Transform _manualSprite;
        [SerializeField] private RectTransform _manualUI;

        [Header("Letter"), SerializeField] private Transform _letterSprite;
        [SerializeField] private RectTransform _letterUI;

        [Header("Sprite Safe Zones"), SerializeField] private Collider2D deskSafeZone;
        [SerializeField] private Collider2D _manualZone;
        [SerializeField] private GraphicRaycaster _raycaster;

        [Header("Input References"), SerializeField] private InputActionReference _mouseClickRef;
        [SerializeField] private InputActionReference _mousePosRef;

        private Vector2 _mousePosition;

        public enum DraggableType { NULL, Sprite, UI }
        public DraggableType CurrentDraggable { get; set; } = DraggableType.NULL;

        private Camera _mainCamera;

        [SerializeField] private Canvas _canvas;
        private RectTransform _canvasRectTransform;

        private bool _isDragging = false;
        private bool _isManual = false;
        private bool _isLetter = false;

        private Transform _dragged;
        private RectTransform _UIdragged;
        private Vector3 _offset;
        private Vector2 _previousMousePosition;

        private int _defaultLayer;
        [SerializeField] private LayerMask _spriteMask;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _canvasRectTransform = _canvas.GetComponent<RectTransform>();
            foreach (DocumentElement doc in _documentElements)
            {
                _docsDict.Add(doc.sprite, doc.UI);
            }
            _defaultLayer = LayerMask.NameToLayer("Default");
        }

        private void Update()
        {
            // Posició del ratolí en el frame actual
            _mousePosition = _mousePosRef.action.ReadValue<Vector2>();
            if (_mouseClickRef.action.IsPressed())
            {
                // Primer cop que es clica un element, per tant, al començar a arrastrar-lo
                if (!_isDragging)
                {
                    TryDrag();
                }
                // S'arrastra l'element
                else
                {
                    Drag();
                }
            }
            else
            {
                // Si al frame anterior s'estava clicant, significa que deixem d'arrastrar-lo en aquest frame
                if (_isDragging)
                {
                    _isDragging = false;
                    CurrentDraggable = DraggableType.NULL;
                    _dragged = null;
                    _UIdragged = null;
                }
            }
        }

        // Try drag te dues comprovacions, si es clica a sobre d'un element UI, o si es clica a sobre d'un GameObject
        private void TryDrag()
        {
            // Informació sobre el punter del ratolí, necessària per al Graphic Raycaster
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = _mousePosition
            };
            var results = new List<RaycastResult>();
            // Realitza un Raycast gràfic per detectar elements UI
            _raycaster.Raycast(pointerData, results);

            // Eliminar tots els elements UI que siguin de capa Default
            for (int i = results.Count - 1; i >= 0; i--) if (results[i].gameObject.layer == _defaultLayer) results.RemoveAt(i);

            // Si hi ha presents elements UI a on es troba el ratolí
            if (results.Count > 0)
            {
                // Switch per mirar a quina capa es troba el primer element UI
                switch (LayerMask.LayerToName(results[0].gameObject.layer))
                {
                    case "Document":
                        CurrentDraggable = DraggableType.UI;
                        _UIdragged = results[0].gameObject.GetComponent<RectTransform>();
                        results[0].gameObject.transform.SetAsLastSibling();
                        _previousMousePosition = _mousePosition;
                        _isDragging = true;
                        _isManual = false;
                        _isLetter = false;
                        break;
                    case "Manual":
                        CurrentDraggable = DraggableType.UI;
                        _UIdragged = results[0].gameObject.GetComponent<RectTransform>();
                        results[0].gameObject.transform.SetAsLastSibling();
                        _previousMousePosition = _mousePosition;
                        _isDragging = true;
                        _isManual = true;
                        _isLetter = false;
                        break;
                    case "Letter":
                        CurrentDraggable = DraggableType.UI;
                        _UIdragged = results[0].gameObject.GetComponent<RectTransform>();
                        results[0].gameObject.transform.SetAsLastSibling();
                        _previousMousePosition = _mousePosition;
                        _isDragging = true;
                        _isManual = false;
                        _isLetter = true;
                        break;
                }
            }
            // En el cas de no trobar cap element UI a sobre el ratolí, es comprova si hi ha GameObjects presents
            else
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(_mainCamera.ScreenToWorldPoint(_mousePosition), Vector2.zero, Mathf.Infinity, _spriteMask);
                var topSprite = hits.OrderBy(h => h.transform.position.y).FirstOrDefault();

                if (topSprite.collider != null)
                {
                    switch (LayerMask.LayerToName(topSprite.transform.gameObject.layer))
                    {
                        case "Document":
                            CurrentDraggable = DraggableType.Sprite;
                            _dragged = topSprite.transform;
                            _offset = _dragged.position - _mainCamera.ScreenToWorldPoint(_mousePosition);
                            _offset.z = 0;
                            _isDragging = true;
                            _isManual = false;
                            _isLetter = false;
                            break;
                        case "Manual":
                            CurrentDraggable = DraggableType.Sprite;
                            _dragged = topSprite.transform;
                            _offset = _dragged.position - _mainCamera.ScreenToWorldPoint(_mousePosition);
                            _offset.z = 0;
                            _isDragging = true;
                            _isManual = true;
                            _isLetter = false;
                            break;
                        case "Letter":
                            CurrentDraggable = DraggableType.Sprite;
                            _dragged = topSprite.transform;
                            _offset = _dragged.position - _mainCamera.ScreenToWorldPoint(_mousePosition);
                            _offset.z = 0;
                            _isDragging = true;
                            _isManual = false;
                            _isLetter = true;
                            break;
                    }
                }
            }
        }

        // Acció d'arrastrar l'element
        private void Drag()
        {
            if (CurrentDraggable == DraggableType.Sprite)
            {
                // La posició a on arrastrar el GameObject es calcula agafant:
                // - La World Position del ratolí
                // - Més l'offset que hi ha entre el ratolí i el centre de l'objecte
                var newPos = _mainCamera.ScreenToWorldPoint(_mousePosition) + _offset;
                newPos.z = 0;
                _dragged.position = newPos;

                if (_isManual || _isLetter)
                {
                    if (!IsMouseInsideManualZone())
                    {
                        SwitchDraggable(DraggableType.UI);
                    }
                }
                else
                {
                    if (!IsMouseInsideSafeZone())
                    {
                        SwitchDraggable(DraggableType.UI);
                    }
                }
            }
            else if (CurrentDraggable == DraggableType.UI)
            {
                // La posició a on arrastrar l'element UI es calcula amb:
                // - La posició anterior de l'element
                // - La diferència de posicions del ratolí en el frame anterior i l'actual
                // - El factor d'escala del Canvas, que s'utilitza per escalar a tots els
                //   elements UI per igual
                Vector2 delta = _mousePosition - _previousMousePosition;
                Vector2 newPos = _UIdragged.anchoredPosition + delta / _canvas.scaleFactor;
                // Funció que s'assegura que l'element UI no es surti de la pantalla
                ClampToScreen(ref newPos, _UIdragged);
                _UIdragged.anchoredPosition = newPos;

                _previousMousePosition = _mousePosition;

                if (_isManual || _isLetter)
                {
                    if (IsMouseInsideManualZone())
                    {
                        SwitchDraggable(DraggableType.Sprite);
                    }
                }
                else
                {
                    if (IsMouseInsideSafeZone())
                    {
                        SwitchDraggable(DraggableType.Sprite);
                    }
                }
            }
        }

        private void SwitchDraggable(DraggableType type)
        {
            CurrentDraggable = type;
            switch (CurrentDraggable)
            {
                case DraggableType.Sprite:
                    if (_isManual) _dragged = _manualSprite;
                    else if (_isLetter) _dragged = _letterSprite;
                    else _dragged = _docsDict.GetKey(_UIdragged);
                    _UIdragged.gameObject.SetActive(false);
                    _dragged.gameObject.SetActive(true);
                    _offset = Vector3.zero;
                    break;
                case DraggableType.UI:
                    _previousMousePosition = _mousePosition;
                    if (_isManual) _UIdragged = _manualUI;
                    else if (_isLetter) _UIdragged = _letterUI;
                    else _UIdragged = _docsDict[_dragged];
                    _dragged.gameObject.SetActive(false);
                    _UIdragged.transform.SetAsLastSibling();
                    _UIdragged.gameObject.SetActive(true);
                    _UIdragged.position = _mainCamera.WorldToScreenPoint(_dragged.position);
                    break;
                default:
                    break;
            }
        }

        private bool IsMouseInsideSafeZone()
        {
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(_mousePosition);
            return deskSafeZone.OverlapPoint(worldPos);
        }

        private bool IsMouseInsideManualZone()
        {
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(_mousePosition);
            return _manualZone.OverlapPoint(worldPos);
        }

        private void ClampToScreen(ref Vector2 Position, RectTransform UIrect)
        {
            Vector2 minPosition = new Vector2(
                -_canvasRectTransform.rect.width * 0.5f + UIrect.rect.width * 0.5f,
                -_canvasRectTransform.rect.height * 0.5f + UIrect.rect.height * 0.5f
            );

            Vector2 maxPosition = new Vector2(
                _canvasRectTransform.rect.width * 0.5f - UIrect.rect.width * 0.5f,
                _canvasRectTransform.rect.height * 0.5f - UIrect.rect.height * 0.5f
            );

            Position.x = Mathf.Clamp(Position.x, minPosition.x, maxPosition.x);
            Position.y = Mathf.Clamp(Position.y, minPosition.y, maxPosition.y);
        }
    }
}