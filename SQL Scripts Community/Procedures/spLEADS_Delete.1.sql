if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spLEADS_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spLEADS_Delete;
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
-- 04/01/2012 Paul.  Add Calls/Leads relationship. 
Create Procedure dbo.spLEADS_Delete
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	-- 08/10/2013 Paul.  Delete Documents relationship. 
	-- BEGIN Oracle Exception
		update LEADS_DOCUMENTS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where LEAD_ID          = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
	
	-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
	-- BEGIN Oracle Exception
		update PROSPECT_LISTS_PROSPECTS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where RELATED_ID       = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
	
	-- 04/01/2012 Paul.  Add Calls/Leads relationship. 
	-- BEGIN Oracle Exception
		update CALLS_LEADS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where LEAD_ID          = @ID
		   and DELETED          = 0;
	-- END Oracle Exception

	-- 04/01/2012 Paul.  Add Meetings/Leads relationship. 
	-- BEGIN Oracle Exception
		update MEETINGS_LEADS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where LEAD_ID          = @ID
		   and DELETED          = 0;
	-- END Oracle Exception

	-- 04/01/2012 Paul.  Add Users/Leads relationship. 
	-- BEGIN Oracle Exception
		update LEADS_USERS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where LEAD_ID          = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
	
	-- 04/01/2012 Paul.  Add Emails/Leads relationship. 
	-- BEGIN Oracle Exception
		update EMAILS_LEADS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where LEAD_ID          = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
	
	-- BEGIN Oracle Exception
		delete from TRACKER
		 where ITEM_ID          = @ID
		   and USER_ID          = @MODIFIED_USER_ID;
	-- END Oracle Exception
	
	exec dbo.spPARENT_Delete @ID, @MODIFIED_USER_ID;
	
	-- BEGIN Oracle Exception
		update LEADS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = @MODIFIED_USER_ID
		 where ID               = @ID
		   and DELETED          = 0;
	-- END Oracle Exception
	
  end
GO

Grant Execute on dbo.spLEADS_Delete to public;
GO

