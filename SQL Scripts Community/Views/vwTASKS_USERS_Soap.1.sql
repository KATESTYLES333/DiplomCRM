if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwTASKS_USERS_Soap')
	Drop View dbo.vwTASKS_USERS_Soap;
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
-- 10/28/2009 Paul.  Add UTC date to allow this table to sync. 
Create View dbo.vwTASKS_USERS_Soap
as
select TASKS.ID    as PRIMARY_ID
     , USERS.ID    as RELATED_ID
     , TASKS.DELETED
     , TASKS.DATE_MODIFIED
     , TASKS.DATE_MODIFIED_UTC
  from      TASKS
 inner join USERS
         on USERS.ID         = TASKS.ASSIGNED_USER_ID
        and USERS.DELETED    = 0

GO

Grant Select on dbo.vwTASKS_USERS_Soap to public;
GO

