using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int Width, Height;

    [SerializeField] private Tile TilePrefab;

    [SerializeField] private Transform Camera;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < Width; i++)
        {
            for(int j = 0; j < Height; j++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3((i+i)/1.9f,(j+j)/1.9f), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";
            }
        }

        Camera.transform.position = new Vector3(Width/2, Height/2, -((Width / 2)*(Height / 2))/2);

    }
}
