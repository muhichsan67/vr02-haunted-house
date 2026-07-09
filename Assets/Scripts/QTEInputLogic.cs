using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace VRSlendermanHouse
{
    public class QTEInputLogic : MonoBehaviour
    {
        [Header("QTE Settings")]
        [SerializeField] private KeyCode mashKey = KeyCode.E;
        [SerializeField] private float targetProgress = 100f;
        [SerializeField] private float pointsPerMash = 12f;
        [SerializeField] private float decayRate = 8f; 

        [Header("Fail Settings")]
        [SerializeField] private float timeLimit = 10f; // Batas waktu 10 detik
        private float timer; // Variabel untuk menghitung mundur

        [Header("UI References")]
        [SerializeField] private Image progressSlider; // Pastikan tipe data ini Image
        [SerializeField] private TextMeshProUGUI tmpPromptText;
        [SerializeField] private CanvasGroup qteCanvasGroup; // Tambahan untuk kontrol visibility UI

        [Header("Audio References")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip failSFX;      

        private float currentProgress = 0f;
        private bool isQTEActive = false;

        private void Awake()
        {
            // Ambil komponen CanvasGroup secara otomatis jika lupa dipasang di Inspector
            if (qteCanvasGroup == null)
            {
                qteCanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            // Sembunyikan UI QTE saat awal game dimulai
            HideQTEUI();
            enabled = false; 
        }

        // 1. UBAH STARTQTE SEPERTI INI (Pastikan enabled = true di paling bawah)
        public void StartQTE()
        {
            Debug.Log("StartQTE Dipanggil!");
            
            // 1. Pastikan GameObject aktif (ini menggantikan SetActive(true))
            gameObject.SetActive(true); 

            // 2. Paksa Alpha jadi 1 agar terlihat
            if (qteCanvasGroup != null)
            {
                qteCanvasGroup.alpha = 1f;
                qteCanvasGroup.blocksRaycasts = true;
                qteCanvasGroup.interactable = true;
            }

            // 3. Pastikan timer dan progress reset
            enabled = true;
            isQTEActive = true;
            timer = timeLimit;
            currentProgress = 0f;
            
            if (progressSlider != null) progressSlider.fillAmount = 0f;
            if (tmpPromptText != null) tmpPromptText.text = "TEKAN " + mashKey.ToString() + " CEPAT!";
        }

        // 2. UBAH UPDATE SEPERTI INI
        private void Update()
        {
            if (!isQTEActive) return;

            // Logika Timer
            timer -= Time.deltaTime;
            
            // Update teks UI
            if (tmpPromptText != null) 
            {
                tmpPromptText.text = "TEKAN " + mashKey.ToString() + " CEPAT! Waktu: " + Mathf.Max(0, Mathf.CeilToInt(timer)) + "s";
            }

            // Logika Mash
            if (Input.GetKeyDown(mashKey))
            {
                currentProgress += pointsPerMash;
            }

            // Logika Decay
            currentProgress -= decayRate * Time.deltaTime;
            currentProgress = Mathf.Clamp(currentProgress, 0f, targetProgress);

            // Update Visual Slider
            if (progressSlider != null)
            {
                progressSlider.fillAmount = currentProgress / targetProgress;
            }

            // PENTING: Tambahkan pengecekan timer > 0 agar tidak langsung gagal di detik pertama
            if (timer > 0)
            {
                // Kondisi Menang
                if (currentProgress >= targetProgress)
                {
                    WinQTE();
                }
            }
            // Kondisi Gagal (Hanya jika timer benar-benar habis DAN progress < 100%)
            else if (timer <= 0)
            {
                TriggerFailQTE();
            }
        }
        private void WinQTE()
        {
            isQTEActive = false;
            enabled = false;
            
            if (tmpPromptText != null) tmpPromptText.text = "KAMU BERHASIL KABUR!";
            Debug.Log("🎉 Player Menang QTE! Pindah ke MainScene...");
            
            SceneManager.LoadScene("MainScene");
        }

        public void TriggerFailQTE()
        {
            isQTEActive = false;
            enabled = false;

            if (tmpPromptText != null) tmpPromptText.text = "KAMU TERTANGKAP!";

            if (audioSource != null && failSFX != null)
            {
                audioSource.PlayOneShot(failSFX);
            }

            Debug.Log("💀 Player Gagal QTE!");
        }

        // Fungsi pembantu untuk memunculkan UI
        private void ShowQTEUI()
        {
            if (qteCanvasGroup != null)
            {
                qteCanvasGroup.alpha = 1f;
                qteCanvasGroup.blocksRaycasts = true;
                qteCanvasGroup.interactable = true;
            }
        }

        // Fungsi pembantu untuk menyembunyikan UI
        private void HideQTEUI()
        {
            if (qteCanvasGroup != null)
            {
                qteCanvasGroup.alpha = 0f;
                qteCanvasGroup.blocksRaycasts = false;
                qteCanvasGroup.interactable = false;
            }
        }
    }
}