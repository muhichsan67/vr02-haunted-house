using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitioner : MonoBehaviour
{
    public CanvasGroup blackoutGroup; // Tarik BlackoutPanel ke sini
    public float fadeSpeed = 1.5f;

    public void StartTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Fade ke Hitam
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            blackoutGroup.alpha = alpha;
            yield return null;
        }

        // 2. Pindah Scene
        SceneManager.LoadScene(sceneName);
    }
}