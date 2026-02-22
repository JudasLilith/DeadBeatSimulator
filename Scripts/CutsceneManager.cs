using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public CanvasGroup fader;
    public CanvasGroup cutscene;
    public GameObject menus;
    public float fadeDuration = 1.0f;
    public float cutsceneDisplayTime = 1.0f;

    // When button is clicked
    public void StartCutscene()
    {
        menus.SetActive(false); // Hides menus
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        yield return StartCoroutine(Fade(cutscene, 0, 1)); // Fade in cutscene
        yield return new WaitForSeconds(cutsceneDisplayTime);
        yield return StartCoroutine(Fade(fader, 0, 1)); // Fade out to black
        SceneManager.LoadScene(1);
    }

    IEnumerator Fade(CanvasGroup group, float start, float end)
    {
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, counter / fadeDuration); // Smoothly calculates value between start and end over fadeDuration
            yield return null;
        }
        group.alpha = end;
    }
}
