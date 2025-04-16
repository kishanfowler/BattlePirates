using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    public GameObject GridPositions;
    public GameObject GameManager;
    public GameObject AIShootingGrid;
    private PlacementManager _placementManager;
    private int _shotsAvailable;
    private bool _canShoot;
    private List<Vector2> _shootableTargets = new();
    private Tile _tile;
    private Tile _targetTile;
    private GridManager _gridManager;
    private GridManager _aiGridManager;
    private GameManager _gameManager;
    [SerializeField] private ShipBase[] ShipPrefabs;
    private List<ShipBase> _aiShipsToPlace = new();
    private ShipManager _shipManager;
    [SerializeField] private float WaitTime;
    private float _timeWaiting;

    private void InitializeAIShips()
    {
        foreach (var shipPrefab in ShipPrefabs)
        {
            ShipBase aiShip = Instantiate(shipPrefab);
            _aiShipsToPlace.Add(aiShip);
        }
        AIPlaceShips();
    }
    private bool CanPlaceShipVertically(Vector2 StartPos, int Length)
    {
        
        for (int i = 0; i < Length; i++)
        {
            Vector2 pos = new Vector2(StartPos.x, StartPos.y + i);
            Tile tile = _gridManager.GetTileAtPosition(pos);
            if (tile == null)
            {
                return false;
            }
            if (_gridManager.GetAllTilePositions().Contains(pos)) return false; // buiten de grid
            if (tile.IsOccupied) return false; // overlapping
        }
        return true;
    }

    private void Start()
    {
        _gridManager = GridPositions.GetComponent<GridManager>();
        _aiGridManager = AIShootingGrid.GetComponent<GridManager>();
        _gameManager = GameManager.GetComponent<GameManager>();
        InitializeAIShips();
        InitShots();
    }

    private void AIPlaceShips()
    {
        GridManager grid = _gridManager;
        foreach (var ship in _aiShipsToPlace)
        {
            var placementManager = ship.GetComponent<PlacementManager>();
            bool placed = false;
            while (!placed)
            {
                TurnShip(ship);
                
                int xMax = (ship.IsHorizontal ? grid.width - ship.ShipLength + 1 : grid.width) ;
                int yMax = ship.IsHorizontal ? grid.height : grid.height - ship.ShipLength + 1;

                int x = Random.Range(0, xMax);
                int y = Random.Range(0, yMax);

                Vector2 startPos = new Vector2(x, y);
                ship.transform.position = new Vector3(startPos.x + grid.XOffset, startPos.y, -1);
                if (ship.IsHorizontal && ship.ShipLength + transform.position.x >= grid.width)
                {
                    var position = ship.transform.position;
                    position = new Vector3(position.x - ship.ShipLength,
                        position.y, -1);
                    ship.transform.position = position;
                }
                
                placed = placementManager.CheckForOccupy(ship);
                // ship.IsPlayerShip = false;

            }
            

            ship.GetComponent<BoxCollider2D>().enabled = false;
            // ship.GetComponent<SpriteRenderer>().enabled = false;
        }

        _gameManager.GameState = GameStates.PlayerTurn;
    }

    private void TurnShip(ShipBase Ship)
    {
        int[] angles = { 0, 270/*, 180, 270*/ };
        int randomIndex = Random.Range(0, angles.Length);
        float randomAngle = angles[randomIndex];
        
        Ship.transform.rotation = Quaternion.Euler(0f, 0f,randomAngle);
        if (Ship.transform.rotation.z != 0)
        {
            Ship.IsHorizontal = true;
        }
        else
        {
            Ship.IsHorizontal = false;
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
        _shootableTargets.Clear();
        _shootableTargets = _aiGridManager.GetAllTilePositions();
    }

    private void AITakeShot()
    {
        if (_shootableTargets.Count == 0)
        {
            Debug.Log("Enemy heeft geen plekken meer om te schieten!");
        }
        else
        {
            int index = Random.Range(0, _shootableTargets.Count);
            Vector2 shot = _shootableTargets[index];
            _shootableTargets.RemoveAt(index);
            _targetTile = _aiGridManager.GetTileAtPosition(shot);
            HandleShot();
        }
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

    // IEnumerator Wait()
    // {
    //     yield return new WaitForSeconds(3);
    // }
    // void Start()
    // {
    //     _gridManager = GridPositions.GetComponent<GridManager>();
    //     _gameManager = GameManager.GetComponent<GameManager>();
    //     InitShots();
    // }
    void Update()
    {
        if (_gameManager.GameState == GameStates.AITurn)
        {
            _timeWaiting += Time.deltaTime;
        }
        if (_gameManager.GameState == GameStates.AITurn && _timeWaiting >= WaitTime)
        {
            AITakeShot();
            _timeWaiting = 0;
            _gameManager.GameState = GameStates.PlayerTurn;
            _gameManager.CanPlayerAttack = true;
            
        }
        
    }
}
