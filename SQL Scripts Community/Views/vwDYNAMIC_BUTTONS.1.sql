if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDYNAMIC_BUTTONS')
	Drop View dbo.vwDYNAMIC_BUTTONS;
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
-- 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
Create View dbo.vwDYNAMIC_BUTTONS
as
select ID
     , VIEW_NAME
     , CONTROL_INDEX
     , CONTROL_TYPE
     , DEFAULT_VIEW
     , MODULE_NAME
     , MODULE_ACCESS_TYPE
     , TARGET_NAME
     , TARGET_ACCESS_TYPE
     , MOBILE_ONLY
     , ADMIN_ONLY
     , EXCLUDE_MOBILE
     , CONTROL_TEXT
     , CONTROL_TOOLTIP
     , CONTROL_ACCESSKEY
     , CONTROL_CSSCLASS
     , TEXT_FIELD
     , ARGUMENT_FIELD
     , COMMAND_NAME
     , URL_FORMAT
     , URL_TARGET
     , ONCLICK_SCRIPT
     , HIDDEN
  from DYNAMIC_BUTTONS
 where DELETED = 0

GO

Grant Select on dbo.vwDYNAMIC_BUTTONS to public;
GO


