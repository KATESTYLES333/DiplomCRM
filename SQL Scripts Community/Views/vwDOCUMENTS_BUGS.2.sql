if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwDOCUMENTS_BUGS')
	Drop View dbo.vwDOCUMENTS_BUGS;
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
Create View dbo.vwDOCUMENTS_BUGS
as
select DOCUMENTS.ID               as DOCUMENT_ID
     , DOCUMENTS.DOCUMENT_NAME    as DOCUMENT_NAME
     , DOCUMENTS.ASSIGNED_USER_ID as DOCUMENT_ASSIGNED_USER_ID
     , vwBUGS.ID                  as BUG_ID
     , vwBUGS.NAME                as BUG_NAME
     , vwBUGS.*
  from           DOCUMENTS
      inner join DOCUMENTS_BUGS
              on DOCUMENTS_BUGS.DOCUMENT_ID = DOCUMENTS.ID
             and DOCUMENTS_BUGS.DELETED     = 0
      inner join vwBUGS
              on vwBUGS.ID                  = DOCUMENTS_BUGS.BUG_ID
 where DOCUMENTS.DELETED = 0

GO

Grant Select on dbo.vwDOCUMENTS_BUGS to public;
GO



