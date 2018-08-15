using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CardboardReality : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public PlaneFinderBehaviour m_PlaneFinder;
    public UDPServer CardboardRealityServer;
    public Playmat Playmat;
    public TMPro.TextMeshProUGUI DebugText;

    public static bool GroundPlaneHitReceived;
    public static bool PlaymatIsPlaced;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    ContentPositioningBehaviour m_ContentPositioningBehaviour;
    int AutomaticHitTestFrameCount;
    #endregion // PRIVATE_MEMBERS

    #region PUBLIC_METHODS

    #region GROUNDPLANE_CALLBACKS
    public void HandleAutomaticHitTest(HitTestResult result)
    {
        AutomaticHitTestFrameCount = Time.frameCount;
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        if (PlaymatIsPlaced)
        {
            return;
        }
        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }

        m_ContentPositioningBehaviour = m_PlaneFinder.GetComponent<ContentPositioningBehaviour>();
        m_ContentPositioningBehaviour.DuplicateStage = false;
        m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
        m_PlaneFinder.enabled = false;
        PlaymatIsPlaced = true;
    }
    #endregion //GROUNDPLANE_CALLBACKS

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

    #endregion // PUBLIC_METHODS

    #region MONOBEHAVIOUR_METHODS
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

    void Update()
    {
        GroundPlaneHitReceived = (AutomaticHitTestFrameCount == Time.frameCount);
    }
    #endregion // MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS
    private void OnCommand(string rawCommand)
    {
        const string kMoveTypeSmallIncrement = "Small";
        //const string kMoveTypeLargeIncrement = "Large";

        string command = System.Text.RegularExpressions.Regex.Replace(rawCommand, @"\t|\n|\r", "");
        DebugText.SetText("Received command: '" + command + "'");
        string[] tokens = command.Split(' ');

        if (tokens.Length < 2)
        {
            Debug.Log("Received invalid command '" + command + "'");
            return;
        }

        if (tokens[0] == Command.Attack)
        {
            string goName = tokens[1];
            GameObject go = GameObject.Find(goName);
            if (go)
            {
                Debug.Log("Sending '" + Command.Attack + "' command to game object '" + go.name + "'");
                go.SendMessage(Command.Attack);
            }
        }
        else if (tokens[0] == Command.Spawn)
        {
            string goName = tokens[1];
            Playmat.Spawn(goName);
        }
        else if (tokens[0] == Command.Hide)
        {
            string goName = tokens[1];
            Playmat.Hide(goName);
        }
        else if (tokens[0] == Command.MoveLeft)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveLeft command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveLeft(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
        else if (tokens[0] == Command.MoveRight)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveRight command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveRight(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
        else if (tokens[0] == Command.MoveForward)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveForward command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveForward(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
        else if (tokens[0] == Command.MoveBackward)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveBackward command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveBackward(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
        else if (tokens[0] == Command.MoveUp)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveUp command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveUp(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
        else if (tokens[0] == Command.MoveDown)
        {
            if (tokens.Length < 3)
            {
                Debug.Log("Received invalid MoveDown command '" + command + "'");
                return;
            }
            string moveSize = tokens[1];
            string goName = tokens[2];
            Playmat.MoveDown(moveSize == kMoveTypeSmallIncrement ? Playmat.IncrementSize.IncrementSmall : Playmat.IncrementSize.IncrementBig, goName);
        }
    }
    #endregion // PRIVATE_METHODS
}
