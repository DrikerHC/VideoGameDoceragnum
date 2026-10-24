using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class ButtonStoreItemController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI StoreItemName;
        [SerializeField] private Button ItemButton;
        [SerializeField] private Button BtnBuy;
        [SerializeField] public StoreItemSO DataItem;

        private void Awake()
        {
            BtnBuy.onClick.AddListener(Buy);
        }

        public void init()
        {
            if (DataItem == null)
                return;

            StoreItemName.text = DataItem.name;
            ItemButton.GetComponent<Image>().sprite = DataItem.ItemSprite;
        }

        private void OnDestroy()
        {
            BtnBuy.onClick.RemoveListener(Buy);
        }

        private void Buy()
        {
            InGameDataManager.Instance.BuyItem(DataItem);
        }
    }
}