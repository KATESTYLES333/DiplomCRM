/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var L10n = new Object();

L10n.GetList = function(sLIST_NAME)
{
	var bgPage = chrome.extension.getBackgroundPage();
	return bgPage.SplendidCache.TerminologyList(sLIST_NAME);
}

// 02/22/2013 Paul.  We need a way to get the list values, such as month names. 
L10n.GetListTerms = function(sLIST_NAME)
{
	var arrTerms = new Array();
	var bgPage   = chrome.extension.getBackgroundPage();
	var arrList  = bgPage.SplendidCache.TerminologyList(sLIST_NAME);
	if ( arrList != null )
	{
		for ( var i = 0; i < arrList.length; i++ )
		{
			var sEntryName = '.' + sLIST_NAME + '.' + arrList[i];
			var sTerm = bgPage.SplendidCache.Terminology(sEntryName);
			if ( sTerm == null )
				sTerm = '';
			arrTerms.push(sTerm);
		}
	}
	return arrTerms;
}

L10n.Term = function(sEntryName)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		var sTerm = bgPage.SplendidCache.Terminology(sEntryName);
		if ( sTerm == null )
		{
			SplendidError.SystemMessage('Term not found: ' + sEntryName);
			return sEntryName;
		}
		return sTerm;
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'L10n.Term');
	}
	return sEntryName;
}

// 10/27/2012 Paul.  It is normal for a list term to return an empty string. 
L10n.ListTerm = function(sLIST_NAME, sNAME)
{
	var sEntryName = '.' + sLIST_NAME + '.' + sNAME;
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		var sTerm = bgPage.SplendidCache.Terminology(sEntryName);
		if ( sTerm == null )
		{
			if ( !Sql.IsEmptyString(sNAME) )
			{
				SplendidError.SystemMessage('Term not found: ' + sEntryName);
				return sEntryName;
			}
			else
			{
				sTerm = '';
			}
		}
		return sTerm;
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'L10n.ListTerm');
	}
	return sEntryName;
}

L10n.TableColumnName = function(sModule, sDISPLAY_NAME)
{
	try
	{
		var bgPage = chrome.extension.getBackgroundPage();
		if (  sDISPLAY_NAME == 'ID'              
			|| sDISPLAY_NAME == 'DELETED'         
			|| sDISPLAY_NAME == 'CREATED_BY'      
			|| sDISPLAY_NAME == 'CREATED_BY_ID'   
			|| sDISPLAY_NAME == 'CREATED_BY_NAME' 
			|| sDISPLAY_NAME == 'DATE_ENTERED'    
			|| sDISPLAY_NAME == 'MODIFIED_USER_ID'
			|| sDISPLAY_NAME == 'DATE_MODIFIED'   
			|| sDISPLAY_NAME == 'DATE_MODIFIED_UTC'
			|| sDISPLAY_NAME == 'MODIFIED_BY'     
			|| sDISPLAY_NAME == 'MODIFIED_BY_ID'  
			|| sDISPLAY_NAME == 'MODIFIED_BY_NAME'
			|| sDISPLAY_NAME == 'ASSIGNED_USER_ID'
			|| sDISPLAY_NAME == 'ASSIGNED_TO'     
			|| sDISPLAY_NAME == 'ASSIGNED_TO_NAME'
			|| sDISPLAY_NAME == 'TEAM_ID'         
			|| sDISPLAY_NAME == 'TEAM_NAME'       
			|| sDISPLAY_NAME == 'TEAM_SET_ID'     
			|| sDISPLAY_NAME == 'TEAM_SET_NAME'   
			|| sDISPLAY_NAME == 'TEAM_SET_LIST'   
			|| sDISPLAY_NAME == 'ID_C'            
			|| sDISPLAY_NAME == 'AUDIT_ID'        
			|| sDISPLAY_NAME == 'AUDIT_ACTION'    
			|| sDISPLAY_NAME == 'AUDIT_DATE'      
			|| sDISPLAY_NAME == 'AUDIT_COLUMNS'   
			|| sDISPLAY_NAME == 'AUDIT_TABLE'     
			|| sDISPLAY_NAME == 'AUDIT_TOKEN'     
			)
		{
			if ( bgPage.SplendidCache.Terminology('.LBL_' + sDISPLAY_NAME) != null )
				sDISPLAY_NAME = bgPage.SplendidCache.Terminology('.LBL_' + sDISPLAY_NAME);
		}
		else
		{
			if ( bgPage.SplendidCache.Terminology(sModule + '.LBL_' + sDISPLAY_NAME) != null )
				sDISPLAY_NAME = bgPage.SplendidCache.Terminology(sModule + '.LBL_' + sDISPLAY_NAME);
		}
		return sDISPLAY_NAME;
	}
	catch(e)
	{
		SplendidError.SystemAlert(e, 'L10n.TableColumnName');
	}
	return sEntryName;
}


