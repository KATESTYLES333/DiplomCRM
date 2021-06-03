if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spUSERS_PasswordUpdate' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spUSERS_PasswordUpdate;
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
-- 05/12/2010 Paul.  SYSTEM_GENERATED_PASSWORD and PWD_LAST_CHANGED are new to help manage forgotten passwrod. 
-- 02/20/2011 Paul.  Log old passwords in the password history table. 
Create Procedure dbo.spUSERS_PasswordUpdate
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	, @USER_HASH        nvarchar(32)
	)
as
  begin
	set nocount on
	
	update USERS
	   set USER_HASH                 = @USER_HASH
	     , PWD_LAST_CHANGED          = getdate()
	     , SYSTEM_GENERATED_PASSWORD = 0
	     , DATE_MODIFIED             = getdate()
	     , DATE_MODIFIED_UTC         = getutcdate()
	     , MODIFIED_USER_ID          = @MODIFIED_USER_ID
	 where ID                        = @ID
	   and DELETED                   = 0;

	exec dbo.spUSERS_PASSWORD_HISTORY_InsertOnly @MODIFIED_USER_ID, @ID, @USER_HASH;
  end
GO
 
Grant Execute on dbo.spUSERS_PasswordUpdate to public;
GO
 
 

