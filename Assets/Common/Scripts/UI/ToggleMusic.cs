using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour {

    private void Start()
    {
        GetComponent<Toggle>().isOn = Music.instance.IsEnabled();
        GetComponent<Toggle>().onValueChanged.AddListener(UpdateUI);
    }

    private void UpdateUI(bool isOn)
    {
        Music.instance.SetEnabled(isOn, true);
    }
}
