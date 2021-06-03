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
-- https://dev.twitter.com/docs/platform-objects/tweets
-- drop table TWITTER_MESSAGES;
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'TWITTER_MESSAGES' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.TWITTER_MESSAGES';
	Create Table dbo.TWITTER_MESSAGES
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_TWITTER_MESSAGES primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, TEAM_ID                            uniqueidentifier null
		, TEAM_SET_ID                        uniqueidentifier null
		, NAME                               nvarchar(140) null
		, DESCRIPTION                        nvarchar(max) null
		, DATE_START                         datetime null
		, TIME_START                         datetime null
		, PARENT_TYPE                        nvarchar(25) null
		, PARENT_ID                          uniqueidentifier null
		, TYPE                               nvarchar(25) null
		, STATUS                             nvarchar(25) null
		, TWITTER_ID                         bigint null
		, TWITTER_USER_ID                    bigint null
		, TWITTER_FULL_NAME                  nvarchar(20) null
		, TWITTER_SCREEN_NAME                nvarchar(15) null
		, ORIGINAL_ID                        bigint null
		, ORIGINAL_USER_ID                   bigint null
		, ORIGINAL_FULL_NAME                 nvarchar(20) null
		, ORIGINAL_SCREEN_NAME               nvarchar(15) null
		)

	create index IDX_TWITTER_MSG_NAME        on dbo.TWITTER_MESSAGES (NAME, DELETED, ID)
	create index IDX_TWITTER_MSG_PARENT_ID   on dbo.TWITTER_MESSAGES (PARENT_ID, ID, DELETED)
	create index IDX_TWITTER_MSG_ASSIGNED_ID on dbo.TWITTER_MESSAGES (ASSIGNED_USER_ID, ID, DELETED)
	create index IDX_TWITTER_MSG_TEAM_ID     on dbo.TWITTER_MESSAGES (TEAM_ID, ASSIGNED_USER_ID, ID, DELETED)
	create index IDX_TWITTER_MSG_TEAM_SET_ID on dbo.TWITTER_MESSAGES (TEAM_SET_ID, ASSIGNED_USER_ID, ID, DELETED)
  end
GO


