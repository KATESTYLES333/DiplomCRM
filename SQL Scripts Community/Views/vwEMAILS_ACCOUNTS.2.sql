if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEMAILS_ACCOUNTS')
	Drop View dbo.vwEMAILS_ACCOUNTS;
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
-- 10/28/2007 Paul.  The include the email parent, but not union all so that duplicates will get filtered. 
-- 11/19/2007 Paul.  We must use union all because an ntext custom field will break the view.
-- The ultimate solution is to convert all ntext to nvarchar(max), but then we would need to drop support for SQL 2000. 
-- 09/09/2008 Paul.  Using a union causes a problem if there is an NTEXT custom field in the PROSPECTS_CSTM table. 
-- The text, ntext, or image data type cannot be selected as DISTINCT.
-- One solution would be to use UNION ALL, but then we would get duplicate records. 
-- So we still use UNION ALL, but also use an outer join to block duplicates. 
Create View dbo.vwEMAILS_ACCOUNTS
as
select EMAILS.ID               as EMAIL_ID
     , EMAILS.NAME             as EMAIL_NAME
     , EMAILS.ASSIGNED_USER_ID as EMAIL_ASSIGNED_USER_ID
     , vwACCOUNTS.ID           as ACCOUNT_ID
     , vwACCOUNTS.NAME         as ACCOUNT_NAME
     , vwACCOUNTS.*
  from            EMAILS
       inner join EMAILS_ACCOUNTS
               on EMAILS_ACCOUNTS.EMAIL_ID = EMAILS.ID
              and EMAILS_ACCOUNTS.DELETED  = 0
       inner join vwACCOUNTS
               on vwACCOUNTS.ID            = EMAILS_ACCOUNTS.ACCOUNT_ID
 where EMAILS.DELETED = 0
union all
select EMAILS.ID               as EMAIL_ID
     , EMAILS.NAME             as EMAIL_NAME
     , EMAILS.ASSIGNED_USER_ID as EMAIL_ASSIGNED_USER_ID
     , vwACCOUNTS.ID           as ACCOUNT_ID
     , vwACCOUNTS.NAME         as ACCOUNT_NAME
     , vwACCOUNTS.*
  from            EMAILS
       inner join vwACCOUNTS
               on vwACCOUNTS.ID              = EMAILS.PARENT_ID
  left outer join EMAILS_ACCOUNTS
               on EMAILS_ACCOUNTS.EMAIL_ID   = EMAILS.ID
              and EMAILS_ACCOUNTS.ACCOUNT_ID = vwACCOUNTS.ID
              and EMAILS_ACCOUNTS.DELETED    = 0
 where EMAILS.DELETED     = 0
   and EMAILS.PARENT_TYPE = N'Accounts'
   and EMAILS_ACCOUNTS.ID is null

GO

Grant Select on dbo.vwEMAILS_ACCOUNTS to public;
GO


