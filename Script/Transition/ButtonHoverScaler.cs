using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

// Menggunakan IPointerClickHandler untuk mendeteksi klik secara akurat
public class ButtonHoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Gambar / Sprite Settings")]
    [SerializeField] private Image targetImage;       
    [SerializeField] private Sprite spriteNormal;     
    [SerializeField] private Sprite spriteHover;      

    [Header("Animasi Scale Settings")]
    [SerializeField] private Vector3 scaleNormal = new Vector3(1f, 1f, 1f); 
    [SerializeField] private Vector3 scaleHover = new Vector3(1.1f, 1.1f, 1f); 
    [SerializeField] private float durasiAnimasi = 0.15f;

    [Header("Komponen Audio")]
    [SerializeField] private AudioSource sfxSource; 
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip clickSound; // SFX saat tombol diklik

    private bool isHovered = false;

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        
        // Otomatis mencari AudioSource di GameObject ini jika belum diisi di Inspector
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        ResetToNormalInstant();
    }

    private void OnEnable()
    {
        ResetToNormalInstant();
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = scaleNormal;
        isHovered = false;

        if (targetImage != null && spriteNormal != null)
        {
            targetImage.sprite = spriteNormal;
        }

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void ResetToNormalInstant()
    {
        transform.DOKill(); 
        transform.localScale = scaleNormal;
        isHovered = false;

        if (targetImage != null && spriteNormal != null)
        {
            targetImage.sprite = spriteNormal;
        }
    }

    // DETEKSI HOVER MASUK
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (targetImage != null && spriteHover != null) targetImage.sprite = spriteHover;

        transform.DOKill();
        
        // TAMBAHKAN .SetUpdate(true) di paling belakang agar animasi kebal terhadap Time.timeScale = 0
        transform.DOScale(scaleHover, durasiAnimasi).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    // DETEKSI HOVER KELUAR
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetToNormalInstant();
    }

    // DETEKSI KLIK (Hanya memicu clickSound)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (sfxSource == null)
            {
                return;
            }

            if (clickSound == null)
            {
                return;
            }

            // Jika lolos pengecekan di atas, suara harusnya berbunyi
            sfxSource.PlayOneShot(clickSound);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}