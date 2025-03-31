using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    private GridManager _gridManager;
    private GameManager _gameManager;
    private GameStates _gameState;
    private Tile _oldTile;

    private void Start()
    {
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseDown()
    {
        if (_oldTile)
        {
            _oldTile.OnDeoccupy();
        }
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
    }

    private void OnMouseUp()
    {
        _gameState = _gameManager.gameState;
        Tile tile = _gridManager.GetTileAtPosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x*1.05f, Input.mousePosition.y*1.05f, -1)));

        if (tile != null)
        {
            if (_gameState == GameStates.PlayerTurn && _gameManager.CanPlayerAttack && tile.CanBeHit)
            {

            }

            if (_gameState == GameStates.PreparationPhase && tile.IsOccupied == false)
            {
                transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -1);
                tile.OnOccupy();
                _oldTile = tile;
                _oldTile.OnDeoccupy();
            }
        }
    }
}
