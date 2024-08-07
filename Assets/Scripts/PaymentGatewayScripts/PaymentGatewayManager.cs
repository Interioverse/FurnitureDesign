using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public class PaymentGatewayManager : MonoBehaviour
{
    public static PaymentGatewayManager instance;
    public Text balanceValue;
    public Text coinsInfo;
    public string closeUrl, coinsBuyUrl;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Get_WalletData();
    }
    public enum Post_Detail
    {
        paymentLink
    }
    public enum Get_Detail
    {
        getwalletdetails
    }

    public void ClosePaymentPage()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

    public void CloseMoneyView()
    {
        print(URlForpayments + " urldefine");
        var g = GameObject.Find("WebViewObject");
        coinsInfo.text = "Cash not Added";
        if (g != null)
        {
            Destroy(g);
        }
        var b = FindObjectOfType<SampleWebView>().gameObject;
        if (b != null) { Destroy(b); }

        closeUrl = "https://backend.interioverse.io/success/1";
        Debug.Log("close url here" + closeUrl);
        StartCoroutine(GetMethodPaymentdetail(closeUrl));
    }

    #region Wallet
    public TMP_InputField totalPayableAmount;

    public void OnSubmitClick()
    {
        if (totalPayableAmount.text == string.Empty)
        {
            UIManager.instance.AccessErrorMessage("Enter the amount");
            return;
        }
        WWWForm paymentdata = new WWWForm();
        paymentdata.AddField("key", "2PBP7IABZ2");
        paymentdata.AddField("txnid", "T2QE");
        paymentdata.AddField("amount", totalPayableAmount.text);
        paymentdata.AddField("email", "admin@yopmail.com");
        paymentdata.AddField("phone", "9493332332");
        paymentdata.AddField("name", "Hahhh");
        paymentdata.AddField("udf1", "");
        paymentdata.AddField("udf2", "");
        paymentdata.AddField("udf3", "");
        paymentdata.AddField("udf4", "");
        paymentdata.AddField("udf5", "");
        paymentdata.AddField("productinfo", "Wallet");
        paymentdata.AddField("udf6", "");
        paymentdata.AddField("udf7", "");
        paymentdata.AddField("udf8", "");
        paymentdata.AddField("udf9", "");
        paymentdata.AddField("udf10", "");
        paymentdata.AddField("userId", 1);
        coinsBuyUrl = BaseUrlScript.paymentgateway;
        Debug.Log("U"+ coinsBuyUrl);
        StartCoroutine(PostMethod(paymentdata, coinsBuyUrl, Post_Detail.paymentLink));

    }
    #endregion
    #region GetFunction
    public void Get_WalletData()
    {
        string url = BaseUrlScript.getwllet+"1";
        Debug.Log("P" + url);
        StartCoroutine(GetMethod(url, Get_Detail.getwalletdetails));
    }
    #endregion
    #region API_Get

    IEnumerator GetMethod(string url, Get_Detail caller)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                if (webRequest.responseCode == (long)System.Net.HttpStatusCode.Unauthorized)
                {
                }
                Debug.Log("Status" + webRequest.downloadHandler.text);
                Debug.Log(webRequest.error);

            }
            else
            {
                switch (caller)
                {
                    case Get_Detail.getwalletdetails:
                        PaymentWalletData(webRequest.downloadHandler.text);
                        break;
                }
            }
        }
    }


    #endregion
    [Space(5f)]
    [Header("status Response")]
    public string StatusResponse = "200";
    string payementgatewayUrl;
    public string URlForpayments = "";

    #region APIJUNCTION

    IEnumerator PostMethod(WWWForm form, string URL, Post_Detail postcall)
    {

        using (UnityWebRequest wwr = UnityWebRequest.Post(URL, form))
        {
            wwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            wwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return wwr.SendWebRequest();

            Debug.Log(postcall + " data fetch" + wwr.downloadHandler.text);
            if (wwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(wwr.error);//
            }
            else
            {
                Debug.Log(wwr.downloadHandler.text + " Form upload complete!");
                Debug.Log(postcall + "Player Status" + wwr.downloadHandler.text);
                string msg = wwr.downloadHandler.text;

                switch (postcall)
                {
                    
                    case Post_Detail.paymentLink:
                        PaymentData(wwr.downloadHandler.text);
                        break;
                }
            }
        }
    }

    #endregion
    #region FunctionData
    public void PaymentData(string data)
    {
        Debug.Log(data + " Form upload complete!");
        Debug.Log("Player Status" + data);
        string msg = data;
        payementgatewayUrl = data;
        Debug.Log("Payment" + payementgatewayUrl);
        PostalPostData filedata = new PostalPostData();
        filedata = JsonUtility.FromJson<PostalPostData>(msg);
        Debug.Log("fileda" + filedata.data);
        Application.OpenURL(filedata.data);
        if (filedata.status == "true")
        {
            URlForpayments = filedata.data;
        }
    }


    IEnumerator GetMethodPaymentdetail(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))//bty any condtion this is not boy possiblity then it's should be cash nott added 
        {
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return webRequest.SendWebRequest();
            print(webRequest.downloadHandler.text + "yeah ");


            if (webRequest.isHttpError || webRequest.isNetworkError)
            {

                Debug.Log("Status" + webRequest.downloadHandler.text + " data define ");
                Debug.Log(webRequest.error);
                coinsInfo.text = "Cash not Added";
                Get_WalletData();
                SceneManager.LoadScene(0);
            }
            else
            {
                print(webRequest.downloadHandler.text + "yeah 1");
                StatusRoot filedata = new StatusRoot();
                filedata = JsonUtility.FromJson<StatusRoot>(webRequest.downloadHandler.text);
                if (filedata.status == "success")
                {
                    Debug.Log("Yeah data aana chiye");
                    coinsInfo.text = "Cash not Added";
                    Get_WalletData();
                    SceneManager.LoadScene(0);
                }
                else
                {
                    print(webRequest.downloadHandler.text + "yeah 2");
                    coinsInfo.text = "Cash Added Successfully";
                    Get_WalletData();
                    SceneManager.LoadScene(0);
                }

            }

        }
    }

    public void PaymentWalletData(string data)
    {
        Debug.Log(data + " Form upload complete!");
        Debug.Log("Player Status " + data);
        string msg = data;
        PaymentRoot pdata = new PaymentRoot();
        pdata = JsonUtility.FromJson<PaymentRoot>(msg);
        if (pdata.status == StatusResponse)
        {
            string balnce = pdata.data.amount.ToString();
            balanceValue.text = balnce +" INR";
            Debug.Log("Balance "+balnce);
        }
    }
    #endregion

    #region PostAllDataFetchFile
    [System.Serializable]

    public class Paymentdata
    {
        public int amount;
        public string userId;
        public string _id;

    }
    [System.Serializable]

    public class PaymentRoot
    {
        public string status;
        public string statusText;
        public string message;
        public Paymentdata data;
        
    }
    [System.Serializable]
    public class PostalPostData
    {
        public string status;
        public string data;
    }
    [System.Serializable]
    public class StatusRoot
    {
        public string name_on_card; 
        public string bank_ref_num; 
        public string udf3; 
        public string hash;
        public string firstname; 
        public string net_amount_debit;
        public string payment_source;
        public string surl;
        public string error_Message;
        public string issuing_bank; 
        public string cardCategory; 
        public string phone;
        public string easepayid;
        public string cardnum;
        public string key; 
        public string udf8;
        public string unmappedstatus;
        public string PG_TYPE; 
        public string addedon; 
        public string cash_back_percentage;
        public string status;
        public string udf1;
        public string udf6;
        public string udf10; 
        public string upi_va;
        public string txnid;
        public string amount;         
    }
    #endregion

    public Text statusText;


    public void DummyPayment(bool value)
    {
        if (value)
        {
            //success
            statusText.color = Color.green;
            statusText.text = "Payment success, redirecting do not refresh";
            /*When you click on proceed a temp script should take */
            //Add the item to orders page
            //Remove the item from cart page
        }
        else
        {
            //failed
            statusText.color = Color.red;
            statusText.text = "Payment failed";
        }
    }
}
