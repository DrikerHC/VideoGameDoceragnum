using UnityEngine;

namespace Doceragnum
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private GameObject _icon;

        public abstract void Interact();
        
        private void Start()
        {
            _icon.SetActive(false);
        }

        protected virtual  void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _icon.SetActive(true);
        }

        protected virtual  void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) _icon.SetActive(false);
        }
    }
}