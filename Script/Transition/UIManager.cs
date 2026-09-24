using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Referensi UI Utama")]
    public GameObject panelUtama;

    [SerializeField] private Health playerHealth;

    [Header("Audio Settings")]
    [Tooltip("Beri jeda waktu (detik) agar SFX klik tidak terpotong")]
    [SerializeField] private float delayPindah = 0.25f; 

    // Button UI untuk ganti scene
    public void PindahScene(string namaScene)
    {
        Time.timeScale = 1f; // Pastikan waktu game berjalan normal sebelum pindah
        StartCoroutine(ProsesPindah(namaScene));
    }

    // untuk memberi jeda sebentar sebelum scene benar-benar dimuat
    private IEnumerator ProsesPindah(string namaScene)
    {
        yield return new WaitForSeconds(delayPindah);
        SceneManager.LoadScene(namaScene);
    }

    // Membuka panel UI sekaligus mempause/mefreeze game
    public void MunculkanPanel(GameObject panelYangMauBuka)
    {
        if (panelYangMauBuka != null)
        {
            // Jangan buka panel jika player sudah mati
            if (playerHealth != null && playerHealth.isDead)
            {
                return; 
            }
            panelYangMauBuka.SetActive(true);
            Time.timeScale = 0f; // Freeze pergerakan/waktu di dalam game
        }
    }

    // Membuka panel UI tanpa mempause game
    public void MunculkanPanel2 (GameObject panelYangMauBuka)
    {
        if (panelYangMauBuka != null)
        {
            if (playerHealth != null && playerHealth.isDead) return;

            panelYangMauBuka.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    // Menutup panel UI dengan sedikit delay (biar animasi/SFX tombol selesai dulu)
    public void SembunyikanPanel(GameObject panelYangMauTutup)
    {
        if (panelYangMauTutup != null)
        {
            StartCoroutine(ProsesSembunyikanPanel(panelYangMauTutup));
        }
    }

    // penutupan panel dengan waktu real-time (tetap jalan meski game di-pause)
    private IEnumerator ProsesSembunyikanPanel(GameObject panelYangMauTutup)
    {
        // Gunakan WaitForSecondsRealtime agar jeda tetap berjalan meskipun Time.timeScale = 0
        yield return new WaitForSecondsRealtime(0.15f);

        if (panelYangMauTutup != null)
        {
            panelYangMauTutup.SetActive(false);
        }

        // Hapus fokus dari tombol UI agar tidak sengaja tertekan lagi via keyboard/gamepad
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Melanjutkan game dari posisi pause (menutup panel & mengembalikan kecepatan waktu)
    public void ResumeGame(GameObject panelYangMauTutup)
    {
        if (panelYangMauTutup != null)
        {
            StartCoroutine(ProsesResumeGame(panelYangMauTutup));
        }
    }

    // Coroutine khusus resume game
    private IEnumerator ProsesResumeGame(GameObject panelYangMauTutup)
    {
        yield return new WaitForSecondsRealtime(0.15f);

        if (panelYangMauTutup != null)
        {
            panelYangMauTutup.SetActive(false);
            Time.timeScale = 1f; // Kembalikan kecepatan waktu game jadi normal
        }
    }

    // Keluar dari aplikasi (hanya bekerja saat sudah dibuild)
    public void Quit()
    {
        Application.Quit();
    }
}