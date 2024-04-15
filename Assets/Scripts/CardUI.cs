using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    public GameManager gameManager;
    public CardData card;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI attackPowerText;
    public bool isSelected;
    public GameObject melee;
    public GameObject ranged;
    public GameObject siege;
    public GameObject gold;
    public int actPower;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        nameText.text = "" + card.cardName;
        descriptionText.text = "" + card.cardDescription;
        if (card.cardType == CardType.Unit)
        {
            
        melee.SetActive(card.melee);
        ranged.SetActive(card.ranged);
        siege.SetActive(card.siege);
        card.actPower = actPower = card.attackPower + card.powerVar;
        attackPowerText.text = "" + actPower;
        }
        
        if (gameManager.selectedCard != this)
        {
            isSelected = false;
        }
        gold.SetActive(card.isGold);  


        if(card.playEfect)
        {
            Efects efects = FindAnyObjectByType<Efects>();
            efects.PlayEfect(card);
            card.playEfect = false;
        }
    }
    public void OnClick()
    {
        if (gameManager.playerTurn == 0 && card.isActive)
        gameManager.UIcardToPlayEfct = this;
        if (card.player == gameManager.playerTurn && gameManager.playerPass[card.player - 1] == false)
        {

        isSelected = true;
        gameManager.selectedCard = this;
        }  
        gameManager.showedCard = this;   
    }
}
        
    

