using UnityEngine;

namespace Doceragnum
{
    public class Door : Interactable
    {
        [SerializeField] private GameObject _leadsToDoor;
        [SerializeField] private GameObject _roomToLoad;

        public override void Interact()
        {
            RoomManager.Instance.SwitchRoom(_roomToLoad, _leadsToDoor.transform.position);
        }
    }
}