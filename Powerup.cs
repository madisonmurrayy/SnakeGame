using System.Text.Json.Serialization;

namespace GUI.Client.Models;

/// <summary>
///  This class represents a powerup for the game.
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  November 21st, 2024
/// </version>
public class Powerup
{
    /// <summary> unique ID </summary>
    [JsonInclude]
    public int power { get; set; }

    /// <summary> location of powerup </summary>
    [JsonInclude]
    public Point2D loc { get; set; }

    /// <summary> 
    ///  Boolean indicating if the powerup was
    ///  collected by a player 
    /// </summary>
    [JsonInclude]
    public bool died { get; set; }

    /// <summary>
    ///  Zero parameter constructor for deserializing
    /// </summary>
    [JsonConstructor]
    public Powerup()
    {
            
    }

    /// <summary>
    ///  Constructs this powerup with the given arguments
    /// </summary>
    /// <param name="power"></param>
    /// <param name="loc"></param>
    /// <param name="died"></param>
    public Powerup(int power, Point2D loc, bool died)
    {
        this.power = power;
        this.loc = loc;
        this.died = died;
    }
}