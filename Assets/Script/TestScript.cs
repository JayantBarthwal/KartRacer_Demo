using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TestScript : MonoBehaviour
{
    public TextMeshPro t;
    /* void Start()
     {
         // Check if the device supports gyroscopes
         if (SystemInfo.supportsGyroscope)
         {
             // Enable the gyroscope
             Input.gyro.enabled = true;
         }
         else
         {
             Debug.LogError("Gyroscope not supported on this device.");
         }
     }

     void Update()
     {
         // Check if the gyroscope is enabled
         if (Input.gyro.enabled)
         {
             // Get the gyroscope input
             Quaternion gyroRotation = Input.gyro.attitude;

             // Apply the rotation to the GameObject's transform
             transform.rotation = Quaternion.Inverse(gyroRotation);
         }
     }*/
    public float sensitivity = 2.0f;

    void Update()
    {
        // Check if the device supports accelerometers
        if (SystemInfo.supportsAccelerometer)
        {
            // Get the accelerometer input
            Vector3 acceleration = Input.acceleration;
            t.text = acceleration.ToString();
            // Apply the acceleration to the GameObject's position
            transform.Translate(acceleration.x * sensitivity, 0, acceleration.y * sensitivity);

            // Optional: Clamp the position to prevent the object from moving too far
            float clampValue = 5.0f; // Adjust as needed
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -clampValue, clampValue),
                transform.position.y,
                Mathf.Clamp(transform.position.z, -clampValue, clampValue)
            );
        }
        else
        {
            Debug.LogError("Accelerometer not supported on this device.");
        }
    }
}
