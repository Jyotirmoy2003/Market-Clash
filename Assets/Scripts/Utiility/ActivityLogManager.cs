using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityLogManager : MonoSingleton<ActivityLogManager>
{
    [Header("References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private TextMeshProUGUI logPrefab;

    public void AddLog(string message)
    {
        TextMeshProUGUI log = Instantiate(logPrefab, contentParent);
        log.text = message;
        Canvas.ForceUpdateCanvases();

        contentParent.GetComponentInParent<ScrollRect>()
            .verticalNormalizedPosition = 0f;
    }

    public void ClearLogs()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}