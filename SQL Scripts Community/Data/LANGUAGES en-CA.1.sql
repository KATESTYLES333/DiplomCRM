

print 'LANGUAGES en-CA';
GO

-- 05/19/2008 Paul.  Unicode strings must be marked as such, otherwise unicode will go in as ???.
exec dbo.spLANGUAGES_InsertOnly N'en-CA'     ,  4105, 1, N'English (Canada)', N'English (Canada)';
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

call dbo.spLANGUAGES_en_CA()
/

call dbo.spSqlDropProcedure('spLANGUAGES_en_CA')
/

-- #endif IBM_DB2 */


