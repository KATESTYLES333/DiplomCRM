

print 'EDITVIEWS_FIELDS RulesWizard';
GO

set nocount on;
GO

-- delete from EDITVIEWS_FIELDS where EDIT_NAME = 'RulesWizard.SearchBasic';
if not exists(select * from EDITVIEWS_FIELDS where EDIT_NAME = 'RulesWizard.SearchBasic' and DELETED = 0) begin -- then
	print 'EDITVIEWS_FIELDS RulesWizard.SearchBasic';
	exec dbo.spEDITVIEWS_InsertOnly             'RulesWizard.SearchBasic' , 'RulesWizard', 'vwRULES_WIZARD', '11%', '22%', 3;
	exec dbo.spEDITVIEWS_FIELDS_InsBound        'RulesWizard.SearchBasic' ,  0, 'Rules.LBL_NAME'                           , 'NAME'                       , 0, null, 255, 25, null;
	exec dbo.spEDITVIEWS_FIELDS_InsControl      'RulesWizard.SearchBasic' ,  1, '.LBL_CURRENT_USER_FILTER'                 , 'CURRENT_USER_ONLY'          , 0, null, 'CheckBox'    , 'return ToggleUnassignedOnly();', null, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBlank        'RulesWizard.SearchBasic' ,  2, null;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'RulesWizard.SearchBasic' ,  3, 'Reports.LBL_MODULE_NAME'                  , 'MODULE_NAME'                , 0, null, 'RulesModules', null, 6;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'RulesWizard.SearchBasic' ,  4, '.LBL_ASSIGNED_TO'                         , 'ASSIGNED_USER_ID'           , 0, null, 'AssignedUser', null, 6;
	exec dbo.spEDITVIEWS_FIELDS_InsBoundList    'RulesWizard.SearchBasic' ,  5, 'Teams.LBL_TEAM'                           , 'TEAM_ID'                    , 0, null, 'Teams'       , null, 6;
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

call dbo.spEDITVIEWS_FIELDS_RulesWizard()
/

call dbo.spSqlDropProcedure('spEDITVIEWS_FIELDS_RulesWizard')
/

-- #endif IBM_DB2 */


