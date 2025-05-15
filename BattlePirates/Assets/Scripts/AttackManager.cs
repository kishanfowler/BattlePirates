using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private GameManager _gameManager;
    private GridManager _gridManager;
    private GameStates _gameState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
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
        //position * 1.05 because of the orthographic projection, otherwise the last row and line of the grid get skipped.
        Tile tile = _gridManager.GetTileAtPosition(Camera.main.ScreenToWorldPoint(new Vector3((Input.mousePosition.x + Input.mousePosition.x) /1.95f, (Input.mousePosition.y+Input.mousePosition.y)/1.9f, -1)));
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
}
