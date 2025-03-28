using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    private Dictionary<Vector2, ShipBase> Ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var item in GameObject.FindObjectsOfType<ShipBase>())
        {
            Ships[new Vector2(item.transform.position.x, item.transform.position.y)] = item;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ShipBase GetShipAtLocation(Vector2 position)
    {
        if (Ships.TryGetValue(position, out var ship))
        {
            return ship;
        }
        return null;
    }
}
