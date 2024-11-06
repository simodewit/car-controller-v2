using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMeters : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The speedometer script on the car")]
    [SerializeField] private SpeedometerV2 speedometer;
    [Tooltip("The engine script on the car")]
    [SerializeField] private EngineV2 engine;
    [Tooltip("The gearbox script on the car")]
    [SerializeField] private GearboxV2 gearbox;
    
    [Header("UI Elements")]
    [Tooltip("The text element for the speed")]
    [SerializeField] private TextMeshProUGUI speedometerUI;
    [Tooltip("The text element for the rpm")]
    [SerializeField] private TextMeshProUGUI rpmUI;
    [Tooltip("The text element for the gear")]
    [SerializeField] private TextMeshProUGUI gearUI;

    private void Start()
    {
        speedometerUI.SetText("-");
        rpmUI.SetText("-");
        gearUI.SetText("-");
    }

    private void Update()
    {
        UI();
    }

    private void UI()
    {
        string speed = speedometer.speed.ToString();
        speedometerUI.SetText(speed);
        
        string rpm = engine.rpm.ToString();
        rpmUI.SetText(rpm);
        
        string gear = gearbox.currentGear.ToString();
        gearUI.SetText(gear);
    }
}
