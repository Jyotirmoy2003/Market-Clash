using System.Collections.Generic;
using UnityEngine;

public class AIController
{
    private const int StrongNegative = -10;   // net card value that triggers a sell

    private readonly Portfolio portfolio;
    private readonly int[] allowedQuantities;

    public AIController(Portfolio portfolio, int[] allowedQuantities)
    {
        this.portfolio = portfolio;
        this.allowedQuantities = allowedQuantities;
    }

    // Performs one action and returns a log line
    public string TakeAction(IReadOnlyList<CompanyShareInfo> shares, List<MarketCardData> cards)
    {
        // Net card movement per company from the AI's own cards
        var net = new Dictionary<string, int>();
        foreach (var s in shares) net[s.companyName] = 0;
        foreach (var c in cards)
            if (c.effectType == CardEffectType.PriceChange && c.company != null)
                net[c.company.companyName] += c.priceChange;

        // 1. Sell a company we own that has strongly negative cards
        CompanyShareInfo sellTarget = null;
        foreach (var s in shares)
        {
            if (portfolio.GetShares(s.companyName) > 0 && net[s.companyName] <= StrongNegative)
                if (sellTarget == null || net[s.companyName] < net[sellTarget.companyName])
                    sellTarget = s;
        }
        if (sellTarget != null) return TrySell(sellTarget);

        // 2. Buy the company with the best positive cards
        CompanyShareInfo buyTarget = null;
        foreach (var s in shares)
            if (net[s.companyName] > 0 && (buyTarget == null || net[s.companyName] > net[buyTarget.companyName]))
                buyTarget = s;

        if (buyTarget != null)
        {
            int qty = LargestAffordable(buyTarget.currentPrice);
            if (qty > 0 && portfolio.Buy(buyTarget.companyName, buyTarget.currentPrice, qty))
                return $"AI bought {qty} {buyTarget.companyName}";

            // Can't afford: sell something owned (worst cards first), else pass
            CompanyShareInfo fallback = null;
            foreach (var s in shares)
                if (portfolio.GetShares(s.companyName) > 0 && (fallback == null || net[s.companyName] < net[fallback.companyName]))
                    fallback = s;
            if (fallback != null) return TrySell(fallback);
        }

        return "AI passed";
    }

    private string TrySell(CompanyShareInfo s)
    {
        int qty = LargestSellable(s.companyName);
        if (qty > 0 && portfolio.Sell(s.companyName, s.currentPrice, qty))
            return $"AI sold {qty} {s.companyName}";
        return "AI passed";
    }

    private int LargestAffordable(int price)
    {
        int best = 0;
        foreach (int q in allowedQuantities)
            if (portfolio.CanBuy(price, q) && q > best) best = q;
        return best;
    }

    private int LargestSellable(string company)
    {
        int best = 0;
        foreach (int q in allowedQuantities)
            if (portfolio.CanSell(company, q) && q > best) best = q;
        return best;
    }
}