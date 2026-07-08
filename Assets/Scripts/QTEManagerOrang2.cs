using System.Collections;
using UnityEngine;

public class QTEManagerOrang2 : MonoBehaviour
{
    [Header("Komponen VR yang akan dikunci")]
    [Tooltip("Tarik objek XR Origin yang berisi Move Provider ke sini")]
    public MonoBehaviour moveProvider;

    [Tooltip("Tarik objek XR Origin yang berisi Turn Provider ke sini")]
    public MonoBehaviour turnProvider;

    [Tooltip("Tarik objek Main Camera yang berisi TrackedPoseDriver ke sini")]
    public MonoBehaviour cameraDriver;

    [Header("Pengaturan Kamera")]
    public Transform mainCamera;
    public float kecepatanNengok = 5f;

    private bool sedangQTE = false;

    public void MulaiQTE(Transform targetFokus)
    {
        if (sedangQTE) return;
        sedangQTE = true;

        if (moveProvider != null) moveProvider.enabled = false;
        if (turnProvider != null) turnProvider.enabled = false;
        if (cameraDriver != null) cameraDriver.enabled = false;

        StartCoroutine(KunciKamera(targetFokus));
    }

    public void SelesaiQTE()
    {
        sedangQTE = false;

        if (moveProvider != null) moveProvider.enabled = true;
        if (turnProvider != null) turnProvider.enabled = true;
        if (cameraDriver != null) cameraDriver.enabled = true;
    }

    private IEnumerator KunciKamera(Transform target)
    {
        while (sedangQTE && target != null)
        {
            Vector3 arahTarget = target.position - mainCamera.position;
            arahTarget.y = 0;

            if (arahTarget != Vector3.zero)
            {
                Quaternion rotasiTarget = Quaternion.LookRotation(arahTarget);
                mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, rotasiTarget, Time.deltaTime * kecepatanNengok);
            }

            yield return null;
        }
    }
}