using UnityEngine;

namespace VRSlendermanHouse
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionDistance = 3.0f;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        [Header("References")]
        [SerializeField] private Camera playerCamera;
        private PlayerCoreLogic playerCore;

        private void Awake()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
            playerCore = GetComponent<PlayerCoreLogic>();
        }

        private void Update()
        {
            if (playerCore != null && playerCore.IsInputLocked) return;

            if (Input.GetKeyDown(interactionKey))
            {
                CheckInteraction();
            }
        }

        private void CheckInteraction()
        {
            if (playerCamera == null) return;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                // PERBAIKAN: Cari skrip Kunci di objek itu sendiri ATAU di Parent-nya
                KeyItem key = hit.collider.GetComponent<KeyItem>();
                if (key == null) key = hit.collider.GetComponentInParent<KeyItem>();
                
                if (key != null)
                {
                    key.Collect();
                    return;
                }

                // PERBAIKAN: Cari skrip Pintu di objek itu sendiri ATAU di Parent-nya (Mengatasi masalah LOD)
                DoorInteraction door = hit.collider.GetComponent<DoorInteraction>();
                if (door == null) door = hit.collider.GetComponentInParent<DoorInteraction>();

                if (door != null)
                {
                    door.TryInteract();
                    return;
                }
            }
        }
    }
}