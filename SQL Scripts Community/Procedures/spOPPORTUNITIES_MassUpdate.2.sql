if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOPPORTUNITIES_MassUpdate' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOPPORTUNITIES_MassUpdate;
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
-- 09/11/2007 Paul.  Add TEAM_ID.
Create Procedure dbo.spOPPORTUNITIES_MassUpdate
	( @ID_LIST           varchar(8000)
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @ACCOUNT_ID        uniqueidentifier
	, @OPPORTUNITY_TYPE  nvarchar(25)
	, @LEAD_SOURCE       nvarchar(25)
	, @DATE_CLOSED       datetime
	, @SALES_STAGE       nvarchar(25)
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	, @TEAM_SET_ADD      bit = null
	)
as
  begin
	set nocount on
	
	declare @ID           uniqueidentifier;
	declare @CurrentPosR  int;
	declare @NextPosR     int;
	declare @TEAM_SET_ID  uniqueidentifier;
	declare @OLD_SET_ID   uniqueidentifier;

	-- 08/29/2009 Paul.  Allow sets to be mass assigned. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	set @CurrentPosR = 1;
	while @CurrentPosR <= len(@ID_LIST) begin -- do
		-- 10/04/2006 Paul.  charindex should not use unicode parameters as it will limit all inputs to 4000 characters. 
		set @NextPosR = charindex(',', @ID_LIST,  @CurrentPosR);
		if @NextPosR = 0 or @NextPosR is null begin -- then
			set @NextPosR = len(@ID_LIST) + 1;
		end -- if;
		set @ID = cast(rtrim(ltrim(substring(@ID_LIST, @CurrentPosR, @NextPosR - @CurrentPosR))) as uniqueidentifier);
		set @CurrentPosR = @NextPosR+1;

		-- 08/29/2009 Paul.  When adding a set, we need to start with the existing set of the current record. 
		if @TEAM_SET_ADD = 1 and @TEAM_SET_ID is not null begin -- then
			-- BEGIN Oracle Exception
				-- 08/29/2009 Paul.  If a primary team was not provided, then load the old primary team. 
				select @OLD_SET_ID = TEAM_SET_ID
				     , @TEAM_ID    = isnull(@TEAM_ID, TEAM_ID)
				  from OPPORTUNITIES
				 where ID                = @ID
				   and DELETED           = 0;
			-- END Oracle Exception
			if @OLD_SET_ID is not null begin -- then
				exec dbo.spTEAM_SETS_AddSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @OLD_SET_ID, @TEAM_ID, @TEAM_SET_ID;
			end -- if;
		end -- if;

		-- BEGIN Oracle Exception
			update OPPORTUNITIES
			   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
			     , DATE_MODIFIED     =  getdate()        
			     , DATE_MODIFIED_UTC =  getutcdate()     
			     , OPPORTUNITY_TYPE  = isnull(@OPPORTUNITY_TYPE, OPPORTUNITY_TYPE)
			     , LEAD_SOURCE       = isnull(@LEAD_SOURCE     , LEAD_SOURCE     )
			     , DATE_CLOSED       = isnull(@DATE_CLOSED     , DATE_CLOSED     )
			     , SALES_STAGE       = isnull(@SALES_STAGE     , SALES_STAGE     )
			     , ASSIGNED_USER_ID  = isnull(@ASSIGNED_USER_ID, ASSIGNED_USER_ID)
			     , TEAM_ID           = isnull(@TEAM_ID         , TEAM_ID         )
			     , TEAM_SET_ID       = isnull(@TEAM_SET_ID     , TEAM_SET_ID     )
			 where ID                = @ID
			   and DELETED           = 0;
		-- END Oracle Exception

		-- 08/30/2009 Paul.  Make sure to update the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- if @TEAM_SET_ID is not null begin -- then
		-- 	exec dbo.spOPPORTUNITIES_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		-- end -- if;

		if dbo.fnIsEmptyGuid(@ACCOUNT_ID) = 0 begin -- then
			-- Delete any existing account/contact relationships for this contact. 
			-- BEGIN Oracle Exception
				update ACCOUNTS_OPPORTUNITIES
				   set DELETED           = 1
				     , MODIFIED_USER_ID  = @MODIFIED_USER_ID
				     , DATE_MODIFIED     =  getdate()        
				     , DATE_MODIFIED_UTC =  getutcdate()     
				 where OPPORTUNITY_ID    = @ID
				   and DELETED           = 0;
			-- END Oracle Exception
			
			-- Assign any new account. 
			insert into ACCOUNTS_OPPORTUNITIES
				( ID              
				, CREATED_BY      
				, DATE_ENTERED    
				, MODIFIED_USER_ID
				, DATE_MODIFIED   
				, ACCOUNT_ID      
				, OPPORTUNITY_ID  
				)
			values
				(  newid()         
				, @MODIFIED_USER_ID
				,  getdate()       
				, @MODIFIED_USER_ID
				,  getdate()       
				, @ACCOUNT_ID      
				, @ID              
				);
		end -- if;
	end -- while;
  end
GO
 
Grant Execute on dbo.spOPPORTUNITIES_MassUpdate to public;
GO
 
 
