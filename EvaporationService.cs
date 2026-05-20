using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace TankEvaporation;

public class EvaporationService : ILoadableSingleton, IUnloadableSingleton
{
    private const string WaterGoodId = "Water";

    private readonly EventBus _eventBus;
    private readonly EntityRegistry _entityRegistry;

    public EvaporationService(EventBus eventBus, EntityRegistry entityRegistry)
    {
        _eventBus = eventBus;
        _entityRegistry = entityRegistry;
    }

    public void Load()
    {
        _eventBus.Register(this);
    }

    public void Unload()
    {
        _eventBus.Unregister(this);
    }

    [OnEvent]
    public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
    {
        DrainAllTanks();
    }

    private void DrainAllTanks()
    {
        var settings = TankEvaporationSettings.Instance;
        float rate = (settings?.EvaporationPercent.Value ?? 5) / 100f;
        bool waterOnly = settings?.WaterOnly.Value ?? false;

        foreach (var entity in _entityRegistry.Entities)
        {
            var singleGoodAllower = entity.GetComponent<SingleGoodAllower>();
            if (singleGoodAllower == null) continue;

            if (!singleGoodAllower.HasAllowedGood) continue;

            // If water-only mode is on, skip tanks not set to water
            if (waterOnly && singleGoodAllower.AllowedGood != WaterGoodId) continue;

            var blockable = entity.GetComponent<BlockableObject>();
            if (blockable != null && !blockable.IsUnblocked) continue;

            var inventories = entity.GetComponentsAllocating<Inventory>();
            if (inventories == null || inventories.Count == 0) continue;

            string goodId = singleGoodAllower.AllowedGood;

            foreach (var inventory in inventories)
            {
                int maxCapacity = inventory.Capacity;
                if (maxCapacity <= 0) continue;

                int stored = inventory.AmountInStock(goodId);
                if (stored <= 0) continue;

                int loss = UnityEngine.Mathf.Max(1, UnityEngine.Mathf.RoundToInt(maxCapacity * rate));
                loss = UnityEngine.Mathf.Min(loss, stored);

                inventory.Take(new GoodAmount(goodId, loss));
            }
        }
    }
}