

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:35 AM.
print 'TERMINOLOGY ACLRoles en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ADMIN_DELEGATION_DISABLED'                 , N'en-US', N'ACLRoles', null, null, N'Admin Delegation is Disabled';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ADMIN_DELEGATION_ENABLED'                  , N'en-US', N'ACLRoles', null, null, N'Admin Delegation is Enabled';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CREATE_ROLE'                               , N'en-US', N'ACLRoles', null, null, N'Create Role';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DESCRIPTION'                               , N'en-US', N'ACLRoles', null, null, N'Description:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_DISABLE_ADMIN_DELEGATION'                  , N'en-US', N'ACLRoles', null, null, N'Disable Delegation';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_EDIT_VIEW_DIRECTIONS'                      , N'en-US', N'ACLRoles', null, null, N'Double-click to change.';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ENABLE_ADMIN_DELEGATION'                   , N'en-US', N'ACLRoles', null, null, N'Enable Delegation';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_FIELD_SECURITY'                            , N'en-US', N'ACLRoles', null, null, N'Field Security';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_COLUMN_NAME'                          , N'en-US', N'ACLRoles', null, null, N'Column Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_DESCRIPTION'                          , N'en-US', N'ACLRoles', null, null, N'Description';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FIELD_NAME'                           , N'en-US', N'ACLRoles', null, null, N'Field Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_MODULE_NAME'                          , N'en-US', N'ACLRoles', null, null, N'Module Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_NAME'                                 , N'en-US', N'ACLRoles', null, null, N'Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_TABLE_NAME'                           , N'en-US', N'ACLRoles', null, null, N'Table Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_VIEW_NAME'                            , N'en-US', N'ACLRoles', null, null, N'View Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NAME'                                      , N'en-US', N'ACLRoles', null, null, N'Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ROLE'                                      , N'en-US', N'ACLRoles', null, null, N'Role';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SEARCH_FORM_TITLE'                         , N'en-US', N'ACLRoles', null, null, N'Search';
exec dbo.spTERMINOLOGY_InsertOnly N'LIST_ROLES'                                    , N'en-US', N'ACLRoles', null, null, N'List Roles';
exec dbo.spTERMINOLOGY_InsertOnly N'LIST_ROLES_BY_USER'                            , N'en-US', N'ACLRoles', null, null, N'List Roles By User';
GO
/* -- #if Oracle
	COMMIT WORK;
END;
/

BEGIN
-- #endif Oracle */
exec dbo.spTERMINOLOGY_InsertOnly N'ACLRoles'                                      , N'en-US', null, N'moduleList'                        ,  80, N'ACL Roles';
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

call dbo.spTERMINOLOGY_ACLRoles_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_ACLRoles_en_us')
/
-- #endif IBM_DB2 */

