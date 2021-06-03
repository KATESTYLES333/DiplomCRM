if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwPROJECTS_BUGS')
	Drop View dbo.vwPROJECTS_BUGS;
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
Create View dbo.vwPROJECTS_BUGS
as
select PROJECT.ID               as PROJECT_ID
     , PROJECT.NAME             as PROJECT_NAME
     , PROJECT.ASSIGNED_USER_ID as PROJECT_ASSIGNED_USER_ID
     , vwBUGS.ID                as BUG_ID
     , vwBUGS.NAME              as BUG_NAME
     , vwBUGS.*
  from           PROJECT
      inner join PROJECTS_BUGS
              on PROJECTS_BUGS.PROJECT_ID = PROJECT.ID
             and PROJECTS_BUGS.DELETED    = 0
      inner join vwBUGS
              on vwBUGS.ID                = PROJECTS_BUGS.BUG_ID
 where PROJECT.DELETED = 0

GO

Grant Select on dbo.vwPROJECTS_BUGS to public;
GO


