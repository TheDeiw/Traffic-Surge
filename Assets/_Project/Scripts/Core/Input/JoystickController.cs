using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.UI;

namespace Core.Input
{
    public class JoystickController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float sensitivity = 0.05f;
        [SerializeField] private float joystickRadius = 150f;

        [Header("UI References")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;

        private bool _isDragging;
        private Vector2 _startTouchPos;

        public Vector2 MoveDirection { get; private set; }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            joystickBackground.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (Touch.activeFingers.Count > 0)
            {
                var touch = Touch.activeFingers[0].currentTouch;
                var touchPos = touch.screenPosition;

                if (touchPos.y < Screen.height / 2f)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        _isDragging = true;
                        _startTouchPos = touchPos;

                        joystickBackground.position = _startTouchPos;
                        joystickHandle.position = _startTouchPos;
                        joystickBackground.gameObject.SetActive(true);
                    }
                    if (touch.phase is UnityEngine.InputSystem.TouchPhase.Moved or UnityEngine.InputSystem.TouchPhase.Stationary && _isDragging)
                    {
                        Vector2 rawDirection = touchPos - _startTouchPos;
                        Vector2 clampedDirection = Vector2.ClampMagnitude(rawDirection, joystickRadius);
                        joystickHandle.anchoredPosition = clampedDirection;
                        Vector2 normalizedInput = clampedDirection / joystickRadius;
                        MoveDirection = normalizedInput * sensitivity;
                    }
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    ResetJoystick();
                }
            }
            else
            {
                ResetJoystick();
            }
        }

        private void ResetJoystick()
        {
            _isDragging = false;
            MoveDirection = Vector2.zero; // Зупиняємо машину
            joystickBackground.gameObject.SetActive(false); // Ховаємо UI
        }

    }
}

