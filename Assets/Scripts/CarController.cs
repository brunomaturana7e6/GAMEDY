using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour, InputSystem_Actions.ICarActions
{
    private enum Axel { Front, Rear }

    [Serializable]
    private struct Wheel
    {
        public GameObject wheel;
        public WheelCollider collider;
        public Axel axel;
    }

    [Header("Settings")]
    [SerializeField] private float maxAcceleration = 30;
    [SerializeField] private float turnSensivility = 1;
    [SerializeField] private float maxSteerAngle = 30;
    [SerializeField] private List<Wheel> wheels;

    private Rigidbody _rb;
    private bool _playerInside = false;
    private InputSystem_Actions _actions;
    private float _direction;
    private float _steer;
    public Animation _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _actions = new InputSystem_Actions();
        _actions.Car.SetCallbacks(this);
        _anim = GetComponent<Animation>();
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
        if (!_playerInside) return;

        foreach (Wheel wheel in wheels)
        {
            wheel.collider.motorTorque = _direction * 600 * maxAcceleration * Time.deltaTime;

            if (wheel.axel == Axel.Front)
            {
                float _steerAngle = _steer * turnSensivility * maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, _steerAngle, 0.6f);
            }
        }

        // Wheel animation logic
        if (_direction != 0)
        {
            if (!_anim.isPlaying)
                _anim.Play("wheel");
        }
        else
        {
            _anim.Stop("wheel");
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

    public void EnableDriving()
    {
        _playerInside = true;
        _actions.Enable();
    }

    public void DisableDriving()
    {
        _playerInside = false;
        _actions.Disable();
        _direction = 0f;
        _steer = 0f;
    }
}
