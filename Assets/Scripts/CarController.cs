using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, InputSystem_Actions.ICarActions
{
    [Serializable]
    private struct Wheel
    {
        public GameObject wheel;
        public WheelCollider collider;
    }
    [SerializeField] private float maxAcceleration = 30;
    [SerializeField] private float brakeAcceleration = 50;
    [SerializeField] private List<Wheel> wheels;
    private Rigidbody _rb;
    private InputSystem_Actions _actions;
    private float _direction;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _actions = new InputSystem_Actions();
        _actions.Car.SetCallbacks(this);
    }
    private void OnEnable()
    {
        _actions.Enable();
    }
    private void OnDisable()
    {
        _actions.Disable();
    }
    private void FixedUpdate()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.collider.motorTorque = _direction * 600 * maxAcceleration * Time.deltaTime;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<float>();
    }
}
