using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool CanBePlaced;
    public bool CanBeHit;
    [SerializeField] private GameObject Highlight;

    void OnPlace()
    {
        CanBePlaced = false;
    }

    void OnHit()
    {
        CanBeHit = false;

    }

    private void OnMouseEnter()
    {
        Highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        Highlight.SetActive(false);
    }
}
