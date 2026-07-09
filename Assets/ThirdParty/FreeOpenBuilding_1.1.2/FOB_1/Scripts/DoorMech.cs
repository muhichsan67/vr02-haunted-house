using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMech : MonoBehaviour
{
    public Vector3 OpenRotation, CloseRotation;
    public float rotSpeed = 1f;
    public bool doorBool;

    // Tambahkan satu variabel baru untuk mencatat posisi Player
    private bool isPlayerNearby = false;

    void Start()
    {
        doorBool = false;
    }

    // Ketika Player masuk ke area sensor pintu
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // Ketika Player keluar dari area sensor pintu
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void Update()
    {
        // 1. Deteksi tombol E dipindah ke Update agar responsif 100% di setiap frame
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            doorBool = !doorBool; // Cara cepat membalikkan nilai true/false
        }

        // 2. Mengubah transform.rotation menjadi transform.localRotation 
        // agar pintu berputar mengikuti engsel lokal di dalam bangunan
        if (doorBool)
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(OpenRotation), rotSpeed * Time.deltaTime);
        else
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(CloseRotation), rotSpeed * Time.deltaTime);
    }
}