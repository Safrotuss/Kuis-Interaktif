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
            tombolUIKartu.onClick.AddListener(KartuDiklikViaUI);
        }
    }

    // KARTU 2D (SISTEM DRAG & DROP / LEPAS MOUSE)
    // otomatis berjalan saat klik kiri mouse dilepas dari objek
    private void OnMouseUp()
    {
        KirimDataKartuKeManager();
    }

    // KARTU CANVAS UI (SISTEM KLIK TOMBOL BIASA)
    private void KartuDiklikViaUI()
    {
        KirimDataKartuKeManager();
    }

    // FUNGSI INTI PENGIRIM DATA (SUPER AMAN & ANTI-SALAH SASARAN)
    private void KirimDataKartuKeManager()
    {
        QuizManager quiz = Object.FindFirstObjectByType<QuizManager>();

        if (quiz != null)
        {
            // PENGAMAN UTAMA: Ambil objek tertinggi dari susunan kartu ini.
            // Ini mencegah script mengirim nama objek anak (seperti teks/gambar bayangan).
            GameObject objekUtamaKartu = transform.root.gameObject;

            // Jaga-jaga jika menggunakan Canvas besar dan transform.root malah mengambil objek Canvas Utama,
            // kita alihkan untuk mengambil objek game object ini sendiri sebagai perwakilan teratas kartu.
            if (objekUtamaKartu.GetComponent<Canvas>() != null)
            {
                objekUtamaKartu = this.gameObject;
            }
            
            // Kirim objek utama kartu yang sudah dipastikan bersih namanya ke QuizManager
            quiz.CekJawabanKartu(objekUtamaKartu);
        }
        else {

        }
    }
}