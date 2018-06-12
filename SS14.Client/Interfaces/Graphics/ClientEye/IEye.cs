using SS14.Shared.Map;
using SS14.Shared.Maths;

namespace SS14.Client.Interfaces.Graphics.ClientEye
{
    /// <summary>
    ///     An Eye is a point through which the player can view the world.
    ///     It's a 2D camera in other game dev lingo basically.
    /// </summary>
    public interface IEye
    {
        Godot.Node Camera { get; }

        MapId MapId { get; set; }

        /// <summary>
        ///     Whether this is the current eye. If true, this one will be used.
        /// </summary>
        bool Current { get; set; }

        Vector2 WorldToScreen(Vector2 point);

        Vector2 ScreenToWorld(Vector2 point, Vector4 scalarplaneequation = new Vector4());
    }
}
