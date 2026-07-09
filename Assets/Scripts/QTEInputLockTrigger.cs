using UnityEngine;

namespace VRSlendermanHouse
{
    public class QTEInputLockTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerCoreLogic playerCore;
        [SerializeField] private Transform qteLookTarget;

        [Header("End Game UI & Sequence Settings")]
        [Tooltip("Tarik objek 'HUD_Canvas' dari Hierarchy ke sini")]
        [SerializeField] private GameObject hudCanvas;
        [Tooltip("Tarik objek 'QTE_Canvas' dari Hierarchy ke sini")]
        [SerializeField] private GameObject qteCanvas;
        [Tooltip("Tarik komponen QTEInputLogic dari objek QTE_Canvas ke sini")]
        [SerializeField] private QTEInputLogic qteInputLogicScript;

        [Header("Testing")]
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && hasTriggered) return;

            if (!other.CompareTag("Player")) return;

            if (playerCore == null)
            {
                playerCore = other.GetComponent<PlayerCoreLogic>();
            }

            if (playerCore == null)
            {
                playerCore = other.GetComponentInParent<PlayerCoreLogic>();
            }

            if (playerCore == null)
            {
                Debug.LogWarning("PlayerCoreLogic tidak ditemukan di Player.");
                return;
            }

            // =========================================================================
            // Cek apakah semua kertas sudah terkumpul
            // =========================================================================
            if (!playerCore.IsAllPapersCollected)
            {
                Debug.Log("🚪 Pintu Keluar: Kamu belum mengumpulkan ke-4 kertas! Trigger dibatalkan.");
                return; 
            }
            // =========================================================================

            // Jika sudah 4 kertas, kode di bawah ini baru akan dieksekusi:
            hasTriggered = true;

            // 1. Kunci pergerakan player dan arahkan kamera ke daun pintu
            playerCore.SetInputLock(true, qteLookTarget);

            // 2. Menghilangkan seluruh HUD Canvas lama (Noise bar, item, dll)
            if (hudCanvas != null)
            {
                hudCanvas.SetActive(false);
            }

            // 3. Pastikan GameObject QTE Canvas aktif di Hierarchy agar komponen di dalamnya bisa bekerja
            if (qteCanvas != null)
            {
                qteCanvas.SetActive(true);
            }

            // 4. Jalankan logika ketuk tombol E dan munculkan visualnya lewat CanvasGroup
            if (qteInputLogicScript != null)
            {
                qteInputLogicScript.StartQTE(); // Fungsi ini otomatis menyalakan skrip dan memunculkan Alpha UI
            }
            else
            {
                Debug.LogError("❌ QTEInputLogicScript belum dipasang pada slot Inspector Trigger!");
            }
        }

        public void ReleasePlayer()
        {
            if (playerCore != null)
            {
                playerCore.SetInputLock(false);
            }
        }
    }
}