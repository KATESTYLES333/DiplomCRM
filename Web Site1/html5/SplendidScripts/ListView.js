/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function ListView_LoadAllLayouts(callback, context)
{
	var xhr = SystemCacheRequestAll('GetAllGridViewsColumns');
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
							//alert(dumpObj(result.d, 'd'));
							GRIDVIEWS_COLUMNS = result.d.results;
							callback.call(context||this, 1, null);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadAllLayouts'));
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadAllLayouts'));
	}
}

function ListView_LoadTable(sTABLE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, callback, context)
{
	// 03/01/2013 Paul.  If sSORT_FIELD is not provided, then clear sSORT_DIRECTION. 
	if ( sSORT_FIELD === undefined || sSORT_FIELD == null || sSORT_FIELD == '' )
	{
		sSORT_FIELD     = '';
		sSORT_DIRECTION = '';
	}
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
							// 10/04/2011 Paul.  ListView_LoadTable returns the rows. 
							callback.call(context||this, 1, result.d.results);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadTable'));
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadTable'));
	}
}

// 10/24/2012 Paul.  rowSEARCH_VALUES is the field data used to build the SQL sFILTER string. 
function ListView_LoadModule(sMODULE_NAME, sSORT_FIELD, sSORT_DIRECTION, sSELECT, sFILTER, rowSEARCH_VALUES, callback, context)
{
	// 03/01/2013 Paul.  If sSORT_FIELD is not provided, then clear sSORT_DIRECTION. 
	if ( sSORT_FIELD === undefined || sSORT_FIELD == null || sSORT_FIELD == '' )
	{
		sSORT_FIELD     = '';
		sSORT_DIRECTION = '';
	}
	var xhr = CreateSplendidRequest('Rest.svc/GetModuleList?ModuleName=' + sMODULE_NAME + '&$orderby=' + encodeURIComponent(sSORT_FIELD + ' ' + sSORT_DIRECTION) + '&$select=' + escape(sSELECT) + '&$filter=' + escape(sFILTER), 'GET');
	xhr.SearchValues = rowSEARCH_VALUES;
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
							// 10/04/2011 Paul.  ListView_LoadModule returns the rows. 
							callback.call(context||this, 1, result.d.results);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadModule'));
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadModule'));
	}
}

function ListView_LoadLayout(sGRID_NAME, callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('GRIDVIEWS_COLUMNS', 'COLUMN_INDEX asc', null, 'GRID_NAME', sGRID_NAME, true);
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=GRIDVIEWS_COLUMNS&$orderby=COLUMN_INDEX asc&$filter=' + encodeURIComponent('(GRID_NAME eq \'' + sGRID_NAME + '\' and DEFAULT_VIEW eq 0)'), 'GET');
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
							SplendidCache.SetGridViewColumns(sGRID_NAME, result.d.results);
							// 10/03/2011 Paul.  ListView_LoadLayout returns the layout. 
							var layout = SplendidCache.GridViewColumns(sGRID_NAME);
							callback.call(context||this, 1, layout);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadLayout'));
				}
			}, context||this);
		}
	}
	try
	{
		// 10/03/2011 Paul.  ListView_LoadLayout returns the layout. 
		var layout = SplendidCache.GridViewColumns(sGRID_NAME);
		if ( layout == null )
		{
			xhr.send();
		}
		else
		{
			callback.call(context||this, 1, layout);
		}
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'ListView_LoadLayout'));
	}
}


