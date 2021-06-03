/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function DynamicButtonsUI_Clear(sActionsPanel)
{
	try
	{
		var divMainLayoutPanelActions = document.getElementById(sActionsPanel);
		//alert('DynamicButtonsUI_Clear(' + sActionsPanel + ')' + divMainLayoutPanelActions);
		if ( divMainLayoutPanelActions != null && divMainLayoutPanelActions.childNodes != null )
		{
			while ( divMainLayoutPanelActions.childNodes.length > 0 )
			{
				divMainLayoutPanelActions.removeChild(divMainLayoutPanelActions.firstChild);
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'DynamicButtonsUI_Clear');
	}
}

function DynamicButtonsUI_LoadButtons(sLayoutPanel, sActionsPanel, layout, row, Page_Command, context)
{
	try
	{
		var divActionsPanel = document.getElementById(sActionsPanel);
		// 10/17/2012 Paul.  Exit if the ActionsPanel does not exist.  This is a sign that the user has navigated elsewhere. 
		if ( divActionsPanel == null )
			return;
		var pnlDynamicButtons = document.createElement('div');
		pnlDynamicButtons.id        = 'pnlDynamicButtons';
		pnlDynamicButtons.className = 'button-panel';
		divActionsPanel.appendChild(pnlDynamicButtons);
		
		for ( var nLayoutIndex in layout )
		{
			var lay = layout[nLayoutIndex];
			var sVIEW_NAME          = lay.VIEW_NAME         ;
			var sCONTROL_TYPE       = lay.CONTROL_TYPE      ;
			var sMODULE_NAME        = lay.MODULE_NAME       ;
			//var sMODULE_ACCESS_TYPE = lay.MODULE_ACCESS_TYPE;
			//var sTARGET_NAME        = lay.TARGET_NAME       ;
			//var sTARGET_ACCESS_TYPE = lay.TARGET_ACCESS_TYPE;
			var sCONTROL_TEXT       = lay.CONTROL_TEXT      ;
			var sCONTROL_TOOLTIP    = lay.CONTROL_TOOLTIP   ;
			var sCONTROL_CSSCLASS   = lay.CONTROL_CSSCLASS  ;
			var sTEXT_FIELD         = lay.TEXT_FIELD        ;
			var sARGUMENT_FIELD     = lay.ARGUMENT_FIELD    ;
			var sCOMMAND_NAME       = lay.COMMAND_NAME      ;
			var sURL_FORMAT         = lay.URL_FORMAT        ;
			var sURL_TARGET         = lay.URL_TARGET        ;
			var sONCLICK_SCRIPT     = lay.ONCLICK_SCRIPT    ;
			// 03/14/2014 Paul.  Allow hidden buttons to be created. 
			var bHIDDEN             = Sql.ToBoolean(lay.HIDDEN);
			
			// 11/24/2012 Paul.  Search and ViewLog are not supported at this time. 
			// 03/10/2013 Paul.  SendInvites is not supported. 
			if ( !Sql.IsEmptyString(sCOMMAND_NAME) && (sCOMMAND_NAME == 'Save.SendInvites' || sCOMMAND_NAME == 'ViewLog' || sCOMMAND_NAME == 'vCard' || sCOMMAND_NAME.indexOf('.Search') > 0) )
				continue;
			else if ( bREMOTE_ENABLED && sVIEW_NAME == 'Users.DetailView' )
			{
				// 10/19/2014 Paul.  Users.DetailView ResetDefaults, ChangePassword, Duplicate are not supported. 
				if (sCOMMAND_NAME == 'EditMyAccount' || sCOMMAND_NAME == 'ResetDefaults' || sCOMMAND_NAME == 'ChangePassword' || sCOMMAND_NAME == 'Duplicate' )
					continue;
			}
			var sCONTROL_ID = '';
			if ( !Sql.IsEmptyString(sCOMMAND_NAME) )
			{
				sCONTROL_ID = 'btnDynamicButtons_' + sCOMMAND_NAME;
			}
			else if ( !Sql.IsEmptyString(sCONTROL_TEXT) )
			{
				sCONTROL_ID = 'btnDynamicButtons_' + sCONTROL_TEXT;
				if ( sCONTROL_TEXT.indexOf('.') >= 0 )
				{
					sCONTROL_ID = sCONTROL_TEXT.split('.')[1];
					sCONTROL_ID = sCONTROL_ID.replace('LBL_', '');
					sCONTROL_ID = sCONTROL_ID.replace('_BUTTON_LABEL', '');
				}
			}
			if ( !Sql.IsEmptyString(sCONTROL_ID) )
			{
				//sCONTROL_ID = sCONTROL_ID.Trim();
				// 12/24/2012 Paul.  Use regex global replace flag. 
				sCONTROL_ID = sCONTROL_ID.replace(/\s/g, '_');
				sCONTROL_ID = sCONTROL_ID.replace(/\./g, '_');
			}
			try
			{
				var arrTEXT_FIELD = new Array();
				var objTEXT_FIELD = new Array();
				if ( sTEXT_FIELD != null )
				{
					arrTEXT_FIELD = sTEXT_FIELD.split(' ');
					objTEXT_FIELD = arrTEXT_FIELD;
					for ( var i = 0 ; i < arrTEXT_FIELD.Length; i++ )
					{
						if ( arrTEXT_FIELD[i].length > 0 )
						{
							objTEXT_FIELD[i] = '';
							if ( row != null )
							{
								if ( row[arrTEXT_FIELD[i]] != null )
									objTEXT_FIELD[i] = row[arrTEXT_FIELD[i]];
							}
						}
					}
				}
				if ( sCONTROL_TYPE == 'Button' )
				{
					if ( Sql.IsEmptyString(sCOMMAND_NAME) )
					{
						sCOMMAND_NAME = sONCLICK_SCRIPT.replace('return false;', '');
						sCOMMAND_NAME = sCOMMAND_NAME.replace('Popup();', 's.Select');
						sCOMMAND_NAME = sCOMMAND_NAME.replace('Opportunitys', 'Opportunities');
					}
					if ( sCOMMAND_NAME.indexOf('.Create') > 0 || sCOMMAND_NAME.indexOf('.Select') > 0 )
					{
						sARGUMENT_FIELD = 'ID,NAME';
					}
					var btn = document.createElement('input');
					btn.type            = 'submit';
					if ( !Sql.IsEmptyString(sCONTROL_ID) )
						btn.id              = sCONTROL_ID;
					btn.value           = '  ' + L10n.Term(sCONTROL_TEXT) + '  ';
					btn.title           = (sCONTROL_TOOLTIP.length > 0) ? L10n.Term(sCONTROL_TOOLTIP) : '';
					btn.className       = sCONTROL_CSSCLASS;
					btn.CommandName     = sCOMMAND_NAME;
					//btn.OnClientClick   = sONCLICK_SCRIPT;
					btn.style.marginRight = '3px';
					// 03/14/2014 Paul.  Allow hidden buttons to be created. 
					if ( bHIDDEN )
						btn.style.display = 'none';
					var oARGUMENT_VALUE = null;
					if ( !Sql.IsEmptyString(sARGUMENT_FIELD) )
					{
						oARGUMENT_VALUE = new Object();
						oARGUMENT_VALUE['PARENT_MODULE'] = sMODULE_NAME;
						var arrFields = sARGUMENT_FIELD.split(',');
						for ( var n in arrFields )
						{
							if ( row[arrFields[n]] != null )
							{
								oARGUMENT_VALUE[arrFields[n]] = row[arrFields[n]];
								//btn.CommandArgument = oARGUMENT_VALUE;
							}
						}
					}
					//btn.onclick = new Function('function("' + sLayoutPanel + '", "' + sCOMMAND_NAME + '", "' + sARGUMENT_VALUE + '")');
					btn.onclick = BindArguments(function(Page_Command, sLayoutPanel, sActionsPanel, sCommandName, oCommandArguments, context)
					{
						Page_Command.call(context, sLayoutPanel, sActionsPanel, sCommandName, oCommandArguments);
					}, Page_Command, sLayoutPanel, sActionsPanel, sCOMMAND_NAME, oARGUMENT_VALUE, context||this);
					pnlDynamicButtons.appendChild(btn);
				}
				else if ( sCONTROL_TYPE == 'HyperLink' )
				{
					var lnk = document.createElement('a');
					pnlDynamicButtons.appendChild(lnk);
					if ( !Sql.IsEmptyString(sCONTROL_ID) )
						lnk.id              = sCONTROL_ID;
					lnk.innerHTML       = L10n.Term(sCONTROL_TEXT);
					lnk.toolTip         = (sCONTROL_TOOLTIP.length > 0) ? L10n.Term(sCONTROL_TOOLTIP) : '';
					lnk.className       = sCONTROL_CSSCLASS;
					//lnk.href            = String_Format(sURL_FORMAT, objTEXT_FIELD);
					//btn.Command        += Page_Command;
					btn.CommandName     = sCOMMAND_NAME;
					//btn.OnClientClick   = sONCLICK_SCRIPT;
					lnk.style.marginRight = '3px';
					lnk.style.marginLeft  = '3px';
				}
				else if ( sCONTROL_TYPE == 'ButtonLink' )
				{
					var btn = document.createElement('input');
					btn.type            = 'submit';
					if ( !Sql.IsEmptyString(sCONTROL_ID) )
						btn.id              = sCONTROL_ID;
					btn.value           = '  ' + L10n.Term(sCONTROL_TEXT) + '  ';
					btn.title           = (sCONTROL_TOOLTIP.length > 0) ? L10n.Term(sCONTROL_TOOLTIP) : '';
					btn.className       = sCONTROL_CSSCLASS;
					btn.CommandName     = sCOMMAND_NAME;
					//if ( sONCLICK_SCRIPT != null && sONCLICK_SCRIPT.length > 0 )
					//	btn.OnClientClick   = String.Format(sONCLICK_SCRIPT, objTEXT_FIELD);
					//else
					//	btn.OnClientClick   = "window.location.href='" + Sql.EscapeJavaScript(String_Format(sURL_FORMAT, objTEXT_FIELD)) + "'; return false;";
					btn.style.marginRight = '3px';
					//btn.onclick = new Function('function("' + sLayoutPanel + '", "' + sCOMMAND_NAME + '", null)');
					btn.onclick = BindArguments(function(Page_Command, sLayoutPanel, sActionsPanel, sCommandName, sCommandArguments, context)
					{
						Page_Command.call(context, sLayoutPanel, sActionsPanel, sCommandName, sCommandArguments);
					}, Page_Command, sLayoutPanel, sActionsPanel, sCOMMAND_NAME, null, context||this);
					pnlDynamicButtons.appendChild(btn);
				}
			}
			catch(e)
			{
				SplendidError.SystemAlert(e, 'DynamicButtonsUI_LoadButtons ' + sCONTROL_TEXT);
			}
		}
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'DynamicButtonsUI_LoadButtons');
	}
}

function DynamicButtonsUI_Load(sLayoutPanel, sActionsPanel, sVIEW_NAME, row, Page_Command, callback, context)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		bgPage.DynamicButtons_LoadLayout(sVIEW_NAME, function(status, message)
		{
			if ( status == 1 )
			{
				// 10/03/2011 Paul.  DynamicButtons_LoadLayout returns the layout. 
				var layout = message;
				try
				{
					DynamicButtonsUI_Clear(sActionsPanel)
					//var layout = bgPage.SplendidCache.DynamicButtons(sVIEW_NAME);
					DynamicButtonsUI_LoadButtons(sLayoutPanel, sActionsPanel, layout, row, Page_Command, this);
					
					callback(1, null);
				}
				catch(e)
				{
					callback(-1, SplendidError.FormatError(e, 'DynamicButtonsUI_Load'));
				}
			}
			else
			{
				callback(status, message);
			}
		}, context||this);
	}
	catch(e)
	{
		callback(-1, SplendidError.FormatError(e, 'DynamicButtonsUI_Load'));
	}
}

