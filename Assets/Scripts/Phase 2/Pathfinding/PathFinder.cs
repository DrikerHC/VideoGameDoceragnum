using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public List<Node> FindPath(Vector3 startWorldPos, Vector3 targetWorldPos)
    {
        Node startNode = gridManager.GetNodeFromWorldPosition(startWorldPos);
        Node targetNode = gridManager.GetNodeFromWorldPosition(targetWorldPos);

        if (startNode == null || targetNode == null || !startNode.IsWalkable || !targetNode.IsWalkable) return null;

        gridManager.ResetAllNodes();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in gridManager.GetNeighbours(currentNode))
            {
                if (closedSet.Contains(neighbour)) continue;

                int movementCost = GetDistance(currentNode, neighbour);
                int tentativeGCost = currentNode.gCost + movementCost;

                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.parent = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dx = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dy = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);
        return 10 * (dx + dy);
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);

        path.Reverse();
        SetDebugPath(path);
        return path;
    }

    private List<Node> currentPathDebug;

    public void SetDebugPath(List<Node> path)
    {
        currentPathDebug = path;
    }

    private void OnDrawGizmos()
    {
        if (currentPathDebug == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < currentPathDebug.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPathDebug[i].worldPosition, currentPathDebug[i + 1].worldPosition);
        }
    }
}
