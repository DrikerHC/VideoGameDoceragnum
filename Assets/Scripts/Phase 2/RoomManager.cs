using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Doceragnum
{
    [System.Serializable]
    public struct Room
    {
        public GameObject RoomGameObject;
        public Collider2D CameraCollider;
        public Transform CameraTransform;
        public bool IsStatic;
    }

    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private CinemachineConfiner2D _confiner;

        [SerializeField] private Room[] _rooms;
        private Dictionary<string, GameObject> _roomNames = new Dictionary<string, GameObject>();
        private Dictionary<GameObject, Room> _roomDict = new Dictionary<GameObject, Room>();
        [SerializeField] private GameObject _currentRoom;
        private GameObject _targetRoom;

        private Vector3 _playerPlacement;

        [Header("Floors Params"), SerializeField] private TriggerListener FloorDownTrigger;
        [SerializeField] private TriggerListener FloorUpTrigger;
        [SerializeField] private GameObject Floor1Room;
        [SerializeField] private GameObject Floor0Room;
        [SerializeField] private Transform PlayerPosFloor1;
        [SerializeField] private Transform PlayerPosFloor0;
        [SerializeField] private Collider2D Floor1Confiner;
        [SerializeField] private Collider2D Floor0Confiner;


        private int _walkingNpcs = 3;
        [SerializeField] private List<Transform> _walkWaypoints;

        private void Awake()
        {
            Instance = this;
            int i = 0;
            foreach (Transform child in transform)
            {
                foreach (Transform room in child)
                {
                    _roomNames[room.name] = room.gameObject;
                    room.gameObject.SetActive(false);
                    _roomDict.Add(_rooms[i].RoomGameObject, _rooms[i]);
                    i++;
                }
            }
        }

        private void OnEnable()
        {
            FloorDownTrigger.OnTriggerEntered += GoDownFloor;
            FloorUpTrigger.OnTriggerEntered += GoUpFloor;
        }

        private void OnDisable()
        {
            FloorDownTrigger.OnTriggerEntered -= GoDownFloor;
            FloorUpTrigger.OnTriggerEntered -= GoUpFloor;
        }

        public void PlaceWalkNpcs()
        {
            var allowedNpcs = InGameDataManager.CharacterPool.GetAllowedList();
            for (int i = 0; i < _walkingNpcs; i++)
            {
                if (allowedNpcs.Count != 0 && _walkWaypoints.Count != 0)
                {
                    var obj = allowedNpcs[0].GameObject;
                    obj.transform.localScale = Vector3.one;
                    int randWaypoint = Random.Range(0, _walkWaypoints.Count);
                    obj.transform.position = _walkWaypoints[randWaypoint].position;
                    obj.SetActive(true);
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                    allowedNpcs.RemoveAt(0);
                    _walkWaypoints.RemoveAt(randWaypoint);
                }
            }
        }

        public void Init()
        {
            _currentRoom.SetActive(true);
            Room roomInfo = _roomDict[_currentRoom];

            UpdateCamera(roomInfo);

            TransitionLoader.Instance.EndTransition();
        }

        public void SwitchRoom(GameObject targetRoom, Vector3 playerPosition)
        {
            _targetRoom = targetRoom;
            _playerPlacement = playerPosition;
            PauseMenu.Instance.CanBeOpened = false;
            TransitionLoader.Instance.OnTransitionDone += LoadRoom;
            TransitionLoader.Instance.BeginTransition();
        }

        private void LoadRoom()
        {
            _currentRoom.SetActive(false);
            _targetRoom.SetActive(true);
            _currentRoom = _targetRoom;
            Player.Instance.transform.position = _playerPlacement;
            TransitionLoader.Instance.OnTransitionDone -= LoadRoom;

            UpdateCamera(_roomDict[_currentRoom]);

            TransitionLoader.Instance.EndTransition();
        }

        public Transform GetRoomTransform(string roomName)
        {
            return _roomNames[roomName].transform;
        }

        private void UpdateCamera(Room roomInfo)
        {
            if (roomInfo.IsStatic)
            {
                _cam.Follow = null;
                _confiner.enabled = false;
                _confiner.BoundingShape2D = null;
                _cam.transform.position = roomInfo.CameraTransform.position;
                _cam.transform.Translate(0, 0, -10);
            }
            else
            {
                _cam.Follow = Player.Instance.transform;
                _confiner.enabled = true;
                _confiner.BoundingShape2D = roomInfo.CameraCollider;
                _confiner.InvalidateBoundingShapeCache();
            }
        }

        private async void GoDownFloor(Collider2D collision)
        {
            _cam.Follow = null;
            _confiner.enabled = false;

            Floor1Room.SetActive(false);
            Floor0Room.SetActive(true);
            Player.Instance.transform.position = PlayerPosFloor0.position;

            _cam.ForceCameraPosition(PlayerPosFloor0.position, Quaternion.identity);

            await Awaitable.NextFrameAsync();

            _confiner.BoundingShape2D = Floor0Confiner;
            _confiner.InvalidateBoundingShapeCache();
            _confiner.enabled = true;

            _cam.Follow = Player.Instance.transform;
        }

        private async void GoUpFloor(Collider2D collision)
        {
            _cam.Follow = null;
            _confiner.enabled = false;

            Floor0Room.SetActive(false);
            Floor1Room.SetActive(true);
            Player.Instance.transform.position = PlayerPosFloor1.position;

            _cam.ForceCameraPosition(PlayerPosFloor1.position, Quaternion.identity);

            await Awaitable.NextFrameAsync();

            _confiner.BoundingShape2D = Floor1Confiner;
            _confiner.InvalidateBoundingShapeCache();
            _confiner.enabled = true;

            _cam.Follow = Player.Instance.transform;
        }
    }
}