using Doceragnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class GameShopManager : MMState
    {
        [SerializeField] private Button VideoJocsButton;

        [SerializeField] private VideojuegoSO[] _VideoJocs;

        [SerializeField] private BotonJuegoController[] _IconosJuegos;


        private void OnEnable()
        {
            VideoJocsButton.onClick.AddListener(OnPlayCallback);
            
            for (int i = 0; i < _IconosJuegos.Length && i<_VideoJocs.Length; i++)
            {
                _IconosJuegos[i].DataJuego = _VideoJocs[i];
                _IconosJuegos[i].init();
            }
        }

        private void OnDisable()
        {
            VideoJocsButton.onClick.RemoveListener(OnPlayCallback);
        }

        private void OnPlayCallback()
        {
            menuManager.ChangeMenu(MenuState.Home);
        }
    }
}