if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACTIVITIES_Reminders')
	Drop View dbo.vwACTIVITIES_Reminders;
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
-- 12/24/2012 Paul.  Can't use vwACTIVITIES_List as the base because we need both USER_ID and ASSIGNED_USER_ID. 

Create View dbo.vwACTIVITIES_Reminders
as
select N'Meetings'                as ACTIVITY_TYPE
     , MEETINGS_USERS.ACCEPT_STATUS
     , MEETINGS_USERS.USER_ID
     , MEETINGS.ID
     , MEETINGS.NAME
     , MEETINGS.LOCATION
     , MEETINGS.DURATION_HOURS
     , MEETINGS.DURATION_MINUTES
     , MEETINGS.DATE_START
     , MEETINGS.DATE_END
     , MEETINGS.REMINDER_TIME
     , MEETINGS.STATUS
     , cast(null as nvarchar(25)) as DIRECTION
     , MEETINGS.ASSIGNED_USER_ID
     , MEETINGS.PARENT_TYPE
     , MEETINGS.PARENT_ID
     , MEETINGS.TEAM_ID
     , MEETINGS.TEAM_SET_ID
     , MEETINGS.DESCRIPTION
  from            MEETINGS
       inner join MEETINGS_USERS
               on MEETINGS_USERS.MEETING_ID         = MEETINGS.ID
              and MEETINGS_USERS.DELETED            = 0
              and MEETINGS_USERS.REMINDER_DISMISSED = 0
              and isnull(MEETINGS_USERS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join USERS
               on USERS.ID                          =  MEETINGS_USERS.USER_ID
              and USERS.DELETED                     =  0
 where MEETINGS.DELETED       = 0
   and MEETINGS.REMINDER_TIME > 0
   and MEETINGS.STATUS       <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-dbo.fnCONFIG_Int('reminder_max_time'), MEETINGS.DATE_START) and dbo.fnDateAdd_Minutes(5, MEETINGS.DATE_START)
union all
select N'Calls'                   as ACTIVITY_TYPE
     , CALLS_USERS.ACCEPT_STATUS
     , CALLS_USERS.USER_ID
     , CALLS.ID
     , CALLS.NAME
     , cast(null as nvarchar(50)) as LOCATION
     , CALLS.DURATION_HOURS
     , CALLS.DURATION_MINUTES
     , CALLS.DATE_START
     , CALLS.DATE_END
     , CALLS.REMINDER_TIME
     , CALLS.STATUS
     , CALLS.DIRECTION
     , CALLS.ASSIGNED_USER_ID
     , CALLS.PARENT_TYPE
     , CALLS.PARENT_ID
     , CALLS.TEAM_ID
     , CALLS.TEAM_SET_ID
     , CALLS.DESCRIPTION
  from            CALLS
       inner join CALLS_USERS
               on CALLS_USERS.CALL_ID            = CALLS.ID
              and CALLS_USERS.DELETED            = 0
              and CALLS_USERS.REMINDER_DISMISSED = 0
              and isnull(CALLS_USERS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join USERS
               on USERS.ID                       = CALLS_USERS.USER_ID
              and USERS.DELETED                  = 0
 where CALLS.DELETED       = 0
   and CALLS.REMINDER_TIME > 0
   and CALLS.STATUS       <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-dbo.fnCONFIG_Int('reminder_max_time'), CALLS.DATE_START) and dbo.fnDateAdd_Minutes(5, CALLS.DATE_START)

GO

Grant Select on dbo.vwACTIVITIES_Reminders to public;
GO


