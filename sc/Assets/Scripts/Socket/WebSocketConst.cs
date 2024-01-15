using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSocketConst
{
    private static uint _client_code = 0;
    public static uint Client_Code
    {
        get
        {
            if (_client_code + 1 >= int.MaxValue) _client_code = 0;
            _client_code++;
            return _client_code;
        }
    }

    public const string WsNet_OnFirstOpen = "WsNet_OnFirstOpen";
    public const string WsNet_OnReConnect = "WsNet_OnReConnect";
    public const string WsNet_OnDisConnect = "WsNet_OnDisConnect";
    public const string WsNet_OnReConnectFail = "WsNet_OnReConnectFail";
}
