if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROJECTS_CASES')
	Drop View dbo.vwPROJECTS_CASES;
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
-- 09/08/2012 Paul.  Project Relations data for Cases moved to PROJECTS_CASES. 
Create View dbo.vwPROJECTS_CASES
as
select PROJECT.ID               as PROJECT_ID
     , PROJECT.NAME             as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID as PROJECT_ASSIGNED_USER_ID
     , vwCASES.ID               as CASE_ID
     , vwCASES.NAME             as CASE_NAME
     , vwCASES.*
  from           PROJECT
      inner join PROJECTS_CASES
              on PROJECTS_CASES.PROJECT_ID = PROJECT.ID
             and PROJECTS_CASES.DELETED    = 0
      inner join vwCASES
              on vwCASES.ID                = PROJECTS_CASES.CASE_ID
 where PROJECT.DELETED = 0

GO

Grant Select on dbo.vwPROJECTS_CASES to public;
GO



