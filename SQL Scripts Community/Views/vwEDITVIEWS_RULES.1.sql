if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEDITVIEWS_RULES')
	Drop View dbo.vwEDITVIEWS_RULES;
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
-- 11/11/2010 Paul.  Change to Pre Load and Post Load. 
Create View dbo.vwEDITVIEWS_RULES
as
select EDITVIEWS.ID
     , EDITVIEWS.NAME
     , EDITVIEWS.MODULE_NAME
     , EDITVIEWS.VIEW_NAME
     , NEW_EVENT_RULES.ID           as NEW_EVENT_ID
     , NEW_EVENT_RULES.NAME         as NEW_EVENT_NAME
     , NEW_EVENT_RULES.XOML         as NEW_EVENT_XOML
     , PRE_LOAD_EVENT_RULES.ID      as PRE_LOAD_EVENT_ID
     , PRE_LOAD_EVENT_RULES.NAME    as PRE_LOAD_EVENT_NAME
     , PRE_LOAD_EVENT_RULES.XOML    as PRE_LOAD_EVENT_XOML
     , POST_LOAD_EVENT_RULES.ID     as POST_LOAD_EVENT_ID
     , POST_LOAD_EVENT_RULES.NAME   as POST_LOAD_EVENT_NAME
     , POST_LOAD_EVENT_RULES.XOML   as POST_LOAD_EVENT_XOML
     , VALIDATION_EVENT_RULES.ID    as VALIDATION_EVENT_ID
     , VALIDATION_EVENT_RULES.NAME  as VALIDATION_EVENT_NAME
     , VALIDATION_EVENT_RULES.XOML  as VALIDATION_EVENT_XOML
     , PRE_SAVE_EVENT_RULES.ID      as PRE_SAVE_EVENT_ID
     , PRE_SAVE_EVENT_RULES.NAME    as PRE_SAVE_EVENT_NAME
     , PRE_SAVE_EVENT_RULES.XOML    as PRE_SAVE_EVENT_XOML
     , POST_SAVE_EVENT_RULES.ID     as POST_SAVE_EVENT_ID
     , POST_SAVE_EVENT_RULES.NAME   as POST_SAVE_EVENT_NAME
     , POST_SAVE_EVENT_RULES.XOML   as POST_SAVE_EVENT_XOML
  from            EDITVIEWS
  left outer join RULES                            NEW_EVENT_RULES
               on NEW_EVENT_RULES.ID             = EDITVIEWS.NEW_EVENT_ID
              and NEW_EVENT_RULES.DELETED        = 0
  left outer join RULES                            PRE_LOAD_EVENT_RULES
               on PRE_LOAD_EVENT_RULES.ID        = EDITVIEWS.PRE_LOAD_EVENT_ID
              and PRE_LOAD_EVENT_RULES.DELETED   = 0
  left outer join RULES                            POST_LOAD_EVENT_RULES
               on POST_LOAD_EVENT_RULES.ID       = EDITVIEWS.POST_LOAD_EVENT_ID
              and POST_LOAD_EVENT_RULES.DELETED  = 0
  left outer join RULES                            VALIDATION_EVENT_RULES
               on VALIDATION_EVENT_RULES.ID      = EDITVIEWS.VALIDATION_EVENT_ID
              and VALIDATION_EVENT_RULES.DELETED = 0
  left outer join RULES                            PRE_SAVE_EVENT_RULES
               on PRE_SAVE_EVENT_RULES.ID        = EDITVIEWS.PRE_SAVE_EVENT_ID
              and PRE_SAVE_EVENT_RULES.DELETED   = 0
  left outer join RULES                            POST_SAVE_EVENT_RULES
               on POST_SAVE_EVENT_RULES.ID       = EDITVIEWS.POST_SAVE_EVENT_ID
              and POST_SAVE_EVENT_RULES.DELETED  = 0
 where EDITVIEWS.DELETED = 0

GO

Grant Select on dbo.vwEDITVIEWS_RULES to public;
GO


