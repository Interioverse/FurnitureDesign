using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableUI : MonoBehaviour
{
    public List<GameObject> itemContents;
    public Image previousImage = null;

    public void TakeClickAction(GameObject itemContent)
    {
        foreach (GameObject _itemContent in itemContents)
        {
            _itemContent.SetActive(false);
        }
        itemContent.SetActive(true);
    }
}
