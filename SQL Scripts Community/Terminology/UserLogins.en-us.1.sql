

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:42 AM.
print 'TERMINOLOGY UserLogins en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ASPNET_SESSIONID'                          , N'en-US', N'UserLogins', null, null, N'ASP.NET Session ID:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_ASPNET_SESSIONID'                     , N'en-US', N'UserLogins', null, null, N'ASP.NET Session ID';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_LOGIN_DATE'                           , N'en-US', N'UserLogins', null, null, N'Login Date';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_LOGIN_STATUS'                         , N'en-US', N'UserLogins', null, null, N'Login Status';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_LOGIN_TYPE'                           , N'en-US', N'UserLogins', null, null, N'Login Type';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_LOGOUT_DATE'                          , N'en-US', N'UserLogins', null, null, N'Logout Date';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_RELATIVE_PATH'                        , N'en-US', N'UserLogins', null, null, N'Relative Path';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_REMOTE_HOST'                          , N'en-US', N'UserLogins', null, null, N'Remote Host';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_SERVER_HOST'                          , N'en-US', N'UserLogins', null, null, N'Server Host';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_TARGET'                               , N'en-US', N'UserLogins', null, null, N'Target';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_USER_AGENT'                           , N'en-US', N'UserLogins', null, null, N'User Agent';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_USER_ID'                              , N'en-US', N'UserLogins', null, null, N'User ID';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_USER_NAME'                            , N'en-US', N'UserLogins', null, null, N'User Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LOGIN_DATE'                                , N'en-US', N'UserLogins', null, null, N'Login Date:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LOGIN_STATUS'                              , N'en-US', N'UserLogins', null, null, N'Login Status:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LOGIN_TYPE'                                , N'en-US', N'UserLogins', null, null, N'Login Type:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LOGOUT_DATE'                               , N'en-US', N'UserLogins', null, null, N'Logout Date:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_RELATIVE_PATH'                             , N'en-US', N'UserLogins', null, null, N'Relative Path:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_REMOTE_HOST'                               , N'en-US', N'UserLogins', null, null, N'Remote Host:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SERVER_HOST'                               , N'en-US', N'UserLogins', null, null, N'Server Host:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_TARGET'                                    , N'en-US', N'UserLogins', null, null, N'Target:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_USER_AGENT'                                , N'en-US', N'UserLogins', null, null, N'User Agent:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_USER_ID'                                   , N'en-US', N'UserLogins', null, null, N'User ID:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_USER_NAME'                                 , N'en-US', N'UserLogins', null, null, N'User Name:';
GO
/* -- #if Oracle
	COMMIT WORK;
END;
/

BEGIN
-- #endif Oracle */

exec dbo.spTERMINOLOGY_InsertOnly N'Succeeded'                                     , N'en-US', null, N'login_status_dom'                  ,   1, N'Succeeded';
exec dbo.spTERMINOLOGY_InsertOnly N'Failed'                                        , N'en-US', null, N'login_status_dom'                  ,   2, N'Failed';

exec dbo.spTERMINOLOGY_InsertOnly N'Windows'                                       , N'en-US', null, N'login_type_dom'                    ,   1, N'Windows';
exec dbo.spTERMINOLOGY_InsertOnly N'Anonymous'                                     , N'en-US', null, N'login_type_dom'                    ,   2, N'Anonymous';
-- 08/07/2013 Paul.  Should have added Impersonate when feature was added. 
exec dbo.spTERMINOLOGY_InsertOnly N'Impersonate'                                   , N'en-US', null, N'login_type_dom'                    ,   3, N'Impersonate';

GO


set nocount off;
GO

/* -- #if Oracle
	COMMIT WORK;
END;
/
-- #endif Oracle */

/* -- #if IBM_DB2
	commit;
  end
/

call dbo.spTERMINOLOGY_UserLogins_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_UserLogins_en_us')
/
-- #endif IBM_DB2 */

