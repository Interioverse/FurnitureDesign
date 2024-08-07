using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;

public class RuntimeTransformHandle : MonoBehaviour
{
    public HandleAxes axes = HandleAxes.XYZ;
    public HandleSpace space = HandleSpace.LOCAL;
    public HandleType type = HandleType.POSITION;
    public HandleSnappingType snappingType = HandleSnappingType.RELATIVE;

    public Vector3 positionSnap = Vector3.zero;
    public float rotationSnap = 0;

    public Vector3 scaleSnap = Vector3.zero;

    public bool autoScale = false;
    public float autoScaleFactor = 1;
    public Camera handleCamera;

    private Vector3 _previousMousePosition;
    private HandleBase _previousAxis;

    private HandleBase _draggingHandle;

    private HandleType _previousType;
    private HandleAxes _previousAxes;

    private PositionHandle _positionHandle;
    private RotationHandle _rotationHandle;
    private ScaleHandle _scaleHandle;

    public Transform newFurniture, target, defaultTarget;
    public GameObject slectionPanel, customizserPanel, addDetailsPanel, leftScreen, copyPastePanel, materialsListPanel;

    [SerializeField] Toggle pos, rot, scl;
    [SerializeField] Product product;

    private void OnEnable()
    {
        if (target == defaultTarget)
        {
            customizserPanel.SetActive(false);
        }
        else
        {
            customizserPanel.SetActive(true);
        }
    }

    void Start()
    {
        slectionPanel.SetActive(true);
        addDetailsPanel.SetActive(false);
        copyPastePanel.SetActive(false);

        _previousType = type;

        if (target == null)
            target = transform;

        GO = target.gameObject;
        if (handleCamera == null)
            handleCamera = Camera.main;

        CreateHandles();
    }

    //public void OpenMenu()
    //{
    //    customizserPanel.SetActive(true);
    //}

    public void ChangeType(int _type)
    {
        type = (HandleType)_type;
    }

    GameObject GO;
    GetClick getClick;
    [SerializeField] TabInputField tabInputField;
    [SerializeField] TextMeshProUGUI targetName, copyPasteInstruction;
    string currentTargetName, previousTargetName;
    float px, py, pz, rx, ry, rz, sx, sy, sz;

    public void AssignTarget(Transform _transform)
    {
        currentTargetName = _transform.name;

        if (previousTargetName != currentTargetName)
        {
            targetName.text = _transform.name;
            previousTargetName = currentTargetName;

            target = _transform;
            GO = target.gameObject;

            if (currentTargetName == "New Furniture" && _transform.name.Contains("Cube"))
            {
                pos.isOn = true;
                ChangeType(0);
            }

            if (target != defaultTarget)
            {
                customizserPanel.SetActive(true);
                getClick = GO.GetComponent<GetClick>();
                if (getClick)
                {
                    //GO.GetComponent<CubeFaceSnap>().enabled = true;
                    Vector3 posValues = getClick.positionVector;
                    Vector3 rotValues = getClick.rotationVector;
                    Vector3 sclValues = getClick.scaleVector;
                    bool materialValue = getClick.isTansperent;

                    px = (float)Math.Round(posValues.x, 3);
                    py = (float)Math.Round(posValues.y, 3);
                    pz = (float)Math.Round(posValues.z, 3);

                    rx = (float)Math.Round(rotValues.x, 3);
                    ry = (float)Math.Round(rotValues.y, 3);
                    rz = (float)Math.Round(rotValues.z, 3);

                    sx = (float)Math.Round(sclValues.x, 3);
                    sy = (float)Math.Round(sclValues.y, 3);
                    sz = (float)Math.Round(sclValues.z, 3);

                    tabInputField.AssignValues(px, py, pz, rx, ry, rz, sx, sy, sz, materialValue);
                }
                else
                {
                    px = (float)Math.Round(newFurniture.localPosition.x, 3);
                    py = (float)Math.Round(newFurniture.localPosition.y, 3);
                    pz = (float)Math.Round(newFurniture.localPosition.z, 3);

                    rx = (float)Math.Round(newFurniture.localEulerAngles.x, 3);
                    ry = (float)Math.Round(newFurniture.localEulerAngles.y, 3);
                    rz = (float)Math.Round(newFurniture.localEulerAngles.z, 3);

                    sx = (float)Math.Round(newFurniture.localScale.x, 3);
                    sy = (float)Math.Round(newFurniture.localScale.y, 3);
                    sz = (float)Math.Round(newFurniture.localScale.z, 3);

                    tabInputField.AssignValues(px, py, pz, rx, ry, rz, sx, sy, sz);
                }
            }
            else
            {
                customizserPanel.SetActive(false);
            }
        }
    }

    public void DeleteThisObject()
    {
        if (target == newFurniture)
        {
            target = defaultTarget;
            GO = null;

            while (newFurniture.childCount > 0)
            {
                DestroyImmediate(newFurniture.GetChild(0).gameObject);
            }

            somethingCopied = false;
            product.ClearDetailsField();
        }

        target = defaultTarget;

        if (GO != null)
        {
            DestroyImmediate(GO);
        }

        if (newFurniture.childCount > 0)
        {
            AssignTarget(newFurniture.GetChild(newFurniture.childCount-1));
        }
        else
        {
            pos.isOn = true;
            ChangeType(0);
            customizserPanel.SetActive(false);
            AssignTarget(defaultTarget);
        }
    }

    void CreateHandles()
    {
        switch (type)
        {
            case HandleType.POSITION:
                _positionHandle = gameObject.AddComponent<PositionHandle>().Initialize(this);
                break;
            case HandleType.ROTATION:
                _rotationHandle = gameObject.AddComponent<RotationHandle>().Initialize(this);
                break;
            case HandleType.SCALE:
                _scaleHandle = gameObject.AddComponent<ScaleHandle>().Initialize(this);
                break;
        }
    }

    void Clear()
    {
        _draggingHandle = null;

        if (_positionHandle) _positionHandle.Destroy();
        if (_rotationHandle) _rotationHandle.Destroy();
        if (_scaleHandle) _scaleHandle.Destroy();
    }

    bool materialToggle;

    void Update()
    {
        if (!addDetailsPanel.activeSelf)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
            {
                pos.isOn = true;
                ChangeType(0);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                rot.isOn = true;
                ChangeType(1);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                scl.isOn = true;
                ChangeType(2);
            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
            {
                if (newFurniture.childCount > 1)
                {
                    AssignTarget(newFurniture);
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
            {
                materialToggle = !materialToggle;
                materialsListPanel.SetActive(materialToggle);
            }

            if (target != defaultTarget)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    if (target != newFurniture)
                    {
                        copiedTransform = target;
                        somethingCopied = true;
                        copyPastePanel.SetActive(true);
                        copyPasteInstruction.text = target.name + " copied";
                        Invoke("DeactivateCopyPastePanel", 1);
                    }
                    else
                    {
                        copyPastePanel.SetActive(true);
                        copyPasteInstruction.text = "You can't copy whole furniture";
                        Invoke("DeactivateCopyPastePanel", 1);
                    }
                }

                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
                {
                    if (somethingCopied)
                    {
                        somethingCopied = false;
                        copiedTransform = target;
                        copyPastePanel.SetActive(true);
                        copyPasteInstruction.text = target.name + " pasted";

                        Transform pastedObject = Instantiate(copiedTransform, newFurniture) as Transform;
                        pastedObject.name = "Cube_" + newFurniture.childCount + "_" + DateTime.Now;
                        /* Do  not delete this */
                        //if (copiedTransform == newFurniture || copiedTransform.name == "Bunch")
                        //{
                        //    pastedObject.name = "Bunch";
                        //}
                        //else
                        //{
                        //    pastedObject.name = "Cube_" + newFurniture.childCount +"_"+ DateTime.Now;
                        //}

                        ////pastedObject.transform.localPosition = new Vector3(copiedTransform.localPosition.x,
                        ////                                             (copiedTransform.localPosition.y + copiedTransform.localScale.y),
                        ////                                             copiedTransform.localPosition.z);
                        //pastedObject.transform.localPosition = new Vector3(copiedTransform.localPosition.x,
                        //                                                   copiedTransform.localPosition.y,
                        //                                                   copiedTransform.localPosition.z);

                        AssignTarget(pastedObject);
                        Invoke("DeactivateCopyPastePanel", 1);
                    }
                }

                else if (Input.GetKeyDown(KeyCode.Delete))
                {
                    DeleteThisObject();
                }
            }

            if (target != null)
            {
                if (autoScale)
                    transform.localScale =
                        Vector3.one * (Vector3.Distance(handleCamera.transform.position, transform.position) * autoScaleFactor) / 15;

                if (_previousType != type || _previousAxes != axes)
                {
                    Clear();
                    CreateHandles();
                    _previousType = type;
                    _previousAxes = axes;
                }

                HandleBase handle = null;
                Vector3 hitPoint = Vector3.zero;
                GetHandle(ref handle, ref hitPoint);

                HandleOverEffect(handle);

                if (Input.GetMouseButton(0) && _draggingHandle != null)
                {
                    _draggingHandle.Interact(_previousMousePosition);
                }

                if (Input.GetMouseButtonDown(0) && handle != null)
                {
                    _draggingHandle = handle;
                    _draggingHandle.StartInteraction(hitPoint);
                }

                if (Input.GetMouseButtonUp(0) && _draggingHandle != null)
                {
                    _draggingHandle.EndInteraction();
                    _draggingHandle = null;
                }

                _previousMousePosition = Input.mousePosition;

                transform.position = target.transform.position;
                if (space == HandleSpace.LOCAL || type == HandleType.SCALE)
                {
                    transform.rotation = target.transform.rotation;
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    void DeactivateCopyPastePanel()
    {
        copyPastePanel.SetActive(false);
    }

    Transform copiedTransform;
    bool somethingCopied;

    void HandleOverEffect(HandleBase p_axis)
    {
        if (_draggingHandle == null && _previousAxis != null && _previousAxis != p_axis)
        {
            _previousAxis.SetDefaultColor();
        }

        if (p_axis != null && _draggingHandle == null)
        {
            p_axis.SetColor(Color.yellow);
        }

        _previousAxis = p_axis;
    }

    private void GetHandle(ref HandleBase p_handle, ref Vector3 p_hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length == 0)
            return;

        foreach (RaycastHit hit in hits)
        {
            p_handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();

            if (p_handle != null)
            {
                p_hitPoint = hit.point;
                return;
            }
        }
    }

    static public RuntimeTransformHandle Create(Transform p_target, HandleType p_handleType)
    {
        RuntimeTransformHandle runtimeTransformHandle = new GameObject().AddComponent<RuntimeTransformHandle>();
        runtimeTransformHandle.target = p_target;
        runtimeTransformHandle.type = p_handleType;

        return runtimeTransformHandle;
    }

    public void ChangeAxes(Dropdown _dropDown)
    {
        axes = (HandleAxes)_dropDown.value;
    }

    public void ChangeSpace(Dropdown _dropDown)
    {
        space = (HandleSpace)_dropDown.value;
    }

    public void ChangeSnappingType(Dropdown _dropDown)
    {
        snappingType = (HandleSnappingType)_dropDown.value;
    }

    public void ChangeAutoScale(Toggle _toggle)
    {
        autoScale = _toggle.isOn;
    }

    public void ChangeAutoScaleFactor(Dropdown _dropDown)
    {
        autoScaleFactor = float.Parse(_dropDown.options[_dropDown.value].text);
    }

    [SerializeField] GameObject prefab;
    [SerializeField] TMP_InputField currentInputField;

    public void SpawnCube()
    {
        GameObject obj = Instantiate(prefab, newFurniture) as GameObject;
        obj.name = "Cube_" + newFurniture.childCount + "_" + DateTime.Now;
        customizserPanel.SetActive(true);
        AssignTarget(obj.transform);
        //UndoRedoManager.Instance.RegisterChange();
    }

    string currentValue;

    public void SetInputFieldSelected(TMP_InputField _inputField)
    {
        currentInputField = _inputField;
        currentValue = currentInputField.text;
    }

    public void SetPositionParameters()
    {
        CheckInputField();

        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

        if (currentInputField.name == "X")
        {
            GO.transform.localPosition = new Vector3(value, GO.transform.localPosition.y, GO.transform.localPosition.z);
        }
        else if (currentInputField.name == "Y")
        {
            GO.transform.localPosition = new Vector3(GO.transform.localPosition.x, value, GO.transform.localPosition.z);
        }
        else if (currentInputField.name == "Z")
        {
            GO.transform.localPosition = new Vector3(GO.transform.localPosition.x, GO.transform.localPosition.y, value);
        }
    }

    public void SetRotationParameters(TMP_InputField _inputField)
    {
        CheckInputField();

        value = float.Parse(currentInputField.text);

        value = UnwrapAngle(value);

        if (currentInputField.name == "X")
        {
            GO.transform.localEulerAngles = new Vector3(value, GO.transform.localEulerAngles.y, GO.transform.localEulerAngles.z);
        }
        else if (currentInputField.name == "Y")
        {
            GO.transform.localEulerAngles = new Vector3(GO.transform.localEulerAngles.x, value, GO.transform.localEulerAngles.z);
        }
        else if (currentInputField.name == "Z")
        {
            GO.transform.localEulerAngles = new Vector3(GO.transform.localEulerAngles.x, GO.transform.localEulerAngles.y, value);
        }
    }

    float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    float value;

    public void SetScaleParameters()
    {
        CheckInputField();

        value = float.Parse(currentInputField.text);
        //float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);
        if (currentInputField.name == "X")
        {
            GO.transform.localScale = new Vector3(value, GO.transform.localScale.y, GO.transform.localScale.z);
        }
        else if (currentInputField.name == "Y")
        {
            GO.transform.localScale = new Vector3(GO.transform.localScale.x, value, GO.transform.localScale.z);
        }
        else if (currentInputField.name == "Z")
        {
            GO.transform.localScale = new Vector3(GO.transform.localScale.x, GO.transform.localScale.y, value);
        }
    }

    [SerializeField] Material transperentMaterial, defaultMaterial;

    public void HideUnhide(Toggle value)
    {
        if (value.isOn)
        {
            GO.GetComponent<Renderer>().material = transperentMaterial;
        }
        else
        {
            GO.GetComponent<Renderer>().material = defaultMaterial;
        }
        getClick.isTansperent = value.isOn;
    }

    public void SetPositionSnap()
    {
        CheckInputField();

        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

        if (currentInputField.name == "X")
        {
            positionSnap.x = value;
        }
        else if (currentInputField.name == "Y")
        {
            positionSnap.y = value;
        }
        else if (currentInputField.name == "Z")
        {
            positionSnap.z = value;
        }
    }

    public void SetRotationSnap()
    {
        CheckInputField();

        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

        rotationSnap = value;
    }

    public void SetScaleSnap()
    {
        CheckInputField();

        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

        if (currentInputField.name == "X")
        {
            scaleSnap.x = value;
        }
        else if (currentInputField.name == "Y")
        {
            scaleSnap.y = value;
        }
        else if (currentInputField.name == "Z")
        {
            scaleSnap.z = value;
        }
    }

    void CheckInputField()
    {
        if (currentInputField.text == null || currentInputField.text == "")
        {
            currentInputField.text = currentValue;
            return;
        }
    }

    bool toggleBool = true;
    [SerializeField] TextMeshProUGUI arrowText;

    public void ShowHideCustomizer()
    {
        toggleBool = !toggleBool;
        arrowText.text = toggleBool ? ">" : "<";
        leftScreen.SetActive(toggleBool);
    }

    public void SetMaterial(Material material)
    {
        if (target.name.Contains("Cube") || target.name.Contains("face"))
        {
            target.GetComponent<Renderer>().material = material;
        }
    }

    public void LoadMainScene()
    {
        while (newFurniture.childCount > 0)
        {
            DestroyImmediate(newFurniture.GetChild(0).gameObject);
        }
        target = defaultTarget;
        UniversalData.Instance.LoadMainScene();
    }
}