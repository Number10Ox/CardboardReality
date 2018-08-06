using System.Collections.Generic;
using UnityEngine;
//using System.Runtime.InteropServices;
using Vuforia;

public class CardboardReality : MonoBehaviour
{
    public GameObject CardboardRealityServer;

    //[DllImport("Plugins")]
    //private static extern int GetInteger();

    void Start()
    {
        if (CardboardRealityServer != null)
        {
            UDPServer serverComponent = CardboardRealityServer.GetComponent<UDPServer>();
            if (serverComponent != null)
            {
                serverComponent.OnCommandDelegate += OnCommand;
            }
        }
    }

    public void OnGUI()
    {
        /*
        string[] args = System.Environment.GetCommandLineArgs();
        string input = "";
        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log("ARG " + i + ": " + args[i]);
            if (args[i] == "-folderInput")
            {
                input = args[i + 1];
            }
        }
        */

        //Debug.Log("Camera Fields:");
        //IEnumerable<CameraDevice.CameraField> fields = CameraDevice.Instance.GetCameraFields();
        //foreach (CameraDevice.CameraField f in fields)
        //{
        //    Debug.Log("key: " + f.Key + ", type:" + f.Type);
        //}

        //ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        //tracker.PersistExtendedTracking(false);

        //CameraDevice.VideoModeData vmd;
        //vmd = CameraDevice.Instance.GetVideoMode(CameraDevice.CameraDeviceMode.MODE_OPTIMIZE_QUALITY);
        //Debug.Log("Dimensions: " + vmd.width + "x" + vmd.height);
    }
    
    private void OnCommand(string command)
    {
        UnityEngine.GameObject go = UnityEngine.GameObject.Find(command);
        if (go)
        {
            Debug.Log("Sending 'Attack' command to game object " + go.name);
            go.SendMessage("Attack");
        }
    }
}
