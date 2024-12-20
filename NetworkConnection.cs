// <copyright file="NetworkConnection.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// Authors: Benjamin Keefer, Madi Murray, and Course Staff
// Version: November 21st, 2024
// </copyright>
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
namespace GUI.Client.Controllers;

/// <summary>
///   Wraps the StreamReader/Writer/TcpClient together so we
///   don't have to keep creating all three for network actions.
/// </summary>
public sealed class NetworkConnection : IDisposable
{
    /// <summary>
    ///   The connection/socket abstraction
    /// </summary>
    private TcpClient _tcpClient = new();

    /// <summary>
    ///   Reading end of the connection
    /// </summary>
    private StreamReader? _reader = null;

    /// <summary>
    ///   Writing end of the connection
    /// </summary>
    private StreamWriter? _writer = null;

    /// <summary> String used for preventing race conditions </summary>
    public static string lockKey = "key";

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.
    ///   </para>
    /// </summary>
    /// <param name="tcpClient">
    ///   An already existing TcpClient
    /// </param>
    public NetworkConnection( TcpClient tcpClient )
    {
        // Initializes tcpclient and name
        _tcpClient = tcpClient;
        Name = string.Empty;
        if ( IsConnected )
        {
            // Only establish the reader/writer if the provided TcpClient is already connected.
            _reader = new StreamReader( _tcpClient.GetStream(), new UTF8Encoding(false) );
            _writer = new StreamWriter( _tcpClient.GetStream(), new UTF8Encoding(false) ) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="NetworkConnection"/> class.
    ///   <para>
    ///     Create a network connection object.  The tcpClient will be unconnected at the start.
    ///   </para>
    /// </summary>
    public NetworkConnection( )
        : this( new TcpClient( ) )
    {
    }

    /// <summary>
    /// Gets a value indicating whether the socket is connected.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            return _tcpClient.Connected;
        }
    }


    /// <summary>
    ///   Try to connect to the given host:port. 
    /// </summary>
    /// <param name="host"> The URL or IP address, e.g., www.cs.utah.edu, or  127.0.0.1. </param>
    /// <param name="port"> The port, e.g., 11000. </param>
    public void Connect( string host, int port )
    {
        try
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(host, port);
            _reader = new StreamReader(_tcpClient.GetStream(), new UTF8Encoding(false));
            _writer = new StreamWriter(_tcpClient.GetStream(), new UTF8Encoding(false)) { AutoFlush = true }; // AutoFlush ensures data is sent immediately
        }
        catch ( Exception )
        {
            Console.WriteLine("Failed to connect client to host.");
        }
    }

    /// <summary>
    ///  Executes the given command to the database.
    /// </summary>
    /// <param name="commandText"></param>
    public string DBUpdateCommand( string commandText )
    {
        string connectString = "server=atr.eng.utah.edu;database=u1434592;uid=u1434592;password=C_sharper";
        using (MySqlConnection conn = new MySqlConnection(connectString))
        {
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                //insert the gid into the database
                command.CommandText = "SELECT last_insert_id() AS gID;";
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader["gID"].ToString()!;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error updating the connection");
                return string.Empty;
            }
        }
        return string.Empty;
    }

    /// <summary> Stores the given name of the client </summary>
    public string Name { get; set; }

    /// <summary>
    ///   Send a message to the remote server.  If the <paramref name="message"/> contains
    ///   new lines, these will be treated on the receiving side as multiple messages.
    ///   This method should attach a newline to the end of the <paramref name="message"/>
    ///   (by using WriteLine).
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <param name="message"> The string of characters to send. </param>
    public void Send( string message )
    {
        // Ensures the server is connected
        if(!IsConnected)
        {
            throw new InvalidOperationException("Server is not connected.");
        }
        // Splits message by new lines
        String[] splitMessage = message.Split("\n");
        // Writes each message to the client
        _writer!.WriteLine(message);
        
    }


    /// <summary>
    ///   Read a message from the remote side of the connection.  The message will contain
    ///   all characters up to the first new line. See <see cref="Send"/>.
    ///   If this operation can not be completed (e.g. because this NetworkConnection is not
    ///   connected), throw an InvalidOperationException.
    /// </summary>
    /// <returns> The contents of the message. </returns>
    public string ReadLine( )
    {
        // Ensures the server is connected
        if (!IsConnected)
        {
            throw new InvalidOperationException("Server is not connected.");
        }
        // Returns message from reader
        return _reader!.ReadLine() ?? throw new InvalidOperationException("EOF reached");
    }

    /// <summary>
    ///   If connected, disconnect the connection and clean 
    ///   up (dispose) any streams.
    /// </summary>
    public void Disconnect( )
    {
        _tcpClient.Close();
        _writer?.Dispose();
        _reader?.Dispose();
    }

    /// <summary>
    ///   Automatically called with a using statement (see IDisposable)
    /// </summary>
    public void Dispose( )
    {
        Disconnect();
    }
}
