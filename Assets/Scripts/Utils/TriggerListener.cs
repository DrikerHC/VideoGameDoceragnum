using System;
using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    public event Action<Collider2D> OnTriggerEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerEntered?.Invoke(collision);
        }
    }
}