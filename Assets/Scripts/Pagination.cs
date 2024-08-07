using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    public GameObject[] gameObjectsList;
    public GameObject[] actives;
    [SerializeField] Button back, home, designers, products;
    [SerializeField] Button profile, mydesigns, cart, orders;

    private void Start()
    {
        ActivateElementAtIndex(0);
        if (this.name == "Left scroll")
        {
            profile.onClick.AddListener(() => ActivateElementAtIndex(0));
            mydesigns.onClick.AddListener(() => ActivateElementAtIndex(1));
            cart.onClick.AddListener(() => ActivateElementAtIndex(2));
            orders.onClick.AddListener(() => ActivateElementAtIndex(3));
        }
        else if (this.name == "Admin Page")
        {
            home.onClick.AddListener(() => ActivateElementAtIndex(0));
            designers.onClick.AddListener(() => ActivateElementAtIndex(1));
            products.onClick.AddListener(() => ActivateElementAtIndex(2));
            //adminActivity.onClick.AddListener(() => ActivateElementAtIndex(3));
        }
        back.onClick.AddListener(() => Back());
    }

    void Back()
    {
        UniversalData.Instance.LoadMainScene();
    }

    public void ActivateElementAtIndex(int index)
    {
        if (index >= 0 && index < gameObjectsList.Length)
        {
            for (int i = 0; i < gameObjectsList.Length; i++)
            {
                if (i == index)
                {
                    gameObjectsList[i].SetActive(true);
                    actives[i].SetActive(true);
                }
                else
                {
                    gameObjectsList[i].SetActive(false);
                    actives[i].SetActive(false);
                }
            }
        }
        else
        {
            Debug.LogWarning("Invalid index. The index must be within the bounds of the gameObjectsList.");
        }
    }
}
