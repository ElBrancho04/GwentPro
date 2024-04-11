using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class File : MonoBehaviour
{
      public GameManager gameManager;
      public List<CardData> cards = new List<CardData>();   
      public bool melee = false;
      public bool ranged = false;
      public bool siege = false;
      public int player;
      public Transform fileTransform;
      public bool hand;


      public void OnClick()
      {
            if (cards.Count < 10 && gameManager.selectedCard != null && gameManager.playerPass[player - 1] == false && player == gameManager.selectedCard.card.player && ((gameManager.selectedCard.card.melee == melee && melee == true )||(gameManager.selectedCard.card.ranged == ranged && ranged == true )||(gameManager.selectedCard.card.siege == siege && siege == true )))
            {
                  gameManager.selectedCard.transform.SetParent(fileTransform);
                  cards.Add(gameManager.selectedCard.card);
                  gameManager.selectedCard.card.isActive = true;
                  gameManager.selectedCard = null;
                  if (player == 1 && gameManager.playerPass[1] == false)
                  {
                        gameManager.playerTurn = 2;
                  }
                  else if(gameManager.playerPass[0] == false)
                  {
                        gameManager.playerTurn = 1;
                  }
            }
      }
      void Update()
      {
            for (int i = 0; i < cards.Count; i++)
            {
                  if (cards[i].isActive == hand)
                  {
                        cards.RemoveAt(i);
                  }
            }
      }
}
