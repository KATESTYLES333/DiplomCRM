if exists (select * from INFORMATION_SCHEMA.ROUTINES where ROUTINE_NAME = 'spCALL_MKTG_PRSPT_LST_Delete' and ROUTINE_TYPE = 'PROCEDURE')
	Drop Procedure dbo.spCALL_MKTG_PRSPT_LST_Delete;
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
Create Procedure dbo.spCALL_MKTG_PRSPT_LST_Delete
	( @MODIFIED_USER_ID   uniqueidentifier
	, @CALL_MARKETING_ID uniqueidentifier
	, @PROSPECT_LIST_ID   uniqueidentifier
	)
as
  begin
	set nocount on

	-- 12/15/2007 Paul.  When removing the first item, we may need to add all the others. 
	-- Lets add them all, then mark the removed as deleted.  This will give us a record of the remove.
	-- 01/20/2008 Paul.  Only add the existing lists if currently marked as ALL_PROSPECT_LISTS. 
	if exists(select * from CALL_MARKETING where ID = @CALL_MARKETING_ID and ALL_PROSPECT_LISTS = 1 and DELETED = 0) begin -- then
		insert into CALL_MARKETING_PROSPECT_LISTS(CREATED_BY, MODIFIED_USER_ID, CALL_MARKETING_ID, PROSPECT_LIST_ID)
		select @MODIFIED_USER_ID
		     , @MODIFIED_USER_ID
		     , CALL_MARKETING.ID
		     , PROSPECT_LIST_CAMPAIGNS.PROSPECT_LIST_ID
		  from            CALL_MARKETING
		       inner join CAMPAIGNS
		               on CAMPAIGNS.ID                        = CALL_MARKETING.CAMPAIGN_ID
		              and CAMPAIGNS.DELETED                   = 0
		       inner join PROSPECT_LIST_CAMPAIGNS
		               on PROSPECT_LIST_CAMPAIGNS.CAMPAIGN_ID = CAMPAIGNS.ID
		              and PROSPECT_LIST_CAMPAIGNS.DELETED     = 0
		 where CALL_MARKETING.ID      = @CALL_MARKETING_ID
		   and CALL_MARKETING.DELETED = 0;

		-- 12/15/2007 Paul.  Disable the ALL flag when the first item is removed. 
		update CALL_MARKETING
		   set ALL_PROSPECT_LISTS = 0
		     , DATE_MODIFIED      = getdate()
		     , DATE_MODIFIED_UTC  = getutcdate()
		     , MODIFIED_USER_ID   = @MODIFIED_USER_ID
		 where ID                 = @CALL_MARKETING_ID
		   and ALL_PROSPECT_LISTS = 1;
	end -- if;
	
	update CALL_MARKETING_PROSPECT_LISTS
	   set DELETED            = 1
	     , DATE_MODIFIED      = getdate()
	     , DATE_MODIFIED_UTC  = getutcdate()
	     , MODIFIED_USER_ID   = @MODIFIED_USER_ID
	 where CALL_MARKETING_ID = @CALL_MARKETING_ID
	   and PROSPECT_LIST_ID   = @PROSPECT_LIST_ID
	   and DELETED            = 0;
  end
GO

Grant Execute on dbo.spCALL_MKTG_PRSPT_LST_Delete to public;
GO


