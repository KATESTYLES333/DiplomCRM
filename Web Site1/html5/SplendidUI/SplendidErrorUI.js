/* Copyright (C) 2011-2014 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var SplendidError = new Object();
SplendidError.divAlert    = null;
SplendidError.bDebug      = true;
SplendidError.arrErrorLog = new Array();
SplendidError.sLastError  = '';

SplendidError.ClearAllErrors = function()
{
	SplendidError.arrErrorLog = new Array();
	SplendidError.sLastError  = '';
}

SplendidError.ClearError = function()
{
	if ( ctlActiveMenu != null && ctlActiveMenu.divError() != null )
	{
		var lblError = document.getElementById('lblError');
		while ( lblError.childNodes.length > 0 )
		{
			lblError.removeChild(lblError.firstChild);
		}
	}
}

SplendidError.ClearAlert = function()
{
	if ( SplendidError.divAlert != null )
	{
		while ( SplendidError.divAlert.childNodes.length > 0 )
		{
			SplendidError.divAlert.removeChild(SplendidError.divAlert.firstChild);
		}
	}
}

SplendidError.SystemError = function(e, method)
{
	var message = SplendidError.FormatError(e, method);
	SplendidError.arrErrorLog.push(message);
	if ( ctlActiveMenu != null && ctlActiveMenu.divError() != null )
	{
		//ctlActiveMenu.divError.innerHTML = message;
		SplendidError.ClearError();
		var lblError = document.getElementById('lblError');
		if ( lblError != null )
			lblError.appendChild(document.createTextNode(message));
	}
	SplendidError.sLastError = message;
}

// 08/23/2014 Paul.  A status message does not add to the error log. 
SplendidError.SystemStatus = function(message)
{
	if ( ctlActiveMenu != null && ctlActiveMenu.divError() != null )
	{
		var lblError = document.getElementById('lblError');
		if ( lblError != null )
		{
			while ( lblError.childNodes.length > 0 )
			{
				lblError.removeChild(lblError.firstChild);
			}
			lblError.appendChild(document.createTextNode(message));
		}
	}
}

SplendidError.SystemMessage = function(message)
{
	//if ( message != null )
	{
		if ( message != null && message != '' )
		{
			SplendidError.arrErrorLog.push(message);
		}
		if ( ctlActiveMenu != null && ctlActiveMenu.divError() != null )
		{
			//ctlActiveMenu.divError.innerHTML = message;
			SplendidError.ClearError();
			var lblError = document.getElementById('lblError');
			if ( lblError != null )
				lblError.appendChild(document.createTextNode(message));
		}
	}
	SplendidError.sLastError = message;
}

SplendidError.SystemLog = function(message)
{
	if ( message != null && message != '' )
	{
		SplendidError.arrErrorLog.push(message);
	}
}

SplendidError.SystemDebug = function(message)
{
	//if ( message != null )
	{
		if ( message != null && message != '' )
		{
			SplendidError.arrErrorLog.push(message);
		}
		if ( ctlActiveMenu != null && ctlActiveMenu.divError() != null && bDebug )
		{
			//ctlActiveMenu.divError.innerHTML = message;
			SplendidError.ClearError();
			var lblError = document.getElementById('lblError');
			if ( lblError != null )
				lblError.appendChild(document.createTextNode(message));
		}
	}
	SplendidError.sLastError = message;
}

SplendidError.SystemAlert = function(e, method)
{
	var message = SplendidError.FormatError(e, method);
	SplendidError.arrErrorLog.push(message);
	alert(message);
}

SplendidError.FormatError = function(e, method)
{
	return e.message + '<br>\n' + dumpObj(e, method);
}

SplendidError.DebugSQL = function(sSQL)
{
	if ( Crm.Config.ToBoolean('show_sql') )
	{
		var divDebugSQL = document.getElementById('divDebugSQL');
		if ( divDebugSQL != null )
		{
			// 12/31/2014 Paul.  Firefox does not like innerText. Use createTextNode. 
			while ( divDebugSQL.childNodes.length > 0 )
				divDebugSQL.removeChild(divDebugSQL.firstChild);
			divDebugSQL.appendChild(document.createTextNode(sSQL));
		}
	}
}


