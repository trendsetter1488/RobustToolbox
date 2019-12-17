using tainicom.Aether.Physics2D.Dynamics;

namespace Robust.Shared.Physics
{
    internal interface IPhysBodyInternal : IPhysBody
    {
        Body AetherBody { get; }
    }
}
