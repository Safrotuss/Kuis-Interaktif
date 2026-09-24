using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    private Button tombolUIKartu;

    void Start()
    {
        tombolUIKartu = GetComponent<Button>();
        
        if (tombolUIKartu != null)
        {
            // Menghubungkan klik Button UI ke fungsi eksekusi
            tombolUIKartu.onClick.AddListener(KartuDiklikViaUI);
        }
    }

    // Deteksi klik mouse untuk objek kartu
    private void OnMouseUp()
    {
        KirimDataKartuKeManager();
    }

    private void KartuDiklikViaUI()
    {
        KirimDataKartuKeManager();
    }

    private void KirimDataKartuKeManager()
    {
        // cari QuizManager di dalam scene
        QuizManager quiz = Object.FindFirstObjectByType<QuizManager>();

        if (quiz != null)
        {
            // Ambil root parent kartu (jika prefab kartu merupakan anak dari objek lain)
            GameObject objekUtamaKartu = transform.root.gameObject;

            if (objekUtamaKartu.GetComponent<Canvas>() != null)
            {
                objekUtamaKartu = this.gameObject;
            }
            
            // Kirim objek kartu ke QuizManager untuk validasi jawaban
            quiz.CekJawabanKartu(objekUtamaKartu);
        }
        else {

        }
    }
}