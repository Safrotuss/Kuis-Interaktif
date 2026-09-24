using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHandSize = 5;
    public float hoverAmount = 1.5f;
    public float scaleAmount = 1.2f;
    [SerializeField] private LayerMask dropAreaLayer;

    [Header("Health References")]
    public Health playerHealth; 
    public EnemyHealth enemyHealth;   

    [Header("References")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    
    private QuizManager quizManager; 
    private List<GameObject> handCards = new List<GameObject>();

    private void Start()
    {
        quizManager = Object.FindFirstObjectByType<QuizManager>();

        if (playerHealth == null) playerHealth = Object.FindFirstObjectByType<Health>();
        if (enemyHealth == null) enemyHealth = Object.FindFirstObjectByType<EnemyHealth>();
    }

    public void SetupCardsForQuestion(List<GameObject> questionPrefabs)
    {
        if (questionPrefabs == null || questionPrefabs.Count == 0) return;

        ClearHandInstant();
        StartCoroutine(DrawQuestionSequence(questionPrefabs));
    }

    private IEnumerator DrawQuestionSequence(List<GameObject> questionPrefabs)
    {
        if (spawnPoint == null) yield break;

        foreach (GameObject prefab in questionPrefabs)
        {
            if (handCards.Count >= maxHandSize) break; 
            if (prefab == null) continue;

            GameObject g = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            g.transform.localScale = Vector3.one;

            CardHover hoverScript = g.GetComponent<CardHover>();
            if (hoverScript == null) hoverScript = g.AddComponent<CardHover>();
            hoverScript.manager = this;

            handCards.Add(g);
            UpdateCardPositions();

            yield return new WaitForSeconds(0.15f); 
        }
    }

    private void ClearHandInstant()
    {
        foreach (GameObject card in handCards)
        {
            if (card != null) Destroy(card);
        }
        handCards.Clear();
    }

    public void UpdateCardPositions()
    {
        if (handCards.Count == 0 || splineContainer == null) return;

        float length = 0.8f;
        float startP = 0.5f - (handCards.Count - 1) * (length / maxHandSize) / 2;

        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i] == null) continue;
            CardHover cardScript = handCards[i].GetComponent<CardHover>();
            if (cardScript != null && (cardScript.isHovered || cardScript.isDragging)) continue;

            float p = startP + i * (length / maxHandSize);
            
            Vector3 worldPos = (Vector3)splineContainer.EvaluatePosition(p);
            Vector3 forward = (Vector3)splineContainer.EvaluateTangent(p);
            Vector3 up = (Vector3)splineContainer.EvaluateUpVector(p);
            
            // Atur kedalaman Z kartu agar sedikit di depan background
            worldPos.z = -1f + (i * -0.05f);

            Quaternion worldRot = Quaternion.identity;
            if (forward != Vector3.zero && up != Vector3.zero)
            {
                worldRot = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
                worldRot *= Quaternion.Euler(180, 0, 0);
            }

            handCards[i].transform.DOMove(worldPos, 0.4f).SetEase(Ease.OutBack);
            handCards[i].transform.DORotateQuaternion(worldRot, 0.4f).SetEase(Ease.OutCubic);
            handCards[i].transform.DOScale(Vector3.one, 0.3f);
        }
    }

    public void PlayCard(GameObject card)
    {
        if (card == null) return;

        CardDisplay display = card.GetComponent<CardDisplay>();
        
        if (display != null && display.data != null)
        {
            CardData data = display.data;

            // Serangan Player
            if (data.enemyDamage > 0 && enemyHealth != null)
            {
                if (playerHealth != null) playerHealth.PlayAttackAnimation();
                enemyHealth.TakeDamage(data.enemyDamage);
            }

            // Serangan Musuh
            if (data.playerDamage > 0 && playerHealth != null)
            {
                if (enemyHealth != null) enemyHealth.PlayAttackAnimation();
                playerHealth.TakeDamage(data.playerDamage);
            }

            // Heal
            if (data.playerHeal > 0 && playerHealth != null) playerHealth.Heal(data.playerHeal);
            if (data.enemyHeal > 0 && enemyHealth != null) enemyHealth.Heal(data.enemyHeal);
        }

        // Jalankan evaluasi benar/salah dan transisi soal di QuizManager
        if (quizManager != null)
        {
            quizManager.CekJawabanKartu(card);
        }

        if (handCards.Contains(card))
        {
            handCards.Remove(card);
        }

        foreach (GameObject c in handCards)
        {
            if (c != null)
            {
                Collider2D col = c.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }

        card.transform.DOKill();
        card.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() => {
            Destroy(card);
            StartCoroutine(ClearRemainingCardsAndNext());
        });
    }

    private IEnumerator ClearRemainingCardsAndNext()
    {
        foreach (GameObject c in handCards)
        {
            if (c != null) c.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        }
        
        yield return new WaitForSeconds(0.2f);
        
        foreach (GameObject c in handCards)
        {
            if (c != null) Destroy(c);
        }
        handCards.Clear();
    }

    // ====================================================================
    // SUB-CLASS CARD HOVER
    // ====================================================================
    public class CardHover : MonoBehaviour
    {
        public HandManager manager;
        public bool isHovered = false;
        public bool isDragging = false;
        
        private Vector3 savedPos;
        private Quaternion savedRot;
        private Camera mainCam;
        private Vector3 scaleBeforeHover = Vector3.one;

        private SpriteRenderer spriteRenderer;
        private Canvas canvasUI;
        private int sortingOrderAsli = 0;

        private void Start()
        {
            mainCam = Camera.main;
            scaleBeforeHover = transform.localScale;

            spriteRenderer = GetComponent<SpriteRenderer>();
            canvasUI = GetComponent<Canvas>();

            if (spriteRenderer != null) sortingOrderAsli = spriteRenderer.sortingOrder;
            if (canvasUI != null) sortingOrderAsli = canvasUI.sortingOrder;
        }

        private void OnMouseEnter()
        {
            if (isDragging || isHovered || manager == null) return;
            isHovered = true;

            if (spriteRenderer != null) spriteRenderer.sortingOrder = 100;
            if (canvasUI != null) canvasUI.sortingOrder = 100;
            
            transform.DOKill();
            transform.DOScale(scaleBeforeHover * manager.scaleAmount, 0.2f);
            transform.DOMove(transform.position + transform.up * manager.hoverAmount, 0.2f).SetEase(Ease.OutCubic);
            transform.DOMoveZ(-3f, 0.1f);
        }

        private void OnMouseExit()
        {
            if (isDragging || manager == null) return;
            isHovered = false;

            if (spriteRenderer != null) spriteRenderer.sortingOrder = sortingOrderAsli;
            if (canvasUI != null) canvasUI.sortingOrder = sortingOrderAsli;

            transform.DOScale(scaleBeforeHover, 0.2f);
            manager.UpdateCardPositions();
        }

        private void OnMouseDown()
        {
            isDragging = true;
            isHovered = false;

            if (spriteRenderer != null) spriteRenderer.sortingOrder = 100;
            if (canvasUI != null) canvasUI.sortingOrder = 100;

            savedPos = transform.position;
            savedRot = transform.rotation;
            transform.DOKill();
            transform.DORotate(Vector3.zero, 0.2f);
        }

        private void OnMouseDrag()
        {
            if (mainCam == null) return;
            Vector3 mousePos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            mousePos.z = -3f; 
            transform.position = mousePos;
        }

        private void OnMouseUp()
        {
            isDragging = false;
            if (mainCam == null || manager == null) return;

            if (spriteRenderer != null) spriteRenderer.sortingOrder = sortingOrderAsli;
            if (canvasUI != null) canvasUI.sortingOrder = sortingOrderAsli;

            Vector3 worldMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(worldMousePos.x, worldMousePos.y);
            
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D, manager.dropAreaLayer);
            
            if (hit != null) 
            {
                manager.PlayCard(gameObject);
            } 
            else 
            {
                ReturnToHand();
            }
        }

        private void ReturnToHand()
        {
            transform.DOKill();
            transform.DOMove(savedPos, 0.3f).SetEase(Ease.OutBack);
            transform.DORotateQuaternion(savedRot, 0.3f).OnComplete(() => {
                if (manager != null) manager.UpdateCardPositions();
            });
        }
    }
}