using System.Collections;
using UnityEngine.UI;

public class SkipLevelDialog : Dialog
{
    public Text message;

    protected override void Start()
    {
        base.Start();
        message.text = string.Format(message.text, ConfigController.Config.skipLevelCost);
    }

    public void OnYes()
    {
        Sound.instance.PlayButton();
        bool result = CurrencyController.DebitBalance(ConfigController.Config.skipLevelCost);
        if (result)
        {
            MainController.instance.OnComplete();
            MainController.instance.OnBallToGoal();
            Close();
        }
        else
        {
            Toast.instance.ShowMessage("You don't have enough rubies");
        }
    }
}
