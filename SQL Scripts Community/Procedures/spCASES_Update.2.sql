if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCASES_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCASES_Update;
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
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 07/25/2009 Paul.  CASE_NUMBER is no longer an identity and must be formatted. 
-- 08/21/2009 Paul.  Add support for dynamic teams. 
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 04/07/2010 Paul.  Add EXCHANGE_FOLDER.
-- 09/11/2010 Paul.  Fix account lookup.  Where clause was filtering on NAME = ACCOUNT_ID. 
-- 04/02/2012 Paul.  Add TYPE and WORK_LOG. 
-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
-- 05/01/2013 Paul.  Add Contacts field to support B2C. 
Create Procedure dbo.spCASES_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @ASSIGNED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(255)
	, @ACCOUNT_NAME      nvarchar(100)
	, @ACCOUNT_ID        uniqueidentifier
	, @STATUS            nvarchar(25)
	, @PRIORITY          nvarchar(25)
	, @DESCRIPTION       nvarchar(max)
	, @RESOLUTION        nvarchar(max)
	, @PARENT_TYPE       nvarchar(25)
	, @PARENT_ID         uniqueidentifier
	, @CASE_NUMBER       nvarchar(30) = null
	, @TEAM_ID           uniqueidentifier = null
	, @TEAM_SET_LIST     varchar(8000) = null
	, @EXCHANGE_FOLDER   bit = null
	, @TYPE              nvarchar(25) = null
	, @WORK_LOG          nvarchar(max) = null
	, @B2C_CONTACT_ID    uniqueidentifier = null
	)
as
  begin
	set nocount on
	
	declare @TEMP_ACCOUNT_ID     uniqueidentifier;
	declare @TEMP_ACCOUNT_NAME   nvarchar(100);
	declare @TEMP_CASE_NUMBER    nvarchar(30);
	declare @TEAM_SET_ID         uniqueidentifier;
	set @TEMP_ACCOUNT_ID   = @ACCOUNT_ID;
	set @TEMP_ACCOUNT_NAME = @ACCOUNT_NAME;
	set @TEMP_CASE_NUMBER  = @CASE_NUMBER;

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	-- 04/20/2010 Paul.  A case can now be created during Inline Edit of an Account. 
	-- In this case, we will need to lookup the Account Name. 
	if dbo.fnIsEmptyGuid(@TEMP_ACCOUNT_ID) = 0 and @TEMP_ACCOUNT_NAME is null begin -- then
		-- 09/11/2010 Paul.  Fix account lookup.  Where clause was filtering on NAME = ACCOUNT_ID. 
		-- BEGIN Oracle Exception
			select @TEMP_ACCOUNT_NAME = NAME
			  from ACCOUNTS
			 where ID          = @TEMP_ACCOUNT_ID
			   and DELETED     = 0;
		-- END Oracle Exception
	end -- if;

	-- 11/02/2006 Paul.  If the ACCOUNT_ID is null, then look it up using @TEMP_ACCOUNT_NAME. 
	-- Converting an account name to an account ID is important to allow importing. 
	-- 02/20/2008 Paul.  Only lookup if @TEMP_ACCOUNT_NAME is provided. 
	if dbo.fnIsEmptyGuid(@TEMP_ACCOUNT_ID) = 1 and @TEMP_ACCOUNT_NAME is not null begin -- then
		-- BEGIN Oracle Exception
			select @TEMP_ACCOUNT_ID = ID
			  from ACCOUNTS
			 where NAME        = @TEMP_ACCOUNT_NAME
			   and DELETED     = 0;
		-- END Oracle Exception
		-- 02/20/2008 Paul.  If account does not exist, then it will be created. 
		-- This is primarily for importing. 
		if dbo.fnIsEmptyGuid(@TEMP_ACCOUNT_ID) = 1 begin -- then
			-- 08/21/2009 Paul.  The layout of spACCOUNTS_Update changed to put the two team fields together and at the end. 
			exec dbo.spACCOUNTS_Update
				  @TEMP_ACCOUNT_ID out
				, @MODIFIED_USER_ID
				, @ASSIGNED_USER_ID
				, @TEMP_ACCOUNT_NAME
				, null                          -- @ACCOUNT_TYPE                 
				, null                          -- @PARENT_ID                    
				, null                          -- @INDUSTRY                     
				, null                          -- @ANNUAL_REVENUE               
				, null                          -- @PHONE_FAX                    
				, null                          -- @BILLING_ADDRESS_STREET       
				, null                          -- @BILLING_ADDRESS_CITY         
				, null                          -- @BILLING_ADDRESS_STATE        
				, null                          -- @BILLING_ADDRESS_POSTALCODE   
				, null                          -- @BILLING_ADDRESS_COUNTRY      
				, null                          -- @DESCRIPTION                  
				, null                          -- @RATING                       
				, null                          -- @PHONE_OFFICE                 
				, null                          -- @PHONE_ALTERNATE              
				, null                          -- @EMAIL1                       
				, null                          -- @EMAIL2                       
				, null                          -- @WEBSITE                      
				, null                          -- @OWNERSHIP                    
				, null                          -- @EMPLOYEES                    
				, null                          -- @SIC_CODE                     
				, null                          -- @TICKER_SYMBOL                
				, null                          -- @SHIPPING_ADDRESS_STREET      
				, null                          -- @SHIPPING_ADDRESS_CITY        
				, null                          -- @SHIPPING_ADDRESS_STATE       
				, null                          -- @SHIPPING_ADDRESS_POSTALCODE  
				, null                          -- @SHIPPING_ADDRESS_COUNTRY     
				, null                          -- @ACCOUNT_NUMBER
				, @TEAM_ID
				, @TEAM_SET_LIST
				, null                          -- @EXCHANGE_FOLDER
				;
		end -- if;
	end -- if;

	if not exists(select * from CASES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		-- 07/25/2009 Paul.  Allow the CASE_NUMBER to be imported. 
		if @TEMP_CASE_NUMBER is null begin -- then
			exec dbo.spNUMBER_SEQUENCES_Formatted 'CASES.CASE_NUMBER', 1, @TEMP_CASE_NUMBER out;
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
			, STATUS           
			, PRIORITY         
			, TYPE             
			, DESCRIPTION      
			, RESOLUTION       
			, WORK_LOG         
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
			, @TEMP_ACCOUNT_NAME 
			, @TEMP_ACCOUNT_ID   
			, @B2C_CONTACT_ID    
			, @STATUS            
			, @PRIORITY          
			, @TYPE              
			, @DESCRIPTION       
			, @RESOLUTION        
			, @WORK_LOG          
			, @TEAM_ID           
			, @TEAM_SET_ID       
			);
	end else begin
		update CASES
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID  
		     , DATE_MODIFIED     =  getdate()         
		     , DATE_MODIFIED_UTC =  getutcdate()      
		     , ASSIGNED_USER_ID  = @ASSIGNED_USER_ID  
		     , CASE_NUMBER       = isnull(@TEMP_CASE_NUMBER, CASE_NUMBER)
		     , NAME              = @NAME              
		     , ACCOUNT_NAME      = @TEMP_ACCOUNT_NAME 
		     , ACCOUNT_ID        = @TEMP_ACCOUNT_ID   
		     , B2C_CONTACT_ID    = @B2C_CONTACT_ID    
		     , STATUS            = @STATUS            
		     , PRIORITY          = @PRIORITY          
		     , TYPE              = @TYPE              
		     , DESCRIPTION       = @DESCRIPTION       
		     , RESOLUTION        = @RESOLUTION        
		     , WORK_LOG          = @WORK_LOG          
		     , TEAM_ID           = @TEAM_ID           
		     , TEAM_SET_ID       = @TEAM_SET_ID       
		 where ID                = @ID                ;
		
		-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
		exec dbo.spSUGARFAVORITES_UpdateName @MODIFIED_USER_ID, @ID, @NAME;
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from CASES_CSTM where ID_C = @ID) begin -- then
			insert into CASES_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spCASES_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		
		if dbo.fnIsEmptyGuid(@PARENT_ID) = 0 begin -- then
			if @PARENT_TYPE = N'Accounts' begin -- then
				exec dbo.spACCOUNTS_CASES_Update @MODIFIED_USER_ID, @PARENT_ID, @ID;
			end else if @PARENT_TYPE = N'Bugs' begin -- then
				exec dbo.spCASES_BUGS_Update     @MODIFIED_USER_ID, @ID, @PARENT_ID;
			end else if @PARENT_TYPE = N'Contacts' begin -- then
				exec dbo.spCONTACTS_CASES_Update @MODIFIED_USER_ID, @PARENT_ID, @ID, null;
			end else if @PARENT_TYPE = N'Emails' begin -- then
				exec dbo.spEMAILS_CASES_Update   @MODIFIED_USER_ID, @PARENT_ID, @ID;
			end -- if;
		end -- if;

		-- 04/07/2010 Paul.  If the Exchange Folder value is NULL, then don't do anything. This is to prevent the Exchange from unsyncing after update. 
		if @EXCHANGE_FOLDER = 0 begin -- then
			exec dbo.spCASES_USERS_Delete @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end else if @EXCHANGE_FOLDER = 1 begin -- then
			exec dbo.spCASES_USERS_Update @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end -- if;
	end -- if;

  end
GO

Grant Execute on dbo.spCASES_Update to public;
GO


