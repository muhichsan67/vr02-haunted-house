using UnityEngine;

namespace VRSlendermanHouse
{
    public class DoorInteraction : MonoBehaviour
    {
        [Header("Door Status")]
        [SerializeField] private bool isLocked = true;
        
        // Referensi otomatis ke skrip DoorMech bawaan di objek pintumu
        private MonoBehaviour doorMechScript;

        private void Awake()
        {
            // Mencari skrip DoorMech yang menempel di objek ini
            doorMechScript = GetComponent("DoorMech") as MonoBehaviour;

            // Jika pintu disetel terkunci, matikan skrip DoorMech agar player tidak bisa membukanya langsung
            if (isLocked && doorMechScript != null)
            {
                doorMechScript.enabled = false;
            }
        }

        public void TryInteract()
        {
            PlayerCoreLogic player = FindFirstObjectByType<PlayerCoreLogic>();
            if (player == null) return;

            if (isLocked)
            {
                // Cek ke PlayerCoreLogic apakah player sudah bawa kunci
                if (player.HasKey)
                {
                    UnlockAndOpen(player);
                }
                else
                {
                    // Jika tidak punya kunci, munculkan bubble text "Its locked..."
                    player.ShowLockedDoorMessage();
                }
            }
            else
            {
                // Jika dari awal tidak terkunci, langsung picu DoorMech untuk buka/tutup pintu
                TriggerDoorMech();
            }
        }

        private void UnlockAndOpen(PlayerCoreLogic player)
        {
            isLocked = false;
            player.ShowBubbleText("I unlocked the door!");
            Debug.Log("🔓 Pintu berhasil dibuka menggunakan kunci!");

            // Aktifkan kembali skrip DoorMech bawaanmu
            if (doorMechScript != null)
            {
                doorMechScript.enabled = true;
            }

            // Perintahkan DoorMech untuk mengeksekusi fungsi bukanya
            TriggerDoorMech();
        }

        private void TriggerDoorMech()
        {
            if (doorMechScript != null)
            {
                // Mengirim perintah aman ke fungsi pembuka bawaan DoorMech milikmu
                doorMechScript.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
                doorMechScript.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}