

print 'GRIDVIEWS_COLUMNS RulesWizard';
GO

set nocount on;
GO

-- delete from GRIDVIEWS_COLUMNS where GRID_NAME = 'RulesWizard.ListView';
if not exists(select * from GRIDVIEWS_COLUMNS where GRID_NAME = 'RulesWizard.ListView' and DELETED = 0) begin -- then
	print 'GRIDVIEWS_COLUMNS RulesWizard.ListView';
	exec dbo.spGRIDVIEWS_InsertOnly           'RulesWizard.ListView', 'RulesWizard', 'vwREPORTS_List';
	exec dbo.spGRIDVIEWS_COLUMNS_InsHyperLink 'RulesWizard.ListView'       , 2, 'Rules.LBL_LIST_NAME'                      , 'NAME'                 , 'NAME'            , '40%', 'listViewTdLinkS1', 'ID'         , '~/RulesWizard/edit.aspx?id={0}', null, 'RulesWizard', 'ASSIGNED_USER_ID';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'RulesWizard.ListView'       , 3, '.LBL_LIST_DATE_MODIFIED'                  , 'DATE_MODIFIED'        , 'DATE_MODIFIED'   , '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'RulesWizard.ListView'       , 4, '.LBL_LIST_ASSIGNED_USER'                  , 'ASSIGNED_TO_NAME'     , 'ASSIGNED_TO_NAME', '15%';
	exec dbo.spGRIDVIEWS_COLUMNS_InsBound     'RulesWizard.ListView'       , 5, 'Teams.LBL_LIST_TEAM'                      , 'TEAM_NAME'            , 'TEAM_NAME'       , '15%';
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

call dbo.spGRIDVIEWS_COLUMNS_RulesWizard()
/

call dbo.spSqlDropProcedure('spGRIDVIEWS_COLUMNS_RulesWizard')
/

-- #endif IBM_DB2 */


