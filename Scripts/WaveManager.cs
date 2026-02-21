using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    int minutesTillEnd = -1; //set to -1 for infinity
    float timer = 0;
    float waveTimer = 0;
    int curStar = 1;
    public float maxSpawnDelay = 3;
    public float minSpawnDelay = 1;
    float spawnDelay;
    public GameObject[] star1List;
    public GameObject[] star2List;
    public GameObject[] star3List;
    public GameObject[] star4List;
    public GameObject[] star5List;

    public Transform spawnPos;
    public float minDistanceAway = 20;
    public float maxDistanceAway = 50;

    // Start is called before the first frame update
    void Start()
    {
        spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        waveTimer += Time.deltaTime;
        timer += Time.deltaTime;
        if (waveTimer >= spawnDelay)
        {
            spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            spawnWave();
            waveTimer = 0;
        }
        //curStar = Mathf.CeilToInt(timer / 60);
        
    }

    void spawnWave()
    {
        int count = Mathf.CeilToInt(timer / 100);
        //int count = 1;
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
            }
            else if (curStar == 3)
            {
                int random = Random.Range(0, star3List.Length);
            }
            else if (curStar == 4)
            {
                int random = Random.Range(0, star4List.Length);
            }
            else if (curStar == 5)
            {
                int random = Random.Range(0, star5List.Length);
            }
        }
    }
}
