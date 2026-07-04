using UnityEngine;
using System.Collections;

public class QuitGameDialog : Dialog
{
    protected override void Start()
    {
        base.Start();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
