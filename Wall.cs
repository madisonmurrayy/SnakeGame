using System.Text.Json.Serialization;

namespace GUI.Client.Models;
/// <summary>
///  This class represents a wall for the snake game.
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  November 21st, 2024
/// </version>
public class Wall
{
    /// <summary> represents wall ID </summary>
    [JsonInclude]
    private int wall;

    /// <summary> one endpoint of the wall </summary>
    [JsonInclude]
    public Point2D p1 { get; set; }

    /// <summary> other endpoint of wall </summary>
    [JsonInclude]
    public Point2D p2 { get; set; }

    /// <summary>
    ///  Constructs a new wall from the given arguments
    /// </summary>
    /// <param name="wall"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public Wall(int wall, Point2D p1, Point2D p2)
    {
        this.wall = wall;
        this.p1 = p1;
        this.p2 = p2;
    }

    /// <summary>
    ///  Zero parameter constructor for deserialization
    /// </summary>
    [JsonConstructor]
    public Wall()
    {

    }
}