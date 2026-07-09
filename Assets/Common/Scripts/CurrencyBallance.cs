using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CurrencyBallance : MonoBehaviour {
    private void Start()
    {
        UpdateBalance();
        StarCurrencyController.onBalanceChanged += OnBalanceChanged;
    }

    private void UpdateBalance()
    {
        gameObject.SetText(StarCurrencyController.GetBalance().ToString());
    }

    private void OnBalanceChanged()
    {
        UpdateBalance();
    }

    private void OnDestroy()
    {
        StarCurrencyController.onBalanceChanged -= OnBalanceChanged;
    }
}
