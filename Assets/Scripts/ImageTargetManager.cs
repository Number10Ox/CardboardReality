/*===============================================================================
Copyright (c) 2016-2017 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System.Collections;
using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler which uses the VuMarkManager.
/// </summary>
public class ImageTargetManager : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES

    [System.Serializable]
    public class TargetInfo
    {
        public GameObject imageTarget;           
        public GameObject asset;
    }

    public TargetInfo[] targets;

    #endregion // PUBLIC_MEMBER_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        AttachGameAssets();
    }

    void Update()
    {
        UpdateActiveObjects();
    }

    void OnDestroy()
    {
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    void UpdateActiveObjects()
    {
        /*
        foreach (var bhvr in m_VuMarkManager.GetActiveBehaviours())

        {
            string vuMarkId = GetVuMarkId(bhvr.VuMarkTarget);
            GameObject associatedObject = FindObjectForVuMark(vuMarkId);
            if (associatedObject != null)
            {
                associatedObject.SetActive(true);
                associatedObject.transform.position = bhvr.transform.position;
                associatedObject.transform.rotation = bhvr.transform.rotation;
            }
            else
            {
                Debug.Log("ERROR: Can't find object for VuMark: " + vuMarkId);
            }
        }
        */
    }

    void AttachGameAssets()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            GameObject asset = targets[i].asset;
            if (asset != null)
            {
                asset.SetActive(false);
            }
        }
    }

    #endregion // PRIVATE_METHODS
}
