using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Letter
{
    public Letter(float angleValue = 0f, float damageValue = 0f, float distanceValue = 0f, char symbolValue = 'a')
    {
        angle = angleValue;
        damage = damageValue;
        distanceDiff = distanceValue;
        symbol = symbolValue;
    }
    public float angle;
    public float damage;
    public float distanceDiff;
    public char symbol;
}

public class EnterText : MonoBehaviour
{

    public bool isPVP = true;
    public float damageMultiplier = 1.5f;
    public GameObject player1;
    public GameObject player2;
    public Vector3 posValue = new Vector3(0f, 0f, 0f);
    float angleRandomness = 0f;
    public float accumulatedDamage = 0f;
    int amountOfLetters = 0;
    int maxAmountOfLetters = 80;
    public float rand;
    TextMesh tm;
    TextMesh limitTm;
    TextMesh damageTm;
    GameObject letterGameObject;
    bool isPause = false;
    public bool isEnd = false;
    public int whoIsPlaying = 0;
    GameObject PauseMenu;

    private Dictionary<char, Letter> letterOptions = new Dictionary<char, Letter>();
    int successfullLetters = 0;
    private Dictionary<char, Letter> successfullLetterOptions = new Dictionary<char, Letter>();
    bool movedSuccessfully = false;
    bool moved = false;
    float elapsed = 0.0f;


    float Atan2Angle(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }


    void Start()
    {
        Debug.Log(Manager.CPUAngle);
        PauseMenu = GameObject.Find("Menus").transform.Find("PauseMenu").gameObject;
        posValue = Camera.main.ScreenToWorldPoint(new Vector3(2 * Screen.width / 6, Screen.height / 2, 0));
        // posValue = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2 - 195, Screen.height / 2, 0));
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        rand = 2f + Random.value;

        GameObject limitGameObject = new GameObject("limitGameObject");
        limitTm = limitGameObject.AddComponent<TextMesh>();
        limitTm.text = "Letters limit: " + amountOfLetters + " / " + maxAmountOfLetters;
        limitTm.fontSize = 20;
        limitTm.offsetZ = 5;
        limitTm.anchor = TextAnchor.UpperRight;
        limitTm.alignment = TextAlignment.Right;
        limitGameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width - 100f, Screen.height - 40, 0));
        limitTm.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        GameObject damageGameObject = new GameObject("damageGameObject");
        damageTm = damageGameObject.AddComponent<TextMesh>();
        damageTm.text = "Accumulated damage: " + accumulatedDamage;
        damageTm.fontSize = 20;
        damageTm.offsetZ = 5;
        damageTm.anchor = TextAnchor.UpperLeft;
        damageTm.alignment = TextAlignment.Left;
        damageGameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(170f, Screen.height - 40, 0));
        damageTm.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
    }

    public void onDamage()
    {
        amountOfLetters = 0;
        limitTm.text = "Letters limit: " + amountOfLetters + " / " + maxAmountOfLetters;
        whoIsPlaying = (whoIsPlaying + 1) % 2;
        accumulatedDamage = 0f;
        damageTm.text = "Accumulated damage: " + accumulatedDamage;
        rand = Random.value;
        if (!isPVP)
        {
            letterOptions = new Dictionary<char, Letter>();
            successfullLetters = 0;
            successfullLetterOptions = new Dictionary<char, Letter>();
            movedSuccessfully = false;
            moved = false;
        }
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private char GetLetter()
    {
        string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
        int num = (int)Mathf.Floor(Random.value * chars.Length);
        return chars[num];
    }

    Letter doMove(char ch)
    {
        GameObject currentTarget;
        if (player1.GetComponent<TargetScript>().playerNumber == whoIsPlaying)
        {
            currentTarget = player2;
        }
        else
        {
            currentTarget = player1;
        }
        if (tm != null)
        {
            Destroy(letterGameObject.GetComponent<Rigidbody2D>());
            Destroy(letterGameObject.GetComponent<BoxCollider2D>());
        }
        amountOfLetters += 1;
        limitTm.text = "Letters limit: " + amountOfLetters + " / " + maxAmountOfLetters;
        if (amountOfLetters > maxAmountOfLetters)
        {
            onDamage();
            posValue = currentTarget.transform.position;
        }
        float charHash = ch.GetHashCode();
        angleRandomness = charHash * rand;
        // angleRandomness = 1;
        Vector3 newPos = new Vector3(posValue.x + 1f * Mathf.Sin(angleRandomness), posValue.y + 1f * Mathf.Cos(angleRandomness), 0f);
        Vector2 a = new Vector2(posValue.x, posValue.y);
        Vector2 b = new Vector2(newPos.x, newPos.y);
        float d1 = Vector2.Distance(currentTarget.transform.position, a);
        float d2 = Vector2.Distance(currentTarget.transform.position, b);

        float letterAngle = Atan2Angle(a, b);

        posValue = newPos;
        letterGameObject = new GameObject("letterGameObject");

        letterGameObject.transform.SetParent(this.transform);
        letterGameObject.transform.eulerAngles = new Vector3(0f, 0f, angleRandomness * Mathf.Rad2Deg);
        BoxCollider2D bc2d = letterGameObject.AddComponent<BoxCollider2D>();
        bc2d.isTrigger = true;
        bc2d.size = new Vector2(4f, 4f);
        Rigidbody2D rg2d = letterGameObject.AddComponent<Rigidbody2D>();
        rg2d.gravityScale = 0f;
        tm = letterGameObject.AddComponent<TextMesh>();
        tm.text = ch.ToString();
        tm.fontSize = 20;
        tm.offsetZ = 5;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        float damage = (Mathf.Sin(angleRandomness * rand * 1000f) + 1.0f) / 2f;
        accumulatedDamage += (damageMultiplier * (0.5f - damage));
        damageTm.text = "Accumulated damage: " + accumulatedDamage;
        tm.GetComponent<Renderer>().material.SetColor("_Color", new Color(Mathf.Lerp(0f, 1f, damage), Mathf.Lerp(1f, 0f, damage), 0f));

        tm.transform.localPosition = new Vector3(0f, 0f, 0f);
        tm.characterSize = 1;
        letterGameObject.AddComponent<LetterScript>();
        letterGameObject.transform.position = posValue;

        return new Letter(letterAngle, (damageMultiplier * (0.5f - damage)), d1 - d2, ch);
    }

    void Update()
    {
        if (!isEnd && Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            PauseMenu.SetActive(isPause);
        }
        else if (!isEnd && !isPause && (Input.anyKeyDown || (!isPVP && whoIsPlaying == 1)))
        {
            if (isPVP || whoIsPlaying == 0)
            {
                if (Input.inputString != "")
                {
                    char ch = char.Parse(Input.inputString.Substring(0, 1));
                    doMove(ch);
                }
            }
            else if (amountOfLetters <= maxAmountOfLetters)
            {
                elapsed += Time.deltaTime;
                if (elapsed > 0.1f)
                {
                    elapsed = 0f;
                    if (!moved || letterOptions.Count < 10 || successfullLetters < 3)
                    {

                        char letterToCheck = GetLetter();
                        if (!letterOptions.ContainsKey(letterToCheck))
                        {
                            Letter l = doMove(letterToCheck);
                            letterOptions.Add(letterToCheck, l);

                            if (l.distanceDiff > 0 && l.damage < 0)
                            {
                                successfullLetterOptions.Add(letterToCheck, l);
                                successfullLetters += 1;
                            }
                            moved = true;
                        }
                    }
                    else
                    {
                        moved = false;
                        movedSuccessfully = false;
                        GameObject currentTarget;
                        if (player1.GetComponent<TargetScript>().playerNumber == whoIsPlaying)
                        {
                            currentTarget = player2;
                        }
                        else
                        {
                            currentTarget = player1;
                        }
                        Vector3 targetPosition = currentTarget.transform.position;
                        Vector2 b = new Vector2(targetPosition.x, targetPosition.y);
                        Vector2 a = new Vector2(posValue.x, posValue.y);
                        float desiredAngle = Atan2Angle(a, b);
                        foreach (KeyValuePair<char, Letter> kvp in successfullLetterOptions)
                        {
                            Letter l = kvp.Value;
                            if (Mathf.Abs(l.angle - desiredAngle) < Manager.CPUAngle)
                            {
                                doMove(l.symbol);
                                movedSuccessfully = true;
                            }
                        }
                        if (!movedSuccessfully && accumulatedDamage < 0)
                        {
                            foreach (KeyValuePair<char, Letter> kvp in letterOptions)
                            {
                                Letter l = kvp.Value;
                                if (Mathf.Abs(l.angle - desiredAngle) < 3)
                                {
                                    doMove(l.symbol);
                                    moved = true;
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
