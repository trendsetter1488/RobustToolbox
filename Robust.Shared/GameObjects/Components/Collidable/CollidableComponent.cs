using System;
using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Physics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using tainicom.Aether.Physics2D.Dynamics;
using BodyType = Robust.Shared.Physics.BodyType;

namespace Robust.Shared.GameObjects
{
    public class CollidableComponent : Component, IPhysBodyInternal
    {
        private bool _collisionEnabled;
        private bool _isHardCollidable;
        private bool _isScrapingFloor;
        private BodyType _bodyType;
        private Body _body;

#pragma warning disable 649
        [Dependency] private readonly IPhysicsManagerInternal _physicsManager;
#pragma warning restore 649

        public override string Name => "Collidable";
        public override uint? NetID => NetIDs.COLLIDABLE;
        public override Type StateType => typeof(CollidableComponentState);

        public CollidableComponent()
        {
            PhysicsShapes = new PhysicsShapeCollection(this);
        }

        public Box2 WorldAABB { get; }

        public Box2 AABB { get; }

        public ICollection<IPhysShape> PhysicsShapes { get; }

        public bool CollisionEnabled
        {
            get => _collisionEnabled;
            set => _collisionEnabled = value;
        }

        public bool IsHardCollidable
        {
            get => _isHardCollidable;
            set => _isHardCollidable = value;
        }

        public bool IsScrapingFloor
        {
            get => _isScrapingFloor;
            set => _isScrapingFloor = value;
        }

        public int CollisionLayer => throw new System.NotImplementedException();

        public int CollisionMask => throw new System.NotImplementedException();

        public MapId MapID => Owner.Transform.MapID;

        Body IPhysBodyInternal.AetherBody => _body;

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref _collisionEnabled, "on", true);
            serializer.DataField(ref _isHardCollidable, "hard", true);
            serializer.DataField(ref _isScrapingFloor, "IsScrapingFloor", false);
            serializer.DataField(ref _bodyType, "bodyType", BodyType.None);
            //serializer.DataField(ref _physShapes, "shapes", new List<IPhysShape>{new PhysShapeAabb()});
        }

        public override void OnAdd()
        {
            base.OnAdd();

            if (_body == null)
            {
                _body = new Body {Tag = Owner.Uid};
            }
            _physicsManager.AddBody(this);
        }

        public override void OnRemove()
        {
            base.OnRemove();

            _physicsManager.RemoveBody(this);
        }

        public void Bumped(IEntity bumpedby)
        {
            throw new System.NotImplementedException();
        }

        public void Bump(List<IEntity> bumpedinto)
        {
            throw new System.NotImplementedException();
        }

        private sealed class PhysicsShapeCollection : ICollection<IPhysShape>
        {
            private readonly CollidableComponent _owner;

            public PhysicsShapeCollection(CollidableComponent owner)
            {
                _owner = owner;
            }

            public IEnumerator<IPhysShape> GetEnumerator()
            {
                throw new System.NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(IPhysShape item)
            {
                throw new System.NotImplementedException();
            }

            public void Clear()
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(IPhysShape item)
            {
                throw new System.NotImplementedException();
            }

            public void CopyTo(IPhysShape[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(IPhysShape item)
            {
                throw new System.NotImplementedException();
            }

            public int Count { get; }
            public bool IsReadOnly { get; }
        }

        public bool TryCollision(Vector2 offset, bool bump = false)
        {
            throw new NotImplementedException();
        }
    }
}
