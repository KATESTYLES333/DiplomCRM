/* Copyright (C) 2011-2014 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function ToggleUnassignedOnly()
{
	var sLayoutPanel = 'divMainLayoutPanel';
	var sASSIGNED_USER_ID = sLayoutPanel + '_ctlEditView_' + 'ASSIGNED_USER_ID';
	var sUNASSIGNED_ONLY  = sLayoutPanel + '_ctlEditView_' + 'UNASSIGNED_ONLY' ;
	if ( sASSIGNED_USER_ID.length > 0 && sUNASSIGNED_ONLY.length > 0 )
	{
		var lstASSIGNED_USER_ID = document.getElementById(sASSIGNED_USER_ID);
		var chkUNASSIGNED_ONLY  = document.getElementById(sUNASSIGNED_ONLY );
		if ( lstASSIGNED_USER_ID != null && chkUNASSIGNED_ONLY != null )
			lstASSIGNED_USER_ID.disabled = chkUNASSIGNED_ONLY.checked;
	}
}

function SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context)
{
	try
	{
		var row = new Object();
		var bgPage = chrome.extension.getBackgroundPage();
		var oEditViewUI = new EditViewUI();
		oEditViewUI.GetValues(sActionsPanel, sEDIT_NAME, true, row);
		//alert(dumpObj(row, 'SearchViewUI_SearchForm row'));
		var cmd = new Object();
		cmd.CommandText = '';
		if ( row != null )
		{
			var oSearchBuilder = new SearchBuilder();
			for ( var sField in row )
			{
				//alert(sField + ' = ' + row[sField]);
				var oValue = row[sField];
				if ( sField.indexOf(' ') > 0 )
				{
					// 12/05/2011 Paul.  If value is empty, then ignore to prevent search of (0 = 1). 
					if ( !Sql.IsEmptyString(oValue) )
					{
						//alert('multiple fields ' + sField)
						oSearchBuilder.Init(oValue);
						var arrFields = sField.split(' ');
						if ( cmd.CommandText.length > 0 )
							cmd.CommandText += ' and ';
						cmd.CommandText += '(0 = 1';
						for ( var n in arrFields )
						{
							if ( typeof(oValue) == 'string' )
								cmd.CommandText += oSearchBuilder.BuildQuery(' or ', arrFields[n]);
							else
								Sql.AppendParameter(cmd, arrFields[n], oValue, true);
						}
						cmd.CommandText += ')';
						//alert(cmd.CommandText);
					}
				}
				else
				{
					if ( typeof(oValue) == 'string' )
					{
						if ( cmd.CommandText.length == 0 )
							cmd.CommandText += oSearchBuilder.BuildQuery('', sField, oValue);
						else
							cmd.CommandText += oSearchBuilder.BuildQuery(' and ', sField, oValue);
					}
					else if ( typeof(oValue) == 'boolean' )
					{
						// 02/03/2013 Paul.  Only set the boolean value if checked. 
						// 02/22/2013 Paul.  Only if checked applies to any checkbox field. 
						if ( oValue )
						{
							if ( sField == 'UNASSIGNED_ONLY' )
							{
								Sql.AppendParameter(cmd, 'ASSIGNED_USER_ID', null);
							}
							else if ( sField == 'CURRENT_USER_ONLY' )
							{
								Sql.AppendParameter(cmd, 'ASSIGNED_USER_ID', Security.USER_ID());
							}
							else
							{
								Sql.AppendParameter(cmd, sField, oValue);
							}
						}
					}
					else
					{
						Sql.AppendParameter(cmd, sField, oValue);
					}
				}
			}
		}
		//alert(cmd.CommandText);
		// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
		if ( cbSearch !== undefined && cbSearch != null )
			cbSearch.call(context, sLayoutPanel, sActionsPanel, cmd.CommandText, row);
		else
			SplendidError.SystemError('SearchViewUI_SearchForm: cbSearch is not defined');
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'SearchViewUI_SearchForm');
	}
}

function SearchViewUI_ClearForm(sLayoutPanel, sActionsPanel, sEDIT_NAME)
{
	try
	{
		var oEditViewUI = new EditViewUI();
		oEditViewUI.ClearValues(sActionsPanel, sEDIT_NAME);
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'SearchViewUI_ClearForm');
	}
}

function SearchViewUI_Clear(sLayoutPanel, sActionsPanel, sMODULE_NAME, sEDIT_NAME, bEnableCaching, cbSearch, callback, context)
{
	try
	{
		var divMainLayoutPanel = document.getElementById(sActionsPanel);
		//alert('SearchViewUI_Clear(' + sActionsPanel + ')' + divMainLayoutPanel);
		
		var ctlSearchView_tblSearch = document.getElementById(sActionsPanel + '_ctlSearchView_tblSearch');
		if ( ctlSearchView_tblSearch == null )
		{
			if ( divMainLayoutPanel != null && divMainLayoutPanel.childNodes != null )
			{
				while ( divMainLayoutPanel.childNodes.length > 0 )
				{
					divMainLayoutPanel.removeChild(divMainLayoutPanel.firstChild);
				}
			}
			// <table class="tabSearchForm" cellspacing="1" cellpadding="0" border="0" style="width:100%;">
			// <table id="ctlListView_ctlSearchView_tblSearch" class="tabSearchView">
			var ctlSearchView = document.createElement('table');
			ctlSearchView.id        = sActionsPanel + '_ctlSearchView';
			ctlSearchView.cellSpacing = 1;
			ctlSearchView.cellPadding = 0;
			ctlSearchView.border      = 0;
			ctlSearchView.style.width = '100%';
			ctlSearchView.className   = 'tabSearchForm';
			var a = document.createElement('a');
			if ( divMainLayoutPanel != null )
			{
				if ( divMainLayoutPanel.childNodes != null && divMainLayoutPanel.childNodes.length > 0 )
					divMainLayoutPanel.insertBefore(ctlSearchView, divMainLayoutPanel.firstChild);
				else
					divMainLayoutPanel.appendChild(ctlSearchView);
			}
			else
			{
				alert('SearchViewUI_Clear: ' + sActionsPanel + ' does not exist');
			}
			
			var tSearchView = document.createElement('tbody');
			ctlSearchView.appendChild(tSearchView);
			var tr = document.createElement('tr');
			tSearchView.appendChild(tr);
			var td = document.createElement('td');
			tr.appendChild(td);
			
			ctlSearchView_tblSearch = document.createElement('table');
			ctlSearchView_tblSearch.id        = sActionsPanel + '_ctlSearchView_tblSearch';
			ctlSearchView_tblSearch.className = 'tabSearchView';
			td.appendChild(ctlSearchView_tblSearch);
			
			var tblSearchButtons = document.createElement('table');
			tblSearchButtons.id               = sActionsPanel + '_ctlSearchView_tblSearchButtons';
			tblSearchButtons.cellSpacing      = 0;
			tblSearchButtons.cellPadding      = 0;
			tblSearchButtons.border           = 0;
			tblSearchButtons.width            = '100%';
			tblSearchButtons.style.paddingTop = '4px';
			td.appendChild(tblSearchButtons);
			
			var tSearchButtons = document.createElement('tbody');
			tblSearchButtons.appendChild(tSearchButtons);
			tr = document.createElement('tr');
			tSearchButtons.appendChild(tr);
			td = document.createElement('td');
			tr.appendChild(td);
			
			// <input type="submit" name="ctl00$cntBody$ctlListView$ctlSearchView$btnSearch" value="Search" 
			// onclick="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;ctl00$cntBody$ctlListView$ctlSearchView$btnSearch&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false))" 
			// id="ctl00_cntBody_ctlListView_ctlSearchView_btnSearch" accesskey="Q" title="Search" class="button">
			
			var btnSearch = document.createElement('input');
			btnSearch.type      = 'submit';
			btnSearch.id        = sActionsPanel + '_ctlSearchView_btnSearch';
			btnSearch.value     = L10n.Term('.LBL_SEARCH_BUTTON_LABEL');
			btnSearch.title     = L10n.Term('.LBL_SEARCH_BUTTON_LABEL');
			btnSearch.className = 'button';
			btnSearch.style.marginRight = '3px';
			btnSearch.onclick   = BindArguments(function(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context)
			{
				SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			}, sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			td.appendChild(btnSearch);
			
			var btnClear = document.createElement('input');
			btnClear.type      = 'submit';
			btnClear.id        = sActionsPanel + '_ctlSearchView_btnClear';
			btnClear.value     = L10n.Term('.LBL_CLEAR_BUTTON_LABEL');
			btnClear.title     = L10n.Term('.LBL_CLEAR_BUTTON_LABEL');
			btnClear.className = 'button';
			btnClear.style.marginRight = '3px';
			btnClear.onclick   = BindArguments(function(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context)
			{
				SearchViewUI_ClearForm (sLayoutPanel, sActionsPanel, sEDIT_NAME);
				SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			}, sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			td.appendChild(btnClear);
			
			if ( bEnableCaching )
			{
				var bgPage = chrome.extension.getBackgroundPage();
				var btnCacheSelected = document.createElement('input');
				btnCacheSelected.type      = 'submit';
				btnCacheSelected.id        = sActionsPanel + '_ctlSearchView_btnCacheSelected';
				btnCacheSelected.value     = L10n.Term('.LBL_CACHE_SELECTED');
				btnCacheSelected.title     = L10n.Term('.LBL_CACHE_SELECTED');
				btnCacheSelected.className = 'button';
				btnCacheSelected.style.marginRight = '3px';
				btnCacheSelected.style.display     = (bgPage.GetEnableOffline() && !bgPage.GetIsOffline()) ? 'inline' : 'none';
				btnCacheSelected.onclick   = BindArguments(function(sFieldID, sMODULE_NAME, callback, context)
				{
					SearchViewUI_CacheSelected(sFieldID, sMODULE_NAME, callback, context);
				}, 'chkMain', sMODULE_NAME, callback, context);
				td.appendChild(btnCacheSelected);
			}
			
			//var txt = document.createTextNode(sActionsPanel);
			//td.appendChild(txt);
		}
		else
		{
			var btnSearch = document.getElementById(sActionsPanel + '_ctlSearchView_btnSearch');
			btnSearch.onclick   = BindArguments(function(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context)
			{
				SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			}, sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			
			var btnClear = document.getElementById(sActionsPanel + '_ctlSearchView_btnClear');
			btnClear.onclick   = BindArguments(function(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context)
			{
				SearchViewUI_ClearForm(sLayoutPanel, sActionsPanel, sEDIT_NAME);
				SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			}, sLayoutPanel, sActionsPanel, sEDIT_NAME, cbSearch, context);
			
			var btnCacheSelected = document.getElementById(sActionsPanel + '_ctlSearchView_btnCacheSelected');
			if ( btnCacheSelected != null )
			{
				var bgPage = chrome.extension.getBackgroundPage();
				btnCacheSelected.style.display = (bgPage.GetEnableOffline() && !bgPage.GetIsOffline()) ? 'inline' : 'none';
				btnCacheSelected.onclick = BindArguments(function(sFieldID, sMODULE_NAME, callback, context)
				{
					SearchViewUI_CacheSelected(sFieldID, sMODULE_NAME, callback, context);
				}, 'chkMain', sMODULE_NAME, callback, context);
			}

			if ( ctlSearchView_tblSearch != null && ctlSearchView_tblSearch.childNodes != null )
			{
				while ( ctlSearchView_tblSearch.childNodes.length > 0 )
				{
					ctlSearchView_tblSearch.removeChild(ctlSearchView_tblSearch.firstChild);
				}
			}
			if ( ctlSearchView_tblSearch == null )
			{
				alert('SearchViewUI_Clear: ' + sActionsPanel + '_ctlSearchView_tblSearch' + ' does not exist');
				return;
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'SearchViewUI_Clear');
	}
}

function SearchViewUI_CacheSelected(sFieldID, sMODULE_NAME, callback, context)
{
	try
	{
		var nCacheItemCount = 0;
		var bgPage = chrome.extension.getBackgroundPage();
		
		var fld = document.getElementsByName(sFieldID);
		for (var i = 0; i < fld.length; i++)
		{
			if ( fld[i].type == 'checkbox' )
			{
				if ( fld[i].checked )
				{
					nCacheItemCount++;
					var sID = fld[i].value;
					// 10/11/2011 Paul.  Remove the item from the selected array. 
					if ( arrSELECTED[sID] != null )
					{
						delete arrSELECTED[sID];
					}
					bgPage.DetailView_LoadItem(sMODULE_NAME, sID, function(status, message)
					{
						if ( status == 1 )
						{
							var row = message;
							var chkMain = document.getElementById('chkMain_' + Sql.ToString(row['ID']).replace('-', '_'));
							if ( chkMain != null && chkMain.type == 'checkbox' )
								chkMain.checked = false;
							
							callback.call(this, 2, 'Loaded ' + sMODULE_NAME + ': ' + row['NAME']);
						}
						nCacheItemCount--;
						if ( nCacheItemCount == 0 )
						{
							callback.call(this, 2, '');
							var chkMainCheckAll = document.getElementById('chkMainCheckAll');
							if ( chkMainCheckAll != null && chkMainCheckAll.type == 'checkbox' )
								chkMainCheckAll.checked = false;
						}
					}, context||this);
				}
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'SearchViewUI_CacheSelected'));
	}
}

function SearchViewUI_Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sEDIT_NAME, row, bEnableCaching, cbSearch, callback, context)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.EditView_LoadLayout(sEDIT_NAME, function(status, message)
		{
			if ( status == 1 )
			{
				// 10/03/2011 Paul.  EditView_LoadLayout returns the layout. 
				var layout = message;
				SearchViewUI_Clear(sLayoutPanel, sActionsPanel, sMODULE_NAME, sEDIT_NAME, bEnableCaching, cbSearch, callback, this);
				//var layout  = bgPage.SplendidCache.EditViewFields(sEDIT_NAME);
				var tblMain = document.getElementById(sActionsPanel + '_ctlSearchView_tblSearch');
				var oEditViewUI = new EditViewUI();
				oEditViewUI.LoadView(sActionsPanel, tblMain, layout, row, sActionsPanel + '_ctlSearchView_btnSearch');
				
				// 12/06/2014 Paul.  Don't display the module header on a mobile device. 
				if ( sPLATFORM_LAYOUT == '.Mobile' )
				{
					var ctlSearchView = document.getElementById(sActionsPanel + '_ctlSearchView');
					ctlSearchView.style.display = 'none';
				}
				
				callback.call(this, 1, null);
			}
			else
			{
				callback.call(this, status, message);
			}
		}, context||this);
	}
	catch(e)
	{
		callback.call(context, -1, SplendidError.FormatError(e, 'SearchViewUI_Load'));
	}
}

