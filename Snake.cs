using System.Text.Json.Serialization;

namespace GUI.Client.Models;
/// <summary>
///  This class represents a snake (eel) for the game.
/// </summary>
/// <author>
///  Benjamin Keefer and Madi Murray
/// </author>
/// <version>
///  November 22nd, 2024
/// </version>
public class Snake
{
    /// <summary> snake's unique ID </summary>
    [JsonInclude]
    public int snake { get; private set; }

    /// <summary> the player's name </summary>
    [JsonInclude]
    public string name { get; private set; }

    /// <summary>
    ///  Each point in this list represents one vertex of the snake's body, 
    ///  where two consecutive vertices make up one straight segment of the body. 
    ///  The first point of the list gives the location of the snake's tail, 
    ///  and the last gives the location of the snake's head. 
    /// </summary>
    [JsonInclude]
    public List<Point2D> body { get; private set; }

    /// <summary>
    ///  axis aligned vector(purley horizontal or vertical) representing the
    ///  direction the snake is moving in
    /// </summary>
    [JsonInclude]
    private Point2D dir;

    /// <summary> represents players score </summary>
    [JsonInclude]
    public int score { get; private set; }

    [JsonIgnore]
    public int maxScore {  get;  set; }

    /// <summary>
    ///  indicates if the snake has died on this frame
    ///  only true on the exact frame in which the snake died
    ///  use this to determine when to start drawing an explosion
    /// </summary>
    [JsonInclude] 
    public bool died { get; private set; }

    /// <summary>
    ///  Boolean for knowing when to draw the snake as pink
    ///  between the time that it dies and respawns
    /// </summary>
    [JsonInclude]
    public bool alive { get; private set; }

    /// <summary>
    ///  Boolean indicating if the player controlling this snake disconnected, this boolean will
    ///  only be true once, then discontinue for rest of game.
    ///  This is used to remove disconnected players from the model
    /// </summary>
    [JsonInclude]
    public bool dc {  get; private set; }

    /// <summary>
    ///  Indicates if the player joined on this frame(only true for one game)
    /// </summary>
    [JsonInclude]
    public bool join { get; private set; }

    /// <summary>
    ///  Constructs a snake from the given arguments.
    /// </summary>
    /// <param name="snake"></param>
    /// <param name="name"></param>
    /// <param name="body"></param>
    /// <param name="dir"></param>
    /// <param name="score"></param>
    /// <param name="died"></param>
    /// <param name="alive"></param>
    /// <param name="dc"></param>
    /// <param name="join"></param>
    public Snake(int snake, string name, List<Point2D> body, Point2D dir, int score, bool died, bool alive, bool dc, bool join)
    {
        this.snake = snake;
        this.name = name;
        this.body = body;
        this.dir = dir;
        this.score = score;
        this.died = died;
        this.alive = alive;
        this.dc = dc;
        this.join = join;
        this.maxScore = 0;
    }

    /// <summary>
    ///  Default zero-parameter snake constructor for deserialization to work.
    /// </summary>
    [JsonConstructor]
    public Snake()
    {

    }

}