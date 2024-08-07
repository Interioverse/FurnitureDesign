using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;

[Serializable]
public class User
{
    public int id;
    public string userName;
    public string password;
    public long phoneNumber;
    public string email;
    public bool isAdmin = false;
    public int fileId;
    public bool isDesigner = false;
    public List<CartItem> cartItems;
    public Address userAddress;
    public List<OrderedItem> orderedItems;

    public User(int id, string userName, string password, long phoneNumber, string email, int userFileID, bool _isDesigner)
    {
        this.id = id;
        this.userName = userName;
        this.password = password;
        this.phoneNumber = phoneNumber;
        this.email = email;
        this.fileId = userFileID;
        this.isDesigner = _isDesigner;
        cartItems = new List<CartItem>();
        userAddress = new Address();
        orderedItems = new List<OrderedItem>();
    }
}

[Serializable]
public class Address
{
    public string addressName;
    public string addressPhoneNumber;
    public string addressEmail;
    public int pincode;
    public string state;
    public string city;
    public string address;
    public string landmark;
}

[Serializable]
public class CartItem
{
    public int productID;
    public string productName;
    public float productPrice;

    public CartItem(int productID, string productName, float productPrice)
    {
        this.productID = productID;
        this.productName = productName;
        this.productPrice = productPrice;
    }
}

[System.Serializable]
public class OrderedItem
{
    public string orderPlacedOn;
    public string date;
    public float totalAmount;
    public string shipToName;
    public string orderID;
    public string itemName;
}

[Serializable]
public class UserWrapper
{
    public List<User> Users = new List<User>();
}

public class UserDatabase : MonoBehaviour
{
    //private const string fileName = "Users.json";
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField phoneNumberInput;
    public TMP_InputField emailInput;

    // Function to generate a random ID with a minimum of 5 digits
    private int GenerateRandomID()
    {
        int minID = 1000; // Minimum 5-digit number
        int maxID = 1000000; // Maximum 5-digit number
        int ID = UnityEngine.Random.Range(minID, maxID);
        if (IsUserIDAlreadyUsed(ID))
        {
            GenerateRandomID();
        }
        return ID;
    }

    private bool IsUserIDAlreadyUsed(int _userID)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (user.id == _userID)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsUserNameAlreadyRegistered(string _userName)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (user.userName.ToLower() == _userName.ToLower())
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Function to check if the entered email is already present in the Users list
    private bool IsEmailAlreadyRegistered(string email)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (user.email.ToLower() == email.ToLower())
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Function to check if the entered phone number is already present in the Users list
    bool IsPhoneNumberAlreadyRegistered(long phoneNumber)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (user.phoneNumber == phoneNumber)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool IsPhoneNumberAlreadyRegisteredForOTPLogin(long phoneNumber)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper != null)
        {
            foreach (User user in userWrapper.Users)
            {
                if (user.phoneNumber == phoneNumber)
                {
                    UniversalData.userName = user.userName;
                    UniversalData.userFileID = user.fileId.ToString();
                    UniversalData.userId = user.id;
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckUserNameAlreadyExist()
    {
        if (IsUserNameAlreadyRegistered(userNameInput.text))
        {
            return true;
        }
        return false;
    }

    public bool CheckEmailAlreadyExist()
    {
        if (IsEmailAlreadyRegistered(emailInput.text))
        {
            return true;
        }
        return false;
    }

    public bool CheckPhoneNumberAlreadyExist()
    {
        if (IsPhoneNumberAlreadyRegistered(long.Parse(phoneNumberInput.text)))
        {
            return true;
        }
        return false;
    }

    public bool CheckPhoneNumberAlreadyExistForOTPLogin()
    {
        if (IsPhoneNumberAlreadyRegisteredForOTPLogin(long.Parse(phoneNumberInput.text)))
        {
            return true;
        }
        return false;
    }

    [SerializeField] Designers designers;

    public (string, Color) UpdateUser(int userID, string newPassword)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);

        if (userWrapper == null)
        {
            return ("User database is empty or invalid!", Color.red);
        }

        // Find the user with the specified userID
        User userToUpdate = userWrapper.Users.Find(user => user.id == userID);

        if (userToUpdate != null)
        {
            // Update user information
            //userToUpdate.userName = newUserName;
            userToUpdate.password = newPassword;
            //userToUpdate.phoneNumber = newPhoneNumber;
            //userToUpdate.email = newEmail;

            // Convert the updated userWrapper back to JSON format
            string updatedJsonData = JsonUtility.ToJson(userWrapper);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return ($"No internet connection", Color.red);
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ResponseData);
            UniversalData.userData = updatedJsonData;

            return ($"User data updated successfully", Color.green);
        }
        else
        {
            return ($"User data not found!", Color.red);
        }
    }

    public int AddToCart(JToken product)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);

        if (userWrapper == null)
        {
            return 2;
        }

        // Find the specific user based on the logged-in user's information (you'll need to implement your own way to get the logged-in user's information)
        User loggedInUser = GetUserBasedOnYourLoginLogic(userWrapper);

        if (loggedInUser == null)
        {
            return 2;
        }

        int itemID = (int)product["id"];

        // Check if the item ID is already present in the cartItems list
        if (!IsItemAlreadyInCart(loggedInUser, itemID))
        {
            string productName = product["productName"].ToString();
            float productPrice = (float)product["price"];

            /* If the item is not already in the cart, add it */
            //int productID = itemID; // Assuming itemID is in string format, convert it to int
            CartItem cartItem = new CartItem(itemID, productName, productPrice);
            loggedInUser.cartItems.Add(cartItem);

            // Update the userWrapper with the modified user data
            UpdateUserInWrapper(userWrapper, loggedInUser);

            // Save the updated user data back to the JSON file
            SaveUserWrapperToJson(userWrapper);

            return 1;
        }
        else
        {
            return 0;
        }
    }

    private bool IsItemAlreadyInCart(User user, int itemID)
    {
        int productID = itemID; // Assuming itemID is in string format, convert it to int

        foreach (CartItem cartItem in user.cartItems)
        {
            if (cartItem.productID == productID)
            {
                return true;
            }
        }

        return false;
    }

    // Implement your own logic to get the logged-in user based on the available information (e.g., username, session, etc.)
    private User GetUserBasedOnYourLoginLogic(UserWrapper userWrapper)
    {
        // Replace this with your own implementation to find the logged-in user based on some information (e.g., username, session, etc.)
        // For this example, we'll assume that we are searching based on a hardcoded username "loggedInUsername"
        int loggedInUserID = UniversalData.userId;
        return userWrapper.Users.Find(user => user.id == loggedInUserID);
    }

    private void UpdateUserInWrapper(UserWrapper userWrapper, User userToUpdate)
    {
        // Find the index of the user to update in the userWrapper list
        int userIndex = userWrapper.Users.FindIndex(user => user.id == userToUpdate.id);

        // If the user is found, update it in the list
        if (userIndex != -1)
        {
            userWrapper.Users[userIndex] = userToUpdate;
        }
    }

    private void SaveUserWrapperToJson(UserWrapper userWrapper)
    {
        string updatedJsonData = JsonUtility.ToJson(userWrapper);

        //#if UNITY_EDITOR
        //        // Save the updated user data to the JSON file (you can replace this with your own saving logic if needed)
        //        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        //        File.WriteAllText(filePath, updatedJsonData);
        //#endif
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ResponseData);
        UniversalData.userData = updatedJsonData;
    }

    private void ResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            //print("Updated");
        }
        else
        {
            print("Something went wrong");
        }
    }

    public int RemoveFromCart(int itemID)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);

        if (userWrapper == null)
        {
            //Debug.LogError("User database is empty or invalid!");
            return 2;
        }

        // Find the specific user based on the logged-in user's information (you'll need to implement your own way to get the logged-in user's information)
        User loggedInUser = GetUserBasedOnYourLoginLogic(userWrapper);

        if (loggedInUser == null)
        {
            //Debug.LogError("Logged-in user not found!");
            return 2;
        }

        int productID = itemID; // Assuming itemID is in string format, convert it to int

        // Check if the item ID is present in the cartItems list
        int itemIndex = -1;
        for (int i = 0; i < loggedInUser.cartItems.Count; i++)
        {
            if (loggedInUser.cartItems[i].productID == productID)
            {
                itemIndex = i;
                break;
            }
        }

        if (itemIndex != -1)
        {
            // Remove the item from the cartItems list
            loggedInUser.cartItems.RemoveAt(itemIndex);

            // Update the userWrapper with the modified user data
            UpdateUserInWrapper(userWrapper, loggedInUser);

            // Save the updated user data back to the JSON file
            SaveUserWrapperToJson(userWrapper);

            //Debug.Log("Item removed from cart successfully!");
            return 1;
        }
        else
        {
            //Debug.Log("Item not found in the cart!");
            return 0;
        }
    }

    User newUser;
    public int id;

    public void AddThisUser(bool _isDesigner, Action<string, UnityWebRequest.Result> ResponseData)
    {
        id = GenerateRandomID();
        string userName = userNameInput.text.ToLower();
        string password = passwordInput.text;
        long phoneNumber = long.Parse(phoneNumberInput.text);
        string email = emailInput.text.ToLower();
        int userFileID = id;
        bool isDesigner = _isDesigner;

        newUser = new User(id, userName, password, phoneNumber, email, userFileID, _isDesigner);

        UserWrapper userWrapper = new UserWrapper();
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper == null)
        {
            userWrapper = new UserWrapper();
        }

        userWrapper.Users.Add(newUser);

        // Convert the userWrapper to JSON format
        string updatedJsonData = JsonUtility.ToJson(userWrapper);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ResponseData);
        UniversalData.userData = updatedJsonData;
    }

    public void AddUserFromOTPLogin(bool _isDesigner, Action<string, UnityWebRequest.Result> ResponseData)
    {
        id = GenerateRandomID();
        string _userName = userNameInput.text +"_"+ id;
        string _password = phoneNumberInput.text;
        long _phoneNumber = long.Parse(phoneNumberInput.text);
        string email = "";
        int userFileID = id;
        bool isDesigner = _isDesigner;

        newUser = new User(id, _userName, _password, _phoneNumber, email, userFileID, isDesigner);

        UserWrapper userWrapper = new UserWrapper();
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        if (userWrapper == null)
        {
            userWrapper = new UserWrapper();
        }

        userWrapper.Users.Add(newUser);

        // Convert the userWrapper to JSON format
        string updatedJsonData = JsonUtility.ToJson(userWrapper);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UniversalData.Instance.noInternetScene.SetActive(true);
            return;
        }
        UniversalData.Instance.noInternetScene.SetActive(false);
        HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ResponseData);
        UniversalData.userData = updatedJsonData;
        Debug.Log(UniversalData.userData);
        UniversalData.userId = id;
        UniversalData.userName = _userName;
        UniversalData.userFileID = userFileID.ToString();
    }

    internal void CreateCard()
    {
        designers.AddCard(newUser);
    }
}
