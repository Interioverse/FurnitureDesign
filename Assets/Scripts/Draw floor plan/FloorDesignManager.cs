using UnityEngine;
using UnityEngine.UI;

public class FloorDesignManager : MonoBehaviour
{
    public static event System.Action OnMKeyPressed;
    public static event System.Action OnDKeyPressed;
    public static event System.Action OnEscapeKeyPressed;

    [SerializeField] GameObject targetGameObject;

    Camera mainCamera;
    bool isDrawMode = false;
    Vector3 originalPosition;
    Quaternion originalRotation;

    [SerializeField] Button move, draw, pan;

    void Start()
    {
        move.onClick.AddListener(MoveToolPressed);
        draw.onClick.AddListener(EnableDrawMode);
        mainCamera = Camera.main;
        StoreOriginalTransform();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            EnableDrawMode();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableDrawMode();
        }
    }

    void MoveToolPressed()
    {
        OnMKeyPressed?.Invoke();
    }

    void StoreOriginalTransform()
    {
        originalPosition = mainCamera.transform.position;
        originalRotation = mainCamera.transform.rotation;
    }

    void EnableDrawMode()
    {
        StoreOriginalTransform();
        mainCamera.orthographic = true;
        mainCamera.transform.position = new Vector3(targetGameObject.transform.position.x, 10f, targetGameObject.transform.position.z);
        mainCamera.transform.LookAt(targetGameObject.transform);
        OnDKeyPressed?.Invoke();
    }

    void DisableDrawMode()
    {
        mainCamera.orthographic = false;
        mainCamera.transform.position = originalPosition;
        mainCamera.transform.rotation = originalRotation;
        OnEscapeKeyPressed?.Invoke();
    }
}
