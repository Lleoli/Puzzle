using System;
using UnityEngine;

public enum ProductType
{
    Consumable,
    NonConsumable,
    Subscription
}

[Serializable]
public class IAPItem
{
    public string productID;
    public ProductType productType;
    public float price;
    public int value;
}

public class Purchaser : MonoBehaviour
{
    public static Purchaser instance;

    public IAPItem[] iapItems;
    public Action<IAPItem, int> onItemPurchased;

    private void Awake()
    {
        instance = this;
    }

    public void InitializePurchasing()
    {
    }

    public void BuyProduct(int index)
    {
    }

    public void BuyConsumable()
    {
    }

    public void BuyNonConsumable()
    {
    }

    public void BuySubscription()
    {
    }

    public void RestorePurchases()
    {
    }
}
