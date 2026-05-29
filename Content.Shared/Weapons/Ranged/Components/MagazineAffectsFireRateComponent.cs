using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class MagazineAffectsFireRateComponent : Component
{
    [DataField]
    public float? DefaultFireRate;
}