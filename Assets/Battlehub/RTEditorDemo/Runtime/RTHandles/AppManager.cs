using Battlehub.RTHandles;
using Battlehub.RTHandles.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    [SerializeField] DemoEditor demoEditor;
    public Transform newFurniture, target, defaultTarget;
    public GameObject slectionPanel, customizserPanel, addDetailsPanel, leftScreen, copyPastePanel, materialsListPanel;
    GameObject GO;
    [SerializeField] TabInputField tabInputField;
    GetTransformDetails getTransformDetails;
    public TextMeshProUGUI targetName, copyPasteInstruction;
    string currentTargetName, previousTargetName;
    float px, py, pz, rx, ry, rz, sx, sy, sz;
    public bool inputFieldFocused;
    public static bool isDesigner;
    [SerializeField] Button addDetails, buyBed;

    //public bool sideFaceSelected;

    public PositionHandle positionHandle;
    public RotationHandle rotationHandle;
    public ScaleHandle scaleHandle;
    public RectTool rectTool;

    PositionHandleModel positionHandleModel;
    RotationHandleModel rotationHandleModel;
    ScaleHandleModel scaleHandleModel;
    //[SerializeField] Product product;

    Vector3 posValues, rotValues, sclValues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        if (isDesigner)
        {
            buyBed.gameObject.SetActive(false);
            addDetails.gameObject.SetActive(true);
        }
        else
        {
            buyBed.gameObject.SetActive(true);
            addDetails.gameObject.SetActive(false);
        }

        if (!outlineManager)
        {
            outlineManager = FindObjectOfType<OutlineManager>();
        }
        slectionPanel.SetActive(true);
        addDetailsPanel.SetActive(false);
        copyPastePanel.SetActive(false);

        if (target == null)
            target = defaultTarget;

        positionHandleModel = (PositionHandleModel)positionHandle.Model;
        rotationHandleModel = (RotationHandleModel)rotationHandle.Model;
        scaleHandleModel = (ScaleHandleModel)scaleHandle.Model;
    }

    Transform copiedTransform;
    bool somethingCopied;

    void Update()
    {
        if (!addDetailsPanel.activeSelf)
        {
            //if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetKeyDown(KeyCode.A))
            //{
            //    if (newFurniture.childCount > 1)
            //    {
            //        AssignDetails(newFurniture);
            //    }
            //}

            if (target != defaultTarget && !inputFieldFocused)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    copiedTransform = target;
                    somethingCopied = true;
                    CopyPaste(copiedTransform.name + " copied");
                    Invoke("DeactivateCopyPastePanel", 1);
                }

                else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
                {
                    if (somethingCopied)
                    {
                        somethingCopied = false;
                        copiedTransform = target;
                        CopyPaste(copiedTransform.name + " pasted");

                        Transform pastedObject = Instantiate(copiedTransform, newFurniture) as Transform;
                        //pastedObject.position = Vector3.zero;
                        pastedObject.name = "Cube_" + UnityEngine.Random.Range(1, 9999999);

                        AssignDetails(pastedObject);
                        demoEditor.SelectedObject(pastedObject.gameObject);
                        Invoke("DeactivateCopyPastePanel", 1);
                    }
                }
            }
        }
    }

    internal void CopyPaste(string value)
    {
        copyPastePanel.SetActive(true);
        copyPasteInstruction.text = value;
    }

    public void ApplySunmice()
    {
        if (target.localEulerAngles != Vector3.zero)
        {
            Vector3 targetRotationValues = target.localEulerAngles;
            target.localEulerAngles = Vector3.zero;
            target.GetComponent<CubeFacePlanes>().ApplySunmica();
            target.localEulerAngles = targetRotationValues;
        }
        else
        {
            target.GetComponent<CubeFacePlanes>().ApplySunmica();
        }
    }

    public void RemoveSunmice()
    {
        target.GetComponent<CubeFacePlanes>().RemoveSunmice();
    }

    internal void DeactivateCopyPastePanel()
    {
        copyPastePanel.SetActive(false);
    }

    Vector3 targetParentScale, targetScale;
    bool isTopBottom, isRightLeft;
    [SerializeField] Material defaultMaterial;

    /* This will not delete the sunmice but set it's material to default value */
    public void DeleteSunmica()
    {
        SetMaterial(defaultMaterial);
    }

    public void SetMaterial(Material material)
    {
        isTopBottom = isRightLeft = false;
        Material materialInstance = new Material(material);
        if (target.name.Contains("Face"))
        {
            /* //Previous code - Do not delete this
            targetParentScale = target.parent.localScale;
            if (target.name.Contains("Front") || target.name.Contains("Back") || target.name.Contains("Left") || target.name.Contains("Right"))
            {
                List<GameObject> sideFaces = target.parent.GetComponent<CubeFacePlanes>().sideFaces;
                ApplyToAllSideFaces(materialInstance, sideFaces);
            }
            else
            {
                target.GetComponent<Renderer>().material = materialInstance;
                TextureAdjust(materialInstance, targetParentScale, true, false);
            }*/

            targetParentScale = target.parent.localScale;
            targetScale = target.localScale;
            target.GetComponent<Renderer>().material = materialInstance;
            if (target.name.Contains("Top") || target.name.Contains("Bottom"))
            {
                isTopBottom = true;
            }
            else if (target.name.Contains("Left") || target.name.Contains("Right"))
            {
                isRightLeft = true;
            }
            /* Never delete this line*/
            //TextureAdjust(materialInstance, targetScale, targetParentScale, isTopBottom, isRightLeft);
        }
    }

    [SerializeField] GameObject loadingScene, designScene;
    [SerializeField] OutlineManager outlineManager;

    internal void LoadShowRoomScene()
    {
        //SceneManager.LoadScene(0);
        outlineManager.Moved = true;
        designScene.SetActive(false);
        loadingScene.SetActive(true);
    }

    /* Never delete this line of codes*/
    //private void TextureAdjust(Material materialInstance, Vector3 targetScale, Vector3 targetParentScale, bool isTopBottom, bool isRightLeft)
    //{
    //    materialInstance.mainTextureScale = new Vector2(targetScale.x + targetParentScale.x, targetScale.y + targetParentScale.y);
    //    if (isTopBottom)
    //    {
    //        materialInstance.mainTextureScale = new Vector2(materialInstance.mainTextureScale.x, (targetParentScale.z * 10));
    //    }
    //    else if (isRightLeft)
    //    {
    //        materialInstance.mainTextureScale = new Vector2((targetParentScale.z * 10), materialInstance.mainTextureScale.y);
    //    }
    //}


    /* // Previous code - Do not delete this
    void ApplyToAllSideFaces(Material material, List<GameObject> _sideFaces)
    {
        foreach (GameObject face in _sideFaces)
        {
            bool isRightLeft = false;
            Material sideFaceMaterialInstance = new Material(material);
            face.GetComponent<Renderer>().material = sideFaceMaterialInstance;
            if (face.name.Contains("Left") || face.name.Contains("Right"))
            {
                isRightLeft = true;
            }
            TextureAdjust(sideFaceMaterialInstance, targetParentScale, false, isRightLeft);
        }
    }*/

    public void PanelControl(bool value)
    {
        if (value)
        {
            AssignDetails(target);
        }
        else
        {
            customizserPanel.SetActive(value);
            materialsListPanel.SetActive(value);
            //sideFaceSelected = false;
        }
    }

    void AssignDetails(Transform _transform)
    {
        if (_transform.name.Contains("Face"))
        {
            //customizserPanel.SetActive(false);
            positionHandleModel.gameObject.SetActive(false);
            rotationHandleModel.gameObject.SetActive(false);
            scaleHandleModel.gameObject.SetActive(false);
            ControlButtons(false);
            //if (!_transform.name.Contains("Top"))
            //{
            //    sideFaceSelected = true;
            //}
            //else
            //{
            //    sideFaceSelected = false;
            //}
        }
        else
        {
            //sideFaceSelected = false;
            ControlButtons(true);
        }

        currentTargetName = _transform.name;

        if (previousTargetName != currentTargetName)
        {
            targetName.text = _transform.name;
            previousTargetName = currentTargetName;

            target = _transform;
            GO = target.gameObject;

            if (target != defaultTarget)
            {
                getTransformDetails = target.GetComponent<GetTransformDetails>();
                if (getTransformDetails)
                {
                    posValues = getTransformDetails.positionVector;
                    rotValues = getTransformDetails.rotationVector;
                    //_ZScale.value = getTransformDetails.zIndexValue;
                    //string tempZ = _ZScale.options[getTransformDetails.zIndexValue].text;
                    //float tempFZ = float.Parse(tempZ) / 1000;
                    //sclValues = new Vector3(getTransformDetails.scaleVector.x, getTransformDetails.scaleVector.y, tempFZ);
                    sclValues = getTransformDetails.scaleVector;
                    

                    AssignTransformValues(posValues, rotValues, sclValues);
                }
            }
        }
    }

    public void ResetTargetTransform()
    {
        target.localPosition = new Vector3(0, (target.localScale.y / 2), 0);
    }

    private void ControlButtons(bool value)
    {
        demoEditor.m_positionToggle.interactable = value;
        demoEditor.m_rotationToggle.interactable = value;
        demoEditor.m_scaleToggle.interactable = value;
        demoEditor.m_rectToggle.interactable = value;
        demoEditor.m_viewToggle.interactable = value;
        demoEditor.m_resetButton.interactable = value;
        rectTool.enabled = value;

        customizserPanel.SetActive(value);
        materialsListPanel.SetActive(!value);
    }

    private void AssignTransformValues(Vector3 posValues, Vector3 rotValues, Vector3 sclValues)
    {
        px = (float)Math.Round(posValues.x, 3);
        py = (float)Math.Round(posValues.y, 3);
        pz = (float)Math.Round(posValues.z, 3);

        rx = (float)Math.Round(rotValues.x, 3);
        ry = (float)Math.Round(rotValues.y, 3);
        rz = (float)Math.Round(rotValues.z, 3);

        sx = (float)Math.Round(sclValues.x, 3) * 1000;
        sy = (float)Math.Round(sclValues.y, 3) * 1000;
        sz = (float)Math.Round(sclValues.z, 3) * 1000;

        //sz = sclValues.z * 1000;
        //sz = zValue * 1000;

        tabInputField.AssignValues(px, py, pz, rx, ry, rz, sx, sy, sz);
    }

    public void ObserveChanges(Vector3 positionVector, Vector3 rotationVector, Vector3 scaleVector)
    {
        AssignTransformValues(positionVector, rotationVector, scaleVector);
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
            if (value < 0)
            {
                value = 0.5f;
                currentInputField.text = value.ToString();
            }
            GO.transform.localPosition = new Vector3(GO.transform.localPosition.x, value, GO.transform.localPosition.z);
        }
        else if (currentInputField.name == "Z")
        {
            GO.transform.localPosition = new Vector3(GO.transform.localPosition.x, GO.transform.localPosition.y, value);
        }

        getTransformDetails.SetValues();
    }

    public void SetRotationParameters()
    {
        CheckInputField();
        //else if (float.TryParse(currentInputField.text, out value))
        //{
        //    //Debug.Log("String is the number: " + value);
        //}

        value = float.Parse(currentInputField.text);
        if (value >= 180)
        {
            value = value - 360;
            currentInputField.text = value.ToString();
        }
        else if (value <= -180)
        {
            value = 360 + value;
            currentInputField.text = value.ToString();
        }
        //float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

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
        getTransformDetails.SetValues();
    }

    float value;
    [SerializeField] TMP_InputField currentInputField;

    float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    [SerializeField] TMP_Dropdown _ZScale;
    [SerializeField] TMP_InputField X,Y,Z;
    public float zValue = 0.006f;

    public void IncreaseOrDecreaseX(int factor)
    {
        SetInputFieldSelected(X);
        //currentInputField = X;
        value = (float.Parse(currentInputField.text)) + factor;
        if (value > 2400)
        {
            value = 2400;
        }
        else if (value < 20)
        {
            value = 20;
        }
        currentInputField.text = value.ToString();
        value = value / 1000;
        GO.transform.localScale = new Vector3(value, GO.transform.localScale.y, GO.transform.localScale.z);
    }

    public void IncreaseOrDecreaseY(int factor)
    {
        SetInputFieldSelected(Y);
        //currentInputField = Y;
        value = (float.Parse(currentInputField.text)) + factor;
        if (value > 2400)
        {
            value = 2400;
        }
        else if (value < 20)
        {
            value = 20;
        }
        currentInputField.text = value.ToString();
        value = value / 1000;
        GO.transform.localScale = new Vector3(GO.transform.localScale.x, value, GO.transform.localScale.z);
    }

    public void IncreaseZ()
    {
        SetInputFieldSelected(Z);
        getTransformDetails.zIndexValue += 1;
        if (getTransformDetails.zIndexValue > 3)
        {
            getTransformDetails.zIndexValue = 3;
            return;
        }
        IncreaseOrDecreaseZ(getTransformDetails.zIndexValue);
    }

    public void DecreaseZ()
    {
        SetInputFieldSelected(Z);
        getTransformDetails.zIndexValue -= 1;
        if (getTransformDetails.zIndexValue < 0)
        {
            getTransformDetails.zIndexValue = 0;
            return;
        }
        IncreaseOrDecreaseZ(getTransformDetails.zIndexValue);
    }

    void IncreaseOrDecreaseZ(int factor)
    {
        currentInputField.text = _ZScale.options[factor].text;
        zValue = float.Parse(currentInputField.text) / 1000;
        getTransformDetails.zVectorValue = zValue;
        GO.transform.localScale = new Vector3(GO.transform.localScale.x, GO.transform.localScale.y, zValue);
    }

    //public float ZValue(int inValue)
    //{
    //    zValue = float.Parse(_ZScale.options[inValue].text) / 1000;
    //    return zValue;
    //}

    //public void SetZScale()
    //{
    //    Z.text = _ZScale.options[_ZScale.value].text;
    //    zValue = float.Parse(Z.text) / 1000;
    //    GO.transform.localScale = new Vector3(GO.transform.localScale.x, GO.transform.localScale.y, zValue);
    //    getTransformDetails.zIndexValue = _ZScale.value;
    //    getTransformDetails.SetValues();
    //}

    public void SetScaleParameters()
    {
        CheckInputField();

        value = float.Parse(currentInputField.text);
        if (value < 20)
        {
            value = 20;
            currentInputField.text = value.ToString();
        }
        else if (value > 2400)
        {
            value = 2400;
            currentInputField.text = value.ToString();
        }
        value = value / 1000;
        //float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);
        if (currentInputField.name == "X")
        {
            GO.transform.localScale = new Vector3(value, GO.transform.localScale.y, GO.transform.localScale.z);
        }
        else if (currentInputField.name == "Y")
        {
            GO.transform.localScale = new Vector3(GO.transform.localScale.x, value, GO.transform.localScale.z);
        }
        //else if (currentInputField.name == "Z")
        //{
        //    GO.transform.localScale = new Vector3(GO.transform.localScale.x, GO.transform.localScale.y, value);
        //}

        getTransformDetails.SetValues();
    }
    string currentValue;
    void CheckInputField()
    {
        if (currentInputField.text == null || currentInputField.text == "")
        {
            currentInputField.text = currentValue;
            return;
        }
    }

    public void SetInputFieldSelected(TMP_InputField _inputField)
    {
        currentInputField = _inputField;
        currentValue = currentInputField.text;
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
            //product.ClearDetailsField();
        }

        target = defaultTarget;

        if (GO != null)
        {
            DestroyImmediate(GO);
        }
    }
}
