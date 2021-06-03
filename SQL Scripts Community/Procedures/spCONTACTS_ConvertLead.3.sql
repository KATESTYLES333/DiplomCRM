if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCONTACTS_ConvertLead' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCONTACTS_ConvertLead;
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
-- 08/23/2009 Paul.  Add support for dynamic teams. 
-- 09/15/2009 Paul.  Convert data type to nvarchar(max) to support Azure. 
-- 10/16/2011 Paul.  Increase size of SALUTATION, FIRST_NAME and LAST_NAME to match SugarCRM. 
Create Procedure dbo.spCONTACTS_ConvertLead
	( @ID                          uniqueidentifier output
	, @MODIFIED_USER_ID            uniqueidentifier
	, @LEAD_ID                     uniqueidentifier
	, @ASSIGNED_USER_ID            uniqueidentifier
	, @SALUTATION                  nvarchar(25)
	, @FIRST_NAME                  nvarchar(100)
	, @LAST_NAME                   nvarchar(100)
	, @ACCOUNT_ID                  uniqueidentifier
	, @LEAD_SOURCE                 nvarchar(100)
	, @TITLE                       nvarchar(25)
	, @DEPARTMENT                  nvarchar(100)
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
	, @OPPORTUNITY_ID              uniqueidentifier
	, @OPPORTUNITY_NAME            nvarchar(255)
	, @OPPORTUNITY_AMOUNT          nvarchar(50)
	, @CAMPAIGN_ID                 uniqueidentifier
	, @TEAM_ID                     uniqueidentifier = null
	, @TEAM_SET_LIST               varchar(8000)
	)
as
  begin
	set nocount on

	declare @TARGET_TRACKER_KEY uniqueidentifier;
	exec dbo.spCONTACTS_Update @ID out, @MODIFIED_USER_ID, @ASSIGNED_USER_ID, @SALUTATION, @FIRST_NAME, @LAST_NAME, @ACCOUNT_ID, @LEAD_SOURCE, @TITLE, @DEPARTMENT, null, null, @DO_NOT_CALL, @PHONE_HOME, @PHONE_MOBILE, @PHONE_WORK, @PHONE_OTHER, @PHONE_FAX, @EMAIL1, @EMAIL2, null, null, @EMAIL_OPT_OUT, @INVALID_EMAIL, @PRIMARY_ADDRESS_STREET, @PRIMARY_ADDRESS_CITY, @PRIMARY_ADDRESS_STATE, @PRIMARY_ADDRESS_POSTALCODE, @PRIMARY_ADDRESS_COUNTRY, @ALT_ADDRESS_STREET, @ALT_ADDRESS_CITY, @ALT_ADDRESS_STATE, @ALT_ADDRESS_POSTALCODE, @ALT_ADDRESS_COUNTRY, @DESCRIPTION, null, null, null, @TEAM_ID, @TEAM_SET_LIST;

	-- 09/10/2007 Paul.  If the lead came from a campaign, then log the conversion. 
	if @CAMPAIGN_ID is not null begin -- then
		set @TARGET_TRACKER_KEY = newid();
		-- 12/28/2007 Paul.  Correct the parameters. 
		exec dbo.spCAMPAIGN_LOG_InsertOnly @MODIFIED_USER_ID, @CAMPAIGN_ID, @TARGET_TRACKER_KEY, @LEAD_ID, N'Leads', N'contact', @ID, N'Contacts', null, null, null;
	end -- if;

	-- 04/24/2006 Paul.  Update the CONTACT_ID with this new Contact. 	
	-- There will always be a lead to update. 
	-- 07/27/2006 Paul.  The status of the lead needs to change to Converted. 
	update LEADS
	   set MODIFIED_USER_ID   = @MODIFIED_USER_ID 
	     , DATE_MODIFIED      =  getdate()        
	     , DATE_MODIFIED_UTC  =  getutcdate()     
	     , CONTACT_ID         = @ID
	     , ACCOUNT_ID         = @ACCOUNT_ID
	     , OPPORTUNITY_ID     = @OPPORTUNITY_ID
	     , OPPORTUNITY_NAME   = @OPPORTUNITY_NAME
	     , OPPORTUNITY_AMOUNT = @OPPORTUNITY_AMOUNT
	     , STATUS             = N'Converted'
	     , CONVERTED          = 1
	 where ID                 = @LEAD_ID;

  end
GO

Grant Execute on dbo.spCONTACTS_ConvertLead to public;
GO


