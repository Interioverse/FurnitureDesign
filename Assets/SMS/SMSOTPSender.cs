using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class SMSOTPSender : MonoBehaviour
{

    //string ComapnyName = "Digital Imerse";
    string ServerOTP;

    [SerializeField] TMP_InputField userName;
    [SerializeField] TMP_InputField mobileNumber;
    //[SerializeField] TMP_InputField Email;
    [SerializeField] TMP_InputField enteredOTP;
    [SerializeField] TMP_Text validationStatus;
    [SerializeField] Button sendOTPButton, backButton;
    [SerializeField] Button validateOTPButtonForLogin, validateOTPButtonForRegistration;
    [SerializeField] Button resendButton;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject sendOTP_Parent;
    [SerializeField] GameObject validateOTP_Parent;
    [SerializeField] SignupManager signupManager;

    //[SerializeField] GameObject MailObj;
    //public TMP_Text PlayerName;

    private void Start()
    {
        sendOTPButton.onClick.AddListener(SendOTP);
        if (backButton)
        {
            backButton.onClick.AddListener(Back);
        }
        if (validateOTPButtonForLogin)
        {
            validateOTPButtonForLogin.onClick.AddListener(ValidateOTP);
        }
        resendButton.onClick.AddListener(ResendOTP);
        if (validateOTPButtonForRegistration)
        {
            //validateOTPButtonForRegistration.onClick.AddListener(ValidateOTPForRegistration);
        }
        resendButton.gameObject.SetActive(false);
        sendOTP_Parent.SetActive(true);
        validateOTP_Parent.SetActive(false);
    }

    void Back()
    {
        mobileNumber.text = "";
        optionsPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ResendOTP()
    {
        resendButton.gameObject.SetActive(false);
        StartCoroutine(SendOTPCoroutine());
    }

    public void SendOTP()
    {
        if (userName.text == "" || mobileNumber.text == "")
        {
            StartCoroutine(OTPstatus("Fields shouldn't be empty", Color.red));
            return;
        }
        else if (mobileNumber.text.Length != 10)
        {
            StartCoroutine(OTPstatus("Please enter valid phone number", Color.red));
            return;
        }
#if UNITY_EDITOR
        ServerOTP = "000000";
        print(ServerOTP);
        StartCoroutine(OTPstatus("OTP sent Successfully!", Color.green));
        StartCoroutine(WiatforResend());
#else
        StartCoroutine(SendOTPCoroutine());
#endif
    }


    private IEnumerator SendOTPCoroutine()
    {
        ServerOTP = GenerateOTP();

        string url = "https://smsapi.edumarcsms.com/api/v1/sendsms";
        //string apiKey = "cjlpehwde000ugdquxea8ta5u"; //old API
        string apiKey = "clkcjfrb100051tqxd5vp30rj";
        string message = "Your {#var#} OTP for verification is: " + ServerOTP + ". OTP is confidential, refrain from sharing it with anyone. By Edumarc Technologies";
        string senderId = "EDUMRC";
        string templateId = "1707168926925165526";

        List<string> numbers = new List<string>();
        numbers.Add(mobileNumber.text);

        Customer cust = new Customer()
        {
            apikey = apiKey,
            message = message,
            senderId = senderId,
            templateId = templateId,
            number = numbers
        };

        string json = JsonUtility.ToJson(cust);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("apikey", apiKey);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                StartCoroutine(OTPstatus("OTP sent Successfully!", Color.green));
                StartCoroutine(WiatforResend());
                //MailObj.GetComponent<SendMail>().Sendemail(Mobilenumber.text, Name.text,Email.text);
            }
            else
            {
                StartCoroutine(OTPstatus("OTP not sent", Color.red));
            }
        }
    }

    private string GenerateOTP()
    {
        string otp = "";
        for (int i = 0; i < 6; i++)
        {
            otp += Random.Range(0, 10).ToString();
        }
        return otp;
    }

    private IEnumerator WiatforResend()
    {
        sendOTP_Parent.SetActive(false);
        validateOTP_Parent.SetActive(true);
        yield return new WaitForSeconds(120f);
        resendButton.gameObject.SetActive(true);
    }

    [SerializeField] Sprite notVerified, verified;
    [SerializeField] UserDatabase userDatabase;

    public void ValidateOTP()
    {
        if (isOTPsame())
        {
            StartCoroutine(OTPstatus("OTP Successfully Validated!", Color.green));
            optionsPanel.SetActive(false);

            /* Do not delete this bode - This code add the user data for otp login user */
            if (!userDatabase.CheckPhoneNumberAlreadyExistForOTPLogin())
            {
                userDatabase.AddUserFromOTPLogin(false, ResponseData);
            }

            SaveData();
        }
        else
        {
            StartCoroutine(OTPstatus("OTP is Invalid, Please try again!", Color.red));
        }
    }

    /* Do not delete this bode - This code add the user data for otp login user*/
    private void ResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            /* This will create the Products json file - but it is not mandatory for OTP login and guest login users */
            string userSpecificJSONData = "{\"Products\": []}";
            HttpAWSconnect.Instance.UpdateList(userSpecificJSONData, userDatabase.id, GetResponseData);
        }
    }

    private void GetResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            OTPstatus("Data created successfully", Color.green);
        }
        else
        {
            OTPstatus("Something went wrong, please try later", Color.red);
        }

        Invoke("ClearFields", 2);
    }

    public void ValidateOTPForRegistration()
    {
        if (isOTPsame())
        {
            StartCoroutine(OTPstatus("OTP Successfully Validated!", Color.green));
            sendOTPButton.interactable = false;
            sendOTP_Parent.SetActive(true);
            //sendOTPButton.GetComponent<Image>().sprite = verified;
            sendOTPButton.GetComponent<Image>().color = Color.green;
            sendOTPButton.GetComponentInChildren<TextMeshProUGUI>().text = "Number verified";
            validateOTP_Parent.SetActive(false);
            signupManager.numberVerified = true;
            mobileNumber.interactable = false;
            signupManager.IsBothVerified();
        }
        else
        {
            StartCoroutine(OTPstatus("OTP is Invalid, Please try again!", Color.red));
        }
    }

    bool isOTPsame()
    {
        return ServerOTP == enteredOTP.text ? true : false;
    }


    private IEnumerator OTPstatus(string value, Color _color)
    {
        validationStatus.color = _color;
        validationStatus.text = value;
        yield return new WaitForSeconds(3f);
        validationStatus.text = "";
        StopCoroutine("OTPstatus");
    }

    private void SaveData()
    {
        //PlayerName.text = "Hi, " + Name.text;
        PlayerPrefs.SetString("Name", userName.text);
        PlayerPrefs.SetString("NewUser", "NO");
        ProjectManager.Instance.LoginWithOTP(mobileNumber.text);
    }
}


public class Customer
{
    public string apikey;
    public string message;
    public List<string> number;
    public string senderId;
    public string templateId;
}