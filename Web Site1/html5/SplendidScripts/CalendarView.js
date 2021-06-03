/* Copyright (C) 2013 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

function CalendarView_GetCalendar(dtDATE_START, dtDATE_END, gASSIGNED_USER_ID, callback, context)
{
	var xhr = CreateSplendidRequest('Rest.svc/GetCalendar?DATE_START=' + encodeURIComponent(dtDATE_START) + '&DATE_END=' + encodeURIComponent(dtDATE_END)  + '&ASSIGNED_USER_ID=' + encodeURIComponent(gASSIGNED_USER_ID), 'GET');
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
					callback.call(context||this, -1, SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
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
		if ( e.number != -2146697208 )
			callback.call(context||this, -1, SplendidError.FormatError(e, 'CalendarView_GetCalendar'));
	}
}


