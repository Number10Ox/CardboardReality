/*==============================================================================
Copyright (c) 2017-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

#if false

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PlaneManager : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public PlaneFinderBehaviour m_PlaneFinder;

    [Header("Playmat Augmentation")]
    public GameObject m_PlaymatAugmentation;
    public static bool GroundPlaneHitReceived;
    public TMPro.TextMeshProUGUI debugText;

    public static bool AnchorExists
    {
        get { return anchorExists; }
        private set { anchorExists = value; }
    }
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    const string unsupportedDeviceTitle = "Unsupported Device";
    const string unsupportedDeviceBody =
        "This device has failed to start the Positional Device Tracker. " +
        "Please check the list of supported Ground Plane devices on our site: " +
        "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";

    StateManager m_StateManager;
    SmartTerrain m_SmartTerrain;
    PositionalDeviceTracker m_PositionalDeviceTracker;
    ContentPositioningBehaviour m_ContentPositioningBehaviour;
    Playmat m_Playmat;
    AnchorBehaviour m_PlaymatAnchor;
    int AutomaticHitTestFrameCount;
    //int m_AnchorCounter;
    static bool anchorExists; // backs public AnchorExists property
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        m_PlaneFinder.HitTestMode = HitTestMode.AUTOMATIC;
        m_Playmat = FindObjectOfType<Playmat>();
        m_PlaymatAnchor = m_PlaymatAugmentation.GetComponentInParent<AnchorBehaviour>();

        UtilityHelper.EnableRendererColliderCanvas(m_PlaymatAugmentation, false);
    }

    void Update()
    {
        if (!VuforiaRuntimeUtilities.IsPlayMode() && !AnchorExists)
        {
            AnchorExists = DoAnchorsExist();
        }

        GroundPlaneHitReceived = (AutomaticHitTestFrameCount == Time.frameCount);

        if (m_Playmat.IsPlaced)
        {
            debugText.SetText("(" + m_PlaymatAugmentation.transform.position.x + ","
                + m_PlaymatAugmentation.transform.position.y + ","
                + m_PlaymatAugmentation.transform.position.z + ")"); 
        }
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    #endregion // MONOBEHAVIOUR_METHODS

    #region GROUNDPLANE_CALLBACKS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        AutomaticHitTestFrameCount = Time.frameCount;

        if (!m_Playmat.IsPlaced)
        {
            //m_Playmat.SetPlaymatAnchor(m_PlaymatAnchor.transform);
            m_Playmat.SetPlaced();
            m_ContentPositioningBehaviour = m_PlaneFinder.GetComponent<ContentPositioningBehaviour>();
            m_ContentPositioningBehaviour.DuplicateStage = false;
            m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
        }
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        /*
        if (result == null)
        {
            Debug.LogError("Invalid hit test result!");
            return;
        }

        if (!m_GroundPlaneUI.IsCanvasButtonPressed())
        {
            Debug.Log("HandleInteractiveHitTest() called.");

            // If the PlaneFinderBehaviour's Mode is Automatic, then the Interactive HitTestResult will be centered.

            // PlaneMode.Ground and PlaneMode.Placement both use PlaneFinder's ContentPositioningBehaviour
            m_ContentPositioningBehaviour = m_PlaneFinder.GetComponent<ContentPositioningBehaviour>();
            m_ContentPositioningBehaviour.DuplicateStage = false;

            // Place object based on Ground Plane mode
            switch (planeMode)
            {
                case PlaneMode.GROUND:

                    m_ContentPositioningBehaviour.AnchorStage = m_PlaneAnchor;
                    m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
                    UtilityHelper.EnableRendererColliderCanvas(m_PlaneAugmentation, true);

                    // Astronaut should rotate toward camera with each placement
                    m_PlaneAugmentation.transform.localPosition = Vector3.zero;
                    UtilityHelper.RotateTowardCamera(m_PlaneAugmentation);

                    AstronautIsPlaced = true;

                    break;

                case PlaneMode.PLACEMENT:

                    if (!m_ProductPlacement.IsPlaced || TouchHandler.DoubleTap)
                    {
                        m_ContentPositioningBehaviour.AnchorStage = m_PlaymatAnchor;
                        m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
                        UtilityHelper.EnableRendererColliderCanvas(m_PlaymatAugmentation, true);
                    }

                    if (!m_ProductPlacement.IsPlaced)
                    {
                        m_ProductPlacement.SetProductAnchor(m_PlaymatAnchor.transform);
                        m_TouchHandler.enableRotation = true;
                    }

                    break;
            }
        }
        */
    }

    #endregion // GROUNDPLANE_CALLBACKS

    #region PUBLIC_BUTTON_METHODS

    public void ResetScene()
    {
        Debug.Log("ResetScene() called.");

        // reset augmentations
        m_Playmat.Reset();
        UtilityHelper.EnableRendererColliderCanvas(m_PlaymatAugmentation, false);

        DeleteAnchors();
        m_Playmat.SetPlaymatAnchor(null);
    }

    public void ResetTrackers()
    {
        Debug.Log("ResetTrackers() called.");

        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        m_SmartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
        m_PositionalDeviceTracker.Reset();
        m_SmartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
    }

    #endregion // PUBLIC_BUTTON_METHODS

    #region PRIVATE_METHODS

    void DeleteAnchors()
    {
        // TODONOW
        //m_PlaymatAnchor.UnConfigureAnchor();
        //AnchorExists = DoAnchorsExist();
    }

    bool DoAnchorsExist()
    {
        if (m_StateManager != null)
        {
            IEnumerable<TrackableBehaviour> trackableBehaviours = m_StateManager.GetActiveTrackableBehaviours();

            foreach (TrackableBehaviour behaviour in trackableBehaviours)
            {
                if (behaviour is AnchorBehaviour)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion // PRIVATE_METHODS

    #region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        Debug.Log("OnVuforiaStarted() called.");

        m_StateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null && m_SmartTerrain != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();
            if (m_PositionalDeviceTracker.IsActive && !m_SmartTerrain.IsActive)
                m_SmartTerrain.Start();
        }
        else
        {
            if (m_PositionalDeviceTracker == null)
                Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (m_SmartTerrain == null)
                Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");

            // TODONOW
            //MessageBox.DisplayMessageBox(unsupportedDeviceTitle, unsupportedDeviceBody, false, null);
        }
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

        // TODONOW
        //if (paused)
        //    ResetScene();
    }

    #endregion // VUFORIA_CALLBACKS


    #region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        Debug.Log("OnTrackerStarted() called.");

        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();

            Debug.Log("PositionalDeviceTracker is Active?: " + m_PositionalDeviceTracker.IsActive +
                      "\nSmartTerrain Tracker is Active?: " + m_SmartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");
    }

    #endregion // DEVICE_TRACKER_CALLBACK_METHODS
}

#endif
