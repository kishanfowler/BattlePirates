using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;

    [SerializeField] private Tile TilePrefab;

    [SerializeField] private Transform Camera;

    private Dictionary<Vector2, Tile> Tiles;

    [SerializeField] private float XOffset, YOffset;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Tiles = new Dictionary<Vector2, Tile>();
        for (int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(((i + i) / 2.2f)+XOffset,((j+j)/2.2f)+YOffset), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";

                Tiles[new Vector2(i + XOffset,j + YOffset)] = spawnedTile;
            }
        }

        Camera.transform.position = new Vector3(Width/2, Height/2, -10);

    }

    public Tile GetTileAtPosition(Vector2 position)
    {
        position = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if(Tiles.TryGetValue(position, out var Tile))
        {
            return Tile;
        }
        return null;
    }
}
