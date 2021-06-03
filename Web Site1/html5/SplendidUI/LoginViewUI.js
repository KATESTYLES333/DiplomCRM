/* Copyright (C) 2011-2014 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */
 
var sPRODUCT_TITLE  = 'SplendidCRM';
var sSTARTUP_MODULE = 'Contacts';

function LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, bShow)
{
	try
	{
		if ( ctlActiveMenu == null )
			return;
		var divHeader_divAuthenticated   = ctlActiveMenu.divAuthenticated();
		var divHeader_spnWelcome         = ctlActiveMenu.spnWelcome();
		var divHeader_spnUserName        = ctlActiveMenu.spnUserName();
		var divHeader_spnLogout          = ctlActiveMenu.spnLogout();
		var divHeader_lnkLogout          = ctlActiveMenu.lnkLogout();
		// 08/22/2014 Paul.  Add SyncNow for offline client. 
		var divHeader_spnSyncNow         = ctlActiveMenu.spnSyncNow();
		var divHeader_lnkSyncNow         = ctlActiveMenu.lnkSyncNow();
		var divHeader_divOnlineStatus    = ctlActiveMenu.divOnlineStatus();
		var divHeader_divOfflineCache    = ctlActiveMenu.divOfflineCache();
		var divHeader_divSplendidStorage = ctlActiveMenu.divSplendidStorage();
		var lnkHeaderCacheAll            = ctlActiveMenu.lnkCacheAll();
		var lnkHeaderSystemLog           = ctlActiveMenu.lnkSystemLog();
		var lnkHeaderSplendidStorage     = ctlActiveMenu.lnkSplendidStorage();
		
		var bgPage = chrome.extension.getBackgroundPage();
		if ( lnkHeaderCacheAll          != null ) lnkHeaderCacheAll.innerHTML              = L10n.Term('.LBL_CACHE_ALL'       );
		if ( lnkHeaderSystemLog         != null ) lnkHeaderSystemLog.innerHTML             = L10n.Term('.LBL_SYSTEM_LOG'      );
		if ( lnkHeaderSplendidStorage   != null ) lnkHeaderSplendidStorage.innerHTML       = L10n.Term('.LBL_SPLENDID_STORAGE');
		if ( divHeader_divAuthenticated != null ) divHeader_divAuthenticated.style.display = (bShow ? 'inline'                  : 'none');
		if ( divHeader_spnWelcome       != null ) divHeader_spnWelcome.innerHTML           = (bShow ? L10n.Term('.NTC_WELCOME') : '');
		if ( divHeader_spnUserName      != null ) divHeader_spnUserName.innerHTML          = (bShow ? (sUSER_NAME + ' ' + sTEAM_NAME) : '');
		if ( divHeader_spnLogout        != null ) divHeader_spnLogout.style.display        = (bWINDOWS_AUTH ? 'none' : 'inline');
		if ( divHeader_divOnlineStatus  != null ) divHeader_divOnlineStatus.innerHTML      =  bgPage.GetIsOffline() ? L10n.Term('.LBL_OFFLINE') : L10n.Term('.LBL_ONLINE');
		if ( divHeader_lnkLogout        != null )
		{
			divHeader_lnkLogout.style.display = (bWINDOWS_AUTH ? 'none' : 'block');
			divHeader_lnkLogout.innerHTML     = (bShow ? L10n.Term('.LBL_LOGOUT' ) : '');
			divHeader_lnkLogout.onclick = function()
			{
				LoginViewUI_PageCommand(sLayoutPanel, sActionsPanel, 'Logout', null, null);
			};
		}
		// 08/22/2014 Paul.  Add SyncNow for offline client. 
		// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
		if ( divHeader_spnSyncNow       != null )
			divHeader_spnSyncNow.style.display = (bShow && bREMOTE_ENABLED && !bMOBILE_CLIENT ? 'block' : 'none');
		if ( divHeader_lnkSyncNow       != null )
		{
			divHeader_lnkSyncNow.style.display = (bShow && bREMOTE_ENABLED && !bMOBILE_CLIENT ? 'block' : 'none');
			divHeader_lnkSyncNow.innerHTML     = (bShow && bREMOTE_ENABLED && !bMOBILE_CLIENT ? L10n.Term('Offline.LNK_OFFLINE_DASHBOARD' ) : '');
			divHeader_lnkSyncNow.onclick = function()
			{
				ShowOfflineDashboard();
			};
		}
		if ( divHeader_divSplendidStorage != null )
		{
			// 10/25/2012 Paul.  Don't display the storage buttons unless caching is enabled. 
			divHeader_divSplendidStorage.style.display = (bShow && bLIST_VIEW_ENABLE_SELECTION ? 'inline' : 'none');
		}
		
		if ( divHeader_divOnlineStatus != null && bgPage.SplendidStorage !== undefined && bgPage.SplendidStorage.db != null )
		{
			divHeader_divOnlineStatus.innerHTML += ' Web SQL';
		}
		
		if ( divHeader_divOfflineCache != null )
		{
			if ( divHeader_divOfflineCache.childNodes != null )
			{
				while ( divHeader_divOfflineCache.childNodes.length > 0 )
				{
					divHeader_divOfflineCache.removeChild(divHeader_divOfflineCache.firstChild);
				}
			}
			divHeader_divOfflineCache.style.display = 'none';
			// 10/19/2011 Paul.  IE6 does not support localStorage. 
			// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
			if ( window.localStorage && localStorage['OFFLINE_CACHE'] != null )
			{
				var arrOFFLINE_CACHE = JSON.parse(localStorage['OFFLINE_CACHE']);
				//alert(dumpObj(arrOFFLINE_CACHE, 'arrOFFLINE_CACHE'));
				// 10/07/2011 Paul.  arrOFFLINE_CACHE.length is not valid. 
				var nOFFLINE_CACHE_length = 0;
				for ( var key in arrOFFLINE_CACHE )
				{
					nOFFLINE_CACHE_length++;
				}
				
				divHeader_divOfflineCache.style.display = (bShow && nOFFLINE_CACHE_length > 0) ? 'inline' : 'none';
				
				var divSubmit = document.createElement('div');
				divHeader_divOfflineCache.appendChild(divSubmit);
				var btnSubmit = document.createElement('input');
				btnSubmit.type      = 'submit';
				btnSubmit.value     = L10n.Term('.LBL_SUBMIT_BUTTON_LABEL');
				btnSubmit.title     = L10n.Term('.LBL_SUBMIT_BUTTON_LABEL');
				btnSubmit.className = 'button';
				btnSubmit.onclick   = function()
				{
					var oEditViewUI = new EditViewUI();
					oEditViewUI.SubmitOffline(sLayoutPanel, sActionsPanel, 'Save');
				}
				divSubmit.appendChild(btnSubmit);
				
				// 03/16/2014 Paul.  Add hidden buttons for Save Duplicate and Save Concurrency. 
				var btnSubmit_SaveDuplicate = document.createElement('input');
				btnSubmit_SaveDuplicate.type          = 'submit';
				btnSubmit_SaveDuplicate.id            = 'btnSubmit_SaveDuplicate';
				btnSubmit_SaveDuplicate.value         = L10n.Term('.LBL_SAVE_DUPLICATE_LABEL');
				btnSubmit_SaveDuplicate.title         = L10n.Term('.LBL_SAVE_DUPLICATE_LABEL');
				btnSubmit_SaveDuplicate.className     = 'button';
				btnSubmit_SaveDuplicate.style.display = 'none';
				btnSubmit_SaveDuplicate.onclick   = function()
				{
					var oEditViewUI = new EditViewUI();
					oEditViewUI.SubmitOffline(sLayoutPanel, sActionsPanel, 'SaveDuplicate');
				}
				divSubmit.appendChild(btnSubmit_SaveDuplicate);
				
				var btnSubmit_SaveConcurrency = document.createElement('input');
				btnSubmit_SaveConcurrency.type          = 'submit';
				btnSubmit_SaveConcurrency.id            = 'btnSubmit_SaveConcurrency';
				btnSubmit_SaveConcurrency.value         = L10n.Term('.LBL_SAVE_CONCURRENCY_LABEL');
				btnSubmit_SaveConcurrency.title         = L10n.Term('.LBL_SAVE_CONCURRENCY_LABEL');
				btnSubmit_SaveConcurrency.className     = 'button';
				btnSubmit_SaveConcurrency.style.display = 'none';
				btnSubmit_SaveConcurrency.onclick   = function()
				{
					var oEditViewUI = new EditViewUI();
					oEditViewUI.SubmitOffline(sLayoutPanel, sActionsPanel, 'SaveConcurrency');
				}
				divSubmit.appendChild(btnSubmit_SaveConcurrency);
				divSubmit.style.display = (bShow && !bgPage.GetIsOffline() && nOFFLINE_CACHE_length > 0) ? 'inline' : 'none';
				
				for ( var key in arrOFFLINE_CACHE )
				{
					var oCached = arrOFFLINE_CACHE[key];
					var divItem = document.createElement('div');
					divHeader_divOfflineCache.appendChild(divItem);
					
					var sModuleLabel = oCached.MODULE_NAME;
					// 10/28/2012 Paul.  The module might be a relationship name. 
					if ( !StartsWith(oCached.MODULE_NAME, 'vw') )
					{
						sModuleLabel = L10n.ListTerm('moduleList', oCached.MODULE_NAME);
						sModuleLabel = Crm.Modules.SingularModuleName(sModuleLabel);
					}
					
					var aView = document.createElement('a');
					divItem.appendChild(aView);
					aView.href = '#';
					aView.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
					{
						var oDetailViewUI = new DetailViewUI();
						// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
						oDetailViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, function(status, message)
						{
							if ( status == 1 )
							{
								SplendidError.SystemMessage('');
							}
							else
							{
								SplendidError.SystemMessage(message);
							}
						}, this);
					}, sLayoutPanel, sActionsPanel, oCached.MODULE_NAME, oCached.ID);
					
					/*
					var imgView = document.createElement('img');
					aView.appendChild(imgView);
					imgView.align             = 'absmiddle';
					imgView.style.height      = '16px';
					imgView.style.width       = '16px';
					imgView.style.borderWidth = '0px';
					imgView.src               = sIMAGE_SERVER + 'App_Themes/Six/images/view_inline.gif';
					imgView.alt               = L10n.Term('.LNK_VIEW');
					imgView.style.padding     = '2px';
					imgView.style.border      = 'none';
					*/

					var spnItem = document.createElement('span');
					aView.appendChild(spnItem);
					spnItem.innerHTML = sModuleLabel + ': ' + oCached.NAME;
					spnItem.style.padding = '2px';
				}
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'LoginViewUI_UpdateHeader');
	}
}

function LoginViewUI_ClearModuleLists(bClearAll, callback)
{
	var arrKeys = new Array();
	var bgPage = chrome.extension.getBackgroundPage();
	bgPage.SplendidStorage.foreach(function(status, key, value)
	{
		if ( status == 1 )
		{
			// 11/28/2011 Paul.  Remove module lists, but not the default. 
			// 09/11/2012 Paul.  Remove items. 
			if ( key.indexOf('Rest.svc/GetModuleList?') > 0 || key.indexOf('Rest.svc/GetModuleItem?') > 0 )
			{
				arrKeys.push(key);
			}
			// 03/10/2013 Paul.  On dev systems, multiple virtual directories will save their data to a single localStorage. 
			// This is causing an out-of-memory error. 
			else if ( StartsWith(key, 'http') && (bClearAll || !StartsWith(key, sREMOTE_SERVER)) )
			{
				arrKeys.push(key);
			}
		}
		else if ( status == 0 )
		{
			//if ( arrKeys.length == 0 )
			//	SplendidError.SystemLog('Nothing to remove from cache');
			while ( arrKeys.length > 0 )
			{
				var key = arrKeys.pop();
				SplendidStorage.removeItem(key);
				SplendidError.SystemLog('Removed item from cache: ' + key);
			}
			if ( callback )
			{
				callback(0, '');
			}
		}
	});
}

function LoginViewUI_PageCommand(sLayoutPanel, sActionsPanel, sCommandName, sCommandArguments, cbLoginComplete)
{
	try
	{
		if ( sCommandName == 'Login' )
		{
			var txtREMOTE_SERVER  = document.getElementById(sLayoutPanel + '_ctlLoginView_txtREMOTE_SERVER' );
			var txtUSER_NAME      = document.getElementById(sLayoutPanel + '_ctlLoginView_txtUSER_NAME'     );
			var txtPASSWORD       = document.getElementById(sLayoutPanel + '_ctlLoginView_txtPASSWORD'      );
			var chkENABLE_OFFLINE = document.getElementById(sLayoutPanel + '_ctlLoginView_chkENABLE_OFFLINE');
			sUSER_NAME = txtUSER_NAME.value;
			sPASSWORD  = txtPASSWORD.value;
			// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
			if ( bMOBILE_CLIENT && txtREMOTE_SERVER != null )
			{
				txtREMOTE_SERVER.value = Trim(txtREMOTE_SERVER.value);
				if ( EndsWith(txtREMOTE_SERVER.value, '.asmx') || EndsWith(txtREMOTE_SERVER.value, '.aspx') || EndsWith(txtREMOTE_SERVER.value, '.svc') )
				{
					var nLastSlash = txtREMOTE_SERVER.value.lastIndexOf('/');
					if ( nLastSlash > 0 )
						txtREMOTE_SERVER.value = txtREMOTE_SERVER.value.substring(0, nLastSlash + 1);
					else
					{
						// 08/17/2014 Paul.  Case-insignificant replacements. 
						txtREMOTE_SERVER.value = txtREMOTE_SERVER.value.replace(/sync.asmx/gi, '');
						txtREMOTE_SERVER.value = txtREMOTE_SERVER.value.replace(/Rest.svc/gi, '');
					}
				}
				if ( !EndsWith(txtREMOTE_SERVER.value, '/') )
					txtREMOTE_SERVER.value += '/';
				// 12/11/2014 Paul.  Prepend http:// if not provided. 
				if ( !StartsWith(txtREMOTE_SERVER.value.toLocaleLowerCase(), 'http://') && !StartsWith(txtREMOTE_SERVER.value.toLocaleLowerCase(), 'https://') )
					txtREMOTE_SERVER.value = 'http://' + txtREMOTE_SERVER.value;
			
				sREMOTE_SERVER = txtREMOTE_SERVER.value;
				sIMAGE_SERVER  = txtREMOTE_SERVER.value;
			}

			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.Login(function(status, message)
			{
				var spnError = document.getElementById(sLayoutPanel + '_ctlLoginView_lblError');
				if ( status == 0 )
				{
					// 09/30/2011 Paul.  If there is no response, check against the stored values. 
					// 10/19/2011 Paul.  IE6 does not support localStorage. 
					// 11/27/2011 Paul.  The USER_NAME is not case-significant, so don't require it here either. 
					// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
					// 10/21/2012 Paul.  We have removed the Enable Offline checkbox because it is always enabled. 
					if ( (chkENABLE_OFFLINE == null || chkENABLE_OFFLINE.checked) && window.localStorage && Sql.ToString(localStorage['USER_NAME']).toLowerCase() == sUSER_NAME.toLowerCase() && localStorage['USER_HASH'] == Sha1.hash(sPASSWORD) )
					{
						sUSER_ID          = localStorage['USER_ID'         ];
						sUSER_NAME        = localStorage['USER_NAME'       ];
						sFULL_NAME        = localStorage['FULL_NAME'       ];
						// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
						sPICTURE          = localStorage['PICTURE'         ];
						sTEAM_ID          = localStorage['TEAM_ID'         ];
						sTEAM_NAME        = localStorage['TEAM_NAME'       ];
						sUSER_LANG        = localStorage['USER_LANG'       ];
						// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
						sUSER_THEME       = localStorage['USER_THEME'      ];
						sUSER_DATE_FORMAT = localStorage['USER_DATE_FORMAT'];
						sUSER_TIME_FORMAT = localStorage['USER_TIME_FORMAT'];
						sUSER_CURRENCY_ID = localStorage['USER_CURRENCY_ID'];
						sUSER_TIMEZONE_ID = localStorage['USER_TIMEZONE_ID'];
						// 12/01/2014 Paul.  Add SignalR fields. 
						sUSER_EXTENSION      = Sql.ToString(localStorage['USER_EXTENSION'     ]);
						sUSER_FULL_NAME      = Sql.ToString(localStorage['USER_FULL_NAME'     ]);
						sUSER_PHONE_WORK     = Sql.ToString(localStorage['USER_PHONE_WORK'    ]);
						sUSER_SMS_OPT_IN     = Sql.ToString(localStorage['USER_SMS_OPT_IN'    ]);
						sUSER_PHONE_MOBILE   = Sql.ToString(localStorage['USER_PHONE_MOBILE'  ]);
						sUSER_TWITTER_TRACKS = Sql.ToString(localStorage['USER_TWITTER_TRACKS']);
						sUSER_CHAT_CHANNELS  = Sql.ToString(localStorage['USER_CHAT_CHANNELS' ]);
					}
					else
					{
						message = L10n.Term('Users.ERR_INVALID_PASSWORD');
						status  = -1;
					}
				}
				if ( status == 0 || status == 1 )
				{
					spnError.innerHTML = 'Login successful';
					// 09/30/2011 Paul.  Don't fetch UI files if we are offline. 
					// 11/27/2011 Paul.  bENABLE_OFFLINE is a global flag, so we cannot turn off just for UI. 
					// bENABLE_OFFLINE must remain enabled in order to retrieved cached UI data when attempting to use offline. 
					// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
					
					// 10/16/2012 Paul.  Always load the global layout cache if it has not been loaded. 
					//bENABLE_OFFLINE = window.localStorage ? chkENABLE_OFFLINE.checked : false;
					// 10/12/2011 Paul.  If we are online, we should take this time to clear local storage.  
					// The concern is that we will be caching all searches and we need a way to clear this before the storage becomes full. 
					// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
					if ( status == 1 && window.localStorage )
					{
						// 10/12/2011 Paul.  Make sure not to clear offline cached data. 
						// 10/12/2011 Paul.  For now, lets just clear module lists. 
						// 11/28/2011 Paul.  We must clear the lists, otherwise searches will eventually exhaust available memory. 
						LoginViewUI_ClearModuleLists();
					}
					
					// 09/09/2014 Paul.  GetUserProfile is already called in Login, so don't call again. 
					//bgPage.GetUserProfile(function(status, message)
					//{
						// 10/19/2011 Paul.  IE6 does not support localStorage. 
						// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
						if ( status == 1 && window.localStorage )
						{
							localStorage['USER_ID'         ] = sUSER_ID         ;
							localStorage['USER_NAME'       ] = sUSER_NAME       ;
							localStorage['FULL_NAME'       ] = sFULL_NAME       ;
							// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
							localStorage['PICTURE'         ] = sPICTURE         ;
							localStorage['TEAM_ID'         ] = sTEAM_ID         ;
							localStorage['TEAM_NAME'       ] = sTEAM_NAME       ;
							localStorage['USER_HASH'       ] = Sha1.hash(sPASSWORD);
							localStorage['USER_LANG'       ] = sUSER_LANG       ;
							// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
							localStorage['USER_THEME'      ] = sUSER_THEME      ;
							localStorage['USER_DATE_FORMAT'] = sUSER_DATE_FORMAT;
							localStorage['USER_TIME_FORMAT'] = sUSER_TIME_FORMAT;
							localStorage['USER_CURRENCY_ID'] = sUSER_CURRENCY_ID;
							localStorage['USER_TIMEZONE_ID'] = sUSER_TIMEZONE_ID;
							// 12/01/2014 Paul.  Add SignalR fields. 
							localStorage['USER_EXTENSION'     ] = sUSER_EXTENSION     ;
							localStorage['USER_FULL_NAME'     ] = sUSER_FULL_NAME     ;
							localStorage['USER_PHONE_WORK'    ] = sUSER_PHONE_WORK    ;
							localStorage['USER_SMS_OPT_IN'    ] = sUSER_SMS_OPT_IN    ;
							localStorage['USER_PHONE_MOBILE'  ] = sUSER_PHONE_MOBILE  ;
							localStorage['USER_TWITTER_TRACKS'] = sUSER_TWITTER_TRACKS;
							localStorage['USER_CHAT_CHANNELS' ] = sUSER_CHAT_CHANNELS ;
							// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
							if ( bMOBILE_CLIENT )
							{
								localStorage['REMOTE_SERVER'  ] = sREMOTE_SERVER;
							}
						}
						// 09/30/2011 Paul.  Clear the password for security reasons. 
						// 11/27/2011 Paul.  Clearing the password prevents automatic re-authentication when the connection is restored. 
						//sPASSWORD = '';
						// 11/28/2011 Paul.  We need to save the user name and password so that we can re-authenticate when the connection is restored. 
						// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
						if ( window.sessionStorage )
						{
							sessionStorage['PASSWORD'        ] = sPASSWORD        ;
							sessionStorage['USER_ID'         ] = sUSER_ID         ;
							sessionStorage['USER_NAME'       ] = sUSER_NAME       ;
							sessionStorage['FULL_NAME'       ] = sFULL_NAME       ;
							// 11/25/2014 Paul.  sPICTURE is used by the ChatDashboard. 
							sessionStorage['PICTURE'         ] = sPICTURE         ;
							sessionStorage['TEAM_ID'         ] = sTEAM_ID         ;
							sessionStorage['TEAM_NAME'       ] = sTEAM_NAME       ;
							sessionStorage['USER_LANG'       ] = sUSER_LANG       ;
							// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
							sessionStorage['USER_THEME'      ] = sUSER_THEME      ;
							sessionStorage['USER_DATE_FORMAT'] = sUSER_DATE_FORMAT;
							sessionStorage['USER_TIME_FORMAT'] = sUSER_TIME_FORMAT;
							sessionStorage['USER_CURRENCY_ID'] = sUSER_CURRENCY_ID;
							sessionStorage['USER_TIMEZONE_ID'] = sUSER_TIMEZONE_ID;
							// 12/01/2014 Paul.  Add SignalR fields. 
							sessionStorage['USER_EXTENSION'     ] = sUSER_EXTENSION     ;
							sessionStorage['USER_FULL_NAME'     ] = sUSER_FULL_NAME     ;
							sessionStorage['USER_PHONE_WORK'    ] = sUSER_PHONE_WORK    ;
							sessionStorage['USER_SMS_OPT_IN'    ] = sUSER_SMS_OPT_IN    ;
							sessionStorage['USER_PHONE_MOBILE'  ] = sUSER_PHONE_MOBILE  ;
							sessionStorage['USER_TWITTER_TRACKS'] = sUSER_TWITTER_TRACKS;
							sessionStorage['USER_CHAT_CHANNELS' ] = sUSER_CHAT_CHANNELS ;
							// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
							if ( bMOBILE_CLIENT )
							{
								sessionStorage['REMOTE_SERVER'] = sREMOTE_SERVER;
							}
						}
						
						// 08/24/2014 Paul.  Remember the last successful login location and repeat on startup. 
						localStorage['LastLoginRemote'] = false;
						// 12/10/2014 Paul.  After successful login, clear the form so that the user will see a change on a slow device. 
						LoginViewUI_Clear(sLayoutPanel, sActionsPanel, function(status, message)
						{
						});
						
						// 12/07/2014 Paul.  Load last active module after login. 
						var sLastModuleTab = '';
						if ( window.localStorage )
							sLastModuleTab = localStorage['LastActiveModule'];
						else
							sLastModuleTab = getCookie('LastActiveModule');
						if ( !Sql.IsEmptyString(sLastModuleTab) )
							sSTARTUP_MODULE = sLastModuleTab;
						var rowDefaultSearch = null;
						SplendidUI_Init(sLayoutPanel, sActionsPanel, sSTARTUP_MODULE, rowDefaultSearch, function(status, message)
						{
							if ( status == 1 )
							{
								LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, true);
								// 10/16/2012 Paul.  Always load the global layout cache if it has not been loaded. 
								//if ( bENABLE_OFFLINE )
								{
									SplendidUI_Cache(function(status, message)
									{
										if ( status == 2 )
										{
											SplendidError.SystemMessage(message);
										}
									});
								}
								if ( cbLoginComplete != null )
									cbLoginComplete(1, null);
								SplendidError.SystemMessage('');
							}
							else
							{
								SplendidError.SystemMessage(message);
							}
						});
						
						// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
						// 12/02/2014 Paul.  We need to start the connection when the page is loaded on the Mobile Client. 
						/*
						if ( status == 1 && bMOBILE_CLIENT )
						{
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
						}
						*/
					//});
				}
				else
				{
					spnError.innerHTML = message;
				}
			});
		}
		else if ( sCommandName == 'Logout' )
		{
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.Logout(function(status, message)
			{
				try
				{
					// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
					if ( bMOBILE_CLIENT )
					{
						try
						{
							SignalR_Connection_Stop();
						}
						catch(e)
						{
						}
					}
					ClearSystemMessage();
					SplendidError.ClearAllErrors();
					// 11/28/2011 Paul.  We need to save the user name and password so that we can re-authenticate when the connection is restored. 
					if ( window.XMLHttpRequest )
					{
						sessionStorage.clear();
					}
					sUSER_NAME  = '';
					sUSER_ID    = '';
					sUSER_THEME = '';
					// 10/15/2011 Paul.  sTabMenuCtl is a div tag now so that we can do more with the panel. 
					// 04/23/2013 Paul.  New approach to menu management. 
					var sLayoutPanel  = 'divMainLayoutPanel';
					var sActionsPanel = 'divMainActionsPanel';
					ctlActiveMenu = new TabMenuUI_None(sLayoutPanel, sActionsPanel);
					ctlActiveMenu.RenderHeader();
					SplendidUI_Clear(sLayoutPanel, sActionsPanel);
					LoginViewUI_Load(sLayoutPanel, sActionsPanel
					, function(status, message)  // cbLoginComplete
					{
						if ( status == 1 )
						{
							// 08/26/2014 Paul.  This is not the best place to set the last login value. 
							//localStorage['LastLoginRemote'] = false;
							SplendidError.SystemMessage('');
						}
					}
					, function(status, message)  // callback
					{
						if ( status == 1 )
						{
							SplendidError.SystemMessage('');
						}
						else
						{
							SplendidError.SystemMessage(message);
						}
					});
				}
				catch(e)
				{
					SplendidError.SystemError(e, 'LoginViewUI_PageCommand Logout()');
				}
			});
		}
		else
		{
			SplendidError.SystemMessage('LoginViewUI_PageCommand: Unknown command ' + sCommandName);
		}
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'LoginViewUI_PageCommand');
	}
}

function LoginViewUI_Clear(sLayoutPanel, sActionsPanel, callback)
{
	try
	{
		var divMainLayoutPanel = document.getElementById(sLayoutPanel);
		if ( divMainLayoutPanel != null && divMainLayoutPanel.childNodes != null )
		{
			while ( divMainLayoutPanel.childNodes.length > 0 )
			{
				divMainLayoutPanel.removeChild(divMainLayoutPanel.firstChild);
			}
		}
		var divMainActionsPanel = document.getElementById(sActionsPanel);
		if ( divMainActionsPanel != null && divMainActionsPanel.childNodes != null )
		{
			while ( divMainActionsPanel.childNodes.length > 0 )
			{
				divMainActionsPanel.removeChild(divMainActionsPanel.firstChild);
			}
		}
		LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, false);
		callback(1, null);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'LoginViewUI_Clear'));
	}
}

function LoginViewUI_LoadView(sLayoutPanel, sActionsPanel, cbLoginComplete, callback)
{
	try
	{
		// 12/10/2014 Paul.  Use new mobile flag. 
		var bIsMobile = isMobileDevice();
		if ( isMobileLandscape() )
			bIsMobile = false;

		var divMainLayoutPanel = document.getElementById(sLayoutPanel);
		var divSpacer = document.createElement('div');
		// 12/10/2014 Paul.  No spacer on a mobile device. 
		if ( !bIsMobile )
		{
			divSpacer.style.height = '40px';
			divMainLayoutPanel.appendChild(divSpacer);
		}

		//<table class="ModuleActionsShadingTable" cellspacing="0" cellpadding="0" align="Center" border="0" style="border-collapse:collapse;width: 450px;">
		var tblMain = document.createElement('table');
		tblMain.id          = sLayoutPanel + '_ctlLoginView_tblMain';
		// 09/25/2011 Paul.  Shrink the table so that it will fit within an Office Extension. 
		// 08/16/2014 Paul.  Not using for Office Extension, so increase width. 
		tblMain.style.width = '550px';
		tblMain.className   = 'ModuleActionsShadingTable';
		tblMain.cellSpacing = 0;
		tblMain.cellPadding = 0;
		tblMain.border      = 0;
		tblMain.align       = 'Center';
		tblMain.style.borderCollapse = 'collapse';
		// 12/10/2014 Paul.  Don't use the outer frame on mobile. 
		if ( !bIsMobile )
			divMainLayoutPanel.appendChild(tblMain);
		
		// 09/25/2011 Paul.  Internet Explorer requires tbody when using the DOM to add tr and td elements. 
		var tbody = document.createElement('tbody');
		tblMain.appendChild(tbody);

		// <tr><td class="ModuleActionsShadingHorizontal" colspan="3"></td></tr>
		var tr = document.createElement('tr');
		tbody.appendChild(tr);
		var td = document.createElement('td');
		tr.appendChild(td);
		td.className = 'ModuleActionsShadingHorizontal';
		td.colSpan   = 3;

		// <td class="ModuleActionsShadingVertical"></td>
		tr = document.createElement('tr');
		tbody.appendChild(tr);
		td = document.createElement('td');
		tr.appendChild(td);
		td.className = 'ModuleActionsShadingVertical';

		// <table class="ModuleActionsInnerTable" cellspacing="0" cellpadding="0" align="Center" border="0" style="width:100%;border-collapse:collapse;">
		var tblInner = document.createElement('table');
		tblInner.width       = '100%';
		tblInner.className   = 'ModuleActionsInnerTable';
		tblInner.cellSpacing = 0;
		tblInner.cellPadding = 0;
		tblInner.border      = 0;
		tblInner.align       = 'Center';
		tblInner.style.borderCollapse = 'collapse';
		td = document.createElement('td');
		tr.appendChild(td);
		td.appendChild(tblInner);

		var tbodyInner = document.createElement('tbody');
		tblInner.appendChild(tbodyInner);
		var trInner = document.createElement('tr');
		tbodyInner.appendChild(trInner);
		var tdInner = document.createElement('td');
		trInner.appendChild(tdInner);
		// <td style="padding-top: 20px; padding-bottom: 20px; padding-left: 40px; padding-right: 40px;">
		tdInner.style.paddingTop    = '20px';
		tdInner.style.paddingBottom = '20px';
		tdInner.style.paddingLeft   = '40px';
		tdInner.style.paddingRight  = '40px';
		// 12/10/2014 Paul.  Don't use the outer frame on mobile. 
		if ( bIsMobile )
			tdInner = divMainLayoutPanel;

		// <table cellspacing="2" cellpadding="0" border="0" style="border-width:0px;width:100%;">
		var tblHeader = document.createElement('table');
		tblHeader.cellSpacing = 2;
		tblHeader.cellPadding = 0;
		tblHeader.border      = 0;
		tblHeader.width       = '100%';
		tdInner.appendChild(tblHeader);

		var tbodyHeader = document.createElement('tbody');
		tblHeader.appendChild(tbodyHeader);
		var trHeader = document.createElement('tr');
		tbodyHeader.appendChild(trHeader);
		// <td style="font-family: Arial; font-size: 14pt; font-weight: bold; color: #003564;">SplendidCRM Enterprise</td>
		var tdHeader = document.createElement('td');
		tdHeader.style.fontFamily = 'Arial';
		tdHeader.style.fontSize   = '14pt';
		tdHeader.style.fontWeight = 'bold';
		tdHeader.color            = '#003564';
		trHeader.appendChild(tdHeader);
		var txt = document.createTextNode(sPRODUCT_TITLE);
		tdHeader.appendChild(txt);

		// <table id="ctlLoginView_tblUser" cellspacing="2" cellpadding="0" align="Center" border="0" style="border-width:0px;width:100%;">
		var tblUser = document.createElement('table');
		tblUser.id          = sLayoutPanel + '_ctlLoginView_tblUser';
		tblUser.cellSpacing = 2;
		tblUser.cellPadding = 0;
		tblUser.border      = 0;
		tblUser.width       = '100%';
		tblUser.align       = 'Center';
		tdInner.appendChild(tblUser);

		var tbodyUser = document.createElement('tbody');
		tblUser.appendChild(tbodyUser);
		var trUser = document.createElement('tr');
		tbodyUser.appendChild(trUser);
		// <td colspan="2" style="font-size: 12px; padding-top: 5px;"><span id="ctlLoginView_lblInstructions">Please login.</span></td>
		var tdUser = document.createElement('td');
		tdUser.colSpan          = 2;
		tdUser.style.fontFamily = 'Arial';
		tdUser.style.fontSize   = '12pt';
		//tdUser.className = 'dataLabel';
		trUser.appendChild(tdUser);
		var spn = document.createElement('span');
		spn.id = sLayoutPanel + '_ctlLoginView_lblInstructions';
		tdUser.appendChild(spn);
		txt = document.createTextNode(L10n.Term('.NTC_LOGIN_MESSAGE'));
		spn.appendChild(txt);

		trUser = document.createElement('tr');
		tbodyUser.appendChild(trUser);
		// <td colspan="2"><span id="ctlLoginView_lblError" class="error"></span></td>
		tdUser = document.createElement('td');
		tdUser.colSpan = 2;
		trUser.appendChild(tdUser);
		spn = document.createElement('span');
		spn.id        = sLayoutPanel + '_ctlLoginView_lblError';
		spn.className = 'error';
		tdUser.appendChild(spn);

		// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
		if ( bMOBILE_CLIENT )
		{
			trUser = document.createElement('tr');
			tbodyUser.appendChild(trUser);
			tdUserLabel = document.createElement('td');
			tdUserLabel.className = 'dataLabel';
			tdUserLabel.width     = '30%';
			trUser.appendChild(tdUserLabel);
			spn = document.createElement('span');
			spn.id        = sLayoutPanel + '_ctlLoginView_lblRemoteServer';
			spn.className = 'dataLabel';
			tdUserLabel.appendChild(spn);
			txt = document.createTextNode(L10n.Term('Offline.LBL_REMOTE_SERVER'));
			spn.appendChild(txt);
		
			// 12/10/2014 Paul.  Place label on its own line. 
			if ( bIsMobile )
			{
				trUser = document.createElement('tr');
				tbodyUser.appendChild(trUser);
			}
			
			var tdUserField = document.createElement('td');
			tdUserField.width = '70%';
			trUser.appendChild(tdUserField);
			var txtREMOTE_SERVER = document.createElement('input');
			txtREMOTE_SERVER.id    = sLayoutPanel + '_ctlLoginView_txtREMOTE_SERVER';
			txtREMOTE_SERVER.type  = 'text';
			if ( bIsMobile )
			{
				txtREMOTE_SERVER.style.width = '90%';
			}
			else
			{
				txtREMOTE_SERVER.width = 360;
				txtREMOTE_SERVER.style.width = '360px';
			}
			// 08/15/2014 Paul.  The border is not displayed on WinRT. 
			txtREMOTE_SERVER.style.border = '1px solid #003564';
			txtREMOTE_SERVER.onkeypress = function(e)
			{
				return RegisterEnterKeyPress(e, sLayoutPanel + '_ctlLoginView_btnLogin');
			};
			tdUserField.appendChild(txtREMOTE_SERVER);
			if ( window.localStorage && localStorage['REMOTE_SERVER'] !== undefined )
				txtREMOTE_SERVER.value = localStorage['REMOTE_SERVER'];
		}

		trUser = document.createElement('tr');
		tbodyUser.appendChild(trUser);
		// <td class="dataLabel" style="width:30%;">User Name:</td><td style="width:70%;"><input type="text" id="ctlLoginView_txtUSER_NAME" value="admin" style="width:180px;" /> &nbsp;</td>
		tdUserLabel = document.createElement('td');
		tdUserLabel.className = 'dataLabel';
		tdUserLabel.width     = '30%';
		trUser.appendChild(tdUserLabel);
		spn = document.createElement('span');
		spn.id        = sLayoutPanel + '_ctlLoginView_lblUserName';
		spn.className = 'dataLabel';
		tdUserLabel.appendChild(spn);
		txt = document.createTextNode(L10n.Term('Users.LBL_USER_NAME'));
		spn.appendChild(txt);
		
		// 12/10/2014 Paul.  Place label on its own line. 
		if ( bIsMobile )
		{
			trUser = document.createElement('tr');
			tbodyUser.appendChild(trUser);
		}
		
		var tdUserField = document.createElement('td');
		tdUserField.width = '70%';
		trUser.appendChild(tdUserField);
		var txtUSER_NAME = document.createElement('input');
		txtUSER_NAME.id    = sLayoutPanel + '_ctlLoginView_txtUSER_NAME';
		txtUSER_NAME.type  = 'text';
		if ( bIsMobile )
		{
			txtUSER_NAME.style.width = '90%';
		}
		else
		{
			txtUSER_NAME.width = 240;
			txtUSER_NAME.style.width = '240px';
		}
		// 08/15/2014 Paul.  The border is not displayed on WinRT. 
		txtUSER_NAME.style.border = '1px solid #003564';
		txtUSER_NAME.onkeypress = function(e)
		{
			return RegisterEnterKeyPress(e, sLayoutPanel + '_ctlLoginView_btnLogin');
		};
		tdUserField.appendChild(txtUSER_NAME);
		// 11/29/2011 Paul.  Make logins easier by recalling the last login. 
		// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
		if ( window.localStorage && localStorage['USER_NAME'] !== undefined )
			txtUSER_NAME.value = localStorage['USER_NAME'];

		trUser = document.createElement('tr');
		tbodyUser.appendChild(trUser);
		// <td class="dataLabel" style="width:30%;">User Name:</td><td style="width:70%;"><input type="text" id="ctlLoginView_txtUSER_NAME" value="admin" style="width:180px;" /> &nbsp;</td>
		tdUserLabel = document.createElement('td');
		tdUserLabel.className = 'dataLabel';
		tdUserLabel.width     = '30%';
		trUser.appendChild(tdUserLabel);
		spn = document.createElement('span');
		spn.id        = sLayoutPanel + '_ctlLoginView_lblUserName';
		spn.className = 'dataLabel';
		tdUserLabel.appendChild(spn);
		txt = document.createTextNode(L10n.Term('Users.LBL_PASSWORD'));
		spn.appendChild(txt);
		
		// 12/10/2014 Paul.  Place label on its own line. 
		if ( bIsMobile )
		{
			trUser = document.createElement('tr');
			tbodyUser.appendChild(trUser);
		}
		
		tdUserField = document.createElement('td');
		tdUserField.width = '70%';
		trUser.appendChild(tdUserField);
		var txtPASSWORD = document.createElement('input');
		txtPASSWORD.id    = sLayoutPanel + '_ctlLoginView_txtPASSWORD';
		txtPASSWORD.type  = 'password';
		if ( bIsMobile )
		{
			txtPASSWORD.style.width = '90%';
		}
		else
		{
			txtPASSWORD.wdith = 240;
			txtPASSWORD.style.width = '240px';
		}
		// 08/15/2014 Paul.  The border is not displayed on WinRT. 
		txtPASSWORD.style.border = '1px solid #003564';
		txtPASSWORD.onkeypress = function(e)
		{
			return RegisterEnterKeyPress(e, sLayoutPanel + '_ctlLoginView_btnLogin');
		};
		tdUserField.appendChild(txtPASSWORD);

		// 12/10/2014 Paul.  Remove the blank lines on a mobile layout. 
		if ( !bIsMobile )
		{
			trUser = document.createElement('tr');
			tbodyUser.appendChild(trUser);
			tdUserLabel = document.createElement('td');
			tdUserLabel.className = 'dataLabel';
			tdUserLabel.width     = '30%';
			trUser.appendChild(tdUserLabel);
			tdUserField = document.createElement('td');
			tdUserField.width = '70%';
			trUser.appendChild(tdUserField);

			// 10/16/2012 Paul.  Always load the global layout cache if it has not been loaded. 
			/*
			var chkENABLE_OFFLINE = document.createElement('input');
			chkENABLE_OFFLINE.id        = sLayoutPanel + '_ctlLoginView_chkENABLE_OFFLINE';
			chkENABLE_OFFLINE.type      = 'checkbox';
			chkENABLE_OFFLINE.className = 'checkbox';
			tdUserField.appendChild(chkENABLE_OFFLINE);
			// 10/04/2011 Paul.  IE8 requires that we set checked after appending. 
			chkENABLE_OFFLINE.checked   = bENABLE_OFFLINE;
			var txtENABLE_OFFLINE = document.createTextNode(' ' + L10n.Term('.LBL_ENABLE_OFFLINE'));
			tdUserField.appendChild(txtENABLE_OFFLINE);
			*/
		}

		trUser = document.createElement('tr');
		tbodyUser.appendChild(trUser);
		tdUserLabel = document.createElement('td');
		tdUserLabel.className = 'dataLabel';
		tdUserLabel.width     = '30%';
		trUser.appendChild(tdUserLabel);

		// 12/10/2014 Paul.  Place label on its own line. 
		if ( bIsMobile )
		{
			trUser = document.createElement('tr');
			tbodyUser.appendChild(trUser);
		}
		
		tdUserField = document.createElement('td');
		tdUserField.width = '70%';
		trUser.appendChild(tdUserField);

		var btnLogin = document.createElement('input');
		btnLogin.id        = sLayoutPanel + '_ctlLoginView_btnLogin';
		btnLogin.type      = 'submit';
		btnLogin.className = 'button';
		btnLogin.value     = L10n.Term('Users.LBL_LOGIN_BUTTON_LABEL');
		btnLogin.title     = btnLogin.value;
		btnLogin.onclick   = function()
		{
			LoginViewUI_PageCommand(sLayoutPanel, sActionsPanel, 'Login', null, cbLoginComplete);
		};
		tdUserField.appendChild(btnLogin);

		// 12/01/2014 Paul.  We need to distinguish between Offline Client and Mobile Client. 
		if ( bREMOTE_ENABLED && !bMOBILE_CLIENT )
		{
			var aWorkOnline = document.createElement('a');
			aWorkOnline.href = '#';
			aWorkOnline.style.paddingLeft = '6px';
			// 12/31/2014 Paul.  Firefox does not like innerText. Use createTextNode. 
			aWorkOnline.appendChild(document.createTextNode(L10n.Term('Offline.LNK_WORK_ONLINE')));
			aWorkOnline.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, cbLoginComplete, callback)
			{
				ClientLoginViewUI_Load(sLayoutPanel, sActionsPanel, cbLoginComplete, callback)
			}, sLayoutPanel, sActionsPanel, cbLoginComplete, callback);
			tdUserField.appendChild(aWorkOnline);
		}

		// <td class="ModuleActionsShadingVertical"></td>
		td = document.createElement('td');
		tr.appendChild(td);
		td.className = 'ModuleActionsShadingVertical';

		// <tr><td class="ModuleActionsShadingHorizontal" colspan="3"></td></tr>
		tr = document.createElement('tr');
		tbody.appendChild(tr);
		td = document.createElement('td');
		tr.appendChild(td);
		td.className = 'ModuleActionsShadingHorizontal';
		td.colSpan   = 3;

		// 12/10/2014 Paul.  No spacer on a mobile device. 
		if ( !bIsMobile )
		{
			divSpacer = document.createElement('div');
			divSpacer.style.height = '40px';
			divMainLayoutPanel.appendChild(divSpacer);
		}

		// <div id="divFooterCopyright" align="center" class="copyRight">
		var divFooterCopyright = document.getElementById('divFooterCopyright');
		// 12/10/2014 Paul.  No copyright on a mobile device. 
		if ( divFooterCopyright == null && !bIsMobile )
		{
			divFooterCopyright = document.createElement('div');
			divFooterCopyright.id        = 'divFooterCopyright';
			divFooterCopyright.align     = 'center';
			divFooterCopyright.className = 'copyRight';
			divMainLayoutPanel.appendChild(divFooterCopyright);

			// Copyright &copy; 2005-2013 <a id="lnkSplendidCRM" href="http://www.splendidcrm.com" target="_blank" class="copyRightLink">SplendidCRM Software, Inc.</a> All Rights Reserved.<br />
			txt = document.createTextNode('Copyright (C) 2005-2015 ');
			divFooterCopyright.appendChild(txt);

			var lnkSplendidCRM = document.createElement('a');
			lnkSplendidCRM.id        = 'lnkSplendidCRM';
			lnkSplendidCRM.className = 'copyRightLink';
			lnkSplendidCRM.href      = 'http://www.splendidcrm.com';
			// 12/31/2014 Paul.  Firefox does not like innerText. Use createTextNode. 
			lnkSplendidCRM.appendChild(document.createTextNode('SplendidCRM Software, Inc.'));
			lnkSplendidCRM.target    = '_blank';
			divFooterCopyright.appendChild(lnkSplendidCRM);
			txt = document.createTextNode(' All Rights Reserved.');
			divFooterCopyright.appendChild(txt);
		}

		if ( txtUSER_NAME.value.length > 0 )
			txtPASSWORD.focus();
		else
			txtUSER_NAME.focus();
		callback(1, null);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'LoginViewUI_LoadView'));
	}
}

function LoginViewUI_Load(sLayoutPanel, sActionsPanel, cbLoginComplete, callback)
{
	try
	{
		LoginViewUI_Clear(sLayoutPanel, sActionsPanel, function(status, message)
		{
			if ( status == 1 )
			{
				LoginViewUI_LoadView(sLayoutPanel, sActionsPanel, cbLoginComplete, function(status, message)
				{
					if ( status == 1 )
					{
						callback(1, '');
					}
					else
					{
						callback(status, message);
					}
				});
			}
			else
			{
				callback(status, message);
			}
		});
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'LoginViewUI_Load'));
	}
}


