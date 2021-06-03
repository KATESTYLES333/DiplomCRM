/* Copyright (C) 2011-2015 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function EditViewUI()
{
	this.MODULE    = null;
	this.ID        = null;
	this.DUPLICATE = false;
	// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
	this.LAST_DATE_MODIFIED = null;
	this.cbSaveComplete = null;
}

EditViewUI.prototype.SubmitOffline = function (sLayoutPanel, sActionsPanel, sSaveType)
{
	try
	{
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				// 12/08/2011 Paul.  IE7 defines window.XMLHttpRequest but not window.localStorage. 
				if ( window.localStorage && localStorage['OFFLINE_CACHE'] != null )
				{
					var arrOFFLINE_CACHE = JSON.parse(localStorage['OFFLINE_CACHE']);
					for ( var key in arrOFFLINE_CACHE )
					{
						var oCached = arrOFFLINE_CACHE[key];
						var sID          = oCached.ID         ;
						var sKEY         = oCached.KEY        ;
						var sMODULE_NAME = oCached.MODULE_NAME;
						var sNAME        = oCached.NAME       ;
						SplendidError.SystemMessage('Saving ' + sNAME);
						
						var result = JSON.parse(localStorage[key]);
						var row    = result.d.results;
						// 03/16/2014 Paul.  Add hidden buttons for Save Duplicate and Save Concurrency. 
						if ( sSaveType == 'SaveDuplicate' || sSaveType == 'SaveConcurrency' )
							row[sSaveType] = true;
						//alert(dumpObj(result, 'result'));
						// 10/16/2011 Paul.  sID is now a parameter so that it can be distinguished from Offline ID for a new record. 
						// We want to make sure to send a NULL for new records so that the ID is generated on the server with a true GUID. 
						// JavaScript cannot generate a true GUID, but this generated value should be valid on the single device. 
						bgPage.UpdateModule(sMODULE_NAME, row, sID, function(status, message)
						{
							try
							{
								if ( status == 1 )
								{
									if ( LoginViewUI_UpdateHeader !== undefined )
									{
										LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, true);
									}
									// 10/07/2011 Paul.  The offline cache should have changed. 
									arrOFFLINE_CACHE = JSON.parse(localStorage['OFFLINE_CACHE']);
									// 10/07/2011 Paul.  arrOFFLINE_CACHE.length is not valid. 
									var nOFFLINE_CACHE_length = 0;
									for ( var key in arrOFFLINE_CACHE )
									{
										nOFFLINE_CACHE_length++;
									}
									// 10/16/2011 Paul.  After a successful update of an offline record, lets fetch the full record. 
									// We should be able to load the item in a fire-and-forget mode. 
									bgPage.DetailView_LoadItem(sMODULE_NAME, message, function(status, message)
									{
									}, this);
									// 10/07/2011 Paul.  If the cached item was removed from the cache, then try the next one. 
									//alert('arrOFFLINE_CACHE[key] ' + arrOFFLINE_CACHE[key]);
									// 10/11/2011 Paul.  We are having problems with the iterator key, so use sKEY instead. 
									if ( !bgPage.GetIsOffline() && nOFFLINE_CACHE_length > 0 && arrOFFLINE_CACHE[sKEY] === undefined )
									{
										this.SubmitOffline(sLayoutPanel, sActionsPanel, 'Save');
									}
									else if ( arrOFFLINE_CACHE[sKEY] !== undefined )
									{
										SplendidError.SystemMessage(sID + ' still exists in the cache, which suggests a problem while saving.');
									}
									else
									{
										// 10/16/2011 Paul.  When done with all the updates, lets refresh the current list. 
										var lnkActiveTab = document.getElementById('ctlTabMenu_' + sMENU_ACTIVE_MODULE);
										if ( lnkActiveTab != null )
										{
											lnkActiveTab.onclick();
										}
									}
									// 03/16/2014 Paul.  Hide the save buttons when done. 
									if ( sSaveType == 'SaveDuplicate' )
									{
										var btnSaveDuplicate = document.getElementById('btnSubmit_SaveDuplicate');
										if ( btnSaveDuplicate != null )
											btnSaveDuplicate.style.display = 'none';
									}
									else if ( sSaveType == 'SaveConcurrency' )
									{
										var btnSaveConcurrency = document.getElementById('btnSubmit_SaveConcurrency');
										if ( btnSaveConcurrency != null )
											btnSaveConcurrency.style.display = 'none';
									}
								}
								// 03/16/2014 Paul.  Put the error name at the end so that we can detect the event. 
								else if ( EndsWith(message, '.ERR_DUPLICATE_EXCEPTION') )
								{
									var btnSaveDuplicate = document.getElementById('btnSubmit_SaveDuplicate');
									if ( btnSaveDuplicate != null )
										btnSaveDuplicate.style.display = 'inline';
									//message = message.replace('.ERR_DUPLICATE_EXCEPTION', '');
									message = message.substring(0, message.length - '.ERR_DUPLICATE_EXCEPTION'.length);
									SplendidError.SystemMessage(message);
								}
								else if ( EndsWith(message, '.ERR_CONCURRENCY_OVERRIDE') )
								{
									var btnSaveConcurrency = document.getElementById('btnSubmit_SaveConcurrency');
									if ( btnSaveConcurrency != null )
										btnSaveConcurrency.style.display = 'inline';
									//message = message.replace('.ERR_DUPLICATE_EXCEPTION', '');
									message = message.substring(0, message.length - '.ERR_CONCURRENCY_OVERRIDE'.length);
									SplendidError.SystemMessage(message);
								}
								else
								{
									SplendidError.SystemMessage(message);
								}
							}
							catch(e)
							{
								SplendidError.SystemError(e, 'UpdateModule');
							}
						}, this);
						// 10/07/2011 Paul.  Only save one record at a time. 
						break;
					}
				}
			}
			else
			{
				SplendidError.SystemMessage(message);
			}
		}, this);
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'EditViewUI.SubmitOffline');
	}
}

EditViewUI.prototype.PageCommand = function(sLayoutPanel, sActionsPanel, sCommandName, sCommandArguments)
{
	// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
	// 03/15/2014 Paul.  Enable override of concurrency error. 
	if ( sCommandName == 'Save' || sCommandName == 'SaveDuplicate' || sCommandName == 'SaveConcurrency' )
	{
		try
		{
			if ( !this.Validate(sLayoutPanel, this.MODULE + '.EditView' + sPLATFORM_LAYOUT) )
				return;
			
			// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.AuthenticatedMethod(function(status, message)
			{
				if ( status == 1 )
				{
					var row = new Object();
					// 10/07/2011 Paul.  EditView_LoadItem can accept an empty ID so that we can have one execution path. 
					bgPage.EditView_LoadItem(this.MODULE, this.ID, function(status, message)
					{
						// 10/07/2011 Paul.  The row needs to start with the existing data. 
						if ( status == 1 )
						{
							row = message;
						}
						this.GetValues(sLayoutPanel, this.MODULE + '.EditView' + sPLATFORM_LAYOUT, false, row);
						if ( this.DUPLICATE )
						{
							this.ID = null;
						}
						row['ID'] = this.ID;
						//alert(dumpObj(row, 'row'));
						//alert(JSON.stringify(row));
						// 10/16/2011 Paul.  sID is now a parameter so that it can be distinguished from Offline ID for a new record. 
						// We want to make sure to send a NULL for new records so that the ID is generated on the server with a true GUID. 
						// JavaScript cannot generate a true GUID, but this generated value should be valid on the single device. 
						
						// 03/16/2014 Paul.  Pass Save Override to the update module. 
						if ( sCommandName == 'SaveDuplicate' || sCommandName == 'SaveConcurrency' )
							row[sCommandName] = true;
						// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
						if ( this.LAST_DATE_MODIFIED != null )
							row['LAST_DATE_MODIFIED'] = this.LAST_DATE_MODIFIED;
						bgPage.UpdateModule(this.MODULE, row, this.ID, function(status, message)
						{
							try
							{
								// 10/06/2011 Paul.  Status 3 means that the value was cached. 
								if ( status == 1 || status == 3 )
								{
									this.ID = message;
									// 01/30/2013 Paul.  Update header before loading detail view. 
									if ( status == 1 )
									{
										if ( status == 3 )
											SplendidError.SystemMessage('Record was saved to the offline cache.');
										else
											SplendidError.SystemMessage('');
									}
									if ( LoginViewUI_UpdateHeader !== undefined )
									{
										LoginViewUI_UpdateHeader(sLayoutPanel, sActionsPanel, true);
									}
									// 12/05/2012 Paul.  If a system table is updated, then we will need to update the cached data. 
									if ( bADMIN_MENU )
									{
										SplendidUI_CacheModule(this.MODULE, function(status, message)
										{
											if ( status == 2 )
											{
												SplendidError.SystemMessage(message);
											}
										});
									}
									
									// 10/06/2011 Paul.  We don't need to use DetailViewUI.LoadObject() because we store the offline cached data in the same location as the online cache. 
									var oDetailViewUI = new DetailViewUI();
									// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
									// This is to make sure that the detail view update does not get partially over-written by the save complete. 
									oDetailViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, this.ID, function(status, message)
									{
										if ( status == 1 )
										{
											SplendidError.SystemMessage('');
											// 10/27/2012 Paul.  We need an event to save the relationship. 
											// 01/30/2013 Paul.  Move the save complete after the detail view has been rendered. 
											if ( this.cbSaveComplete != null )
											{
												// 10/27/2012 Paul.  After a successful relationship save, we will be sent to the detail view of the parent. 
												this.cbSaveComplete(this.ID, this.MODULE);
											}
										}
										else
										{
											SplendidError.SystemMessage(message);
										}
										// 01/30/2013 Paul.  Make sure to only clear the module after a cbSaveComplete call. 
										this.MODULE    = null;
										this.ID        = null;
										this.DUPLICATE = false;
										// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
										this.LAST_DATE_MODIFIED = null;
									}, this);
								}
								// 03/16/2014 Paul.  Put the error name at the end so that we can detect the event. 
								else if ( EndsWith(message, '.ERR_DUPLICATE_EXCEPTION') )
								{
									var btnSaveDuplicate = document.getElementById('btnDynamicButtons_SaveDuplicate');
									if ( btnSaveDuplicate != null )
										btnSaveDuplicate.style.display = 'inline';
									//message = message.replace('.ERR_DUPLICATE_EXCEPTION', '');
									message = message.substring(0, message.length - '.ERR_DUPLICATE_EXCEPTION'.length);
									SplendidError.SystemMessage(message);
								}
								else if ( EndsWith(message, '.ERR_CONCURRENCY_OVERRIDE') )
								{
									var btnSaveConcurrency = document.getElementById('btnDynamicButtons_SaveConcurrency');
									if ( btnSaveConcurrency != null )
										btnSaveConcurrency.style.display = 'inline';
									//message = message.replace('.ERR_DUPLICATE_EXCEPTION', '');
									message = message.substring(0, message.length - '.ERR_CONCURRENCY_OVERRIDE'.length);
									SplendidError.SystemMessage(message);
								}
								else
								{
									SplendidError.SystemMessage(message);
								}
							}
							catch(e)
							{
								SplendidError.SystemAlert(e, 'UpdateModule');
							}
						}, this);
					}, this);
				}
				else
				{
					SplendidError.SystemMessage(message);
				}
			}, this);
		}
		catch(e)
		{
			SplendidError.SystemError(e, 'EditViewUI.PageCommand');
		}
	}
	else if ( sCommandName == 'Cancel' )
	{
		// 10/21/2012 Paul.  If this is a new record being cancelled, return to the list. 
		if ( Sql.IsEmptyString(this.ID) )
		{
			var sGRID_NAME = this.MODULE + '.ListView' + sPLATFORM_LAYOUT;
			var oListViewUI = new ListViewUI();
			oListViewUI.Reset(sLayoutPanel, this.MODULE);
			oListViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, sGRID_NAME, null, function(status, message)
			{
				if ( status == 0 || status == 1 )
				{
					this.MODULE    = null;
					this.ID        = null;
					this.DUPLICATE = false;
					// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
					this.LAST_DATE_MODIFIED = null;
				}
			});
		}
		else
		{
			var oDetailViewUI = new DetailViewUI();
			// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
			oDetailViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, this.ID, function(status, message)
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
			this.MODULE    = null;
			this.ID        = null;
			this.DUPLICATE = false;
			// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
			this.LAST_DATE_MODIFIED = null;
		}
	}
	else
	{
		SplendidError.SystemMessage('EditViewUI.PageCommand: Unknown command ' + sCommandName);
	}
}

EditViewUI.prototype.Clear = function(sLayoutPanel)
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
		if ( divMainLayoutPanel == null )
		{
			alert('EditViewUI.Clear: ' + sLayoutPanel + ' does not exist');
			return;
		}
		//<table class="tabForm" cellspacing="1" cellpadding="0" border="0" style="width:100%;">
		//	<tr>
		//		<td>
		//			<table id="ctlEditView_tblMain" class="tabEditView">
		//			</table>
		//		</td>
		//	</tr>
		//</table>
		var tblForm = document.createElement('table');
		var tbody   = document.createElement('tbody');
		var tr      = document.createElement('tr');
		var td      = document.createElement('td');
		tblForm.className   = 'tabForm';
		tblForm.cellSpacing = 1;
		tblForm.cellPadding = 0;
		tblForm.border      = 0;
		tblForm.width       = '100%';
		tblForm.appendChild(tbody);
		tbody.appendChild(tr);
		tr.appendChild(td);
		divMainLayoutPanel.appendChild(tblForm);
		
		var ctlEditView_tblMain = document.createElement('table');
		ctlEditView_tblMain.id        = sLayoutPanel + '_ctlEditView_tblMain';
		ctlEditView_tblMain.width     = '100%';
		ctlEditView_tblMain.className = 'tabEditView';
		td.appendChild(ctlEditView_tblMain);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'EditViewUI.Clear');
	}
}

EditViewUI.prototype.Validate = function(sLayoutPanel, sEDIT_NAME)
{
	var nInvalidFields = 0;
	//alert('sLayoutPanel ' + sLayoutPanel);
	var bEnableTeamManagement  = Crm.Config.enable_team_management();
	var bRequireTeamManagement = Crm.Config.require_team_management();
	var bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
	var bgPage  = chrome.extension.getBackgroundPage();
	var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
	for ( var nLayoutIndex in layout )
	{
		var lay = layout[nLayoutIndex];
		var sFIELD_TYPE                 = lay.FIELD_TYPE                ;
		var sDATA_LABEL                 = lay.DATA_LABEL                ;
		var sDATA_FIELD                 = lay.DATA_FIELD                ;
		var sDISPLAY_FIELD              = lay.DISPLAY_FIELD             ;
		var sLIST_NAME                  = lay.LIST_NAME                 ;
		var sONCLICK_SCRIPT             = lay.ONCLICK_SCRIPT            ;
		var sMODULE_TYPE                = lay.MODULE_TYPE               ;
		// 12/12/2012 Paul.  UI_REQUIRED is not used on SQLite, so use the DATA_REQUIRED value. 
		var bUI_REQUIRED                = Sql.ToBoolean(lay.UI_REQUIRED) || Sql.ToBoolean(lay.DATA_REQUIRED);
		try
		{
			if ( (sDATA_FIELD == 'TEAM_ID' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( !bEnableTeamManagement )
				{
					sFIELD_TYPE = 'Blank';
					bUI_REQUIRED = false;
				}
				else
				{
					if ( bEnableDynamicTeams )
					{
						// 08/31/2009 Paul.  Don't convert to TeamSelect inside a Search view or Popup view. 
						if ( sEDIT_NAME.indexOf('.Search') < 0 && sEDIT_NAME.indexOf('.Popup') < 0 )
						{
							sDATA_LABEL     = '.LBL_TEAM_SET_NAME';
							sDATA_FIELD     = 'TEAM_SET_NAME';
							sFIELD_TYPE     = 'TeamSelect';
							sONCLICK_SCRIPT = '';
						}
					}
					else
					{
						// 04/18/2010 Paul.  If the user manually adds a TeamSelect, we need to convert to a ModulePopup. 
						if ( sFIELD_TYPE == 'TeamSelect' )
						{
							sDATA_LABEL     = 'Teams.LBL_TEAM';
							sDATA_FIELD     = 'TEAM_ID';
							sDISPLAY_FIELD  = 'TEAM_NAME';
							sFIELD_TYPE     = 'ModulePopup';
							sMODULE_TYPE    = 'Teams';
							sONCLICK_SCRIPT = '';
						}
					}
					if ( bRequireTeamManagement )
						bUI_REQUIRED = true;
				}
			}
			if ( bUI_REQUIRED )
			{
				if ( sFIELD_TYPE == 'ModuleAutoComplete' )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
						reqNAME.style.display = Sql.IsEmptyString(txt.value) ? 'inline' : 'none';
						if ( Sql.IsEmptyString(txt.value) )
							nInvalidFields++;
					}
				}
				else if ( sFIELD_TYPE == 'ModulePopup' || sFIELD_TYPE == 'ChangeButton' )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
						reqNAME.style.display = Sql.IsEmptyString(txt.value) ? 'inline' : 'none';
						if ( Sql.IsEmptyString(txt.value) )
							nInvalidFields++;
					}
				}
				else if ( sFIELD_TYPE == 'TextBox' || sFIELD_TYPE == 'HtmlEditor' )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
						reqNAME.style.display = Sql.IsEmptyString(txt.value) ? 'inline' : 'none';
						if ( Sql.IsEmptyString(txt.value) )
							nInvalidFields++;
					}
				}
				else if ( sFIELD_TYPE == 'DatePicker' )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						try
						{
							var dt = $('#' + txt.id).datepicker('getDate');
							if ( isNaN(dt) )
								txt.value = '';
						}
						catch(e)
						{
							txt.value = '';
						}
						var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
						reqNAME.style.display = Sql.IsEmptyString(txt.value) ? 'inline' : 'none';
						if ( Sql.IsEmptyString(txt.value) )
							nInvalidFields++;
					}
				}
				else if ( sFIELD_TYPE == 'DateTimeEdit' || sFIELD_TYPE == 'DateTimeNewRecord' || sFIELD_TYPE == 'DateTimePicker' )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						try
						{
							var dt = $('#' + txt.id).datetimepicker('getDate');
							if ( isNaN(dt) )
								txt.value = '';
						}
						catch(e)
						{
							txt.value = '';
						}
						var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
						reqNAME.style.display = Sql.IsEmptyString(txt.value) ? 'inline' : 'none';
						if ( Sql.IsEmptyString(txt.value) )
							nInvalidFields++;
					}
				}
				else if ( sFIELD_TYPE == 'ListBox' )
				{
					if ( sLIST_NAME != null )
					{
						var lst = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
						if ( lst != null )
						{
							// 09/02/2011 Paul.  Always use an array so that we can distinguish between list entry and text entry. 
							var arr = new Array();
							if ( lst.multiple )
							{
								for ( var j = 0; j < lst.options.length; j++ )
								{
									if ( lst.options[j].selected )
									{
										arr.push(lst.options[j].value);
									}
								}
								var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
								reqNAME.style.display = (arr.length == 0) ? 'inline' : 'none';
								if ( arr.length == 0 )
									nInvalidFields++;
							}
						}
					}
				}
				else if ( sFIELD_TYPE == 'TeamSelect' )
				{
					var sTEAM_SET_LIST = '';
					var arrTeams = document.getElementsByName(sLayoutPanel + '_ctlEditView_TEAM_SET_LIST');
					for ( var i = 0; i < arrTeams.length; i++ )
					{
						if( sTEAM_SET_LIST.length > 0 )
							sTEAM_SET_LIST += ',';
						sTEAM_SET_LIST += arrTeams[i].value;
					}
					var reqNAME = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED');
					reqNAME.style.display = Sql.IsEmptyString(sTEAM_SET_LIST) ? 'inline' : 'none';
					if ( Sql.IsEmptyString(sTEAM_SET_LIST) )
						nInvalidFields++;
				}
			}
		}
		catch(e)
		{
			SplendidError.SystemAlert(e, 'EditViewUI.GetValues ' + sFIELD_TYPE + ' ' + sDATA_FIELD);
		}
	}
	return nInvalidFields == 0;
}

EditViewUI.prototype.GetValues = function(sLayoutPanel, sEDIT_NAME, bSearch, row)
{
	//alert('sLayoutPanel ' + sLayoutPanel);
	var bEnableTeamManagement  = Crm.Config.enable_team_management();
	var bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
	var bgPage  = chrome.extension.getBackgroundPage();
	var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
	for ( var nLayoutIndex in layout )
	{
		var lay = layout[nLayoutIndex];
		var sFIELD_TYPE                 = lay.FIELD_TYPE                ;
		var sDATA_LABEL                 = lay.DATA_LABEL                ;
		var sDATA_FIELD                 = lay.DATA_FIELD                ;
		var sDISPLAY_FIELD              = lay.DISPLAY_FIELD             ;
		var sLIST_NAME                  = lay.LIST_NAME                 ;
		var sONCLICK_SCRIPT             = lay.ONCLICK_SCRIPT            ;
		var nFORMAT_ROWS                = Sql.ToInteger(lay.FORMAT_ROWS);
		var sMODULE_TYPE                = lay.MODULE_TYPE               ;
		
		try
		{
			if ( (sDATA_FIELD == 'TEAM_ID' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( !bEnableTeamManagement )
				{
					sFIELD_TYPE = 'Blank';
				}
				else
				{
					if ( bEnableDynamicTeams )
					{
						// 08/31/2009 Paul.  Don't convert to TeamSelect inside a Search view or Popup view. 
						if ( sEDIT_NAME.indexOf('.Search') < 0 && sEDIT_NAME.indexOf('.Popup') < 0 )
						{
							sDATA_LABEL     = '.LBL_TEAM_SET_NAME';
							sDATA_FIELD     = 'TEAM_SET_NAME';
							sFIELD_TYPE     = 'TeamSelect';
							sONCLICK_SCRIPT = '';
						}
					}
					else
					{
						// 04/18/2010 Paul.  If the user manually adds a TeamSelect, we need to convert to a ModulePopup. 
						if ( sFIELD_TYPE == 'TeamSelect' )
						{
							sDATA_LABEL     = 'Teams.LBL_TEAM';
							sDATA_FIELD     = 'TEAM_ID';
							sDISPLAY_FIELD  = 'TEAM_NAME';
							sFIELD_TYPE     = 'ModulePopup';
							sMODULE_TYPE    = 'Teams';
							sONCLICK_SCRIPT = '';
						}
					}
				}
			}
			if ( sFIELD_TYPE == 'Hidden' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null && txt.value != null )
				{
					//alert(sDATA_FIELD + ' = ' + txt.value);
					row[sDATA_FIELD] = txt.value;
				}
			}
			else if ( sFIELD_TYPE == 'ModuleAutoComplete' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null && txt.value != null )
				{
					//alert(sDATA_FIELD + ' = ' + txt.value);
					row[sDATA_FIELD] = txt.value;
				}
			}
			else if ( sFIELD_TYPE == 'ModulePopup' || sFIELD_TYPE == 'ChangeButton' )
			{
				var hid = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( hid != null && hid.value != null )
				{
					//alert(sDISPLAY_FIELD + ' = ' + txt.value);
					// 10/18/2011 Paul.  If this is a search operation, then we want the exact value. 
					if ( bSearch )
						row[sDATA_FIELD] = '=' + hid.value;
					else
						row[sDATA_FIELD] = hid.value;
				}
				// 12/23/2012 Paul.  If the label is PARENT_TYPE, then change the label to a DropDownList.
				if ( sDATA_LABEL == 'PARENT_TYPE' && sFIELD_TYPE == 'ChangeButton' )
				{
					var lst = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_PARENT_TYPE');
					if ( lst != null )
					{
						row['PARENT_TYPE'] = lst.options[lst.options.selectedIndex].value;
					}
				}
			}
			else if ( sFIELD_TYPE == 'TextBox' || sFIELD_TYPE == 'HtmlEditor' )
			{
				if ( nFORMAT_ROWS == 0 )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null && txt.value != null )
					{
						//alert(sDATA_FIELD + ' = ' + txt.value);
						row[sDATA_FIELD] = txt.value;
					}
				}
				else
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					// 10/14/2011 Paul.  We still access the value from a textarea. 
					if ( txt != null && txt.value != null )
					{
						//alert(sDATA_FIELD + ' = ' + txt.value);
						row[sDATA_FIELD] = txt.value;
					}
				}
			}
			else if ( sFIELD_TYPE == 'DatePicker' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null && txt.value != null )
				{
					//alert(sDATA_FIELD + ' = ' + txt.value);
					try
					{
						
						row[sDATA_FIELD] = ToJsonDate($('#' + txt.id).datepicker('getDate'));
					}
					catch(e)
					{
						row[sDATA_FIELD] = null;
					}
				}
			}
			else if ( sFIELD_TYPE == 'DateTimeEdit' || sFIELD_TYPE == 'DateTimeNewRecord' || sFIELD_TYPE == 'DateTimePicker' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null && txt.value != null )
				{
					//alert(sDATA_FIELD + ' = ' + txt.value);
					try
					{
						var dtVALUE = $('#' + txt.id).datetimepicker('getDate');
						row[sDATA_FIELD] = ToJsonDate(dtVALUE);
						//alert(sDATA_FIELD + ' ' + txt.value + ' ' + dtVALUE.toString() + ' ' + FromJsonDate(row[sDATA_FIELD], Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT()))
					}
					catch(e)
					{
						row[sDATA_FIELD] = null;
					}
				}
			}
			else if ( sFIELD_TYPE == 'ListBox' )
			{
				if ( sLIST_NAME != null )
				{
					var lst = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( lst != null )
					{
						// 09/02/2011 Paul.  Always use an array so that we can distinguish between list entry and text entry. 
						var arr = new Array();
						if ( lst.multiple )
						{
							for ( var j = 0; j < lst.options.length; j++ )
							{
								if ( lst.options[j].selected )
								{
									arr.push(lst.options[j].value);
								}
							}
							if ( arr.length > 0 )
							{
								//alert(sDATA_FIELD + ' = ' + dumpObj(arr, ''));
								row[sDATA_FIELD] = arr;
							}
						}
						else
						{
							if ( lst.options.selectedIndex >= 0 )
							{
								//alert(sDATA_FIELD + ' = ' + lst.options[lst.options.selectedIndex].value);
								// 09/09/2011 Paul.  We need another way to determine if we should use a wildcard search or an exact string. 
								// 09/09/2011 Paul.  Using an array is not good as it causes problems when during a ModuleUpdate operation. 
								if ( bSearch )
								{
									arr.push(lst.options[lst.options.selectedIndex].value);
									row[sDATA_FIELD] = arr;
								}
								else
								{
									row[sDATA_FIELD] = lst.options[lst.options.selectedIndex].value;
								}
							}
						}
					}
				}
			}
			// 08/01/2013 Paul.  Add support for CheckBoxList. 
			else if ( sFIELD_TYPE == 'CheckBoxList' )
			{
				if ( sLIST_NAME != null )
				{
					var arrLIST = L10n.GetList(sLIST_NAME);
					if ( arrLIST != null )
					{
						var arr = new Array();
						row[sDATA_FIELD] = null;
						for ( var i = 0; i < arrLIST.length; i++ )
						{
							var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i]);
							if ( chk != null )
							{
								if ( chk.checked )
									arr.push(chk.value);
							}
						}
						if ( arr.length > 0 )
							row[sDATA_FIELD] = arr;
					}
				}
			}
			// 08/01/2013 Paul.  Add support for Radio. 
			else if ( sFIELD_TYPE == 'Radio' )
			{
				if ( sLIST_NAME != null )
				{
					var arrLIST = L10n.GetList(sLIST_NAME);
					if ( arrLIST != null )
					{
						row[sDATA_FIELD] = null;
						for ( var i = 0; i < arrLIST.length; i++ )
						{
							var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i]);
							if ( chk != null )
							{
								if ( chk.checked )
								{
									row[sDATA_FIELD] = chk.value;
									break;
								}
							}
						}
					}
				}
			}
			else if ( sFIELD_TYPE == 'CheckBox' )
			{
				var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				// 01/19/2013 Paul.  Make sure to save both the checked and the unchecked state. 
				if ( chk != null )
					row[sDATA_FIELD] = chk.checked;
			}
			else if ( sFIELD_TYPE == 'TeamSelect' )
			{
				var sTEAM_ID       = '';
				var sTEAM_SET_LIST = '';
				var arrTeams = document.getElementsByName(sLayoutPanel + '_ctlEditView_TEAM_SET_LIST');
				for ( var i = 0; i < arrTeams.length; i++ )
				{
					if( sTEAM_SET_LIST.length > 0 )
						sTEAM_SET_LIST += ',';
					if( sTEAM_ID.length == 0 )
						sTEAM_ID = arrTeams[i].value;
					sTEAM_SET_LIST += arrTeams[i].value;
				}
				var arrPrimary = document.getElementsByName(sLayoutPanel + '_ctlEditView_TEAM_PRIMARY');
				for ( var i = 0; i < arrPrimary.length; i++ )
				{
					if ( arrPrimary[i].checked )
					{
						sTEAM_ID = arrPrimary[i].value;
					}
				}
				row['TEAM_ID'      ] = sTEAM_ID      ;
				row['TEAM_SET_LIST'] = sTEAM_SET_LIST;
			}
		}
		catch(e)
		{
			SplendidError.SystemAlert(e, 'EditViewUI.GetValues ' + sFIELD_TYPE + ' ' + sDATA_FIELD);
		}
	}
	return row;
}

EditViewUI.prototype.ClearValues = function(sLayoutPanel, sEDIT_NAME)
{
	//alert('sEDIT_NAME ' + sEDIT_NAME);
	var row = new Array();
	var bEnableTeamManagement  = Crm.Config.enable_team_management();
	var bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
	var bgPage  = chrome.extension.getBackgroundPage();
	var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
	for ( var nLayoutIndex in layout )
	{
		var lay = layout[nLayoutIndex];
		var sFIELD_TYPE                 = lay.FIELD_TYPE                ;
		var sDATA_FIELD                 = lay.DATA_FIELD                ;
		var sDISPLAY_FIELD              = lay.DISPLAY_FIELD             ;
		var sLIST_NAME                  = lay.LIST_NAME                 ;
		var nFORMAT_ROWS                = Sql.ToInteger(lay.FORMAT_ROWS);
		try
		{
			if ( sFIELD_TYPE == 'Hidden' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null )
				{
					txt.value = '';
				}
			}
			else if ( sFIELD_TYPE == 'ModuleAutoComplete' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null )
				{
					txt.value = '';
				}
			}
			else if ( sFIELD_TYPE == 'ModulePopup' || sFIELD_TYPE == 'ChangeButton' )
			{
				var sTEMP_DISPLAY_FIELD = Sql.IsEmptyString(sDISPLAY_FIELD) ? sDATA_FIELD + '_NAME' : sDISPLAY_FIELD;
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sTEMP_DISPLAY_FIELD);
				if ( txt != null )
				{
					txt.value = '';
				}
				var hid = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( hid != null )
				{
					hid.value = '';
				}
			}
			else if ( sFIELD_TYPE == 'TextBox' || sFIELD_TYPE == 'HtmlEditor' )
			{
				if ( nFORMAT_ROWS == 0 )
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null )
					{
						txt.value = '';
					}
				}
				else
				{
					var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( txt != null )
					{
						txt.innerHTML = '';
					}
				}
			}
			else if ( sFIELD_TYPE == 'DatePicker' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null )
				{
					txt.value = '';
				}
			}
			else if ( sFIELD_TYPE == 'DateTimeEdit' || sFIELD_TYPE == 'DateTimeNewRecord' || sFIELD_TYPE == 'DateTimePicker' )
			{
				var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( txt != null )
				{
					txt.value = '';
				}
			}
			else if ( sFIELD_TYPE == 'ListBox' )
			{
				if ( sLIST_NAME != null )
				{
					var lst = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
					if ( lst != null )
					{
						if ( lst.multiple )
						{
							for ( var j = 0; j < lst.options.length; j++ )
							{
								lst.options[j].selected = false;
							}
						}
						else
						{
							lst.options.selectedIndex = 0;
						}
					}
				}
			}
			// 08/01/2013 Paul.  Add support for CheckBoxList. 
			else if ( sFIELD_TYPE == 'CheckBoxList' )
			{
				if ( sLIST_NAME != null )
				{
					var arrLIST = L10n.GetList(sLIST_NAME);
					if ( arrLIST != null )
					{
						for ( var i = 0; i < arrLIST.length; i++ )
						{
							var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i]);
							if ( chk != null )
								chk.checked = false;
						}
					}
				}
			}
			// 08/01/2013 Paul.  Add support for Radio. 
			else if ( sFIELD_TYPE == 'Radio' )
			{
				if ( sLIST_NAME != null )
				{
					var arrLIST = L10n.GetList(sLIST_NAME);
					if ( arrLIST != null )
					{
						for ( var i = 0; i < arrLIST.length; i++ )
						{
							var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i]);
							if ( chk != null )
								chk.checked = false;
						}
					}
				}
			}
			else if ( sFIELD_TYPE == 'CheckBox' )
			{
				var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD);
				if ( chk != null )
					chk.checked = false;
			}
		}
		catch(e)
		{
			SplendidError.SystemAlert(e, 'EditViewUI.ClearValues' + sFIELD_TYPE + ' ' + sDATA_FIELD);
		}
	}
	return row;
}

EditViewUI.prototype.AutoComplete = function(sLayoutPanel, sMODULE_TYPE, sTEXT_FIELD, sVALUE_FIELD)
{
	// http://jqueryui.com/demos/autocomplete/remote-jsonp.html
	$('#' + sLayoutPanel + '_ctlEditView_' + sTEXT_FIELD).autocomplete(
	{
		  minLength: 2
		, source: function(request, response)
		{
			try
			{
				var sTABLE_NAME = Crm.Modules.TableName(sMODULE_TYPE);
				var sMETHOD = sTABLE_NAME + '_' + sTEXT_FIELD + '_List';
				var bgPage = chrome.extension.getBackgroundPage();
				bgPage.AutoComplete_ModuleMethod(sMODULE_TYPE, sMETHOD, '{"prefixText": ' + JSON.stringify(request.term) + ', "count": 12}', function(status, message)
				{
					if ( status == 1 )
					{
						response($.map(message, function(item)
						{
							return { label: item, value: item };
						}));
					}
				}, this);
			}
			catch(e)
			{
				SplendidError.SystemAlert(e, 'EditViewUI.AutoComplete' + sMODULE_TYPE + ' ' + sTEXT_FIELD);
			}
		}
		, select: function(event, ui)
		{
			try
			{
				//alert(dumpObj(ui.item, 'ui.item'));
				if ( sVALUE_FIELD != null && ui.item && !Sql.IsEmptyString(ui.item.value) )
				{
					var sTABLE_NAME = Crm.Modules.TableName(sMODULE_TYPE);
					var sMETHOD = sTABLE_NAME + '_' + sTEXT_FIELD + '_Get';
					var bgPage = chrome.extension.getBackgroundPage();
					bgPage.AutoComplete_ModuleMethod(sMODULE_TYPE, sMETHOD, '{"sNAME": ' + JSON.stringify(ui.item.value) + '}', function(status, message)
					{
						//alert(dumpObj(message, 'AutoComplete response'));
						var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sTEXT_FIELD );
						var hid = document.getElementById(sLayoutPanel + '_ctlEditView_' + sVALUE_FIELD);
						if ( txt != null ) txt.value = (status == 1) ? message.NAME : '';
						if ( hid != null ) hid.value = (status == 1) ? message.ID   : '';
					}, this);
				}
			}
			catch(e)
			{
				SplendidError.SystemAlert(e, 'EditViewUI.AutoComplete' + sMODULE_TYPE + ' ' + sTEXT_FIELD);
			}
		}
		, open: function()
		{
			$(this).removeClass('ui-corner-all').addClass('ui-corner-top');
		}
		, close: function()
		{
			$(this).removeClass('ui-corner-top').addClass('ui-corner-all');
		}
	});
}

EditViewUI.prototype.AutoCompleteBlur = function(sLayoutPanel, sMODULE_TYPE, sTEXT_FIELD, sVALUE_FIELD, sSubmitID)
{
	try
	{
		var txt = document.getElementById(sLayoutPanel + '_ctlEditView_' + sTEXT_FIELD );
		var hid = document.getElementById(sLayoutPanel + '_ctlEditView_' + sVALUE_FIELD);
		
		if ( !Sql.IsEmptyString(txt.value) )
		{
			var sTABLE_NAME = Crm.Modules.TableName(sMODULE_TYPE);
			var sMETHOD = sTABLE_NAME + '_' + sTEXT_FIELD + '_Get';
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.AutoComplete_ModuleMethod(sMODULE_TYPE, sMETHOD, '{"sNAME": ' + JSON.stringify(txt.value) + '}', function(status, message)
			{
				//alert(dumpObj(message, 'AutoComplete response status = ' + status));
				if ( txt != null ) txt.value = (status == 0 || status == 1) ? message.NAME : '';
				if ( hid != null ) hid.value = (status == 0 || status == 1) ? message.ID   : '';
				if ( sSubmitID !== undefined && sSubmitID != null )
				{
					var btnSubmit = document.getElementById(sLayoutPanel + '_ctlEditView_' + sSubmitID);
					if ( btnSubmit != null )
						btnSubmit.onclick();
				}
			}, this);
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'EditViewUI.AutoCompleteBlur' + sMODULE_TYPE + ' ' + sTEXT_FIELD);
	}
}

EditViewUI.prototype.LoadTeamSelect = function(sLayoutPanel, tdField, sTEAM_SET_ID, sTEAM_SET_LIST, bUI_REQUIRED, bAllowDefaults)
{
	try
	{
		var ctlEditView_TEAM_SET_NAME = document.createElement('table');
		var tbody = document.createElement('tbody');
		var tr    = document.createElement('tr');
		ctlEditView_TEAM_SET_NAME.id          = sLayoutPanel + '_ctlEditView_TEAM_SET_NAME';
		ctlEditView_TEAM_SET_NAME.className   = 'listView';
		ctlEditView_TEAM_SET_NAME.cellSpacing = 0;
		ctlEditView_TEAM_SET_NAME.cellPadding = 2;
		ctlEditView_TEAM_SET_NAME.border      = 1;
		ctlEditView_TEAM_SET_NAME.style.borderCollapse = 'collapse';
		ctlEditView_TEAM_SET_NAME.appendChild(tbody);
		tr.className = 'listViewThS1';
		tbody.appendChild(tr);
		tdField.appendChild(ctlEditView_TEAM_SET_NAME);

		var tdTeam = document.createElement('th');
		tdTeam.innerHTML = '&nbsp;';
		tr.appendChild(tdTeam);
		var tdPrimary = document.createElement('th');
		tr.appendChild(tdPrimary);
		tdPrimary.innerHTML = L10n.Term('Teams.LBL_LIST_PRIMARY_TEAM');
		var tdEdit = document.createElement('th');
		tdEdit.align     = 'right';
		tdEdit.innerHTML = '&nbsp;';
		tr.appendChild(tdEdit);
		
		if ( bAllowDefaults && !Sql.IsEmptyString(Security.TEAM_ID()) && !Sql.IsEmptyString(Security.TEAM_NAME()) )
		{
			tr = document.createElement('tr');
			tbody.appendChild(tr);
			
			tdTeam = document.createElement('td');
			tr.appendChild(tdTeam);
			
			var txt = document.createTextNode(Security.TEAM_NAME());
			tdTeam.appendChild(txt);
			
			var hid = document.createElement('input');
			hid.id        = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
			hid.name      = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
			hid.type      = 'hidden';
			hid.value     = Security.TEAM_ID();
			tdTeam.appendChild(hid);
			
			tdPrimary = document.createElement('td');
			tr.appendChild(tdPrimary);
			var chk = document.createElement('input');
			chk.id        = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
			chk.name      = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
			chk.type      = 'checkbox';
			chk.className = 'checkbox';
			chk.disabled  = 'disabled';
			tdPrimary.appendChild(chk);
			// 10/26/2011 Paul.  The checked flag must be set after adding. 
			chk.checked   = true;
			chk.value     = Security.TEAM_ID();
			
			tdEdit = document.createElement('td');
			tdEdit.align = 'right';
			tdEdit.style.whiteSpace = 'nowrap';
			tr.appendChild(tdEdit);
			var imgDelete = document.createElement('input');
			imgDelete.type   = 'image';
			imgDelete.src    = sIMAGE_SERVER + 'App_Themes/Six/images/delete_inline.gif';
			imgDelete.style.borderWidth = '0px';
			imgDelete.onclick = BindArguments(function(tr)
			{
				tr.parentNode.removeChild(tr);
			}, tr);
			tdEdit.appendChild(imgDelete);
		}
		if ( !Sql.IsEmptyString(sTEAM_SET_LIST) )
		{
			var arrTEAM_SET_LIST = sTEAM_SET_LIST.split(',');
			for ( var i = 0; i < arrTEAM_SET_LIST.length; i++ )
			{
				var sTEAM_ID = arrTEAM_SET_LIST[i];
				tr = document.createElement('tr');
				tbody.appendChild(tr);
				
				tdTeam = document.createElement('td');
				tr.appendChild(tdTeam);
				
				var txt = document.createTextNode(Crm.Teams.Name(sTEAM_ID));
				var hid = document.createElement('input');
				hid.id        = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
				hid.name      = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
				hid.type      = 'hidden';
				hid.value     = sTEAM_ID;
				tdTeam.appendChild(hid);
				tdTeam.appendChild(txt);
				
				tdPrimary = document.createElement('td');
				tr.appendChild(tdPrimary);
				var chk = document.createElement('input');
				chk.id        = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
				chk.name      = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
				chk.type      = 'checkbox';
				chk.className = 'checkbox';
				chk.disabled  = 'disabled';
				tdPrimary.appendChild(chk);
				// 10/26/2011 Paul.  The checked flag must be set after adding. 
				chk.checked   = (i == 0);
				chk.value     = sTEAM_ID;
				
				tdEdit = document.createElement('td');
				tdEdit.align = 'right';
				tdEdit.style.whiteSpace = 'nowrap';
				tr.appendChild(tdEdit);
				var imgDelete = document.createElement('input');
				imgDelete.type   = 'image';
				imgDelete.src    = sIMAGE_SERVER + 'App_Themes/Six/images/delete_inline.gif';
				imgDelete.style.borderWidth = '0px';
				imgDelete.onclick = BindArguments(function(tr)
				{
					tr.parentNode.removeChild(tr);
				}, tr);
				tdEdit.appendChild(imgDelete);
			}
		}
		
		// 10/26/2011 Paul.  Adding teams will always be at the bottom.  We will not allow in-place editing. 
		tr = document.createElement('tr');
		tbody.appendChild(tr);
		
		tdTeam = document.createElement('td');
		tr.appendChild(tdTeam);
		
		var txt = document.createElement('input');
		txt.id        = sLayoutPanel + '_ctlEditView_TEAM_NAME';
		txt.type      = 'text';
		txt.onkeypress = function(e)
		{
			return RegisterEnterKeyPress(e, sLayoutPanel + '_ctlEditView_TEAM_NAME_btnInsert');
		};
		tdTeam.appendChild(txt);
		
		var hid = document.createElement('input');
		hid.id        = sLayoutPanel + '_ctlEditView_TEAM_ID';
		hid.type      = 'hidden';
		tdTeam.appendChild(hid);
		
		var btnChange = document.createElement('input');
		btnChange.id        = sLayoutPanel + '_ctlEditView_TEAM_NAME_btnChange';
		btnChange.type      = 'button';
		btnChange.className = 'button';
		btnChange.title     = L10n.Term('.LBL_SELECT_BUTTON_TITLE');
		btnChange.value     = L10n.Term('.LBL_SELECT_BUTTON_LABEL');
		btnChange.style.marginLeft  = '4px';
		btnChange.style.marginRight = '2px';
		btnChange.onclick = BindArguments(function(txt, hid, sMODULE_TYPE)
		{
			var $dialog = $('<div id="' + hid.id + '_divPopup"><div id="divPopupActionsPanel" /><div id="divPopupLayoutPanel" /></div>');
			$dialog.dialog(
			{
				  modal    : true
				, resizable: true
				, width    : $(window).width() > 0 ? ($(window).width() - 60) : 800
				, height   : (navigator.userAgent.indexOf('iPad') > 0 ? 'auto' : ($(window).height() > 0 ? $(window).height() - 60 : 800))
				, title    : L10n.Term('Teams.LBL_LIST_FORM_TITLE')
				, create   : function(event, ui)
				{
					try
					{
						var oPopupViewUI = new PopupViewUI();
						oPopupViewUI.Load('divPopupLayoutPanel', 'divPopupActionsPanel', 'Teams', false, function(status, message)
						{
							if ( status == 1 )
							{
								hid.value = message.ID  ;
								txt.value = message.NAME;
								// 02/21/2013 Paul.  Use close instead of destroy. 
								$dialog.dialog('close');
								
								var btnSubmit = document.getElementById(sLayoutPanel + '_ctlEditView_TEAM_NAME_btnInsert');
								btnSubmit.click();
							}
							else if ( status == -2 )
							{
								// 02/21/2013 Paul.  Use close instead of destroy. 
								$dialog.dialog('close');
							}
							else if ( status == -1 )
							{
								SplendidError.SystemMessage(message);
							}
						});
					}
					catch(e)
					{
						SplendidError.SystemError(e, 'PopupViewUI dialog');
					}
				}
				, close: function(event, ui)
				{
					$dialog.dialog('destroy');
					// 10/17/2011 Paul.  We have to remove the new HTML, otherwise there will be multiple definitions for divPopupLayoutPanel. 
					var divPopup = document.getElementById(hid.id + '_divPopup');
					divPopup.parentNode.removeChild(divPopup);
				}
			});
		}, txt, hid, 'Teams');
		tdTeam.appendChild(btnChange);
		this.AutoComplete(sLayoutPanel, 'Teams', 'TEAM_NAME', 'TEAM_ID');
		txt.onblur = BindArguments(this.AutoCompleteBlur, sLayoutPanel, 'Teams', 'TEAM_NAME', 'TEAM_ID', 'TEAM_NAME_btnInsert');

		// 12/15/2014 Paul.  Use small button on mobile device. 
		var bIsMobile = isMobileDevice();
		if ( isMobileLandscape() )
			bIsMobile = false;
		if ( bIsMobile )
		{
			btnChange.style.display = 'none';
			var aChange = document.createElement('a');
			tdTeam.appendChild(aChange);
			var iChange = document.createElement('i');
			iChange.className = 'fa fa-2x fa-location-arrow navButton';
			aChange.style.verticalAlign = 'bottom';
			aChange.appendChild(iChange);
			aChange.onclick = function()
			{
				btnChange.click();
			};
		}
		
		tdPrimary = document.createElement('td');
		tr.appendChild(tdPrimary);
		var chk = document.createElement('input');
		chk.type      = 'checkbox';
		chk.className = 'checkbox';
		tdPrimary.appendChild(chk);
		chk.checked   = bAllowDefaults;
		
		tdEdit = document.createElement('td');
		tdEdit.align = 'right';
		tdEdit.style.whiteSpace = 'nowrap';
		tr.appendChild(tdEdit);
		var imgInsert = document.createElement('input');
		imgInsert.id     = sLayoutPanel + '_ctlEditView_TEAM_NAME_btnInsert';
		imgInsert.type   = 'image';
		imgInsert.src    = sIMAGE_SERVER + 'App_Themes/Six/images/accept_inline.gif';
		imgInsert.style.borderWidth = '0px';
		imgInsert.onclick = BindArguments(function(tr, txt, hid, chk)
		{
			var sTEAM_ID   = hid.value;
			var sTEAM_NAME = txt.value;
			var bPRIMARY   = chk.checked;
			if ( !Sql.IsEmptyString(sTEAM_ID) )
			{
				var trNew = document.createElement('tr');
				tbody.insertBefore(trNew, tr);
				var tdNew = document.createElement('td');
				trNew.appendChild(tdNew);
				
				var txtNew = document.createTextNode(sTEAM_NAME);
				tdNew.appendChild(txtNew);
				var hidNew = document.createElement('input');
				hidNew.id    = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
				hidNew.name  = sLayoutPanel + '_ctlEditView_TEAM_SET_LIST';
				hidNew.type  = 'hidden';
				hidNew.value = sTEAM_ID;
				tdNew.appendChild(hidNew);
				
				tdNew = document.createElement('td');
				trNew.appendChild(tdNew);
				var chkNew = document.createElement('input');
				chkNew.id        = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
				chkNew.name      = sLayoutPanel + '_ctlEditView_TEAM_PRIMARY';
				chkNew.type      = 'checkbox';
				chkNew.className = 'checkbox';
				chkNew.disabled  = 'disabled';
				tdNew.appendChild(chkNew);
				// 10/26/2011 Paul.  The checked flag must be set after adding. 
				chkNew.checked   = bPRIMARY;
				chkNew.value     = sTEAM_ID;
				
				tdNew = document.createElement('td');
				trNew.appendChild(tdNew);
				tdNew.align = 'right';
				tdNew.style.whiteSpace = 'nowrap';
				var imgDelete = document.createElement('input');
				imgDelete.type   = 'image';
				imgDelete.src    = sIMAGE_SERVER + 'App_Themes/Six/images/delete_inline.gif';
				imgDelete.style.borderWidth = '0px';
				imgDelete.onclick = BindArguments(function(trNew)
				{
					trNew.parentNode.removeChild(trNew);
				}, trNew);
				tdNew.appendChild(imgDelete);
				
				if ( bPRIMARY )
				{
					// 10/26/2011 Paul.  If setting this as primary, then clear all previous. 
					var arrPrimary = document.getElementsByName(sLayoutPanel + '_ctlEditView_TEAM_PRIMARY');
					for ( var i = 0; i < arrPrimary.length; i++ )
					{
						arrPrimary[i].checked = false;
					}
					chkNew.checked = bPRIMARY;
				}
			}
			hid.value = '';
			txt.value = '';
			chk.checked = false;
		}, tr, txt, hid, chk);
		tdEdit.appendChild(imgInsert);
		var imgCancel = document.createElement('input');
		imgCancel.id     = sLayoutPanel + '_ctlEditView_TEAM_NAME_btnCancel';
		imgCancel.type   = 'image';
		imgCancel.src    = sIMAGE_SERVER + 'App_Themes/Six/images/decline_inline.gif';
		imgCancel.style.borderWidth = '0px';
		imgCancel.onclick = BindArguments(function(txt, hid)
		{
			hid.value = '';
			txt.value = '';
		}, txt, hid);
		tdEdit.appendChild(imgCancel);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'EditViewUI.LoadTeamSelect');
	}
}

EditViewUI.prototype.LoadView = function(sLayoutPanel, tblMain, layout, row, sSubmitID)
{
	try
	{
		// 10/17/2012 Paul.  Exit if the Main does not exist.  This is a sign that the user has navigated elsewhere. 
		if ( tblMain == null )
			return;
		var tbody = document.createElement('tbody');
		tblMain.appendChild(tbody);
		
		var tr        = null;
		var nColIndex = 0;
		var tdLabel   = null;
		var tdField   = null;
		var bEnableTeamManagement  = Crm.Config.enable_team_management();
		var bRequireTeamManagement = Crm.Config.require_team_management();
		var bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
		var bRequireUserAssignment = Crm.Config.require_user_assignment();
		// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
		if ( row != null && row['DATE_MODIFIED'] !== undefined )
			this.LAST_DATE_MODIFIED = row['DATE_MODIFIED'];

		// 08/31/2012 Paul.  Add support for speech. 
		var bEnableSpeech = Crm.Config.enable_speech();
		var sUSER_AGENT   = navigator.userAgent;
		if ( sUSER_AGENT == 'Chrome' || sUSER_AGENT.indexOf('Android') > 0 || sUSER_AGENT.indexOf('iPad') > 0 || sUSER_AGENT.indexOf('iPhone') > 0 )
			bEnableSpeech = Crm.Config.enable_speech();
		// 12/06/2014 Paul.  Use new mobile flag. 
		var bIsMobile = isMobileDevice();
		if ( isMobileLandscape() )
			bIsMobile = false;
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			//alert(dumpObj(lay, 'EditViewUI.LoadView layout'));
			var sEDIT_NAME                  = lay.EDIT_NAME                 ;
			var sFIELD_TYPE                 = lay.FIELD_TYPE                ;
			var sDATA_LABEL                 = lay.DATA_LABEL                ;
			var sDATA_FIELD                 = lay.DATA_FIELD                ;
			var sDATA_FORMAT                = lay.DATA_FORMAT               ;
			var sDISPLAY_FIELD              = lay.DISPLAY_FIELD             ;
			var sCACHE_NAME                 = lay.CACHE_NAME                ;
			var sLIST_NAME                  = lay.LIST_NAME                 ;
			// 12/05/2012 Paul.  UI_REQUIRED is not used on SQLite, so use the DATA_REQUIRED value. 
			var bUI_REQUIRED                = Sql.ToBoolean(lay.UI_REQUIRED) || Sql.ToBoolean(lay.DATA_REQUIRED);
			var sONCLICK_SCRIPT             = lay.ONCLICK_SCRIPT            ;
			var sFORMAT_SCRIPT              = lay.FORMAT_SCRIPT             ;
			var nFORMAT_TAB_INDEX           = Sql.ToInteger(lay.FORMAT_TAB_INDEX );
			var nFORMAT_MAX_LENGTH          = Sql.ToInteger(lay.FORMAT_MAX_LENGTH);
			var nFORMAT_SIZE                = Sql.ToInteger(lay.FORMAT_SIZE      );
			var nFORMAT_ROWS                = Sql.ToInteger(lay.FORMAT_ROWS      );
			var nFORMAT_COLUMNS             = Sql.ToInteger(lay.FORMAT_COLUMNS   );
			var nCOLSPAN                    = Sql.ToInteger(lay.COLSPAN          );
			var nROWSPAN                    = Sql.ToInteger(lay.ROWSPAN          );
			var sLABEL_WIDTH                = lay.LABEL_WIDTH               ;
			var sFIELD_WIDTH                = lay.FIELD_WIDTH               ;
			var nDATA_COLUMNS               = Sql.ToInteger(lay.DATA_COLUMNS     );
			var sVIEW_NAME                  = lay.VIEW_NAME                 ;
			var sFIELD_VALIDATOR_ID         = lay.FIELD_VALIDATOR_ID        ;
			var sFIELD_VALIDATOR_MESSAGE    = lay.FIELD_VALIDATOR_MESSAGE   ;
			var sUI_VALIDATOR               = lay.UI_VALIDATOR              ;
			var sVALIDATION_TYPE            = lay.VALIDATION_TYPE           ;
			var sREGULAR_EXPRESSION         = lay.REGULAR_EXPRESSION        ;
			var sDATA_TYPE                  = lay.DATA_TYPE                 ;
			var sMININUM_VALUE              = lay.MININUM_VALUE             ;
			var sMAXIMUM_VALUE              = lay.MAXIMUM_VALUE             ;
			var sCOMPARE_OPERATOR           = lay.COMPARE_OPERATOR          ;
			var sMODULE_TYPE                = lay.MODULE_TYPE               ;
			var sFIELD_VALIDATOR_NAME       = lay.FIELD_VALIDATOR_NAME      ;
			var sTOOL_TIP                   = lay.TOOL_TIP                  ;
			var bVALID_RELATED              = false                         ;
			var sRELATED_SOURCE_MODULE_NAME = lay.RELATED_SOURCE_MODULE_NAME;
			var sRELATED_SOURCE_VIEW_NAME   = lay.RELATED_SOURCE_VIEW_NAME  ;
			var sRELATED_SOURCE_ID_FIELD    = lay.RELATED_SOURCE_ID_FIELD   ;
			var sRELATED_SOURCE_NAME_FIELD  = lay.RELATED_SOURCE_NAME_FIELD ;
			var sRELATED_VIEW_NAME          = lay.RELATED_VIEW_NAME         ;
			var sRELATED_ID_FIELD           = lay.RELATED_ID_FIELD          ;
			var sRELATED_NAME_FIELD         = lay.RELATED_NAME_FIELD        ;
			var sRELATED_JOIN_FIELD         = lay.RELATED_JOIN_FIELD        ;
			var sPARENT_FIELD               = lay.PARENT_FIELD              ;
			var sFIELD_VALIDATOR_MESSAGE    = lay.FIELD_VALIDATOR_MESSAGE   ;
			var sVALIDATION_TYPE            = lay.VALIDATION_TYPE           ;
			var sREGULAR_EXPRESSION         = lay.REGULAR_EXPRESSION        ;
			var sDATA_TYPE                  = lay.DATA_TYPE                 ;
			var sMININUM_VALUE              = lay.MININUM_VALUE             ;
			var sMAXIMUM_VALUE              = lay.MAXIMUM_VALUE             ;
			var sCOMPARE_OPERATOR           = lay.COMPARE_OPERATOR          ;
			var sTOOL_TIP                   = ''                            ;
			var sMODULE_NAME                = ''                            ;
			
			if ( nDATA_COLUMNS == 0 )
				nDATA_COLUMNS = 2;
			
			var arrEDIT_NAME = sEDIT_NAME.split('.');
			if ( arrEDIT_NAME.length > 0 )
				sMODULE_NAME = arrEDIT_NAME[0];
			
			if ( (sDATA_FIELD == 'TEAM_ID' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( !bEnableTeamManagement )
				{
					sFIELD_TYPE  = 'Blank';
					// 10/24/2012 Paul.  Clear the label to prevent a term lookup. 
					sDATA_LABEL  = null;
					sDATA_FIELD  = null;
					bUI_REQUIRED = false;
				}
				else
				{
					if ( bEnableDynamicTeams )
					{
						// 08/31/2009 Paul.  Don't convert to TeamSelect inside a Search view or Popup view. 
						if ( sEDIT_NAME.indexOf('.Search') < 0 && sEDIT_NAME.indexOf('.Popup') < 0 )
						{
							sDATA_LABEL     = '.LBL_TEAM_SET_NAME';
							sDATA_FIELD     = 'TEAM_SET_NAME';
							sFIELD_TYPE     = 'TeamSelect';
							sONCLICK_SCRIPT = '';
						}
					}
					else
					{
						// 04/18/2010 Paul.  If the user manually adds a TeamSelect, we need to convert to a ModulePopup. 
						if ( sFIELD_TYPE == 'TeamSelect' )
						{
							sDATA_LABEL     = 'Teams.LBL_TEAM';
							sDATA_FIELD     = 'TEAM_ID';
							sDISPLAY_FIELD  = 'TEAM_NAME';
							sFIELD_TYPE     = 'ModulePopup';
							sMODULE_TYPE    = 'Teams';
							sONCLICK_SCRIPT = '';
						}
					}
					if ( bRequireTeamManagement )
						bUI_REQUIRED = true;
				}
			}
			// 02/03/2013 Paul.  FAVORITE_RECORD_ID is not supported on the HTML5 Offline Client or the Browser Extensions. 
			if ( sDATA_FIELD == 'FAVORITE_RECORD_ID' )
			{
				sFIELD_TYPE = 'Blank';
			}
			else if ( sDATA_FIELD == 'EXCHANGE_FOLDER' )
			{
				if ( !Crm.Modules.ExchangeFolders(sMODULE_NAME) )
				{
					sFIELD_TYPE = 'Blank';
				}
			}
			else if ( sDATA_FIELD == 'ASSIGNED_USER_ID' )
			{
				if ( bRequireUserAssignment )
					bUI_REQUIRED = true;
			}
			if ( sFIELD_TYPE == 'Blank' )
			{
				bUI_REQUIRED = false;
			}
			// 09/02/2012 Paul.  A separator will create a new table. We need to match the outer and inner layout. 
			if ( sFIELD_TYPE == 'Separator' )
			{
				var divMainLayoutPanel = document.getElementById(sLayoutPanel);
				var tblOuter     = document.createElement('table');
				var tbodyOuter   = document.createElement('tbody');
				var trOuter      = document.createElement('tr');
				var tdOuter      = document.createElement('td');
				// 09/27/2012 Paul.  Separator can have an ID and can have a style so that it can be hidden. 
				if ( !Sql.IsEmptyString(sDATA_FIELD) )
					tblOuter.id = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
				if ( !Sql.IsEmptyString(sDATA_FORMAT) )
					tblOuter.setAttribute('style', sDATA_FORMAT);
				tblOuter.className   = 'tabForm';
				tblOuter.cellSpacing = 1;
				tblOuter.cellPadding = 0;
				tblOuter.border      = 0;
				tblOuter.width       = '100%';
				tblOuter.style.marginTop = '5px';
				tblOuter.appendChild(tbodyOuter);
				tbodyOuter.appendChild(trOuter);
				trOuter.appendChild(tdOuter);
				divMainLayoutPanel.appendChild(tblOuter);
		
				tblMain = document.createElement('table');
				tblMain.width     = '100%';
				tblMain.className = 'tabEditView';
				tdOuter.appendChild(tblMain);
				tbody = document.createElement('tbody');
				tblMain.appendChild(tbody);
				
				nColIndex = 0;
				tr = null;
				continue;
			}
			// 09/03/2012 Paul.  We are going to ignore address buttons. 
			else if ( sFIELD_TYPE == 'AddressButtons' )
			{
				continue;
			}
			// 11/17/2007 Paul.  On a mobile device, each new field is on a new row. 
			// 12/02/2005 Paul.  COLSPAN == -1 means that a new column should not be created. 
			// 12/06/2014 Paul.  Use new mobile flag. 
			if ( (nCOLSPAN >= 0 && nColIndex == 0) || tr == null || bIsMobile )
			{
				// 11/25/2005 Paul.  Don't pre-create a row as we don't want a blank
				// row at the bottom.  Add rows just before they are needed. 
				tr = document.createElement('tr');
				tbody.appendChild(tr);
			}
			// 06/20/2009 Paul.  The label and the field will be on separate rows for a NewRecord form. 
			var trLabel = tr;
			var trField = tr;
			if ( nCOLSPAN >= 0 || tdLabel == null || tdField == null )
			{
				tdLabel = document.createElement('td');
				tdField = document.createElement('td');
				trLabel.appendChild(tdLabel);
				if ( sLABEL_WIDTH == '100%' && sFIELD_WIDTH == '0%' && nDATA_COLUMNS == 1 )
				{
					trField = document.createElement('tr');
					tbody.appendChild(tr);
				}
				else
				{
					// 06/20/2009 Paul.  Don't specify the normal styles for a NewRecord form. 
					// This is so that the label will be left aligned. 
					tdLabel.className = 'dataLabel';
					tdLabel.vAlign    = 'top';
					tdLabel.width     = sLABEL_WIDTH;
					tdField.className = 'dataField';
					tdField.vAlign    = 'top';
				}
				trField.appendChild(tdField);
				if ( nCOLSPAN > 0 )
				{
					tdField.colSpan = nCOLSPAN;
				}
				// 11/28/2005 Paul.  Don't use the field width if COLSPAN is specified as we want it to take the rest of the table.  The label width will be sufficient. 
				if ( nCOLSPAN == 0 && sFIELD_WIDTH != '0%' )
					tdField.Width  = sFIELD_WIDTH;
				
				if ( sDATA_LABEL != null )
				{
					if ( sDATA_LABEL.indexOf('.') >= 0 )
					{
						var txt = document.createTextNode(L10n.Term(sDATA_LABEL));
						tdLabel.appendChild(txt);
					}
					else if ( !Sql.IsEmptyString(sDATA_LABEL) && row != null )
					{
						var txt = document.createTextNode(Sql.ToString(row[sDATA_LABEL]) + ':');
						tdLabel.appendChild(txt);
					}
				}
				if ( bUI_REQUIRED )
				{
					var lblRequired = document.createElement('span');
					tdLabel.appendChild(lblRequired);
					lblRequired.className = 'required';
					lblRequired.innerHTML = L10n.Term('.LBL_REQUIRED_SYMBOL');
				}
			}
			//alert(sDATA_FIELD);
			try
			{
				if ( sFIELD_TYPE == 'Blank' )
				{
					tdLabel.innerHTML = '&nbsp;'
					tdField.innerHTML = '&nbsp;'
				}
				// 09/03/2012 Paul.  A header is similar to a label, but without the data field. 
				else if ( sFIELD_TYPE == 'Header' )
				{
					tdLabel.innerHTML = '<h4>' + tdLabel.innerHTML + '</h4>';
					tdField.innerHTML = '';
				}
				else if ( sFIELD_TYPE == 'Hidden' )
				{
					// 02/28/2008 Paul.  When the hidden field is the first in the row, we end up with a blank row. 
					// Just ignore for now as IE does not have a problem with the blank row. 
					nCOLSPAN = -1;
					var txt = document.createElement('input');
					txt.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					txt.type      = 'hidden';
					if ( row != null )
					{
						if ( !Sql.IsEmptyString(sDATA_FIELD) && row[sDATA_FIELD] != null )
							txt.value = row[sDATA_FIELD];
					}
					else if ( sDATA_FIELD == 'TEAM_ID' )
						txt.value = Security.TEAM_ID();
					else if ( sDATA_FIELD == 'ASSIGNED_USER_ID' )
						txt.value = Security.USER_ID();
					tdField.appendChild(txt);
				}
				else if ( sFIELD_TYPE == 'ModuleAutoComplete' )
				{
					var txt = document.createElement('input');
					txt.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					txt.type      = 'text';
					if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
					if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
					if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
					if ( row != null && row[sDATA_FIELD] != null )
						txt.value = row[sDATA_FIELD];
					tdField.appendChild(txt);
					// 08/31/2012 Paul.  Add support for speech. 
					if ( bEnableSpeech )
					{
						txt.setAttribute('speech', 'speech');
						txt.setAttribute('x-webkit-speech', 'x-webkit-speech');
					}
					if ( sSubmitID != null )
					{
						txt.onkeypress = function(e)
						{
							return RegisterEnterKeyPress(e, sSubmitID);
						};
					}
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
					this.AutoComplete(sLayoutPanel, sMODULE_TYPE, sDATA_FIELD, null);
				}
				else if ( sFIELD_TYPE == 'ModulePopup' || sFIELD_TYPE == 'ChangeButton' )
				{
					var lstField = null;
					// 12/01/2012 Paul.  If the label is PARENT_TYPE, then change the label to a DropDownList.
					if ( sDATA_LABEL == 'PARENT_TYPE' && sFIELD_TYPE == 'ChangeButton' )
					{
						while ( tdLabel.childNodes.length > 0 )
						{
							tdLabel.removeChild(tdLabel.firstChild);
						}
						lstField = document.createElement('select');
						tdLabel.appendChild(lstField);
						lstField.id = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_PARENT_TYPE';
						if ( nFORMAT_TAB_INDEX > 0 )
						{
							lstField.tabindex = nFORMAT_TAB_INDEX;
						}
						var sDATA_VALUE = null;
						if ( row != null && row['PARENT_TYPE'] != null )
							sDATA_VALUE = row['PARENT_TYPE'];
						var arrLIST = L10n.GetList('record_type_display');
						if ( arrLIST != null )
						{
							for ( var i = 0; i < arrLIST.length; i++ )
							{
								var opt = document.createElement('option');
								lstField.appendChild(opt);
								opt.setAttribute('value', arrLIST[i]);
								opt.innerHTML = L10n.ListTerm('record_type_display', arrLIST[i]);
								if ( sDATA_VALUE != null && sDATA_VALUE == arrLIST[i] )
									opt.setAttribute('selected', 'selected');
							}
						}
					}
					tdField.style.whiteSpace = 'nowrap';
					// 10/18/2011 Paul.  A custom field will not have a display name. 
					var sTEMP_DISPLAY_FIELD = Sql.IsEmptyString(sDISPLAY_FIELD) ? sDATA_FIELD + '_NAME' : sDISPLAY_FIELD;
					var txt = document.createElement('input');
					txt.id = sLayoutPanel + '_ctlEditView_' + sTEMP_DISPLAY_FIELD;
					txt.type      = 'text';
					//txt.readOnly  = true;
					if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
					if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
					if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
					if ( row != null )
					{
						if ( !Sql.IsEmptyString(sDISPLAY_FIELD) && row[sDISPLAY_FIELD] != null )
							txt.value = row[sDISPLAY_FIELD];
						// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
						else if ( row[sDISPLAY_FIELD] === undefined && !Sql.IsEmptyString(row[sDATA_FIELD]) && !Sql.IsEmptyString(sMODULE_TYPE) )
						{
							BindArguments(function(sMODULE_NAME, sID, txt, context)
							{
								Crm.Modules.ItemName(sMODULE_NAME, sID, function(status, message)
								{
									if ( status == 1 )
										txt.value = message;
								}, context);
							}, sMODULE_TYPE, row[sDATA_FIELD], txt, this)();
						}
					}
					else if ( sEDIT_NAME.indexOf('.Search') < 0 && sDATA_FIELD == 'TEAM_ID' )
					{
						txt.value = Security.TEAM_NAME();
					}
					else if ( sEDIT_NAME.indexOf('.Search') < 0 && sDATA_FIELD == 'ASSIGNED_USER_ID' )
					{
						if ( sDISPLAY_FIELD == 'ASSIGNED_TO_NAME' )
							txt.value = Security.FULL_NAME();
						else
							txt.value = Security.USER_NAME();
					}
					tdField.appendChild(txt);
					if ( sSubmitID != null )
					{
						txt.onkeypress = function(e)
						{
							return RegisterEnterKeyPress(e, sSubmitID);
						};
					}
					
					var hid = document.createElement('input');
					hid.id   = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					hid.type = 'hidden';
					if ( row != null )
					{
						if ( row[sDATA_FIELD] != null )
							hid.value = row[sDATA_FIELD];
					}
					else if ( sEDIT_NAME.indexOf('.Search') < 0 && sDATA_FIELD == 'TEAM_ID' && !Sql.IsEmptyGuid(Security.TEAM_ID()) )
					{
						hid.value = Security.TEAM_ID();
					}
					else if ( sEDIT_NAME.indexOf('.Search') < 0 && sDATA_FIELD == 'ASSIGNED_USER_ID' )
					{
						hid.value = Security.USER_ID();
					}
					tdField.appendChild(hid);
					
					var btnChange = document.createElement('input');
					btnChange.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_btnChange';
					btnChange.type      = 'button';
					btnChange.className = 'button';
					btnChange.title     = L10n.Term('.LBL_SELECT_BUTTON_TITLE');
					btnChange.value     = L10n.Term('.LBL_SELECT_BUTTON_LABEL');
					btnChange.style.marginLeft  = '4px';
					btnChange.style.marginRight = '2px';
					btnChange.onclick = BindArguments(function(txt, hid, sMODULE_TYPE, sDATA_LABEL, sFIELD_TYPE, sDATA_FIELD)
					{
						// 12/01/2012 Paul.  If this is a Parent Type popup, then we need to get the type from the dropdown list. 
						if ( sDATA_LABEL == 'PARENT_TYPE' && sFIELD_TYPE == 'ChangeButton' )
						{
							var lstField = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_PARENT_TYPE');
							sMODULE_TYPE = lstField.options[lstField.options.selectedIndex].value;
						}
						var $dialog = $('<div id="' + hid.id + '_divPopup"><div id="divPopupActionsPanel" /><div id="divPopupLayoutPanel" /></div>');
						$dialog.dialog(
						{
							  modal    : true
							, resizable: true
							, width    : $(window).width() > 0 ? ($(window).width() - 60) : 800
							, height   : (navigator.userAgent.indexOf('iPad') > 0 ? 'auto' : ($(window).height() > 0 ? $(window).height() - 60 : 800))
							, title    : L10n.Term(sMODULE_TYPE + '.LBL_LIST_FORM_TITLE')
							, create   : function(event, ui)
							{
								try
								{
									var oPopupViewUI = new PopupViewUI();
									oPopupViewUI.Load('divPopupLayoutPanel', 'divPopupActionsPanel', sMODULE_TYPE, false, function(status, message)
									{
										if ( status == 1 )
										{
											hid.value = message.ID  ;
											txt.value = message.NAME;
											// 02/21/2013 Paul.  Use close instead of destroy. 
											$dialog.dialog('close');
										}
										else if ( status == -2 )
										{
											// 02/21/2013 Paul.  Use close instead of destroy. 
											$dialog.dialog('close');
										}
										else if ( status == -1 )
										{
											SplendidError.SystemMessage(message);
										}
									});
								}
								catch(e)
								{
									SplendidError.SystemError(e, 'PopupViewUI dialog');
								}
							}
							, close    : function(event, ui)
							{
								$dialog.dialog('destroy');
								// 10/17/2011 Paul.  We have to remove the new HTML, otherwise there will be multiple definitions for divPopupLayoutPanel. 
								var divPopup = document.getElementById(hid.id + '_divPopup');
								divPopup.parentNode.removeChild(divPopup);
							}
						});
					}, txt, hid, sMODULE_TYPE, sDATA_LABEL, sFIELD_TYPE, sDATA_FIELD);
					tdField.appendChild(btnChange);
					// 12/15/2014 Paul.  Use small button on mobile device. 
					if ( bIsMobile )
					{
						btnChange.style.display = 'none';
						var aChange = document.createElement('a');
						tdField.appendChild(aChange);
						var iChange = document.createElement('i');
						iChange.className = 'fa fa-2x fa-location-arrow navButton';
						aChange.style.verticalAlign = 'bottom';
						aChange.appendChild(iChange);
						// 01/25/2015 Paul.  Need to bind arguments in order to ensure event attached to correct button. 
						aChange.onclick = BindArguments(function(btnChange)
						{
							btnChange.click();
						}, btnChange);
					}
					
					var btnClear = document.createElement('input');
					btnClear.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + "_btnClear";
					btnClear.type      = 'button';
					btnClear.className = 'button';
					btnClear.title     = L10n.Term('.LBL_CLEAR_BUTTON_TITLE');
					btnClear.value     = L10n.Term('.LBL_CLEAR_BUTTON_LABEL');
					btnClear.style.marginLeft  = '2px';
					btnClear.style.marginRight = '4px';
					btnClear.onclick = BindArguments(function(txt, hid)
					{
						hid.value = '';
						txt.value = '';
					}, txt, hid);
					tdField.appendChild(btnClear);
					// 12/15/2014 Paul.  Use small button on mobile device. 
					if ( bIsMobile )
					{
						btnClear.style.display = 'none';
						var aClear = document.createElement('a');
						tdField.appendChild(aClear);
						var iClear = document.createElement('i');
						iClear.className = 'fa fa-2x fa-remove navButton';
						aClear.style.verticalAlign = 'bottom';
						aClear.appendChild(iClear);
						// 01/25/2015 Paul.  Need to bind arguments in order to ensure event attached to correct button. 
						aClear.onclick = BindArguments(function(btnClear)
						{
							btnClear.click();
						}, btnClear);
					}
					
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
					this.AutoComplete(sLayoutPanel, sMODULE_TYPE, sTEMP_DISPLAY_FIELD, sDATA_FIELD);
					txt.onblur = BindArguments(this.AutoCompleteBlur, sLayoutPanel, sMODULE_TYPE, sTEMP_DISPLAY_FIELD, sDATA_FIELD);
				}
				else if ( sFIELD_TYPE == 'TeamSelect' )
				{
					var sTEAM_SET_ID   = '';
					var sTEAM_SET_LIST = '';
					if ( row != null )
					{
						sTEAM_SET_ID   = row['TEAM_SET_ID'];
						sTEAM_SET_LIST = row['TEAM_SET_LIST'];
					}
					var bAllowDefaults = row == null && sEDIT_NAME.indexOf('.Search') < 0 && sEDIT_NAME.indexOf('.Popup') < 0;
					this.LoadTeamSelect(sLayoutPanel, tdField, sTEAM_SET_ID, sTEAM_SET_LIST, bUI_REQUIRED, bAllowDefaults);
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
				}
				else if ( sFIELD_TYPE == 'TextBox' || sFIELD_TYPE == 'HtmlEditor' )
				{
					if ( nFORMAT_ROWS == 0 )
					{
						var txt = document.createElement('input');
						txt.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						txt.type      = 'text';
						if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
						if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
						if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
						tdField.appendChild(txt);
						// 09/10/2011 Paul.  Search fields can have multiple fields. 
						if ( sDATA_FIELD.indexOf(' ') > 0 )
						{
							var arrDATA_FIELD = sDATA_FIELD.split(' ');
							for ( var nFieldIndex in arrDATA_FIELD )
							{
								if ( row != null && row[arrDATA_FIELD[nFieldIndex]] != null )
									txt.value = row[arrDATA_FIELD[nFieldIndex]];
							}
						}
						else
						{
							if ( row != null && row[sDATA_FIELD] != null )
								txt.value = row[sDATA_FIELD];
						}
						// 08/31/2012 Paul.  Add support for speech. 
						if ( sFIELD_TYPE == 'TextBox' && bEnableSpeech )
						{
							txt.setAttribute('speech', 'speech');
							txt.setAttribute('x-webkit-speech', 'x-webkit-speech');
						}
						if ( sSubmitID != null )
						{
							txt.onkeypress = function(e)
							{
								return RegisterEnterKeyPress(e, sSubmitID);
							};
						}
					}
					else
					{
						var txt = document.createElement('textarea');
						tdField.appendChild(txt);
						txt.id = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
						if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
						if ( nFORMAT_ROWS       > 0 ) txt.rows      = nFORMAT_ROWS      ;
						if ( nFORMAT_COLUMNS    > 0 ) txt.cols      = nFORMAT_COLUMNS   ;
						if ( row != null && row[sDATA_FIELD] != null )
							txt.value = row[sDATA_FIELD];
						// 08/31/2012 Paul.  Add support for speech. 
						if ( sFIELD_TYPE == 'TextBox' && bEnableSpeech )
						{
							var txtSpeech = document.createElement('input');
							tdField.appendChild(txtSpeech);
							txtSpeech.id = txt.id + '_SPEECH';
							//txtSpeech.setAttribute('style', 'width: 15px; height: 20px; border: 0px; background-color: transparent; vertical-align:top;');
							txtSpeech.style.width           = '15px';
							txtSpeech.style.width           = '20px';
							txtSpeech.style.border          = '0px' ;
							txtSpeech.style.backgroundColor = 'transparent';
							txtSpeech.style.verticalAlign   = 'top';
							txtSpeech.setAttribute('speech', 'speech');
							txtSpeech.setAttribute('x-webkit-speech', 'x-webkit-speech');
							txtSpeech.onspeechchange = BindArguments(function(txtSpeech, txt)
							{
								try
								{
									txt.value += txtSpeech.value + ' ';
									txtSpeech.value = '';
									txt.focus();
								}
								catch(e)
								{
								}
							}, txtSpeech, txt);
							txtSpeech.onwebkitspeechchange = BindArguments(function(txtSpeech, txt)
							{
								try
								{
									txt.value += txtSpeech.value + ' ';
									txtSpeech.value = '';
									txt.focus();
								}
								catch(e)
								{
								}
							}, txtSpeech, txt);
						}
					}
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
				}
				else if ( sFIELD_TYPE == 'DatePicker' )
				{
					var txt = document.createElement('input');
					txt.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					txt.type      = 'text';
					if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
					if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
					if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
					tdField.appendChild(txt);
					if ( row != null && row[sDATA_FIELD] != null )
					{
						txt.value = FromJsonDate(row[sDATA_FIELD], Security.USER_DATE_FORMAT());
					}
					if ( sSubmitID != null )
					{
						txt.onkeypress = function(e)
						{
							return RegisterEnterKeyPress(e, sSubmitID);
						};
					}
					// http://www.phpeveryday.com/articles/jQuery-UI-Changing-the-date-format-for-Datepicker-P1023.html
					var sDATE_FORMAT = Security.USER_DATE_FORMAT();
					sDATE_FORMAT = sDATE_FORMAT.replace('yyyy', 'yy');
					sDATE_FORMAT = sDATE_FORMAT.replace('MM'  , 'mm');
					$('#' + txt.id).datepicker( { dateFormat: sDATE_FORMAT } );
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
				}
				else if ( sFIELD_TYPE == 'DateTimeEdit' || sFIELD_TYPE == 'DateTimeNewRecord' || sFIELD_TYPE == 'DateTimePicker' )
				{
					var sDATE_FORMAT = Security.USER_DATE_FORMAT();
					var sTIME_FORMAT = Security.USER_TIME_FORMAT();
					// 05/05/2013 Paul.  Remove the day name from the edit field. 
					sDATE_FORMAT = sDATE_FORMAT.replace('dddd,', '');
					sDATE_FORMAT = sDATE_FORMAT.replace('dddd' , '');
					sDATE_FORMAT = sDATE_FORMAT.replace('ddd,' , '');
					sDATE_FORMAT = sDATE_FORMAT.replace('ddd'  , '');
					sDATE_FORMAT = Trim(sDATE_FORMAT);
					
					var txt = document.createElement('input');
					txt.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					txt.type      = 'text';
					if ( nFORMAT_MAX_LENGTH > 0 ) txt.maxlength = nFORMAT_MAX_LENGTH;
					if ( nFORMAT_TAB_INDEX  > 0 ) txt.tabindex  = nFORMAT_TAB_INDEX ;
					if ( nFORMAT_SIZE       > 0 ) txt.size      = nFORMAT_SIZE      ;
					tdField.appendChild(txt);
					if ( row != null && row[sDATA_FIELD] != null )
					{
						txt.value = FromJsonDate(row[sDATA_FIELD], sDATE_FORMAT + ' ' + sTIME_FORMAT);
					}
					if ( sSubmitID != null )
					{
						txt.onkeypress = function(e)
						{
							return RegisterEnterKeyPress(e, sSubmitID);
						};
					}
					// 05/05/2013 Paul.  We need to convert .NET date formatting to TimePicker date formatting. 
					// http://arshaw.com/fullcalendar/docs/utilities/formatDate/
					// http://trentrichardson.com/examples/timepicker/
					// http://docs.jquery.com/UI/Datepicker/formatDate
					sDATE_FORMAT = sDATE_FORMAT.replace('dddd', 'DD');
					sDATE_FORMAT = sDATE_FORMAT.replace('ddd' , 'D' );
					sDATE_FORMAT = sDATE_FORMAT.replace('yyyy', 'yy');
					sDATE_FORMAT = sDATE_FORMAT.replace('MMMM', 'XX');  // Temp variables. 
					sDATE_FORMAT = sDATE_FORMAT.replace('MMM' , 'X' );
					sDATE_FORMAT = sDATE_FORMAT.replace('MM'  , 'mm');
					sDATE_FORMAT = sDATE_FORMAT.replace('M'   , 'm' );
					sDATE_FORMAT = sDATE_FORMAT.replace('XX'  , 'MM');  // Temp variables. 
					sDATE_FORMAT = sDATE_FORMAT.replace('X'   , 'M' );
					var bAMPM        = (sTIME_FORMAT.indexOf('t') >= 0) || (sTIME_FORMAT.indexOf('T') >= 0);
					$('#' + txt.id).datetimepicker( { dateFormat: sDATE_FORMAT, timeFormat: sTIME_FORMAT, ampm: bAMPM } );
					// 05/05/2013 Paul.  Add format for debugging. 
					//tdField.appendChild(document.createTextNode(sDATE_FORMAT + ' ' + sTIME_FORMAT));
					if ( bUI_REQUIRED )
					{
						var reqNAME = document.createElement('span');
						reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
						reqNAME.className         = 'required';
						reqNAME.style.paddingLeft = '4px';
						reqNAME.style.display     = 'none';
						reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
						tdField.appendChild(reqNAME);
					}
				}
				else if ( sFIELD_TYPE == 'ListBox' )
				{
					if ( sLIST_NAME != null )
					{
						var lst = document.createElement('select');
						tdField.appendChild(lst);
						lst.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						if ( nFORMAT_ROWS > 0 )
						{
							lst.multiple = 'multiple';
							lst.size     = nFORMAT_ROWS;
						}
						if ( nFORMAT_TAB_INDEX > 0 )
						{
							lst.tabindex = nFORMAT_TAB_INDEX;
						}
						var sDATA_VALUE = null;
						if ( row != null && row[sDATA_FIELD] != null )
							sDATA_VALUE = row[sDATA_FIELD];
						// 09/27/2012 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
						if ( !Sql.IsEmptyString(sPARENT_FIELD) )
						{
							var lstPARENT_FIELD = document.getElementById(sLayoutPanel + '_ctlEditView_' + sPARENT_FIELD);
							if ( lstPARENT_FIELD != null )
							{
								sLIST_NAME = lstPARENT_FIELD.options[lstPARENT_FIELD.options.selectedIndex].value;
								lstPARENT_FIELD.onchange = BindArguments(function(lst, lstPARENT_FIELD, bUI_REQUIRED)
								{
									lst.options.length = 0;
									var sLIST_NAME = lstPARENT_FIELD.options[lstPARENT_FIELD.options.selectedIndex].value;
									var arrLIST = L10n.GetList(sLIST_NAME);
									if ( arrLIST != null )
									{
										if ( !bUI_REQUIRED )
										{
											var opt = document.createElement('option');
											lst.appendChild(opt);
											opt.setAttribute('value', '');
											opt.innerHTML = L10n.Term('.LBL_NONE');
											if ( sDATA_VALUE != null && sDATA_VALUE == '' )
												opt.setAttribute('selected', 'selected');
										}
										for ( var i = 0; i < arrLIST.length; i++ )
										{
											var opt = document.createElement('option');
											lst.appendChild(opt);
											opt.setAttribute('value', arrLIST[i]);
											// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
											opt.innerHTML = L10n.ListTerm(sLIST_NAME, arrLIST[i]);
											if ( sDATA_VALUE != null && sDATA_VALUE == arrLIST[i] )
												opt.setAttribute('selected', 'selected');
										}
									}
								}, lst, lstPARENT_FIELD, bUI_REQUIRED);
							}
						}
						var arrLIST = L10n.GetList(sLIST_NAME);
						if ( arrLIST != null )
						{
							if ( !bUI_REQUIRED )
							{
								var opt = document.createElement('option');
								lst.appendChild(opt);
								opt.setAttribute('value', '');
								opt.innerHTML = L10n.Term('.LBL_NONE');
								if ( sDATA_VALUE != null && sDATA_VALUE == '' )
									opt.setAttribute('selected', 'selected');
							}
							for ( var i = 0; i < arrLIST.length; i++ )
							{
								var opt = document.createElement('option');
								lst.appendChild(opt);
								opt.setAttribute('value', arrLIST[i]);
								// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
								opt.innerHTML = L10n.ListTerm(sLIST_NAME, arrLIST[i]);
								if ( sDATA_VALUE != null && sDATA_VALUE == arrLIST[i] )
									opt.setAttribute('selected', 'selected');
							}
							if ( bUI_REQUIRED && nFORMAT_ROWS > 0 )
							{
								var reqNAME = document.createElement('span');
								reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
								reqNAME.className         = 'required';
								reqNAME.style.paddingLeft = '4px';
								reqNAME.style.display     = 'none';
								reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
								tdField.appendChild(reqNAME);
							}
						}
						// 09/27/2012 Paul. Allow onchange code to be stored in the database.  
						if ( !Sql.IsEmptyString(sONCLICK_SCRIPT) )
						{
							lst.onchange = BindArguments(function(sONCLICK_SCRIPT)
							{
								if ( StartsWith(sONCLICK_SCRIPT, 'return ') )
									sONCLICK_SCRIPT = sONCLICK_SCRIPT.substring(7);
								eval(sONCLICK_SCRIPT);
							}, sONCLICK_SCRIPT);
						}
					}
				}
				// 08/01/2013 Paul.  Add support for CheckBoxList. 
				else if ( sFIELD_TYPE == 'CheckBoxList' )
				{
					if ( sLIST_NAME != null )
					{
						var lst = document.createElement('div');
						tdField.appendChild(lst);
						lst.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						if ( nFORMAT_ROWS > 0 )
						{
							lst.style.height    = nFORMAT_ROWS.toString() + 'px';
							lst.style.overflowY = 'auto';
						}
						if ( nFORMAT_TAB_INDEX > 0 )
						{
							lst.tabindex = nFORMAT_TAB_INDEX;
						}
						var sDATA_VALUE = null;
						if ( row != null && row[sDATA_FIELD] != null )
							sDATA_VALUE = row[sDATA_FIELD];
						var arrLIST = L10n.GetList(sLIST_NAME);
						if ( arrLIST != null )
						{
							for ( var i = 0; i < arrLIST.length; i++ )
							{
								var chk = document.createElement('input');
								// 06/18/2011 Paul.  IE requires that the input type be defined prior to appending the field. 
								chk.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i];
								chk.type      = 'checkbox';
								chk.className = 'checkbox';
								lst.appendChild(chk);
								chk.setAttribute('value', arrLIST[i]);
								var lab = document.createElement('label');
								lab.setAttribute('for', chk.id);
								lst.appendChild(lab);
								lab.innerHTML = L10n.ListTerm(sLIST_NAME, arrLIST[i]);
								var br = document.createElement('br');
								lst.appendChild(br);
							}
							// 08/01/2013 Paul.  Expand XML values from CheckBoxList. 
							if ( sDATA_VALUE != null && StartsWith(sDATA_VALUE, '<?xml') )
							{
								var xmlVALUES = $.parseXML(sDATA_VALUE);
								$(xmlVALUES).find('Value').each(function()
								{
									var chk = document.getElementById(sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + $(this).text());
									if ( chk != null )
										chk.checked = true;
								});
							}
							if ( bUI_REQUIRED && nFORMAT_ROWS > 0 )
							{
								var reqNAME = document.createElement('span');
								reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
								reqNAME.className         = 'required';
								reqNAME.style.paddingLeft = '4px';
								reqNAME.style.display     = 'none';
								reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
								tdField.appendChild(reqNAME);
							}
						}
					}
				}
				// 08/01/2013 Paul.  Add support for Radio. 
				else if ( sFIELD_TYPE == 'Radio' )
				{
					if ( sLIST_NAME != null )
					{
						var lst = document.createElement('div');
						tdField.appendChild(lst);
						lst.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
						if ( nFORMAT_ROWS > 0 )
						{
							lst.style.height    = nFORMAT_ROWS.toString() + 'px';
							lst.style.overflowY = 'auto';
						}
						if ( nFORMAT_TAB_INDEX > 0 )
						{
							lst.tabindex = nFORMAT_TAB_INDEX;
						}
						var sDATA_VALUE = null;
						if ( row != null && row[sDATA_FIELD] != null )
							sDATA_VALUE = row[sDATA_FIELD];
						var arrLIST = L10n.GetList(sLIST_NAME);
						if ( arrLIST != null )
						{
							if ( !bUI_REQUIRED )
							{
								var rad = document.createElement('input');
								rad.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_';
								rad.name      = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
								rad.type      = 'radio';
								rad.className = 'radio';
								lst.appendChild(rad);
								rad.setAttribute('value', '');
								var lab = document.createElement('label');
								lab.setAttribute('for', rad.id);
								lst.appendChild(lab);
								lab.innerHTML = L10n.Term('.LBL_NONE');
								var br = document.createElement('br');
								lst.appendChild(br);
								if ( sDATA_VALUE == null || sDATA_VALUE == '' )
									rad.checked = true;
							}
							for ( var i = 0; i < arrLIST.length; i++ )
							{
								var rad = document.createElement('input');
								// 06/18/2011 Paul.  IE requires that the input type be defined prior to appending the field. 
								rad.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_' + arrLIST[i];
								rad.name      = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
								rad.type      = 'radio';
								rad.className = 'radio';
								lst.appendChild(rad);
								rad.setAttribute('value', arrLIST[i]);
								var lab = document.createElement('label');
								lab.setAttribute('for', rad.id);
								lst.appendChild(lab);
								lab.innerHTML = L10n.ListTerm(sLIST_NAME, arrLIST[i]);
								var br = document.createElement('br');
								lst.appendChild(br);
								if ( sDATA_VALUE == arrLIST[i] )
									rad.checked = true;
							}
							if ( bUI_REQUIRED && nFORMAT_ROWS > 0 )
							{
								var reqNAME = document.createElement('span');
								reqNAME.id                = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD + '_REQUIRED';
								reqNAME.className         = 'required';
								reqNAME.style.paddingLeft = '4px';
								reqNAME.style.display     = 'none';
								reqNAME.innerHTML         = L10n.Term('.ERR_REQUIRED_FIELD');
								tdField.appendChild(reqNAME);
							}
						}
					}
				}
				else if ( sFIELD_TYPE == 'CheckBox' )
				{
					var chk = document.createElement('input');
					// 06/18/2011 Paul.  IE requires that the input type be defined prior to appending the field. 
					chk.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					chk.type      = 'checkbox';
					chk.className = 'checkbox';
					// 09/25/2011 Paul.  IE does not allow you to set the type after it is added to the document. 
					tdField.appendChild(chk);
					if ( nFORMAT_TAB_INDEX > 0 ) chk.tabindex  = nFORMAT_TAB_INDEX;
					var sDATA_VALUE = 'false';
					if ( row != null && row[sDATA_FIELD] != null )
						sDATA_VALUE = row[sDATA_FIELD];
					chk.checked = Sql.ToBoolean(sDATA_VALUE);
					// 03/10/2013 Paul. Add support for onClick. 
					if ( !Sql.IsEmptyString(sONCLICK_SCRIPT) )
					{
						chk.onclick = BindArguments(function(sONCLICK_SCRIPT)
						{
							if ( StartsWith(sONCLICK_SCRIPT, 'return ') )
								sONCLICK_SCRIPT = sONCLICK_SCRIPT.substring(7);
							eval(sONCLICK_SCRIPT);
						}, sONCLICK_SCRIPT);
					}
				}
				else if ( sFIELD_TYPE == 'Label' )
				{
					var lbl = document.createElement('span');
					lbl.id        = sLayoutPanel + '_ctlEditView_' + sDATA_FIELD;
					tdField.appendChild(lbl);
					if ( sDATA_FIELD.indexOf('.') > 0 )
						lbl.innerHTML = L10n.Term(sDATA_FIELD);
					else if ( row != null && row[sDATA_FIELD] != null )
						lbl.innerHTML = row[sDATA_FIELD];
				}
				else
				{
					//08/31/2012 Paul.  Add debugging code. 
					//alert('Unknown field type: ' + sFIELD_TYPE);
				}
			}
			catch(e)
			{
				SplendidError.SystemAlert(e, sFIELD_TYPE + ' ' + sDATA_FIELD);
			}
			// 12/02/2007 Paul.  Each view can now have its own number of data columns. 
			// This was needed so that search forms can have 4 data columns. The default is 2 columns. 
			if ( nCOLSPAN > 0 )
				nColIndex += nCOLSPAN;
			else if ( nCOLSPAN == 0 )
				nColIndex++;
			if ( nColIndex >= nDATA_COLUMNS )
				nColIndex = 0;
		}
		// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sFORM_SCRIPT = lay.SCRIPT;
			if ( !Sql.IsEmptyString(sFORM_SCRIPT) )
			{
				sFORM_SCRIPT = sFORM_SCRIPT.replace("SPLENDID_EDITVIEW_LAYOUT_ID", sLayoutPanel + '_ctlEditView');
				eval(sFORM_SCRIPT);
			}
			break;
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'EditViewUI.LoadView');
	}
}

EditViewUI.prototype.LoadItem = function(sLayoutPanel, sActionsPanel, layout, sEDIT_NAME, sMODULE_NAME, sID, sSubmitID, callback)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		if ( !Sql.IsEmptyString(sID) )
		{
			bgPage.EditView_LoadItem(sMODULE_NAME, sID, function(status, message)
			{
				if ( status == 1 )
				{
					// 10/04/2011 Paul.  EditViewUI.LoadItem returns the row. 
					var row = message;
					DynamicButtonsUI_Clear(sActionsPanel);
					this.Clear(sLayoutPanel);
					// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
					ctlActiveMenu.ActivateTab(sMODULE_NAME, sID, 'EditView', this);
					// 01/30/2013 Paul.  Clicking on the sub-title will refresh the view as a way to correct bugs in the rendering. 
					SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, Sql.ToString(row['NAME']), Sql.ToString(row['ID']));
					DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sEDIT_NAME, row, this.PageCommand, function(status, message)
					{
						if ( status != 1 )
							callback(status, message);
					}, this);
					
					//var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
					var tblMain = document.getElementById(sLayoutPanel + '_ctlEditView_tblMain');
					this.LoadView(sLayoutPanel, tblMain, layout, row, sSubmitID);
				}
				else
				{
					callback(status, message);
				}
			}, this);
		}
		else
		{
			this.Clear(sLayoutPanel);
			ctlActiveMenu.ActivateTab(sMODULE_NAME, sID, 'EditView', this);
			
			var row = null;
			SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, '');
			DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sEDIT_NAME, row, this.PageCommand, function(status, message)
			{
				if ( status != 1 )
					callback(status, message);
			}, this);
			
			//var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
			var tblMain = document.getElementById(sLayoutPanel + '_ctlEditView_tblMain');
			this.LoadView(sLayoutPanel, tblMain, layout, row, sSubmitID);
		}
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'EditViewUI.LoadItem'));
	}
}

EditViewUI.prototype.LoadObject = function(sLayoutPanel, sActionsPanel, sEDIT_NAME, sMODULE_NAME, row, sSubmitID, PageCommand, callback)
{
	try
	{
		this.MODULE = sMODULE_NAME;
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
		{
			if ( status == 0 || status == 1 )
			{
				bgPage.EditView_LoadLayout(sEDIT_NAME, function(status, message)
				{
					if ( status == 1 )
					{
						// 10/03/2011 Paul.  EditView_LoadLayout returns the layout. 
						var layout = message;
						// 07/30/2013 Paul.  Check for layout not row. 
						if ( layout != null )
						{
							DynamicButtonsUI_Clear(sActionsPanel);
							this.Clear(sLayoutPanel);
							// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
							var sID = Sql.ToString(row['ID']);
							ctlActiveMenu.ActivateTab(sMODULE_NAME, sID, 'EditView', this);
							SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, '');
							DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sEDIT_NAME, row, PageCommand, function(status, message)
							{
							}, this);
							
							//var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
							var tblMain = document.getElementById(sLayoutPanel + '_ctlEditView_tblMain');
							this.LoadView(sLayoutPanel, tblMain, layout, row, sSubmitID);
							
							callback(status, message);
						}
						else
						{
							callback(-1, message);
						}
					}
					else
					{
						callback(status, message);
					}
				}, this);
			}
			else
			{
				callback(status, message);
			}
		}, this);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'EditViewUI.LoadObject'));
	}
}

EditViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, bDUPLICATE)
{
	try
	{
		this.MODULE    = null;
		this.ID        = null;
		this.DUPLICATE = false;
		// 03/16/2014 Paul.  LAST_DATE_MODIFIED is needed for concurrency test. 
		this.LAST_DATE_MODIFIED = null;
		
		var bgPage = chrome.extension.getBackgroundPage();
		// 11/29/2011 Paul.  We are having an issue with the globals getting reset, so we need to re-initialize. 
		if ( !bgPage.SplendidCache.IsInitialized() )
		{
			SplendidUI_ReInit(sLayoutPanel, sActionsPanel, sMODULE_NAME);
			return;
		}
		
		this.MODULE    = sMODULE_NAME;
		this.ID        = sID         ;
		this.DUPLICATE = bDUPLICATE  ;
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						var sEDIT_NAME = sMODULE_NAME + '.EditView' + sPLATFORM_LAYOUT;
						bgPage.EditView_LoadLayout(sEDIT_NAME, function(status, message)
						{
							if ( status == 1 )
							{
								// 10/03/2011 Paul.  EditView_LoadLayout returns the layout. 
								var layout = message;
								this.LoadItem(sLayoutPanel, sActionsPanel, layout, sEDIT_NAME, sMODULE_NAME, sID, 'btnDynamicButtons_Save', function(status, message)
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
							else
							{
								SplendidError.SystemMessage(message);
							}
						}, this);
					}
					else
					{
						SplendidError.SystemMessage(message);
					}
				}, this);
			}
			else
			{
				SplendidError.SystemMessage(message);
			}
		}, this);
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'EditViewUI.Load');
	}
}

