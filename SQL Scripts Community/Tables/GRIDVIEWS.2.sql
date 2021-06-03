
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
-- 11/22/2010 Paul.  Add support for Business Rules Framework. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'GRIDVIEWS' and COLUMN_NAME = 'PRE_LOAD_EVENT_ID') begin -- then
	print 'alter table GRIDVIEWS add PRE_LOAD_EVENT_ID uniqueidentifier null';
	alter table GRIDVIEWS add PRE_LOAD_EVENT_ID uniqueidentifier null;
end -- if;
GO

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'GRIDVIEWS' and COLUMN_NAME = 'POST_LOAD_EVENT_ID') begin -- then
	print 'alter table GRIDVIEWS add POST_LOAD_EVENT_ID uniqueidentifier null';
	alter table GRIDVIEWS add POST_LOAD_EVENT_ID uniqueidentifier null;
end -- if;
GO

-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'GRIDVIEWS' and COLUMN_NAME = 'SCRIPT') begin -- then
	print 'alter table GRIDVIEWS add SCRIPT nvarchar(max) null';
	alter table GRIDVIEWS add SCRIPT nvarchar(max) null;
end -- if;
GO


