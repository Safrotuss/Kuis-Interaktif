using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("Referensi UI Utama")]
    public GameObject panelUtama; // Panel yang biasanya muncul pertama (misal: HUD)

    [SerializeField] private Health playerHealth;

    [Header("Audio Settings (Biar Aman Pindah Scene)")]
    [Tooltip("Beri jeda waktu (detik) agar suara klik selesai berbunyi sebelum scene berganti.")]
    [SerializeField] private float jedaPindahScene = 0.25f; 

    public void PindahScene(string namaScene)
    {
        Time.timeScale = 1f; 
        StartCoroutine(ProsesPindahSceneDenganJeda(namaScene));
    }

    private IEnumerator ProsesPindahSceneDenganJeda(string namaScene)
    {
        yield return new WaitForSeconds(jedaPindahScene);
        SceneManager.LoadScene(namaScene);
    }

    // MEMUNCULKAN PANEL (Sambil Freeze Game untuk Pause menu)
    public void MunculkanPanel(GameObject panelYangMauBuka)
    {
        if (panelYangMauBuka != null)
        {
            if (playerHealth != null && playerHealth.isDead)
            {
                return; 
            }
            panelYangMauBuka.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // MEMUNCULKAN PANEL (Tanpa Freeze Game)
    public void MunculkanPanel2 (GameObject panelYangMauBuka)
    {
        if (panelYangMauBuka != null)
        {
            if (playerHealth != null && playerHealth.isDead) return;

            panelYangMauBuka.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    // MENYEMBUNYIKAN PANEL 
    public void SembunyikanPanel(GameObject panelYangMauTutup)
    {
        if (panelYangMauTutup != null)
        {
            StartCoroutine(ProsesSembunyikanPanel(panelYangMauTutup));
        }
    }

    private IEnumerator ProsesSembunyikanPanel(GameObject panelYangMauTutup)
    {
        // Beri jeda 0.15 detik realtime agar SFX tombol close sempat berbunyi
        yield return new WaitForSecondsRealtime(0.15f);

        if (panelYangMauTutup != null)
        {
            panelYangMauTutup.SetActive(false);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ResumeGame(GameObject panelYangMauTutup)
    {
        if (panelYangMauTutup != null)
        {
            StartCoroutine(ProsesResumeGame(panelYangMauTutup));
        }
    }

    private IEnumerator ProsesResumeGame(GameObject panelYangMauTutup)
    {
        yield return new WaitForSecondsRealtime(0.15f);

        if (panelYangMauTutup != null)
        {
            panelYangMauTutup.SetActive(false);
            Time.timeScale = 1f; 
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}