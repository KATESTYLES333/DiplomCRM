if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwBUGS_CASES')
	Drop View dbo.vwBUGS_CASES;
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
Create View dbo.vwBUGS_CASES
as
select BUGS.ID               as BUG_ID
     , BUGS.NAME             as BUG_NAME
     , BUGS.ASSIGNED_USER_ID as BUG_ASSIGNED_USER_ID
     , vwCASES.ID            as CASE_ID
     , vwCASES.NAME          as CASE_NAME
     , vwCASES.*
  from           BUGS
      inner join CASES_BUGS
              on CASES_BUGS.BUG_ID  = BUGS.ID
             and CASES_BUGS.DELETED = 0
      inner join vwCASES
              on vwCASES.ID         = CASES_BUGS.CASE_ID
 where BUGS.DELETED = 0

GO

Grant Select on dbo.vwBUGS_CASES to public;
GO



