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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'USERS_FEEDS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.USERS_FEEDS';
	Create Table dbo.USERS_FEEDS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_USERS_FEEDS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, USER_ID                            uniqueidentifier not null
		, FEED_ID                            uniqueidentifier not null
		, RANK                               int not null default(0)
		)

	-- 09/10/2009 Paul.  The indexes should be fully covered. 
	create index IDX_USERS_FEEDS_USER_ID on dbo.USERS_FEEDS (USER_ID, DELETED, FEED_ID)
	create index IDX_USERS_FEEDS_FEED_ID on dbo.USERS_FEEDS (FEED_ID, DELETED, USER_ID)

	alter table dbo.USERS_FEEDS add constraint FK_USERS_FEEDS_USER_ID foreign key ( USER_ID ) references dbo.USERS ( ID )
	alter table dbo.USERS_FEEDS add constraint FK_USERS_FEEDS_FEED_ID foreign key ( FEED_ID ) references dbo.FEEDS ( ID )
  end
GO



