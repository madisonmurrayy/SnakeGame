using System.Text.Json.Serialization;

namespace GUI.Client.Models;
/// <summary>
///  Used to represent the coordinates of all model objects
///  Refers to a simple class that represents a 2D point in space(just an X,Y pair)
///  Will deal with integer coordinates, so X and Y are ints
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  November 21st, 2024
/// </version>
public class Point2D
{
    /// <summary> X coordinate </summary>
    public int X {  get; set; }
    /// <summary> Y coordinate </summary>
    public int Y { get; set; }
        
    /// <summary>
    ///  Constructs a Point2D from the given x and y arguments
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Point2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///  Zero-parameter constructor for deserialization.
    /// </summary>
    [JsonConstructor]
    public Point2D()
    {

    }
}