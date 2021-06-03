if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spLANGUAGES_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spLANGUAGES_Update;
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
Create Procedure dbo.spLANGUAGES_Update
	( @ID                uniqueidentifier output
	, @MODIFIED_USER_ID  uniqueidentifier
	, @NAME              nvarchar(10)
	, @LCID              int
	, @ACTIVE            bit
	, @NATIVE_NAME       nvarchar(80)
	, @DISPLAY_NAME      nvarchar(80)
	)
as
  begin
	set nocount on
	
	if not exists(select * from LANGUAGES where ID = @ID) begin -- then
		if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
			set @ID = newid();
		end -- if;
		insert into LANGUAGES
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, NAME             
			, LCID             
			, ACTIVE           
			, NATIVE_NAME      
			, DISPLAY_NAME     
			)
		values
			( @ID               
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @MODIFIED_USER_ID 
			,  getdate()        
			, @NAME             
			, @LCID             
			, @ACTIVE           
			, @NATIVE_NAME      
			, @DISPLAY_NAME     
			);
	end else begin
		update LANGUAGES
		   set MODIFIED_USER_ID  = @MODIFIED_USER_ID 
		     , DATE_MODIFIED     =  getdate()        
		     , DATE_MODIFIED_UTC =  getutcdate()     
		     , NAME              = @NAME             
		     , LCID              = @LCID             
		     , ACTIVE            = @ACTIVE           
		     , NATIVE_NAME       = @NATIVE_NAME      
		     , DISPLAY_NAME      = @DISPLAY_NAME     
		 where ID                = @ID               ;
	end -- if;
  end
GO
 
Grant Execute on dbo.spLANGUAGES_Update to public;
GO
 

