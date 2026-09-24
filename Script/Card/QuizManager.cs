using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelSoal
    {
        public string namaSoal;
        public Sprite gambarTeksSoal; 
        public List<GameObject> paketPrefabKartu; 
        
        [Header("Kunci Jawaban")]
        public GameObject prefabKartuBenar; 
        public Sprite gambarJawabanBenar;   
    }

    [Header("Player Settings")]
    public Health playerHealth;

    [Header("UI References")]
    public GameObject panelUtamaSoal; 
    public Image imageTeksSoal;
    
    [Header("Buttons")]
    public Button buttonNext;      // Tombol sembunyikan/tutup panel soal
    public Button buttonAmbil;     // Tombol ambil/tarik kartu
    public Button buttonBukaSoal;  // Tombol buka kembali panel soal

    [Header("Game Data")]
    public List<LevelSoal> daftarSoal = new List<LevelSoal>();
    public HandManager handManager;

    [Header("Evaluation UI")]
    public GameObject panelJawabanBenar;
    public GameObject panelJawabanSalah;
    public Image imageKunciJawabanDiPanelSalah; 

    [Header("Timing Settings")]
    [SerializeField] private float durasiAnimasiAttack = 0.8f; 
    [SerializeField] private float durasiMembacaPanel = 2.5f;  

    [Header("Finish UI")]
    public GameObject panelNextLevel;

    [Header("Game Over UI")]
    public GameObject panelKalah;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource quizAudioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip correctSound;  
    [SerializeField] private AudioClip wrongSound;

    private int indexSekarang = 0;
    private bool isProcessing = false; 

    void Start()
    {
        Time.timeScale = 1f;

        if (buttonNext != null)
        {
            buttonNext.onClick.RemoveAllListeners();
            buttonNext.onClick.AddListener(KlikTombolSembunyikanSoal);
        }
        
        if (buttonAmbil != null)
        {
            buttonAmbil.onClick.RemoveAllListeners();
            buttonAmbil.onClick.AddListener(KlikTombolAmbil);
        }
        
        if (buttonBukaSoal != null)
        {
            buttonBukaSoal.onClick.RemoveAllListeners();
            buttonBukaSoal.onClick.AddListener(KlikTombolBukaSoal);
        }
        
        if (panelJawabanBenar != null) panelJawabanBenar.SetActive(false);
        if (panelJawabanSalah != null) panelJawabanSalah.SetActive(false);

        if (daftarSoal != null && daftarSoal.Count > 0)
        {
            LoadLevel(0);
        }
        else
        {
            Debug.LogError("[QuizManager] List 'Daftar Soal' masih kosong di Inspector!");
        }
    }

    public void LoadLevel(int index)
    {
        if (daftarSoal == null || index < 0 || index >= daftarSoal.Count)
        {
            Debug.LogError($"[QuizManager] Index soal {index} di luar batas! Total soal: {(daftarSoal != null ? daftarSoal.Count : 0)}");
            return;
        }

        indexSekarang = index;
        isProcessing = false; 

        if (panelJawabanBenar != null) panelJawabanBenar.SetActive(false);
        if (panelJawabanSalah != null) panelJawabanSalah.SetActive(false);

        // Aktifkan panel soal
        if (panelUtamaSoal != null) panelUtamaSoal.SetActive(true);
        if (buttonNext != null) buttonNext.gameObject.SetActive(true);
        
        // Nonaktifkan tombol pendukung selama soal dibaca
        if (buttonBukaSoal != null) 
        {
            buttonBukaSoal.gameObject.SetActive(true);
            buttonBukaSoal.interactable = false; 
        }

        if (buttonAmbil != null) 
        {
            if (buttonAmbil.transform.parent != null)
                buttonAmbil.transform.parent.gameObject.SetActive(true); 

            buttonAmbil.gameObject.SetActive(true); 
            buttonAmbil.interactable = false; 
        }

        // Tampilkan sprite soal
        if (imageTeksSoal != null)
        {
            imageTeksSoal.transform.DOKill();
            if (daftarSoal[indexSekarang].gambarTeksSoal != null)
            {
                imageTeksSoal.sprite = daftarSoal[indexSekarang].gambarTeksSoal;
                imageTeksSoal.transform.localScale = Vector3.zero;
                imageTeksSoal.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
        }
    }

    public void KlikTombolSembunyikanSoal()
    {
        StartCoroutine(ProsesSembunyikanPanelSoal());
    }

    private IEnumerator ProsesSembunyikanPanelSoal()
    {
        yield return new WaitForSeconds(0.1f);

        if (buttonNext != null) buttonNext.gameObject.SetActive(false);

        if (imageTeksSoal != null)
        {
            imageTeksSoal.transform.DOKill();
            imageTeksSoal.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        }

        yield return new WaitForSeconds(0.25f);

        if (panelUtamaSoal != null) panelUtamaSoal.SetActive(false);
        
        // Aktifkan tombol buka soal dan tombol ambil kartu
        if (!isProcessing)
        {
            if (buttonBukaSoal != null)
            {
                buttonBukaSoal.gameObject.SetActive(true);
                buttonBukaSoal.interactable = true;
            }

            if (buttonAmbil != null)
            {
                buttonAmbil.gameObject.SetActive(true);
                buttonAmbil.interactable = true;
                buttonAmbil.transform.localScale = Vector3.one;
            }
        }
    }

    public void KlikTombolBukaSoal()
    {
        if (isProcessing) return; 

        if (panelUtamaSoal != null) panelUtamaSoal.SetActive(true);
        
        if (buttonBukaSoal != null) buttonBukaSoal.interactable = false; 
        if (buttonAmbil != null) buttonAmbil.interactable = false; 
        
        if (buttonNext != null) buttonNext.gameObject.SetActive(true); 

        if (imageTeksSoal != null)
        {
            imageTeksSoal.transform.DOKill();
            imageTeksSoal.transform.localScale = Vector3.zero;
            imageTeksSoal.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }
    }

    public void KlikTombolAmbil()
    {
        if (handManager != null)
        {
            List<GameObject> paketKartu = daftarSoal[indexSekarang].paketPrefabKartu;
            if (paketKartu != null && paketKartu.Count > 0)
            {
                handManager.SetupCardsForQuestion(paketKartu);
            }
            else
            {
                Debug.LogError($"[QuizManager] paketPrefabKartu pada index {indexSekarang} kosong!");
            }
        }
        
        if (buttonAmbil != null) buttonAmbil.interactable = false;
    }

    public void CekJawabanKartu(GameObject kartuYangDipilih)
    {
        if (kartuYangDipilih == null || isProcessing) return;
        isProcessing = true; 

        if (buttonBukaSoal != null) buttonBukaSoal.interactable = false;
        if (buttonAmbil != null) buttonAmbil.interactable = false;

        GameObject kunciJawaban = daftarSoal[indexSekarang].prefabKartuBenar;
        bool apakahBenar = false;

        if (kunciJawaban != null)
        {
            apakahBenar = kartuYangDipilih.name.StartsWith(kunciJawaban.name);
        }

        StartCoroutine(JalankanTransisiOtomatis(apakahBenar));
    }

    private IEnumerator JalankanTransisiOtomatis(bool apakahBenar)
    {
        yield return new WaitForSeconds(durasiAnimasiAttack);

        if (apakahBenar)
        {
            TampilkanPanelBenar();
        }
        else
        {
            TampilkanPanelSalah();
        }

        yield return new WaitForSeconds(durasiMembacaPanel);

        if (indexSekarang + 1 < daftarSoal.Count)
        {
            indexSekarang++;
            LoadLevel(indexSekarang);
        }
        else
        {
            if (panelJawabanBenar != null) panelJawabanBenar.SetActive(false);
            if (panelJawabanSalah != null) panelJawabanSalah.SetActive(false);
            MunculkanPanelKemenangan();
        }
    }

    private void TampilkanPanelBenar()
    {
        if (panelJawabanBenar != null)
        {
            panelJawabanBenar.SetActive(true);
            panelJawabanBenar.transform.localScale = Vector3.zero;
            panelJawabanBenar.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }

        if (quizAudioSource != null && correctSound != null)
        {
            quizAudioSource.PlayOneShot(correctSound);
        }
    }

    private void TampilkanPanelSalah()
    {
        if (panelJawabanSalah != null)
        {
            LevelSoal soalAktif = daftarSoal[indexSekarang];

            if (imageKunciJawabanDiPanelSalah != null && soalAktif.gambarJawabanBenar != null)
            {
                imageKunciJawabanDiPanelSalah.sprite = soalAktif.gambarJawabanBenar;
            }

            panelJawabanSalah.SetActive(true);
            panelJawabanSalah.transform.localScale = Vector3.zero;
            panelJawabanSalah.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        }

        if (quizAudioSource != null && wrongSound != null)
        {
            quizAudioSource.PlayOneShot(wrongSound);
        }
    }

    private void MunculkanPanelKemenangan()
    {
        if (panelUtamaSoal != null) panelUtamaSoal.SetActive(false);
        if (buttonBukaSoal != null) buttonBukaSoal.gameObject.SetActive(false); 

        if (buttonAmbil != null && buttonAmbil.transform.parent != null)
        {
            buttonAmbil.transform.parent.gameObject.SetActive(false);
        }

        if (panelNextLevel != null)
        {
            panelNextLevel.SetActive(true);
            panelNextLevel.transform.localScale = Vector3.zero;
            panelNextLevel.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }

        if (quizAudioSource != null && winSound != null)
        {
            quizAudioSource.PlayOneShot(winSound);
        }
    }
}