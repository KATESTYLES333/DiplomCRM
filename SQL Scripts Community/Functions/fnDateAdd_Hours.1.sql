if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnDateAdd_Hours' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnDateAdd_Hours;
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
Create Function dbo.fnDateAdd_Hours(@INTERVAL int, @VALUE datetime)
returns datetime
as
  begin
	-- 11/16/2008 Paul.  No need to abbreviate. 
	return dateadd(Hour, @INTERVAL, @VALUE);
  end
GO

Grant Execute on dbo.fnDateAdd_Hours to public;
GO


