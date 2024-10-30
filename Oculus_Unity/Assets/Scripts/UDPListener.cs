/*
 Code from https://gist.github.com/unitycoder/7ad714e72f5fed1c50c6d7a188082388
*/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
    [SerializeField] Skeleton skeleton;

    UdpClient clientData;
    IPEndPoint ipEndPointData;

    int portData = 5005;
    public int receiveBufferSize = 1024*5;
    public bool showDebug = true;
    private object obj = null;
    private System.AsyncCallback AC;
    byte[] receivedBytes;

    void Start()
    {
        InitializeUDPListener();
    }

    public void InitializeUDPListener()
    {
        ipEndPointData = new IPEndPoint(IPAddress.Any , portData);

        clientData = new UdpClient();
        clientData.Client.ReceiveBufferSize = receiveBufferSize;
        clientData.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
        clientData.ExclusiveAddressUse = false;
        clientData.EnableBroadcast = true;
        clientData.Client.Bind(ipEndPointData);
        clientData.DontFragment = true;

        AC = new System.AsyncCallback(ReceivedUDPPacket);
        clientData.BeginReceive(AC, obj);

        if (showDebug) Debug.Log("BufSize: " + clientData.Client.ReceiveBufferSize);
        Debug.Log("UDP - Start Receiving..");
    }

    void ReceivedUDPPacket(System.IAsyncResult result)
    {
        receivedBytes = clientData.EndReceive(result, ref ipEndPointData);
        List<Vector3> landmarks = ParsePacketIntoLandmarks();

        if (landmarks != null && landmarks.Count != 0)
        {
            skeleton.UpdateLandmarkCoordinates(landmarks);
        }

        clientData.BeginReceive(AC, obj);
    }

    List<Vector3> ParsePacketIntoLandmarks()
    {
        string receivedString = Encoding.ASCII.GetString(receivedBytes);

        try
        {
            List<Vector3> landmarks = new List<Vector3>();

            LandmarksDTO landmarksDTO = JsonUtility.FromJson<LandmarksDTO>(receivedString);

            foreach (CoordsDTO coordDTO in landmarksDTO.landmarks) {
                Debug.Log("Coords: " + coordDTO);

                landmarks.Add(new Vector3(coordDTO.x, coordDTO.y, coordDTO.z));
            }

            return landmarks;
        }
        catch (System.Exception)
        {
            Debug.Log("Incorrect JSON received");
            return null;
        }
    }

    void OnDestroy()
    {
        if (clientData != null)
        {
            clientData.Close();
        }
    }
}
