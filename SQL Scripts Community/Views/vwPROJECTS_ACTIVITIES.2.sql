if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROJECTS_ACTIVITIES')
	Drop View dbo.vwPROJECTS_ACTIVITIES;
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
-- 02/01/2006 Paul.  DB2 does not like to return NULL.  So cast NULL to the correct data type. 
-- 04/21/2006 Paul.  Email does have a status, make sure to return it.
-- 07/15/2006 Paul.  There is a relationship with tasks. 
-- 08/07/2006 Paul.  Notes has a direct relationship with Contacts.
-- 11/27/2006 Paul.  Add TEAM_ID. 
-- 11/27/2006 Paul.  Return TEAM.ID so that a deleted team will return NULL even if a value remains in the related record. 
-- 10/28/2007 Paul.  Include emails from the relationship table. 
-- 09/09/2008 Paul.  Always use a union all to prevent the implied distinct. 
-- 09/30/2008 Jake.  The join on the tasks table had parent type of ProjectTask not Project.
-- 02/13/2009 Paul.  Notes should assume the ownership of the parent record. 
-- 08/30/2009 Paul.  All module views must have a TEAM_SET_ID. 
-- 10/25/2010 Paul.  TEAM_SET_LIST is needed by the RulesWizard. 
-- 09/05/2013 Paul.  Add ASSIGNED_TO. 
-- 09/26/2013 Paul.  Add SMS_MESSAGES to activity list. 
-- 10/22/2013 Paul.  Add TWITTER_MESSAGES to activity list. 
-- 11/29/2014 Paul.  Add CHAT_MESSAGES to activity list. 
Create View dbo.vwPROJECTS_ACTIVITIES
as
select TASKS.ID
     , TASKS.ID                       as ACTIVITY_ID
     , N'Tasks'                       as ACTIVITY_TYPE
     , TASKS.NAME                     as ACTIVITY_NAME
     , TASKS.ASSIGNED_USER_ID         as ACTIVITY_ASSIGNED_USER_ID
     , TASKS.STATUS                   as STATUS
     , N'none'                        as DIRECTION
     , TASKS.DATE_DUE                 as DATE_DUE
     , TASKS.DATE_MODIFIED            as DATE_MODIFIED
     , TASKS.DATE_MODIFIED_UTC        as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , (case TASKS.STATUS when N'Not Started'   then 1
                          when N'In Progress'   then 1
                          when N'Pending Input' then 1
        else 0
        end)                          as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join TASKS
               on TASKS.PARENT_ID          = PROJECT.ID
              and TASKS.PARENT_TYPE        = N'Project'
              and TASKS.DELETED            = 0
  left outer join TEAMS
               on TEAMS.ID                 = TASKS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = TASKS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join CONTACTS
               on CONTACTS.ID              = TASKS.CONTACT_ID
              and CONTACTS.DELETED         = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = TASKS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select MEETINGS.ID
     , MEETINGS.ID                    as ACTIVITY_ID
     , N'Meetings'                    as ACTIVITY_TYPE
     , MEETINGS.NAME                  as ACTIVITY_NAME
     , MEETINGS.ASSIGNED_USER_ID      as ACTIVITY_ASSIGNED_USER_ID
     , MEETINGS.STATUS                as STATUS
     , N'none'                        as DIRECTION
     , MEETINGS.DATE_START            as DATE_DUE
     , MEETINGS.DATE_MODIFIED         as DATE_MODIFIED
     , MEETINGS.DATE_MODIFIED_UTC     as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , (case MEETINGS.STATUS when N'Planned' then 1
        else 0
        end)                          as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join MEETINGS
               on MEETINGS.PARENT_ID           = PROJECT.ID
              and MEETINGS.PARENT_TYPE         = N'Project'
              and MEETINGS.DELETED             = 0
  left outer join TEAMS
               on TEAMS.ID                     = MEETINGS.TEAM_ID
              and TEAMS.DELETED                = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID                 = MEETINGS.TEAM_SET_ID
              and TEAM_SETS.DELETED            = 0
  left outer join MEETINGS_CONTACTS
               on MEETINGS_CONTACTS.MEETING_ID = MEETINGS.ID
              and MEETINGS_CONTACTS.DELETED    = 0
  left outer join CONTACTS
               on CONTACTS.ID                  = MEETINGS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED             = 0
  left outer join USERS                          USERS_ASSIGNED
               on USERS_ASSIGNED.ID            = MEETINGS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select CALLS.ID
     , CALLS.ID                       as ACTIVITY_ID
     , N'Calls'                       as ACTIVITY_TYPE
     , CALLS.NAME                     as ACTIVITY_NAME
     , CALLS.ASSIGNED_USER_ID         as ACTIVITY_ASSIGNED_USER_ID
     , CALLS.STATUS                   as STATUS
     , CALLS.DIRECTION                as DIRECTION
     , CALLS.DATE_START               as DATE_DUE
     , CALLS.DATE_MODIFIED            as DATE_MODIFIED
     , CALLS.DATE_MODIFIED_UTC        as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , (case CALLS.STATUS when N'Planned' then 1
        else 0
        end)                          as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join CALLS
               on CALLS.PARENT_ID        = PROJECT.ID
              and CALLS.PARENT_TYPE      = N'Project'
              and CALLS.DELETED          = 0
  left outer join TEAMS
               on TEAMS.ID               = CALLS.TEAM_ID
              and TEAMS.DELETED          = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID           = CALLS.TEAM_SET_ID
              and TEAM_SETS.DELETED      = 0
  left outer join CALLS_CONTACTS
               on CALLS_CONTACTS.CALL_ID = CALLS.ID
              and CALLS_CONTACTS.DELETED = 0
  left outer join CONTACTS
               on CONTACTS.ID            = CALLS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED       = 0
  left outer join USERS                    USERS_ASSIGNED
               on USERS_ASSIGNED.ID      = CALLS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select EMAILS.ID
     , EMAILS.ID                      as ACTIVITY_ID
     , N'Emails'                      as ACTIVITY_TYPE
     , EMAILS.NAME                    as ACTIVITY_NAME
     , EMAILS.ASSIGNED_USER_ID        as ACTIVITY_ASSIGNED_USER_ID
     , EMAILS.STATUS                  as STATUS
     , N'none'                        as DIRECTION
     , EMAILS.DATE_START              as DATE_DUE
     , EMAILS.DATE_START              as DATE_MODIFIED
     , EMAILS.DATE_MODIFIED_UTC       as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , 0                              as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join EMAILS
               on EMAILS.PARENT_ID           = PROJECT.ID
              and EMAILS.PARENT_TYPE         = N'Project'
              and EMAILS.DELETED             = 0
  left outer join EMAILS_PROJECTS
               on EMAILS_PROJECTS.PROJECT_ID = PROJECT.ID
              and EMAILS_PROJECTS.EMAIL_ID   = EMAILS.ID
              and EMAILS_PROJECTS.DELETED    = 0
  left outer join TEAMS
               on TEAMS.ID                   = EMAILS.TEAM_ID
              and TEAMS.DELETED              = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID               = EMAILS.TEAM_SET_ID
              and TEAM_SETS.DELETED          = 0
  left outer join EMAILS_CONTACTS
               on EMAILS_CONTACTS.EMAIL_ID   = EMAILS.ID
              and EMAILS_CONTACTS.DELETED    = 0
  left outer join CONTACTS
               on CONTACTS.ID                = EMAILS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED           = 0
  left outer join USERS                        USERS_ASSIGNED
               on USERS_ASSIGNED.ID          = EMAILS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
   and EMAILS_PROJECTS.ID is null
union all
select EMAILS.ID
     , EMAILS.ID                      as ACTIVITY_ID
     , N'Emails'                      as ACTIVITY_TYPE
     , EMAILS.NAME                    as ACTIVITY_NAME
     , EMAILS.ASSIGNED_USER_ID        as ACTIVITY_ASSIGNED_USER_ID
     , EMAILS.STATUS                  as STATUS
     , N'none'                        as DIRECTION
     , EMAILS.DATE_START              as DATE_DUE
     , EMAILS.DATE_START              as DATE_MODIFIED
     , EMAILS.DATE_MODIFIED_UTC       as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , 0                              as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join EMAILS_PROJECTS
               on EMAILS_PROJECTS.PROJECT_ID = PROJECT.ID
              and EMAILS_PROJECTS.DELETED    = 0
       inner join EMAILS
               on EMAILS.ID                = EMAILS_PROJECTS.EMAIL_ID
              and EMAILS.DELETED           = 0
  left outer join TEAMS
               on TEAMS.ID                 = EMAILS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = EMAILS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join EMAILS_CONTACTS
               on EMAILS_CONTACTS.EMAIL_ID = EMAILS.ID
              and EMAILS_CONTACTS.DELETED  = 0
  left outer join CONTACTS
               on CONTACTS.ID              = EMAILS_CONTACTS.CONTACT_ID
              and CONTACTS.DELETED         = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = EMAILS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select NOTES.ID
     , NOTES.ID                       as ACTIVITY_ID
     , N'Notes'                       as ACTIVITY_TYPE
     , NOTES.NAME                     as ACTIVITY_NAME
     , PROJECT.ASSIGNED_USER_ID       as ACTIVITY_ASSIGNED_USER_ID
     , N'Note'                        as STATUS
     , N'none'                        as DIRECTION
     , cast(null as datetime)         as DATE_DUE
     , NOTES.DATE_MODIFIED            as DATE_MODIFIED
     , NOTES.DATE_MODIFIED_UTC        as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , 0                              as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join NOTES
               on NOTES.PARENT_ID          = PROJECT.ID
              and NOTES.PARENT_TYPE        = N'Project'
              and NOTES.DELETED            = 0
  left outer join TEAMS
               on TEAMS.ID                 = NOTES.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = NOTES.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join CONTACTS
               on CONTACTS.ID              = NOTES.CONTACT_ID
              and CONTACTS.DELETED         = 0
  left outer join USERS                      USERS_ASSIGNED
               on USERS_ASSIGNED.ID        = NOTES.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select SMS_MESSAGES.ID
     , SMS_MESSAGES.ID                as ACTIVITY_ID
     , N'SmsMessages'                 as ACTIVITY_TYPE
     , SMS_MESSAGES.NAME              as ACTIVITY_NAME
     , SMS_MESSAGES.ASSIGNED_USER_ID  as ACTIVITY_ASSIGNED_USER_ID
     , SMS_MESSAGES.STATUS            as STATUS
     , (case SMS_MESSAGES.TYPE when N'inbound' then N'Inbound' else N'Outbound' end) as DIRECTION
     , SMS_MESSAGES.DATE_START        as DATE_DUE
     , SMS_MESSAGES.DATE_START        as DATE_MODIFIED
     , SMS_MESSAGES.DATE_MODIFIED_UTC as DATE_MODIFIED_UTC
     , PROJECT.ID                     as PROJECT_ID
     , PROJECT.NAME                   as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID       as PROJECT_ASSIGNED_USER_ID
     , CONTACTS.ID                    as CONTACT_ID
     , CONTACTS.ASSIGNED_USER_ID      as CONTACT_ASSIGNED_USER_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME) as CONTACT_NAME
     , 0                              as IS_OPEN
     , TEAMS.ID                       as TEAM_ID
     , TEAMS.NAME                     as TEAM_NAME
     , TEAM_SETS.ID                   as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME        as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST        as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME       as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join SMS_MESSAGES
               on SMS_MESSAGES.PARENT_ID     = PROJECT.ID
              and SMS_MESSAGES.PARENT_TYPE   = N'Project'
              and SMS_MESSAGES.DELETED       = 0
  left outer join TEAMS
               on TEAMS.ID                   = SMS_MESSAGES.TEAM_ID
              and TEAMS.DELETED              = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID               = SMS_MESSAGES.TEAM_SET_ID
              and TEAM_SETS.DELETED          = 0
  left outer join CONTACTS
               on CONTACTS.ID                = SMS_MESSAGES.TO_ID
              and CONTACTS.DELETED           = 0
  left outer join USERS                        USERS_ASSIGNED
               on USERS_ASSIGNED.ID          = SMS_MESSAGES.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select TWITTER_MESSAGES.ID
     , TWITTER_MESSAGES.ID                as ACTIVITY_ID
     , N'TwitterMessages'                 as ACTIVITY_TYPE
     , TWITTER_MESSAGES.NAME              as ACTIVITY_NAME
     , TWITTER_MESSAGES.ASSIGNED_USER_ID  as ACTIVITY_ASSIGNED_USER_ID
     , TWITTER_MESSAGES.STATUS            as STATUS
     , (case TWITTER_MESSAGES.TYPE when N'inbound' then N'Inbound' else N'Outbound' end) as DIRECTION
     , TWITTER_MESSAGES.DATE_START        as DATE_DUE
     , TWITTER_MESSAGES.DATE_START        as DATE_MODIFIED
     , TWITTER_MESSAGES.DATE_MODIFIED_UTC as DATE_MODIFIED_UTC
     , PROJECT.ID                         as PROJECT_ID
     , PROJECT.NAME                       as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID           as PROJECT_ASSIGNED_USER_ID
     , cast(null as uniqueidentifier)     as CONTACT_ID
     , cast(null as uniqueidentifier)     as CONTACT_ASSIGNED_USER_ID
     , cast(null as nvarchar(200))        as CONTACT_NAME
     , 0                                  as IS_OPEN
     , TEAMS.ID                           as TEAM_ID
     , TEAMS.NAME                         as TEAM_NAME
     , TEAM_SETS.ID                       as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME            as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST            as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME           as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join TWITTER_MESSAGES
               on TWITTER_MESSAGES.PARENT_ID   = PROJECT.ID
              and TWITTER_MESSAGES.PARENT_TYPE = N'Project'
              and TWITTER_MESSAGES.DELETED     = 0
  left outer join TEAMS
               on TEAMS.ID                     = TWITTER_MESSAGES.TEAM_ID
              and TEAMS.DELETED                = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID                 = TWITTER_MESSAGES.TEAM_SET_ID
              and TEAM_SETS.DELETED            = 0
  left outer join USERS                          USERS_ASSIGNED
               on USERS_ASSIGNED.ID            = TWITTER_MESSAGES.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select CHAT_MESSAGES.ID
     , CHAT_MESSAGES.ID                   as ACTIVITY_ID
     , N'ChatMessages'                    as ACTIVITY_TYPE
     , CHAT_MESSAGES.NAME                 as ACTIVITY_NAME
     , CHAT_CHANNELS.ASSIGNED_USER_ID     as ACTIVITY_ASSIGNED_USER_ID
     , cast(null as nvarchar(25))         as STATUS
     , N'none'                            as DIRECTION
     , cast(null as datetime)             as DATE_DUE
     , CHAT_MESSAGES.DATE_ENTERED         as DATE_MODIFIED
     , CHAT_MESSAGES.DATE_MODIFIED_UTC    as DATE_MODIFIED_UTC
     , PROJECT.ID                         as PROJECT_ID
     , PROJECT.NAME                       as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID           as PROJECT_ASSIGNED_USER_ID
     , cast(null as uniqueidentifier)     as CONTACT_ID
     , cast(null as uniqueidentifier)     as CONTACT_ASSIGNED_USER_ID
     , cast(null as nvarchar(200))        as CONTACT_NAME
     , 0                                  as IS_OPEN
     , TEAMS.ID                           as TEAM_ID
     , TEAMS.NAME                         as TEAM_NAME
     , TEAM_SETS.ID                       as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME            as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST            as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME           as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join CHAT_MESSAGES
               on CHAT_MESSAGES.PARENT_ID      = PROJECT.ID
              and CHAT_MESSAGES.PARENT_TYPE    = N'Project'
              and CHAT_MESSAGES.DELETED        = 0
       inner join CHAT_CHANNELS
               on CHAT_CHANNELS.ID             = CHAT_MESSAGES.CHAT_CHANNEL_ID
              and CHAT_CHANNELS.DELETED        = 0
  left outer join TEAMS
               on TEAMS.ID                     = CHAT_CHANNELS.TEAM_ID
              and TEAMS.DELETED                = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID                 = CHAT_CHANNELS.TEAM_SET_ID
              and TEAM_SETS.DELETED            = 0
  left outer join USERS                          USERS_ASSIGNED
               on USERS_ASSIGNED.ID            = CHAT_CHANNELS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0
union all
select CHAT_MESSAGES.ID
     , CHAT_MESSAGES.ID                   as ACTIVITY_ID
     , N'ChatMessages'                    as ACTIVITY_TYPE
     , CHAT_MESSAGES.NAME                 as ACTIVITY_NAME
     , CHAT_CHANNELS.ASSIGNED_USER_ID     as ACTIVITY_ASSIGNED_USER_ID
     , cast(null as nvarchar(25))         as STATUS
     , N'none'                            as DIRECTION
     , cast(null as datetime)             as DATE_DUE
     , CHAT_MESSAGES.DATE_ENTERED         as DATE_MODIFIED
     , CHAT_MESSAGES.DATE_MODIFIED_UTC    as DATE_MODIFIED_UTC
     , PROJECT.ID                         as PROJECT_ID
     , PROJECT.NAME                       as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID           as PROJECT_ASSIGNED_USER_ID
     , cast(null as uniqueidentifier)     as CONTACT_ID
     , cast(null as uniqueidentifier)     as CONTACT_ASSIGNED_USER_ID
     , cast(null as nvarchar(200))        as CONTACT_NAME
     , 0                                  as IS_OPEN
     , TEAMS.ID                           as TEAM_ID
     , TEAMS.NAME                         as TEAM_NAME
     , TEAM_SETS.ID                       as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME            as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST            as TEAM_SET_LIST
     , USERS_ASSIGNED.USER_NAME           as ASSIGNED_TO
     , dbo.fnFullName(USERS_ASSIGNED.FIRST_NAME   , USERS_ASSIGNED.LAST_NAME   ) as ASSIGNED_TO_NAME
  from            PROJECT
       inner join CHAT_CHANNELS
               on CHAT_CHANNELS.PARENT_ID      = PROJECT.ID
              and CHAT_CHANNELS.PARENT_TYPE    = N'Project'
              and CHAT_CHANNELS.DELETED        = 0
       inner join CHAT_MESSAGES
               on CHAT_MESSAGES.CHAT_CHANNEL_ID= CHAT_CHANNELS.ID
              and CHAT_MESSAGES.DELETED        = 0
  left outer join TEAMS
               on TEAMS.ID                     = CHAT_CHANNELS.TEAM_ID
              and TEAMS.DELETED                = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID                 = CHAT_CHANNELS.TEAM_SET_ID
              and TEAM_SETS.DELETED            = 0
  left outer join USERS                          USERS_ASSIGNED
               on USERS_ASSIGNED.ID            = CHAT_CHANNELS.ASSIGNED_USER_ID
 where PROJECT.DELETED = 0

GO

Grant Select on dbo.vwPROJECTS_ACTIVITIES to public;
GO


