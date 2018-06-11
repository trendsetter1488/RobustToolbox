using System;
using System.Collections.Generic;
using System.Linq;
using SS14.Server.Interfaces.GameObjects;
using SS14.Shared.GameObjects;
using SS14.Shared.GameObjects.Serialization;
using SS14.Shared.Log;
using SS14.Shared.Maths;
using SS14.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace SS14.Server.GameObjects
{
    public class MeshComponent : Component
    {
        public override string Name => "Mesh";
        public override uint? NetID => NetIDs.MESH;

        private bool _visible;
        private Vector3 _scale;
        private Vector3 _rotation;
        private Vector3 _offset;
        private string _scenename;
        private string _meshinstance;

        public override void ExposeData(EntitySerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _visible, "visible", true);
            serializer.DataField(ref _scale, "scale", Vector3.One);
            serializer.DataField(ref _rotation, "rotation", Vector3.Zero);
            serializer.DataField(ref _offset, "offset", Vector3.Zero);
            serializer.DataField(ref _scenename, "scene", null);
            serializer.DataField(ref _meshinstance, "mesh", null);
        }

        public override ComponentState GetComponentState()
        {
            return new MeshComponentState(_visible, _scale, _rotation, _offset, _scenename, _meshinstance);
        }

        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                Dirty();
            }
        }

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                Dirty();
            }
        }

        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                Dirty();
            }
        }

        public Vector3 Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                Dirty();
            }
        }

        public string SceneName
        {
            get => _scenename;
            set
            {
                _scenename = value;
                Dirty();
            }
        }

        public string MeshInstance
        {
            get => _meshinstance;
            set
            {
                _meshinstance = value;
                Dirty();
            }
        }
    }
}
