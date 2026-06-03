using System.Linq;
using Content.Client.Hands.Systems;
//using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Input;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects; // Triad
using Robust.Shared.Input.Binding;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Hands;

public sealed class ClientRMCHandsSystem : EntitySystem // Triad
{
    [Dependency] private readonly HandsSystem _hands = default!;

    public override void Initialize()
    {
        base.Initialize();

        CommandBinds.Builder
            .Bind(CMKeyFunctions.RMCInteractWithOtherHand,
                InputCmdHandler.FromDelegate(session =>
					{
						if (session?.AttachedEntity is not { } ent)
							return;

						if (!TryComp(ent, out HandsComponent? hands))
							return;

						// Triad start
						if (!hands.Hands.Values
                            .Where(h => h.Name != hands.ActiveHand?.Name)
                            .TryFirstOrDefault(out var other))
						{
							return;
						}

						_hands.UIHandClick(hands, other.Name);
						// Triad end
					}))
            .Register<ClientRMCHandsSystem>();
    }
}