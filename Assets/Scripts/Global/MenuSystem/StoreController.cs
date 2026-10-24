using System;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class StoreController : MMState
    {
        [SerializeField] private Button VideoJocsButton;
        [SerializeField] private StoreItemSO[] _storeItems;

        [SerializeField] private ButtonStoreItemController[] _IconosStoreItems;

        private void OnEnable()
        {
            VideoJocsButton.onClick.AddListener(OnPlayCallback);

            for (int i = 0; i < _IconosStoreItems.Length && i < _storeItems.Length; i++)
            {
                _IconosStoreItems[i].DataItem = _storeItems[i];
                _IconosStoreItems[i].init();
            }
        }

        private void OnDisable()
        {
            VideoJocsButton.onClick.RemoveListener(OnPlayCallback);
        }

        private void OnPlayCallback()
        {
            print("back");
            menuManager.ChangeMenu(MenuState.Home);
        }
    }
}