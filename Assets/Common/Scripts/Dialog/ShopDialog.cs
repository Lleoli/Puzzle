using System;
using System.Collections.Generic;
using System.IO;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : Dialog
{
    private const string CsvTablePath = "DataTable/shopdialog_source";
    private const int MoveStepItemBonus = 10;
    private const int FishPackAmount = 3;
    private const int RouteHintIndex = 0;
    private const int AddMovesIndex = 1;
    private const int SkipLevelIndex = 2;
    private const int FishPackIndex = 3;
    private const int CatSkinIndex = 4;

    public Text[] rubyNumbers;
    public Text[] prices;
    [SerializeField] private VideoPlay watchVideoPlayer;

    private List<shopdialog> products = new List<shopdialog>();

    protected override void Start()
    {
        base.Start();

        List<shopdialog> rows = LoadTableRows();
        ApplyLabels(rows);
        ApplyProducts(rows);
    }

    public void OnBuyProduct(int index)
    {
        if (Sound.instance != null) Sound.instance.PlayButtonClick();

        shopdialog item = GetProduct(index);
        if (item == null)
            return;

        if (!CanUseProduct(index))
            return;

        int cost = GetItemCost(item);
        if (!TryPayProduct(index, cost))
            return;

        OnShopItemPurchased(item, index);
    }

    public void OnWatchVideoClick()
    {
        if (Sound.instance != null) Sound.instance.PlayButtonClick();

        VideoPlay player = GetWatchVideoPlayer();
        if (player == null)
        {
            ShowToast("\u672a\u914d\u7f6e\u89c6\u9891");
            return;
        }

        Close();
        player.PlayVideo();
    }

    private VideoPlay GetWatchVideoPlayer()
    {
        if (watchVideoPlayer != null)
            return watchVideoPlayer;

        VideoPlay[] players = Resources.FindObjectsOfTypeAll<VideoPlay>();
        foreach (VideoPlay player in players)
        {
            if (player != null && player.gameObject.scene.IsValid())
            {
                watchVideoPlayer = player;
                return watchVideoPlayer;
            }
        }

        return null;
    }
    private bool CanUseProduct(int index)
    {
        if (index == CatSkinIndex)
        {
            ShowToast("\u6682\u672a\u89e3\u9501");
            return false;
        }

        if (index == FishPackIndex)
            return true;

        bool canUse = Board.instance != null && (MainController.instance == null || !MainController.instance.isComplete);
        if (!canUse)
        {
            ShowToast("\u5f53\u524d\u5173\u5361\u65e0\u6cd5\u4f7f\u7528\u8be5\u9053\u5177");
            return false;
        }

        if (index == RouteHintIndex && (Board.instance.hintBeginShowing || Board.instance.hintShowing))
        {
            ShowToast("\u8def\u7ebf\u63d0\u793a\u5df2\u663e\u793a");
            return false;
        }

        return true;
    }

    private bool TryPayProduct(int index, int cost)
    {
        if (cost <= 0)
            return true;

        if (index == FishPackIndex)
        {
            if (CurrencyController.DebitBalance(cost))
                return true;

            ShowToast("\u94bb\u77f3\u4e0d\u8db3");
            return false;
        }

        if (StarCurrencyController.DebitBalance(cost))
            return true;

        ShowToast("\u5c0f\u9c7c\u5e72\u4e0d\u8db3");
        return false;
    }

    private List<shopdialog> LoadTableRows()
    {
        TextAsset asset = Resources.Load<TextAsset>(CsvTablePath);
        if (asset != null)
            return LoadCsvRows(asset.text);

#if UNITY_EDITOR
        string editorCsvPath = Path.Combine(Application.dataPath, "../ExcelData/shopdialog.csv");
        if (File.Exists(editorCsvPath))
            return LoadCsvRows(File.ReadAllText(editorCsvPath));
#endif

        List<shopdialog> rows = shopdialog.LoadBytes();
        return rows ?? new List<shopdialog>();
    }

    private List<shopdialog> LoadCsvRows(string tableText)
    {
        List<shopdialog> rows = new List<shopdialog>();
        using (StringReader reader = new StringReader(tableText))
        {
            string line;
            int lineIndex = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineIndex++;
                if (lineIndex <= 3 || string.IsNullOrEmpty(line.Trim()))
                    continue;

                string[] columns = line.Split(',');
                if (columns.Length < 6)
                    continue;

                rows.Add(new shopdialog
                {
                    type = columns[0].Trim(),
                    key = columns[1].Trim(),
                    text = columns[2].Trim(),
                    price = columns[3].Trim(),
                    cost = ParseInt(columns[4]),
                    active = ParseBool(columns[5])
                });
            }
        }

        return rows;
    }

    private void ApplyLabels(List<shopdialog> rows)
    {
        Dictionary<string, string> labels = new Dictionary<string, string>();
        foreach (shopdialog row in rows)
        {
            if (!IsRowType(row, "label") || string.IsNullOrEmpty(row.key))
                continue;

            labels[row.key] = row.text;
        }

        SetTextByCurrentValue("\u9053\u5177\u5546\u5e97", GetLabel(labels, "shop_tab_item"));
        SetTextByCurrentValue("\u5f15\u5bfc\u89c6\u9891", GetLabel(labels, "shop_tab_video"));
        SetTextByCurrentValue("\u5f53\u524d\u6570\u91cf", GetLabel(labels, "footer_label"));
        SetTextByCurrentValue("watch  video", GetLabel(labels, "video_button"));
        SetTextByCurrentValue("watch video", GetLabel(labels, "video_button"));
        SetTextByCurrentValue("3", GetLabel(labels, "video_cost"));
    }

    private void ApplyProducts(List<shopdialog> rows)
    {
        products = new List<shopdialog>();
        foreach (shopdialog row in rows)
        {
            if (IsRowType(row, "product") && row.active)
                products.Add(row);
        }

        products.Sort((a, b) => ParseInt(a.key).CompareTo(ParseInt(b.key)));

        int count = Mathf.Min(products.Count, rubyNumbers == null ? 0 : rubyNumbers.Length, prices == null ? 0 : prices.Length);
        for (int i = 0; i < count; i++)
        {
            shopdialog item = products[i];
            if (rubyNumbers[i] != null) rubyNumbers[i].text = item.text;
            if (prices[i] != null) prices[i].text = item.price;
        }
    }

    private shopdialog GetProduct(int index)
    {
        if (products == null || products.Count == 0)
            ApplyProducts(LoadTableRows());

        if (products == null || index < 0 || index >= products.Count)
            return null;

        return products[index];
    }

    protected virtual void OnShopItemPurchased(shopdialog item, int index)
    {
        switch (index)
        {
            case RouteHintIndex:
                OnShopItem0Effect();
                break;
            case AddMovesIndex:
                OnShopItem1Effect();
                break;
            case SkipLevelIndex:
                OnShopItem2Effect();
                break;
            case FishPackIndex:
                OnShopItem3Effect();
                break;
            case CatSkinIndex:
                OnShopItem4Effect();
                break;
        }
    }

    protected virtual void OnShopItem0Effect()
    {
        if (Board.instance != null && Board.instance.ShowHintFromShop())
            Close();
    }

    protected virtual void OnShopItem1Effect()
    {
        if (Board.instance != null && Board.instance.AddTargetMoves(MoveStepItemBonus))
        {
            ShowToast("\u6b65\u6570+" + MoveStepItemBonus);
            Close();
        }
    }

    protected virtual void OnShopItem2Effect()
    {
        if (MainController.instance == null || MainController.instance.isComplete)
            return;

        Close();
        MainController.instance.OnComplete();
        MainController.instance.OnBallToGoal();
    }

    protected virtual void OnShopItem3Effect()
    {
        StarCurrencyController.CreditBonusStars(FishPackAmount);
        ShowToast("\u5c0f\u9c7c\u5e72+" + FishPackAmount);
        Close();
    }

    protected virtual void OnShopItem4Effect()
    {
        ShowToast("\u6682\u672a\u89e3\u9501");
    }

    private void ShowToast(string message)
    {
        if (Toast.instance != null)
            Toast.instance.ShowMessage(message);
    }

    private void SetTextByCurrentValue(string currentValue, string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
            return;

        Text[] texts = GetComponentsInChildren<Text>(true);
        foreach (Text text in texts)
        {
            if (text != null && text.text == currentValue)
                text.text = newValue;
        }
    }

    private static string GetLabel(Dictionary<string, string> labels, string key)
    {
        return labels.ContainsKey(key) ? labels[key] : null;
    }

    private static bool IsRowType(shopdialog row, string type)
    {
        return row != null && string.Equals(row.type, type, StringComparison.OrdinalIgnoreCase);
    }

    private static int ParseInt(string value)
    {
        int result;
        return int.TryParse(value, out result) ? result : 0;
    }

    private static bool ParseBool(string value)
    {
        bool result;
        return bool.TryParse(value, out result) ? result : true;
    }

    private static int GetItemCost(shopdialog item)
    {
        return item.cost;
    }
}
