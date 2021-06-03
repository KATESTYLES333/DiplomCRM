
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
-- 10/08/2006 Paul.  Recreate IMPORT_MAPS with NAME, SOURCE and MODULE as nvarchar fields. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'IMPORT_MAPS' and COLUMN_NAME = 'NAME' and DATA_TYPE = 'varbinary') begin -- then
	print 'Drop Table IMPORT_MAPS';
	Drop Table dbo.IMPORT_MAPS;
	
	print 'Create Table dbo.IMPORT_MAPS';
	Create Table dbo.IMPORT_MAPS
		( ID                                 uniqueidentifier not null default(newid()) constraint PK_IMPORT_MAPS primary key
		, DELETED                            bit not null default(0)
		, CREATED_BY                         uniqueidentifier null
		, DATE_ENTERED                       datetime not null default(getdate())
		, MODIFIED_USER_ID                   uniqueidentifier null
		, DATE_MODIFIED                      datetime not null default(getdate())

		, ASSIGNED_USER_ID                   uniqueidentifier null
		, NAME                               nvarchar(150) null
		, SOURCE                             nvarchar(25) null
		, MODULE                             nvarchar(25) not null
		, HAS_HEADER                         bit not null default(1)
		, IS_PUBLISHED                       bit not null default(0)
		, CONTENT                            nvarchar(max) null
		, RULES_XML                          nvarchar(max) null
		)

	create index IDX_IMPORT_MAPS_ASSIGNED_USER_ID_MODULE_NAME on dbo.IMPORT_MAPS (ASSIGNED_USER_ID, MODULE, NAME, DELETED)
end -- if;
GO

-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
if exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'IMPORT_MAPS' and COLUMN_NAME = 'CONTENT' and DATA_TYPE not in ('ntext', 'nvarchar')) begin -- then
	print 'alter table IMPORT_MAPS drop column CONTENT';
	alter table IMPORT_MAPS drop column CONTENT;
	print 'alter table IMPORT_MAPS add CONTENT nvarchar(max) null';
	alter table IMPORT_MAPS add CONTENT nvarchar(max) null;
end -- if;
GO

-- 09/17/2013 Paul.  Add Business Rules to import. 
if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'IMPORT_MAPS' and COLUMN_NAME = 'RULES_XML') begin -- then
	print 'alter table IMPORT_MAPS add RULES_XML nvarchar(max) null';
	alter table IMPORT_MAPS add RULES_XML nvarchar(max) null;
end -- if;
GO


