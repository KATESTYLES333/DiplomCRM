if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spOUTBOUND_EMAILS_Update' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spOUTBOUND_EMAILS_Update;
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
Create Procedure dbo.spOUTBOUND_EMAILS_Update
	( @ID                 uniqueidentifier output
	, @MODIFIED_USER_ID   uniqueidentifier
	, @NAME               nvarchar(50)
	, @TYPE               nvarchar(15)
	, @USER_ID            uniqueidentifier
	, @MAIL_SENDTYPE      nvarchar(8)
	, @MAIL_SMTPTYPE      nvarchar(20)
	, @MAIL_SMTPSERVER    nvarchar(100)
	, @MAIL_SMTPPORT      int
	, @MAIL_SMTPUSER      nvarchar(100)
	, @MAIL_SMTPPASS      nvarchar(100)
	, @MAIL_SMTPAUTH_REQ  bit
	, @MAIL_SMTPSSL       int
	, @FROM_NAME          nvarchar(100) = null
	, @FROM_ADDR          nvarchar(100) = null
	)
as
  begin
	set nocount on
	
	if not exists(select * from OUTBOUND_EMAILS where ID = @ID) begin -- then
		-- 07/09/2010 Paul.  Don't create the OUTBOUND_EMAILS record unless the SMTP User is specified. 
		if @MAIL_SMTPUSER is not null begin -- then
			if dbo.fnIsEmptyGuid(@ID) = 1 begin -- then
				set @ID = newid();
			end -- if;
			insert into OUTBOUND_EMAILS
				( ID                
				, CREATED_BY        
				, DATE_ENTERED      
				, MODIFIED_USER_ID  
				, DATE_MODIFIED     
				, DATE_MODIFIED_UTC 
				, NAME              
				, TYPE              
				, USER_ID           
				, MAIL_SENDTYPE     
				, MAIL_SMTPTYPE     
				, MAIL_SMTPSERVER   
				, MAIL_SMTPPORT     
				, MAIL_SMTPUSER     
				, MAIL_SMTPPASS     
				, MAIL_SMTPAUTH_REQ 
				, MAIL_SMTPSSL      
				, FROM_NAME         
				, FROM_ADDR         
				)
			values 	( @ID                
				, @MODIFIED_USER_ID        
				,  getdate()         
				, @MODIFIED_USER_ID  
				,  getdate()         
				,  getutcdate()      
				, @NAME              
				, @TYPE              
				, @USER_ID           
				, @MAIL_SENDTYPE     
				, @MAIL_SMTPTYPE     
				, @MAIL_SMTPSERVER   
				, @MAIL_SMTPPORT     
				, @MAIL_SMTPUSER     
				, @MAIL_SMTPPASS     
				, @MAIL_SMTPAUTH_REQ 
				, @MAIL_SMTPSSL      
				, @FROM_NAME         
				, @FROM_ADDR         
				);
		end -- if;
	end else begin
		update OUTBOUND_EMAILS
		   set MODIFIED_USER_ID   = @MODIFIED_USER_ID  
		     , DATE_MODIFIED      =  getdate()         
		     , DATE_MODIFIED_UTC  =  getutcdate()      
		     , NAME               = @NAME              
		     , TYPE               = @TYPE              
		     , USER_ID            = @USER_ID           
		     , MAIL_SENDTYPE      = @MAIL_SENDTYPE     
		     , MAIL_SMTPTYPE      = @MAIL_SMTPTYPE     
		     , MAIL_SMTPSERVER    = @MAIL_SMTPSERVER   
		     , MAIL_SMTPPORT      = @MAIL_SMTPPORT     
		     , MAIL_SMTPUSER      = @MAIL_SMTPUSER     
		     , MAIL_SMTPPASS      = @MAIL_SMTPPASS     
		     , MAIL_SMTPAUTH_REQ  = @MAIL_SMTPAUTH_REQ 
		     , MAIL_SMTPSSL       = @MAIL_SMTPSSL      
		     , FROM_NAME          = @FROM_NAME         
		     , FROM_ADDR          = @FROM_ADDR         
		 where ID                 = @ID                ;
	end -- if;
  end
GO

Grant Execute on dbo.spOUTBOUND_EMAILS_Update to public;
GO


