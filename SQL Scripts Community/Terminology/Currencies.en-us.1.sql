

-- Terminology generated from database [SplendidCRM5_50] on 11/18/2010 1:19:35 AM.
print 'TERMINOLOGY Currencies en-us';
GO

set nocount on;
GO

exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CONVERSION_RATE'                           , N'en-US', N'Currencies', null, null, N'Conversion Rate:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_CURRENCY'                                  , N'en-US', N'Currencies', null, null, N'Currency';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_ISO4217'                                   , N'en-US', N'Currencies', null, null, N'ISO 4217 Code:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_CONVERSION_RATE'                      , N'en-US', N'Currencies', null, null, N'Conversion Rate';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NEW_FORM_TITLE'                            , N'en-US', N'Currencies', null, null, N'Create Currency';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_FORM_TITLE'                           , N'en-US', N'Currencies', null, null, N'Currencies';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_ISO4217'                              , N'en-US', N'Currencies', null, null, N'ISO 4217 Code';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_NAME'                                 , N'en-US', N'Currencies', null, null, N'Name';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_RATE'                                 , N'en-US', N'Currencies', null, null, N'Rate';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_STATUS'                               , N'en-US', N'Currencies', null, null, N'Status';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_LIST_SYMBOL'                               , N'en-US', N'Currencies', null, null, N'Symbol';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_MODULE_NAME'                               , N'en-US', N'Currencies', null, null, N'Currencies';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_NAME'                                      , N'en-US', N'Currencies', null, null, N'Name:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_STATUS'                                    , N'en-US', N'Currencies', null, null, N'Status:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_SYMBOL'                                    , N'en-US', N'Currencies', null, null, N'Symbol:';
exec dbo.spTERMINOLOGY_InsertOnly N'LBL_US_DOLLAR'                                 , N'en-US', N'Currencies', null, null, N'US Dollar:';
GO

exec dbo.spTERMINOLOGY_InsertOnly N'Currencies'                                    , N'en-US', null, N'moduleList'                        ,  62, N'Currencies';

-- 10/17/2013 Paul.  currency_status_dom was inadvertantly deleted a while ago. 
exec dbo.spTERMINOLOGY_InsertOnly N'Active'                                        , N'en-US', null, N'currency_status_dom'               ,   1, N'Active';
exec dbo.spTERMINOLOGY_InsertOnly N'Inactive'                                      , N'en-US', null, N'currency_status_dom'               ,   2, N'Inactive';
GO

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

call dbo.spTERMINOLOGY_Currencies_en_us()
/

call dbo.spSqlDropProcedure('spTERMINOLOGY_Currencies_en_us')
/
-- #endif IBM_DB2 */

