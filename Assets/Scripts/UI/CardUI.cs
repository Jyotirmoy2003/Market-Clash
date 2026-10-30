using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    public TMP_Text cardText;

    private MarketCardData data;

    // revealNow = true for the player's cards, false for the AI's
    public void Setup(MarketCardData cardData, bool revealNow)
    {
        data = cardData;
        cardText.text = revealNow ? Format(data) : "";
    }

    public void Reveal()
    {
        if (data != null) cardText.text = Format(data);
    }

    private static string Format(MarketCardData d)
    {
        if (d.effectType == CardEffectType.MarketFreeze)
            return $"{d.company.companyName}\nMARKET FREEZE";

        string sign = d.priceChange >= 0 ? "+" : "-";
        return $"{d.company.companyName}\n{sign}Rs. {Mathf.Abs(d.priceChange)}";
    }
}