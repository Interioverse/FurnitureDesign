using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Text;

public class Cart : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statusText, status;
    public TextMeshProUGUI totalAmount;
    public GameObject itemPrefab, emptyCart, signintoaddtocart;
    public Transform content;
    UserWrapper userWrapper;
    public float cartTotalAmount;
    public Button addAddress, saveAddress, buy;
    [SerializeField] GameObject shippmentAddressPage, paymentPage;
    [SerializeField] PaymentGatewayManager PGM;
    User targetUser;
    bool assigned;

    private void Awake()
    {
        if (content.childCount > 0)
        {
            foreach (Transform item in content)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        cartTotalAmount = 0;
        addAddress.gameObject.SetActive(false);
        buy.gameObject.SetActive(false);
    }

    private void Start()
    {
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        targetUser = GetUserByID(UniversalData.userId);

        if (targetUser != null)
        {
            //userName.text = "Hi " + targetUser.userName;
            foreach (CartItem cartItem in targetUser.cartItems)
            {
                SpawnItem(cartItem.productID, cartItem.productName, cartItem.productPrice);
            }
            if (content.childCount > 0)
            {
                emptyCart.SetActive(false);
                signintoaddtocart.SetActive(false);
                addAddress.gameObject.SetActive(true);
                totalAmount.gameObject.SetActive(true);
            }
            else
            {
                emptyCart.SetActive(true);
                signintoaddtocart.SetActive(false);
            }
        }
        else
        {
            Debug.Log("User not found with ID: ");
            emptyCart.SetActive(false);
            signintoaddtocart.SetActive(true);
        }

        addAddress.onClick.AddListener(() => OpenAddAddressPanel());
        saveAddress.onClick.AddListener(() => SaveUserAddress());
        buy.onClick.AddListener(() => Buy());
    }

    private User GetUserByID(int userID)
    {
        foreach (User user in userWrapper.Users)
        {
            if (user.id == userID)
            {
                return user;
            }
        }
        return null;
    }

    public List<int> cartItemIds;

    public void SpawnItem(int productId, string productName, float productPrice)
    {
        // Instantiate the prefab
        GameObject newItem = Instantiate(itemPrefab, content);
        newItem.name = productId.ToString();
        newItem.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "<b>Name: </b>" + productName;
        newItem.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = "<b>Price: </b>" + productPrice.ToString() +" INR";
        newItem.transform.Find("Remove").GetComponent<Button>().onClick.AddListener(() => Remove(productId, productPrice));
        //newItem.transform.Find("Buy this").GetComponent<Button>().onClick.AddListener(() => BuyThis(productId));
        ChangeTheTotalAmount(productPrice);
    }

    //private void BuyThis(int productId)
    //{
    //    //Proceed
    //    print("Bought " + productId);
    //    //Open address panel
    //}

    [SerializeField] UserDatabase userDatabase;

    private void Remove(int productId, float productPrice)
    {
        if (!userDatabase)
        {
            userDatabase = FindObjectOfType<UserDatabase>();
        }

        if (userDatabase.RemoveFromCart(productId) == 1)
        {
            foreach (Transform item in content)
            {
                if (item.name == productId.ToString())
                {
                    DestroyImmediate(item.gameObject);
                    ChangeTheTotalAmount(-productPrice);
                    statusText.color = Color.green;
                    statusText.text = "Item removed from cart successfully!";
                    Invoke("ClearStatus", 2);
                }
            }

            if (content.childCount <= 0)
            {
                addAddress.gameObject.SetActive(false);
                totalAmount.gameObject.SetActive(false);
                emptyCart.SetActive(true);
            }
        }
        else if (userDatabase.RemoveFromCart(productId) == 0)
        {
            statusText.color = Color.red;
            statusText.text = "Item not found in the cart";
            Invoke("ClearStatus", 2);
        }
    }

    void ClearStatus()
    {
        statusText.text = "";
    }

    public void ChangeTheTotalAmount(float productPrice)
    {
        cartTotalAmount += productPrice;
        totalAmount.text = "Total amount: " + cartTotalAmount + " INR";
    }

    void OpenAddAddressPanel()
    {
        shippmentAddressPage.SetActive(true);
        DisplayUserAddressDetails(UniversalData.userId);
    }

    public void DisplayUserAddressDetails(int userID)
    {
        if (targetUser != null && !assigned)
        {
            Address userAddress = targetUser.userAddress;

            pincodeInput.text = userAddress.pincode.ToString();
            stateInput.text = userAddress.state;
            cityInput.text = userAddress.city;
            addressInput.text = userAddress.address;
            landmarkInput.text = userAddress.landmark;
            addressNameInput.text = userAddress.addressName;
            addressPhoneNumberInput.text = userAddress.addressPhoneNumber;
            addressEmailInput.text = userAddress.addressEmail;
            assigned = true;
            StatusTextDesign(Color.black, "Address taken based on your previous entry");
        }
    }

    void Buy()
    {
        //To Do
        //collect all items and get the all the required details of the respective details

        paymentPage.SetActive(true);
        PGM.totalPayableAmount.text = cartTotalAmount.ToString();
        //Proceed with payment integration
    }

    public TMP_InputField addressNameInput;
    public TMP_InputField addressPhoneNumberInput;
    public TMP_InputField addressEmailInput;
    public TMP_InputField pincodeInput;
    public TMP_InputField stateInput;
    public TMP_InputField cityInput;
    public TMP_InputField addressInput;
    public TMP_InputField landmarkInput;

    void SaveUserAddress()
    {
        if (Validate())
        {
            int pincode = int.Parse(pincodeInput.text);
            string state = stateInput.text;
            string city = cityInput.text;
            string address = addressInput.text;
            string landmark = landmarkInput.text;
            string addressName = addressNameInput.text;
            string addressPhoneNumber = addressPhoneNumberInput.text;
            string addressEmail = addressEmailInput.text;

            UpdateUserAddress(UniversalData.userId, pincode, state, city, address, landmark, addressName, addressPhoneNumber, addressEmail);
        }
    }

    public void UpdateUserAddress(int userID, int pincode, string state, string city, string address, string landmark, string addressName, string addressPhoneNumber, string addressEmail)
    {
        UserWrapper userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);

        if (userWrapper == null)
        {
            Debug.LogError("User database is empty or invalid!");
            return;
        }

        // Find the user with the specified userID
        User userToUpdate = userWrapper.Users.Find(user => user.id == userID);

        if (userToUpdate != null)
        {
            // Update user's address information
            userToUpdate.userAddress.addressName = addressName;
            userToUpdate.userAddress.addressPhoneNumber = addressPhoneNumber;
            userToUpdate.userAddress.addressEmail = addressEmail;
            userToUpdate.userAddress.pincode = pincode;
            userToUpdate.userAddress.state = state;
            userToUpdate.userAddress.city = city;
            userToUpdate.userAddress.address = address;
            userToUpdate.userAddress.landmark = landmark;

            // Convert the updated userWrapper back to JSON format
            string updatedJsonData = JsonUtility.ToJson(userWrapper);
            UniversalData.userData = updatedJsonData;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UniversalData.Instance.noInternetScene.SetActive(true);
                return;
            }
            UniversalData.Instance.noInternetScene.SetActive(false);
            HttpAWSconnect.Instance.UpdateList(updatedJsonData, 0, ShipAddressAddResponseData);
        }
        else
        {
            Debug.LogWarning($"User with ID {userID} not found!");
        }
    }

    private void ShipAddressAddResponseData(string data, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.Success)
        {
            StatusTextDesign(Color.green, "Address added");
            Invoke("Close", 2);
        }
        else
        {
            print("Something went wrong");
        }
    }

    void Close()
    {
        shippmentAddressPage.gameObject.SetActive(false);
        buy.gameObject.SetActive(true);
        ResetStatus();
    }

    bool Validate()
    {
        if (addressNameInput.text == "" || addressPhoneNumberInput.text == "" || addressEmailInput.text == "" || pincodeInput.text == "" || stateInput.text == "" || cityInput.text == "" || addressInput.text == "" || landmarkInput.text == "")
        {
            StatusTextDesign(Color.red, "Fields should not be empty");
            return false;
        }
        else if (!IsValidEmail(addressEmailInput.text))
        {
            StatusTextDesign(Color.red, "Invalid Email");
            return false;
        }
        else if (addressPhoneNumberInput.text.Length != 10)
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

    public string GenerateRandomID(int segments = 4, int charactersPerSegment = 4)
    {
        StringBuilder idBuilder = new StringBuilder();

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < charactersPerSegment; j++)
            {
                char randomChar = GetRandomChar();
                idBuilder.Append(randomChar);
            }

            if (i < segments - 1)
            {
                idBuilder.Append("-");
            }
        }

        return idBuilder.ToString();
    }

    private char GetRandomChar()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        int index = UnityEngine.Random.Range(0, chars.Length);
        return chars[index];
    }

    private void CopyOfStart()
    {
        string randomID = GenerateRandomID();
        Debug.Log("Random ID: " + randomID);
    }
}
