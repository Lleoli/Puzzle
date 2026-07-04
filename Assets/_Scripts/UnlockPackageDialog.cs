using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class UnlockPackageDialog : Dialog
{
    public int worldIndex;

    protected override void Start()
    {
        base.Start();
        Purchaser.instance.onItemPurchased += OnItemPurchased;
    }

    public void OnUnlock()
    {
        Sound.instance.PlayButton();
        Purchaser.instance.BuyProduct(5);
        Close();
    }

    private void OnItemPurchased(IAPItem item, int index)
    {
        // A consumable product has been purchased by this user.
        if (item.productType == ProductType.Consumable)
        {
            Toast.instance.ShowMessage("Your purchase is successful");
            CUtils.SetBuyItem();

            Prefs.UnlockWorld(Prefs.currentMode, worldIndex);
            WorldController.instance.UpdateUI();
        }
    }

    private void OnDestroy()
    {
        Purchaser.instance.onItemPurchased -= OnItemPurchased;
    }
}
