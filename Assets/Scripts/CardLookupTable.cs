using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLookupTable : MonoBehaviour {

    [System.Serializable]
    public class CardInfo
    {
        public string cardId;
        public GameObject obj;
    }

    public CardInfo[] Cards;

    public bool HasCard(string cardId)
    {
        int length = Cards.Length;
        for (int i = 0; i < length; ++i)
        {
            if (Cards[i].cardId == cardId)
            {
                return true;
            }
        }

        return false;
    }
    
    public GameObject GetGameObjectForCardId(string cardId)
    {
        int length = Cards.Length;
        for (int i = 0; i < length; ++i)
        {
            if (Cards[i].cardId == cardId)
            {
                return Cards[i].obj;
            }
        }

        return null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
