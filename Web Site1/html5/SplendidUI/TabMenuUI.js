/* Copyright (C) 2011-2015 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var bADMIN_MENU            = false;
var sMENU_ACTIVE_MODULE    = '';
var pnlMENU_ACTIVE_SUBMENU = null;
var ctlActiveMenu          = null;

function TabMenuUI_None(sLayoutPanel, sActionsPanel)
{
	this.LayoutPanel  = sLayoutPanel;
	this.ActionsPanel = sActionsPanel;
	this.divHeader_divError           = null;
	this.divHeader_divAuthenticated   = null;
	this.divHeader_spnWelcome         = null;
	this.divHeader_spnUserName        = null;
	this.divHeader_spnLogout          = null;
	this.divHeader_lnkLogout          = null;
	// 08/22/2014 Paul.  Add SyncNow for offline client. 
	this.divHeader_spnSyncNow         = null;
	this.divHeader_lnkSyncNow         = null;
	this.divHeader_divOnlineStatus    = null;
	this.divHeader_divOfflineCache    = null;
	this.divHeader_divSplendidStorage = null;
	this.lnkHeaderCacheAll            = null;
	this.lnkHeaderSystemLog           = null;
	this.lnkHeaderSplendidStorage     = null;
}

TabMenuUI_None.prototype.divError           = function() { return this.divHeader_divError          ; }
TabMenuUI_None.prototype.divAuthenticated   = function() { return this.divHeader_divAuthenticated  ; }
TabMenuUI_None.prototype.spnWelcome         = function() { return this.divHeader_spnWelcome        ; }
TabMenuUI_None.prototype.spnUserName        = function() { return this.divHeader_spnUserName       ; }
TabMenuUI_None.prototype.spnLogout          = function() { return this.divHeader_spnLogout         ; }
TabMenuUI_None.prototype.lnkLogout          = function() { return this.divHeader_lnkLogout         ; }
// 08/22/2014 Paul.  Add SyncNow for offline client. 
TabMenuUI_None.prototype.spnSyncNow         = function() { return this.divHeader_spnSyncNow        ; }
TabMenuUI_None.prototype.lnkSyncNow         = function() { return this.divHeader_lnkSyncNow        ; }
TabMenuUI_None.prototype.divOnlineStatus    = function() { return this.divHeader_divOnlineStatus   ; }
TabMenuUI_None.prototype.divOfflineCache    = function() { return this.divHeader_divOfflineCache   ; }
TabMenuUI_None.prototype.divSplendidStorage = function() { return this.divHeader_divSplendidStorage; }
TabMenuUI_None.prototype.lnkCacheAll        = function() { return this.lnkHeaderCacheAll           ; }
TabMenuUI_None.prototype.lnkSystemLog       = function() { return this.lnkHeaderSystemLog          ; }
TabMenuUI_None.prototype.lnkSplendidStorage = function() { return this.lnkHeaderSplendidStorage    ; }

TabMenuUI_None.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, callback)
{
}

// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
TabMenuUI_None.prototype.ActivateTab = function(sMODULE_NAME, sID, sLAYOUT_MODE)
{
}

TabMenuUI_None.prototype.Render = function(sLayoutPanel, sActionsPanel, arrDetailViewRelationship, arrQuickCreate, result)
{
}

TabMenuUI_None.prototype.RenderHeader = function()
{
	try
	{
		// 05/06/2013 Paul.  Clear pointers as the tags will be deleted. 
		this.divHeader_divError           = null;
		this.divHeader_divAuthenticated   = null;
		this.divHeader_spnWelcome         = null;
		this.divHeader_spnUserName        = null;
		this.divHeader_spnLogout          = null;
		this.divHeader_lnkLogout          = null;
		// 08/22/2014 Paul.  Add SyncNow for offline client. 
		this.divHeader_spnSyncNow         = null;
		this.divHeader_lnkSyncNow         = null;
		this.divHeader_divOnlineStatus    = null;
		this.divHeader_divOfflineCache    = null;
		this.divHeader_divSplendidStorage = null;
		this.lnkHeaderCacheAll            = null;
		this.lnkHeaderSystemLog           = null;
		this.lnkHeaderSplendidStorage     = null;
		TabMenuUI_Clear('ctlHeader');
		TabMenuUI_Clear('ctlTabMenu');
		TabMenuUI_Clear('ctlAtlanticToolbar');
		var ctlHeader = document.getElementById('ctlHeader');
		if ( ctlHeader != null )
		{
			// <table id="tblHeader" border="0" cellpadding="0" cellspacing="0" width="100%" style=" background-color: #ffd14e">
			var tblHeader = document.createElement('table');
			tblHeader.id          = 'tblHeader';
			tblHeader.cellPadding = 0;
			tblHeader.cellSpacing = 0;
			tblHeader.border      = 0;
			tblHeader.width       = '100%';
			tblHeader.style.backgroundColor = '#ffd14e';
			ctlHeader.appendChild(tblHeader);
			var tr = document.createElement('tr');
			tblHeader.appendChild(tr);

			var td = document.createElement('td');
			td.width  = '50%';
			td.vAlign = 'top';
			tr.appendChild(td);
			// <table border="0" cellpadding="0" cellspacing="0" width="100%">
			var tblLogo = document.createElement('table');
			tblLogo.cellPadding = 0;
			tblLogo.cellSpacing = 0;
			tblLogo.border      = 0;
			tblLogo.width       = '100%';
			td.appendChild(tblLogo);
			tr = document.createElement('tr');
			tblLogo.appendChild(tr);

			td = document.createElement('td');
			td.width  = '207';
			tr.appendChild(td);
			//	 <a href="#" onclick="Reload();">
			//	 	<img id="divHeader_imgCompanyLogo" src="../Include/images/SplendidCRM_Logo.gif" alt="SplendidCRM" width="207" height="60" border="0" />
			//	 </a>
			var aLogo = document.createElement('a');
			aLogo.href    = '#';
			aLogo.onclick = function()
			{
				Reload();
			};
			td.appendChild(aLogo);
			var divHeader_imgCompanyLogo = document.createElement('img');
			divHeader_imgCompanyLogo.id     = 'divHeader_imgCompanyLogo';
			divHeader_imgCompanyLogo.src    = sIMAGE_SERVER + 'Include/images/SplendidCRM_Logo.gif';
			divHeader_imgCompanyLogo.alt    = 'SplendidCRM';
			divHeader_imgCompanyLogo.width  = 207;
			divHeader_imgCompanyLogo.height = 60;
			divHeader_imgCompanyLogo.border = 0;
			aLogo.appendChild(divHeader_imgCompanyLogo);
			td = document.createElement('td');
			tr.appendChild(td);
			// <div id="lblError" class="error"></div>
			this.divHeader_divError = document.createElement('div');
			this.divHeader_divError.id        = 'lblError';
			this.divHeader_divError.className = 'error';
			td.appendChild(this.divHeader_divError);

			td = document.createElement('td');
			td.width  = '50%';
			td.vAlign = 'top';
			tr.appendChild(td);
			var tblStatus = document.createElement('table');
			tblStatus.cellPadding = 2;
			tblStatus.cellSpacing = 0;
			tblStatus.border      = 0;
			tblStatus.width       = '100%';
			td.appendChild(tblStatus);
			tr = document.createElement('tr');
			tblStatus.appendChild(tr);

			td = document.createElement('td');
			td.vAlign = 'top';
			tr.appendChild(td);
			td = document.createElement('td');
			td.vAlign = 'top';
			tr.appendChild(td);
			// <div id="divHeader_divOfflineCache" style="vertical-align: top; height: 60px; overflow-y: scroll; width: 100%;"></div>
			this.divHeader_divOfflineCache = document.createElement('div');
			this.divHeader_divOfflineCache.id                  = 'divHeader_divOfflineCache';
			this.divHeader_divOfflineCache.className           = 'error';
			this.divHeader_divOfflineCache.style.verticalAlign = 'top';
			this.divHeader_divOfflineCache.style.height        = '60px';
			this.divHeader_divOfflineCache.style.width         = '100%';
			this.divHeader_divOfflineCache.style.overflowY     = 'scroll';
			td.appendChild(this.divHeader_divOfflineCache);

			// <td align="right" class="myArea" nowrap="nowrap" style="padding-right: 10px;" width="220">
			td = document.createElement('td');
			td.className          = 'myArea';
			td.align              = 'right';
			td.width              = '220';
			td.setAttribute('nowrap', 'nowrap');  // 04/25/2013 Paul.  IE9 is ignoring the nowrap whiteSpace style. 
			td.style.whiteSpace   = 'nowrap';
			td.style.paddingRight = '10px';
			tr.appendChild(td);

			// 	<div id="divHeader_divOnlineStatus" class="welcome"></div>
			this.divHeader_divOnlineStatus = document.createElement('div');
			this.divHeader_divOnlineStatus.id        = 'divHeader_divOnlineStatus';
			this.divHeader_divOnlineStatus.className = 'welcome';
			td.appendChild(this.divHeader_divOnlineStatus);
			// 	<div id="divHeader_divSystemLog" style="white-space: nowrap">
			// 		<a id="lnkHeaderSystemLog" href="#" onclick="ShowSystemLog()" class="welcome">System Log</a>
			// 	</div>
			var divHeader_divSystemLog = document.createElement('div');
			divHeader_divSystemLog.id               = 'divHeader_divSystemLog';
			divHeader_divSystemLog.style.whiteSpace = 'nowrap';
			td.appendChild(divHeader_divSystemLog);
			this.lnkHeaderSystemLog = document.createElement('a');
			this.lnkHeaderSystemLog.id        = 'lnkHeaderSystemLog';
			this.lnkHeaderSystemLog.href      = '#';
			this.lnkHeaderSystemLog.className = 'welcome';
			this.lnkHeaderSystemLog.innerHTML = 'System Log';
			this.lnkHeaderSystemLog.onclick = function()
			{
				ShowSystemLog();
			};
			divHeader_divSystemLog.appendChild(this.lnkHeaderSystemLog);
			LoginViewUI_UpdateHeader(this.LayoutPanel, this.ActionsPanel, false);
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'TabMenuUI_None.RenderHeader');
	}
}

function TabMenuUI_Clear(sDiv)
{
	try
	{
		var div = document.getElementById(sDiv);
		if ( div.childNodes != null )
		{
			while ( div.childNodes.length > 0 )
			{
				div.removeChild(div.firstChild);
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'TabMenuUI_Clear');
	}
}

function TabMenuUI_Clicked(sLayoutPanel, sActionsPanel, sMODULE_NAME)
{
	try
	{
		//SplendidError.SystemMessage('Loading tab menu for ' + sMODULE_NAME);
		ctlActiveMenu.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, function(status, message)
		{
			// 02/22/2013 Paul.  The Calendar has a separate view. 
			if ( sMODULE_NAME == 'Calendar' )
			{
				//SplendidError.SystemMessage('Loading Calendar.');
				var oCalendarViewUI = new CalendarViewUI();
				oCalendarViewUI.Clear(sLayoutPanel, sActionsPanel);
				oCalendarViewUI.Load(sLayoutPanel, sActionsPanel, function(status, message)
				{
					if ( status == 1 )
					{
						try
						{
							if ( window.localStorage )
								localStorage['LastActiveModule'] = sMODULE_NAME;
							else
								setCookie('LastActiveModule', sMODULE_NAME, 180);
						}
						catch(e)
						{
							// 03/10/2013 Paul.  IE9 is throwing an out-of-memory error. Just ignore the error. 
							//if ( window.localStorage.remainingSpace !== undefined )
							//	alert('remainingSpace = ' + window.localStorage.remainingSpace);
							SplendidError.SystemLog('TabMenuUI_Clicked: ' + e.message);
						}
						SplendidError.SystemMessage('');
					}
					else
					{
						SplendidError.SystemMessage(message);
					}
				});
			}
			// 11/25/2014 Paul.  The ChatDashboard has a separate view. 
			else if ( sMODULE_NAME == 'ChatDashboard' )
			{
				//SplendidError.SystemMessage('Loading ChatDashboard.');
				var oChatDashboardUI = new ChatDashboardUI();
				oChatDashboardUI.Render(sLayoutPanel, sActionsPanel, function(status, message)
				{
					if ( status == 1 )
					{
						try
						{
							if ( window.localStorage )
								localStorage['LastActiveModule'] = sMODULE_NAME;
							else
								setCookie('LastActiveModule', sMODULE_NAME, 180);
						}
						catch(e)
						{
							// 03/10/2013 Paul.  IE9 is throwing an out-of-memory error. Just ignore the error. 
							//if ( window.localStorage.remainingSpace !== undefined )
							//	alert('remainingSpace = ' + window.localStorage.remainingSpace);
							SplendidError.SystemLog('TabMenuUI_Clicked: ' + e.message);
						}
						SplendidError.SystemMessage('');
					}
					else
					{
						SplendidError.SystemMessage(message);
					}
				});
			}
			else
			{
				var sGRID_NAME = sMODULE_NAME + '.ListView' + sPLATFORM_LAYOUT;
				//SplendidError.SystemMessage('Loading ' + sGRID_NAME + '.');
				var oListViewUI = new ListViewUI();
				oListViewUI.Reset(sLayoutPanel, sMODULE_NAME);
				oListViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, null, function(status, message)
				{
					if ( status == 1 )
					{
						// 12/05/2012 Paul.  Admin modules should not be loaded when the app starts. 
						if ( !bADMIN_MENU )
						{
							try
							{
								if ( window.localStorage )
									localStorage['LastActiveModule'] = sMODULE_NAME;
								else
									setCookie('LastActiveModule', sMODULE_NAME, 180);
							}
							catch(e)
							{
								// 03/10/2013 Paul.  IE9 is throwing an out-of-memory error. Just ignore the error. 
								//if ( window.localStorage.remainingSpace !== undefined )
								//	alert('remainingSpace = ' + window.localStorage.remainingSpace);
								SplendidError.SystemLog('TabMenuUI_Clicked: ' + e.message);
							}
						}
						SplendidError.SystemMessage('');
					}
					else
					{
						SplendidError.SystemMessage(' ' + message);
					}
				});
			}
		});
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'TabMenuUI_Clicked');
	}
}

function TabMenuUI_HidePopup()
{
	if ( pnlMENU_ACTIVE_SUBMENU != null )
	{
		pnlMENU_ACTIVE_SUBMENU.style.display = 'none';
		pnlMENU_ACTIVE_SUBMENU.style.visibility = 'hidden';
		pnlMENU_ACTIVE_SUBMENU = null;
	}
}

function TabMenuUI_PopupManagement(pnlModuleActions, ctlTabMenu_tabMenuInner)
{
	// http://www.quirksmode.org/dom/events/index.html
	ctlTabMenu_tabMenuInner.onmouseover = function(event)
	{
		if ( pnlMENU_ACTIVE_SUBMENU == pnlModuleActions )
			return;
		if ( pnlMENU_ACTIVE_SUBMENU != null )
		{
			pnlMENU_ACTIVE_SUBMENU.style.display    = 'none';
			pnlMENU_ACTIVE_SUBMENU.style.visibility = 'hidden';
			pnlMENU_ACTIVE_SUBMENU = null;
		}
		pnlModuleActions.style.display    = 'inline';
		pnlModuleActions.style.visibility = 'visible';
		pnlModuleActions.style.position   = 'absolute';
		var rect = ctlTabMenu_tabMenuInner.getBoundingClientRect();
		// 09/25/2011 Paul.  Internet Explorer does not support window.pageXOffset and window.pageYOffset. 
		// 12/12/2012 Paul.  Surface returns rect including scrolled values.  Surface does not allow setting style values to integers. 
		if ( navigator.userAgent.indexOf('MSAppHost') > 0 )
		{
			pnlModuleActions.style.left = Math.floor(rect.left  ) + 'px';
			pnlModuleActions.style.top  = Math.floor(rect.bottom) + 'px';  // 12/12/2012 Paul.  There is no gap. 
		}
		else
		{
			if ( window.pageXOffset !== undefined )
				pnlModuleActions.style.left       = rect.left   + window.pageXOffset;
			else if ( document.body.scrollLeft !== undefined )
				pnlModuleActions.style.left       = rect.left   + document.body.scrollLeft;
			if ( window.pageYOffset !== undefined )
				pnlModuleActions.style.top        = rect.bottom + window.pageYOffset;
			else if ( document.body.scrollTop !== undefined )
				pnlModuleActions.style.top        = rect.bottom + document.body.scrollTop - 4;  // 11/30/2012 Paul.  Increase to 4 to remove the gap. 
		}
		// 08/15/2014 Paul.  Don't let popup go off screen. 
		rect = pnlModuleActions.getBoundingClientRect();
		if ( rect.right > $(window).width() )
		{
			pnlModuleActions.style.left = ($(window).width() - (rect.right - rect.left)) + 'px';
		}
		pnlMENU_ACTIVE_SUBMENU = pnlModuleActions;
		//SplendidError.SystemMessage('ctlTabMenu_tabMenuInner Left = ' + rect.left + ', Bottom = ' + rect.bottom );
	};
	// http://www.quirksmode.org/dom/w3c_cssom.html
	ctlTabMenu_tabMenuInner.onmouseout = function(event)
	{
		var rect = pnlModuleActions.getBoundingClientRect();
		//event.pageX;
		//event.pageY;
		// 09/25/2011 Paul.  IE does not support the event. 
		if ( event === undefined )
			event = window.event;
		//SplendidError.SystemMessage('event(' + event.clientX + ', ' + event.clientY + ') rect(' + rect.left + ', ' + rect.top + ', ' + rect.right + ', ' + rect.bottom + ') tabMenuInner');
		//SplendidError.SystemMessage('event.clientX < rect.left ' + (event.clientX < rect.left).toString() + ' || event.clientX > rect.right ' + (event.clientX > rect.right).toString() + ' || event.clientY < rect.top ' + (event.clientY < rect.top).toString() + ' || event.clientY > (rect.bottom + 2) ' + (event.clientY > (rect.bottom + 2)).toString() );
		if ( event.clientX < rect.left || event.clientX > rect.right || event.clientY < rect.top || event.clientY > (rect.bottom + 2) )
		{
			//SplendidError.SystemMessage('event(' + event.clientX + ', ' + event.clientY + ') rect(' + rect.left + ', ' + rect.top + ', ' + rect.right + ', ' + rect.bottom + ') tabMenuInner out');
			pnlModuleActions.style.display    = 'none';
			pnlModuleActions.style.visibility = 'hidden';
			pnlMENU_ACTIVE_SUBMENU = null;
		}
	};
	pnlModuleActions.onmouseover = function(event)
	{
	};
	pnlModuleActions.onmouseout = function(event)
	{
		var rect = pnlModuleActions.getBoundingClientRect();
		// 09/25/2011 Paul.  IE does not support the event. 
		if ( event === undefined )
			event = window.event;
		//SplendidError.SystemMessage('event(' + event.clientX + ', ' + event.clientY + ') rect(' + rect.left + ', ' + rect.top + ', ' + rect.right + ', ' + rect.bottom + ') ModuleActions');
		//SplendidError.SystemMessage('event.clientX < rect.left ' + (event.clientX < rect.left).toString() + ' || event.clientX > rect.right ' + (event.clientX > rect.right).toString() + ' || event.clientY < rect.top ' + (event.clientY < rect.top).toString() + ' || event.clientY > (rect.bottom + 2) ' + (event.clientY > (rect.bottom + 2)).toString() );
		if ( event.clientX < rect.left || event.clientX > rect.right || event.clientY < (rect.top - 2) || event.clientY > rect.bottom )
		{
			pnlModuleActions.style.display    = 'none';
			pnlModuleActions.style.visibility = 'hidden';
			pnlMENU_ACTIVE_SUBMENU = null;
			//SplendidError.SystemMessage('event(' + event.clientX + ', ' + event.clientY + ') rect(' + rect.left + ', ' + rect.top + ', ' + rect.right + ', ' + rect.bottom + ') ModuleActions out');
		}
	};
}


