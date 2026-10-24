using Doceragnum;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StudentShenanigans : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject _minigamePanel;
    [SerializeField] private GameObject _quickTimePrefab;
    public Action OnGameFinished { get; set; }

    private int _keysToWin = 3;

    public void StartGame()
    {
        Player.Instance.IsLocked = true;
        Player.Instance.InsideMinigame = true;
        _minigamePanel.SetActive(true);
        InvokeRepeating(nameof(SpawnQuickTimeAction), 3, 3);
    }

    public void Finish()
    {
        _minigamePanel.SetActive(false);
        Player.Instance.IsLocked = false;
        _keysToWin = 3;
        CancelInvoke();
        OnGameFinished?.Invoke();
    }

    private void SpawnQuickTimeAction()
    {
        var quickTime = Instantiate(_quickTimePrefab, _minigamePanel.transform);
        quickTime.GetComponent<RectTransform>().anchoredPosition = new Vector3(Random.Range(-400, 400), Random.Range(-215, 215));
        quickTime.GetComponent<QuickTimeAction>().Init(1, this);
    }

    public void LowerCount()
    {
        _keysToWin--;
        if (_keysToWin <= 0)
        {
            Finish();
        }
    }
}