<%@ Control CodeBehind="LoadSplendid.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.html5.LoadSplendid" %>
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
<script type="text/javascript">
function ClearSystemMessage()
{
	SplendidError.ClearError();
	SplendidError.ClearAlert();
}

function LoadSplendid()
{
	try
	{
		// 04/23/2013 Paul.  New approach to menu management. 
		sPRODUCT_TITLE    = 'SplendidCRM <%# Application["CONFIG.service_level"] %>';
		sREMOTE_SERVER    = '<%# Request.Url.Scheme + "://" + Request.Url.Host + Sql.ToString(Application["rootURL"]) %>';
		sIMAGE_SERVER     = '../';
		sPLATFORM_LAYOUT  = '';
		if ( isMobileDevice() )
		{
			sPLATFORM_LAYOUT  = '.Mobile';
			var divFooterCopyright = document.getElementById('divFooterCopyright');
			if ( divFooterCopyright != null )
				divFooterCopyright.style.display = 'none';
		}
		
		sAUTHENTICATION   = '<%# Security.IsWindowsAuthentication() || Security.IsAuthenticated() ? "Windows" : "CRM" %>';
		bWINDOWS_AUTH     =  <%# Security.IsWindowsAuthentication() ? "true" : "false" %>;
		bIS_OFFLINE       = false;
		bENABLE_OFFLINE   = (window.XMLHttpRequest !== undefined);
		sUSER_ID          = '<%# Security.USER_ID   %>';
		sUSER_NAME        = '<%# Sql.EscapeJavaScript(Security.USER_NAME) %>';
		sTEAM_ID          = '<%# Security.TEAM_ID   %>';
		sTEAM_NAME        = '<%# Sql.EscapeJavaScript(Security.TEAM_NAME) %>';
		sUSER_LANG        = '<%# L10n.NAME          %>';
		sUSER_DATE_FORMAT = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DATEFORMAT"   ])) %>';
		sUSER_TIME_FORMAT = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/TIMEFORMAT"   ])) %>';
		// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
		sUSER_THEME       = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["USER_SETTINGS/DEFAULT_THEME"])) %>';
		if ( sUSER_THEME != 'Six' && sUSER_THEME != 'Atlantic' )
			sUSER_THEME = 'Atlantic';
		sPASSWORD         = '';
		
		// 11/25/2014 Paul.  Add SignalR fields. 
		sUSER_EXTENSION      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["EXTENSION"   ])) %>';
		sUSER_FULL_NAME      = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["FULL_NAME"   ])) %>';
		sUSER_PHONE_WORK     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_WORK"  ])) %>';
		sUSER_SMS_OPT_IN     = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["SMS_OPT_IN"  ])) %>';
		sUSER_PHONE_MOBILE   = '<%# Sql.EscapeJavaScript(Sql.ToString(Session["PHONE_MOBILE"])) %>';
		sUSER_TWITTER_TRACKS = '';
		sUSER_CHAT_CHANNELS  = '<%# SplendidCache.MyChatChannels() %>';
		try
		{
			// 11/26/2014 Paul.  The server stub does not account for the virtual directory, so update the URL. 
			$.connection.hub.url = sREMOTE_SERVER + '/signalr';
			// 11/25/2014 Paul.  Needs to be started after the variables are setup. 
			SignalR_Connection_Start();
		}
		catch(e)
		{
		}
		
		SplendidStorage.maxDatabase = <%# Sql.ToInteger(Application["CONFIG.html5.max_database"]) %>;
		cbNetworkStatusChanged = NetworkStatusChanged;
		arrUserContextMenu =
		[ { id: 'lnkHeaderSystemLog'      , text: '.LBL_SYSTEM_LOG'      , action: ShowSystemLog       }
		, { id: 'lnkHeaderSplendidStorage', text: '.LBL_SPLENDID_STORAGE', action: ShowSplendidStorage }
		, { id: 'lnkHeaderCacheAll'       , text: '.LBL_CACHE_ALL'       , action: CacheAllModules     }
		, { id: 'divHeader_lnkLogout'     , text: '.LBL_LOGOUT'          , action: null                }
		];
		
		Terminology_SetTerm(''     , 'NTC_LOGIN_MESSAGE'      , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".NTC_LOGIN_MESSAGE"          )) %>');
		Terminology_SetTerm(''     , 'LBL_ENABLE_OFFLINE'     , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_ENABLE_OFFLINE"         )) %>');
		Terminology_SetTerm(''     , 'LBL_ONLINE'             , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_ONLINE"                 )) %>');
		Terminology_SetTerm(''     , 'LBL_OFFLINE'            , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_OFFLINE"                )) %>');
		Terminology_SetTerm(''     , 'LBL_SEARCH_BUTTON_LABEL', '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_SEARCH_BUTTON_LABEL"    )) %>');
		Terminology_SetTerm(''     , 'LBL_CLEAR_BUTTON_LABEL' , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_CLEAR_BUTTON_LABEL"     )) %>');
		Terminology_SetTerm(''     , 'LBL_CACHE_SELECTED'     , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_CACHE_SELECTED"         )) %>');
		Terminology_SetTerm(''     , 'NTC_CACHE_CONFIRMATION' , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".NTC_CACHE_CONFIRMATION"     )) %>');
		Terminology_SetTerm('Users', 'LBL_USER_NAME'          , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], "Users.LBL_USER_NAME"         )) %>');
		Terminology_SetTerm('Users', 'LBL_PASSWORD'           , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], "Users.LBL_PASSWORD"          )) %>');
		Terminology_SetTerm('Users', 'LBL_LOGIN_BUTTON_LABEL' , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], "Users.LBL_LOGIN_BUTTON_LABEL")) %>');
		Terminology_SetTerm('Users', 'LBL_ERROR'              , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], "Users.LBL_ERROR"             )) %>');
		Terminology_SetTerm('Users', 'ERR_INVALID_PASSWORD'   , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], "Users.ERR_INVALID_PASSWORD"  )) %>');
		
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.SplendidStorage.Init(function(status, message)
		{
			bgPage.IsOnline(function(status, message)
			{
				var divHeader_divOnlineStatus = ctlActiveMenu.divOnlineStatus();
				if ( status == 1 )
				{
					// 09/28/2011 Paul.  Site is online. 
					if ( divHeader_divOnlineStatus != null )
						divHeader_divOnlineStatus.innerHTML = L10n.Term('.LBL_ONLINE');
					bIS_OFFLINE = false;
				}
				else if ( status == 0 )
				{
					// 09/28/2011 Paul.  Site is offline. 
					if ( divHeader_divOnlineStatus != null )
						divHeader_divOnlineStatus.innerHTML = L10n.Term('.LBL_OFFLINE');
					bIS_OFFLINE = true;
				}
				else if ( status < 0 )
				{
					SplendidError.SystemMessage(message);
					if ( divHeader_divOnlineStatus != null )
						divHeader_divOnlineStatus.innerHTML = L10n.Term('Users.LBL_ERROR');
					bIS_OFFLINE = true;
				}
				bgPage.IsAuthenticated(function(status, message)
				{
					try
					{
						if ( status == 1 )
						{
							var rowDefaultSearch = null;
							var sLastModuleTab = '';
							if ( window.localStorage )
								sLastModuleTab = localStorage['LastActiveModule'];
							else
								sLastModuleTab = getCookie('LastActiveModule');
							if ( !Sql.IsEmptyString(sLastModuleTab) )
								sSTARTUP_MODULE = sLastModuleTab;
							var sLayoutPanel  = 'divMainLayoutPanel';
							var sActionsPanel = 'divMainActionsPanel';
							SplendidUI_Init(sLayoutPanel, sActionsPanel, sSTARTUP_MODULE, rowDefaultSearch, function(status, message)
							{
								// 10/10/2011 Paul.  Once the globals have been loaded, we can update the header. 
								if ( status == 3 )
								{
									LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, true)
								}
								else if ( status == 1 )
								{
									SplendidError.SystemMessage('');
								}
								else
								{
									SplendidError.SystemMessage(message);
								}
							});
						}
						else if ( status == 0 )
						{
							ShowOptionsDialog();
						}
						else
						{
							SplendidError.SystemMessage(message);
						}
					}
					catch(e)
					{
						SplendidError.SystemError(e, 'default.html IsAuthenticated()');
					}
				});
			});
		});
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'LoadSplendid');
	}
}

window.onload = function()
{
	Terminology_SetTerm(''     , 'NTC_WELCOME'            , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".NTC_WELCOME"                )) %>');
	Terminology_SetTerm(''     , 'LBL_LOGOUT'             , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_LOGOUT"                 )) %>');
	Terminology_SetTerm(''     , 'LBL_SYSTEM_LOG'         , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_SYSTEM_LOG"             )) %>');
	Terminology_SetTerm(''     , 'LBL_CACHE_ALL'          , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_CACHE_ALL"              )) %>');
	Terminology_SetTerm(''     , 'LBL_SPLENDID_STORAGE'   , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_SPLENDID_STORAGE"       )) %>');
	Terminology_SetTerm(''     , 'LBL_ONLINE'             , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_ONLINE"                 )) %>');
	Terminology_SetTerm(''     , 'LBL_OFFLINE'            , '<%# Sql.EscapeJavaScript(L10N.Term(Application, Request["HTTP_ACCEPT_LANGUAGE"], ".LBL_OFFLINE"                )) %>');
	
	var sLayoutPanel  = 'divMainLayoutPanel';
	var sActionsPanel = 'divMainActionsPanel';
	sIMAGE_SERVER     = '../';
	ctlActiveMenu = new TabMenuUI_None(sLayoutPanel, sActionsPanel);
	ctlActiveMenu.RenderHeader();

	// 05/06/2013 Paul.  Return early when debugging, otherwise LoadSplendid will get called twice. 
	<%# Sql.ToBoolean(Request["debug"]) ? "LoadSplendid(); return;" : String.Empty %>
	if ( window.applicationCache !== undefined )
	{
		try
		{
			var nManifestProgress = 1;
			
			window.applicationCache.addEventListener('cached', LoadSplendid, false);
			window.applicationCache.addEventListener('cached', function()
			{
				SplendidError.SystemMessage('manifest cached');
			}, false);
			
			window.applicationCache.addEventListener('noupdate', LoadSplendid, false);
			window.applicationCache.addEventListener('noupdate', function()
			{
				SplendidError.SystemMessage('manifest noupdate');
			}, false);
			
			window.applicationCache.addEventListener('obsolete', LoadSplendid, false);
			window.applicationCache.addEventListener('obsolete', function()
			{
				SplendidError.SystemMessage('manifest obsolete');
			}, false);
			
			window.applicationCache.addEventListener('checking', function()
			{
				SplendidError.SystemMessage('manifest checking');
			}, false);
			
			window.applicationCache.addEventListener('downloading', function()
			{
				SplendidError.SystemMessage('manifest downloading');
			}, false);
			
			window.applicationCache.addEventListener('progress', function()
			{
				SplendidError.SystemMessage('manifest progress ' + nManifestProgress);
				nManifestProgress++;
			}, false);
			
			window.applicationCache.addEventListener('error', LoadSplendid, false);
			window.applicationCache.addEventListener('error', function()
			{
				SplendidError.SystemMessage('manifest error');
			}, false);
			
			// 09/27/2011 Paul.  updateready does not always fire as document. 
			// https://developer.mozilla.org/en/Offline_resources_in_Firefox
			window.applicationCache.addEventListener('updateready', function(e)
			{
				SplendidError.SystemMessage('manifest update ready ');
				// 10/16/2011 Paul.  An easier alternative to swapCache() is just to reload the entire page at a time suitable for the user, using location.reload().
				// http://www.whatwg.org/specs/web-apps/current-work/#applicationcache
				//window.applicationCache.swapCache();
				//window.onload = LoadSplendid;
				// Even after swapping the cache the currently loaded page won't use it until it is reloaded, so force a reload so it is current.
				//window.location.reload(true);
				Reload();
			}, false);
		}
		catch(e)
		{
			alert('default.aspx onload ' + e.message);
		}
	}
	else
	{
		LoadSplendid();
	}
}
</script>

