using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class NetMsg
{
    /// <summary>
    /// 操作码
    /// </summary>
    public int opCode { get; set; }
    /// <summary>
    /// 自操作码
    /// </summary>
    public int subCode { get; set; }

    public object value { get; set; }

    public NetMsg()
    {

    }

    public NetMsg(int opCode, int subCode, object value)
    {
        this.opCode = opCode;
        this.subCode = subCode;
        this.value = value;
    }

    public void Change(int opCode, int subCode, object value)
    {
        this.opCode = opCode;
        this.subCode = subCode;
        this.value = value;
    }
}

