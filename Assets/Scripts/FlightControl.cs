using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FlightControl : MonoBehaviour
{
    [SerializeField]
    private SpeedData _speedData;

    [SerializeField]
    private BoostData _boostData;

    [SerializeField]
    private RollData _rollData;

    [SerializeField]
    private PitchData _pitchData;

    [SerializeField]
    private GravityPullData _gravityPullData;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _minFOWBoost, _maxFOWBoost;

    [SerializeField]
    private Quaternion _rotation;

    [SerializeField]
    private PlayerInput _playerInput;

    public event UnityAction<SpeedData> OnSpeedChanged;
    public event UnityAction<BoostData> OnBoostChanged;

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
        UpdateFOW(_boostData);
        gameObject.transform.rotation = _rotation;
        Vector3 move = gameObject.transform.forward.normalized * _speedData.Speed * Time.fixedDeltaTime * _boostData.BoostScale;
        move += GetGravityPull();
        Move(move);
    }

    #region Getters
    public SpeedData GetSpeedData() => _speedData;
    public BoostData GetBoostData() => _boostData;
    public RollData GetRollData() => _rollData;
    public PitchData GetPitchData() => _pitchData;
    public GravityPullData GetGravityPullData() => _gravityPullData;

    #endregion
    private Vector3 GetGravityPull()
    {
        if(_speedData.Speed >= 0)
        {
            float currentGravityPull = Mathf.Lerp(_gravityPullData.MaxGravityPull, _gravityPullData.MinGravityPull, Mathf.Min(_speedData.Speed / _gravityPullData.MinSpeedRequiredToRemoveGravityPull,1) + (_boostData.BoostScale - 1) / (_boostData.MaxBoostScale - 1));

            return Vector3.down * currentGravityPull * Time.fixedDeltaTime;
        }

        return Vector3.zero;
    }
    private void UpdateAccelerationSpeed(float accelerationInput)
    {
        if (accelerationInput == 0)
            return;

        if(accelerationInput > 0)
        {
            _speedData.Speed = Mathf.Min(_speedData.Speed + _speedData.AccelerationSpeed * Time.fixedDeltaTime, _speedData.MaxSpeed);
        }else if(accelerationInput < 0)
        {
            _speedData.Speed = Mathf.Max(_speedData.Speed - _speedData.AccelerationSpeed * Time.fixedDeltaTime, _speedData.MinSpeed);
        }

        OnSpeedChanged?.Invoke(_speedData);
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
            newRotation.z += _rollData.RollSpeed * Time.fixedDeltaTime;
            newRotation.z = newRotation.z >= 0 && newRotation.z <= _rollData.MaxRollLeft + 10? Mathf.Min(newRotation.z, _rollData.MaxRollLeft) : Mathf.Max(newRotation.z, _rollData.MaxRollRight);
        }else if(rollInput < 0)
        {
            newRotation.z -= _rollData.RollSpeed * Time.fixedDeltaTime;
            newRotation.z = newRotation.z > _rollData.MaxRollRight - 10 && newRotation.z <= 360 ? Mathf.Max(newRotation.z, _rollData.MaxRollRight) : Mathf.Min(newRotation.z, _rollData.MaxRollLeft) ;
        }
        else
        {
            newRotation.z = newRotation.z > 0 && newRotation.z <= 180 ? Mathf.Lerp(newRotation.z, 0, _rollData.RollReturnSpeed * Time.fixedDeltaTime) : Mathf.Lerp(newRotation.z, 359.9f, _rollData.RollReturnSpeed * Time.fixedDeltaTime);
        }

        _rotation.eulerAngles = newRotation;
    }

    private void Pitch(float pitchInput)
    {
        Vector3 newRotation = _rotation.eulerAngles;

        Vector2 pitchScaler = Vector2.zero;
        pitchScaler.x = newRotation.z >= 0 && newRotation.z <= _rollData.MaxRollLeft + 10 ? Mathf.Lerp(1, 0, Mathf.Clamp(newRotation.z, 0, _rollData.MaxRollLeft) / _rollData.MaxRollLeft) :
            Mathf.Lerp(1, 0, (360 - Mathf.Clamp(newRotation.z, _rollData.MaxRollRight, 360)) / (360 - _rollData.MaxRollRight));
        pitchScaler.y = 1 - pitchScaler.x;

        Vector2 pitch = pitchScaler * _pitchData.PitchSpeed * Time.fixedDeltaTime;

        if (pitchInput > 0)
        {
            newRotation += newRotation.z >= 0 && newRotation.z <= _rollData.MaxRollLeft + 10 ? new Vector3(pitch.x, pitch.y, 0) : new Vector3(pitch.x, -pitch.y, 0);
        }
        else if (pitchInput < 0)
        {
            newRotation -= newRotation.z >= 0 && newRotation.z <= _rollData.MaxRollLeft + 10 ? new Vector3(pitch.x, pitch.y, 0) : new Vector3(pitch.x, -pitch.y, 0);
        }

        _rotation.eulerAngles = newRotation;
    }

    private void Boost(float boostInput)
    {
        if (boostInput > 0)
        {
            _boostData.BoostScale = Mathf.Min(_boostData.BoostScale + _boostData.BoostAccelerationSpeed * Time.fixedDeltaTime, _boostData.MaxBoostScale);
            _speedData.Speed = _speedData.MaxSpeed;
            OnBoostChanged?.Invoke(_boostData);
        }
        else if (boostInput <= 0)
        {
            _boostData.BoostScale = Mathf.Max(_boostData.BoostScale - _boostData.BoostAccelerationSpeed * Time.fixedDeltaTime, _boostData.MinBoostScale);
            OnBoostChanged?.Invoke(_boostData);
        }
    }

    private void UpdateFOW(BoostData boostData)
    {
        _camera.fieldOfView = Mathf.Lerp(_minFOWBoost, _maxFOWBoost, (boostData.BoostScale - 1) / (boostData.MaxBoostScale -1));
    }
    
}
