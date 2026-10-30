using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] GameConfig config;
    [SerializeField] private TradePopup tradePopup;
    private int actionsRemaining;


   [SerializeField] private ShareSpwner shareSpawner;
   [SerializeField] private CardManager cardManager;

    public GameState CurrentState { get; private set; }

    public Portfolio PlayerPortfolio { get; private set; }
    public Portfolio AIPortfolio { get; private set; }


    private AIController aiController;
    private int currentRound = 1;

   

















    private void Start()
    {
        var names = System.Array.ConvertAll(config.companies, c => c.companyName);
        PlayerPortfolio = new Portfolio(config.startingCash, names);
        AIPortfolio = new Portfolio(config.startingCash, names);

        aiController = new AIController(AIPortfolio, config.allowedQuantities);


        shareSpawner.BuyClicked  += i => OpenTrade(true, i);
        shareSpawner.SellClicked += i => OpenTrade(false, i);
        shareSpawner.PassClicked += HandlePass;


        SetState(GameState.RoundStart);
    }



   public void SetState(GameState newState)
    {
        CurrentState = newState;
        UIManager.Instance.SetStateText(newState.ToString());

        switch (newState)
        {
            case GameState.RoundStart:
                shareSpawner.InitCompanyshareToPlayer();
                shareSpawner.SetShareButtons(false);
                UIManager.Instance.UpdatePlayerPortfolio(PlayerPortfolio, shareSpawner.Shares);
                StartCoroutine(RoundStartRoutine());
                break;

            case GameState.PlayerTurn:
                actionsRemaining = config.actionsPerRound;
                UIManager.Instance.SetActionsText(actionsRemaining);
                shareSpawner.RefreshButtons(PlayerPortfolio);
                UIManager.Instance.UpdatePlayerPortfolio(PlayerPortfolio, shareSpawner.Shares);
                break;
            case GameState.AITurn:
                shareSpawner.SetShareButtons(false);
                StartCoroutine(AITurnRoutine());
                break;
            case GameState.MarketReveal:
                shareSpawner.SetShareButtons(false);
                StartCoroutine(MarketRevealRoutine());
                break;

            case GameState.Settlement:
                shareSpawner.SetShareButtons(false);
                StartCoroutine(SettlementRoutine());
                break;
            case GameState.GameOver:
                shareSpawner.SetShareButtons(false);
                UIManager.Instance.ShowGameOver(NetWorth(PlayerPortfolio), NetWorth(AIPortfolio));
                break;

            default:
                shareSpawner.SetShareButtons(false);
                break;
        }
    }

    private IEnumerator RoundStartRoutine()
    {
        yield return new WaitForSeconds(2f);   // let the player see the share prices
        cardManager.DealCards();
        yield return new WaitForSeconds(3f);   // let the player read their cards
        SetState(GameState.PlayerTurn);
    }

    private System.Collections.IEnumerator ReadyToPlayerTurn()
    {
        yield return new WaitForSeconds(5f);
        SetState(GameState.PlayerTurn);
    }


    private IEnumerator AITurnRoutine()
    {
        for (int i = 0; i < config.actionsPerRound; i++)
        {
            string log = aiController.TakeAction(shareSpawner.Shares, cardManager.aiCards);
            Debug.Log($"AI action {i + 1}/{config.actionsPerRound}: {log}");
        }

        yield return new WaitForSeconds(5f);
        SetState(GameState.MarketReveal);
    }


    #region Trade Buy Phase
    private void OpenTrade(bool isBuy, int index)
    {
        if (CurrentState != GameState.PlayerTurn || actionsRemaining <= 0) return;

        var s = shareSpawner.Shares[index];
        tradePopup.Open(isBuy, s.companyName, s.currentPrice, PlayerPortfolio,
                        config.allowedQuantities, qty => ExecuteTrade(isBuy, index, qty));
    }

    private void ExecuteTrade(bool isBuy, int index, int qty)
    {
        if (CurrentState != GameState.PlayerTurn || actionsRemaining <= 0) return;
        if (System.Array.IndexOf(config.allowedQuantities, qty) < 0) return;

        var s = shareSpawner.Shares[index];
        bool ok = isBuy
            ? PlayerPortfolio.Buy(s.companyName, s.currentPrice, qty)
            : PlayerPortfolio.Sell(s.companyName, s.currentPrice, qty);

        if (!ok)
        {
            Debug.Log("Trade rejected. No action used.");
            return;                       // failed trade does NOT consume an action
        }
        ConsumeAction();
    }

    private void HandlePass(int index)
    {
        if (CurrentState != GameState.PlayerTurn || actionsRemaining <= 0) return;
        ConsumeAction();                  // no popup, just uses the action
    }

    private void ConsumeAction()
    {
        actionsRemaining--;
        UIManager.Instance.SetActionsText(actionsRemaining);
        UIManager.Instance.UpdatePlayerPortfolio(PlayerPortfolio, shareSpawner.Shares);

        if (actionsRemaining > 0)
            shareSpawner.RefreshButtons(PlayerPortfolio);
        else
            SetState(GameState.AITurn);   // buttons get disabled by the default case
    }

    #endregion




    #region MarketReveal


    private IEnumerator MarketRevealRoutine()
    {
        yield return StartCoroutine(cardManager.RevealAICards(0.8f));   // flip AI cards one by one
        yield return new WaitForSeconds(1f);

        ApplyMarketMovement();
        yield return new WaitForSeconds(2f);

        SetState(GameState.Settlement);
    }
    private void ApplyMarketMovement()
    {
        var shares = shareSpawner.Shares;

        for (int i = 0; i < shares.Count; i++)
        {
            string name = shares[i].companyName;
            int net = 0;
            bool frozen = false;

            // Sum all 6 cards (player + AI) for this company
            foreach (var card in AllCards())
            {
                if (card.company == null || card.company.companyName != name) continue;
                if (card.effectType == CardEffectType.MarketFreeze) frozen = true;
                else net += card.priceChange;
            }

            int oldPrice = shares[i].currentPrice;
            int newPrice = frozen ? oldPrice : Mathf.Max(config.minPrice, oldPrice + net);   // clamp once, on the total

            shareSpawner.SetPrice(i, newPrice);
            Debug.Log($"{name}: Rs. {oldPrice} -> Rs. {newPrice} (net {net}{(frozen ? ", FROZEN" : "")})");
        }
    }

    private IEnumerable<MarketCardData> AllCards()
    {
        foreach (var c in cardManager.playerCards) yield return c;
        foreach (var c in cardManager.aiCards) yield return c;
    }

    private IEnumerator SettlementRoutine()
    {
        UIManager.Instance.UpdatePlayerPortfolio(PlayerPortfolio, shareSpawner.Shares);
        Debug.Log($"Round {currentRound} settled. Player net worth: Rs. {NetWorth(PlayerPortfolio)} | AI net worth: Rs. {NetWorth(AIPortfolio)}");

        yield return new WaitForSeconds(5f);

        if (currentRound >= config.totalRounds)
        {
            SetState(GameState.GameOver);
        }
        else
        {
            currentRound++;
            SetState(GameState.RoundStart);
        }
    }

    private int NetWorth(Portfolio p)
    {
        int value = 0;
        foreach (var s in shareSpawner.Shares)
            value += p.GetShares(s.companyName) * s.currentPrice;
        return p.Cash + value;
    }


    #endregion

















}


[System.Serializable]
public class CompanyShareInfo
{
    public string companyName;
    public int currentPrice;
}