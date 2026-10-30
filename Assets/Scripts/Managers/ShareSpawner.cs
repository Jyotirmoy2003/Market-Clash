using System;
using System.Collections.Generic;
using UnityEngine;

public class ShareSpwner : MonoBehaviour
{

    [SerializeField] private GameConfig config;
    [SerializeField] private CompanyShareUIPrefab companyShareUIPrefab;

    private readonly List<CompanyShareInfo> companyShares = new();
    public IReadOnlyList<CompanyShareInfo> Shares => companyShares;
    private readonly List<CompanyShareUIPrefab> spawnedShareUIs = new();
    private bool spawned;
    public event Action<int> BuyClicked, SellClicked, PassClicked;

















    // Call once when the game starts (Ready state)
    public void InitCompanyshareToPlayer()
    {
        if (spawned) { RefreshPrices(); return; }

        UIManager ui = UIManager.Instance;
        if (ui == null || ui.sharePanelParent == null || companyShareUIPrefab == null || config == null)
        {
            Debug.LogError("ShareSpawner: UIManager, panel, prefab or config is missing.");
            return;
        }

        // Build the list from GameConfig
        companyShares.Clear();
        foreach (var company in config.companies)
            companyShares.Add(new CompanyShareInfo { companyName = company.companyName, currentPrice = company.startingPrice });

        // Spawn the rows (this replaces the old foreach loop)
        for (int i = 0; i < companyShares.Count; i++)
        {
            int idx = i;
            var info = companyShares[i];
            CompanyShareUIPrefab row = Instantiate(companyShareUIPrefab, ui.sharePanelParent);
            row.Init(info.companyName, "Rs. " + info.currentPrice,
                () => BuyClicked?.Invoke(idx),
                () => SellClicked?.Invoke(idx),
                () => PassClicked?.Invoke(idx));
            row.SetButtons(false, false, false);
            spawnedShareUIs.Add(row);
        }

        spawned = true;
    }

    // Call after prices change (after Market Reveal)
    public void SetPrice(int index, int newPrice)
    {
        companyShares[index].currentPrice = newPrice;
        spawnedShareUIs[index].UpdatePrice(newPrice);
    }

    public void RefreshPrices()
    {
        for (int i = 0; i < companyShares.Count; i++)
            spawnedShareUIs[i].UpdatePrice(companyShares[i].currentPrice);
    }

    public void RefreshButtons(Portfolio portfolio)
    {
        for (int i = 0; i < companyShares.Count; i++)
        {
            var info = companyShares[i];
            bool canBuy = portfolio.CanBuy(info.currentPrice, 1);   // can afford at least 1 share
            bool canSell = portfolio.GetShares(info.companyName) > 0; // owns at least 1 share
            spawnedShareUIs[i].SetButtons(canBuy, canSell, true);
        }
    }

    public void SetShareButtons(bool value)
    {
        foreach (var row in spawnedShareUIs)
            if (row != null) row.SetButtons(value, value, value);
    }
}
