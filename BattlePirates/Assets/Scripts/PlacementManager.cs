using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private List<Tile> _oldTiles = new List<Tile>();
    private List<Tile> _placementTiles = new List<Tile>();
    private ShipBase _ship;
    private int _unoccupiedTiles = 0;
    private Vector3 _oldPosition;

    private void Start()
    {
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
        }
        PlaceShip();
    }

    private void PlaceShip()
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
        _placementTiles.Clear();
    }
}
    
