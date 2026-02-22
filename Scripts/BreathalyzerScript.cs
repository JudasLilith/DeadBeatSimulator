using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreathalyzerScript : MonoBehaviour
{
    bool ranLoadScene = false;
    public Animator canvasAnim;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Y) && !ranLoadScene)
        {
            ranLoadScene = true;
            StartCoroutine(goToMainMenu());
        }
    }

    IEnumerator goToMainMenu()
    {
        canvasAnim.Play("BeerSip");
        yield return new WaitForSeconds(6.25f);
        SceneManager.LoadScene("MainScene");
    }
}
