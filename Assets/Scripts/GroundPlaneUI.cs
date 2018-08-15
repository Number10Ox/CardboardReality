/*==============================================================================
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GroundPlaneUI : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI m_TrackerStatus;
    public TMPro.TextMeshProUGUI m_Instructions;
    public CardboardReality m_cardboardReality;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    //ProductPlacement m_ProductPlacement;
    //TouchHandler m_TouchHandler;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_TrackerStatus.text = "";

        //m_ProductPlacement = FindObjectOfType<ProductPlacement>();
        //m_TouchHandler = FindObjectOfType<TouchHandler>();

        m_EventSystem = FindObjectOfType<EventSystem>();

        Vuforia.DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (!CardboardReality.PlaymatIsPlaced)
        {
            if (CardboardReality.GroundPlaneHitReceived)
            {
                // We got an automatic hit test this frame

                // Hide the onscreen reticle when we get a hit test
                //m_ScreenReticle.alpha = 0;

                m_Instructions.transform.parent.gameObject.SetActive(true);
                m_Instructions.enabled = true;
                m_Instructions.text = "Tap to place playmat";
            }
            else
            {
                m_Instructions.text = "Point device towards ground";
            }
        }
        else
        {
            m_Instructions.text = "";
        }
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        Vuforia.DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }
    #endregion // MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS
    public void Reset()
    {
    }
    #endregion // PUBLIC_METHODS

    #region VUFORIA_CALLBACKS
    void OnDevicePoseStatusChanged(Vuforia.TrackableBehaviour.Status status, Vuforia.TrackableBehaviour.StatusInfo statusInfo)
    {
        Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");

        switch (statusInfo)
        {
            case Vuforia.TrackableBehaviour.StatusInfo.INITIALIZING:
                m_TrackerStatus.text = "Tracker Initializing";
                break;
            case Vuforia.TrackableBehaviour.StatusInfo.EXCESSIVE_MOTION:
                m_TrackerStatus.text = "Excessive Motion";
                break;
            case Vuforia.TrackableBehaviour.StatusInfo.INSUFFICIENT_FEATURES:
                m_TrackerStatus.text = "Insufficient Features";
                break;
            default:
                m_TrackerStatus.text = "";
                break;
        }

    }
    #endregion // VUFORIA_CALLBACKS
}
