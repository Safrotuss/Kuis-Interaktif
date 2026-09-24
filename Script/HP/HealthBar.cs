using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("UI Component References")]
    [SerializeField] private RectTransform barRect; 
    [SerializeField] private RectMask2D mask; 
    [SerializeField] private TextMeshProUGUI hpText; 

    private float maxRightMask;
    private float initialRightMask;
    private int maxHpBackup = 100;

    void Start()
    {
        StartCoroutine(InitializeBar());
    }

    IEnumerator InitializeBar()
    {
        yield return null; 
        if (barRect != null && mask != null)
        {
            maxRightMask = barRect.rect.width;
            initialRightMask = mask.padding.z;
        }
    }

    public void SetValue(int currentHp, int maxHp)
    {
        if (mask == null) return;

        // Simpan nilai max HP ke backup agar teks tidak kosong saat inisialisasi awal
        maxHpBackup = maxHp;

        if (maxHp <= 0) maxHp = 100; 

        // Hitung rasio sisa darah saat ini
        float ratio = (float)currentHp / maxHp;
        float targetWidth = ratio * maxRightMask;
        float newRightPadding = maxRightMask + initialRightMask - targetWidth;
        
        // Geser padding top/right/bottom/left milik RectMask2D secara realtime
        Vector4 padding = mask.padding;
        padding.z = newRightPadding;
        mask.padding = padding;

        // Update teks angka di bar darah
        if (hpText != null) 
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    public void SetValue(int newValue)
    {
        SetValue(newValue, maxHpBackup);
    }
}