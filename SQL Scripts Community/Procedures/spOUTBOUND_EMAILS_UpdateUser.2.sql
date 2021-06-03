if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOUTBOUND_EMAILS_UpdateUser' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOUTBOUND_EMAILS_UpdateUser;
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
-- 07/16/2013 Paul.  spOUTBOUND_EMAILS_Update now returns the ID. 
Create Procedure dbo.spOUTBOUND_EMAILS_UpdateUser
	( @MODIFIED_USER_ID   uniqueidentifier
	, @USER_ID            uniqueidentifier
	, @MAIL_SMTPUSER      nvarchar(100)
	, @MAIL_SMTPPASS      nvarchar(100)
	)
as
  begin
	set nocount on
	
	-- 07/11/2010 Paul.  Make sure to call the base Update procedure. 
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from OUTBOUND_EMAILS
		 where USER_ID = @USER_ID 
		   and TYPE    = N'system-override'
		   and DELETED = 0;
	-- END Oracle Exception
	exec dbo.spOUTBOUND_EMAILS_Update @ID out, @MODIFIED_USER_ID, N'system', N'system-override', @USER_ID, null, null, null, null, @MAIL_SMTPUSER, @MAIL_SMTPPASS, 1, null, null, null;
  end
GO

Grant Execute on dbo.spOUTBOUND_EMAILS_UpdateUser to public;
GO


