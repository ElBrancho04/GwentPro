using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShowCard : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI attackPowerText;
    public int player;
    public GameObject melee;
    public GameObject ranged;
    public GameObject siege;
    public GameObject gold;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Método para asignar los datos de la carta al UI
    void Update()
    {
        if(gameManager.showedCard != null && player == gameManager.showedCard.card.player)
        {

        nameText.text = "" + gameManager.showedCard.card.cardName;
        descriptionText.text = "" + gameManager.showedCard.card.cardDescription;
        if (gameManager.showedCard.card.cardType == CardType.Unit)
        {
        attackPowerText.text = "" + (gameManager.showedCard.card.attackPower + gameManager.showedCard.card.powerVar);
        melee.SetActive(gameManager.showedCard.card.melee);
        ranged.SetActive(gameManager.showedCard.card.ranged);
        siege.SetActive(gameManager.showedCard.card.siege);
        }
        else
        {
            attackPowerText.text = "Sp";
            melee.SetActive(false);
            ranged.SetActive(false);    
            siege.SetActive(false);
        }
        gold.SetActive(gameManager.showedCard.card.isGold);

        }
    }
}
        
    

