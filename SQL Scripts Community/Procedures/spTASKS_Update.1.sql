if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTASKS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTASKS_Update;
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
-- 12/29/2007 Paul.  Add TEAM_ID so that it is not updated separately. 
-- 08/21/2009 Paul.  Add support for dynamic teams. 
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
-- 09/01/2012 Paul.  Add LAST_ACTIVITY_DATE. 
Create Procedure dbo.spTASKS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(50)
	, @STATUS            nvarchar(25)
	, @DATE_TIME_DUE     datetime
	, @DATE_TIME_START   datetime
	, @PARENT_TYPE       nvarchar(25)
	, @PARENT_ID         uniqueidentifier
	, @CONTACT_ID        uniqueidentifier
	, @PRIORITY          nvarchar(25)
	, @DESCRIPTION       nvarchar(max)
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	)
as
  begin
	set nocount on
	
	declare @DATE_DUE_FLAG       bit;
	declare @DATE_START_FLAG     bit;
	declare @DATE_START          datetime;
	declare @TIME_START          datetime;
	declare @DATE_DUE            datetime;
	declare @TIME_DUE            datetime;
	declare @TEAM_SET_ID         uniqueidentifier;

	set @DATE_DUE_FLAG   = (case when @DATE_TIME_DUE   is not null then 1 else 0 end);
	set @DATE_START_FLAG = (case when @DATE_TIME_START is not null then 1 else 0 end);

	set @DATE_START = dbo.fnStoreDateOnly(@DATE_TIME_START);
	set @TIME_START = dbo.fnStoreTimeOnly(@DATE_TIME_START);
	set @DATE_DUE   = dbo.fnStoreDateOnly(@DATE_TIME_DUE  );
	set @TIME_DUE   = dbo.fnStoreTimeOnly(@DATE_TIME_DUE  );

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	if not exists(select * from TASKS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into TASKS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DATE_MODIFIED_UTC
			, ASSIGNED_USER_ID 
			, NAME             
			, STATUS           
			, DATE_DUE_FLAG    
			, DATE_DUE         
			, TIME_DUE         
			, DATE_START_FLAG  
			, DATE_START       
			, TIME_START       
			, PARENT_TYPE      
			, PARENT_ID        
			, CONTACT_ID       
			, PRIORITY         
			, DESCRIPTION      
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
			, @STATUS            
			, @DATE_DUE_FLAG     
			, @DATE_DUE          
			, @TIME_DUE          
			, @DATE_START_FLAG   
			, @DATE_START        
			, @TIME_START        
			, @PARENT_TYPE       
			, @PARENT_ID         
			, @CONTACT_ID        
			, @PRIORITY          
			, @DESCRIPTION       
			, @TEAM_ID           
			, @TEAM_SET_ID       
			);
	end else begin
		update TASKS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID  
		     , DATE_MODIFIED     =  getdate()         
		     , DATE_MODIFIED_UTC =  getutcdate()      
		     , ASSIGNED_USER_ID  = @ASSIGNED_USER_ID  
		     , NAME              = @NAME              
		     , STATUS            = @STATUS            
		     , DATE_DUE_FLAG     = @DATE_DUE_FLAG     
		     , DATE_DUE          = @DATE_DUE          
		     , TIME_DUE          = @TIME_DUE          
		     , DATE_START_FLAG   = @DATE_START_FLAG   
		     , DATE_START        = @DATE_START        
		     , TIME_START        = @TIME_START        
		     , PARENT_TYPE       = @PARENT_TYPE       
		     , PARENT_ID         = @PARENT_ID         
		     , CONTACT_ID        = @CONTACT_ID        
		     , PRIORITY          = @PRIORITY          
		     , DESCRIPTION       = @DESCRIPTION       
		     , TEAM_ID           = @TEAM_ID           
		     , TEAM_SET_ID       = @TEAM_SET_ID       
		 where ID                = @ID                ;
		
		-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
		exec dbo.spSUGARFAVORITES_UpdateName @MODIFIED_USER_ID, @ID, @NAME;
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from TASKS_CSTM where ID_C = @ID) begin -- then
			insert into TASKS_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spTASKS_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		
		-- 09/01/2012 Paul.  Add LAST_ACTIVITY_DATE. 
		if dbo.fnIsEmptyGuid(@PARENT_ID) = 0 begin -- then
			exec dbo.spPARENT_UpdateLastActivity @MODIFIED_USER_ID, @PARENT_ID, @PARENT_TYPE;
		end -- if;
	end -- if;

  end
GO

Grant Execute on dbo.spTASKS_Update to public;
GO


