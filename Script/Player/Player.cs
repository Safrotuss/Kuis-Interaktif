using UnityEngine;

public class Player : MonoBehaviour
{
    // Komponen Animator untuk mengontrol animasi Player
    Animator myAnimation;

    void Start()
    {
        // menghubungkan Animator yang ada pada GameObject Player
        myAnimation = GetComponent<Animator>();
    }
}