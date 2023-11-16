using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Suit suit;
    public int val;
    [HideInInspector]
    public GameObject cardObj;

    void Awake(){
        cardObj = this.gameObject;
    }

}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
