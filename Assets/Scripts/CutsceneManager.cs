using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void OnEnable()
    {
        // Mendaftarkan fungsi ketika video selesai diputar
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video selesai. Masuk ke Game Utama...");
        // Memuat scene utama game (Index 2: MainGameplay)
        SceneManager.LoadScene(2);
    }

    // Opsi tambahan: Pemain bisa skip video dengan menekan tombol Space/Enter
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(2);
        }
    }
}