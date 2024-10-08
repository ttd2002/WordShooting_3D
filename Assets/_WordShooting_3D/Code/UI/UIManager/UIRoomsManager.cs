using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomsManager : WMonoBehaviour
{
    [SerializeField] private GameObject textFieldPwd;
    public GameObject TextFieldPwd => textFieldPwd;
    [SerializeField] private GameObject textFieldPwdJoin;
    public GameObject TextFieldPwdJoin => textFieldPwdJoin;

    public string roomName;
    public string roomPassword;
    public void OpenTextFieldPwd()
    {
        this.textFieldPwd.SetActive(true);
    }
    public void CloseTextFieldPwd()
    {
        this.textFieldPwd.SetActive(false);
    }
    public void OpenTextFieldPwdJoin()
    {
        this.textFieldPwdJoin.SetActive(true);
    }
    public void CloseTextFieldPwdJoin()
    {
        this.textFieldPwdJoin.SetActive(false);
    }
    public void GetRoomName(string sessionName)
    {
        roomName = sessionName;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadTextFieldPass();
        this.LoadTextFieldPassJoin();
    }

    protected virtual void LoadTextFieldPass()
    {
        if (this.textFieldPwd != null) return;
        this.textFieldPwd = transform.Find("TextfieldPass").gameObject;
        Debug.Log(transform.name + ": LoadTextFieldPass", gameObject);
    }
    protected virtual void LoadTextFieldPassJoin()
    {
        if (this.textFieldPwdJoin != null) return;
        this.textFieldPwdJoin = transform.Find("TextfieldPassJoin").gameObject;
        Debug.Log(transform.name + ": LoadTextFieldPassJoin", gameObject);
    }

}
