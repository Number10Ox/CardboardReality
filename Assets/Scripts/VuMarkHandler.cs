/*===============================================================================
Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler which uses the VuMarkManager.
/// </summary>
public class VuMarkHandler : MonoBehaviour
{
    [System.Serializable]
    public class TargetInfo
    {
        public string vumarkId;           
        public GameObject obj;
        public float xOffset;
        public float yOffset;
        public float zOffset;
    }

    public TargetInfo[] targets;

    const float FILTER_DEFAULT_ALPHA = 0.25f; // if ALPHA = 1 OR 0, no filter applies.
    public float filterAlpha = FILTER_DEFAULT_ALPHA;
    public bool useLowPassFilter = true;
    public bool adjustRotation = true;

    // PanelShowHide m_IdPanel;
    VuMarkManager m_VuMarkManager;
    VuMarkTarget m_ClosestVuMark;
    VuMarkTarget m_CurrentVuMark;

    IDictionary<string, Vector3> vuMarkPositions = new Dictionary<string, Vector3>();

    void Start()
    {
        // register callbacks to VuMark Manager
        m_VuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
        m_VuMarkManager.RegisterVuMarkDetectedCallback(OnVuMarkDetected);
        m_VuMarkManager.RegisterVuMarkLostCallback(OnVuMarkLost);

        HideAllGameObjects();
    }

    void Update()
    {
        UpdateActiveObjects();
    }

    void OnDestroy()
    {
        // unregister callbacks from VuMark Manager
        m_VuMarkManager.UnregisterVuMarkDetectedCallback(OnVuMarkDetected);
        m_VuMarkManager.UnregisterVuMarkLostCallback(OnVuMarkLost);
    }

    /// <summary>
    /// This method will be called whenever a new VuMark is detected
    /// </summary>
    public void OnVuMarkDetected(VuMarkTarget target)
    {
        Debug.Log("New VuMark: " + GetVuMarkId(target));
    }

    /// <summary>
    /// This method will be called whenever a tracked VuMark is lost
    /// </summary>
    public void OnVuMarkLost(VuMarkTarget target)
    {
        Debug.Log("Lost VuMark: " + GetVuMarkId(target));

        string vuMarkId = GetVuMarkId(target);
        GameObject associatedObject = FindObjectForVuMark(vuMarkId);
        if (associatedObject != null)
        {
            vuMarkPositions.Remove(vuMarkId);
            associatedObject.SetActive(false);
        }
    }

    void UpdateActiveObjects()
    {
        foreach (var bhvr in m_VuMarkManager.GetActiveBehaviours())
        {
            string vuMarkId = GetVuMarkId(bhvr.VuMarkTarget);
            float xOffset;
            float yOffset;
            float zOffset;
            GameObject associatedObject = FindObjectAndOffsetsForVuMark(vuMarkId, out xOffset, out yOffset, out zOffset);
            if (associatedObject != null)
            {
                associatedObject.SetActive(true);
                Quaternion vuMarkRotation = bhvr.transform.rotation;
                Quaternion zInverted = Quaternion.AngleAxis(-180.0f, Vector3.up);

                Vector3 vuMarkPosition;
                if (!vuMarkPositions.ContainsKey(vuMarkId))
                {
                    vuMarkPosition = bhvr.transform.position;
                }
                else
                {
                    vuMarkPosition.x = vuMarkPositions[vuMarkId].x + filterAlpha * (bhvr.transform.position.x - vuMarkPositions[vuMarkId].x);
                    vuMarkPosition.y = vuMarkPositions[vuMarkId].y + filterAlpha * (bhvr.transform.position.y - vuMarkPositions[vuMarkId].y);
                    vuMarkPosition.z = vuMarkPositions[vuMarkId].z + filterAlpha * (bhvr.transform.position.z - vuMarkPositions[vuMarkId].z);
                }

                vuMarkPositions[vuMarkId] = vuMarkPosition;
                vuMarkPosition.x += xOffset;
                vuMarkPosition.y += yOffset;
                vuMarkPosition.z += zOffset;

                associatedObject.transform.position = vuMarkPosition;
                if (adjustRotation)
                {
                    associatedObject.transform.rotation = bhvr.transform.rotation;// * zInverted; Don't seem to want this
                }
            }
            else
            {
                Debug.Log("ERROR: Can't find object for VuMark: " + vuMarkId);
            }
        }
    }

    void UpdateClosestTarget()
    {
        Camera cam = DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        float closestDistance = Mathf.Infinity;

        foreach (var bhvr in m_VuMarkManager.GetActiveBehaviours())
        {
            Vector3 worldPosition = bhvr.transform.position;
            Vector3 camPosition = cam.transform.InverseTransformPoint(worldPosition);

            float distance = Vector3.Distance(Vector2.zero, camPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                m_ClosestVuMark = bhvr.VuMarkTarget;
            }
        }

        if (m_ClosestVuMark != null &&
            m_CurrentVuMark != m_ClosestVuMark)
        {
            var vuMarkId = GetVuMarkId(m_ClosestVuMark);
            var vuMarkDataType = GetVuMarkDataType(m_ClosestVuMark);
            var vuMarkImage = GetVuMarkImage(m_ClosestVuMark);
            var vuMarkDesc = GetNumericVuMarkDescription(m_ClosestVuMark);

            m_CurrentVuMark = m_ClosestVuMark;

            StartCoroutine(ShowPanelAfter(0.5f, vuMarkId, vuMarkDataType, vuMarkDesc, vuMarkImage));
        }
    }

    IEnumerator ShowPanelAfter(float seconds, string vuMarkId, string vuMarkDataType, string vuMarkDesc, Sprite vuMarkImage)
    {
        yield return new WaitForSeconds(seconds);

        //m_IdPanel.Show(vuMarkId, vuMarkDataType, vuMarkDesc, vuMarkImage);
    }

    string GetVuMarkDataType(VuMarkTarget vumark)
    {
        switch (vumark.InstanceId.DataType)
        {
            case InstanceIdType.BYTES:
                return "Bytes";
            case InstanceIdType.STRING:
                return "String";
            case InstanceIdType.NUMERIC:
                return "Numeric";
        }
        return string.Empty;
    }

    string GetVuMarkId(VuMarkTarget vumark)
    {
        switch (vumark.InstanceId.DataType)
        {
            case InstanceIdType.BYTES:
                return vumark.InstanceId.HexStringValue;
            case InstanceIdType.STRING:
                return vumark.InstanceId.StringValue;
            case InstanceIdType.NUMERIC:
                return vumark.InstanceId.NumericValue.ToString();
        }
        return string.Empty;
    }

    Sprite GetVuMarkImage(VuMarkTarget vumark)
    {
        var instanceImg = vumark.InstanceImage;
        if (instanceImg == null)
        {
            Debug.Log("VuMark Instance Image is null.");
            return null;
        }

        // First we create a texture
        Texture2D texture = new Texture2D(instanceImg.Width, instanceImg.Height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };
        instanceImg.CopyToTexture(texture);

        // Then we turn the texture into a Sprite
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }

    string GetNumericVuMarkDescription(VuMarkTarget vumark)
    {
        int vuMarkIdNumeric;

        if (int.TryParse(GetVuMarkId(vumark), out vuMarkIdNumeric))
        {
            // Change the description based on the VuMark id
            switch (vuMarkIdNumeric % 4)
            {
                case 1:
                    return "Astronaut";
                case 2:
                    return "Drone";
                case 3:
                    return "Fissure";
                case 0:
                    return "Oxygen Tank";
                default:
                    return "Astronaut";
            }
        }

        return string.Empty; // if VuMark DataType is byte or string
    }

    GameObject FindObjectForVuMark(string vuMarkId)
    {
        // TODO Could create associative array
        for (int i = 0; i < targets.Length; i++)
        {
            string targetVuMarkId = targets[i].vumarkId;
            if (targetVuMarkId == vuMarkId)
            {
                return targets[i].obj;
            }
        }

        return null;
    }

    GameObject FindObjectAndOffsetsForVuMark(string vuMarkId, out float xOffset, out float yOffset, out float zOffset)
    {
        // TODO Could create associative array
        for (int i = 0; i < targets.Length; i++)
        {
            string targetVuMarkId = targets[i].vumarkId;
            if (targetVuMarkId == vuMarkId)
            {
                xOffset = targets[i].xOffset;
                yOffset = targets[i].yOffset;
                zOffset = targets[i].zOffset;
                return targets[i].obj;
            }
        }

        xOffset = 0.0f;
        yOffset = 0.0f;
        zOffset = 0.0f;
        return null;
    }

    void HideAllGameObjects()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            GameObject obj = targets[i].obj;
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
