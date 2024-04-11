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
        attackPowerText.text = "" + gameManager.showedCard.card.attackPower;
        melee.SetActive(gameManager.showedCard.card.melee);
        ranged.SetActive(gameManager.showedCard.card.ranged);
        siege.SetActive(gameManager.showedCard.card.siege);
        }
        
    }
}
        
    

