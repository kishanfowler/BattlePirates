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

        gameObject.transform.Rotate(0, 0, 90, Space.World);
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
    }

    private void OnMouseUp()
    {
        TryPlaceShip();
    }

    public void TryPlaceShip()
    {
        for (int i = 0; i < gameObject.GetComponentsInChildren<ShipPlacer>().Length; i++)
        {
            _placementTiles.Add(gameObject.GetComponentsInChildren<ShipPlacer>()[i].GetTile());
            _ship.OccupiedTileLocations.Add(_placementTiles[i].GridPosition);
        }
        _occupiedTileList = _ship.OccupiedTileLocations;
        PlaceShip();
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

    public bool CheckForOccupy(ShipBase Ship)
    {
        var tilePositions = _gridManager.GetAllTilePositions();
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
            if (_gridManager.IsTileOccupied(shipTilePos))
            {
                return false;
            }

            tilesToOccupy.Add(shipTilePos);
        }
        foreach (var pos in tilesToOccupy)
        {
            _gridManager.SetTileOccupied(pos,true);
        }
        return true;
    }


}
    
