if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spSCHEDULERS_UpdateLastRun' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spSCHEDULERS_UpdateLastRun;
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
-- 12/31/2007 Paul.  The LAST_RUN will likely be computed and rounded down to the 5 minute interval. 
Create Procedure dbo.spSCHEDULERS_UpdateLastRun
	( @ID                uniqueidentifier
	, @MODIFIED_USER_ID  uniqueidentifier
	, @LAST_RUN          datetime
	)
as
  begin
	set nocount on
	
	update SCHEDULERS
	   set LAST_RUN = @LAST_RUN
	 where ID      = @ID
	   and DELETED = 0;
  end
GO

Grant Execute on dbo.spSCHEDULERS_UpdateLastRun to public;
GO


