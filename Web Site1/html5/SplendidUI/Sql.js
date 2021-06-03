/* Copyright (C) 2011-2013 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var Sql = new Object();

Sql.ToString = function(o)
{
	if ( o === undefined || o == null )
		return '';
	return o;
}

Sql.ToInteger = function(n)
{
	// 04/25/2013 Paul.  ToInteger should not return NaN for an empty string. 
	if ( n === undefined || n == null || n == '' )
		return 0;
	n = parseInt(n);
	if ( isNaN(n) )
		return 0;
	return n;
}

Sql.IsInteger = function(n)
{
	if ( n === undefined || n == null || n == '' )
		return false;
	n = parseInt(n);
	return !isNaN(n);
}

Sql.ToFloat = function(f)
{
	if ( f === undefined || f == null || f == '' )
		return 0;
	f = parseFloat(f);
	if ( isNaN(f) )
		return 0.0;
	return f;
}

Sql.IsFloat = function(f)
{
	if ( f === undefined || f == null || f == '' )
		return false;
	f = parseFloat(f);
	return !isNaN(f);
}

Sql.ToDateTime = function(sDate)
{
	if ( sDate === undefined || sDate == null || sDate == '' )
		return new Date(1970, 1, 1);
	var dt = new Date(sDate);
	if ( isNaN(dt) )
		return new Date(1970, 1, 1);
	return dt;
}

Sql.IsDate = function (sDate)
{
	var dt = new Date(sDate);
	return !isNaN(dt);
}

Sql.IsEmail = function(sEmail)
{
	var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
	return filter.test(sEmail);
}

Sql.ToBoolean = function(b)
{
	if ( b === undefined || b == null )
		return false;
	return (b == 'true' || b == 'True' || b == 'on' || b == '1' || b == true || b == 1);
}

Sql.EscapeSQL = function(str)
{
	if ( str != null )
	{
		str.replace('\'', '\'\'');
	}
	return str;
}

Sql.EscapeSQLLike = function(str)
{
	if ( str != null )
	{
		str = str.replace('\\', '\\\\');
		str = str.replace('%' , '\\%');
		str = str.replace('_' , '\\_');
	}
	return str;
}

Sql.EscapeJavaScript = function(str)
{
	if ( str != null )
	{
		str = str.replace('\\', '\\\\');
		str = str.replace('\'', '\\\'');
		str = str.replace('\"', '\\\"');
		str = str.replace('\t', '\\t');
		str = str.replace('\r', '\\r');
		str = str.replace('\n', '\\n');
	}
	return str;
}

Sql.EscapeEmail = function(str)
{
	if ( str != null )
	{
		str = str.replace('&', '&amp;');
		str = str.replace('<', '&lt;');
		str = str.replace('>', '&gt;');
	}
	return str;
}

Sql.IsEmptyString = function(str)
{
	if ( str === undefined || str == null || str == '' )
		return true;
	return false;
}

Sql.ToGuid = function(g)
{
	if ( g === undefined || g == null || g == '' || typeof(g) != 'string' )
		return null;
	return g.toLowerCase();
}

Sql.IsEmptyGuid = function(str)
{
	if ( str === undefined || str == null || str == '' || str == '00000000-0000-0000-0000-000000000000' )
		return true;
	return false;
}

Sql.AppendParameter = function(cmd, sField, oValue, bOrClause)
{
	var ControlChars = { CrLf: '\r\n' };
	if ( bOrClause === undefined )
		bOrClause = false;
	// http://www.javascriptkit.com/javatutors/determinevar2.shtml
	if ( oValue == null )
	{
		if ( cmd.CommandText.length > 0 )
		{
			if ( bOrClause )
				cmd.CommandText += ' or ';
			else
				cmd.CommandText += ' and ';
		}
		cmd.CommandText += sField + ' is null' + ControlChars.CrLf;
	}
	else if ( typeof(oValue) == 'number' )
	{
		if ( cmd.CommandText.length > 0 )
		{
			if ( bOrClause )
				cmd.CommandText += ' or ';
			else
				cmd.CommandText += ' and ';
		}
		cmd.CommandText += sField + ' = ' + oValue + ControlChars.CrLf;
	}
	else if ( typeof(oValue) == 'boolean' )
	{
		if ( cmd.CommandText.length > 0 )
		{
			if ( bOrClause )
				cmd.CommandText += ' or ';
			else
				cmd.CommandText += ' and ';
		}
		cmd.CommandText += sField + ' = ' + (oValue ? '1' : '0') + ControlChars.CrLf;
	}
	else if ( typeof(oValue) == 'string' )
	{
		if ( cmd.CommandText.length > 0 )
		{
			if ( bOrClause )
				cmd.CommandText += ' or ';
			else
				cmd.CommandText += ' and ';
		}
		if ( oValue.length == 0 )
		{
			cmd.CommandText += sField + ' is null' + ControlChars.CrLf;
		}
		else
		{
			cmd.CommandText += sField + ' = \'' + this.EscapeSQL(oValue) + '\'' + ControlChars.CrLf;
		}
	}
	else if ( typeof(oValue) == 'object' )
	{
		if ( oValue.length )  // Array test. 
		{
			if ( oValue.length > 0 )
			{
				var bIncludeNull = false;
				var sValueList   = '';
				for ( var i = 0; i < oValue.length; i++ )
				{
					if ( oValue[i] == null || oValue[i].length == 0 )
					{
						bIncludeNull = true;
					}
					else
					{
						if ( sValueList.length > 0 )
							sValueList += ', ';
						sValueList += '\'' + this.EscapeSQL(oValue[i]) + '\'';
					}
				}
				if ( cmd.CommandText.length > 0 )
				{
					if ( bOrClause )
						cmd.CommandText += ' or ';
					else
						cmd.CommandText += ' and ';
				}
				if ( sValueList.length > 0 )
				{
					if ( bIncludeNull )
						cmd.CommandText += '(' + sField + ' is null or ' + sField + ' in (' + sValueList + '))' + ControlChars.CrLf;
					else
						cmd.CommandText += sField + ' in (' + sValueList + ')' + ControlChars.CrLf;
				}
				else if ( bIncludeNull )
				{
					cmd.CommandText += sField + ' is null' + ControlChars.CrLf;
				}
			}
		}
	}
}


