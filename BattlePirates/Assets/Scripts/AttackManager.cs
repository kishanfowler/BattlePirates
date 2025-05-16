using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private ShipManager _shipManager;
    private AIManager _aiManager;
    private GameStates _gameState;
    private bool _canAISpecialAttack = true;
    private bool _canPlayerSpecialAttack = true;
    private bool _plusAttack = true;
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
            Attack();
        } 

        if (Input.GetKey(KeyCode.M))
        {
            DoSpecialAttack(SpecialAttacks.Mist);
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
        if (_gameManager.GameState == GameStates.PlayerTurn && _canPlayerSpecialAttack)
        {
            switch (attackType)
            {
                case SpecialAttacks.Mist:
                    Instantiate(MistPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -2), Quaternion.identity);
                    _canPlayerSpecialAttack = false;
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
                    _canPlayerSpecialAttack = false;
                    break;
                case SpecialAttacks.Plus:
                    var screenPosition = Input.mousePosition;
                    screenPosition.z = 9f; // z-distance from camera
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

                    // Instantiate the indicator at the world position
                    var indicator = Instantiate(PlusIndicator, worldPosition, Quaternion.identity);

                    while (_plusAttack)
                    {
                        // Update indicator position
                        screenPosition = Input.mousePosition;
                        screenPosition.z = 9f;
                        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                        indicator.transform.position = worldPosition;

                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            Tile centerTile = _gridManager.GetTileAtPosition(worldPosition);
                            if (centerTile != null)
                            {
                                centerTile.OnHit();

                                // Define 4 cardinal directions
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
                                _canPlayerSpecialAttack = false;
                                break;
                            }
                        }
                    }
                    _canPlayerSpecialAttack = false;
                    break;
                /*var indicator = Instantiate(PlusIndicator, new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1), Quaternion.identity);
                while (_plusAttack)
                {
                    indicator.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Tile tile = _gridManager.GetTileAtPosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9)));
                        if (tile)
                        {
                            tile.OnHit();
                            if (_gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x + 1, tile.transform.position.y)))
                            {
                                _gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x + 1, tile.transform.position.y)).OnHit();
                            }
                            if (_gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x - 1, tile.transform.position.y)))
                            {
                                _gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x - 1, tile.transform.position.y)).OnHit();
                            }
                            if (_gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x, tile.transform.position.y + 1)))
                            {
                                _gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x, tile.transform.position.y + 1)).OnHit();
                            }
                            if (_gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x, tile.transform.position.y - 1)))
                            {
                                _gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x, tile.transform.position.y - 1)).OnHit();
                            }
                            _plusAttack = false;
                            _canPlayerSpecialAttack = false;
                            break;
                        }
                    }
                }
                _canPlayerSpecialAttack = false;
                break;*/
                case SpecialAttacks.Dutchman:
                    var ship = Instantiate(DutchManPrefab, new Vector3(-8, 6, -1), quaternion.identity);
                    ship.GetComponent<ShipBase>().IsPlayerShip = true;
                    _canPlayerSpecialAttack = false;
                    break;
            }
        }
        if(_gameManager.GameState == GameStates.AITurn && _canAISpecialAttack)
        {

        }
    }
}
