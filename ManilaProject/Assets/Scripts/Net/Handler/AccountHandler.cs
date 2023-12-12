using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case AccountCode.Register_SRES:
                Register_SRES((int)value);
                break;
            case AccountCode.Login_SRES:
                Login_SRES((int)value);
                break;
            case AccountCode.GetUserInfo_SRES:
                Models.GameModel.userDto = (UserDto)value;
                //跳转场景
                SceneManager.LoadScene("2.PersonalInterface");
                break;
            default:
                break;
        }
    }
    private void Register_SRES(int value)
    {
        if (value == -1)
        {
            EventCenter.Broadcast(EventType.Hint, "用户名已被注册");
            return;
        }
        if (value == 0)
        {
            EventCenter.Broadcast(EventType.Hint, "注册成功");
            return;
        }
    }
    private void Login_SRES(int value)
    {
        if(value == -1)
        {
            EventCenter.Broadcast(EventType.Hint, "用户名不存在");
        }
        else if(value == -2)
        {
            EventCenter.Broadcast(EventType.Hint, "密码不正确");
        }
        else if (value == -3)
        {
            EventCenter.Broadcast(EventType.Hint, "该账户已在线");
        }
        else if (value == 0)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.GetUserInfo_CREQ, null);
            EventCenter.Broadcast(EventType.Hint, "登陆成功");
        }
    }

}
