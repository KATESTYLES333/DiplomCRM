

print 'EDITVIEWS_FIELDS defaults';
--delete from EDITVIEWS_FIELDS where EDIT_NAME like '%.EditView'
--GO

set nocount on;
GO

-- 11/17/2007 Paul.  Add spEDITVIEWS_InsertOnly to simplify creation of Mobile views.
-- 11/24/2006 Paul.  Add TEAM_ID for team management. 
-- 11/25/2006 Paul.  Convert Assigned To from a ListBox to a ChangeButton. 
-- 08/22/2008 Paul.  Move professional modules to a separate file. 
-- 08/28/2008 Paul.  International users are having trouble with the Phone Number validator. Remove them for now. 
-- 09/14/2008 Paul.  DB2 does not work well with optional parameters. 
-- 08/24/2009 Paul.  Change TEAM_NAME to TEAM_SET_NAME. 
-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
-- 08/01/2010 Paul.  Add ASSIGNED_TO_NAME so that we can display the full name in lists like Sugar. 
-- 04/02/2012 Paul.  Add ASSIGNED_USER_ID to Notes, Documents, EmailTemplates. 

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Accounts.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Accounts.EditView'       , 'Accounts'      , 'vwACCOUNTS_Edit'      , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  0, 'Accounts.LBL_ACCOUNT_NAME'              , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  1, 'Accounts.LBL_PHONE'                     , 'PHONE_OFFICE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  1, 'Phone Number'                           , 'PHONE_OFFICE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  2, 'Accounts.LBL_WEBSITE'                   , 'WEBSITE'                    , 0, 1, 255, 28, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  3, 'Accounts.LBL_FAX'                       , 'PHONE_FAX'                  , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  3, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  4, 'Accounts.LBL_TICKER_SYMBOL'             , 'TICKER_SYMBOL'              , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  5, 'Accounts.LBL_OTHER_PHONE'               , 'PHONE_ALTERNATE'            , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  5, 'Phone Number'                           , 'PHONE_ALTERNATE'            , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Accounts.EditView'       ,  6, 'Accounts.LBL_MEMBER_OF'                 , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'Accounts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  7, 'Accounts.LBL_EMAIL'                     , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  7, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  8, 'Accounts.LBL_EMPLOYEES'                 , 'EMPLOYEES'                  , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       ,  9, 'Accounts.LBL_OTHER_EMAIL_ADDRESS'       , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  9, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 10, 'Accounts.LBL_OWNERSHIP'                 , 'OWNERSHIP'                  , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 11, 'Accounts.LBL_RATING'                    , 'RATING'                     , 0, 2,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Accounts.EditView'       , 12, 'Accounts.LBL_INDUSTRY'                  , 'INDUSTRY'                   , 0, 1, 'industry_dom'       , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 13, 'Accounts.LBL_SIC_CODE'                  , 'SIC_CODE'                   , 0, 2,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Accounts.EditView'       , 14, 'Accounts.LBL_TYPE'                      , 'ACCOUNT_TYPE'               , 0, 1, 'account_type_dom'   , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 15, 'Accounts.LBL_ANNUAL_REVENUE'            , 'ANNUAL_REVENUE'             , 0, 2,  25, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Accounts.EditView'       , 16, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Accounts.EditView'       , 17, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Accounts.EditView'       , 18, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Accounts.EditView'       , 19, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Accounts.EditView'       , 20;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditView'       , 21, 'Accounts.LBL_BILLING_ADDRESS_STREET'    , 'BILLING_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Accounts.EditView'       , 22, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditView'       , 23, 'Accounts.LBL_SHIPPING_ADDRESS_STREET'   , 'SHIPPING_ADDRESS_STREET'    , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 24, 'Accounts.LBL_CITY'                      , 'BILLING_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 25, 'Accounts.LBL_CITY'                      , 'SHIPPING_ADDRESS_CITY'      , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 26, 'Accounts.LBL_STATE'                     , 'BILLING_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 27, 'Accounts.LBL_STATE'                     , 'SHIPPING_ADDRESS_STATE'     , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 28, 'Accounts.LBL_POSTAL_CODE'               , 'BILLING_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 29, 'Accounts.LBL_POSTAL_CODE'               , 'SHIPPING_ADDRESS_POSTALCODE', 0, 4,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 30, 'Accounts.LBL_COUNTRY'                   , 'BILLING_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditView'       , 31, 'Accounts.LBL_COUNTRY'                   , 'SHIPPING_ADDRESS_COUNTRY'   , 0, 4, 100, 10, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Accounts.EditView'       , 32;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditView'       , 33, 'Accounts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 5,   8, 60, 3;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Accounts.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  1, 'Phone Number'                           , 'PHONE_OFFICE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  3, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  5, 'Phone Number'                           , 'PHONE_ALTERNATE'            , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  7, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Accounts.EditView'       ,  9, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Accounts.EditView'       , 16, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Accounts.EditView'       , 16, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Accounts.EditView'       , 18, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;

	-- 08/26/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Accounts.EditView'       ,  6, 'Accounts.LBL_MEMBER_OF'                 , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME', 'Accounts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Accounts.EditView'       , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Accounts.';
		exec dbo.spEDITVIEWS_FIELDS_CnvControl     'Accounts.EditView'       , 17, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		-- 09/02/2012 Paul.  Move above Accounts.EditView so that fix would be applied before merge. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditAddress' and DATA_LABEL = 'Accounts.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Accounts.EditAddress'
			   and DATA_LABEL        = 'Accounts.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Accounts.EditView', 'Accounts.EditAddress', 'Accounts.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditDescription' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Accounts.EditView', 'Accounts.EditDescription', 'Accounts.LBL_DESCRIPTION_INFORMATION', null;
	end -- if;
end -- if;
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Accounts.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Accounts.EditAddress', 'Accounts', 'vwACCOUNTS_Edit', '15%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditAddress'    ,  0, 'Accounts.LBL_BILLING_ADDRESS_STREET'    , 'BILLING_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Accounts.EditAddress'    ,  1, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditAddress'    ,  2, 'Accounts.LBL_SHIPPING_ADDRESS_STREET'   , 'SHIPPING_ADDRESS_STREET'    , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  3, 'Accounts.LBL_CITY'                      , 'BILLING_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  4, 'Accounts.LBL_CITY'                      , 'SHIPPING_ADDRESS_CITY'      , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  5, 'Accounts.LBL_STATE'                     , 'BILLING_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  6, 'Accounts.LBL_STATE'                     , 'SHIPPING_ADDRESS_STATE'     , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  7, 'Accounts.LBL_POSTAL_CODE'               , 'BILLING_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  8, 'Accounts.LBL_POSTAL_CODE'               , 'SHIPPING_ADDRESS_POSTALCODE', 0, 4,  20, 15, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    ,  9, 'Accounts.LBL_COUNTRY'                   , 'BILLING_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Accounts.EditAddress'    , 10, 'Accounts.LBL_COUNTRY'                   , 'SHIPPING_ADDRESS_COUNTRY'   , 0, 4, 100, 10, null;
end -- if;
*/
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Accounts.EditDescription' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Accounts.EditDescription';
	exec dbo.spEDITVIEWS_InsertOnly            'Accounts.EditDescription', 'Accounts', 'vwACCOUNTS_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Accounts.EditDescription',  0, 'Accounts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end -- if;
*/
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView';
-- 07/04/2007 Paul.  The Releases list references the fields as IDs, but we need to use them as text values in the detail and grid views. 
-- 11/27/2008 Paul.  Remove created by and modified by fields. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Bugs.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Bugs.EditView'           , 'Bugs'          , 'vwBUGS_Edit'          , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Bugs.EditView'           ,  0, 'Bugs.LBL_BUG_NUMBER'                    , 'BUG_NUMBER'                 , null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'    , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  2, 'Bugs.LBL_PRIORITY'                      , 'PRIORITY'                   , 0, 1, 'bug_priority_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  4, 'Bugs.LBL_TYPE'                          , 'TYPE'                       , 0, 1, 'bug_type_dom'        , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Bugs.EditView'           ,  5, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  6, 'Bugs.LBL_SOURCE'                        , 'SOURCE'                     , 0, 1, 'source_dom'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  7, 'Bugs.LBL_STATUS'                        , 'STATUS'                     , 0, 1, 'bug_status_dom'      , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  8, 'Bugs.LBL_PRODUCT_CATEGORY'              , 'PRODUCT_CATEGORY'           , 0, 1, 'product_category_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Bugs.EditView'           ,  9, 'Bugs.LBL_RESOLUTION'                    , 'RESOLUTION'                 , 0, 2, 'bug_resolution_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 11, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           , 12, 'Bugs.LBL_FOUND_IN_RELEASE'              , 'FOUND_IN_RELEASE_ID'        , 0, 1, 'FOUND_IN_RELEASE'    , 'Releases', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           , 13, 'Bugs.LBL_FIXED_IN_RELEASE'              , 'FIXED_IN_RELEASE_ID'        , 0, 1, 'FIXED_IN_RELEASE'    , 'Releases', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 14, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsFile        'Bugs.EditView'           , 16, 'Bugs.LBL_FILENAME'                      , 'ATTACHMENT'                 , 0, 2, 255, 60, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Bugs.EditView'           , 17, 'Bugs.LBL_SUBJECT'                       , 'NAME'                       , 1, 3,   1, 70, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Bugs.EditView'           , 18, 'Bugs.LBL_DESCRIPTION'                   , 'DESCRIPTION'                , 0, 3,   8, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 19, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Bugs.EditView'           , 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Bugs.EditView'           , 21, 'Bugs.LBL_WORK_LOG'                      , 'WORK_LOG'                   , 0, 3,   2, 80, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Bugs.EditView'           ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Bugs.EditView'           ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Bugs.EditView'           ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Bugs.';
		exec dbo.spEDITVIEWS_FIELDS_CnvControl     'Bugs.EditView'           ,  5, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	end -- if;
	-- 07/04/2007 Paul.  The Releases list references the fields as IDs, but we need to use them as text values in the detail and grid views. 
	-- 08/05/2010 Paul.  Convert releases to ModulePopup. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Bugs.EditView' and DATA_FIELD in ('FOUND_IN_RELEASE_ID', 'FIXED_IN_RELEASE_ID', 'FOUND_IN_RELEASE', 'FIXED_IN_RELEASE') and FIELD_TYPE = 'ListBox' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Bugs.EditView'
		   and DATA_FIELD       in ('FOUND_IN_RELEASE_ID', 'FIXED_IN_RELEASE_ID', 'FOUND_IN_RELEASE', 'FIXED_IN_RELEASE')
		   and FIELD_TYPE        = 'ListBox'
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           , 12, 'Bugs.LBL_FOUND_IN_RELEASE'              , 'FOUND_IN_RELEASE_ID'        , 0, 1, 'FOUND_IN_RELEASE'    , 'Releases', null;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Bugs.EditView'           , 13, 'Bugs.LBL_FIXED_IN_RELEASE'              , 'FIXED_IN_RELEASE_ID'        , 0, 1, 'FIXED_IN_RELEASE'    , 'Releases', null;
	end -- if;
end -- if;
GO

-- 03/04/2009 Paul.  Fix SHOULD_REMIND to use REMINDER_TIME.
-- 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
-- 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
-- 03/22/2013 Paul.  Add Recurrence fields. 
-- 12/14/2013 Paul.  Increase size of name to 150. 
-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Calls.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Calls.EditView'         , 'Calls'         , 'vwCALLS_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          ,  0, 'Calls.LBL_NAME'                         , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          ,  1, 'Calls.LBL_STATUS'                       , 'DIRECTION'                  , 0, 2, 'call_direction_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          ,  2, null                                     , 'STATUS'                     , 0, 2, 'call_status_dom'    , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          ,  3, 'Calls.LBL_DATE_TIME'                    , 'DATE_START'                 , 1, 1, 'DateTimePicker'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Calls.EditView'          ,  4, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Calls.EditView'          ,  5, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Calls.EditView'          ,  6, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          ,  7, 'Calls.LBL_DURATION'                     , 'DURATION_HOURS'             , 1, 1,   2,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          ,  8, null                                     , 'DURATION_MINUTES'           , 1, 1, 'call_minutes_dom'   , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Calls.EditView'          ,  9, null                                     , 'Calls.LBL_HOURS_MINUTES'    , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          , 10, null                                     , 'ALL_DAY_EVENT'              , 0, 1, 'CheckBox'           , 'ToggleAllDayEvent(this);', -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Calls.EditView'          , 11, null                                     , 'Calls.LBL_ALL_DAY'          , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'Calls.EditView'          , 12, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          , 13, 'Calls.LBL_REMINDER'                     , 'SHOULD_REMIND'              , 0, 1, 'CheckBox'           , 'toggleDisplay(''REMINDER_TIME'');', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 14, null                                     , 'REMINDER_TIME'              , 0, 1, 'reminder_time_dom'  , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 15, 'Calls.LBL_EMAIL_REMINDER_TIME'          , 'EMAIL_REMINDER_TIME'        , 0, 2, 'reminder_time_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Calls.EditView'          , 16, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 17, 'Calls.LBL_SMS_REMINDER_TIME'            , 'SMS_REMINDER_TIME'          , 0, 2, 'reminder_time_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Calls.EditView'          , 18, 'Calls.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 3,   8, 60, null;
	-- 03/22/2013 Paul.  Add Recurrence fields. 
	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Calls.EditView'          , 19;
	exec dbo.spEDITVIEWS_FIELDS_InsHeader      'Calls.EditView'          , 20, 'Calendar.LBL_REPEAT_TAB', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 21, 'Calls.LBL_REPEAT_TYPE'                  , 'REPEAT_TYPE'                , 0, 4, 'repeat_type_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          , 22, 'Calendar.LBL_REPEAT_END_AFTER'          , 'REPEAT_COUNT'               , 0, 4,  25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Calls.EditView'          , 23, null                                     , 'Calendar.LBL_REPEAT_OCCURRENCES', -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          , 24, 'Calendar.LBL_REPEAT_INTERVAL'           , 'REPEAT_INTERVAL'            , 0, 4,  25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          , 25, 'Calls.LBL_REPEAT_UNTIL'                 , 'REPEAT_UNTIL'               , 0, 4, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckLst    'Calls.EditView'          , 26, 'Calls.LBL_REPEAT_DOW'                   , 'REPEAT_DOW'                 , 0, 4, 'scheduler_day_dom', '1', 3, null;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Calls.EditView'          ,  4, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Calls.EditView'          ,  4, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Calls.EditView'          ,  5, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and ONCLICK_SCRIPT like '%should_remind_list%' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Calls.EditView: Fix SHOULD_REMIND to use REMINDER_TIME.';
		update EDITVIEWS_FIELDS
		   set ONCLICK_SCRIPT    = 'toggleDisplay(''REMINDER_TIME'');'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Calls.EditView'
		   and ONCLICK_SCRIPT    like '%should_remind_list%'
		   and DELETED           = 0;
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Calls.EditView'          , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
	exec dbo.spEDITVIEWS_FIELDS_CnvBoundLst    'Calls.EditView'          , 13, 'Calls.LBL_EMAIL_REMINDER_TIME'          , 'EMAIL_REMINDER_TIME'        , 0, 2, 'reminder_time_dom'  , null, null;

	-- 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and DATA_FIELD = 'ALL_DAY_EVENT' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME    = 'Calls.EditView'
		   and FIELD_INDEX >= 10
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          , 10, null                                     , 'ALL_DAY_EVENT'              , 0, 1, 'CheckBox'           , 'ToggleAllDayEvent(this);', -1, null;
		exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Calls.EditView'          , 11, null                                     , 'Calls.LBL_ALL_DAY'          , -1;
	end -- if;
	-- 03/22/2013 Paul.  Add Recurrence fields. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and DATA_FIELD = 'REPEAT_TYPE' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Calls.EditView: Add REPEAT fields.';
		exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Calls.EditView'          , 17;
		exec dbo.spEDITVIEWS_FIELDS_InsHeader      'Calls.EditView'          , 18, 'Calendar.LBL_REPEAT_TAB', 3;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 19, 'Calls.LBL_REPEAT_TYPE'                  , 'REPEAT_TYPE'                , 0, 4, 'repeat_type_dom'    , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          , 20, 'Calendar.LBL_REPEAT_END_AFTER'          , 'REPEAT_COUNT'               , 0, 4,  25, 10, null;
		exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Calls.EditView'          , 21, null                                     , 'Calendar.LBL_REPEAT_OCCURRENCES', -1;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Calls.EditView'          , 22, 'Calendar.LBL_REPEAT_INTERVAL'           , 'REPEAT_INTERVAL'            , 0, 4,  25, 10, null;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Calls.EditView'          , 23, 'Calls.LBL_REPEAT_UNTIL'                 , 'REPEAT_UNTIL'               , 0, 4, 'DatePicker'         , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsCheckLst    'Calls.EditView'          , 24, 'Calls.LBL_REPEAT_DOW'                   , 'REPEAT_DOW'                 , 0, 4, 'scheduler_day_dom', '1', 3, null;
	end -- if;
	-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Calls.EditView' and DATA_FIELD = 'SMS_REMINDER_TIME' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Calls.EditView: Add SMS_REMINDER_TIME.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 2
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Calls.EditView'
		   and FIELD_INDEX      >= 16
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Calls.EditView'          , 16, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Calls.EditView'          , 17, 'Calls.LBL_SMS_REMINDER_TIME'            , 'SMS_REMINDER_TIME'          , 0, 2, 'reminder_time_dom'  , null, null;
	end -- if;
end -- if;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Campaigns.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Campaigns.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Campaigns.EditView'     , 'Campaigns'     , 'vwCAMPAIGNS_Edit'     , '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      ,  0, 'Campaigns.LBL_NAME'                     , 'NAME'                       , 1, 1,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Campaigns.EditView'      ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Campaigns.EditView'      ,  2, 'Campaigns.LBL_CAMPAIGN_STATUS'          , 'STATUS'                     , 1, 1, 'campaign_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Campaigns.EditView'      ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Campaigns.EditView'      ,  4, 'Campaigns.LBL_CAMPAIGN_START_DATE'      , 'START_DATE'                 , 0, 1, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      ,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Campaigns.EditView'      ,  6, 'Campaigns.LBL_CAMPAIGN_END_DATE'        , 'END_DATE'                   , 1, 1, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Campaigns.EditView'      ,  8, 'Campaigns.LBL_CAMPAIGN_TYPE'            , 'CAMPAIGN_TYPE'              , 1, 1, 'campaign_type_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      ,  9, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 11, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Campaigns.EditView'      , 12, 'Campaigns.LBL_CURRENCY'                 , 'CURRENCY_ID'                , 1, 1, 'Currencies'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 13, 'Campaigns.LBL_CAMPAIGN_IMPRESSIONS'     , 'IMPRESSIONS'                , 0, 2, 25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 14, 'Campaigns.LBL_CAMPAIGN_BUDGET'          , 'BUDGET'                     , 0, 1, 25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 15, 'Campaigns.LBL_CAMPAIGN_ACTUAL_COST'     , 'ACTUAL_COST'                , 0, 2, 25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 16, 'Campaigns.LBL_CAMPAIGN_EXPECTED_REVENUE', 'EXPECTED_REVENUE'           , 0, 1, 25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 17, 'Campaigns.LBL_CAMPAIGN_EXPECTED_COST'   , 'EXPECTED_COST'              , 0, 2, 25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 18, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 19, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Campaigns.EditView'      , 20, 'Campaigns.LBL_CAMPAIGN_OBJECTIVE'       , 'OBJECTIVE'                  , 0, 3,   8, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Campaigns.EditView'      , 21, 'Campaigns.LBL_CAMPAIGN_CONTENT'         , 'CONTENT'                    , 0, 4,   8, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 22, 'Campaigns.LBL_TRACKER_TEXT'             , 'TRACKER_TEXT'               , 0, 4, 255, 50, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 23, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 24, 'Campaigns.LBL_REFER_URL'                , 'REFER_URL'                  , 0, 4, 255, 50, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Campaigns.EditView'      , 25, null;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Campaigns.EditView'      ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Campaigns.EditView'      ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Campaigns.EditView'      ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;

	-- 09/10/2007 Paul.  Tracker information has been moved to a relationship panel. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Campaigns.EditView' and DATA_LABEL = 'Campaigns.LBL_TRACKER_TEXT' and FIELD_INDEX = 20 and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Campaigns.EditView: Remove tracking.';
		update EDITVIEWS_FIELDS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME      = 'Campaigns.EditView'
		   and DATA_LABEL       in ('Campaigns.LBL_TRACKER_URL', 'Campaigns.LBL_TRACKER_TEXT', 'Campaigns.LBL_REFER_URL', 'Campaigns.LBL_TRACKER_COUNT')
		   and DELETED          = 0;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Campaigns.EditView' and DATA_FIELD = 'IMPRESSIONS' and DELETED = 0) begin -- then
		print 'Add Impressions to Campaign.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME  = 'Campaigns.EditView'
		   and FIELD_INDEX >= 12
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Campaigns.EditView'      , 12, 'Campaigns.LBL_CURRENCY'                 , 'CURRENCY_ID'                , 1, 1, 'Currencies'          , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Campaigns.EditView'      , 13, 'Campaigns.LBL_CAMPAIGN_IMPRESSIONS'     , 'IMPRESSIONS'                , 0, 2, 25, 10, null;
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Campaigns.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Campaigns.EditView'      , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
end -- if;
GO

-- 04/02/2012 Paul.  Add TYPE and WORK_LOG. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Cases.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Cases.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Cases.EditView'          , 'Cases'         , 'vwCASES_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Cases.EditView'          ,  0, 'Cases.LBL_CASE_NUMBER'                  , 'CASE_NUMBER'                , null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Cases.EditView'          ,  1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Cases.EditView'          ,  2, 'Cases.LBL_PRIORITY'                     , 'PRIORITY'                   , 0, 1, 'case_priority_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Cases.EditView'          ,  3, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Cases.EditView'          ,  4, 'Cases.LBL_STATUS'                       , 'STATUS'                     , 0, 1, 'case_status_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Cases.EditView'          ,  5, 'Cases.LBL_ACCOUNT_NAME'                 , 'ACCOUNT_ID'                 , 1, 1, 'ACCOUNT_NAME'       , 'Accounts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Cases.EditView'          ,  6, 'Cases.LBL_TYPE'                         , 'TYPE'                       , 0, 1, 'case_type_dom'      , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Cases.EditView'          ,  7, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Cases.EditView'          ,  8, 'Cases.LBL_SUBJECT'                      , 'NAME'                       , 1, 3, 1, 70, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Cases.EditView'          ,  9, 'Cases.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 3, 8, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Cases.EditView'          , 10, 'Cases.LBL_RESOLUTION'                   , 'RESOLUTION'                 , 0, 3, 5, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Cases.EditView'          , 11, 'Cases.LBL_WORK_LOG'                     , 'WORK_LOG'                   , 0, 3, 5, 80, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Cases.EditView'          ,  1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Cases.EditView'          ,  1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Cases.EditView'          ,  3, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 08/26/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Cases.EditView'          ,  5, 'Cases.LBL_ACCOUNT_NAME'                 , 'ACCOUNT_ID'                 , 1, 1, 'ACCOUNT_NAME'       , 'Accounts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Cases.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Cases.EditView'          , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Cases.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Cases.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME  = 'Cases.EditView'
		   and FIELD_INDEX >= 6
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Cases.EditView'          ,  6, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Cases.EditView'          ,  7, null;
	end -- if;
end -- if;
GO

-- 12/18/2005 Paul.  Account is not required on Contacts.EditView. 
-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Contacts.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Contacts.EditView'      , 'Contacts'      , 'vwCONTACTS_Edit'      , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Contacts.EditView'       ,  0, 'Contacts.LBL_FIRST_NAME'                , 'SALUTATION'                 , 0, 1, 'salutation_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  1, null                                     , 'FIRST_NAME'                 , 0, 1,  25, 25, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  2, 'Contacts.LBL_OFFICE_PHONE'              , 'PHONE_WORK'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  3, 'Contacts.LBL_LAST_NAME'                 , 'LAST_NAME'                  , 1, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  4, 'Contacts.LBL_MOBILE_PHONE'              , 'PHONE_MOBILE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Contacts.EditView'       ,  5, 'Contacts.LBL_ACCOUNT_NAME'              , 'ACCOUNT_ID'                 , 0, 1, 'ACCOUNT_NAME'       , 'Accounts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  6, 'Contacts.LBL_HOME_PHONE'                , 'PHONE_HOME'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Contacts.EditView'       ,  7, 'Contacts.LBL_LEAD_SOURCE'               , 'LEAD_SOURCE'                , 0, 1, 'lead_source_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  8, 'Contacts.LBL_OTHER_PHONE'               , 'PHONE_OTHER'                , 0, 2,  25, 30, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       ,  9, 'Contacts.LBL_TITLE'                     , 'TITLE'                      , 0, 1,  40, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 10, 'Contacts.LBL_FAX_PHONE'                 , 'PHONE_FAX'                  , 0, 2,  25, 30, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 11, 'Contacts.LBL_DEPARTMENT'                , 'DEPARTMENT'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 12, 'Contacts.LBL_EMAIL_ADDRESS'             , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 12, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 13, 'Contacts.LBL_BIRTHDATE'                 , 'BIRTHDATE'                  , 0, 1, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 14, 'Contacts.LBL_OTHER_EMAIL_ADDRESS'       , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 14, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Contacts.EditView'       , 15, 'Contacts.LBL_REPORTS_TO'                , 'REPORTS_TO_ID'              , 0, 1, 'REPORTS_TO_NAME'    , 'Contacts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 16, 'Contacts.LBL_ASSISTANT'                 , 'ASSISTANT'                  , 0, 2,  75, 25, null;
	-- 02/09/2006 Paul.  SugarCRM uses the CONTACTS_USERS table to allow each user to choose the contacts they want sync'd with Outlook. 
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 17, 'Contacts.LBL_SYNC_CONTACT'              , 'SYNC_CONTACT'               , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 18, 'Contacts.LBL_ASSISTANT_PHONE'           , 'ASSISTANT_PHONE'            , 0, 2,  25, 25, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 18, 'Phone Number'                           , 'ASSISTANT_PHONE'            , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 19, 'Contacts.LBL_DO_NOT_CALL'               , 'DO_NOT_CALL'                , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 20, 'Contacts.LBL_EMAIL_OPT_OUT'             , 'EMAIL_OPT_OUT'              , 0, 2, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Contacts.EditView'       , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Contacts.EditView'       , 22, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Contacts.EditView'       , 23, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 24, 'Contacts.LBL_INVALID_EMAIL'             , 'INVALID_EMAIL'              , 0, 2, 'CheckBox'           , null, null, null;

	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Contacts.EditView'       , 25, 'Contacts.LBL_SMS_OPT_IN'                , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Contacts.EditView'       , 26, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Contacts.EditView'       , 27;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditView'       , 28, 'Contacts.LBL_PRIMARY_ADDRESS'           , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditView'       , 29, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditView'       , 30, 'Contacts.LBL_ALTERNATE_ADDRESS'         , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 31, 'Contacts.LBL_CITY'                      , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 32, 'Contacts.LBL_CITY'                      , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 33, 'Contacts.LBL_STATE'                     , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 34, 'Contacts.LBL_STATE'                     , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 35, 'Contacts.LBL_POSTAL_CODE'               , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 36, 'Contacts.LBL_POSTAL_CODE'               , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 37, 'Contacts.LBL_COUNTRY'                   , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditView'       , 38, 'Contacts.LBL_COUNTRY'                   , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Contacts.EditView'       , 39;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditView'       , 40, 'Contacts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 5,   8, 60, 3;

	-- 08/31/2010 Paul.  Update the Address information if the Account changes. 
	exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'Contacts.EditView', 'ACCOUNT_ID' , '1';
end else begin
	-- 02/09/2006 Paul.  SugarCRM introduced a Sync Contact checkbox.  For existing systems, replace the blank before Assistant Phone with Sync Contact.
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DATA_FIELD = 'SYNC_CONTACT' and DELETED = 0) begin -- then
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 17 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FIELD_TYPE       = 'CheckBox'
			     , DATA_LABEL       = 'Contacts.LBL_SYNC_CONTACT'
			     , DATA_FIELD       = 'SYNC_CONTACT'
			     , DATA_REQUIRED    = 0
			     , FORMAT_TAB_INDEX = 1
			     , DATE_MODIFIED    = getdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Contacts.EditView'
			   and FIELD_TYPE       = 'Blank'
			   and FIELD_INDEX      = 17 
			   and DELETED          = 0;
		end -- if;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Contacts.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 12, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 14, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Contacts.EditView'       , 18, 'Phone Number'                           , 'ASSISTANT_PHONE'            , '.ERR_INVALID_PHONE_NUMBER';
	end -- if;
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Contacts.EditView'       , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Contacts.EditView'       , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Contacts.EditView'       , 23, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Contacts.EditView'       ,  5, 'Contacts.LBL_ACCOUNT_NAME'              , 'ACCOUNT_ID'                 , 0, 1, 'ACCOUNT_NAME'       , 'Accounts', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Contacts.EditView'       , 15, 'Contacts.LBL_REPORTS_TO'                , 'REPORTS_TO_ID'              , 0, 1, 'REPORTS_TO_NAME'    , 'Contacts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Contacts.EditView'       , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 08/31/2010 Paul.  Update the Address information if the Account changes. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DATA_FIELD = 'ACCOUNT_ID' and DATA_FORMAT is null and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'Contacts.EditView', 'ACCOUNT_ID' , '1';
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditAddress' and DATA_LABEL = 'Contacts.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Contacts.EditAddress'
			   and DATA_LABEL        = 'Contacts.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Contacts.EditView', 'Contacts.EditAddress', 'Contacts.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditDescription' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Contacts.EditView', 'Contacts.EditDescription', 'Contacts.LBL_DESCRIPTION_INFORMATION', null;
	end -- if;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DATA_FIELD = 'SMS_OPT_IN' and DELETED = 0) begin -- then
		print 'Add SMS_OPT_IN to Contacts.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 2
		     , MODIFIED_USER_ID  = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		 where EDIT_NAME         = 'Contacts.EditView'
		   and FIELD_INDEX      >= 25
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Contacts.EditView'       , 25, 'Contacts.LBL_SMS_OPT_IN'                , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Contacts.EditView'       , 26, null;
	end -- if;
	-- 10/22/2013 Paul.  Add Twitter Screen Name. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditView' and DATA_FIELD = 'TWITTER_SCREEN_NAME' and DELETED = 0) begin -- then
		print 'Add TWITTER_SCREEN_NAME to Contacts.';
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'Contacts.EditView'       , 26, 'Contacts.LBL_TWITTER_SCREEN_NAME'       , 'TWITTER_SCREEN_NAME'        , 0, 2, 15, 15, null;
	end -- if;
end -- if;
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Contacts.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Contacts.EditAddress', 'Contacts', 'vwCONTACTS_Edit', '15%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditAddress'    ,  0, 'Contacts.LBL_PRIMARY_ADDRESS'           , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Contacts.EditAddress'    ,  1, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditAddress'    ,  2, 'Contacts.LBL_ALTERNATE_ADDRESS'         , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  3, 'Contacts.LBL_CITY'                      , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  4, 'Contacts.LBL_CITY'                      , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  5, 'Contacts.LBL_STATE'                     , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  6, 'Contacts.LBL_STATE'                     , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  7, 'Contacts.LBL_POSTAL_CODE'               , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  8, 'Contacts.LBL_POSTAL_CODE'               , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    ,  9, 'Contacts.LBL_COUNTRY'                   , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Contacts.EditAddress'    , 10, 'Contacts.LBL_COUNTRY'                   , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;
end -- if;
*/
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Contacts.EditDescription' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Contacts.EditDescription';
	exec dbo.spEDITVIEWS_InsertOnly            'Contacts.EditDescription', 'Contacts', 'vwCONTACTS_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Contacts.EditDescription',  0, 'Contacts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end -- if;
*/
GO

-- 04/02/2012 Paul.  Add ASSIGNED_USER_ID to Notes, Documents. 
-- 01/22/2013 Paul.  Add PRIMARY_MODULE so that mail merge templates can be uploaded. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Documents.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Documents.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Documents.EditView', 'Documents', 'vwDOCUMENTS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Documents.EditView'      ,  0, 'Documents.LBL_DOC_NAME'                 , 'DOCUMENT_NAME'              , 1, 1, 255, 40, 3;
	-- 03/06/2006 Paul.  Filename label should have required flag. 
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Documents.EditView'      ,  1, 'Documents.LBL_FILENAME'                 , 'FILENAME'                   , null;
	exec dbo.spEDITVIEWS_FIELDS_InsFile        'Documents.EditView'      ,  2, null                                     , 'CONTENT'                    , 1, 2, 255, 20, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Documents.EditView'      ,  3, 'Documents.LBL_DOC_VERSION'              , 'REVISION'                   , 1, 3,  25, 20, null;
	-- 05/18/2011 Paul.  We need to allow the user to upload a mail-merge template without the Word plug-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      ,  4, 'Documents.LBL_TEMPLATE_TYPE'            , 'TEMPLATE_TYPE'              , 0, 3, 'document_template_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Documents.EditView'      ,  5, 'Documents.LBL_IS_TEMPLATE'              , 'IS_TEMPLATE'                , 0, 3, 'CheckBox'      , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      ,  6, 'Documents.LBL_CATEGORY_VALUE'           , 'CATEGORY_ID'                , 0, 4, 'document_category_dom'   , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      ,  7, 'Documents.LBL_SUBCATEGORY_VALUE'        , 'SUBCATEGORY_ID'             , 0, 5, 'document_subcategory_dom', null, null;
	-- 03/04/2006 Paul.  Status is a required field in SugarCRM 3.5.1.
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      ,  8, 'Documents.LBL_DOC_STATUS'               , 'STATUS_ID'                  , 1, 6, 'document_status_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Documents.EditView'      ,  9, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'               , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Documents.EditView'      , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'        , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      , 11, 'Documents.LBL_PRIMARY_MODULE'           , 'PRIMARY_MODULE'             , 0, 1, 'Modules'                 , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Documents.EditView'      , 12, 'Documents.LBL_DOC_ACTIVE_DATE'          , 'ACTIVE_DATE'                , 1, 8, 'DatePicker'              , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Documents.EditView'      , 13, 'Documents.LBL_DOC_EXP_DATE'             , 'EXP_DATE'                   , 0, 9, 'DatePicker'              , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Documents.EditView'      , 14, 'Documents.LBL_DESCRIPTION'              , 'DESCRIPTION'                , 0,10,  10, 90, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Documents.EditView'      ,  7, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'               , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Documents.EditView'      ,  7, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'               , 'Teams', null;
	-- 01/22/2013 Paul.  Add PRIMARY_MODULE so that mail merge templates can be uploaded. 
	exec dbo.spEDITVIEWS_FIELDS_CnvBoundLst    'Documents.EditView'      , 11, 'Documents.LBL_PRIMARY_MODULE'           , 'PRIMARY_MODULE'             , 0, 1, 'Modules'                 , null, null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Documents.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Documents.EditView'      , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'               , 'Teams', null;
	end -- if;
	-- 05/18/2011 Paul.  We need to allow the user to upload a mail-merge template without the Word plug-in. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Documents.EditView' and DATA_FIELD = 'TEMPLATE_TYPE' and DELETED = 0) begin -- then
		print 'Add TEMPLATE_TYPE to Documents.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME    = 'Documents.EditView'
		   and FIELD_INDEX >= 4
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Documents.EditView'      ,  4, 'Documents.LBL_TEMPLATE_TYPE'            , 'TEMPLATE_TYPE'              , 0, 3, 'document_template_type_dom', null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Documents.EditView'      ,  5, 'Documents.LBL_IS_TEMPLATE'              , 'IS_TEMPLATE'                , 0, 3, 'CheckBox'      , null, null, null;
	end -- if;
	-- 04/02/2012 Paul.  Add ASSIGNED_USER_ID to Notes, Documents. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Documents.EditView' and DATA_FIELD = 'ASSIGNED_USER_ID' and DELETED = 0) begin -- then
		print 'Add ASSIGNED_USER_ID to Documents.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME    = 'Documents.EditView'
		   and FIELD_INDEX >= 10
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Documents.EditView'      , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'        , 'Users', null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Documents.EditView'      , 11, null;
	end -- if;
end -- if;
GO

/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Emails.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Emails.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Emails.EditView'        , 'Emails'        , 'vwEMAILS_Edit'        , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Emails.EditView'         ,  0, 'Emails.LBL_DATE_AND_TIME'               , 'DATE_START'                 , 1, 1, 'DateTimeEdit'       , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'Emails.EditView'         ,  1, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 2, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Emails.EditView'         ,  2, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Emails.EditView'         ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Emails.EditView'         ,  4, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Emails.EditView'         ,  6, 'Emails.LBL_FROM'                        , 'FROM_NAME'                  , 0, 0, 255, 40, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Emails.EditView'         ,  7, 'Emails.LBL_TO'                          , 'TO_ADDRS'                   , 0, 0,   1, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Emails.EditView'         ,  8, 'Emails.LBL_CC'                          , 'CC_ADDRS'                   , 0, 0,   1, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Emails.EditView'         ,  9, 'Emails.LBL_BCC'                         , 'BCC_ADDRS'                  , 0, 0,   1, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Emails.EditView'         , 10, 'Emails.LBL_SUBJECT'                     , 'NAME'                       , 0, 0,   1, 80, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Emails.EditView'         , 11, 'Emails.LBL_BODY'                        , 'DESCRIPTION'                , 0, 0,  20,100, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Emails.EditView'         ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Emails.EditView'         ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Emails.EditView'         ,  2, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
end -- if;
*/
GO

/*
-- 04/21/2006 Paul.  Change BODY to BODY_HTML. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailTemplates.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EmailTemplates.EditView';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailTemplates.EditView' ,  0, 'EmailTemplates.LBL_NAME'           , 'NAME'                       , 1, 1, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'EmailTemplates.EditView' ,  1, 'EmailTemplates.LBL_DESCRIPTION'    , 'DESCRIPTION'                , 0, 1,   1, 90, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'EmailTemplates.EditView' ,  2, 4;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'EmailTemplates.EditView' ,  3, 'EmailTemplates.LBL_SUBJECT'        , 'SUBJECT'                    , 0, 1,   1, 90, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'EmailTemplates.EditView' ,  4, 'EmailTemplates.LBL_BODY'           , 'BODY_HTML'                  , 0, 4,  10, 90, 3;
end -- if;
*/
GO

--delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditView'
-- 09/01/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Employees.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Employees.EditView'     , 'Employees'     , 'vwEMPLOYEES_Edit'     , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  1, 'Employees.LBL_TITLE'                    , 'TITLE'                      , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  2, 'Employees.LBL_OFFICE_PHONE'             , 'PHONE_WORK'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  3, 'Employees.LBL_DEPARTMENT'               , 'DEPARTMENT'                 , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  4, 'Employees.LBL_MOBILE_PHONE'             , 'PHONE_MOBILE'               , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Employees.EditView'      ,  5, 'Employees.LBL_REPORTS_TO'               , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'    , 'Employees', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  6, 'Employees.LBL_OTHER'                    , 'PHONE_OTHER'                , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  6, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditView'      ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  8, 'Employees.LBL_FAX'                      , 'PHONE_FAX'                  , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  8, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      ,  9, 'Employees.LBL_EMAIL'                    , 'EMAIL1'                     , 0, 5, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  9, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 10, 'Employees.LBL_HOME_PHONE'               , 'PHONE_HOME'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      , 10, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 11, 'Employees.LBL_OTHER_EMAIL'              , 'EMAIL2'                     , 0, 5, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      , 11, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditView'      , 12, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Employees.EditView'      , 13, 'Employees.LBL_MESSENGER_TYPE'           , 'MESSENGER_TYPE'             , 0, 5, 'messenger_type_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditView'      , 14, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 15, 'Employees.LBL_MESSENGER_ID'             , 'MESSENGER_ID'               , 0, 5,  25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditView'      , 16, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Employees.EditView'      , 17, 'Employees.LBL_NOTES'                    , 'DESCRIPTION'                , 0, 7,   4, 80, 3;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Employees.EditView'      , 18;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Employees.EditView'      , 19, 'Employees.LBL_PRIMARY_ADDRESS'          , 'ADDRESS_STREET'             , 0, 8,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 20, 'Employees.LBL_CITY'                     , 'ADDRESS_CITY'               , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 21, 'Employees.LBL_STATE'                    , 'ADDRESS_STATE'              , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 22, 'Employees.LBL_POSTAL_CODE'              , 'ADDRESS_POSTALCODE'         , 0, 8,  20, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditView'      , 23, 'Employees.LBL_COUNTRY'                  , 'ADDRESS_COUNTRY'            , 0, 8, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditView'      , 24, null;
end else begin
	-- 09/01/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Employees.EditView'      ,  5, 'Employees.LBL_REPORTS_TO'               , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'    , 'Employees', null;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Employees.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  6, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  8, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      ,  9, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      , 10, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Employees.EditView'      , 11, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		-- 09/02/2012 Paul.  Move above Employees.EditView so that fix would be applied before merge. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditAddress' and DATA_LABEL = 'Employees.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Employees.EditAddress'
			   and DATA_LABEL        = 'Employees.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Employees.EditView', 'Employees.EditAddress', 'Employees.LBL_ADDRESS_INFORMATION', null;
	end -- if;
end -- if;
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Employees.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Employees.EditAddress', 'Employees', 'vwEMPLOYEES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Employees.EditAddress'   ,  0, 'Employees.LBL_PRIMARY_ADDRESS'          , 'ADDRESS_STREET'             , 0, 8,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditAddress'   ,  1, 'Employees.LBL_CITY'                     , 'ADDRESS_CITY'               , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditAddress'   ,  2, 'Employees.LBL_STATE'                    , 'ADDRESS_STATE'              , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditAddress'   ,  3, 'Employees.LBL_POSTAL_CODE'              , 'ADDRESS_POSTALCODE'         , 0, 8,  20, 10, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditAddress'   ,  4, 'Employees.LBL_COUNTRY'                  , 'ADDRESS_COUNTRY'            , 0, 8, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditAddress'   ,  5, null;
end -- if;
*/
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Employees.EditStatus' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Employees.EditStatus';
	exec dbo.spEDITVIEWS_InsertOnly            'Employees.EditStatus', 'Employees', 'vwEMPLOYEES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditStatus'    ,  0, 'Employees.LBL_FIRST_NAME'               , 'FIRST_NAME'                 , 0, 1,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditStatus'    ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Employees.EditStatus'    ,  2, 'Employees.LBL_LAST_NAME'                , 'LAST_NAME'                  , 0, 1,  30, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditStatus'    ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Employees.EditStatus'    ,  4, 'Employees.LBL_EMPLOYEE_STATUS'          , 'EMPLOYEE_STATUS'            , 0, 1, 'employee_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Employees.EditStatus'    ,  5, null;
end -- if;
GO

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spEDITVIEWS_FIELDS_Defaults()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_Defaults')
/



Create Procedure dbo.spEDITVIEWS_FIELDS_Defaults()
language sql
  begin
-- #endif IBM_DB2 */

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'iFrames.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS iFrames.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'iFrames.EditView'       , 'iFrames'       , 'vwIFRAMES_Edit'       , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'iFrames.EditView'        ,  0, 'iFrames.LBL_NAME'                       , 'NAME'                       , 1, 1, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'iFrames.EditView'        ,  1, 'iFrames.LBL_STATUS'                     , 'STATUS'                     , 0, 2, 'CheckBox'      , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'iFrames.EditView'        ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'iFrames.EditView'        ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'iFrames.EditView'        ,  4, 'iFrames.LBL_URL'                        , 'URL'                        , 1, 1, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'iFrames.EditView'        ,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'iFrames.EditView'        ,  6, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'iFrames.EditView'        ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'iFrames.EditView'        ,  8, 'iFrames.LBL_PLACEMENT'                  , 'PLACEMENT'                  , 1, 2, 'DROPDOWN_PLACEMENT', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'iFrames.EditView'        ,  9, 'iFrames.LBL_TYPE'                       , 'TYPE'                       , 1, 2, 'DROPDOWN_TYPE'     , null, null;
end -- if;
--GO

-- 04/02/2012 Paul.  Add WEBSITE. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.EditView'         , 'Leads'         , 'vwLEADS_Edit'         , '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.EditView'          ,  0, 'Leads.LBL_LEAD_SOURCE'                  , 'LEAD_SOURCE'                , 0, 1, 'lead_source_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.EditView'          ,  1, 'Leads.LBL_STATUS'                       , 'STATUS'                     , 0, 2, 'lead_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditView'          ,  2, 'Leads.LBL_LEAD_SOURCE_DESCRIPTION'      , 'LEAD_SOURCE_DESCRIPTION'    , 0, 1,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditView'          ,  3, 'Leads.LBL_STATUS_DESCRIPTION'           , 'STATUS_DESCRIPTION'         , 0, 2,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          ,  4, 'Leads.LBL_REFERED_BY'                   , 'REFERED_BY'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditView'          ,  5, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Leads.EditView'          ,  6, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Leads.EditView'          ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.EditView'          ,  8, 'Leads.LBL_FIRST_NAME'                   , 'SALUTATION'                 , 0, 1, 'salutation_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          ,  9, null                                     , 'FIRST_NAME'                 , 0, 1,  25, 25, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 10, 'Leads.LBL_OFFICE_PHONE'                 , 'PHONE_WORK'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 10, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 11, 'Leads.LBL_LAST_NAME'                    , 'LAST_NAME'                  , 1, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 12, 'Leads.LBL_MOBILE_PHONE'                 , 'PHONE_MOBILE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 12, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Leads.EditView'          , 13, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 14, 'Leads.LBL_HOME_PHONE'                   , 'PHONE_HOME'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 14, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 15, 'Leads.LBL_ACCOUNT_NAME'                 , 'ACCOUNT_NAME'               , 0, 1, 150, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 16, 'Leads.LBL_OTHER_PHONE'                  , 'PHONE_OTHER'                , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 16, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 17, 'Leads.LBL_WEBSITE'                      , 'WEBSITE'                    , 0, 1, 255, 28, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 18, 'Leads.LBL_FAX_PHONE'                    , 'PHONE_FAX'                  , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 18, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 19, 'Leads.LBL_TITLE'                        , 'TITLE'                      , 0, 1,  40, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 20, 'Leads.LBL_EMAIL_ADDRESS'                , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 20, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 21, 'Leads.LBL_DEPARTMENT'                   , 'DEPARTMENT'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 22, 'Leads.LBL_OTHER_EMAIL_ADDRESS'          , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 22, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditView'          , 23, 'Leads.LBL_DO_NOT_CALL'                  , 'DO_NOT_CALL'                , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditView'          , 24, 'Leads.LBL_EMAIL_OPT_OUT'                , 'EMAIL_OPT_OUT'              , 0, 2, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.EditView'          , 25, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Leads.EditView'          , 26, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.EditView'          , 27, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditView'          , 28, 'Leads.LBL_INVALID_EMAIL'                , 'INVALID_EMAIL'              , 0, 2, 'CheckBox'           , null, null, null;

	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.EditView'          , 29, 'Leads.LBL_SMS_OPT_IN'                   , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Leads.EditView'          , 30;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditView'          , 31, 'Leads.LBL_PRIMARY_ADDRESS_STREET'       , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditView'          , 32, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditView'          , 33, 'Leads.LBL_ALT_ADDRESS_STREET'           , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 34, 'Leads.LBL_CITY'                         , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 35, 'Leads.LBL_CITY'                         , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 36, 'Leads.LBL_STATE'                        , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 37, 'Leads.LBL_STATE'                        , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 38, 'Leads.LBL_POSTAL_CODE'                  , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 39, 'Leads.LBL_POSTAL_CODE'                  , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 40, 'Leads.LBL_COUNTRY'                      , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , 41, 'Leads.LBL_COUNTRY'                      , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Leads.EditView'          , 42;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditView'          , 43, 'Leads.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 5,   8, 60, 3;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Leads.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 10, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 12, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 14, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 16, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 18, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 20, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.EditView'          , 22, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Leads.EditView'          , 25, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Leads.EditView'          , 25, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Leads.EditView'          , 27, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.EditView'          , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER.
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Leads.';
		exec dbo.spEDITVIEWS_FIELDS_CnvControl     'Leads.EditView'          ,  5, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	end -- if;
	-- 04/02/2012 Paul.  Add WEBSITE. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DATA_FIELD = 'WEBSITE' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditView'          , -1, 'Leads.LBL_WEBSITE'                      , 'WEBSITE'                    , 0, 1, 255, 28, null;
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		-- 09/02/2012 Paul.  Move above Leads.EditView so that fix would be applied before merge. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditAddress' and DATA_LABEL = 'Leads.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Leads.EditAddress'
			   and DATA_LABEL        = 'Leads.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Leads.EditView', 'Leads.EditAddress', 'Leads.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditDescription' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Leads.EditView', 'Leads.EditDescription', 'Leads.LBL_DESCRIPTION_INFORMATION', null;
	end -- if;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DATA_FIELD = 'SMS_OPT_IN' and DELETED = 0) begin -- then
		print 'Add SMS_OPT_IN to Leads.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 1
		     , MODIFIED_USER_ID  = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		 where EDIT_NAME         = 'Leads.EditView'
		   and FIELD_INDEX      >= 29
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.EditView'       , 29, 'Leads.LBL_SMS_OPT_IN'                , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
	end -- if;
	-- 10/22/2013 Paul.  Add Twitter Screen Name. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditView' and DATA_FIELD = 'TWITTER_SCREEN_NAME' and DELETED = 0) begin -- then
		print 'Add TWITTER_SCREEN_NAME to Leads.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 1
		     , MODIFIED_USER_ID  = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		 where EDIT_NAME         = 'Leads.EditView'
		   and FIELD_INDEX      >= 30
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'Leads.EditView'       , 30, 'Leads.LBL_TWITTER_SCREEN_NAME'       , 'TWITTER_SCREEN_NAME'        , 0, 2, 15, 15, null;
	end -- if;
end -- if;
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.EditAddress', 'Leads', 'vwLEADS_Edit', '15%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditAddress'       ,  0, 'Leads.LBL_PRIMARY_ADDRESS_STREET'       , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.EditAddress'       ,  1, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditAddress'       ,  2, 'Leads.LBL_ALT_ADDRESS_STREET'           , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  3, 'Leads.LBL_CITY'                         , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  4, 'Leads.LBL_CITY'                         , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  5, 'Leads.LBL_STATE'                        , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  6, 'Leads.LBL_STATE'                        , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  7, 'Leads.LBL_POSTAL_CODE'                  , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  8, 'Leads.LBL_POSTAL_CODE'                  , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       ,  9, 'Leads.LBL_COUNTRY'                      , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.EditAddress'       , 10, 'Leads.LBL_COUNTRY'                      , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;
end -- if;
*/
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.EditDescription' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.EditDescription';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.EditDescription', 'Leads', 'vwLEADS_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.EditDescription'   ,  0, 'Leads.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end -- if;
*/
GO

-- 12/06/2006 Paul.  DURATION_MINUTES was accidentally replaced with TEAM_ID.
-- Move TEAM_ID to index 3 and restore the minutes list. 
-- 03/04/2009 Paul.  Fix SHOULD_REMIND to use REMINDER_TIME.
-- 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
-- 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
-- 03/22/2013 Paul.  Add Recurrence fields. 
-- 12/14/2013 Paul.  Increase size of name to 150. 
-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Meetings.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Meetings.EditView'      , 'Meetings'      , 'vwMEETINGS_Edit'      , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       ,  0, 'Meetings.LBL_NAME'                      , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       ,  1, 'Meetings.LBL_STATUS'                    , 'STATUS'                     , 1, 2, 'meeting_status_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       ,  2, 'Meetings.LBL_LOCATION'                  , 'LOCATION'                   , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Meetings.EditView'       ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       ,  4, 'Meetings.LBL_DATE_TIME'                 , 'DATE_START'                 , 1, 1, 'DateTimePicker'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Meetings.EditView'       ,  5, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       ,  6, 'Meetings.LBL_DURATION'                  , 'DURATION_HOURS'             , 1, 1,   2,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       ,  7, null                                     , 'DURATION_MINUTES'           , 1, 1, 'meeting_minutes_dom', -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       ,  8, null                                     , 'Meetings.LBL_HOURS_MINUTES'    , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       ,  9, null                                     , 'ALL_DAY_EVENT'              , 0, 1, 'CheckBox'           , 'ToggleAllDayEvent(this);', -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       , 10, null                                     , 'Meetings.LBL_ALL_DAY'       , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'Meetings.EditView'       , 11, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       , 12, 'Meetings.LBL_REMINDER'                  , 'SHOULD_REMIND'              , 0, 1, 'CheckBox'           , 'toggleDisplay(''REMINDER_TIME'');', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 13, null                                     , 'REMINDER_TIME'              , 0, 1, 'reminder_time_dom'  , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 14, 'Meetings.LBL_EMAIL_REMINDER_TIME'       , 'EMAIL_REMINDER_TIME'        , 0, 2, 'reminder_time_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Meetings.EditView'       , 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 16, 'Meetings.LBL_SMS_REMINDER_TIME'         , 'SMS_REMINDER_TIME'          , 0, 2, 'reminder_time_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Meetings.EditView'       , 17, 'Meetings.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 3,   8, 60, null;
	-- 03/22/2013 Paul.  Add Recurrence fields. 
	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Meetings.EditView'       , 18;
	exec dbo.spEDITVIEWS_FIELDS_InsHeader      'Meetings.EditView'       , 19, 'Calendar.LBL_REPEAT_TAB', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 20, 'Meetings.LBL_REPEAT_TYPE'               , 'REPEAT_TYPE'                , 0, 4, 'repeat_type_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       , 21, 'Calendar.LBL_REPEAT_END_AFTER'          , 'REPEAT_COUNT'               , 0, 4,  25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       , 22, null                                     , 'Calendar.LBL_REPEAT_OCCURRENCES', -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       , 23, 'Calendar.LBL_REPEAT_INTERVAL'           , 'REPEAT_INTERVAL'            , 0, 4,  25, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       , 24, 'Meetings.LBL_REPEAT_UNTIL'              , 'REPEAT_UNTIL'               , 0, 4, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsCheckLst    'Meetings.EditView'       , 25, 'Meetings.LBL_REPEAT_DOW'                , 'REPEAT_DOW'                 , 0, 4, 'scheduler_day_dom', '1', 3, null;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Meetings.EditView'       ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Meetings.EditView'       ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Meetings.EditView'       ,  5, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and ONCLICK_SCRIPT like '%should_remind_list%' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Meetings.EditView: Fix SHOULD_REMIND to use REMINDER_TIME.';
		update EDITVIEWS_FIELDS
		   set ONCLICK_SCRIPT    = 'toggleDisplay(''REMINDER_TIME'');'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Meetings.EditView'
		   and ONCLICK_SCRIPT    like '%should_remind_list%'
		   and DELETED           = 0;
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Meetings.EditView'       , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 12/26/2012 Paul.  Add EMAIL_REMINDER_TIME. 
	exec dbo.spEDITVIEWS_FIELDS_CnvBoundLst    'Meetings.EditView'       , 11, 'Meetings.LBL_EMAIL_REMINDER_TIME'       , 'EMAIL_REMINDER_TIME'        , 0, 2, 'reminder_time_dom'  , null, null;

	-- 03/31/2013 Paul.  Make sure to use Meetings.LBL_HOURS_MINUTES and not Calls.LBL_HOURS_MINUTES to prevent duplicate records. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DATA_FIELD = 'Meetings.LBL_HOURS_MINUTES' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 1
		 where EDIT_NAME    = 'Meetings.EditView'
		   and FIELD_INDEX >= 8
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       ,  8, null                                     , 'Meetings.LBL_HOURS_MINUTES' , -1;
	end -- if;
	-- 03/07/2013 Paul.  Add ALL_DAY_EVENT. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DATA_FIELD = 'ALL_DAY_EVENT' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME    = 'Meetings.EditView'
		   and FIELD_INDEX >= 9
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       ,  9, null                                     , 'ALL_DAY_EVENT'              , 0, 1, 'CheckBox'           , 'ToggleAllDayEvent(this);', -1, null;
		exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       , 10, null                                     , 'Meetings.LBL_ALL_DAY'       , -1;
	end -- if;
	-- 03/22/2013 Paul.  Add Recurrence fields. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DATA_FIELD = 'REPEAT_TYPE' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Meetings.EditView: Add REPEAT fields.';
		exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Meetings.EditView'       , 16;
		exec dbo.spEDITVIEWS_FIELDS_InsHeader      'Meetings.EditView'       , 17, 'Calendar.LBL_REPEAT_TAB', 3;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 18, 'Meetings.LBL_REPEAT_TYPE'               , 'REPEAT_TYPE'                , 0, 4, 'repeat_type_dom'    , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       , 19, 'Calendar.LBL_REPEAT_END_AFTER'          , 'REPEAT_COUNT'               , 0, 4,  25, 10, null;
		exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Meetings.EditView'       , 20, null                                     , 'Calendar.LBL_REPEAT_OCCURRENCES', -1;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Meetings.EditView'       , 21, 'Calendar.LBL_REPEAT_INTERVAL'           , 'REPEAT_INTERVAL'            , 0, 4,  25, 10, null;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Meetings.EditView'       , 22, 'Meetings.LBL_REPEAT_UNTIL'              , 'REPEAT_UNTIL'               , 0, 4, 'DatePicker'         , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsCheckLst    'Meetings.EditView'       , 23, 'Meetings.LBL_REPEAT_DOW'                , 'REPEAT_DOW'                 , 0, 4, 'scheduler_day_dom', '1', 3, null;
	end -- if;
	-- 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Meetings.EditView' and DATA_FIELD = 'SMS_REMINDER_TIME' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Meetings.EditView: Add SMS_REMINDER_TIME.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 2
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Meetings.EditView'
		   and FIELD_INDEX      >= 15
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Meetings.EditView'       , 15, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Meetings.EditView'       , 16, 'Meetings.LBL_SMS_REMINDER_TIME'         , 'SMS_REMINDER_TIME'          , 0, 2, 'reminder_time_dom'  , null, null;
	end -- if;
end -- if;
GO

-- 07/11/2007 Paul.  Add TEAM_ID for team management.  
-- Not sure when SugarCRM added this, but it is needed to ensure that notes are shown when teams are required. 
-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 04/02/2012 Paul.  Add ASSIGNED_USER_ID to Notes, Documents. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Notes.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Notes.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Notes.EditView'         , 'Notes'         , 'vwNOTES_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Notes.EditView'          ,  0, 'Notes.LBL_CONTACT_NAME'                 , 'CONTACT_ID'                 , 0, 1, 'CONTACT_NAME'       , 'Contacts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'Notes.EditView'          ,  1, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 2, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Notes.EditView'          ,  2, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Notes.EditView'          ,  3, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Notes.EditView'          ,  4, 'Notes.LBL_SUBJECT'                      , 'NAME'                       , 1, 1, 255,100, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsFile        'Notes.EditView'          ,  5, 'Notes.LBL_FILENAME'                     , 'ATTACHMENT'                 , 0, 2, 255, 60, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Notes.EditView'          ,  6, null                                     , 'FILENAME'                   , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Notes.EditView'          ,  7, 'Notes.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 3,  30, 90, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Notes.EditView'          ,  2, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Notes.EditView'          ,  2, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Notes.EditView'          ,  0, 'Notes.LBL_CONTACT_NAME'                 , 'CONTACT_ID'                 , 0, 1, 'CONTACT_NAME'       , 'Contacts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Notes.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Notes.EditView'          , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 04/02/2012 Paul.  Add ASSIGNED_USER_ID to Notes, Documents. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Notes.EditView'          ,  3, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Notes.EditView' and DATA_FIELD = 'ASSIGNED_USER_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Notes.EditView'          , -1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	end -- if;
end -- if;
GO

-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 07/22/2010 Paul.  Add Campaign Popup to Opportunities. 
-- 10/06/2010 Paul.  Size of NAME field was increased to 150. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Opportunities.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Opportunities.EditView' , 'Opportunities' , 'vwOPPORTUNITIES_Edit' , '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Opportunities.EditView'  ,  0, 'Opportunities.LBL_OPPORTUNITY_NAME'     , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Opportunities.EditView'  ,  1, 'Opportunities.LBL_CURRENCY'             , 'CURRENCY_ID'                , 0, 2, 'Currencies'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  ,  2, 'Opportunities.LBL_ACCOUNT_NAME'         , 'ACCOUNT_ID'                 , 1, 1, 'ACCOUNT_NAME'        , 'Accounts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Opportunities.EditView'  ,  3, 'Opportunities.LBL_AMOUNT'               , 'AMOUNT'                     , 1, 2,  25, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Opportunities.EditView'  ,  4, 'Opportunities.LBL_TYPE'                 , 'OPPORTUNITY_TYPE'           , 0, 1, 'opportunity_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Opportunities.EditView'  ,  5, 'Opportunities.LBL_DATE_CLOSED'          , 'DATE_CLOSED'                , 1, 2, 'DatePicker'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Opportunities.EditView'  ,  6, 'Opportunities.LBL_LEAD_SOURCE'          , 'LEAD_SOURCE'                , 0, 1, 'lead_source_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Opportunities.EditView'  ,  7, 'Opportunities.LBL_NEXT_STEP'            , 'NEXT_STEP'                  , 0, 2,  25, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'           , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Opportunities.EditView'  ,  9, 'Opportunities.LBL_PROBABILITY'          , 'PROBABILITY'                , 0, 2,   3,  4, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'    , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Opportunities.EditView'  , 11, 'Opportunities.LBL_SALES_STAGE'          , 'SALES_STAGE'                , 0, 2, 'sales_stage_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Opportunities.EditView'  , 12, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  , 13, 'Opportunities.LBL_CAMPAIGN_NAME'        , 'CAMPAIGN_ID'                , 0, 2, 'CAMPAIGN_NAME'        , 'Campaigns', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Opportunities.EditView'  , 14, 'Opportunities.LBL_DESCRIPTION'          , 'DESCRIPTION'                , 0, 3,   8, 60, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Opportunities.EditView'  ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'           , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Opportunities.EditView'  ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'           , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Opportunities.EditView'  , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'    , 'Users', null;
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Opportunities.EditView'  ,  2, 'Opportunities.LBL_ACCOUNT_NAME'         , 'ACCOUNT_ID'                 , 1, 1, 'ACCOUNT_NAME'        , 'Accounts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'           , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER.
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Opportunities.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME  = 'Opportunities.EditView'
		   and FIELD_INDEX >= 12
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Opportunities.EditView'  , 12, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Opportunities.EditView'  , 13, null;
	end -- if;
	-- 07/22/2010 Paul.  Add Campaign Popup. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView' and DATA_FIELD = 'CAMPAIGN_ID' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS: Add Campaign to Opportunities.';
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Opportunities.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 13 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DELETED           = 1
			     , MODIFIED_USER_ID  = null
			     , DATE_MODIFIED     = getdate()
			     , DATE_MODIFIED_UTC = getutcdate()
			 where EDIT_NAME         = 'Opportunities.EditView'
			   and FIELD_TYPE        = 'Blank'
			   and FIELD_INDEX       = 13
			   and DELETED           = 0;
		end else begin
			update EDITVIEWS_FIELDS
			   set FIELD_INDEX       = FIELD_INDEX + 1
			     , MODIFIED_USER_ID  = null
			     , DATE_MODIFIED     = getdate()
			     , DATE_MODIFIED_UTC = getutcdate()
			 where EDIT_NAME         = 'Opportunities.EditView'
			   and FIELD_INDEX       >= 13
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Opportunities.EditView'  , 13, 'Opportunities.LBL_CAMPAIGN_NAME'        , 'CAMPAIGN_ID'                , 0, 2, 'CAMPAIGN_NAME'        , 'Campaigns', null;
	end -- if;
end -- if;
GO

-- 01/13/2010 Paul.  New Project fields in SugarCRM. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Project.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Project.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Project.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Project.EditView'       , 'Project'       , 'vwPROJECTS_Edit'      , '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Project.EditView'        ,  0, 'Project.LBL_NAME'                       , 'NAME'                       , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Project.EditView'        ,  1, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Project.EditView'        ,  2, 'Project.LBL_ESTIMATED_START_DATE'       , 'ESTIMATED_START_DATE'       , 0, 1, 'DatePicker'               , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Project.EditView'        ,  3, 'Project.LBL_ESTIMATED_END_DATE'         , 'ESTIMATED_END_DATE'         , 0, 1, 'DatePicker'               , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Project.EditView'        ,  4, 'Project.LBL_STATUS'                     , 'STATUS'                     , 0, 1, 'project_status_dom'       , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Project.EditView'        ,  5, 'Project.LBL_PRIORITY'                   , 'PRIORITY'                   , 0, 1, 'projects_priority_options', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Project.EditView'        ,  6, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'         , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Project.EditView'        ,  7, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Project.EditView'        ,  8, 'Project.LBL_DESCRIPTION'                , 'DESCRIPTION'                , 0, 3,   8, 60, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Project.EditView'        ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Project.EditView'        ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Project.EditView'        ,  2, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'         , 'Users', null;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Project.EditView' and DATA_FIELD = 'ESTIMATED_START_DATE' and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS Project.EditView: Add start date and end date';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 4
		 where EDIT_NAME  = 'Project.EditView'
		   and FIELD_INDEX >= 2
		   and DELETED      = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Project.EditView'        ,  2, 'Project.LBL_ESTIMATED_START_DATE'       , 'ESTIMATED_START_DATE'       , 0, 1, 'DatePicker'               , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Project.EditView'        ,  3, 'Project.LBL_ESTIMATED_END_DATE'         , 'ESTIMATED_END_DATE'         , 0, 1, 'DatePicker'               , null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Project.EditView'        ,  4, 'Project.LBL_STATUS'                     , 'STATUS'                     , 0, 1, 'project_status_dom'       , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Project.EditView'        ,  5, 'Project.LBL_PRIORITY'                   , 'PRIORITY'                   , 0, 1, 'projects_priority_options', null, null;
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Project.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Project.EditView'        , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                , 'Teams', null;
	end -- if;
	-- 04/03/2010 Paul.  Add EXCHANGE_FOLDER.
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Project.EditView' and DATA_FIELD = 'EXCHANGE_FOLDER' and DELETED = 0) begin -- then
		print 'Add EXCHANGE_FOLDER to Projects.';
		exec dbo.spEDITVIEWS_FIELDS_CnvControl     'Project.EditView'        ,  1, '.LBL_EXCHANGE_FOLDER'                   , 'EXCHANGE_FOLDER'            , 0, 1, 'CheckBox'           , null, null, null;
	end -- if;
end -- if;
GO

-- 01/19/2010 Paul.  We need to be able to format a Float field to prevent too many decimal places. 
-- 11/03/2011 Paul.  Change field name to match stored procedure. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ProjectTask.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ProjectTask.EditView'   , 'ProjectTask'   , 'vwPROJECT_TASKS_Edit' , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    ,  0, 'Project.LBL_NAME'                       , 'NAME'                       , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProjectTask.EditView'    ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'                , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'ProjectTask.EditView'    ,  2, 'ProjectTask.LBL_STATUS'                 , 'STATUS'                     , 0, 2, 'project_task_status_options'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProjectTask.EditView'    ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                       , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    ,  4, 'ProjectTask.LBL_TASK_NUMBER'            , 'TASK_NUMBER'                , 0, 3,   4,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProjectTask.EditView'    ,  5, 'ProjectTask.LBL_DEPENDS_ON_ID'          , 'DEPENDS_ON_ID'              , 0,13, 'DEPENDS_ON_NAME'                 , 'ProjectTask', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'ProjectTask.EditView'    ,  6, 'ProjectTask.LBL_PRIORITY'               , 'PRIORITY'                   , 0, 4, 'project_task_priority_options'   , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'ProjectTask.EditView'    ,  7, 'ProjectTask.LBL_MILESTONE_FLAG'         , 'MILESTONE_FLAG'             , 0,14, 'CheckBox'      , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    ,  8, 'ProjectTask.LBL_ORDER_NUMBER'           , 'ORDER_NUMBER'               , 0, 5,   4,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProjectTask.EditView'    ,  9, 'ProjectTask.LBL_PARENT_ID'              , 'PROJECT_ID'                 , 1,15, 'PROJECT_NAME'                    , 'Project', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    , 10, 'ProjectTask.LBL_PERCENT_COMPLETE'       , 'PERCENT_COMPLETE'           , 0, 6,   3,  4, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'ProjectTask.EditView'    , 11, 'ProjectTask.LBL_UTILIZATION'            , 'UTILIZATION'                , 0,18, 'project_task_utilization_options', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'ProjectTask.EditView'    , 12, 'ProjectTask.LBL_DATE_START'             , 'DATE_TIME_START'            , 0, 7, 'DateTimeEdit'                    , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    , 13, 'ProjectTask.LBL_ESTIMATED_EFFORT'       , 'ESTIMATED_EFFORT'           , 0,19,   4,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'ProjectTask.EditView'    , 14, 'ProjectTask.LBL_DATE_DUE'               , 'DATE_TIME_DUE'              , 0, 8, 'DateTimeEdit'                    , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProjectTask.EditView'    , 15, 'ProjectTask.LBL_ACTUAL_EFFORT'          , 'ACTUAL_EFFORT'              , 0,20,   4,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'ProjectTask.EditView'    , 16, 'ProjectTask.LBL_DESCRIPTION'            , 'DESCRIPTION'                , 0,21,   8, 60, 3;
	exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'ProjectTask.EditView', 'ESTIMATED_EFFORT', 'f1';
	exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'ProjectTask.EditView', 'ACTUAL_EFFORT'   , 'f1';
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'ProjectTask.EditView'    ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                       , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProjectTask.EditView'    ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                       , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProjectTask.EditView'    ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'                , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProjectTask.EditView'    ,  5, 'ProjectTask.LBL_DEPENDS_ON_ID'          , 'DEPENDS_ON_ID'              , 0,13, 'DEPENDS_ON_NAME'                 , 'ProjectTask', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProjectTask.EditView'    ,  9, 'ProjectTask.LBL_PARENT_ID'              , 'PROJECT_ID'                 , 1,15, 'PROJECT_NAME'                    , 'Project', null;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DATA_FIELD = 'ESTIMATED_EFFORT' and DATA_FORMAT is null and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'ProjectTask.EditView', 'ESTIMATED_EFFORT', 'f1';
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DATA_FIELD = 'ACTUAL_EFFORT' and DATA_FORMAT is null and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'ProjectTask.EditView', 'ACTUAL_EFFORT'   , 'f1';
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProjectTask.EditView'    , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'                       , 'Teams', null;
	end -- if;
	-- 11/03/2011 Paul.  Change field name to match stored procedure. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DATA_FIELD = 'DATE_DUE' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'DATE_TIME_DUE'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'ProjectTask.EditView'
		   and DATA_FIELD        = 'DATE_DUE'
		   and DELETED           = 0;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProjectTask.EditView' and DATA_FIELD = 'DATE_START' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'DATE_TIME_START'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'ProjectTask.EditView'
		   and DATA_FIELD        = 'DATE_START'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

-- 08/12/2007 Paul.  Add List Type and Domain Name to support Campaign management.
-- 01/09/2010 Paul.  A Dynamic List is one that uses SQL to build the prospect list. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'ProspectLists.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProspectLists.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ProspectLists.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ProspectLists.EditView' , 'ProspectLists' , 'vwPROSPECT_LISTS_Edit' , '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProspectLists.EditView'  ,  0, 'ProspectLists.LBL_NAME'                 , 'NAME'                       , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProspectLists.EditView'  ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'      , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'ProspectLists.EditView'  ,  2, 'ProspectLists.LBL_LIST_TYPE'            , 'LIST_TYPE'                  , 1, 1, 'prospect_list_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProspectLists.EditView'  ,  3, 'ProspectLists.LBL_DOMAIN_NAME'          , 'DOMAIN_NAME'                , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProspectLists.EditView'  ,  4, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'             , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'ProspectLists.EditView'  ,  5, 'ProspectLists.LBL_DYNAMIC_LIST'         , 'DYNAMIC_LIST'               , 0, 1, 'CheckBox'              , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'ProspectLists.EditView'  ,  6, 'ProspectLists.LBL_DESCRIPTION'          , 'DESCRIPTION'                , 0, 3,   8, 80, 3;
end else begin
	-- 08/12/2007 Paul.  Keep index at 3 as it refers to a value before List Type was added. 
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'ProspectLists.EditView'  ,  2, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'             , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProspectLists.EditView'  ,  4, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'             , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'ProspectLists.EditView'  ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'      , 'Users', null;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProspectLists.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 5 and DELETED = 0) begin -- then
		print 'EDITVIEWS_FIELDS ProspectLists.EditView: Add DYNAMIC_LIST';
		update EDITVIEWS_FIELDS
		   set DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		     , DELETED           = 1
		 where EDIT_NAME         = 'ProspectLists.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 5
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'ProspectLists.EditView'  ,  5, 'ProspectLists.LBL_DYNAMIC_LIST'         , 'DYNAMIC_LIST'               , 0, 1, 'CheckBox'              , null, null, null;
	end -- if;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProspectLists.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ProspectLists.EditView'  , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'             , 'Teams', null;
	end -- if;
end -- if;
GO

-- 08/12/2007 Paul.  Add List Type and Domain Name to support Campaign management.
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ProspectLists.EditView' and DATA_FIELD = 'LIST_TYPE' and DELETED = 0) begin -- then
	print 'Add List Type to Prospect List.';
	update EDITVIEWS_FIELDS
	   set FIELD_INDEX  = FIELD_INDEX + 2
	 where EDIT_NAME  = 'ProspectLists.EditView'
	   and FIELD_INDEX >= 2
	   and DELETED      = 0;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'ProspectLists.EditView'  ,  2, 'ProspectLists.LBL_LIST_TYPE'            , 'LIST_TYPE'                  , 1, 1, 'prospect_list_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ProspectLists.EditView'  ,  3, 'ProspectLists.LBL_DOMAIN_NAME'          , 'DOMAIN_NAME'                , 0, 1, 255, 35, null;
end -- if;
GO


/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spEDITVIEWS_FIELDS_Defaults()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_Defaults')
/

Create Procedure dbo.spEDITVIEWS_FIELDS_Defaults()
language sql
  begin
-- #endif IBM_DB2 */


-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Prospects.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.EditView'     , 'Prospects'     , 'vwPROSPECTS_Edit'     , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.EditView'      ,  0, 'Prospects.LBL_FIRST_NAME'               , 'SALUTATION'                 , 0, 1, 'salutation_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  1, null                                     , 'FIRST_NAME'                 , 0, 1,  25, 25, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  2, 'Prospects.LBL_OFFICE_PHONE'             , 'PHONE_WORK'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  3, 'Prospects.LBL_LAST_NAME'                , 'LAST_NAME'                  , 1, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  4, 'Prospects.LBL_MOBILE_PHONE'             , 'PHONE_MOBILE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      ,  5, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  6, 'Prospects.LBL_HOME_PHONE'               , 'PHONE_HOME'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  8, 'Prospects.LBL_OTHER_PHONE'              , 'PHONE_OTHER'                , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      ,  9, 'Prospects.LBL_TITLE'                    , 'TITLE'                      , 0, 1,  40, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 10, 'Prospects.LBL_FAX_PHONE'                , 'PHONE_FAX'                  , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 11, 'Prospects.LBL_DEPARTMENT'               , 'DEPARTMENT'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 12, 'Prospects.LBL_EMAIL_ADDRESS'            , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 12, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditView'      , 13, 'Prospects.LBL_BIRTHDATE'                , 'BIRTHDATE'                  , 0, 1, 'DatePicker'         , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 14, 'Prospects.LBL_OTHER_EMAIL_ADDRESS'      , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 14, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      , 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 16, 'Prospects.LBL_ASSISTANT'                , 'ASSISTANT'                  , 0, 2,  75, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      , 17, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 18, 'Prospects.LBL_ASSISTANT_PHONE'          , 'ASSISTANT_PHONE'            , 0, 2,  25, 25, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 18, 'Phone Number'                           , 'ASSISTANT_PHONE'            , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditView'      , 19, 'Prospects.LBL_DO_NOT_CALL'              , 'DO_NOT_CALL'                , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditView'      , 20, 'Prospects.LBL_EMAIL_OPT_OUT'            , 'EMAIL_OPT_OUT'              , 0, 2, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.EditView'      , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      , 22, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.EditView'      , 23, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditView'      , 24, 'Prospects.LBL_INVALID_EMAIL'            , 'INVALID_EMAIL'              , 0, 2, 'CheckBox'           , null, null, null;

	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.EditView'      , 25, 'Prospects.LBL_SMS_OPT_IN'                , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      , 26, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Prospects.EditView'      , 27;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditView'      , 28, 'Prospects.LBL_PRIMARY_ADDRESS_STREET'   , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditView'      , 29, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditView'      , 30, 'Prospects.LBL_ALT_ADDRESS_STREET'       , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 31, 'Prospects.LBL_CITY'                     , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 32, 'Prospects.LBL_CITY'                     , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 33, 'Prospects.LBL_STATE'                    , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 34, 'Prospects.LBL_STATE'                    , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 35, 'Prospects.LBL_POSTAL_CODE'              , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 36, 'Prospects.LBL_POSTAL_CODE'              , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 37, 'Prospects.LBL_COUNTRY'                  , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditView'      , 38, 'Prospects.LBL_COUNTRY'                  , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Prospects.EditView'      , 39;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditView'      , 40, 'Prospects.LBL_DESCRIPTION'              , 'DESCRIPTION'                , 0, 5,   8, 60, 3;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Prospects.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 12, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 14, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.EditView'      , 18, 'Phone Number'                           , 'ASSISTANT_PHONE'            , '.ERR_INVALID_PHONE_NUMBER';
	end -- if;
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Prospects.EditView'      , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Prospects.EditView'      , 21, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Prospects.EditView'      , 23, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.EditView'      , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		-- 09/02/2012 Paul.  Move above Prospects.EditView so that fix would be applied before merge. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditAddress' and DATA_LABEL = 'Prospects.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Prospects.EditAddress'
			   and DATA_LABEL        = 'Prospects.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Prospects.EditView', 'Prospects.EditAddress', 'Prospects.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditDescription' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Prospects.EditView', 'Prospects.EditDescription', 'Prospects.LBL_DESCRIPTION_INFORMATION', null;
	end -- if;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditView' and DATA_FIELD = 'SMS_OPT_IN' and DELETED = 0) begin -- then
		print 'Add SMS_OPT_IN to Prospects.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 2
		     , MODIFIED_USER_ID  = null
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getdate()
		 where EDIT_NAME         = 'Prospects.EditView'
		   and FIELD_INDEX      >= 25
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.EditView'      , 25, 'Prospects.LBL_SMS_OPT_IN'                , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.EditView'      , 26, null;
	end -- if;
	-- 10/22/2013 Paul.  Add Twitter Screen Name. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditView' and DATA_FIELD = 'TWITTER_SCREEN_NAME' and DELETED = 0) begin -- then
		print 'Add TWITTER_SCREEN_NAME to Prospects.';
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'Prospects.EditView'      , 26, 'Prospects.LBL_TWITTER_SCREEN_NAME'       , 'TWITTER_SCREEN_NAME'        , 0, 2, 15, 15, null;
	end -- if;
end -- if;
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Prospects.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.EditAddress', 'Prospects', 'vwPROSPECTS_Edit', '15%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditAddress'   ,  0, 'Prospects.LBL_PRIMARY_ADDRESS_STREET'   , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.EditAddress'   ,  1, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditAddress'   ,  2, 'Prospects.LBL_ALT_ADDRESS_STREET'       , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  3, 'Prospects.LBL_CITY'                     , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  4, 'Prospects.LBL_CITY'                     , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  5, 'Prospects.LBL_STATE'                    , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  6, 'Prospects.LBL_STATE'                    , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  7, 'Prospects.LBL_POSTAL_CODE'              , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  8, 'Prospects.LBL_POSTAL_CODE'              , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   ,  9, 'Prospects.LBL_COUNTRY'                  , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.EditAddress'   , 10, 'Prospects.LBL_COUNTRY'                  , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;
end -- if;
*/
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.EditDescription' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Prospects.EditDescription';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.EditDescription', 'Prospects', 'vwPROSPECTS_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.EditDescription',  0, 'Prospects.LBL_DESCRIPTION'              , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end -- if;
*/
GO

-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 11/03/2011 Paul.  Change field name to match stored procedure. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Tasks.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Tasks.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Tasks.EditView'         , 'Tasks'         , 'vwTASKS_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Tasks.EditView'          ,  0, 'Tasks.LBL_SUBJECT'                      , 'NAME'                       , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Tasks.EditView'          ,  1, 'Tasks.LBL_STATUS'                       , 'STATUS'                     , 1, 2, 'task_status_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Tasks.EditView'          ,  2, 'Tasks.LBL_DUE_DATE_AND_TIME'            , 'DATE_TIME_DUE'              , 0, 1, 'DateTimeEdit'       , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'Tasks.EditView'          ,  3, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 2, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Tasks.EditView'          ,  4, 'Tasks.LBL_START_DATE_AND_TIME'          , 'DATE_TIME_START'            , 0, 1, 'DateTimeEdit'       , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Tasks.EditView'          ,  5, 'Tasks.LBL_CONTACT'                      , 'CONTACT_ID'                 , 0, 2, 'CONTACT_NAME'       , 'Contacts', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Tasks.EditView'          ,  6, 'Tasks.LBL_PRIORITY'                     , 'PRIORITY'                   , 1, 1, 'task_priority_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Tasks.EditView'          ,  7, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Tasks.EditView'          ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Tasks.EditView'          ,  9, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Tasks.EditView'          , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Tasks.EditView'          , 11, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Tasks.EditView'          , 12, 'Tasks.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 3,   8, 60, 3;
end else begin
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Tasks.EditView'          ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Tasks.EditView'          ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Tasks.EditView'          , 10, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Tasks.EditView'          ,  5, 'Tasks.LBL_CONTACT'                      , 'CONTACT_ID'                 , 0, 2, 'CONTACT_NAME'       , 'Contacts', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Tasks.EditView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Tasks.EditView'          ,  8, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 11/03/2011 Paul.  Change field name to match stored procedure. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Tasks.EditView' and DATA_FIELD = 'DATE_DUE' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'DATE_TIME_DUE'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Tasks.EditView'
		   and DATA_FIELD        = 'DATE_DUE'
		   and DELETED           = 0;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Tasks.EditView' and DATA_FIELD = 'DATE_START' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_FIELD        = 'DATE_TIME_START'
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Tasks.EditView'
		   and DATA_FIELD        = 'DATE_START'
		   and DELETED           = 0;
	end -- if;
end -- if;                                                           
GO

--delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView'
-- 09/04/2006 Paul.  Remove from EMAIL and OTHER_EMAIL.  This data goes in the EmailOptions panel. 
-- 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditView'         , 'Users'         , 'vwUSERS_Edit'         , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          ,  1, 'Users.LBL_EMPLOYEE_STATUS'              , 'EMPLOYEE_STATUS'            , 0, 5, 'employee_status_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditView'          ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  3, 'Users.LBL_TITLE'                        , 'TITLE'                      , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  4, 'Users.LBL_OFFICE_PHONE'                 , 'PHONE_WORK'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  4, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  5, 'Users.LBL_DEPARTMENT'                   , 'DEPARTMENT'                 , 0, 5,  50, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  6, 'Users.LBL_MOBILE_PHONE'                 , 'PHONE_MOBILE'               , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  6, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Users.EditView'          ,  7, 'Contacts.LBL_REPORTS_TO'                , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'     , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  8, 'Users.LBL_OTHER'                        , 'PHONE_OTHER'                , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  9, 'Users.LBL_EXTENSION'                    , 'EXTENSION'                  , 0, 5,  25, 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 10, 'Users.LBL_FAX'                          , 'PHONE_FAX'                  , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 11, 'Users.LBL_FACEBOOK_ID'                  , 'FACEBOOK_ID'                , 0, 5,  25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 12, 'Users.LBL_HOME_PHONE'                   , 'PHONE_HOME'                 , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 12, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          , 13, 'Users.LBL_MESSENGER_TYPE'               , 'MESSENGER_TYPE'             , 0, 5, 'messenger_type_dom'  , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 14, 'Users.LBL_MESSENGER_ID'                 , 'MESSENGER_ID'               , 0, 5,  25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.EditView'          , 15, 'Users.LBL_PRIMARY_ADDRESS'              , 'ADDRESS_STREET'             , 0, 8,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 16, 'Users.LBL_CITY'                         , 'ADDRESS_CITY'               , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 17, 'Users.LBL_STATE'                        , 'ADDRESS_STATE'              , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 18, 'Users.LBL_POSTAL_CODE'                  , 'ADDRESS_POSTALCODE'         , 0, 8,  20, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 19, 'Users.LBL_COUNTRY'                      , 'ADDRESS_COUNTRY'            , 0, 8,  20, 10, null;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditView'          , 20, 'Users.LBL_SMS_OPT_IN'                   , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.EditView'          , 21, 'Users.LBL_NOTES'                        , 'DESCRIPTION'                , 0, 7,   4, 80, 3;
end else begin
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Users.EditView'          ,  7, 'Contacts.LBL_REPORTS_TO'                , 'REPORTS_TO_ID'              , 0, 5, 'REPORTS_TO_NAME'     , 'Users', null;

	-- 03/25/2011 Paul.  Create a separate field for the Facebook ID. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'FACEBOOK_ID' and DELETED = 0) begin -- then
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 11 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DELETED          = 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and FIELD_TYPE       = 'Blank'
			   and FIELD_INDEX      = 11
			   and DELETED          = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          , 11, 'Users.LBL_FACEBOOK_ID'                  , 'FACEBOOK_ID'                , 0, 5,  25, 35, null;
	end -- if;

	-- 08/24/2013 Paul.  Add EXTENSION_C in preparation for Asterisk click-to-call. 
	-- 09/20/2013 Paul.  Move EXTENSION to the main table. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'EXTENSION_C' and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DATA_FIELD       = 'EXTENSION'
			     , DATA_LABEL       = 'Users.LBL_EXTENSION'
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and DATA_FIELD       = 'EXTENSION_C'
			   and DELETED          = 0;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD = 'EXTENSION' and DELETED = 0) begin -- then
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_TYPE = 'Blank' and FIELD_INDEX = 9 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set DELETED          = 1
			     , DATE_MODIFIED    = getdate()
			     , DATE_MODIFIED_UTC= getutcdate()
			     , MODIFIED_USER_ID = null
			 where EDIT_NAME        = 'Users.EditView'
			   and FIELD_TYPE       = 'Blank'
			   and FIELD_INDEX      = 9
			   and DELETED          = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditView'          ,  9, 'Users.LBL_EXTENSION'                    , 'EXTENSION'                  , 0, 5,  25, 20, null;
	end -- if;

	-- 01/21/2008 Paul.  Some older systems still have EMAIL1 and EMAIL2 in the main. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and DATA_FIELD in ('EMAIL1', 'EMAIL2') and DELETED = 0) begin -- then
		print 'Remove EMAIL1 and EMAIL2 from Users Main panel.';
		update EDITVIEWS_FIELDS
		   set DELETED          = 1
		     , DATE_MODIFIED    = getdate()
		     , DATE_MODIFIED_UTC= getutcdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditView'
		   and DATA_FIELD       in ('EMAIL1', 'EMAIL2')
		   and DELETED          = 0;
	end -- if;
/*
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Users.EditView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  4, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  6, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditView'          , 12, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	end -- if;
*/
	-- 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditAddress' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Users.EditView', 'Users.EditAddress', 'Users.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	-- 09/27/2013 Paul.  SMS messages need to be opt-in. 
	exec dbo.spEDITVIEWS_FIELDS_CnvBoundLst    'Users.EditView'          , 20, 'Users.LBL_SMS_OPT_IN'                   , 'SMS_OPT_IN'                 , 0, 1, 'dom_sms_opt_in'     , null, null;
end -- if;
GO

/*
-- 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditAddress', 'Users', 'vwUSERS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Users.EditAddress'       ,  0, 'Users.LBL_PRIMARY_ADDRESS'              , 'ADDRESS_STREET'             , 0, 8,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditAddress'       ,  1, 'Users.LBL_CITY'                         , 'ADDRESS_CITY'               , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditAddress'       ,  2, 'Users.LBL_STATE'                        , 'ADDRESS_STATE'              , 0, 8, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditAddress'       ,  3, 'Users.LBL_POSTAL_CODE'                  , 'ADDRESS_POSTALCODE'         , 0, 8,  20, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditAddress'       ,  4, 'Users.LBL_COUNTRY'                      , 'ADDRESS_COUNTRY'            , 0, 8,  20, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditAddress'       ,  5, null;
end -- if;
*/
GO

-- 08/05/2006 Paul.  Convert MailOptions to a dynamic view so that fields can be easily removed. 
-- 08/05/2006 Paul.  SplendidCRM does not support anything other than the build-in .NET mail.
-- 01/20/2008 Paul.  Add EMAIL1 so that users can be the target of a campaign. 
-- 07/08/2010 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS.  Add MAIL_SMTPUSER and MAIL_SMTPPASS. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Users.EditMailOptions';
	exec dbo.spEDITVIEWS_InsertOnly            'Users.EditMailOptions', 'Users', 'vwUSERS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  1, 'Users.LBL_EMAIL'                        , 'EMAIL1'                     , 0, 9, 128, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  1, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  2, 'Users.LBL_OTHER_EMAIL'                  , 'EMAIL2'                     , 0, 9, 128, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  2, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
--	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  3, 'Users.LBL_MAIL_FROMNAME'                , 'MAIL_FROMNAME'              , 0, 9, 128, 25, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  4, 'Users.LBL_MAIL_FROMADDRESS'             , 'MAIL_FROMADDRESS'           , 0, 9, 128, 25, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Users.EditMailOptions'   ,  5, 'Users.LBL_MAIL_SENDTYPE'                , 'MAIL_SENDTYPE'              , 0, 9, 'notifymail_sendtype'  , null, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditMailOptions'   ,  6, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  7, 'Users.LBL_MAIL_SMTPSERVER'              , 'MAIL_SMTPSERVER'            , 0, 9,  64, 25, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  8, 'Users.LBL_MAIL_SMTPPORT'                , 'MAIL_SMTPPORT'              , 0, 9,   4,  5, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Users.EditMailOptions'   ,  9, 'Users.LBL_MAIL_SMTPAUTH_REQ'            , 'MAIL_SMTPAUTH_REQ'          , 0, 9, 'CheckBox'      , null, null, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Users.EditMailOptions'   , 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  3, 'Users.LBL_MAIL_SMTPUSER'                , 'MAIL_SMTPUSER'              , 0, 9,  64, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Users.EditMailOptions'   ,  4, 'Users.LBL_MAIL_SMTPPASS'                , 'MAIL_SMTPPASS'              , 0, 9,  64, 25, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DATA_FIELD = 'EMAIL1' and DELETED = 0) begin -- then
		print 'Add EMAIL1 to Users.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX      = FIELD_INDEX + 2
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditMailOptions'
		   and FIELD_INDEX     >= 1
		   and DELETED          = 0;
    		-- 01/20/2008 Paul.  The reply info should not be required. 
		update EDITVIEWS_FIELDS
		   set UI_REQUIRED      = 0
		     , DATA_REQUIRED    = 0
		     , DATE_MODIFIED    = getdate()
		     , MODIFIED_USER_ID = null
		 where EDIT_NAME        = 'Users.EditMailOptions'
		   and DATA_FIELD      in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS')
		   and UI_REQUIRED      = 1
		   and DELETED          = 0;

		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  1, 'Users.LBL_EMAIL'                        , 'EMAIL1'                     , 0, 9, 128, 25, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  2, 'Users.LBL_OTHER_EMAIL'                  , 'EMAIL2'                     , 0, 9, 128, 25, null;

	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Users.EditMailOptions: Update validators';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  1, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Users.EditMailOptions'   ,  2, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 07/08/2010 Paul.  Remove MAIL_FROMNAME and MAIL_FROMADDRESS.  Add MAIL_SMTPUSER and MAIL_SMTPPASS. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Users.EditMailOptions' and DATA_FIELD in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS') and DELETED = 0) begin -- then
		print 'Remove MAIL_FROMNAME and MAIL_FROMADDRESS.  Add MAIL_SMTPUSER and MAIL_SMTPPASS.';
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Users.EditMailOptions'
		   and DATA_FIELD        in ('MAIL_FROMNAME', 'MAIL_FROMADDRESS')
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'Users.EditMailOptions'   ,  3, 'EmailMan.LBL_MAIL_SMTPUSER'             , 'MAIL_SMTPUSER'              , 0, 9,  64, 25, null;
		exec dbo.spEDITVIEWS_FIELDS_InsPassword    'Users.EditMailOptions'   ,  4, 'EmailMan.LBL_MAIL_SMTPPASS'             , 'MAIL_SMTPPASS'              , 0, 9,  64, 25, null;
	end -- if;
end -- if;
GO

-- 12/10/2007 Paul.  Removed references to TestCases, TestPlans and TestRuns.


-- 08/20/2008 Paul.  Add Team and Assigned User ID. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.ConvertView';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.ConvertView', 'Leads', 'vwLEADS_Convert', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertView'       ,  0, 'Contacts.LBL_FIRST_NAME'                , 'SALUTATION'                 , 0, 1, 'salutation_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  1, null                                     , 'FIRST_NAME'                 , 0, 1,  25, 25, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  2, 'Contacts.LBL_OFFICE_PHONE'              , 'PHONE_WORK'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  3, 'Contacts.LBL_LAST_NAME'                 , 'LAST_NAME'                  , 1, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  4, 'Contacts.LBL_MOBILE_PHONE'              , 'PHONE_MOBILE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  4, 'Phone Number'                           , 'PHONE_MOBILE'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  5, 'Contacts.LBL_TITLE'                     , 'TITLE'                      , 0, 1,  40, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  6, 'Contacts.LBL_HOME_PHONE'                , 'PHONE_HOME'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  7, 'Contacts.LBL_DEPARTMENT'                , 'DEPARTMENT'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  8, 'Contacts.LBL_OTHER_PHONE'               , 'PHONE_OTHER'                , 0, 2,  25, 30, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       ,  9, 'Contacts.LBL_EMAIL_ADDRESS'             , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  9, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       , 10, 'Contacts.LBL_FAX_PHONE'                 , 'PHONE_FAX'                  , 0, 2,  25, 30, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertView'       , 11, 'Contacts.LBL_OTHER_EMAIL_ADDRESS'       , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       , 11, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.ConvertView'       , 12, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.ConvertView'       , 13, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertView'       , 14, 'Contacts.LBL_LEAD_SOURCE'               , 'LEAD_SOURCE'                , 0, 1, 'lead_source_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.ConvertView'       , 15, 'Contacts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Leads.ConvertView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  2, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  4, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  6, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  8, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       ,  9, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       , 10, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertView'       , 11, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 08/20/2008 Paul.  Add Team and Assigned User ID to an existing list. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertView' and DATA_FIELD = 'ASSIGNED_USER_ID' and DELETED = 0) begin -- then
		print 'Add Team and Assigned User ID to Leads Convert.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX  = FIELD_INDEX + 2
		 where EDIT_NAME    = 'Leads.ConvertView'
		   and FIELD_INDEX >= 12
		   and DELETED      = 0;
		-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.ConvertView'       , 12, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.ConvertView'       , 13, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	end -- if;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Leads.ConvertView'       , 13, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Leads.ConvertView'       , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
end -- if;
GO

-- 12/14/2013 Paul.  Add layout in Leads Convert. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewAccount';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewAccount' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.ConvertViewAccount';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.ConvertViewAccount', 'Accounts', 'vwACCOUNTS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewAccount'    ,  0, 'Accounts.LBL_ACCOUNT_NAME'              , 'NAME'                       , 1, 6, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewAccount'    ,  1, 'Accounts.LBL_PHONE'                     , 'PHONE_OFFICE'               , 0, 6,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Leads.ConvertViewAccount'    ,  1, 'Phone Number'                           , 'PHONE_OFFICE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewAccount'    ,  2, 'Accounts.LBL_WEBSITE'                   , 'WEBSITE'                    , 0, 6, 255, 28, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewAccount'    ,  3, 'Accounts.LBL_TYPE'                      , 'ACCOUNT_TYPE'               , 0, 6, 'account_type_dom'   , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.ConvertViewAccount'    ,  4, 'Accounts.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 6,   4, 60, 3;
end -- if;
GO

-- 12/14/2013 Paul.  Add layout in Leads Convert. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewOpportunity';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewOpportunity' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.ConvertViewOpportunity';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.ConvertViewOpportunity', 'Opportunities', 'vwOPPORTUNITIES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewOpportunity',  0, 'Opportunities.LBL_OPPORTUNITY_NAME'     , 'NAME'                       , 1, 7, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewOpportunity',  1, 'Opportunities.LBL_CURRENCY'             , 'CURRENCY_ID'                , 0, 7, 'Currencies'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewOpportunity',  2, 'Opportunities.LBL_AMOUNT'               , 'AMOUNT'                     , 1, 7,  25, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewOpportunity',  3, 'Opportunities.LBL_TYPE'                 , 'OPPORTUNITY_TYPE'           , 0, 7, 'opportunity_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.ConvertViewOpportunity',  4, 'Opportunities.LBL_DATE_CLOSED'          , 'DATE_CLOSED'                , 1, 7, 'DatePicker'          , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewOpportunity',  5, 'Opportunities.LBL_SALES_STAGE'          , 'SALES_STAGE'                , 0, 7, 'sales_stage_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.ConvertViewOpportunity',  6, 'Opportunities.LBL_DESCRIPTION'          , 'DESCRIPTION'                , 0, 7,   4, 60, 3;
end -- if;
GO

-- 12/14/2013 Paul.  Add layout in Leads Convert. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewAppointment';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewAppointment' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.ConvertViewAppointment';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.ConvertViewAppointment', 'Calls', 'vwCALLS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewAppointment',  0, 'Calls.LBL_NAME'                         , 'NAME'                       , 1, 8, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewAppointment',  1, 'Calls.LBL_STATUS'                       , 'DIRECTION'                  , 0, 8, 'call_direction_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewAppointment',  2, null                                     , 'STATUS'                     , 0, 8, 'call_status_dom'    , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.ConvertViewAppointment',  3, 'Calls.LBL_DATE_TIME'                    , 'DATE_START'                 , 1, 8, 'DateTimePicker'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewAppointment',  4, 'Calls.LBL_DURATION'                     , 'DURATION_HOURS'             , 1, 8,   2,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Leads.ConvertViewAppointment',  5, null                                     , 'DURATION_MINUTES'           , 1, 8, 'call_minutes_dom'   , -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Leads.ConvertViewAppointment',  6, null                                     , 'Calls.LBL_HOURS_MINUTES'    , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Leads.ConvertViewAppointment',  7, null                                     , 'ALL_DAY_EVENT'              , 0, 8, 'CheckBox'           , 'ToggleAllDayEvent(this);', -1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Leads.ConvertViewAppointment',  8, null                                     , 'Calls.LBL_ALL_DAY'          , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.ConvertViewAppointment',  9, 'Calls.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 8,   4, 60, null;
end -- if;
GO

-- 12/14/2013 Paul.  Add layout in Leads Convert. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewNote';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Leads.ConvertViewNote' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.ConvertViewNote';
	exec dbo.spEDITVIEWS_InsertOnly            'Leads.ConvertViewNote', 'Notes', 'vwNOTES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Leads.ConvertViewNote'       ,  0, 'Notes.LBL_NOTE_SUBJECT'                 , 'NAME'                       , 1, 9, 255, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Leads.ConvertViewNote'       ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Leads.ConvertViewNote'       ,  2, 'Notes.LBL_NOTE'                         , 'DESCRIPTION'                , 0, 9,   4, 60, 3;
end -- if;
GO

-- 04/24/2006 Paul.  SugarCRM 4.0 allows Prospects to be converted to Leads.
-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.ConvertView', 'Prospects', 'vwPROSPECTS_Convert', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.ConvertView'   ,  0, 'Leads.LBL_TARGET_OF_CAMPAIGNS'          , 'CAMPAIGN_ID'                , 0, 1, 'CAMPAIGN_NAME'      , 'Campaigns', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   ,  2, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.ConvertView'   ,  3, 'Leads.LBL_LEAD_SOURCE'                  , 'LEAD_SOURCE'                , 0, 1, 'lead_source_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.ConvertView'   ,  4, 'Leads.LBL_STATUS'                       , 'STATUS'                     , 0, 2, 'lead_status_dom'    , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertView'   ,  5, 'Leads.LBL_LEAD_SOURCE_DESCRIPTION'      , 'LEAD_SOURCE_DESCRIPTION'    , 0, 1,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertView'   ,  6, 'Leads.LBL_STATUS_DESCRIPTION'           , 'STATUS_DESCRIPTION'         , 0, 2,   3, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   ,  7, 'Leads.LBL_REFERED_BY'                   , 'REFERED_BY'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   ,  8, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   ,  9, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   , 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Prospects.ConvertView'   , 11, 'Leads.LBL_FIRST_NAME'                   , 'SALUTATION'                 , 0, 1, 'salutation_dom'     , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 12, null                                     , 'FIRST_NAME'                 , 0, 1,  25, 25, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 13, 'Leads.LBL_OFFICE_PHONE'                 , 'PHONE_WORK'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 13, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 14, 'Leads.LBL_LAST_NAME'                    , 'LAST_NAME'                  , 1, 1,  25, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 15, 'Leads.LBL_MOBILE_PHONE'                 , 'PHONE_MOBILE'               , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 15, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   , 16, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 17, 'Leads.LBL_HOME_PHONE'                   , 'PHONE_HOME'                 , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 17, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 18, 'Leads.LBL_ACCOUNT_NAME'                 , 'ACCOUNT_NAME'               , 0, 1, 150, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 19, 'Leads.LBL_OTHER_PHONE'                  , 'PHONE_OTHER'                , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 19, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   , 20, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 21, 'Leads.LBL_FAX_PHONE'                    , 'PHONE_FAX'                  , 0, 2,  25, 20, null;
--	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 21, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 22, 'Leads.LBL_TITLE'                        , 'TITLE'                      , 0, 1,  40, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 23, 'Leads.LBL_EMAIL_ADDRESS'                , 'EMAIL1'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 23, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 24, 'Leads.LBL_DEPARTMENT'                   , 'DEPARTMENT'                 , 0, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 25, 'Leads.LBL_OTHER_EMAIL_ADDRESS'          , 'EMAIL2'                     , 0, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 25, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.ConvertView'   , 26, 'Leads.LBL_DO_NOT_CALL'                  , 'DO_NOT_CALL'                , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.ConvertView'   , 27, 'Leads.LBL_EMAIL_OPT_OUT'                , 'EMAIL_OPT_OUT'              , 0, 2, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.ConvertView'   , 28, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Prospects.ConvertView'   , 29, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.ConvertView'   , 30, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.ConvertView'   , 31, 'Leads.LBL_INVALID_EMAIL'                , 'INVALID_EMAIL'              , 0, 2, 'CheckBox'           , null, null, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Prospects.ConvertView'   , 32;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertView'   , 33, 'Leads.LBL_PRIMARY_ADDRESS_STREET'       , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.ConvertView'   , 34, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertView'   , 35, 'Leads.LBL_ALT_ADDRESS_STREET'           , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 36, 'Leads.LBL_CITY'                         , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 37, 'Leads.LBL_CITY'                         , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 38, 'Leads.LBL_STATE'                        , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 39, 'Leads.LBL_STATE'                        , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 40, 'Leads.LBL_POSTAL_CODE'                  , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 41, 'Leads.LBL_POSTAL_CODE'                  , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 42, 'Leads.LBL_COUNTRY'                      , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertView'   , 43, 'Leads.LBL_COUNTRY'                      , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'Prospects.ConvertView'   , 44;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertView'   , 45, 'Leads.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 5,   8, 60, 3;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertView' and FIELD_VALIDATOR_ID is not null and DELETED = 0) begin -- then
		print 'Prospects.ConvertView: Update validators';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 13, 'Phone Number'                           , 'PHONE_WORK'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 15, 'Phone Number'                           , 'PHONE_MOBILE'               , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 17, 'Phone Number'                           , 'PHONE_HOME'                 , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 19, 'Phone Number'                           , 'PHONE_OTHER'                , '.ERR_INVALID_PHONE_NUMBER';
--		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 21, 'Phone Number'                           , 'PHONE_FAX'                  , '.ERR_INVALID_PHONE_NUMBER';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 23, 'Email Address'                          , 'EMAIL1'                     , '.ERR_INVALID_EMAIL_ADDRESS';
		exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Prospects.ConvertView'   , 25, 'Email Address'                          , 'EMAIL2'                     , '.ERR_INVALID_EMAIL_ADDRESS';
	end -- if;
	-- 08/24/2009 Paul.  Keep the old conversion and let the field be fixed during the TEAMS Update. 
	exec dbo.spEDITVIEWS_FIELDS_CnvChange      'Prospects.ConvertView'   , 28, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , null, null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Prospects.ConvertView'   , 28, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Prospects.ConvertView'   , 30, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'Prospects.ConvertView'   ,  0, 'Leads.LBL_TARGET_OF_CAMPAIGNS'          , 'CAMPAIGN_ID'                , 0, 1, 'CAMPAIGN_NAME'      , 'Campaigns', null;
	-- 02/24/2010 Paul.  When upgrading from and old version, the Team ID will not exist. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertView' and DATA_FIELD = 'TEAM_ID' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'Prospects.ConvertView'   , -1, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertAddress' and DELETED = 0) begin -- then
		-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
		-- 09/02/2012 Paul.  Move above Prospects.EditView so that fix would be applied before merge. 
		if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertAddress' and DATA_LABEL = 'Prospects.LBL_COUNTRY' and FORMAT_MAX_LENGTH = 20 and DELETED = 0) begin -- then
			update EDITVIEWS_FIELDS
			   set FORMAT_MAX_LENGTH = 100
			     , DATE_MODIFIED     = getdate()
			     , MODIFIED_USER_ID  = null
			 where EDIT_NAME         = 'Prospects.ConvertAddress'
			   and DATA_LABEL        = 'Prospects.LBL_COUNTRY'
			   and FORMAT_MAX_LENGTH = 20
			   and DELETED           = 0;
		end -- if;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Prospects.ConvertView', 'Prospects.ConvertAddress', 'Leads.LBL_ADDRESS_INFORMATION', null;
	end -- if;
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertDescription' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'Prospects.ConvertView', 'Prospects.ConvertDescription', 'Leads.LBL_DESCRIPTION_INFORMATION', null;
	end -- if;
end -- if;
--GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertAddress' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Leads.EditAddress';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.ConvertAddress', 'Prospects', 'vwPROSPECTS_Convert', '15%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertAddress',  0, 'Leads.LBL_PRIMARY_ADDRESS_STREET'       , 'PRIMARY_ADDRESS_STREET'     , 0, 3,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Prospects.ConvertAddress',  1, null                                     , null                         , 0, null, 'AddressButtons', null, null, 5;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertAddress',  2, 'Leads.LBL_ALT_ADDRESS_STREET'           , 'ALT_ADDRESS_STREET'         , 0, 4,   2, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  3, 'Leads.LBL_CITY'                         , 'PRIMARY_ADDRESS_CITY'       , 0, 3, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  4, 'Leads.LBL_CITY'                         , 'ALT_ADDRESS_CITY'           , 0, 4, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  5, 'Leads.LBL_STATE'                        , 'PRIMARY_ADDRESS_STATE'      , 0, 3, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  6, 'Leads.LBL_STATE'                        , 'ALT_ADDRESS_STATE'          , 0, 4, 100, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  7, 'Leads.LBL_POSTAL_CODE'                  , 'PRIMARY_ADDRESS_POSTALCODE' , 0, 3,  20, 15, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  8, 'Leads.LBL_POSTAL_CODE'                  , 'ALT_ADDRESS_POSTALCODE'     , 0, 4,  20, 15, null;
	-- 06/07/2006 Paul.  Fix max length of country. It should be 100. 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress',  9, 'Leads.LBL_COUNTRY'                      , 'PRIMARY_ADDRESS_COUNTRY'    , 0, 3, 100, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Prospects.ConvertAddress', 10, 'Leads.LBL_COUNTRY'                      , 'ALT_ADDRESS_COUNTRY'        , 0, 4, 100, 10, null;
end -- if;
*/
GO

-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Prospects.ConvertDescription' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Prospects.ConvertDescription';
	exec dbo.spEDITVIEWS_InsertOnly            'Prospects.ConvertDescription', 'Prospects', 'vwPROSPECTS_Convert', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'Prospects.ConvertDescription',  0, 'Leads.LBL_DESCRIPTION'                  , 'DESCRIPTION'                , 0, 5,   8, 60, null;
end -- if;
*/
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ACLRoles.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ACLRoles.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ACLRoles.EditView', 'ACLRoles', 'vwACL_ROLES_Edit', '15%', '85%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ACLRoles.EditView'       ,  0, 'ACLRoles.LBL_NAME'                      , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'ACLRoles.EditView'       ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'ACLRoles.EditView'       ,  2, 'ACLRoles.LBL_DESCRIPTION'               , 'DESCRIPTION'                , 0, 2,   8, 60, null;
end -- if;
GO

-- 07/08/2007 Paul.  Add CampaignTrackers module. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'CampaignTrackers.EditView'
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'CampaignTrackers.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS CampaignTrackers.EditView';
	-- 09/02/2012 Paul.  Should be EditView and not DetailView. 
	exec dbo.spEDITVIEWS_InsertOnly            'CampaignTrackers.EditView', 'CampaignTrackers', 'vwCAMPAIGN_TRKRS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'CampaignTrackers.EditView',  0, 'CampaignTrackers.LBL_EDIT_CAMPAIGN_NAME'   , 'CAMPAIGN_NAME'             , 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CampaignTrackers.EditView',  1, 'CampaignTrackers.LBL_EDIT_TRACKER_NAME'    , 'TRACKER_NAME'              , 1, 2,  30, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'CampaignTrackers.EditView',  2, 'CampaignTrackers.LBL_EDIT_OPT_OUT'         , 'IS_OPTOUT'                 , 0, 1, 'CheckBox', 'IS_OPTOUT_Clicked();', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CampaignTrackers.EditView',  3, 'CampaignTrackers.LBL_EDIT_TRACKER_URL'     , 'TRACKER_URL'               , 1, 2, 255, 90, 3;
end -- if;
GO

-- 07/08/2007 Paul.  Add EmailMarketing module. 
-- 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailMarketing.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailMarketing.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EmailMarketing.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'EmailMarketing.EditView', 'EmailMarketing', 'vwEMAIL_MARKETING_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailMarketing.EditView'  ,  0, 'EmailMarketing.LBL_NAME'                   , 'NAME'                      , 1, 1, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EmailMarketing.EditView'  ,  1, 'EmailMarketing.LBL_STATUS_TEXT'            , 'STATUS'                    , 1, 2, 'email_marketing_status_dom'      , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EmailMarketing.EditView'  ,  2, 'EmailMarketing.LBL_FROM_MAILBOX_NAME'      , 'INBOUND_EMAIL_ID'          , 1, 1, 'InboundEmailBounce', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailMarketing.EditView'  ,  3, 'EmailMarketing.LBL_FROM_NAME'              , 'FROM_NAME'                 , 1, 2, 100, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'EmailMarketing.EditView'  ,  4, 'EmailMarketing.LBL_START_DATE_TIME'        , 'DATE_START'                , 1, 1, 'DateTimeEdit' , null, null, null;
	-- 08/29/2009 Paul.  Don't convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'EmailMarketing.EditView'  ,  5, 'EmailMarketing.LBL_TEMPLATE'               , 'TEMPLATE_ID'               , 1, 2, 'TEMPLATE_NAME', 'return EmailTemplatePopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'EmailMarketing.EditView'  ,  6, 'EmailMarketing.LBL_ALL_PROSPECT_LISTS'     , 'ALL_PROSPECT_LISTS'        , 0, 1, 'CheckBox'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailMarketing.EditView'  ,  7, 'EmailMarketing.LBL_REPLY_TO_NAME'          , 'REPLY_TO_NAME'             , 0, 2, 100, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'EmailMarketing.EditView'  ,  8, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailMarketing.EditView'  ,  9, 'EmailMarketing.LBL_REPLY_TO_ADDR'          , 'REPLY_TO_ADDR'             , 0, 2, 100, 30, null;
end else begin
	-- 12/25/2007 Paul.  Convert FROM_ADDR to INBOUND_EMAIL_ID listbox. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailMarketing.EditView' and DATA_FIELD = 'FROM_ADDR' and FIELD_INDEX = 2 and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED     = 1
		 where EDIT_NAME   = 'EmailMarketing.EditView'
		   and DATA_FIELD  = 'FROM_ADDR'
		   and FIELD_INDEX = 2
		   and DELETED     = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'EmailMarketing.EditView'  ,  2, 'EmailMarketing.LBL_FROM_MAILBOX_NAME'      , 'INBOUND_EMAIL_ID'          , 1, 1, 'InboundEmailBounce', null, null;
	end -- if;
	-- 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailMarketing.EditView' and DATA_FIELD = 'REPLY_TO_NAME' and FIELD_INDEX = 2 and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'EmailMarketing.EditView'  ,  7, 'EmailMarketing.LBL_REPLY_TO_NAME'          , 'REPLY_TO_NAME'             , 0, 2, 100, 30, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'EmailMarketing.EditView'  ,  8, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailMarketing.EditView'  ,  9, 'EmailMarketing.LBL_REPLY_TO_ADDR'          , 'REPLY_TO_ADDR'             , 0, 2, 100, 30, null;
	end -- if;
end -- if;
GO

-- 08/28/2012 Paul.  Add Call Marketing. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'CallMarketing.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'CallMarketing.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS CallMarketing.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'CallMarketing.EditView'   , 'CallMarketing', 'vwCALL_MARKETING_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CallMarketing.EditView'   ,  0, 'CallMarketing.LBL_NAME'                   , 'NAME'                           , 1, 1, 255, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'CallMarketing.EditView'   ,  1, 'CallMarketing.LBL_STATUS'                 , 'STATUS'                         , 1, 2, 'call_marketing_status_dom'       , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CallMarketing.EditView'   ,  2, 'CallMarketing.LBL_SUBJECT'                , 'SUBJECT'                        , 1, 1, 50, 30, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'CallMarketing.EditView'   ,  3, 'CallMarketing.LBL_DISTRIBUTION'           , 'DISTRIBUTION'                   , 1, 2, 'call_distribution_dom'           , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'CallMarketing.EditView'   ,  4, '.LBL_ASSIGNED_TO'                         , 'ASSIGNED_USER_ID'               , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'CallMarketing.EditView'   ,  5, 'Teams.LBL_TEAM'                           , 'TEAM_ID'                        , 0, 2, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'CallMarketing.EditView'   ,  6, 'CallMarketing.LBL_DATE_START'             , 'DATE_START'                     , 1, 1, 'DateTimeEdit' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CallMarketing.EditView'   ,  7, 'CallMarketing.LBL_DURATION'               , 'DURATION_HOURS'                 , 1, 2,   2,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'CallMarketing.EditView'   ,  8, null                                       , 'DURATION_MINUTES'               , 1, 2,   2,  2, -1;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'CallMarketing.EditView'   ,  9, null                                       , 'Calls.LBL_HOURS_MINUTES', -1;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'CallMarketing.EditView'   , 10, 'CallMarketing.LBL_DATE_END'               , 'DATE_END'                       , 0, 1, 'DateTimeEdit' , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'CallMarketing.EditView'   , 11, 'CallMarketing.LBL_ALL_PROSPECT_LISTS'     , 'ALL_PROSPECT_LISTS'             , 0, 2, 'CheckBox'     , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'CallMarketing.EditView'   , 12, 'Calls.LBL_DESCRIPTION'                    , 'DESCRIPTION'                    , 0, 3,   8, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_UpdateDataFormat null, 'CallMarketing.EditView', 'TEAM_ID', '1';
end -- if;
GO

-- 12/29/2007 Paul.  spEDITVIEWS_FIELDS_Update does not work well in Oracle inside the Data.sql, so create a separate password procedure. 
-- 07/19/2010 Paul.  Now that we allow IMAP, we need to allow the Mailbox Folder. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
-- 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
-- delete from EDITVIEWS where NAME = 'InboundEmail.EditView';
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS InboundEmail.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'InboundEmail.EditView'    , 'InboundEmail', 'vwINBOUND_EMAILS_Edit', '25%', '25%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    ,  0, 'InboundEmail.LBL_NAME'                     , 'NAME'                      , 1, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditView'    ,  1, 'InboundEmail.LBL_STATUS'                   , 'STATUS'                    , 1, 2, 'user_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    ,  2, 'InboundEmail.LBL_SERVER_URL'               , 'SERVER_URL'                , 1, 1, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    ,  3, 'InboundEmail.LBL_LOGIN'                    , 'EMAIL_USER'                , 1, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditView'    ,  4, 'InboundEmail.LBL_SERVER_TYPE'              , 'SERVICE'                   , 0, 1, 'dom_email_server_type', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'InboundEmail.EditView'    ,  5, 'InboundEmail.LBL_PASSWORD'                 , 'EMAIL_PASSWORD'            , 1, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    ,  6, 'InboundEmail.LBL_PORT'                     , 'PORT'                      , 1, 1,  10, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'InboundEmail.EditView'    ,  7, 'InboundEmail.LBL_MAILBOX_SSL'              , 'MAILBOX_SSL'               , 0, 2, 'CheckBox', 'toggleUseSSL();', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'InboundEmail.EditView'    ,  8, 'InboundEmail.LBL_MARK_READ'                , 'MARK_READ'                 , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'InboundEmail.EditView'    ,  9, 'InboundEmail.LBL_ONLY_SINCE'               , 'ONLY_SINCE'                , 0, 2, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 10, 'InboundEmail.LBL_MAILBOX'                  , 'MAILBOX'                   , 1, 1, 50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'InboundEmail.EditView'    , 11, null;

	exec dbo.spEDITVIEWS_FIELDS_InsSeparator   'InboundEmail.EditView'    , 12;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditView'    , 13, 'InboundEmail.LBL_MAILBOX_TYPE'             , 'MAILBOX_TYPE'              , 1, 3, 'dom_mailbox_type', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'InboundEmail.EditView'    , 14, 'InboundEmail.LBL_AUTOREPLY'                , 'TEMPLATE_ID'               , 0, 4, 'TEMPLATE_NAME', 'EmailTemplates', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditView'    , 15, 'InboundEmail.LBL_GROUP_QUEUE'              , 'GROUP_ID'                  , 1, 3, 'EmailGroups', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 16, 'InboundEmail.LBL_FROM_NAME'                , 'FROM_NAME'                 , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 17, 'InboundEmail.LBL_REPLY_TO_NAME'            , 'REPLY_TO_NAME'             , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 18, 'InboundEmail.LBL_FROM_ADDR'                , 'FROM_ADDR'                 , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 19, 'InboundEmail.LBL_REPLY_TO_ADDR'            , 'REPLY_TO_ADDR'             , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 20, 'InboundEmail.LBL_FILTER_DOMAIN'            , 'FILTER_DOMAIN'             , 0, 4, 100, 35, null;
end else begin
	-- 01/19/2008 Paul.  The service is required, and we will limit to POP3. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditView' and DATA_FIELD = 'SERVICE' and UI_REQUIRED = 0 and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DATA_REQUIRED     = 1
		     , UI_REQUIRED       = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'InboundEmail.EditView'
		   and DATA_FIELD        = 'SERVICE'
		   and UI_REQUIRED       = 0
		   and DELETED           = 0;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditView' and DATA_FIELD = 'MAILBOX' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditView'    , 10, 'InboundEmail.LBL_MAILBOX'                  , 'MAILBOX'                   , 1, 1, 50, 35, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'InboundEmail.EditView'    , 11, null;
	end -- if;
	-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
	if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditOptions' and DELETED = 0) begin -- then
		-- 09/02/2012 Paul.  Previous fix must still be applied. 
		exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'InboundEmail.EditOptions' ,  1, 'InboundEmail.LBL_AUTOREPLY'                , 'TEMPLATE_ID'               , 0, 4, 'TEMPLATE_NAME', 'EmailTemplates', null;
		exec dbo.spEDITVIEWS_FIELDS_MergeView      'InboundEmail.EditView', 'InboundEmail.EditOptions', 'InboundEmail.LBL_EMAIL_OPTIONS', null;
	end -- if;
	-- 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditView' and DATA_FIELD = 'REPLY_TO_NAME' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'InboundEmail.EditView'    , 17, 'InboundEmail.LBL_REPLY_TO_NAME'            , 'REPLY_TO_NAME'             , 0, 4, 100, 35, null;
		exec dbo.spEDITVIEWS_FIELDS_CnvBound       'InboundEmail.EditView'    , 19, 'InboundEmail.LBL_REPLY_TO_ADDR'            , 'REPLY_TO_ADDR'             , 0, 4, 100, 35, null;
	end -- if;
end -- if;
GO

-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
-- 09/02/2012 Paul.  Merge layout so that there is only one table to render in the HTML5 Client. 
/*
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditOptions';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.EditOptions' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS InboundEmail.EditOptions';
	exec dbo.spEDITVIEWS_InsertOnly            'InboundEmail.EditOptions' , 'InboundEmail', 'vwINBOUND_EMAILS_Edit', '25%', '25%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditOptions' ,  0, 'InboundEmail.LBL_MAILBOX_TYPE'             , 'MAILBOX_TYPE'              , 1, 3, 'dom_mailbox_type', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'InboundEmail.EditOptions' ,  1, 'InboundEmail.LBL_AUTOREPLY'                , 'TEMPLATE_ID'               , 0, 4, 'TEMPLATE_NAME', 'EmailTemplates', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'InboundEmail.EditOptions' ,  2, 'InboundEmail.LBL_GROUP_QUEUE'              , 'GROUP_ID'                  , 1, 3, 'EmailGroups', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditOptions' ,  3, 'InboundEmail.LBL_FROM_NAME'                , 'FROM_NAME'                 , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'InboundEmail.EditOptions' ,  4, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditOptions' ,  5, 'InboundEmail.LBL_FROM_ADDR'                , 'FROM_ADDR'                 , 0, 4, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'InboundEmail.EditOptions' ,  6, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'InboundEmail.EditOptions' ,  7, 'InboundEmail.LBL_FILTER_DOMAIN'            , 'FILTER_DOMAIN'             , 0, 4, 100, 35, null;
end else begin
	-- 08/27/2009 Paul.  Convert the ChangeButton to a ModulePopup. 
	exec dbo.spEDITVIEWS_FIELDS_CnvModulePopup 'InboundEmail.EditOptions' ,  1, 'InboundEmail.LBL_AUTOREPLY'                , 'TEMPLATE_ID'               , 0, 4, 'TEMPLATE_NAME', 'EmailTemplates', null;
end -- if;
*/
GO

-- 07/16/2010 Paul.  Separate layout for Imap settings.
-- 01/24/2013 Paul.  Change view name to EmailClient.SettingsView. 
if exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'InboundEmail.SettingsView.Imap' and DELETED = 0) begin -- then
	update EDITVIEWS_FIELDS
	   set EDIT_NAME         = 'EmailClient.SettingsView'
	     , MODIFIED_USER_ID  = null
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	 where EDIT_NAME         = 'InboundEmail.SettingsView.Imap'
	   and DELETED           = 0;

	update EDITVIEWS
	   set NAME              = 'EmailClient.SettingsView'
	     , MODULE_NAME       = 'EmailClient'
	     , MODIFIED_USER_ID  = null
	     , DATE_MODIFIED     = getdate()
	     , DATE_MODIFIED_UTC = getutcdate()
	 where NAME              = 'InboundEmail.SettingsView.Imap'
	   and DELETED           = 0;
end -- if;
GO


-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailClient.SettingsView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailClient.SettingsView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EmailClient.SettingsView';
	exec dbo.spEDITVIEWS_InsertOnly            'EmailClient.SettingsView', 'EmailClient', 'vwINBOUND_EMAILS_Edit', '25%', '25%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailClient.SettingsView',  0, 'InboundEmail.LBL_NAME'                     , 'NAME'                      , 1, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailClient.SettingsView',  1, 'InboundEmail.LBL_MAILBOX'                  , 'MAILBOX'                   , 1, 2,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailClient.SettingsView',  2, 'InboundEmail.LBL_SERVER_URL'               , 'SERVER_URL'                , 1, 1, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailClient.SettingsView',  3, 'InboundEmail.LBL_LOGIN'                    , 'EMAIL_USER'                , 1, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'EmailClient.SettingsView',  4, 'InboundEmail.LBL_SERVER_TYPE'              , 'SERVICE'                   , 0, 1, 'Label'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsPassword    'EmailClient.SettingsView',  5, 'InboundEmail.LBL_PASSWORD'                 , 'EMAIL_PASSWORD'            , 1, 2, 100, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'EmailClient.SettingsView',  6, 'InboundEmail.LBL_PORT'                     , 'PORT'                      , 1, 1,  10, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'EmailClient.SettingsView',  7, 'InboundEmail.LBL_MAILBOX_SSL'              , 'MAILBOX_SSL'               , 0, 2, 'CheckBox', 'toggleUseSSL();', null, null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView';
-- 07/28/2010 Paul.  We need a flag to exclude a button from a mobile device. 
-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DELETED = 0) begin -- then 
	print 'EDITVIEWS_FIELDS DynamicButtons.EditView'; 
	exec dbo.spEDITVIEWS_InsertOnly 'DynamicButtons.EditView', 'DynamicButtons', 'vwDYNAMIC_BUTTONS_Edit', '15%', '35%', null; 
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView',  0, 'DynamicButtons.LBL_VIEW_NAME'         , 'VIEW_NAME'          , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'DynamicButtons.EditView',  1, 'DynamicButtons.LBL_CONTROL_INDEX'     , 'CONTROL_INDEX'      , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  2, 'DynamicButtons.LBL_CONTROL_TYPE'      , 'CONTROL_TYPE'       , 1, 1, 'dynamic_button_type_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  3, 'DynamicButtons.LBL_MODULE_NAME'       , 'MODULE_NAME'        , 0, 1, 'Modules'                , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  4, 'DynamicButtons.LBL_MODULE_ACCESS_TYPE', 'MODULE_ACCESS_TYPE' , 0, 1, 'module_access_type_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  5, 'DynamicButtons.LBL_TARGET_NAME'       , 'TARGET_NAME'        , 0, 1, 'Modules'                , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'DynamicButtons.EditView',  6, 'DynamicButtons.LBL_TARGET_ACCESS_TYPE', 'TARGET_ACCESS_TYPE' , 0, 1, 'module_access_type_dom' , null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  7, 'DynamicButtons.LBL_MOBILE_ONLY'       , 'MOBILE_ONLY'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  8, 'DynamicButtons.LBL_ADMIN_ONLY'        , 'ADMIN_ONLY'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  9, 'DynamicButtons.LBL_EXCLUDE_MOBILE'    , 'EXCLUDE_MOBILE'     , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 10, 'DynamicButtons.LBL_CONTROL_TEXT'      , 'CONTROL_TEXT'       , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 11, 'DynamicButtons.LBL_CONTROL_TOOLTIP'   , 'CONTROL_TOOLTIP'    , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 12, 'DynamicButtons.LBL_CONTROL_ACCESSKEY' , 'CONTROL_ACCESSKEY'  , 0, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 13, 'DynamicButtons.LBL_CONTROL_CSSCLASS'  , 'CONTROL_CSSCLASS'   , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 14, 'DynamicButtons.LBL_TEXT_FIELD'        , 'TEXT_FIELD'         , 0, 1, 200, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 15, 'DynamicButtons.LBL_ARGUMENT_FIELD'    , 'ARGUMENT_FIELD'     , 0, 1, 200, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 16, 'DynamicButtons.LBL_COMMAND_NAME'      , 'COMMAND_NAME'       , 0, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 17, 'DynamicButtons.LBL_URL_FORMAT'        , 'URL_FORMAT'         , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 18, 'DynamicButtons.LBL_URL_TARGET'        , 'URL_TARGET'         , 0, 1,  20, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'DynamicButtons.EditView', 19, 'DynamicButtons.LBL_ONCLICK_SCRIPT'    , 'ONCLICK_SCRIPT'     , 0, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView', 20, 'DynamicButtons.LBL_HIDDEN'            , 'HIDDEN'             , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'DynamicButtons.EditView', 21, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DATA_FIELD = 'EXCLUDE_MOBILE' and DELETED = 0) begin -- then
		print 'DynamicButtons: Add EXCLUDE_MOBILE.';
		update EDITVIEWS_FIELDS
		   set FIELD_INDEX       = FIELD_INDEX + 1
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'DynamicButtons.EditView'
		   and FIELD_INDEX      >= 9
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView',  9, 'DynamicButtons.LBL_EXCLUDE_MOBILE'    , 'EXCLUDE_MOBILE'     , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 03/14/2014 Paul.  Allow hidden buttons to be created. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'DynamicButtons.EditView' and DATA_FIELD = 'HIDDEN' and DELETED = 0) begin -- then
		print 'DynamicButtons: Add HIDDEN.';
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'DynamicButtons.EditView', 20, 'DynamicButtons.LBL_HIDDEN'            , 'HIDDEN'             , 0, 1, 'CheckBox', null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'DynamicButtons.EditView', 21, null;
	end -- if;
end -- if;
GO

-- 09/09/2009 Paul.  Allow direct editing of the module table. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView';
-- 12/02/2009 Paul.  Add the ability to disable Mass Updates. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Modules.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Modules.EditView'       , 'Modules', 'vwMODULES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  0, 'Modules.LBL_MODULE_NAME'              , 'MODULE_NAME'           , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Modules.EditView'       ,  1, 'Modules.LBL_DISPLAY_NAME'             , 'DISPLAY_NAME'          , 1, 1, 50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  2, 'Modules.LBL_RELATIVE_PATH'            , 'RELATIVE_PATH'         , null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  3, 'Modules.LBL_TABLE_NAME'               , 'TABLE_NAME'            , null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Modules.EditView'       ,  4, 'Modules.LBL_TAB_ORDER'                , 'TAB_ORDER'             , 1, 1, 10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  5, 'Modules.LBL_PORTAL_ENABLED'           , 'PORTAL_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  6, 'Modules.LBL_MODULE_ENABLED'           , 'MODULE_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  7, 'Modules.LBL_TAB_ENABLED'              , 'TAB_ENABLED'           , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  8, 'Modules.LBL_IS_ADMIN'                 , 'IS_ADMIN'              , null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'Modules.EditView'       ,  9, 'Modules.LBL_CUSTOM_ENABLED'           , 'CUSTOM_ENABLED'        , null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 10, 'Modules.LBL_REPORT_ENABLED'           , 'REPORT_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 11, 'Modules.LBL_IMPORT_ENABLED'           , 'IMPORT_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 12, 'Modules.LBL_MOBILE_ENABLED'           , 'MOBILE_ENABLED'        , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 13, 'Modules.LBL_CUSTOM_PAGING'            , 'CUSTOM_PAGING'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 14, 'Modules.LBL_MASS_UPDATE_ENABLED'      , 'MASS_UPDATE_ENABLED'   , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED'   , 'DEFAULT_SEARCH_ENABLED', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 16, 'Modules.LBL_EXCHANGE_SYNC'            , 'EXCHANGE_SYNC'         , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_FOLDERS'         , 'EXCHANGE_FOLDERS'      , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 18, 'Modules.LBL_EXCHANGE_CREATE_PARENT'   , 'EXCHANGE_CREATE_PARENT', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 19, 'Modules.LBL_REST_ENABLED'             , 'REST_ENABLED'          , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED', 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Modules.EditView'       , 21, null;
end else begin
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'MASS_UPDATE_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 14, 'Modules.LBL_MASS_UPDATE_ENABLED'      , 'MASS_UPDATE_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'DEFAULT_SEARCH_ENABLED' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 15
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 15, 'Modules.LBL_DEFAULT_SEARCH_ENABLED'   , 'DEFAULT_SEARCH_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/01/2010 Paul.  Add Exchange Sync flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_SYNC' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 16, 'Modules.LBL_EXCHANGE_SYNC'            , 'EXCHANGE_SYNC'         , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/04/2010 Paul.  Add Exchange Folders flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_FOLDERS' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_FOLDERS'         , 'EXCHANGE_FOLDERS'      , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 04/05/2010 Paul.  Add Exchange Create Parent flag. Need to be able to disable Account creation. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'EXCHANGE_CREATE_PARENT' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 17, 'Modules.LBL_EXCHANGE_CREATE_PARENT'   , 'EXCHANGE_CREATE_PARENT', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 06/23/2010 Paul.  Allow editing of the Portal flag. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'PORTAL_ENABLED' and DELETED = 0) begin -- then
		update EDITVIEWS_FIELDS
		   set DELETED           = 1
		     , DATE_MODIFIED     = getdate()
		     , MODIFIED_USER_ID  = null
		 where EDIT_NAME         = 'Modules.EditView'
		   and FIELD_TYPE        = 'Blank'
		   and FIELD_INDEX       = 5
		   and DELETED           = 0;
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       ,  5, 'Modules.LBL_PORTAL_ENABLED'   , 'PORTAL_ENABLED', 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 06/18/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'REST_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 19, 'Modules.LBL_REST_ENABLED'             , 'REST_ENABLED'          , 0, 1, 'CheckBox', null, null, null;
	end -- if;
	-- 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
	if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Modules.EditView' and DATA_FIELD = 'DUPLICATE_CHECHING_ENABLED' and DELETED = 0) begin -- then
		exec dbo.spEDITVIEWS_FIELDS_InsControl     'Modules.EditView'       , 20, 'Modules.LBL_DUPLICATE_CHECHING_ENABLED', 'DUPLICATE_CHECHING_ENABLED', 0, 1, 'CheckBox', null, null, null;
		exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Modules.EditView'       , 21, null;
	end -- if;
end -- if;
GO

-- 09/12/2009 Paul.  Allow editing of Field Validators. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'FieldValidators.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'FieldValidators.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS FieldValidators.EditView';
	exec dbo.spEDITVIEWS_InsertOnly          'FieldValidators.EditView' , 'FieldValidators', 'vwFIELD_VALIDATORS', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 0, 'FieldValidators.LBL_NAME'              , 'NAME'               , 1, 1,   50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 1, 'FieldValidators.LBL_VALIDATION_TYPE'   , 'VALIDATION_TYPE'    , 1, 1,   50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 2, 'FieldValidators.LBL_REGULAR_EXPRESSION', 'REGULAR_EXPRESSION' , 0, 1, 2000, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 3, 'FieldValidators.LBL_DATA_TYPE'         , 'DATA_TYPE'          , 0, 1,   25, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 4, 'FieldValidators.LBL_MININUM_VALUE'     , 'MININUM_VALUE'      , 0, 1,  255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 5, 'FieldValidators.LBL_MAXIMUM_VALUE'     , 'MAXIMUM_VALUE'      , 0, 1,  255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'FieldValidators.EditView' , 6, 'FieldValidators.LBL_COMPARE_OPERATOR'  , 'COMPARE_OPERATOR'   , 0, 1,   25, 35, null;

end -- if;
GO

-- 05/17/2010 Paul.  Allow editing of Languages. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'Languages.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Languages.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Languages.EditView';
	exec dbo.spEDITVIEWS_InsertOnly          'Languages.EditView'       , 'Languages', 'vwLANGUAGES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 0, 'Languages.LBL_NAME'                    , 'NAME'               , 1, 1,   10, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 1, 'Languages.LBL_LCID'                    , 'LCID'               , 1, 1,   10, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl   'Languages.EditView'       , 2, 'Languages.LBL_ACTIVE'                  , 'ACTIVE'             , 0, 1, 'CheckBox', null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 3, 'Languages.LBL_NATIVE_NAME'             , 'NATIVE_NAME'        , 1, 1,   80, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound     'Languages.EditView'       , 4, 'Languages.LBL_DISPLAY_NAME'            , 'DISPLAY_NAME'       , 1, 1,   80, 35, null;

end -- if;
GO

if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'EmailClient.ImportView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS EmailClient.ImportView';
	exec dbo.spEDITVIEWS_InsertOnly            'EmailClient.ImportView'  , 'EmailClient'       , 'vwEMAILS'             , '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'EmailClient.ImportView'  ,  0, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'EmailClient.ImportView'  ,  1, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'EmailClient.ImportView'  ,  2, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'EmailClient.ImportView'  ,  3, null;
end -- if;
GO

-- 08/05/2010 Paul.  Create full editing for Releases. 
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'Releases.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS Releases.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'Releases.EditView', 'Releases', 'vwRELEASES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Releases.EditView'       ,  0, 'Releases.LBL_NAME'                      , 'NAME'                       , 1, 1,  50, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList   'Releases.EditView'       ,  1, 'Releases.LBL_STATUS'                    , 'STATUS'                     , 1, 1, 'release_status_dom', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'Releases.EditView'       ,  2, 'Releases.LBL_LIST_ORDER'                , 'LIST_ORDER'                 , 1, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'Releases.EditView'       ,  3, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'Releases.EditView'       ,  2, 'Integer'                                , 'LIST_ORDER'                 , '.ERR_INVALID_INTEGER';
end -- if;
GO

-- 01/23/2012 Paul.  Create full editing for number sequences. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'NumberSequences.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'NumberSequences.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS NumberSequences.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'NumberSequences.EditView', 'NumberSequences', 'vwNUMBER_SEQUENCES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  0, 'NumberSequences.LBL_NAME'               , 'NAME'                       , 1, 1,  60, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  1, 'NumberSequences.LBL_CURRENT_VALUE'      , 'CURRENT_VALUE'              , 1, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'NumberSequences.EditView',  2, 'Integer'                                , 'CURRENT_VALUE'              , '.ERR_INVALID_INTEGER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  3, 'NumberSequences.LBL_ALPHA_PREFIX'       , 'ALPHA_PREFIX'               , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  4, 'NumberSequences.LBL_ALPHA_SUFFIX'       , 'ALPHA_SUFFIX'               , 0, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  5, 'NumberSequences.LBL_SEQUENCE_STEP'      , 'SEQUENCE_STEP'              , 1, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'NumberSequences.EditView',  6, 'Integer'                                , 'SEQUENCE_STEP'              , '.ERR_INVALID_INTEGER';
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'NumberSequences.EditView',  7, 'NumberSequences.LBL_NUMERIC_PADDING'    , 'NUMERIC_PADDING'            , 1, 1,  10, 10, null;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'NumberSequences.EditView',  8, 'Integer'                                , 'NUMERIC_PADDING'            , '.ERR_INVALID_INTEGER';
end -- if;
GO

-- 09/10/2012 Paul.  Add User Signatures. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'UserSignatures.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'UserSignatures.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS UserSignatures.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'UserSignatures.EditView' , 'UserSignatures', 'vwUSERS_SIGNATURES', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'UserSignatures.EditView' ,  0, 'UserSignatures.LBL_NAME'                , 'NAME'                       , 1, 1, 255, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl     'UserSignatures.EditView' ,  1, 'UserSignatures.LBL_PRIMARY_SIGNATURE'   , 'PRIMARY_SIGNATURE'          , 0, 1, 'CheckBox'           , null, null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsHtmlEditor  'UserSignatures.EditView' ,  2, 'UserSignatures.LBL_SIGNATURE_HTML'      , 'SIGNATURE_HTML'             , 0, 5, 120,800, 3;
end -- if;
GO

-- 09/22/2013 Paul.  Add OutboundSms module. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'OutboundSms.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'OutboundSms.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS OutboundSms.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'OutboundSms.EditView', 'OutboundSms', 'vwOUTBOUND_SMS_Edit', '20%', '30%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'OutboundSms.EditView'    ,  0, 'OutboundSms.LBL_NAME'                   , 'NAME'                       , 1, 1,  60, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'OutboundSms.EditView'    ,  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'OutboundSms.EditView'    ,  2, 'OutboundSms.LBL_FROM_NUMBER'            , 'FROM_NUMBER'                , 1, 1, 100, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'OutboundSms.EditView'    ,  1, null;
end -- if;
GO

-- 10/22/2013 Paul.  Add TwitterMessages module.
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'TwitterMessages.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'TwitterMessages.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS TwitterMessages.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'TwitterMessages.EditView', 'TwitterMessages', 'vwTWITTER_MESSAGES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'TwitterMessages.EditView',  0, 'TwitterMessages.LBL_NAME'               , 'NAME'                       , 1, 1,   2, 100, 3;
	exec dbo.spEDITVIEWS_FIELDS_InsValidator   'TwitterMessages.EditView',  0, 'Twitter Message'                        , 'NAME'                       , 'TwitterMessages.ERR_INVALID_MESSAGE';
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'TwitterMessages.EditView',  1, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'TwitterMessages.EditView',  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'TwitterMessages.EditView',  3, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'TwitterMessages.EditView',  4, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'TwitterMessages.EditView',  5, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank       'TwitterMessages.EditView',  6, null;
end -- if;
GO

-- 11/05/2014 Paul.  Add ChatChannels module. 
-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'ChatChannels.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ChatChannels.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ChatChannels.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ChatChannels.EditView', 'ChatChannels', 'vwCHAT_CHANNELS_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsBound       'ChatChannels.EditView'   ,  0, 'ChatChannels.LBL_NAME'                  , 'NAME'                       , 1, 1, 150, 35, null;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'ChatChannels.EditView'   ,  1, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ChatChannels.EditView'   ,  2, '.LBL_ASSIGNED_TO'                       , 'ASSIGNED_USER_ID'           , 0, 1, 'ASSIGNED_TO_NAME'   , 'Users', null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ChatChannels.EditView'   ,  3, 'Teams.LBL_TEAM'                         , 'TEAM_ID'                    , 0, 1, 'TEAM_NAME'          , 'Teams', null;
end -- if;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'ChatMessages.EditView';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'ChatMessages.EditView' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS ChatMessages.EditView';
	exec dbo.spEDITVIEWS_InsertOnly            'ChatMessages.EditView', 'ChatMessages', 'vwCHAT_MESSAGES_Edit', '15%', '35%', null;
	exec dbo.spEDITVIEWS_FIELDS_InsMultiLine   'ChatMessages.EditView'   ,  0, 'ChatMessages.LBL_NAME'                  , 'DESCRIPTION'                , 0, 1,   3, 80, null;
	exec dbo.spEDITVIEWS_FIELDS_InsModulePopup 'ChatMessages.EditView'   ,  1, 'ChatMessages.LBL_CHAT_CHANNEL_NAME'     , 'CHAT_CHANNEL_ID'            , 1, 1, 'CHAT_CHANNEL_NAME'  , 'ChatChannels', null;
	exec dbo.spEDITVIEWS_FIELDS_InsFile        'ChatMessages.EditView'   ,  2, 'Notes.LBL_FILENAME'                     , 'ATTACHMENT'                 , 0, 2, 255, 60, null;
	exec dbo.spEDITVIEWS_FIELDS_InsLabel       'ChatMessages.EditView'   ,  3, null                                     , 'FILENAME'                   , -1;
	exec dbo.spEDITVIEWS_FIELDS_InsChange      'ChatMessages.EditView'   ,  4, 'PARENT_TYPE'                            , 'PARENT_ID'                  , 0, 1, 'PARENT_NAME'        , 'return ParentPopup();', null;
end -- if;
GO

set nocount off;
GO

/* -- #if Oracle
	EXCEPTION
		WHEN NO_DATA_FOUND THEN
			StoO_selcnt := 0;
		WHEN OTHERS THEN
			RAISE;
	END;
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spEDITVIEWS_FIELDS_Defaults()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_Defaults')
/

-- #endif IBM_DB2 */



