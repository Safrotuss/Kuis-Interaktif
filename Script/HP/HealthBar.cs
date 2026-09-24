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
    private int maxHpBackup = 100; // Backup jika maxHp terlambat terbaca saat Start

    void Start()
    {
        // Menggunakan GetDelayed (tunggu 1 frame) agar RectTransform terbaca sempurna
        StartCoroutine(InitializeBar());
    }

    IEnumerator InitializeBar()
    {
        yield return null; // Tunggu sebentar agar UI render dulu
        if (barRect != null && mask != null)
        {
            maxRightMask = barRect.rect.width;
            initialRightMask = mask.padding.z;
        }
    }

    // Perbaikan Utama: Fungsi SetValue sekarang meminta 2 data (HP saat ini & HP Maksimal)
    // Dengan cara ini, HealthBar tidak perlu mengemis data ke script Health lama lagi
    public void SetValue(int currentHp, int maxHp)
    {
        if (mask == null) return;

        // Simpan nilai max HP ke backup agar teks tidak kosong saat inisialisasi awal
        maxHpBackup = maxHp;

        // Cegah pembagian dengan angka 0 agar tidak terjadi error Crash/Infinity
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

    // Fungsi overloads lama agar tidak memicu error kompilasi jika dipanggil dari script lain
    public void SetValue(int newValue)
    {
        SetValue(newValue, maxHpBackup);
    }
}