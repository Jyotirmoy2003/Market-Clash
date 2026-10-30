using System.Collections.Generic;
using UnityEngine;

public class Portfolio
{
    public int Cash { get; private set; }
    private readonly Dictionary<string, int> holdings = new();

    public Portfolio(int startingCash, IEnumerable<string> companyNames)
    {
        Cash = startingCash;
        foreach (var name in companyNames) holdings[name] = 0;
    }

    public int GetShares(string company) => holdings.TryGetValue(company, out int n) ? n : 0;

    public bool CanBuy(int price, int qty) => qty > 0 && price > 0 && Cash >= price * qty;
    public bool CanSell(string company, int qty) => qty > 0 && GetShares(company) >= qty;

    public bool Buy(string company, int price, int qty)
    {
        if (!holdings.ContainsKey(company) || !CanBuy(price, qty)) return false;
        Cash -= price * qty;
        holdings[company] += qty;
        return true;
    }

    public bool Sell(string company, int price, int qty)
    {
        if (!holdings.ContainsKey(company) || !CanSell(company, qty)) return false;
        Cash += price * qty;
        holdings[company] -= qty;
        return true;
    }

    public int GetPortfolioValue(Dictionary<string, int> prices)
    {
        int total = 0;
        foreach (var kv in holdings) total += kv.Value * prices[kv.Key];
        return total;
    }
}