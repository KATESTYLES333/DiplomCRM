if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spNOTES_Import' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spNOTES_Import;
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
-- 02/04/2010 Paul.  Special import procedure to allow date from ACT! import. 
-- 09/01/2012 Paul.  Add LAST_ACTIVITY_DATE. 
-- 01/21/2013 Paul.  ASSIGNED_USER_ID should have been added on 04/02/2012. 
Create Procedure dbo.spNOTES_Import
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @DATE_MODIFIED     datetime
	, @NAME              nvarchar(255)
	, @PARENT_TYPE       nvarchar(25)
	, @PARENT_ID         uniqueidentifier
	, @CONTACT_ID        uniqueidentifier
	, @DESCRIPTION       nvarchar(max)
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	, @ASSIGNED_USER_ID  uniqueidentifier = null
	)
as
  begin
	set nocount on
	
	declare @TEMP_DATE_MODIFIED  datetime;
	declare @TEAM_SET_ID         uniqueidentifier;

	-- 02/04/2010 Paul.  DATE_ENTERED cannot be NULL. 
	set @TEMP_DATE_MODIFIED = @DATE_MODIFIED;
	if @TEMP_DATE_MODIFIED is null begin -- then
		set @TEMP_DATE_MODIFIED = getdate();
	end -- if;

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	if not exists(select * from NOTES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into NOTES
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DATE_MODIFIED_UTC
			, NAME             
			, PARENT_TYPE      
			, PARENT_ID        
			, CONTACT_ID       
			, DESCRIPTION      
			, TEAM_ID          
			, TEAM_SET_ID      
			, ASSIGNED_USER_ID 
			)
		values
			( @ID                
			, @MODIFIED_USER_ID  
			, @TEMP_DATE_MODIFIED
			, @MODIFIED_USER_ID  
			, @TEMP_DATE_MODIFIED
			,  getutcdate()      
			, @NAME              
			, @PARENT_TYPE       
			, @PARENT_ID         
			, @CONTACT_ID        
			, @DESCRIPTION       
			, @TEAM_ID           
			, @TEAM_SET_ID       
			, @ASSIGNED_USER_ID 
			);
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from NOTES_CSTM where ID_C = @ID) begin -- then
			insert into NOTES_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spNOTES_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		
		-- 09/01/2012 Paul.  Add LAST_ACTIVITY_DATE. 
		if dbo.fnIsEmptyGuid(@PARENT_ID) = 0 begin -- then
			exec dbo.spPARENT_UpdateLastActivity @MODIFIED_USER_ID, @PARENT_ID, @PARENT_TYPE;
		end -- if;
	end -- if;

  end
GO

Grant Execute on dbo.spNOTES_Import to public;
GO


