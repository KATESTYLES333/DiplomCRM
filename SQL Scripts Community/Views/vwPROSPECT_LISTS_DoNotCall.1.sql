if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROSPECT_LISTS_DoNotCall')
	Drop View dbo.vwPROSPECT_LISTS_DoNotCall;
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
Create View dbo.vwPROSPECT_LISTS_DoNotCall
as
select PROSPECT_LISTS.ID                                         as ID
     , PROSPECT_LISTS.NAME                                       as NAME
     , PROSPECT_LISTS.LIST_TYPE                                  as LIST_TYPE
     , PROSPECT_LISTS_PROSPECTS.RELATED_TYPE                     as RELATED_TYPE
     , CONTACTS.ID                                               as RELATED_ID
     , dbo.fnFullName(CONTACTS.FIRST_NAME, CONTACTS.LAST_NAME)   as RELATED_NAME
     , CONTACTS.PHONE_WORK                                       as PHONE_WORK
  from            PROSPECT_LISTS
       inner join PROSPECT_LISTS_PROSPECTS
               on PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID = PROSPECT_LISTS.ID
              and PROSPECT_LISTS_PROSPECTS.RELATED_TYPE     = N'Contacts'
              and PROSPECT_LISTS_PROSPECTS.DELETED          = 0
       inner join CONTACTS
               on CONTACTS.ID                               = PROSPECT_LISTS_PROSPECTS.RELATED_ID
              and CONTACTS.DELETED                          = 0
 where PROSPECT_LISTS.DELETED = 0
   and CONTACTS.DO_NOT_CALL = 1
union all
select PROSPECT_LISTS.ID                                         as ID
     , PROSPECT_LISTS.NAME                                       as NAME
     , PROSPECT_LISTS.LIST_TYPE                                  as LIST_TYPE
     , PROSPECT_LISTS_PROSPECTS.RELATED_TYPE                     as RELATED_TYPE
     , PROSPECTS.ID                                              as RELATED_ID
     , dbo.fnFullName(PROSPECTS.FIRST_NAME, PROSPECTS.LAST_NAME) as RELATED_NAME
     , PROSPECTS.PHONE_WORK                                      as PHONE_WORK
  from            PROSPECT_LISTS
       inner join PROSPECT_LISTS_PROSPECTS
               on PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID = PROSPECT_LISTS.ID
              and PROSPECT_LISTS_PROSPECTS.RELATED_TYPE     = N'Prospects'
              and PROSPECT_LISTS_PROSPECTS.DELETED          = 0
       inner join PROSPECTS
               on PROSPECTS.ID                              = PROSPECT_LISTS_PROSPECTS.RELATED_ID
              and PROSPECTS.DELETED                         = 0
 where PROSPECT_LISTS.DELETED = 0
   and PROSPECTS.DO_NOT_CALL = 1
union all
select PROSPECT_LISTS.ID                                         as ID
     , PROSPECT_LISTS.NAME                                       as NAME
     , PROSPECT_LISTS.LIST_TYPE                                  as LIST_TYPE
     , PROSPECT_LISTS_PROSPECTS.RELATED_TYPE                     as RELATED_TYPE
     , LEADS.ID                                                  as RELATED_ID
     , dbo.fnFullName(LEADS.FIRST_NAME, LEADS.LAST_NAME)         as RELATED_NAME
     , LEADS.PHONE_WORK                                          as PHONE_WORK
  from            PROSPECT_LISTS
       inner join PROSPECT_LISTS_PROSPECTS
               on PROSPECT_LISTS_PROSPECTS.PROSPECT_LIST_ID = PROSPECT_LISTS.ID
              and PROSPECT_LISTS_PROSPECTS.RELATED_TYPE     = N'Leads'
              and PROSPECT_LISTS_PROSPECTS.DELETED          = 0
       inner join LEADS
               on LEADS.ID                                  = PROSPECT_LISTS_PROSPECTS.RELATED_ID
              and LEADS.DELETED                             = 0
 where PROSPECT_LISTS.DELETED = 0
   and LEADS.DO_NOT_CALL = 1
GO

Grant Select on dbo.vwPROSPECT_LISTS_DoNotCall to public;
GO



