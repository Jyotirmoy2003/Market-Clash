using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameConfig config;

    public List<MarketCardData> playerCards = new();
    public List<MarketCardData> aiCards = new();

    [NaughtyAttributes.Button]
    public void DealCards()
    {
        // Discard last round's cards
        playerCards.Clear();
        aiCards.Clear();

        if (config == null || config.cardPool == null || config.cardPool.Length == 0)
        {
            Debug.LogError("CardManager: card pool is empty or config is missing.");
            return;
        }

        for (int i = 0; i < config.cardsPerHand; i++)
        {
            playerCards.Add(DrawRandomCard());
            aiCards.Add(DrawRandomCard());
        }

        Debug.Log($"Player cards: {string.Join(", ", playerCards.ConvertAll(c => c.cardName))}");
        Debug.Log($"AI cards: {string.Join(", ", aiCards.ConvertAll(c => c.cardName))}");

        SpawnCardUIs();
    }

    private MarketCardData DrawRandomCard()
    {
        return config.cardPool[Random.Range(0, config.cardPool.Length)];
    }







    [SerializeField] private CardUI cardUIPrefab;

    private readonly List<CardUI> playerCardUIs = new();
    private readonly List<CardUI> aiCardUIs = new();

    // Call this at the end of DealCards()
    [NaughtyAttributes.Button]
    private void SpawnCardUIs()
    {
        UIManager ui = UIManager.Instance;
        if (ui == null || cardUIPrefab == null) { Debug.LogError("CardManager: UIManager or CardUI prefab missing."); return; }

        ClearCardUIs();

        foreach (var card in playerCards)
        {
            CardUI cu = Instantiate(cardUIPrefab, ui.playerCardPanel);
            cu.Setup(card, true);
            playerCardUIs.Add(cu);
        }

        foreach (var card in aiCards)
        {
            CardUI cu = Instantiate(cardUIPrefab, ui.aiCardPanel);
            cu.Setup(card, false); // empty text, data kept inside
            aiCardUIs.Add(cu);
        }
    }

    private void ClearCardUIs()
    {
        foreach (var c in playerCardUIs) if (c != null) Destroy(c.gameObject);
        foreach (var c in aiCardUIs) if (c != null) Destroy(c.gameObject);
        playerCardUIs.Clear();
        aiCardUIs.Clear();
    }

    [NaughtyAttributes.Button]
    public void DebugRevealCard()
    {
        StartCoroutine(RevealAICards());
    }

    // Reveals AI cards one by one
    public IEnumerator RevealAICards(float delay = 0.6f)
    {
        foreach (var cu in aiCardUIs)
        {
            cu.Reveal();
            yield return new WaitForSeconds(delay);
        }
    }
}