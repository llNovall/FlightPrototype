using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpeed : MonoBehaviour
{
    [SerializeField]
    private FlightControl _flightControl;

    [SerializeField]
    private Slider _sldSpeed;

    [SerializeField]
    private TextMeshProUGUI _txtMinSpeed, _txtMaxSpeed;

    private void Start()
    {
        SpeedData speedData = _flightControl.GetSpeedData();
        _sldSpeed.minValue = speedData.MinSpeed;
        _sldSpeed.maxValue = speedData.MaxSpeed;
        _sldSpeed.value = speedData.Speed;

        //_txtMinSpeed.text = speedData.MinSpeed.ToString();
        _txtMaxSpeed.text = speedData.MaxSpeed.ToString();
        _flightControl.OnSpeedChanged += FlightControl_OnSpeedChanged;
    }

    private void FlightControl_OnSpeedChanged(SpeedData speedData) => _sldSpeed.value = speedData.Speed;
}
