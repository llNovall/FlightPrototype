using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightControl : MonoBehaviour
{
    [SerializeField]
    private float _minSpeed, _maxSpeed, _currentSpeed, _accelerationSpeed, _rollSpeed, _rollReturnSpeed, _maxRollLeft, _maxRollRight;

    [SerializeField]
    private float _pitchSpeed, _minBoostScale, _maxBoostScale, _currentBoostScale, _boostAccelerationSpeed;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _minFOWBoost, _maxFOWBoost;

    [SerializeField]
    private Quaternion _rotation;

    [SerializeField]
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = gameObject.AddComponent<PlayerInput>();
        _playerInput.actions = Resources.Load<InputActionAsset>("Controls");
        _playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        foreach (var item in _playerInput.actions)
        {
            item.Enable();
        }

        _playerInput.defaultControlScheme = "Keyboard & Mouse";
    }

    private void FixedUpdate()
    {
        UpdateAccelerationSpeed(_playerInput.actions["Accelerate"].ReadValue<float>());
        Roll(_playerInput.actions["Roll"].ReadValue<float>());
        Pitch(_playerInput.actions["Pitch"].ReadValue<float>());
        Boost(_playerInput.actions["Boost"].ReadValue<float>());
        UpdateFOW(_currentBoostScale, _maxBoostScale);
        gameObject.transform.rotation = _rotation;
        Vector3 move = gameObject.transform.forward.normalized * _currentSpeed * Time.fixedDeltaTime * _currentBoostScale;
        Move(move);

        
    }

    private void UpdateAccelerationSpeed(float increase)
    {
        if(increase > 0)
        {
            _currentSpeed = Mathf.Min(_currentSpeed + _accelerationSpeed * Time.fixedDeltaTime, _maxSpeed);
        }else if(increase < 0)
        {
            _currentSpeed = Mathf.Max(_currentSpeed - _accelerationSpeed * Time.fixedDeltaTime, _minSpeed);
        }
    }

    private void Move(Vector3 newPosition)
    {
        if (newPosition.sqrMagnitude < 0.01f)
            return;

        gameObject.transform.position += newPosition;
    }

    private void Roll(float rollInput)
    {
        Vector3 newRotation = _rotation.eulerAngles;

        if(rollInput > 0)
        {
            newRotation.z += _rollSpeed * Time.fixedDeltaTime;
            newRotation.z = newRotation.z >= 0 && newRotation.z <= _maxRollLeft + 10? Mathf.Min(newRotation.z, _maxRollLeft) : Mathf.Max(newRotation.z, _maxRollRight);
        }else if(rollInput < 0)
        {
            newRotation.z -= _rollSpeed * Time.fixedDeltaTime;
            newRotation.z = newRotation.z > _maxRollRight - 10 && newRotation.z <= 360 ? Mathf.Max(newRotation.z, _maxRollRight) : Mathf.Min(newRotation.z, _maxRollLeft) ;
        }
        else
        {
            newRotation.z = newRotation.z > 0 && newRotation.z <= 180 ? Mathf.Lerp(newRotation.z, 0, _rollReturnSpeed * Time.fixedDeltaTime) : Mathf.Lerp(newRotation.z, 359.9f, _rollReturnSpeed * Time.fixedDeltaTime);
        }

        _rotation.eulerAngles = newRotation;
    }

    private void Pitch(float pitchInput)
    {
        //if (pitchDirection == 0)
        //    return;

        Vector3 newRotation = _rotation.eulerAngles;

        Vector2 pitchScaler = Vector2.zero;
        pitchScaler.x = newRotation.z >= 0 && newRotation.z <= _maxRollLeft + 10 ? Mathf.Lerp(1, 0, Mathf.Clamp(newRotation.z, 0, _maxRollLeft) / _maxRollLeft) :
            Mathf.Lerp(1, 0, (360 - Mathf.Clamp(newRotation.z, _maxRollRight, 360)) / (360 - _maxRollRight));
        pitchScaler.y = 1 - pitchScaler.x;

        Vector2 pitch = pitchScaler * _pitchSpeed * Time.fixedDeltaTime;

        if (pitchInput > 0)
        {
            newRotation += newRotation.z >= 0 && newRotation.z <= _maxRollLeft + 10 ? new Vector3(pitch.x, pitch.y, 0) : new Vector3(pitch.x, -pitch.y, 0);

        }
        else if (pitchInput < 0)
        {

            newRotation -= newRotation.z >= 0 && newRotation.z <= _maxRollLeft + 10 ? new Vector3(pitch.x, pitch.y, 0) : new Vector3(pitch.x, -pitch.y, 0);
        }

        _rotation.eulerAngles = newRotation;
    }

    private void Boost(float boostIncrease)
    {
        if (boostIncrease > 0)
        {
            _currentBoostScale = Mathf.Min(_currentBoostScale + _boostAccelerationSpeed * Time.fixedDeltaTime, _maxBoostScale);
        }
        else if (boostIncrease <= 0)
        {
            _currentBoostScale = Mathf.Max(_currentBoostScale - _boostAccelerationSpeed * Time.fixedDeltaTime, _minBoostScale);
        }
    }

    private void UpdateFOW(float currentBoost, float maxBoost)
    {
        _camera.fieldOfView = Mathf.Lerp(_minFOWBoost, _maxFOWBoost, (currentBoost - 1) / (maxBoost -1));
    }
    
}
