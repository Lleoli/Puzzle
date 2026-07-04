using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour {

    private void Start()
    {
        GetComponent<Toggle>().isOn = Sound.instance.IsEnabled();
        GetComponent<Toggle>().onValueChanged.AddListener(UpdateUI);
    }

    private void UpdateUI(bool isOn)
    {
        Sound.instance.SetEnabled(isOn);
    }
}
