/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function AutoComplete_ModuleMethod(sMODULE_NAME, sMETHOD, sREQUEST, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	if ( sMODULE_NAME == 'Teams' )
		sMODULE_NAME = 'Administration/Teams';
	var xhr = CreateSplendidRequest(sMODULE_NAME + '/AutoComplete.asmx/' + sMETHOD);
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
						callback.call(context||this, 1, result.d);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'AutoComplete_ModuleMethod'));
				}
			});
		}
	}
	try
	{
		xhr.send(sREQUEST);
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'AutoComplete_ModuleMethod'));
	}
}


