using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainCanvas;   // the whole game UI
    [SerializeField] private CanvasGroup popupCanvas;  // this popup
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button[] quantityButtons; // 3 buttons
    [SerializeField] private TMP_Text[] quantityLabels;
    [SerializeField] private Button closeButton;

    private int[] quantities;
    private Action<int> onSelected;
    private bool isOpen;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        for (int i = 0; i < quantityButtons.Length; i++)
        {
            int idx = i;
            quantityButtons[i].onClick.AddListener(() => OnQuantityClicked(idx));
        }
        SetPopupVisible(false);
    }

    public void Open(bool isBuy, string company, int price, Portfolio portfolio,
                     int[] allowedQuantities, Action<int> onSelectedCallback)
    {
        if (isOpen) return;
        isOpen = true;
        quantities = allowedQuantities;
        onSelected = onSelectedCallback;

        titleText.text = (isBuy ? "Buy " : "Sell ") + company + $" (Rs. {price})";

        for (int i = 0; i < quantityButtons.Length; i++)
        {
            bool valid = i < quantities.Length;
            quantityButtons[i].gameObject.SetActive(valid);
            if (!valid) continue;

            int qty = quantities[i];
            quantityLabels[i].text = qty + (qty == 1 ? " share" : " shares");
            quantityButtons[i].interactable = isBuy
                ? portfolio.CanBuy(price, qty)
                : portfolio.CanSell(company, qty);
        }

        mainCanvas.alpha = 0.2f;
        mainCanvas.interactable = false;
        SetPopupVisible(true);
    }

    private void OnQuantityClicked(int index)
    {
        if (!isOpen) return;               // blocks rapid double-clicks
        int qty = quantities[index];
        var callback = onSelected;
        Close();
        callback?.Invoke(qty);
    }

    public void Close()
    {
        isOpen = false;
        onSelected = null;
        mainCanvas.alpha = 1f;
        mainCanvas.interactable = true;
        SetPopupVisible(false);
    }

    private void SetPopupVisible(bool visible)
    {
        popupCanvas.alpha = visible ? 1f : 0f;
        popupCanvas.interactable = visible;
        popupCanvas.blocksRaycasts = visible;
    }
}