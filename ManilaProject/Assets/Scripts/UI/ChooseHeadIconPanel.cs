using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseHeadIconPanel : MonoBehaviour {

    private Button headIcon1;
    private Button headIcon2;
    private Button headIcon3;
    private Button headIcon4;
    private Button headIcon5;
    private Button headIcon6;
    private Button headIcon7;
    private Button headIcon8;

    private void Awake()
    {
        gameObject.SetActive(false);
        EventCenter.AddListener(EventType.ChooseHeadIcon, ChooseHeadIcon);
        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ChooseHeadIcon, ChooseHeadIcon);
    }
    private void Init()
    {
        headIcon1 = transform.Find("btn1").GetComponent<Button>();
        headIcon2 = transform.Find("btn2").GetComponent<Button>();
        headIcon3 = transform.Find("btn3").GetComponent<Button>();
        headIcon4 = transform.Find("btn4").GetComponent<Button>();
        headIcon5 = transform.Find("btn5").GetComponent<Button>();
        headIcon6 = transform.Find("btn6").GetComponent<Button>();
        headIcon7 = transform.Find("btn7").GetComponent<Button>();
        headIcon8 = transform.Find("btn8").GetComponent<Button>();

        headIcon1.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_1"); });
        headIcon2.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_2"); });
        headIcon3.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_3"); });
        headIcon4.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_4"); });
        headIcon5.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_5"); });
        headIcon6.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_6"); });
        headIcon7.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_7"); });
        headIcon8.onClick.AddListener(delegate() { OnImageButtonClick("headIcon_8"); });

    }

    private void ChooseHeadIcon()
    {
        gameObject.SetActive(true);
    }
    private void OnImageButtonClick(string IconName)
    {
        gameObject.SetActive(false);
        Models.GameModel.userDto.IconName = IconName;
        EventCenter.Broadcast(EventType.BackPersonalInterface);
        //这里把头像名字放到密码里面传给服务器的
        AccountDto dto = new AccountDto(Models.GameModel.userDto.UserName, IconName);
        NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.ChooseHeadIcon_CREQ, dto);
    }
}
