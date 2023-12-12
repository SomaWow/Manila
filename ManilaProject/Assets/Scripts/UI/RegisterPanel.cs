using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisterPanel : MonoBehaviour {

    private InputField input_Username;
    private InputField input_Password;
    private Button btn_Register;
    private Button btn_Back;
    private Button btn_Hide;
    private bool isShowPassword = false;

    private void Awake()
    {
        EventCenter.AddListener(EventType.ShowRegisterPanel, Show);
        Init();
        
    }

    private void Init()
    {
        input_Username = transform.Find("input_Username").GetComponent<InputField>();
        input_Password = transform.Find("input_Password").GetComponent<InputField>();
        btn_Register = transform.Find("btn_Register").GetComponent<Button>();
        btn_Register.onClick.AddListener(OnRegisterButtonClick);
        btn_Back = transform.Find("btn_Back").GetComponent<Button>();
        btn_Back.onClick.AddListener(OnBackButtonClick);
        btn_Hide = transform.Find("btn_Hide").GetComponent<Button>();
        btn_Hide.onClick.AddListener(OnHideButtonClick);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowRegisterPanel, Show);
    }
    /// <summary>
    /// 密码显示或隐藏
    /// </summary>
    private void OnHideButtonClick()
    {
        isShowPassword = !isShowPassword;
        if(isShowPassword)
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
    /// 返回按钮
    /// </summary>
    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        EventCenter.Broadcast(EventType.ShowLoginPanel);
    }
    /// <summary>
    /// 注册按钮
    /// </summary>
    private void OnRegisterButtonClick()
    {
        if(input_Username.text == null || input_Username.text == "")
        {
            EventCenter.Broadcast(EventType.Hint, "请输入用户名");
            return;
        }
        if (input_Password.text == null || input_Password.text == "")
        {
            EventCenter.Broadcast(EventType.Hint, "请输入密码");
            return;
        }
        //向服务器发送数据，注册一个用户
        //TODO
        AccountDto dto = new AccountDto(input_Username.text, input_Password.text);
        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.Register_CREQ, dto);

    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

}
