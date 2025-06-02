using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShipManager : MonoBehaviour
{
    public List<ShipBase> Ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Ships = new List<ShipBase>(GameObject.FindObjectsByType<ShipBase>(sortMode: FindObjectsSortMode.None));
    }
}
