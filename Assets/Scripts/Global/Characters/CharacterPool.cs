using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using ZLinq;
using DocumentType = Doceragnum.F1Manager.DocumentType;

namespace Doceragnum
{
    public class CharacterPool : MonoBehaviour
    {
        [SerializeField] private VisitorGenerator _visitorGenerator;
        [SerializeField] private GameObject _charPrefab;
        [SerializeField] private GameObject _mainCharPrefab;
        [SerializeField] private int _size;

        public struct CharacterInfo
        {
            public GameObject GameObject;
            public CharacterData CharacterData;
            public bool HasEntered;
        }
        private List<CharacterInfo> _pool;

        public List<GameObject> _mainCharactersPool;
        public int _charsIndex = 0;

public void GenerateMainCharactersDocuments()
        {
            foreach (GameObject character in _mainCharactersPool)
            {
                var npcComponent = character.GetComponent<NPC>();
                if (npcComponent == null || npcComponent.Data == null) continue;

                var data = npcComponent.Data;

                // Comprobamos de manera segura que el modelo y los documentos existan en el Scriptable Object
                if (data.Model.Documents != null)
                {
                    List<DocumentUI> docs = new List<DocumentUI>();
                    foreach (var docType in data.Model.Documents)
                    {
                        switch (docType)
                        {
                            case DocumentType.StudentCard:
                                StudentCardDocument studentCard = F1Manager.Instance.StudentCardGameObject.GetComponent<StudentCardDocument>();
                                docs.Add(studentCard);
                                break;
                            case DocumentType.DNI:
                                IdentityCard DNI = F1Manager.Instance.IdentityCardGameObject.GetComponent<IdentityCard>();
                                docs.Add(DNI);
                                break;
                            case DocumentType.ProfessorCard:
                                ProfessorCard professorCard = F1Manager.Instance.ProfessorCardGameObject.GetComponent<ProfessorCard>();
                                docs.Add(professorCard);
                                break;
                        }
                    }
                    data.Documents = docs;
                }
            }
        }

        // FUNCIÓN MODIFICADA: Ahora es exclusiva para el movimiento y setup de la Fase 2
        public void InitMainCharacters()
        {
            foreach (GameObject character in _mainCharactersPool)
            {
                var npcComponent = character.GetComponent<NPC>();
                if (npcComponent == null || npcComponent.Data == null) continue;

                var data = npcComponent.Data;
                
                character.SetActive(true);
                character.name = data.Name;
                character.GetComponent<SpriteRenderer>().sprite = data.Sprite;
                character.transform.position = data.DefaultPosition;
                character.transform.SetParent(RoomManager.Instance.GetRoomTransform(data.LocatedInRoom), true);
            }
        }

        public void AllowInside(int index)
        {
            var npc = _pool[index];
            npc.HasEntered = true;
            _pool[index] = npc;
        }

        public List<CharacterInfo> GetAllowedList()
        {
            return _pool.AsValueEnumerable().Where(c => c.HasEntered == true).ToList();
        }

        public List<CharacterInfo> getActiveCharacters()
        {
            return _pool.AsValueEnumerable().Where(c => c.GameObject.activeSelf == true).ToList();
        }

        public CharacterInfo GetElement(int index)
        {
            if (_pool.Count <= index)
            {
                GameObject obj = Instantiate(_charPrefab, transform);
                obj.SetActive(false);
                CharacterInfo newInfo = new CharacterInfo { GameObject = obj, CharacterData = CreateCharacterData() };
                _pool.Add(newInfo);
            }
            var info = _pool[index];
            info.GameObject.SetActive(true);
            return info;
        }

        public GameObject GetMainCharacter()
        {
            var obj = _mainCharactersPool[_charsIndex];
            _charsIndex++;
            return obj;
        }

        public void ReturnElement(GameObject element)
        {
            element.SetActive(false);
        }

        public void Refill()
        {
            ClearPool();

            for (int i = 0; i < _size; i++)
            {
                GameObject obj = Instantiate(_charPrefab, transform);
                obj.GetComponent<NavMeshAgent>().enabled = false;
                obj.SetActive(false);
                _pool.Add(new CharacterInfo { GameObject = obj, CharacterData = CreateCharacterData() });
            }
            // Fer instanciament dels main characters mirant el rule set del dia

            var mainChars = InGameDataManager.Data.RuleSets[InGameDataManager.Data.DayCount - 1].StoryCharacters;
            for (int i = 0; i < mainChars.Length; i++)
            {
                GameObject obj = Instantiate(_mainCharPrefab, transform);
                obj.GetComponent<NPC>().Data = mainChars[i];
                obj.SetActive(false);
                _mainCharactersPool.Add(obj);
            }
        }

        private void ClearPool()
        {
            if (_pool == null || _mainCharactersPool == null)
            {
                _pool = new List<CharacterInfo>();
                _mainCharactersPool = new List<GameObject>();
                return;
            }
            foreach (var info in _pool)
            {
                if (info.GameObject != null) Destroy(info.GameObject);

                if (info.CharacterData != null) Destroy(info.CharacterData);
            }
            _pool.Clear();
            _mainCharactersPool.Clear();
            _charsIndex = 0;
        }

        private CharacterData CreateCharacterData()
        {
            return _visitorGenerator.GenerateVisitorInfo();
        }
    }
}