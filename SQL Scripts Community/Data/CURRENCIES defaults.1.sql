

print 'CURRENCIES defaults';
GO

set nocount on;
GO

-- http://www.id3.org/iso4217.html
exec dbo.spCURRENCIES_InsertOnly 'E340202E-6291-4071-B327-A34CB4DF239B', null, 'U.S. Dollar', '$', 'USD', 1.0, null;
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

call dbo.spCURRENCIES_Defaults()
/

call dbo.spSqlDropProcedure('spCURRENCIES_Defaults')
/

-- #endif IBM_DB2 */


