if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spLEADS_Import' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spLEADS_Import;
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
-- 04/21/2006 Paul.  CAMPAIGN_ID was added in SugarCRM 4.0.
-- 12/29/2007 Paul.  Add TEAM_ID so that it is not updated separately. 
-- 08/21/2009 Paul.  Add support for dynamic teams. 
-- 08/23/2009 Paul.  Decrease set list so that index plus ID will be less than 900 bytes. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 02/14/2010 Paul.  Allow links to Account and Contact. 
-- 04/07/2010 Paul.  Add EXCHANGE_FOLDER.
-- 07/21/2010 Paul.  Allow multiple street fields and combine them into one.  
-- Lots of systems export multiple street lines and we need a simple way to combine them into one. 
-- 10/16/2011 Paul.  Increase size of SALUTATION, FIRST_NAME and LAST_NAME to match SugarCRM. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
-- 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
Create Procedure dbo.spLEADS_Import
	( @ID                          uniqueidentifier output
	, @MODIFIED_USER_ID            uniqueidentifier
	, @ASSIGNED_USER_ID            uniqueidentifier
	, @SALUTATION                  nvarchar(25)
	, @FIRST_NAME                  nvarchar(100)
	, @LAST_NAME                   nvarchar(100)
	, @TITLE                       nvarchar(100)
	, @REFERED_BY                  nvarchar(100)
	, @LEAD_SOURCE                 nvarchar(100)
	, @LEAD_SOURCE_DESCRIPTION     nvarchar(max)
	, @STATUS                      nvarchar(100)
	, @STATUS_DESCRIPTION          nvarchar(max)
	, @DEPARTMENT                  nvarchar(100)
	, @REPORTS_TO_ID               uniqueidentifier
	, @DO_NOT_CALL                 bit
	, @PHONE_HOME                  nvarchar(25)
	, @PHONE_MOBILE                nvarchar(25)
	, @PHONE_WORK                  nvarchar(25)
	, @PHONE_OTHER                 nvarchar(25)
	, @PHONE_FAX                   nvarchar(25)
	, @EMAIL1                      nvarchar(100)
	, @EMAIL2                      nvarchar(100)
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
	, @ACCOUNT_NAME                nvarchar(150)
	, @CAMPAIGN_ID                 uniqueidentifier
	, @TEAM_ID                     uniqueidentifier = null
	, @TEAM_SET_LIST               varchar(8000) = null
	, @CONTACT_ID                  uniqueidentifier = null
	, @ACCOUNT_ID                  uniqueidentifier = null
	, @EXCHANGE_FOLDER             bit = null
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
	declare @TEMP_PRIMARY_ADDRESS_STREET nvarchar(600);
	declare @TEMP_ALT_ADDRESS_STREET     nvarchar(600);

	-- 07/21/2010 Paul.  Allow multiple street fields and combine them into one.  
	set @TEMP_PRIMARY_ADDRESS_STREET = dbo.fnCombineAddress(@PRIMARY_ADDRESS_STREET, @PRIMARY_ADDRESS_STREET1, @PRIMARY_ADDRESS_STREET2, @PRIMARY_ADDRESS_STREET3);
	set @TEMP_ALT_ADDRESS_STREET     = dbo.fnCombineAddress(@ALT_ADDRESS_STREET    , @ALT_ADDRESS_STREET1    , @ALT_ADDRESS_STREET2    , @ALT_ADDRESS_STREET3    );

	-- 07/21/2010 Paul.  DATE_ENTERED cannot be NULL. 
	set @TEMP_DATE_ENTERED = @DATE_ENTERED;
	if @TEMP_DATE_ENTERED is null begin -- then
		set @TEMP_DATE_ENTERED = getdate();
	end -- if;
	set @TEMP_DATE_MODIFIED = @DATE_MODIFIED;
	if @TEMP_DATE_MODIFIED is null begin -- then
		set @TEMP_DATE_MODIFIED = getdate();
	end -- if;

	-- 08/22/2009 Paul.  Normalize the team set by placing the primary ID first, then order list by ID and the name by team names. 
	-- 08/23/2009 Paul.  Use a team set so that team name changes can propagate. 
	exec dbo.spTEAM_SETS_NormalizeSet @TEAM_SET_ID out, @MODIFIED_USER_ID, @TEAM_ID, @TEAM_SET_LIST;

	if not exists(select * from LEADS where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into LEADS
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
			, TITLE                      
			, REFERED_BY                 
			, LEAD_SOURCE                
			, LEAD_SOURCE_DESCRIPTION    
			, STATUS                     
			, STATUS_DESCRIPTION         
			, DEPARTMENT                 
			, REPORTS_TO_ID              
			, DO_NOT_CALL                
			, PHONE_HOME                 
			, PHONE_MOBILE               
			, PHONE_WORK                 
			, PHONE_OTHER                
			, PHONE_FAX                  
			, EMAIL1                     
			, EMAIL2                     
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
			, ACCOUNT_NAME               
			, CAMPAIGN_ID                
			, TEAM_ID                    
			, TEAM_SET_ID                
			, CONTACT_ID                 
			, ACCOUNT_ID                 
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
			, @TITLE                      
			, @REFERED_BY                 
			, @LEAD_SOURCE                
			, @LEAD_SOURCE_DESCRIPTION    
			, @STATUS                     
			, @STATUS_DESCRIPTION         
			, @DEPARTMENT                 
			, @REPORTS_TO_ID              
			, @DO_NOT_CALL                
			, @PHONE_HOME                 
			, @PHONE_MOBILE               
			, @PHONE_WORK                 
			, @PHONE_OTHER                
			, @PHONE_FAX                  
			, @EMAIL1                     
			, @EMAIL2                     
			, @EMAIL_OPT_OUT              
			, @INVALID_EMAIL              
			, @TEMP_PRIMARY_ADDRESS_STREET
			, @PRIMARY_ADDRESS_CITY       
			, @PRIMARY_ADDRESS_STATE      
			, @PRIMARY_ADDRESS_POSTALCODE 
			, @PRIMARY_ADDRESS_COUNTRY    
			, @TEMP_ALT_ADDRESS_STREET    
			, @ALT_ADDRESS_CITY           
			, @ALT_ADDRESS_STATE          
			, @ALT_ADDRESS_POSTALCODE     
			, @ALT_ADDRESS_COUNTRY        
			, @DESCRIPTION                
			, @ACCOUNT_NAME               
			, @CAMPAIGN_ID                
			, @TEAM_ID                    
			, @TEAM_SET_ID                
			, @CONTACT_ID                 
			, @ACCOUNT_ID                 
			, @SMS_OPT_IN                 
			, @TWITTER_SCREEN_NAME        
			);
	end else begin
		update LEADS
		   set MODIFIED_USER_ID            = @MODIFIED_USER_ID           
		     , DATE_MODIFIED               =  getdate()                  
		     , DATE_MODIFIED_UTC           =  getutcdate()               
		     , ASSIGNED_USER_ID            = @ASSIGNED_USER_ID           
		     , SALUTATION                  = @SALUTATION                 
		     , FIRST_NAME                  = @FIRST_NAME                 
		     , LAST_NAME                   = @LAST_NAME                  
		     , TITLE                       = @TITLE                      
		     , REFERED_BY                  = @REFERED_BY                 
		     , LEAD_SOURCE                 = @LEAD_SOURCE                
		     , LEAD_SOURCE_DESCRIPTION     = @LEAD_SOURCE_DESCRIPTION    
		     , STATUS                      = @STATUS                     
		     , STATUS_DESCRIPTION          = @STATUS_DESCRIPTION         
		     , DEPARTMENT                  = @DEPARTMENT                 
		     , REPORTS_TO_ID               = @REPORTS_TO_ID              
		     , DO_NOT_CALL                 = @DO_NOT_CALL                
		     , PHONE_HOME                  = @PHONE_HOME                 
		     , PHONE_MOBILE                = @PHONE_MOBILE               
		     , PHONE_WORK                  = @PHONE_WORK                 
		     , PHONE_OTHER                 = @PHONE_OTHER                
		     , PHONE_FAX                   = @PHONE_FAX                  
		     , EMAIL1                      = @EMAIL1                     
		     , EMAIL2                      = @EMAIL2                     
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
		     , ACCOUNT_NAME                = @ACCOUNT_NAME               
		     , CAMPAIGN_ID                 = @CAMPAIGN_ID                
		     , TEAM_ID                     = @TEAM_ID                    
		     , TEAM_SET_ID                 = @TEAM_SET_ID                
		     , CONTACT_ID                  = @CONTACT_ID                 
		     , ACCOUNT_ID                  = @ACCOUNT_ID                 
		     , SMS_OPT_IN                  = @SMS_OPT_IN                 
		     , TWITTER_SCREEN_NAME         = @TWITTER_SCREEN_NAME        
		 where ID                          = @ID                         ;
	end -- if;

	-- 08/22/2009 Paul.  If insert fails, then the rest will as well. Just display the one error. 
	if @@ERROR = 0 begin -- then
		if not exists(select * from LEADS_CSTM where ID_C = @ID) begin -- then
			insert into LEADS_CSTM ( ID_C ) values ( @ID );
		end -- if;
		
		-- 08/21/2009 Paul.  Add or remove the team relationship records. 
		-- 08/30/2009 Paul.  Instead of using @TEAM_SET_LIST, use the @TEAM_SET_ID to build the module-specific team relationships. 
		-- 08/31/2009 Paul.  Instead of managing a separate teams relationship, we will leverage TEAM_SETS_TEAMS. 
		-- exec dbo.spLEADS_TEAMS_Update @ID, @MODIFIED_USER_ID, @TEAM_SET_ID;

		-- 04/07/2010 Paul.  If the Exchange Folder value is NULL, then don't do anything. This is to prevent the Exchange from unsyncing after update. 
		if @EXCHANGE_FOLDER = 0 begin -- then
			exec dbo.spLEADS_USERS_Delete @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end else if @EXCHANGE_FOLDER = 1 begin -- then
			exec dbo.spLEADS_USERS_Update @MODIFIED_USER_ID, @ID, @MODIFIED_USER_ID;
		end -- if;
	end -- if;

  end
GO

Grant Execute on dbo.spLEADS_Import to public;
GO


