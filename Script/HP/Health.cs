using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Health : MonoBehaviour
{
    public int maxHp = 100;
    
    // Nilai HP dibuat mandiri per instans (bukan static)
    public int currentHp = -1; 

    public bool isDead = false;
    
    [Header("UI References")]
    [SerializeField] private HealthBar healthBar; 

    [Header("Asset Store VFX")]
    [SerializeField] private GameObject hitEffectPrefab; 
    [SerializeField] private float durasiEfekHidup = 1.5f;

    [Header("Juice Effects")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.4f;

    [SerializeField] private Animator animator;

    [Header("Game Over Settings")]
    [SerializeField] private GameObject panelKekalahanPilihan; 

    [Header("Komponen Audio")]
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip AttackSFX;

    void Awake()
    {
        if (currentHp == -1)
        {
            currentHp = maxHp;
        }
    }

    private void Start()
    {
        if (currentHp <= 0) 
        {
            currentHp = maxHp;
            isDead = false;
        }

        if (healthBar != null) healthBar.SetValue(currentHp, maxHp);
    }

    public void PlayAttackAnimation()
    {
        if (isDead) return;
        if (animator != null) animator.SetTrigger("Attack");
        if (AttackSFX != null && sfxSource != null) sfxSource.PlayOneShot(AttackSFX);
    }

   // Mengurangi HP, memperbarui HealthBar, spawn VFX kena pukul, dan nge-shake sprite via DOTween
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
        
        if (healthBar != null) healthBar.SetValue(currentHp, maxHp);

        if (hitEffectPrefab != null)
        {
            GameObject efekTerbuat = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(efekTerbuat, durasiEfekHidup);
        }

        transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true);

        if (currentHp <= 0) Die();
    }
   
    // Jeda delay sebelum nampilin panel game over melalui animasi Pop-Up (DOScale)
    private IEnumerator ShowSelectedGameOverPanel()
    {
        yield return new WaitForSeconds(2.0f);
        if (panelKekalahanPilihan != null)
        {
            panelKekalahanPilihan.SetActive(true);
            panelKekalahanPilihan.transform.localScale = Vector3.zero;
            panelKekalahanPilihan.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }
    }

    // Nambahin HP karakter dan nge-clamp biar gak melewati maxHp
    public void Heal(int amount)
    {
        if (isDead) return;
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        if (healthBar != null) healthBar.SetValue(currentHp, maxHp);
    }

    // Saat HP habis UI Quiz dan movement dimatikan, dan panggil animasi mati
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        QuizManager quiz = Object.FindFirstObjectByType<QuizManager>();
        if (quiz != null) quiz.enabled = false; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        if (animator != null) animator.SetTrigger("Dead"); 
        StartCoroutine(ShowSelectedGameOverPanel());
    }
}