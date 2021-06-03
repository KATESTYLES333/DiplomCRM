

print 'LANGUAGES.2';
GO

-- 05/19/2008 Paul.  Unicode strings must be marked as such, otherwise unicode will go in as ???.
-- 05/20/2008 Paul.  The configuration wizard allows languages to be enabled, so we can default to disabling these.
-- 04/06/2010 Paul.  Add support for Farsi/Persian. 
-- 08/01/2013 Paul.  We are using Microsoft Translator instead of Google, so the supported languages have changed. 
-- http://msdn.microsoft.com/en-us/library/hh456380.aspx
exec dbo.spLANGUAGES_InsertOnly N'ar-SA'     ,  1025, 0, N'??????? (??????? ??????? ????????)', N'Arabic (Saudi Arabia)';
exec dbo.spLANGUAGES_InsertOnly N'bg-BG'     ,  1026, 0, N'????????? (????????)', N'Bulgarian (Bulgaria)';
exec dbo.spLANGUAGES_InsertOnly N'ca-ES'     ,  1027, 0, N'catal� (catal�)', N'Catalan (Catalan)';
exec dbo.spLANGUAGES_InsertOnly N'zh-TW'     ,  1028, 0, N'??(??) (??)', N'Chinese (Taiwan)';
exec dbo.spLANGUAGES_InsertOnly N'zh-CN'     ,  2052, 0, N'??(??) (???????)', N'Chinese (People''s Republic of China)';
exec dbo.spLANGUAGES_InsertOnly N'cs-CZ'     ,  1029, 0, N'ce�tina (Cesk� republika)', N'Czech (Czech Republic)';
exec dbo.spLANGUAGES_InsertOnly N'da-DK'     ,  1030, 0, N'dansk (Danmark)', N'Danish (Denmark)';
exec dbo.spLANGUAGES_InsertOnly N'nl-NL'     ,  1043, 0, N'Nederlands (Nederland)', N'Dutch (Netherlands)';
exec dbo.spLANGUAGES_InsertOnly N'en-US'     ,  1033, 1, N'English (United States)', N'English (United States)';
exec dbo.spLANGUAGES_InsertOnly N'en-AU'     ,  3081, 1, N'English (Australia)', N'English (Australia)';
exec dbo.spLANGUAGES_InsertOnly N'en-CA'     ,  4105, 1, N'English (Canada)', N'English (Canada)';
exec dbo.spLANGUAGES_InsertOnly N'en-GB'     ,  2057, 1, N'English (United Kingdom)', N'English (United Kingdom)';
exec dbo.spLANGUAGES_InsertOnly N'et-EE'     ,  1061, 0, N'eesti (Eesti)', N'Estonian (Estonia)';
exec dbo.spLANGUAGES_InsertOnly N'fa-IR'     ,  1065, 0, N'????? (?????)', N'Persian (Iran)';
exec dbo.spLANGUAGES_InsertOnly N'fi-FI'     ,  1035, 0, N'suomi (Suomi)', N'Finnish (Finland)';
exec dbo.spLANGUAGES_InsertOnly N'fr-FR'     ,  1036, 0, N'fran�ais (France)', N'French (France)';
exec dbo.spLANGUAGES_InsertOnly N'de-DE'     ,  1031, 0, N'Deutsch (Deutschland)', N'German (Germany)';
exec dbo.spLANGUAGES_InsertOnly N'de-CH'     ,  2055, 0, N'Deutsch (Schweiz)', N'German (Switzerland)';
exec dbo.spLANGUAGES_InsertOnly N'de-AT'     ,  3079, 0, N'Deutsch (�sterreich)', N'German (Austria)';
exec dbo.spLANGUAGES_InsertOnly N'el-GR'     ,  1032, 0, N'e??????? (????da)', N'Greek (Greece)';
exec dbo.spLANGUAGES_InsertOnly N'he-IL'     ,  1037, 0, N'????? (?????)', N'Hebrew (Israel)';
exec dbo.spLANGUAGES_InsertOnly N'hi-IN'     ,  1081, 0, N'????? (????)', N'Hindi (India)';
exec dbo.spLANGUAGES_InsertOnly N'hu-HU'     ,  1038, 0, N'Magyar (Magyarorsz�g)', N'Hungarian (Hungary)';
exec dbo.spLANGUAGES_InsertOnly N'id-ID'     ,  1057, 0, N'Bahasa Indonesia (Indonesia)', N'Indonesian (Indonesia)';
exec dbo.spLANGUAGES_InsertOnly N'it-IT'     ,  1040, 0, N'italiano (Italia)', N'Italian (Italy)';
exec dbo.spLANGUAGES_InsertOnly N'ja-JP'     ,  1041, 0, N'??? (??)', N'Japanese (Japan)';
exec dbo.spLANGUAGES_InsertOnly N'ko-KR'     ,  1042, 0, N'??? (????)', N'Korean (Korea)';
exec dbo.spLANGUAGES_InsertOnly N'lv-LV'     ,  1062, 0, N'latvie�u (Latvija)', N'Latvian (Latvia)';
exec dbo.spLANGUAGES_InsertOnly N'lt-LT'     ,  1063, 0, N'lietuviu (Lietuva)', N'Lithuanian (Lithuania)';
exec dbo.spLANGUAGES_InsertOnly N'ms-MY'     ,  1086, 0, N'Bahasa Malaysia (Malaysia)', 'Malay (Malaysia)';
exec dbo.spLANGUAGES_InsertOnly N'nb-NO'     ,  1044, 0, N'norsk (bokm�l) (Norge)', N'Norwegian (Bokm�l) (Norway)';
exec dbo.spLANGUAGES_InsertOnly N'nn-NO'     ,  2068, 0, N'norsk (nynorsk) (Noreg)', N'Norwegian (Nynorsk) (Norway)';
exec dbo.spLANGUAGES_InsertOnly N'pl-PL'     ,  1045, 0, N'polski (Polska)', N'Polish (Poland)';
exec dbo.spLANGUAGES_InsertOnly N'pt-PT'     ,  2070, 0, N'portugu�s (Portugal)', N'Portuguese (Portugal)';
exec dbo.spLANGUAGES_InsertOnly N'pt-BR'     ,  1046, 0, N'Portugu�s (Brasil)', N'Portuguese (Brazil)';
exec dbo.spLANGUAGES_InsertOnly N'ro-RO'     ,  1048, 0, N'rom�na (Rom�nia)', N'Romanian (Romania)';
exec dbo.spLANGUAGES_InsertOnly N'ru-RU'     ,  1049, 0, N'??????? (??????)', N'Russian (Russia)';
exec dbo.spLANGUAGES_InsertOnly N'sk-SK'     ,  1051, 0, N'slovencina (Slovensk� republika)', N'Slovak (Slovakia)';
exec dbo.spLANGUAGES_InsertOnly N'sl-SI'     ,  1060, 0, N'slovenski (Slovenija)', N'Slovenian (Slovenia)';
exec dbo.spLANGUAGES_InsertOnly N'es-ES'     ,  3082, 0, N'espa�ol (Espa�a)', N'Spanish (Spain)';
exec dbo.spLANGUAGES_InsertOnly N'es-VE'     ,  8202, 0, N'Espa�ol (Republica Bolivariana de Venezuela)', N'Spanish (Venezuela)';
exec dbo.spLANGUAGES_InsertOnly N'sv-SE'     ,  1053, 0, N'svenska (Sverige)', N'Swedish (Sweden)';
exec dbo.spLANGUAGES_InsertOnly N'th-TH'     ,  1054, 0, N'??? (???)', N'Thai (Thailand)';
exec dbo.spLANGUAGES_InsertOnly N'tr-TR'     ,  1055, 0, N'T�rk�e (T�rkiye)', N'Turkish (Turkey)';
exec dbo.spLANGUAGES_InsertOnly N'uk-UA'     ,  1058, 0, N'?????????? (???????)', N'Ukrainian (Ukraine)';
exec dbo.spLANGUAGES_InsertOnly N'ur-PK'     ,  1056, 0, N'????? (???????)', 'Urdu (Islamic Republic of Pakistan)';
exec dbo.spLANGUAGES_InsertOnly N'vi-VN'     ,  1066, 0, N'Ti�ng Vi�?t Nam (Vi�?t Nam)', N'Vietnamese (Viet Nam)';
GO

-- 08/01/2013 Paul.  We are using Microsoft Translator instead of Google, so the supported languages have changed. 
--exec dbo.spLANGUAGES_InsertOnly N'fil-PH'    ,  1124, 0, N'Filipino (Pilipinas)', N'Filipino (Philippines)';
--exec dbo.spLANGUAGES_InsertOnly N'gl-ES'     ,  1110, 0, N'galego (galego)', N'Galician (Galician)';
--exec dbo.spLANGUAGES_InsertOnly N'hr-HR'     ,  1050, 0, N'hrvatski (Hrvatska)', N'Croatian (Croatia)';
--exec dbo.spLANGUAGES_InsertOnly N'mt-MT'     ,  1082, 0, N'Malti (Malta)', N'Maltese (Malta)';
--exec dbo.spLANGUAGES_InsertOnly N'sq-AL'     ,  1052, 0, N'shqipe (Shqip�ria)', N'Albanian (Albania)';
--exec dbo.spLANGUAGES_InsertOnly N'sr-Latn-CS',  2074, 0, N'srpski (Srbija)', N'Serbian (Latin, Serbia)';
--exec dbo.spLANGUAGES_InsertOnly N'sr-Cyrl-CS',  3098, 0, N'?????? (??????)', N'Serbian (Cyrillic, Serbia)';
--exec dbo.spLANGUAGES_InsertOnly N'sr-Latn-BA',  6170, 0, N'srpski (Bosna i Hercegovina)', N'Serbian (Latin, Bosnia and Herzegovina)';
--exec dbo.spLANGUAGES_InsertOnly N'sr-Cyrl-BA',  7194, 0, N'?????? (????? ? ???????????)', N'Serbian (Cyrillic, Bosnia and Herzegovina)';
if exists(select * from vwLANGUAGES where NAME = 'fil-PH') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'fil-PH') begin -- then
		print 'LANGUAGES: Deleting fil-PH';
		exec dbo.spLANGUAGES_Delete null, 'fil-PH';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'gl-ES') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'gl-ES') begin -- then
		print 'LANGUAGES: Deleting gl-ES';
		exec dbo.spLANGUAGES_Delete null, 'gl-ES';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'hr-HR') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'hr-HR') begin -- then
		print 'LANGUAGES: Deleting hr-HR';
		exec dbo.spLANGUAGES_Delete null, 'hr-HR';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'mt-MT') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'mt-MT') begin -- then
		print 'LANGUAGES: Deleting mt-MT';
		exec dbo.spLANGUAGES_Delete null, 'mt-MT';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'sq-AL') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'sq-AL') begin -- then
		print 'LANGUAGES: Deleting sq-AL';
		exec dbo.spLANGUAGES_Delete null, 'sq-AL';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'sr-Latn-CS') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'sr-Latn-CS') begin -- then
		print 'LANGUAGES: Deleting sr-Latn-CS';
		exec dbo.spLANGUAGES_Delete null, 'sr-Latn-CS';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'sr-Cyrl-CS') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'sr-Cyrl-CS') begin -- then
		print 'LANGUAGES: Deleting sr-Cyrl-CS';
		exec dbo.spLANGUAGES_Delete null, 'sr-Cyrl-CS';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'sr-Latn-BA') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'sr-Latn-BA') begin -- then
		print 'LANGUAGES: Deleting sr-Latn-BA';
		exec dbo.spLANGUAGES_Delete null, 'sr-Latn-BA';
	end -- if;
end -- if;
if exists(select * from vwLANGUAGES where NAME = 'sr-Cyrl-BA') begin -- then
	if not exists(select * from vwTERMINOLOGY where LANG = 'sr-Cyrl-BA') begin -- then
		print 'LANGUAGES: Deleting sr-Cyrl-BA';
		exec dbo.spLANGUAGES_Delete null, 'sr-Cyrl-BA';
	end -- if;
end -- if;
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

call dbo.spLANGUAGES_Defaults()
/

call dbo.spSqlDropProcedure('spLANGUAGES_Defaults')
/

-- #endif IBM_DB2 */


