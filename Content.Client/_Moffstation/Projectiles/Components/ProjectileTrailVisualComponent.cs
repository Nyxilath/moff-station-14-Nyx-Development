using Content.Client.Projectiles;
using Content.Client._Starfall.Particles;

namespace Content.Client._Moffstation.Projectiles.Components;

[RegisterComponent, Access(typeof(ProjectileSystem))]
public sealed partial class ProjectileTrailVisualComponent : Component
{
    [ViewVariables]
    public ActiveEmitter? Emitter;
}
