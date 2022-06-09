using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSInput : MonoBehaviour
{
    private CharacterController controller;

    private void Awake()
    {
        Messenger<float>.AddListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public const float baseSpeed = 6.0f;

    public float speed = 6.0f;

    private float Zaxis;
    private float Xaxis;
    public float gravity = -9.0f;

    private void Update()
    {
        Zaxis = Input.GetAxis("Vertical") * speed;
        Xaxis = Input.GetAxis("Horizontal") * speed;
        
        Vector3 movement = new Vector3 (Xaxis, 0, Zaxis);
        movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);
        controller.Move(movement);

    }

    private void OnSpeedChanged(float value)
    {
        speed = baseSpeed * value;
    }

    private void OnDestroy()
    {
        Messenger<float>.RemoveListener(GameEvent.SPEED_CHANGED, OnSpeedChanged);
    }
}
