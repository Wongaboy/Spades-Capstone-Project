using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public Suit suit;
    public int val;
    public Card(int v, Suit s)
    {
        val = v;
        suit = s;
    }
}

public enum Suit { SPADE, HEART, CLUB, DIAMOND}
