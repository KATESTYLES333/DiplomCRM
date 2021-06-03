if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCONTACTS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCONTACTS_Update;
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
-- 03/13/2008 Paul.  Remove @ACCOUNT_NAME and create spCONTACTS_Import instead. This will become a pattern for import. 
-- 08/26/2008 Paul.  Always call spACCOUNTS_CONTACTS_Update at it will clear the relationship if necessary. 
-- 08/21/2009 Paul.  Add support for dynamic teams. 
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 10/16/2011 Paul.  Increase size of SALUTATION, FIRST_NAME and LAST_NAME to match SugarCRM. 
-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
-- 07/05/2012 Paul.  Create normalized and indexed phone fields for fast call center lookups. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
-- 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
Create Procedure dbo.spCONTACTS_Update
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
	, @TEAM_SET_LIST               varchar(8000) = null
	, @SMS_OPT_IN                  nvarchar(25) = null
	, @TWITTER_SCREEN_NAME         nvarchar(20) = null
	)
as
  begin
	set nocount on
	
	declare @TEAM_SET_ID         uniqueidentifier;
	declare @NAME                nvarchar(255);

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
			,  getdate()                  
			, @MODIFIED_USER_ID           
			,  getdate()                  
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
			, @PRIMARY_ADDRESS_STREET     
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
		     , PRIMARY_ADDRESS_STREET      = @PRIMARY_ADDRESS_STREET     
		     , PRIMARY_ADDRESS_CITY        = @PRIMARY_ADDRESS_CITY       
		     , PRIMARY_ADDRESS_STATE       = @PRIMARY_ADDRESS_STATE      
		     , PRIMARY_ADDRESS_POSTALCODE  = @PRIMARY_ADDRESS_POSTALCODE 
		     , PRIMARY_ADDRESS_COUNTRY     = @PRIMARY_ADDRESS_COUNTRY    
		     , ALT_ADDRESS_STREET          = @ALT_ADDRESS_STREET         
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
		
		-- 04/03/2012 Paul.  When the name changes, update the favorites table. 
		select @NAME = NAME
		  from vwCONTACTS
		 where ID = @ID;
		exec dbo.spSUGARFAVORITES_UpdateName @MODIFIED_USER_ID, @ID, @NAME;
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
			exec dbo.spACCOUNTS_CONTACTS_Update @MODIFIED_USER_ID, @ACCOUNT_ID, @ID;
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

	-- 07/05/2012 Paul.  Create normalized and indexed phone fields for fast call center lookups. 
	if @@ERROR = 0 begin -- then
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Home'     , @PHONE_HOME;
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Mobile'   , @PHONE_MOBILE;
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Work'     , @PHONE_WORK;
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Other'    , @PHONE_OTHER;
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Fax'      , @PHONE_FAX;
		exec dbo.spPHONE_NUMBERS_Update @MODIFIED_USER_ID, @ID, N'Contacts', N'Assistant', @ASSISTANT_PHONE;
	end -- if;
  end
GO

Grant Execute on dbo.spCONTACTS_Update to public;
GO


