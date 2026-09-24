using UnityEngine;

// Bisa membuat file data kartu baru langsung lewat klik kanan di Project Window
[CreateAssetMenu(fileName = "NewCard", menuName = "Card/CardData")]
public class CardData : ScriptableObject
{
    // Info kartu 
    public string cardName;
    [TextArea] public string description;
    public Sprite cardImage;

    // Efek kartu ke Player
    [Header("Player Effects")]
    public int playerDamage;
    public int playerHeal; 

    // Efek kartu ke Musuh
    [Header("Enemy Effects")]
    public int enemyDamage; 
    public int enemyHeal; 
}