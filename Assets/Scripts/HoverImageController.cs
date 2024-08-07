using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HoverImageController : MonoBehaviour
{
    public GameObject hoverImage;
    Button _thisButton;
    internal string userEmail;
    internal int userFileID;
    [SerializeField] Pagination pagination;
    [SerializeField] DesignersProducts designersProducts;

    private void Start()
    {
        if (!pagination)
        {
            pagination = FindObjectOfType<Pagination>();
            designersProducts = pagination.gameObjectsList[2].GetComponent<DesignersProducts>();
        }

        _thisButton = hoverImage.GetComponent<Button>();
        _thisButton.onClick.AddListener(() => PerformThis(userFileID, userEmail));
    }

    private void PerformThis(int _userFileID, string email)
    {
        OnPointerExit();
        pagination.ActivateElementAtIndex(2);
        //designersProducts.Downloadfile(_userFileID.ToString(), email);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.GetFileData(_userFileID, ProductsData);
    }

    string productsData;

    void ProductsData(string data, UnityWebRequest.Result result)
    {
        JObject parsedResponse = JsonConvert.DeserializeObject<JObject>(data);
        JToken dataToken = parsedResponse["data"];
        if (dataToken.Type == JTokenType.Null)
        {
            productsData = "{\"Products\":[]}";
            return;
        }

        if (result == UnityWebRequest.Result.Success)
        {
            JObject dataObject = (JObject)JObject.Parse(data)["data"];
            string fileData = dataObject["file"].Value<string>();
            JObject fileObject = JObject.Parse(fileData);
            JArray productsArray = (JArray)fileObject["Products"];
            productsData = "{\"Products\":" + productsArray + "}".ToString();
            designersProducts.FetchAllFurnitures(userFileID, userEmail, productsData);
        }
        else
        {
            productsData = "{\"Products\":[]}";
        }
    }

    public void OnPointerEnter()
    {
        hoverImage.SetActive(true);
    }

    public void OnPointerExit()
    {
        hoverImage.SetActive(false);
    }
}
