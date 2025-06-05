using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipManager : MonoBehaviour
{
    public List<ShipBase> _ships;
    public static ShipManager ShipManagerInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _ships = new List<ShipBase>(FindObjectsByType<ShipBase>(sortMode: FindObjectsSortMode.None));
        ShipManagerInstance = this; 
    }
}
