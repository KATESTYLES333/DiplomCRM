if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwLEADS_PROSPECT_LISTS')
	Drop View dbo.vwLEADS_PROSPECT_LISTS;
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
Create View dbo.vwLEADS_PROSPECT_LISTS
as
select LEADS.ID                   as LEAD_ID
     , dbo.fnFullName(LEADS.FIRST_NAME, LEADS.LAST_NAME) as LEAD_NAME
     , LEADS.ASSIGNED_USER_ID     as LEAD_ASSIGNED_USER_ID
     , vwPROSPECT_LISTS.ID        as PROSPECT_LIST_ID
     , vwPROSPECT_LISTS.NAME      as PROSPECT_LIST_NAME
     , vwPROSPECT_LISTS.*
     , (select count(*)
          from PROSPECT_LISTS_PROSPECTS
         where PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID = vwPROSPECT_LISTS.ID
           and PROSPECT_LISTS_PROSPECTS.DELETED          = 0
       ) as ENTRIES
  from            LEADS
       inner join PROSPECT_LISTS_PROSPECTS
               on PROSPECT_LISTS_PROSPECTS.RELATED_ID   = LEADS.ID
              and PROSPECT_LISTS_PROSPECTS.RELATED_TYPE = N'Leads'
              and PROSPECT_LISTS_PROSPECTS.DELETED      = 0
       inner join vwPROSPECT_LISTS
               on vwPROSPECT_LISTS.ID                   = PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID
 where LEADS.DELETED = 0

GO

Grant Select on dbo.vwLEADS_PROSPECT_LISTS to public;
GO


