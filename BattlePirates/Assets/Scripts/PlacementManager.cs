using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private List<Tile> _oldTiles = new();
    private List<Tile> _placementTiles = new();
    private ShipBase _ship;
    private int _unoccupiedTiles = 0;
    private Vector3 _oldPosition;
    private GameObject _SelectedObject;
    private bool _mistPlaced = false;
    private bool _shipCanPlace = false;
    public List<Vector2> tilesToOccupy;
    private bool _actionDone;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
        _gameManager = GameManager.GameManagerInstance;
        _ship = gameObject.GetComponent<ShipBase>();
        if (_gameManager.GameState == GameStates.PreparationPhase)
        {
            _shipCanPlace = true;
        }
    }

    private void OnMouseDown()
    {
        if (_shipCanPlace || !_mistPlaced)
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
    }

    private void Update()
    {
        if (_gameManager.GameState != GameStates.PreparationPhase)
        {
            _shipCanPlace = false;
        }
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

        if (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.E) && !_actionDone)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                transform.Rotate(0, 0, -90, Space.World);
                _actionDone = true;
                StartCoroutine(ResetAction());
            }
        }

        if (Input.GetMouseButton(0) && Input.GetKeyUp(KeyCode.Q) && !_actionDone)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                transform.Rotate(0, 0, 90, Space.World);
                _actionDone = true;
                StartCoroutine(ResetAction());
            }
        }
    }

    private void OnMouseDrag()
    {
        if (_shipCanPlace || !_mistPlaced)
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
        }
    }

    private void OnMouseUp()
    {

        if (_SelectedObject != _ship && !_mistPlaced)
        {
            PlaceMist();
        }
        if(_shipCanPlace)
        {
            if (_ship.OccupiedTileLocations != null)
            {
                _ship.OccupiedTileLocations.Clear();
            }
            TryPlaceShip();
        }
        _SelectedObject = null;
    }

    public void TryPlaceShip()
    {
        for (int i = 0; i < gameObject.GetComponentsInChildren<ShipPlacer>().Length; i++)
        {
            if (gameObject.GetComponentsInChildren<ShipPlacer>()[i].GetTile())
            {
                _placementTiles.Add(gameObject.GetComponentsInChildren<ShipPlacer>()[i].GetTile());
                _ship.OccupiedTileLocations.Add(_placementTiles[i].TileMiddle);
            }
            else
            {
                break;
            }
        }
        if(_ship.OccupiedTileLocations.Count == _ship.ShipLength)
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
        Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        var tile = _gridManager.GetTileAtWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (tile)
        {
            _ship.transform.position = tile.TileMiddle;
        }
        if (_ship.OccupiedTileLocations != null)
        {
            for (int i = 0; i < _ship.OccupiedTileLocations.Count; i++)
            {
                transform.position = new Vector3(tile.TileMiddle.x, tile.TileMiddle.y, - 1);
            }
            for (int i = 0; i < _ship.OccupiedTileLocations.Count; i++)
            {
                _gridManager.GetTileAtWorldPosition(_ship.OccupiedTileLocations[i]).OnOccupy();
                _oldTiles.Add(_gridManager.GetTileAtWorldPosition(_ship.OccupiedTileLocations[i]));
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

    private void PlaceMist()
    {
        _mistPlaced = true;
    }
    private IEnumerator ResetAction()
    {
        yield return new WaitForEndOfFrame(); // of eventueel `yield return null;`
        _actionDone = false;
    }
}
    
