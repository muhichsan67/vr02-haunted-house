using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRSlendermanHouse
{
    public class PlayerCoreLogic : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Transform playerCamera;

        [Tooltip("Masukkan script controller dari ModularFirstPersonController di sini.")]
        [SerializeField] private Behaviour[] controllerComponentsToDisable;

        [Header("Flashlight")]
        [SerializeField] private GameObject carriedFlashlightObject;
        [SerializeField] private Light flashlightLight;
        [SerializeField] private KeyCode flashlightToggleKey = KeyCode.Z;

        [Header("Paper Collection System")]
        [SerializeField] private TextMeshProUGUI tmpPaperNumber;
        private int collectedPapers = 0;
        private int totalPapers = 4;
        // Tambahkan baris ini agar script pintu bisa mengecek status kertas
        public bool IsAllPapersCollected => collectedPapers >= totalPapers;

        [Header("Blackout Settings")]
        [SerializeField] private float blackoutDuration = 3.0f;
        [SerializeField] private bool includeFlashlightInBlackout = false;

        [Header("Noise UI & Bubble Text References")]
        [SerializeField] private Image imgNoiseBarFill;
        [SerializeField] private TextMeshProUGUI tmpNoisePercentage;
        [Tooltip("Tarik UI TextMeshPro untuk teks narasi/bubble ke sini!")]
        [SerializeField] private TextMeshProUGUI tmpBubbleText;

        // =========================================================================
        // KONTROL KHUSUS AUTO-HIDE BUBBLE TEXT
        // =========================================================================
        [Header("Bubble Text Auto-Hide Settings")]
        [Tooltip("Tarik GameObject 'HUD_BubbleTextPlayer' dari Hierarchy ke sini")]
        [SerializeField] private GameObject hudBubbleTextGroup;
        [Tooltip("Berapa lama teks narasi bertahan sebelum hilang kembali otomatis")]
        [SerializeField] private float textDisplayDuration = 3.0f;
        // =========================================================================

        [Header("Noise Design Settings")]
        [SerializeField] private float maxNoise = 100f;
        [SerializeField] private float noiseDecayRate = 3f; 

        private float currentNoise = 0f;
        private int currentFloorSection = 3; 
        private bool isInSafeZone = false;   
        private bool threshold50Triggered = false; 

        [Header("Input Lock Settings")]
        [SerializeField] private bool lockFlashlightDuringQTE = true;

        // Tambahkan baris ini di bagian atas kelas bersama dengan variabel [Header] lainnya
        [Header("End Game Event Settings")]
        [Tooltip("Tarik game object 'LampuExit' dari Hierarchy ke sini")]
        [SerializeField] private Light exitLampLight;
        [SerializeField] private Color horrorGreenColor = new Color(0f, 0.6f, 0.1f); // Warna hijau kesan horror

        // 1. Tambahkan ini di bagian atas kelas agar bisa ditarik di Inspector
        [Header("QTE Integration")]
        [SerializeField] private QTEInputLogic qteInputLogic; 
        
        private bool hasFailed = false; // Mencegah fail dipanggil terus-menerus

        private bool hasFlashlight = false;
        private bool flashlightOn = false;
        private bool inputLocked = false;
        private bool hasKey = false; 

        private Coroutine cameraLockRoutine;
        private Coroutine blackoutRoutine;
        private Coroutine noiseBlackoutRoutine;
        private Coroutine bubbleTextRoutine; // Tracker timer Bubble Text

        // Properties Aksesibilitas Script Lain
        public bool HasFlashlight => hasFlashlight;
        public bool IsInputLocked => inputLocked;
        public bool HasKey => hasKey;

        private void Awake()
        {
            if (playerRoot == null)
            {
                playerRoot = transform;
            }

            if (carriedFlashlightObject != null)
            {
                carriedFlashlightObject.SetActive(false);
            }

            if (tmpBubbleText != null)
            {
                tmpBubbleText.text = "";
            }

            // PERBAIKAN: Hanya grup Bubble Text yang otomatis di-uncheck (sembunyi) di awal game
            if (hudBubbleTextGroup != null) 
            {
                hudBubbleTextGroup.SetActive(false);
            }

            SetFlashlight(false);
            UpdatePaperUI();
            UpdateNoiseUI();
        }

        private void Update()
        {
            HandleFlashlightInput();
            HandleNoiseSystem();
        }

        private void HandleFlashlightInput()
        {
            if (!Input.GetKeyDown(flashlightToggleKey))
            {
                return;
            }

            if (!hasFlashlight)
            {
                Debug.Log("Senter belum diambil, tombol Z masih terkunci.");
                ShowBubbleText("I need to find flashlight first");
                return;
            }

            if (inputLocked && lockFlashlightDuringQTE)
            {
                return;
            }

            ToggleFlashlight();
        }

        // =========================================================================
        // NOISE SYSTEM (Kembali 100% Menggunakan Logika Murni Milikmu)
        // =========================================================================
        private void HandleNoiseSystem()
        {
            if (inputLocked)
            {
                DecayNoiseOnly();
                UpdateNoiseUI();
                CheckThresholds();
                return;
            }

            bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
            bool isRunning = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            bool isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C); 

            float addedNoiseRate = 0f;

            if (isInSafeZone)
            {
                addedNoiseRate = 0f;
            }
            else
            {
                UpdateFloorSection();

                switch (currentFloorSection)
                {
                    case 3: 
                        if (isRunning) addedNoiseRate = 5f;
                        else addedNoiseRate = 0f;
                        break;

                    case 2: 
                        if (isRunning) addedNoiseRate = 10f;
                        else if (isCrouching) addedNoiseRate = 0f;
                        else if (isMoving) addedNoiseRate = 2f;
                        break;

                    case 1: 
                        if (isRunning) addedNoiseRate = 15f;
                        else if (isCrouching) addedNoiseRate = 2f;
                        else if (isMoving) addedNoiseRate = 7f;
                        break;
                }
            }

            float netNoiseRate = addedNoiseRate;
            if (isInSafeZone || !isRunning)
            {
                netNoiseRate -= noiseDecayRate;
            }

            currentNoise += netNoiseRate * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && !isInSafeZone)
            {
                float jumpBurst = 0f;
                if (currentFloorSection == 3) jumpBurst = 5f;
                else if (currentFloorSection == 2) jumpBurst = 10f;
                else if (currentFloorSection == 1) jumpBurst = 15f;

                currentNoise += jumpBurst;
            }

            currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);

            UpdateNoiseUI();
            CheckThresholds();
        }

        private void DecayNoiseOnly()
        {
            currentNoise -= noiseDecayRate * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0f, maxNoise);
        }

        private void UpdateFloorSection()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                string surfaceName = hit.collider.name.ToLower();

                if (surfaceName.Contains("lantai3") || surfaceName.Contains("section1") || surfaceName.Contains("terang"))
                {
                    currentFloorSection = 3;
                }
                else if (surfaceName.Contains("lantai2") || surfaceName.Contains("section2") || surfaceName.Contains("remang"))
                {
                    currentFloorSection = 2;
                }
                else if (surfaceName.Contains("lantai1") || surfaceName.Contains("section3") || surfaceName.Contains("gelap") || surfaceName.Contains("kegelapan"))
                {
                    currentFloorSection = 1;
                }
            }
        }
        // =========================================================================

        private void CheckThresholds()
        {
            if (currentNoise >= 100f)
            {
                // Pastikan kamu sudah punya referensi ke script QTEInputLogic
                qteInputLogic.TriggerFailQTE();
            }
            if (currentNoise >= 50f)
            {
                if (!threshold50Triggered)
                {
                    threshold50Triggered = true;
                    
                    if (noiseBlackoutRoutine != null)
                    {
                        StopCoroutine(noiseBlackoutRoutine);
                    }
                    noiseBlackoutRoutine = StartCoroutine(NoiseThresholdBlackoutRoutine());
                }
            }
            else
            {
                if (threshold50Triggered)
                {
                    threshold50Triggered = false;
                }
            }
        }

        private IEnumerator NoiseThresholdBlackoutRoutine()
        {
            Debug.LogWarning("⚠️ Noise Level menyentuh 50%! Memulai blackout ruangan 3x kedipan.");

            Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            List<Light> lightsToRestore = new List<Light>();

            foreach (Light lt in allLights)
            {
                if (lt == flashlightLight && !includeFlashlightInBlackout)
                {
                    continue;
                }

                if (lt != null && lt.enabled && lt.gameObject.activeInHierarchy)
                {
                    lightsToRestore.Add(lt);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                foreach (Light lt in lightsToRestore)
                {
                    if (lt != null) lt.enabled = false;
                }
                yield return new WaitForSeconds(0.5f);

                foreach (Light lt in lightsToRestore)
                {
                    if (lt != null) lt.enabled = true;
                }

                if (i < 2)
                {
                    yield return new WaitForSeconds(0.15f);
                }
            }

            noiseBlackoutRoutine = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "SafeZone_Tangga_Full")
            {
                isInSafeZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name == "SafeZone_Tangga_Full")
            {
                isInSafeZone = false;
            }
        }

        private void UpdateNoiseUI()
        {
            if (imgNoiseBarFill != null)
            {
                imgNoiseBarFill.fillAmount = currentNoise / maxNoise;
            }

            if (tmpNoisePercentage != null)
            {
                float percentage = (currentNoise / maxNoise) * 100f;
                tmpNoisePercentage.text = Mathf.RoundToInt(percentage) + "%";
            }
        }

        // =========================================================================
        // MANAJEMEN TIMER BUBBLE TEXT (MEMUNCULKAN & MENYEMBUNYIKAN OTOMATIS)
        // =========================================================================
        public void ShowBubbleText(string message)
        {
            if (tmpBubbleText != null && hudBubbleTextGroup != null)
            {
                tmpBubbleText.text = message;
                
                // Aktifkan objek HUD_BubbleTextPlayer agar teksnya terlihat di layar
                hudBubbleTextGroup.SetActive(true); 

                if (bubbleTextRoutine != null)
                {
                    StopCoroutine(bubbleTextRoutine);
                }
                // Mulai timer untuk menyembunyikan teks kembali secara otomatis
                bubbleTextRoutine = StartCoroutine(ClearBubbleTextAfterDelay(textDisplayDuration));
            }
        }

        private IEnumerator ClearBubbleTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (hudBubbleTextGroup != null)
            {
                hudBubbleTextGroup.SetActive(false); // Sembunyikan HUD_BubbleTextPlayer kembali
            }
            bubbleTextRoutine = null;
        }

        // =========================================================================
        // MANAJEMEN KUNCI & INTERAKSI PINTU
        // =========================================================================
        public void CollectKey()
        {
            hasKey = true;
            Debug.Log("🔑 Kunci berhasil diambil!");
            ShowBubbleText("I found the key!");
        }

        public void ShowLockedDoorMessage()
        {
            ShowBubbleText("Its locked, i need to get the key");
        }

        public void CollectFlashlight()
        {
            hasFlashlight = true;

            if (carriedFlashlightObject != null)
            {
                carriedFlashlightObject.SetActive(true);
            }

            SetFlashlight(false);
            Debug.Log("Senter berhasil diambil.");
        }

        public void CollectPaper()
        {
            collectedPapers++;
            if (collectedPapers > totalPapers) collectedPapers = totalPapers;

            UpdatePaperUI();

            // Hitung sisa kertas dan tampilkan ke Bubble Text
            int papersLeft = totalPapers - collectedPapers;
            ShowBubbleText("ugh, " + papersLeft + " paper left");

            if (blackoutRoutine != null)
            {
                StopCoroutine(blackoutRoutine);
            }
            blackoutRoutine = StartCoroutine(BlackoutSequence());

            if (collectedPapers >= totalPapers)
            {
                OnAllPapersCollected();
            }
        }

        private void UpdatePaperUI()
        {
            if (tmpPaperNumber != null)
            {
                tmpPaperNumber.text = collectedPapers + "/" + totalPapers;
            }
        }

        private IEnumerator BlackoutSequence()
        {
            Light[] allLights = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude);
            List<Light> lightsToRestore = new List<Light>();

            foreach (Light lt in allLights)
            {
                if (lt == flashlightLight && !includeFlashlightInBlackout)
                {
                    continue;
                }

                if (lt != null && lt.enabled && lt.gameObject.activeInHierarchy)
                {
                    lightsToRestore.Add(lt);
                    lt.enabled = false;
                }
            }

            yield return new WaitForSeconds(blackoutDuration);

            foreach (Light lt in lightsToRestore)
            {
                if (lt != null)
                {
                    lt.enabled = true;
                }
            }

            blackoutRoutine = null;
        }

        private void OnAllPapersCollected()
        {
            Debug.Log("Semua kertas telah terkumpul!");
            
            // 1. Memunculkan teks narasi akhir pada bubble text
            ShowBubbleText("alright, lets get out of here");

            // 2. Mengubah warna LampuExit menjadi hijau horror
            if (exitLampLight != null)
            {
                exitLampLight.color = horrorGreenColor;
                exitLampLight.intensity = 120f; // Sedikit menaikkan intensitas agar efek hijaunya terasa terpancar
                exitLampLight.enabled = true;  // Memastikan lampu menyala murni
            }
        }

        public void ToggleFlashlight()
        {
            SetFlashlight(!flashlightOn);
        }

        public void SetFlashlight(bool isOn)
        {
            flashlightOn = isOn;

            if (flashlightLight != null)
            {
                flashlightLight.enabled = isOn;
            }
        }

        public void SetInputLock(bool locked)
        {
            SetInputLock(locked, null);
        }

        public void SetInputLock(bool locked, Transform lookTarget)
        {
            inputLocked = locked;

            foreach (Behaviour component in controllerComponentsToDisable)
            {
                if (component != null)
                {
                    component.enabled = !locked;
                }
            }

            if (locked && lookTarget != null)
            {
                StartCameraLock(lookTarget);
            }
            else
            {
                StopCameraLock();
            }
        }

        private void StartCameraLock(Transform lookTarget)
        {
            StopCameraLock();
            cameraLockRoutine = StartCoroutine(CameraLockRoutine(lookTarget));
        }

        private void StopCameraLock()
        {
            if (cameraLockRoutine != null)
            {
                StopCoroutine(cameraLockRoutine);
                cameraLockRoutine = null;
            }
        }

        private IEnumerator CameraLockRoutine(Transform lookTarget)
        {
            while (inputLocked && lookTarget != null)
            {
                FaceTarget(lookTarget);
                yield return null;
            }
        }

        private void FaceTarget(Transform lookTarget)
        {
            if (playerRoot == null || playerCamera == null)
            {
                return;
            }

            Vector3 flatDirection = lookTarget.position - playerRoot.position;
            flatDirection.y = 0f;

            if (flatDirection.sqrMagnitude > 0.001f)
            {
                playerRoot.rotation = Quaternion.LookRotation(flatDirection);
            }

            Vector3 cameraDirection = lookTarget.position - playerCamera.position;

            if (cameraDirection.sqrMagnitude > 0.001f)
            {
                Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection);
                float pitch = cameraRotation.eulerAngles.x;

                if (pitch > 180f)
                {
                    pitch -= 360f;
                }

                playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }
    }
}