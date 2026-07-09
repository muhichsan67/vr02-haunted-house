using UnityEngine;

public class GateInteraction : MonoBehaviour
{
    public string targetSceneName = "MainScene"; // Nama scene tujuan
    public SceneTransitioner transitioner; // Tarik object yang ada skrip Transitioner tadi
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Memasuki Rumah...");
            transitioner.StartTransition(targetSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}