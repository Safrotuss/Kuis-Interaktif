using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Animasi Scale Settings")]
    [SerializeField] private Vector3 scaleNormal = Vector3.one; 
    [SerializeField] private Vector3 scaleHover = new Vector3(1.1f, 1.1f, 1f); 
    [SerializeField] private float durasiAnimasi = 0.15f;

    [Header("Komponen Audio")]
    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        // Mencari AudioSource 
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        ResetToNormalInstant();
    }

    private void OnDisable()
    {
        ResetToNormalInstant();

        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void ResetToNormalInstant()
    {
        transform.DOKill(); 
        transform.localScale = scaleNormal;
    }

    // Deteksi hover masuk
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(scaleHover, durasiAnimasi).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    // Deteksi hover keluar
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(scaleNormal, durasiAnimasi).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    // Deteksi klik
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (sfxSource != null && clickSound != null)
            {
                sfxSource.PlayOneShot(clickSound);
            }
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}