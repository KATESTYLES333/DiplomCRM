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
-- drop table dbo.OUTBOUND_SMS;
if not exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'OUTBOUND_SMS' and TABLE_TYPE = 'BASE TABLE')
  begin
	print 'Create Table dbo.OUTBOUND_SMS';
	Create Table dbo.OUTBOUND_SMS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_OUTBOUND_SMS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())
		, DATE_MODIFIED_UTC                  datetime null default(getutcdate())

		, NAME                               nvarchar(60) null
		, USER_ID                            uniqueidentifier null
		, FROM_NUMBER                        nvarchar(100) null
		)

	create index IDX_OUTBOUND_SMS_USER_ID on dbo.OUTBOUND_SMS (USER_ID, DELETED, ID)
  end
GO


