using UnityEngine;

namespace VRSlendermanHouse
{
    public class QTEInputLockTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerCoreLogic playerCore;
        [SerializeField] private Transform qteLookTarget;

        [Header("Testing")]
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && hasTriggered)
            {
                return;
            }

            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (playerCore == null)
            {
                playerCore = other.GetComponent<PlayerCoreLogic>();
            }

            if (playerCore == null)
            {
                Debug.LogWarning("PlayerCoreLogic tidak ditemukan di Player.");
                return;
            }

            hasTriggered = true;
            playerCore.SetInputLock(true, qteLookTarget);
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
