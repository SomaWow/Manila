using MyServer;
using MySql.Data.MySqlClient;
using Protocol.Code.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    public class DatabaseManager
    {
        private static Dictionary<int, ClientPeer> idClientDic;
        private static MySqlConnection sqlConnect;
        /// <summary>
        /// 连接数据库
        /// </summary>
        public static void StartConnect()
        {
            idClientDic = new Dictionary<int, ClientPeer>();
            string conStr = "database=manila;data source=127.0.0.1;port=3306;User=root;pwd=liman233";
            sqlConnect = new MySqlConnection(conStr);
            sqlConnect.Open();
        }
        /// <summary>
        /// 判断用户名是否已经注册
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsExistUserName(string userName)
        {
            MySqlCommand cmd = new MySqlCommand("select UserName from user where UserName=@name", sqlConnect);
            cmd.Parameters.AddWithValue("name", userName);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool result = reader.HasRows;
            reader.Close();
            return result;
        }
        /// <summary>
        /// 注册新的账户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        public static void CreateUser(string userName, string pwd)
        {
            MySqlCommand cmd = new MySqlCommand("insert into user set UserName=@name, Password=@pwd,Online=0,IconName='default',Win=0,Lose=0",sqlConnect);
            cmd.Parameters.AddWithValue("name", userName);
            cmd.Parameters.AddWithValue("pwd", pwd);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 用户名和密码是否匹配
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        public static bool IsMatch(string userName, string pwd)
        {
            MySqlCommand cmd = new MySqlCommand("select * from user where UserName=@name", sqlConnect);
            cmd.Parameters.AddWithValue("name", userName);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                bool result = reader.GetString("Password") == pwd;
                reader.Close();
                return result;
            }
            reader.Close();
            return false;
        }
        /// <summary>
        /// 判断用户是否登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsOnline(string userName)
        {
            MySqlCommand cmd = new MySqlCommand("select Online from user where UserName=@name", sqlConnect);
            cmd.Parameters.AddWithValue("name", userName);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                bool result = reader.GetBoolean("Online");
                reader.Close();
                return result;
            }
            reader.Close();
            return false;
        }
        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="client"></param>
        public static void Login(string userName, ClientPeer client)
        {
            MySqlCommand cmd = new MySqlCommand("update user set Online=true where UserName=@name", sqlConnect);
            cmd.Parameters.AddWithValue("name", userName);
            cmd.ExecuteNonQuery();

            MySqlCommand cmd1 = new MySqlCommand("select * from user where UserName=@name", sqlConnect);
            cmd1.Parameters.AddWithValue("name", userName);
            MySqlDataReader reader = cmd1.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                int id = reader.GetInt32("Id");
                client.Id = id;
                client.UserName = userName;
                if(idClientDic.ContainsKey(id) == false)
                {
                    idClientDic.Add(id, client);
                }
                reader.Close();
            }
        }
        public static void OffLine(ClientPeer client)
        {
            if (idClientDic.ContainsKey(client.Id))
            {
                idClientDic.Remove(client.Id);
            }
            MySqlCommand cmd = new MySqlCommand("update user set Online=false where Id=@id", sqlConnect);
            cmd.Parameters.AddWithValue("id", client.Id);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 使用用户id获得客户端连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ClientPeer GetClientPeerByUserId(int id)
        {
            if(idClientDic.ContainsKey(id))
            {
                return idClientDic[id];
            }
            return null;
        }
        /// <summary>
        /// 构造用户信息传输模型
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserDto CreateUserDto(int userId)
        {
            MySqlCommand cmd = new MySqlCommand("select * from user where Id=@id", sqlConnect);
            cmd.Parameters.AddWithValue("id", userId);
            MySqlDataReader reader = cmd.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                UserDto dto = new UserDto(userId, reader.GetString("UserName"), reader.GetString("IconName"),reader.GetInt32("Win"),reader.GetInt32("Lose"));
                reader.Close();
                return dto;
            }
            reader.Close();
            return null;
        }
        /// <summary>
        /// 第一次登陆选择头像
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="iconName"></param>
        public static void ChooseHeadIcon(string userName, string iconName)
        {
            MySqlCommand cmd = new MySqlCommand("update user set IconName=@iconName where UserName=@name", sqlConnect);
            cmd.Parameters.AddWithValue("iconName", iconName);
            cmd.Parameters.AddWithValue("name", userName);
            cmd.ExecuteNonQuery();
        }

        public static void Win(int userId)
        {
            MySqlCommand cmd = new MySqlCommand("update user set Win=Win+1 where Id=@userId", sqlConnect);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.ExecuteNonQuery();
        }

        public static void Lose(int userId)
        {
            MySqlCommand cmd = new MySqlCommand("update user set Lose=Lose+1 where Id=@userId", sqlConnect);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.ExecuteNonQuery();
        }
    }
}
