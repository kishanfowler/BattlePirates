using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private ShipManager _shipManager;
    private GameStates _gameState;
    public GameObject MistPrefab;
    public GameObject PlusIndicator;
    public GameObject DutchManPrefab;
    private bool _canAISpecialAttack = true;
    private bool _canPlayerSpecialAttack = true;
    private bool _plusAttack = true;


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
        _shipManager = GameObject.Find("GridManager").GetComponent<ShipManager>();

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Attack();
        } 
    }

    void Attack()
    {
        // //position * 1.05 because of the orthographic projection, otherwise the last row and line of the grid get skipped.
        // Tile tile = _gridManager.GetTileAtPosition(Camera.main.ScreenToWorldPoint(new Vector3((Input.mousePosition.x + Input.mousePosition.x) /1.95f, (Input.mousePosition.y+Input.mousePosition.y)/1.9f, -1)));
        // _gameState = _gameManager.GameState;
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

    void SpecialAttack(SpecialAttacks attackType)
    {
        if (_gameManager.GameState == GameStates.PlayerTurn && _canPlayerSpecialAttack)
        {
            switch (attackType)
            {
                case SpecialAttacks.Mist:
                    Instantiate(MistPrefab, new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1), Quaternion.identity);
                    _canPlayerSpecialAttack = false;
                    break;
                case SpecialAttacks.Coin:
                    //animatie stuffs
                    if (UnityEngine.Random.Range(0, 1) == 0)
                    {
                        int randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                        while (!_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                            break;
                        }
                        if (_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            Tile tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                            while (tile.CanBeHit == false)
                            {
                                tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                                break;
                            }
                            tile.OnHit();
                        }
                    }
                    else
                    {
                        int randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                        while (_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            randomShip = UnityEngine.Random.Range(0, _shipManager._ships.Count);
                            break;
                        }
                        if (!_shipManager._ships[randomShip].IsPlayerShip)
                        {
                            Tile tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                            while (tile.CanBeHit == false)
                            {
                                tile = _shipManager._ships[randomShip].GetComponentsInChildren<ShipPlacer>()[UnityEngine.Random.Range(0, _shipManager._ships[randomShip].ShipLength)].GetTile();
                                break;
                            }
                            tile.OnHit();
                        }
                    }
                    _canPlayerSpecialAttack = false;
                    break;
                case SpecialAttacks.Plus:
                    var indicator = Instantiate(PlusIndicator, new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1), Quaternion.identity);
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
                                if (_gridManager.GetTileAtPosition(new Vector2(tile.transform.position.x, tile.transform.position.y +1)))
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
                    break;
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
