if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spFIELDS_META_DATA_Insert' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spFIELDS_META_DATA_Insert;
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
-- 04/21/2006 Paul.  MASS_UPDATE was added in SugarCRM 4.0.
-- 12/16/2006 Paul.  Only add the column if the column does not exist. 
-- 12/16/2006 Paul.  Only add the meta data record if it does not already exist. 
-- 01/02/2008 Paul.  Make sure to add the column to the audit table and to add the triggers. 
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 11/17/2008 Paul.  We need to create the audit views for the workflow engine. 
Create Procedure dbo.spFIELDS_META_DATA_Insert
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(255)
	, @LABEL             nvarchar(255)
	, @LABEL_TERM        nvarchar(255)
	, @CUSTOM_MODULE     nvarchar(255)
	, @DATA_TYPE         nvarchar(255)
	, @MAX_SIZE          int
	, @REQUIRED          bit
	, @AUDITED           bit
	, @DEFAULT_VALUE     nvarchar(255)
	, @DROPDOWN_LIST     nvarchar(50)
	, @MASS_UPDATE       bit
	)
as
  begin
	set nocount on
	
	declare @CUSTOM_TABLE       nvarchar(255);
	declare @CUSTOM_COLUMN      nvarchar(255);
	declare @REQUIRED_OPTION    nvarchar(255);
	declare @TEMP_NAME          nvarchar(255);
	declare @TEMP_LABEL_TERM    nvarchar(255);
	declare @TEMP_MAX_SIZE      int;
	declare @TEMP_DROPDOWN_LIST nvarchar(50);
	declare @TABLE_NAME         nvarchar(255);

	set @TEMP_NAME          = @NAME;
	set @TEMP_LABEL_TERM    = @LABEL_TERM;
	set @TEMP_MAX_SIZE      = @MAX_SIZE;
	set @TEMP_DROPDOWN_LIST = @DROPDOWN_LIST;
	if @REQUIRED = 1 begin -- then
		set @REQUIRED_OPTION = 'required';
	end else begin
		set @REQUIRED_OPTION = 'optional';
	end -- if;
	if @DATA_TYPE = 'varchar' begin -- then
		if @TEMP_MAX_SIZE = 0 begin -- then
			set @TEMP_MAX_SIZE = 255;  -- 01/06/2006 Paul.  SugarCRM 4.0 Default size is 255. 
		end else if @TEMP_MAX_SIZE > 4000 begin -- then
			set @TEMP_MAX_SIZE = 4000;
		end -- if;
	end else if @DATA_TYPE = 'enum' begin -- then
		set @TEMP_MAX_SIZE = 50;  -- 02/02/2006 Paul.  Should be integer.  Caught when converting to DB2.
	end else begin
		set @TEMP_MAX_SIZE = null;
	end -- if;
	if @DATA_TYPE <> 'enum' begin -- then
		set @TEMP_DROPDOWN_LIST = null;
	end -- if;


	-- 01/06/2006 Paul.  Append _C to the end of custom column names. 
	set @TEMP_NAME       = dbo.fnCustomFieldName(@TEMP_NAME );
	-- 01/11/2006 Paul.  Append _C to the end of the term label. 
	-- 04/05/2006 Paul.  Bug fix, append to @TEMP_LABEL_TERM and not @LABEL. 
	-- set @TEMP_LABEL_TERM    = dbo.fnCustomFieldName(@TEMP_LABEL_TERM);
	-- 04/23/2007 Paul.  The term name needs to be derived from the field name with LBL_ prepended, otherwise the Reporting system will not be able to find the term. 
	set @TEMP_LABEL_TERM = dbo.fnCustomLabelName(@TEMP_NAME);
	set @CUSTOM_TABLE    = dbo.fnCustomTableName(@CUSTOM_MODULE);
	-- 01/06/2006 Paul.  I like all column names to be upper case. 
	set @CUSTOM_COLUMN   = upper(@TEMP_NAME);
	-- 01/06/2006 Paul.  Try and add the column first as it is more likely to fail than the insert statement. 
	-- 12/16/2006 Paul.  Only add the column if the column does not exist. 
	if not exists(select * from vwSqlColumns where ObjectName = @CUSTOM_TABLE and ColumnName = @CUSTOM_COLUMN) begin -- then
		exec dbo.spSqlTableAddColumn @CUSTOM_TABLE, @CUSTOM_COLUMN, @DATA_TYPE, @TEMP_MAX_SIZE, @REQUIRED, @DEFAULT_VALUE;
	end -- if;

	if @@ERROR = 0 begin -- then
		-- BEGIN Oracle Exception
			select @ID = ID
			  from FIELDS_META_DATA
			 where NAME          = @TEMP_NAME         
			   and CUSTOM_MODULE = @CUSTOM_MODULE
			   and DELETED       = 0             ;
		-- END Oracle Exception

		-- 12/16/2006 Paul.  Only add the meta data record if it does not already exist. 
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
			insert into FIELDS_META_DATA
				( ID               
				, CREATED_BY       
				, DATE_ENTERED     
				, MODIFIED_USER_ID 
				, DATE_MODIFIED    
				, NAME             
				, LABEL            
				, CUSTOM_MODULE    
				, DATA_TYPE        
				, MAX_SIZE         
				, REQUIRED_OPTION  
				, AUDITED          
				, DEFAULT_VALUE    
				, EXT1             
				, MASS_UPDATE      
				)
			values
				( @ID               
				, @MODIFIED_USER_ID 
				,  getdate()        
				, @MODIFIED_USER_ID 
				,  getdate()        
				, @TEMP_NAME             
				, @LABEL            
				, @CUSTOM_MODULE    
				, @DATA_TYPE        
				, @TEMP_MAX_SIZE         
				, @REQUIRED_OPTION  
				, @AUDITED          
				, @DEFAULT_VALUE    
				, @TEMP_DROPDOWN_LIST    
				, @MASS_UPDATE      
				);
		end -- if;
	end -- if;

	if @@ERROR = 0 begin -- then
		-- 01/11/2006 Paul.  Always default to english. 
		exec dbo.spTERMINOLOGY_InsertOnly @TEMP_LABEL_TERM, 'en-US', @CUSTOM_MODULE, null, null, @LABEL;
	end -- if;


	-- 01/06/2006 Paul.  If successful adding a field, then refresh all views. 
	if @@ERROR = 0 begin -- then

		-- 01/02/2008 Paul.  Make sure to add the column to the audit table and to add the triggers. 
		if exists(select * from vwSqlTablesAudited where TABLE_NAME = @CUSTOM_TABLE) begin -- then
			exec dbo.spSqlBuildAuditTable   @CUSTOM_TABLE;
			exec dbo.spSqlBuildAuditTrigger @CUSTOM_TABLE;

			-- 11/17/2008 Paul.  We need to create the audit views for the workflow engine. 
			-- BEGIN Oracle Exception
				select @TABLE_NAME = TABLE_NAME
				  from vwMODULES
				 where MODULE_NAME = @CUSTOM_MODULE;
			-- END Oracle Exception
			if @TABLE_NAME is not null begin -- then
				exec dbo.spSqlBuildAuditView @TABLE_NAME;
			end -- if;
		end -- if;
		exec dbo.spSqlRefreshAllViews ;
	end -- if;
  end
GO

Grant Execute on dbo.spFIELDS_META_DATA_Insert to public;
GO


