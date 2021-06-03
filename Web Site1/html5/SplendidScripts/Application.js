/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function Application_Modules(callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('MODULES', 'MODULE_NAME asc');
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=MODULES&$orderby=MODULE_NAME asc', 'GET');
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
							MODULES = new Object();
							for ( var i = 0; i < result.d.results.length; i++ )
							{
								var sMODULE_NAME = result.d.results[i].MODULE_NAME;
								MODULES[sMODULE_NAME] = result.d.results[i];
							}
							//alert(dumpObj(MODULES, 'MODULES'));
							callback.call(context||this, 1, MODULES);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Modules'));
				}
			});
		}
	}
	try
	{
		if ( MODULES == null )
			xhr.send();
		else
			callback.call(context||this, 1, MODULES);
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Modules'));
	}
}

function Application_Config(callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('CONFIG', 'NAME asc', 'NAME,VALUE');
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=CONFIG&$select=NAME,VALUE&$orderby=NAME asc', 'GET');
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
							CONFIG = new Object();
							for ( var i = 0; i < result.d.results.length; i++ )
							{
								var sNAME = result.d.results[i].NAME;
								CONFIG[sNAME] = result.d.results[i].VALUE;
							}
							//alert(dumpObj(CONFIG, 'CONFIG'));
							callback.call(context||this, 1, CONFIG);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Config'));
				}
			});
		}
	}
	try
	{
		if ( CONFIG == null )
			xhr.send();
		else
			callback.call(context||this, 1, CONFIG);
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Config'));
	}
}

function Application_Teams(callback, context)
{
	// 06/11/2012 Paul.  Wrap System Cache requests for Cordova. 
	var xhr = SystemCacheRequest('TEAMS', 'NAME asc', 'ID,NAME');
	//var xhr = CreateSplendidRequest('Rest.svc/GetModuleTable?TableName=TEAMS&$select=ID,NAME&$orderby=NAME asc', 'GET');
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
							TEAMS = new Object();
							for ( var i = 0; i < result.d.results.length; i++ )
							{
								var sID = result.d.results[i].ID;
								TEAMS[sID] = result.d.results[i];
							}
							//alert(dumpObj(TEAMS, 'TEAMS'));
							callback.call(context||this, 1, TEAMS);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Teams'));
				}
			});
		}
	}
	try
	{
		// 01/01/2012 Paul.  Make this call more efficient by checking the enabled flag. 
		var b = SplendidCache.Config('enable_team_management');
		if ( b === undefined || b == null || b == false )
			b = false;
		else if ( b == 'true' || b == 'on' || b == '1' || b == true || b == 1 )
			b = true;
		
		if ( b )
		{
			if ( TEAMS == null )
				xhr.send();
			else
				callback.call(context||this, 1, TEAMS);
		}
		else
		{
			TEAMS = new Object();
			callback.call(context||this, 1, TEAMS);
		}
	}
	catch(e)
	{
		// 03/28/2012 Paul.  IE9 is returning -2146697208 when working offline. 
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'Application_Teams'));
	}
}


