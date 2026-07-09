using System;
using System.Collections.Generic;
using System.IO;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : Dialog
{
    private const string CsvTablePath = "DataTable/shopdialog_source";
    private const int MoveStepItemBonus = 3;

    public Text[] rubyNumbers;
    public Text[] prices;

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
        if (cost > 0 && !CurrencyController.DebitBalance(cost))
        {
            if (Toast.instance != null)
                Toast.instance.ShowMessage("金币不足");
            return;
        }

        OnShopItemPurchased(item, index);
    }

    private bool CanUseProduct(int index)
    {
        if (index != 0)
            return true;

        bool canUse = Board.instance != null && (MainController.instance == null || !MainController.instance.isComplete);
        if (!canUse && Toast.instance != null)
            Toast.instance.ShowMessage("当前关卡无法使用该道具");

        return canUse;
    }

    private List<shopdialog> LoadTableRows()
    {
#if UNITY_EDITOR
        string editorCsvPath = Path.Combine(Application.dataPath, "../ExcelData/shopdialog.csv");
        return LoadCsvRows(File.ReadAllText(editorCsvPath));
#else
        List<shopdialog> rows = shopdialog.LoadBytes();
        if (rows != null)
            return rows;

        TextAsset asset = Resources.Load<TextAsset>(CsvTablePath);
        return LoadCsvRows(asset.text);
#endif
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

        SetTextByCurrentValue("道具商店", GetLabel(labels, "shop_tab_item"));
        SetTextByCurrentValue("引导视频", GetLabel(labels, "shop_tab_video"));
        SetTextByCurrentValue("当前数量", GetLabel(labels, "footer_label"));
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
            case 0:
                OnShopItem0Effect();
                break;
            case 1:
                OnShopItem1Effect();
                break;
            case 2:
                OnShopItem2Effect();
                break;
            case 3:
                OnShopItem3Effect();
                break;
            case 4:
                OnShopItem4Effect();
                break;
        }
    }

    protected virtual void OnShopItem0Effect()
    {
        if (Board.instance != null && Board.instance.AddTargetMoves(MoveStepItemBonus))
        {
            if (Toast.instance != null)
                Toast.instance.ShowMessage("步数+" + MoveStepItemBonus);

            Close();
        }
    }

    protected virtual void OnShopItem1Effect()
    {
    }

    protected virtual void OnShopItem2Effect()
    {
    }

    protected virtual void OnShopItem3Effect()
    {
    }

    protected virtual void OnShopItem4Effect()
    {
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
