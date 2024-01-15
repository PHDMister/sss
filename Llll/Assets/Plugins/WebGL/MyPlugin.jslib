var MyPlugin = {
  StringReturnValueFunction: function () {
    var returnStr = window.top.location.href;
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  
  },
  SetTopFun:function()
  {
    var strA="平台值：";
    var ua = navigator.userAgent.toLowerCase();
	
	if(ua.match(/android/i) == "android")
	{
	  strA="android";
	  window["AIEraJsHandler"].updateHeaderbarShow(JSON.stringify({show:0}));
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  strA="IOS";
	   var  data = {
                "methord":"updateHeaderbarShow:",
                "param":{show:"0"},
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
	//var bufferB = _malloc(lengthBytesUTF8(strA) + 1);
    //writeStringToMemory(strA, bufferB);
	return bufferB;
  },
    ShowTopFun:function()
  {
    var strA="平台值：";
    var ua = navigator.userAgent.toLowerCase();
	
	if(ua.match(/android/i) == "android")
	{
	  strA="android";
	  window["AIEraJsHandler"].showNavigateStatusBar();
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  strA="IOS";
	   var  data = {
                "methord":"showNavigateStatusBar:",
                "param":null,
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
	//var bufferB = _malloc(lengthBytesUTF8(strA) + 1);
    //writeStringToMemory(strA, bufferB);
	//return bufferB;
  },
  SetLandscapeModeFun:function()
  {
   var ua = navigator.userAgent.toLowerCase();
  if(ua.match(/android/i) == "android")
	{
	  window["AIEraJsHandler"].changeScreenOrientation(JSON.stringify({orientation:0}));
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	   
	   var  data = {
                "methord":"changeScreenOrientation:",
                "param":{orientation:"0"},
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  
   SetPortraitModeFun:function()
  {
   var ua = navigator.userAgent.toLowerCase();
  if(ua.match(/android/i) == "android")
	{
	  window["AIEraJsHandler"].changeScreenOrientation(JSON.stringify({orientation:1}));
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	   
	   var  data = {
                "methord":"changeScreenOrientation:",
                "param":{orientation:"1"},
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  
  GetWindowWidth:function()
  {
     var width = window.screen.availWidth
	 //var bufferC = _malloc(lengthBytesUTF8(width) + 1);
    // writeStringToMemory(width, bufferC);
     return width;
  },
  GetWinddowHeight:function()
  {
    //浏览器可用高度，减去工具栏、地址输入框等
      var height = window.screen.availHeight;
	  //var bufferD = _malloc(lengthBytesUTF8(height) + 1);
      // writeStringToMemory(height, bufferD);
	  return height;
  },
  ResetCanvasSize:function(width,height)
  {
  //修改画布大小 unity-canvas
  
  	document.getElementById("unity-canvas").style.width = width ;
	document.getElementById("unity-canvas").style.height = height;
  
  },
  
  ResetABCanvasSize:function(width,height)
  {
  //修改画布大小 unity-canvas
 console.log(" 输出 width："+width+"   height: "+height);
  	document.getElementById("unity-canvas").width = width ;
	document.getElementById("unity-canvas").height = height;
  
  },
  
  
  getASize:function()
  {
  //修改画布大小 unity-canvas
  
  var a=	document.getElementById("unity-canvas").style.width  ;
	 
  return a;
  },
  
  getBSize:function( )
  {
  //修改画布大小 unity-canvas
  
  	 
	var b=document.getElementById("unity-canvas").style.height  ;
  return b;
  },
  
  
  CloseGameFun:function()
  {
  var ua = navigator.userAgent.toLowerCase();
	
	if(ua.match(/android/i) == "android")
	{
	  
	  window["AIEraJsHandler"].finish();
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  
	   var  data = {
                "methord":"finish:",
                "param":null,
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  GoToRainbowBazaar:function(url)
  {
  url=Pointer_stringify(url);
   console.log(" plugin URL :  "+url);
  var ua = navigator.userAgent.toLowerCase();
	
	if(ua.match(/android/i) == "android")
	{
	  
	  window["AIEraJsHandler"].openUrl(url);
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  
	   var  data = {
                "methord":"openUrl:",
                "param":url,
            };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  OpenWebUrl:function(url)
  {
   url=Pointer_stringify(url);
   console.log(" plugin URL :  "+url);
   var ua = navigator.userAgent.toLowerCase();
   window.location.href=url;
  },
  
  ShowTopBarFun:function()
  {
    var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	 window["AIEraJsHandler"].updateHeaderbarShow(JSON.stringify({show:1}));
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"updateHeaderbarShow:",
            "param":{show:"1"},
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  CloseTopBarFun:function()
  {
    var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	 window["AIEraJsHandler"].updateHeaderbarShow(JSON.stringify({show:0}));
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"updateHeaderbarShow:",
            "param":{show:"0"},
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  HideKeyboardFun:function()
  {
    var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	 window["AIEraJsHandler"].hideKeyboard();
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"hideKeyboard:",
            "param": null,
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  showVideoPlayer:function() 
  {
	  document.getElementById("player-con-wrapper").style.display = "block";
	  var player = new Aliplayer(
          {
            id: "player-con",
            source:
              "artc://pull.hotdogapp.net/hotdogapp/hotdogapp?auth_key=1697531300-0-0-2f3cd3726de179ce843ab903d3079a20",
            width: "100%",
            height: "100vh",
            // cover: 'https://img.alicdn.com/tps/TB1EXIhOFXXXXcIaXXXXXXXXXXX-760-340.jpg',
            autoplay: true,
            preload: true,
            isLive: true,
          },
          function (player) {
            console.log("The player is created");
          }
        );
  },
  OpenChatWindow:function()
  {
    var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	 window["AIEraJsHandler"].openChatWindow("");
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"openChatWindow:",
            "param": null,
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);
	}
  },
  GetOperationPlatformFun:function()
  {
    var ua = navigator.userAgent.toLowerCase();
	var ipad= ua.match(/ipad/i) == "ipad";
	var iphone= ua.match(/iphone os/i) == "iphone os";
	var midp= ua.match(/midp/i) == "midp";
	var uc7= ua.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
	var uc= ua.match(/ucweb/i) == "ucweb";
	var android= ua.match(/android/i) == "android";
	var windowsce= ua.match(/windows ce/i) == "windows ce";
	var windowsmd=ua.match(/windows mobile/i) == "windows mobile";
	
	if (!(ipad || iphone || midp || uc7 || uc || android || windowsce || windowsmd)) {
        // PC 端  返回1
        return 1;
    }else{
        // 移动端  返回2
          return 2;
    }
  },
  
  //获取当前环境平台类型
  GetEnvironmentPlatformFun:function()
  {
	var ua = navigator.userAgent.toLowerCase();	
	
	var ipad = ua.match(/ipad/i) == "ipad";
	if(ipad) return 1;
	
	var iphone = ua.match(/iphone os/i) == "iphone os";
	if(iphone) return 2;
	
	var android = ua.match(/android/i) == "android";
	if(android) return 3;
	
	var windowsce = ua.match(/windows ce/i) == "windows ce";
	if(windowsce) return 4;
	
	var windowsmd = ua.match(/windows mobile/i) == "windows mobile";
	if(windowsmd) return 5;
	
	var midp = ua.match(/midp/i) == "midp";
	var uc7 = ua.match(/rv:1.2.3.4/i) == "rv:1.2.3.4";
	var uc = ua.match(/ucweb/i) == "ucweb";
	if(!(midp || uc7 || uc)) return 6;
	
	return 7;
  },
  
  //网络是否可用
  GetNetworkAvailableFun:function()
  {
	var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	  var isAcailble = window.AIEraJsHandler.isNetworkAvailable();
	  if(isAcailble) return 1;
	  return 0;
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"isNetworkAvailable:",
            "param": null,
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);	
      return window.mynetworkavailable;		
	}
	return 1;
  }, 
	
  //app是否在前台
  GetAppIsForegroundFun:function()
  {
	var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	  var isForeground = window.AIEraJsHandler.isForeground();
	  if(isForeground) return 1;
	  return 0;
	}
	else if(ua.match(/iphone os/i) == "iphone os")
	{
	  var data = {
            "methord":"isForeground:",
            "param": null,
        };
	  window["webkit"].messageHandlers.AIEraJsHandler.postMessage(data);	
	  return window.myforegroundState;
	}
	return 1;
  },
  
  //开启或隐藏系统状态拦 目前适用于Android
  ChangeSystemStatusBarFun:function(isShow)
  {
	var ua = navigator.userAgent.toLowerCase();
	if(ua.match(/android/i) == "android")
	{
	   window.AIEraJsHandler.changeSystemStatusBar(isShow);
	}
  },
  
  //初始化字段
  InitWindowCustomPropDefine:function()
  {
	window.mynetworkavailable = 1;
	window.myforegroundState = 1;
	window.callbackIsNetworkAvailable = function(sCode)
	{
		window.mynetworkavailable = sCode;
	}
	window.callbackIsForeground = function(fCode)
	{
		window.myforegroundState = fCode;
	}
  },
  
 ImUtilIns:function()
  {
	ImUtil.getInstance().setOption({
		onMessageChange:function(msg) 
		{
			// gameInstance.then((unityInstance) => {
			// 	unityInstance.SendMessage("Main Camera","OnMessageChange",msg);        
			// });
			myGameInstance.SendMessage("Main Camera","OnMessageChange",msg);
			console.log("ImUtilIns	onMessageChange message: " + msg);
		},
		onKicked:function(reason)
		{
			// gameInstance.then((unityInstance) => {
			// 	unityInstance.SendMessage("Main Camera","OnKicked",reason);        
			// });
			myGameInstance.SendMessage("Main Camera","OnKicked",reason);
			console.log("ImUtilIns	onKicked reason: " + reason);
		},
		onWillReconnect:function(reconnect)
		{
			// gameInstance.then((unityInstance) => {
			// 	unityInstance.SendMessage("Main Camera","OnWillReconnect",reconnect);
			// });
			myGameInstance.SendMessage("Main Camera","OnWillReconnect",reconnect);
			console.log("ImUtilIns	onWillReconnect	reconnect: " + reconnect)
		},
		onBroadcastMsg:function(msg)
		{
			myGameInstance.SendMessage("Main Camera","OnBroadcastMsg",msg);
			console.log("ImUtilIns	onBroadcastMsg	msg: " + msg)
		},
		onLoginCall: function(msg)
		{
			console.log("ImUtilIns	onLoginCall	msg: " + msg);
			myGameInstance.SendMessage("Main Camera","OnLoginCall",msg);
		},
		isOpenLog: true,//打开日志开关
		openAntispam: false,//打开反垃圾词库
	});
  },

 //初始化
  ImInit:function(appKey,token,account)
  {
	appKey = Pointer_stringify(appKey);
	token = Pointer_stringify(token);
	account = Pointer_stringify(account);
	ImUtil.getInstance().init(appKey,token,account);
  },

  // 更新用户信息
  ImUpdateUserInfo:function(nickname,avatar)
  {
	nickname = Pointer_stringify(nickname);
	avatar = Pointer_stringify(avatar);
	ImUtil.getInstance().updateUserInfo(nickname,avatar,function (isSuccess,msg){
		console.log(isSuccess ? "updateUserInfo Success" : "updateUserInfo Fail");
		console.log(msg);
		if(isSuccess)
		{
			myGameInstance.SendMessage("Main Camera","OnUpdateUserInfo",msg);
		}
	});
  },

  //进入群
  ImInChatRoom:function(roomId,roomAddress)
  {
	roomId = Pointer_stringify(roomId);
	roomAddress = Pointer_stringify(roomAddress);
	ImUtil.getInstance().inChatRoom(roomId,roomAddress);
  },

  //退出群的时候需要调用
  ImDisconnect:function()
  {
	ImUtil.getInstance().disconnect(function (isSuccess, msg) {
		console.log(isSuccess ? ("disconnect Success " + msg) : ("disconnect Fail " + msg));
		if(isSuccess)
		{
			ImUtil.getInstance().destroy(function (isSuccess,msg)
			{
				console.log(isSuccess ? ("destroy Success " + msg) : ("destroy Fail " + msg));
				if(isSuccess)
				{
					myGameInstance.SendMessage("Main Camera","OnDisconnect","");
				}
			});
		}
	});
  },

  ImConnect:function()
  {
	ImUtil.getInstance().connect(function (isSuccess, msg) {
		console.log(isSuccess ? ("connect Success " + msg) : ("connect Fail " + msg));
		if(isSuccess)
		{
			myGameInstance.SendMessage("Main Camera","OnConnect",msg);
		}		
	});
  },

  // 发送消息
  ImSendTextMessage:function(msg,toUId,custom)
  {
	msg = Pointer_stringify(msg);
	toUId = Pointer_stringify(toUId);
	custom=Pointer_stringify(custom);
	ImUtil.getInstance().sendTextMessage(msg, function (isSuccess, msg) {
		if(isSuccess)
		{
			myGameInstance.SendMessage("Main Camera","OnSendTextMessage",msg);
		}
		console.log(isSuccess ? "sendTextMessage Success" : "sendTextMessage Fail");
	}, false, "",toUId,custom);
  },

  //退出网页的时候断开连接
  ImOutLogin:function()
  {
	  ImUtil.getInstance().outLogin(function (isSuccess, msg) {

	});
  },

	//获取用户资料
	ImGetUserInfo:function(account)
	{
		account = Pointer_stringify(account);
		ImUtil.getInstance().getUser(account,function(isSuccess,msg)
		{
			if(isSuccess)
			{
				myGameInstance.SendMessage("Main Camera","OnGetUserInfo",msg);
			}
			console.log(isSuccess ? "getUser Success " + msg : "getUser Fail " + msg);
		});
	},
};
mergeInto(LibraryManager.library, MyPlugin);
