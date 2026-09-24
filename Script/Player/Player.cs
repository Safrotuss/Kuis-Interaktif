using UnityEngine;

public class Player : MonoBehaviour
{
    Animator myAnimation;

    void Start()
    {
        myAnimation = GetComponent<Animator>();
    }
}
