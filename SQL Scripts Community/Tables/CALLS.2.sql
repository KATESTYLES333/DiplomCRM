
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
-- 07/16/2005 Paul.  Version 3.0.1 added the OUTLOOK_ID field. 
-- 11/22/2006 Paul.  Add TEAM_ID for team management. 
-- 09/04/2012 Paul.  Version 6.5.4 added REPEAT fields. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'OUTLOOK_ID') begin -- then
	print 'alter table CALLS add OUTLOOK_ID nvarchar(255) null';
	alter table CALLS add OUTLOOK_ID nvarchar(255) null;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'TEAM_ID') begin -- then
	print 'alter table CALLS add TEAM_ID uniqueidentifier null';
	alter table CALLS add TEAM_ID uniqueidentifier null;

	create index IDX_CALLS_TEAM_ID on dbo.CALLS (TEAM_ID, ASSIGNED_USER_ID, DELETED, ID);
end -- if;
GO

-- 04/21/2008 Paul.  SugarCRM 5.0 has dropped TIME_START and combined it with DATE_START. 
-- We did this long ago, but we kept the use of TIME_START for compatibility with MySQL. 
-- We will eventually duplicate this behavior, but not now.  Add the fields if missing. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'TIME_START') begin -- then
	print 'alter table CALLS add TIME_START datetime null';
	alter table CALLS add TIME_START datetime null;
end -- if;
GO

-- 08/21/2009 Paul.  Add support for dynamic teams. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'TEAM_SET_ID') begin -- then
	print 'alter table CALLS add TEAM_SET_ID uniqueidentifier null';
	alter table CALLS add TEAM_SET_ID uniqueidentifier null;

	-- 08/31/2009 Paul.  Add index for TEAM_SET_ID as we will soon filter on it.
	create index IDX_CALLS_TEAM_SET_ID on dbo.CALLS (TEAM_SET_ID, ASSIGNED_USER_ID, DELETED, ID);
end -- if;
GO

-- 08/21/2009 Paul.  Add UTC date so that this module can be sync'd. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'DATE_MODIFIED_UTC') begin -- then
	print 'alter table CALLS add DATE_MODIFIED_UTC datetime null default(getutcdate())';
	alter table CALLS add DATE_MODIFIED_UTC datetime null default(getutcdate());
end -- if;
GO

-- 09/04/2012 Paul.  Version 6.5.4 added REPEAT fields. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'EMAIL_REMINDER_TIME') begin -- then
	print 'alter table CALLS add EMAIL_REMINDER_TIME int null default(-1)';
	alter table CALLS add EMAIL_REMINDER_TIME int null default(-1);
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'EMAIL_REMINDER_TIME') begin -- then
		print 'alter table CALLS_AUDIT add EMAIL_REMINDER_TIME int null';
		alter table CALLS_AUDIT add EMAIL_REMINDER_TIME int null;
	end -- if;
end -- if;
GO

-- 12/25/2012 Paul.  EMAIL_REMINDER_SENT was moved to relationship table so that it can be applied per recipient. 
if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'EMAIL_REMINDER_SENT') begin -- then
	exec dbo.spSqlDropDefaultConstraint 'CALLS', 'EMAIL_REMINDER_SENT';

	print 'alter table CALLS drop column EMAIL_REMINDER_SENT';
	alter table CALLS drop column EMAIL_REMINDER_SENT;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'EMAIL_REMINDER_SENT') begin -- then
		print 'alter table CALLS_AUDIT drop column EMAIL_REMINDER_SENT';
		alter table CALLS_AUDIT drop column EMAIL_REMINDER_SENT;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_TYPE') begin -- then
	print 'alter table CALLS add REPEAT_TYPE nvarchar(25) null';
	alter table CALLS add REPEAT_TYPE nvarchar(25) null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_TYPE') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_TYPE nvarchar(25) null';
		alter table CALLS_AUDIT add REPEAT_TYPE nvarchar(25) null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_INTERVAL') begin -- then
	print 'alter table CALLS add REPEAT_INTERVAL int null default(1)';
	alter table CALLS add REPEAT_INTERVAL int null default(1);
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_INTERVAL') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_INTERVAL int null';
		alter table CALLS_AUDIT add REPEAT_INTERVAL int null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_DOW') begin -- then
	print 'alter table CALLS add REPEAT_DOW nvarchar(7) null';
	alter table CALLS add REPEAT_DOW nvarchar(7) null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_DOW') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_DOW nvarchar(7) null';
		alter table CALLS_AUDIT add REPEAT_DOW nvarchar(7) null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_UNTIL') begin -- then
	print 'alter table CALLS add REPEAT_UNTIL datetime null';
	alter table CALLS add REPEAT_UNTIL datetime null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_UNTIL') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_UNTIL datetime null';
		alter table CALLS_AUDIT add REPEAT_UNTIL datetime null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_COUNT') begin -- then
	print 'alter table CALLS add REPEAT_COUNT int null';
	alter table CALLS add REPEAT_COUNT int null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_COUNT') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_COUNT int null';
		alter table CALLS_AUDIT add REPEAT_COUNT int null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'REPEAT_PARENT_ID') begin -- then
	print 'alter table CALLS add REPEAT_PARENT_ID uniqueidentifier null';
	alter table CALLS add REPEAT_PARENT_ID uniqueidentifier null;
	-- 03/22/2013 Paul.  Index for updating recurring events. 
	create index IDX_CALLS_REPEAT_PARENT_ID on dbo.CALLS (REPEAT_PARENT_ID, DELETED, DATE_START, DATE_MODIFIED_UTC, ID);
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'REPEAT_PARENT_ID') begin -- then
		print 'alter table CALLS_AUDIT add REPEAT_PARENT_ID uniqueidentifier null';
		alter table CALLS_AUDIT add REPEAT_PARENT_ID uniqueidentifier null;
	end -- if;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'RECURRING_SOURCE') begin -- then
	print 'alter table CALLS add RECURRING_SOURCE nvarchar(25) null';
	alter table CALLS add RECURRING_SOURCE nvarchar(25) null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'RECURRING_SOURCE') begin -- then
		print 'alter table CALLS_AUDIT add RECURRING_SOURCE nvarchar(25) null';
		alter table CALLS_AUDIT add RECURRING_SOURCE nvarchar(25) null;
	end -- if;
end -- if;
GO

-- 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'ALL_DAY_EVENT') begin -- then
	print 'alter table CALLS add ALL_DAY_EVENT bit null';
	alter table CALLS add ALL_DAY_EVENT bit null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'ALL_DAY_EVENT') begin -- then
		print 'alter table CALLS_AUDIT add ALL_DAY_EVENT bit null';
		alter table CALLS_AUDIT add ALL_DAY_EVENT bit null;
	end -- if;
end -- if;
GO

-- 09/06/2013 Paul.  Increase NAME size to 150 to support Asterisk. 
if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'NAME' and CHARACTER_MAXIMUM_LENGTH < 150) begin -- then
	print 'alter table CALLS alter column NAME nvarchar(150) null';
	alter table CALLS alter column NAME nvarchar(150) null;
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'NAME' and CHARACTER_MAXIMUM_LENGTH < 150) begin -- then
	print 'alter table CALLS_AUDIT alter column NAME nvarchar(150) null';
	alter table CALLS_AUDIT alter column NAME nvarchar(150) null;
end -- if;
GO

-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS' and COLUMN_NAME = 'SMS_REMINDER_TIME') begin -- then
	print 'alter table CALLS add SMS_REMINDER_TIME int null default(-1)';
	alter table CALLS add SMS_REMINDER_TIME int null default(-1);
end -- if;
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'CALLS_AUDIT') begin -- then
	if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'CALLS_AUDIT' and COLUMN_NAME = 'SMS_REMINDER_TIME') begin -- then
		print 'alter table CALLS_AUDIT add SMS_REMINDER_TIME int null';
		alter table CALLS_AUDIT add SMS_REMINDER_TIME int null;
	end -- if;
end -- if;
GO


