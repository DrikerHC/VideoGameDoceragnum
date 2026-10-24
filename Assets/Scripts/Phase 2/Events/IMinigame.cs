using System;

public interface IMinigame
{
    void StartGame();
    public Action OnGameFinished { get; set; }
}
