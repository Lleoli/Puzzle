using UnityEngine;
using System.Collections;

public class ButtonQuitGame : MyButton
{
    protected override void Start()
    {
        base.Start();
#if UNITY_WEBGL
        gameObject.SetActive(false);
#endif
    }

    public override void OnButtonClick()
    {
        base.OnButtonClick();
        Application.Quit();
    }
}
