using System.Collections;
using UnityEngine;

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

        [Header("Input Lock")]
        [SerializeField] private bool lockFlashlightDuringQTE = true;

        private bool hasFlashlight = false;
        private bool flashlightOn = false;
        private bool inputLocked = false;

        private Coroutine cameraLockRoutine;

        public bool HasFlashlight => hasFlashlight;
        public bool IsInputLocked => inputLocked;

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

            SetFlashlight(false);
        }

        private void Update()
        {
            HandleFlashlightInput();
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
                return;
            }

            if (inputLocked && lockFlashlightDuringQTE)
            {
                return;
            }

            ToggleFlashlight();
        }

        public void CollectFlashlight()
        {
            hasFlashlight = true;

            if (carriedFlashlightObject != null)
            {
                carriedFlashlightObject.SetActive(true);
            }

            SetFlashlight(false);
            Debug.Log("Senter berhasil diambil. Tombol Z sekarang aktif.");
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

            Debug.Log(locked ? "Player input locked." : "Player input unlocked.");
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
