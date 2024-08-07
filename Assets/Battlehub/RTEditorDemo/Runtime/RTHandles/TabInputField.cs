using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TabInputField : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public TMP_InputField ZinputField;
    public Toggle isTransperent;
    [SerializeField] int inputElementSelected;

    private void Start()
    {
        // Attach event listeners to each input field in the array
        foreach (var inputField in inputFields)
        {
            inputField.onSelect.AddListener(OnInputFieldSelected);
            inputField.onDeselect.AddListener(OnInputFieldDeselected);
        }
    }

    private void OnInputFieldSelected(string value)
    {
        TMP_InputField selectedInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        AppManager.Instance.inputFieldFocused = true;
    }

    private void OnInputFieldDeselected(string value)
    {
        TMP_InputField deselectedInputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
        AppManager.Instance.inputFieldFocused = false;
        //AppManager.Instance.currentInputField = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            inputElementSelected--;
            if (inputElementSelected < 0) inputElementSelected = (inputFields.Length - 1);
            SelectInputField();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            inputElementSelected++;
            if (inputElementSelected > (inputFields.Length - 1)) inputElementSelected = 0;
            SelectInputField();
        }
    }

    void SelectInputField()
    {
        inputFields[inputElementSelected].Select();
    }

    public void ManualInputFieldSelected(int selectedIFindex)
    {
        inputElementSelected = selectedIFindex;
        SelectInputField();
    }

    internal void AssignValues(float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz)
    {
        Set(px, py, pz, rx, ry, rz, sx, sy, sz);
    }

    //internal void AssignValues(float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, bool _isTransperent)
    //{
    //    Set(px, py, pz, rx, ry, rz, sx, sy, sz);
    //    isTransperent.isOn = _isTransperent;
    //    return;
    //}

    private void Set(float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz)
    {
        inputFields[0].text = px.ToString();
        inputFields[1].text = py.ToString();
        inputFields[2].text = pz.ToString();

        inputFields[3].text = rx.ToString();
        inputFields[4].text = ry.ToString();
        inputFields[5].text = rz.ToString();

        inputFields[6].text = sx.ToString();
        inputFields[7].text = sy.ToString();
        ZinputField.text = sz.ToString();
        //inputFields[8].text = sz.ToString();
    }
}
