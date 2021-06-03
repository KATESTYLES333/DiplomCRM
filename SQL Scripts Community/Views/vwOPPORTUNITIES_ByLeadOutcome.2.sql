if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwOPPORTUNITIES_ByLeadOutcome')
	Drop View dbo.vwOPPORTUNITIES_ByLeadOutcome;
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
-- 09/20/2005 Paul.  Outer join to TERMINOLOGY just in case the data is missing. 
-- 02/01/2006 Paul.  DB2 does not like comments in the middle of the Create View statement. 
-- 11/27/2006 Paul.  Add TEAM_ID. 
-- 08/30/2009 Paul.  Dynamic teams required an ID and TEAM_SET_ID. 
Create View dbo.vwOPPORTUNITIES_ByLeadOutcome
as
select OPPORTUNITIES.ID
     , (case OPPORTUNITIES.SALES_STAGE
        when N'Closed Lost' then N'Closed Lost'
        when N'Closed Won'  then N'Closed Won'  
        else N'Other'
        end) as SALES_STAGE
     , isnull(OPPORTUNITIES.LEAD_SOURCE, N'') as LEAD_SOURCE
     , OPPORTUNITIES.ASSIGNED_USER_ID
     , OPPORTUNITIES.AMOUNT_USDOLLAR
     , OPPORTUNITIES.TEAM_ID
     , OPPORTUNITIES.TEAM_SET_ID
     , USERS.USER_NAME
     , TERMINOLOGY.LIST_ORDER
  from            OPPORTUNITIES
       inner join USERS
               on USERS.ID              = OPPORTUNITIES.ASSIGNED_USER_ID
              and USERS.DELETED         = 0
  left outer join TERMINOLOGY
               on TERMINOLOGY.NAME      = OPPORTUNITIES.LEAD_SOURCE
              and TERMINOLOGY.LIST_NAME = N'lead_source_dom'
              and TERMINOLOGY.LANG      = N'en-US'
              and TERMINOLOGY.DELETED   = 0
 where OPPORTUNITIES.DELETED = 0

GO

Grant Select on dbo.vwOPPORTUNITIES_ByLeadOutcome to public;
GO


