if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnDateOnly' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnDateOnly;
GO



/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved.
 * 
 * This program is free software: you can redistribute it and/or modify it under the terms of the 
 * GNU Affero General Public License as published by the Free Software Foundation, either version 3 
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>. 
 * 
 * You can contact SplendidCRM Software, Inc. at email address support@splendidcrm.com. 
 *********************************************************************************************************************/
-- 06/01/2007 Paul.  Using convert to get the date is causing a problem on international installations. 
-- The date is internally stored as two 4-byte integers.  Convert to decimal and truncate to clear the time component. 
-- http://www.sql-server-helper.com/functions/get-date-only.aspx
-- Use decimal(15,8) for better accuracy. 
-- select cast(floor(cast(cast('06/01/2007 11:59:59.998 pm' as datetime) as decimal(15,8))) as datetime)
Create Function dbo.fnDateOnly(@VALUE datetime)
returns datetime
as
  begin
	return cast(floor(cast(@VALUE as decimal(15,8))) as datetime);
  end
GO

Grant Execute on dbo.fnDateOnly to public;
GO


