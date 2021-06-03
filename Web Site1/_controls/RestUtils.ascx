<%@ Control Language="c#" AutoEventWireup="false" Codebehind="RestUtils.ascx.cs" Inherits="SplendidCRM._controls.RestUtils" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script runat="server">
/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved.
 * 
 * This program is free software: you can redistribute it and/or modify it under the terms of the 
 * GNU Affero General Public License as published by the Free Software Foundation, either version 3 
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>. 
 * 
 * You can contact SplendidCRM Software, Inc. at email address support@splendidcrm.com. 
 * 
 * In accordance with Section 7(b) of the GNU Affero General Public License version 3, 
 * the Appropriate Legal Notices must display the following words on all interactive user interfaces: 
 * "Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved."
 *********************************************************************************************************************/
</script>
<SplendidCRM:InlineScript runat="server">
	<script type="text/javascript">
// 08/25/2013 Paul.  Move sREMOTE_SERVER definition to the master pages. 
//var sREMOTE_SERVER  = '<%# Application["rootURL"] %>';
// 10/24/2014 Paul.  bREMOTE_ENABLED needs to be in the UI page so that it can be quickly accessed by the Formatting functions. 
var sIMAGE_SERVER = sREMOTE_SERVER;
var bREMOTE_ENABLED = false;
var sAUTHENTICATION = '';
var L10n = new Object();
var TERMINOLOGY = new Object();
var TERMINOLOGY_LISTS = new Object();

L10n.Term = function(sTerm)
{
	if ( TERMINOLOGY[sTerm] === undefined )
		return sTerm;
	return TERMINOLOGY[sTerm];
};

L10n.GetList = function(sListName)
{
	if ( TERMINOLOGY_LISTS[sListName] === undefined )
		return sListName;
	return TERMINOLOGY_LISTS[sListName];
};

L10n.GetListTerms = function(sListName)
{
	if ( TERMINOLOGY_LISTS[sListName] === undefined )
		return sListName;
	return TERMINOLOGY_LISTS[sListName];
};
L10n.ListTerm = function(sLIST_NAME, sNAME)
{
	var sEntryName = '.' + sLIST_NAME + '.' + sNAME;
	return L10n.Term(sEntryName);
}

var Crm     = new Object();
Crm.Config  = new Object();
Crm.Config.ToInteger = function(sName)
{
	return Sql.ToInteger(CONFIG[sName]);
}
Crm.Config.enable_team_management = function()
{
	return Sql.ToBoolean('<%# SplendidCRM.Crm.Config.enable_team_management() %>');
}
Crm.Config.require_team_management = function()
{
	return Sql.ToBoolean('<%# SplendidCRM.Crm.Config.enable_team_management() %>');
}
Crm.Config.enable_dynamic_teams = function()
{
	return Sql.ToBoolean('<%# SplendidCRM.Crm.Config.enable_team_management() %>');
}
Crm.Config.require_user_assignment = function()
{
	return Sql.ToBoolean('<%# SplendidCRM.Crm.Config.require_user_assignment() %>');
}
Crm.Config.enable_speech = function()
{
	return Sql.ToBoolean('<%# Utils.SupportsSpeech && Sql.ToBoolean(Application["CONFIG.enable_speech"]) %>');
}
// 01/18/2015 Paul.  Crm.Modules.TableName is used by SearchViewUI. 
Crm.Modules = new Object();
Crm.Modules.TableName = function(sMODULE)
{
	// 01/18/2015 Paul.  Instead of requiring the MODULES table, just cheat and convert to upper case. 
	switch ( sMODULE )
	{
		case 'ProjectTask':  sMODULE = 'PROJECT_TASK';  break;
	}
	return sMODULE.toUpperCase();
}

var Security = new Object();
Security.USER_ID = function()
{
	return '<%# Security.USER_ID %>';
};
Security.USER_NAME = function()
{
	return '<%# Security.USER_NAME %>';
};
Security.TEAM_ID = function()
{
	return '<%# Security.TEAM_ID %>';
};
Security.USER_TIME_FORMAT = function()
{
	return '<%# Session["USER_SETTINGS/TIMEFORMAT"] %>';
};
Security.USER_DATE_FORMAT = function()
{
	return '<%# Session["USER_SETTINGS/DATEFORMAT"] %>';
};
Security.PICTURE = function()
{
	return '<%# Sql.EscapeJavaScript(Security.PICTURE) %>';
};
// 01/18/2015 Paul.  SplendidError.SystemError() was used on ChatDashboard. 
var sUSER_LANG = '<%# Sql.ToString (Session["USER_SETTINGS/CULTURE"]) %>';
if ( sUSER_LANG == '' )
	sUSER_LANG = 'en-US';

var SplendidError = new Object();
SplendidError.FormatError = function(e, method)
{
	return e.message + '<br>\n' + dumpObj(e, method);
};
// 01/18/2015 Paul.  SplendidError.SystemError() was used on ChatDashboard. 
SplendidError.SystemError = function(e, method)
{
	var message = SplendidError.FormatError(e, method);
	SplendidError.SystemMessage(message);
}
SplendidError.SystemStatus = function(message)
{
}
SplendidError.SystemAlert = function(e, method)
{
	alert(dumpObj(e, method));
};
SplendidError.SystemMessage = function(message)
{
	var divError = document.getElementById('divError');
	divError.innerHTML = message;
};

function DetailViewUI()
{
	this.MODULE  = null;
	this.ID      = null;
}
DetailViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, callback)
{
	window.location.href = sREMOTE_SERVER + sMODULE_NAME + '/view.aspx?ID=' + sID;
}

// 01/18/2015 Paul.  SplendidCache is used by ChatDashboard PopupViewUI. 
var CONFIG                    = new Object();
var GRIDVIEWS_COLUMNS         = new Object();
var EDITVIEWS_FIELDS          = new Object();
var SplendidCache = new Object();
SplendidCache.GridViewColumns = function(sGRID_NAME)
{
	return GRIDVIEWS_COLUMNS[sGRID_NAME];
}
SplendidCache.EditViewFields = function(sEDIT_NAME)
{
	return EDITVIEWS_FIELDS[sEDIT_NAME];
}
SplendidCache.SetGridViewColumns = function(sGRID_NAME, data)
{
	GRIDVIEWS_COLUMNS[sGRID_NAME] = data;
}
SplendidCache.SetEditViewFields = function(sEDIT_NAME, data)
{
	EDITVIEWS_FIELDS[sEDIT_NAME] = data;
}
SplendidCache.Config = function(sName)
{
	if ( CONFIG[sName] === undefined )
		return null;
	return CONFIG[sName];
};

var background = new Object();
background.SplendidCache = SplendidCache;
try
{
	// 01/15/2015 Paul.  ListView_LoadModule will be included in ChatDashboard, but not Calendar.  Catch and ignore the error. 
	background.ListView_LoadModule       = ListView_LoadModule;
	background.ListView_LoadLayout       = ListView_LoadLayout;
	background.EditView_LoadLayout       = EditView_LoadLayout;
	// 01/18/2015 Paul.  We need a special Terminology_LoadModule that will not prepend the sUSER_LANG. 
	//background.Terminology_LoadModule    = Terminology_LoadModule;
	background.AutoComplete_ModuleMethod = AutoComplete_ModuleMethod;
}
catch(e)
{
}

// 01/18/2015 Paul.  We need a special Terminology_LoadModule that will not prepend the sUSER_LANG. 
background.Terminology_LoadModule = function(sMODULE_NAME, callback, context)
{
	// 06/11/2012 Paul.  Wrap Terminology requests for Cordova. 
	var xhr = TerminologyRequest(sMODULE_NAME, null, 'NAME asc', sUSER_LANG);
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=TERMINOLOGY&$orderby=NAME asc&$filter=' + encodeURIComponent('(LANG eq \'' + sUSER_LANG + '\' and MODULE_NAME eq \'' + sMODULE_NAME + '\' and LIST_NAME is null)'), 'GET');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							TERMINOLOGY[sUSER_LANG + '.' + sMODULE_NAME + '.Loaded'] = true;
							//alert(dumpObj(result, 'result'));
							for ( var i = 0; i < result.d.results.length; i++ )
							{
								var obj = result.d.results[i];
								//Terminology_SetTerm(sMODULE_NAME, obj['NAME'], obj['DISPLAY_NAME']);
								TERMINOLOGY[sMODULE_NAME + '.' + obj['NAME']] = obj['DISPLAY_NAME'];
							}
							// 10/04/2011 Paul. Return the entire TERMINOLOGY table. 
							callback.call(context||this, 1, { 'sUSER_LANG': sUSER_LANG, 'TERMINOLOGY': TERMINOLOGY, 'TERMINOLOGY_LISTS': TERMINOLOGY_LISTS } );
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.status == 0 )
							callback.call(context||this, 0, result.ExceptionDetail.Message);
						else if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Terminology_LoadModule'));
				}
			}, context||this);
		}
	}
	try
	{
		if ( TERMINOLOGY[sUSER_LANG + '.' + sMODULE_NAME + '.Loaded'] == null )
			xhr.send();
		else
			callback.call(context||this, 1, { 'sUSER_LANG': sUSER_LANG, 'TERMINOLOGY': TERMINOLOGY, 'TERMINOLOGY_LISTS': TERMINOLOGY_LISTS } );
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'Terminology_LoadModule'));
	}
}

background.RemoteServer = function()
{
	return sREMOTE_SERVER;
};
background.UpdateModuleTable = function(sTABLE_NAME, row, sID, callback, context)
{
	if ( sTABLE_NAME == null )
	{
		callback.call(context||this, -1, 'UpdateModuleTable: sTABLE_NAME is invalid.');
		return;
	}
	else if ( row == null )
	{
		callback.call(context||this, -1, 'UpdateModuleTable: row is invalid.');
		return;
	}
	var xhr = CreateSplendidRequest('Rest.svc/UpdateModuleTable?TableName=' + sTABLE_NAME, 'POST', 'application/octet-stream');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							sID = result.d;
							callback.call(context||this, 1, sID);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'UpdateModule'));
				}
			});
		}
	}
	try
	{
		xhr.send(JSON.stringify(row));
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'UpdateModuleTable'));
	}
}

background.UpdateModule = function(sMODULE_NAME, row, sID, callback, context)
{
	if ( sMODULE_NAME == null )
	{
		callback.call(context||this, -1, 'UpdateModule: sMODULE_NAME is invalid.');
		return;
	}
	else if ( row == null )
	{
		callback.call(context||this, -1, 'UpdateModule: row is invalid.');
		return;
	}
	var xhr = CreateSplendidRequest('Rest.svc/UpdateModule?ModuleName=' + sMODULE_NAME, 'POST', 'application/octet-stream');
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							sID = result.d;
							callback.call(context||this, 1, sID);
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'UpdateModule'));
				}
			});
		}
	}
	try
	{
		xhr.send(JSON.stringify(row));
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'UpdateModule'));
	}
};

// 01/18/2015 Paul.  IsAuthenticated is used by ChatDashboard PopupViewUI. 
background.IsAuthenticated = function(callback, context)
{
	var xhr = CreateSplendidRequest('Rest.svc/IsAuthenticated');
	// 12/21/2014 Paul.  Use 2 second timeout for IsAuthenticated. 
	xhr.timeout = 2000;
	xhr.onreadystatechange = function()
	{
		if ( xhr.readyState == 4 )
		{
			GetSplendidResult(xhr, function(result)
			{
				try
				{
					if ( result.status == 200 )
					{
						if ( result.d !== undefined )
						{
							if ( result.d == true )
							{
								callback.call(context||this, 1, '');
							}
							else
							{
								callback.call(context||this, 0, '');
							}
						}
						else
						{
							callback.call(context||this, -1, xhr.responseText);
						}
					}
					else
					{
						if ( result.ExceptionDetail !== undefined )
							callback.call(context||this, -1, result.ExceptionDetail.Message);
						else
							callback.call(context||this, -1, xhr.responseText);
					}
				}
				catch(e)
				{
					callback.call(context||this, -1, SplendidError.FormatError(e, 'IsAuthenticated'));
				}
			});
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		// 03/28/2012 Paul.  We need to return a status and a message, not a result object. 
		if ( Security.USER_ID() == '' )
			callback.call(context||this, 0, '');
		else
			callback.call(context||this, 1, '');
	}
}

// 01/18/2015 Paul.  AuthenticatedMethod is used by ChatDashboard PopupViewUI. 
background.AuthenticatedMethod = function AuthenticatedMethod(callback, context)
{
	background.IsAuthenticated(function(status, message)
	{
		if ( status == 1 )
		{
			callback.call(context||this, 1, null);
		}
		else if ( status == 0 )
		{
			callback.call(context||this, status, 'Failed to authenticate. Please login again. ');
		}
		else
		{
			callback.call(context||this, status, message);
		}
	}, context);
}

var chrome = new Object();
chrome.extension = new Object();
chrome.extension.getBackgroundPage = function()
{
	return background;
};

function CreateSplendidRequest(sPath, sMethod, sContentType)
{
	// http://www.w3.org/TR/XMLHttpRequest/
	var xhr = null;
	try
	{
		if ( window.XMLHttpRequest )
			xhr = new XMLHttpRequest();
		else if ( window.ActiveXObject )
			xhr = new ActiveXObject("Msxml2.XMLHTTP");
		
		var url = sREMOTE_SERVER + sPath;
		if ( sMethod === undefined )
			sMethod = 'POST';
		if ( sContentType === undefined )
			sContentType = 'application/json; charset=utf-8';
		xhr.open(sMethod, url, true);
		if ( sAUTHENTICATION == 'Basic' )
			xhr.setRequestHeader('Authorization', 'Basic ' + Base64.encode(sUSER_NAME + ':' + sPASSWORD));
		xhr.setRequestHeader('content-type', sContentType);
		// 09/27/2011 Paul.  Add the URL to the object for debugging purposes. 
		// 10/19/2011 Paul.  IE6 does not allow this. 
		if ( window.XMLHttpRequest )
		{
			xhr.url    = url;
			xhr.Method = sMethod;
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'CreateSplendidRequest');
	}
	return xhr;
}

function GetSplendidResult(xhr, callback, context)
{
	var result = null;
	try
	{
		//alert(dumpObj(xhr, 'xhr.status = ' + xhr.status));
		if ( xhr.responseText.length > 0 )
		{
			result = JSON.parse(xhr.responseText);
			result.status = xhr.status;
			callback.call(context||this, result);
		}
		else if ( xhr.status == 0 || xhr.status == 2 || xhr.status == 12002 || xhr.status == 12007 || xhr.status == 12029 || xhr.status == 12030 || xhr.status == 12031 || xhr.status == 12152 )
		{
		}
		else if ( xhr.status == 405 )
		{
			var sMessage = 'Method Not Allowed.  ' + xhr.url;
			result = { 'status': xhr.status, 'ExceptionDetail': { 'status': xhr.status, 'Message': sMessage } };
			callback.call(context||this, result);
		}
		else
		{
			result = { 'status': xhr.status, 'ExceptionDetail': { 'status': xhr.status, 'Message': xhr.statusText + '(' + xhr.status + ')' } };
			callback.call(context||this, result);
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'GetSplendidResult');
		callback.call(context||this, result);
	}
}

function BindArguments(fn)
{
	var args = [];
	for ( var n = 1; n < arguments.length; n++ )
		args.push(arguments[n]);
	return function () { return fn.apply(this, args); };
}

function RegisterEnterKeyPress(e, sSubmitID)
{
	if ( e != null )
	{
		if ( e.which == 13 )
		{
			var btnSubmit = document.getElementById(sSubmitID);
			if ( btnSubmit != null )
				btnSubmit.click();
			return false;
		}
	}
	else if ( event != null )
	{
		if ( event.keyCode == 13 )
		{
			event.returnValue = false;
			event.cancel = true;
			var btnSubmit = document.getElementById(sSubmitID);
			if ( btnSubmit != null )
				btnSubmit.click();
		}
	}
}

function ValidateCredentials()
{
	return true;
}

	</script>
</SplendidCRM:InlineScript>


