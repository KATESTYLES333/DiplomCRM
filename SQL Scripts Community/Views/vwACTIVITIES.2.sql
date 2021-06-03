if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACTIVITIES')
	Drop View dbo.vwACTIVITIES;
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
-- 11/22/2006 Paul.  Add TEAM_ID for team management. 
-- 11/27/2006 Paul.  Return TEAM.ID so that a deleted team will return NULL even if a value remains in the related record. 
-- 08/30/2009 Paul.  All module views must have a TEAM_SET_ID. 
-- 10/25/2010 Paul.  TEAM_SET_LIST is needed by the RulesWizard. 
-- 09/05/2013 Paul.  Add ASSIGNED_TO. 
-- 02/07/2014 Paul.  Add SMS_MESSAGES to activity list. 
-- 02/07/2014 Paul.  Add TWITTER_MESSAGES to activity list. 
-- 11/29/2014 Paul.  Add CHAT_MESSAGES to activity list. 
Create View dbo.vwACTIVITIES
as
select TASKS.ID
     , N'Tasks'                       as ACTIVITY_TYPE
     , TASKS.NAME
     , TASKS.ASSIGNED_USER_ID         as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            TASKS
  left outer join TEAMS
               on TEAMS.ID                 = TASKS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = TASKS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = TASKS.ASSIGNED_USER_ID
 where TASKS.DELETED = 0
union all
select MEETINGS.ID
     , N'Meetings'                    as ACTIVITY_TYPE
     , MEETINGS.NAME
     , MEETINGS.ASSIGNED_USER_ID      as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            MEETINGS
  left outer join TEAMS
               on TEAMS.ID                 = MEETINGS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = MEETINGS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = MEETINGS.ASSIGNED_USER_ID
 where MEETINGS.DELETED = 0
union all
select CALLS.ID
     , N'Calls'                       as ACTIVITY_TYPE
     , CALLS.NAME
     , CALLS.ASSIGNED_USER_ID         as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            CALLS
  left outer join TEAMS
               on TEAMS.ID                 = CALLS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = CALLS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = CALLS.ASSIGNED_USER_ID
 where CALLS.DELETED = 0
union all
select EMAILS.ID
     , N'Emails'                      as ACTIVITY_TYPE
     , EMAILS.NAME
     , EMAILS.ASSIGNED_USER_ID        as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            EMAILS
  left outer join TEAMS
               on TEAMS.ID                 = EMAILS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = EMAILS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = EMAILS.ASSIGNED_USER_ID
 where EMAILS.DELETED = 0
union all
select NOTES.ID
     , N'Notes'                       as ACTIVITY_TYPE
     , NOTES.NAME
     , cast(null as uniqueidentifier) as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            NOTES
  left outer join TEAMS
               on TEAMS.ID                 = NOTES.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = NOTES.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = NOTES.ASSIGNED_USER_ID
 where NOTES.DELETED = 0
union all
select SMS_MESSAGES.ID
     , N'SmsMessages'                 as ACTIVITY_TYPE
     , SMS_MESSAGES.NAME
     , SMS_MESSAGES.ASSIGNED_USER_ID  as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                       as ACTIVITY_TEAM_ID
     , TEAMS.NAME                     as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            SMS_MESSAGES
  left outer join TEAMS
               on TEAMS.ID                 = SMS_MESSAGES.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = SMS_MESSAGES.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = SMS_MESSAGES.ASSIGNED_USER_ID
 where SMS_MESSAGES.DELETED = 0
union all
select TWITTER_MESSAGES.ID
     , N'TwitterMessages'                as ACTIVITY_TYPE
     , TWITTER_MESSAGES.NAME
     , TWITTER_MESSAGES.ASSIGNED_USER_ID as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                          as ACTIVITY_TEAM_ID
     , TEAMS.NAME                        as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                      as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME           as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST           as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME          as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            TWITTER_MESSAGES
  left outer join TEAMS
               on TEAMS.ID                 = TWITTER_MESSAGES.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = TWITTER_MESSAGES.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = TWITTER_MESSAGES.ASSIGNED_USER_ID
 where TWITTER_MESSAGES.DELETED = 0
union all
select CHAT_MESSAGES.ID
     , N'ChatMessages'                   as ACTIVITY_TYPE
     , CHAT_MESSAGES.NAME
     , CHAT_CHANNELS.ASSIGNED_USER_ID    as ACTIVITY_ASSIGNED_USER_ID
     , TEAMS.ID                          as ACTIVITY_TEAM_ID
     , TEAMS.NAME                        as ACTIVITY_TEAM_NAME
     , TEAM_SETS.ID                      as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME           as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST           as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME          as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            CHAT_MESSAGES
       inner join CHAT_CHANNELS
               on CHAT_CHANNELS.ID         = CHAT_MESSAGES.CHAT_CHANNEL_ID
              and CHAT_CHANNELS.DELETED    = 0
  left outer join TEAMS
               on TEAMS.ID                 = CHAT_CHANNELS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = CHAT_CHANNELS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = CHAT_CHANNELS.ASSIGNED_USER_ID
 where CHAT_MESSAGES.DELETED = 0

GO

Grant Select on dbo.vwACTIVITIES to public;
GO


