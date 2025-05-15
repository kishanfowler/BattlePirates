using System.Collections.Generic;
using UnityEngine;

public class ShipBase : MonoBehaviour
{
    public int ShipLength;
    public bool IsPlayerShip;
    public List<Vector2> OccupiedTileLocations = new();

    [SerializeField] private Sprite SunkenShipSprite;
    public bool IsHorizontal { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
