using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsOccupied;
    public bool CanBeHit;
    public bool IsHit;
    public Vector2 GridPosition { get; private set; }
    [SerializeField] private GameObject Highlight;
    [SerializeField] private Sprite HitSprite;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject HitEffectPrefab;
    public void Init(Vector2 Position)
    {
        GridPosition = Position;
    }
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBaseSprite(Sprite sprite)
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprite != null)
            _spriteRenderer.sprite = sprite;
        else
            Debug.LogWarning($"Geen sprite toegewezen aan tile op positie {GridPosition}");
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
            if (HitEffectPrefab != null)
            {
                // Instantieer effect boven de tile, iets lager op de Z-as
                Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y, -1.5f);
                Instantiate(HitEffectPrefab, effectPosition, Quaternion.identity);
            }
            else if (HitSprite != null)
            {
                // Fallback: maak het object met sprite aan in code
                GameObject hitVisual = new GameObject("HitEffect");
                hitVisual.transform.position = new Vector3(transform.position.x, transform.position.y, -1.5f);
                var sr = hitVisual.AddComponent<SpriteRenderer>();
                sr.sprite = HitSprite;
                sr.sortingOrder = 1; // hoger dan tile zelf
            }
            else
            {
                Debug.LogWarning("Geen HitEffectPrefab of HitSprite ingesteld!");
            }
        }
        else
        {
            Sprite miss = Resources.Load<Sprite>($"MapWithHoles/{gameObject.name}");
            if (miss != null)
            {
                _spriteRenderer.sprite = miss;
            }
            else
            {
                Debug.LogWarning($"Miss-sprite niet gevonden voor: {gameObject.name}");
            }
        }
    }
}
