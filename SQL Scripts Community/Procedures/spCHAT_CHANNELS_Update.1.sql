if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCHAT_CHANNELS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCHAT_CHANNELS_Update;
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
Create Procedure dbo.spCHAT_CHANNELS_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @ASSIGNED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(150)
	, @PARENT_ID          uniqueidentifier
	, @PARENT_TYPE        nvarchar(25)
	, @TEAM_ID            uniqueidentifier
	, @TEAM_SET_LIST      varchar(8000)
	)
as
  begin
	set nocount on
	
	declare @TEAM_SET_ID uniqueidentifier;
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;
	
	if not exists(select * from CHAT_CHANNELS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into CHAT_CHANNELS
			( ID                
			, CREATED_BY        
			, DATE_ENTERED      
			, MODIFIED_USER_ID  
			, DATE_MODIFIED     
			, DATE_MODIFIED_UTC 
			, ASSIGNED_USER_ID  
			, NAME              
			, PARENT_ID         
			, PARENT_TYPE       
			, TEAM_ID           
			, TEAM_SET_ID       
			)
		values
			( @ID                
			, @MODIFIED_USER_ID  
			,  getdate()         
			, @MODIFIED_USER_ID  
			,  getdate()         
			,  getutcdate()      
			, @ASSIGNED_USER_ID  
			, @NAME              
			, @PARENT_ID         
			, @PARENT_TYPE       
			, @TEAM_ID           
			, @TEAM_SET_ID       
			);
	end else begin
		update CHAT_CHANNELS
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()         
		     , DATE_MODIFIED_UTC  =  getutcdate()      
		     , ASSIGNED_USER_ID   = @ASSIGNED_USER_ID  
		     , NAME               = @NAME              
		     , PARENT_ID          = @PARENT_ID         
		     , PARENT_TYPE        = @PARENT_TYPE       
		     , TEAM_ID            = @TEAM_ID           
		     , TEAM_SET_ID        = @TEAM_SET_ID       
		 where ID                 = @ID                ;
		
		exec dbo.spSUGARFAVORITES_UpdateName @MODIFIED_USER_ID, @ID, @NAME;
	end -- if;
  end
GO

Grant Execute on dbo.spCHAT_CHANNELS_Update to public;
GO


