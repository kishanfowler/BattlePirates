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

    private void OnMouseDown()
    {
        Attack();
    }

    void Attack()
    {
        Tile tile = _gridManager.GetTileAtPosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x * 1.05f, Input.mousePosition.y * 1.05f, -1)));
        _gameState = _gameManager.gameState;
        if (_gameState == GameStates.PlayerTurn && _gameManager.CanPlayerAttack && tile.CanBeHit)
        {
            tile.OnHit();
        }
    }
}
