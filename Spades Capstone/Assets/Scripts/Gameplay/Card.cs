using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Suit suit;
    public int val;
    // private SpriteRenderer face;
    // private SpriteRenderer back;
    public Card(int v, Suit s)
    {
        val = v;
        suit = s;
    }

}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
