/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function EditView_LoadAllLayouts(callback, context)
{
	var xhr = SystemCacheRequestAll('GetAllEditViewsFields');
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
							EDITVIEWS_FIELDS = result.d.results;
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadAllLayouts'));
				}
			});
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadAllLayouts'));
	}
}

function EditView_LoadItem(sMODULE_NAME, sID, callback, context)
{
	var xhr = CreateSplendidRequest('Rest.svc/GetModuleItem?ModuleName=' + sMODULE_NAME + '&ID=' + sID, 'GET');
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
							// 10/04/2011 Paul.  EditViewUI.LoadItem returns the row. 
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadItem'));
				}
			});
		}
	}
	try
	{
		// 10/07/2011 Paul.  We want to allow an empty ID to return a valid empty result. 
		// 10/10/2011 Paul.  Sql object is not available in the background page. 
		if ( sID === undefined || sID == null || sID == '' )
		{
			var row = new Object();
			row['ID'] = null;
			callback.call(context||this, 1, row);
		}
		else
		{
			xhr.send();
		}
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadItem'));
	}
}

function EditView_LoadLayout(sEDIT_NAME, callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('EDITVIEWS_FIELDS', 'FIELD_INDEX asc', null, 'EDIT_NAME', sEDIT_NAME, true);
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=EDITVIEWS_FIELDS&$orderby=FIELD_INDEX asc&$filter=' + encodeURIComponent('(EDIT_NAME eq \'' + sEDIT_NAME + '\' and DEFAULT_VIEW eq 0)'), 'GET');
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
							SplendidCache.SetEditViewFields(sEDIT_NAME, result.d.results);
							// 10/04/2011 Paul.  EditView_LoadLayout returns the layout. 
							var layout = SplendidCache.EditViewFields(sEDIT_NAME);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadLayout'));
				}
			});
		}
	}
	try
	{
		// 10/03/2011 Paul.  EditView_LoadLayout returns the layout. 
		var layout = SplendidCache.EditViewFields(sEDIT_NAME);
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'EditView_LoadLayout'));
	}
}

