if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROSPECT_LISTS_CONTACTS')
	Drop View dbo.vwPROSPECT_LISTS_CONTACTS;
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
-- 04/22/2006 Paul.  SugarCRM 4.0 uses plural RELATED_TYPES, like Contacts, Leads, Prospects and Users. 
-- 12/05/2006 Paul.  Literals should be in unicode to reduce conversions at runtime. 
-- 01/09/2010 Paul.  A Dynamic List is one that uses SQL to build the prospect list. 
Create View dbo.vwPROSPECT_LISTS_CONTACTS
as
select PROSPECT_LISTS.ID               as PROSPECT_LIST_ID
     , PROSPECT_LISTS.NAME             as PROSPECT_LIST_NAME
     , PROSPECT_LISTS.ASSIGNED_USER_ID as PROSPECT_ASSIGNED_USER_ID
     , PROSPECT_LISTS.DYNAMIC_LIST     as PROSPECT_DYNAMIC_LIST
     , vwCONTACTS.ID                   as CONTACT_ID
     , vwCONTACTS.NAME                 as CONTACT_NAME
     , vwCONTACTS.*
  from            PROSPECT_LISTS
       inner join PROSPECT_LISTS_PROSPECTS
               on PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID = PROSPECT_LISTS.ID
              and PROSPECT_LISTS_PROSPECTS.RELATED_TYPE     = N'Contacts'
              and PROSPECT_LISTS_PROSPECTS.DELETED          = 0
       inner join vwCONTACTS
               on vwCONTACTS.ID                             = PROSPECT_LISTS_PROSPECTS.RELATED_ID
 where PROSPECT_LISTS.DELETED = 0

GO

Grant Select on dbo.vwPROSPECT_LISTS_CONTACTS to public;
GO


