using Doceragnum;

public class EventObject : Interactable
{
    public override void Interact()
    {
        gameObject.GetComponent<IMinigame>().StartGame();
    }
}
