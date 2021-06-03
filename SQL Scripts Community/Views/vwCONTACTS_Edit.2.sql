if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCONTACTS_Edit')
	Drop View dbo.vwCONTACTS_Edit;
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
-- 02/09/2006 Paul.  SugarCRM uses the CONTACTS_USERS table to allow each user to 
-- choose the contacts they want sync'd with Outlook. 
-- This view requires that the USER_ID be set, otherwise too many records will be returned 
-- due to the CONTACTS_USERS join.
-- 03/06/2006 Paul.  The join to CONTACTS_USERS must occur external to the view. 
-- This is the only way to ensure that the record is always returned, with the sync flag set. 
-- 11/08/2008 Paul.  Move description to base view. 
-- 03/05/2009 Paul.  Add PORTAL_PASSWORD for Splendid Portal.  Sugar added it in 4.5.0. 
Create View dbo.vwCONTACTS_Edit
as
select vwCONTACTS.*
     , dbo.fnFullAddressHtml(vwCONTACTS.PRIMARY_ADDRESS_STREET, vwCONTACTS.PRIMARY_ADDRESS_CITY, vwCONTACTS.PRIMARY_ADDRESS_STATE, vwCONTACTS.PRIMARY_ADDRESS_POSTALCODE, vwCONTACTS.PRIMARY_ADDRESS_COUNTRY) as PRIMARY_ADDRESS_HTML
     , dbo.fnFullAddressHtml(vwCONTACTS.ALT_ADDRESS_STREET    , vwCONTACTS.ALT_ADDRESS_CITY    , vwCONTACTS.ALT_ADDRESS_STATE    , vwCONTACTS.ALT_ADDRESS_POSTALCODE    , vwCONTACTS.ALT_ADDRESS_COUNTRY    ) as ALT_ADDRESS_HTML
     , CONTACTS.PORTAL_PASSWORD
  from            vwCONTACTS
  left outer join CONTACTS
               on CONTACTS.ID               = vwCONTACTS.ID

GO

Grant Select on dbo.vwCONTACTS_Edit to public;
GO



