using UnityEngine;

public class ShipPlacer : MonoBehaviour
{
    public LayerMask LayerMask;

    public Tile GetTile()
    {
        return Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y), LayerMask).GetComponent<Tile>();
    }
}
