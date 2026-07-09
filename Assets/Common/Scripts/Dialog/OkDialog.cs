using UnityEngine;
using System.Collections;
using System;

public class OkDialog : Dialog {
    public Action onOkClick;
    public GameObject title, message;
    public virtual void OnOkClick()
    {
        if (Sound.instance != null) Sound.instance.PlayButtonClick();
        if (onOkClick != null) onOkClick();
        if (Sound.instance != null) Sound.instance.PlayButtonClick();
        Close();
    }
}
