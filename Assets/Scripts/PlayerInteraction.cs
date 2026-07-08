using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk Unity 6 New Input System
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float interactDistance = 5f;
    public LayerMask interactableLayer;
    public LayerMask pushableLayer; 

    [Header("Pickup Settings")]
    public float holdDistance = 2.5f; 
    public float attractionForce = 12f; 
    
    [Header("UI Reference")]
    public TextMeshProUGUI uiStatusText;

    private Renderer _lastHighlightedRenderer;
    private Color _originalColor;
    private Rigidbody _heldRigidbody;

    void Update()
    {
        HandleHighlight();
        HandleInteractionE();
    }

    void FixedUpdate()
    {
        HandleMouseHoldPhysics(); 
    }

    void HandleHighlight()
    {
        Transform camTransform = Camera.main != null ? Camera.main.transform : transform;
        Ray ray = new Ray(camTransform.position, camTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            Renderer currentRenderer = hit.collider.GetComponent<Renderer>();
            if (currentRenderer != null && currentRenderer != _lastHighlightedRenderer)
            {
                ResetHighlight();
                _lastHighlightedRenderer = currentRenderer;
                _originalColor = currentRenderer.material.color;
                currentRenderer.material.color = new Color(1f, 0.85f, 0f); 
            }
        }
        else
        {
            ResetHighlight();
        }
    }

    void ResetHighlight()
    {
        if (_lastHighlightedRenderer != null)
        {
            _lastHighlightedRenderer.material.color = _originalColor;
            _lastHighlightedRenderer = null;
        }
    }

    void HandleMouseHoldPhysics()
    {
        if (Mouse.current == null) return;

        Transform camTransform = Camera.main != null ? Camera.main.transform : transform;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = new Ray(camTransform.position, camTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, pushableLayer))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    _heldRigidbody = rb;
                    _heldRigidbody.isKinematic = false; 
                    _heldRigidbody.useGravity = false; 
                }
            }
        }

        if (Mouse.current.leftButton.isPressed && _heldRigidbody != null)
        {
            Vector3 targetHoldPosition = camTransform.position + camTransform.forward * holdDistance;
            Vector3 moveDirection = targetHoldPosition - _heldRigidbody.position;
            _heldRigidbody.linearVelocity = moveDirection * attractionForce;
            _heldRigidbody.angularVelocity = Vector3.zero;
            _heldRigidbody.rotation = Quaternion.Slerp(_heldRigidbody.rotation, camTransform.rotation, Time.fixedDeltaTime * 10f);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _heldRigidbody != null)
        {
            _heldRigidbody.useGravity = true;
            _heldRigidbody.linearVelocity = Vector3.zero;
            _heldRigidbody = null; 
        }
    }

    void HandleInteractionE()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Transform camTransform = Camera.main != null ? Camera.main.transform : transform;
            Ray ray = new Ray(camTransform.position, camTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
            {
                TextMeshProUGUI localText = hit.collider.GetComponentInChildren<TextMeshProUGUI>();
                AudioSource localAudio = hit.collider.GetComponent<AudioSource>();

                if (TriggerZoneDetector.isPlayerAuthorized)
                {
                    string objectName = hit.collider.gameObject.name;
                    
                    if (localAudio != null)
                    {
                        localAudio.Stop();
                        localAudio.Play();
                    }

                    // Menggunakan Kode Hex warna (#00FFFF) agar parsing TextMeshPro jauh lebih aman dan stabil
                    if (objectName.Contains("MonaLisa") && localText != null)
                    {
                        localText.text = "<color=#00FFFF>[AUDIO GUIDE]: Mona Lisa dirancang oleh Leonardo da Vinci pada tahun 1503.</color>";
                    }
                    else if (localText != null)
                    {
                        localText.text = "<color=#00FFFF>[AUDIO GUIDE]: Menampilkan deskripsi sejarah patung Albert.</color>";
                    }
                }
                else
                {
                    Debug.LogWarning("[AKSES DITOLAK]: Ambil gawai Audio Guide terlebih dahulu di lantai!");
                }
            }
        }
    }
}