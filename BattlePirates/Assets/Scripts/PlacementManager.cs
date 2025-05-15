using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GameStates _gameState;
    private GridManager _gridManager;
    private List<Tile> _oldTiles = new();
    private List<Tile> _placementTiles = new();
    private List<Vector2> _occupiedTileList;
    private ShipBase _ship;
    private int _unoccupiedTiles = 0;
    private Vector3 _oldPosition;
    private GameObject _SelectedObject;
    public List<Vector2> tilesToOccupy;

    private void Awake()
    {
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        _ship = gameObject.GetComponent<ShipBase>();
    }

    private void OnMouseDown()
    {
        if (_oldTiles.Count > 0)
        {
            for (int i = 0; i < _oldTiles.Count; i++)
            {
                _oldTiles[i].OnDeoccupy();
            }
        }

        _oldPosition = gameObject.transform.position;
    }

    private void Update()
    {
        // Als je de linkermuisknop indrukt
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                _SelectedObject = hit.collider.gameObject;
            }
        }

        // Als muis wordt vastgehouden en E wordt ingedrukt
        if (Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.E))
        {
            if (_SelectedObject != null)
            {
                _SelectedObject.transform.Rotate(0, 0, -90, Space.World);
            }
        }
        if (Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.Q))
        {
            if (_SelectedObject != null)
            {
                _SelectedObject.transform.Rotate(0, 0, 90, Space.World);
            }
        }
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
    }

    private void OnMouseUp()
    {
        if (_ship.OccupiedTileLocations != null && _occupiedTileList != null)
        {
            _occupiedTileList.Clear();
            _ship.OccupiedTileLocations.Clear();
        }
        _SelectedObject = null;
        TryPlaceShip();
    }

    public void TryPlaceShip()
    {
        for (int i = 0; i < gameObject.GetComponentsInChildren<ShipPlacer>().Length; i++)
        {
            if (gameObject.GetComponentsInChildren<ShipPlacer>()[i].GetTile())
            {
                _placementTiles.Add(gameObject.GetComponentsInChildren<ShipPlacer>()[i].GetTile());
                _ship.OccupiedTileLocations.Add(_placementTiles[i].GridPosition);
            }
            else
            {
                break;
            }
        }
        _occupiedTileList = _ship.OccupiedTileLocations;
        if(_occupiedTileList.Count == _ship.ShipLength)
        {
            PlaceShip();
        }
        else
        {
            _ship.transform.position = _oldPosition;
        }
    }

    public void PlaceShip()
    {
        if (_ship.OccupiedTileLocations != null)
        {
            for (int i = 0; i < _ship.OccupiedTileLocations.Count; i++)
            {
                transform.position = new Vector3(_ship.OccupiedTileLocations[i].x,_ship.OccupiedTileLocations[i].y,-1);
            }
            for (int i = 0; i < _occupiedTileList.Count; i++)
            {
                _gridManager.GetTileAtPosition(_occupiedTileList[i]).OnOccupy();
                _oldTiles.Add(_gridManager.GetTileAtPosition(_occupiedTileList[i]));
            }
            _unoccupiedTiles = 0;
        }
        else
        {
            for (int i = 0; i < _placementTiles.Count; i++)
            {
                if(_placementTiles[i].IsOccupied == false)
                {
                    _unoccupiedTiles++;
                }
            }
            if (_unoccupiedTiles == _placementTiles.Count)
            {
                transform.position = new Vector3(_placementTiles[GameManager.BetterClamp((_ship.ShipLength - 1), 1, 3)].transform.position.x, _placementTiles[GameManager.BetterClamp(_ship.ShipLength - 1, 1, 3)].transform.position.y, -1);
                for (int i = 0; i < _placementTiles.Count; i++)
                {
                    _placementTiles[i].OnOccupy();
                }
                _oldTiles = _placementTiles;
                _unoccupiedTiles = 0;
            }
            else
            {
                gameObject.transform.position = _oldPosition;
            }
        }
        _placementTiles.Clear();
    }

    public bool CheckForOccupy(ShipBase Ship, GridManager gridManager)
    {
        var tilePositions = gridManager.GetAllTilePositions();
        Vector2 direction = Ship.IsHorizontal? Vector2.right: Vector2.up;
        var position = Ship.transform.position;
        var shipStartPos = new Vector2(position.x, position.y);
        bool overlap = false;
        tilesToOccupy = new List<Vector2>();


        for (int i = 0; i < Ship.ShipLength; i++)
        {
            Vector2 shipTilePos = shipStartPos + direction * i;

            // Check: bestaat de tile überhaupt?
            if (!tilePositions.Contains(shipTilePos))
            {
                return false;
            }

            // Check: is de tile al bezet?
            if (gridManager.IsTileOccupied(shipTilePos))
            {
                return false;
            }

            tilesToOccupy.Add(shipTilePos);
        }
        foreach (var pos in tilesToOccupy)
        {
            gridManager.SetTileOccupied(pos,true);
        }
        return true;
    }


}
    
