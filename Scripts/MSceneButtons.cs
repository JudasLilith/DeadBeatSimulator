using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MSceneButtons : MonoBehaviour
{
    public Animator canvasAnim;
    public void restartCurScene()
    {
        StartCoroutine(actualRestartCurScene());
    }

    public void goToBeerDrink()
    {
        StartCoroutine(beer());
    }

    IEnumerator beer()
    {
        canvasAnim.Play("Restart");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.01f);
        SceneManager.LoadScene("BeerSip");
    }

    IEnumerator actualRestartCurScene()
    {
        WaveManager.curStar = 1;
        canvasAnim.Play("Restart");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.01f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
