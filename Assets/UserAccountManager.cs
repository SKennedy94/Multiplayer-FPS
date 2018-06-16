using System.Collections;
using UnityEngine;
using DatabaseControl;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public static string LoggedIn_Username { get; protected set; }
    public static string LoggedIn_Password { get protected set; }
}
