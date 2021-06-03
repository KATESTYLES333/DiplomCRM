if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwUSERS_Edit')
	Drop View dbo.vwUSERS_Edit;
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
-- 11/08/2008 Paul.  Move description to base view. 
-- 07/09/2010 Paul.  SMTP values belong in the OUTBOUND_EMAILS table. 
Create View dbo.vwUSERS_Edit
as
select vwUSERS.*
     , dbo.fnFullAddressHtml(vwUSERS.ADDRESS_STREET, vwUSERS.ADDRESS_CITY, vwUSERS.ADDRESS_STATE, vwUSERS.ADDRESS_POSTALCODE, vwUSERS.ADDRESS_COUNTRY) as ADDRESS_HTML
     , OUTBOUND_EMAILS.MAIL_SMTPUSER
     , OUTBOUND_EMAILS.MAIL_SMTPPASS
  from            vwUSERS
  left outer join USERS
               on USERS.ID = vwUSERS.ID
  left outer join OUTBOUND_EMAILS
               on OUTBOUND_EMAILS.USER_ID         = USERS.ID
              and OUTBOUND_EMAILS.TYPE            = N'system-override'
              and OUTBOUND_EMAILS.DELETED         = 0

GO

Grant Select on dbo.vwUSERS_Edit to public;
GO



