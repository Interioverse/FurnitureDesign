using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TabInputField : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    //public Toggle isTransperent;
    [SerializeField] int inputElementSelected;

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

    internal void AssignValues(float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, bool _isTransperent)
    {
        Set(px, py, pz, rx, ry, rz, sx, sy, sz);
        //isTransperent.isOn = _isTransperent;
        return;
    }

    private void Set(float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz)
    {
        inputFields[7].text = px.ToString();
        inputFields[8].text = py.ToString();
        inputFields[9].text = pz.ToString();

        inputFields[10].text = rx.ToString();
        inputFields[11].text = ry.ToString();
        inputFields[12].text = rz.ToString();

        inputFields[13].text = sx.ToString();
        inputFields[14].text = sy.ToString();
        inputFields[15].text = sz.ToString();
    }
}
