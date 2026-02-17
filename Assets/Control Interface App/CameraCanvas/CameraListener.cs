using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RGBTcpReceiver : MonoBehaviour
{
    public int listenPort = 5000;
    public int frameWidth = 640;
    public int frameHeight = 480;
    public int timeoutCount = 120;
    public Texture timeoutTexture;
    public RawImage cameraImage;

    private int count = 0;
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Texture2D receivedTexture;
    private byte[] frameBuffer;
    private int frameSize;
    private int bytesReceived = 0;

    void Start()
    {
        if (cameraImage == null)
        {
            Debug.LogError("Please assign a RawImage UI element to display the video.");
            enabled = false;
            return;
        }

        frameSize = frameWidth * frameHeight * 3; // RGB = 3 bytes per pixel
        frameBuffer = new byte[frameSize];
        
        receivedTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
        
        tcpListener = new TcpListener(IPAddress.Any, listenPort);
        tcpListener.Start();
        Debug.Log($"TCP listener started on port {listenPort}");
    }

    void Update()
    {
        try
        {
            // Accept new connections if we don't have one
            if (tcpClient == null || !tcpClient.Connected)
            {
                if (tcpListener.Pending())
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    stream = tcpClient.GetStream();
                    stream.ReadTimeout = 1; // 1 ms timeout
                    bytesReceived = 0;
                    Debug.Log("Client connected!");
                }
                else
                {
                    
                    return;
                }
            }

            // Read data from stream
            if (stream != null && stream.DataAvailable)
            {
                count = 0;
                
                // Read as much data as available
                int bytesToRead = frameSize - bytesReceived;
                int read = stream.Read(frameBuffer, bytesReceived, bytesToRead);
                bytesReceived += read;

                // If we have a complete frame
                if (bytesReceived >= frameSize)
                {
                    // Load raw RGB data into texture
                    receivedTexture.LoadRawTextureData(frameBuffer);
                    receivedTexture.Apply();
                    
                    cameraImage.texture = receivedTexture;


                    // Reset for next frame
                    bytesReceived = 0;
                }
            }
            else if (tcpClient != null && !tcpClient.Connected)
            {
                // Connection lost
                Debug.Log("Client disconnected");
                stream?.Close();
                tcpClient?.Close();
                tcpClient = null;
                stream = null;
                bytesReceived = 0;
            }
            count++;
            Debug.Log(count);
            if (count >= timeoutCount)
            {
                if (cameraImage.texture != timeoutTexture)
                {
                    cameraImage.texture = timeoutTexture;
                }
                count = 0;
                tcpClient.Close();
            }
        }
        catch (System.IO.IOException)
        {
            // Timeout or connection issue - ignore
            Debug.Log("hmm");
        }
        catch (SocketException e)
        {
            Debug.LogWarning($"Socket exception: {e.Message}");
            stream?.Close();
            tcpClient?.Close();
            tcpClient = null;
            stream = null;
            bytesReceived = 0;
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
        }
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }
    }
}