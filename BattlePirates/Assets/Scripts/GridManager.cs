using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;

    [SerializeField] private Tile TilePrefab;

    [SerializeField] private Transform Camera;

    private Dictionary<Vector2, Tile> Tiles;

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
                var spawnedTile = Instantiate(TilePrefab, new Vector3((i+i)/2.2f,(j+j)/2.2f), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";

                Tiles[new Vector2(i,j)] = spawnedTile;
            }
        }

        Camera.transform.position = new Vector3(Width/2, Height/2, -10);

    }

    public Tile GetTileAtPosition(Vector2 position)
    {
        if(Tiles.TryGetValue(position, out var Tile))
        {
            return Tile;
        }
        return null;
    }
}
