if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spEMAIL_IMAGES_Copy' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spEMAIL_IMAGES_Copy;
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
Create Procedure dbo.spEMAIL_IMAGES_Copy
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @COPY_ID           uniqueidentifier
	, @PARENT_ID         uniqueidentifier
	)
as
  begin
	set nocount on
	
	set @ID = newid();
	insert into EMAIL_IMAGES
		( ID               
		, CREATED_BY       
		, DATE_ENTERED     
		, MODIFIED_USER_ID 
		, DATE_MODIFIED    
		, PARENT_ID        
		, FILENAME         
		, FILE_EXT         
		, FILE_MIME_TYPE   
		, CONTENT          
		)
	select	  @ID               
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @MODIFIED_USER_ID 
		,  getdate()        
		, @PARENT_ID        
		,  FILENAME         
		,  FILE_EXT         
		,  FILE_MIME_TYPE   
		,  CONTENT          
	  from EMAIL_IMAGES
	 where ID = @COPY_ID;
  end
GO

Grant Execute on dbo.spEMAIL_IMAGES_Copy to public;
GO


