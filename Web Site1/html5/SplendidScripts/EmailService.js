/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function EmailService_ParseEmail(request, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('BrowserExtensions/EmailService.svc/ParseEmail');
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
						// 10/19/2011 Paul.  EmailService_ParseEmail now returns the result without the d. 
						if ( result.d !== undefined )
							callback.call(context||this, 1, result.d);
						else
							callback.call(context||this, -1, xhr.responseText);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_ParseEmail'));
				}
			});
		}
	}
	try
	{
		xhr.send('{"EmailHeaders": ' + JSON.stringify(request) + '}');
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_ParseEmail'));
	}
}

function EmailService_ArchiveEmail(request, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('BrowserExtensions/EmailService.svc/ArchiveEmail', 'POST', 'application/octet-stream');
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
						// 10/19/2011 Paul.  EmailService_ArchiveEmail now returns the result without the d. 
						if ( result.d !== undefined )
							callback.call(context||this, 1, result.d);
						else
							callback.call(context||this, -1, xhr.responseText);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_ArchiveEmail'));
				}
			});
		}
	}
	try
	{
		xhr.send(request);
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_ArchiveEmail'));
	}
}

function EmailService_SetEmailRelationships(sID, arrSelection, callback, context)
{
	if ( !ValidateCredentials() )
	{
		callback.call(context||this, -1, 'Invalid connection information.');
		return;
	}
	var xhr = CreateSplendidRequest('BrowserExtensions/EmailService.svc/SetEmailRelationships');
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
						// 10/19/2011 Paul.  EmailService_SetEmailRelationships now returns the result without the d. 
						if ( result.d !== undefined )
							callback.call(context||this, 1, result.d);
						else
							callback.call(context||this, -1, xhr.responseText);
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_SetEmailRelationships'));
				}
			});
		}
	}
	try
	{
		xhr.send('{"ID": ' + JSON.stringify(sID) + ', "Selection": ' + JSON.stringify(arrSelection) + '}');
	}
	catch(e)
	{
		callback.call(context||this, -1, SplendidError.FormatError(e, 'EmailService_SetEmailRelationships'));
	}
}


