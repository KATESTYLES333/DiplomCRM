if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCASES_BUGS_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCASES_BUGS_Delete;
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
Create Procedure dbo.spCASES_BUGS_Delete
	( @MODIFIED_USER_ID uniqueidentifier
	, @CASE_ID          uniqueidentifier
	, @BUG_ID           uniqueidentifier
	)
as
  begin
	set nocount on
	
	update CASES_BUGS
	   set DELETED          = 1
	     , DATE_MODIFIED    = getdate()
	     , DATE_MODIFIED_UTC= getutcdate()
	     , MODIFIED_USER_ID = @MODIFIED_USER_ID
	 where CASE_ID          = @CASE_ID
	   and BUG_ID           = @BUG_ID
	   and DELETED          = 0;
  end
GO

Grant Execute on dbo.spCASES_BUGS_Delete to public;
GO

