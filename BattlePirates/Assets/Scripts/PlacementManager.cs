using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GameStates _gameState;
    private List<Tile> _oldTiles = new List<Tile>();
    private List<Tile> _placementTiles = new List<Tile>();
    private Tile _tile;
    private int _index = 0;
    private ShipBase _ship;

    private void Start()
    {
        _ship = gameObject.GetComponent<ShipBase>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseDown()
    {
        if (_oldTiles.Count > 0)
        {
            foreach (Tile _oldTile in _oldTiles)
            {
                _oldTile.OnDeoccupy();
            }
        }

        gameObject.transform.Rotate(0, 0, 90, Space.World);
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
    }

    private void OnMouseUp()
    {
        _gameState = _gameManager.GameState;

        foreach (ShipPlacer item in gameObject.GetComponentsInChildren<ShipPlacer>())
        {
            _tile = item.GetTile();
            _placementTiles.Add(_tile);
            if(_tile) 
            {
                _index++;
            }

            if (_index == gameObject.GetComponentsInChildren<ShipPlacer>().Length)
            {
                PlaceShip();
            }   
        }
    }

    private void PlaceShip()
    {
        if (_gameState == GameStates.PreparationPhase && _tile.IsOccupied == false)
        {
            transform.position = new Vector3(_placementTiles[GameManager.BetterClamp((_ship.ShipLength - 1), 1, 3)].transform.position.x, _placementTiles[GameManager.BetterClamp(_ship.ShipLength-1,1,3)].transform.position.y, -1);
            foreach (Tile PlacementTile in _placementTiles)
            {
                PlacementTile.OnOccupy();
            }
            _oldTiles = _placementTiles;
        }
        _placementTiles.Clear();
        _index = 0;
    }

     
}
    
