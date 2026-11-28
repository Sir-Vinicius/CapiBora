using System;
using System.ComponentModel.Design.Serialization;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class SecurityCameraVision : MonoBehaviour
{
    //Events
    public event Action<Transform> OnPlayerDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerDetected?.Invoke(other.transform);
        }
    }
}
