if exists (select * from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = 'vwEMAILS_Inbound')
	Drop View dbo.vwEMAILS_Inbound;
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
-- 05/20/2009 Paul.  When checking for inbound emails, make sure not to filter by deleted, otherwise the email could get imported again. 
Create View dbo.vwEMAILS_Inbound
as
select ID
     , MESSAGE_ID
  from EMAILS
 where MESSAGE_ID is not null

GO

Grant Select on dbo.vwEMAILS_Inbound to public;
GO

 

