if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwMODULES_AppVars')
	Drop View dbo.vwMODULES_AppVars;
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
-- 12/30/2007 Paul.  We need a dynamic way to determine if the module record can be assigned or placed in a team. 
-- 01/01/2008 Paul.  Notes is a special module that assumes the identity of its parent. Allow it to use ASSIGNED_USER_ID. 
-- 01/01/2008 Paul.  Products is a special module that assumes the identity of its Order. Allow it to use ASSIGNED_USER_ID. 
-- 01/01/2008 Paul.  Activites are also special in that they assume the identity of thier base. 
-- 01/18/2008 Paul.  Oracle requires unicode strings. 
-- 09/08/2009 Paul.  Custom Paging can be enabled /disabled per module. 
-- 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
-- 01/13/2010 Paul.  Allow default search to be disabled. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 04/04/2010 Paul.  Add Exchange Folders flag. 
-- 04/05/2010 Paul.  Add Exchange Create Parent flag. Need to be able to disable Account creation. 
-- 05/06/2010 Paul.  Add DISPLAY_NAME for the Six theme. 
-- 05/06/2010 Paul.  Add IS_ADMIN for the Six theme. 
-- 09/09/2010 Paul.  Change the case statements to make the code EffiProz friendly. 
-- 08/22/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 03/13/2013 Paul.  Fix IS_ASSIGNED to ensure that it returns 1 or 0. NOTES should return 1. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
Create View dbo.vwMODULES_AppVars
as
select MODULE_NAME
     , TABLE_NAME
     , RELATIVE_PATH
     , (select count(*)
         from vwSqlTablesAudited
        where vwSqlTablesAudited.TABLE_NAME = vwMODULES.TABLE_NAME
       ) as IS_AUDITED
     , (case MODULE_NAME 
        when N'Activities' then 1
        else (select count(*)
                from vwSqlColumns
               where vwSqlColumns.ObjectName = vwMODULES.TABLE_NAME
                 and ColumnName = N'TEAM_ID'
             )
        end) as IS_TEAMED
     , (case MODULE_NAME 
        when N'Activities' then 1
        when N'Products'   then 1
        else (select count(*)
                from vwSqlColumns
               where vwSqlColumns.ObjectName = vwMODULES.TABLE_NAME
                 and ColumnName = N'ASSIGNED_USER_ID'
             )
        end) as IS_ASSIGNED
     , CUSTOM_PAGING
     , MASS_UPDATE_ENABLED
     , DEFAULT_SEARCH_ENABLED
     , EXCHANGE_SYNC
     , EXCHANGE_FOLDERS
     , EXCHANGE_CREATE_PARENT
     , DISPLAY_NAME
     , IS_ADMIN
     , REST_ENABLED
     , DUPLICATE_CHECHING_ENABLED
  from vwMODULES
GO

Grant Select on dbo.vwMODULES_AppVars to public;
GO



