/*
 * This program is free software: you can redistribute it and/or modify it under the terms of the 
 * GNU Affero General Public License as published by the Free Software Foundation, either version 3 
 * of the License, or (at your option) any later version.
 * 
 * In accordance with Section 7(b) of the GNU Affero General Public License version 3, 
 * the Appropriate Legal Notices must display the following words on all interactive user interfaces: 
 * "Copyright (C) 2005-2015 SplendidCRM Software, Inc. All rights reserved."
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
		// 09/19/2016 Paul.  The entire filter string needs to be encoded. 
		var filter = '(' + sFilterField + ' eq \'' + sFilterValue + '\'';
		if ( bDefaultView !== undefined && bDefaultView === true )
			filter += ' and DEFAULT_VIEW eq 0';
		filter += ')';
		sUrl += '&$filter=' + encodeURIComponent(filter);
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
		// 09/19/2016 Paul.  The entire filter string needs to be encoded. 
		var filter = '(LANG eq \'' + sUSER_LANG + '\'';
		if ( sMODULE_NAME != null )
			filter += ' and MODULE_NAME eq \'' + sMODULE_NAME + '\'';
		else
			filter += ' and MODULE_NAME is null';
		if ( sLIST_NAME != null )
			filter += ' and LIST_NAME eq \'' + sLIST_NAME + '\'';
		else
			filter += ' and LIST_NAME is null';
		filter += ')';
		sUrl += '&$filter=' + encodeURIComponent(filter);
	}
	var xhr = CreateSplendidRequest(sUrl, 'GET');
	return xhr;
}

