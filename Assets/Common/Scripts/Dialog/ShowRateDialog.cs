using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRateDialog : MonoBehaviour {

	public void Show()
    {
#if !UNITY_WEBGL
        if (!CUtils.IsGameRated())
        {
            Timer.Schedule(this, 0.3f, () =>
            {
                DialogController.instance.ShowDialog(DialogType.RateGame);
            });
        }
#endif
    }
}
