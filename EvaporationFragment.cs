using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace TankEvaporation;

public class EvaporationFragment : IEntityPanelFragment
{
    private const string WaterGoodId = "Water";
    private const string HeaderKey = "TE.Header";
    private const string DailyLeakKey = "TE.DailyLeak";
    private const string DaysEmptyKey = "TE.DaysEmpty";

    private readonly ILoc _loc;

    private VisualElement _root = null!;
    private Label _headerLabel = null!;
    private Label _dailyLeakLabel = null!;
    private Label _daysEmptyLabel = null!;

    private SingleGoodAllower? _singleGoodAllower;
    private Inventory? _inventory;

    public EvaporationFragment(ILoc loc)
    {
        _loc = loc;
    }

    public VisualElement InitializeFragment()
    {
        _root = new VisualElement();
        _root.style.marginTop = 4;
        _root.style.paddingTop = 8;
        _root.style.paddingBottom = 8;
        _root.style.paddingLeft = 10;
        _root.style.paddingRight = 10;
        _root.style.backgroundColor = new StyleColor(new Color(0.239f, 0.180f, 0.118f, 1f));
        var borderColor = new StyleColor(new Color(0.420f, 0.310f, 0.165f));
        _root.style.borderTopWidth = 1;
        _root.style.borderBottomWidth = 1;
        _root.style.borderLeftWidth = 1;
        _root.style.borderRightWidth = 1;
        _root.style.borderTopColor = borderColor;
        _root.style.borderBottomColor = borderColor;
        _root.style.borderLeftColor = borderColor;
        _root.style.borderRightColor = borderColor;

        _headerLabel = new Label();
        _headerLabel.style.color = new StyleColor(new Color(0.941f, 0.753f, 0.251f));
        _headerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        _headerLabel.style.marginBottom = 4;
        _root.Add(_headerLabel);

        _dailyLeakLabel = new Label();
        _dailyLeakLabel.style.color = new StyleColor(new Color(0.831f, 0.769f, 0.659f));
        _root.Add(_dailyLeakLabel);

        _daysEmptyLabel = new Label();
        _daysEmptyLabel.style.color = new StyleColor(new Color(0.831f, 0.769f, 0.659f));
        _root.Add(_daysEmptyLabel);

        _root.style.display = DisplayStyle.None;
        return _root;
    }

    public void ShowFragment(BaseComponent entity)
    {
        _singleGoodAllower = entity.GetComponent<SingleGoodAllower>();
        if (_singleGoodAllower == null)
        {
            _root.style.display = DisplayStyle.None;
            return;
        }

        var inventories = entity.GetComponentsAllocating<Inventory>();
        _inventory = inventories != null && inventories.Count > 0 ? inventories[0] : null;

        if (_inventory == null)
        {
            _root.style.display = DisplayStyle.None;
            return;
        }

        // Hide panel on non-water tanks when Water Only setting is enabled
        bool waterOnly = TankEvaporationSettings.Instance?.WaterOnly.Value ?? false;
        if (waterOnly && _singleGoodAllower.HasAllowedGood && _singleGoodAllower.AllowedGood != WaterGoodId)
        {
            _root.style.display = DisplayStyle.None;
            return;
        }

        _root.style.display = DisplayStyle.Flex;
    }

    public void ClearFragment()
    {
        _singleGoodAllower = null;
        _inventory = null;
        _root.style.display = DisplayStyle.None;
    }

    public void UpdateFragment()
    {
        if (_root.style.display == DisplayStyle.None) return;
        if (_singleGoodAllower == null || _inventory == null) return;

        _headerLabel.text = _loc.T(HeaderKey);

        if (!_singleGoodAllower.HasAllowedGood)
        {
            _dailyLeakLabel.text = _loc.T(DailyLeakKey, "-", "-");
            _daysEmptyLabel.text = _loc.T(DaysEmptyKey, "-");
            return;
        }

        int rate = TankEvaporationSettings.Instance?.EvaporationPercent.Value ?? 5;
        float rateF = rate / 100f;
        int maxCapacity = _inventory.Capacity;
        int stored = _inventory.AmountInStock(_singleGoodAllower.AllowedGood);
        int dailyLoss = UnityEngine.Mathf.Max(1, UnityEngine.Mathf.RoundToInt(maxCapacity * rateF));

        _dailyLeakLabel.text = _loc.T(DailyLeakKey, dailyLoss.ToString(), rate.ToString());

        if (stored > 0 && dailyLoss > 0)
        {
            float daysLeft = (float)stored / dailyLoss;
            _daysEmptyLabel.text = _loc.T(DaysEmptyKey, daysLeft.ToString("F0"));
        }
        else
        {
            _daysEmptyLabel.text = _loc.T(DaysEmptyKey, "0");
        }
    }
}