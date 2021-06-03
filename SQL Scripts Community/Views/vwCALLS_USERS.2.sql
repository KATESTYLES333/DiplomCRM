if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCALLS_USERS')
	Drop View dbo.vwCALLS_USERS;
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
Create View dbo.vwCALLS_USERS
as
select CALLS.ID               as CALL_ID
     , CALLS.NAME             as CALL_NAME
     , CALLS.ASSIGNED_USER_ID as CALL_ASSIGNED_USER_ID
     , USERS.ID               as USER_ID
     , dbo.fnFullName(USERS.FIRST_NAME, USERS.LAST_NAME) as FULL_NAME
     , USERS.USER_NAME
     , USERS.EMAIL1
     , USERS.PHONE_WORK
     , CALLS_USERS.DATE_ENTERED
  from           CALLS
      inner join CALLS_USERS
              on CALLS_USERS.CALL_ID = CALLS.ID
             and CALLS_USERS.DELETED = 0
      inner join USERS
              on USERS.ID            = CALLS_USERS.USER_ID
             and USERS.DELETED       = 0
 where CALLS.DELETED = 0

GO

Grant Select on dbo.vwCALLS_USERS to public;
GO


