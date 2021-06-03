if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCALLS_CONTACTS')
	Drop View dbo.vwCALLS_CONTACTS;
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
-- 10/25/2009 Paul.  The view needs to include the call if the contact is a parent. 
-- 07/27/2010 Paul.  Add ACCEPT_STATUS so that it can be references in the layout view. 
Create View dbo.vwCALLS_CONTACTS
as
select CALLS.ID               as CALL_ID
     , CALLS.NAME             as CALL_NAME
     , CALLS.ASSIGNED_USER_ID as CALL_ASSIGNED_USER_ID
     , CALLS_CONTACTS.ACCEPT_STATUS
     , vwCONTACTS.ID          as CONTACT_ID
     , vwCONTACTS.NAME        as CONTACT_NAME
     , vwCONTACTS.*
  from            CALLS
       inner join CALLS_CONTACTS
               on CALLS_CONTACTS.CALL_ID = CALLS.ID
              and CALLS_CONTACTS.DELETED = 0
       inner join vwCONTACTS
               on vwCONTACTS.ID          = CALLS_CONTACTS.CONTACT_ID
 where CALLS.DELETED = 0
 union all
select CALLS.ID               as CALL_ID
     , CALLS.NAME             as CALL_NAME
     , CALLS.ASSIGNED_USER_ID as CALL_ASSIGNED_USER_ID
     , CALLS_CONTACTS.ACCEPT_STATUS
     , vwCONTACTS.ID          as CONTACT_ID
     , vwCONTACTS.NAME        as CONTACT_NAME
     , vwCONTACTS.*
  from            CALLS
       inner join vwCONTACTS
               on vwCONTACTS.ID             = CALLS.PARENT_ID
  left outer join CALLS_CONTACTS
               on CALLS_CONTACTS.CALL_ID    = CALLS.ID
              and CALLS_CONTACTS.CONTACT_ID = vwCONTACTS.ID
              and CALLS_CONTACTS.DELETED    = 0
 where CALLS.DELETED     = 0
   and CALLS.PARENT_TYPE = N'Contacts'
   and CALLS_CONTACTS.ID is null

GO

Grant Select on dbo.vwCALLS_CONTACTS to public;
GO

