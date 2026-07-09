using UnityEngine;
using System.Collections;
using System;

public class YesNoDialog : Dialog{
    public Action onYesClick;
    public Action onNoClick;
    public GameObject title, message;

    public virtual void OnYesClick()
    {
        if (onYesClick != null) onYesClick();
        if (Sound.instance != null) Sound.instance.PlayButtonClick();
        Close();
    }

    public virtual void OnNoClick()
    {
        if (onNoClick != null) onNoClick();
        if (Sound.instance != null) Sound.instance.PlayButtonClick();
        Close();
    }
}
