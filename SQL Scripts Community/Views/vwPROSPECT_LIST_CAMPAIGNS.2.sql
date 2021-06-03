if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROSPECT_LIST_CAMPAIGNS')
	Drop View dbo.vwPROSPECT_LIST_CAMPAIGNS;
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
Create View dbo.vwPROSPECT_LIST_CAMPAIGNS
as
select PROSPECT_LISTS.ID               as PROSPECT_LIST_ID
     , PROSPECT_LISTS.NAME             as PROSPECT_LIST_NAME
     , PROSPECT_LISTS.ASSIGNED_USER_ID as PROSPECT_ASSIGNED_USER_ID
     , vwCAMPAIGNS.ID                  as CAMPAIGN_ID
     , vwCAMPAIGNS.NAME                as CAMPAIGN_NAME
     , vwCAMPAIGNS.*
  from            PROSPECT_LISTS
       inner join PROSPECT_LIST_CAMPAIGNS
               on PROSPECT_LIST_CAMPAIGNS.PROSPECT_LIST_ID  = PROSPECT_LISTS.ID
              and PROSPECT_LIST_CAMPAIGNS.DELETED           = 0
       inner join vwCAMPAIGNS
               on vwCAMPAIGNS.ID                            = PROSPECT_LIST_CAMPAIGNS.CAMPAIGN_ID
 where PROSPECT_LISTS.DELETED = 0

GO

Grant Select on dbo.vwPROSPECT_LIST_CAMPAIGNS to public;
GO


