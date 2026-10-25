using Content.Shared.Radiation.Systems;
using System.Numerics;
using Robust.Shared.Prototypes;
using Content.Shared._Starfall.Particles;

namespace Content.Shared.Radiation.Components;

/// <summary>
///     Irradiate all objects in range.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedRadiationSystem))]
public sealed partial class RadiationSourceComponent : Component
{
    /// <summary>
    ///     Radiation intensity in center of the source in rads per second.
    ///     From there radiation rays will travel over distance and loose intensity
    ///     when hit radiation blocker.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("intensity")]
    public float Intensity = 1;

    /// <summary>
    ///     Defines how fast radiation rays will loose intensity
    ///     over distance. The bigger the value, the shorter range
    ///     of radiation source will be.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("slope")]
    public float Slope = 0.5f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled = true;

    /// <summary>The particle effect to emit.</summary>
    //[DataField(required: true)]
    [DataField]
    public ProtoId<ParticleEffectPrototype> Effect = "SfFireContinuous";

    /// <summary>
    /// Optional color tint applied to every particle of this emitter.
    /// Multiplied on top of the prototype colors; null leaves colors unchanged.
    /// </summary>
    [DataField]
    public Color? ColorOverride;

    /// <summary>
    /// Starting intensity multiplier (0–1+). Scales emission rate and particle size.
    /// 1.0 = normal.
    /// </summary>
    [DataField]
    public float VisualIntensity = 1f;

    /// <summary>
    /// Local-space offset applied to the spawn origin of this emitter.
    /// Useful for shifting the effect away from the entity's anchor point.
    /// </summary>
    [DataField]
    public Vector2 SpawnOffset;
}
