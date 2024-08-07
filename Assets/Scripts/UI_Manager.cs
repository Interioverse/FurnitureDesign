using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{

    [Header("Payment Gateway")]

    public InputField enterAmount;
    public GameObject addMoneyView, addCoinsNofitications, addCashpanel;
    public Text coinsInfo;
    public string closeUrl,coinsBuyUrl;

   


    #region Singleton

    public static UI_Manager Instance;
    private void Awake()
    {
        Instance = this;
    }
    
   
    #endregion   


    #region Mono
    // Start is called before the first frame update
    void Start()
    {
        
    }
   

    

    // Update is called once per frame
    void Update()
    {

    }

    #endregion
    public void OpenWebview()
    {
        addMoneyView.SetActive(true);
    }

    public void CloseMoneyView()
    {
        addMoneyView.SetActive(false);
        addCoinsNofitications.SetActive(true);
        var g = GameObject.Find("WebViewObject");
        if (g != null)
        {
            Destroy(g);
        }
        Debug.Log(closeUrl);

        if (closeUrl == "https://gamessoft.co.in/dev2/paymentSuccessUrl")
        {
            coinsInfo.text = "Cash Added Successfully";

        }
        else
        {
            coinsInfo.text = "Cash not Added";
        }
    }

    public void OnSubmitButtonClick()
    {
        addCashpanel.SetActive(false);
        if (enterAmount.text == string.Empty)
        {
           // UIManager.Instance.ProcessMessageDetail("Please Enter the Amount", false);
            return;
        }
        coinsBuyUrl = "https://diceearn.com/addWalletAmount?user_id=" + PlayerPrefs.GetString("user_id") + "&amount=" + enterAmount.text;
        Debug.Log(coinsBuyUrl);
        OpenWebview();
    }
   


}
