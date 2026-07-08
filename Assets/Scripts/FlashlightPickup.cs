using UnityEngine;

namespace VRSlendermanHouse
{
    public class FlashlightPickup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerCoreLogic playerCore;

        [Header("Testing Mode")]
        [SerializeField] private bool allowTriggerPickupForTesting = true;
        [SerializeField] private KeyCode pickupKey = KeyCode.E;

        private bool playerInRange = false;

        private void Update()
        {
            if (!allowTriggerPickupForTesting)
            {
                return;
            }

            if (playerInRange && Input.GetKeyDown(pickupKey))
            {
                PickUp();
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

        public void PickUp()
        {
            if (playerCore == null)
            {
                Debug.LogWarning("PlayerCoreLogic belum di-assign.");
                return;
            }

            playerCore.CollectFlashlight();
            gameObject.SetActive(false);
        }
    }
}
