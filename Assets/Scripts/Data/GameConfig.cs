using UnityEngine;

[CreateAssetMenu(menuName = "MarketClash/Game Config")]
public class GameConfig : ScriptableObject
{
    public int totalRounds = 3;
    public int startingCash = 1000;
    public int actionsPerRound = 3;
    public int cardsPerHand = 3;
    public int minPrice = 5;
    public int[] allowedQuantities = { 1, 5, 10 };
    public CompanyData[] companies;
    public MarketCardData[] cardPool;
}