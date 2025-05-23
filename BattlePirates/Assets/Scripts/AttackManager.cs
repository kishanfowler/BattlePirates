using Spine.Unity;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private ShipManager _shipManager;
    private AIManager _aiManager;
    private GameStates _gameState;
    public bool CanPlayerSpecialAttack = true;
    private bool _plusAttack = false;
    private bool _hasClicked = false;
    public GameObject MistPrefab;
    public GameObject PlusIndicator;
    public GameObject DutchManPrefab;
    public SkeletonAnimation _SkeletonAnimation;
    public AnimationReferenceAsset HeadsAnimation;
    public AnimationReferenceAsset TailsAnimation;
    public GameObject CoinAnimationObject;

    public enum SpecialAttacks
    {
        Mist,
        Coin,
        Plus,
        Dutchman
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        _shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
        _aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!_plusAttack)
            {
                Attack();
            }
            else
            {
                HandlePlusAttack(PlusIndicator.transform.position);
            }
        }

        if (_plusAttack)
        {
            PlusIndicator.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void Attack()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // afstand tot camera bij orthografisch
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Tile tile = _gridManager.GetTileAtPosition(worldPos);
        _gameState = _gameManager.GameState;
        if (!tile) return;
        if (_gameState == GameStates.PlayerTurn && _gameManager.CanPlayerAttack && tile.CanBeHit)
        {
            tile.OnHit();
            if (!tile.IsOccupied)
            {
                _gameManager.GameState = GameStates.AITurn;
                _gameManager.CanPlayerAttack = false;
                _gameManager.TimerHasReset = false;
            }
        }
    }

    public void DoSpecialAttack(SpecialAttacks attackType)
    {
        if (_gameManager.GameState == GameStates.PlayerTurn && CanPlayerSpecialAttack)
        {
            switch (attackType)
            {
                case SpecialAttacks.Mist:
                    Instantiate(MistPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -2), Quaternion.identity);
                    CanPlayerSpecialAttack = false;
                    break;
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
                        int randomShip = UnityEngine.Random.Range(0, _aiManager.AIShipsToPlace.Count);
                        while (_aiManager.AIShipsToPlace[randomShip].IsPlayerShip)
                        {
                            randomShip = UnityEngine.Random.Range(0, _aiManager.AIShipsToPlace.Count);
                            if (!_aiManager.AIShipsToPlace[randomShip].IsPlayerShip)
                            {
                                break;
                            }
                        }
                        if (!_aiManager.AIShipsToPlace[randomShip].IsPlayerShip)
                        {
                            Tile tile = _aiManager.AIShipsToPlace[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _aiManager.AIShipsToPlace[randomShip].ShipLength)].GetTile();
                            while (tile.CanBeHit == false)
                            {
                                tile = _aiManager.AIShipsToPlace[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _aiManager.AIShipsToPlace[randomShip].ShipLength)].GetTile();
                                break;
                            }
                            tile.OnHit();
                        }
                    }
                    CanPlayerSpecialAttack = false;
                    break;
                case SpecialAttacks.Plus:
                    PlusIndicator = Instantiate(PlusIndicator, new Vector3(0, 0, 10), Quaternion.identity);
                    _plusAttack = true;
                    break;
                case SpecialAttacks.Dutchman:
                    var ship = Instantiate(DutchManPrefab, new Vector3(-8, 6, -1), quaternion.identity);
                    ship.GetComponent<ShipBase>().IsPlayerShip = true;
                    CanPlayerSpecialAttack = false;
                    break;
            }
        }
    }

    private void HandlePlusAttack(Vector3 IndicatorPosition)
    {
        Tile centerTile = _gridManager.GetTileAtPosition(IndicatorPosition);
        if (centerTile != null)
        {
            centerTile.OnHit();

            Vector2[] directions = new Vector2[]
            {
                Vector2.right,
                Vector2.left,
                Vector2.up,
                Vector2.down
            };

            foreach (Vector2 dir in directions)
            {
                Vector2 neighborPos = (Vector2)centerTile.transform.position + dir;
                Tile neighborTile = _gridManager.GetTileAtPosition(neighborPos);
                if (neighborTile != null)
                {
                    neighborTile.OnHit();
                }
            }

            _plusAttack = false;
            CanPlayerSpecialAttack = false;
        }
    }
}
