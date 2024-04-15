using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CardUI selectedCard;
    public int playerTurn = 0;
    public bool[] playerPass = new bool[2];
    public PlayerDeck playerDeck1;
    public PlayerDeck playerDeck2;
    public CardSpawner cardSpawner;
    public GameObject cardPrefab1;
    public GameObject cardPrefab2;
    public File playerHand1;
    public File playerHand2;
    public ShowCard showCard1;
    public ShowCard showCard2;
    public CardUI showedCard;
    public int roundsWon1;
    public int roundsWon2;
    public GameObject punto1P1;
    public GameObject punto2P1;
    public GameObject punto1P2;
    public GameObject punto2P2;
    public RoundWinnerCalculator roundWinnerCalculator;
    public File fileToPlayEfct;
    public CardUI UIcardToPlayEfct;
    public CardData cardToPlayEfct;
    public File[] tablero;

    void LateUpdate()
    {
        if (playerPass[0] == true && playerPass[1] == true)
        {
            roundWinnerCalculator.RoundWinnerCal();
            playerPass[0] = false;
            playerPass[1] = false;
            StartCoroutine(DrawCards(playerDeck1,playerDeck2));
        }
    }

    
    IEnumerator DrawCards(PlayerDeck playerDeck1, PlayerDeck playerDeck2)
    {
        cardSpawner.cardPrefab = cardPrefab1;
        cardSpawner.fileFile = playerHand1;
        cardSpawner.fieldTransform = playerHand1.transform;

        cardSpawner.SpawnCard(playerDeck1.DrawCard());

        cardSpawner.cardPrefab = cardPrefab2;
        cardSpawner.fileFile = playerHand2;
        cardSpawner.fieldTransform = playerHand2.transform;

        cardSpawner.SpawnCard(playerDeck2.DrawCard());
        yield return new WaitForSecondsRealtime(1f);
        cardSpawner.SpawnCard(playerDeck2.DrawCard());

        cardSpawner.cardPrefab = cardPrefab1;
        cardSpawner.fileFile = playerHand1;
        cardSpawner.fieldTransform = playerHand1.transform;

        cardSpawner.SpawnCard(playerDeck1.DrawCard());
    }
    
    void Update()
    {
        if (UIcardToPlayEfct != null)
        cardToPlayEfct = UIcardToPlayEfct.card;
        if (showedCard != null && showedCard.card.player != showCard1.player)
        {
            showCard1.gameObject.SetActive(false);
            showCard2.gameObject.SetActive(true);
        }
        if (showedCard != null && showedCard.card.player != showCard2.player)
        {
            showCard2.gameObject.SetActive(false);
            showCard1.gameObject.SetActive(true);
        }
        if (showedCard == null)
        {
            showCard1.gameObject.SetActive(false);
            showCard2.gameObject.SetActive(false);
        }

        if(roundsWon1 == 1)
        punto1P1.SetActive(true);

        if(roundsWon1 == 2)
        punto2P1.SetActive(true);

        if(roundsWon2 == 1)
        punto1P2.SetActive(true);

        if(roundsWon2 == 2)
        punto2P2.SetActive(true);
    }

    public void RemoveCard(GameObject card)
    {
        List<CardData> cardsFile = card.transform.parent.GetComponent<File>().cards;
        cardsFile.Remove(card.GetComponent<CardUI>().card);
        StartCoroutine(Wait(card));
    }

    IEnumerator Wait(GameObject card)
    {
        Destroy(card.GetComponent<CardUI>());
        yield return new WaitForSecondsRealtime(2.5f);
        Destroy(card);
    }
}
