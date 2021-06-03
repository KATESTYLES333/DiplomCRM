/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function PopupViewUI()
{
	this.SORT_FIELD     = 'NAME';
	this.SORT_DIRECTION = 'asc';
	this.MODULE_NAME    = '';
	this.GRID_NAME      = '';
	this.SEARCH_FILTER  = '';
	this.SEARCH_VALUES  = null;
	this.MULTI_SELECT   = false;
	this.OnMainClicked  = null;
}

PopupViewUI.prototype.Clear = function(sLayoutPanel, sMODULE_NAME)
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
			// 01/18/2015 Paul.  We noticed "divPopupLayoutPanel does not exist" when cancelling related selection on ChatDashboard on Android 4.4.2 phone. 
			//alert('PopupViewUI.Clear: ' + sLayoutPanel + ' does not exist');
			return;
		}
		SplendidUI_ListHeader(sLayoutPanel, sMODULE_NAME + '.LBL_LIST_FORM_TITLE');
		
		var ctlPopupViewButtons = document.createElement('div');
		ctlPopupViewButtons.id = sLayoutPanel + '_ctlPopupView';
		divMainLayoutPanel.appendChild(ctlPopupViewButtons);
		
		var btnClear       = document.createElement('input');
		btnClear.id        = sLayoutPanel + '_ctlPopupView_btnClear';
		btnClear.type      = 'button';
		btnClear.className = 'button';
		btnClear.title     = L10n.Term('.LBL_CLEAR_BUTTON_TITLE');
		btnClear.value     = L10n.Term('.LBL_CLEAR_BUTTON_LABEL');
		btnClear.style.paddingLeft  = '10px';
		btnClear.style.paddingRight = '10px';
		btnClear.style.marginRight  = '2px';
		btnClear.style.marginBottom = '4px';
		btnClear.onclick = BindArguments(this.OnMainClicked, 1, { 'ID': '', 'NAME': '' } );
		ctlPopupViewButtons.appendChild(btnClear);
		
		var btnCancel = document.createElement('input');
		btnCancel.id        = sLayoutPanel + '_ctlPopupView_btnCancel';
		btnCancel.type      = 'button';
		btnCancel.className = 'button';
		btnCancel.title     = L10n.Term('.LBL_CANCEL_BUTTON_TITLE');
		btnCancel.value     = L10n.Term('.LBL_CANCEL_BUTTON_LABEL');
		btnCancel.style.paddingLeft  = '10px';
		btnCancel.style.paddingRight = '10px';
		btnCancel.style.marginLeft   = '2px';
		btnCancel.style.marginBottom = '4px';
		btnCancel.onclick = BindArguments(this.OnMainClicked, -2, null );
		ctlPopupViewButtons.appendChild(btnCancel);
		
		// <table id="ctlListView_grdMain" class="listView" cellspacing="1" cellpadding="3" rules="all" border="0" border="1" width="100%">
		var ctlListView_grdMain = document.createElement('table');
		ctlListView_grdMain.id        = sLayoutPanel + '_ctlListView_grdMain';
		ctlListView_grdMain.width     = '100%';
		ctlListView_grdMain.className = 'listView';
		divMainLayoutPanel.appendChild(ctlListView_grdMain);
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'PopupViewUI.Clear'));
	}
}

PopupViewUI.prototype.Reset = function(sLayoutPanel, sMODULE_NAME)
{
	try
	{
		this.SORT_FIELD     = 'NAME';
		this.SORT_DIRECTION = 'asc' ;
		this.SEARCH_FILTER  = ''    ;
		this.SEARCH_VALUES  = null  ;
		this.Clear(sLayoutPanel, sMODULE_NAME);
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'PopupViewUI.Reset'));
	}
}

PopupViewUI.prototype.Sort = function(sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION)
{
	try
	{
		this.SORT_FIELD     = sFIELD_NAME;
		this.SORT_DIRECTION = sDIRECTION;
		SplendidError.SystemMessage('Sorting ' + sFIELD_NAME + ' ' + sDIRECTION);
		var bgPage = chrome.extension.getBackgroundPage();
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
				this.LoadModule(sLayoutPanel, sActionsPanel, this.MODULE_NAME, this.GRID_NAME, this.SEARCH_FILTER, this.SEARCH_VALUES, function(status, message)
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
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'PopupViewUI.Sort'));
	}
}

// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
PopupViewUI.prototype.Search = function(sLayoutPanel, sActionsPanel, sSEARCH_FILTER, rowSEARCH_VALUES)
{
	try
	{
		//alert('PopupViewUI.Search ' + dumpObj(arrData, 'arrData'));
		SplendidError.SystemMessage('Searching ');
		this.SEARCH_FILTER = sSEARCH_FILTER  ;
		this.SEARCH_VALUES = rowSEARCH_VALUES;
		//alert('PopupViewUI.Search ' + this.SEARCH_FILTER);
		var bgPage = chrome.extension.getBackgroundPage();
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
				this.LoadModule(sLayoutPanel, sActionsPanel, this.MODULE_NAME, this.GRID_NAME, this.SEARCH_FILTER, this.SEARCH_VALUES, function(status, message)
				{
					if ( status == 1 )
					{
						//SplendidError.SystemMessage(this.SEARCH_FILTER);
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
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'PopupViewUI.Search'));
	}
}

PopupViewUI.prototype.CheckAll = function(chkMainCheckAll, sFieldID)
{
	try
	{
		var fld = document.getElementsByName(sFieldID);
		for (var i = 0; i < fld.length; i++)
		{
			if ( fld[i].type == 'checkbox' )
			{
				fld[i].checked = chkMainCheckAll.checked;
				if( fld[i].onclick != null )
				{
					fld[i].onclick();
				}
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'PopupViewUI.CheckAll');
	}
}

PopupViewUI.prototype.RenderHeader = function(sLayoutPanel, sActionsPanel, layout, tbody)
{
	// <tr class="listViewThS1">
	var tr = document.createElement('tr');
	tbody.appendChild(tr);
	tr.className = 'listViewThS1';
	if ( layout.length > 0 )
	{
		// 09/01/2011 Paul.  First column will be for actions. 
		var td = document.createElement('td');
		tr.appendChild(td);
		if ( this.MULTI_SELECT )
		{
			var chkMainCheckAll = document.createElement('input');
			chkMainCheckAll.id        = 'chkMainCheckAll';
			chkMainCheckAll.name      = 'chkMainCheckAll';
			chkMainCheckAll.type      = 'checkbox';
			chkMainCheckAll.className = 'checkbox';
			chkMainCheckAll.onclick   = BindArguments(this.CheckAll, chkMainCheckAll, 'chkMain');
			chkMainCheckAll.style.padding       = '2px';
			chkMainCheckAll.style.verticalAlign = 'middle';
			td.appendChild(chkMainCheckAll);
		}
		var bEnableTeamManagement = Crm.Config.enable_team_management();
		var bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			//alert(dumpObj(lay, 'lay'));
			var sCOLUMN_TYPE                = lay.COLUMN_TYPE               ;
			var sHEADER_TEXT                = lay.HEADER_TEXT               ;
			var sSORT_EXPRESSION            = lay.SORT_EXPRESSION           ;
			var sITEMSTYLE_WIDTH            = lay.ITEMSTYLE_WIDTH           ;
			var sITEMSTYLE_CSSCLASS         = lay.ITEMSTYLE_CSSCLASS        ;
			var sITEMSTYLE_HORIZONTAL_ALIGN = lay.ITEMSTYLE_HORIZONTAL_ALIGN;
			var sITEMSTYLE_VERTICAL_ALIGN   = lay.ITEMSTYLE_VERTICAL_ALIGN  ;
			var sITEMSTYLE_WRAP             = lay.ITEMSTYLE_WRAP            ;
			var sDATA_FIELD                 = lay.DATA_FIELD                ;
			var sDATA_FORMAT                = lay.DATA_FORMAT               ;
			var sURL_FIELD                  = lay.URL_FIELD                 ;
			var sURL_FORMAT                 = lay.URL_FORMAT                ;
			var sURL_TARGET                 = lay.URL_TARGET                ;
			var sLIST_NAME                  = lay.LIST_NAME                 ;
			var sURL_MODULE                 = lay.URL_MODULE                ;
			var sURL_ASSIGNED_FIELD         = lay.URL_ASSIGNED_FIELD        ;
			var sVIEW_NAME                  = lay.VIEW_NAME                 ;
			var sMODULE_NAME                = lay.MODULE_NAME               ;
			var sMODULE_TYPE                = lay.MODULE_TYPE               ;
			var sPARENT_FIELD               = lay.PARENT_FIELD              ;
			
			td = document.createElement('td');
			tr.appendChild(td);
			if ( (sDATA_FIELD == 'TEAM_NAME' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( bEnableTeamManagement && bEnableDynamicTeams )
				{
					sHEADER_TEXT = '.LBL_LIST_TEAM_SET_NAME';
					sDATA_FIELD  = 'TEAM_SET_NAME';
				}
				else if ( !bEnableTeamManagement )
				{
					td.style.display = 'none';
					// 10/24/2012 Paul.  Clear the sort so that there would be no term lookup. 
					sHEADER_TEXT     = null;
					sSORT_EXPRESSION = null;
				}
			}
			
			if ( sSORT_EXPRESSION != null )
			{
				var a = document.createElement('a');
				td.appendChild(a);
				a.innerHTML = '<nobr>' + L10n.Term(sHEADER_TEXT) + '</nobr>';
				var img = document.createElement('img');
				td.appendChild(img);
				img.align             = 'absmiddle';
				img.style.height      = '10px';
				img.style.width       = '8px';
				img.style.borderWidth = '0px';
				if ( sSORT_EXPRESSION == this.SORT_FIELD )
				{
					// img src="../App_Themes/Six/images/arrow_up.gif" align="absmiddle" style="border-width:0px;height:10px;width:8px;" />
					if ( this.SORT_DIRECTION == 'asc' )
					{
						img.src = sIMAGE_SERVER + 'App_Themes/Six/images/arrow_up.gif';
						a.href = '#';
						a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION, context)
						{
							context.Sort.call(context, sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION);
						}, sLayoutPanel, sActionsPanel, sSORT_EXPRESSION, 'desc', this);
					}
					else
					{
						img.src = sIMAGE_SERVER + 'App_Themes/Six/images/arrow_down.gif';
						a.href = '#';
						a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION, context)
						{
							context.Sort.call(context, sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION);
						}, sLayoutPanel, sActionsPanel, sSORT_EXPRESSION, 'asc', this);
					}
				}
				else
				{
					// img src="../App_Themes/Six/images/arrow.gif" align="absmiddle" style="border-width:0px;height:10px;width:8px;" />
					img.src = sIMAGE_SERVER + 'App_Themes/Six/images/arrow.gif';
					a.href = '#';
					a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION, context)
					{
						context.Sort.call(context, sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION);
					}, sLayoutPanel, sActionsPanel, sSORT_EXPRESSION, 'asc', this);
				}
			}
			else if ( sHEADER_TEXT != null )
			{
				var txt = document.createTextNode(L10n.Term(sHEADER_TEXT));
				td.appendChild(txt);
			}
		}
	}
}

PopupViewUI.prototype.RenderRow = function(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tr, row)
{
	if ( layout.length > 0 )
	{
		// 09/01/2011 Paul.  First column will be for actions. 
		var td = document.createElement('td');
		tr.appendChild(td);
		td.style.whiteSpace = 'nowrap';

		if ( this.MULTI_SELECT )
		{
			var chkMain = document.createElement('input');
			chkMain.id        = 'chkMain_' + Sql.ToString(row['ID']).replace('-', '_');
			chkMain.name      = 'chkMain';
			chkMain.type      = 'checkbox';
			chkMain.className = 'checkbox';
			chkMain.Module    = sLIST_MODULE_NAME;
			chkMain.value     = row['ID'  ];
			chkMain.tooltip   = row['NAME'];
			//chkMain.onclick   = BindArguments(this.OnMainClicked, chkMain, sLIST_MODULE_NAME, row['ID'], row['NAME']);
			chkMain.style.padding       = '2px';
			chkMain.style.verticalAlign = 'middle';
			// 09/25/2011 Paul.  IE does not allow you to set the type after it is added to the document. 
			td.appendChild(chkMain);
			// 10/04/2011 Paul.  IE8 requires that we set checked after appending. 
			if ( SelectionUI_IsSelected(row['ID']) )
				chkMain.checked = true;
		}

		var bEnableTeamManagement = Crm.Config.enable_team_management();
		var bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			//alert(dumpObj(lay, 'lay'));
			var sCOLUMN_TYPE                = lay.COLUMN_TYPE               ;
			var sHEADER_TEXT                = lay.HEADER_TEXT               ;
			var sSORT_EXPRESSION            = lay.SORT_EXPRESSION           ;
			var sITEMSTYLE_WIDTH            = lay.ITEMSTYLE_WIDTH           ;
			var sITEMSTYLE_CSSCLASS         = lay.ITEMSTYLE_CSSCLASS        ;
			var sITEMSTYLE_HORIZONTAL_ALIGN = lay.ITEMSTYLE_HORIZONTAL_ALIGN;
			var sITEMSTYLE_VERTICAL_ALIGN   = lay.ITEMSTYLE_VERTICAL_ALIGN  ;
			var sITEMSTYLE_WRAP             = lay.ITEMSTYLE_WRAP            ;
			var sDATA_FIELD                 = lay.DATA_FIELD                ;
			var sDATA_FORMAT                = lay.DATA_FORMAT               ;
			var sURL_FIELD                  = lay.URL_FIELD                 ;
			var sURL_FORMAT                 = lay.URL_FORMAT                ;
			var sURL_TARGET                 = lay.URL_TARGET                ;
			var sLIST_NAME                  = lay.LIST_NAME                 ;
			var sURL_MODULE                 = lay.URL_MODULE                ;
			var sURL_ASSIGNED_FIELD         = lay.URL_ASSIGNED_FIELD        ;
			var sVIEW_NAME                  = lay.VIEW_NAME                 ;
			var sMODULE_NAME                = lay.MODULE_NAME               ;
			var sMODULE_TYPE                = lay.MODULE_TYPE               ;
			var sPARENT_FIELD               = lay.PARENT_FIELD              ;
			
			td = document.createElement('td');
			tr.appendChild(td);
			if ( (sDATA_FIELD == 'TEAM_NAME' || sDATA_FIELD == 'TEAM_SET_NAME') )
			{
				if ( bEnableTeamManagement && bEnableDynamicTeams )
				{
					sHEADER_TEXT = '.LBL_LIST_TEAM_SET_NAME';
					sDATA_FIELD  = 'TEAM_SET_NAME';
				}
				else if ( !bEnableTeamManagement )
				{
					td.style.display = 'none';
					// 10/24/2012 Paul.  Clear the sort so that there would be no term lookup. 
					sHEADER_TEXT     = null;
					sSORT_EXPRESSION = null;
				}
			}
			
			if ( sITEMSTYLE_WIDTH            != null ) td.width     = sITEMSTYLE_WIDTH           ;
			if ( sITEMSTYLE_CSSCLASS         != null ) td.className = sITEMSTYLE_CSSCLASS        ;
			if ( sITEMSTYLE_HORIZONTAL_ALIGN != null ) td.align     = sITEMSTYLE_HORIZONTAL_ALIGN;
			if ( sITEMSTYLE_VERTICAL_ALIGN   != null ) td.vAlign    = sITEMSTYLE_VERTICAL_ALIGN  ;
			
			if (   sCOLUMN_TYPE == 'BoundColumn' 
			    && (   sDATA_FORMAT == 'Date'
			        || sDATA_FORMAT == 'DateTime'
			        || sDATA_FORMAT == 'Currency'
			        || sDATA_FORMAT == 'Image'
			        || sDATA_FORMAT == 'MultiLine'
			       )
			   )
			{
				sCOLUMN_TYPE = 'TemplateColumn';
			}
			if ( sCOLUMN_TYPE == 'TemplateColumn' )
			{
				//alert(sDATA_FORMAT + ' ' + row[sDATA_FIELD]);
				if ( row[sDATA_FIELD] != null )
				{
					if ( sDATA_FORMAT == 'HyperLink' )
					{
						var a = document.createElement('a');
						td.appendChild(a);
						a.href      = '#';
						a.innerHTML = row[sDATA_FIELD];
						if ( this.OnMainClicked != null )
						{
							a.onclick = BindArguments(this.OnMainClicked, 1, { 'ID': row['ID'], 'NAME': row[sDATA_FIELD] } );
						}
					}
					else if ( sDATA_FORMAT == 'Date' )
					{
						var sDATA_VALUE = row[sDATA_FIELD];
						sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT());
						td.innerHTML = sDATA_VALUE;
					}
					else if ( sDATA_FORMAT == 'DateTime' )
					{
						var sDATA_VALUE = row[sDATA_FIELD];
						sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());
						td.innerHTML = sDATA_VALUE;
					}
					else if ( sDATA_FORMAT == 'Currency' )
					{
						var sDATA_VALUE = row[sDATA_FIELD];
						td.innerHTML = sDATA_VALUE;
					}
					else if ( sDATA_FORMAT == 'MultiLine' )
					{
						var sDATA_VALUE = row[sDATA_FIELD];
						td.innerHTML = sDATA_VALUE;
					}
					else if ( sDATA_FORMAT == 'Image' )
					{
					}
					else if ( sDATA_FORMAT == 'JavaScript' )
					{
					}
					else if ( sDATA_FORMAT == 'Hover' )
					{
					}
				}
			}
			else if ( sCOLUMN_TYPE == 'BoundColumn' )
			{
				if ( row[sDATA_FIELD] != null )
				{
					if ( sLIST_NAME != null )
					{
						// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
						var sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]);
						td.innerHTML = sDATA_VALUE;
					}
					else
					{
						var sDATA_VALUE = row[sDATA_FIELD];
						td.innerHTML = sDATA_VALUE;
					}
				}
			}
		}
	}
}

PopupViewUI.prototype.GridColumns = function(layout)
{
	var arrSelectFields = new Array();
	if ( layout.length > 0 )
	{
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sSORT_EXPRESSION            = lay.SORT_EXPRESSION           ;
			var sDATA_FIELD                 = lay.DATA_FIELD                ;
			var sDATA_FORMAT                = lay.DATA_FORMAT               ;
			var sURL_FIELD                  = lay.URL_FIELD                 ;
			var sURL_ASSIGNED_FIELD         = lay.URL_ASSIGNED_FIELD        ;
			var sPARENT_FIELD               = lay.PARENT_FIELD              ;
			
			if ( sDATA_FORMAT == 'Hover' )
				continue;
			if ( sDATA_FIELD != null && sDATA_FIELD.length > 0 )
			{
				arrSelectFields.push(sDATA_FIELD);
			}
			if ( sSORT_EXPRESSION != null && sSORT_EXPRESSION.length > 0 )
			{
				if ( sDATA_FIELD != sSORT_EXPRESSION )
					arrSelectFields.push(sSORT_EXPRESSION);
			}
			if ( sURL_FIELD != null && sURL_FIELD.length > 0 )
			{
				if ( sURL_FIELD.indexOf(' ') >= 0 )
				{
					var arrURL_FIELD = sURL_FIELD.split(' ');
					for ( var i in arrURL_FIELD )
					{
						var s = arrURL_FIELD[i];
						if ( s.indexOf('.') == -1 && s.length > 0 )
						{
							arrSelectFields.push(s);
						}
					}
				}
				else if ( sURL_FIELD.indexOf('.') == -1 )
				{
					arrSelectFields.push(sURL_FIELD);
				}
				if ( sURL_ASSIGNED_FIELD != null && sURL_ASSIGNED_FIELD.length > 0 )
				{
					arrSelectFields.push(sURL_ASSIGNED_FIELD);
				}
			}
			if ( sPARENT_FIELD != null && sPARENT_FIELD.length > 0 )
			{
				arrSelectFields.push(sPARENT_FIELD);
			}
		}
	}
	return arrSelectFields.join(',');
}

// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
PopupViewUI.prototype.LoadModule = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, sSEARCH_FILTER, rowSEARCH_VALUES, callback)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		//var layout = bgPage.SplendidCache.GridViewColumns(sGRID_NAME);
		bgPage.ListView_LoadLayout(sGRID_NAME, function(status, message)
		{
			if ( status == 1 )
			{
				// 10/03/2011 Paul. ListView_LoadLayout returns the layout. 
				var layout = message;
				var sSELECT_FIELDS = this.GridColumns(layout);
				// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
				bgPage.ListView_LoadModule(sMODULE_NAME, this.SORT_FIELD, this.SORT_DIRECTION, sSELECT_FIELDS, sSEARCH_FILTER, rowSEARCH_VALUES, function(status, message)
				{
					// 10/04/2011 Paul.  ListView_LoadModule returns the row. 
					// 10/21/2012 Paul.  Always display the ListView header. 
					this.Clear(sLayoutPanel, sMODULE_NAME);
					var ctlListView_grdMain = document.getElementById(sLayoutPanel + '_ctlListView_grdMain');
					// 10/17/2012 Paul.  Exit if the Main does not exist.  This is a sign that the user has navigated elsewhere. 
					if ( ctlListView_grdMain == null )
						return;
					var tbody = document.createElement('tbody');
					ctlListView_grdMain.appendChild(tbody);
					
					this.RenderHeader(sLayoutPanel, sActionsPanel, layout, tbody);
					
					if ( status == 1 )
					{
						var rows = message;
						for ( var i = 0; i < rows.length; i++ )
						{
							var tr = document.createElement('tr');
							tbody.appendChild(tr);
							if ( i % 2 == 0 )
								tr.className = 'oddListRowS1';
							else
								tr.className = 'evenListRowS1';
							
							var row = rows[i];
							this.RenderRow(sLayoutPanel, sActionsPanel, sMODULE_NAME, layout, tr, row);
						}
						callback(1, null);
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
		callback(-1, SplendidError.FormatError(e, 'PopupViewUI.LoadModule'));
	}
}

PopupViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, bMultiSelect, callback)
{
	try
	{
		var sGRID_NAME = sMODULE_NAME + '.PopupView' + sPLATFORM_LAYOUT;
		this.MODULE_NAME   = sMODULE_NAME;
		this.GRID_NAME     = sGRID_NAME  ;
		this.SEARCH_FILTER = ''          ;
		this.SEARCH_VALUES = null        ;
		this.MULTI_SELECT  = bMultiSelect;
		this.OnMainClicked = callback    ;

		var bgPage = chrome.extension.getBackgroundPage();
		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						// 09/10/2011 Paul.  Make sure to load the layout first as it might be needed inside SearchViewUI_SearchForm, or PopupViewUI.LoadModule, which run in parallel. 
						bgPage.ListView_LoadLayout(sGRID_NAME, function(status, message)
						{
							if ( status == 0 || status == 1 )
							{
								// 10/03/2011 Paul. ListView_LoadLayout returns the layout. 
								var layout = message;
								var rowDefaultSearch = null;
								SearchViewUI_Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sMODULE_NAME + '.SearchPopup' + sPLATFORM_LAYOUT, rowDefaultSearch, false, this.Search, function(status, message)
								{
									if ( status == -1 )
										callback(status, message);
								}, this);
								// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
								this.LoadModule(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, this.SEARCH_FILTER, this.SEARCH_VALUES, function(status, message)
								{
									if ( status == -1 )
										callback(status, message);
								});
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
			else
			{
				callback(-1, message);
			}
		}, this);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'PopupViewUI.Load'));
	}
}


