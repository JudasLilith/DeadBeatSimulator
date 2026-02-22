using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScript : MonoBehaviour
{
    public float deathTime = 1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(death());
    }
    IEnumerator death()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
