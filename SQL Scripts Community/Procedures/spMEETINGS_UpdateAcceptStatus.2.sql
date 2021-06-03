if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spMEETINGS_UpdateAcceptStatus' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spMEETINGS_UpdateAcceptStatus;
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
Create Procedure dbo.spMEETINGS_UpdateAcceptStatus
	( @ID                 uniqueidentifier
	, @MODIFIED_USER_ID   uniqueidentifier
	, @INVITEE_TYPE       nvarchar(25)
	, @INVITEE_ID         uniqueidentifier
	, @ACCEPT_STATUS      nvarchar(25)
	)
as
  begin
	set nocount on
	
	if @INVITEE_TYPE = N'Users' begin -- then
		update MEETINGS_USERS
		   set ACCEPT_STATUS       = @ACCEPT_STATUS
		     , MODIFIED_USER_ID    = @MODIFIED_USER_ID
		     , DATE_MODIFIED       = getdate()
		     , DATE_MODIFIED_UTC   = getutcdate()
		 where MEETING_ID          = @ID
		   and USER_ID             = @INVITEE_ID
		   and DELETED             = 0;
	end else if @INVITEE_TYPE = N'Contacts' begin -- then
		update MEETINGS_CONTACTS
		   set ACCEPT_STATUS       = @ACCEPT_STATUS
		     , MODIFIED_USER_ID    = @MODIFIED_USER_ID
		     , DATE_MODIFIED       = getdate()
		     , DATE_MODIFIED_UTC   = getutcdate()
		 where MEETING_ID          = @ID
		   and CONTACT_ID          = @INVITEE_ID
		   and DELETED             = 0;
	end else if @INVITEE_TYPE = N'Leads' begin -- then
		update MEETINGS_LEADS
		   set ACCEPT_STATUS       = @ACCEPT_STATUS
		     , MODIFIED_USER_ID    = @MODIFIED_USER_ID
		     , DATE_MODIFIED       = getdate()
		     , DATE_MODIFIED_UTC   = getutcdate()
		 where MEETING_ID          = @ID
		   and LEAD_ID             = @INVITEE_ID
		   and DELETED             = 0;
	end -- if;
  end
GO

Grant Execute on dbo.spMEETINGS_UpdateAcceptStatus to public;
GO


