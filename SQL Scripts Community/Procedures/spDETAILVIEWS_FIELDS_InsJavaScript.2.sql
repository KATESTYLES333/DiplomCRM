if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spDETAILVIEWS_FIELDS_InsJavaScript' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spDETAILVIEWS_FIELDS_InsJavaScript;
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
-- 08/02/2010 Paul.  Increase the size of the URL_FIELD and URL_FORMAT so that we can add a javascript info column. 
-- 10/30/2013 Paul.  Increase size of URL_TARGET. 
Create Procedure dbo.spDETAILVIEWS_FIELDS_InsJavaScript
	( @DETAIL_NAME       nvarchar( 50)
	, @FIELD_INDEX       int
	, @DATA_LABEL        nvarchar(150)
	, @DATA_FIELD        nvarchar(100)
	, @URL_FIELD         nvarchar(max)
	, @URL_FORMAT        nvarchar(max)
	, @URL_TARGET        nvarchar( 60)
	, @COLSPAN           int
	)
as
  begin
	declare @ID uniqueidentifier;
	
	-- BEGIN Oracle Exception
		select @ID = ID
		  from DETAILVIEWS_FIELDS
		 where DETAIL_NAME  = @DETAIL_NAME
		   and FIELD_INDEX  = @FIELD_INDEX
		   and DELETED      = 0            
		   and DEFAULT_VIEW = 0            ;
	-- END Oracle Exception
	if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
		set @ID = newid();
		insert into DETAILVIEWS_FIELDS
			( ID               
			, CREATED_BY       
			, DATE_ENTERED     
			, MODIFIED_USER_ID 
			, DATE_MODIFIED    
			, DETAIL_NAME      
			, FIELD_INDEX      
			, FIELD_TYPE       
			, DATA_LABEL       
			, DATA_FIELD       
			, URL_FIELD        
			, URL_FORMAT       
			, URL_TARGET       
			, COLSPAN          
			)
		values 
			( @ID               
			, null              
			,  getdate()        
			, null              
			,  getdate()        
			, @DETAIL_NAME      
			, @FIELD_INDEX      
			, N'JavaScript'     
			, @DATA_LABEL       
			, @DATA_FIELD       
			, @URL_FIELD        
			, @URL_FORMAT       
			, @URL_TARGET       
			, @COLSPAN          
			);
	end -- if;
  end
GO
 
Grant Execute on dbo.spDETAILVIEWS_FIELDS_InsJavaScript to public;
GO
 

