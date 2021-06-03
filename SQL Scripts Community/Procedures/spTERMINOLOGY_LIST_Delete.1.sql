if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spTERMINOLOGY_LIST_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spTERMINOLOGY_LIST_Delete;
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
Create Procedure dbo.spTERMINOLOGY_LIST_Delete
	( @ID               uniqueidentifier
	, @MODIFIED_USER_ID uniqueidentifier
	)
as
  begin
	set nocount on
	
	declare @LANG        nvarchar(10);
	declare @MODULE_NAME nvarchar(20);
	declare @LIST_NAME   nvarchar(50);
	declare @LIST_ORDER  int;

	if exists(select * from TERMINOLOGY where ID = @ID and DELETED = 0) begin -- then
		select @LANG        = LANG
		     , @MODULE_NAME = MODULE_NAME
		     , @LIST_NAME   = LIST_NAME
		     , @LIST_ORDER  = LIST_ORDER
		  from TERMINOLOGY
		 where ID           = @ID;

			-- 04/02/2006 Paul.  Catch the Oracle NO_DATA_FOUND exception. 
		-- BEGIN Oracle Exception
			update TERMINOLOGY
			   set DELETED          = 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = @MODIFIED_USER_ID
			 where ID               = @ID
			   and DELETED          = 0;
		-- END Oracle Exception
		
		-- BEGIN Oracle Exception
			update TERMINOLOGY
			   set LIST_ORDER       = LIST_ORDER - 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = @MODIFIED_USER_ID
			 where LANG             = @LANG
			   and (MODULE_NAME = @MODULE_NAME or (MODULE_NAME is null and @MODULE_NAME is null))
			   and LIST_NAME        = @LIST_NAME
			   and LIST_ORDER       > @LIST_ORDER
			   and DELETED          = 0;
		-- END Oracle Exception
	end -- if;
  end
GO

Grant Execute on dbo.spTERMINOLOGY_LIST_Delete to public;
GO



