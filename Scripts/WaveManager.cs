using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    int minutesTillEnd = -1; //set to -1 for infinity
    float timer = 0;
    float waveTimer = 0;
    float timerCount = 60;
    float blinkerTimer = 0;
    static public int curStar = 1;
    public float maxSpawnDelay = 3;
    public float minSpawnDelay = 1;
    float spawnDelay;
    public GameObject[] star1List;
    public GameObject[] star2List;
    public GameObject[] star3List;
    public GameObject[] star4List;
    public GameObject[] star5List;
    public GameObject boss;
    bool spawnedBoss = false;
    public Sprite fullStar;
    public GameObject starContainer;
    bool canBlinkAgain = true;
    public Image star1;
    public Image star2;
    public Image star3;
    public Image star4;
    public Image star5;


    public Transform spawnPos;
    public float minDistanceAway = 20;
    public float maxDistanceAway = 50;

    public TextMeshProUGUI timerText;

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    IEnumerator starBlinking()
    {
        starContainer.SetActive(true);
        for (int i = 0; i < 6; i++)
        {
            starContainer.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            starContainer.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
        starContainer.SetActive(true);
        canBlinkAgain = true;
        blinkerTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (curStar == 1)
        {
            star1.sprite = fullStar;
        }
        if (curStar == 2)
        {
            star2.sprite = fullStar;
        }
        if (curStar == 3)
        {
            star3.sprite = fullStar;
        }
        if (curStar == 4)
        {
            star4.sprite = fullStar;
        }
        if (curStar == 5)
        {
            star5.sprite = fullStar;
        }
        if (curStar >= 5 && !spawnedBoss)
        {
            spawnedBoss = true;
            GameObject obj = Instantiate(boss, spawnPos.position, Quaternion.identity);
            float angle = Random.Range(0, 360);
            Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            obj.transform.Translate(moveDir.normalized * maxDistanceAway);
            obj.GetComponent<BaseEnemyScript>().setStartAngle();
        }
        waveTimer += Time.deltaTime;
        timer += Time.deltaTime;
        timerCount -= Time.deltaTime;
        blinkerTimer += Time.deltaTime;
        if (timerCount <= 0)
        {
            timerCount = 60;
        }
        if (blinkerTimer >= 27 && canBlinkAgain)
        {
            canBlinkAgain = false;
            StartCoroutine(starBlinking());
        }
        
        if (waveTimer >= spawnDelay)
        {
            spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            spawnWave();
            waveTimer = 0;
        }
        curStar = Mathf.CeilToInt(timer / 30);
        timerText.text = "" + (3 - Mathf.CeilToInt(timer / 60)) + ":";
        int toAdd = Mathf.CeilToInt(timerCount) - 1;
        if (toAdd < 10)
        {
            timerText.text += "0" + toAdd;
        }
        else
        {
            timerText.text += toAdd;
        }

        if (timer >= 60 * 3)
        {
            curStar = -99;
            //win
        }
    }

    void spawnWave()
    {
        int count = Mathf.CeilToInt(timer / 25);
        //int count = 1;
        if (curStar >= 5)
        {
            count = 2;
        }
        for (int i = 0; i < count; i++)
        {
            float distance = Random.Range(minDistanceAway, maxDistanceAway);
            if (curStar == 1)
            {
                int random = Random.Range(0, star1List.Length);
                GameObject obj = Instantiate(star1List[random], spawnPos.position, Quaternion.identity);
                float angle = Random.Range(0, 360);
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                obj.transform.Translate(moveDir.normalized * distance);
                obj.GetComponent<BaseEnemyScript>().setStartAngle();
            }
            else if (curStar == 2)
            {
                int random = Random.Range(0, star2List.Length);
                GameObject obj = Instantiate(star2List[random], spawnPos.position, Quaternion.identity);
                float angle = Random.Range(0, 360);
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                obj.transform.Translate(moveDir.normalized * distance);
                obj.GetComponent<BaseEnemyScript>().setStartAngle();
            }
            else if (curStar == 3)
            {
                int random = Random.Range(0, star3List.Length);
                GameObject obj = Instantiate(star3List[random], spawnPos.position, Quaternion.identity);
                float angle = Random.Range(0, 360);
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                obj.transform.Translate(moveDir.normalized * distance);
                obj.GetComponent<BaseEnemyScript>().setStartAngle();
            }
            else if (curStar == 4)
            {
                int random = Random.Range(0, star4List.Length);
                GameObject obj = Instantiate(star4List[random], spawnPos.position, Quaternion.identity);
                float angle = Random.Range(0, 360);
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                obj.transform.Translate(moveDir.normalized * distance);
                obj.GetComponent<BaseEnemyScript>().setStartAngle();
            }
            else if (curStar >= 5)
            {
                int random = Random.Range(0, star5List.Length);
                GameObject obj = Instantiate(star5List[random], spawnPos.position, Quaternion.identity);
                float angle = Random.Range(0, 360);
                Vector2 moveDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                obj.transform.Translate(moveDir.normalized * distance);
                obj.GetComponent<BaseEnemyScript>().setStartAngle();
            }
        }
    }
}
