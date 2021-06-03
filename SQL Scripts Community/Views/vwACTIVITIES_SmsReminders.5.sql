if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACTIVITIES_SmsReminders')
	Drop View dbo.vwACTIVITIES_SmsReminders;
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
-- 12/23/2013 Paul.  SMS_OPT_IN must be Yes in order to get an SMS reminder. 
Create View dbo.vwACTIVITIES_SmsReminders
as
select N'Meetings'                           as ACTIVITY_TYPE
     , N'Users'                              as INVITEE_TYPE
     , MEETINGS_USERS.USER_ID                as INVITEE_ID
     , MEETINGS_USERS.ACCEPT_STATUS
     , USERS.FIRST_NAME
     , USERS.LAST_NAME
     , USERS.PHONE_MOBILE
     , USERS.TIMEZONE_ID
     , USERS.LANG
     , MEETINGS.ID
     , MEETINGS.NAME
     , MEETINGS.LOCATION
     , MEETINGS.DURATION_HOURS
     , MEETINGS.DURATION_MINUTES
     , MEETINGS.DATE_START
     , MEETINGS.DATE_END
     , MEETINGS.STATUS
     , cast(null as nvarchar(25))            as DIRECTION
     , MEETINGS.REMINDER_TIME
     , MEETINGS.SMS_REMINDER_TIME
     , MEETINGS.ASSIGNED_USER_ID
     , MEETINGS.PARENT_TYPE
     , MEETINGS.PARENT_ID
     , MEETINGS.TEAM_ID
     , MEETINGS.TEAM_SET_ID
     , MEETINGS.DESCRIPTION
     , MEETINGS.DATE_ENTERED
     , MEETINGS.DATE_MODIFIED
     , MEETINGS.DATE_MODIFIED_UTC
     , MEETINGS.CREATED_BY         as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            MEETINGS
       inner join MEETINGS_USERS
               on MEETINGS_USERS.MEETING_ID          = MEETINGS.ID
              and MEETINGS_USERS.DELETED             = 0
              and MEETINGS_USERS.SMS_REMINDER_SENT   = 0
              and isnull(MEETINGS_USERS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join USERS
               on USERS.ID                           =  MEETINGS_USERS.USER_ID
              and USERS.DELETED                      =  0
              and USERS.PHONE_MOBILE is not null
              and USERS.SMS_OPT_IN                   = N'Yes'
  left outer join USERS                                USERS_ASSIGNED
               on USERS_ASSIGNED.ID                  = MEETINGS.ASSIGNED_USER_ID
  left outer join USERS                                USERS_CREATED_BY
               on USERS_CREATED_BY.ID                = MEETINGS.CREATED_BY
 where MEETINGS.DELETED             = 0
   and MEETINGS.SMS_REMINDER_TIME   > 0
   and MEETINGS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-MEETINGS.SMS_REMINDER_TIME, MEETINGS.DATE_START) and dbo.fnDateAdd_Minutes(1, MEETINGS.DATE_START)
union all
select N'Meetings'                           as ACTIVITY_TYPE
     , N'Contacts'                           as INVITEE_TYPE
     , MEETINGS_CONTACTS.CONTACT_ID          as INVITEE_ID
     , MEETINGS_CONTACTS.ACCEPT_STATUS
     , CONTACTS.FIRST_NAME
     , CONTACTS.LAST_NAME
     , CONTACTS.PHONE_MOBILE
     , cast(null as uniqueidentifier)        as TIMEZONE_ID
     , cast(null as nvarchar(10))            as LANG
     , MEETINGS.ID
     , MEETINGS.NAME
     , MEETINGS.LOCATION
     , MEETINGS.DURATION_HOURS
     , MEETINGS.DURATION_MINUTES
     , MEETINGS.DATE_START
     , MEETINGS.DATE_END
     , MEETINGS.STATUS
     , cast(null as nvarchar(25))            as DIRECTION
     , MEETINGS.REMINDER_TIME
     , MEETINGS.SMS_REMINDER_TIME
     , MEETINGS.ASSIGNED_USER_ID
     , MEETINGS.PARENT_TYPE
     , MEETINGS.PARENT_ID
     , MEETINGS.TEAM_ID
     , MEETINGS.TEAM_SET_ID
     , MEETINGS.DESCRIPTION
     , MEETINGS.DATE_ENTERED
     , MEETINGS.DATE_MODIFIED
     , MEETINGS.DATE_MODIFIED_UTC
     , MEETINGS.CREATED_BY         as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            MEETINGS
       inner join MEETINGS_CONTACTS
               on MEETINGS_CONTACTS.MEETING_ID          = MEETINGS.ID
              and MEETINGS_CONTACTS.DELETED             = 0
              and MEETINGS_CONTACTS.SMS_REMINDER_SENT   = 0
              and isnull(MEETINGS_CONTACTS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join CONTACTS
               on CONTACTS.ID                           =  MEETINGS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED                      =  0
              and CONTACTS.PHONE_MOBILE is not null
              and CONTACTS.SMS_OPT_IN                   = N'Yes'
  left outer join USERS                                   USERS_ASSIGNED
               on USERS_ASSIGNED.ID                     = MEETINGS.ASSIGNED_USER_ID
  left outer join USERS                                   USERS_CREATED_BY
               on USERS_CREATED_BY.ID                   = MEETINGS.CREATED_BY
 where MEETINGS.DELETED             = 0
   and MEETINGS.SMS_REMINDER_TIME   > 0
   and MEETINGS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-MEETINGS.SMS_REMINDER_TIME, MEETINGS.DATE_START) and dbo.fnDateAdd_Minutes(1, MEETINGS.DATE_START)
union all
select N'Meetings'                           as ACTIVITY_TYPE
     , N'Leads'                              as INVITEE_TYPE
     , MEETINGS_LEADS.LEAD_ID                as INVITEE_ID
     , MEETINGS_LEADS.ACCEPT_STATUS
     , LEADS.FIRST_NAME
     , LEADS.LAST_NAME
     , LEADS.PHONE_MOBILE
     , cast(null as uniqueidentifier)        as TIMEZONE_ID
     , cast(null as nvarchar(10))            as LANG
     , MEETINGS.ID
     , MEETINGS.NAME
     , MEETINGS.LOCATION
     , MEETINGS.DURATION_HOURS
     , MEETINGS.DURATION_MINUTES
     , MEETINGS.DATE_START
     , MEETINGS.DATE_END
     , MEETINGS.STATUS
     , cast(null as nvarchar(25))            as DIRECTION
     , MEETINGS.REMINDER_TIME
     , MEETINGS.SMS_REMINDER_TIME
     , MEETINGS.ASSIGNED_USER_ID
     , MEETINGS.PARENT_TYPE
     , MEETINGS.PARENT_ID
     , MEETINGS.TEAM_ID
     , MEETINGS.TEAM_SET_ID
     , MEETINGS.DESCRIPTION
     , MEETINGS.DATE_ENTERED
     , MEETINGS.DATE_MODIFIED
     , MEETINGS.DATE_MODIFIED_UTC
     , MEETINGS.CREATED_BY         as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            MEETINGS
       inner join MEETINGS_LEADS
               on MEETINGS_LEADS.MEETING_ID          =  MEETINGS.ID
              and MEETINGS_LEADS.DELETED             =  0
              and MEETINGS_LEADS.SMS_REMINDER_SENT   = 0
              and isnull(MEETINGS_LEADS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join LEADS
               on LEADS.ID                           =  MEETINGS_LEADS.LEAD_ID
              and LEADS.DELETED                      =  0
              and LEADS.PHONE_MOBILE is not null
              and LEADS.SMS_OPT_IN                   = N'Yes'
  left outer join USERS                                USERS_ASSIGNED
               on USERS_ASSIGNED.ID                  = MEETINGS.ASSIGNED_USER_ID
  left outer join USERS                                USERS_CREATED_BY
               on USERS_CREATED_BY.ID                = MEETINGS.CREATED_BY
 where MEETINGS.DELETED             = 0
   and MEETINGS.SMS_REMINDER_TIME   > 0
   and MEETINGS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-MEETINGS.SMS_REMINDER_TIME, MEETINGS.DATE_START) and dbo.fnDateAdd_Minutes(1, MEETINGS.DATE_START)
union all
select N'Calls'                              as ACTIVITY_TYPE
     , N'Users'                              as INVITEE_TYPE
     , CALLS_USERS.USER_ID                   as INVITEE_ID
     , CALLS_USERS.ACCEPT_STATUS
     , USERS.FIRST_NAME
     , USERS.LAST_NAME
     , USERS.PHONE_MOBILE
     , USERS.TIMEZONE_ID
     , USERS.LANG
     , CALLS.ID
     , CALLS.NAME
     , cast(null as nvarchar(50))            as LOCATION
     , CALLS.DURATION_HOURS
     , CALLS.DURATION_MINUTES
     , CALLS.DATE_START
     , CALLS.DATE_END
     , CALLS.STATUS
     , CALLS.DIRECTION
     , CALLS.REMINDER_TIME
     , CALLS.SMS_REMINDER_TIME
     , CALLS.ASSIGNED_USER_ID
     , CALLS.PARENT_TYPE
     , CALLS.PARENT_ID
     , CALLS.TEAM_ID
     , CALLS.TEAM_SET_ID
     , CALLS.DESCRIPTION
     , CALLS.DATE_ENTERED
     , CALLS.DATE_MODIFIED
     , CALLS.DATE_MODIFIED_UTC
     , CALLS.CREATED_BY            as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            CALLS
       inner join CALLS_USERS
               on CALLS_USERS.CALL_ID             = CALLS.ID
              and CALLS_USERS.DELETED             = 0
              and CALLS_USERS.SMS_REMINDER_SENT   = 0
              and isnull(CALLS_USERS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join USERS
               on USERS.ID                        = CALLS_USERS.USER_ID
              and USERS.DELETED                   = 0
              and USERS.PHONE_MOBILE is not null
              and USERS.SMS_OPT_IN                = N'Yes'
  left outer join USERS                             USERS_ASSIGNED
               on USERS_ASSIGNED.ID               = CALLS.ASSIGNED_USER_ID
  left outer join USERS                             USERS_CREATED_BY
               on USERS_CREATED_BY.ID             = CALLS.CREATED_BY
 where CALLS.DELETED             = 0
   and CALLS.SMS_REMINDER_TIME   > 0
   and CALLS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-CALLS.SMS_REMINDER_TIME, CALLS.DATE_START) and dbo.fnDateAdd_Minutes(1, CALLS.DATE_START)
union all
select N'Calls'                              as ACTIVITY_TYPE
     , N'Contacts'                           as INVITEE_TYPE
     , CALLS_CONTACTS.CONTACT_ID             as INVITEE_ID
     , CALLS_CONTACTS.ACCEPT_STATUS
     , CONTACTS.FIRST_NAME
     , CONTACTS.LAST_NAME
     , CONTACTS.PHONE_MOBILE
     , cast(null as uniqueidentifier)        as TIMEZONE_ID
     , cast(null as nvarchar(10))            as LANG
     , CALLS.ID
     , CALLS.NAME
     , cast(null as nvarchar(50))            as LOCATION
     , CALLS.DURATION_HOURS
     , CALLS.DURATION_MINUTES
     , CALLS.DATE_START
     , CALLS.DATE_END
     , CALLS.STATUS
     , CALLS.DIRECTION
     , CALLS.REMINDER_TIME
     , CALLS.SMS_REMINDER_TIME
     , CALLS.ASSIGNED_USER_ID
     , CALLS.PARENT_TYPE
     , CALLS.PARENT_ID
     , CALLS.TEAM_ID
     , CALLS.TEAM_SET_ID
     , CALLS.DESCRIPTION
     , CALLS.DATE_ENTERED
     , CALLS.DATE_MODIFIED
     , CALLS.DATE_MODIFIED_UTC
     , CALLS.CREATED_BY            as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            CALLS
       inner join CALLS_CONTACTS
               on CALLS_CONTACTS.CALL_ID             = CALLS.ID
              and CALLS_CONTACTS.DELETED             = 0
              and CALLS_CONTACTS.SMS_REMINDER_SENT   = 0
              and isnull(CALLS_CONTACTS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join CONTACTS
               on CONTACTS.ID                        = CALLS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED                   = 0
              and CONTACTS.PHONE_MOBILE is not null
              and CONTACTS.SMS_OPT_IN                = N'Yes'
  left outer join USERS                                USERS_ASSIGNED
               on USERS_ASSIGNED.ID                  = CALLS.ASSIGNED_USER_ID
  left outer join USERS                                USERS_CREATED_BY
               on USERS_CREATED_BY.ID                = CALLS.CREATED_BY
 where CALLS.DELETED             = 0
   and CALLS.SMS_REMINDER_TIME   > 0
   and CALLS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-CALLS.SMS_REMINDER_TIME, CALLS.DATE_START) and dbo.fnDateAdd_Minutes(1, CALLS.DATE_START)
union all
select N'Calls'                              as ACTIVITY_TYPE
     , N'Leads'                              as INVITEE_TYPE
     , CALLS_LEADS.LEAD_ID                   as INVITEE_ID
     , CALLS_LEADS.ACCEPT_STATUS
     , LEADS.FIRST_NAME
     , LEADS.LAST_NAME
     , LEADS.PHONE_MOBILE
     , cast(null as uniqueidentifier)        as TIMEZONE_ID
     , cast(null as nvarchar(10))            as LANG
     , CALLS.ID
     , CALLS.NAME
     , cast(null as nvarchar(50))            as LOCATION
     , CALLS.DURATION_HOURS
     , CALLS.DURATION_MINUTES
     , CALLS.DATE_START
     , CALLS.DATE_END
     , CALLS.STATUS
     , CALLS.DIRECTION
     , CALLS.REMINDER_TIME
     , CALLS.SMS_REMINDER_TIME
     , CALLS.ASSIGNED_USER_ID
     , CALLS.PARENT_TYPE
     , CALLS.PARENT_ID
     , CALLS.TEAM_ID
     , CALLS.TEAM_SET_ID
     , CALLS.DESCRIPTION
     , CALLS.DATE_ENTERED
     , CALLS.DATE_MODIFIED
     , CALLS.DATE_MODIFIED_UTC
     , CALLS.CREATED_BY            as CREATED_BY_ID
     , USERS_ASSIGNED.USER_NAME    as ASSIGNED_TO
     , USERS_CREATED_BY.USER_NAME  as CREATED_BY
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
  from            CALLS
       inner join CALLS_LEADS
               on CALLS_LEADS.CALL_ID             = CALLS.ID
              and CALLS_LEADS.DELETED             = 0
              and CALLS_LEADS.SMS_REMINDER_SENT   = 0
              and isnull(CALLS_LEADS.ACCEPT_STATUS, N'none') <> N'decline'
       inner join LEADS
               on LEADS.ID                        = CALLS_LEADS.LEAD_ID
              and LEADS.DELETED                   = 0
              and LEADS.PHONE_MOBILE is not null
              and LEADS.SMS_OPT_IN                = N'Yes'
  left outer join USERS                             USERS_ASSIGNED
               on USERS_ASSIGNED.ID               = CALLS.ASSIGNED_USER_ID
  left outer join USERS                             USERS_CREATED_BY
               on USERS_CREATED_BY.ID             = CALLS.CREATED_BY
 where CALLS.DELETED             = 0
   and CALLS.SMS_REMINDER_TIME   > 0
   and CALLS.STATUS             <> N'Held'
   and getdate() between dbo.fnDateAdd_Seconds(-CALLS.SMS_REMINDER_TIME, CALLS.DATE_START) and dbo.fnDateAdd_Minutes(1, CALLS.DATE_START)

GO

Grant Select on dbo.vwACTIVITIES_SmsReminders to public;
GO


