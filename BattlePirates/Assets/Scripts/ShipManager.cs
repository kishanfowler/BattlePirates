using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public List<ShipBase> _ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _ships = new List<ShipBase>(GameObject.FindObjectsByType<ShipBase>(sortMode: FindObjectsSortMode.None));
    }
}
