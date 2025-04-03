using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public ShipBase[] _ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ships = GameObject.FindObjectsOfType<ShipBase>();
    }
}
