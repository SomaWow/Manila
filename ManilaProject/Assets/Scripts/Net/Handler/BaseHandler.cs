using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHandler {

    public abstract void OnReceive(int subCode, object value);
}
