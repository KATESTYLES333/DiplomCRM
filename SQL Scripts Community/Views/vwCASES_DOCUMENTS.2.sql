if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwCASES_DOCUMENTS')
	Drop View dbo.vwCASES_DOCUMENTS;
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
-- 01/16/2013 Paul.  Fix SELECTED_DOCUMENT_REVISION_ID. 
Create View dbo.vwCASES_DOCUMENTS
as
select CASES.ID                    as CASE_ID
     , CASES.NAME                  as CASE_NAME
     , CASES.ASSIGNED_USER_ID      as CASE_ASSIGNED_USER_ID
     , DOCUMENT_REVISIONS.ID       as SELECTED_DOCUMENT_REVISION_ID
     , DOCUMENT_REVISIONS.REVISION as SELECTED_REVISION
     , vwDOCUMENTS.ID              as DOCUMENT_ID
     , vwDOCUMENTS.*
  from            CASES
       inner join DOCUMENTS_CASES
               on DOCUMENTS_CASES.CASE_ID         = CASES.ID
              and DOCUMENTS_CASES.DELETED         = 0
       inner join vwDOCUMENTS
               on vwDOCUMENTS.ID                  = DOCUMENTS_CASES.DOCUMENT_ID
  left outer join DOCUMENT_REVISIONS
               on DOCUMENT_REVISIONS.ID           = DOCUMENTS_CASES.DOCUMENT_REVISION_ID
              and DOCUMENT_REVISIONS.DELETED      = 0
 where CASES.DELETED = 0

GO

Grant Select on dbo.vwCASES_DOCUMENTS to public;
GO


