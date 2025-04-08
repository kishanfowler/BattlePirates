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
    private List<Vector2> _shootableTargets = new List<Vector2>();
    private ShipBase[] _enemyShips;
    private Tile _tile;
    private Tile _targetTile;
    private GridManager _gridManager;
    private GameManager _gameManager;

    // public AIManager()
    // {
    //     _enemyShips = new[] {
    //         gameObject.AddComponent<ShipBase>()
    //     };
    // }

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
