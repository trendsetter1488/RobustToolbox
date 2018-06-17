using SS14.Shared.Map;
using System.Collections.Generic;
using SS14.Client.Interfaces.Map;
using SS14.Shared.IoC;
using SS14.Client.Interfaces;
using SS14.Shared.Log;
using SS14.Client.Graphics.ClientEye;

namespace SS14.Client.Map
{
    public class ClientMapManager3D : MapManager
    {
        [Dependency]
        private IClientTileDefinitionManager tileDefinitionManager;
        [Dependency]
        private ISceneTreeHolder sceneTree;

        private Dictionary<MapId, Dictionary<GridId, Godot.GridMap>> RenderGridMaps = new Dictionary<MapId, Dictionary<GridId, Godot.GridMap>>();

        public ClientMapManager3D()
        {
            TileChanged += UpdateTileMapOnUpdate;
            MapCreated += UpdateOnMapCreated;
            MapDestroyed += UpdateOnMapDestroyed;
            OnGridCreated += UpdateOnGridCreated;
            OnGridRemoved += UpdateOnGridRemoved;
            GridChanged += UpdateOnGridModified;
        }

        private void UpdateOnGridModified(object sender, GridChangedEventArgs args)
        {
            var gridmap = RenderGridMaps[args.Grid.MapID][args.Grid.Index];
            foreach ((int x, int y, Tile tile) in args.Modified)
            {
                gridmap.SetCellItem(x, y, 0, tile.TileId);
            }
        }

        private void UpdateTileMapOnUpdate(object sender, TileChangedEventArgs args)
        {
            var gridmap = RenderGridMaps[args.NewTile.MapIndex][args.NewTile.GridIndex];
            gridmap.SetCellItem(args.NewTile.X, args.NewTile.Y, 0, args.NewTile.Tile.TileId);
        }

        private void UpdateOnGridCreated(MapId mapId, GridId gridId)
        {
            var gridmap = new Godot.GridMap
            {
                Theme = tileDefinitionManager.MeshLibrary,
                // TODO: Unhardcode this cell size.
                CellSize = new Godot.Vector3(1, 1, 1),
                CellOctantSize = 1,
                CellCenterZ = false,
                CellCenterX = false,
                CellCenterY = false,
            };
            gridmap.SetName($"Grid {mapId}.{gridId}");
            sceneTree.WorldRoot.AddChild(gridmap);
            // Creating a map makes a grid before mapcreated is fired, so...
            if (!RenderGridMaps.TryGetValue(mapId, out var map))
            {
                map = new Dictionary<GridId, Godot.GridMap>();
                RenderGridMaps[mapId] = map;
            }
            map[gridId] = gridmap;
        }

        private void UpdateOnGridRemoved(MapId mapId, GridId gridId)
        {
            Logger.Debug($"Removing grid {mapId}.{gridId}");
            var gridmap = RenderGridMaps[mapId][gridId];
            gridmap.QueueFree();
            gridmap.Dispose();
            RenderGridMaps[mapId].Remove(gridId);
        }

        private void UpdateOnMapCreated(object sender, MapEventArgs eventArgs)
        {
            Logger.Debug($"Adding map {eventArgs.Map.Index}");
            if (!RenderGridMaps.ContainsKey(eventArgs.Map.Index))
            {
                RenderGridMaps[eventArgs.Map.Index] = new Dictionary<GridId, Godot.GridMap>();
            }
        }

        private void UpdateOnMapDestroyed(object sender, MapEventArgs eventArgs)
        {
            foreach (var grid in RenderGridMaps[eventArgs.Map.Index].Values)
            {
                grid.QueueFree();
                grid.Dispose();
            }

            RenderGridMaps.Remove(eventArgs.Map.Index);
        }
    }
}
