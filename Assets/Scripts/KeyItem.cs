using UnityEngine;

namespace VRSlendermanHouse
{
    public class KeyItem : MonoBehaviour
    {
        // Fungsi utama untuk mengambil kunci (dipicu oleh PlayerInteraction lewat tombol E)
        public void Collect()
        {
            PlayerCoreLogic player = FindFirstObjectByType<PlayerCoreLogic>();
            if (player != null)
            {
                player.CollectKey(); // Memicu status kunci & bubble text di Player
                Destroy(gameObject); // Menghapus objek kunci dari map
            }
        }
    }
}