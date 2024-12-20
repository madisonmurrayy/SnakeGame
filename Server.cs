// <copyright file="Server.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// Authors: Benjamin Keefer, Madi Murray, and Course Staff
// Version: November 5th, 2024
// </copyright>

using System.Net;
using System.Net.Sockets;



namespace GUI.Client.Controllers;

/// <summary>
///   Represents a server task that waits for connections on a given
///   port and calls the provided delegate when a connection is made.
/// </summary>
public static class Server
{

    /// <summary>
    ///   Wait on a TcpListener for new connections. Alert the main program
    ///   via a callback (delegate) mechanism.
    /// </summary>
    /// <param name="handleConnect">
    ///   Handler for what the user wants to do when a connection is made.
    ///   This should be run asynchronously via a new thread.
    /// </param>
    /// <param name="port"> The port (e.g., 11000) to listen on. </param>
    public static void StartServer( Action<NetworkConnection> handleConnect, int port )
    {
        TcpListener listener = new(IPAddress.Any, port);

        listener.Start();

        while (true)
        {
            // Accepts connection to client
            TcpClient client = listener.AcceptTcpClient();
            
            // Handles client concurrently
            new Thread(() => handleConnect(new NetworkConnection(client))).Start();
        }
    }
}
