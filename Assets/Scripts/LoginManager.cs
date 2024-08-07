using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField email, password;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] MoveObject moveObject;
    [SerializeField] Button signInButton, showHideButton;
    [SerializeField] Toggle rememberMe;
    [SerializeField] Sprite showPasswordSprite;
    [SerializeField] Sprite hidePasswordSprite;
    bool isPasswordVisible = false;

    string userName, userData;

    private void Start()
    {
        // Load the name and number from PlayerPrefs
        if (PlayerPrefs.GetInt("Remembered", 0) == 1)
        {
            rememberMe.isOn = true;
            email.text = PlayerPrefs.GetString("UserEmail", "");
            password.text = PlayerPrefs.GetString("UserPassword", "");
        }
        else
        {
            rememberMe.isOn = false;
        }
        showHideButton.onClick.AddListener(TogglePasswordVisibility);
        signInButton.onClick.AddListener(ValidateLogin);
    }

    void TogglePasswordVisibility()
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

    public void AdminLogin(bool value)
    {
        UniversalData.checkAdmin = value;
    }

    void ValidateLogin()
    {
        userData = UniversalData.userData;

        string enteredLogin = email.text.ToLower();
        string enteredPassword = password.text;

        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (UniversalData.checkAdmin)
                {
                    if (user.email == enteredLogin && user.password == enteredPassword)
                    {
                        if (user.isAdmin)
                        {
                            EntryApproved(user);
                            return;
                        }
                        else
                        {
                            StatusTextDesign(Color.red, "Not an Admin");
                            return;
                        }
                    }
                }
                else
                {
                    if (user.email == enteredLogin && user.password == enteredPassword)
                    {
                        EntryApproved(user);
                        return;
                    }
                }
            }
        }

        StatusTextDesign(Color.red, "Invalid login credentials.");
    }

    private void EntryApproved(User user)
    {
        UniversalData.isAdmin = user.isAdmin;
        AppManager.isDesigner = UniversalData.isDesigner = user.isDesigner;
        //if (user.isDesigner)
        //{
        //    ProjectManager.Instance.desktopPlayerMovement.gameObject.name = user.id.ToString();
        //}
        UniversalData.userId = user.id;
        UniversalData.userFileID = user.id.ToString();
        UniversalData.userName = user.userName;
        UniversalData.guestLogin = false;
        userName = user.userName;
        if (rememberMe.isOn)
        {
            RememberMe(user.email, user.password);
        }
        else
        {
            PlayerPrefs.SetString("UserEmail", "");
            PlayerPrefs.SetString("UserPassword", "");
            PlayerPrefs.SetInt("Remembered", false ? 1 : 0);
            PlayerPrefs.Save();
        }
        StatusTextDesign(Color.green, "Login successful!");
        Invoke("GiveSomeDelay", 1);
        return;
    }

    void GiveSomeDelay()
    {
        ProjectManager.Instance.ControlPanels(false);
        moveObject.Move();
        ProjectManager.Instance.Login(userName);
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

    void RememberMe(string email, string password)
    {
        PlayerPrefs.SetString("UserEmail", email);
        PlayerPrefs.SetString("UserPassword", password);
        PlayerPrefs.SetInt("Remembered", true ? 1 : 0);
        PlayerPrefs.Save();
    }

    //void RememberMe(string email, string password)
    //{
    //    DateTime lastSetTime = DateTime.MinValue;

    //    // Check if the preferences were previously set
    //    if (PlayerPrefs.HasKey("LastSetTime"))
    //    {
    //        long ticks = Convert.ToInt64(PlayerPrefs.GetString("LastSetTime"));
    //        lastSetTime = new DateTime(ticks);
    //    }

    //    // Calculate the time difference between now and the last set time
    //    TimeSpan timeDifference = DateTime.Now - lastSetTime;

    //    // If it has been 30 days or more, reset the preferences
    //    if (timeDifference.Days >= 30)
    //    {
    //        PlayerPrefs.DeleteKey("UserEmail");
    //        PlayerPrefs.DeleteKey("UserPassword");
    //        PlayerPrefs.DeleteKey("Remembered");
    //    }

    //    // Save the new email and password
    //    PlayerPrefs.SetString("UserEmail", email);
    //    PlayerPrefs.SetString("UserPassword", password);
    //    PlayerPrefs.SetInt("Remembered", true ? 1 : 0);

    //    // Update the last set time to the current time
    //    PlayerPrefs.SetString("LastSetTime", DateTime.Now.Ticks.ToString());

    //    PlayerPrefs.Save();
    //}
}
