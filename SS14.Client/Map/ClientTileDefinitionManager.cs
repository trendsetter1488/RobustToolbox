using SS14.Client.Interfaces.Map;
using SS14.Client.Interfaces.ResourceManagement;
using SS14.Client.ResourceManagement;
using SS14.Shared.Interfaces.Map;
using SS14.Shared.IoC;
using SS14.Shared.Log;
using SS14.Shared.Map;
using System.Collections.Generic;

namespace SS14.Client.Map
{
    /// <summary>
    ///     Special TileDefinitionManager that makes a Godot TileSet for usage by TileMaps.
    /// </summary>
    public class ClientTileDefinitionManager : TileDefinitionManager, IClientTileDefinitionManager
    {
        [Dependency]
        readonly IResourceCache resourceCache;

        public Godot.TileSet TileSet { get; private set; } = new Godot.TileSet();

        public Godot.MeshLibrary MeshLibrary { get; private set; } = new Godot.MeshLibrary();
        

        private Dictionary<ushort, TextureResource> Textures = new Dictionary<ushort, TextureResource>();

        public override ushort Register(ITileDefinition tileDef)
        {
            var ret = base.Register(tileDef);
            
            TileSet.CreateTile(ret);
            MeshLibrary.CreateItem(ret);

            if (!string.IsNullOrEmpty(tileDef.SpriteName))
            {
                var texture = resourceCache.GetResource<TextureResource>($@"/Textures/Tiles/{tileDef.SpriteName}.png");
                TileSet.TileSetTexture(ret, texture.Texture.GodotTexture);
                Textures[ret] = texture;
                
                var surfacetool = new Godot.SurfaceTool();
                surfacetool.Begin(Godot.Mesh.PrimitiveType.TriangleStrip);

                surfacetool.AddColor(new Godot.Color(0, 0, 0));
                surfacetool.AddUv(new Godot.Vector2(0, 1));
                surfacetool.AddNormal(new Godot.Vector3(0, 0, 1));
                surfacetool.AddVertex(new Godot.Vector3(0, 1,0));

                surfacetool.AddColor(new Godot.Color(0, 0, 0));
                surfacetool.AddUv(new Godot.Vector2(1, 1));
                surfacetool.AddNormal(new Godot.Vector3(0, 0, 1));
                surfacetool.AddVertex(new Godot.Vector3(1, 1, 0));

                surfacetool.AddColor(new Godot.Color(0, 0, 0));
                surfacetool.AddUv(new Godot.Vector2(0, 0));
                surfacetool.AddNormal(new Godot.Vector3(0, 0, 1));
                surfacetool.AddVertex(new Godot.Vector3(0, 0, 0));

                surfacetool.AddColor(new Godot.Color(0, 0, 0));
                surfacetool.AddUv(new Godot.Vector2(1, 0));
                surfacetool.AddNormal(new Godot.Vector3(0, 0, 1));
                surfacetool.AddVertex(new Godot.Vector3(1, 0, 0));

                var material = new Godot.SpatialMaterial();
                material.AlbedoTexture = texture.Texture;
                surfacetool.SetMaterial(material);
                var mesh = surfacetool.Commit();
                MeshLibrary.SetItemMesh(ret, mesh);
            }

            return ret;
        }
    }
}
