using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Database;
using MyServer;
using Protocol.Code;
using Protocol.Code.DTO;

namespace GameServer.Logic
{
    public class AccountHandler : IHandler
    {
        public void Disconnect(ClientPeer client)
        {
            DatabaseManager.OffLine(client);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch(subCode)
            {
                case AccountCode.Register_CREQ:
                    Register(client, value as AccountDto);
                    break;
                case AccountCode.Login_CREQ:
                    Login(client, value as AccountDto);
                    break;
                case AccountCode.GetUserInfo_CREQ:
                    GetUserInfo(client);
                    break;
                case AccountCode.ChooseHeadIcon_CREQ:
                    ChooseHeadIcon(value as AccountDto);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 客户端注册的处理
        /// </summary>
        /// <param name="dto"></param>
        private void Register(ClientPeer client, AccountDto dto)
        {
            SingleExecute.Instance.Execute(()=> {
                if (DatabaseManager.IsExistUserName(dto.userName))
                {
                    //用户已经存在
                    client.SendMsg(OpCode.Account, AccountCode.Register_SRES, -1);
                    return;
                }
                DatabaseManager.CreateUser(dto.userName, dto.password);
                //注册成功
                client.SendMsg(OpCode.Account, AccountCode.Register_SRES, 0);
            });
        }
        /// <summary>
        /// 客户端登陆的请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dto"></param>
        private void Login(ClientPeer client, AccountDto dto)
        {
            SingleExecute.Instance.Execute(() => {
                if (DatabaseManager.IsExistUserName(dto.userName) == false)
                {
                    //用户名不存在
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -1);
                    return;
                }
                if(DatabaseManager.IsMatch(dto.userName, dto.password) == false)
                {
                    //密码不正确
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -2);
                    return;
                }
                if(DatabaseManager.IsOnline(dto.userName))
                {
                    //该账号已经在线
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -3);
                    return;
                }
                //登陆成功
                DatabaseManager.Login(dto.userName, client);
                client.SendMsg(OpCode.Account, AccountCode.Login_SRES, 0);

            });
        }
        /// <summary>
        /// 获得用户所有信息
        /// </summary>
        /// <param name="client"></param>
        private void GetUserInfo(ClientPeer client)
        {
            SingleExecute.Instance.Execute(()=> {
                UserDto dto = DatabaseManager.CreateUserDto(client.Id);
                client.SendMsg(OpCode.Account, AccountCode.GetUserInfo_SRES, dto);
            });
        }
        /// <summary>
        /// 第一次登陆修改头像
        /// </summary>
        private void ChooseHeadIcon(AccountDto dto)
        {
            SingleExecute.Instance.Execute(() => {
                DatabaseManager.ChooseHeadIcon(dto.userName, dto.password); //这里的password存储的是头像
            });
        }
    }
}
