using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class LoginValidation : MonoBehaviour
{
    TMP_InputField username, password;

    public UnityEvent OnCredentialsValidated;
    public UnityEvent OnCredentialsInvalidated;

    public void Validate()
    {
        //Greyboxing login functionality.
        if (username.text != string.Empty && password.text != string.Empty)
        {
            OnCredentialsValidated?.Invoke();
        }
        else
        {
            OnCredentialsInvalidated?.Invoke();
        }
    }
}
