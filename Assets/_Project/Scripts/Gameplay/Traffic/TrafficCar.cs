using System;
using UnityEngine;
using Core.GameManagment;

namespace Gameplay.Traffic
{
    public enum CarBehavior
    {
        Standard,
        LaneChange,
        Parking
    }
    public class TrafficCar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject brakeLights;

        [SerializeField] private LayerMask obstacleLayer;

        private float _maxSpeed;
        private float _currentSpeed;
        private CarBehavior _behavior;

        private float _raycastTimer;
        private const float RaycastInterval = 0.1f;

        private bool _isChangingLane;
        private float _targetLaneX;

        public void Init(Vector3 startPos, bool isOncoming, float moveSpeed, CarBehavior carBehavior)
        {
            transform.position = startPos;
            _maxSpeed = moveSpeed;
            _behavior = carBehavior;
            _currentSpeed = _maxSpeed;
            _isChangingLane = false;
            brakeLights.SetActive(false);

            if (isOncoming)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void Update()
        {
            CheckObstacle();
            HandleBehavior();
            Move();
            CheckDespawn();
        }

        private void CheckObstacle()
        {
            // Perform a raycast in front of the car to detect obstacles every RaycastInterval seconds
            _raycastTimer -= Time.deltaTime;
            if (_raycastTimer > 0) return;
            _raycastTimer = RaycastInterval;

           Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
           if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, 10f, obstacleLayer))
           {
               // Obstacle detected
               brakeLights.SetActive(true);
               _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * 10f);
           }
           else
           {
               // No obstacle detected
               brakeLights.SetActive(false);
               _currentSpeed = Mathf.Lerp(_currentSpeed, _maxSpeed, Time.deltaTime * 1f);
           }

        }

        private void HandleBehavior()
        {
            switch (_behavior)
            {
                case CarBehavior.Standard:
                    break;

                case CarBehavior.LaneChange:
                    if (!_isChangingLane)
                    {
                        float distanceToPlayer = Mathf.Abs(transform.position.z - Score.CurrentCount);

                        if (distanceToPlayer < 30f)
                        {
                            _isChangingLane = true;

                            // Determine the target lane based on the current position
                            if (transform.position.x < 0)
                            {
                                _targetLaneX = transform.position.x < -2.5 ? -1.7f : -4f;
                            }
                            else
                            {
                                _targetLaneX = transform.position.x > 2.5 ? 1.7f : 4f;
                            }
                        }
                    }
                    break;

                case CarBehavior.Parking:
                    _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * 1f);
                    brakeLights.SetActive(true);

                    if (_currentSpeed < 3f && _currentSpeed > 0.5f)
                    {
                        transform.Translate(Vector3.right * (Time.deltaTime * 0.5f));
                    }
                    break;
            }
        }

        private void Move()
        {
            if (_isChangingLane)
            {
                float newX = Mathf.MoveTowards(transform.position.x, _targetLaneX, Time.deltaTime * 3f);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                //Rotate the car slightly towards the target lane for a more natural effect
                float targetRotationY = _targetLaneX < transform.position.x ? 15f : -15f;
                transform.Rotate(0, targetRotationY, 0, Space.Self);
            }

            Vector3 moveDirection = transform.forward;
            transform.Translate(moveDirection * (_currentSpeed * Time.deltaTime), Space.World);
        }

        private void CheckDespawn()
        {
            if (transform.position.z < Score.CurrentCount - 10f)
            {
                TrafficManager.Instance.ReturnCarToPool(this.gameObject);
            }
        }
    }
}

