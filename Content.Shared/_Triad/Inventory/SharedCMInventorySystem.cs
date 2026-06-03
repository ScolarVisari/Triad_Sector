using Content.Shared._RMC14.Input;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.Input.Binding;
using System.Linq;

namespace Content.Shared._RMC14.Inventory;

public abstract class SharedCMInventorySystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;

    private EntityQuery<RMCPickupDroppedItemsComponent> _pickupDroppedItemsQuery;

    public override void Initialize()
    {
        _pickupDroppedItemsQuery = GetEntityQuery<RMCPickupDroppedItemsComponent>();

        SubscribeLocalEvent<RMCItemPickupComponent, DroppedEvent>(OnItemDropped);
        SubscribeLocalEvent<RMCItemPickupComponent, RMCDroppedEvent>(OnItemDropped);

        CommandBinds.Builder
            .Bind(CMKeyFunctions.RMCPickUpDroppedItems,
                InputCmdHandler.FromDelegate(session =>
                {
                    if (session?.AttachedEntity is { } entity)
                        TryPickupDroppedItems(entity);
                }, handle: false))
            .Register<SharedCMInventorySystem>();
    }

    public override void Shutdown()
    {
        base.Shutdown();
        CommandBinds.Unregister<SharedCMInventorySystem>();
    }

    protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref DroppedEvent args)
        => HandleDroppedItem(ent, args.User);

    protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref RMCDroppedEvent args)
        => HandleDroppedItem(ent, args.User);

    protected void HandleDroppedItem(Entity<RMCItemPickupComponent> item, EntityUid user)
    {
        if (_pickupDroppedItemsQuery.TryComp(user, out var pickupDroppedItems))
            pickupDroppedItems.DroppedItems.Add(item.Owner);
    }

    protected void TryPickupDroppedItems(EntityUid user)
    {
        if (!_pickupDroppedItemsQuery.TryComp(user, out var pickupDroppedItems))
            return;

        var sortedItems = pickupDroppedItems.DroppedItems
            .OrderByDescending(item => HasComp<GunComponent>(item))
            .ThenByDescending(item => HasComp<MeleeWeaponComponent>(item))
            .ToList();

        foreach (var item in sortedItems.Distinct())
        {
            if (!_container.IsEntityInContainer(item) && _interaction.InRangeUnobstructed(user, item))
            {
                if (_hands.TryPickupAnyHand(user, item))
                {
                    pickupDroppedItems.DroppedItems.Remove(item);
                    break;
                }
            }
        }
    }
}
