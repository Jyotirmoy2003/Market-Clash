using System;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public TMP_Text gameStateText; 
    public TMP_Text actionsText; 

    [Header("Player")]
    [Foldout("Player Protfolio")] [SerializeField] TMP_Text caseValue;
    [Foldout("Player Protfolio")] [SerializeField] TMP_Text holdingsValue;
    [Foldout("Player Protfolio")] [SerializeField] TMP_Text networthValue;

    [SerializeField] CanvasGroup blockInputCanvasgroup;

    public Transform sharePanelParent;
    public Transform playerCardPanel;
    public Transform aiCardPanel;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private TMP_Text gameOverText;
    
    



    public void TogglePlayerInput(bool isOn)
    {
        blockInputCanvasgroup.blocksRaycasts = !isOn;
    }
    public void UpdatePlayerPortfolio(Portfolio portfolio, IReadOnlyList<CompanyShareInfo> shares)
    {
        int sharesValue = 0;
        var sb = new StringBuilder();

        foreach (var s in shares)
        {
            int owned = portfolio.GetShares(s.companyName);
            sharesValue += owned * s.currentPrice;
            sb.AppendLine($"{s.companyName}: {owned}");
        }

        caseValue.text = $"Rs. {portfolio.Cash}";
        holdingsValue.text = sb.ToString().TrimEnd();
        networthValue.text = $"Rs. {portfolio.Cash + sharesValue}";
    }

    public void SetStateText(string stateName)
    {
        gameStateText.text = stateName;   // use your own TMP field name here
    }



    public void SetActionsText(int remaining)
    {
        actionsText.text = $"Actions remaining: {remaining}";   // use your own TMP field name
    }



    // UIManager.cs: no new [SerializeField] lines needed
    public void ShowGameOver(int playerNetWorth, int aiNetWorth)
    {
        string result = playerNetWorth > aiNetWorth ? "Player Wins!"
                    : aiNetWorth > playerNetWorth ? "AI Wins!"
                    : "Draw!";

        gameOverText.text = $"{result}\n\nPlayer net worth: Rs. {playerNetWorth}\nAI net worth: Rs. {aiNetWorth}";
        gameOverPanel.SetActive(true);
        mainUI.SetActive(false);
    }
}
