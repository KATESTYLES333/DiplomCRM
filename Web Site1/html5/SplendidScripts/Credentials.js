/* Copyright (C) 2011-2014 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var sREMOTE_SERVER    = '';
var sAUTHENTICATION   = '';
var sUSER_NAME        = '';
var sPASSWORD         = '';
var sUSER_ID          = '';
var sUSER_LANG        = 'en-US';
// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
var sUSER_THEME       = 'Atlantic';
var sUSER_DATE_FORMAT = 'MM/dd/yyyy';
var sUSER_TIME_FORMAT = 'h:mm tt';
var sUSER_CURRENCY_ID = 'E340202E-6291-4071-B327-A34CB4DF239B';
var sUSER_TIMEZONE_ID = 'BFA61AF7-26ED-4020-A0C1-39A15E4E9E0A';
var sFULL_NAME        = '';
// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
var sPICTURE          = '';
var sTEAM_ID          = '';
var sTEAM_NAME        = '';
var bIS_OFFLINE       = false;
var bENABLE_OFFLINE   = false;
var cbNetworkStatusChanged = null;
// 11/25/2014 Paul.  Add SignalR fields. 
var sUSER_EXTENSION      = '';
var sUSER_FULL_NAME      = '';
var sUSER_PHONE_WORK     = '';
var sUSER_SMS_OPT_IN     = '';
var sUSER_PHONE_MOBILE   = '';
var sUSER_TWITTER_TRACKS = '';
var sUSER_CHAT_CHANNELS  = '';

function GetIsOffline()
{
	return bIS_OFFLINE;
}

function GetEnableOffline()
{
	return bENABLE_OFFLINE;
}

// 12/09/2014 Paul.  Remote Server is on the background page of the browser extensions. 
function RemoteServer()
{
	return sREMOTE_SERVER;
}

function LoadCredentials()
{
	try
	{
		//alert('LoadCredentials');
		sREMOTE_SERVER    = (localStorage['REMOTE_SERVER'   ] !== undefined) ? localStorage['REMOTE_SERVER'   ] : '';
		sAUTHENTICATION   = (localStorage['AUTHENTICATION'  ] !== undefined) ? localStorage['AUTHENTICATION'  ] : '';
		sUSER_NAME        = (localStorage['USER_NAME'       ] !== undefined) ? localStorage['USER_NAME'       ] : '';
		sFULL_NAME        = (localStorage['FULL_NAME'       ] !== undefined) ? localStorage['FULL_NAME'       ] : '';
		// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
		sPICTURE          = (localStorage['PICTURE'         ] !== undefined) ? localStorage['PICTURE'         ] : '';
		sUSER_LANG        = (localStorage['USER_LANG'       ] !== undefined) ? localStorage['USER_LANG'       ] : '';
		// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
		sUSER_THEME       = (localStorage['USER_THEME'      ] !== undefined) ? localStorage['USER_THEME'      ] : '';
		sUSER_DATE_FORMAT = (localStorage['USER_DATE_FORMAT'] !== undefined) ? localStorage['USER_DATE_FORMAT'] : '';
		sUSER_TIME_FORMAT = (localStorage['USER_TIME_FORMAT'] !== undefined) ? localStorage['USER_TIME_FORMAT'] : '';
		sUSER_CURRENCY_ID = (localStorage['USER_CURRENCY_ID'] !== undefined) ? localStorage['USER_CURRENCY_ID'] : '';
		sUSER_TIMEZONE_ID = (localStorage['USER_TIMEZONE_ID'] !== undefined) ? localStorage['USER_TIMEZONE_ID'] : '';
		sPASSWORD         = (localStorage['PASSWORD'        ] !== undefined) ? localStorage['PASSWORD'        ] : '';
		// 12/01/2014 Paul.  Add SignalR fields. 
		sUSER_EXTENSION      = (localStorage['USER_EXTENSION'     ] !== undefined) ? localStorage['USER_EXTENSION'     ] : '';
		sUSER_FULL_NAME      = (localStorage['USER_FULL_NAME'     ] !== undefined) ? localStorage['USER_FULL_NAME'     ] : '';
		sUSER_PHONE_WORK     = (localStorage['USER_PHONE_WORK'    ] !== undefined) ? localStorage['USER_PHONE_WORK'    ] : '';
		sUSER_SMS_OPT_IN     = (localStorage['USER_SMS_OPT_IN'    ] !== undefined) ? localStorage['USER_SMS_OPT_IN'    ] : '';
		sUSER_PHONE_MOBILE   = (localStorage['USER_PHONE_MOBILE'  ] !== undefined) ? localStorage['USER_PHONE_MOBILE'  ] : '';
		sUSER_TWITTER_TRACKS = (localStorage['USER_TWITTER_TRACKS'] !== undefined) ? localStorage['USER_TWITTER_TRACKS'] : '';
		sUSER_CHAT_CHANNELS  = (localStorage['USER_CHAT_CHANNELS' ] !== undefined) ? localStorage['USER_CHAT_CHANNELS' ] : '';


		if ( sREMOTE_SERVER !== undefined && sREMOTE_SERVER.length > 0 && Right(sREMOTE_SERVER.value, 1) != '/' )
			sREMOTE_SERVER.value += '/';
		try
		{
			if ( sPASSWORD !== undefined && sPASSWORD.length > 0 )
				sPASSWORD = Aes.Ctr.decrypt(sPASSWORD, 'Splendid', 256)
		}
		catch(e)
		{
			sPASSWORD = '';
		}
	}
	catch(e)
	{
		alert('LoadCredentials ' + e.message);
	}
}

function SaveCredentials(sRemoteServer, sAuthentication, sUserName, sPassword)
{
	try
	{
		//alert('SaveCredentials ' + sRemoteServer + ' ' + sAuthentication + ' ' + sUserName + ' ' + sPassword);
		sREMOTE_SERVER  = sRemoteServer  ;
		sAUTHENTICATION = sAuthentication;
		sUSER_NAME      = sUserName      ;
		sPASSWORD       = sPassword      ;
		localStorage['REMOTE_SERVER'   ] = sREMOTE_SERVER   ;
		localStorage['AUTHENTICATION'  ] = sAUTHENTICATION  ;
		localStorage['USER_NAME'       ] = sUSER_NAME       ;
		localStorage['FULL_NAME'       ] = sFULL_NAME       ;
		// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
		localStorage['PICTURE'         ] = sPICTURE         ;
		localStorage['USER_LANG'       ] = sUSER_LANG       ;
		// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
		localStorage['USER_THEME'      ] = sUSER_THEME      ;
		localStorage['USER_DATE_FORMAT'] = sUSER_DATE_FORMAT;
		localStorage['USER_TIME_FORMAT'] = sUSER_TIME_FORMAT;
		localStorage['USER_CURRENCY_ID'] = sUSER_CURRENCY_ID;
		localStorage['USER_TIMEZONE_ID'] = sUSER_TIMEZONE_ID;
		if ( sPASSWORD !== undefined && sPASSWORD.length > 0 )
			localStorage['PASSWORD'      ] = Aes.Ctr.encrypt(sPASSWORD, 'Splendid', 256);
		else
			localStorage['PASSWORD'      ] = sPASSWORD      ;
		// 08/30/2011 Paul.  Clear the user after changing credentials. 
		sUSER_ID          = '';
		sUSER_LANG        = 'en-US';
		sUSER_DATE_FORMAT = 'MM/dd/yyyy';
		sUSER_TIME_FORMAT = 'h:mm tt';
		sUSER_THEME       = 'Atlantic';
		sUSER_CURRENCY_ID = 'E340202E-6291-4071-B327-A34CB4DF239B';
		sUSER_TIMEZONE_ID = 'BFA61AF7-26ED-4020-A0C1-39A15E4E9E0A';
		
		// 12/01/2014 Paul.  Add SignalR fields. 
		localStorage['USER_EXTENSION'     ] = sUSER_EXTENSION     ;
		localStorage['USER_FULL_NAME'     ] = sUSER_FULL_NAME     ;
		localStorage['USER_PHONE_WORK'    ] = sUSER_PHONE_WORK    ;
		localStorage['USER_SMS_OPT_IN'    ] = sUSER_SMS_OPT_IN    ;
		localStorage['USER_PHONE_MOBILE'  ] = sUSER_PHONE_MOBILE  ;
		localStorage['USER_TWITTER_TRACKS'] = sUSER_TWITTER_TRACKS;
		localStorage['USER_CHAT_CHANNELS' ] = sUSER_CHAT_CHANNELS ;
		sUSER_EXTENSION      = '';
		sUSER_FULL_NAME      = '';
		sUSER_PHONE_WORK     = '';
		sUSER_SMS_OPT_IN     = '';
		sUSER_PHONE_MOBILE   = '';
		sUSER_TWITTER_TRACKS = '';
		sUSER_CHAT_CHANNELS  = '';
	}
	catch(e)
	{
		alert('SaveCredentials ' + e.message);
	}
}

function ValidateCredentials()
{
	if ( sREMOTE_SERVER === undefined || sREMOTE_SERVER.length == 0 )
	{
		//alert('ValidateCredentials sREMOTE_SERVER is invalid ' + sREMOTE_SERVER);
		return false;
	}
	if ( sAUTHENTICATION === undefined || (sAUTHENTICATION != 'CRM' && sAUTHENTICATION != 'Basic' && sAUTHENTICATION != 'Windows') )
	{
		//alert('ValidateCredentials sAUTHENTICATION is invalid ' + sAUTHENTICATION);
		return false;
	}
	return true;
}


