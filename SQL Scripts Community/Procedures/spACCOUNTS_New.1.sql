if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spACCOUNTS_New' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spACCOUNTS_New;
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
-- 07/26/2009 Paul.  Enough customers requested ACCOUNT_NUMBER that it makes sense to add it now. 
-- 11/28/2009 Paul.  Add UTC date. 
-- 01/14/2010 Paul.  Add support for Team Sets. 
-- 01/16/2012 Paul.  Assigned User ID and Team ID are now parameters. 
-- 07/05/2012 Paul.  Create normalized and indexed phone fields for fast call center lookups. 
Create Procedure dbo.spACCOUNTS_New
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(150)
	, @PHONE_OFFICE      nvarchar(25)
	, @WEBSITE           nvarchar(255)
	, @ASSIGNED_USER_ID  uniqueidentifier = null
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	)
as
  begin
	set nocount on

	declare @TEAM_SET_ID         uniqueidentifier;
	declare @TEMP_ACCOUNT_NUMBER nvarchar(30);
	
	-- 01/16/2012 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 01/16/2012 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	exec dbo.spNUMBER_SEQUENCES_Formatted 'ACCOUNTS.ACCOUNT_NUMBER', 1, @TEMP_ACCOUNT_NUMBER out;

	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
	end -- if;
	insert into ACCOUNTS
		( ID                          
		, CREATED_BY                  
		, DATE_ENTERED                
		, MODIFIED_USER_ID            
		, DATE_MODIFIED               
		, DATE_MODIFIED_UTC           
		, ASSIGNED_USER_ID            
		, ACCOUNT_NUMBER              
		, NAME                        
		, PHONE_OFFICE                
		, WEBSITE                     
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
		, @TEMP_ACCOUNT_NUMBER         
		, @NAME                        
		, @PHONE_OFFICE                
		, @WEBSITE                     
		, @TEAM_ID                     
		, @TEAM_SET_ID                 
		);

	-- 03/04/2006 Paul.  Add record to custom table. 
	if not exists(select * from ACCOUNTS_CSTM where ID_C = @ID) begin -- then
		insert into ACCOUNTS_CSTM ( ID_C ) values ( @ID );
	end -- if;

	-- 07/05/2012 Paul.  Create normalized and indexed phone fields for fast call center lookups. 
	if @@ERROR = 0 begin -- then
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Accounts', N'Office'   , @PHONE_OFFICE;
	end -- if;
  end
GO

Grant Execute on dbo.spACCOUNTS_New to public;
GO


