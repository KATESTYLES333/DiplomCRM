if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spFEEDS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spFEEDS_Update;
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
Create Procedure dbo.spFEEDS_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @TITLE             nvarchar(100)
	, @DESCRIPTION       nvarchar(max)
	, @URL               nvarchar(255)
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	)
as
  begin
	set nocount on

	declare @TEAM_SET_ID         uniqueidentifier;

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	if not exists(select * from FEEDS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into FEEDS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DATE_MODIFIED_UTC
			, ASSIGNED_USER_ID 
			, TITLE            
			, DESCRIPTION      
			, URL              
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
			, @TITLE             
			, @DESCRIPTION       
			, @URL               
			, @TEAM_ID           
			, @TEAM_SET_ID       
			);
	end else begin
		update FEEDS
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID  
		     , DATE_MODIFIED     =  getdate()         
		     , DATE_MODIFIED_UTC =  getutcdate()      
		     , ASSIGNED_USER_ID  = @ASSIGNED_USER_ID  
		     , TITLE             = @TITLE             
		     , DESCRIPTION       = @DESCRIPTION       
		     , URL               = @URL               
		     , TEAM_ID           = @TEAM_ID           
		     , TEAM_SET_ID       = @TEAM_SET_ID       
		 where ID                = @ID                ;
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from FEEDS_CSTM where ID_C = @ID) begin -- then
			insert into FEEDS_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spFEEDS_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		
		-- 12/06/2005 Paul.  Assign the feed to the user that created it. 
		exec dbo.spUSERS_FEEDS_Update @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @ID, 0;
	end -- if;

  end
GO

Grant Execute on dbo.spFEEDS_Update to public;
GO


