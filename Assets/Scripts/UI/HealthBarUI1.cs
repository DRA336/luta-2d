using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBarUI : MonoBehaviour
{
    [Header("Arraste aqui o Player/Bot (PlayerHits)")]
    public PlayerHits target;
    public GeralBot targetBot; // Caso queira usar com Bots

    [Header("Imagem de Fill (se vazio, usa a própria)")]
    public Image fillImage;

    [Header("Cor por porcentagem (opcional)")]
    public Gradient colorByPercent;

    [Header("Suavização")]
    public float lerpSpeed = 10f;

    float targetFill = 1f;

    void Awake()
    {
        if (!fillImage) fillImage = GetComponent<Image>();
        if (fillImage)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            // defina o Fill Origin (Left/Right) no Inspector
        }
    }

    void OnEnable()
    {
        if (target != null) target.OnHealthChanged += HandleHealthChanged;
        if (target != null) SetInstant(target.CurrentHP, target.MaxHP);
    }

    void OnDisable()
    {
        if (target != null) target.OnHealthChanged -= HandleHealthChanged;
    }

    public void SetTarget(PlayerHits newTarget)
    {
        if (target == newTarget) return;
        if (target != null) target.OnHealthChanged -= HandleHealthChanged;
        target = newTarget;
        if (isActiveAndEnabled && target != null)
        {
            target.OnHealthChanged += HandleHealthChanged;
            SetInstant(target.CurrentHP, target.MaxHP);
        }
    }

    void HandleHealthChanged(float current, float max)
    {
        targetFill = max > 0 ? Mathf.Clamp01(current / max) : 0f;
    }

    void Update()
    {
        if (!fillImage) return;
        fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
        if (colorByPercent != null) fillImage.color = colorByPercent.Evaluate(fillImage.fillAmount);
    }

    void SetInstant(float current, float max)
    {
        targetFill = max > 0 ? Mathf.Clamp01(current / max) : 0f;
        if (!fillImage) return;
        fillImage.fillAmount = targetFill;
        if (colorByPercent != null) fillImage.color = colorByPercent.Evaluate(targetFill);
    }
}
