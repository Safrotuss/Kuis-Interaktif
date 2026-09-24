using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite cardImage;

    [Header("Player Effects")]
    public int playerDamage; // Jika positif, darah player berkurang
    public int playerHeal;   // Jika positif, darah player bertambah

    [Header("Enemy Effects")]
    public int enemyDamage;  // Jika positif, darah musuh berkurang
    public int enemyHeal;    // Jika positif, darah musuh bertambah
}