

print 'MODULES defaults';
GO

set nocount on;
GO

-- 05/02/2006 Paul.  Add TABLE_NAME as direct table queries are required by SOAP and we need a mapping. 
-- 05/20/2006 Paul.  Add REPORT_ENABLED if the module can be the basis of a report. ACL rules will still apply. 
-- 10/06/2006 Paul.  Add IMPORT_ENABLED if the module can allow importing. 
-- 04/11/2007 Paul.  Since we are using InsertOnly procedures, we don't need all the if exists filters. 
-- 02/09/2008 Paul.  Move maintenance code to separate file. 
-- 08/22/2008 Paul.  Move professional modules to a separate file. 
-- 09/17/2008 Paul.  Enable Mobile for the core modules (Accounts, Contacts, Leads, Opportunities, Cases, Bugs, Calls, Meetings).
-- 11/27/2008 Paul.  Re-arrange the tabs to match the order in SugarCRM 5.1b.
-- 01/13/2010 Paul.  Set default for MASS_UPDATE_ENABLED. 
-- 04/01/2010 Paul.  Add Exchange Sync flag. 
-- 05/02/2010 Paul.  Add defaults for Exchange Folders and Exchange Create Parent. 
-- 08/01/2010 Paul.  Reorder to match the latest Sugar release. 
-- 08/02/2010 Paul.  Sugar Activities is now the Calendar.  We will keep the old Calendar name for now. 
-- 09/12/2011 Paul.  REST_ENABLED provides a way to enable/disable a module in the REST API. 

exec dbo.spMODULES_InsertOnly null, 'Home'                  , '.moduleList.Home'                     , '~/Home/'                            , 1, 1,  1, 0, 0, 0, 0, 0, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Accounts'              , '.moduleList.Accounts'                 , '~/Accounts/'                        , 1, 1,  2, 0, 1, 1, 1, 0, 'ACCOUNTS'       , 1, 1, 1, 1, 1, 1;
exec dbo.spMODULES_InsertOnly null, 'Contacts'              , '.moduleList.Contacts'                 , '~/Contacts/'                        , 1, 1,  3, 0, 1, 1, 1, 0, 'CONTACTS'       , 1, 1, 1, 1, 1, 1;
exec dbo.spMODULES_InsertOnly null, 'Opportunities'         , '.moduleList.Opportunities'            , '~/Opportunities/'                   , 1, 1,  4, 0, 1, 1, 1, 0, 'OPPORTUNITIES'  , 1, 1, 1, 1, 1, 1;
exec dbo.spMODULES_InsertOnly null, 'Leads'                 , '.moduleList.Leads'                    , '~/Leads/'                           , 1, 1,  5, 0, 1, 1, 1, 0, 'LEADS'          , 1, 1, 1, 1, 1, 1;
-- 02/23/2013 Paul.  In order to show the Calendar module, we need to enable it as a REST table. 
-- 02/26/2013 Paul.  New calendar uses html5. 
exec dbo.spMODULES_InsertOnly null, 'Calendar'              , '.moduleList.Calendar'                 , '~/Calendar/html5/'                  , 1, 1,  6, 0, 0, 0, 0, 0, 'ACTIVITIES'     , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Documents'             , '.moduleList.Documents'                , '~/Documents/'                       , 1, 1,  8, 0, 1, 1, 0, 0, 'DOCUMENTS'      , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Emails'                , '.moduleList.Emails'                   , '~/Emails/'                          , 1, 1,  9, 0, 1, 1, 0, 0, 'EMAILS'         , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Campaigns'             , '.moduleList.Campaigns'                , '~/Campaigns/'                       , 1, 1, 10, 0, 1, 1, 0, 0, 'CAMPAIGNS'      , 0, 1, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Cases'                 , '.moduleList.Cases'                    , '~/Cases/'                           , 1, 1, 14, 0, 1, 1, 0, 0, 'CASES'          , 1, 1, 1, 1, 1, 1;
exec dbo.spMODULES_InsertOnly null, 'Project'               , '.moduleList.Project'                  , '~/Projects/'                        , 1, 1, 15, 0, 1, 1, 0, 0, 'PROJECT'        , 0, 1, 1, 1, 1, 1;
exec dbo.spMODULES_InsertOnly null, 'Bugs'                  , '.moduleList.Bugs'                     , '~/Bugs/'                            , 1, 1, 16, 0, 1, 1, 0, 0, 'BUGS'           , 1, 1, 1, 1, 1, 1;
-- 01/11/2015 Paul.  Show the dashboard as part of new HTML5 version 9. 
exec dbo.spMODULES_InsertOnly null, 'Dashboard'             , '.moduleList.Dashboard'                , '~/Dashboard/'                       , 1, 1,  7, 0, 0, 0, 0, 0, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Activities'            , '.moduleList.Activities'               , '~/Activities/'                      , 1, 0,  0, 0, 0, 0, 0, 0, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'iFrames'               , '.moduleList.iFrames'                  , '~/iFrames/'                         , 1, 0,  0, 0, 0, 0, 0, 0, 'IFRAMES'        , 0, 1, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Feeds'                 , '.moduleList.Feeds'                    , '~/Feeds/'                           , 1, 0,  0, 0, 0, 0, 0, 0, 'FEEDS'          , 0, 0, 0, 0, 0, 0;

-- 07/19/2010 Paul.  Add email client. 
exec dbo.spMODULES_InsertOnly null, 'EmailClient'           , '.moduleList.EmailClient'              , '~/EmailClient/'                     , 1, 0,  0, 0, 0, 0, 0, 0, null             , 0, 0, 0, 0, 0, 0;
-- 07/07/2007 Paul.  Add CampaignTrackers and EmailMarketing.  
exec dbo.spMODULES_InsertOnly null, 'CampaignTrackers'      , '.moduleList.CampaignTrackers'         , '~/CampaignTrackers/'                , 1, 0,  0, 0, 1, 0, 0, 0, 'CAMPAIGN_TRKRS' , 0, 0, 0, 0, 0, 0;
-- 05/14/2008 Paul.  Add CampaignLog. 
-- 08/13/2014 Paul.  Make the CampaignLog reportable so that workflow can be attached. 
exec dbo.spMODULES_InsertOnly null, 'CampaignLog'           , '.moduleList.CampaignLog'              , '~/CampaignTrackers/'                , 1, 0,  0, 0, 1, 1, 0, 0, 'CAMPAIGN_LOG'   , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'EmailMarketing'        , '.moduleList.EmailMarketing'           , '~/EmailMarketing/'                  , 1, 0,  0, 0, 1, 0, 0, 0, 'EMAIL_MARKETING', 0, 0, 0, 0, 0, 0;
-- 08/28/2012 Paul.  Add Call Marketing. 
exec dbo.spMODULES_InsertOnly null, 'CallMarketing'         , '.moduleList.CallMarketing'            , '~/CallMarketing/'                   , 1, 0,  0, 0, 1, 0, 0, 0, 'CALL_MARKETING' , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Administration'        , '.moduleList.Administration'           , '~/Administration/'                  , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 03/09/2010 Paul.  Add Languages so that admin roles can be applied. 
exec dbo.spMODULES_InsertOnly null, 'Languages'             , 'Administration.LBL_MANAGE_LANGUAGES'  , '~/Administration/Languages/'        , 1, 0,  0, 0, 0, 0, 0, 1, 'LANGUAGES'      , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'AuditEvents'           , 'Administration.LBL_AUDIT_EVENTS_TITLE', '~/Administration/AuditEvents/'      , 1, 0,  0, 0, 0, 0, 0, 1, 'AUDIT_EVENTS'   , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Dropdown'              , '.moduleList.Dropdown'                 , '~/Administration/Dropdown/'         , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 03/03/2010 Paul.  Add the Config module so that shortcuts can be displayed. 
exec dbo.spMODULES_InsertOnly null, 'Config'                , '.moduleList.Config'                   , '~/Administration/Config/'           , 1, 0,  0, 0, 0, 0, 0, 1, 'CONFIG'         , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Releases'              , '.moduleList.Releases'                 , '~/Administration/Releases/'         , 1, 0,  0, 0, 1, 1, 0, 1, 'RELEASES'       , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Calls'                 , '.moduleList.Calls'                    , '~/Calls/'                           , 1, 0,  0, 0, 1, 1, 0, 0, 'CALLS'          , 1, 1, 0, 0, 0, 1;
-- 09/09/2009 Paul.  Employees are stored in the USERS table, so specify the table in the record. 
exec dbo.spMODULES_InsertOnly null, 'Employees'             , '.moduleList.Employees'                , '~/Employees/'                       , 1, 0,  0, 0, 0, 0, 0, 0, 'USERS'          , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Meetings'              , '.moduleList.Meetings'                 , '~/Meetings/'                        , 1, 0,  0, 0, 1, 1, 0, 0, 'MEETINGS'       , 1, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Notes'                 , '.moduleList.Notes'                    , '~/Notes/'                           , 1, 0,  0, 0, 1, 1, 1, 0, 'NOTES'          , 0, 1, 0, 0, 0, 1;
-- 05/26/2007 Paul.  Fix Project Tasks module name.  This should be singular to ensure compatibility with SugarCRM. 
exec dbo.spMODULES_InsertOnly null, 'ProjectTask'           , '.moduleList.ProjectTask'              , '~/ProjectTasks/'                    , 1, 0,  0, 0, 1, 1, 0, 0, 'PROJECT_TASK'   , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Tasks'                 , '.moduleList.Tasks'                    , '~/Tasks/'                           , 1, 0,  0, 0, 1, 1, 0, 0, 'TASKS'          , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Users'                 , '.moduleList.Users'                    , '~/Users/'                           , 1, 0,  0, 0, 1, 1, 0, 1, 'USERS'          , 0, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'Prospects'             , '.moduleList.Prospects'                , '~/Prospects/'                       , 1, 0,  0, 0, 1, 1, 1, 0, 'PROSPECTS'      , 1, 1, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'ProspectLists'         , '.moduleList.ProspectLists'            , '~/ProspectLists/'                   , 1, 0,  0, 0, 1, 1, 0, 0, 'PROSPECT_LISTS' , 0, 1, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'EmailTemplates'        , '.moduleList.EmailTemplates'           , '~/EmailTemplates/'                  , 1, 0,  0, 0, 1, 0, 0, 0, 'EMAIL_TEMPLATES', 0, 1, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Import'                , '.moduleList.Import'                   , '~/Import/'                          , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'EditCustomFields'      , '.moduleList.EditCustomFields'         , '~/Administration/EditCustomFields/' , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'DynamicLayout'         , '.moduleList.DynamicLayout'            , '~/Administration/DynamicLayout/'    , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Terminology'           , '.moduleList.Terminology'              , '~/Administration/Terminology/'      , 1, 0,  0, 0, 0, 0, 0, 1, 'TERMINOLOGY'    , 0, 0, 0, 0, 0, 0;
-- 04/22/2006 Paul.  Add ACLRoles as a module.  Set the CUSTOM_ENABLED flag. 
-- 05/26/2007 Paul.  There is no compelling reason to allow ACLRoles to be customized. 
exec dbo.spMODULES_InsertOnly null, 'ACLRoles'              , '.moduleList.ACLRoles'                 , '~/Administration/ACLRoles/'         , 1, 0,  0, 0, 0, 0, 0, 1, 'ACL_ROLES'      , 0, 0, 0, 0, 0, 0;
-- 10/25/2006 Paul.  Create the Help module so that access rights can be defined. 
exec dbo.spMODULES_InsertOnly null, 'Help'                  , '.moduleList.Help'                     , '~/Help/'                            , 1, 0,  0, 0, 0, 0, 0, 1, null             , 0, 0, 0, 0, 0, 0;
-- 12/14/2007 Paul.  Need to a a module record for Shortcuts so that its own shortcuts will appear. 
-- 07/24/2008 Paul.  Admin modules are not typically reported on.
exec dbo.spMODULES_InsertOnly null, 'Shortcuts'             , '.moduleList.Shortcuts'                , '~/Administration/Shortcuts/'        , 1, 0,  0, 0, 1, 0, 0, 1, 'SHORTCUTS'      , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'EmailMan'              , '.moduleList.EmailMan'                 , '~/Administration/EmailMan/'         , 1, 0,  0, 0, 1, 0, 0, 1, 'EMAILMAN'       , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'InboundEmail'          , '.moduleList.InboundEmail'             , '~/Administration/InboundEmail/'     , 1, 0,  0, 0, 1, 0, 0, 1, 'INBOUND_EMAILS' , 0, 0, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'Schedulers'            , '.moduleList.Schedulers'               , '~/Administration/Schedulers/'       , 1, 0,  0, 0, 1, 0, 0, 1, 'SCHEDULERS'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  DynamicButtons should be treated as a module. 
exec dbo.spMODULES_InsertOnly null, 'DynamicButtons'        , '.moduleList.DynamicButtons'           , '~/Administration/DynamicButtons/'   , 1, 0,  0, 0, 0, 0, 0, 1, 'DYNAMIC_BUTTONS', 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  Currencies module.
exec dbo.spMODULES_InsertOnly null, 'Currencies'            , '.moduleList.Currencies'               , '~/Administration/Currencies/'       , 1, 0,  0, 0, 0, 0, 0, 1, 'CURRENCIES'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  System Log.
exec dbo.spMODULES_InsertOnly null, 'SystemLog'             , '.moduleList.SystemLog'                , '~/Administration/SystemLog/'        , 1, 0,  0, 0, 0, 0, 0, 1, 'SYSTEM_LOG'     , 0, 0, 0, 0, 0, 0;
-- 05/13/2008 Paul.  User Log.
exec dbo.spMODULES_InsertOnly null, 'UserLogins'            , '.moduleList.UserLogins'               , '~/Administration/UserLogins/'       , 1, 0,  0, 0, 0, 0, 0, 1, 'USERS_LOGINS'   , 0, 0, 0, 0, 0, 0;
-- 09/09/2009 Paul.  Allow direct editing of the module table. 
exec dbo.spMODULES_InsertOnly null, 'Modules'               , '.moduleList.Modules'                  , '~/Administration/Modules/'          , 1, 0,  0, 0, 0, 0, 0, 1, 'MODULES'        , 0, 0, 0, 0, 0, 0;
-- 09/12/2009 Paul.  Allow editing of the field validators. 
exec dbo.spMODULES_InsertOnly null, 'FieldValidators'       , '.moduleList.FieldValidators'          , '~/Administration/FieldValidators/'  , 1, 0,  0, 0, 0, 0, 0, 1, 'FIELD_VALIDATORS', 0, 0, 0, 0, 0, 0;
-- 11/22/2009 Paul.  System Sync Log.
exec dbo.spMODULES_InsertOnly null, 'SystemSyncLog'         , '.moduleList.SystemSyncLog'            , '~/Administration/SystemSyncLog/'    , 1, 0,  0, 0, 0, 0, 0, 1, 'SYSTEM_SYNC_LOG' , 0, 0, 0, 0, 0, 0;
-- 04/15/2011 Paul.  Add facebook authentication. 
exec dbo.spMODULES_InsertOnly null, 'Facebook'              , '.moduleList.Facebook'                 , '~/Administration/Facebook/'         , 1, 0,  0, 0, 0, 0, 0, 1, null              , 0, 0, 0, 0, 0, 0;

-- 04/23/2011 Paul.  DetailViewsRelationships should be treated as a module so that the merge modules can be retrieved by the Word Plug-in. 
exec dbo.spMODULES_InsertOnly null, 'DetailViewsRelationships', '.moduleList.DetailViewsRelationships' , '~/Administration/DetailViewsRelationships/'   , 1, 0,  0, 0, 0, 0, 0, 1, 'DETAILVIEWS_RELATIONSHIPS', 0, 0, 0, 0, 0, 0;

-- 06/08/2012 Paul.  Add an images module to make it easier to get the Image name in the DetailView. 
exec dbo.spMODULES_InsertOnly null, 'Images'                , '.moduleList.Images'                   , '~/Images/'                          , 1, 0,  0, 0, 0, 0, 0, 0, 'IMAGES'          , 0, 0, 0, 0, 0, 0;

-- 09/10/2012 Paul.  Add User Signatures. 
exec dbo.spMODULES_InsertOnly null, 'UserSignatures'        , '.moduleList.UserSignatures'           , '~/Users/UserSignatures/'            , 1, 0,  0, 0, 0, 0, 0, 0, 'USERS_SIGNATURES', 0, 0, 0, 0, 0, 0;

-- 01/11/2013 Paul.  Should have created NumberSequences long ago so that menu when editing would work properly. 
-- 02/22/2013 Paul.  NumberSequences is an admin module. 
exec dbo.spMODULES_InsertOnly null, 'NumberSequences'        , '.moduleList.NumberSequences'         , '~/Administration/NumberSequences/'  , 1, 0,  0, 0, 0, 0, 0, 1, 'NUMBER_SEQUENCES', 0, 0, 0, 0, 0, 0;

-- 08/07/2013 Paul.  Add Undelete module. 
exec dbo.spMODULES_InsertOnly null, 'Undelete'               , '.moduleList.Undelete'                , '~/Administration/Undelete/'         , 1, 0,  0, 0, 0, 0, 0, 1, null              , 0, 0, 0, 0, 0, 0;

-- 09/22/2013 Paul.  Add SmsMessages module. 
exec dbo.spMODULES_InsertOnly null, 'SmsMessages'            , '.moduleList.SmsMessages'             , '~/SmsMessages/'                     , 1, 1, 17, 0, 1, 1, 0, 0, 'SMS_MESSAGES'    , 0, 1, 0, 0, 0, 0;
exec dbo.spMODULES_InsertOnly null, 'OutboundSms'            , '.moduleList.OutboundSms'             , '~/Administration/OutboundSms/'      , 1, 0,  0, 0, 0, 0, 0, 1, 'OUTBOUND_SMS'    , 0, 0, 0, 0, 0, 0;
-- 10/22/2013 Paul.  Add TwitterMessages module.
exec dbo.spMODULES_InsertOnly null, 'TwitterMessages'        , '.moduleList.TwitterMessages'         , '~/TwitterMessages/'                 , 1, 1, 18, 0, 1, 1, 0, 0, 'TWITTER_MESSAGES', 0, 1, 0, 0, 0, 0;
-- 11/11/2013 Paul.  We want the admin Twilio page to have a clickable tab. 
exec dbo.spMODULES_InsertOnly null, 'Twilio'                 , '.moduleList.Twilio'                  , '~/Administration/Twilio/'           , 1, 0,  0, 0, 0, 0, 0, 1, null              , 0, 0, 0, 0, 0, 0;
-- 11/05/2014 Paul.  Add ChatChannels module. 
-- delete from MODULES where MODULE_NAME in ('ChatMessages', 'ChatChannels');
exec dbo.spMODULES_InsertOnly null, 'ChatChannels'           , '.moduleList.ChatChannels'            , '~/ChatChannels/'                    , 1, 1, 19, 0, 0, 0, 0, 0, 'CHAT_CHANNELS'   , 1, 0, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'ChatMessages'           , '.moduleList.ChatMessages'            , '~/ChatMessages/'                    , 1, 0,  0, 0, 0, 0, 0, 0, 'CHAT_MESSAGES'   , 0, 0, 0, 0, 0, 1;
exec dbo.spMODULES_InsertOnly null, 'ChatDashboard'          , '.moduleList.ChatDashboard'           , '~/ChatDashboard/'                   , 1, 1, 20, 0, 0, 0, 0, 0, null              , 1, 0, 0, 0, 0, 1;
GO

-- 03/14/2014 Paul.  DUP_CHECH_ENABLED enables duplicate checking. 
if exists(select * from MODULES where MODULE_NAME = N'Accounts' and DUPLICATE_CHECHING_ENABLED is null) begin -- then
	print 'MODULES: Update DUPLICATE_CHECHING_ENABLED defaults.';
	update MODULES
	   set DUPLICATE_CHECHING_ENABLED = 1
	     , DATE_MODIFIED       = getdate()
	     , MODIFIED_USER_ID    = null
	 where MODULE_NAME in 
		( N'Accounts'
		, N'Bugs'
		, N'Campaigns'
		, N'Cases'
		, N'Contacts'
		, N'Documents'
		, N'EmailTemplates'
		, N'Leads'
		, N'Opportunities'
		, N'Project'
		, N'ProjectTask'
		, N'ProspectLists'
		, N'Prospects'
		, N'Tasks'
		);
end -- if;
GO

-- 08/24/2008 Paul.  Reorder the modules. 
exec dbo.spMODULES_Reorder null;
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

call dbo.spMODULES_Defaults()
/

call dbo.spSqlDropProcedure('spMODULES_Defaults')
/

-- #endif IBM_DB2 */


