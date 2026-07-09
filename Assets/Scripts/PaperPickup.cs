using UnityEngine;

namespace VRSlendermanHouse
{
    public class PaperPickup : MonoBehaviour
    {
        [Header("References")]
        private PlayerCoreLogic playerCore;

        [Header("Pickup Settings")]
        [SerializeField] private KeyCode pickupKey = KeyCode.E; // Menyamakan tombol interaksi tim
        
        [Header("Audio Settings")]
        [SerializeField] private AudioClip paperPickupSFX;

        private bool playerInRange = false;

        private void Update()
        {
            // Jika pemain berada di dekat kertas dan menekan tombol E
            if (playerInRange && Input.GetKeyDown(pickupKey))
            {
                PickUpPaper();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;

                if (playerCore == null)
                {
                    playerCore = other.GetComponent<PlayerCoreLogic>();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }

        private void PickUpPaper()
        {
            if (playerCore == null)
            {
                Debug.LogWarning("PlayerCoreLogic tidak ditemukan pada objek Player yang masuk ke trigger.");
                return;
            }

            // Jalankan fungsi tambah angka kertas di Player Core
            playerCore.CollectPaper();

            if (paperPickupSFX != null)
            {
                AudioSource.PlayClipAtPoint(paperPickupSFX, transform.position);
            }

            // Hancurkan objek kertas di dunia game karena sudah diambil
            Destroy(gameObject);
        }
    }
}