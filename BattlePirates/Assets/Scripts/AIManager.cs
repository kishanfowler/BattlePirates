using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    public GameObject GridPositions;
    public GameObject GameManager;
    private int _shotsAvailable;
    private bool _canShoot;
    private List<Vector2> _shootableTargets = new();
    private Tile _tile;
    private Tile _targetTile;
    private GridManager _gridManager;
    private GameManager _gameManager;
    [SerializeField] private ShipBase[] ShipPrefabs;
    private List<ShipBase> _aiShipsToPlace = new ();
    private ShipManager _shipManager;

    private void InitializeAIShips()
    {
        _shipManager = gameObject.GetComponent<ShipManager>();
        ShipPrefabs = _shipManager._ships;
        foreach (var shipPrefab in ShipPrefabs)
        {
            ShipBase aiShip = Instantiate(shipPrefab);
            aiShip.gameObject.SetActive(false);
            _aiShipsToPlace.Add(aiShip);
        }
    }
    private bool CanPlaceShipVertically(Vector2 StartPos, int Length)
    {
        for (int i = 0; i < Length; i++)
        {
            Vector2 pos = new Vector2(StartPos.x, StartPos.y + i);

            if (_gridManager.GetAllTilePositions().Contains(pos)) return false; // buiten de grid
            if (_gridManager.GetTileAtPosition(pos).IsOccupied) return false; // overlapping
        }

        return true;
    }

    private void Awake()
    {
        InitializeAIShips();
    }

    private void AIPlaceShips()
    {
        GridManager grid = GridPositions.GetComponent<GridManager>();
        foreach (var ship in _aiShipsToPlace)
        {
            bool placed = false;
            while (!placed)
            {
                int x = Random.Range(0, grid.width);
                int y = Random.Range(0, grid.height - ship.ShipLength + 1);

                Vector2 startPos = new Vector2(x, y);

                if (CanPlaceShipVertically(startPos, ship.ShipLength))
                {
                    PlaceShipVertically(startPos, ship.ShipLength);
                    placed = true;
                }



            }
        }
    }

    private void PlaceShipVertically(Vector2 StartPos, int ShipLength)
    {
        for (int i = 0; i < ShipLength; i++)
        {
            Vector2 pos = new Vector2(StartPos.x, StartPos.y + i);
            Tile tile = _gridManager.GetTileAtPosition(pos);
            tile.IsOccupied = true;
        }
    }

    private void InitShots()
    {
        GridManager grid = GridPositions.GetComponent<GridManager>();
        int gridWidth = grid.width;
        int gridHeight = grid.height;
        _shootableTargets.Clear();
        _shootableTargets = _gridManager.GetAllTilePositions();
    }

    private void AITakeShot()
    {
        if (_shootableTargets.Count == 0)
        {
            Debug.Log("Enemy heeft geen plekken meer om te schieten!");
            return;
        }

        int index = Random.Range(0, _shootableTargets.Count);
        Vector2 shot = _shootableTargets[index];
        _shootableTargets.RemoveAt(index);
        _targetTile = _gridManager.GetTileAtPosition(shot);
        HandleShot();
    }

    private void HandleShot()
    {
        if (_targetTile != null)
        {
            _targetTile.OnHit();
            _targetTile = null;
        }
        else
        {
            AITakeShot();
        }
    }

    void Start()
    {
        _gridManager = GridPositions.GetComponent<GridManager>();
        _gameManager = GameManager.GetComponent<GameManager>();
        InitShots();
    }
    void Update()
    {
        if (_gameManager.GameState == GameStates.AITurn)
        {
            AITakeShot();
            _gameManager.GameState = GameStates.PlayerTurn;
            _gameManager.CanPlayerAttack = true;
        }
    }
}
