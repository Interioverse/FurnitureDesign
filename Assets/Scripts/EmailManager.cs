using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UE.Email;
using System.Text.RegularExpressions;

public class EmailManager : MonoBehaviour
{
    [SerializeField] GameObject sender, validator;
    [SerializeField] TMP_InputField nameInput, toEmailID, enterOTP;
    [SerializeField] Button sendEmail, validateEmail;
    string from, subject, body, smtp, user, accountRequests, to, OTP, password;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] SignupManager signupManager;

    private void Awake()
    {
        from = "interioversedesign@gmail.com";
        user = "interioversedesign@gmail.com";
        accountRequests = "partners@interioverse.com";
        password = "50F7C4F3CA47100BC5AB0E0A0D59E4EC08E8";
        smtp = "smtp.elasticemail.com";
        if (sendEmail)
        {
            //sendEmail.onClick.AddListener(() => GenerateOTPAndSendEmail());
        }
        if (validateEmail)
        {
            //validateEmail.onClick.AddListener(() => ValidateEmail());
        }
    }

    internal bool SendDesignerAccountCreateRequest(string name, string email, string phoneNumber)
    {
        subject = "Account create request";
        string _hi = "Hi Interioverse";
        string _message = "Please create the designer account with the below information.";
        string _name = "Name: " + HTML.Bold(name);
        string _email = "Email: " + HTML.Bold(email);
        string _phoneNumber = "Phone number: " + HTML.Bold(phoneNumber);
        //string _password = "password: " + HTML.Bold(emailPassword);
        string _regards = "Best Regards";
        //body = _hi + HTML.Br + HTML.Br + _message + HTML.Br + HTML.Br + _name + HTML.Br + _phoneNumber + HTML.Br + _email + HTML.Br + _password + HTML.Br + HTML.Br + _regards + HTML.Br + name; //Old
        body = _hi + HTML.Br + HTML.Br + _message + HTML.Br + HTML.Br + _name + HTML.Br + _phoneNumber + HTML.Br + _email + HTML.Br + "" + HTML.Br + HTML.Br + _regards + HTML.Br + name; //New

        //#if UNITY_EDITOR
        //        Debug.Log(body);
        //        return true;
        //#endif

#if !UNITY_EDITOR
        Email.SendEmail(from, accountRequests, subject, body, smtp, user, password);
        return true;
#else
        return true;
#endif
    }

    void BackToMain()
    {
        sender.SetActive(true);
        validator.SetActive(false);
    }

    void GenerateOTPAndSendEmail()
    {
        if (nameInput.text == "" || toEmailID.text == "")
        {
            DesignStatusText(Color.red, "Fields shouldn't be empty", 2);
            return;
        }
        else if (!IsValidEmail(toEmailID.text))
        {
            DesignStatusText(Color.red, "Invalid email address", 2);
            return;
        }
        GenerateOTP();
    }

    private void DesignStatusText(Color _color, string message, int waitTime)
    {
        statusText.color = _color;
        statusText.text = message;
        Invoke("ResetText", waitTime);
    }

    private void ResetText()
    {
        statusText.color = Color.green;
        statusText.text = "";
    }

    public static bool IsValidEmail(string email)
    {
        // Define the regular expression pattern for email validation
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);
        return match.Success;
    }

    private void GenerateOTP()
    {
        DesignStatusText(Color.blue, "Email sent\nPlease check your <u><b>Spam</b></u> folder also", 3);

        OTP = "";

        for (int i = 0; i < 6; i++)
        {
            OTP += Random.Range(0, 10).ToString();
        }

        to = toEmailID.text;
#if UNITY_EDITOR
        print(OTP);
#endif

        subject = "Verify your email";
        string _hi = "Hi " + nameInput.text;
        string _message = "Thank you for choosing Interioverse. Use the following OTP "  + HTML.Bold(OTP) + " to validate your email. ";
        string _extention = "This OTP is valid for 5 minutes";
        string _regards = "Best Regards";
        string _companyName = "Interioverse";
        body = _hi + HTML.Br + HTML.Br + _message + HTML.Br + _extention + HTML.Br + HTML.Br + _regards + HTML.Br + _companyName;
#if !UNITY_EDITOR
        Email.SendEmail(from, to, subject, body, smtp, user, password);
        DesignStatusText(Color.blue, "OTP sent \nPlease check your <u><b>Spam</b></u> folder also", 5);
#endif
        sender.SetActive(false);
        validator.SetActive(true);
        SaveOTP(OTP);
    }

    static void SaveOTP(string otpValue)
    {
        PlayerPrefs.SetString("OTP", otpValue);
        PlayerPrefs.Save();
    }

    [SerializeField] Sprite notVerified, verified;

    void ValidateEmail()
    {
        if (enterOTP.text == "")
        {
            DesignStatusText(Color.red, "Incorrect OTP", 2);
            return;
        }

        if (enterOTP.text == GetOTP())
        {
            DesignStatusText(Color.green, "Email validated", 2);
            //nameInput.interactable = false;
            toEmailID.interactable = false;
            sendEmail.interactable = false;
            validator.SetActive(false);
            //sendEmail.GetComponent<Image>().sprite = verified;
            sendEmail.GetComponent<Image>().color = Color.green;
            sendEmail.GetComponentInChildren<TextMeshProUGUI>().text = "Email verified";
            ////sendEmail.gameObject.SetActive(false);
            //afterValidation.SetActive(true);
            sender.SetActive(true);
            signupManager.mailVerified = true;
            signupManager.IsBothVerified();
        }
        else
        {
            DesignStatusText(Color.red, "Incorrect OTP", 2);
        }
    }

    static string GetOTP()
    {
        return PlayerPrefs.GetString("OTP", "");
    }

    public bool SendDesignerAccountInfo()
    {
        to = toEmailID.text;
        subject = "Designer approved";
        string _hi = "Hi " + nameInput.text;
        string _message = "<b>Congratulations!!!</b> Your account is been approved.";
        string _extention = "Now you can design and publish your furniture designs to Interioverse. ";
        string _regards = "Best Regards";
        string _companyName = "Interioverse";
        body = _hi + HTML.Br + HTML.Br + _message + HTML.Br + _extention + HTML.Br + HTML.Br + _regards + HTML.Br + _companyName;
#if !UNITY_EDITOR
        Email.SendEmail(from, to, subject, body, smtp, user, password);
        return true;
#endif
        return false;
    }
}