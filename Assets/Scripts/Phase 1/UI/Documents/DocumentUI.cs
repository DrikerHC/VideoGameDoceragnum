using TMPro;
using UnityEngine;

namespace Doceragnum
{
    public abstract class DocumentUI : MonoBehaviour
    {
        public GameObject DeskDocument;
        [SerializeField] private Vector2 _spritePos;
        public F1Manager.DocumentType DocumentType;

        private string _dni = "";
        [SerializeField] private TextMeshProUGUI _dniText;

        public string DNI
        {
            get { return _dni; }
            set
            {
                _dni = value;
                _dniText.text = value;
            }
        }

        public abstract void InitVisitorData(CharacterData visitorInfo);

        public void DisableDocument()
        {
            gameObject.SetActive(false);
            DeskDocument.SetActive(false);
            DeskDocument.transform.position = _spritePos;
        }
    }
}