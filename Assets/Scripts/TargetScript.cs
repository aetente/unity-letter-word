using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetScript : MonoBehaviour
{

  float health = 100f;
  TextMesh tm;

  public int playerNumber = 0;

  GameObject WinnerMenu;
  Text WinnerText;

  void Start()
  {
    WinnerMenu = GameObject.Find("Menus").transform.Find("WinnerMenu").gameObject;
    WinnerText = WinnerMenu.transform.Find("MenuText").gameObject.GetComponent<Text>();
    float xPos;
    if (playerNumber == 0)
    {
      xPos = 2 * Screen.width / 6;
      // xPos = Screen.width/2 - 195;
    }
    else
    {
      xPos = Screen.width - 2 * Screen.width / 6;
      // xPos = Screen.width/2 + 195;
    }
    transform.position = Camera.main.ScreenToWorldPoint(new Vector3(xPos, Screen.height / 2, 1));
    GameObject holdHealthInfo = new GameObject("holdHealthInfo");
    tm = holdHealthInfo.AddComponent<TextMesh>();
    tm.text = health.ToString("F2") + " / 100.00";
    tm.fontSize = 20;
    tm.offsetZ = 5;
    tm.anchor = TextAnchor.UpperCenter;
    tm.alignment = TextAlignment.Center;
    holdHealthInfo.transform.position = transform.position + new Vector3(0, 5, 0);
    tm.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
  }

  void Update()
  {
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    GameObject gameControll = other.gameObject.transform.parent.gameObject;
    EnterText gameControllText = gameControll.GetComponent<EnterText>();
    if (gameControllText.whoIsPlaying != playerNumber)
    {
      health += gameControllText.accumulatedDamage;
      tm.text = health.ToString("F2") + " / 100.00";
      if (health <= 0f)
      {
        WinnerMenu.SetActive(true);
        if (gameControll.GetComponent<EnterText>().isPVP)
        {
          WinnerText.text = "Player " + (((playerNumber + 1) % 2) + 1) + " has won!";
        }
        else
        {
          if (playerNumber == 1)
          {
            WinnerText.text = "Player has won!";
          }
          else
          {

            WinnerText.text = "CPU has won!";
          }
        }
        gameControllText.isEnd = true;
      }
      else
      {
        gameControllText.posValue = transform.position;
        gameControllText.onDamage();
      }
    }
  }
}
