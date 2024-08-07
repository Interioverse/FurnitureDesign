using Interioverse;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Profile : SingletonComponent<Profile>
{
    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TMP_InputField _userNameIF, _userEmailIF, _userNumberIF;
    private UserWrapper userWrapper;
    User targetUser;

    void Start()
    {
        userWrapper = JsonUtility.FromJson<UserWrapper>(UniversalData.userData);
        targetUser = GetUserByID(UniversalData.userId);
        _userNameIF.text = userName.text = targetUser.userName;
        _userEmailIF.text = targetUser.email;
        _userNumberIF.text = targetUser.phoneNumber.ToString();
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
}
