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

    public void goToCredit()
    {
        StartCoroutine(creditssStuff());
    }

    IEnumerator creditssStuff()
    {
        WaveManager.curStar = 1;
        canvasAnim.Play("Restart");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.01f);
        SceneManager.LoadScene("Credits");
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
