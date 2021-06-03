/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2015 SplendidCRM Software, Inc. All rights reserved.
 * 
 * This program is free software: you can redistribute it and/or modify it under the terms of the 
 * GNU Affero General Public License as published by the Free Software Foundation, either version 3 
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
 * See the GNU Affero General Public License for more details.
 * 
 * You should have received a copy of the GNU Affero General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>. 
 * 
 * You can contact SplendidCRM Software, Inc. at email address support@splendidcrm.com. 
 * 
 * In accordance with Section 7(b) of the GNU Affero General Public License version 3, 
 * the Appropriate Legal Notices must display the following words on all interactive user interfaces: 
 * "Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved."
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Caching;
using System.Diagnostics;

namespace SplendidCRM
{
	// 12/24/2007 Paul.  Use an array to define the custom caches so that list is in the Cache module. 
	// This should reduce the number of times that we have to edit the SplendidDynamic module. 
	public delegate DataTable SplendidCacheCallback();

	public class SplendidCacheReference
	{
		private string                m_sName          ;
		private string                m_sDataValueField;
		private string                m_sDataTextField ;
		private SplendidCacheCallback m_fnDataSource   ;

		public string Name
		{
			get { return m_sName; }
		}

		public string DataValueField
		{
			get { return m_sDataValueField; }
		}

		public string DataTextField
		{
			get { return m_sDataTextField; }
		}

		public SplendidCacheCallback DataSource
		{
			get { return m_fnDataSource; }
			set { m_fnDataSource = value; }
		}

		public SplendidCacheReference(string sName, string sDataValueField, string sDataTextField, SplendidCacheCallback fnDataSource)
		{
			m_sName           = sName          ;
			m_sDataValueField = sDataValueField;
			m_sDataTextField  = sDataTextField ;
			m_fnDataSource    = fnDataSource   ;
		}
	}

	/// <summary>
	/// Summary description for SplendidCache.
	/// </summary>
	// 08/23/2015 Paul  Partial class so that we can add to it in separate files. 
	public partial class SplendidCache
	{
		// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
		public static List<SplendidCacheReference> CustomCaches = new List<SplendidCacheReference>
			{ new SplendidCacheReference("AssignedUser"      , "ID"         , "USER_NAME"   , new SplendidCacheCallback(SplendidCache.AssignedUser      ))
			// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
			, new SplendidCacheReference("AssignedTo"        , "USER_NAME"  , "USER_NAME"   , new SplendidCacheCallback(SplendidCache.AssignedUser      ))
			, new SplendidCacheReference("Currencies"        , "ID"         , "NAME_SYMBOL" , new SplendidCacheCallback(SplendidCache.Currencies        ))
			, new SplendidCacheReference("Release"           , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Release           ))
			, new SplendidCacheReference("Manufacturers"     , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Manufacturers     ))
			// 08/13/2010 Paul.  Add discounts to line items. 
			, new SplendidCacheReference("Discounts"         , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Discounts         ))
			, new SplendidCacheReference("Shippers"          , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Shippers          ))
			// 02/15/2015 Paul.  Change from terminology payment_types_dom to PaymentTypes list for QuickBooks Online. 
			, new SplendidCacheReference("PaymentTypes"      , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.PaymentTypes      ))
			// 02/27/2015 Paul.  Change from terminology payment_terms_dom to PaymentTerms list for QuickBooks Online. 
			, new SplendidCacheReference("PaymentTerms"      , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.PaymentTerms      ))
			// 12/21/2010 Paul.  Allow regions to be used in a list. 
			, new SplendidCacheReference("Regions"           , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Regions           ))
			, new SplendidCacheReference("ProductTypes"      , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.ProductTypes      ))
			, new SplendidCacheReference("ProductCategories" , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.ProductCategories ))
			, new SplendidCacheReference("ContractTypes"     , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.ContractTypes     ))
			, new SplendidCacheReference("ForumTopics"       , "NAME"       , "NAME"        , new SplendidCacheCallback(SplendidCache.ForumTopics       ))  // 07/15/2007 Paul.  Add Forum Topics to the list of possible dropdowns. 
			// 09/03/2008 Paul.  Not sure why the text was set to MODULE_NAME, but it should be DISPLAY_NAME. 
			, new SplendidCacheReference("Modules"           , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.Modules           ))  // 12/13/2007 Paul.  Managing shortcuts needs a dropdown of modules. 
			// 10/18/2011 Paul.  The HTML5 Offline Client needs a list of module tables. 
			, new SplendidCacheReference("ModuleTables"      , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.Modules           ))  // 12/13/2007 Paul.  Managing shortcuts needs a dropdown of modules. 
			// 11/10/2010 Paul.  Provide access to Rules Modules in SearchViews. 
			, new SplendidCacheReference("RulesModules"      , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.RulesModules      ))
			, new SplendidCacheReference("ReportingModules"  , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.ReportingModules  ))
			, new SplendidCacheReference("WorkflowModules"   , "MODULE_NAME", "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.WorkflowModules   ))
			, new SplendidCacheReference("EmailGroups"       , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.EmailGroups       ))
			, new SplendidCacheReference("InboundEmailBounce", "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.InboundEmailBounce))
			// 11/18/2008 Paul.  Teams can be used in the search panels. 
			, new SplendidCacheReference("Teams"             , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Teams             ))
			// 01/24/2010 Paul.  Place the report list in the cache so that it would be available in SearchView. 
			, new SplendidCacheReference("Reports"           , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Reports           ))
			// 09/10/2012 Paul.  Add User Signatures. 
			, new SplendidCacheReference("UserSignatures"    , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.UserSignatures    ))
			// 01/21/2013 Paul.  Allow Time Zones to be used in EditView. 
			, new SplendidCacheReference("TimeZones"         , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.TimezonesListbox  ))
			// 07/18/2013 Paul.  Add support for multiple outbound emails. 
			// 09/23/2013 Paul.  OutboundMail should use the display name field. 
			, new SplendidCacheReference("OutboundMail"      , "ID"         , "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.OutboundMail      ))
			// 09/23/2013 Paul.  Add support for multiple outbound sms. 
			, new SplendidCacheReference("OutboundSms"       , "ID"         , "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.OutboundSms       ))
			// 12/13/2013 Paul.  Allow each product to have a default tax rate. 
			// 09/23/2013 Paul.  Add support for multiple outbound sms. 
			, new SplendidCacheReference("TaxRates"          , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.TaxRates          ))
			// 12/12/2015 Paul.  /n Software and .netCharge use different lists. 
			, new SplendidCacheReference("LibraryPaymentGateways", "NAME"   , "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.LibraryPaymentGateways))
			// 12/16/2015 Paul.  credit_card_year should be a custom list that adds 10 years to current year. 
			, new SplendidCacheReference("credit_card_year"      , "NAME"   , "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.CreditCardYears       ))
			// 05/12/2016 Paul.  Add Tags module. 
			, new SplendidCacheReference("Tags"              , "ID"         , "NAME"        , new SplendidCacheCallback(SplendidCache.Tags              ))
			// 02/21/2017 Paul.  Allow langauges to be used in a list. 
			, new SplendidCacheReference("Languages"         , "NAME"       , "DISPLAY_NAME", new SplendidCacheCallback(SplendidCache.Languages         ))
			};

		// 06/02/2016 Paul.  Activities views will use new function that accepts an array of modules. 
		public static string[] arrActivityModules = new string[] {"Calls", "Meetings", "Tasks", "Emails", "Notes", "SmsMessages", "TwitterMessages", "ChatMessages"};

		// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
		public static void AddReportSource(string sName, string sDataValueField, string sDataTextField, DataTable dt)
		{
			SplendidCacheReference cacheReportList = new SplendidCacheReference(sName, sDataValueField, sDataTextField, delegate { return dt; });
			HttpRuntime.Cache.Remove("Reports.Source." + sName);
			HttpRuntime.Cache.Insert("Reports.Source." + sName, cacheReportList, null,  DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
		}

		// 08/20/2008 Paul.  Provide a central location to clear cache values based on a table change. 
		public static void ClearTable(string sTABLE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			switch ( sTABLE_NAME )
			{
				// Cached Data. 
				case "CONTRACT_TYPES"           :  Cache.Remove("vwCONTRACT_TYPES_LISTBOX"               );  break;
				case "CURRENCIES"               :  Cache.Remove("vwCURRENCIES_LISTBOX"                   );  break;
				case "FORUM_TOPICS"             :  Cache.Remove("vwFORUM_TOPICS_LISTBOX"                 );  break;
				case "FORUMS"                   :  break;
				case "INBOUND_EMAILS"           :  SplendidCache.ClearInboundEmails()                     ;  break;
				case "MANUFACTURERS"            :  Cache.Remove("vwMANUFACTURERS_LISTBOX"                );  break;
				case "PRODUCT_CATEGORIES"       :  Cache.Remove("vwPRODUCT_CATEGORIES_LISTBOX"           );  break;
				case "PRODUCT_TYPES"            :  Cache.Remove("vwPRODUCT_TYPES_LISTBOX"                );  break;
				case "RELEASES"                 :  Cache.Remove("vwRELEASES_LISTBOX"                     );  break;
				// 08/13/2010 Paul.  Add discounts to line items. 
				case "DISCOUNTS"                :  Cache.Remove("vwDISCOUNTS_LISTBOX"                    );  break;
				case "SHIPPERS"                 :  Cache.Remove("vwSHIPPERS_LISTBOX"                     );  break;
				// 12/21/2010 Paul.  Allow regions to be used in a list. 
				case "REGIONS"                  :  Cache.Remove("vwREGIONS"                              );  break;
				case "TAX_RATES"                :  Cache.Remove("vwTAX_RATES_LISTBOX"                    );  break;
				// 11/18/2008 Paul.  Teams can be used in the search panels. 
				case "TEAMS"                    :  Cache.Remove("vwTEAMS"                                );  break;
				case "USERS"                    :  SplendidCache.ClearUsers()                             ;  break;
				// Cached System Tables. 
				case "ACL_ACTIONS"              :  break;
				case "ACL_ROLES"                :  break;
				case "ACL_ROLES_ACTIONS"        :  break;
				case "ACL_ROLES_USERS"          :  break;
				case "CONFIG"                   :  break;
				case "CUSTOM_FIELDS"            :  break;
				case "DETAILVIEWS"              :  break;
				case "DETAILVIEWS_FIELDS"       :  SplendidCache.ClearSet("vwDETAILVIEWS_FIELDS."        );  break;
				case "DETAILVIEWS_RELATIONSHIPS":  SplendidCache.ClearSet("vwDETAILVIEWS_RELATIONSHIPS." );  break;
				// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
				case "EDITVIEWS_RELATIONSHIPS"  :  SplendidCache.ClearSet("vwEDITVIEWS_RELATIONSHIPS."   );  break;
				case "DYNAMIC_BUTTONS"          :  SplendidCache.ClearSet("vwDYNAMIC_BUTTONS."           );  break;
				case "EDITVIEWS"                :  break;
				case "EDITVIEWS_FIELDS"         :  SplendidCache.ClearSet("vwEDITVIEWS_FIELDS."          );  break;
				case "FIELDS_META_DATA"         :
					SplendidCache.ClearSet("vwFIELDS_META_DATA_Validated."  );
					SplendidCache.ClearSet("vwFIELDS_META_DATA_Unvalidated.");
					SplendidCache.ClearSet("vwSqlColumns_Reporting."        );
					SplendidCache.ClearSet("vwSqlColumns_Workflow."         );
					SplendidCache.ClearSet("vwSqlColumns_Searching."        );
					break;
				case "GRIDVIEWS"                :  break;
				case "GRIDVIEWS_COLUMNS"        :  SplendidCache.ClearSet("vwGRIDVIEWS_COLUMNS."         );  break;
				case "LANGUAGES"                :  SplendidCache.ClearLanguages()                         ;  break;
				case "MODULES"                  :
				{
					Cache.Remove("vwMODULES");
					Cache.Remove("vwCUSTOM_EDIT_MODULES");
					foreach(DictionaryEntry oKey in Cache)
					{
						string sKey = oKey.Key.ToString();
						// 11/02/2009 Paul.  We will need a list of modules to manage offline clients. 
						// 06/02/2016 Paul.  Stream security. 
						// 06/02/2016 Paul.  Remove . in front of vwMODULES_Access_ByUser_.  It is not language dependent. 
						if ( sKey.Contains(".vwMODULES_Reporting_") || sKey.Contains(".vwMODULES_Import_") || sKey.EndsWith(".vwMODULES_Workflow") || sKey.Contains("vwMODULES_Access_ByUser_") || sKey.Contains("vwMODULES_Stream_ByUser_") )
							Cache.Remove(sKey);
					}
					// 10/24/2009 Paul.  Clear the newly cached item module item. 
					Cache.Remove("vwMODULES_Popup");
					// 10/24/2009 Paul.  We still can't use the standard page caching otherwise we risk getting an unauthenticated page cached, which would prevent all popups. 
					//HttpResponse.RemoveOutputCacheItem("~/Include/javascript/ModulePopupScripts.aspx");
					break;
				}
				case "RELATIONSHIPS"            :
				{
					Cache.Remove("vwRELATIONSHIPS_Reporting");
					foreach(DictionaryEntry oKey in Cache)
					{
						string sKey = oKey.Key.ToString();
						if ( sKey.EndsWith(".vwRELATIONSHIPS_Workflow") )
							Cache.Remove(sKey);
					}
					break;
				}
				case "SHORTCUTS"                :  break;
				case "TERMINOLOGY"              :  Cache.Remove("vwTERMINOLOGY_PickList"); break;
				case "TERMINOLOGY_ALIASES"      :  break;
				case "TIMEZONES"                :  Cache.Remove("vwTIMEZONES");  Cache.Remove("vwTIMEZONES_LISTBOX");  break;
			}
		}

		public static void ClearSet(string sName)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				if ( sKey.StartsWith(sName) )
					Cache.Remove(sKey);
			}
		}

		// 02/16/2012 Paul.  Move custom cache logic to a method. 
		public static void SetListSource(string sCACHE_NAME, System.Web.UI.WebControls.ListControl lstField)
		{
			bool bCustomCache = false;
			// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
			List<SplendidCacheReference> arrCustomCaches = SplendidCache.CustomCaches;
			foreach ( SplendidCacheReference cache in arrCustomCaches )
			{
				if ( cache.Name == sCACHE_NAME )
				{
					lstField.DataValueField = cache.DataValueField;
					lstField.DataTextField  = cache.DataTextField ;
					SplendidCacheCallback cbkDataSource = cache.DataSource;
					lstField.DataSource     = cbkDataSource();
					bCustomCache = true;
					break;
				}
			}
			// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
			if ( !bCustomCache )
			{
				SplendidCacheReference cache = HttpRuntime.Cache.Get("Reports.Source." + sCACHE_NAME) as SplendidCacheReference;
				if ( cache != null )
				{
					lstField.DataValueField = cache.DataValueField;
					lstField.DataTextField  = cache.DataTextField ;
					SplendidCacheCallback cbkDataSource = cache.DataSource;
					lstField.DataSource     = cbkDataSource();
					bCustomCache = true;
				}
			}
			if ( !bCustomCache )
			{
				lstField.DataValueField = "NAME"        ;
				lstField.DataTextField  = "DISPLAY_NAME";
				lstField.DataSource     = SplendidCache.List(sCACHE_NAME);
			}
		}

		public static string CustomList(string sCacheName, string sValue, ref bool bCustomCache)
		{
			string sDisplayName = String.Empty;
			bCustomCache = false;
			// 10/04/2015 Paul.  Changed custom caches to a dynamic list. 
			List<SplendidCacheReference> arrCustomCaches = SplendidCache.CustomCaches;
			foreach ( SplendidCacheReference cache in arrCustomCaches )
			{
				if ( cache.Name == sCacheName )
				{
					SplendidCacheCallback cbkDataSource = cache.DataSource;
					DataView vwList = new DataView(cbkDataSource());
					vwList.RowFilter = cache.DataValueField + " = '" + Sql.EscapeSQL(sValue) + "'";
					if ( vwList.Count > 0 )
						sDisplayName = Sql.ToString(vwList[0][cache.DataTextField]);
					bCustomCache = true;
					break;
				}
			}
			return sDisplayName;
		}

		public static DateTime DefaultCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddDays(1);
#endif
		}

		public static DateTime CacheExpiration5Minutes()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddMinutes(5);
#endif
		}

		public static void ClearList(string sLanguage, string sListName)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove(sLanguage + "." + sListName);
		}

		public static DataTable List(string sListName)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;

			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + "." + sListName) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 10/13/2005 Paul.  Use distinct because the same list appears to be duplicated in various modules. 
						// appointment_filter_dom is in an Activities and a History module.
						// ORDER BY items must appear in the select list if SELECT DISTINCT is specified. 
						// 05/20/2016 Paul.  Oracle does not allow distinct on NCLOB. 
						sSQL = "select " +  (!Sql.IsOracle(con) ? "distinct" : String.Empty) + ControlChars.CrLf
						     + "       NAME                  " + ControlChars.CrLf
						     + "     , DISPLAY_NAME          " + ControlChars.CrLf
						     + "     , LIST_ORDER            " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY         " + ControlChars.CrLf
						     + " where lower(LIST_NAME) = @LIST_NAME" + ControlChars.CrLf  // 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
						     + "   and lower(LANG     ) = @LANG     " + ControlChars.CrLf  // 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
						     + " order by LIST_ORDER         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
							Sql.AddParameter(cmd, "@LIST_NAME", sListName.ToLower());
							Sql.AddParameter(cmd, "@LANG"     , L10n.NAME.ToLower());
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert(L10n.NAME + "." + sListName, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
						// 12/03/2005 Paul.  Most lists require data, so if the language-specific list does not exist, just use English. 
						if ( dt.Rows.Count == 0 )
						{
							if ( String.Compare(L10n.NAME, "en-US", true) != 0 )
							{
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
									Sql.AddParameter(cmd, "@LIST_NAME", sListName.ToLower());
									Sql.AddParameter(cmd, "@LANG"     , "en-US"  .ToLower());
							
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										dt = new DataTable();
										da.Fill(dt);
										Cache.Insert(L10n.NAME + "." + sListName, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul.  Ignore list errors. 
					// 03/30/2006 Paul.  IBM DB2 is returning an error, which is causing a data-binding error. 
					// SQL1585N A system temporary table space with sufficient page size does not exist. 
					// 03/30/2006 Paul.  In case of error, we should return NULL. 
					return null;
				}
			}
			return dt;
		}

		/*
		// 10/20/2013 Paul.  Lists are no longer module-specific. 
		public static DataTable List(string sModuleName, string sListName)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;

			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + "." + sListName) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                     " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY                    " + ControlChars.CrLf
						     + " where lower(MODULE_NAME) = @MODULE_NAME" + ControlChars.CrLf
						     + "   and lower(LIST_NAME  ) = @LIST_NAME  " + ControlChars.CrLf
						     + "   and lower(LANG       ) = @LANG       " + ControlChars.CrLf
						     + " order by LIST_ORDER                    " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
							Sql.AddParameter(cmd, "@MODULE_NAME", sModuleName.ToLower());
							Sql.AddParameter(cmd, "@LIST_NAME"  , sListName  .ToLower());
							Sql.AddParameter(cmd, "@LANG"       , L10n.NAME  .ToLower());
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert(L10n.NAME + "." + sListName, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}
		*/

		public static void ClearUsers()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwUSERS_ASSIGNED_TO");
			Cache.Remove("vwUSERS_List");
			Cache.Remove("vwUSERS_Groups");
		}

		// 01/18/2007 Paul.  If AssignedUser list, then use the cached value to find the value. 
		public static string AssignedUser(Guid gID)
		{
			string sUSER_NAME = String.Empty;
			if ( !Sql.IsEmptyGuid(gID) )
			{
				DataView vwAssignedUser = new DataView(SplendidCache.AssignedUser());
				vwAssignedUser.RowFilter = "ID = '" + gID.ToString() + "'";
				if ( vwAssignedUser.Count > 0 )
				{
					sUSER_NAME = Sql.ToString(vwAssignedUser[0]["USER_NAME"]);
				}
			}
			return sUSER_NAME;
		}

		public static DataTable AssignedUser()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 04/15/2008 Paul.  When Team Management is enabled, only show users that are in this users teams. 
			bool bTeamFilter = !Security.IS_ADMIN && Crm.Config.enable_team_management();
			string sCACHE_NAME = String.Empty;
			if ( bTeamFilter )
				sCACHE_NAME = "vwTEAMS_ASSIGNED_TO." + Security.USER_ID.ToString();
			else
				sCACHE_NAME = "vwUSERS_ASSIGNED_TO";
			DataTable dt = Cache.Get(sCACHE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						if ( bTeamFilter )
						{
							sSQL = "select ID                 " + ControlChars.CrLf
							     + "     , USER_NAME          " + ControlChars.CrLf
							     + "  from vwTEAMS_ASSIGNED_TO" + ControlChars.CrLf
							     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf
							     + " order by USER_NAME       " + ControlChars.CrLf;
						}
						else
						{
							sSQL = "select ID                 " + ControlChars.CrLf
							     + "     , USER_NAME          " + ControlChars.CrLf
							     + "  from vwUSERS_ASSIGNED_TO" + ControlChars.CrLf
							     + " order by USER_NAME       " + ControlChars.CrLf;
						}
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							if ( bTeamFilter )
								Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert(sCACHE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable CustomEditModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCUSTOM_EDIT_MODULES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                 " + ControlChars.CrLf
						     + "     , NAME as DISPLAY_NAME " + ControlChars.CrLf
						     + "  from vwCUSTOM_EDIT_MODULES" + ControlChars.CrLf
						     + " order by NAME              " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwCUSTOM_EDIT_MODULES", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 11/02/2009 Paul.  We will need a list of modules to manage offline clients. 
		public static List<String> AccessibleModules(HttpContext Context, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			List<String> arr = Cache.Get("vwMODULES_Access_ByUser_" + gUSER_ID.ToString()) as List<String>;
			if ( arr == null )
			{
				arr = new List<String>();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "  from vwMODULES_Access_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										// 11/08/2009 Paul.  We need a list that can grow dynamically as some rows will be ignored. 
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
											if ( Sql.ToBoolean(Context.Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												arr.Add(sMODULE_NAME);
											}
										}
										Cache.Insert("vwMODULES_Access_ByUser_" + gUSER_ID.ToString(), arr, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
									}
									else
									{
										// 11/08/2009 Paul.  If this is the first time an offline client user logs-in, there will be 
										// no user record and no accessible modules.  We need a default set, so requery. 
										sSQL = "select MODULE_NAME             " + ControlChars.CrLf
										     + "  from vwMODULES               " + ControlChars.CrLf
										     + " order by MODULE_NAME          " + ControlChars.CrLf;
										cmd.CommandText = sSQL;
										cmd.Parameters.Clear();
										da.Fill(dt);
										
										for ( int i = 0; i < dt.Rows.Count; i++ )
										{
											DataRow row = dt.Rows[i];
											string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
											// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
											if ( Sql.ToBoolean(Context.Application["Modules." + sMODULE_NAME + ".Exists"]) )
											{
												arr.Add(sMODULE_NAME);
											}
										}
										// 11/08/2009 Paul.  If we are using the default set of modules, then don't cache the results. 
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return arr;
		}

		// 01/24/2018 Paul.  The Calendar needs to determine if Calls module is enabled. 
		public static DataTable AccessibleModulesTable(HttpContext Context, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMODULES_AccessTable_ByUser_" + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "     , RELATIVE_PATH           " + ControlChars.CrLf
						     + "  from vwMODULES_Access_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dt);
								if ( dt.Rows.Count > 0 )
								{
									// 11/08/2009 Paul.  We need a list that can grow dynamically as some rows will be ignored. 
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
										if ( !Sql.ToBoolean(Context.Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Cache.Insert("vwMODULES_AccessTable_ByUser_" + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable ReportingModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Reporting_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/06/2009 Paul.  We need the ID and TABLE_NAME when generating the SemanticModel for the ReportBuilder. 
						// 07/23/2010 Paul.  Make sure to sort the Modules table. 
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Reporting     " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf
						     + " order by DISPLAY_NAME         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Reporting_" + Security.USER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 11/10/2010 Paul.  vwMODULES_Rules is nearly identical to vwMODULES_Reporting. 
		public static DataTable RulesModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Rules_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/06/2009 Paul.  We need the ID and TABLE_NAME when generating the SemanticModel for the ReportBuilder. 
						// 07/23/2010 Paul.  Make sure to sort the Modules table. 
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , TABLE_NAME              " + ControlChars.CrLf
						     + "     , MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Rules         " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf
						     + " order by DISPLAY_NAME         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Rules_" + Security.USER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable WorkflowModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Workflow") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Workflow      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Workflow", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable WorkflowRelationships()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwRELATIONSHIPS_Workflow") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                        " + ControlChars.CrLf
						     + "  from vwRELATIONSHIPS_Workflow" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwRELATIONSHIPS_Workflow", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable ImportModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 08/06/2008 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Import_" + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Import        " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + "    or USER_ID is null         " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Import_" + Security.USER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static string[] ReportingModulesList()
		{
			// 07/23/2010 Paul.  Make sure to sort the Modules table. 
			DataView vw = new DataView(SplendidCache.ReportingModules());
			vw.Sort = "DISPLAY_NAME";
			string[] arr = new string[vw.Count];
			for ( int i = 0; i < vw.Count; i++ )
			{
				arr[i] = Sql.ToString(vw[i]["MODULE_NAME"]);
			}
			return arr;
		}

		public static DataTable ReportingRelationships()
		{
			return ReportingRelationships(HttpContext.Current.Application);
		}

		public static DataTable ReportingRelationships(HttpApplicationState Application)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwRELATIONSHIPS_Reporting") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                        " + ControlChars.CrLf
						     + "  from vwRELATIONSHIPS_Reporting" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwRELATIONSHIPS_Reporting", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearFilterColumns(string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwSqlColumns_Reporting." + sMODULE_NAME);
			Cache.Remove("vwSqlColumns_Workflow."  + sMODULE_NAME);
		}

		public static DataTable ReportingFilterColumns(string sMODULE_NAME)
		{
			return ReportingFilterColumns(HttpContext.Current.Application, sMODULE_NAME);
		}

		// 10/20/2009 Paul.  We need to allow the ReportingFilterColumns to be called from a background task. 
		public static DataTable ReportingFilterColumns(HttpApplicationState Application, string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns_Reporting." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Reporting  " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Insert("vwSqlColumns_Reporting." + sMODULE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable WorkflowFilterColumns(string sMODULE_NAME)
		{
			return WorkflowFilterColumns(HttpContext.Current.Application, sMODULE_NAME);
		}

		// 06/03/2009 Paul.  This function can be call from the workflow engine, so we need to pass in the application. 
		// 07/23/2008 Paul.  Use a separate view for workflow so that we can filter the audting fields. 
		public static DataTable WorkflowFilterColumns(HttpApplicationState Application, string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns_Workflow." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Workflow   " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 07/22/2008 Paul.  There are no views for the audit tables. 
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							// 11/26/2008 Paul.  We now have audit views. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Insert("vwSqlColumns_Workflow." + sMODULE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable WorkflowFilterUpdateColumns(string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns_WorkflowUpdate." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						// 02/18/2009 Paul.  Include the custom fields in the list of workflow columns that can be updated. 
						// 02/18/2009 Paul.  We need to know if the column is an identity so the workflow engine can avoid updating it.
						sSQL = "select ObjectName                                      " + ControlChars.CrLf
						     + "     , ColumnName                                      " + ControlChars.CrLf
						     + "     , ColumnType                                      " + ControlChars.CrLf
						     + "     , NAME                                            " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                                    " + ControlChars.CrLf
						     + "     , SqlDbType                                       " + ControlChars.CrLf
						     + "     , CsType                                          " + ControlChars.CrLf
						     + "     , colid                                           " + ControlChars.CrLf
						     + "     , IsIdentity                                      " + ControlChars.CrLf
						     + "  from vwSqlColumns_WorkflowUpdate                     " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME                        " + ControlChars.CrLf
						     + "union all                                              " + ControlChars.CrLf
						     + "select ObjectName                                      " + ControlChars.CrLf
						     + "     , ColumnName                                      " + ControlChars.CrLf
						     + "     , ColumnType                                      " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.NAME                " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                                    " + ControlChars.CrLf
						     + "     , SqlDbType                                       " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.CsType              " + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.colid + 100 as colid" + ControlChars.CrLf
						     + "     , vwSqlColumns_WorkflowUpdate.IsIdentity          " + ControlChars.CrLf
						     + "  from      vwFIELDS_META_DATA_Validated               " + ControlChars.CrLf
						     + " inner join vwSqlColumns_WorkflowUpdate                " + ControlChars.CrLf
						     + "         on vwSqlColumns_WorkflowUpdate.ObjectName = vwFIELDS_META_DATA_Validated.TABLE_NAME + '_CSTM'" + ControlChars.CrLf
						     + "        and vwSqlColumns_WorkflowUpdate.NAME       = vwFIELDS_META_DATA_Validated.NAME" + ControlChars.CrLf
						     + " where vwFIELDS_META_DATA_Validated.MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf
						     + " order by colid                                        " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 08/13/2008 Paul.  When updating inside the workflow engine, we can only update the core fields at this time. 
							// So make sure to match the fields with the update procedure. 
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + sMODULE_NAME + "_Update"));
							Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Insert("vwSqlColumns_WorkflowUpdate." + sMODULE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static string ReportingFilterColumnsListName(string sMODULE_NAME, string sDATA_FIELD)
		{
			return ReportingFilterColumnsListName(HttpContext.Current.Application, sMODULE_NAME, sDATA_FIELD);
		}

		// 06/03/2009 Paul.  This function can be call from the workflow engine, so we need to pass in the application. 
		public static string ReportingFilterColumnsListName(HttpApplicationState Application, string sMODULE_NAME, string sDATA_FIELD)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sLIST_NAME = Cache.Get("vwSqlColumns_ListName." + sMODULE_NAME + "." + sDATA_FIELD) as string;
			if ( sLIST_NAME == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select LIST_NAME               " + ControlChars.CrLf
						     + "  from vwSqlColumns_ListName   " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + "   and DATA_FIELD = @DATA_FIELD" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sMODULE_NAME));
							Sql.AddParameter(cmd, "@DATA_FIELD", sDATA_FIELD);
							// 07/16/2008 Paul.  Don't need the data adapter. 
							sLIST_NAME = Sql.ToString(cmd.ExecuteScalar());
							Cache.Insert("vwSqlColumns_ListName." + sMODULE_NAME + "." + sDATA_FIELD, sLIST_NAME, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return sLIST_NAME;
		}

		public static DataTable ImportColumns(string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns_Import." + sMODULE_NAME) as DataTable;
			if ( dt == null )
			{
				string sTABLE_NAME = Sql.ToString(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".TableName"]);
				if ( Sql.IsEmptyString(sTABLE_NAME) )
					sTABLE_NAME = sMODULE_NAME.ToUpper();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						/*
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Import     " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + sMODULE_NAME + "_Update"));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Insert("vwSqlColumns_Import." + sMODULE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
						*/
						// 10/08/2006 Paul.  MySQL does not seem to have a way to provide the paramters. Use the SqlProcs.Factory to build the table. 
						dt = new DataTable();
						dt.Columns.Add("ColumnName"  , Type.GetType("System.String"));
						dt.Columns.Add("NAME"        , Type.GetType("System.String"));
						dt.Columns.Add("DISPLAY_NAME", Type.GetType("System.String"));
						dt.Columns.Add("ColumnType"  , Type.GetType("System.String"));
						dt.Columns.Add("Size"        , Type.GetType("System.Int32" ));
						dt.Columns.Add("Scale"       , Type.GetType("System.Int32" ));
						dt.Columns.Add("Precision"   , Type.GetType("System.Int32" ));
						dt.Columns.Add("colid"       , Type.GetType("System.Int32" ));
						dt.Columns.Add("CustomField" , Type.GetType("System.Boolean"));
						{
							IDbCommand cmdImport = null;
							try
							{
								// 03/13/2008 Paul.  The factory will throw an exception if the procedure is not found. 
								// Catching an exception is expensive, but trivial considering all the other processing that will occur. 
								// We need this same logic in ImportView.GenerateImport. 
								cmdImport = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Import");
							}
							catch
							{
								cmdImport = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
							}
							for ( int i =0; i < cmdImport.Parameters.Count; i++ )
							{
								IDbDataParameter par = cmdImport.Parameters[i] as IDbDataParameter;
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								row["ColumnName"  ] = par.ParameterName;
								row["NAME"        ] = Sql.ExtractDbName(cmdImport, par.ParameterName);
								row["DISPLAY_NAME"] = row["NAME"];
								row["ColumnType"  ] = par.DbType.ToString();
								row["Size"        ] = par.Size         ;
								row["Scale"       ] = par.Scale        ;
								row["Precision"   ] = par.Precision    ;
								row["colid"       ] = i                ;
								row["CustomField" ] = false            ;
							}
						}

						// 09/19/2007 Paul.  Add the fields from the custom table. 
						// Exclude ID_C as it is expect and required. We don't want it to appear in the mapping table. 
						con.Open();
						string sSQL;
						// 09/19/2007 Paul.  We need to allow import into TEAM fields. 
						// 12/13/2007 Paul.  Only add the TEAM fields if the base table has a TEAM_ID field. 
						if ( Crm.Config.enable_team_management() )
						{
							// 02/20/2008 Paul.  We have a new way to determine if a module has a TEAM_ID. 
							// Also, the update procedure already includes TEAM_ID, so we only have to add the TEAM_NAME. 
							bool bModuleIsTeamed = Sql.ToBoolean(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".Teamed"]);
							DataRow row = dt.NewRow();
							row = dt.NewRow();
							row["ColumnName"  ] = "TEAM_NAME";
							row["NAME"        ] = "TEAM_NAME";
							row["DISPLAY_NAME"] = "TEAM_NAME";
							row["ColumnType"  ] = "string";
							row["Size"        ] = 128;
							row["colid"       ] = dt.Rows.Count;
							row["CustomField" ] = false;
							dt.Rows.Add(row);
						}

						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
						     + "   and ColumnName <> 'ID_C'    " + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								DataTable dtCSTM = new DataTable();
								da.Fill(dtCSTM);
								foreach ( DataRow rowCSTM in dtCSTM.Rows )
								{
									DataRow row = dt.NewRow();
									row["ColumnName"  ] = Sql.ToString (rowCSTM["ColumnName"]);
									row["NAME"        ] = Sql.ToString (rowCSTM["ColumnName"]);
									row["DISPLAY_NAME"] = Sql.ToString (rowCSTM["ColumnName"]);
									row["ColumnType"  ] = Sql.ToString (rowCSTM["CsType"    ]);
									row["Size"        ] = Sql.ToInteger(rowCSTM["length"    ]);
									// 09/19/2007 Paul.  Scale and Precision are not used. 
									//row["Scale"       ] = Sql.ToInteger(rowCSTM["Scale"     ]);
									//row["Precision"   ] = Sql.ToInteger(rowCSTM["Precision" ]);
									row["colid"       ] = dt.Rows.Count;
									row["CustomField" ] = true;
									dt.Rows.Add(row);
								}
							}
						}
						Cache.Insert("vwSqlColumns_Import." + sMODULE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable Release()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwRELEASES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwRELEASES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwRELEASES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable ProductCategories()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwPRODUCT_CATEGORIES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                          " + ControlChars.CrLf
						     + "     , NAME                        " + ControlChars.CrLf
						     + "  from vwPRODUCT_CATEGORIES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER               " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwPRODUCT_CATEGORIES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable ProductTypes()
		{
			return ProductTypes(HttpContext.Current.Application);
		}

		// 05/19/2012 Paul.  ProductTypes needs to be called from the scheduler, so the application must be provided. 
		public static DataTable ProductTypes(HttpApplicationState Application)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwPRODUCT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                     " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPRODUCT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwPRODUCT_TYPES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable Manufacturers()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMANUFACTURERS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                     " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwMANUFACTURERS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwMANUFACTURERS_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 08/13/2010 Paul.  Add discounts to line items. 
		public static DataTable Discounts()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwDISCOUNTS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 08/15/2010 Paul.  We need all the discount fields. 
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwDISCOUNTS_LISTBOX" + ControlChars.CrLf
						     + " order by NAME            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwDISCOUNTS_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable Shippers()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSHIPPERS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwSHIPPERS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwSHIPPERS_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 02/15/2015 Paul.  Change from terminology payment_types_dom to PaymentTypes list for QuickBooks Online. 
		public static DataTable PaymentTypes()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwPAYMENT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/16/2015 Paul.  In order to remain compatible with existing systems, use NAME instead of ID as the primary key. 
						sSQL = "select NAME as ID             " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPAYMENT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwPAYMENT_TYPES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 02/15/2015 Paul.  Change from terminology payment_terms_dom to PaymentTerms list for QuickBooks Online. 
		public static DataTable PaymentTerms()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwPAYMENT_TERMS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/16/2015 Paul.  In order to remain compatible with existing systems, use NAME instead of ID as the primary key. 
						sSQL = "select NAME as ID             " + ControlChars.CrLf
						     + "     , NAME                   " + ControlChars.CrLf
						     + "  from vwPAYMENT_TERMS_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwPAYMENT_TERMS_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 12/21/2010 Paul.  Allow regions to be used in a list. 
		public static DataTable Regions()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwREGIONS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                " + ControlChars.CrLf
						     + "     , NAME              " + ControlChars.CrLf
						     + "  from vwREGIONS         " + ControlChars.CrLf
						     + " order by NAME           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwREGIONS", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 06/29/2014 Paul.  Add support for http://www.zip-tax.com/. 
		public static void ClearTaxRates()
		{
			HttpRuntime.Cache.Remove("vwTAX_RATES_LISTBOX");
		}

		public static DataTable TaxRates()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTAX_RATES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 03/31/2007 Paul.  We need to cache the tax rate value for quick access in Quotes and Orders. 
						// 04/07/2016 Paul.  Tax rates per team. 
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwTAX_RATES_LISTBOX" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 04/07/2016 Paul.  Tax rates per team. 
							if ( Sql.ToBoolean(HttpContext.Current.Application["CONFIG.Orders.EnableTaxRateTeams"]) )
								Security.Filter(cmd, "TaxRates", "list");
							cmd.CommandText += " order by LIST_ORDER      " + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/29/2014 Paul.  Automatically add the tax rate to the display name. 
								foreach ( DataRow row in dt.Rows )
								{
									string  sNAME  = Sql.ToString (row["NAME" ]);
									Decimal dVALUE = Sql.ToDecimal(row["VALUE"]);
									if ( !sNAME.EndsWith("%") )
									{
										string  sVALUE = dVALUE.ToString("0.000");
										// 06/29/2014 Paul.  Only use the third decimal place when necessary. 
										if ( sVALUE.EndsWith("0") )
											sVALUE = sVALUE.Substring(0, sVALUE.Length - 1);
										sNAME += "  " + sVALUE + "%";
										row["NAME"] = sNAME;
									}
								}
								Cache.Insert("vwTAX_RATES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable ContractTypes()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCONTRACT_TYPES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                      " + ControlChars.CrLf
						     + "     , NAME                    " + ControlChars.CrLf
						     + "  from vwCONTRACT_TYPES_LISTBOX" + ControlChars.CrLf
						     + " order by LIST_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwCONTRACT_TYPES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable Currencies()
		{
			return Currencies(HttpContext.Current.Application);
		}

		// 07/12/2014 Paul.  Currencies needs to be called from the scheduler, so the application must be provided. 
		public static DataTable Currencies(HttpApplicationState Application)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCURRENCIES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 05/29/2008 Paul.  ISO4217 is needed to process PayPal transactions. 
						sSQL = "select ID                  " + ControlChars.CrLf
						     + "     , NAME                " + ControlChars.CrLf
						     + "     , SYMBOL              " + ControlChars.CrLf
						     + "     , NAME_SYMBOL         " + ControlChars.CrLf
						     + "     , CONVERSION_RATE     " + ControlChars.CrLf
						     + "     , ISO4217             " + ControlChars.CrLf
						     + "  from vwCURRENCIES_LISTBOX" + ControlChars.CrLf
						     + " order by NAME             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwCURRENCIES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable Timezones()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTIMEZONES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *           " + ControlChars.CrLf
						     + "  from vwTIMEZONES " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwTIMEZONES", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable TimezonesListbox()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTIMEZONES_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                 " + ControlChars.CrLf
						     + "     , NAME               " + ControlChars.CrLf
						     + "  from vwTIMEZONES_LISTBOX" + ControlChars.CrLf
						     + " order by BIAS desc       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwTIMEZONES_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static void ClearLanguages()
		{
			// 02/18/2008 Paul.  HttpRuntime.Cache is a better and faster way to get to the cache. 
			HttpRuntime.Cache.Remove("vwLANGUAGES");
		}

		//.02/18/2008 Paul.  Languages will also be used in the UI, so provide a version without parameters. 
		public static DataTable Languages()
		{
			return Languages(HttpContext.Current.Application);
		}

		// 02/18/2008 Paul.  Languages needs to be called from the scheduler, so the application must be provided. 
		public static DataTable Languages(HttpApplicationState Application)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwLANGUAGES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 05/20/2008 Paul.  Only display active languages. 
						// 05/20/2008 Paul.  Sort by display name so that Chinese is not at the bottom. 
						sSQL = "select NAME              " + ControlChars.CrLf
						     + "     , NATIVE_NAME       " + ControlChars.CrLf
						     + "     , DISPLAY_NAME      " + ControlChars.CrLf
						     + "  from vwLANGUAGES_Active" + ControlChars.CrLf
						     + " order by DISPLAY_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwLANGUAGES", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					// 10/20/2009 Paul.  Make sure to pass the Application as this function can be called in a background task. 
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 10/24/2009 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public static DataTable ModulesPopups(HttpContext Context)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMODULES_Popup") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME   " + ControlChars.CrLf
						     + "     , RELATIVE_PATH " + ControlChars.CrLf
						     + "  from vwMODULES     " + ControlChars.CrLf
						     + " order by MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								dt.Columns.Add("HAS_POPUP"    , typeof(System.Boolean));
								dt.Columns.Add("SINGULAR_NAME", typeof(System.String ));

								HttpApplicationState Application = Context.Application;
								foreach(DataRow row in dt.Rows)
								{
									string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
									string sMODULE_NAME   = Sql.ToString(row["MODULE_NAME"]);
									string sSINGULAR_NAME = sMODULE_NAME;
									if ( sSINGULAR_NAME.EndsWith("ies") )
										sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 3) + "y";
									else if ( sSINGULAR_NAME.EndsWith("s") )
										sSINGULAR_NAME = sSINGULAR_NAME.Substring(0, sSINGULAR_NAME.Length - 1);
									row["SINGULAR_NAME"] = sSINGULAR_NAME;
									
									// 09/03/2009 Paul.  File IO is expensive, so cache the results of the Exists test. 
									// 11/19/2009 Paul.  Simplify the exists test. 
									// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
									row["HAS_POPUP"] = Utils.CachedFileExists(Context, sRELATIVE_PATH + "Popup.aspx");
									sRELATIVE_PATH = sRELATIVE_PATH.Replace("~/", Sql.ToString(Application["rootURL"]));
									row["RELATIVE_PATH"] = sRELATIVE_PATH;
								}
								Cache.Insert("vwMODULES_Popup", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 04/03/2010 Paul.  Exchange Sync is a per-module feature, even though only Accounts, Bugs, Cases, Leads, Opportunities and Projects are available. 
		public static DataTable ExchangeModulesSync(HttpContext Context)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMODULES_ExchangeSync") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME      " + ControlChars.CrLf
						     + "     , EXCHANGE_FOLDERS " + ControlChars.CrLf
						     + "  from vwMODULES        " + ControlChars.CrLf
						     + " where EXCHANGE_SYNC = 1" + ControlChars.CrLf
						     + " order by MODULE_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwMODULES_ExchangeSync", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DateTime ExchangeFolderCacheExpiration()
		{
#if DEBUG
			return DateTime.Now.AddSeconds(1);
#else
			return DateTime.Now.AddHours(1);
#endif
		}

		public static void ClearExchangeFolders(Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString());
		}

		public static DataTable ExchangeFolders(HttpContext Context, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 04/25/2010 Paul.  We want the well known folders to be first. 
						sSQL = "select *                                    " + ControlChars.CrLf
						     + "  from vwEXCHANGE_FOLDERS                   " + ControlChars.CrLf
						     + " where ASSIGNED_USER_ID = @ASSIGNED_USER_ID " + ControlChars.CrLf
						     + " order by WELL_KNOWN_FOLDER desc, MODULE_NAME, PARENT_NAME, REMOTE_KEY" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwEXCHANGE_FOLDERS." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/19/2010 Paul.  Clear the PaymentGateways cache. 
		public static void ClearPaymentGateways()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwPAYMENT_GATEWAYS");
		}

		public static DataTable PaymentGateways(HttpContext Context)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwPAYMENT_GATEWAYS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                 " + ControlChars.CrLf
						     + "  from vwPAYMENT_GATEWAYS" + ControlChars.CrLf
						     + " order by NAME           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwPAYMENT_GATEWAYS", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 04/22/2010 Paul.  We need to clear the module folders table any time the subscription changes. 
		public static void ClearExchangeModulesFolders(HttpContext Context, string sMODULE_NAME, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sTABLE_NAME = Crm.Modules.TableName(Context.Application, sMODULE_NAME);
			Cache.Remove("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString());
		}

		// 04/04/2010 Paul.  Exchange Folders is a per-module feature, even though only Accounts, Bugs, Cases, Leads, Opportunities and Projects are available. 
		public static DataTable ExchangeModulesFolders(HttpContext Context, string sMODULE_NAME, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sTABLE_NAME = Crm.Modules.TableName(Context.Application, sMODULE_NAME);
			DataTable dt = Cache.Get("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString()) as DataTable;
			//04/04/2010 Paul.  ExchangeModulesFolders can return NULL if Exchange Folders is not supported. 
			// At this time, only Accounts, Bugs, Cases, Contacts, Leads, Opportunities and Projects are supported. 
			if ( dt == null && (   sMODULE_NAME == "Accounts" 
			                    || sMODULE_NAME == "Bugs" 
			                    || sMODULE_NAME == "Cases" 
			                    || sMODULE_NAME == "Contacts" 
			                    || sMODULE_NAME == "Leads" 
			                    || sMODULE_NAME == "Opportunities" 
			                    || sMODULE_NAME == "Project"
			                   )
			   )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL = String.Empty;
						// 05/14/2010 Paul.  We need the NEW_FOLDER flag to determine if we should perform a first SyncAll. 
						sSQL = "select ID                                  " + ControlChars.CrLf
						     + "     , NAME                                " + ControlChars.CrLf
						     + "     , NEW_FOLDER                          " + ControlChars.CrLf
						     + "  from vw" + sTABLE_NAME + "_ExchangeFolder" + ControlChars.CrLf
						     + " where USER_ID = @USER_ID                  " + ControlChars.CrLf
						     + " order by NAME                             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vw" + sTABLE_NAME + "_ExchangeFolders." + gUSER_ID.ToString(), dt, null, ExchangeFolderCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable Modules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 05/27/2009 Paul.  Module names are returned translated, so make sure to cache based on the language. 
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME   " + ControlChars.CrLf
						     + "     , DISPLAY_NAME  " + ControlChars.CrLf
						     + "  from vwMODULES     " + ControlChars.CrLf
						     + " order by MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 08/07/2013 Paul.  Add Undelete module. 
		public static DataTable AuditedModules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Audited") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME      " + ControlChars.CrLf
						     + "     , DISPLAY_NAME     " + ControlChars.CrLf
						     + "  from vwMODULES_Audited" + ControlChars.CrLf
						     + " order by MODULE_NAME   " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Audited", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearTerminologyPickLists()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwTERMINOLOGY_PickList");
		}

		public static DataTable TerminologyPickLists()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTERMINOLOGY_PickList") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY_PickList" + ControlChars.CrLf
						     + " order by LIST_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwTERMINOLOGY_PickList", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable ActiveUsers()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwUSERS_List") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID          " + ControlChars.CrLf
						     + "     , USER_NAME   " + ControlChars.CrLf
						     + "  from vwUSERS_List" + ControlChars.CrLf
						     + " order by USER_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 09/16/2005 Paul.  Users change a lot, so have a very short timeout. 
								Cache.Insert("vwUSERS_List", dt, null, DateTime.Now.AddSeconds(15), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static void ClearTabMenu()
		{
			try
			{
				HttpSessionState Session = HttpContext.Current.Session;
				// 02/25/2010 Paul.  Clear the GroupMenu. 
				Session.Remove("SplendidGroupMenuHtml");
				Session.Remove("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString());
				//System.Web.Caching.Cache Cache = HttpRuntime.Cache;
				//Cache.Remove("vwMODULES_TabMenu");
				// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
				Session.Remove("vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString());
				// 11/17/2007 Paul.  Also clear the mobile menu. 
				Session.Remove("vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString());
				HttpRuntime.Cache.Remove("vwMODULES_Popup");
				// 10/24/2009 Paul.  ModulePopupScripts is a very popular file and we need to cache it as often as possible, yet still allow an invalidation for module changes. 
				// 10/24/2009 Paul.  We still can't use the standard page caching otherwise we risk getting an unauthenticated page cached, which would prevent all popups. 
				// HttpResponse.RemoveOutputCacheItem("~/Include/javascript/ModulePopupScripts.aspx");
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public static DataTable TabMenu()
		{
			//System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
			HttpSessionState Session = HttpContext.Current.Session;
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = Session["vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME             " + ControlChars.CrLf
							     + "     , DISPLAY_NAME            " + ControlChars.CrLf
							     + "     , RELATIVE_PATH           " + ControlChars.CrLf
							     + "  from vwMODULES_TabMenu_ByUser" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
							     + "    or USER_ID is null         " + ControlChars.CrLf
							     + " order by TAB_ORDER            " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
									foreach ( DataRow row in dt.Rows )
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										if ( !Sql.ToBoolean(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Session["vwMODULES_TabMenu_ByUser." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public static DataTable TabFeeds()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwFEEDS_MyTabFeeds_ByUser." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select TITLE             " + ControlChars.CrLf
							     + "     , URL               " + ControlChars.CrLf
							     + "  from vwFEEDS_MyTabFeeds" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID" + ControlChars.CrLf
							     + "    or USER_ID is null   " + ControlChars.CrLf
							     + " order by RANK           " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwFEEDS_MyTabFeeds_ByUser." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable MobileMenu()
		{
			//System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 04/28/2006 Paul.  The menu is now cached in the Session, so it will only get cleared when the user logs out. 
			HttpSessionState Session = HttpContext.Current.Session;
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = Session["vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME                " + ControlChars.CrLf
							     + "     , DISPLAY_NAME               " + ControlChars.CrLf
							     + "     , RELATIVE_PATH              " + ControlChars.CrLf
							     + "  from vwMODULES_MobileMenu_ByUser" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID         " + ControlChars.CrLf
							     + "    or USER_ID is null            " + ControlChars.CrLf
							     + " order by TAB_ORDER               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwMODULES_MobileMenu_ByUser." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public static void ClearShortcuts(string sMODULE_NAME)
		{
			HttpContext.Current.Session.Remove("vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString());
		}

		public static DataTable Shortcuts(string sMODULE_NAME)
		{
			//System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 04/28/2006 Paul.  The shortcuts is now cached in the Session, so it will only get cleared when the user logs out. 
			// 04/28/2006 Paul.  Include the GUID in the USER_ID to that the user does not have to log-out in order to get the correct menu. 
			DataTable dt = HttpContext.Current.Session["vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 09/26/2017 Paul.  Add Archive access right. 
							sSQL = "select MODULE_NAME                            " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                           " + ControlChars.CrLf
							     + "     , RELATIVE_PATH                          " + ControlChars.CrLf
							     + "     , IMAGE_NAME                             " + ControlChars.CrLf
							     + "     , SHORTCUT_ACLTYPE                       " + ControlChars.CrLf
							     + "  from vwSHORTCUTS_Menu_ByUser                " + ControlChars.CrLf
							     + " where MODULE_NAME = @MODULE_NAME             " + ControlChars.CrLf
							     + "   and (USER_ID = @USER_ID or USER_ID is null)" + ControlChars.CrLf
							     + " order by SHORTCUT_ORDER                      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME);
								Sql.AddParameter(cmd, "@USER_ID"    , Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									HttpContext.Current.Session["vwSHORTCUTS_Menu_ByUser." + sMODULE_NAME + "." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public static DataTable LastViewed(string sMODULE_NAME)
		{
			DataTable dt = new DataTable("vwTRACKER");
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                         " + ControlChars.CrLf
					     + "  from vwTRACKER_LastViewed      " + ControlChars.CrLf
					     + " where USER_ID     = @USER_ID    " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@USER_ID"    , Security.USER_ID);
						if( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME != "Home" )
						{
							cmd.CommandText += "   and MODULE_NAME = @MODULE_NAME" + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@MODULE_NAME", sMODULE_NAME    );
						}
						cmd.CommandText += " order by DATE_ENTERED desc      " + ControlChars.CrLf;
						try
						{
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataSet ds = new DataSet() )
								{
									ds.Tables.Add(dt);
									int nHistoryMaxViewed = Sql.ToInteger(HttpContext.Current.Application["CONFIG.history_max_viewed"]);
									if ( nHistoryMaxViewed == 0 )
										nHistoryMaxViewed = 10;
									da.Fill(ds, 0, nHistoryMaxViewed, "vwTRACKER");
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return dt;
		}

		public static DataTable Themes()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("Themes") as DataTable;
			if ( dt == null )
			{
				try
				{
					dt = new DataTable();
					dt.Columns.Add("NAME", Type.GetType("System.String"));
					
					FileInfo objInfo = null;
					string[] arrDirectories;
					// 03/07/2007 Paul.  Theme files have moved to App_Themes in version 1.4. 
					if ( Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Themes")) )
						arrDirectories = Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/App_Themes"));
					else
						arrDirectories = Directory.GetDirectories(HttpContext.Current.Server.MapPath("~/Themes"));
					for ( int i = 0 ; i < arrDirectories.Length ; i++ )
					{
						// 12/04/2005 Paul.  Only include theme if an images folder exists.  This is a quick test. 
						// 08/14/2006 Paul.  Mono uses a different slash than Windows, so use Path.Combine(). 
						if ( Directory.Exists(Path.Combine(arrDirectories[i], "images")) )
						{
							// 11/17/2007 Paul.  Don't allow the user to select the Mobile theme.
							objInfo = new FileInfo(arrDirectories[i]);
#if !DEBUG
							if ( objInfo.Name == "Mobile" )
								continue;
#endif
							DataRow row = dt.NewRow();
							row["NAME"] = objInfo.Name;
							dt.Rows.Add(row);
						}
					}
					// 11/19/2005 Paul.  The themes cache need never expire as themes almost never change. 
					Cache.Insert("Themes", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static string XmlFile(string sPATH_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sDATA = Cache.Get("XmlFile." + sPATH_NAME) as string;
			if ( sDATA == null )
			{
				try
				{
					using ( StreamReader rd = new StreamReader(sPATH_NAME, System.Text.Encoding.UTF8) )
					{
						sDATA = rd.ReadToEnd();
					}
					// 11/19/2005 Paul.  The file cache need never expire as themes almost never change. 
					Cache.Insert("XmlFile." + sPATH_NAME, sDATA, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					throw(new Exception("Could not load file: " + sPATH_NAME, ex));
				}
			}
			return sDATA;
		}

		public static void ClearGridView(string sGRID_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwGRIDVIEWS_COLUMNS." + sGRID_NAME);
			Cache.Remove("vwGRIDVIEWS_RULES."   + sGRID_NAME);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable GridViewColumns(string sGRID_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sGRID_NAME.Contains(".ArchiveView") )
			{
				dt = SplendidCache.GridViewColumns(sGRID_NAME);
			}
			else
			{
				dt = SplendidCache.GridViewColumns(sGRID_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = SplendidCache.GridViewColumns(sGRID_NAME);
			}
			return dt;
		}

		public static DataTable GridViewColumns(string sGRID_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwGRIDVIEWS_COLUMNS." + sGRID_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_COLUMNS   " + ControlChars.CrLf
						     + " where GRID_NAME = @GRID_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by COLUMN_INDEX       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@GRID_NAME", sGRID_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwGRIDVIEWS_COLUMNS." + sGRID_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 03/11/2014 Paul.  This rule could be for EditView, DetailView or GridView, so we have to clear them all. 
		public static void ClearBusinessRules()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				if ( sKey.StartsWith("vwEDITVIEWS_RULES.") || sKey.StartsWith("vwDETAILVIEWS_RULES.") || sKey.StartsWith("vwGRIDVIEWS_RULES.") )
					Cache.Remove(sKey);
			}
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable GridViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sNAME.Contains(".ArchiveView") )
			{
				dt = SplendidCache.GridViewRules(sNAME);
			}
			else
			{
				dt = SplendidCache.GridViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = SplendidCache.GridViewRules(sNAME);
			}
			return dt;
		}

		// 11/22/2010 Paul.  Apply Business Rules. 
		public static DataTable GridViewRules(string sNAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwGRIDVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_RULES  " + ControlChars.CrLf
						     + " where NAME = @NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwGRIDVIEWS_RULES." + sNAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 12/04/2010 Paul.  We need to cache the business rules separately so that they can be accessed by Reports. 
		public static void ClearReportRules(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwBUSINESS_RULES." + gID.ToString());
		}

		public static DataTable ReportRules(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwREPORT_RULES." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME " + ControlChars.CrLf
						     + "     , XOML        " + ControlChars.CrLf
						     + "  from vwRULES_Edit" + ControlChars.CrLf
						     + " where ID = @ID    " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwREPORT_RULES." + gID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static void ClearDetailView(string sDETAIL_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME);
			Cache.Remove("vwDETAILVIEWS_RULES."  + sDETAIL_NAME);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable DetailViewFields(string sDETAIL_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sDETAIL_NAME.Contains(".ArchiveView") )
			{
				dtFields = SplendidCache.DetailViewFields(sDETAIL_NAME);
			}
			else
			{
				dtFields = SplendidCache.DetailViewFields(sDETAIL_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count == 0 )
					dtFields = SplendidCache.DetailViewFields(sDETAIL_NAME);
			}
			return dtFields;
		}

		public static DataTable DetailViewFields(string sDETAIL_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                         " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_FIELDS      " + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by FIELD_INDEX            " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwDETAILVIEWS_FIELDS." + sDETAIL_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable DetailViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sNAME.Contains(".ArchiveView") )
			{
				dt = SplendidCache.DetailViewRules(sNAME);
			}
			else
			{
				dt = SplendidCache.DetailViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = SplendidCache.DetailViewRules(sNAME);
			}
			return dt;
		}

		// 11/10/2010 Paul.  Apply Business Rules. 
		public static DataTable DetailViewRules(string sNAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwDETAILVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                  " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RULES" + ControlChars.CrLf
						     + " where NAME = @NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwDETAILVIEWS_RULES." + sNAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static DataTable TabGroups()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTAB_GROUPS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/25/2010 Paul.  The Group may not be visible on the menu bar. 
						sSQL = "select NAME          " + ControlChars.CrLf
						     + "     , TITLE         " + ControlChars.CrLf
						     + "     , GROUP_MENU    " + ControlChars.CrLf
						     + "  from vwTAB_GROUPS  " + ControlChars.CrLf
						     + " where ENABLED = 1   " + ControlChars.CrLf
						     + " order by GROUP_ORDER" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwTAB_GROUPS", dt, null, DefaultCacheExpiration(), System.Web.Caching.Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static DataTable ModuleGroups()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMODULES_GROUPS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/24/2010 Paul.  We need to specify an order to the modules for the tab menu. 
						sSQL = "select GROUP_NAME                  " + ControlChars.CrLf
						     + "     , MODULE_NAME                 " + ControlChars.CrLf
						     + "     , MODULE_MENU                 " + ControlChars.CrLf
						     + "  from vwMODULES_GROUPS            " + ControlChars.CrLf
						     + " where ENABLED = 1                 " + ControlChars.CrLf
						     + " order by GROUP_ORDER, MODULE_ORDER, MODULE_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwMODULES_GROUPS", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static DataTable ModuleGroupsByUser()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 02/24/2010 Paul.  We need to specify an order to the modules for the tab menu. 
							sSQL = "select GROUP_NAME                  " + ControlChars.CrLf
							     + "     , MODULE_NAME                 " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                " + ControlChars.CrLf
							     + "     , RELATIVE_PATH               " + ControlChars.CrLf
							     + "  from vwMODULES_GROUPS_ByUser     " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID          " + ControlChars.CrLf
							     + " order by GROUP_ORDER, MODULE_ORDER, MODULE_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									// 11/06/2009 Paul.  We need a fast way to disable modules that do not exist on the Offline Client. 
									foreach ( DataRow row in dt.Rows )
									{
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										if ( !Sql.ToBoolean(HttpContext.Current.Application["Modules." + sMODULE_NAME + ".Exists"]) )
										{
											row.Delete();
										}
									}
									dt.AcceptChanges();
									Cache.Insert("vwMODULES_GROUPS_ByUser." + Security.USER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearDetailViewRelationships(string sDETAIL_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME);
		}

		public static DataTable DetailViewRelationships(string sDETAIL_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                          " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RELATIONSHIPS" + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME " + ControlChars.CrLf
						     + " order by RELATIONSHIP_ORDER      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwDETAILVIEWS_RELATIONSHIPS." + sDETAIL_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
		public static void ClearEditViewRelationships(string sEDIT_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 04/27/2010 Paul.  There are two cached items that need to be cleared. 
			Cache.Remove("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + ".NewRecord"     );
			Cache.Remove("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + ".ExistingRecord");
		}

		// 04/19/20910 Paul.  Add separate table for EditView Relationships. 
		public static DataTable EditViewRelationships(string sEDIT_NAME, bool bNewRecord)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + (bNewRecord ? ".NewRecord" : ".ExistingRecord")) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                          " + ControlChars.CrLf
						     + "  from vwEDITVIEWS_RELATIONSHIPS  " + ControlChars.CrLf
						     + " where EDIT_NAME = @EDIT_NAME     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							if ( bNewRecord )
								cmd.CommandText += "   and NEW_RECORD_ENABLED = 1     " + ControlChars.CrLf;
							else
								cmd.CommandText += "   and EXISTING_RECORD_ENABLED = 1" + ControlChars.CrLf;
							cmd.CommandText += " order by RELATIONSHIP_ORDER      " + ControlChars.CrLf;
							Sql.AddParameter(cmd, "@EDIT_NAME", sEDIT_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwEDITVIEWS_RELATIONSHIPS." + sEDIT_NAME + (bNewRecord ? ".NewRecord" : ".ExistingRecord"), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static void ClearUserDashlets(string sDETAIL_NAME)
		{
			HttpContext.Current.Session.Remove("vwDASHLETS_USERS." + sDETAIL_NAME);
		}

		// 07/10/2009 Paul.  We are now allowing relationships to be user-specific. 
		public static DataTable UserDashlets(string sDETAIL_NAME, Guid gUSER_ID)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwDASHLETS_USERS." + sDETAIL_NAME] as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 09/20/2009 Paul.  Use the special view vwDASHLETS_USERS_Assigned that will included deleted dashlets. 
						// This will allow a user to delete all dashlets without having the defaults re-assigned. 
						// 12/03/2009 Paul.  The Title is used for the tabbed subpanels. 
						// 01/24/2010 Paul.  We need the ID for report dashlet management. 
						sSQL = "select ID                              " + ControlChars.CrLf
						     + "     , DETAIL_NAME                     " + ControlChars.CrLf
						     + "     , MODULE_NAME                     " + ControlChars.CrLf
						     + "     , CONTROL_NAME                    " + ControlChars.CrLf
						     + "     , TITLE                           " + ControlChars.CrLf
						     + "     , RELATIONSHIP_ORDER              " + ControlChars.CrLf
						     + "  from vwDASHLETS_USERS                " + ControlChars.CrLf
						     + " where DASHLET_ENABLED  = 1            " + ControlChars.CrLf
						     + "   and ASSIGNED_USER_ID = @USER_ID     " + ControlChars.CrLf
						     + "   and DETAIL_NAME      = @DETAIL_NAME " + ControlChars.CrLf
						     + "union                                  " + ControlChars.CrLf
						     + "select ID                              " + ControlChars.CrLf
						     + "     , DETAIL_NAME                     " + ControlChars.CrLf
						     + "     , MODULE_NAME                     " + ControlChars.CrLf
						     + "     , CONTROL_NAME                    " + ControlChars.CrLf
						     + "     , TITLE                           " + ControlChars.CrLf
						     + "     , RELATIONSHIP_ORDER              " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RELATIONSHIPS     " + ControlChars.CrLf
						     + " where DETAIL_NAME = @DETAIL_NAME      " + ControlChars.CrLf
						     + "   and not exists(select *                              " + ControlChars.CrLf
						     + "                    from vwDASHLETS_USERS_Assigned      " + ControlChars.CrLf
						     + "                   where ASSIGNED_USER_ID = @USER_ID    " + ControlChars.CrLf
						     + "                     and DETAIL_NAME      = @DETAIL_NAME" + ControlChars.CrLf
						     + "                 )                                      " + ControlChars.CrLf
						     + " order by RELATIONSHIP_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID"    , gUSER_ID    );
							Sql.AddParameter(cmd, "@DETAIL_NAME", sDETAIL_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Session["vwDASHLETS_USERS." + sDETAIL_NAME] = dt;
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static void ClearEditView(string sEDIT_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			// 03/16/2012 Paul.  With the addition of the SearchView flag, we need to update the key to include the flag. 
			Cache.Remove("vwEDITVIEWS_FIELDS." + sEDIT_NAME + ".True" );
			Cache.Remove("vwEDITVIEWS_FIELDS." + sEDIT_NAME + ".False");
			Cache.Remove("vwEDITVIEWS_RULES."  + sEDIT_NAME);
		}

		// 10/13/2011 Paul.  Special list of EditViews for the search area with IS_MULTI_SELECT. 
		public static DataTable EditViewFields(string sEDIT_NAME)
		{
			return EditViewFields(sEDIT_NAME, false);
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable EditViewFields(string sEDIT_NAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sEDIT_NAME.Contains(".ArchiveView") )
			{
				dtFields = SplendidCache.EditViewFields(sEDIT_NAME);
			}
			else
			{
				dtFields = SplendidCache.EditViewFields(sEDIT_NAME + "." + sPRIMARY_ROLE_NAME);
				if ( dtFields.Rows.Count == 0 )
					dtFields = SplendidCache.EditViewFields(sEDIT_NAME);
			}
			return dtFields;
		}

		// 05/05/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable EditViewFields(string sEDIT_NAME, string sPRIMARY_ROLE_NAME, bool bSearchView)
		{
			DataTable dtFields = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sEDIT_NAME.Contains(".ArchiveView") )
			{
				dtFields = SplendidCache.EditViewFields(sEDIT_NAME, bSearchView);
			}
			else
			{
				dtFields = SplendidCache.EditViewFields(sEDIT_NAME + "." + sPRIMARY_ROLE_NAME, bSearchView);
				if ( dtFields.Rows.Count == 0 )
					dtFields = SplendidCache.EditViewFields(sEDIT_NAME, bSearchView);
			}
			return dtFields;
		}

		public static DataTable EditViewFields(string sEDIT_NAME, bool bSearchView)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwEDITVIEWS_FIELDS." + sEDIT_NAME + "." + bSearchView.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						// 10/13/2011 Paul.  Special list of EditViews for the search area with IS_MULTI_SELECT. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from " + (bSearchView ? "vwEDITVIEWS_FIELDS_SearchView" : "vwEDITVIEWS_FIELDS") + ControlChars.CrLf
						     + " where EDIT_NAME = @EDIT_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by FIELD_INDEX        " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@EDIT_NAME", sEDIT_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwEDITVIEWS_FIELDS." + sEDIT_NAME + "." + bSearchView.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 06/29/2012 Paul.  Business Rules need to be cleared after saving. 
		public static void ClearEditViewRules(string sNAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwEDITVIEWS_RULES." + sNAME);
		}

		// 05/10/2016 Paul.  The User Primary Role is used with role-based views. 
		public static DataTable EditViewRules(string sNAME, string sPRIMARY_ROLE_NAME)
		{
			DataTable dt = null;
			// 10/1/2017 Paul.  Role-based view will not be applied with the ArchiveView. 
			if ( Sql.IsEmptyString(sPRIMARY_ROLE_NAME) || sNAME.Contains(".ArchiveView") )
			{
				dt = SplendidCache.EditViewRules(sNAME);
			}
			else
			{
				dt = SplendidCache.EditViewRules(sNAME + "." + sPRIMARY_ROLE_NAME);
				if ( dt.Rows.Count == 0 )
					dt = SplendidCache.EditViewRules(sNAME);
			}
			return dt;
		}

		// 11/10/2010 Paul.  Apply Business Rules. 
		public static DataTable EditViewRules(string sNAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwEDITVIEWS_RULES." + sNAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                " + ControlChars.CrLf
						     + "  from vwEDITVIEWS_RULES" + ControlChars.CrLf
						     + " where NAME = @NAME     " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@NAME", sNAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwEDITVIEWS_RULES." + sNAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static void ClearDynamicButtons(string sVIEW_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwDYNAMIC_BUTTONS." + sVIEW_NAME);
		}

		public static DataTable DynamicButtons(string sVIEW_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwDYNAMIC_BUTTONS." + sVIEW_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 01/09/2006 Paul.  Exclude DEFAULT_VIEW. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwDYNAMIC_BUTTONS     " + ControlChars.CrLf
						     + " where VIEW_NAME = @VIEW_NAME" + ControlChars.CrLf
						     + "   and (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by CONTROL_INDEX      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@VIEW_NAME", sVIEW_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwDYNAMIC_BUTTONS." + sVIEW_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static void ClearFieldsMetaData(string sMODULE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwFIELDS_META_DATA_Validated." + sMODULE_NAME);
			ClearFilterColumns(sMODULE_NAME);
			ClearSearchColumns(sMODULE_NAME);
		}

		// 09/09/2009 Paul.  Change the field name to be more obvious. 
		// 08/30/2016 Paul.  The Business Process Engine needs to be able to get the list of custom fields in a background thread. 
		public static DataTable FieldsMetaData_Validated(string sTABLE_NAME)
		{
			return FieldsMetaData_Validated(HttpContext.Current.Application, sTABLE_NAME);
		}

		public static DataTable FieldsMetaData_Validated(HttpApplicationState Application, string sTABLE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwFIELDS_META_DATA_Validated." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                             " + ControlChars.CrLf
						     + "  from vwFIELDS_META_DATA_Validated  " + ControlChars.CrLf
						     + " where TABLE_NAME = @TABLE_NAME      " + ControlChars.CrLf
						     + " order by colid                      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwFIELDS_META_DATA_Validated." + sTABLE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 11/26/2009 Paul.  We need the actual table fields when sync'ing with the offline client. 
		public static DataTable SqlColumns(string sTABLE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ColumnName              " + ControlChars.CrLf
						     + "     , CsType                  " + ControlChars.CrLf
						     + "     , length                  " + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @TABLE_NAME" + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@TABLE_NAME", Sql.MetadataName(cmd, sTABLE_NAME));
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwSqlColumns." + sTABLE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		// 05/25/2008 Paul.  We needed an easy way to get to the custom fields for tables not related to a module. 
		// 05/25/2008 Paul.  There is no automated clearing of this cache entry. The admin must manually perform a Reload. 
		public static DataTable FieldsMetaData_UnvalidatedCustomFields(string sTABLE_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwFIELDS_META_DATA_Unvalidated." + sTABLE_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ColumnName   as NAME    " + ControlChars.CrLf
						     + "     , CsType       as CsType  " + ControlChars.CrLf
						     + "     , length       as MAX_SIZE" + ControlChars.CrLf
						     + "  from vwSqlColumns            " + ControlChars.CrLf
						     + " where ObjectName = @TABLE_NAME" + ControlChars.CrLf
						     + "   and ColumnName <> 'ID_C'    " + ControlChars.CrLf
						     + " order by colid                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@TABLE_NAME", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwFIELDS_META_DATA_Unvalidated." + sTABLE_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
					dt = new DataTable();
				}
			}
			return dt;
		}

		public static DataTable ForumTopics()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwFORUM_TOPICS_LISTBOX") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                    " + ControlChars.CrLf
						     + "  from vwFORUM_TOPICS_LISTBOX  " + ControlChars.CrLf
						     + " order by LIST_ORDER           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwFORUM_TOPICS_LISTBOX", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static void ClearSavedSearch(string sMODULE)
		{
			HttpContext.Current.Session.Remove("vwSAVED_SEARCH." + sMODULE);
		}

		public static DataTable SavedSearch(string sMODULE)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwSAVED_SEARCH." + sMODULE] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 07/29/2008 Paul.  A global saved search is one where ASSIGNED_USER_ID is null. 
							// 09/01/2010 Paul.  Store a copy of the DEFAULT_SEARCH_ID in the table so that we don't need to read the XML in order to get the value. 
							sSQL = "select ID                         " + ControlChars.CrLf
							     + "     , NAME                       " + ControlChars.CrLf
							     + "     , CONTENTS                   " + ControlChars.CrLf
							     + "     , DEFAULT_SEARCH_ID          " + ControlChars.CrLf
							     + "  from vwSAVED_SEARCH             " + ControlChars.CrLf
							     + " where (ASSIGNED_USER_ID = @USER_ID or ASSIGNED_USER_ID is null)" + ControlChars.CrLf
							     + "   and SEARCH_MODULE    = @MODULE " + ControlChars.CrLf
							     + " order by NAME                    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								Sql.AddParameter(cmd, "@MODULE" , sMODULE         );
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwSAVED_SEARCH." + sMODULE] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		public static void ClearSearchColumns(string sVIEW_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwSqlColumns_Searching." + sVIEW_NAME);
		}

		public static DataTable SearchColumns(string sVIEW_NAME)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwSqlColumns_Searching." + sVIEW_NAME) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
						// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwSqlColumns_Searching  " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sVIEW_NAME));
							// 04/16/2009 Paul.  A customer did not want to see all fields, just the ones that were available in the EditView. 
							if ( Sql.ToBoolean(HttpContext.Current.Application["CONFIG.search_editable_columns_only"]) )
							{
								// 04/17/2009 Paul.  Key off of the view name so that we don't have to change other areas of the code. 
								cmd.CommandText += "   and ColumnName in (select DATA_FIELD from vwEDITVIEWS_FIELDS_Searching where VIEW_NAME = @VIEW_NAME)" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@VIEW_NAME", sVIEW_NAME);
							}
							cmd.CommandText += " order by colid                " + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								// 06/10/2006 Paul.  The default sliding scale is not appropriate as columns can be added. 
								Cache.Insert("vwSqlColumns_Searching." + sVIEW_NAME, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearEmailGroups()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwUSERS_Groups");
		}

		public static DataTable EmailGroups()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwUSERS_Groups") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwUSERS_Groups" + ControlChars.CrLf
						     + " order by NAME       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwUSERS_Groups", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearInboundEmails()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwINBOUND_EMAILS_Bounce");
			Cache.Remove("vwINBOUND_EMAILS_Monitored");
		}

		//.02/16/2008 Paul.  InboundEmailBounce will also be used in the UI, so provide a version without parameters. 
		public static DataTable InboundEmailBounce()
		{
			return InboundEmailBounce(HttpContext.Current);
		}

		// 02/16/2008 Paul.  InboundEmailBounce needs to be called from the scheduler, so the application must be provided. 
		// 10/27/2008 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public static DataTable InboundEmailBounce(HttpContext Context)
		{
			// 02/16/2008 Paul.  When called from the scheduler, the context is not available, so get the cache from HttpRuntime. 
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwINBOUND_EMAILS_Bounce") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                      " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Bounce" + ControlChars.CrLf
						     + " order by NAME                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwINBOUND_EMAILS_Bounce", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 02/16/2008 Paul.  InboundEmailMonitored needs to be called from the scheduler, so the application must be provided. 
		// 10/27/2008 Paul.  Pass the context instead of the Application so that more information will be available to the error handling. 
		public static DataTable InboundEmailMonitored(HttpContext Context)
		{
			// 02/16/2008 Paul.  When called from the scheduler, the context is not available, so get the cache from HttpRuntime. 
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwINBOUND_EMAILS_Monitored") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                       " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Monitored" + ControlChars.CrLf
						     + " order by NAME                 " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwINBOUND_EMAILS_Monitored", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 11/18/2008 Paul.  Teams can be used in the search panels. 
		public static DataTable Teams()
		{
			DataTable dt = null;
			// 11/25/2006 Paul.  An admin can see all teams, but most users will only see the teams which they are assigned to. 
			if ( Security.IS_ADMIN )
			{
				System.Web.Caching.Cache Cache = HttpRuntime.Cache;
				dt = Cache.Get("vwTEAMS") as DataTable;
				if ( dt == null )
				{
					try
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID     " + ControlChars.CrLf
							     + "     , NAME   " + ControlChars.CrLf
							     + "  from vwTEAMS" + ControlChars.CrLf
							     + " order by NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									Cache.Insert("vwTEAMS", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
			else
			{
				HttpSessionState Session = HttpContext.Current.Session;
				dt = Session["vwTEAMS_MyList"] as DataTable;
				if ( dt == null )
				{
					try
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                                      " + ControlChars.CrLf
							     + "     , NAME                                    " + ControlChars.CrLf
							     + "  from vwTEAMS_MyList                          " + ControlChars.CrLf
							     + " where MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf
							     + " order by NAME                                 " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									dt = new DataTable();
									da.Fill(dt);
									Session["vwTEAMS_MyList"] = dt;
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
				}
			}
			return dt;
		}

		public static void ClearTags()
		{
			HttpRuntime.Cache.Remove("vwTAGS");
		}

		// 05/12/2016 Paul.  Tags can be used in the search panels. 
		public static DataTable Tags()
		{
			DataTable dt = null;
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			dt = Cache.Get("vwTAGS") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID     " + ControlChars.CrLf
						     + "     , NAME   " + ControlChars.CrLf
						     + "  from vwTAGS " + ControlChars.CrLf
						     + " order by NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwTAGS", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable ACLFieldAliases(HttpContext Context)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwACL_FIELDS_ALIASES") as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                   " + ControlChars.CrLf
						     + "  from vwACL_FIELDS_ALIASES" + ControlChars.CrLf
						     + " order by MODULE_NAME, NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwACL_FIELDS_ALIASES", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearReport(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sID = gID.ToString();
			//Cache.Remove("vwREPORTS." + sID);
			//Cache.Remove("vwREPORTS.Parameters." + sID);
			//Cache.Remove("vwREPORTS.Parameters.EditView." + sID);
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
				if ( sKey.StartsWith("vwREPORTS.") && sKey.Contains(sID) )
					Cache.Remove(sKey);
			}
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public static DataTable ReportParametersEditView(Guid gID, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwREPORTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
					if ( HttpContext.Current != null )
						bShowAssignedUser = Sql.ToBoolean(HttpContext.Current.Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtReportParameters = SplendidCache.ReportParameters(gID, gUSER_ID);
					if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
					{
						string sMODULE_NAME = Sql.ToString(dtReportParameters.Rows[0]["MODULE_NAME"]);
						DataTable dtEditView = SplendidCache.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtReportParameters.Rows )
						{
							// 03/09/2012 Paul.  Making the data field upper case will simplify tests later. 
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]).ToUpper();
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) && sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID" )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									break;
								}
							}
							// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 03/06/2012 Paul.  A report parameter can have a special Date Rule field. 
								if ( sDATA_FIELD == "DATE_RULE" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "date_rule_dom";
								}
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								else if ( sDEFAULT_VALUE.StartsWith("=") )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = Crm.Modules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
									// 04/11/2016 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
									if ( bMULTI_VALUE )
									{
										DataRow row2 = dt.NewRow();
										dt.Rows.Add(row2);
										for ( int i = 0; i < dt.Columns.Count; i++ )
										{
											row2[i] = row[i];
										}
										row ["DATA_FIELD"] = sDATA_FIELD + "_AFTER" ;
										row2["DATA_FIELD"] = sDATA_FIELD + "_BEFORE";
										if ( HttpContext.Current != null && HttpContext.Current.Session != null )
										{
											L10N L10n = new L10N(Sql.ToString (HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
											row ["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_AFTER" );
											row2["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_BEFORE");
										}
										else
										{
											row ["DATA_LABEL"] = "On or After:";
											row2["DATA_LABEL"] = "Before:";
										}
									}
								}
								// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
								else if ( sDATA_FIELD == "ASSIGNED_TO" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "AssignedTo";
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
					Cache.Insert("vwREPORTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public static DataTable ReportParameters(Guid gID, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwREPORTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					DataTable dtReport = SplendidCache.Report(gID);
					if ( dtReport.Rows.Count > 0 )
					{
						DataRow rdr = dtReport.Rows[0];
						string sRDL         = Sql.ToString(rdr["RDL"        ]);
						string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						
						RdlDocument rdl = new RdlDocument();
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								SplendidCache.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							// 03/04/2012 Paul.  Collection of values. 
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values/Value");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
					Cache.Insert("vwREPORTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 08/11/2014 Paul.  We need a similar method to get report parameters from an unsaved report. 
		public static DataTable ReportParametersEditView(string sMODULE_NAME, string sRDL)
		{
			DataTable dt = null;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
					if ( HttpContext.Current != null )
						bShowAssignedUser = Sql.ToBoolean(HttpContext.Current.Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtReportParameters = SplendidCache.ReportParameters(sMODULE_NAME, sRDL);
					if ( dtReportParameters != null && dtReportParameters.Rows.Count > 0 )
					{
						DataTable dtEditView = SplendidCache.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtReportParameters.Rows )
						{
							// 03/09/2012 Paul.  Making the data field upper case will simplify tests later. 
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]).ToUpper();
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) && sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID" )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									break;
								}
							}
							// 03/09/2012 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 03/06/2012 Paul.  A report parameter can have a special Date Rule field. 
								if ( sDATA_FIELD == "DATE_RULE" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "date_rule_dom";
								}
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								else if ( sDEFAULT_VALUE.StartsWith("=") )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = Crm.Modules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									// 04/11/2016 Paul.  Use BuildTermName. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD);
									row["DATA_FIELD"                ] = sDATA_FIELD;
									// 04/11/2016 Paul.  Special support for between clause as a parameter. Needed to be separated into 2 report parameters. 
									if ( bMULTI_VALUE )
									{
										DataRow row2 = dt.NewRow();
										dt.Rows.Add(row2);
										for ( int i = 0; i < dt.Columns.Count; i++ )
										{
											row2[i] = row[i];
										}
										row ["DATA_FIELD"] = sDATA_FIELD + "_AFTER" ;
										row2["DATA_FIELD"] = sDATA_FIELD + "_BEFORE";
										if ( HttpContext.Current != null && HttpContext.Current.Session != null )
										{
											L10N L10n = new L10N(Sql.ToString (HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
											row ["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_AFTER" );
											row2["DATA_LABEL"] =  L10n.Term(Utils.BuildTermName(sMODULE_NAME, sDATA_FIELD)) + " " + L10n.Term("SavedSearch.LBL_SEARCH_BEFORE");
										}
										else
										{
											row ["DATA_LABEL"] = "On or After:";
											row2["DATA_LABEL"] = "Before:";
										}
									}
								}
								// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								// 03/06/2012 Paul.  A report parameter can include an Assigned To list. 
								else if ( sDATA_FIELD == "ASSIGNED_TO" )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = "AssignedTo";
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 08/11/2014 Paul.  We need a similar method to get report parameters from an unsaved report. 
		public static DataTable ReportParameters(string sMODULE_NAME, string sRDL)
		{
			DataTable dt = null;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					if ( !Sql.IsEmptyString(sMODULE_NAME) )
					{
						
						RdlDocument rdl = new RdlDocument();
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								SplendidCache.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							// 03/04/2012 Paul.  Collection of values. 
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values/Value");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/01/2012 Paul.  Caching by name might not be a good idea as we may have issues clearing the cache value. 
		public static Guid ReportByName(string sReportName)
		{
			Guid gID = Guid.Empty;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select ID            " + ControlChars.CrLf
					     + "  from vwREPORTS_Edit" + ControlChars.CrLf
					     + " where NAME = @NAME  " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sReportName);
						gID = Sql.ToGuid(cmd.ExecuteScalar());
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			return gID;
		}

		// 04/06/2011 Paul.  Cache reports. 
		public static DataTable Report(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwREPORTS." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwREPORTS_Edit" + ControlChars.CrLf
						     + " where ID = @ID      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwREPORTS." + gID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 01/24/2010 Paul.  Clear the Report List on save. 
		public static void ClearReports()
		{
			try
			{
				HttpContext.Current.Session.Remove("vwREPORTS_List." + Security.USER_ID.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		// 01/24/2010 Paul.  Place the report list in the cache so that it would be available in SearchView. 
		public static DataTable Reports()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwREPORTS_List." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID            " + ControlChars.CrLf
							     + "     , NAME          " + ControlChars.CrLf
							     + "  from vwREPORTS_List" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								//Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", Security.USER_ID);
								// 01/20/2011 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								// This new approach to report security should have been applied many months ago. 
								Security.Filter(cmd, "Reports", "list");
								cmd.CommandText += " order by NAME" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwREPORTS_List." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static DataTable ChartParametersEditView(Guid gID, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCHARTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("EDIT_NAME"                   , typeof(String ) );
				dt.Columns.Add("FIELD_INDEX"                 , typeof(Int32  ) );
				dt.Columns.Add("FIELD_TYPE"                  , typeof(String ) );
				dt.Columns.Add("DATA_LABEL"                  , typeof(String ) );
				dt.Columns.Add("DATA_FIELD"                  , typeof(String ) );
				dt.Columns.Add("DATA_FORMAT"                 , typeof(String ) );
				dt.Columns.Add("DISPLAY_FIELD"               , typeof(String ) );
				dt.Columns.Add("CACHE_NAME"                  , typeof(String ) );
				dt.Columns.Add("DATA_REQUIRED"               , typeof(Boolean) );
				dt.Columns.Add("UI_REQUIRED"                 , typeof(Boolean) );
				dt.Columns.Add("ONCLICK_SCRIPT"              , typeof(String ) );
				dt.Columns.Add("FORMAT_SCRIPT"               , typeof(String ) );
				dt.Columns.Add("FORMAT_TAB_INDEX"            , typeof(Int16  ) );
				dt.Columns.Add("FORMAT_MAX_LENGTH"           , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_SIZE"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_ROWS"                 , typeof(Int32  ) );
				dt.Columns.Add("FORMAT_COLUMNS"              , typeof(Int32  ) );
				dt.Columns.Add("COLSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("ROWSPAN"                     , typeof(Int32  ) );
				dt.Columns.Add("LABEL_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("FIELD_WIDTH"                 , typeof(String ) );
				dt.Columns.Add("DATA_COLUMNS"                , typeof(Int32  ) );
				dt.Columns.Add("MODULE_TYPE"                 , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_MODULE_NAME"  , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_VIEW_NAME"    , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_ID_FIELD"     , typeof(String ) );
				dt.Columns.Add("RELATED_SOURCE_NAME_FIELD"   , typeof(String ) );
				dt.Columns.Add("RELATED_VIEW_NAME"           , typeof(String ) );
				dt.Columns.Add("RELATED_ID_FIELD"            , typeof(String ) );
				dt.Columns.Add("RELATED_NAME_FIELD"          , typeof(String ) );
				dt.Columns.Add("RELATED_JOIN_FIELD"          , typeof(String ) );
				dt.Columns.Add("PARENT_FIELD"                , typeof(String ) );
				dt.Columns.Add("FIELD_VALIDATOR_MESSAGE"     , typeof(String ) );
				dt.Columns.Add("VALIDATION_TYPE"             , typeof(String ) );
				dt.Columns.Add("REGULAR_EXPRESSION"          , typeof(String ) );
				dt.Columns.Add("DATA_TYPE"                   , typeof(String ) );
				dt.Columns.Add("MININUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("MAXIMUM_VALUE"               , typeof(String ) );
				dt.Columns.Add("COMPARE_OPERATOR"            , typeof(String ) );
				dt.Columns.Add("TOOL_TIP"                    , typeof(String ) );
				
				try
				{
					// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
					bool bShowAssignedUser = false;
					if ( HttpContext.Current != null )
						bShowAssignedUser = Sql.ToBoolean(HttpContext.Current.Application["CONFIG.Reports.ShowAssignedUser"]);
					// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
					DataTable dtChartParameters = SplendidCache.ChartParameters(gID, gUSER_ID);
					if ( dtChartParameters != null && dtChartParameters.Rows.Count > 0 )
					{
						string sMODULE_NAME = Sql.ToString(dtChartParameters.Rows[0]["MODULE_NAME"]);
						DataTable dtEditView = SplendidCache.EditViewFields(sMODULE_NAME + ".EditView");
						foreach ( DataRow rowParameter in dtChartParameters.Rows )
						{
							string sDATA_FIELD    = Sql.ToString(rowParameter["NAME"         ]);
							string sDATA_LABEL    = Sql.ToString(rowParameter["PROMPT"       ]);
							string sDATA_TYPE     = Sql.ToString(rowParameter["DATA_TYPE"    ]);
							string sDEFAULT_VALUE = Sql.ToString(rowParameter["DEFAULT_VALUE"]);
							bool   bHIDDEN        = Sql.ToBoolean(rowParameter["HIDDEN"      ]);
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDATA_SET_NAME = Sql.ToString (rowParameter["DATA_SET_NAME"]);
							bool   bMULTI_VALUE   = Sql.ToBoolean(rowParameter["MULTI_VALUE"  ]);
							bool bFieldFound = false;
							// 04/09/2011 Paul.  ID is not allowed as a parameter because it is used by the Report ID in the Dashlet. 
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							if ( sDATA_FIELD == "ID" || bHIDDEN )
								continue;
							foreach ( DataRow rowEditView in dtEditView.Rows )
							{
								if ( sDATA_FIELD == Sql.ToString(rowEditView["DATA_FIELD"]) )
								{
									bFieldFound = true;
									DataRow row = dt.NewRow();
									dt.Rows.Add(row);
									row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
									row["FIELD_INDEX"               ] = dt.Rows.Count;
									row["FIELD_TYPE"                ] = rowEditView["FIELD_TYPE"                ];
									// 03/04/2012 Paul.  Custom label was being applied to field, not label. 
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : Sql.ToString(rowEditView["DATA_LABEL"]);
									row["DATA_FIELD"                ] = rowEditView["DATA_FIELD"                ];
									row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
									row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
									row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
									row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
									row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
									row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
									row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
									row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
									row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
									row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
									row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
									row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
									//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
									//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
									row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
									row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
									row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
									row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
									//row["RELATED_SOURCE_MODULE_NAME"] = rowEditView["RELATED_SOURCE_MODULE_NAME"];
									//row["RELATED_SOURCE_VIEW_NAME"  ] = rowEditView["RELATED_SOURCE_VIEW_NAME"  ];
									//row["RELATED_SOURCE_ID_FIELD"   ] = rowEditView["RELATED_SOURCE_ID_FIELD"   ];
									//row["RELATED_SOURCE_NAME_FIELD" ] = rowEditView["RELATED_SOURCE_NAME_FIELD" ];
									//row["RELATED_VIEW_NAME"         ] = rowEditView["RELATED_VIEW_NAME"         ];
									//row["RELATED_ID_FIELD"          ] = rowEditView["RELATED_ID_FIELD"          ];
									//row["RELATED_NAME_FIELD"        ] = rowEditView["RELATED_NAME_FIELD"        ];
									//row["RELATED_JOIN_FIELD"        ] = rowEditView["RELATED_JOIN_FIELD"        ];
									//row["PARENT_FIELD"              ] = rowEditView["PARENT_FIELD"              ];
									row["FIELD_VALIDATOR_MESSAGE"   ] = rowEditView["FIELD_VALIDATOR_MESSAGE"   ];
									row["VALIDATION_TYPE"           ] = rowEditView["VALIDATION_TYPE"           ];
									row["REGULAR_EXPRESSION"        ] = rowEditView["REGULAR_EXPRESSION"        ];
									row["DATA_TYPE"                 ] = rowEditView["DATA_TYPE"                 ];
									row["MININUM_VALUE"             ] = rowEditView["MININUM_VALUE"             ];
									row["MAXIMUM_VALUE"             ] = rowEditView["MAXIMUM_VALUE"             ];
									row["COMPARE_OPERATOR"          ] = rowEditView["COMPARE_OPERATOR"          ];
									row["TOOL_TIP"                  ] = rowEditView["TOOL_TIP"                  ];
									// 03/04/2012 Paul.  Apply MultiValue flag if set in the report. 
									if ( Sql.ToString(row["FIELD_TYPE"]) == "ListBox" && bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
									break;
								}
							}
							// 01/15/2013 Paul.  ASSIGNED_USER_ID is a special parameter that is not a prompt. 
							// 01/15/2013 Paul.  A customer wants to be able to change the assigned user as this was previously allowed in report prompts. 
							if ( !bFieldFound && (bShowAssignedUser || (sDATA_FIELD != "ASSIGNED_USER_ID" && sDATA_FIELD != "TEAM_ID")) )
							{
								DataRow row = dt.NewRow();
								dt.Rows.Add(row);
								row["EDIT_NAME"                 ] = sMODULE_NAME + ".EditView";
								row["FIELD_INDEX"               ] = dt.Rows.Count;
								// 11/15/2011 Paul.  If the value starts with a equals, then this is a formula and should not be treated as a date control. 
								if ( sDEFAULT_VALUE.StartsWith("=") )
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_FIELD.EndsWith("_ID") )
								{
									// 01/15/2013 Paul.  If the field ends in ID, then the module name must be determined from the field. 
									string sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 3);
									if ( sDATA_FIELD == "ASSIGNED_USER_ID" )
										sPOPUP_TABLE_NAME = "USERS";
									else if ( sPOPUP_TABLE_NAME.EndsWith("Y") )
										sPOPUP_TABLE_NAME = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 4) + "IES";
									else if ( sPOPUP_TABLE_NAME != "PROJECT" && sPOPUP_TABLE_NAME != "PROJECT_TASK" )
										sPOPUP_TABLE_NAME += "S";
									string sPOPUP_MODULE_NAME = Crm.Modules.ModuleName(sPOPUP_TABLE_NAME);
									row["FIELD_TYPE"                ] = "ModulePopup";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sPOPUP_MODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["MODULE_TYPE"               ] = sPOPUP_MODULE_NAME;
									row["DISPLAY_FIELD"             ] = sDATA_FIELD.Substring(0, sDATA_FIELD.Length - 2) + "NAME";
									// 04/09/2011 Paul.  Auto-submit the selection. 
									// Auto-submit is not working because we need to hit the actual submit button in order to get parameter processing. 
									//row["DATA_FORMAT"               ] = "1";
								}
								else if ( sDATA_FIELD.StartsWith("DATETIME_") || sDATA_FIELD.EndsWith("_DATETIME") || sDATA_FIELD.Contains("_DATETIME_") )
								{
									row["FIELD_TYPE"                ] = "DateTimePicker";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								else if ( sDATA_TYPE == "DateTime" || sDATA_FIELD.StartsWith("DATE_") || sDATA_FIELD.EndsWith("_DATE") || sDATA_FIELD.Contains("_DATE_") )
								{
									row["FIELD_TYPE"                ] = "DatePicker";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
								else if ( !Sql.IsEmptyString(sDATA_SET_NAME) )
								{
									row["FIELD_TYPE"                ] = "ListBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
									row["CACHE_NAME"                ] = sDATA_SET_NAME;
									if ( bMULTI_VALUE )
										row["FORMAT_ROWS"] = 4;
								}
								else
								{
									row["FIELD_TYPE"                ] = "TextBox";
									row["DATA_LABEL"                ] = !Sql.IsEmptyString(sDATA_LABEL) ? sDATA_LABEL : sMODULE_NAME + ".LBL_" + sDATA_FIELD;
									row["DATA_FIELD"                ] = sDATA_FIELD;
								}
								//row["DATA_FORMAT"               ] = rowEditView["DATA_FORMAT"               ];
								//row["DISPLAY_FIELD"             ] = rowEditView["DISPLAY_FIELD"             ];
								//row["CACHE_NAME"                ] = rowEditView["CACHE_NAME"                ];
								//row["DATA_REQUIRED"             ] = rowEditView["DATA_REQUIRED"             ];
								//row["UI_REQUIRED"               ] = rowEditView["UI_REQUIRED"               ];
								//row["ONCLICK_SCRIPT"            ] = rowEditView["ONCLICK_SCRIPT"            ];
								//row["FORMAT_SCRIPT"             ] = rowEditView["FORMAT_SCRIPT"             ];
								//row["FORMAT_TAB_INDEX"          ] = rowEditView["FORMAT_TAB_INDEX"          ];
								//row["FORMAT_MAX_LENGTH"         ] = rowEditView["FORMAT_MAX_LENGTH"         ];
								//row["FORMAT_SIZE"               ] = rowEditView["FORMAT_SIZE"               ];
								//row["FORMAT_ROWS"               ] = rowEditView["FORMAT_ROWS"               ];
								//row["FORMAT_COLUMNS"            ] = rowEditView["FORMAT_COLUMNS"            ];
								//row["COLSPAN"                   ] = rowEditView["COLSPAN"                   ];
								//row["ROWSPAN"                   ] = rowEditView["ROWSPAN"                   ];
								//row["LABEL_WIDTH"               ] = rowEditView["LABEL_WIDTH"               ];
								//row["FIELD_WIDTH"               ] = rowEditView["FIELD_WIDTH"               ];
								//row["DATA_COLUMNS"              ] = rowEditView["DATA_COLUMNS"              ];
								//row["MODULE_TYPE"               ] = rowEditView["MODULE_TYPE"               ];
							}
						}
					}
					Cache.Insert("vwCHARTS.Parameters.EditView." + gID.ToString() + "." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 05/03/2011 Paul.  We need to include the USER_ID because we cache the Assigned User ID and the Team ID. 
		public static DataTable ChartParameters(Guid gID, Guid gUSER_ID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCHARTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add("NAME"         , typeof(String));
				dt.Columns.Add("MODULE_NAME"  , typeof(String));
				dt.Columns.Add("DATA_TYPE"    , typeof(String));  // String, Boolean, DateTime, Integer, Float
				dt.Columns.Add("NULLABLE"     , typeof(bool  ));
				dt.Columns.Add("ALLOW_BLANK"  , typeof(bool  ));
				dt.Columns.Add("MULTI_VALUE"  , typeof(bool  ));
				// 02/03/2012 Paul.  Add support for the Hidden flag. 
				dt.Columns.Add("HIDDEN"       , typeof(bool  ));
				dt.Columns.Add("PROMPT"       , typeof(String));
				dt.Columns.Add("DEFAULT_VALUE", typeof(String));
				// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
				dt.Columns.Add("DATA_SET_NAME"   , typeof(String));
				
				try
				{
					DataTable dtChart = SplendidCache.Chart(gID);
					if ( dtChart.Rows.Count > 0 )
					{
						DataRow rdr = dtChart.Rows[0];
						string sRDL         = Sql.ToString(rdr["RDL"        ]);
						string sMODULE_NAME = Sql.ToString(rdr["MODULE_NAME"]);
						
						RdlDocument rdl = new RdlDocument();
						rdl.LoadRdl(sRDL);
						rdl.NamespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						
						// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
						string sReportID = rdl.SelectNodeValue("rd:ReportID");
						XmlNodeList nlReportParameters = rdl.SelectNodesNS("ReportParameters/ReportParameter");
						foreach ( XmlNode xReportParameter in nlReportParameters )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							// 11/15/2011 Paul.  Must use rdl.SelectNodeValue to get the properties. 
							string sName         = XmlUtil.GetNamedItem    (xReportParameter, "Name"    );
							string sDataType     = rdl.SelectNodeValue(xReportParameter, "DataType");
							bool   bNullable     = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Nullable"  ));
							bool   bAllowBlank   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "AllowBlank"));
							bool   bMultiValue   = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "MultiValue"));
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							bool   bHidden       = Sql.ToBoolean(rdl.SelectNodeValue(xReportParameter, "Hidden"    ));
							string sPrompt       = rdl.SelectNodeValue(xReportParameter, "Prompt"  );
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							string sDataSetName    = rdl.SelectNodeValue(xReportParameter, "ValidValues/DataSetReference/DataSetName");
							string sDefaultValue   = rdl.SelectNodeValue(xReportParameter, "DefaultValue/Values/Value");
							// 02/16/2012 Paul.  Add support for specific parameter values. 
							XmlNodeList nlValidValues = rdl.SelectNodesNS(xReportParameter, "ValidValues/ParameterValues/ParameterValue");
							if ( nlValidValues.Count > 0 )
							{
								DataTable dtValidValues = new DataTable();
								dtValidValues.Columns.Add("VALUE", typeof(String));
								dtValidValues.Columns.Add("NAME" , typeof(String));
								foreach ( XmlNode xValidValue in nlValidValues )
								{
									DataRow rowValid = dtValidValues.NewRow();
									rowValid["VALUE"] = rdl.SelectNodeValue(xValidValue, "Value");
									rowValid["NAME" ] = rdl.SelectNodeValue(xValidValue, "Label");
									dtValidValues.Rows.Add(rowValid);
								}
								SplendidCache.AddReportSource(sReportID + "." + sName + ".SpecificValues", "VALUE", "NAME", dtValidValues);
								row["DATA_SET_NAME"] = sReportID + "." + sName + ".SpecificValues";
							}
							XmlNodeList nlDefaultValues = rdl.SelectNodesNS(xReportParameter, "DefaultValue/Values");
							if ( nlDefaultValues.Count > 0 )
							{
								if ( bMultiValue )
								{
									XmlDocument xml = new XmlDocument();
									xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
									xml.AppendChild(xml.CreateElement("Values"));
									foreach ( XmlNode xDefaultValue in nlDefaultValues )
									{
										XmlNode xValue = xml.CreateElement("Value");
										xml.DocumentElement.AppendChild(xValue);
										// 10/05/2012 Paul.  Check default value for null, not new value. 
										bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
										if ( !bNull )
											xValue.InnerText = xDefaultValue.InnerText;
									}
									row["DEFAULT_VALUE"] = xml.OuterXml;
								}
								else
								{
									// <Value xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:nil="true" />
									XmlNode xDefaultValue = nlDefaultValues[0];
									bool bNull = Sql.ToBoolean(XmlUtil.GetNamedItem(xDefaultValue, "xsi:nil"));
									if ( !bNull )
										row["DEFAULT_VALUE"] = Sql.ToString(xDefaultValue.InnerText);
								}
							}
							
							row["NAME"       ] = sName       ;
							row["MODULE_NAME"] = sMODULE_NAME;
							row["DATA_TYPE"  ] = sDataType   ;
							row["NULLABLE"   ] = bNullable   ;
							row["ALLOW_BLANK"] = bAllowBlank ;
							row["MULTI_VALUE"] = bMultiValue ;
							// 02/03/2012 Paul.  Add support for the Hidden flag. 
							row["HIDDEN"     ] = bHidden     ;
							row["PROMPT"     ] = sPrompt     ;
							// 02/16/2012 Paul.  We need a separate list for report parameter lists. 
							if ( !Sql.IsEmptyString(sDataSetName) )
								row["DATA_SET_NAME"] = sReportID + "." + sDataSetName;
							if ( String.Compare(sName, "ASSIGNED_USER_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.USER_ID.ToString();
							}
							else if ( String.Compare(sName, "TEAM_ID", true) == 0 )
							{
								row["DEFAULT_VALUE"] = Security.TEAM_ID.ToString();
							}
						}
					}
					Cache.Insert("vwCHARTS.Parameters." + gID.ToString() + "." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/29/2011 Paul.  Cache Charts. 
		public static DataTable Chart(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwCHARTS." + gID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *             " + ControlChars.CrLf
						     + "  from vwCHARTS_Edit" + ControlChars.CrLf
						     + " where ID = @ID      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwCHARTS." + gID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 10/29/2011 Paul.  Clear the Chart List on save. 
		public static void ClearCharts()
		{
			try
			{
				HttpContext.Current.Session.Remove("vwCHARTS_List." + Security.USER_ID.ToString());
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public static void ClearChart(Guid gID)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sID = gID.ToString();
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				if ( sKey.StartsWith("vwCHART.") && sKey.Contains(sID) )
					Cache.Remove(sKey);
			}
		}

		// 10/29/2011 Paul.  Place the chart list in the cache so that it would be available in SearchView. 
		public static DataTable Charts()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwCHARTS_List." + Security.USER_ID.ToString()] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID            " + ControlChars.CrLf
							     + "     , NAME          " + ControlChars.CrLf
							     + "  from vwCHARTS_List" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "Charts", "list");
								cmd.CommandText += " order by NAME" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwCHARTS_List." + Security.USER_ID.ToString()] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2011 Paul.  Cache the Sync Table data. 
		public static DataTable SyncTables(string sTABLE_NAME, bool bExcludeSystemTables)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwSYSTEM_SYNC_TABLES." + sTABLE_NAME] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select MODULE_NAME                " + ControlChars.CrLf
							     + "     , TABLE_NAME                 " + ControlChars.CrLf
							     + "     , VIEW_NAME                  " + ControlChars.CrLf
							     + "     , MODULE_SPECIFIC            " + ControlChars.CrLf
							     + "     , MODULE_FIELD_NAME          " + ControlChars.CrLf
							     + "     , IS_ASSIGNED                " + ControlChars.CrLf
							     + "     , ASSIGNED_FIELD_NAME        " + ControlChars.CrLf
							     + "     , HAS_CUSTOM                 " + ControlChars.CrLf
							     + "     , IS_RELATIONSHIP            " + ControlChars.CrLf
							     + "     , MODULE_NAME_RELATED        " + ControlChars.CrLf
							     + "  from vwSYSTEM_SYNC_TABLES       " + ControlChars.CrLf
							     + " where TABLE_NAME = @TABLE_NAME   " + ControlChars.CrLf;
							if ( bExcludeSystemTables )
								sSQL += "   and IS_SYSTEM = 0              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwSYSTEM_SYNC_TABLES." + sTABLE_NAME] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 06/03/2011 Paul.  Cache the Rest Table data. 
		public static DataTable RestTables(string sTABLE_NAME, bool bExcludeSystemTables)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwSYSTEM_REST_TABLES." + sTABLE_NAME] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 05/25/2011 Paul.  Tables available to the REST API are not bound by the SYNC_ENABLED flag. 
							// 09/28/2011 Paul.  Include the system flag so that we can cache only system tables. 
							sSQL = "select MODULE_NAME                " + ControlChars.CrLf
							     + "     , TABLE_NAME                 " + ControlChars.CrLf
							     + "     , VIEW_NAME                  " + ControlChars.CrLf
							     + "     , MODULE_SPECIFIC            " + ControlChars.CrLf
							     + "     , MODULE_FIELD_NAME          " + ControlChars.CrLf
							     + "     , IS_SYSTEM                  " + ControlChars.CrLf
							     + "     , IS_ASSIGNED                " + ControlChars.CrLf
							     + "     , ASSIGNED_FIELD_NAME        " + ControlChars.CrLf
							     + "     , HAS_CUSTOM                 " + ControlChars.CrLf
							     + "     , IS_RELATIONSHIP            " + ControlChars.CrLf
							     + "     , MODULE_NAME_RELATED        " + ControlChars.CrLf
							     + "  from vwSYSTEM_REST_TABLES       " + ControlChars.CrLf
							     + " where TABLE_NAME = @TABLE_NAME   " + ControlChars.CrLf;
							if ( bExcludeSystemTables )
								sSQL += "   and IS_SYSTEM = 0              " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								cmd.CommandTimeout = 0;
								Sql.AddParameter(cmd, "@TABLE_NAME", sTABLE_NAME);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwSYSTEM_REST_TABLES." + sTABLE_NAME] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearFavorites()
		{
			HttpContext.Current.Session.Remove("vwSUGARFAVORITES_MyFavorites");
		}

		public static DataTable Favorites()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwSUGARFAVORITES_MyFavorites"] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                           " + ControlChars.CrLf
							     + "  from vwSUGARFAVORITES_MyFavorites" + ControlChars.CrLf
							     + " where USER_ID = @USER_ID          " + ControlChars.CrLf
							     + " order by ITEM_SUMMARY             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwSUGARFAVORITES_MyFavorites"] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}

		// 10/02/2016 Paul.  Favorites for use on Arctic theme. 
		public static DataView Favorites(string sMODULE_NAME)
		{
			DataTable dtFavorites = Favorites();
			DataView vwFavorites = new DataView(dtFavorites);
			if ( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME != "Home" )
				vwFavorites.RowFilter = "MODULE_NAME = '" + sMODULE_NAME + "'";
			return vwFavorites;
		}

		// 10/09/2015 Paul.  Add support for subscriptions. 
		public static void ClearSubscriptions()
		{
			HttpContext.Current.Session.Remove("vwSUBSCRIPTIONS_MySubscriptions");
		}

		public static DataTable Subscriptions()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwSUBSCRIPTIONS_MySubscriptions"] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					// 11/17/2007 Paul.  New function to determine if user is authenticated. 
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                              " + ControlChars.CrLf
							     + "  from vwSUBSCRIPTIONS                " + ControlChars.CrLf
							     + " where SUBSCRIPTION_USER_ID = @USER_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwSUBSCRIPTIONS_MySubscriptions"] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 11/21/2005 Paul. Ignore error, but then we need to find a way to display the connection error. 
					// The most likely failure here is a connection failure. 
				}
			}
			return dt;
		}


		// 09/10/2012 Paul.  Add User Signatures. 
		public static void ClearUserSignatures()
		{
			HttpContext.Current.Session.Remove("vwUSERS_SIGNATURES");
		}

		// 09/10/2012 Paul.  Add User Signatures. 
		public static DataTable UserSignatures()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwUSERS_SIGNATURES"] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                             " + ControlChars.CrLf
							     + "     , NAME                           " + ControlChars.CrLf
							     + "     , SIGNATURE_HTML                 " + ControlChars.CrLf
							     + "  from vwUSERS_SIGNATURES             " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID             " + ControlChars.CrLf
							     + " order by PRIMARY_SIGNATURE desc, NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwUSERS_SIGNATURES"] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		// 04/20/2016 Paul.  Add team management to Outbound Emails. 
		public static void ClearOutboundMail()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				if ( sKey.StartsWith("vwOUTBOUND_EMAILS") )
					Cache.Remove(sKey);
			}
		}

		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		// 04/20/2016 Paul.  Add team management to Outbound Emails. 
		public static DataTable OutboundMail()
		{
			Guid gUSER_ID = Security.USER_ID;
			// 02/09/2017 Paul.  Change from Session to global cache. 
			//HttpSessionState Session = HttpContext.Current.Session;
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			//DataTable dt = Session["vwOUTBOUND_EMAILS." + gUSER_ID.ToString()] as DataTable;
			DataTable dt = Cache.Get("vwOUTBOUND_EMAILS." + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							// 01/17/2017 Paul.  Get all fields. 
							sSQL = "select *                " + ControlChars.CrLf
							     + "  from vwOUTBOUND_EMAILS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 04/20/2016 Paul.  Add team management to Outbound Emails. 
								// 04/20/2016 Paul.  We are not going to use the default filter as the Require Team flag can dramatically change the behavior. 
								// We want old records with null TEAM_ID to still be accessible, so we must use the left outer join. 
								//Security.Filter(cmd, "OutboundEmail", "list");
								bool bEnableTeamManagement  = Crm.Config.enable_team_management();
								bool bEnableDynamicTeams    = Crm.Config.enable_dynamic_teams();
								if ( bEnableTeamManagement )
								{
									if ( bEnableDynamicTeams )
									{
										cmd.CommandText += "  left outer join " + Sql.MetadataName(cmd, "vwTEAM_SET_MEMBERSHIPS_Security") + " vwTEAM_SET_MEMBERSHIPS" + ControlChars.CrLf;
										cmd.CommandText += "               on vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID = TEAM_SET_ID        " + ControlChars.CrLf;
										cmd.CommandText += "              and vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_USER_ID     = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
									}
									else
									{
										cmd.CommandText += "  left outer join vwTEAM_MEMBERSHIPS" + ControlChars.CrLf;
										cmd.CommandText += "               on vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID = TEAM_ID            " + ControlChars.CrLf;
										cmd.CommandText += "              and vwTEAM_MEMBERSHIPS.MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf;
									}
									Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
								}
								cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
								if ( bEnableTeamManagement )
								{
									if ( bEnableDynamicTeams )
										cmd.CommandText += "   and (TEAM_SET_ID is null or vwTEAM_SET_MEMBERSHIPS.MEMBERSHIP_TEAM_SET_ID is not null)" + ControlChars.CrLf;
									else
										cmd.CommandText += "   and (TEAM_ID is null or vwTEAM_MEMBERSHIPS.MEMBERSHIP_TEAM_ID is not null)" + ControlChars.CrLf;
								}
								cmd.CommandText += "   and (USER_ID = @USER_ID or USER_ID is null)" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
								cmd.CommandText += " order by USER_ID desc, DISPLAY_NAME  " + ControlChars.CrLf;
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									// 02/09/2017 Paul.  Change from Session to global cache. 
									//Session["vwOUTBOUND_EMAILS." + gUSER_ID.ToString()] = dt;
									Cache.Insert("vwOUTBOUND_EMAILS." + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 09/23/2013 Paul.  Add support for multiple outbound sms. 
		public static void ClearOutboundSms()
		{
			HttpContext.Current.Session.Remove("vwOUTBOUND_SMS");
		}

		// 09/23/2013 Paul.  Add support for multiple outbound sms. 
		public static DataTable OutboundSms()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			DataTable dt = Session["vwOUTBOUND_SMS"] as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID                           " + ControlChars.CrLf
							     + "     , NAME                         " + ControlChars.CrLf
							     + "     , FROM_NUMBER                  " + ControlChars.CrLf
							     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
							     + "  from vwOUTBOUND_SMS               " + ControlChars.CrLf
							     + " where USER_ID = @USER_ID           " + ControlChars.CrLf
							     + "    or USER_ID is null              " + ControlChars.CrLf
							     + " order by USER_ID desc, DISPLAY_NAME" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
									Session["vwOUTBOUND_SMS"] = dt;
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		// 12/24/2012 Paul.  Cache the activity reminders for 5 minutes. 
		public static void ClearUserReminders()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			Cache.Remove("vwACTIVITIES_Reminders." + Security.USER_ID.ToString());
		}

		// 12/24/2012 Paul.  Cache the activity reminders for 5 minutes. 
		public static DataTable UserReminders()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwACTIVITIES_Reminders." + Security.USER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 12/25/2012 Paul.  Date math is handled by the view. 
						sSQL = "select *                     " + ControlChars.CrLf
						     + "  from vwACTIVITIES_Reminders" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							// 06/02/2016 Paul.  Activities views will use new function that accepts an array of modules. 
							// 06/09/2017 Paul.  We need to include tasks. 
							// 12/03/2017 Paul.  Module name field needs to be a parameter because it can change between MODULE_NAME and ACTIVITY_TYPE. 
							Security.Filter(cmd, new string[] {"Calls", "Meetings", "Tasks"}, "list", "ASSIGNED_USER_ID", "ACTIVITY_TYPE");
							Sql.AppendParameter(cmd, Security.USER_ID, "USER_ID");
							cmd.CommandText += " order by DATE_START" + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwACTIVITIES_Reminders." + Security.USER_ID.ToString(), dt, null, CacheExpiration5Minutes(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearTwitterTracks()
		{
			HttpContext.Current.Session.Remove("vwTWITTER_TRACKS");
			HttpRuntime.Cache.Remove("vwTWITTER_TRACKS");
		}

		// 10/26/2013 Paul.  Add TwitterTrackers to the cache. 
		public static string MyTwitterTracks()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			if ( Session["vwTWITTER_TRACKS"] == null )
			{
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select distinct NAME              " + ControlChars.CrLf
							     + "  from vwTWITTER_TRACKS_Active    " + ControlChars.CrLf
							     + " where ASSIGNED_USER_ID = @USER_ID" + ControlChars.CrLf
							     + " order by NAME                    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_ID", Security.USER_ID);
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 10/27/2013 Paul.  Prebuild the tracks string so that a page request is fast. 
										StringBuilder sbTracks = new StringBuilder();
										if ( dt != null && dt.Rows.Count > 0 )
										{
											foreach ( DataRow row in dt.Rows )
											{
												string sNAME = Sql.ToString(row["NAME"]);
												if ( !Sql.IsEmptyString(sNAME) )
												{
													if ( sbTracks.Length > 0 )
														sbTracks.Append(',');
													// 10/27/2013 Paul.  The Twitter events are mapped in lowercase, make lowercase in advance. 
													sbTracks.Append(sNAME.ToLower());
												}
											}
										}
										Session["vwTWITTER_TRACKS"] = sbTracks.ToString();
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToString(Session["vwTWITTER_TRACKS"]);
		}

		public static DataTable TwitterTracks(HttpApplicationState Application)
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get("vwTWITTER_TRACKS") as DataTable;
			if ( dt == null )
			{
				dt = new DataTable();
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                   " + ControlChars.CrLf
						     + "     , TYPE                   " + ControlChars.CrLf
						     + "  from vwTWITTER_TRACKS_Active" + ControlChars.CrLf
						     + " order by NAME                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dt);
								Cache.Insert("vwTWITTER_TRACKS", dt, null, SplendidCache.DefaultCacheExpiration(), System.Web.Caching.Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static void ClearChatChannels()
		{
			HttpContext.Current.Session.Remove("vwCHAT_CHANNELS");
			HttpRuntime.Cache.Remove("vwCHAT_CHANNELS");
		}

		// 11/10/2014 Paul.  Add ChatChannels to the cache. 
		public static string MyChatChannels()
		{
			HttpSessionState Session = HttpContext.Current.Session;
			if ( Session["vwCHAT_CHANNELS"] == null )
			{
				try
				{
					if ( Security.IsAuthenticated() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select ID             " + ControlChars.CrLf
							     + "  from vwCHAT_CHANNELS" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, "ChatChannels", "list");
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										StringBuilder sbChannels = new StringBuilder();
										if ( dt != null && dt.Rows.Count > 0 )
										{
											foreach ( DataRow row in dt.Rows )
											{
												Guid gID = Sql.ToGuid(row["ID"]);
												if ( sbChannels.Length > 0 )
													sbChannels.Append(',');
												sbChannels.Append(gID.ToString());
											}
										}
										Session["vwCHAT_CHANNELS"] = sbChannels.ToString();
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToString(Session["vwCHAT_CHANNELS"]);
		}

		// 12/12/2015 Paul.  /n Software and .netCharge use different lists. 
		public static DataTable LibraryPaymentGateways()
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			string sLibraryListName = "payment_gateway_dom";
			// dotnetCharge.dll or nsoftware.InPayWeb.dll. 
			if ( Sql.ToString(Application["CONFIG.PaymentGateway.Library"]) == "nsoftware.InPayWeb" )
				sLibraryListName = "payment_gateway_nsoftware";
			DataTable dt = Cache.Get("vwAPI_PAYMENT_GATEWAYS." + sLibraryListName) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME                         " + ControlChars.CrLf
						     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY                " + ControlChars.CrLf
						     + " where lower(LIST_NAME) = @LIST_NAME" + ControlChars.CrLf
						     + "   and lower(LANG     ) = @LANG     " + ControlChars.CrLf
						     + " order by LIST_ORDER                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@LIST_NAME", sLibraryListName);
							// 12/12/2015 Paul.  Use the english list as the primary location. 
							Sql.AddParameter(cmd, "@LANG"     , "en-us");
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								Cache.Insert("vwAPI_PAYMENT_GATEWAYS." + sLibraryListName, dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		// 12/16/2015 Paul.  credit_card_year should be a custom list that adds 10 years to current year. 
		public static DataTable CreditCardYears()
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			DataTable dt = Cache.Get(".credit_card_year") as DataTable;
			if ( dt == null )
			{
				try
				{
					dt = new DataTable();
					dt.Columns.Add("NAME"        );
					dt.Columns.Add("DISPLAY_NAME");
					for ( int i = DateTime.Today.Year - 1; i <  DateTime.Today.Year + 10; i++ )
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						row["NAME"        ] = i.ToString();
						row["DISPLAY_NAME"] = i.ToString();
					}
					Cache.Insert(".credit_card_year", dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					// 10/16/2005 Paul. Ignore list errors. 
				}
			}
			return dt;
		}

		public static DataTable StreamModules(Guid gUSER_ID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			L10N L10n = new L10N(HttpContext.Current.Session["USER_SETTINGS/CULTURE"] as string);
			DataTable dt = Cache.Get(L10n.NAME + ".vwMODULES_Stream_ByUser_" + gUSER_ID.ToString()) as DataTable;
			if ( dt == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select MODULE_NAME             " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "  from vwMODULES_Stream_ByUser " + ControlChars.CrLf
						     + " where USER_ID = @USER_ID      " + ControlChars.CrLf
						     + " order by MODULE_NAME          " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								dt = new DataTable();
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									row["DISPLAY_NAME"] = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
								}
								Cache.Insert(L10n.NAME + ".vwMODULES_Stream_ByUser_" + gUSER_ID.ToString(), dt, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return dt;
		}

		public static List<string> StreamModulesArray(Guid gUSER_ID)
		{
			List<string> arr = new List<string>();
			DataTable dt = StreamModules(gUSER_ID);
			foreach ( DataRow row in dt.Rows )
			{
				string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
				arr.Add(sMODULE_NAME);
			}
			return arr;
		}

		// 10/05/2017 Paul.  Add Archive relationship view. 
		public static void ClearArchiveViewExists()
		{
			System.Web.Caching.Cache Cache = HttpRuntime.Cache;
			foreach(DictionaryEntry oKey in Cache)
			{
				string sKey = oKey.Key.ToString();
				if ( sKey.StartsWith("ArchiveExists.") )
					Cache.Remove(sKey);
			}
		}

		// 10/05/2017 Paul.  Add Archive relationship view. 
		public static bool ArchiveViewExists(string sVIEW_NAME)
		{
			string sARCHIVE_VIEW = sVIEW_NAME + "_ARCHIVE";
			Cache Cache = System.Web.HttpRuntime.Cache;
			object oExists = Cache.Get("ArchiveViewExists." + sARCHIVE_VIEW);
			if ( oExists == null )
			{
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select count(*)              " + ControlChars.CrLf
						     + "  from vwSqlViews            " + ControlChars.CrLf
						     + " where VIEW_NAME = @VIEW_NAME" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@VIEW_NAME", Sql.MetadataName(con, sARCHIVE_VIEW));
							oExists = Sql.ToBoolean(cmd.ExecuteScalar());
							Cache.Insert("ArchiveViewExists." + sARCHIVE_VIEW, oExists, null, DefaultCacheExpiration(), Cache.NoSlidingExpiration);
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(HttpContext.Current.Application, "Error", new StackTrace(true).GetFrame(0), ex);
				}
			}
			return Sql.ToBoolean(oExists);
		}
	}
}


