/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var Crm     = new Object();
Crm.Config  = new Object();
Crm.Modules = new Object();
Crm.Teams   = new Object();

Crm.Config.enable_team_management = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config('enable_team_management'));
}

Crm.Config.require_team_management = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config('require_team_management'));
}

Crm.Config.enable_dynamic_teams = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config('enable_dynamic_teams'));
}

Crm.Config.require_user_assignment = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config('require_user_assignment'));
}

// 08/31/2012 Paul.  Add support for speech. 
Crm.Config.enable_speech = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config('enable_speech'));
}

Crm.Config.ToBoolean = function(sName)
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToBoolean(bgPage.SplendidCache.Config(sName));
}

Crm.Config.ToInteger = function(sName)
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToInteger(bgPage.SplendidCache.Config(sName));
}

Crm.Config.ToString = function(sName)
{
	var bgPage = chrome.extension.getBackgroundPage();
	return Sql.ToString(bgPage.SplendidCache.Config(sName));
}

Crm.Modules.TableName = function(sMODULE)
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.Module(sMODULE).TABLE_NAME;
}

Crm.Modules.SingularTableName = function(sTABLE_NAME)
{
	if ( Right(sTABLE_NAME, 3) == 'IES' && sTABLE_NAME.length > 3 )
		sTABLE_NAME = sTABLE_NAME.substring(0, sTABLE_NAME.length - 3) + 'Y';
	else if ( Right(sTABLE_NAME, 1) == 'S' )
		sTABLE_NAME = sTABLE_NAME.substring(0, sTABLE_NAME.length - 1);
	return sTABLE_NAME;
}

Crm.Modules.SingularModuleName = function(sMODULE)
{
	if ( Right(sMODULE, 3) == 'ies' && sMODULE.length > 3 )
		sMODULE = sMODULE.substring(0, sMODULE.length - 3) + 'y';
	else if ( Right(sMODULE, 1) == 's' )
		sMODULE = sMODULE.substring(0, sMODULE.length - 1);
	return sMODULE;
}

Crm.Modules.ExchangeFolders = function(sMODULE)
{
	var bgPage = chrome.extension.getBackgroundPage();
	var oModule = bgPage.SplendidCache.Module(sMODULE);
	// 10/24/2014 Paul.  The module should not return NULL, but we don't want to generate an error here. 
	if ( oModule === undefined )
		return false;
	return Sql.ToBoolean(oModule.EXCHANGE_SYNC) && Sql.ToBoolean(oModule.EXCHANGE_FOLDERS);
}

Crm.Modules.ItemName = function(sMODULE_NAME, sID, callback, context)
{
	var bgPage = chrome.extension.getBackgroundPage();
	bgPage.DetailView_LoadItem(sMODULE_NAME, sID, function(status, message)
	{
		if ( status == 1 )
			callback.call(context, status, message['NAME']);
		else
			callback.call(context, status, null);
	}, context);
}



Crm.Teams.Name = function(sID)
{
	var bgPage = chrome.extension.getBackgroundPage();
	var rowTeam = bgPage.SplendidCache.Team(sID);
	if ( rowTeam !== undefined && rowTeam != null )
		return rowTeam.NAME;
	else
		return '';
}

var Security = new Object();
Security.USER_ID = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserID();
}

Security.USER_NAME = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserName();
}

Security.FULL_NAME = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.FullName();
}

// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
Security.PICTURE = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.Picture();
}

Security.USER_LANG = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserLang();
}

// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
Security.USER_THEME = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserTheme();
}

Security.USER_DATE_FORMAT = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserDateFormat();
}

Security.USER_TIME_FORMAT = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.UserTimeFormat();
}

Security.TEAM_ID = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.TeamID();
}

Security.TEAM_NAME = function()
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.TeamName();
}

