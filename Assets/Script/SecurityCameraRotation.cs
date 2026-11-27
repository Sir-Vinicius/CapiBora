using UnityEngine;
using System.Collections;

public class SecurityCameraRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float endAngle = 180f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float waitTime = 2f;

    private void Start()
    {
        StartCoroutine(RotateCamera());
    }

    private IEnumerator RotateCamera()
    {
        while (true)
        {
            yield return RotateToAngle(endAngle);

            yield return new WaitForSeconds(waitTime);

            yield return RotateToAngle(startAngle);

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        while (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, targetAngle)) > 0.1f)
        {
            Vector3 rotation = transform.localEulerAngles;

            rotation.y = Mathf.MoveTowardsAngle(rotation.y, targetAngle, rotationSpeed * Time.deltaTime);

            transform.localEulerAngles = rotation;

            yield return null;
        }
    }
}