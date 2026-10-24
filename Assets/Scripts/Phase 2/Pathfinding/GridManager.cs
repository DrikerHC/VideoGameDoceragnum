using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    private Node[,] _grid;
    // Cantonada inferior esquerra del Tilemap
    private Vector3Int _minCell;
    private int _width, _height;

    [SerializeField] private TileBase _walkableTile;

    private void Awake()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        // Obtenim els límits del Tilemap (només les cel·les que realment existeixen)
        BoundsInt bounds = _tilemap.cellBounds;

        _minCell = bounds.min;
        _width = bounds.size.x;
        _height = bounds.size.y;

        _grid = new Node[_width, _height];

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3Int cellPos = new Vector3Int(_minCell.x + x, _minCell.y + y, _minCell.z);

                TileBase tile = _tilemap.GetTile(cellPos);

                // Mira si la cel·la actual equival a una cel·la caminable
                bool isWalkable;
                if (tile != null && tile.Equals(_walkableTile))
                {
                    isWalkable = true;
                }
                else
                {
                    isWalkable = false;
                }

                // Posició en el món (centre de la cel·la)
                Vector3 worldPos = _tilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f);

                // Crea el node
                _grid[x, y] = new Node(new Vector2Int(x, y), worldPos, isWalkable);
            }
        }
    }

    public Node GetNode(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= _width || gridPos.y < 0 || gridPos.y >= _height)
            return null;

        return _grid[gridPos.x, gridPos.y];
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        Vector3Int cell = _tilemap.WorldToCell(worldPos);
        int x = cell.x - _minCell.x;
        int y = cell.y - _minCell.y;

        if (x < 0 || x >= _width || y < 0 || y >= _height)
            return null;

        return _grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int[,] directions = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            Vector2Int neighbourPos = node.gridPosition + new Vector2Int(directions[i, 0], directions[i, 1]);
            Node neighbour = GetNode(neighbourPos);

            if (neighbour != null && neighbour.IsWalkable) neighbours.Add(neighbour);
        }
        return neighbours;
    }

    public void ResetAllNodes()
    {
        if (_grid == null) return;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Node node = _grid[x, y];
                if (node != null)
                {
                    node.gCost = int.MaxValue;
                    node.hCost = 0;
                    node.parent = null;
                }
            }
        }
    }
}
