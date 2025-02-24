using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public float heartFadeDuration = 0.3f;   // 🌈 Heart fade duration
    public float heartScaleDuration = 0.2f;  // 🕯️ Heart scale animation

    [Header("Effects")]
    public ParticleSystem hitParticles;
    public Camera mainCamera;
    public float cameraShakeDuration = 0.1f;
    public float cameraShakeMagnitude = 0.1f;

    [Header("Screen Flash")]
    public Image screenFlashImage;           // 🔥 Fullscreen UI Image for flash
    public Color flashColor = new Color(1, 0, 0, 0.5f);
    public float screenFlashDuration = 0.2f;
    public int criticalHealthThreshold = 2;  // Flash when health <= this

    [Header("Knockback Settings")]
    private bool isKnockback = false;
    public float baseKnockbackForce = 5f;
    public float knockbackScaling = 2f;

    [Header("Sprite Flash Settings")]
    public SpriteRenderer playerSprite;
    public Color spriteFlashColor = Color.red;
    public float spriteFlashDuration = 0.1f;

    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        originalColor = playerSprite.color;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= criticalHealthThreshold) StartCoroutine(ScreenFlash()); // 🔥 Critical screen flash
        if (currentHealth <= 0) Die();
        else StartCoroutine(DamageReaction(hitDirection));
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
                hearts[i].color = Color.white;
                StartCoroutine(ScaleHeart(hearts[i].transform, Vector3.one * 1.2f, Vector3.one)); // 🕯️ Scale up when gained
            }
            else
            {
                hearts[i].sprite = emptyHeart;
                StartCoroutine(FadeHeart(hearts[i]));
                StartCoroutine(ScaleHeart(hearts[i].transform, Vector3.one, Vector3.one * 0.8f)); // 🕯️ Scale down when lost
            }
            hearts[i].enabled = i < maxHealth;
        }
    }

    IEnumerator FadeHeart(Image heartImage)
    {
        float elapsed = 0f;
        Color startColor = heartImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < heartFadeDuration)
        {
            heartImage.color = Color.Lerp(startColor, endColor, elapsed / heartFadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        heartImage.color = endColor;
    }

    IEnumerator ScaleHeart(Transform heart, Vector3 startScale, Vector3 endScale)
    {
        float elapsed = 0f;
        while (elapsed < heartScaleDuration)
        {
            heart.localScale = Vector3.Lerp(startScale, endScale, elapsed / heartScaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        heart.localScale = endScale;
    }

    IEnumerator ScreenFlash() // 🔥 Screen flash for critical hits
    {
        if (screenFlashImage == null) yield break;
        screenFlashImage.color = flashColor;
        float elapsed = 0f;
        Color transparentColor = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

        while (elapsed < screenFlashDuration)
        {
            screenFlashImage.color = Color.Lerp(flashColor, transparentColor, elapsed / screenFlashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        screenFlashImage.color = transparentColor;
    }

    void Die()
    {
        Debug.Log("💀 Player Died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator DamageReaction(Vector2 hitDirection)
    {
        isKnockback = true;
        if (mainCamera != null) StartCoroutine(ScreenShake());
        if (hitParticles != null) hitParticles.Play();
        StartCoroutine(SpriteFlash());

        Time.timeScale = 0f; // 🧊 Hit freeze
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            float healthFactor = 1 + ((maxHealth - currentHealth) * knockbackScaling / maxHealth);
            rb.AddForce(hitDirection.normalized * baseKnockbackForce * healthFactor, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }

    IEnumerator ScreenShake()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            float y = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            mainCamera.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.localPosition = originalPos;
    }

    IEnumerator SpriteFlash()
    {
        playerSprite.color = spriteFlashColor;
        yield return new WaitForSeconds(spriteFlashDuration);
        playerSprite.color = originalColor;
    }

    public bool GetKnockbackState() => isKnockback;
}
