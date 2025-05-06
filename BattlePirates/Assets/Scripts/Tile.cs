using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsOccupied;
    public bool CanBeHit;
    public bool IsHit;
    [SerializeField] private GameObject Highlight;
    [SerializeField] private Sprite HitSprite;
    [SerializeField] private Sprite MissSprite;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnOccupy()
    {
        IsOccupied = true;
    }

    public void OnDeoccupy()
    {
        IsOccupied = false;
    }

    public void OnHit()
    {
        CanBeHit = false;
        IsHit = true;
        if (Highlight != null)
        {
            Highlight.SetActive(false);
            Highlight = null;
        }
        ChangeSprite();
    }

    private void OnMouseEnter()
    {
        if (Highlight)
        {
            Highlight.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (Highlight)
        {
            Highlight.SetActive(false);
        }
    }

    private void ChangeSprite()
    {
        if (IsOccupied)
        {
            _spriteRenderer.sprite = HitSprite;
        }
        else
        {
            _spriteRenderer.sprite = MissSprite;
        }
    }
}
