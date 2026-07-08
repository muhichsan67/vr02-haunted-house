using UnityEngine.Playables;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public PlayableDirector playable; 

    private void OnTriggerEnter (Collider other) {
        if(other.transform.tag == "Player") {
            playable.Play();
            Debug.Log("Playable is playing: " + playable.gameObject.name);
            Destroy(this.gameObject);
        }
    }
}
