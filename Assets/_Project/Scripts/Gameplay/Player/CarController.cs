using System;
using Core.Input;
using UnityEngine;

namespace Gameplay.Player
{
    public class CarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private JoystickController joystick;
        //[SerializeField] private GameObject carModel;
        [SerializeField] private GameObject player;
        [SerializeField] private float speed = 5f;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed;

        [Header("Horizontal Boundaries (X-Axis)")]
        [SerializeField] private float minX = -4f;
        [SerializeField] private float maxX = 4f;
        [SerializeField] private float softBoundaryZoneX = 1.5f;

        [Header("Vertical Boundaries (Z-Axis)")]
        [SerializeField] private float minZ = -2f;
        [SerializeField] private float maxZ = 6f;
        [SerializeField] private float softBoundaryZoneZ = 1.0f;

        private void Update()
        {
            Vector2 moveDirection = joystick.MoveDirection;

            if(moveDirection != Vector2.zero)
            {
                Vector3 moveVector = new Vector3(moveDirection.x, 0, moveDirection.y);

                moveVector.x = ApplySoftBoundary(transform.localPosition.x, moveVector.x, minX, maxX, softBoundaryZoneX);
                moveVector.z = ApplySoftBoundary(transform.localPosition.z, moveVector.z, minZ, maxZ, softBoundaryZoneZ);

                transform.Translate(moveVector * (moveSpeed * Time.deltaTime), Space.World);
            }

            Vector3 clampedPos = transform.localPosition;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
            transform.localPosition = clampedPos;
            
            var targetRotation = moveDirection.x * 600f; 
            var currentRotation = transform.localEulerAngles.y;
            var smoothedRotation = ApplyRotation(currentRotation, targetRotation);
            transform.localRotation = Quaternion.Euler(0, smoothedRotation, 0);
        }

        private void LateUpdate()
        {
            player.SetActive(true);
            Vector3 moveDirection = Vector3.forward;
            player.transform.Translate(moveDirection * (speed * Time.deltaTime), Space.World);
        }

        /// <summary>
        /// Smoothly applies rotation to the car based on the target rotation derived from the joystick input.
        /// </summary>
        private static float ApplyRotation(float currentRotation, float targetRotation)
        {
            return Mathf.LerpAngle(currentRotation, targetRotation, 10f * Time.deltaTime);
        }

        /// <summary>
        /// Smoothly reduces the movement input as the object approaches the defined limits.
        /// Acts as an invisible cushion instead of a hard wall.
        /// </summary>
        private float ApplySoftBoundary(float currentPos, float desiredMove, float limitMin, float limitMax, float zoneSize)
        {
            float moveMultiplier = 1f;

            if (desiredMove > 0 && currentPos > (limitMax - zoneSize))
            {
                float distanceToEdge = limitMax - currentPos;
                moveMultiplier = Mathf.Clamp01(distanceToEdge / zoneSize);
                moveMultiplier = Mathf.SmoothStep(0f, 1f, moveMultiplier);
            }
            else if (desiredMove < 0 && currentPos < (limitMin + zoneSize))
            {
                float distanceToEdge = currentPos - limitMin;
                moveMultiplier = Mathf.Clamp01(distanceToEdge / zoneSize);
                moveMultiplier = Mathf.SmoothStep(0f, 1f, moveMultiplier);
            }

            return desiredMove * moveMultiplier;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw Horizontal Boundaries (Red = Hard limit, Yellow = Soft limit start)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(minX, 0, minZ - 2), new Vector3(minX, 0, maxZ + 2));
            Gizmos.DrawLine(new Vector3(maxX, 0, minZ - 2), new Vector3(maxX, 0, maxZ + 2));

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(minX + softBoundaryZoneX, 0, minZ), new Vector3(minX + softBoundaryZoneX, 0, maxZ));
            Gizmos.DrawLine(new Vector3(maxX - softBoundaryZoneX, 0, minZ), new Vector3(maxX - softBoundaryZoneX, 0, maxZ));

            // Draw Vertical Boundaries (Blue = Hard limit, Cyan = Soft limit start)
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(minX - 2, 0, minZ), new Vector3(maxX + 2, 0, minZ));
            Gizmos.DrawLine(new Vector3(minX - 2, 0, maxZ), new Vector3(maxX + 2, 0, maxZ));

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(minX, 0, minZ + softBoundaryZoneZ), new Vector3(maxX, 0, minZ + softBoundaryZoneZ));
            Gizmos.DrawLine(new Vector3(minX, 0, maxZ - softBoundaryZoneZ), new Vector3(maxX, 0, maxZ - softBoundaryZoneZ));
        }
#endif
    }
}


