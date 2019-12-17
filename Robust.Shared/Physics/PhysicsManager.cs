using System;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Physics;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using tainicom.Aether.Physics2D.Dynamics;

namespace Robust.Shared.Physics
{
    /// <inheritdoc />
    internal class PhysicsManager : IPhysicsManagerInternal
    {
        private readonly World _world = new World();

        /// <summary>
        ///     returns true if collider intersects a physBody under management. Does not trigger Bump.
        /// </summary>
        /// <param name="collider">Rectangle to check for collision</param>
        /// <param name="map">Map ID to filter</param>
        /// <returns></returns>
        public bool IsColliding(Box2 collider, MapId map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     returns true if collider intersects a physBody under management and calls Bump.
        /// </summary>
        /// <param name="entity">Rectangle to check for collision</param>
        /// <param name="offset"></param>
        /// <param name="bump"></param>
        /// <returns></returns>
        public bool TryCollide(IEntity entity, Vector2 offset, bool bump = true)
        {
            throw new NotImplementedException();
        }

        public void Step(float frameTime)
        {
            _world.Step(frameTime);
        }

        /// <summary>
        ///     Adds a physBody to the manager.
        /// </summary>
        /// <param name="physBody"></param>
        public void AddBody(IPhysBodyInternal physBody)
        {
            _world.Add(physBody.AetherBody);
        }

        /// <summary>
        ///     Removes a physBody from the manager
        /// </summary>
        /// <param name="physBody"></param>
        public void RemoveBody(IPhysBodyInternal physBody)
        {
            _world.Remove(physBody.AetherBody);
        }

        /// <inheritdoc />
        public RayCastResults IntersectRay(Ray ray, float maxLength = 50, IEntity ignoredEnt = null)
        {
            throw new NotImplementedException();
        }

        public event Action<DebugRayData> DebugDrawRay;
    }
}
