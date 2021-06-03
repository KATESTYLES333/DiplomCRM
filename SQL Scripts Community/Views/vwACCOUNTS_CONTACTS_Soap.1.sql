if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwACCOUNTS_CONTACTS_Soap')
	Drop View dbo.vwACCOUNTS_CONTACTS_Soap;
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
-- 06/13/2007 Paul.  The date to return is that of the related object. 
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
Create View dbo.vwACCOUNTS_CONTACTS_Soap
as
select ACCOUNTS_CONTACTS.ACCOUNT_ID as PRIMARY_ID
     , ACCOUNTS_CONTACTS.CONTACT_ID as RELATED_ID
     , ACCOUNTS_CONTACTS.DELETED
     , CONTACTS.DATE_MODIFIED
     , CONTACTS.DATE_MODIFIED_UTC
  from      ACCOUNTS_CONTACTS
 inner join ACCOUNTS
         on ACCOUNTS.ID      = ACCOUNTS_CONTACTS.ACCOUNT_ID
        and ACCOUNTS.DELETED = ACCOUNTS_CONTACTS.DELETED
 inner join CONTACTS
         on CONTACTS.ID      = ACCOUNTS_CONTACTS.CONTACT_ID
        and CONTACTS.DELETED = ACCOUNTS_CONTACTS.DELETED

GO

Grant Select on dbo.vwACCOUNTS_CONTACTS_Soap to public;
GO


