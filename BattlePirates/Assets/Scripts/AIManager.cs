using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AIManager : MonoBehaviour
{
    public GameObject GridPositions;
    // public GameObject GameManager;
    public GameObject AIShootingGrid;
    public GameObject UIPlaying;
    private PlacementManager _placementManager;
    private int _shotsAvailable;
    private bool _canShoot;
    private List<Vector2> _shootableTargets = new();
    private Tile _tile;
    private Tile _targetTile;
    private GridManager _gridManager;
    private GridManager _aiGridManager;
    private GameManager _gameManager;
    private ButtonHandler _buttonHandler;
    [SerializeField] private ShipBase[] ShipPrefabs;
    public List<ShipBase> AIShipsToPlace = new();
    private ShipManager _shipManager;
    [SerializeField] private float WaitTime;
    private float _timeWaiting;
    private bool _canSpecialAttack;
    [SerializeField] private int TurnsPlayed = 0;
    public int MinimumTurnsBeforeSpecial;
    public int MaximumTurnsBeforeSpecial;
    private bool _specialAttackChosen = false;
    [SerializeField] private SpecialAttacks ChosenSpecialAttack;
    [SerializeField] private GameObject PlusIndicator;
    [SerializeField] private GameObject StandardShotIndicator;
    private bool _plusAttack = true;
    public ShipBase DutchmanPrefab;
    [SerializeField] private int RandomTurn;
    private bool _hasIndicator = false;
    public SkeletonAnimation _SkeletonAnimation;
    public AnimationReferenceAsset HeadsAnimation;
    public AnimationReferenceAsset TailsAnimation;
    public GameObject CoinAnimationObject;
    private enum SpecialAttacks
    {
        Coin,
        Plus,
        Dutchman,
        Mist
    }
    private void InitializeAIShips()
    {
        foreach (var shipPrefab in ShipPrefabs)
        {
            ShipBase aiShip = Instantiate(shipPrefab);
            AIShipsToPlace.Add(aiShip);
        }
        if (ChosenSpecialAttack == SpecialAttacks.Dutchman)
        {
            ShipBase aiDutchman = Instantiate(DutchmanPrefab);
            AIShipsToPlace.Add(aiDutchman);
        }
        AIPlaceShips();
    }

    private void Start()
    {
        _gridManager = GridPositions.GetComponent<GridManager>();
        _aiGridManager = AIShootingGrid.GetComponent<GridManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gameManager.CanPlayerAttack = true;
        _buttonHandler = UIPlaying.GetComponent<ButtonHandler>();
        if (!_specialAttackChosen)
        {
            ChosenSpecialAttack = (SpecialAttacks)Random.Range(0, Enum.GetNames(typeof(SpecialAttacks)).Length);
            RandomTurn = Random.Range(MinimumTurnsBeforeSpecial, MaximumTurnsBeforeSpecial);
            _specialAttackChosen = true;
            _canSpecialAttack = true;
        }
        InitializeAIShips();
        InitShots();
    }

    private void AIPlaceShips()
    {
        GridManager grid = _gridManager;
        foreach (var ship in AIShipsToPlace)
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
                
                placed = placementManager.CheckForOccupy(ship, grid);
                // ship.IsPlayerShip = false;

            }
            

            ship.GetComponent<BoxCollider2D>().enabled = false;
            ship.GetComponent<SpriteRenderer>().enabled = false;
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

    private void InitShots()
    {
        _shootableTargets.Clear();
        _shootableTargets = _aiGridManager.GetAllTilePositions();
    }

    private IEnumerator ExecuteAIShotRoutine()
    {
        if (_shootableTargets.Count == 0)
        {
            Debug.LogWarning("Enemy heeft geen plekken meer om te schieten!");
        }
        else
        {
            int index = Random.Range(0, _shootableTargets.Count);
            Vector2 shot = _shootableTargets[index];
            _shootableTargets.RemoveAt(index);
            _targetTile = _aiGridManager.GetTileAtPosition(shot);
            if (!_hasIndicator)
            {
                var indicator = Instantiate(StandardShotIndicator,
                    new Vector3(_targetTile.transform.position.x, _targetTile.transform.position.y, -1.5f),
                    quaternion.identity);
                _hasIndicator = true;
                Destroy(indicator, WaitTime);
            }
            yield return new WaitForSeconds(WaitTime);
            HandleShot();
            
            _hasIndicator = false;
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
            StartCoroutine(ExecuteAIShotRoutine());
        }
    }

    public void RemoveShips()
    {
        if (AIShipsToPlace != null)
        {
            foreach (var ship in AIShipsToPlace)
            {
                Destroy(ship.gameObject);
            }
            AIShipsToPlace.Clear();
        }
        var playerShips= GameObject.FindGameObjectsWithTag("Ship");
        if (playerShips != null)
        {
            foreach (var ship in playerShips)
            {
                Destroy(ship.gameObject);
            }
        }
    }
    
    private void Update()
    {
        if (_gameManager.GameState == GameStates.AITurn)
        {
            _timeWaiting += Time.deltaTime;
        }
        if (_gameManager.GameState == GameStates.AITurn && _timeWaiting >= WaitTime)
        {
            if (TurnsPlayed == RandomTurn)
            {
                if (_canSpecialAttack && ChosenSpecialAttack != SpecialAttacks.Dutchman)
                {
                    StartCoroutine(ExecuteAISpecialAttackRoutine(ChosenSpecialAttack));
                }
            }
            else
            {
                StartCoroutine(ExecuteAIShotRoutine());
            }
            TurnsPlayed++;
            _gameManager.GameState = GameStates.PlayerTurn;
            _gameManager.CanPlayerAttack = true;
            _gameManager.TimerHasReset = false;
            _timeWaiting = 0;

        }

        if (_gameManager.GameState == GameStates.PlayerTurn && _gridManager.AreAllAIShipTilesHit())
        {
            _buttonHandler.ShowVictoryScreen();
            RemoveShips();
        }
        
    }

    private IEnumerator ExecuteAISpecialAttackRoutine(SpecialAttacks attackType)
    {
        if (_gameManager.GameState == GameStates.AITurn && _canSpecialAttack)
        {
            switch (attackType)
            {
                case SpecialAttacks.Coin:
                    var RandomNumber = UnityEngine.Random.Range(0, 1);
                    // If the coin is Tails, hit a player ship
                    if (RandomNumber == 0)
                    {
                        CoinAnimationObject.SetActive(true);
                        _SkeletonAnimation.state.SetAnimation(0, TailsAnimation, false);
                        int randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                        while (!_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                            if (_shipManager._ships[randomShip].IsPlayerShip)
                            {
                                break;
                            }
                        }
                        if (_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            Tile tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                            while (tile.CanBeHit == false)
                            {
                                tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                                if(tile.CanBeHit)
                                {
                                    break;
                                }
                            }
                            tile.OnHit();
                        }
                    }
                    // If the coin is Heads, hit an enemy ship
                    else
                    {
                        CoinAnimationObject.SetActive(true);
                        _SkeletonAnimation.state.SetAnimation(0, HeadsAnimation, false);
                        int randomShip = UnityEngine.Random.Range(0, AIShipsToPlace.Count);
                        while (AIShipsToPlace[randomShip].IsPlayerShip)
                        {
                            randomShip = UnityEngine.Random.Range(0, AIShipsToPlace.Count);
                            if (!AIShipsToPlace[randomShip].IsPlayerShip)
                            {
                                break;
                            }
                        }
                        if (!AIShipsToPlace[randomShip].IsPlayerShip)
                        {
                            Tile tile = AIShipsToPlace[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, AIShipsToPlace[randomShip].ShipLength)].GetTile();
                            while (tile.CanBeHit == false)
                            {
                                tile = AIShipsToPlace[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, AIShipsToPlace[randomShip].ShipLength)].GetTile();
                                break;
                            }
                            tile.OnHit();
                        }
                    }

                    break;
                case SpecialAttacks.Plus:
                    int index = Random.Range(0, _shootableTargets.Count);
                    Vector2 shot = _shootableTargets[index];
                    _shootableTargets.RemoveAt(index);

                    _targetTile = _aiGridManager.GetTileAtPosition(shot);

                    Vector2 centerPos = _targetTile.transform.position;
                    var indicator = Instantiate(PlusIndicator,
                        new Vector3(centerPos.x, centerPos.y, -1.5f),
                        Quaternion.identity);
                    Destroy(indicator, WaitTime);
                    yield return new WaitForSeconds(WaitTime);
                    Vector2[] directions = {
                        Vector2.zero,               // midden
                        Vector2.right,              // rechts
                        Vector2.left,               // links
                        Vector2.up,                 // boven
                        Vector2.down                // onder
                    };

                    foreach (var dir in directions)
                    {
                        Tile tile = _aiGridManager.GetTileAtPosition(centerPos + dir);
                        if (tile != null)
                        {
                            tile.OnHit();
                        }
                    }

                    _canSpecialAttack = false;
                    break;
                case SpecialAttacks.Dutchman:
                    _canSpecialAttack = false;
                    break;
                case SpecialAttacks.Mist:
                    _canSpecialAttack = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attackType), attackType, null);
            }        
            _canSpecialAttack = false;
        }

    }
    
}