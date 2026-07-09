using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRSlendermanHouse
{
    public class PlayerCoreBAK : MonoBehaviour
    {
        // ... (Variabel lain biarkan tetap ada) ...

        [Header("QTE Integration")]
        [SerializeField] private QTEInputLogic qteInputLogic; // KUNCI UTAMA
        
        private Coroutine flickerRoutine;
        private float currentNoise = 0f;
        private bool threshold50Triggered = false;

        // ... (Awake, HandleFlashlightInput, dll biarkan seperti semula) ...

        private void CheckThresholds()
        {
            // 1. Fail Logic (100% Noise)
            if (currentNoise >= 100f)
            {
                if (qteInputLogic != null) qteInputLogic.TriggerFailQTE();
            }

            // 2. Flickering Logic (Continuous jika > 50%)
            if (currentNoise > 50f)
            {
                if (flickerRoutine == null)
                {
                    flickerRoutine = StartCoroutine(ContinuousFlickerRoutine());
                }
            }
            else
            {
                if (flickerRoutine != null)
                {
                    StopCoroutine(flickerRoutine);
                    flickerRoutine = null;
                    RestoreAllLights();
                }
            }
        }

        private IEnumerator ContinuousFlickerRoutine()
        {
            Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            
            while (true) 
            {
                // Matikan
                foreach (Light lt in allLights) 
                {
                    if (lt != null && lt.enabled) lt.enabled = false;
                }
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
                
                // Nyalakan
                foreach (Light lt in allLights) 
                {
                    if (lt != null) lt.enabled = true;
                }
                yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            }
        }

        private void RestoreAllLights()
        {
            Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            foreach (Light lt in allLights)
            {
                if (lt != null) lt.enabled = true;
            }
        }

        // ... (Sisanya kode lama kamu yang lainnya tetap di bawah sini) ...
    }
}