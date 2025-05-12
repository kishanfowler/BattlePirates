using UnityEngine;

public class ShipPlacer : MonoBehaviour
{
    public LayerMask LayerMask;

    public Tile GetTile()
    {
        var pos = new Vector2(transform.position.x, transform.position.y);
        var hit = Physics2D.OverlapPoint(pos, LayerMask);

        if (hit == null)
        {
            Debug.LogWarning($"[GetTile] Geen collider geraakt op positie {pos} voor {gameObject.name}");
            return null;
        }

        var tile = hit.GetComponent<Tile>();
        if (tile == null)
        {
            Debug.LogError($"[GetTile] Collider geraakt ({hit.name}) maar geen Tile component erop!");
            return null;
        }

        return tile;
    }
}