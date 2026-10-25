using Content.Client._Starfall.Particles;
using Content.Client.Radiation.Overlays;
using Content.Shared.Radiation.Events;
using Content.Shared.Radiation.Systems;
using Robust.Client.Graphics;

using Content.Shared._Starfall.Particles;
using Content.Shared.Radiation.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Radiation.Systems;

public sealed partial class RadiationSystem : SharedRadiationSystem
{
    [Dependency] private IOverlayManager _overlayMan = default!;

    [Dependency] private ParticleSystem _particles = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    // Track emitter references so we can stop them when the entity leaves PVS or is removed.
    private readonly Dictionary<EntityUid, ActiveEmitter> _activeEmitters = new();

    public List<DebugRadiationRay>? Rays;
    public Dictionary<NetEntity, Dictionary<Vector2i, float>>? ResistanceGrids;

    public override void Initialize()
    {
        SubscribeNetworkEvent<OnRadiationOverlayToggledEvent>(OnOverlayToggled);
        SubscribeNetworkEvent<OnRadiationOverlayUpdateEvent>(OnOverlayUpdate);
        SubscribeNetworkEvent<OnRadiationOverlayResistanceUpdateEvent>(OnResistanceUpdate);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlayMan.RemoveOverlay<RadiationDebugOverlay>();
    }

    private void OnOverlayToggled(OnRadiationOverlayToggledEvent ev)
    {
        if (ev.IsEnabled)
            _overlayMan.AddOverlay(new RadiationDebugOverlay());
        else
            _overlayMan.RemoveOverlay<RadiationDebugOverlay>();
    }

    private void OnOverlayUpdate(OnRadiationOverlayUpdateEvent ev)
    {
        if (!_overlayMan.TryGetOverlay(out RadiationDebugOverlay? overlay))
            return;

        var str = $"Radiation update: {ev.ElapsedTimeMs}ms with. Receivers: {ev.ReceiversCount}, " +
                  $"Sources: {ev.SourcesCount}, Rays: {ev.Rays.Count}";
        Log.Info(str);

        Rays = ev.Rays;
    }

    private void OnResistanceUpdate(OnRadiationOverlayResistanceUpdateEvent ev)
    {
        ResistanceGrids = ev.Grids;
    }

    [SubscribeLocalEvent]
    private void OnCompInit(Entity<RadiationSourceComponent> ent, ref ComponentInit args)
    {
        // Stop any lingering emitter from a previous PVS cycle for this entity.
        if (_activeEmitters.TryGetValue(ent.Owner, out var old))
        {
            _particles.RemoveParticle(old);
            _activeEmitters.Remove(ent.Owner);
        }

        var coords = _transform.GetMapCoordinates(ent.Owner);
        var emitter = _particles.SpawnEffect(ent.Comp.Effect, coords, ent.Owner, ent.Comp.ColorOverride);
        if (emitter == null)
            return;

        if (ent.Comp.VisualIntensity != 1f)
            emitter.Intensity = ent.Comp.VisualIntensity;

        if (ent.Comp.SpawnOffset != default)
            emitter.SpawnOffset = ent.Comp.SpawnOffset;

        _activeEmitters[ent.Owner] = emitter;
    }

    [SubscribeLocalEvent]
    private void OnCompShutdown(Entity<RadiationSourceComponent> ent, ref ComponentShutdown args)
    {
        if (_activeEmitters.Remove(ent.Owner, out var emitter))
            _particles.RemoveParticle(emitter);
    }
}
