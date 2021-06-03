/* Copyright (C) 2011-2015 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function DetailViewUI()
{
	this.MODULE  = null;
	this.ID      = null;
}

DetailViewUI.prototype.PageCommand = function(sLayoutPanel, sActionsPanel, sCommandName, sCommandArguments)
{
	try
	{
		if ( sCommandName == 'Edit' )
		{
			var oEditViewUI = new EditViewUI();
			oEditViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, this.ID, false);
			this.MODULE  = null;
			this.ID      = null;
		}
		else if ( sCommandName == 'Duplicate' )
		{
			var oEditViewUI = new EditViewUI();
			oEditViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, this.ID, true);
			this.MODULE  = null;
			this.ID      = null;
		}
		else if ( sCommandName == 'Delete' )
		{
			var bgPage = chrome.extension.getBackgroundPage();
			bgPage.DeleteModuleItem(this.MODULE, this.ID, function(status, message)
			{
				if ( status == 1 )
				{
					var sGRID_NAME = this.MODULE + '.ListView' + sPLATFORM_LAYOUT;
					var oListViewUI = new ListViewUI();
					oListViewUI.Reset(sLayoutPanel, this.MODULE);
					oListViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, sGRID_NAME, null, function(status, message)
					{
						if ( status == 0 || status == 1 )
						{
							this.MODULE  = null;
							this.ID      = null;
						}
					});
				}
				else
				{
					SplendidError.SystemMessage(message);
				}
			}, this);
		}
		else if ( sCommandName == 'ViewLog' )
		{
			SplendidError.SystemMessage('ViewLog not supported');
		}
		else if ( sCommandName == 'Cancel' )
		{
			var sGRID_NAME = this.MODULE + '.ListView' + sPLATFORM_LAYOUT;
			var oListViewUI = new ListViewUI();
			oListViewUI.Reset(sLayoutPanel, this.MODULE);
			oListViewUI.Load(sLayoutPanel, sActionsPanel, this.MODULE, sGRID_NAME, null, function(status, message)
			{
				if ( status == 0 || status == 1 )
				{
					this.MODULE  = null;
					this.ID      = null;
				}
			});
		}
		else
		{
			SplendidError.SystemMessage('DetailViewUI.PageCommand: Unknown command ' + sCommandName);
		}
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'DetailViewUI.PageCommand');
	}
}

DetailViewUI.prototype.Clear = function(sLayoutPanel)
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
			alert('DetailViewUI.Clear: ' + sLayoutPanel + ' does not exist');
			return;
		}
		// <table id="ctlDetailView_tblMain" class="tabDetailView">
		var ctlDetailView_tblMain = document.createElement('table');
		ctlDetailView_tblMain.id        = sLayoutPanel + '_ctlDetailView_tblMain';
		ctlDetailView_tblMain.width     = '100%';
		ctlDetailView_tblMain.className = 'tabDetailView';
		divMainLayoutPanel.appendChild(ctlDetailView_tblMain);
		// 10/08/2012 Paul.  Add DetailSubPanel. 
		// <div id="divDetailSubPanel">
		var divDetailSubPanel = document.createElement('div');
		divDetailSubPanel.id = sLayoutPanel + '_divDetailSubPanel';
		divMainLayoutPanel.appendChild(divDetailSubPanel);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'DetailViewUI.Clear');
	}
}

NormalizeDescription = function(sDESCRIPTION)
{
	// 06/04/2010 Paul.  Try and prevent excess blank lines. 
	sDESCRIPTION = sDESCRIPTION.replace(/\r\n/g     , '\n');
	sDESCRIPTION = sDESCRIPTION.replace(/\r/g       , '\n');
	sDESCRIPTION = sDESCRIPTION.replace(/<br \/>\n/g, '\n');
	sDESCRIPTION = sDESCRIPTION.replace(/<br\/>\n/g , '\n');
	sDESCRIPTION = sDESCRIPTION.replace(/<br>\n/g   , '\n');
	sDESCRIPTION = sDESCRIPTION.replace(/\n/g       , '<br \/>\r\n');
	return sDESCRIPTION;
}


DetailViewUI.prototype.LoadView = function(sLayoutPanel, sActionsPanel, tblMain, layout, row)
{
	try
	{
		// 10/17/2012 Paul.  Exit if the Main does not exist.  This is a sign that the user has navigated elsewhere. 
		if ( tblMain == null )
			return;
		var tbody = document.createElement('tbody');
		tblMain.appendChild(tbody);
		
		var tr = null;
		var nColumn = 0;
		var bEnableTeamManagement = Crm.Config.enable_team_management();
		var bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
		// 12/06/2014 Paul.  Use new mobile flag. 
		var bIsMobile = isMobileDevice();
		if ( isMobileLandscape() )
			bIsMobile = false;
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sFIELD_TYPE   = lay.FIELD_TYPE  ;
			var sDATA_LABEL   = lay.DATA_LABEL  ;
			var sDATA_FIELD   = lay.DATA_FIELD  ;
			var sDATA_FORMAT  = lay.DATA_FORMAT ;
			var sURL_FIELD    = lay.URL_FIELD   ;
			var sURL_FORMAT   = lay.URL_FORMAT  ;
			var sURL_TARGET   = lay.URL_TARGET  ;
			var sLIST_NAME    = lay.LIST_NAME   ;
			var nCOLSPAN      = Sql.ToInteger(lay.COLSPAN);
			var sLABEL_WIDTH  = lay.LABEL_WIDTH ;
			var sFIELD_WIDTH  = lay.FIELD_WIDTH ;
			var nDATA_COLUMNS = Sql.ToInteger(lay.DATA_COLUMNS);
			var sVIEW_NAME    = lay.VIEW_NAME   ;
			var sMODULE_NAME  = lay.MODULE_NAME ;
			var sTOOL_TIP     = lay.TOOL_TIP    ;
			var sMODULE_TYPE  = lay.MODULE_TYPE ;
			var sPARENT_FIELD = lay.PARENT_FIELD;
			
			// 02/28/2014 Paul.  We are going to start using the data column in the Preview panel. 
			if ( nDATA_COLUMNS == 0 )
				nDATA_COLUMNS = 2;
			
			if ( (sDATA_FIELD == 'TEAM_NAME' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( !bEnableTeamManagement )
				{
					sFIELD_TYPE = 'Blank';
				}
				else if ( bEnableDynamicTeams )
				{
					sDATA_LABEL = '.LBL_TEAM_SET_NAME';
					sDATA_FIELD = 'TEAM_SET_NAME';
				}
			}
			if ( sDATA_FIELD == 'EXCHANGE_FOLDER' )
			{
				if ( !Crm.Modules.ExchangeFolders(sMODULE_NAME) )
				{
					sFIELD_TYPE = 'Blank';
				}
			}
			// 09/02/2012 Paul.  A separator will create a new table. We need to match the outer and inner layout. 
			if ( sFIELD_TYPE == 'Separator' )
			{
				var tblNew = document.createElement('table');
				// 09/27/2012 Paul.  Separator can have an ID and can have a style so that it can be hidden. 
				if ( !Sql.IsEmptyString(sDATA_FIELD) )
					tblNew.id = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD;
				if ( !Sql.IsEmptyString(sDATA_FORMAT) )
					tblNew.setAttribute('style', sDATA_FORMAT);
				tblNew.className = 'tabDetailView';
				tblNew.style.marginTop = '5px';
				if ( tblMain.nextSibling == null )
					tblMain.parentNode.appendChild(tblNew);
				else
					tblMain.parentNode.insertBefore(tblNew, tblMain.nextSibling);
				tblMain = tblNew;
				tbody = document.createElement('tbody');
				tblMain.appendChild(tbody);
				nColumn = 0;
				tr = null;
				continue;
			}
			// 12/06/2014 Paul.  Use new mobile flag. 
			if ( nColumn % nDATA_COLUMNS == 0 || tr == null || bIsMobile )
			{
				tr = document.createElement('tr');
				tbody.appendChild(tr);
			}
			var tdLabel = document.createElement('td');
			tr.appendChild(tdLabel);
			tdLabel.className = 'tabDetailViewDL';
			tdLabel.vAlign    = 'top';
			tdLabel.width     = sLABEL_WIDTH;
			if ( sDATA_LABEL != null )
			{
				if ( sFIELD_TYPE != 'Blank' )
				{
					if ( sDATA_LABEL.indexOf('.') >= 0 )
					{
						var txt = document.createTextNode(L10n.Term(sDATA_LABEL));
						tdLabel.appendChild(txt);
					}
					else if ( !Sql.IsEmptyString(sDATA_LABEL) && !Sql.IsEmptyString(row[sDATA_LABEL]) )
					{
						var txt = document.createTextNode(row[sDATA_LABEL]);
						tdLabel.appendChild(txt);
					}
				}
			}
			
			var tdField = document.createElement('td');
			tr.appendChild(tdField);
			tdField.className = 'tabDetailViewDF';
			tdField.vAlign    = 'top';
			tdField.width     = sFIELD_WIDTH;
			tdField.id        = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD;
			if ( nCOLSPAN > 0 )
			{
				tdField.colSpan = nCOLSPAN;
				nColumn++;
			}
			if ( sFIELD_TYPE == 'HyperLink' )
			{
				// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
				if ( (row[sDATA_FIELD] != null || row[sDATA_FIELD] === undefined) && sURL_FORMAT != null && sDATA_FORMAT != null )
				{
					var a = document.createElement('a');
					tdField.appendChild(a);
					if ( sURL_FORMAT.substr(0, 2) == '~/' )
					{
						var arrURL_FORMAT = sURL_FORMAT.split('/');
						var sURL_MODULE_NAME = sMODULE_NAME;
						if ( arrURL_FORMAT.length > 1 )
							sURL_MODULE_NAME = arrURL_FORMAT[1];
						if ( sURL_MODULE_NAME == 'Parents' )
						{
							sURL_MODULE_NAME = row[sDATA_LABEL];
						}
						a.href = '#';
						a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
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
						}, sLayoutPanel, sActionsPanel, sURL_MODULE_NAME, row[sURL_FIELD]);
						// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
						if ( row[sDATA_FIELD] === undefined && !Sql.IsEmptyString(row[sURL_FIELD]) )
						{
							BindArguments(function(sMODULE_NAME, sID, sDATA_FORMAT, a, context)
							{
								Crm.Modules.ItemName(sMODULE_NAME, sID, function(status, message)
								{
									if ( status == 1 )
									{
										a.innerHTML = sDATA_FORMAT.replace('{0}', message);
									}
								}, context);
							}, sURL_MODULE_NAME, row[sURL_FIELD], sDATA_FORMAT, a, this)();
						}
					}
					else
					{
						a.href = sURL_FORMAT.replace('{0}', row[sURL_FIELD]);
					}
					// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
					if ( row[sDATA_FIELD] !== undefined )
					{
						a.innerHTML = sDATA_FORMAT.replace('{0}', row[sDATA_FIELD]);
					}
				}
			}
			else if ( sFIELD_TYPE == 'String' )
			{
				if ( sDATA_FORMAT != null )
				{
					try
					{
						if ( sDATA_FORMAT.indexOf(' ') > 0 )
						{
							var arrDATA_FORMAT = sDATA_FORMAT.split(' ');
							var arrDATA_FIELD  = sDATA_FIELD.split(' ');
							for ( var nFormatIndex = 0; nFormatIndex < arrDATA_FIELD.length; nFormatIndex++ )
							{
								if ( arrDATA_FIELD[nFormatIndex].indexOf('.') >= 0 )
								{
									sDATA_FORMAT = sDATA_FORMAT.replace('{' + nFormatIndex.toString() + '}', L10n.Term(arrDATA_FIELD[nFormatIndex]));
								}
								else
								{
									//alert(arrDATA_FIELD[nFormatIndex] + ' ' + row[arrDATA_FIELD[nFormatIndex]]);
									if ( row[arrDATA_FIELD[nFormatIndex]] == null )
									{
										sDATA_FORMAT = sDATA_FORMAT.replace('{' + nFormatIndex.toString() + '}', '');
									}
									else
									{
										var sDATA_VALUE = row[arrDATA_FIELD[nFormatIndex]];
										sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());
										//alert(row[arrDATA_FIELD[nFormatIndex]] + ' = ' + sDATA_VALUE);
										sDATA_FORMAT = sDATA_FORMAT.replace('{' + nFormatIndex.toString() + '}', sDATA_VALUE);
									}
								}
							}
							// 12/24/2012 Paul.  Use regex global replace flag. 
							tdField.id = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD.replace(/\s/g, '_');
							tdField.innerHTML = sDATA_FORMAT;
						}
						else if ( row[sDATA_FIELD] != null )
						{
							// 12/24/2012 Paul.  Use regex global replace flag. 
							tdField.id = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD.replace(/\s/g, '_');
							if ( !Sql.IsEmptyString(sLIST_NAME) )
							{
								var sDATA_VALUE = Sql.ToString(row[sDATA_FIELD]);
								// 08/01/2013 Paul.  Expand XML values from CheckBoxList. 
								if ( StartsWith(sDATA_VALUE, '<?xml') )
								{
									var sVALUES = '';
									var xmlVALUES = $.parseXML(sDATA_VALUE);
									$(xmlVALUES).find('Value').each(function()
									{
										if ( sVALUES.length > 0 )
											sVALUES += ', ';
										sVALUES += L10n.ListTerm(sLIST_NAME, $(this).text());
									});
									sDATA_VALUE = sVALUES;
								}
								else
								{
									// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
									sDATA_VALUE = L10n.ListTerm(sLIST_NAME, sDATA_VALUE);
								}
								tdField.innerHTML = sDATA_FORMAT.replace('{0}', sDATA_VALUE);
							}
							else
							{
								var sDATA_VALUE = row[sDATA_FIELD];
								try
								{
									if ( sDATA_FORMAT.indexOf('{0:d}') >= 0 )
									{
										sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT());
										tdField.innerHTML = sDATA_FORMAT.replace('{0:d}', sDATA_VALUE);
									}
									else if ( sDATA_FORMAT.indexOf('{0:c}') >= 0 )
									{
										tdField.innerHTML = sDATA_FORMAT.replace('{0:c}', sDATA_VALUE);
									}
									// 10/03/2011 Paul.  If the data value is an integer, then substr() will throw an exception. 
									else if ( typeof(sDATA_VALUE) == 'string' && sDATA_VALUE.substr(0, 7) == '\\/Date(' )
									{
										sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());
										tdField.innerHTML = sDATA_FORMAT.replace('{0}', sDATA_VALUE);
									}
									else
									{
										// 08/26/2014 Paul.  Text with angle brackets (such as an email), will generate an error when used with innerHTML. 
										//tdField.innerHTML = sDATA_FORMAT.replace('{0}', sDATA_VALUE);
										tdField.appendChild(document.createTextNode(sDATA_FORMAT.replace('{0}', sDATA_VALUE)));
									}
								}
								catch(e)
								{
									//alert(dumpObj(sDATA_VALUE, e.message));
								}
							}
						}
						// 11/30/2012 Paul.  Special formatting for Address HTML fields are normally provided by special _Edit view.
						else if ( sDATA_FORMAT == '{0}' && EndsWith(sDATA_FIELD, 'ADDRESS_HTML') )
						{
							// 'PRIMARY_ADDRESS_HTML'
							// 'ALT_ADDRESS_HTML'
							// 'BILLING_ADDRESS_HTML'
							// 'SHIPPING_ADDRESS_HTML'
							var sADDRESS_BASE = sDATA_FIELD.replace('_HTML', '_');
							tdField.innerHTML = Sql.ToString(row[sADDRESS_BASE + 'STREET'    ]) + '<br />'
							                  + Sql.ToString(row[sADDRESS_BASE + 'CITY'      ]) + ' '
							                  + Sql.ToString(row[sADDRESS_BASE + 'STATE'     ]) + ' &nbsp;&nbsp;'
							                  + Sql.ToString(row[sADDRESS_BASE + 'POSTALCODE']) + '<br />'
							                  + Sql.ToString(row[sADDRESS_BASE + 'COUNTRY'   ]) + ' ';
						}
					}
					catch(e)
					{
						SplendidError.SystemAlert(e, 'DetailViewUI.LoadView');
					}
				}
			}
			else if ( sFIELD_TYPE == 'TextBox' )
			{
				if ( row[sDATA_FIELD] != null )
				{
					// 12/24/2012 Paul.  Use regex global replace flag. 
					tdField.id = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD.replace(/\s/g, '_');
					var sDATA = row[sDATA_FIELD];
					sDATA = NormalizeDescription(sDATA);
					try
					{
						tdField.innerHTML = sDATA;
					}
					catch(e)
					{
						sDATA = row[sDATA_FIELD];
						sDATA = sDATA.replace(/</g, '&lt;');
						sDATA = sDATA.replace(/>/g, '&gt;');
						var pre = document.createElement('pre');
						tdField.appendChild(pre);
						pre.innerHTML = sDATA;
					}
				}
			}
			else if ( sFIELD_TYPE == 'CheckBox' )
			{
				var chk = document.createElement('input');
				// 06/18/2011 Paul.  IE requires that the input type be defined prior to appending the field. 
				// 12/24/2012 Paul.  Use regex global replace flag. 
				chk.id        = sLayoutPanel + '_ctlDetailView_' + sDATA_FIELD.replace(/\s/g, '_');
				chk.type      = 'checkbox';
				chk.className = 'checkbox';
				chk.disabled  = 'disabled';
				// 09/25/2011 Paul.  IE does not allow you to set the type after it is added to the document. 
				tdField.appendChild(chk);
				chk.checked   = Sql.ToBoolean(row[sDATA_FIELD]);
			}
			else if ( sFIELD_TYPE == 'Blank' )
			{
				tdField.innerHTML = '';
			}
			// 09/03/2012 Paul.  A header is similar to a label, but without the data field. 
			else if ( sFIELD_TYPE == 'Header' )
			{
				tdLabel.innerHTML = '<h4>' + tdLabel.innerHTML + '</h4>';
				tdField.innerHTML = '';
			}
			else
			{
				tdField.innerHTML = 'Unsupported field type: ' + sFIELD_TYPE;
			}
			nColumn++;
		}
		// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sFORM_SCRIPT = lay.SCRIPT;
			if ( !Sql.IsEmptyString(sFORM_SCRIPT) )
			{
				sFORM_SCRIPT = sFORM_SCRIPT.replace("SPLENDID_DETAILVIEW_LAYOUT_ID", sLayoutPanel + '_ctlDetailView');
				eval(sFORM_SCRIPT);
			}
			break;
		}
	}
	catch(e)
	{
		// 10/08/2012 Paul.  callback is not available here. 
		SplendidError.SystemAlert(e, 'DetailViewUI.LoadView');
	}
}

DetailViewUI.prototype.LoadItem = function(sLayoutPanel, sActionsPanel, layout, sDETAIL_NAME, sMODULE_NAME, sID, callback)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.DetailView_LoadItem(sMODULE_NAME, sID, function(status, message)
		{
			if ( status == 1 )
			{
				// 10/04/2011 Paul.  DetailViewUI.LoadItem returns the row. 
				var row = message;
				DynamicButtonsUI_Clear(sActionsPanel);
				this.Clear(sLayoutPanel);
				// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
				ctlActiveMenu.ActivateTab(sMODULE_NAME, sID, 'DetailView', this);
				// 01/30/2013 Paul.  Clicking on the sub-title will refresh the view as a way to correct bugs in the rendering. 
				SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, Sql.ToString(row['NAME']), Sql.ToString(row['ID']));
				DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sDETAIL_NAME, row, this.PageCommand, function(status, message)
				{
					if ( status != 1 )
						callback(status, message);
				}, this);
				
				//var layout = bgPage.SplendidCache.DetailViewFields(sDETAIL_NAME);
				var tblMain = document.getElementById(sLayoutPanel + '_ctlDetailView_tblMain');
				this.LoadView(sLayoutPanel, sActionsPanel, tblMain, layout, row);
				
				var oDetailViewRelationshipsUI = new DetailViewRelationshipsUI();
				oDetailViewRelationshipsUI.Load(sLayoutPanel, sActionsPanel, sDETAIL_NAME, row, this.PageCommand, function(status, message)
				{
					// 01/30/2013 Paul.  We need the callback event here to continue the Load operation. 
					callback(status, message);
				});
			}
			else
			{
				callback(status, message);
			}
		}, this);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'DetailViewUI.LoadItem'));
	}
}

DetailViewUI.prototype.LoadObject = function(sLayoutPanel, sActionsPanel, sDETAIL_NAME, sMODULE_NAME, row, PageCommand, callback)
{
	try
	{
		this.MODULE  = sMODULE_NAME;
		this.ID      = null;
		
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
		{
			if ( status == 0 || status == 1 )
			{
				bgPage.DetailView_LoadLayout(sDETAIL_NAME, function(status, message)
				{
					if ( status == 1 )
					{
						// 10/03/2011 Paul.  DetailView_LoadLayout returns the layout. 
						var layout = message;
						if ( row != null )
						{
							DynamicButtonsUI_Clear(sActionsPanel);
							this.Clear(sLayoutPanel);
							
							if ( PageCommand == null )
								PageCommand = this.PageCommand;
							DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sDETAIL_NAME, row, PageCommand, function(status, message)
							{
							}, this);
							
							//var layout = bgPage.SplendidCache.DetailViewFields(sDETAIL_NAME);
							var tblMain = document.getElementById(sLayoutPanel + '_ctlDetailView_tblMain');
							this.LoadView(sLayoutPanel, sActionsPanel, tblMain, layout, row);
							
							callback(status, message);
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
		callback(-1, SplendidError.FormatError(e, 'DetailViewUI.LoadObject'));
	}
}

DetailViewUI.prototype.Parent = function(sID, callback, context)
{
	var sTABLE_NAME     = 'vwPARENTS';
	var sSORT_FIELD     = 'PARENT_ID';
	var sSORT_DIRECTION = '';
	var sSELECT         = 'PARENT_ID, PARENT_NAME, PARENT_TYPE, PARENT_ASSIGNED_USER_ID';
	var sFILTER         = 'PARENT_ID eq \'' + sID + '\'';
	var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=' + sTABLE_NAME + '&$orderby=' + encodeURIComponent(sSORT_FIELD + ' ' + sSORT_DIRECTION) + '&$select=' + escape(sSELECT) + '&$filter=' + escape(sFILTER), 'GET');
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
							if ( result.d.results.length > 0 )
								callback.call(context||this, 1, result.d.results[0]);
							else
								callback.call(context||this, -1, 'Item not found for ID = ' + sID);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewUI.Parent'));
				}
			}, context||this);
		}
	}
	try
	{
		xhr.send();
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewUI.Parent'));
	}
}

// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
DetailViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, callback, context)
{
	try
	{
		this.MODULE  = null;
		this.ID      = null;
		
		var bgPage = chrome.extension.getBackgroundPage();
		// 11/29/2011 Paul.  We are having an issue with the globals getting reset, so we need to re-initialize. 
		if ( !bgPage.SplendidCache.IsInitialized() )
		{
			SplendidUI_ReInit(sLayoutPanel, sActionsPanel, sMODULE_NAME);
			return;
		}
		// 01/19/2013 Paul.  A Parents module requires a lookup to get the module name. 
		if ( sMODULE_NAME == 'Parents' )
		{
			this.Parent(sID, function(status, message)
			{
				if ( status == 1 && message !== undefined && message != null )
				{
					var row = message;
					sMODULE_NAME = row['PARENT_TYPE'];
					this.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID);
				}
				else
				{
					SplendidError.SystemMessage(message);
				}
			}, this);
			// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
			callback.call(context||this, 0, null);
			return;
		}
		
		this.MODULE  = sMODULE_NAME;
		this.ID      = sID;
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						var sDETAIL_NAME = sMODULE_NAME + '.DetailView' + sPLATFORM_LAYOUT;
						bgPage.DetailView_LoadLayout(sDETAIL_NAME, function(status, message)
						{
							if ( status == 1 )
							{
								SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, '');
								// 10/03/2011 Paul.  DetailView_LoadLayout returns the layout. 
								var layout = message;
								if ( sID != null )
								{
									this.LoadItem(sLayoutPanel, sActionsPanel, layout, sDETAIL_NAME, sMODULE_NAME, sID, function(status, message)
									{
										// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
										callback.call(context||this, status, message);
									});
								}
							}
							else
							{
								// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
								callback.call(context||this, status, message);
							}
						}, this);
					}
					else
					{
						// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
						callback.call(context||this, status, message);
					}
				}, this);
			}
			else
			{
				// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
				callback.call(context||this, status, message);
			}
		}, this);
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		// 01/30/2013 Paul.  We need to be able to execute code after loading a DetailView. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewUI.Load'));
	}
}


