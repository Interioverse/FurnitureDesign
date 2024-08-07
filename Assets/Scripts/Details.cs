using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Details : MonoBehaviour
{
    public TextMeshProUGUI _name, _price, _description;

    public void SetDetails(string nm, string prc, string des)
    {
        _name.text = nm;
        _price.text = prc;
        _description.text = des;
    }
}
