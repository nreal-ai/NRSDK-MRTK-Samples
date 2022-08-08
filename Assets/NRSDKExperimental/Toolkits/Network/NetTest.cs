using NRKernal;
using NRKernal.Experimental.NetWork;
using UnityEngine;

/// <summary> A net test. </summary>
public class NetTest : MonoBehaviour
{
    /// <summary> The network. </summary>
    private NetWorkClient network;
    /// <summary> Starts this object. </summary>
    private void Start()
    {
        network = new NetWorkClient();

        EnterRoomData data = new EnterRoomData();
        data.result = false;

        var serilizer = SerializerFactory.Create();
        var bnary = serilizer.Serialize(data);

        var data2 = serilizer.Deserialize<EnterRoomData>(bnary);
        NRDebugger.Info(data2.result);
    }

    /// <summary> Connects this object. </summary>
    public void Connect()
    {
        network.Connect("192.168.69.213", 6000);
    }

    /// <summary> Enter room. </summary>
    public void EnterRoom()
    {
        network.EnterRoomRequest();
    }

    /// <summary> Exit room. </summary>
    public void ExitRoom()
    {
        network.ExitRoomRequest();
    }

    /// <summary> Updates the camera parameter. </summary>
    public void UpdateCameraParam()
    {
        network.UpdateCameraParamRequest();
    }

    /// <summary> Disconnects this object. </summary>
    public void Disconnect()
    {
        network.Dispose();
    }
}
