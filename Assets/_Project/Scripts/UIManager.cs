using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private ConstructionSite constructionSite; 

    private int currentMoney = 100;

    private void Start()
    {
        if (constructionSite != null)
        {
            constructionSite.OnPartCompleted += HandlePartCompleted;
        }

        UpdateMoneyText();
    }

    private void OnDestroy()
    {
        if (constructionSite != null)
        {
            constructionSite.OnPartCompleted -= HandlePartCompleted;
        }
    }

    private void HandlePartCompleted()
    {
        currentMoney++;
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }
    
    // public void AddMoney(int amount)
    // {
    //     currentMoney += amount;
    //     UpdateMoneyText();
    // }
}