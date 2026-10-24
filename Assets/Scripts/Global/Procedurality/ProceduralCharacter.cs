using Assets.Scripts.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Doceragnum
{
    public class ProceduralCharacter : MonoBehaviour
    {
        Random rand;

        private int _characterId;
        private NavMeshAgent _agent;
        public bool IsAgentActive { get; set; } = false;
        public bool HasArrived { get; set; } = false;
        public Vector3 Target { get; set; }
        [SerializeField] private SpriteElements _spriteElements;

        private SpriteRenderer _hairRenderer;
        private SpriteRenderer _shirtRenderer;
        private SpriteRenderer _pantsRenderer;

        private void Awake()
        {
            var childObj = transform.GetChild(0);
            _hairRenderer = childObj.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            _shirtRenderer = childObj.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
            _pantsRenderer = childObj.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            print("Ha tocado con algo");
            transform.GetChild(0).gameObject.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            print("desaparece");
            print(collision.name);
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void Start()
        {
            _characterId = InGameDataManager.Data.NpcCount++;
            int characterSeed = InGameDataManager.Data.Seed.Seeded(_characterId);
            rand = new Random(characterSeed);
            SpriteEntry hair = _spriteElements.Hairs[rand.Next(_spriteElements.Hairs.Length)];
            SpriteEntry shirt = _spriteElements.Shirts[rand.Next(_spriteElements.Shirts.Length)];
            SpriteEntry pants = _spriteElements.Pants[rand.Next(_spriteElements.Pants.Length)];

            _hairRenderer.sprite = hair.Sprite;
            _shirtRenderer.sprite = shirt.Sprite;
            _pantsRenderer.sprite = pants.Sprite;

            //Debug.Log($"Personatge procedural generat amb cabell {hair.Name}, camiseta {shirt.Name} i pantalons {pants.Name}");
        }

        private void Update()
        {
            if (!IsAgentActive) return;

            if (HasArrived) return;

            if (_agent.remainingDistance < 0.1f)
            {
                HasArrived = true;
                transform.position = Target;
                StartCoroutine(WaitingItOutConsensualStyle());
            }
        }

        // quirked up white boy goated with the sauce busting it down maybe-in-a-not-so-sexual style
        private IEnumerator WaitingItOutConsensualStyle()
        {
            yield return new WaitForSeconds(2);
            F2Manager.Instance.Npcwalker.SetNextDestination(this);
        }
    }
}

// Para guardar y cargar:
// - Cada npc tiene un so con su info, incluido su id
// - Luego al final se guarda el npc count y los so con la info de cada npc