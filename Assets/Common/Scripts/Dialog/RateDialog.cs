using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateDialog : Dialog {

	public void OnYesClick()
    {
        CUtils.RateGame();
    }

    public void OnNoClick()
    {
        Close();
    }
}
