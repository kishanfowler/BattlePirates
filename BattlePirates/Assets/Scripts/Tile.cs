using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsOccupied;
    public bool CanBeHit;
    [SerializeField] private GameObject Highlight;

    public void OnOccupy()
    {
        IsOccupied = true;
    }

    public void OnDeoccupy()
    {
        IsOccupied = false;
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
