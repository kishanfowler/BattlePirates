using UnityEngine;

public class ShipBase : MonoBehaviour
{
    public int ShipLength;
    public bool IsPlayerShip;
    [SerializeField] private Sprite SunkenShipSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IsPlayerShip = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
