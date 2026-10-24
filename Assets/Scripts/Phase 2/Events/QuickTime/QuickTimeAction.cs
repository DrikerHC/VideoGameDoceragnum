using Bogus;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI.Extensions;

public class QuickTimeAction : MonoBehaviour
{
    private UICircle _uiCircle;
    [SerializeField] private TextMeshProUGUI _actionKeyText;
    [SerializeField] private string[] _possibleLetters;

    private float _totalTime;
    private float _remainingTime;
    private bool _isActive = false;

    private StudentShenanigans _eventManager;

    public void Init(int totalTime, StudentShenanigans eventObj)
    {
        Faker faker = new Faker();
        _uiCircle = GetComponent<UICircle>();
        _isActive = true;
        _eventManager = eventObj;
        _actionKeyText.text = faker.PickRandom(_possibleLetters).ToUpper();
        _totalTime = totalTime;
        _remainingTime = _totalTime;
        Keyboard.current.onTextInput += CheckLetter;
    }

    private void Update()
    {
        if (!_isActive) return;
        _remainingTime -= Time.deltaTime;
        _uiCircle.SetArc(Mathf.InverseLerp(0, _totalTime, _remainingTime));
        if (_remainingTime <= 0)
        {
            Keyboard.current.onTextInput -= CheckLetter;
            if (gameObject != null) Destroy(gameObject);
        }
    }

    private void CheckLetter(char input)
    {
        if (_actionKeyText.text.ToLower()[0].Equals(input))
        {
            Keyboard.current.onTextInput -= CheckLetter;
            _eventManager.LowerCount();
            if (gameObject != null) Destroy(gameObject);
        }
    }
}
