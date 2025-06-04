using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;

    [SerializeField] private Tile TilePrefab;

    [SerializeField] private Transform Camera;

    private Dictionary<Vector2, Tile> _tiles;

    [SerializeField] public int XOffset;
    [SerializeField] private int YOffset;
    private bool _gridGenDone;
    public bool GridGenDone => _gridGenDone;
    public Dictionary<Vector2, Tile> Tiles => _tiles;

    public int width => Width;

    public int height => Height;

    private void Awake()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        
        _tiles = new Dictionary<Vector2, Tile>();
        for (int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                var position = new Vector2(i + XOffset, j + YOffset);
                var spawnedTile = Instantiate(TilePrefab, new Vector3(i+XOffset,j+YOffset, -0.5f), Quaternion.identity, gameObject.transform);
                spawnedTile.name = $"Tile {i} {j}";
                spawnedTile.Init(new Vector2(i,j)); // <-- hier geef je tile zijn gridpositie
                var sprite = Resources.Load<Sprite>($"MapWithoutHoles/{spawnedTile.name}");
                spawnedTile.SetBaseSprite(sprite);
                _tiles[position] = spawnedTile;
            }
        }
        _gridGenDone = true;
    }

    public Tile GetTileAtWorldPosition(Vector2 position)
    {
        position = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if(_tiles.TryGetValue(position, out var Tile))
        {
            return Tile;
        }
        return null;
    }



    public List<Vector2> GetAllTilePositions()
    {
        return _tiles.Keys.ToList();
    }

    public void SetTileOccupied(Vector2 pos, bool occupied)
    {
        if (_tiles.ContainsKey(pos))
        {
            _tiles[pos].IsOccupied = occupied;
        }
    }
    public bool IsTileOccupied(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile.IsOccupied;
        }

        return false; // als de tile niet bestaat
    }

    public bool AreAllAIShipTilesHit()
    {
        foreach (var tile in _tiles.Values)
        {
            if (tile.IsOccupied && !tile.IsHit)
            {
                return false;
            }
        }

        return true;
    }
}
