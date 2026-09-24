using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyHealth : MonoBehaviour
{
    public int maxHp = 100;
    public static int currentHp = -1; 

    public bool isDead = false;
    
    [Header("UI References")]
    [SerializeField] private HealthBar healthBar; 

    [Header("Asset Store VFX")]
    [SerializeField] private GameObject hitEffectPrefab; 
    [SerializeField] private float durasiEfekHidup = 1.5f;

    [Header("Juice Effects")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.5f;

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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
        
        if (healthBar != null) healthBar.SetValue(currentHp, maxHp);

        if (hitEffectPrefab != null)
        {
            // Spawn prefab efek tepat di posisi koordinat Player saat ini
            GameObject efekTerbuat = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

            // Pengaman: Hapus objek efek tersebut dari memori game setelah beberapa detik 
            // agar clone-nya tidak menumpuk di Hierarchy dan bikin game lag!
            Destroy(efekTerbuat, durasiEfekHidup);
        }
   
        transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true);

        if (currentHp <= 0) Die();
    }

    private IEnumerator ShowSelectedGameOverPanel()
    {
        yield return new WaitForSeconds(2.0f);
        if (panelKekalahanPilihan != null)
        {
            panelKekalahanPilihan.SetActive(true);
            panelKekalahanPilihan.transform.localScale = Vector3.zero;
            panelKekalahanPilihan.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        if (healthBar != null) healthBar.SetValue(currentHp, maxHp);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        QuizManager quiz = Object.FindFirstObjectByType<QuizManager>();
        if (quiz != null)
        {
            if (quiz.panelUtamaSoal != null) quiz.panelUtamaSoal.SetActive(false);
            if (quiz.buttonAmbil != null) quiz.buttonAmbil.gameObject.SetActive(false);
            quiz.enabled = false; 
        }

        Player playerMovement = GetComponent<Player>();
        if (playerMovement != null) playerMovement.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        if (animator != null) animator.SetTrigger("Dead"); 
        StartCoroutine(ShowSelectedGameOverPanel());
    }
}