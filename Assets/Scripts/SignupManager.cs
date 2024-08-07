using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SignupManager : MonoBehaviour
{
    public TMP_InputField userName, email, phoneNumber, password, cnfPassword, guestUserName;
    public TextMeshProUGUI status, warning;
    //[SerializeField] ProjectManager projectManager;
    [SerializeField] MoveObject moveObject;
    [SerializeField] EmailManager emailManager;
    [SerializeField] Button signUpUser, CreateRegisterRequest, createDesignerAccount, showHidePW, showHideCPW;
    [SerializeField] Toggle isDesignerToggle;
    [SerializeField] Sprite showPasswordSprite;
    [SerializeField] Sprite hidePasswordSprite;
    [SerializeField] GameObject loginPanel;
    public bool mailVerified, numberVerified;
    bool cadetSignup;
    bool isPasswordVisible = false;
    bool isDesigner = false;
    [SerializeField] GameObject optionsPanel, guestNamePanel, CADETPanel;

    public void IsDesignerChange()
    {
        if (isDesignerToggle.isOn)
        {
            signUpUser.gameObject.SetActive(false);
            CreateRegisterRequest.gameObject.SetActive(true);
        }
        else
        {
            CreateRegisterRequest.gameObject.SetActive(false);
            signUpUser.gameObject.SetActive(true);
        }
    }

    public void CADETSignUp()
    {
        cadetSignup = true;
    }

    private void Start()
    {
        if (signUpUser)
        {
            signUpUser.onClick.AddListener(() => CreateUserAccount());
        }
        if (CreateRegisterRequest)
        {
            //CreateRegisterRequest.interactable = false;
            CreateRegisterRequest.onClick.AddListener(() => RequestDesignerAccount());
        }
        if (createDesignerAccount)
        {
            createDesignerAccount.onClick.AddListener(() => CreateDesignerAccount());
        }

        showHidePW.onClick.AddListener(() => TogglePasswordVisibility(password, showHidePW));
        showHideCPW.onClick.AddListener(() => TogglePasswordVisibility(cnfPassword, showHideCPW));
    }

    void TogglePasswordVisibility(TMP_InputField password, Button showHideButton)
    {
        isPasswordVisible = !isPasswordVisible;

        if (isPasswordVisible)
        {
            password.contentType = TMP_InputField.ContentType.Standard;
            showHideButton.image.sprite = hidePasswordSprite;
        }
        else
        {
            password.contentType = TMP_InputField.ContentType.Password;
            showHideButton.image.sprite = showPasswordSprite;
        }

        // Refresh the input field to apply changes
        password.ForceLabelUpdate();
    }

    void RequestDesignerAccount()
    {
        if (Validate())
        {
            //Proceed with login
            //if (emailManager.SendDesignerAccountCreateRequest(userName.text, email.text, phoneNumber.text, password.text))
            if (emailManager.SendDesignerAccountCreateRequest(userName.text, email.text, phoneNumber.text))
                {
                // Do what you want
                guestUserName.text = userName.text;
                StatusTextDesign(Color.blue, "Congrats!!! Account creation went for approval\n<b>Entering as guest</b>");
                CreateRegisterRequest.interactable = false;
                Invoke("GiveSomeDelay", 5);
            }
            else
            {
                StatusTextDesign(Color.red, "Something went wrong, please try later");
                CreateRegisterRequest.interactable = false;
                Invoke("GiveSomeDelay", 3);
            }
        }
    }

    void GiveSomeDelay()
    {
        CADETPanel.SetActive(false);
        ProjectManager.Instance.ControlPanels(false);
        CreateRegisterRequest.interactable = true;
        LoginAsGuest();
    }

    public void LoginAsGuest()
    {
        optionsPanel.SetActive(false);
        guestNamePanel.SetActive(false);
        UniversalData.userName = "Guest" +" "+ UnityEngine.Random.Range(0, 9);
        AppManager.isDesigner = UniversalData.isDesigner = false;
        ProjectManager.Instance.ControlPanels(false);
        if (!cadetSignup)
        {
            moveObject.Move();
        }
        ProjectManager.Instance.GuestLogin("Products");
    }

    [SerializeField] UserDatabase userDatabase;

    void CreateUserAccount()
    {
        if (!userDatabase)
        {
            userDatabase = this.GetComponent<UserDatabase>();
        }

        if (Validate())
        {
            if (userDatabase.CheckUserNameAlreadyExist())
            {
                StatusTextDesign(Color.red, "Name already used, please use different name");
            }
            else if (userDatabase.CheckEmailAlreadyExist())
            {
                StatusTextDesign(Color.red, "Email is already registered");
            }
            else if (userDatabase.CheckPhoneNumberAlreadyExist())
            {
                StatusTextDesign(Color.red, "Mobile number is already registered");
            }
            else
            {
                isDesigner = false;
                userDatabase.AddThisUser(isDesigner, ResponseData);
            }
        }
    }

    void CreateDesignerAccount()
    {
        if (!userDatabase)
        {
            userDatabase = this.GetComponent<UserDatabase>();
        }

        if (Validate())
        {
            if (userDatabase.CheckUserNameAlreadyExist())
            {
                StatusTextDesign(Color.red, "Name already used, please use different name");
            }
            else if (userDatabase.CheckEmailAlreadyExist())
            {
                StatusTextDesign(Color.red, "Email is already registered");
            }
            else if (userDatabase.CheckPhoneNumberAlreadyExist())
            {
                StatusTextDesign(Color.red, "Mobile number is already registered");
            }
            else
            {
                isDesigner = true;
                userDatabase.AddThisUser(isDesigner, ResponseData);
            }
        }
    }

    private void ResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            if (isDesigner)
            {
                userDatabase.CreateCard();
            }

            string userSpecificJSONData = "{\"Products\": []}";
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return;
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.UpdateList(userSpecificJSONData, userDatabase.id, GetResponseData);
        }
        else
        {
            StatusTextDesign(Color.red, "Registration failed, try again later");
        }

        Invoke("ClearFields", 2);
    }

    public void UpdateDesignerAccount(int userId)
    {
        if (!userDatabase)
        {
            userDatabase = this.GetComponent<UserDatabase>();
        }

        if (Validate())
        {
            (string message, Color _color) = userDatabase.UpdateUser(userId, password.text);
            StatusTextDesign(_color, message);
            Invoke("ClearFields", 2);
        }
    }

    // This piece of code is sending mail to the designer mail ID that His account is created with the details provided.
    private void GetResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            //StatusTextDesign(Color.green, "Account created successfully");
            if (!isDesigner)
            {
                gameObject.SetActive(false);
                loginPanel.SetActive(true);
            }

            ///* Send mail to designer about approval of the account */
            //if (emailManager.SendDesignerAccountInfo())
            //{
            //    StatusTextDesign(Color.green, "Data created and info sent successfully");
            //}
            //else
            //{
            //    StatusTextDesign(Color.red, "Data created but info not sent");
            //}
        }
        else
        {
            StatusTextDesign(Color.red, "Account data not created");
        }

        Invoke("ClearFields", 2);
    }

    internal void ClearFields()
    {
        email.text = userName.text = phoneNumber.text = password.text = cnfPassword.text = "";
    }

    bool Validate()
    {
        if (email.text == "" || userName.text == "" || phoneNumber.text == "")
        {
            StatusTextDesign(Color.red, "Fields should not be empty");
            return false;
        }
        else if (userName.text.Length < 4)
        {
            StatusTextDesign(Color.red, "Username should be atleast 4 characters");
            return false;
        }
        //else if (password.text == "" || cnfPassword.text == "")
        //{
        //    StatusTextDesign(Color.red, "Please enter the password");
        //    return false;
        //}
        //else if (password.text != cnfPassword.text)
        //{
        //    StatusTextDesign(Color.red, "Password miss match");
        //    return false;
        //}
        else if (!IsValidEmail(email.text))
        {
            StatusTextDesign(Color.red, "Invalid Email");
            return false;
        }
        else if (phoneNumber.text.Length != 10)
        {
            StatusTextDesign(Color.red, "Please enter valid phone number");
            return false;
        }

        return true;
    }

    public static bool IsValidEmail(string email)
    {
        // Define the regular expression pattern for email validation
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);
        return match.Success;
    }

    void StatusTextDesign(Color color, string message)
    {
        status.color = color;
        status.text = message;
        Invoke("ResetStatus", 2);
    }

    void ResetStatus()
    {
        status.color = Color.white;
        status.text = "";
    }

    internal void IsBothVerified()
    {
        if (mailVerified && numberVerified)
        {
            CreateRegisterRequest.interactable = true;
            signUpUser.interactable = true;
        }
    }
}
