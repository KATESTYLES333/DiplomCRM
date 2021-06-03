if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwLEADS_ConvertAppointment')
	Drop View dbo.vwLEADS_ConvertAppointment;
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
-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
Create View dbo.vwLEADS_ConvertAppointment
as
select cast(null as nvarchar(150))    as NAME
     , cast(0 as int)                 as DURATION_HOURS
     , cast(15 as int)                as DURATION_MINUTES
     , cast(null as bit)              as ALL_DAY_EVENT
     , cast(null as nvarchar(50))     as LOCATION
     , getdate()                      as DATE_START
     , cast(null as datetime)         as DATE_TIME
     , cast(null as datetime)         as DATE_END
     , cast(null as nvarchar(25))     as PARENT_TYPE
     , cast(null as nvarchar(25))     as STATUS
     , cast(null as nvarchar(25))     as DIRECTION
     , cast(null as int)              as REMINDER_TIME
     , cast(null as int)              as EMAIL_REMINDER_TIME
     , cast(null as int)              as SMS_REMINDER_TIME
     , cast(null as uniqueidentifier) as PARENT_ID
     , cast(null as nvarchar(150))    as PARENT_NAME
     , cast(null as uniqueidentifier) as PARENT_ASSIGNED_USER_ID
     , cast(null as nvarchar(25))     as REPEAT_TYPE
     , cast(null as int)              as REPEAT_INTERVAL
     , cast(null as nvarchar(7))      as REPEAT_DOW
     , cast(null as datetime)         as REPEAT_UNTIL
     , cast(null as int)              as REPEAT_COUNT
     , cast(null as nvarchar(25))     as RECURRING_SOURCE
     , cast(null as uniqueidentifier) as REPEAT_PARENT_ID
     , cast(null as nvarchar(150))    as REPEAT_PARENT_NAME
     , vwLEADS_Convert.*
  from vwLEADS_Convert

GO

Grant Select on dbo.vwLEADS_ConvertAppointment to public;
GO

 

