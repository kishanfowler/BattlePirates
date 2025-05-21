using System;
using Spine.Unity;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private ShipManager _shipManager;
    private GameStates _gameState;
    public bool CanPlayerSpecialAttack = true;
    private bool _plusAttack = false;
    private bool _hasClicked = false;
    public GameObject MistPrefab;
    public GameObject PlusIndicator;
    private bool _canAISpecialAttack = true;


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
                    CanPlayerSpecialAttack = false;
                    break;
                case SpecialAttacks.Plus:
                    PlusIndicator = Instantiate(PlusIndicator, new Vector3(0, 0, 10), Quaternion.identity);
                    _plusAttack = true;
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
