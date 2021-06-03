

print 'SYSTEM_REST_TABLES Default';
-- delete from SYSTEM_REST_TABLES;
--GO

set nocount on;
GO

-- 06/18/2011 Paul.  SYSTEM_REST_TABLES are nearly identical to SYSTEM_SYNC_TABLES,
-- but the Module tables typically refer to the base view instead of the raw table. 
-- 06/18/2011 Paul.  We do not anticipate a need access to all the system tables via the REST API. 

-- System Tables
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACL_ROLES'                       , 'ACL_ROLES'                     , 'ACLRoles'                 , null                       , 0, null, 1, 0, null, 0;
-- 10/24/2011 Paul.  The HTML5 Offline Client needs access to the config table. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CONFIG'                          , 'vwCONFIG_Sync'                   , 'Config'                   , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CURRENCIES'                      , 'vwCURRENCIES'                    , 'Currencies'               , null                       , 0, null, 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'FIELD_VALIDATORS'                , 'vwFIELD_VALIDATORS'              , 'FieldValidators'          , null                       , 0, null, 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'FIELDS_META_DATA'                , 'vwFIELDS_META_DATA'              , null                       , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'LANGUAGES'                       , 'vwLANGUAGES'                     , null                       , null                       , 0, null, 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'NUMBER_SEQUENCES'                , 'vwNUMBER_SEQUENCES'              , null                       , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'RELEASES'                        , 'vwRELEASES'                      , 'Releases'                 , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TIMEZONES'                       , 'vwTIMEZONES'                     , null                       , null                       , 0, null, 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TAB_GROUPS'                      , 'TAB_GROUPS'                    , null                       , null                       , 0, null, 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'RULES'                           , 'RULES'                         , 'Rules'                    , null                       , 0, null, 1, 0, null, 0;
GO

-- System UI Tables
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACL_ACTIONS'                     , 'ACL_ACTIONS'                   , 'ACLRoles'                 , null                       , 1, 'CATEGORY'   , 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACL_ROLES_ACTIONS'               , 'vwACL_ROLES_ACTIONS_Category'  , 'ACLRoles'                 , null                       , 1, 'CATEGORY'   , 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DASHLETS'                        , 'DASHLETS'                      , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DETAILVIEWS'                     , 'DETAILVIEWS'                   , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DETAILVIEWS_FIELDS'              , 'vwDETAILVIEWS_FIELDS'            , null                       , null                       , 2, 'DETAIL_NAME', 1, 0, null, 0;
-- 08/31/2011 Paul.  DETAILVIEWS_RELATIONSHIPS does have a module associated with it. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DETAILVIEWS_RELATIONSHIPS'       , 'vwDETAILVIEWS_RELATIONSHIPS'     , 'DetailViewsRelationships' , null                       , 2, 'DETAIL_NAME', 1, 0, null, 0;
if exists(select * from vwMODULES where MODULE_NAME = 'DetailViewsRelationships' and (REST_ENABLED = 0 or REST_ENABLED is null)) begin -- then
	update MODULES
	   set REST_ENABLED         = 1
	     , MODIFIED_USER_ID     = null    
	     , DATE_MODIFIED        =  getdate()           
	     , DATE_MODIFIED_UTC    =  getutcdate()        
	 where MODULE_NAME          = 'DetailViewsRelationships'
	   and DELETED              = 0;
end -- if;
GO

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DYNAMIC_BUTTONS'                 , 'vwDYNAMIC_BUTTONS'               , 'DynamicButtons'           , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'EDITVIEWS'                       , 'EDITVIEWS'                     , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'EDITVIEWS_FIELDS'                , 'vwEDITVIEWS_FIELDS'              , null                       , null                       , 2, 'EDIT_NAME'  , 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'GRIDVIEWS'                       , 'GRIDVIEWS'                     , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'GRIDVIEWS_COLUMNS'               , 'vwGRIDVIEWS_COLUMNS'             , null                       , null                       , 2, 'GRID_NAME'  , 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'MODULES'                         , 'vwMODULES'                       , 'Modules'                  , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'SHORTCUTS'                       , 'vwSHORTCUTS'                     , 'Shortcuts'                , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TERMINOLOGY_ALIASES'             , 'TERMINOLOGY_ALIASES'           , 'Terminology'              , null                       , 0, null         , 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TERMINOLOGY_HELP'                , 'TERMINOLOGY_HELP'              , 'Terminology'              , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TERMINOLOGY'                     , 'vwTERMINOLOGY'                   , 'Terminology'              , null                       , 3, 'MODULE_NAME', 1, 0, null, 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'MODULES_GROUPS'                  , 'MODULES_GROUPS'                , null                       , null                       , 1, 'MODULE_NAME', 1, 0, null, 0;
GO

-- User Tables
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TAB_MENUS'                       , 'vwMODULES_TabMenu_ByUser'        , 'Modules'                  , null                       , 0, null, 1, 1, 'USER_ID'         , 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACL_ROLES_USERS'                 , 'ACL_ROLES_USERS'               , 'ACLRoles'                 , 'Users'                    , 0, null, 1, 1, 'USER_ID'         , 1;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DASHLETS_USERS'                  , 'DASHLETS_USERS'                , null                       , 'Users'                    , 0, null, 1, 1, 'ASSIGNED_USER_ID', 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TEAMS'                           , 'vwTEAMS'                         , 'Teams'                    , null                       , 0, null, 1, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'USERS'                           , 'vwUSERS'                         , 'Users'                    , null                       , 0, null, 1, 0, null, 0;

-- Module Tables
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACCOUNTS'                        , 'vwACCOUNTS'                      , 'Accounts'                 , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'BUGS'                            , 'vwBUGS'                          , 'Bugs'                     , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CALLS'                           , 'vwCALLS'                         , 'Calls'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CAMPAIGNS'                       , 'vwCAMPAIGNS'                     , 'Campaigns'                , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CASES'                           , 'vwCASES'                         , 'Cases'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CONTACTS'                        , 'vwCONTACTS'                      , 'Contacts'                 , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'DOCUMENTS'                       , 'vwDOCUMENTS'                     , 'Documents'                , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'EMAIL_TEMPLATES'                 , 'vwEMAIL_TEMPLATES'               , 'EmailTemplates'           , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'EMAILS'                          , 'vwEMAILS'                        , 'Emails'                   , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'FEEDS'                           , 'vwFEEDS'                         , 'Feeds'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'IFRAMES'                         , 'vwIFRAMES'                       , 'iFrames'                  , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'IMAGES'                          , 'vwIMAGES'                        , 'Images'                   , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'LEADS'                           , 'vwLEADS'                         , 'Leads'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'MEETINGS'                        , 'vwMEETINGS'                      , 'Meetings'                 , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'NOTES'                           , 'vwNOTES'                         , 'Notes'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'OPPORTUNITIES'                   , 'vwOPPORTUNITIES'                 , 'Opportunities'            , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'PROJECT'                         , 'vwPROJECT'                       , 'Project'                  , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'PROJECT_TASK'                    , 'vwPROJECT_TASK'                  , 'ProjectTask'              , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'PROSPECT_LISTS'                  , 'vwPROSPECT_LISTS'                , 'ProspectLists'            , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'PROSPECTS'                       , 'vwPROSPECTS'                     , 'Prospects'                , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'TASKS'                           , 'vwTASKS'                         , 'Tasks'                    , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
GO

-- Relationship Tables
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_ACTIVITIES'             , 'vwACCOUNTS_ACTIVITIES'           , 'Accounts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_ACTIVITIES_HISTORY'     , 'vwACCOUNTS_ACTIVITIES_HISTORY'   , 'Accounts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_ACTIVITIES_OPEN'        , 'vwACCOUNTS_ACTIVITIES_OPEN'      , 'Accounts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_BUGS'                   , 'vwACCOUNTS_BUGS'                 , 'Accounts'                 , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_CASES'                  , 'vwACCOUNTS_CASES'                , 'Accounts'                 , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_CONTACTS'               , 'vwACCOUNTS_CONTACTS'             , 'Accounts'                 , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_DOCUMENTS'              , 'vwACCOUNTS_DOCUMENTS'            , 'Accounts'                 , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_LEADS'                  , 'vwACCOUNTS_LEADS'                , 'Accounts'                 , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_MEMBERS'                , 'vwACCOUNTS_MEMBERS'              , 'Accounts'                 , 'Accounts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_OPPORTUNITIES'          , 'vwACCOUNTS_OPPORTUNITIES'        , 'Accounts'                 , 'Opportunities'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_PROJECTS'               , 'vwACCOUNTS_PROJECTS'             , 'Accounts'                 , 'Project'                  , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwACCOUNTS_USERS'                  , 'vwACCOUNTS_USERS'                , 'Accounts'                 , 'Users'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_ACTIVITIES'                 , 'vwBUGS_ACTIVITIES'               , 'Bugs'                     , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_ACTIVITIES_HISTORY'         , 'vwBUGS_ACTIVITIES_HISTORY'       , 'Bugs'                     , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_ACTIVITIES_OPEN'            , 'vwBUGS_ACTIVITIES_OPEN'          , 'Bugs'                     , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_ACCOUNTS'                   , 'vwBUGS_ACCOUNTS'                 , 'Bugs'                     , 'Accounts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_CASES'                      , 'vwBUGS_CASES'                    , 'Bugs'                     , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_CONTACTS'                   , 'vwBUGS_CONTACTS'                 , 'Bugs'                     , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_DOCUMENTS'                  , 'vwBUGS_DOCUMENTS'                , 'Bugs'                     , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwBUGS_USERS'                      , 'vwBUGS_USERS'                    , 'Bugs'                     , 'Users'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALLS_CONTACTS'                  , 'vwCALLS_CONTACTS'                , 'Calls'                    , 'Contacts'                 , 0, null, 0, 0, null, 1;
-- 04/01/2012 Paul.  Add Calls/Leads relationship. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALLS_LEADS'                     , 'vwCALLS_LEADS'                   , 'Calls'                    , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALLS_NOTES'                     , 'vwCALLS_NOTES'                   , 'Calls'                    , 'Notes'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALLS_USERS'                     , 'vwCALLS_USERS'                   , 'Calls'                    , 'Users'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCAMPAIGN_TRKRS'                  , 'vwCAMPAIGN_TRKRS'                , 'CampaignTrackers'         , null                       , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_ACTIVITIES'                , 'vwCASES_ACTIVITIES'              , 'Cases'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_ACTIVITIES_HISTORY'        , 'vwCASES_ACTIVITIES_HISTORY'      , 'Cases'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_ACTIVITIES_OPEN'           , 'vwCASES_ACTIVITIES_OPEN'         , 'Cases'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_BUGS'                      , 'vwCASES_BUGS'                    , 'Cases'                    , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_CONTACTS'                  , 'vwCASES_CONTACTS'                , 'Cases'                    , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_DOCUMENTS'                 , 'vwCASES_DOCUMENTS'               , 'Cases'                    , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_KBDOCUMENTS'               , 'vwCASES_KBDOCUMENTS'             , 'Cases'                    , 'KBDocuments'              , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCASES_USERS'                     , 'vwCASES_USERS'                   , 'Cases'                    , 'Users'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_ACTIVITIES'             , 'vwCONTACTS_ACTIVITIES'           , 'Contacts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_ACTIVITIES_HISTORY'     , 'vwCONTACTS_ACTIVITIES_HISTORY'   , 'Contacts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_ACTIVITIES_OPEN'        , 'vwCONTACTS_ACTIVITIES_OPEN'      , 'Contacts'                 , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_DIRECT_REPORTS'         , 'vwCONTACTS_DIRECT_REPORTS'       , 'Contacts'                 , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_DOCUMENTS'              , 'vwCONTACTS_DOCUMENTS'            , 'Contacts'                 , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_LEADS'                  , 'vwCONTACTS_LEADS'                , 'Contacts'                 , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_OPPORTUNITIES'          , 'vwCONTACTS_OPPORTUNITIES'        , 'Contacts'                 , 'Opportunities'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_PROJECTS'               , 'vwCONTACTS_PROJECTS'             , 'Contacts'                 , 'Project'                  , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_PROSPECT_LISTS'         , 'vwCONTACTS_PROSPECT_LISTS'       , 'Contacts'                 , 'ProspectLists'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_BUGS'                   , 'vwCONTACTS_BUGS'                 , 'Contacts'                 , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_CASES'                  , 'vwCONTACTS_CASES'                , 'Contacts'                 , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCONTACTS_USERS'                  , 'vwCONTACTS_USERS'                , 'Contacts'                 , 'Users'                    , 0, null, 0, 0, null, 1;

-- 09/15/2012 Paul.  New tables for Accounts, Bugs, Cases, Contacts, Contracts, Leads, Opportunities, Quotes. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_ACCOUNTS'              , 'vwDOCUMENTS_ACCOUNTS'            , 'Documents'                , 'Accounts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_BUGS'                  , 'vwDOCUMENTS_BUGS'                , 'Documents'                , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_CASES'                 , 'vwDOCUMENTS_CASES'               , 'Documents'                , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_CONTACTS'              , 'vwDOCUMENTS_CONTACTS'            , 'Documents'                , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_LEADS'                 , 'vwDOCUMENTS_LEADS'               , 'Documents'                , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENTS_OPPORTUNITIES'         , 'vwDOCUMENTS_OPPORTUNITIES'       , 'Documents'                , 'Opportunities'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwDOCUMENT_REVISIONS'              , 'vwDOCUMENT_REVISIONS'            , 'Documents'                , null                       , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAIL_IMAGES'                    , 'vwEMAIL_IMAGES'                  , 'Images'                   , null                       , 0, null, 0, 0, null, 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAIL_MARKETING'                 , 'vwEMAIL_MARKETING'               , 'EmailMarketing'           , null                       , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAIL_MARKETING_PROSPECT_LISTS'  , 'vwEMAIL_MARKETING_PROSPECT_LISTS', 'EmailMarketing'           , 'ProspectLists'            , 0, null, 0, 0, null, 1;
-- 08/28/2012 Paul.  Add Call Marketing. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALL_MARKETING'                  , 'vwCALL_MARKETING'                , 'CallMarketing'            , null                       , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwCALL_MARKETING_PROSPECT_LISTS'   , 'vwCALL_MARKETING_PROSPECT_LISTS' , 'CallMarketing'            , 'ProspectLists'            , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_ACCOUNTS'                 , 'vwEMAILS_ACCOUNTS'               , 'Emails'                   , 'Accounts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_BUGS'                     , 'vwEMAILS_BUGS'                   , 'Emails'                   , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_CASES'                    , 'vwEMAILS_CASES'                  , 'Emails'                   , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_CONTACTS'                 , 'vwEMAILS_CONTACTS'               , 'Emails'                   , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_LEADS'                    , 'vwEMAILS_LEADS'                  , 'Emails'                   , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_OPPORTUNITIES'            , 'vwEMAILS_OPPORTUNITIES'          , 'Emails'                   , 'Opportunities'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_PROJECT_TASKS'            , 'vwEMAILS_PROJECT_TASKS'          , 'Emails'                   , 'ProjectTask'              , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_PROJECTS'                 , 'vwEMAILS_PROJECTS'               , 'Emails'                   , 'Project'                  , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_PROSPECTS'                , 'vwEMAILS_PROSPECTS'              , 'Emails'                   , 'Prospects'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_TASKS'                    , 'vwEMAILS_TASKS'                  , 'Emails'                   , 'Tasks'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwEMAILS_USERS'                    , 'vwEMAILS_USERS'                  , 'Emails'                   , 'Users'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwKBDOCUMENTS_CASES'               , 'vwKBDOCUMENTS_CASES'             , 'KBDocuments'              , 'Cases'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_ACTIVITIES'                , 'vwLEADS_ACTIVITIES'              , 'Leads'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_ACTIVITIES_HISTORY'        , 'vwLEADS_ACTIVITIES_HISTORY'      , 'Leads'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_ACTIVITIES_OPEN'           , 'vwLEADS_ACTIVITIES_OPEN'         , 'Leads'                    , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_DOCUMENTS'                 , 'vwLEADS_DOCUMENTS'               , 'Leads'                    , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_PROSPECT_LISTS'            , 'vwLEADS_PROSPECT_LISTS'          , 'Leads'                    , 'ProspectLists'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwLEADS_USERS'                     , 'vwLEADS_USERS'                   , 'Leads'                    , 'Users'                    , 0, null, 0, 0, null, 1;


exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwMEETINGS_CONTACTS'               , 'vwMEETINGS_CONTACTS'             , 'Meetings'                 , 'Contacts'                 , 0, null, 0, 0, null, 1;
-- 04/01/2012 Paul.  Add Meetings/Leads relationship. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwMEETINGS_LEADS'                  , 'vwMEETINGS_LEADS'                , 'Meetings'                 , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwMEETINGS_USERS'                  , 'vwMEETINGS_USERS'                , 'Meetings'                 , 'Users'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwNOTE_ATTACHMENTS'                , 'vwNOTE_ATTACHMENTS'              , 'Notes'                    , null                       , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_ACTIVITIES'        , 'vwOPPORTUNITIES_ACTIVITIES'      , 'Opportunities'            , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_ACTIVITIES_HISTORY', 'vwOPPORTUNITIES_ACTIVITIES_HISTORY', 'Opportunities'          , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_ACTIVITIES_OPEN'   , 'vwOPPORTUNITIES_ACTIVITIES_OPEN' , 'Opportunities'            , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_CONTACTS'          , 'vwOPPORTUNITIES_CONTACTS'        , 'Opportunities'            , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_DOCUMENTS'         , 'vwOPPORTUNITIES_DOCUMENTS'       , 'Opportunities'            , 'Documents'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_LEADS'             , 'vwOPPORTUNITIES_LEADS'           , 'Opportunities'            , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_PROJECTS'          , 'vwOPPORTUNITIES_PROJECTS'        , 'Opportunities'            , 'Project'                  , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_CONTACTS'          , 'vwOPPORTUNITIES_CONTACTS'        , 'Opportunities'            , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwOPPORTUNITIES_USERS'             , 'vwOPPORTUNITIES_USERS'           , 'Opportunities'            , 'Users'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECT_RELATION'                , 'vwPROJECT_RELATION'              , 'Project'                  , null                       , 0, null, 0, 0, null, 1;

-- 09/15/2012 Paul.  New tables for Accounts, Bugs, Cases, Contacts, Opportunities, ProjectTask, Threads, Quotes. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_ACTIVITIES'             , 'vwPROJECTS_ACTIVITIES'           , 'Project'                  , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_ACTIVITIES_HISTORY'     , 'vwPROJECTS_ACTIVITIES_HISTORY'   , 'Project'                  , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_ACTIVITIES_OPEN'        , 'vwPROJECTS_ACTIVITIES_OPEN'      , 'Project'                  , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_ACCOUNTS'               , 'vwPROJECTS_ACCOUNTS'             , 'Project'                  , 'Accounts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_BUGS'                   , 'vwPROJECTS_BUGS'                 , 'Project'                  , 'Bugs'                     , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_CASES'                  , 'vwPROJECTS_CASES'                , 'Project'                  , 'Cases'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_CONTACTS'               , 'vwPROJECTS_CONTACTS'             , 'Project'                  , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_OPPORTUNITIES'          , 'vwPROJECTS_OPPORTUNITIES'        , 'Project'                  , 'Opportunities'            , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_PROJECT_TASKS'          , 'vwPROJECTS_PROJECT_TASKS'        , 'Project'                  , 'ProjectTask'              , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECTS_USERS'                  , 'vwPROJECT_USERS'                 , 'Project'                  , 'Users'                    , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECT_TASKS_ACTIVITIES'        , 'vwPROJECT_TASKS_ACTIVITIES'      , 'ProjectTask'              , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECT_TASKS_ACTIVITIES_HISTORY', 'vwPROJECT_TASKS_ACTIVITIES_HISTORY', 'ProjectTask'            , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROJECT_TASKS_ACTIVITIES_OPEN'   , 'vwPROJECT_TASKS_ACTIVITIES_OPEN' , 'ProjectTask'              , 'Activities'               , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECTS_ACTIVITIES'            , 'vwPROSPECTS_ACTIVITIES'          , 'Prospects'                , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECTS_ACTIVITIES_HISTORY'    , 'vwPROSPECTS_ACTIVITIES_HISTORY'  , 'Prospects'                , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECTS_ACTIVITIES_OPEN'       , 'vwPROSPECTS_ACTIVITIES_OPEN'     , 'Prospects'                , 'Activities'               , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECTS_PROSPECT_LISTS'        , 'vwPROSPECTS_PROSPECT_LISTS'      , 'Prospects'                , 'ProspectLists'            , 0, null, 0, 0, null, 1;

exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECT_LIST_CAMPAIGNS'         , 'vwPROSPECT_LIST_CAMPAIGNS'       , 'ProspectLists'            , 'Campaigns'                , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECT_LISTS_PROSPECTS'        , 'vwPROSPECT_LISTS_PROSPECTS'      , 'ProspectLists'            , null                       , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECT_LISTS_CONTACTS'         , 'vwPROSPECT_LISTS_CONTACTS'       , 'ProspectLists'            , 'Contacts'                 , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECT_LISTS_LEADS'            , 'vwPROSPECT_LISTS_LEADS'          , 'ProspectLists'            , 'Leads'                    , 0, null, 0, 0, null, 1;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPROSPECT_LISTS_USERS'            , 'vwPROSPECT_LISTS_USERS'          , 'ProspectLists'            , 'Users'                    , 0, null, 0, 0, null, 1;

--exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_FEEDS'                     , 'vwUSERS_FEEDS'                   , 'Users'                    , 'Feeds'                    , 0, null, 0, 1, 'USER_ID', 1;
-- 09/15/2012 Paul.  Add UserSignatures. 
-- 06/27/2014 Paul.  User Signatures should not have MODULE_NAME_RELATED specified. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwUSERS_SIGNATURES'                , 'vwUSERS_SIGNATURES'              , 'Users'                    , null                       , 0, null, 0, 0, 'USER_ID', 1;

-- 01/19/2013 Paul.  Activities need access to the vwPARENTS view in order to allow click through the parent link on the list view. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'vwPARENTS'                         , 'vwPARENTS'                       , null                       , null                       , 0, null, 0, 0, 'PARENT_ASSIGNED_USER_ID', 1;

-- 02/23/2013 Paul.  In order to show the Calendar module, we need to enable it as a REST table. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'ACTIVITIES'                        , 'vwACTIVITIES_List'               , 'Calendar'                 , null                       , 0, null, 0, 1, null, 0;

-- 11/17/2014 Paul.  Add ChatChannels module. 
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CHAT_CHANNELS'                     , 'vwCHAT_CHANNELS'                 , 'ChatChannels'             , null                       , 0, null, 0, 1, 'ASSIGNED_USER_ID', 0;
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CHAT_MESSAGES'                     , 'vwCHAT_MESSAGES'                 , 'ChatMessages'             , null                       , 0, null, 0, 0, 'PARENT_ASSIGNED_USER_ID', 0;
-- delete from SYSTEM_REST_TABLES where TABLE_NAME = 'CHAT_DASHBOARD';
exec dbo.spSYSTEM_REST_TABLES_InsertOnly null, 'CHAT_DASHBOARD'                    , 'vwCHAT_MESSAGES_List'            , 'ChatDashboard'            , null                       , 0, null, 0, 1, null, 0;
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

call dbo.spSYSTEM_REST_TABLES_Default()
/

call dbo.spSqlDropProcedure('spSYSTEM_REST_TABLES_Default')
/

-- #endif IBM_DB2 */


