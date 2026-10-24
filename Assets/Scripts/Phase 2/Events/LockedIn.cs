using System;
using Doceragnum;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockedIn : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject _minigamePanel;
    [SerializeField] private RectTransform _keyTransform;
    [SerializeField] private InputActionReference _interactRef;
    public Action OnGameFinished { get; set; }
    private bool _canInteract = true;

    private Sequence _oscillationTween;
    private bool _isHeadingUp = true;

    private void OnDisable()
    {
        _interactRef.action.started -= OnInteract;
    }

    public void StartGame()
    {
        Player.Instance.IsLocked = true;
        Player.Instance.InsideMinigame = true;
        _minigamePanel.SetActive(true);
        _oscillationTween = Sequence.Create(-1, Sequence.SequenceCycleMode.Yoyo)
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 800, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(false)))
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 150, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(true)));
        _interactRef.action.started += OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!_canInteract) return;
        _canInteract = false;
        _oscillationTween.Stop();
        Tween.UIAnchoredPositionX(_keyTransform, 1035, 1, Ease.OutCirc).OnComplete(target: this, target => target.CheckIfKeyIsCorrect());
    }

    private void CheckIfKeyIsCorrect()
    {
        Debug.Log($"Posición Y: {_keyTransform.anchoredPosition.y}");
        if (365 <= _keyTransform.anchoredPosition.y && _keyTransform.anchoredPosition.y <= 415)
        {
            Finish();
        }
        else
        {
            Tween.UIAnchoredPositionX(_keyTransform, 500, 1, Ease.InCubic).OnComplete(target: this, target => target.ResumeOscillation());
        }
    }

    private void Finish()
    {
        _minigamePanel.SetActive(false);
        Player.Instance.IsLocked = false;
        Player.Instance.InsideMinigame = false;
        _canInteract = true;
        _isHeadingUp = true;
        _keyTransform.anchoredPosition = new Vector2(500, 150);
        OnGameFinished?.Invoke();
    }

    private void ResumeOscillation()
    {
        if (_isHeadingUp)
        {
            Tween.UIAnchoredPositionY(_keyTransform, 800, 0.5f, Ease.InOutQuad).OnComplete(target: this, target => target.OscillationDown());
        }
        else
        {
            Tween.UIAnchoredPositionY(_keyTransform, 150, 0.5f, Ease.InOutQuad).OnComplete(target: this, target => target.OscillationUp());
        }
    }

    private void SwitchDirection(bool direction)
    {
        _isHeadingUp = direction;
    }

    private void OscillationDown()
    {
        SwitchDirection(false);
        _oscillationTween = Sequence.Create(-1, Sequence.SequenceCycleMode.Yoyo)
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 150, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(true)))
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 800, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(false)));
        _canInteract = true;
    }

    private void OscillationUp()
    {
        SwitchDirection(true);
        _oscillationTween = Sequence.Create(-1, Sequence.SequenceCycleMode.Yoyo)
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 800, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(false)))
            .Chain(Tween.UIAnchoredPositionY(_keyTransform, 150, 1, Ease.InOutQuad).OnComplete(target: this, target => target.SwitchDirection(true)));
        _canInteract = true;
    }
}