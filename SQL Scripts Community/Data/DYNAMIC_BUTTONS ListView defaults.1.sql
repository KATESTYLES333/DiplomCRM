

print 'DYNAMIC_BUTTONS ListView defaults';
-- delete from DYNAMIC_BUTTONS where VIEW_NAME like '%.ListView'
--GO

set nocount on;
GO


-- 07/16/2010 Paul.  Exchange can have different buttons than Imap. 
-- 09/12/2010 Paul.  Fix delete to be SQL-92 compliant. 
if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView' and DELETED = 0) begin -- then
	delete from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView';
end -- if;
GO

-- 01/25/2013 Paul.  Module name should be EmailClient, not Emails. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView.Exchange' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS EmailClient.ListView.Exchange';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'EmailClient.ListView.Exchange' , 0, 'EmailClient'       , 'view'  , null, null, 'CheckMail'           , null, 'Emails.LBL_BUTTON_CHECK'                       , 'Emails.LBL_BUTTON_CHECK'                       , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'EmailClient.ListView.Exchange' , 1, 'EmailClient'       , 'edit'  , null, null, 'Compose'             , null, 'Emails.LNK_NEW_SEND_EMAIL'                     , 'Emails.LNK_NEW_SEND_EMAIL'                     , null, null, null;
end else begin
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView.Exchange' and MODULE_NAME = 'Emails' and DELETED = 0) begin -- then
		update DYNAMIC_BUTTONS
		   set MODULE_NAME       = 'EmailClient'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where VIEW_NAME         = 'EmailClient.ListView.Exchange'
		   and MODULE_NAME       = 'Emails'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

-- 01/25/2013 Paul.  Module name should be EmailClient, not Emails. 
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView.Imap' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS EmailClient.ListView.Imap';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'EmailClient.ListView.Imap'      , 0, 'EmailClient'      , 'view'  , null, null, 'CheckMail'           , null, 'Emails.LBL_BUTTON_CHECK'                       , 'Emails.LBL_BUTTON_CHECK'                       , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'EmailClient.ListView.Imap'      , 1, 'EmailClient'      , 'edit'  , null, null, 'Compose'             , null, 'Emails.LNK_NEW_SEND_EMAIL'                     , 'Emails.LNK_NEW_SEND_EMAIL'                     , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'EmailClient.ListView.Imap'      , 2, 'EmailClient'      , 'edit'  , null, null, 'Settings'            , null, 'EmailClient.LBL_SETTINGS_BUTTON_LABEL'         , 'EmailClient.LBL_SETTINGS_BUTTON_TITLE'         , null, null, null;
end else begin
	if exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'EmailClient.ListView.Imap' and MODULE_NAME = 'Emails' and DELETED = 0) begin -- then
		update DYNAMIC_BUTTONS
		   set MODULE_NAME       = 'EmailClient'
		     , DATE_MODIFIED     = getdate()
		     , DATE_MODIFIED_UTC = getutcdate()
		     , MODIFIED_USER_ID  = null
		 where VIEW_NAME         = 'EmailClient.ListView.Imap'
		   and MODULE_NAME       = 'Emails'
		   and DELETED           = 0;
	end -- if;
end -- if;
GO

-- delete from DYNAMIC_BUTTONS where VIEW_NAME = 'TwitterMessages.ImportView';
if not exists(select * from DYNAMIC_BUTTONS where VIEW_NAME = 'TwitterMessages.ImportView' and DELETED = 0) begin -- then
	print 'DYNAMIC_BUTTONS TwitterMessages.ImportView';
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'TwitterMessages.ImportView'      , 0, 'TwitterMessages' , 'edit'  , null, null, 'Search'              , null, 'TwitterMessages.LBL_BUTTON_SEARCH'           , 'TwitterMessages.LBL_BUTTON_SEARCH'           , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'TwitterMessages.ImportView'      , 1, 'TwitterMessages' , 'edit'  , null, null, 'SignIn'              , null, 'TwitterMessages.LBL_BUTTON_SIGNIN'           , 'TwitterMessages.LBL_BUTTON_SIGNIN'           , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'TwitterMessages.ImportView'      , 2, 'TwitterMessages' , 'edit'  , null, null, 'SignOut'             , null, 'TwitterMessages.LBL_BUTTON_SIGNOUT'          , 'TwitterMessages.LBL_BUTTON_SIGNOUT'          , null, null, null;
	exec dbo.spDYNAMIC_BUTTONS_InsButton    'TwitterMessages.ImportView'      , 3, 'TwitterMessages' , 'edit'  , null, null, 'Import'              , null, '.LBL_IMPORT'                                 , '.LBL_IMPORT'                                 , null, null, null;
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

call dbo.spDYNAMIC_BUTTONS_ListView()
/

call dbo.spSqlDropProcedure('spDYNAMIC_BUTTONS_ListView')
/

-- #endif IBM_DB2 */


