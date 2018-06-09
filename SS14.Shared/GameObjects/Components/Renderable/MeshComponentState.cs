using System;
using System.Collections.Generic;
using SS14.Shared.Maths;
using SS14.Shared.Serialization;

namespace SS14.Shared.GameObjects
{
    [Serializable, NetSerializable]
    public class MeshComponentState : ComponentState
    {
        public readonly bool Visible;
        public readonly Vector3 Scale;
        public readonly Vector3 Rotation;
        public readonly Vector3 Offset;
        public readonly string ScenePath;
        public readonly string MeshInstance;

        public MeshComponentState(
            bool visible,
            Vector3 scale,
            Vector3 rotation,
            Vector3 offset,
            string scenepath,
            string meshinstance)
            : base(NetIDs.MESH)
        {
            Visible = visible;
            Scale = scale;
            Rotation = rotation;
            Offset = offset;
            ScenePath = scenepath;
            MeshInstance = meshinstance;
        }
    }
}
