using UnityEngine;
using Vuforia;

public class Playmat : MonoBehaviour {
    const float DefaultSmallMoveIncrement = 0.01f;
    const float DefaultBigMoveIncrement = 0.1f;

    public enum IncrementSize
    {
        IncrementSmall,
        IncrementBig
    }

    #region PUBLIC_MEMBERS
    public AnchorBehaviour PlaymatAnchor;
    public CardLookupTable CardLookupTable;
    public float SmallMoveIncrement = DefaultSmallMoveIncrement;
    public float BigMoveIncrement = DefaultBigMoveIncrement;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    #if UNITY_EDITOR
    private GameObject m_currentAugment;
    #endif
    #endregion // PRIVATE_MEMBERS

    #region PUBLIC_METHODS
    public void Spawn(string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            augment.SetActive(true);
        }
        else
        {
            Debug.Log("Command Spawn can't find game object for cardId '" + cardId + "'");
        }
    }

    public void Hide(string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            augment.SetActive(false);
        }
        else
        {
            Debug.Log("Command Hide can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveLeft(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentLeft(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveLeft can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveRight(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentRight(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveRight can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveForward(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentForward(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveForward can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveBackward(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentBackward(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveBackward can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveUp(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentUp(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveUp can't find game object for cardId '" + cardId + "'");
        }
    }

    public void MoveDown(IncrementSize increment, string cardId)
    {
        GameObject augment = CardLookupTable.GetGameObjectForCardId(cardId);
        if (augment != null)
        {
            MoveAugmentDown(increment == IncrementSize.IncrementSmall ? SmallMoveIncrement : BigMoveIncrement, augment);
        }
        else
        {
            Debug.Log("Command MoveDown can't find game object for cardId '" + cardId + "'");
        }
    }
    #endregion // PUBLIC_METHODS

    #region MONOBEHAVIOUR_METHODS
    void Start () {
        #if UNITY_EDITOR
        // TODONOW For testing purposes, just select first object in table
        if (CardLookupTable != null && CardLookupTable.Cards.Length > 0)
        {
            m_currentAugment = CardLookupTable.Cards[0].obj;
        }
        #endif

        int length = CardLookupTable.Cards.Length;
        for (int i = 0; i < length; ++i)
        {
            #if UNITY_EDITOR
            CardLookupTable.Cards[i].obj.SetActive(true); 
            #else
            CardLookupTable.Cards[i].obj.SetActive(false);
            #endif // UNITY_EDITOR
        }

        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }
	
	// Update is called once per frame
	void Update () {
        #if UNITY_EDITOR
        // TODONOW For testing purposes, just select first object in table
        if (m_currentAugment != null)
        {
            if (Input.GetKeyUp(KeyCode.H))
            {
                MoveAugmentLeft(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                MoveAugmentForward(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                MoveAugmentBackward(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                MoveAugmentRight(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
            if (Input.GetKeyUp(KeyCode.LeftBracket))
            {
                MoveAugmentUp(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
            if (Input.GetKeyUp(KeyCode.RightBracket))
            {
                MoveAugmentDown(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement, m_currentAugment);
            }
        }
        #endif // UNITY_EDITOR
    }
    #endregion //MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS
    private void MoveAugmentLeft(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.x -= increment;
        augment.transform.position = position;
    }

    private void MoveAugmentRight(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.x += increment;
        augment.transform.position = position;
    }

    private void MoveAugmentForward(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.z += increment;
        augment.transform.position = position;
    }

    private void MoveAugmentBackward(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.z -= increment;
        augment.transform.position = position;
    }

    private void MoveAugmentUp(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.y += increment;
        augment.transform.position = position;
    }

    private void MoveAugmentDown(float increment, GameObject augment)
    {
        Vector3 position = augment.transform.position;
        position.y -= increment;
        augment.transform.position = position;
    }

    private bool ShiftKeyDown()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");
    }
    #endregion PRIVATE_METHODS
}
