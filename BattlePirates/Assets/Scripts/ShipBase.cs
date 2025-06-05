using System.Collections.Generic;
using UnityEngine;

public class ShipBase : MonoBehaviour
{
    public int ShipLength;
    public bool IsPlayerShip;
    public List<Vector2> OccupiedTileLocations = new();
    private GameManager _gameManager;
    [SerializeField] private Sprite SunkenShipSprite;
    public bool IsHorizontal { get; set; }
    public bool ShipDestroyed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _gameManager = GameManager.GameManagerInstance;
        DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        if (IsPlayerShip && !ShipDestroyed && _gameManager.GameState != GameStates.PreparationPhase)
        {
            int j = 0;
            for (int i = 0; i < OccupiedTileLocations.Count; i++)
            {
                if (_gameManager.AIGridManager.GetTileAtWorldPosition(OccupiedTileLocations[i]).IsHit)
                {
                    j++;
                }
                if(j == OccupiedTileLocations.Count)
                {
                    j = 0;
                    ShipDestroyed = true;
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 180);
                }
            }
        }
    }
}
