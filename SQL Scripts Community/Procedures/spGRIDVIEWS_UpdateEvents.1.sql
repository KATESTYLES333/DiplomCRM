if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spGRIDVIEWS_UpdateEvents' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spGRIDVIEWS_UpdateEvents;
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
-- 11/22/2010 Paul.  Add support for Business Rules Framework. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
-- 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
Create Procedure dbo.spGRIDVIEWS_UpdateEvents
	( @MODIFIED_USER_ID    uniqueidentifier
	, @NAME                nvarchar(50)
	, @PRE_LOAD_EVENT_ID   uniqueidentifier
	, @POST_LOAD_EVENT_ID  uniqueidentifier
	, @SCRIPT              nvarchar(max) = null
	)
as
  begin
	-- BEGIN Oracle Exception
		update GRIDVIEWS
		   set MODIFIED_USER_ID    = @MODIFIED_USER_ID   
		     , DATE_MODIFIED       =  getdate()          
		     , DATE_MODIFIED_UTC   =  getutcdate()       
		     , PRE_LOAD_EVENT_ID   = @PRE_LOAD_EVENT_ID  
		     , POST_LOAD_EVENT_ID  = @POST_LOAD_EVENT_ID 
		     , SCRIPT              = @SCRIPT             
		 where NAME                = @NAME               
		   and DELETED             = 0                   ;
	-- END Oracle Exception
  end
GO
 
Grant Execute on dbo.spGRIDVIEWS_UpdateEvents to public;
GO
 

