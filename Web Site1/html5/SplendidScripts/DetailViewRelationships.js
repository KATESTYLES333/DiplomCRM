/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function DetailViewRelationships_LoadAllLayouts(callback, context)
{
	var xhr = SystemCacheRequestAll('GetAllDetailViewsRelationships');
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
							DETAILVIEWS_RELATIONSHIPS = result.d.results;
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewRelationships_LoadAllLayouts'));
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
			callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewRelationships_LoadAllLayouts'));
	}
}

function DetailViewRelationships_LoadLayout(sDETAIL_NAME, callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('DETAILVIEWS_RELATIONSHIPS', 'RELATIONSHIP_ORDER asc', null, 'DETAIL_NAME', sDETAIL_NAME);
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=DETAILVIEWS_RELATIONSHIPS&$orderby=RELATIONSHIP_ORDER asc&$filter=' + encodeURIComponent('DETAIL_NAME eq \'' + sDETAIL_NAME + '\''), 'GET');
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
							SplendidCache.SetDetailViewRelationships(sDETAIL_NAME, result.d.results);
							// 10/04/2011 Paul.  DetailViewRelationships_LoadLayout returns the layout. 
							var layout = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewRelationships_LoadLayout'));
				}
			});
		}
	}
	try
	{
		var layout = SplendidCache.DetailViewRelationships(sDETAIL_NAME);
		if ( layout == null )
			xhr.send();
		else
			callback.call(context||this, 1, layout);
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'DetailViewRelationships_LoadLayout'));
	}
}

