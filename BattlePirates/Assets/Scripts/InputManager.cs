using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    private Vector3 LastPosition;
    [SerializeField] private LayerMask PlacementLayerMask;
    [SerializeField] private Texture2D _Cursor;

    public delegate void InputAction();

    public static event InputAction Click;

    public static event InputAction OpenMenu;

    private void Start()
    {
        Cursor.SetCursor(_Cursor, Vector2.zero, CursorMode.Auto);
        
    }

    public Vector3 GetSelectedPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.nearClipPlane;
        Ray ray = Camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, PlacementLayerMask))
        {
            LastPosition = hit.point;
        }

        return LastPosition;
    }

    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            OpenMenu();

        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            Click();

        }
    }
}
