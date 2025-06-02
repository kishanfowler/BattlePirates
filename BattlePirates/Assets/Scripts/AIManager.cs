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
    [Header("References")]
    [SerializeField] private GameObject GridPositions;
    [SerializeField] private GameObject AIShootingGrid;
    [SerializeField] private GameObject UIPlaying;

    [Header("Ship Setup")]
    [SerializeField] private ShipBase[] ShipPrefabs;
    [SerializeField] private ShipBase DutchmanPrefab;
    public IReadOnlyList<ShipBase> AIShipsToPlace => _aiShipsToPlace;
    [Header("Attack Logic")]
    [SerializeField] private GameObject PlusIndicator;
    [SerializeField] private GameObject StandardShotIndicator;
    [SerializeField] private float WaitTime;
    [SerializeField] private int MinimumTurnsBeforeSpecial;
    [SerializeField] private int MaximumTurnsBeforeSpecial;
    
    [Header("Animations")]
    public SkeletonAnimation SkeletonAnimation;
    public AnimationReferenceAsset HeadsAnimation;
    public AnimationReferenceAsset TailsAnimation;
    public GameObject CoinAnimationObject;

    private GridManager _gridManager;
    private GridManager _aiGridManager;
    private ButtonHandler _buttonHandler;
    private ShipManager _shipManager;
    private GameManager _gameManager;
    private PlacementManager _placementManager;
    private List<ShipBase> _aiShipsToPlace = new();
    private List<Vector2> _shootableTargets = new();
    private Tile _targetTile;

    private int _shotsAvailable;
    private int _turnsPlayed;
    private int _randomTurn;
    private bool _canShoot;
    private bool _hasIndicator;
    private bool _canSpecialAttack;
    private bool _specialAttackChosen;
    private bool _plusAttack = true;
    private float _timeWaiting;

    private enum SpecialAttacks { Coin, Plus, Dutchman, Mist }
    [SerializeField] private SpecialAttacks ChosenSpecialAttack;

    
    private void Awake()
    {
        _gridManager = GridPositions.GetComponent<GridManager>();
        _aiGridManager = AIShootingGrid.GetComponent<GridManager>();
        _buttonHandler = UIPlaying.GetComponent<ButtonHandler>();
        _shipManager = FindFirstObjectByType<ShipManager>(); // Consider dependency injection here too
        _gameManager = GameManager.GameManagerInstance;
        if (_gameManager == null)
        {
            Debug.LogError("GameManager instance not found.");
            enabled = false;
            return;
        }
    }
    private void Start()
    {
        if (_gameManager == null)
        {
            Debug.LogError("GameManager reference not set.");
            enabled = false;
            return;
        }

        _gameManager.CanPlayerAttack = true;

        if (!_specialAttackChosen)
        {
            ChosenSpecialAttack = (SpecialAttacks)Random.Range(0, Enum.GetNames(typeof(SpecialAttacks)).Length);
            _randomTurn = Random.Range(MinimumTurnsBeforeSpecial, MaximumTurnsBeforeSpecial);
            _specialAttackChosen = true;
            _canSpecialAttack = true;
        }

        InitializeAIShips();
        InitShots();
    }
    private void InitializeAIShips()
    {
        foreach (var prefab in ShipPrefabs)
        {
            var ship = Instantiate(prefab);
            _aiShipsToPlace.Add(ship);
        }

        if (ChosenSpecialAttack == SpecialAttacks.Dutchman)
        {
            var dutchman = Instantiate(DutchmanPrefab);
            _aiShipsToPlace.Add(dutchman);
        }

        AIPlaceShips();
    }
    private void AIPlaceShips()
    {
        foreach (var ship in _aiShipsToPlace)
        {
            var placementManager = ship.GetComponent<PlacementManager>();
            bool placed = false;

            while (!placed)
            {
                RotateShipRandomly(ship);

                int xMax = ship.IsHorizontal ? _gridManager.width - ship.ShipLength + 1 : _gridManager.width;
                int yMax = ship.IsHorizontal ? _gridManager.height : _gridManager.height - ship.ShipLength + 1;

                int x = Random.Range(0, xMax);
                int y = Random.Range(0, yMax);
                ship.transform.position = new Vector3(x + _gridManager.XOffset, y, -1);

                placed = placementManager.CheckForOccupy(ship, _gridManager);
            }

            ship.GetComponent<BoxCollider2D>().enabled = false;
            ship.GetComponent<SpriteRenderer>().enabled = false;
        }

        _gameManager.GameState = GameStates.PlayerTurn;
    }

    private void RotateShipRandomly(ShipBase Ship)
    {
        float angle = Random.Range(0, 2) == 0 ? 0f : 270f;
        Ship.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Ship.IsHorizontal = Ship.transform.rotation.eulerAngles.z != 0f;
    }

    private void InitShots()
    {
        _shootableTargets = _aiGridManager.GetAllTilePositions();
    }
    private void Update()
    {
        if (_gameManager.GameState == GameStates.AITurn)
        {
            _timeWaiting += Time.deltaTime;

            if (_timeWaiting >= WaitTime)
            {
                if (_turnsPlayed == _randomTurn && _canSpecialAttack && ChosenSpecialAttack != SpecialAttacks.Dutchman)
                {
                    StartCoroutine(ExecuteAISpecialAttackRoutine(ChosenSpecialAttack));
                }
                else
                {
                    StartCoroutine(ExecuteAIShotRoutine());
                }

                _turnsPlayed++;
                _gameManager.GameState = GameStates.PlayerTurn;
                _gameManager.CanPlayerAttack = true;
                _gameManager.TimerHasReset = false;
                _timeWaiting = 0;
            }
        }
        
        if (_gameManager.GameState == GameStates.PlayerTurn && _gridManager.AreAllAIShipTilesHit())
        {
            _buttonHandler.ShowVictoryScreen();
            RemoveShips();
        }
        
        if (_gameManager.GameState == GameStates.AITurn && _gridManager.AreAllPlayerShipTilesHit(GetAllPlayerOccupiedTiles().ToArray()))
        {
            _buttonHandler.ShowDefeatScreen();
        }
    }
    private IEnumerator ExecuteAIShotRoutine()
    {
        if (_shootableTargets.Count == 0)
        {
            Debug.LogWarning("No more targets to shoot.");
            yield break;
        }
        
        int index = Random.Range(0, _shootableTargets.Count);
        Vector2 shot = _shootableTargets[index];
        _shootableTargets.RemoveAt(index);
        _targetTile = _aiGridManager.GetTileAtPosition(shot);
        if (!_targetTile.IsHit)
        {
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
            if (_targetTile.IsOccupied)
            {
                _targetTile = null;
                _hasIndicator = false;
                StartCoroutine(ExecuteAIShotRoutine());
            }
        }
        else
        {
            StartCoroutine(ExecuteAIShotRoutine());
        }
    }

    public void RemoveShips()
    {
        foreach (var ship in _aiShipsToPlace)
            Destroy(ship.gameObject);
        _aiShipsToPlace.Clear();

        foreach (var ship in GameObject.FindGameObjectsWithTag("Ship"))
            Destroy(ship);
    }

    private IEnumerator ExecuteAISpecialAttackRoutine(SpecialAttacks attackType)
    {
        if (_gameManager.GameState != GameStates.AITurn || !_canSpecialAttack)
            yield break;

        switch (attackType)
        {
            case SpecialAttacks.Coin:
                yield return ExecuteCoinToss();
                break;

            case SpecialAttacks.Plus:
                yield return ExecutePlusAttack();
                break;

            case SpecialAttacks.Dutchman:
            case SpecialAttacks.Mist:
                break;
        }

        _canSpecialAttack = false;
    }
    private IEnumerator ExecuteCoinToss()
    {
        CoinAnimationObject.SetActive(true);
        bool isHeads = Random.Range(0, 2) == 1;
        SkeletonAnimation.state.SetAnimation(0, isHeads ? HeadsAnimation : TailsAnimation, false);

        var shipList = isHeads ? _aiShipsToPlace : _shipManager.Ships.FindAll(s => s.IsPlayerShip);
        if (shipList.Count == 0) yield break;

        ShipBase ship = shipList[Random.Range(0, shipList.Count)];
        var placers = ship.GetComponentsInChildren<ShipPlacer>();
        Tile tile = placers[Random.Range(0, placers.Length)].GetTile();

        while (!tile.CanBeHit)
        {
            tile = placers[Random.Range(0, placers.Length)].GetTile();
        }

        tile.OnHit();
        yield return new WaitForSeconds(WaitTime);
    }
    private IEnumerator ExecutePlusAttack()
    {
        int index = Random.Range(0, _shootableTargets.Count);
        Vector2 centerPos = _shootableTargets[index];
        _shootableTargets.RemoveAt(index);

        _targetTile = _aiGridManager.GetTileAtPosition(centerPos);

        var indicator = Instantiate(PlusIndicator, new Vector3(centerPos.x, centerPos.y, -1.5f), Quaternion.identity);
        Destroy(indicator, WaitTime);
        yield return new WaitForSeconds(WaitTime);

        Vector2[] directions = { Vector2.zero, Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        foreach (var dir in directions)
        {
            Tile tile = _aiGridManager.GetTileAtPosition(centerPos + dir);
            tile?.OnHit();
        }
    }
    public List<Tile> GetAllPlayerOccupiedTiles()
    {
        List<Tile> playerTiles = new();

        foreach (var ship in _shipManager.Ships)
        {
            if (!ship.IsPlayerShip) continue;

            ShipPlacer[] placers = ship.GetComponentsInChildren<ShipPlacer>();
            foreach (var placer in placers)
            {
                Tile tile = placer.GetTile();
                if (tile != null && tile.IsOccupied)
                {
                    playerTiles.Add(tile);
                }
            }
        }

        return playerTiles;
    }
    
}