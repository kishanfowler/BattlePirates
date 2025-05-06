using UnityEngine;

public class ShipBase : MonoBehaviour
{
    public int ShipLength;
    public bool IsPlayerShip;

    [SerializeField] private Sprite SunkenShipSprite;
    public bool IsHorizontal { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
