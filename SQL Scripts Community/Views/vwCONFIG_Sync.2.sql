if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCONFIG_Sync')
	Drop View dbo.vwCONFIG_Sync;
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
-- 11/21/2009 Paul.  We need to exclude sensitive data from the offline client. 
-- 03/23/2010 Paul.  Exchange settings will not be synchronized. 
-- 03/19/2011 Paul.  facebook settings will not be synchronized. 
-- 06/05/2011 Paul.  Google settings will not be synchronized. 
-- 10/24/2011 Paul.  Exclude more. 
-- 04/15/2012 Paul.  Excluded Twitter and LinkedIn. 
-- 04/22/2012 Paul.  Excluded Salesforce. 
-- 05/28/2012 Paul.  Excluded QuickBooks. 
-- 12/25/2012 Paul.  Exclude reminder flags. 
-- 09/20/2013 Paul.  Exclude PayTrace, Asterisk and Twilio. 
-- 08/24/2014 Paul.  Exclude Avaya, MicrosoftTranslator, Password, ZipTaxAPI. 
Create View dbo.vwCONFIG_Sync
as
select ID
     , DELETED
     , CREATED_BY
     , DATE_ENTERED
     , MODIFIED_USER_ID
     , DATE_MODIFIED
     , DATE_MODIFIED_UTC
     , CATEGORY
     , NAME
     , VALUE
  from CONFIG
 where NAME not like 'Amazon%'
   and NAME not like 'CreditCard%'
   and NAME not like 'InboundEmail%'
   and NAME not like 'PaymentGateway%'
   and NAME not like 'PayPal%'
   and NAME not like 'smtp%'
   and NAME not like 'Exchange%'
   and NAME not like 'facebook%'
   and NAME not like 'Google%'
   and NAME not like 'Twitter%'
   and NAME not like 'LinkedIn%'
   and NAME not like 'Salesforce%'
   and NAME not like 'QuickBooks%'
   and NAME not like 'PayTrace%'
   and NAME not like 'Asterisk%'
   and NAME not like 'Twilio%'
   and NAME not like 'Avaya%'
   and NAME not like 'MicrosoftTranslator%'
   and NAME not like 'Password%'
   and NAME not like 'ZipTaxAPI%'
   and NAME not in ('enable_reminder_popdowns', 'enable_email_reminders', 'default_password')

GO

Grant Select on dbo.vwCONFIG_Sync to public;
GO


