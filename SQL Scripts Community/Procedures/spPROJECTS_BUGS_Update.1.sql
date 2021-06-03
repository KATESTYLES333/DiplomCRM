if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spPROJECTS_BUGS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spPROJECTS_BUGS_Update;
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
Create Procedure dbo.spPROJECTS_BUGS_Update
	( @MODIFIED_USER_ID  uniqueidentifier
	, @PROJECT_ID        uniqueidentifier
	, @BUG_ID            uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @ID uniqueidentifier;
	-- BEGIN Oracle Exception
		select @ID = ID
		  from PROJECTS_BUGS
		 where PROJECT_ID        = @PROJECT_ID
		   and BUG_ID            = @BUG_ID
		   and DELETED           = 0;
	-- END Oracle Exception
	
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into PROJECTS_BUGS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, PROJECT_ID       
			, BUG_ID           
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @PROJECT_ID       
			, @BUG_ID           
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spPROJECTS_BUGS_Update to public;
GO
 
