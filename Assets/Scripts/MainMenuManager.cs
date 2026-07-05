using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Dibutuhkan jika ingin memanipulasi teks lewat script

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private float flickerSpeed = 2f;

    void Update()
    {
        // 1. Deteksi Input Keyboard (Mendukung Enter Utama dan Enter Numpad)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Debug.Log("<color=green>PENCETAN ENTER TERDETEKSI!</color>");
            StartGame();
        }

        // 2. Fungsi Efek Berkedip (Flickering/Breathing) secara halus
        if (promptText != null)
        {
            // Menggunakan rumus matematika Sinus untuk membuat transparan-terang secara berulang
            float alpha = (Mathf.Sin(Time.time * flickerSpeed) + 1f) / 2f;
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
        }
    }

    private void StartGame()
    {
        // Mengecek apakah scene dengan indeks 1 sudah terdaftar di Build Settings
        if (SceneManager.sceneCountInBuildSettings > 1)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogError("Gagal pindah scene! Pastikan 'IntroCutscene' sudah dimasukkan ke File > Build Settings.");
        }
    }
}