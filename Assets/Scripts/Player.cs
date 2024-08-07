//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private Transform rayCaster;

    private Rigidbody playerRigidBody;
    private Vector3 inputValues;
    private Vector3 flyInputs;
    private Vector3 playerPosAtStart;
    private bool play = false;
    private float xAxis;
    private float yAxis;
    private float zAxis;

    public float horizontalSpeed = 1f;

    #region StartAwakeUpdate

    private void Start()
    {
        playerPosAtStart = transform.position;
        play = true;
    }

    bool joyStickInput = true;
    //public float vlcty;

    private void Update()
    {
        if (play)
        {
            if (joyStickInput)
            {
                PlayerMovement(GetJoyStickAxisValues());
                PlayerRotation(GetJoyStickAxisValues());
            }
            else
            {
                PlayerMovement(GetKeyBoardAxisValues());
                PlayerRotation(GetKeyBoardAxisValues());
            }
        }
    }

    public void JoyStickCondition(bool value)
    {
        joyStickInput = value;
    }
    #endregion

    #region GetAxisValues
    private Vector3 GetJoyStickAxisValues()
    {
        xAxis = JoyStick.input.x;
        zAxis = JoyStick.input.y;
        inputValues = new Vector3(xAxis, 0, zAxis);
        return inputValues;
    }

    private Vector3 GetJSFlyInputValues()
    {
        xAxis = JoyStick.input.x;
        yAxis = JoyStick.input.y;
        zAxis = JoyStick.input.z;
        flyInputs = new Vector3(xAxis, yAxis, zAxis);
        return flyInputs;
    }

    private Vector3 GetKeyBoardAxisValues()
    {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        inputValues = new Vector3(xAxis, yAxis, zAxis);
        return inputValues;
    }
    #endregion

    #region PlayerMovement

    private void PlayerMovement(Vector3 inputValues)
    {
        transform.position += inputValues * moveSpeed * Time.deltaTime;
    }

    private void PlayerRotation(Vector3 inputValues)
    {
        if (inputValues.magnitude > 0)
        {
            Quaternion from = transform.rotation;
            Quaternion to = Quaternion.LookRotation(inputValues);
            transform.rotation = Quaternion.Lerp(from, to, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    //public void DisablePlayerMovement()
    //{
    //    play = false;
    //}
}