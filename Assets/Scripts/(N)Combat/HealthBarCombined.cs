using UnityEngine;
using UnityEngine.UI;


public class HealthBarCombined : MonoBehaviour
{
    [Header("Alvo de HP (PlayerHits)")]
    public PlayerHits target;                  // arraste Player ou Bot

    [Header("Imagens")]
    public Image fillImage;                    // A barra que enche/esvazia (ex.: verde)
    public Image damageGhostImage;             // Opcional: “ghost” que desce mais devagar (ex.: amarelo)
    public Image frameImage;                   // Opcional: a moldura/borda (NÃO será alterada)

    [Header("Cores (opcional)")]
    public Gradient colorByPercent;            // Aplica na fillImage conforme percentual

    [Header("Animação")]
    public float lerpSpeed = 12f;              // quão rápida a barra acompanha o alvo
    public float ghostDelay = 0.15f;           // atraso antes do ghost começar a descer
    public float ghostLerp = 6f;               // velocidade do ghost descendo

    [Header("Texto (opcional)")]
    public Text hpText;                        // UI.Text com "HPAtual/Max"

    [Header("Tester (opcional)")]
    public bool enableTester = false;          // se true, tecla K aplica dano
    public KeyCode testerDamageKey = KeyCode.K;
    public float testerDamage = 10f;

    float targetFill = 1f;
    float ghostTimer = 0f;
    float currentHPcache = 1f, maxHPcache = 1f;

    void Reset()
    {
        // se soltar o script na mesma imagem da barra, preenche automático
        if (!fillImage) fillImage = GetComponent<Image>();
    }

    void Awake()
    {
        if (!fillImage) fillImage = GetComponent<Image>();
        EnsureFilled(fillImage);
        EnsureFilled(damageGhostImage);
    }

    void OnEnable()
    {
        if (target != null)
        {
            target.OnHealthChanged += HandleHealthChanged;
            target.OnHitReceived += HandleHit; // para saber o dano exato (opcional)
            // estado inicial
            HandleHealthChanged(target.CurrentHP, target.MaxHP);
            SetInstant(target.CurrentHP, target.MaxHP);
        }
        else
        {
            SetInstant(1f, 1f); // começa cheia sem alvo
        }
    }

    void OnDisable()
    {
        if (target != null)
        {
            target.OnHealthChanged -= HandleHealthChanged;
            target.OnHitReceived  -= HandleHit;
        }
    }

    public void SetTarget(PlayerHits newTarget)
    {
        if (target == newTarget) return;
        if (target != null)
        {
            target.OnHealthChanged -= HandleHealthChanged;
            target.OnHitReceived  -= HandleHit;
        }
        target = newTarget;
        if (isActiveAndEnabled && target != null)
        {
            target.OnHealthChanged += HandleHealthChanged;
            target.OnHitReceived  += HandleHit;
            HandleHealthChanged(target.CurrentHP, target.MaxHP);
            SetInstant(target.CurrentHP, target.MaxHP);
        }
    }

    void Update()
    {
        // Tester de dano (apenas depuração)
        if (enableTester && target != null && Input.GetKeyDown(testerDamageKey))
        {
            target.ReceiveHit(new HitData { amount = testerDamage });
        }

        if (fillImage)
        {
            // anima o fill até a meta
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFill, lerpSpeed * Time.deltaTime);
            if (colorByPercent != null)
                fillImage.color = colorByPercent.Evaluate(fillImage.fillAmount);
        }

        // Ghost bar: só desce (nunca sobe acima do fill)
        if (damageGhostImage)
        {
            if (ghostTimer > 0f) ghostTimer -= Time.deltaTime;
            else
            {
                float targetGhost = Mathf.Max(targetFill, damageGhostImage.fillAmount); // impede subir
                damageGhostImage.fillAmount = Mathf.MoveTowards(damageGhostImage.fillAmount, targetGhost, ghostLerp * Time.deltaTime);
            }
        }

        // Texto opcional
        if (hpText)
        {
            hpText.text = $"{Mathf.CeilToInt(currentHPcache)}/{Mathf.CeilToInt(maxHPcache)}";
        }
    }

    // ===== Eventos do PlayerHits =====
    void HandleHealthChanged(float current, float max)
    {
        currentHPcache = current;
        maxHPcache = Mathf.Max(1f, max);

        float newFill = Mathf.Clamp01(current / maxHPcache);

        // se tomou dano (diminuiu), dá um atraso antes do ghost começar a descer
        if (damageGhostImage && newFill < targetFill)
            ghostTimer = ghostDelay;

        targetFill = newFill;
    }

    void HandleHit(HitData hit)
    {
        // Se quiser, pode exibir um número de dano aqui (popup)
        // Ex.: Debug.Log($"Dano recebido: {hit.amount}");
    }

    // ===== Util =====
    void SetInstant(float current, float max)
    {
        float f = Mathf.Clamp01((max > 0f) ? current / max : 0f);
        targetFill = f;

        if (fillImage) fillImage.fillAmount = f;
        if (damageGhostImage) damageGhostImage.fillAmount = f;

        if (colorByPercent != null && fillImage)
            fillImage.color = colorByPercent.Evaluate(f);
    }

    static void EnsureFilled(Image img)
    {
        if (!img) return;
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        // Ajuste o Fill Origin (Left/Right) no Inspector conforme barra esquerda/direita
    }
}

