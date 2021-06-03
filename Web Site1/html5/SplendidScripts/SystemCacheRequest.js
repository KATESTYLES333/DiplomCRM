/* Copyright (C) 2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

 function SystemCacheRequestAll(sMethodName)
{
	var sUrl = 'Rest.svc/' + sMethodName;
	var xhr = CreateSplendidRequest(sUrl, 'GET');
	return xhr;
}

// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
function SystemCacheRequest(sTableName, sOrderBy, sSelectFields, sFilterField, sFilterValue, bDefaultView)
{
	var sUrl = 'Rest.svc/GetModuleTable?TableName=' + sTableName;
	if ( sSelectFields !== undefined && sSelectFields != null )
		sUrl += '&$select=' + sSelectFields;
	if ( sOrderBy !== undefined && sOrderBy != null )
		sUrl += '&$orderby=' + sOrderBy;
	if ( sFilterField !== undefined && sFilterField != null && sFilterValue !== undefined && sFilterValue != null )
	{
		sUrl += '&$filter=' + encodeURIComponent('(' + sFilterField + ' eq \'' + sFilterValue + '\'');
		if ( bDefaultView !== undefined && bDefaultView === true )
			sUrl += ' and DEFAULT_VIEW eq 0';
		sUrl += ')';
	}
	var xhr = CreateSplendidRequest(sUrl, 'GET');
	return xhr;
}

// 06/11/2012 Paul.  Wrap Terminology requests for Cordova. 
function TerminologyRequest(sMODULE_NAME, sLIST_NAME, sOrderBy, sUSER_LANG)
{
	var sUrl = 'Rest.svc/GetModuleTable?TableName=TERMINOLOGY';
	if ( sOrderBy !== undefined && sOrderBy != null )
		sUrl += '&$orderby=' + sOrderBy;
	if ( sMODULE_NAME == null && sLIST_NAME == null )
	{
		sUrl += '&$filter=' + encodeURIComponent('(LANG eq \'' + sUSER_LANG + '\' and (MODULE_NAME is null or MODULE_NAME eq \'Teams\' or NAME eq \'LBL_NEW_FORM_TITLE\'))');
	}
	else
	{
		sUrl += '&$filter=' + encodeURIComponent('(LANG eq \'' + sUSER_LANG + '\'');
		if ( sMODULE_NAME != null )
			sUrl += ' and MODULE_NAME eq \'' + sMODULE_NAME + '\'';
		else
			sUrl += ' and MODULE_NAME is null';
		if ( sLIST_NAME != null )
			sUrl += ' and LIST_NAME eq \'' + sLIST_NAME + '\'';
		else
			sUrl += ' and LIST_NAME is null';
		sUrl += ')';
	}
	var xhr = CreateSplendidRequest(sUrl, 'GET');
	return xhr;
}


