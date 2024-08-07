using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] GameObject objectToControl;
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float rotationSpeed = 30.0f;
    [SerializeField] DesktopPlayerMovement desktopPlayerMovement;

    bool isUpButtonPressed = false;
    bool isDownButtonPressed = false;
    bool isRightButtonPressed = false;
    bool isLeftButtonPressed = false;

    public void UpdateTheObjectandDPM(GameObject player)
    {
        desktopPlayerMovement = player.GetComponent<DesktopPlayerMovement>();
        objectToControl = desktopPlayerMovement.gameObject;
    }

    void Update()
    {
        if (desktopPlayerMovement && objectToControl)
        {
            if (desktopPlayerMovement.enabled)
            {
                if (isUpButtonPressed)
                {
                    objectToControl.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                }

                if (isDownButtonPressed)
                {
                    objectToControl.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
                }

                if (isRightButtonPressed)
                {
                    objectToControl.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
                }

                if (isLeftButtonPressed)
                {
                    objectToControl.transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void OnUpButtonPressed()
    {
        isUpButtonPressed = true;
    }

    public void OnUpButtonReleased()
    {
        isUpButtonPressed = false;
    }

    public void OnDownButtonPressed()
    {
        isDownButtonPressed = true;
    }

    public void OnDownButtonReleased()
    {
        isDownButtonPressed = false;
    }

    public void OnRightButtonPressed()
    {
        isRightButtonPressed = true;
    }

    public void OnRightButtonReleased()
    {
        isRightButtonPressed = false;
    }

    public void OnLeftButtonPressed()
    {
        isLeftButtonPressed = true;
    }

    public void OnLeftButtonReleased()
    {
        isLeftButtonPressed = false;
    }
}
