using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private InputManager InputManager;
    [SerializeField] public GridManager GridManager;

    void OnEnable()
    {
        InputManager.Click += Click;

        InputManager.OpenMenu += OpenMenu;
    }

    void OnDisable()
    {
        InputManager.Click -= Click;

        InputManager.OpenMenu -= OpenMenu;
    }

    void Click()
    {
        if (GridManager.GetTileAtPosition(InputManager.GetSelectedPosition()).CanBeHit)
        {

        }
    }

    void OpenMenu()
    {

    }
}
