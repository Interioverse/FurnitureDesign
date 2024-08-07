using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FurnitureDetails : MonoBehaviour
{
    public TextMeshProUGUI _name, _designer, _description, _category, _price, _status;
    public Button EditBed, ViewIn360, AddToCart, BuyBed;
    public Button pending, approved, delete;
    public Image currentStatus;
    public TMP_InputField approvedName, approvedPrice;
    public GameObject cartAdded, alreadyInCart;

    //private void Start()
    //{
    //    if (UniversalData.Instance.loadingScene.activeSelf)
    //    {
    //        EditBed.onClick.AddListener(ShowHide);
    //        ViewIn360.onClick.AddListener(ShowHide);
    //        BuyBed.onClick.AddListener(ShowHide);
    //    }
    //}

    public void ShowHide()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }
}