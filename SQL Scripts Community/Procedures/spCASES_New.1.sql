if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCASES_New' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCASES_New;
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
-- 06/20/2009 Paul.  We need to get and assign the default team otherwise the new record 
-- will not be displayed if the Team Required flag is set. 
-- 07/25/2009 Paul.  CASE_NUMBER is no longer an identity and must be formatted. 
-- 11/28/2009 Paul.  Add UTC date. 
-- 01/14/2010 Paul.  Add support for Team Sets. 
-- 01/16/2012 Paul.  Assigned User ID and Team ID are now parameters. 
-- 05/01/2013 Paul.  Add Contacts field to support B2C. 
Create Procedure dbo.spCASES_New
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(150)
	, @ACCOUNT_NAME      nvarchar(100)
	, @ACCOUNT_ID        uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier = null
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	, @B2C_CONTACT_ID    uniqueidentifier = null
	)
as
  begin
	set nocount on
	
	declare @TEAM_SET_ID         uniqueidentifier;
	declare @TEMP_CASE_NUMBER nvarchar(30);

	-- 01/16/2012 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 01/16/2012 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	exec dbo.spNUMBER_SEQUENCES_Formatted 'CASES.CASE_NUMBER', 1, @TEMP_CASE_NUMBER out;
	
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
	end -- if;
	insert into CASES
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, DATE_MODIFIED_UTC
		, ASSIGNED_USER_ID 
		, CASE_NUMBER      
		, NAME             
		, ACCOUNT_NAME     
		, ACCOUNT_ID       
		, B2C_CONTACT_ID   
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
		, @TEMP_CASE_NUMBER
		, @NAME            
		, @ACCOUNT_NAME    
		, @ACCOUNT_ID      
		, @B2C_CONTACT_ID  
		, @TEAM_ID         
		, @TEAM_SET_ID     
		);

	-- 03/04/2006 Paul.  Add record to custom table. 
	if not exists(select * from CASES_CSTM where ID_C = @ID) begin -- then
		insert into CASES_CSTM ( ID_C ) values ( @ID );
	end -- if;

  end
GO

Grant Execute on dbo.spCASES_New to public;
GO


