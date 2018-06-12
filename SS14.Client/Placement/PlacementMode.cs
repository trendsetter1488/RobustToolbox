using System;
using System.Collections.Generic;
using SS14.Client.Graphics;
using SS14.Client.Graphics.ClientEye;
using SS14.Client.Interfaces;
using SS14.Client.ResourceManagement;
using SS14.Client.Utility;
using SS14.Shared.Interfaces.GameObjects.Components;
using SS14.Shared.Interfaces.Map;
using SS14.Shared.IoC;
using SS14.Shared.Log;
using SS14.Shared.Map;
using SS14.Shared.Maths;
using SS14.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace SS14.Client.Placement
{
    abstract public class PlacementMode
    {
        public readonly PlacementManager pManager;

        /// <summary>
        /// Holds the current tile we are hovering our mouse over
        /// </summary>
        public TileRef CurrentTile { get; set; }

        /// <summary>
        /// Local coordinates of our cursor on the map
        /// </summary>
        public LocalCoordinates MouseCoords { get; set; }

        /// <summary>
        /// Texture resource to draw to represent the entity we are tryign to spawn
        /// </summary>
        public Texture SpriteToDraw { get; set; }

        /// <summary>
        /// A multi mesh which we can use to represent the model we are going to draw
        /// </summary>
        public Godot.MultiMeshInstance MeshToDraw => pManager.MeshToDraw;


        private Godot.Vector3 rotation = Godot.Vector3.Zero;

        /// <summary>
        /// Color set to the ghost entity when it has a valid spawn position
        /// </summary>
        public Color ValidPlaceColor { get; set; } = new Color(20, 180, 20); //Default valid color is green

        /// <summary>
        /// Color set to the ghost entity when it has an invalid spawn position
        /// </summary>
        public Color InvalidPlaceColor { get; set; } = new Color(180, 20, 20); //Default invalid placement is red

        /// <summary>
        /// Used for line and grid placement to determine how spaced apart the entities should be
        /// </summary>
        protected float GridDistancing = 1f;

        /// <summary>
        /// Whether this mode requires us to verify the player is spawning within a certain range of themselves
        /// </summary>
        public virtual bool RangeRequired => false;

        /// <summary>
        /// Whether this mode can use the line placement mode
        /// </summary>
        public virtual bool HasLineMode => false;

        /// <summary>
        /// Whether this mode can use the grid placement mode
        /// </summary>
        public virtual bool HasGridMode => false;

        protected PlacementMode(PlacementManager pMan)
        {
            pManager = pMan;
        }

        public virtual string ModeName => GetType().Name;

        /// <summary>
        /// Aligns the location of placement based on cursor location
        /// </summary>
        /// <param name="mouseScreen"></param>
        /// <returns>Returns whether the current position is a valid placement position</returns>
        public abstract void AlignPlacementMode(ScreenCoordinates mouseScreen);

        /// <summary>
        /// Verifies the location of placement is a valid position to place at
        /// </summary>
        /// <param name="mouseScreen"></param>
        /// <returns></returns>
        public abstract bool IsValidPosition(LocalCoordinates position);

        public virtual void Render()
        {
            if (SpriteToDraw == null)
            {
                UpdateDrawInfo();
            }

            var size = SpriteToDraw.Size;

            IEnumerable<LocalCoordinates> locationcollection;
            switch (pManager.PlacementType)
            {
                case PlacementManager.PlacementTypes.None:
                    locationcollection = SingleCoordinate();
                    break;
                case PlacementManager.PlacementTypes.Line:
                    locationcollection = LineCoordinates();
                    break;
                case PlacementManager.PlacementTypes.Grid:
                    locationcollection = GridCoordinates();
                    break;
                default:
                    locationcollection = SingleCoordinate();
                    break;
            }

            if(IoCManager.arewethreeD)
            {
                List<(Vector2, bool)> CoordinateAndSuccess = new List<(Vector2, bool)>();
                var instancecount = 0;
                foreach (var coordinate in locationcollection)
                {
                    instancecount++;
                    CoordinateAndSuccess.Add((coordinate.Position, IsValidPosition(coordinate) ? true : false));
                }

                MeshToDraw.Multimesh.SetInstanceCount(instancecount);
                instancecount = 0;
                foreach ((var position, bool success) in CoordinateAndSuccess)
                {
                    var transform = new Godot.Transform(Godot.Basis.Identity, new Godot.Vector3(position.X, position.Y, 0));

                    var eulerrotations = rotation.Convert();
                    switch (pManager.Direction)
                    {
                        case Direction.North:
                            break;
                        case Direction.East:
                            eulerrotations += new Vector3(0, 0, 90);
                            break;
                        case Direction.South:
                            eulerrotations += new Vector3(0, 0, 180);
                            break;
                        case Direction.West:
                            eulerrotations += new Vector3(0, 0, 270);
                            break;
                    }
                    var quat = Quaternion.EulerToQuat(eulerrotations);
                    transform.basis = new Godot.Basis(new Godot.Quat(quat.x, quat.y, quat.z, quat.W));

                    var placementcolor = success ? ValidPlaceColor.Convert() : InvalidPlaceColor.Convert();

                    MeshToDraw.Multimesh.SetInstanceTransform(instancecount, transform);
                    MeshToDraw.Multimesh.SetInstanceColor(instancecount, placementcolor);
                    instancecount++;
                }
            }
            else
            {
                foreach (var coordinate in locationcollection)
                {
                    var pos = coordinate.Position * EyeManager.PIXELSPERMETER - size / 2f;
                    var color = IsValidPosition(coordinate) ? ValidPlaceColor : InvalidPlaceColor;
                    pManager.drawNode.DrawTexture(SpriteToDraw.GodotTexture, pos.Convert(), color.Convert());
                }
            }
        }

        public IEnumerable<LocalCoordinates> SingleCoordinate()
        {
            yield return MouseCoords;
        }

        public IEnumerable<LocalCoordinates> LineCoordinates()
        {
            var placementdiff = MouseCoords.ToWorld().Position - pManager.StartPoint.ToWorld().Position;
            var iterations = 0f;
            Vector2 distance;
            if (Math.Abs(placementdiff.X) > Math.Abs(placementdiff.Y))
            {
                iterations = Math.Abs(placementdiff.X / GridDistancing);
                distance = new Vector2(placementdiff.X > 0 ? 1 : -1, 0) * GridDistancing;
            }
            else
            {
                iterations = Math.Abs(placementdiff.Y / GridDistancing);
                distance = new Vector2(0, placementdiff.Y > 0 ? 1 : -1) * GridDistancing;
            }

            for (var i = 0; i <= iterations; i++)
            {
                yield return new LocalCoordinates(pManager.StartPoint.Position + distance * i, pManager.StartPoint.Grid);
            }
        }

        public IEnumerable<LocalCoordinates> GridCoordinates()
        {
            var placementdiff = MouseCoords.ToWorld().Position - pManager.StartPoint.ToWorld().Position;
            var distanceX = new Vector2(placementdiff.X > 0 ? 1 : -1, 0) * GridDistancing;
            var distanceY = new Vector2(0, placementdiff.Y > 0 ? 1 : -1) * GridDistancing;

            var iterationsX = Math.Abs(placementdiff.X / GridDistancing);
            var iterationsY = Math.Abs(placementdiff.Y / GridDistancing);

            for (var x = 0; x <= iterationsX; x++)
            {
                for (var y = 0; y <= iterationsY; y++)
                {
                    yield return new LocalCoordinates(pManager.StartPoint.Position + distanceX * x + distanceY * y, pManager.StartPoint.Grid);
                }
            }
        }

        public TextureResource GetSprite(string key)
        {
            return pManager.ResourceCache.GetResource<TextureResource>("/Textures/" + key);
        }

        public bool TryGetSprite(string key, out TextureResource sprite)
        {
            return pManager.ResourceCache.TryGetResource("/Textures/" + key, out sprite);
        }

        public void UpdateDrawInfo()
        {
            SetSprite();
            AdjustMesh();
        }

        public void SetSprite()
        {
            SpriteToDraw = pManager.CurrentBaseSprite.TextureFor(pManager.Direction);
        }

        public void AdjustMesh()
        {
            var prototype = pManager.CurrentPrototype;
            if (prototype.Components.TryGetValue("Mesh", out var mapping))
            {
                if (mapping.TryGetNode("scene", out YamlNode node))
                {
                    var resource = (Godot.PackedScene)Godot.GD.Load("res://models/content/" + node.AsString() + ".dae");

                    if (mapping.TryGetNode("state", out node))
                    {
                        var scene = resource.Instance();
                        var newmeshinstance = (Godot.MeshInstance)scene.GetNode(node.AsString());

                        if (newmeshinstance != null)
                        {
                            if (mapping.TryGetNode("Rotation", out node))
                            {
                                rotation = node.AsVector3().Convert();
                            }
                            if (mapping.TryGetNode("Translation", out node))
                            {
                                MeshToDraw.Translation = node.AsVector3().Convert();
                            }
                            if (mapping.TryGetNode("Scale", out node))
                            {
                                MeshToDraw.Scale = node.AsVector3().Convert();
                            }

                            //Create the multimesh using the prepared to color mesh we generated, we set its colors and transforms in render
                            MeshToDraw.Multimesh = new Godot.MultiMesh()
                            {
                                TransformFormat = Godot.MultiMesh.TransformFormatEnum.Transform3d,
                                InstanceCount = 0,
                                Mesh = (Godot.Mesh)newmeshinstance.Mesh.Duplicate(),
                                ColorFormat = Godot.MultiMesh.ColorFormatEnum.Float
                            };
                            return;
                        }
                    }
                }
            }

            MeshFailure();
        }

        /// <summary>
        /// Give our mesh a default cube mesh to render with
        /// </summary>
        public void MeshFailure()
        {
            MeshToDraw.Multimesh = new Godot.MultiMesh()
            {
                TransformFormat = Godot.MultiMesh.TransformFormatEnum.Transform3d,
                InstanceCount = 0,
                ColorFormat = Godot.MultiMesh.ColorFormatEnum.Float,
                Mesh = new Godot.CubeMesh()
                {
                    Size = new Godot.Vector3(0.5f, 0.5f, 0.5f),
                }
            };
        }

        /// <summary>
        /// Checks if the player is spawning within a certain range of his character if range is required on this mode
        /// </summary>
        /// <returns></returns>
        public bool RangeCheck(LocalCoordinates coordinates)
        {
            if (!RangeRequired)
                return true;
            var range = pManager.CurrentPermission.Range;
            if (range > 0 && !pManager.PlayerManager.LocalPlayer.ControlledEntity.GetComponent<ITransformComponent>().LocalPosition.InRange(coordinates, range))
                return false;
            return true;
        }

        public bool IsColliding(LocalCoordinates coordinates)
        {
            var bounds = pManager.ColliderAABB;
            var worldcoords = coordinates.ToWorld();

            var collisionbox = Box2.FromDimensions(
                bounds.Left + worldcoords.Position.X,
                bounds.Top + worldcoords.Position.Y,
                bounds.Width,
                bounds.Height);

            if (pManager.CollisionManager.IsColliding(collisionbox, coordinates.MapID))
                return true;

            return false;
        }

        protected Vector2 ScreenToWorld(Vector2 point)
        {
            return pManager.eyeManager.ScreenToWorld(point);
        }

        protected Vector2 WorldToScreen(Vector2 point)
        {
            return pManager.eyeManager.WorldToScreen(point);
        }

        protected LocalCoordinates ScreenToPlayerGrid(ScreenCoordinates coords)
        {
            var worldPos = pManager.eyeManager.ScreenToWorld(coords.Position);
            var mapMgr = IoCManager.Resolve<IMapManager>();
            var entityGrid = pManager.PlayerManager.LocalPlayer.ControlledEntity.GetComponent<ITransformComponent>().GridID;
            return new LocalCoordinates(worldPos, mapMgr.GetMap(coords.MapID).GetGrid(entityGrid));
        }
    }
}
