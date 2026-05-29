using Robust.Shared.GameStates;

namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed partial class MagazineFireRateModifierComponent : Component
{
    [DataField]
    public float FireRateMultiplier = 1f;

    [DataField]
    public float FireRateOverride = 0f;
}