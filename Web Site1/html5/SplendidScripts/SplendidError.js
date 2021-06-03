/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var SplendidError = new Object();
SplendidError.bDebug = true;

SplendidError.SystemError = function(e, method)
{
	var message = this.FormatError(e, method);
}

SplendidError.SystemMessage = function(message)
{
}

SplendidError.SystemLog = function(message)
{
}

SplendidError.SystemAlert = function(e, method)
{
	alert(this.FormatError(e, method));
}

SplendidError.FormatError = function(e, method)
{
	return e.message + '\n<br>' + dumpObj(e, method);
}


