using System.Collections;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] bool mainDoor;
    public Transform door;
    public float moveDistance = 2f; // Distance to move along the Z-axis
    public float moveSpeed = 3f; // Speed at which to move
    //public AudioSource audioSource;
    public ProjectManager projectManager;
    public RoomManager roomManager;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    public bool open;
    private Coroutine moveCoroutine;

    //private void Start()
    //{
    //    logidIn = UniversalData.Instance.loginStatus;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Child")
        {
            //open
            CommonCode(-2, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Child")
        {
            //close
            CommonCode(2, false);
        }
    }

    public void CommonCode(float value, bool openClose)
    {
        moveDistance = value;
        initialPosition = door.position;
        targetPosition = initialPosition + new Vector3(0f, 0f, moveDistance);
        MoveTheDoor(openClose);
    }

    public void MoveTheDoor(bool openClose)
    {
        if (mainDoor)
        {
            if (UniversalData.loginStatus)
            {
                Move();
            }
            else
            {
                //Show Login screen
                ProjectManager.Instance.ControlPanels(true);
            }
        }
        else
        {
            if (UniversalData.Instance.playerSpawned)
            {
                open = openClose;
                Move();
            }
            else
            {
                ProjectManager.Instance.HandlePrivateRoomEntry();
            }
        }
    }

    public void Move()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        //audioSource.Play();
        moveCoroutine = StartCoroutine(MoveObjectCoroutine());
    }

    private IEnumerator MoveObjectCoroutine()
    {
        float t = 0f;
        Vector3 startPosition = door.position;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            door.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        //audioSource.Stop();

        if (!mainDoor)
        {
            if (open)
            {
                roomManager.leaveRoom.gameObject.SetActive(false);
            }
            else
            {
                roomManager.leaveRoom.gameObject.SetActive(true);
            }
        }
    }
}
