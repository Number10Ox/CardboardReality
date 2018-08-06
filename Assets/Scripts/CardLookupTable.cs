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


    public CardInfo[] cards;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
