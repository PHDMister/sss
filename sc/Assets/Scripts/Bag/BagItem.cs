public class BagItem
{
    private island_item cfg;
    public int CfgId => cfg.item_id;
    public string Name => cfg.item_name;
    public string Icon => cfg.item_icon;
    public int BigProbability => cfg.big_probability;
    public int Price => cfg.price;
    public int Type => cfg.item_type;
    public uint Count
    {
        get;
        private set;
    }

    public void UpdateCount(uint count)
    {
        Count = count;
    }

    public BagItem(island_item cfg)
    {
        this.cfg = cfg;
    }
}