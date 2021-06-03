if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEDITVIEWS_InsertOnly' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEDITVIEWS_InsertOnly;
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
-- 12/02/2007 Paul.  Add field for data columns. 
Create Procedure dbo.spEDITVIEWS_InsertOnly
	( @NAME              nvarchar(50)
	, @MODULE_NAME       nvarchar(25)
	, @VIEW_NAME         nvarchar(50)
	, @LABEL_WIDTH       nvarchar(10)
	, @FIELD_WIDTH       nvarchar(10)
	, @DATA_COLUMNS      int = null
	)
as
  begin
	if not exists(select * from EDITVIEWS where NAME = @NAME and DELETED = 0) begin -- then
		insert into EDITVIEWS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, MODULE_NAME      
			, VIEW_NAME        
			, LABEL_WIDTH      
			, FIELD_WIDTH      
			, DATA_COLUMNS     
			)
		values 
			( newid()           
			, null              
			,  getdate()        
			, null              
			,  getdate()        
			, @NAME             
			, @MODULE_NAME      
			, @VIEW_NAME        
			, @LABEL_WIDTH      
			, @FIELD_WIDTH      
			, @DATA_COLUMNS     
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spEDITVIEWS_InsertOnly to public;
GO
 

