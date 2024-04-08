using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public enum CardType
{
    Unit,
    Special
}


[CreateAssetMenu(fileName = "New Card", menuName = "Cards/New Card")]
public class CardData : ScriptableObject
{
    public Sprite cardImage;
    public int id;
    public string cardName;
    public string powerDescription;
    public string cardDescription;
    public bool isGold;
    public CardType cardType;
    bool isUnitCard;
    public bool isActive = false;
    public int player;
 
    // Propiedades específicas para cartas de unidad
    public bool melee;
    public bool ranged;
    public bool siege;
    public static bool staticMelee;
    public static bool staticRanged;
    public static bool staticSiege;
    public int attackPower;

}