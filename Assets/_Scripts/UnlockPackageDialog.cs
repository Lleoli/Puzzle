using UnityEngine;

public class UnlockPackageDialog : Dialog
{
    public int worldIndex;

    protected override void Start()
    {
        base.Start();
    }

    public void OnUnlock()
    {
        Sound.instance.PlayButton();
        Close();
    }
}
