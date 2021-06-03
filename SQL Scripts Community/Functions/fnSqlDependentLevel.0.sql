if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'fnSqlDependentLevel' and ROUTINE_TYPE = 'FUNCTION')
	Drop Function dbo.fnSqlDependentLevel;
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
-- 04/22/2010 Paul.  We need to ignore foreign keys that reference self. 
Create Function dbo.fnSqlDependentLevel(@Object varchar(255), @Type varchar(2))
returns int
as
  begin
	declare @DepCount int;
	set @DepCount = 0;
	if @Type = 'U' begin -- then
		--select 'alter table '      + upper(TABLE_CONSTRAINTS.TABLE_NAME       ) + space(32-len(TABLE_CONSTRAINTS.TABLE_NAME     )) 
		--     + ' add constraint  ' + upper(TABLE_CONSTRAINTS.CONSTRAINT_NAME  ) + space(60-len(TABLE_CONSTRAINTS.CONSTRAINT_NAME)) 
		--     + ' foreign key ('    + upper(CONSTRAINT_COLUMN_USAGE.COLUMN_NAME) + ') '
		--     + ' references '      + PRIMARY_COLUMN_USAGE.TABLE_NAME + ' (' + upper(PRIMARY_COLUMN_USAGE.COLUMN_NAME) + ')' as ADD_CONSTRAINT
		select @DepCount = max(dbo.fnSqlDependentLevel(PRIMARY_KEYS.TABLE_NAME, @Type))
		  from      INFORMATION_SCHEMA.TABLE_CONSTRAINTS         TABLE_CONSTRAINTS
		 inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE   CONSTRAINT_COLUMN_USAGE
		         on CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME    = TABLE_CONSTRAINTS.CONSTRAINT_NAME
		 inner join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS   REFERENTIAL_CONSTRAINTS
		         on REFERENTIAL_CONSTRAINTS.CONSTRAINT_NAME    = TABLE_CONSTRAINTS.CONSTRAINT_NAME
		 inner join INFORMATION_SCHEMA.TABLE_CONSTRAINTS         PRIMARY_KEYS
		         on PRIMARY_KEYS.CONSTRAINT_NAME               = REFERENTIAL_CONSTRAINTS.UNIQUE_CONSTRAINT_NAME
		        and PRIMARY_KEYS.CONSTRAINT_TYPE               = 'PRIMARY KEY'
		-- inner join INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE   PRIMARY_COLUMN_USAGE
		--         on PRIMARY_COLUMN_USAGE.CONSTRAINT_NAME       = PRIMARY_KEYS.CONSTRAINT_NAME
		 where TABLE_CONSTRAINTS.CONSTRAINT_TYPE = 'FOREIGN KEY'
		-- 04/22/2010 Paul.  We need to ignore foreign keys that reference self. 
		   and TABLE_CONSTRAINTS.TABLE_NAME = @Object
		   and TABLE_CONSTRAINTS.TABLE_NAME <> PRIMARY_KEYS.TABLE_NAME;
	end -- if;
	if @DepCount is null begin -- then
		return 0;
	end -- if;
	return 1 + @DepCount;
  end
GO

Grant Execute on dbo.fnSqlDependentLevel to public
GO



