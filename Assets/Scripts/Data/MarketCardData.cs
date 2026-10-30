using UnityEngine;

public enum CardEffectType { PriceChange, MarketFreeze }

[CreateAssetMenu(menuName = "MarketClash/Card")]
public class MarketCardData : ScriptableObject
{
    public string cardName;
    public CompanyData company;
    public CardEffectType effectType;
    public int priceChange; // ignored for MarketFreeze
}