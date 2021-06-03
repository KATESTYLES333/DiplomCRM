if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTAB_GROUPS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTAB_GROUPS_Update;
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
-- 02/25/2010 Paul.  We need a flag to determine if the group is displayed on the menu. 
Create Procedure dbo.spTAB_GROUPS_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(25)
	, @TITLE              nvarchar(100)
	, @ENABLED            bit
	, @GROUP_ORDER        int
	, @GROUP_MENU         bit
	)
as
  begin
	set nocount on
	
	-- BEGIN Oracle Exception
		select @ID = ID
		  from TAB_GROUPS
		 where NAME         = @NAME
		   and DELETED      = 0    ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into TAB_GROUPS
			( ID                
			, CREATED_BY        
			, DATE_ENTERED      
			, MODIFIED_USER_ID  
			, DATE_MODIFIED     
			, DATE_MODIFIED_UTC 
			, NAME              
			, TITLE             
			, ENABLED           
			, GROUP_ORDER       
			, GROUP_MENU        
			)
		values 	( @ID                
			, @MODIFIED_USER_ID  
			,  getdate()         
			, @MODIFIED_USER_ID  
			,  getdate()         
			,  getutcdate()      
			, @NAME              
			, @TITLE             
			, @ENABLED           
			, @GROUP_ORDER       
			, @GROUP_MENU        
			);
	end else begin
		update TAB_GROUPS
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()         
		     , DATE_MODIFIED_UTC  =  getutcdate()      
		     , TITLE              = @TITLE             
		     , ENABLED            = @ENABLED           
		     , GROUP_ORDER        = @GROUP_ORDER       
		     , GROUP_MENU         = GROUP_MENU        
		 where ID                 = @ID                ;
	end -- if;
  end
GO

Grant Execute on dbo.spTAB_GROUPS_Update to public;
GO


