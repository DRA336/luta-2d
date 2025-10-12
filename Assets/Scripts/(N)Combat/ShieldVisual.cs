using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShieldVisual : MonoBehaviour
{
    [Header("Aparência")]
    public SpriteRenderer sr;
    public Color idleColor = new Color(0.4f, 0.8f, 1f, 0.5f);
    public Color flashColor = new Color(0.9f, 1f, 1f, 0.9f);
    public float showScale = 1.0f;
    public float hideScale = 0.2f;
    public float appearSpeed = 12f;
    public float disappearSpeed = 16f;
    public float flashTime = 0.08f;

    [Header("Seguimento (opcional)")]
    public Transform followTarget;
    public Vector3 followOffset = Vector3.zero;

    [Header("Controle de collider (opcional)")]
    public bool manageCollider = false;       // <<< deixe FALSE
    public Collider2D defenseCollider;        // arraste apenas se for usar o modo acima

    bool visible = false;
    float flashUntil = 0f;

    void Reset(){ sr = GetComponent<SpriteRenderer>(); }

    void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * hideScale;
        SetAlpha(0f);
    }

    void Update()
    {
        if (followTarget) transform.position = followTarget.position + followOffset;

        // flash
        if (sr)
        {
            if (Time.time < flashUntil) sr.color = Color.Lerp(sr.color, flashColor, 0.5f);
            else sr.color = Color.Lerp(sr.color, idleColor, 0.2f);
        }

        // scale/alpha
        float speed = visible ? appearSpeed : disappearSpeed;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * (visible ? showScale : hideScale), Time.deltaTime * speed);

        float targetA = visible ? idleColor.a : 0f;
        var c = sr.color; c.a = Mathf.Lerp(c.a, targetA, Time.deltaTime * speed);
        sr.color = c;
    }

    void SetAlpha(float a){ var c = sr.color; c.a = a; sr.color = c; }

    // ===== API =====
    public void Show()
    {
        visible = true;
        if (manageCollider && defenseCollider) defenseCollider.enabled = true; // só se você QUISER que o visual controle
    }

    public void Hide()
    {
        visible = false;
        if (manageCollider && defenseCollider) defenseCollider.enabled = false;
    }

    public void Flash(){ flashUntil = Time.time + flashTime; }
}
