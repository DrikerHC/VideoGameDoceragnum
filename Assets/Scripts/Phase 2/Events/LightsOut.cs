using System;
using Doceragnum;
using UnityEngine;
using Random = System.Random;

public class LightsOut : MonoBehaviour, IMinigame
{
    [SerializeField] private GameObject _minigamePanel;

    [SerializeField] private GameObject[] Relays;
    private int _relaysToFix;

    public int RelaysToFix
    {
        get { return _relaysToFix; }
        set
        {

            _relaysToFix = value;
            Debug.Log(_relaysToFix);
            if (value == 0)
            {
                Debug.Log("No quedan relays por arreglar");
                Finish();
            }
        }
    }

    public Action OnGameFinished { get; set; }

    private void OnEnable()
    {
        RelayButton.OnRelayClicked += RelayFixed;
    }

    private void OnDisable()
    {
        RelayButton.OnRelayClicked -= RelayFixed;
    }

    public void StartGame()
    {
        Player.Instance.IsLocked = true;
        Random rand = new Random();
        _relaysToFix = rand.Next(3, 6);
        for (int i = 0; i < _relaysToFix; i++)
        {
            var relay = Relays[rand.Next(Relays.Length)].GetComponent<RelayButton>();
            while (relay.IsActive)
            {
                relay = Relays[rand.Next(Relays.Length)].GetComponent<RelayButton>();
            }
            relay.IsActive = true;
            relay.ClicksToFix = rand.Next(2, 5);
            relay.RelayColor = Color.red;
        }

        _minigamePanel.SetActive(true);
    }

    private void RelayFixed(RelayButton relay)
    {
        relay.RelayColor = Color.white;
        RelaysToFix--;
    }

    public void Finish()
    {
        _minigamePanel.SetActive(false);
        Player.Instance.IsLocked = false;
        OnGameFinished?.Invoke();
    }
}
