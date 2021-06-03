if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCONTACTS_REPORTS_TO_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCONTACTS_REPORTS_TO_Update;
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
Create Procedure dbo.spCONTACTS_REPORTS_TO_Update
	( @MODIFIED_USER_ID  uniqueidentifier
	, @CONTACT_ID        uniqueidentifier
	, @REPORTS_TO_ID     uniqueidentifier
	)
as
  begin
	set nocount on
	
	update CONTACTS
	   set REPORTS_TO_ID     = @REPORTS_TO_ID    
	     , MODIFIED_USER_ID  = @MODIFIED_USER_ID 
	     , DATE_MODIFIED     =  getdate()        
	     , DATE_MODIFIED_UTC =  getutcdate()     
	 where ID                = @CONTACT_ID       ;
  end
GO

Grant Execute on dbo.spCONTACTS_REPORTS_TO_Update to public;
GO


