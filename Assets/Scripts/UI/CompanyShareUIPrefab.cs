using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanyShareUIPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text tmp_CompanyName;
    [SerializeField] TMP_Text tmp_SharePrice;
    [SerializeField] Button buyButton;
    [SerializeField] Button sellButton;
    [SerializeField] Button passButton;



    public void Init(string companyName,string sharePrice)
    {
        tmp_CompanyName.text = companyName;
        tmp_SharePrice.text = sharePrice;

    }

    public void Init(string companyName, string sharePrice, Action onBuy, Action onSell, Action onPass)
    {
        tmp_CompanyName.text = companyName;
        tmp_SharePrice.text = sharePrice;

        buyButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
        passButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke());
        sellButton.onClick.AddListener(() => onSell?.Invoke());
        passButton.onClick.AddListener(() => onPass?.Invoke());
    }
    public void SetInteractable(bool value)
    {
        buyButton.interactable = value;
        sellButton.interactable = value;
        passButton.interactable = value;
    }

    public void UpdatePrice(int price)
    {
        tmp_SharePrice.text = "Rs. " + price;
    }

    public void SetButtons(bool canBuy, bool canSell, bool canPass)
    {
        buyButton.interactable = canBuy;
        sellButton.interactable = canSell;
        passButton.interactable = canPass;
    }

}
