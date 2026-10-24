using Doceragnum;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NpcWalker : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;

    public void SetNpcWalk()
    {
        var activeChars = InGameDataManager.CharacterPool.getActiveCharacters();
        foreach (var _char in activeChars)
        {
            GameObject obj = _char.GameObject;
            var otherWaypoints = _waypoints.Where(w => w.position != obj.transform.position).ToArray();
            Vector3 destination = otherWaypoints[Random.Range(0, otherWaypoints.Length)].position;
            var agent = obj.GetComponent<NavMeshAgent>();
            agent.enabled = true;
            agent.SetDestination(destination);
            var script = obj.GetComponent<ProceduralCharacter>();
            script.IsAgentActive = true;
            script.Target = destination;
        }
    }

    public void SetNextDestination(ProceduralCharacter script)
    {
        var otherWaypoints = _waypoints.Where(w => w.position != script.Target).ToArray();
        Vector3 destination = otherWaypoints[Random.Range(0, otherWaypoints.Length)].position;
        script.GetComponent<NavMeshAgent>().SetDestination(destination);
        script.HasArrived = false;
        script.Target = destination;
    }
}