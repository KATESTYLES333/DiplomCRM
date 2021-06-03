/* Copyright (C) 2011-2015 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

// 10/06/2011 Paul.  The ListView enable selection needs to be a global flag. 
var bLIST_VIEW_ENABLE_SELECTION = true;

function ListViewUI()
{
	this.SORT_FIELD     = 'NAME';
	this.SORT_DIRECTION = 'asc';
	this.MODULE_NAME    = '';
	this.GRID_NAME      = '';
	this.SEARCH_FILTER  = '';
	this.SEARCH_VALUES  = null;
	this.OnMainClicked  = null;
	// 01/30/2013 Paul.  We need more data to sort relationship data. 
	this.TABLE_NAME     = '';
	this.RELATED_MODULE = '';
	this.PRIMARY_ID     = '';
	// 08/31/2014 Paul.  Provide a way for the Offline Client to hide View and Edit buttons. 
	this.HIDE_VIEW_EDIT = false;
	this.SHOW_CONFLICTS = false;
}

ListViewUI.prototype.Clear = function(sLayoutPanel, sMODULE_NAME)
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
			alert('ListViewUI.Clear: ' + sLayoutPanel + ' does not exist');
			return;
		}
		// 12/06/2014 Paul.  Don't display the module header on a mobile device. 
		if ( sPLATFORM_LAYOUT != '.Mobile' )
			SplendidUI_ListHeader(sLayoutPanel, sMODULE_NAME + '.LBL_LIST_FORM_TITLE');
		// <table id="ctlListView_grdMain" class="listView" cellspacing="1" cellpadding="3" rules="all" border="0" border="1" width="100%">
		var ctlListView_grdMain = document.createElement('table');
		ctlListView_grdMain.id        = sLayoutPanel + '_ctlListView_grdMain';
		ctlListView_grdMain.width     = '100%';
		ctlListView_grdMain.className = 'listView';
		divMainLayoutPanel.appendChild(ctlListView_grdMain);
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'ListViewUI.Clear'));
	}
};

ListViewUI.prototype.Reset = function(sLayoutPanel, sMODULE_NAME)
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
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'ListViewUI.Reset'));
	}
};

ListViewUI.prototype.Sort = function(sLayoutPanel, sActionsPanel, sFIELD_NAME, sDIRECTION)
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
				// 01/30/2013 Paul.  Sorting a relationship view tasks extra effort.  We need to clear the layout panel and render again as a relationship panel. 
				if ( !Sql.IsEmptyString(this.RELATED_MODULE) )
				{
					var divMainLayoutPanel = document.getElementById(sLayoutPanel);
					if ( divMainLayoutPanel != null && divMainLayoutPanel.childNodes != null )
					{
						while ( divMainLayoutPanel.childNodes.length > 0 )
						{
							divMainLayoutPanel.removeChild(divMainLayoutPanel.firstChild);
						}
					}
					this.LoadRelatedModule(sLayoutPanel, sActionsPanel, this.MODULE_NAME, this.RELATED_MODULE, this.GRID_NAME, this.TABLE_NAME, this.SORT_FIELD, this.SORT_DIRECTION, this.SEARCH_FILTER, this.PRIMARY_ID, function(status, message)
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
			}
			else
			{
				SplendidError.SystemMessage(message);
			}
		}, this);
	}
	catch(e)
	{
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'ListViewUI.Sort'));
	}
};

// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
ListViewUI.prototype.Search = function(sLayoutPanel, sActionsPanel, sSEARCH_FILTER, rowSEARCH_VALUES)
{
	try
	{
		//alert('ListViewUI.Search ' + dumpObj(arrData, 'arrData'));
		SplendidError.SystemMessage('Searching ');
		this.SEARCH_FILTER = sSEARCH_FILTER  ;
		this.SEARCH_VALUES = rowSEARCH_VALUES;
		//alert('ListViewUI.Search ' + this.SEARCH_FILTER);
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
		SplendidError.SystemMessage(SplendidError.FormatError(e, 'ListViewUI.Search'));
	}
};

ListViewUI.prototype.CheckAll = function(chkMainCheckAll, sFieldID)
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
		SplendidError.SystemAlert(e, 'ListViewUI.CheckAll');
	}
};

ListViewUI.prototype.Render = function(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tbody, rows, sPRIMARY_MODULE, sPRIMARY_ID)
{
	this.RenderHeader(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tbody, rows, sPRIMARY_MODULE, sPRIMARY_ID);
	// RenderRow was moved to the paginator onSelect event. 
}

ListViewUI.prototype.RenderHeader = function(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tbody, rows, sPRIMARY_MODULE, sPRIMARY_ID)
{
	// 12/06/2014 Paul.  Use new mobile flag. 
	var bIsMobile = isMobileDevice();
	if ( isMobileLandscape() )
		bIsMobile = false;
	
	// <tr class="listViewPaginationTdS1">
	var tr = document.createElement('tr');
	tbody.appendChild(tr);
	tr.className = 'listViewPaginationTdS1';
	tr.align     = 'right';
	var td = document.createElement('td');
	tr.appendChild(td);
	// 12/09/2012 Paul.  Add one for the actions and one for the last column. 
	td.colSpan = layout.length + 2;
	var spnPagination = document.createElement('div');
	td.appendChild(spnPagination);
	spnPagination.id = tbody.parentNode.id + '_pagination';

	// <tr class="listViewThS1">
	tr = document.createElement('tr');
	tbody.appendChild(tr);
	tr.className = 'listViewThS1';
	if ( layout.length > 0 )
	{
		// 09/01/2011 Paul.  First column will be for actions. 
		td = document.createElement('td');
		td.width = '60';
		// 12/06/2014 Paul.  Use new mobile flag. 
		if ( bIsMobile )
			td.style.display = 'none';
		tr.appendChild(td);
		if ( this.OnMainClicked != null )
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
				//a.innerHTML = '<nobr>' + L10n.Term(sHEADER_TEXT) + '</nobr>';
				a.style.whiteSpace = 'nowrap';
				a.appendChild(document.createTextNode(L10n.Term(sHEADER_TEXT)));
				var img = document.createElement('img');
				td.appendChild(img);
				img.align       = 'absmiddle';
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
		// 12/01/2012 Paul.  Add one last column at the end to allow for < 100% specified widths. 
		// 12/06/2014 Paul.  Don't add the spacer column on a mobile device. 
		if ( !bIsMobile )
		{
			td = document.createElement('td');
			tr.appendChild(td);
		}

		// 12/09/2012 Paul.  Add pagination and move page rendering to onSelect. 
		var thisListViewUI = this;
		if ( rows != null && rows.length > 0 )
		{
			// 12/09/2012 Paul.  Using pageNumbers style has undesirable effects. 
			//spnPagination.className = 'pageNumbers';
			// http://www.xarg.org/2011/09/jquery-pagination-revised/
			$('#' + spnPagination.id).paging(rows.length, 
			{ onSelect: function(page)
				{
					while ( tbody.childNodes.length > 2 )
					{
						tbody.removeChild(tbody.lastChild);
					}
					for ( var i = this.slice[0]; i < this.slice[1]; i++ )
					{
						var tr = document.createElement('tr');
						tbody.appendChild(tr);
						if ( i % 2 == 0 )
							tr.className = 'oddListRowS1';
						else
							tr.className = 'evenListRowS1';
						var row = rows[i];
						// 12/06/2014 Paul.  Use new mobile flag. 
						thisListViewUI.RenderRow(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tr, row, sPRIMARY_MODULE, sPRIMARY_ID, bIsMobile);
					}
					return false;
				}
			, perpage : Crm.Config.ToInteger('list_max_entries_per_page')
			, format  : '< . >'
			, onFormat: function(type)
				{
					switch ( type )
					{
						case 'block':  // n and c
							if ( this.value != this.page )
								return '<a href="#">' + this.value + '</a>';
							else
								return this.value;
						case 'first':  // [
							if ( this.page > 1 )
								return '<a href="#">' + L10n.Term('.LNK_LIST_FIRST') + '</a>';
							else
								return  '' + L10n.Term('.LNK_LIST_FIRST') + '';
						case 'prev' :  // <
							if ( this.page > 1 )
								return '<a href="#">&lt; ' + L10n.Term('.LNK_LIST_PREVIOUS') + '</a>';
							else
								return '&lt; ' + L10n.Term('.LNK_LIST_PREVIOUS') + '';
						case 'next' :  // >
							if ( this.page < this.pages )
								return '<a href="#">' + L10n.Term('.LNK_LIST_NEXT') + ' &gt;</a>';
							else
								return '' + L10n.Term('.LNK_LIST_NEXT') + ' &gt;';
						case 'last' :  // ]
							if ( this.page < this.pages )
								return '<a href="#">' + L10n.Term('.LNK_LIST_LAST') + '</a>';
							else
								return '' + L10n.Term('.LNK_LIST_LAST') + '';
						case 'leap' :
							//litPageRange.Text = String.Format("&nbsp; <span class=\"pageNumbers\">({0} - {1} {2} {3})</span> ", nPageStart, nPageEnd, sOf, vw.Count);
							return ' ( ' + (this.slice[0] + 1) + ' - ' + this.slice[1] + ' ' + L10n.Term('.LBL_LIST_OF') + ' ' + this.number + ' ) ';
						case 'fill' :
							return ' ';
					}
					return '';
				}
			});
		}
	}
};

// 12/06/2014 Paul.  Use new mobile flag. 
ListViewUI.prototype.RenderRow = function(sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, layout, tr, row, sPRIMARY_MODULE, sPRIMARY_ID, bIsMobile)
{
	try
	{
		if ( layout.length > 0 )
		{
			// 09/01/2011 Paul.  First column will be for actions. 
			var td = document.createElement('td');
			tr.appendChild(td);
			td.width = '60';
			// 12/06/2014 Paul.  Use new mobile flag. 
			if ( bIsMobile )
				td.style.display = 'none';
			//td.style.whiteSpace = 'nowrap';
			//td.style.textWrap   = 'none';
			// 12/01/2012 Paul.  Style is not working, so use nobr tag. 
			var nobrActions = document.createElement('nobr');
			td.appendChild(nobrActions);

			if ( this.OnMainClicked != null )
			{
				// 11/27/2011 Paul.  IE has a problem setting the Name when it does not match the ID. 
				// http://webbugtrack.blogspot.com/2007/10/bug-235-createelement-is-broken-in-ie.html
				var chkMain = null;
				// 11/27/2011 Paul.  Detect Internet Explorer. http://msdn.microsoft.com/en-us/library/ms537509(v=vs.85).aspx
				// 06/10/2012 Paul.  Windows Phone 7 is an Internet Explorer, but it does not allow createElement with text. 
				// 11/15/2012 Paul.  Microsoft Surface does not like the special input. 
				if ( navigator.appName == 'Microsoft Internet Explorer' && navigator.userAgent.indexOf('IEMobile', 0) == -1 && navigator.userAgent.indexOf('Touch') == -1 )
				{
					try
					{
						chkMain = document.createElement('<input name="chkMain" />');
					}
					catch(e)
					{
						chkMain = document.createElement('input');
					}
				}
				else
				{
					chkMain = document.createElement('input');
				}
				chkMain.id        = 'chkMain_' + Sql.ToString(row['ID']).replace('-', '_');
				chkMain.name      = 'chkMain';
				chkMain.type      = 'checkbox';
				chkMain.className = 'checkbox';
				chkMain.Module    = sLIST_MODULE_NAME;
				chkMain.value     = row['ID'  ];
				chkMain.tooltip   = row['NAME'];
				chkMain.onclick   = BindArguments(this.OnMainClicked, chkMain, sLIST_MODULE_NAME, row['ID'], row['NAME']);
				chkMain.style.padding       = '2px';
				chkMain.style.verticalAlign = 'middle';
				// 09/25/2011 Paul.  IE does not allow you to set the type after it is added to the document. 
				nobrActions.appendChild(chkMain);
				// 10/04/2011 Paul.  IE8 requires that we set checked after appending. 
				if ( SelectionUI_IsSelected(row['ID']) )
					chkMain.checked = true;
			}

			// 12/01/2012 Paul.  For activities lists, we need to convert the activity to the base module. 
			if ( sLIST_MODULE_NAME == 'Activities' && row['ACTIVITY_TYPE'] !== undefined )
			{
				sLIST_MODULE_NAME = row['ACTIVITY_TYPE'];
			}
			var aView = document.createElement('a');
			nobrActions.appendChild(aView);
			aView.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
			{
				// 10/28/2012 Paul.  With a sub-panel, we need to prevent inline editing. 
				if ( sLayoutPanel.indexOf('_') > 0 )
				{
					sLayoutPanel  = sLayoutPanel.split('_')[0];
					sActionsPanel = sLayoutPanel.replace('Layout', 'Actions');
				}
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
			}, sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, row['ID']);

			var imgView = document.createElement('img');
			aView.appendChild(imgView);
			imgView.align             = 'absmiddle';
			imgView.style.height      = '16px';
			imgView.style.width       = '16px';
			imgView.style.borderWidth = '0px';
			imgView.src               = sIMAGE_SERVER + 'App_Themes/Six/images/view_inline.gif';
			imgView.alt               = L10n.Term('.LNK_VIEW');
			imgView.style.padding     = '2px';
			// 12/01/2012 Paul.  View image needs a little more spacing to the right. 
			imgView.style.marginRight = '4px';
			imgView.style.border      = 'none';

			var aEdit = document.createElement('a');
			nobrActions.appendChild(aEdit);
			aEdit.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, bDUPLICATE)
			{
				// 10/28/2012 Paul.  With a sub-panel, we need to prevent inline editing. 
				if ( sLayoutPanel.indexOf('_') > 0 )
				{
					sLayoutPanel  = sLayoutPanel.split('_')[0];
					sActionsPanel = sLayoutPanel.replace('Layout', 'Actions');
				}
				var oEditViewUI = new EditViewUI();
				oEditViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, bDUPLICATE);
			}, sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, row['ID'], false);

			var imgEdit = document.createElement('img');
			aEdit.appendChild(imgEdit);
			imgEdit.align             = 'absmiddle';
			imgEdit.style.height      = '16px';
			imgEdit.style.width       = '16px';
			imgEdit.style.borderWidth = '0px';
			imgEdit.src               = sIMAGE_SERVER + 'App_Themes/Six/images/edit_inline.gif';
			imgEdit.alt               = L10n.Term('.LNK_EDIT');
			imgEdit.style.padding     = '2px';
			// 12/01/2012 Paul.  View image needs a little more spacing to the right. 
			imgEdit.style.marginRight = '4px';
			imgEdit.style.border      = 'none';

			// 12/01/2012 Paul.  Users are not viewable or editable. 
			// 12/04/2012 Paul.  We are going to allow users to be viewed and edited. 
			// 12/05/2012 Paul.  Only allow Users module if in Admin mode. 
			// 08/31/2014 Paul.  Provide a way for the Offline Client to hide View and Edit buttons. 
			if ( (!bADMIN_MENU && sLIST_MODULE_NAME == 'Users') || this.HIDE_VIEW_EDIT )
			{
				imgView.style.display = 'none';
				imgEdit.style.display = 'none';
			}
			// 08/31/2014 Paul.  Special buttons for conflict management. 
			if ( this.SHOW_CONFLICTS )
			{
				var aApplyLocal = document.createElement('a');
				nobrActions.appendChild(aApplyLocal);
				aApplyLocal.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
				{
					SyncUtils.SyncApplyLocal(sMODULE_NAME, sID);
				}, sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, row['ID']);

				var imgApplyLocal = document.createElement('img');
				aApplyLocal.appendChild(imgApplyLocal);
				imgApplyLocal.align             = 'absmiddle';
				imgApplyLocal.style.height      = '18px';
				imgApplyLocal.style.width       = '18px';
				imgApplyLocal.style.borderWidth = '0px';
				imgApplyLocal.src               = sIMAGE_SERVER + 'App_Themes/Six/images/ApplyLocal_inline.gif';
				imgApplyLocal.alt               = L10n.Term('Offline.LBL_APPLY_LOCAL');
				imgApplyLocal.style.padding     = '1px';
				imgApplyLocal.style.marginRight = '4px';
				imgApplyLocal.style.border      = 'none';

				var aRestoreRemote = document.createElement('a');
				nobrActions.appendChild(aRestoreRemote);
				aRestoreRemote.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
				{
					SyncUtils.SyncRestoreRemote(sMODULE_NAME, sID);
				}, sLayoutPanel, sActionsPanel, sLIST_MODULE_NAME, row['ID']);

				var imgRestoreRemote = document.createElement('img');
				aRestoreRemote.appendChild(imgRestoreRemote);
				imgRestoreRemote.align             = 'absmiddle';
				imgRestoreRemote.style.height      = '18px';
				imgRestoreRemote.style.width       = '18px';
				imgRestoreRemote.style.borderWidth = '0px';
				imgRestoreRemote.src               = sIMAGE_SERVER + 'App_Themes/Six/images/RestoreRemote_inline.gif';
				imgRestoreRemote.alt               = L10n.Term('Offline.LBL_RESTORE_REMOTE');
				imgRestoreRemote.style.padding     = '1px';
				imgRestoreRemote.style.marginRight = '4px';
				imgRestoreRemote.style.border      = 'none';
			}

			if ( sPRIMARY_MODULE != null && sPRIMARY_ID != null )
			{
				var aDelete = document.createElement('a');
				nobrActions.appendChild(aDelete);
				aDelete.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sPRIMARY_MODULE, sPRIMARY_ID, sRELATED_MODULE, sRELATED_ID, tr)
				{
					var bgPage = chrome.extension.getBackgroundPage();
					bgPage.DeleteRelatedItem(sPRIMARY_MODULE, sPRIMARY_ID, sRELATED_MODULE, sRELATED_ID, function(status, message)
					{
						if ( status == 1 )
						{
							tr.parentNode.removeChild(tr);
						}
						else if ( status == -1 )
						{
							SplendidError.SystemMessage(message);
						}
					}, this);
				}, sLayoutPanel, sActionsPanel, sPRIMARY_MODULE, sPRIMARY_ID, sLIST_MODULE_NAME, row['ID'], tr);
		
				var imgDelete = document.createElement('img');
				aDelete.appendChild(imgDelete);
				imgDelete.align             = 'absmiddle';
				imgDelete.style.height      = '16px';
				imgDelete.style.width       = '16px';
				imgDelete.style.borderWidth = '0px';
				imgDelete.src               = sIMAGE_SERVER + 'App_Themes/Six/images/delete_inline.gif';
				imgDelete.alt               = L10n.Term('.LNK_DELETE');
				imgDelete.style.padding     = '2px';
				imgDelete.style.marginRight = '4px';
				imgDelete.style.border      = 'none';
				// 12/01/2012 Paul.  Users are not viewable or editable. 
				// 12/04/2012 Paul.  We are going to allow users to be viewed and edited. 
				// 12/05/2012 Paul.  Only allow Users module if in Admin mode. 
				if ( !bADMIN_MENU && sLIST_MODULE_NAME == 'Users' )
					imgDelete.style.display = 'none';
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
						// 08/26/2014 Paul.  Ignore ImageButton. 
						|| sDATA_FORMAT == 'ImageButton'
					   )
				   )
				{
					sCOLUMN_TYPE = 'TemplateColumn';
				}
				if ( sCOLUMN_TYPE == 'TemplateColumn' )
				{
					// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
					if ( row[sDATA_FIELD] != null || row[sDATA_FIELD] === undefined )
					{
						// 12/01/2012 Paul.  Users cannot be viewed or edited. 
						// 12/04/2012 Paul.  We are going to allow users to be viewed and edited. 
						// 12/05/2012 Paul.  Only allow Users module if in Admin mode. 
						if ( sDATA_FORMAT == 'HyperLink' && row[sURL_FIELD] != null && (bADMIN_MENU || sURL_MODULE != 'Users') )
						{
							var a = document.createElement('a');
							td.appendChild(a);
							// 12/01/2012 Paul.  For activities lists, we need to convert the activity to the base module. 
							if ( sURL_MODULE == 'Activities' && row['ACTIVITY_TYPE'] !== undefined )
							{
								sURL_MODULE = row['ACTIVITY_TYPE'];
							}
							if ( sURL_FORMAT.indexOf('view.aspx?id=') > 0 )
							{
								a.href = '#';
								a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID)
								{
									// 10/28/2012 Paul.  With a sub-panel, we need to prevent inline editing. 
									if ( sLayoutPanel.indexOf('_') > 0 )
									{
										sLayoutPanel  = sLayoutPanel.split('_')[0];
										sActionsPanel = sLayoutPanel.replace('Layout', 'Actions');
									}
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
								}, sLayoutPanel, sActionsPanel, sURL_MODULE, row[sURL_FIELD]);
							}
							else if ( sURL_FORMAT.indexOf('edit.aspx?id=') > 0 )
							{
								a.href = '#';
								a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, bDUPLICATE)
								{
									var oEditViewUI = new EditViewUI();
									oEditViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sID, bDUPLICATE);
								}, sLayoutPanel, sActionsPanel, sURL_MODULE, row[sURL_FIELD], false);
							}
							// 08/31/2014 Paul.  The offline client needs a way to jump to the module list. 
							else if ( sURL_FORMAT.indexOf('Conflicts/default.aspx') > 0 && row[sURL_FIELD] !== undefined )
							{
								sURL_MODULE = row[sURL_FIELD];
								a.href = '#';
								a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME)
								{
									var rowDefaultSearch = null;
									var sGRID_NAME   = sMODULE_NAME + '.ListView' + sPLATFORM_LAYOUT;
									var oListViewUI = new ListViewUI();
									oListViewUI.Reset(sLayoutPanel, sMODULE_NAME);
									oListViewUI.SHOW_CONFLICTS = true;
									oListViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, rowDefaultSearch, function(status, message)
									{
										if ( status != 1 )
										{
											SplendidError.SystemMessage(message);
										}
									});
								}, sLayoutPanel, sActionsPanel, sURL_MODULE);
							}
							else if ( sURL_FORMAT.indexOf('default.aspx') > 0 && row[sURL_FIELD] !== undefined )
							{
								sURL_MODULE = row[sURL_FIELD];
								a.href = '#';
								a.onclick = BindArguments(function(sLayoutPanel, sActionsPanel, sMODULE_NAME)
								{
									var rowDefaultSearch = null;
									var sGRID_NAME   = sMODULE_NAME + '.ListView' + sPLATFORM_LAYOUT;
									var oListViewUI = new ListViewUI();
									oListViewUI.Reset(sLayoutPanel, sMODULE_NAME);
									oListViewUI.Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, rowDefaultSearch, function(status, message)
									{
										if ( status != 1 )
										{
											SplendidError.SystemMessage(message);
										}
									});
								}, sLayoutPanel, sActionsPanel, sURL_MODULE);
							}
							// 10/25/2012 Paul.  On the Surface, there are fields that we need to lookup, like ACCOUNT_NAME. 
							if ( row[sDATA_FIELD] === undefined && !Sql.IsEmptyString(row[sURL_FIELD]) )
							{
								BindArguments(function(sURL_MODULE, sID, a, context)
								{
									Crm.Modules.ItemName(sURL_MODULE, sID, function(status, message)
									{
										if ( status == 1 )
										{
											//a.innerHTML = message;
											a.appendChild(document.createTextNode(message));
										}
									}, context);
								}, sURL_MODULE, row[sURL_FIELD], a, this)();
							}
							else if ( row[sDATA_FIELD] !== undefined )
							{
								// 08/24/2014 Paul.  WinRT does not like to add text with angle brackets. 
								//a.innerHTML = row[sDATA_FIELD];
								a.appendChild(document.createTextNode(row[sDATA_FIELD]));
							}
						}
						else if ( sDATA_FORMAT == 'Date' )
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT());
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
						else if ( sDATA_FORMAT == 'DateTime' )
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							sDATA_VALUE = FromJsonDate(sDATA_VALUE, Security.USER_DATE_FORMAT() + ' ' + Security.USER_TIME_FORMAT());
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
						else if ( sDATA_FORMAT == 'Currency' )
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
						else if ( sDATA_FORMAT == 'MultiLine' )
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
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
						// 08/26/2014 Paul.  Ignore ImageButton. 
						else if ( sDATA_FORMAT == 'ImageButton' )
						{
						}
						else
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
					}
				}
				else if ( sCOLUMN_TYPE == 'BoundColumn' )
				{
					if ( row[sDATA_FIELD] != null )
					{
						// 12/01/2012 Paul.  The activity status needs to be dynamically converted to the correct list. 
						if ( sLIST_NAME == 'activity_status' )
						{
							var sDATA_VALUE    = '';
							var sACTIVITY_TYPE = row['ACTIVITY_TYPE'];
							switch ( sACTIVITY_TYPE )
							{
								case 'Tasks'   :
									sLIST_NAME  = 'task_status_dom'   ;
									sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]);
									break;
								case 'Meetings':
									sLIST_NAME  = 'meeting_status_dom';
									sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]);
									break;
								case 'Calls'   :
									// 07/15/2006 Paul.  Call status is translated externally. 
									sDATA_VALUE = L10n.ListTerm('call_direction_dom', row['DIRECTION']) + ' ' + L10n.ListTerm('call_status_dom', row['STATUS']);
									break;
								case 'Notes'   :
									// 07/15/2006 Paul.  Note Status is not normally as it does not have a status. 
									sDATA_VALUE = L10n.Term('.activity_dom.Note');
									break;
								// 06/15/2006 Paul.  This list name for email_status does not follow the standard. 
								case 'Emails'  :
									sLIST_NAME  = 'dom_email_status'  ;
									sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]);
									break;
								// 04/21/2006 Paul.  If the activity does not have a status (such as a Note), then use activity_dom. 
								default        :
									sLIST_NAME  = 'activity_dom'      ;
									sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]) + '[' + sACTIVITY_TYPE + ']';
									break;
							}
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
						else if ( sLIST_NAME != null )
						{
							// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
							var sDATA_VALUE = L10n.ListTerm(sLIST_NAME, row[sDATA_FIELD]);
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
						else
						{
							var sDATA_VALUE = row[sDATA_FIELD];
							//td.innerHTML = sDATA_VALUE;
							td.appendChild(document.createTextNode(sDATA_VALUE));
						}
					}
				}
			}
			// 12/01/2012 Paul.  Add one last column at the end to allow for < 100% specified widths. 
			// 12/06/2014 Paul.  Don't add the spacer column on a mobile device. 
			if ( !bIsMobile )
			{
				td = document.createElement('td');
				tr.appendChild(td);
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemError(e, 'ListViewUI.RenderRow');
	}
};

ListViewUI.prototype.GridColumns = function(layout)
{
	var arrSelectFields = new Array();
	if ( layout.length > 0 )
	{
		var bEnableTeamManagement = Crm.Config.enable_team_management();
		var bEnableDynamicTeams   = Crm.Config.enable_dynamic_teams();
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sSORT_EXPRESSION            = lay.SORT_EXPRESSION           ;
			var sDATA_FIELD                 = lay.DATA_FIELD                ;
			var sLIST_NAME                  = lay.LIST_NAME                 ;
			var sDATA_FORMAT                = lay.DATA_FORMAT               ;
			var sURL_FIELD                  = lay.URL_FIELD                 ;
			var sURL_MODULE                 = lay.URL_MODULE                ;
			var sURL_ASSIGNED_FIELD         = lay.URL_ASSIGNED_FIELD        ;
			var sPARENT_FIELD               = lay.PARENT_FIELD              ;
			
			if ( sDATA_FORMAT == 'Hover' )
				continue;
			if ( sDATA_FIELD != null && sDATA_FIELD.length > 0 )
			{
				// 09/16/2014 Paul.  Need to prevent duplicate entries in array. 
				if ( $.inArray(sDATA_FIELD, arrSelectFields) == -1 )
					arrSelectFields.push(sDATA_FIELD);
				// 08/29/2014 Paul.  Add the team set when adding the team name as the swap will be made inline. 
				if ( bEnableTeamManagement && bEnableDynamicTeams && sDATA_FIELD == 'TEAM_NAME' )
				{
					if ( $.inArray('TEAM_SET_NAME', arrSelectFields) == -1 )
						arrSelectFields.push('TEAM_SET_NAME');
				}
			}
			if ( sSORT_EXPRESSION != null && sSORT_EXPRESSION.length > 0 )
			{
				if (sDATA_FIELD != sSORT_EXPRESSION)
				{
					if ( $.inArray(sSORT_EXPRESSION, arrSelectFields) == -1 )
						arrSelectFields.push(sSORT_EXPRESSION);
				}
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
							if ( $.inArray(s, arrSelectFields) == -1 )
								arrSelectFields.push(s);
						}
					}
				}
				else if ( sURL_FIELD.indexOf('.') == -1 )
				{
					if ( $.inArray(sURL_FIELD, arrSelectFields) == -1 )
						arrSelectFields.push(sURL_FIELD);
				}
				if ( sURL_ASSIGNED_FIELD != null && sURL_ASSIGNED_FIELD.length > 0 )
				{
					if ( $.inArray(sURL_ASSIGNED_FIELD, arrSelectFields) == -1 )
						arrSelectFields.push(sURL_ASSIGNED_FIELD);
				}
			}
			if ( sPARENT_FIELD != null && sPARENT_FIELD.length > 0 )
			{
				if ( $.inArray(sPARENT_FIELD, arrSelectFields) == -1 )
					arrSelectFields.push(sPARENT_FIELD);
			}
			// 12/01/2012 Paul.  ACTIVITY_TYPE is an implied required field for ACTIVITY views. 
			if ( sLIST_NAME == 'activity_status' )
			{
				if ( $.inArray('ID'           , arrSelectFields) == -1 ) arrSelectFields.push('ID'           );
				if ( $.inArray('ACTIVITY_TYPE', arrSelectFields) == -1 ) arrSelectFields.push('ACTIVITY_TYPE');
				// 12/01/2012 Paul.  Direction and Status are used by Calls. 
				if ( $.inArray('DIRECTION'    , arrSelectFields) == -1 ) arrSelectFields.push('DIRECTION'    );
				if ( $.inArray('STATUS'       , arrSelectFields) == -1 ) arrSelectFields.push('STATUS'       );
			}
			else if ( sURL_MODULE == 'Activities' )
			{
				if ( $.inArray('ID'           , arrSelectFields) == -1 ) arrSelectFields.push('ID'           );
				if ( $.inArray('ACTIVITY_TYPE', arrSelectFields) == -1 ) arrSelectFields.push('ACTIVITY_TYPE');
			}
		}
	}
	return arrSelectFields.join(',');
};

// 10/11/2012 Paul.  Separate method for loading related lists. 
ListViewUI.prototype.LoadRelatedModule = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sRELATED_MODULE, sGRID_NAME, sTABLE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSEARCH_FILTER, sPRIMARY_ID, callback)
{
	try
	{
		// 01/30/2013 Paul.  We need to save the key ListViewUI parameters for sorting. 
		this.MODULE_NAME   = sMODULE_NAME    ;
		this.GRID_NAME     = sGRID_NAME      ;
		this.SEARCH_FILTER = sSEARCH_FILTER  ;
		this.SEARCH_VALUES = null            ;
		this.TABLE_NAME     = sTABLE_NAME    ;
		this.RELATED_MODULE = sRELATED_MODULE;
		this.PRIMARY_ID     = sPRIMARY_ID    ;
		
		var bgPage = chrome.extension.getBackgroundPage();
		//var layout = bgPage.SplendidCache.GridViewColumns(sGRID_NAME);
		bgPage.ListView_LoadLayout(sGRID_NAME, function(status, message)
		{
			if ( status == 1 )
			{
				// 10/03/2011 Paul. ListView_LoadLayout returns the layout. 
				var layout = message;
				var sSELECT_FIELDS = this.GridColumns(layout);
				bgPage.ListView_LoadTable(sTABLE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT_FIELDS, sSEARCH_FILTER, function(status, message)
				{
					if ( status == 1 )
					{
						// 10/04/2011 Paul.  ListView_LoadTable returns the row. 
						var rows = message;
						// 10/11/2012 Paul.  Clear adds a header, that we don't want. 
						//this.Clear(sLayoutPanel, sMODULE_NAME);
						var divMainLayoutPanel = document.getElementById(sLayoutPanel);
						// 10/17/2012 Paul.  Exit if the MainLayoutPanel does not exist.  This is a sign that the user has navigated elsewhere. 
						if ( divMainLayoutPanel == null )
							return;
						// <table id="ctlListView_grdMain" class="listView" cellspacing="1" cellpadding="3" rules="all" border="0" border="1" width="100%">
						var ctlListView_grdMain = document.createElement('table');
						ctlListView_grdMain.id        = sLayoutPanel + '_ctlListView_grdMain';
						ctlListView_grdMain.width     = '100%';
						ctlListView_grdMain.className = 'listView';
						divMainLayoutPanel.appendChild(ctlListView_grdMain);

						var tbody = document.createElement('tbody');
						ctlListView_grdMain.appendChild(tbody);
						
						this.Render(sLayoutPanel, sActionsPanel, sRELATED_MODULE, layout, tbody, rows, sMODULE_NAME, sPRIMARY_ID);
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
		callback(-1, SplendidError.FormatError(e, 'ListViewUI.LoadRelatedModule'));
	}
};

// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
ListViewUI.prototype.LoadModule = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, sSEARCH_FILTER, rowSEARCH_VALUES, callback)
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
				// 08/31/2014 Paul.  When in conflicts mode, filter by the table-specific conflicts view. 
				if ( this.SHOW_CONFLICTS )
				{
					if ( !Sql.IsEmptyString(sSEARCH_FILTER) )
						sSEARCH_FILTER += ' and ';
					var sTABLE_NAME = Crm.Modules.TableName(sMODULE_NAME);
					sSEARCH_FILTER = Sql.ToString(sSEARCH_FILTER) + 'ID in (select ID from vw' + sTABLE_NAME + '_REMOTE_Conflicted)';
				}
				bgPage.ListView_LoadModule(sMODULE_NAME, this.SORT_FIELD, this.SORT_DIRECTION, sSELECT_FIELDS, sSEARCH_FILTER, rowSEARCH_VALUES, function(status, message)
				{
					// 10/04/2011 Paul.  ListView_LoadModule returns the row. 
					// 10/21/2012 Paul.  Always display the ListView header. 
					this.Clear(sLayoutPanel, sMODULE_NAME);
					// 12/06/2014 Paul.  LayoutMode is used on the Mobile view. 
					ctlActiveMenu.ActivateTab(sMODULE_NAME, null, 'ListView');
					var ctlListView_grdMain = document.getElementById(sLayoutPanel + '_ctlListView_grdMain');
					var tbody = document.createElement('tbody');
					ctlListView_grdMain.appendChild(tbody);
					
					if ( status == 1 )
					{
						var rows = message;
						this.Render(sLayoutPanel, sActionsPanel, sMODULE_NAME, layout, tbody, rows, null, null);
						callback(1, null);
					}
					else
					{
						this.Render(sLayoutPanel, sActionsPanel, sMODULE_NAME, layout, tbody, null, null, null);
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
		callback(-1, SplendidError.FormatError(e, 'ListViewUI.LoadModule'));
	}
};

ListViewUI.prototype.Load = function(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, rowDefaultSearch, callback)
{
	try
	{
		this.MODULE_NAME   = null;
		this.GRID_NAME     = null;
		this.SEARCH_FILTER = '';
		
		var bgPage = chrome.extension.getBackgroundPage();
		// 11/29/2011 Paul.  We are having an issue with the globals getting reset, so we need to re-initialize. 
		if ( !bgPage.SplendidCache.IsInitialized() )
		{
			SplendidUI_ReInit(sLayoutPanel, sActionsPanel, sMODULE_NAME);
			return;
		}
		
		this.MODULE_NAME   = sMODULE_NAME;
		this.GRID_NAME     = sGRID_NAME;
		this.SEARCH_FILTER = '';
		this.SEARCH_VALUES = null;
		this.OnMainClicked = bLIST_VIEW_ENABLE_SELECTION ? SelectionUI_chkMain_Clicked : null;

		// 10/04/2011 Paul.  The session might have timed-out, so first check if we are authenticated. 
		bgPage.AuthenticatedMethod(function(status, message)
		{
			if ( status == 1 )
			{
				bgPage.Terminology_LoadModule(sMODULE_NAME, function(status, message)
				{
					if ( status == 0 || status == 1 )
					{
						// 09/10/2011 Paul.  Make sure to load the layout first as it might be needed inside SearchViewUI_SearchForm, or this.LoadModule, which run in parallel. 
						bgPage.ListView_LoadLayout(sGRID_NAME, function(status, message)
						{
							if ( status == 1 )
							{
								// 10/15/2012 Paul.  The sub title should already be converted from a term. 
								SplendidUI_ModuleHeader(sLayoutPanel, sActionsPanel, sMODULE_NAME, L10n.ListTerm('moduleList', 'Home'));
								// 10/03/2011 Paul. ListView_LoadLayout returns the layout. 
								var layout = message;
								// 08/31/2014 Paul.  Offline Client dashboard does not need search. 
								if ( !this.HIDE_VIEW_EDIT )
								{
									SearchViewUI_Load(sLayoutPanel, sActionsPanel, sMODULE_NAME, sMODULE_NAME + '.SearchBasic' + sPLATFORM_LAYOUT, rowDefaultSearch, bLIST_VIEW_ENABLE_SELECTION, this.Search, function(status, message)
									{
										if ( status == 1 )
										{
											//SplendidError.SystemMessage('');
										}
										else
										{
											callback(status, message);
										}
										if ( rowDefaultSearch != null )
										{
											var sEDIT_NAME = sMODULE_NAME + '.SearchBasic' + sPLATFORM_LAYOUT;
											SearchViewUI_SearchForm(sLayoutPanel, sActionsPanel, sEDIT_NAME, this.Search, this);
											// 01/01/2012 Paul.  Need to send final completion message. 
											callback(1, message);
										}
									}, this);
								}
								if ( rowDefaultSearch == null || this.HIDE_VIEW_EDIT )
								{
									// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
									this.LoadModule(sLayoutPanel, sActionsPanel, sMODULE_NAME, sGRID_NAME, this.SEARCH_FILTER, this.SEARCH_VALUES, function(status, message)
									{
										if ( status == 1 && bLIST_VIEW_ENABLE_SELECTION )
											SelectionUI_Render();
										callback(status, message);
									});
								}
								// 10/16/2012 Paul.  Always load the global layout cache if it has not been loaded. 
								SplendidUI_Cache(function(status, message)
								{
									if ( status == 2 )
									{
										SplendidError.SystemMessage(message);
									}
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
		callback(-1, SplendidError.FormatError(e, 'ListViewUI.Load'));
	}
};


