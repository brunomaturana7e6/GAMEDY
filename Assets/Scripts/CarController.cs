using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, InputSystem_Actions.ICarActions
{
    private enum Axel
    {
        Front,
        Rear
    }
    [Serializable]
    private struct Wheel
    {
        public GameObject wheel;
        public WheelCollider collider;
        public Axel axel;
    }
    [SerializeField] private float maxAcceleration = 30;
    [SerializeField] private float brakeAcceleration = 50;
    [SerializeField] private float turnSensivility = 1;
    [SerializeField] private float maxSteerAngle = 30;
    [SerializeField] private List<Wheel> wheels;
    private Rigidbody _rb;
    private InputSystem_Actions _actions;
    private float _direction;
    private float _steer;
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
            if(wheel.axel == Axel.Front)
            {
                var _steerAngle = _steer * turnSensivility * maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, _steerAngle, 0.6f);
                wheel.collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            }
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<float>();
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        _steer = context.ReadValue<float>();
    }
}
