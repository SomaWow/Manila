using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour {
    private InputField input_Username;
    private InputField input_Password;
    private Button btn_Login;
    private Button btn_Register;
    private Button btn_Hide;
    private bool isShowPassword = false;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowLoginPanel, Show);
        Init();

    }
    private void Init()
    {
        input_Username = transform.Find("input_Username").GetComponent<InputField>();
        input_Password = transform.Find("input_Password").GetComponent<InputField>();
        btn_Login = transform.Find("btn_Login").GetComponent<Button>();
        btn_Login.onClick.AddListener(OnLoginButtonClick);
        btn_Register = transform.Find("btn_Register").GetComponent<Button>();
        btn_Register.onClick.AddListener(OnRegisterButtonClick); //检测点击事件
        btn_Hide = transform.Find("btn_Hide").GetComponent<Button>();
        btn_Hide.onClick.AddListener(OnHideButtonClick);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowLoginPanel, Show);
    }
    /// <summary>
    /// 密码显示或隐藏
    /// </summary>
    private void OnHideButtonClick()
    {
        isShowPassword = !isShowPassword;
        if (isShowPassword)
        {
            input_Password.contentType = InputField.ContentType.Standard;
            btn_Hide.GetComponentInChildren<Text>().text = "隐藏";
        }
        else
        {
            input_Password.contentType = InputField.ContentType.Password;
            btn_Hide.GetComponentInChildren<Text>().text = "显示";
        }
        EventSystem.current.SetSelectedGameObject(input_Password.gameObject);
    }
    /// <summary>
    /// 点击注册按钮
    /// </summary>
    private void OnRegisterButtonClick()
    {
        gameObject.SetActive(false);
        EventCenter.Broadcast(EventType.ShowRegisterPanel);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 点击登陆按钮
    /// </summary>
    private void OnLoginButtonClick()
    {
        if (input_Username.text == null || input_Username.text == "")
        {
            EventCenter.Broadcast(EventType.Hint, "请输入用户名");
            return;
        }
        if (input_Password.text == null || input_Password.text == "")
        {
            EventCenter.Broadcast(EventType.Hint, "请输入密码");
            return;
        }
        //向服务器发送消息，登陆
        AccountDto dto = new AccountDto(input_Username.text, input_Password.text);
        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.Login_CREQ, dto);

    }
}
