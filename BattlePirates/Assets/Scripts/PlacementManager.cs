using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private GridManager _gridManager;
    private GameManager _gameManager;
    private GameStates _gameState;

    private void Start()
    {
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseDown()
    {
        //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, (Mathf.Sign(-transform.rotation.z) * (-transform.rotation.z + 90)), transform.rotation.w);
    }

    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9));
    }

    private void OnMouseUp()
    {
        _gameState = _gameManager.gameState;

        RaycastHit hit;
        //layermask 3 is a custom layermask for hittable tiles
        LayerMask layerMask = 3;
        Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, Mathf.Infinity, layerMask);

        Debug.Log(hit.point);

        Tile tile = _gridManager.GetTileAtPosition(hit.point);

        Debug.Log(tile.transform.position);

        if (tile != null)
        {
            if (_gameState == GameStates.PlayerTurn && _gameManager.CanPlayerAttack && tile.CanBeHit)
            {

            }

            if (_gameState == GameStates.PreparationPhase && tile.CanBePlaced)
            {
                transform.position = tile.transform.position;
            }
        }
    }
}
