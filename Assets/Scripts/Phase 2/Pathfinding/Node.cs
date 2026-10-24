using UnityEngine;

public class Node
{
    // Grid Coords
    public Vector2Int gridPosition;
    // Word Pos (Cell Center)
    public Vector3 worldPosition;
    public bool IsWalkable;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    // Previous Node
    public Node parent;

    public Node(Vector2Int gridPos, Vector3 worldPos, bool isWalkable)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
        IsWalkable = isWalkable;
    }
}
