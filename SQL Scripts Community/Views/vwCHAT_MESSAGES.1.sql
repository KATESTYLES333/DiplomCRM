if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCHAT_MESSAGES')
	Drop View dbo.vwCHAT_MESSAGES;
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
Create View dbo.vwCHAT_MESSAGES
as
select CHAT_MESSAGES.ID
     , CHAT_MESSAGES.NAME
     , CHAT_MESSAGES.DATE_ENTERED
     , CHAT_MESSAGES.DATE_MODIFIED
     , CHAT_MESSAGES.DATE_MODIFIED_UTC
     , CHAT_MESSAGES.PARENT_ID
     , CHAT_MESSAGES.PARENT_TYPE
     , CHAT_MESSAGES.NOTE_ATTACHMENT_ID
     , NOTE_ATTACHMENTS.FILENAME
     , NOTE_ATTACHMENTS.FILE_EXT
     , NOTE_ATTACHMENTS.FILE_MIME_TYPE
     , isnull(datalength(NOTE_ATTACHMENTS.ATTACHMENT), 0)   as FILE_SIZE
     , (case when NOTE_ATTACHMENTS.ATTACHMENT is not null then 1 else 0 end) as ATTACHMENT_READY
     , vwPARENTS.PARENT_NAME             as PARENT_NAME
     , vwPARENTS.PARENT_ASSIGNED_USER_ID as PARENT_ASSIGNED_USER_ID
     , CHAT_CHANNELS.ID                  as CHAT_CHANNEL_ID
     , CHAT_CHANNELS.NAME                as CHAT_CHANNEL_NAME
     , CHAT_CHANNELS.ASSIGNED_USER_ID    as CHAT_CHANNEL_ASSIGNED_USER_ID
     , TEAMS.ID                          as TEAM_ID
     , TEAMS.NAME                        as TEAM_NAME
     , TEAM_SETS.ID                      as TEAM_SET_ID
     , TEAM_SETS.TEAM_SET_NAME           as TEAM_SET_NAME
     , TEAM_SETS.TEAM_SET_LIST           as TEAM_SET_LIST
     , USERS_CREATED_BY.USER_NAME        as CREATED_BY
     , USERS_MODIFIED_BY.USER_NAME       as MODIFIED_BY
     , CHAT_MESSAGES.CREATED_BY          as CREATED_BY_ID
     , CHAT_MESSAGES.MODIFIED_USER_ID
     , dbo.fnFullName(USERS_CREATED_BY.FIRST_NAME , USERS_CREATED_BY.LAST_NAME ) as CREATED_BY_NAME
     , dbo.fnFullName(USERS_MODIFIED_BY.FIRST_NAME, USERS_MODIFIED_BY.LAST_NAME) as MODIFIED_BY_NAME
     , USERS_CREATED_BY.PICTURE          as CREATED_BY_PICTURE
     , CHAT_MESSAGES.DESCRIPTION
  from            CHAT_MESSAGES
  left outer join NOTE_ATTACHMENTS
               on NOTE_ATTACHMENTS.ID      = CHAT_MESSAGES.NOTE_ATTACHMENT_ID
              and NOTE_ATTACHMENTS.DELETED = 0
  left outer join vwPARENTS
               on vwPARENTS.PARENT_ID      = CHAT_MESSAGES.PARENT_ID
  left outer join CHAT_CHANNELS
               on CHAT_CHANNELS.ID         = CHAT_MESSAGES.CHAT_CHANNEL_ID
              and CHAT_CHANNELS.DELETED    = 0
  left outer join TEAMS
               on TEAMS.ID                 = CHAT_CHANNELS.TEAM_ID
              and TEAMS.DELETED            = 0
  left outer join TEAM_SETS
               on TEAM_SETS.ID             = CHAT_CHANNELS.TEAM_SET_ID
              and TEAM_SETS.DELETED        = 0
  left outer join USERS                      USERS_CREATED_BY
               on USERS_CREATED_BY.ID      = CHAT_MESSAGES.CREATED_BY
  left outer join USERS                      USERS_MODIFIED_BY
               on USERS_MODIFIED_BY.ID     = CHAT_MESSAGES.MODIFIED_USER_ID
 where CHAT_MESSAGES.DELETED = 0

GO

Grant Select on dbo.vwCHAT_MESSAGES to public;
GO


