using UnityEngine;
using Vuforia;

public class Playmat : MonoBehaviour {
    static readonly float kDefaultBigMoveIncrement = 0.1f;
    static readonly float kDefaultSmallMoveIncrement = 0.01f;

    #region PUBLIC_MEMBERS
    public AnchorBehaviour PlaymatAnchor;
    public CardLookupTable CardLookupTable;
    public float BigMoveIncrement = kDefaultBigMoveIncrement;
    public float SmallMoveIncrement = kDefaultSmallMoveIncrement;
    #endregion // PUBLIC_MEMBERS

    #region PUBLIC_METHODS
    #endregion // PUBLIC_METHODS

    GameObject m_currentAugment; 

    // Use this for initialization
    void Start () {
        // TODONOW For testing purposes, just select first object in table
        if (CardLookupTable != null && CardLookupTable.cards.Length > 0)
        {
            m_currentAugment = CardLookupTable.cards[0].obj;
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (m_currentAugment != null)
        {
            if (Input.GetKeyUp(KeyCode.H))
            {
                MoveAugmentLeft(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                MoveAugmentForward(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                MoveAugmentBackward(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                MoveAugmentRight(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
            if (Input.GetKeyUp(KeyCode.LeftBracket))
            {
                MoveAugmentUp(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
            if (Input.GetKeyUp(KeyCode.RightBracket))
            {
                MoveAugmentDown(ShiftKeyDown() ? SmallMoveIncrement : BigMoveIncrement);
            }
        }
    }

    private void MoveAugmentLeft(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.x -= increment;
        m_currentAugment.transform.position = position;
    }

    private void MoveAugmentRight(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.x += increment;
        m_currentAugment.transform.position = position;
    }

    private void MoveAugmentForward(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.z += increment;
        m_currentAugment.transform.position = position;
    }

    private void MoveAugmentBackward(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.z -= increment;
        m_currentAugment.transform.position = position;
    }

    private void MoveAugmentUp(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.y += increment;
        m_currentAugment.transform.position = position;
    }

    private void MoveAugmentDown(float increment)
    {
        Vector3 position = m_currentAugment.transform.position;
        position.y -= increment;
        m_currentAugment.transform.position = position;
    }

    private bool ShiftKeyDown()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}
