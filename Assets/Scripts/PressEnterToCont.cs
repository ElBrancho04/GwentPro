using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterToCont : MonoBehaviour
{
    public bool turnScreen;
    public GameManager gameManager;
    public int player;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return new WaitForSecondsRealtime(0.2f);
                if (turnScreen)
                {
                    if (player == 1)
                    gameManager.waitForActivate1 = false;
                    else
                    gameManager.waitForActivate2 = false;
                    gameObject.SetActive(false);
                }
                else
                {
                    SceneManager.LoadScene("MenuScene");
                }
            }   
        }
    }
}
