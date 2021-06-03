if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCONTACTS_Import' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCONTACTS_Import;
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
-- 04/21/2006 Paul.  TITLE was increased to nvarchar(50) in SugarCRM 4.0.
-- 12/29/2007 Paul.  Add TEAM_ID so that it is not updated separately. 
-- 02/19/2008 Paul.  Move relationship management to spACCOUNTS_CONTACTS_Update. 
-- 02/20/2008 Paul.  If the ACCOUNT_ID is null, then look it up using @ACCOUNT_NAME. 
-- 08/27/2008 Paul.  PostgreSQL does not allow modifying input parameters.  Use a local temp variable. 
-- 08/21/2009 Paul.  Add support for dynamic teams. 
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 03/16/2010 Paul.  The TEAM_SET_LIST must be part of the import to get saved or restored. 
-- 05/10/2010 Paul.  Allow import of date fields to improve ACT! imports. 
-- 06/28/2010 Paul.  We need to use the @TEMP_ACCOUNT_ID. 
-- 07/21/2010 Paul.  Allow multiple street fields and combine them into one.  
-- Lots of systems export multiple street lines and we need a simple way to combine them into one. 
-- 03/16/2011 Paul.  We need to allow a company to be treated as separate if the address is different. 
-- 10/16/2011 Paul.  Increase size of SALUTATION, FIRST_NAME and LAST_NAME to match SugarCRM. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
-- 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
Create Procedure dbo.spCONTACTS_Import
	( @ID                          uniqueidentifier output
	, @MODIFIED_USER_ID            uniqueidentifier
	, @ASSIGNED_USER_ID            uniqueidentifier
	, @SALUTATION                  nvarchar(25)
	, @FIRST_NAME                  nvarchar(100)
	, @LAST_NAME                   nvarchar(100)
	, @ACCOUNT_ID                  uniqueidentifier
	, @LEAD_SOURCE                 nvarchar(100)
	, @TITLE                       nvarchar(50)
	, @DEPARTMENT                  nvarchar(100)
	, @REPORTS_TO_ID               uniqueidentifier
	, @BIRTHDATE                   datetime
	, @DO_NOT_CALL                 bit
	, @PHONE_HOME                  nvarchar(25)
	, @PHONE_MOBILE                nvarchar(25)
	, @PHONE_WORK                  nvarchar(25)
	, @PHONE_OTHER                 nvarchar(25)
	, @PHONE_FAX                   nvarchar(25)
	, @EMAIL1                      nvarchar(100)
	, @EMAIL2                      nvarchar(100)
	, @ASSISTANT                   nvarchar(75)
	, @ASSISTANT_PHONE             nvarchar(25)
	, @EMAIL_OPT_OUT               bit
	, @INVALID_EMAIL               bit
	, @PRIMARY_ADDRESS_STREET      nvarchar(150)
	, @PRIMARY_ADDRESS_CITY        nvarchar(100)
	, @PRIMARY_ADDRESS_STATE       nvarchar(100)
	, @PRIMARY_ADDRESS_POSTALCODE  nvarchar(20)
	, @PRIMARY_ADDRESS_COUNTRY     nvarchar(100)
	, @ALT_ADDRESS_STREET          nvarchar(150)
	, @ALT_ADDRESS_CITY            nvarchar(100)
	, @ALT_ADDRESS_STATE           nvarchar(100)
	, @ALT_ADDRESS_POSTALCODE      nvarchar(20)
	, @ALT_ADDRESS_COUNTRY         nvarchar(100)
	, @DESCRIPTION                 nvarchar(max)
	, @PARENT_TYPE                 nvarchar(25)
	, @PARENT_ID                   uniqueidentifier
	, @SYNC_CONTACT                bit = 0
	, @TEAM_ID                     uniqueidentifier = null
	, @ACCOUNT_NAME                nvarchar(100) = null
	, @TEAM_SET_LIST               varchar(8000) = null
	, @DATE_ENTERED                datetime = null
	, @DATE_MODIFIED               datetime = null
	, @PRIMARY_ADDRESS_STREET1     nvarchar(150) = null
	, @PRIMARY_ADDRESS_STREET2     nvarchar(150) = null
	, @PRIMARY_ADDRESS_STREET3     nvarchar(150) = null
	, @ALT_ADDRESS_STREET1         nvarchar(150) = null
	, @ALT_ADDRESS_STREET2         nvarchar(150) = null
	, @ALT_ADDRESS_STREET3         nvarchar(150) = null
	, @SMS_OPT_IN                  nvarchar(25) = null
	, @TWITTER_SCREEN_NAME         nvarchar(20) = null
	)
as
  begin
	set nocount on
	
	declare @TEAM_SET_ID                 uniqueidentifier;
	declare @TEMP_DATE_ENTERED           datetime;
	declare @TEMP_DATE_MODIFIED          datetime;
	declare @TEMP_ACCOUNT_ID             uniqueidentifier;
	declare @TEMP_PRIMARY_ADDRESS_STREET nvarchar(600);
	declare @TEMP_ALT_ADDRESS_STREET     nvarchar(600);
	declare @IMPORT_CONTACT_ACCOUNT_ADDR bit;
	declare @ENABLE_DYNAMIC_TEAMS        bit;
	declare @ID_LIST                     varchar(8000);
	declare @ACCOUNT_TEAM_SET_LIST       varchar(8000);
	set @TEMP_ACCOUNT_ID = @ACCOUNT_ID;

	-- 07/21/2010 Paul.  Allow multiple street fields and combine them into one.  
	set @TEMP_PRIMARY_ADDRESS_STREET = dbo.fnCombineAddress(@PRIMARY_ADDRESS_STREET, @PRIMARY_ADDRESS_STREET1, @PRIMARY_ADDRESS_STREET2, @PRIMARY_ADDRESS_STREET3);
	set @TEMP_ALT_ADDRESS_STREET     = dbo.fnCombineAddress(@ALT_ADDRESS_STREET    , @ALT_ADDRESS_STREET1    , @ALT_ADDRESS_STREET2    , @ALT_ADDRESS_STREET3    );

	-- 05/10/2010 Paul.  DATE_ENTERED cannot be NULL. 
	set @TEMP_DATE_ENTERED = @DATE_ENTERED;
	if @TEMP_DATE_ENTERED is null begin -- then
		set @TEMP_DATE_ENTERED = getdate();
	end -- if;
	set @TEMP_DATE_MODIFIED = @DATE_MODIFIED;
	if @TEMP_DATE_MODIFIED is null begin -- then
		set @TEMP_DATE_MODIFIED = getdate();
	end -- if;

	-- 02/20/2008 Paul.  If the ACCOUNT_ID is null, then look it up using @ACCOUNT_NAME. 
	-- Converting an account name to an account ID is important to allow importing. 
	if dbo.fnIsEmptyGuid(@TEMP_ACCOUNT_ID) = 1 and @ACCOUNT_NAME is not null begin -- then
		-- 03/16/2011 Paul.  We need to allow a company to be treated as separate if the address is different. 
		set @IMPORT_CONTACT_ACCOUNT_ADDR = dbo.fnCONFIG_Boolean('Import.Contacts.Accounts.Address');
		set @ENABLE_DYNAMIC_TEAMS        = dbo.fnCONFIG_Boolean('enable_dynamic_teams');
		if @IMPORT_CONTACT_ACCOUNT_ADDR = 1 begin -- then
			-- BEGIN Oracle Exception
				select @TEMP_ACCOUNT_ID = ID
				  from ACCOUNTS
				 where NAME        = @ACCOUNT_NAME
				   and isnull(BILLING_ADDRESS_STREET    , '') = isnull(@PRIMARY_ADDRESS_STREET    , '')
				   and isnull(BILLING_ADDRESS_CITY      , '') = isnull(@PRIMARY_ADDRESS_CITY      , '')
				   and isnull(BILLING_ADDRESS_STATE     , '') = isnull(@PRIMARY_ADDRESS_STATE     , '')
				   and isnull(BILLING_ADDRESS_POSTALCODE, '') = isnull(@PRIMARY_ADDRESS_POSTALCODE, '')
				   and DELETED     = 0;
			-- END Oracle Exception
		end else begin
			-- BEGIN Oracle Exception
				select @TEMP_ACCOUNT_ID = ID
				  from ACCOUNTS
				 where NAME        = @ACCOUNT_NAME
				   and DELETED     = 0;
			-- END Oracle Exception
		end -- if;
		-- 02/20/2008 Paul.  If account does not exist, then it will be created. 
		-- This is primarily for importing. 
		if dbo.fnIsEmptyGuid(@TEMP_ACCOUNT_ID) = 1 begin -- then
			-- 08/21/2009 Paul.  The layout of spACCOUNTS_Update changed to put the two team fields together and at the end. 
			-- 04/07/2010 Paul.  Added @EXCHANGE_FOLDER field. 
			exec dbo.spACCOUNTS_Update
				  @TEMP_ACCOUNT_ID out
				, @MODIFIED_USER_ID
				, @ASSIGNED_USER_ID
				, @ACCOUNT_NAME
				, null                          -- @ACCOUNT_TYPE                 
				, null                          -- @PARENT_ID                    
				, null                          -- @INDUSTRY                     
				, null                          -- @ANNUAL_REVENUE               
				, @PHONE_FAX                    
				, @TEMP_PRIMARY_ADDRESS_STREET  -- @BILLING_ADDRESS_STREET       
				, @PRIMARY_ADDRESS_CITY         -- @BILLING_ADDRESS_CITY         
				, @PRIMARY_ADDRESS_STATE        -- @BILLING_ADDRESS_STATE        
				, @PRIMARY_ADDRESS_POSTALCODE   -- @BILLING_ADDRESS_POSTALCODE   
				, @PRIMARY_ADDRESS_COUNTRY      -- @BILLING_ADDRESS_COUNTRY      
				, null                          -- @DESCRIPTION                  
				, null                          -- @RATING                       
				, @PHONE_WORK                   -- @PHONE_OFFICE                 
				, @PHONE_OTHER                  -- @PHONE_ALTERNATE              
				, @EMAIL1                       
				, @EMAIL2                       
				, null                          -- @WEBSITE                      
				, null                          -- @OWNERSHIP                    
				, null                          -- @EMPLOYEES                    
				, null                          -- @SIC_CODE                     
				, null                          -- @TICKER_SYMBOL                
				, @TEMP_ALT_ADDRESS_STREET      -- @SHIPPING_ADDRESS_STREET      
				, @ALT_ADDRESS_CITY             -- @SHIPPING_ADDRESS_CITY        
				, @ALT_ADDRESS_STATE            -- @SHIPPING_ADDRESS_STATE       
				, @ALT_ADDRESS_POSTALCODE       -- @SHIPPING_ADDRESS_POSTALCODE  
				, @ALT_ADDRESS_COUNTRY          -- @SHIPPING_ADDRESS_COUNTRY     
				, null                          -- @ACCOUNT_NUMBER
				, @TEAM_ID
				, @TEAM_SET_LIST
				, null                          -- @EXCHANGE_FOLDER
				;
		end else begin
			-- 03/16/2011 Paul.  If the account already exists and we are using dynamic teams, then add to the team list. 
			if @ENABLE_DYNAMIC_TEAMS = 1 begin -- then
				set @ID_LIST = cast(@TEMP_ACCOUNT_ID as char(36));
				set @ACCOUNT_TEAM_SET_LIST = cast(@TEAM_ID as char(36));
				exec dbo.spACCOUNTS_MassUpdate @ID_LIST, @MODIFIED_USER_ID, null, null, null, null, @ACCOUNT_TEAM_SET_LIST, 1;
			end -- if;
		end -- if;
	end -- if;

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	if not exists(select * from CONTACTS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into CONTACTS
			( ID                         
			, CREATED_BY                 
			, DATE_ENTERED               
			, MODIFIED_USER_ID           
			, DATE_MODIFIED              
			, DATE_MODIFIED_UTC          
			, ASSIGNED_USER_ID           
			, SALUTATION                 
			, FIRST_NAME                 
			, LAST_NAME                  
			, LEAD_SOURCE                
			, TITLE                      
			, DEPARTMENT                 
			, REPORTS_TO_ID              
			, BIRTHDATE                  
			, DO_NOT_CALL                
			, PHONE_HOME                 
			, PHONE_MOBILE               
			, PHONE_WORK                 
			, PHONE_OTHER                
			, PHONE_FAX                  
			, EMAIL1                     
			, EMAIL2                     
			, ASSISTANT                  
			, ASSISTANT_PHONE            
			, EMAIL_OPT_OUT              
			, INVALID_EMAIL              
			, PRIMARY_ADDRESS_STREET     
			, PRIMARY_ADDRESS_CITY       
			, PRIMARY_ADDRESS_STATE      
			, PRIMARY_ADDRESS_POSTALCODE 
			, PRIMARY_ADDRESS_COUNTRY    
			, ALT_ADDRESS_STREET         
			, ALT_ADDRESS_CITY           
			, ALT_ADDRESS_STATE          
			, ALT_ADDRESS_POSTALCODE     
			, ALT_ADDRESS_COUNTRY        
			, DESCRIPTION                
			, TEAM_ID                    
			, TEAM_SET_ID                
			, SMS_OPT_IN                 
			, TWITTER_SCREEN_NAME        
			)
		values
			( @ID                         
			, @MODIFIED_USER_ID           
			, @TEMP_DATE_ENTERED          
			, @MODIFIED_USER_ID           
			, @TEMP_DATE_MODIFIED         
			,  getutcdate()               
			, @ASSIGNED_USER_ID           
			, @SALUTATION                 
			, @FIRST_NAME                 
			, @LAST_NAME                  
			, @LEAD_SOURCE                
			, @TITLE                      
			, @DEPARTMENT                 
			, @REPORTS_TO_ID              
			, @BIRTHDATE                  
			, @DO_NOT_CALL                
			, @PHONE_HOME                 
			, @PHONE_MOBILE               
			, @PHONE_WORK                 
			, @PHONE_OTHER                
			, @PHONE_FAX                  
			, @EMAIL1                     
			, @EMAIL2                     
			, @ASSISTANT                  
			, @ASSISTANT_PHONE            
			, @EMAIL_OPT_OUT              
			, @INVALID_EMAIL              
			, @TEMP_PRIMARY_ADDRESS_STREET
			, @PRIMARY_ADDRESS_CITY       
			, @PRIMARY_ADDRESS_STATE      
			, @PRIMARY_ADDRESS_POSTALCODE 
			, @PRIMARY_ADDRESS_COUNTRY    
			, @ALT_ADDRESS_STREET         
			, @ALT_ADDRESS_CITY           
			, @ALT_ADDRESS_STATE          
			, @ALT_ADDRESS_POSTALCODE     
			, @ALT_ADDRESS_COUNTRY        
			, @DESCRIPTION                
			, @TEAM_ID                    
			, @TEAM_SET_ID                
			, @SMS_OPT_IN                 
			, @TWITTER_SCREEN_NAME        
			);
	end else begin
		update CONTACTS
		   set MODIFIED_USER_ID            = @MODIFIED_USER_ID           
		     , DATE_MODIFIED               =  getdate()                  
		     , DATE_MODIFIED_UTC           =  getutcdate()               
		     , ASSIGNED_USER_ID            = @ASSIGNED_USER_ID           
		     , SALUTATION                  = @SALUTATION                 
		     , FIRST_NAME                  = @FIRST_NAME                 
		     , LAST_NAME                   = @LAST_NAME                  
		     , LEAD_SOURCE                 = @LEAD_SOURCE                
		     , TITLE                       = @TITLE                      
		     , DEPARTMENT                  = @DEPARTMENT                 
		     , REPORTS_TO_ID               = @REPORTS_TO_ID              
		     , BIRTHDATE                   = @BIRTHDATE                  
		     , DO_NOT_CALL                 = @DO_NOT_CALL                
		     , PHONE_HOME                  = @PHONE_HOME                 
		     , PHONE_MOBILE                = @PHONE_MOBILE               
		     , PHONE_WORK                  = @PHONE_WORK                 
		     , PHONE_OTHER                 = @PHONE_OTHER                
		     , PHONE_FAX                   = @PHONE_FAX                  
		     , EMAIL1                      = @EMAIL1                     
		     , EMAIL2                      = @EMAIL2                     
		     , ASSISTANT                   = @ASSISTANT                  
		     , ASSISTANT_PHONE             = @ASSISTANT_PHONE            
		     , EMAIL_OPT_OUT               = @EMAIL_OPT_OUT              
		     , INVALID_EMAIL               = @INVALID_EMAIL              
		     , PRIMARY_ADDRESS_STREET      = @TEMP_PRIMARY_ADDRESS_STREET
		     , PRIMARY_ADDRESS_CITY        = @PRIMARY_ADDRESS_CITY       
		     , PRIMARY_ADDRESS_STATE       = @PRIMARY_ADDRESS_STATE      
		     , PRIMARY_ADDRESS_POSTALCODE  = @PRIMARY_ADDRESS_POSTALCODE 
		     , PRIMARY_ADDRESS_COUNTRY     = @PRIMARY_ADDRESS_COUNTRY    
		     , ALT_ADDRESS_STREET          = @TEMP_ALT_ADDRESS_STREET    
		     , ALT_ADDRESS_CITY            = @ALT_ADDRESS_CITY           
		     , ALT_ADDRESS_STATE           = @ALT_ADDRESS_STATE          
		     , ALT_ADDRESS_POSTALCODE      = @ALT_ADDRESS_POSTALCODE     
		     , ALT_ADDRESS_COUNTRY         = @ALT_ADDRESS_COUNTRY        
		     , DESCRIPTION                 = @DESCRIPTION                
		     , TEAM_ID                     = @TEAM_ID                    
		     , TEAM_SET_ID                 = @TEAM_SET_ID                
		     , SMS_OPT_IN                  = @SMS_OPT_IN                 
		     , TWITTER_SCREEN_NAME         = @TWITTER_SCREEN_NAME        
		 where ID                          = @ID                         ;
		-- Delete any existing account/contact relationships for this contact. 
		-- 02/19/2008 Paul.  Move relationship management to spACCOUNTS_CONTACTS_Update. 
		/*
		update ACCOUNTS_CONTACTS
		   set DELETED                     = 1
		     , MODIFIED_USER_ID            = @MODIFIED_USER_ID 
		     , DATE_MODIFIED               =  getdate()        
		     , DATE_MODIFIED_UTC           =  getutcdate()     
		 where CONTACT_ID                  = @ID;
		*/
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from CONTACTS_CSTM where ID_C = @ID) begin -- then
			insert into CONTACTS_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spCONTACTS_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;
		
		-- Assign any new account. 
		-- 08/26/2008 Paul.  Always call spACCOUNTS_CONTACTS_Update at it will clear the relationship if necessary. 
		--if dbo.fnIsEmptyGuid(@ACCOUNT_ID) = 0 begin -- then
			-- 06/28/2010 Paul.  We need to use the @TEMP_ACCOUNT_ID. 
			exec dbo.spACCOUNTS_CONTACTS_Update @MODIFIED_USER_ID, @TEMP_ACCOUNT_ID, @ID;
		--end -- if;
		
		if dbo.fnIsEmptyGuid(@PARENT_ID) = 0 begin -- then
			if @PARENT_TYPE = N'Bugs' begin -- then
				exec dbo.spCONTACTS_BUGS_Update          @MODIFIED_USER_ID, @ID, @PARENT_ID, null;
			end else if @PARENT_TYPE = N'Cases' begin -- then
				exec dbo.spCONTACTS_CASES_Update         @MODIFIED_USER_ID, @ID, @PARENT_ID, null;
			end else if @PARENT_TYPE = N'Emails' begin -- then
				exec dbo.spEMAILS_CONTACTS_Update        @MODIFIED_USER_ID, @PARENT_ID, @ID;
			end else if @PARENT_TYPE = N'Opportunities' begin -- then
				-- 03/04/2006 Paul.  PARENT_ID and ID had to be swapped. 
				exec dbo.spOPPORTUNITIES_CONTACTS_Update @MODIFIED_USER_ID, @PARENT_ID, @ID, null;
			end -- if;
		end -- if;
		
		-- 02/09/2006 Paul.  SugarCRM uses the CONTACTS_USERS table to allow each user to 
		-- choose the contacts they want sync'd with Outlook. 
		-- 02/18/2006 Paul.  Use the @MODIFIED_USER_ID as the user to sync with. All users can sync with this contact. 
		-- 11/01/2006 Paul.  The sync flag should not be treated as 1 if it is null. 
		-- 06/13/2007 Paul.  If the Sync value is NULL, then don't do anything. This is to prevent the Outlook plug-in from unsyncing after update. 
		if @SYNC_CONTACT = 0 begin -- then
			exec dbo.spCONTACTS_USERS_Delete @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end else if @SYNC_CONTACT = 1 begin -- then
			exec dbo.spCONTACTS_USERS_Update @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end -- if;
	end -- if;
  end
GO

Grant Execute on dbo.spCONTACTS_Import to public;
GO


