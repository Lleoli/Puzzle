using UnityEngine;
using System.Collections;
using System.IO;

public class DevelopmentOnly : MonoBehaviour {
    public bool setBalance;
    public int ruby;

    public bool unlockAllLevels;

    public bool clearAllPrefs;

    private void Start()
    {
        if (setBalance)
            CurrencyController.SetBalance(ruby);
        
        if (unlockAllLevels)
        {
            for(int i = 0; i < Const.NUMWORLD; i++)
            {
                Prefs.UnlockWorld("Classic", i);
                Prefs.SetUnlockedLevel("Classic", i, Const.NUMLEVEL);
            }

            for (int i = 0; i < Const.NUMWORLD; i++)
            {
                Prefs.UnlockWorld("Star", i);
                Prefs.SetUnlockedLevel("Star", i, Const.NUMLEVEL);
            }
        }

        if (clearAllPrefs)
        {
            CPlayerPrefs.DeleteAll();
            CPlayerPrefs.Save();
        }
    }
}
