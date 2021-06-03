

print 'CHAT_CHANNELS defaults';

set nocount on;
GO

if not exists(select * from CHAT_CHANNELS where (ID = '67FD30A9-3336-4C36-91AC-CE0D4E51B994' or NAME = 'General') and DELETED = 0) begin -- then
	print 'CHAT_CHANNELS General';
	exec dbo.spCHAT_CHANNELS_Update '67FD30A9-3336-4C36-91AC-CE0D4E51B994', null, null, N'General', null, null, '17BB7135-2B95-42DC-85DE-842CAFF927A0', null;
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

call dbo.spCHAT_CHANNELS_Defaults()
/

call dbo.spSqlDropProcedure('spCHAT_CHANNELS_Defaults')
/

-- #endif IBM_DB2 */



