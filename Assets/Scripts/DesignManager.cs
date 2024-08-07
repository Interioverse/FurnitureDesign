//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using System;
//using System.Globalization;

//public class DesignManager : MonoBehaviour
//{
//    [SerializeField] GameObject prefab;
//    [SerializeField] Transform newFurniture;
//    [SerializeField] RuntimeTransformHandle runtimeTransformHandle;
//    public TabInputField TabInputField;
//    GameObject spawnedObject;

//    public Transform target;

//    public void SpawnCube()
//    {
//        spawnedObject = Instantiate(prefab, newFurniture) as GameObject;
//        spawnedObject.name = "Cube " + newFurniture.childCount;
//        runtimeTransformHandle.parameterPanel.SetActive(true);
//        runtimeTransformHandle.customizserPanel.SetActive(true);
//        ChangeTarget();
//    }

//    void ChangeTarget()
//    {
//        target = spawnedObject.transform;
//        runtimeTransformHandle.AssignTarget(spawnedObject.transform);
//    }

//    [SerializeField] TMP_InputField currentInputField;

//    public void SetInputFieldSelected(TMP_InputField _inputField)
//    {
//        currentInputField = _inputField;
//    }

//    public void SetPositionParameters()
//    {
//        if (currentInputField.text == null || currentInputField.text == "")
//        {
//            return;
//        }
           
//        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);

//        if (currentInputField.name == "X")
//        {
//            target.position = new Vector3(value, target.position.y, target.position.z);
//        }
//        else if (currentInputField.name == "Y")
//        {
//            target.position = new Vector3(target.position.x, value, target.position.z);
//        }
//        else if (currentInputField.name == "Z")
//        {
//            target.position = new Vector3(target.position.x, target.position.y, value);
//        }
//    }

//    public void SetRotationParameters(TMP_InputField _inputField)
//    {
//        currentInputField = _inputField;
//    }

//    public void SetScaleParameters()
//    {
//        if (currentInputField.text == null || currentInputField.text == "")
//        {
//            return;
//        }
//        //float value = float.Parse(currentInputField.text);
//        float value = Single.Parse(currentInputField.text, CultureInfo.InvariantCulture);
//        if (currentInputField.name == "X")
//        {
//            target.localScale = new Vector3(value, target.localScale.y, target.localScale.z);
//        }
//        else if (currentInputField.name == "Y")
//        {
//            target.localScale = new Vector3(target.localScale.x, value, target.localScale.z);
//        }
//        else if (currentInputField.name == "Z")
//        {
//            target.localScale = new Vector3(target.localScale.x, target.localScale.y, value);
//        }
//    }
//}
