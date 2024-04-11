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
    public Transform myTransform;
    public GameObject melee;
    public GameObject ranged;
    public GameObject siege;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Método para asignar los datos de la carta al UI
    void Update()
    {
        nameText.text = "" + card.cardName;
        descriptionText.text = "" + card.cardDescription;
        attackPowerText.text = "" + card.attackPower;
        if (gameManager.selectedCard != this)
        {
            isSelected = false;
        }

        melee.SetActive(card.melee);
        ranged.SetActive(card.ranged);
        siege.SetActive(card.siege);
    }
    public void OnClick()
    {
        if (card.player == gameManager.playerTurn && gameManager.playerPass[card.player - 1] == false)
        {

        isSelected = true;
        gameManager.selectedCard = this;
        }  
        gameManager.showedCard = this;   
    }
}
        
    

