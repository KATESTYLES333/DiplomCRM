/*
 * Copyright (C) 2011 SplendidCRM Software, Inc. All Rights Reserved. 
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and 
 * trademarks, in and to the contents of this file.  You will not link to or in any way combine the 
 * contents of this file or any derivatives with any Open Source Code in any manner that would require 
 * the contents of this file to be made available to any third party. 
 * 
 * IN NO EVENT SHALL SPLENDIDCRM BE RESPONSIBLE FOR ANY DAMAGES OF ANY KIND, INCLUDING ANY DIRECT, 
 * SPECIAL, PUNITIVE, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES.  Other limitations of liability 
 * and disclaimers set forth in the License. 
 * 
 */
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM
{
	// http://www.odata.org/developers/protocols/json-format
	// http://brennan.offwhite.net/blog/2008/10/21/simple-wcf-and-ajax-integration/
	[ServiceContract]
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
	public class Rest
	{
		#region Scalar functions
		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public string Version()
		{
			// 03/10/2011 Paul.  We do not need to set the content type because the default is json. 
			//WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return Sql.ToString(HttpContext.Current.Application["SplendidVersion"]);
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public string Edition()
		{
			//WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return Sql.ToString(HttpContext.Current.Application["CONFIG.service_level"]);
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public DateTime UtcTime()
		{
			//WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return DateTime.UtcNow;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public bool IsAuthenticated()
		{
			//WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
			return Security.IsAuthenticated();
		}
		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Guid GetUserID()
		{
			if ( Security.IsAuthenticated() )
				return Security.USER_ID;
			else
				return Guid.Empty;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public string GetUserName()
		{
			if ( Security.IsAuthenticated() )
				return Security.USER_NAME;
			else
				return String.Empty;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Guid GetTeamID()
		{
			if ( Security.IsAuthenticated() )
				return Security.TEAM_ID;
			else
				return Guid.Empty;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public string GetTeamName()
		{
			if ( Security.IsAuthenticated() )
				return Security.TEAM_NAME;
			else
				return String.Empty;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public string GetUserLanguage()
		{
			if ( Security.IsAuthenticated() )
				return Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]);
			else
				return "en-US";
		}

		public class UserProfile
		{
			public Guid   USER_ID         ;
			public string USER_NAME       ;
			public string FULL_NAME       ;
			public Guid   TEAM_ID         ;
			public string TEAM_NAME       ;
			public string USER_LANG       ;
			public string USER_DATE_FORMAT;
			public string USER_TIME_FORMAT;
			// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
			public string USER_THEME      ;
			public string USER_CURRENCY_ID;
			public string USER_TIMEZONE_ID;
			// 11/21/2014 Paul.  Add User Picture. 
			public string PICTURE         ;
			// 12/01/2014 Paul.  Add SignalR fields. 
			public string USER_EXTENSION     ;
			public string USER_FULL_NAME     ;
			public string USER_PHONE_WORK    ;
			public string USER_SMS_OPT_IN    ;
			public string USER_PHONE_MOBILE  ;
			public string USER_TWITTER_TRACKS;
			public string USER_CHAT_CHANNELS ;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public UserProfile GetUserProfile()
		{
			if ( Security.IsAuthenticated() )
			{
				UserProfile profile = new UserProfile();
				profile.USER_ID          = Security.USER_ID  ;
				profile.USER_NAME        = Security.USER_NAME;
				profile.FULL_NAME        = Security.FULL_NAME;
				profile.TEAM_ID          = Security.TEAM_ID  ;
				profile.TEAM_NAME        = Security.TEAM_NAME;
				profile.USER_LANG        = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"   ]);
				profile.USER_DATE_FORMAT = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/DATEFORMAT"]);
				profile.USER_TIME_FORMAT = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/TIMEFORMAT"]);
				// 04/23/2013 Paul.  The HTML5 Offline Client now supports Atlantic theme. 
				profile.USER_THEME       = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/THEME"     ]);
				profile.USER_CURRENCY_ID = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CURRENCY"  ]);
				profile.USER_TIMEZONE_ID = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"  ]);
				// 11/21/2014 Paul.  Add User Picture. 
				profile.PICTURE          = Security.PICTURE  ;
				// 12/01/2014 Paul.  Add SignalR fields. 
				profile.USER_EXTENSION      = Sql.ToString(HttpContext.Current.Session["EXTENSION"   ]);
				profile.USER_FULL_NAME      = Sql.ToString(HttpContext.Current.Session["FULL_NAME"   ]);
				profile.USER_PHONE_WORK     = Sql.ToString(HttpContext.Current.Session["PHONE_WORK"  ]);
				profile.USER_SMS_OPT_IN     = Sql.ToString(HttpContext.Current.Session["SMS_OPT_IN"  ]);
				profile.USER_PHONE_MOBILE   = Sql.ToString(HttpContext.Current.Session["PHONE_MOBILE"]);
				profile.USER_TWITTER_TRACKS = SplendidCache.MyTwitterTracks();
				profile.USER_CHAT_CHANNELS  = SplendidCache.MyChatChannels();
				
				// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
				SplendidSession.CreateSession(HttpContext.Current.Session);
				return profile;
			}
			else
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
		}
		#endregion

		#region json utils
		// http://msdn.microsoft.com/en-us/library/system.datetime.ticks.aspx
		private static long UnixTicks(DateTime dt)
		{
			return (dt.Ticks - 621355968000000000) / 10000;
		}

		private static string ToJsonDate(object dt)
		{
			return "\\/Date(" + UnixTicks(Sql.ToDateTime(dt)).ToString() + ")\\/";
		}

		// 08/03/2012 Paul.  FromJsonDate is used on Web Capture services. 
		public static DateTime FromJsonDate(string s)
		{
			DateTime dt = DateTime.MinValue;
			if ( s.StartsWith("\\/Date(") && s.EndsWith(")\\/") )
			{
				s = s.Replace("\\/Date(", "");
				s = s.Replace(")\\/", "");
				long lEpoch = Sql.ToLong(s);
				dt = new DateTime(lEpoch * 10000 + 621355968000000000);
			}
			else
			{
				dt = Sql.ToDateTime(s);
			}
			return dt;
		}

		// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
		// http://schotime.net/blog/index.php/2008/07/27/dataset-datatable-to-json/
		private static List<Dictionary<string, object>> RowsToDictionary(string sBaseURI, string sModuleName, DataTable dt, TimeZone T10n)
		{
			List<Dictionary<string, object>> objs = new List<Dictionary<string, object>>();
			// 10/11/2012 Paul.  dt will be null when no results security filter applied. 
			if ( dt != null )
			{
				foreach (DataRow dr in dt.Rows)
				{
					// 06/28/2011 Paul.  Now that we have switched to using views, the results may not have an ID column. 
					Dictionary<string, object> drow = new Dictionary<string, object>();
					if ( dt.Columns.Contains("ID") )
					{
						Guid gID = Sql.ToGuid(dr["ID"]);
						if ( !Sql.IsEmptyString(sBaseURI) && !Sql.IsEmptyString(sModuleName) )
						{
							Dictionary<string, object> metadata = new Dictionary<string, object>();
							metadata.Add("uri", sBaseURI + "?ModuleName=" + sModuleName + "&ID=" + gID.ToString() + "");
							metadata.Add("type", "SplendidCRM." + sModuleName);
							if ( dr.Table.Columns.Contains("DATE_MODIFIED_UTC") )
							{
								DateTime dtDATE_MODIFIED_UTC = Sql.ToDateTime(dr["DATE_MODIFIED_UTC"]);
								metadata.Add("etag", gID.ToString() + "." + dtDATE_MODIFIED_UTC.Ticks.ToString() );
							}
							drow.Add("__metadata", metadata);
						}
					}
				
					for (int i = 0; i < dt.Columns.Count; i++)
					{
						if ( dt.Columns[i].DataType.FullName == "System.DateTime" )
						{
							// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
							drow.Add(dt.Columns[i].ColumnName, ToJsonDate(T10n.FromServerTime(dr[i])) );
						}
						else
						{
							drow.Add(dt.Columns[i].ColumnName, dr[i]);
						}
					}
					objs.Add(drow);
				}
			}
			return objs;
		}

		// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
		private static Dictionary<string, object> ToJson(string sBaseURI, string sModuleName, DataTable dt, TimeZone T10n)
		{
			Dictionary<string, object> d = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			results.Add("results", RowsToDictionary(sBaseURI, sModuleName, dt, T10n));
			d.Add("d", results);
			if ( dt != null )
				d.Add("__count", dt.Rows.Count.ToString());
			return d;
		}

		// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
		private static Dictionary<string, object> ToJson(string sBaseURI, string sModuleName, DataRow dr, TimeZone T10n)
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> drow    = new Dictionary<string, object>();
			
			// 06/28/2011 Paul.  Now that we have switched to using views, the results may not have an ID column. 
			if ( dr.Table.Columns.Contains("ID") )
			{
				Guid gID = Sql.ToGuid(dr["ID"]);
				if ( !Sql.IsEmptyString(sBaseURI) && !Sql.IsEmptyString(sModuleName) )
				{
					Dictionary<string, object> metadata = new Dictionary<string, object>();
					metadata.Add("uri", sBaseURI + "?ModuleName=" + sModuleName + "&ID=" + gID.ToString() + "");
					metadata.Add("type", "SplendidCRM." + sModuleName);
					if ( dr.Table.Columns.Contains("DATE_MODIFIED_UTC") )
					{
						DateTime dtDATE_MODIFIED_UTC = Sql.ToDateTime(dr["DATE_MODIFIED_UTC"]);
						metadata.Add("etag", gID.ToString() + "." + dtDATE_MODIFIED_UTC.Ticks.ToString() );
					}
					drow.Add("__metadata", metadata);
				}
			}
			
			for (int i = 0; i < dr.Table.Columns.Count; i++)
			{
				if ( dr.Table.Columns[i].DataType.FullName == "System.DateTime" )
				{
					// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
					drow.Add(dr.Table.Columns[i].ColumnName, ToJsonDate(T10n.FromServerTime(dr[i])) );
				}
				else
				{
					drow.Add(dr.Table.Columns[i].ColumnName, dr[i]);
				}
			}
			
			results.Add("results", drow);
			d.Add("d", results);
			return d;
		}

		private static string ConvertODataFilter(string sFILTER, IDbCommand cmd)
		{
			// Logical Operators
			sFILTER = sFILTER.Replace(" eq true" , " eq 1");
			sFILTER = sFILTER.Replace(" eq false", " eq 0");
			sFILTER = sFILTER.Replace(" ne true" , " ne 1");
			sFILTER = sFILTER.Replace(" ne false", " ne 0");
			sFILTER = sFILTER.Replace(" gt ", " > ");
			sFILTER = sFILTER.Replace(" lt ", " < ");
			sFILTER = sFILTER.Replace(" eq ", " = ");
			sFILTER = sFILTER.Replace(" ne ", " <> ");
			// Arithmetic Operators
			sFILTER = sFILTER.Replace(" add ", " + ");
			sFILTER = sFILTER.Replace(" sub ", " - ");
			sFILTER = sFILTER.Replace(" mul ", " * ");
			sFILTER = sFILTER.Replace(" div ", " / ");
			sFILTER = sFILTER.Replace(" mod ", " % ");
			// Date Functions
			if ( Sql.IsSQLServer(cmd) )
			{
				//sFILTER = sFILTER.Replace("year("  , "dbo.fnDatePart('year', "  );
				//sFILTER = sFILTER.Replace("month(" , "dbo.fnDatePart('month', " );
				//sFILTER = sFILTER.Replace("day("   , "dbo.fnDatePart('day', "   );
				sFILTER = sFILTER.Replace("hour("  , "dbo.fnDatePart('hour', "  );
				sFILTER = sFILTER.Replace("minute(", "dbo.fnDatePart('minute', ");
				sFILTER = sFILTER.Replace("second(", "dbo.fnDatePart('second', ");
			}
			else
			{
				//sFILTER = sFILTER.Replace("year("  , "fnDatePart('year', "  );
				//sFILTER = sFILTER.Replace("month(" , "fnDatePart('month', " );
				//sFILTER = sFILTER.Replace("day("   , "fnDatePart('day', "   );
				sFILTER = sFILTER.Replace("hour("  , "fnDatePart('hour', "  );
				sFILTER = sFILTER.Replace("minute(", "fnDatePart('minute', ");
				sFILTER = sFILTER.Replace("second(", "fnDatePart('second', ");
			}
			// Math Functions
			int nStart = sFILTER.IndexOf("round(");
			while ( nStart > 0 )
			{
				int nEnd = sFILTER.IndexOf(")", nStart);
				if ( nEnd > 0 )
				{
					sFILTER = sFILTER.Substring(0, nEnd - 1) + ", 0" + sFILTER.Substring(nEnd - 1);
				}
				nStart = sFILTER.IndexOf("round(", nStart + 1);
			}
			// String Functions
			sFILTER = sFILTER.Replace("tolower(", "lower(");
			sFILTER = sFILTER.Replace("toupper(", "upper(");
			if ( Sql.IsSQLServer(cmd) )
			{
				sFILTER = sFILTER.Replace("length("     , "len(");
				sFILTER = sFILTER.Replace("trim("       , "dbo.fnTrim(");
				sFILTER = sFILTER.Replace("concat("     , "dbo.fnConcat(");
				sFILTER = sFILTER.Replace("startswith(" , "dbo.fnStartsWith(");
				sFILTER = sFILTER.Replace("endswith("   , "dbo.fnEndsWith(");
				sFILTER = sFILTER.Replace("indexof("    , "dbo.fnIndexOf(");
				sFILTER = sFILTER.Replace("substringof(", "dbo.fnSubstringOf(");
			}
			return sFILTER;
		}

		private static Stream ToJsonStream(Dictionary<string, object> d)
		{
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		private static List<string> AccessibleModules()
		{
			List<string> lstMODULES = SplendidCache.AccessibleModules(HttpContext.Current, Security.USER_ID);
			if ( Crm.Config.enable_team_management() )
			{
				if ( !lstMODULES.Contains("Teams") )
					lstMODULES.Add("Teams");
			}
			// 11/08/2009 Paul.  We need to combine the two module lists into a single list. 
			// 11/22/2009 Paul.  Simplify the logic by having a local list of system modules. 
			/*
			string[] arrSystemModules = new string[] { "ACL", "ACLActions", "ACLRoles", "Audit", "Config", "Currencies", "Dashlets"
			                                         , "DocumentRevisions", "DynamicButtons", "Export", "FieldValidators", "Import"
			                                         , "Merge", "Modules", "Offline", "Releases", "Roles", "SavedSearch", "Shortcuts"
			                                         , "TeamNotices", "Terminology", "Users", "SystemSyncLog"
			                                         };
			string[] arrSystemModules = new string[] { "Currencies", "DocumentRevisions", "Releases" };
			foreach ( string sSystemModule in arrSystemModules )
				lstMODULES.Add(sSystemModule);
			*/
			lstMODULES.Add("Currencies"       );
			lstMODULES.Add("DocumentRevisions");
			lstMODULES.Add("Releases"         );
			// 11/30/2012 Paul.  Activities is a supported module so that we can get Open Activities and History to display in the HTML5 Offline Client. 
			lstMODULES.Add("Activities"       );
			// 12/02/2014 Paul.  Offline is a supported module so that we can show terms on the Mobile Client. 
			lstMODULES.Add("Offline"          );
			return lstMODULES;
		}
		#endregion

		#region Login
		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Guid Login(string UserName, string Password, string Version)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sUSER_NAME   = UserName;
			string sPASSWORD    = Password;
			string sVERSION     = Version ;
			Guid gUSER_ID       = Guid.Empty;
			Guid gUSER_LOGIN_ID = Guid.Empty;
			
			// 02/23/2011 Paul.  SYNC service should check for lockout. 
			if ( SplendidInit.LoginFailures(Application, sUSER_NAME) >= Crm.Password.LoginLockoutCount(Application) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_USER_LOCKED_OUT")));
			}
			// 04/16/2013 Paul.  Allow system to be restricted by IP Address. 
			if ( SplendidInit.InvalidIPAddress(Application, Request.UserHostAddress) )
			{
				L10N L10n = new L10N("en-US");
				throw(new Exception(L10n.Term("Users.ERR_INVALID_IP_ADDRESS")));
			}
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				sSQL = "select ID                    " + ControlChars.CrLf
				     + "     , USER_NAME             " + ControlChars.CrLf
				     + "     , FULL_NAME             " + ControlChars.CrLf
				     + "     , IS_ADMIN              " + ControlChars.CrLf
				     + "     , STATUS                " + ControlChars.CrLf
				     + "     , PORTAL_ONLY           " + ControlChars.CrLf
				     + "     , TEAM_ID               " + ControlChars.CrLf
				     + "     , TEAM_NAME             " + ControlChars.CrLf
				     + "  from vwUSERS_Login         " + ControlChars.CrLf
				     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf
				     + "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					string sUSER_HASH = Security.HashPassword(sPASSWORD);
					// 12/25/2009 Paul.  Use lowercase username to match the primary authentication function. 
					Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME.ToLower());
					Sql.AddParameter(cmd, "@USER_HASH", sUSER_HASH);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( dt.Rows.Count > 0 )
							{
								DataRow row = dt.Rows[0];
								Security.USER_ID     = Sql.ToGuid   (row["ID"         ]);
								Security.USER_NAME   = Sql.ToString (row["USER_NAME"  ]);
								Security.FULL_NAME   = Sql.ToString (row["FULL_NAME"  ]);
								Security.IS_ADMIN    = Sql.ToBoolean(row["IS_ADMIN"   ]);
								Security.PORTAL_ONLY = Sql.ToBoolean(row["PORTAL_ONLY"]);
								Security.TEAM_ID     = Sql.ToGuid   (row["TEAM_ID"    ]);
								Security.TEAM_NAME   = Sql.ToString (row["TEAM_NAME"  ]);
								gUSER_ID = Sql.ToGuid(row["ID"]);

								SplendidInit.LoadUserPreferences(gUSER_ID, String.Empty, String.Empty);
								SplendidInit.LoadUserACL(gUSER_ID);
								
								SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, gUSER_ID, sUSER_NAME, "Anonymous", "Succeeded", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
								Security.USER_LOGIN_ID = gUSER_LOGIN_ID;
								// 02/20/2011 Paul.  Log the success so that we can lockout the user. 
								SplendidInit.LoginTracking(Application, sUSER_NAME, true);
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SyncUser login for " + sUSER_NAME);
							}
							else
							{
								SqlProcs.spUSERS_LOGINS_InsertOnly(ref gUSER_LOGIN_ID, Guid.Empty, sUSER_NAME, "Anonymous", "Failed", Session.SessionID, Request.UserHostName, Request.Url.Host, Request.Path, Request.AppRelativeCurrentExecutionFilePath, Request.UserAgent);
								// 02/20/2011 Paul.  Log the failure so that we can lockout the user. 
								SplendidInit.LoginTracking(Application, sUSER_NAME, false);
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "SECURITY: failed attempted login for " + sUSER_NAME + " using Sync api");
							}
						}
					}
				}
			}
			if ( gUSER_ID == Guid.Empty )
			{
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "Invalid username and/or password for " + sUSER_NAME);
				throw(new Exception("Invalid username and/or password for " + sUSER_NAME));
			}
			return gUSER_ID;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void Logout()
		{
			try
			{
				Guid gUSER_LOGIN_ID = Security.USER_LOGIN_ID;
				if ( !Sql.IsEmptyGuid(gUSER_LOGIN_ID) )
					SqlProcs.spUSERS_LOGINS_Logout(gUSER_LOGIN_ID);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
			HttpContext.Current.Session.Abandon();
			// 11/15/2014 Paul.  Prevent resuse of SessionID. 
			// http://support.microsoft.com/kb/899918
			HttpContext.Current.Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
		}
		#endregion

		// 10/12/2012 Paul.  Instead of making a request for each module, create Get All requests to build the cache more quickly. 
		#region Get System Layout
		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllGridViewsColumns()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                                         " + ControlChars.CrLf
						     + "  from vwGRIDVIEWS_COLUMNS                       " + ControlChars.CrLf
						     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by GRID_NAME, COLUMN_INDEX                " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									bool bClearScript = false;
									string sLAST_VIEW_NAME = String.Empty;
									List<Dictionary<string, object>> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sGRID_NAME   = Sql.ToString(row["GRID_NAME" ]);
										string sDATA_FIELD  = Sql.ToString(row["DATA_FIELD"]);
										string sMODULE_NAME = String.Empty;
										string[] arrGRID_NAME = sGRID_NAME.Split('.');
										if ( arrGRID_NAME.Length > 0 )
										{
											if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
												sMODULE_NAME = arrGRID_NAME[0];
											else if ( Sql.ToBoolean(Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
												sMODULE_NAME = arrGRID_NAME[1];
											else
												sMODULE_NAME = arrGRID_NAME[0];
										}
										if ( !lstMODULES.Contains(sMODULE_NAME) )
											continue;
										if ( sLAST_VIEW_NAME != sGRID_NAME )
										{
											bClearScript = false;
											sLAST_VIEW_NAME = sGRID_NAME;
											layout = new List<Dictionary<string, object>>();
											objs.Add(sLAST_VIEW_NAME, layout);
										}
										bool bIsReadable = true;
										if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
										{
											Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
											bIsReadable  = acl.IsReadable();
										}
										// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
										if ( bClearScript )
											row["SCRIPT"] = DBNull.Value;
										bClearScript = true;
										if ( bIsReadable )
										{
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											layout.Add(drow);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllDetailViewsFields()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                                         " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_FIELDS                      " + ControlChars.CrLf
						     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by DETAIL_NAME, FIELD_INDEX               " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									bool bClearScript = false;
									string sLAST_VIEW_NAME = String.Empty;
									List<Dictionary<string, object>> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sDETAIL_NAME = Sql.ToString (row["DETAIL_NAME"]);
										string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD" ]);
										string sMODULE_NAME = String.Empty;
										string[] arrDETAIL_NAME = sDETAIL_NAME.Split('.');
										if ( arrDETAIL_NAME.Length > 0 )
											sMODULE_NAME = arrDETAIL_NAME[0];
										if ( !lstMODULES.Contains(sMODULE_NAME) )
											continue;
										if ( sLAST_VIEW_NAME != sDETAIL_NAME )
										{
											bClearScript = false;
											sLAST_VIEW_NAME = sDETAIL_NAME;
											layout = new List<Dictionary<string, object>>();
											objs.Add(sLAST_VIEW_NAME, layout);
										}
										bool bIsReadable  = true;
										if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
										{
											// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
											Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
											bIsReadable  = acl.IsReadable();
										}
										// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
										if ( bClearScript )
											row["SCRIPT"] = DBNull.Value;
										bClearScript = true;
										if ( bIsReadable )
										{
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											layout.Add(drow);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllEditViewsFields()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						bool bSearchView = false;
						sSQL = "select *                                         " + ControlChars.CrLf
						     + "  from " + (bSearchView ? "vwEDITVIEWS_FIELDS_SearchView" : "vwEDITVIEWS_FIELDS") + ControlChars.CrLf
						     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by EDIT_NAME, FIELD_INDEX                 " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									bool bClearScript = false;
									string sLAST_VIEW_NAME = String.Empty;
									List<Dictionary<string, object>> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sEDIT_NAME     = Sql.ToString(row["EDIT_NAME"    ]);
										string sFIELD_TYPE    = Sql.ToString(row["FIELD_TYPE"   ]);
										string sDATA_FIELD    = Sql.ToString(row["DATA_FIELD"   ]);
										string sDATA_FORMAT   = Sql.ToString(row["DATA_FORMAT"  ]);
										string sDISPLAY_FIELD = Sql.ToString(row["DISPLAY_FIELD"]);
										string sMODULE_NAME   = String.Empty;
										string[] arrEDIT_NAME = sEDIT_NAME.Split('.');
										if ( arrEDIT_NAME.Length > 0 )
											sMODULE_NAME = arrEDIT_NAME[0];
										if ( !lstMODULES.Contains(sMODULE_NAME) )
											continue;
										if ( sLAST_VIEW_NAME != sEDIT_NAME )
										{
											bClearScript = false;
											sLAST_VIEW_NAME = sEDIT_NAME;
											layout = new List<Dictionary<string, object>>();
											objs.Add(sLAST_VIEW_NAME, layout);
										}
										bool bIsReadable  = true;
										bool bIsWriteable = true;
										if ( SplendidInit.bEnableACLFieldSecurity )
										{
											// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
											Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
											bIsReadable  = acl.IsReadable();
											// 02/16/2011 Paul.  We should allow a Read-Only field to be searchable, so always allow writing if the name contains Search. 
											bIsWriteable = acl.IsWriteable() || sEDIT_NAME.Contains(".Search");
										}
										if ( !bIsReadable )
										{
											row["FIELD_TYPE"] = "Blank";
										}
										else if ( !bIsWriteable )
										{
											row["FIELD_TYPE"] = "Label";
										}
										// 09/20/2012 Paul.  We need a SCRIPT field that is form specific, but only on the first record of the layout. 
										if ( bClearScript )
											row["SCRIPT"] = DBNull.Value;
										bClearScript = true;
										Dictionary<string, object> drow = new Dictionary<string, object>();
										for ( int j = 0; j < dt.Columns.Count; j++ )
										{
											if ( dt.Columns[j].ColumnName == "ID" )
												continue;
											// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
											drow.Add(dt.Columns[j].ColumnName, row[j]);
										}
										layout.Add(drow);
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllDetailViewsRelationships()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 10/09/2012 Paul.  Make sure to filter by modules with REST enabled. 
						// 11/30/2012 Paul.  Activities is not a real table, but it should be included. 
						sSQL = "select *                                                            " + ControlChars.CrLf
						     + "  from vwDETAILVIEWS_RELATIONSHIPS                                  " + ControlChars.CrLf
						     + " where MODULE_NAME in (select MODULE_NAME from vwSYSTEM_REST_TABLES)" + ControlChars.CrLf
						     + "    or MODULE_NAME = 'Activities'                                   " + ControlChars.CrLf
						     + " order by DETAIL_NAME, RELATIONSHIP_ORDER                           " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									string sLAST_VIEW_NAME = String.Empty;
									List<Dictionary<string, object>> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sDETAIL_NAME = Sql.ToString(row["DETAIL_NAME"]);
										string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
										if ( !lstMODULES.Contains(sMODULE_NAME) )
											continue;
										if ( sLAST_VIEW_NAME != sDETAIL_NAME )
										{
											sLAST_VIEW_NAME = sDETAIL_NAME;
											layout = new List<Dictionary<string, object>>();
											objs.Add(sLAST_VIEW_NAME, layout);
										}
										bool bVisible = (SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "list") >= 0);
										if ( bVisible )
										{
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											layout.Add(drow);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllDynamicButtons()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select *                                         " + ControlChars.CrLf
						     + "  from vwDYNAMIC_BUTTONS                         " + ControlChars.CrLf
						     + " where (DEFAULT_VIEW = 0 or DEFAULT_VIEW is null)" + ControlChars.CrLf
						     + " order by VIEW_NAME, CONTROL_INDEX               " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									string sLAST_VIEW_NAME = String.Empty;
									List<Dictionary<string, object>> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sVIEW_NAME          = Sql.ToString (row["VIEW_NAME"         ]);
										string sCONTROL_TYPE       = Sql.ToString (row["CONTROL_TYPE"      ]);
										string sMODULE_NAME        = Sql.ToString (row["MODULE_NAME"       ]);
										string sMODULE_ACCESS_TYPE = Sql.ToString (row["MODULE_ACCESS_TYPE"]);
										string sTARGET_NAME        = Sql.ToString (row["TARGET_NAME"       ]);
										string sTARGET_ACCESS_TYPE = Sql.ToString (row["TARGET_ACCESS_TYPE"]);
										bool   bADMIN_ONLY         = Sql.ToBoolean(row["ADMIN_ONLY"        ]);
										// 10/13/2012 Paul.  Layouts that start with a dot are templates and can be ignored. 
										if ( sVIEW_NAME.StartsWith(".") )
											continue;
										if ( !Sql.IsEmptyString(sMODULE_NAME) && !lstMODULES.Contains(sMODULE_NAME) )
											continue;
										if ( !Sql.IsEmptyString(sTARGET_NAME) && !lstMODULES.Contains(sTARGET_NAME) )
											continue;
										if ( sLAST_VIEW_NAME != sVIEW_NAME )
										{
											sLAST_VIEW_NAME = sVIEW_NAME;
											layout = new List<Dictionary<string, object>>();
											objs.Add(sLAST_VIEW_NAME, layout);
										}
										bool bVisible = (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY);
										if ( String.Compare(sCONTROL_TYPE, "Button", true) == 0 || String.Compare(sCONTROL_TYPE, "HyperLink", true) == 0 || String.Compare(sCONTROL_TYPE, "ButtonLink", true) == 0 )
										{
											if ( bVisible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
											{
												int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
												// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
												//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
												if ( bVisible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
												{
													nACLACCESS = SplendidCRM.Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
													// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
													//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
												}
											}
										}
										if ( bVisible )
										{
											Dictionary<string, object> drow = new Dictionary<string, object>();
											for ( int j = 0; j < dt.Columns.Count; j++ )
											{
												if ( dt.Columns[j].ColumnName == "ID" )
													continue;
												// 10/13/2012 Paul.  Must not return value as a string as the client is expecting numerics and booleans in their native format. 
												drow.Add(dt.Columns[j].ColumnName, row[j]);
											}
											layout.Add(drow);
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllTerminology()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);
			
			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					List<string> lstMODULES = AccessibleModules();
					lstMODULES.Add("Users");
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select NAME               " + ControlChars.CrLf
						     + "     , MODULE_NAME        " + ControlChars.CrLf
						     + "     , LIST_NAME          " + ControlChars.CrLf
						     + "     , DISPLAY_NAME       " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY      " + ControlChars.CrLf
						     + " where lower(LANG) = @LANG" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
							Sql.AddParameter(cmd, "@LANG", L10n.NAME.ToLower());
							cmd.CommandText += "   and ( 1 = 0" + ControlChars.CrLf;
							cmd.CommandText += "         or MODULE_NAME is null" + ControlChars.CrLf;
							cmd.CommandText += "     ";
							Sql.AppendParameter(cmd, lstMODULES.ToArray(), "MODULE_NAME", true);
							cmd.CommandText += "       )" + ControlChars.CrLf;
							cmd.CommandText += " order by MODULE_NAME, LIST_NAME, NAME" + ControlChars.CrLf;
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									string sLAST_MODULE_NAME = String.Empty;
									objs.Add(L10n.NAME + ".Loaded", true);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sNAME         = Sql.ToString(row["NAME"        ]);
										string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
										string sLIST_NAME    = Sql.ToString(row["LIST_NAME"   ]);
										string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
										// 02/02/2013 Paul.  The HTML5 Offline Client and Browser Extensions crash because of an exception here. 
										// {"ExceptionDetail":{"HelpLink":null,"InnerException":null,"Message":"An item with the same key has already been added."}}
										// 02/02/2013 Paul.  Custom fields can have a table name instead of a module name. 
										if ( !Sql.IsEmptyString(sMODULE_NAME) && sMODULE_NAME == sMODULE_NAME.ToUpper() )
										{
											string sTABLE_NAME = sMODULE_NAME;
											if ( !Sql.IsEmptyString(Application["Modules." + sTABLE_NAME + ".ModuleName"]) )
												sMODULE_NAME = Sql.ToString(Application["Modules." + sTABLE_NAME + ".ModuleName"]);
										}
										if ( sLAST_MODULE_NAME != sMODULE_NAME )
										{
											sLAST_MODULE_NAME = sMODULE_NAME;
											objs.Add(L10n.NAME + "." + sMODULE_NAME + ".Loaded", true);
										}
										if ( !Sql.IsEmptyString(sLIST_NAME) )
											objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sNAME] = sDISPLAY_NAME;
										else
											objs[L10n.NAME + "." + sMODULE_NAME + "." + sNAME] = sDISPLAY_NAME;
									}
								}
								// 10/12/2012 Paul.  Since we are replacing the entire Terminology List object, we need to include custom lists. 
								using ( DataTable dt = SplendidCache.Currencies() )
								{
									string sLIST_NAME = "Currencies";
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID           = Sql.ToString(row["ID"         ]);
										string sDISPLAY_NAME = Sql.ToString(row["NAME_SYMBOL"]);
										objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
									}
								}
								using ( DataTable dt = SplendidCache.Release() )
								{
									string sLIST_NAME = "Release";
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID           = Sql.ToString(row["ID"  ]);
										string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
										objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
									}
								}
								using ( DataTable dt = SplendidCache.ContractTypes() )
								{
									string sLIST_NAME = "ContractTypes";
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID           = Sql.ToString(row["ID"  ]);
										string sDISPLAY_NAME = Sql.ToString(row["NAME"]);
										objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
									}
								}
								using ( DataTable dt = SplendidCache.AssignedUser() )
								{
									string sLIST_NAME = "AssignedUser";
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID           = Sql.ToString(row["ID"       ]);
										string sDISPLAY_NAME = Sql.ToString(row["USER_NAME"]);
										objs[L10n.NAME + "." + "." + sLIST_NAME + "." + sID] = sDISPLAY_NAME;
									}
								}
								// 02/24/2013 Paul.  Build Calendar names from culture instead of from terminology. 
								objs[L10n.NAME + ".Calendar.YearMonthPattern"] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.YearMonthPattern;
								objs[L10n.NAME + ".Calendar.MonthDayPattern" ] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthDayPattern;
								objs[L10n.NAME + ".Calendar.LongDatePattern" ] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
								objs[L10n.NAME + ".Calendar.ShortTimePattern"] = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/TIMEFORMAT"]);
								objs[L10n.NAME + ".Calendar.ShortDatePattern"] = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/DATEFORMAT"]);
								objs[L10n.NAME + ".Calendar.FirstDayOfWeek"  ] = ((int) System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek).ToString();
								for ( int i = 1; i <= 12; i++ )
								{
									string sID           = i.ToString();
									string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames[i- 1];
									objs[L10n.NAME + "." + ".month_names_dom." + sID] = sDISPLAY_NAME;
								}
								for ( int i = 1; i <= 12; i++ )
								{
									string sID           = i.ToString();
									string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[i- 1];
									objs[L10n.NAME + "." + ".short_month_names_dom." + sID] = sDISPLAY_NAME;
								}
								for ( int i = 0; i <= 6; i++ )
								{
									string sID           = i.ToString();
									string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames[i];
									objs[L10n.NAME + "." + ".day_names_dom." + sID] = sDISPLAY_NAME;
								}
								for ( int i = 0; i <= 6; i++ )
								{
									string sID           = i.ToString();
									string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i];
									objs[L10n.NAME + "." + ".short_day_names_dom." + sID] = sDISPLAY_NAME;
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAllTerminologyLists()
		{
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);

			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select distinct                " + ControlChars.CrLf
						     + "       NAME                    " + ControlChars.CrLf
						     + "     , DISPLAY_NAME            " + ControlChars.CrLf
						     + "     , LIST_NAME               " + ControlChars.CrLf
						     + "     , LIST_ORDER              " + ControlChars.CrLf
						     + "  from vwTERMINOLOGY           " + ControlChars.CrLf
						     + " where lower(LANG) = @LANG     " + ControlChars.CrLf
						     + "   and LIST_NAME is not null   " + ControlChars.CrLf
						     + " order by LIST_NAME, LIST_ORDER" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
							Sql.AddParameter(cmd, "@LANG", L10n.NAME.ToLower());
							
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									string sLAST_LIST_NAME = String.Empty;
									List<string> layout = null;
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sNAME      = Sql.ToString(row["NAME"     ]);
										string sLIST_NAME = Sql.ToString(row["LIST_NAME"]);
										if ( sLAST_LIST_NAME != sLIST_NAME )
										{
											sLAST_LIST_NAME = sLIST_NAME;
											layout = new List<string>();
											objs.Add(L10n.NAME + "." + sLAST_LIST_NAME, layout);
										}
										layout.Add(sNAME);
									}
								}
								// 10/12/2012 Paul.  Since we are replacing the entire Terminology List object, we need to include custom lists. 
								using ( DataTable dt = SplendidCache.Currencies() )
								{
									List<string> layout = new List<string>();
									objs.Add(L10n.NAME + ".Currencies", layout);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID = Sql.ToString(row["ID"]);
										layout.Add(sID);
									}
								}
								using ( DataTable dt = SplendidCache.Release() )
								{
									List<string> layout = new List<string>();
									objs.Add(L10n.NAME + ".Release", layout);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID = Sql.ToString(row["ID"]);
										layout.Add(sID);
									}
								}
								using ( DataTable dt = SplendidCache.ContractTypes() )
								{
									List<string> layout = new List<string>();
									objs.Add(L10n.NAME + ".ContractTypes", layout);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID = Sql.ToString(row["ID"]);
										layout.Add(sID);
									}
								}
								using ( DataTable dt = SplendidCache.AssignedUser() )
								{
									List<string> layout = new List<string>();
									objs.Add(L10n.NAME + ".AssignedUser", layout);
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										string sID = Sql.ToString(row["ID"]);
										layout.Add(sID);
									}
								}
								// 02/24/2013 Paul.  Build Calendar names from culture instead of from terminology. 
								List<string> lstMonthNames = new List<string>();
								objs.Add(L10n.NAME + ".month_names_dom", lstMonthNames);
								for ( int i = 1; i <= 12; i++ )
								{
									string sID = i.ToString();
									lstMonthNames.Add(sID);
								}
								List<string> lstShortMonthNames = new List<string>();
								objs.Add(L10n.NAME + ".short_month_names_dom", lstShortMonthNames);
								for ( int i = 1; i <= 12; i++ )
								{
									string sID = i.ToString();
									lstShortMonthNames.Add(sID);
								}
								List<string> lstDayNames = new List<string>();
								objs.Add(L10n.NAME + ".day_names_dom", lstDayNames);
								for ( int i = 0; i <= 6; i++ )
								{
									string sID = i.ToString();
									lstDayNames.Add(sID);
								}
								List<string> lstShortDayNames = new List<string>();
								objs.Add(L10n.NAME + ".short_day_names_dom", lstShortDayNames);
								for ( int i = 0; i <= 6; i++ )
								{
									string sID = i.ToString();
									lstShortDayNames.Add(sID);
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			d.Add("__count", objs.Count.ToString());
			return ToJsonStream(d);
		}

		#endregion

		#region Get
		// 08/11/2012 Paul.  Add ability to search phone numbers using REST API. 
		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream PhoneSearch(string PhoneNumber)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			if ( !Security.IsAuthenticated() )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			PhoneNumber = Utils.NormalizePhone(PhoneNumber);
			
			// Accounts, Contacts, Leads, Prospects, Calls
			DataTable dtPhones = new DataTable();
			dtPhones.Columns.Add("ID"         , Type.GetType("System.Guid"  ));
			dtPhones.Columns.Add("NAME"       , Type.GetType("System.String"));
			dtPhones.Columns.Add("MODULE_NAME", Type.GetType("System.String"));
			if ( !Sql.IsEmptyString(PhoneNumber) )
			{
				DataTable dtFields = SplendidCache.DetailViewRelationships("Home.PhoneSearch");
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						foreach ( DataRow rowModule in dtFields.Rows )
						{
							string sMODULE_NAME = Sql.ToString(rowModule["MODULE_NAME"]);
							int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "list");
							if ( sMODULE_NAME != "Calls" && nACLACCESS >= 0 )
							{
								string sSQL;
								sSQL = "select ID              " + ControlChars.CrLf
								     + "     , NAME            " + ControlChars.CrLf
								     + "  from vwPHONE_NUMBERS_" + Crm.Modules.TableName(Application, sMODULE_NAME) + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Security.Filter(cmd, sMODULE_NAME, "list");
								//Sql.AppendParameter(cmd, sPhoneNumber, Sql.SqlFilterMode.Contains, "NORMALIZED_NUMBER");
								SearchBuilder sb = new SearchBuilder(PhoneNumber, cmd);
								cmd.CommandText += sb.BuildQuery("   and ", "NORMALIZED_NUMBER");
								cmd.CommandText += "order by NAME";
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										foreach ( DataRow row in dt.Rows )
										{
											DataRow rowPhone = dtPhones.NewRow();
											rowPhone["ID"         ] = row["ID"  ];
											rowPhone["NAME"       ] = row["NAME"];
											rowPhone["MODULE_NAME"] = sMODULE_NAME;
											dtPhones.Rows.Add(rowPhone);
										}
									}
								}
							}
						}
					}
				}
			}
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath.Replace("/PhoneSearch", "/GetModuleItem");
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			string sResponse = json.Serialize(ToJson(sBaseURI, "Leads", dtPhones, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		// 10/16/2011 Paul.  HTML5 Offline Client needs access to the custom lists. 
		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetCustomList(string ListName)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			if ( Sql.IsEmptyString(ListName) )
				throw(new Exception("The list name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			if ( !Security.IsAuthenticated() )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			DataTable dt = new DataTable();
			dt.Columns.Add("NAME"        );
			dt.Columns.Add("DISPLAY_NAME");
			bool bCustomCache = false;
			// 02/24/2013 Paul.  Add custom calendar lists. 
			if ( ListName == "month_names_dom" )
			{
				for ( int i = 1; i <= 12; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames[i- 1];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "short_month_names_dom" )
			{
				for ( int i = 1; i <= 12; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames[i- 1];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "day_names_dom" )
			{
				for ( int i = 0; i <= 6; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DayNames[i];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else if ( ListName == "short_day_names_dom" )
			{
				for ( int i = 0; i <= 6; i++ )
				{
					string sID           = i.ToString();
					string sDISPLAY_NAME = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i];
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
					row["NAME"        ] = sID;
					row["DISPLAY_NAME"] = sDISPLAY_NAME;
				}
				bCustomCache = true;
			}
			else
			{
				SplendidCacheReference[] arrCustomCaches = SplendidCache.CustomCaches;
				foreach ( SplendidCacheReference cache in arrCustomCaches )
				{
					if ( cache.Name == ListName )
					{
						string sDataValueField = cache.DataValueField;
						string sDataTextField  = cache.DataTextField ;
						SplendidCacheCallback cbkDataSource = cache.DataSource;
						foreach ( DataRow rowCustom in cbkDataSource().Rows )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							row["NAME"        ] = Sql.ToString(rowCustom[sDataValueField]);
							row["DISPLAY_NAME"] = Sql.ToString(rowCustom[sDataTextField ]);
						}
						bCustomCache = true;
					}
				}
			}
			if ( !bCustomCache )
			{
				dt = SplendidCache.List(ListName);
			}
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			string sResponse = json.Serialize(ToJson(sBaseURI, ListName, dt, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetModuleTable(string TableName)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			int    nSKIP     = Sql.ToInteger(Request.QueryString["$skip"   ]);
			int    nTOP      = Sql.ToInteger(Request.QueryString["$top"    ]);
			string sFILTER   = Sql.ToString (Request.QueryString["$filter" ]);
			string sORDER_BY = Sql.ToString (Request.QueryString["$orderby"]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY = Sql.ToString (Request.QueryString["$groupby"]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT   = Sql.ToString (Request.QueryString["$select" ]);
			string[] arrItems = Request.QueryString.GetValues("Items");
			Guid[] Items = null;
			// 06/17/2011 Paul.  arrItems might be null. 
			if ( arrItems != null && arrItems.Length > 0 )
			{
				Items = new Guid[arrItems.Length];
				for ( int i = 0; i < arrItems.Length; i++ )
				{
					Items[i] = Sql.ToGuid(arrItems[i]);
				}
			}
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sFILTER_KEYWORDS = (" " + r.Replace(sFILTER, " ") + " ").ToLower();
			if ( sFILTER_KEYWORDS.Contains(" select ") )
			{
				throw(new Exception("Subqueries are not allowed."));
			}
			if ( sFILTER.Contains(";") )
			{
				// 06/18/2011 Paul.  This is to prevent the user from attempting to inject SQL. 
				throw(new Exception("A semicolon is not allowed anywhere in a filter. "));
			}
			if ( sORDER_BY.Contains(";") )
			{
				// 06/18/2011 Paul.  This is to prevent the user from attempting to inject SQL. 
				throw(new Exception("A semicolon is not allowed anywhere in a sort expression. "));
			}
			if ( !Security.IsAuthenticated() )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			// 08/22/2011 Paul.  Add admin control to REST API. 
			string sMODULE_NAME = Sql.ToString(Application["Modules." + TableName + ".ModuleName"]);
			// 08/22/2011 Paul.  Not all tables will have a module name, such as relationship tables. 
			// Tables will get another security filter later in the code. 
			if ( !Sql.IsEmptyString(sMODULE_NAME) )
			{
				int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "list");
				if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".RestEnabled"]) || nACLACCESS < 0 )
				{
					L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}
			
			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			DataTable dt = GetTable(TableName, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, Items);
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath.Replace("/GetModuleTable", "/GetModuleItem");
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			string sResponse = json.Serialize(ToJson(sBaseURI, sMODULE_NAME, dt, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetModuleList(string ModuleName)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			int    nSKIP     = Sql.ToInteger(Request.QueryString["$skip"   ]);
			int    nTOP      = Sql.ToInteger(Request.QueryString["$top"    ]);
			string sFILTER   = Sql.ToString (Request.QueryString["$filter" ]);
			string sORDER_BY = Sql.ToString (Request.QueryString["$orderby"]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY = Sql.ToString (Request.QueryString["$groupby"]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT   = Sql.ToString (Request.QueryString["$select" ]);
			
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sFILTER_KEYWORDS = (" " + r.Replace(sFILTER, " ") + " ").ToLower();
			if ( sFILTER_KEYWORDS.Contains(" select ") )
				throw(new Exception("Subqueries are not allowed."));

			UniqueStringCollection arrSELECT = new UniqueStringCollection();
			sSELECT = sSELECT.Replace(" ", "");
			if ( !Sql.IsEmptyString(sSELECT) )
			{
				foreach ( string s in sSELECT.Split(',') )
				{
					string sColumnName = r.Replace(s, "");
					if ( !Sql.IsEmptyString(sColumnName) )
						arrSELECT.Add(sColumnName);
				}
			}
			
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			DataTable dt = GetTable(sTABLE_NAME, nSKIP, nTOP, sFILTER, sORDER_BY, sGROUP_BY, arrSELECT, null);
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath.Replace("/GetModuleList", "/GetModuleItem");
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			string sResponse = json.Serialize(ToJson(sBaseURI, ModuleName, dt, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetModuleItem(string ModuleName, Guid ID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "view");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			Guid[] arrITEMS = new Guid[1] { ID };
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			DataTable dt = GetTable(sTABLE_NAME, 0, 1, String.Empty, String.Empty, String.Empty, null, arrITEMS);
			if ( dt == null || dt.Rows.Count == 0 )
				throw(new Exception("Item not found: " + ModuleName + " " + ID.ToString()));
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			Dictionary<string, object> dict = ToJson(sBaseURI, ModuleName, dt.Rows[0], T10n);
			
			string sEXPAND = Sql.ToString (Request.QueryString["$expand"]);
			if ( sEXPAND == "*" )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					Dictionary<string, object> d       = dict["d"] as Dictionary<string, object>;
					Dictionary<string, object> results = d["results"] as Dictionary<string, object>;
					DataTable dtRelationships = SplendidCache.DetailViewRelationships(ModuleName + ".DetailView");
					foreach ( DataRow row in dtRelationships.Rows )
					{
						try
						{
							string sRELATED_MODULE     = Sql.ToString(row["MODULE_NAME"]);
							string sRELATED_TABLE      = Sql.ToString(Application["Modules." + sRELATED_MODULE + ".TableName"]);
							string sRELATED_FIELD_NAME = Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
							if ( !d.ContainsKey(sRELATED_MODULE) && SplendidCRM.Security.GetUserAccess(sRELATED_MODULE, "list") >= 0 )
							{
								using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sRELATED_TABLE, true) )
								{
									string sSQL;
									if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
									{
										UniqueStringCollection arrSearchFields = new UniqueStringCollection();
										SplendidDynamic.SearchGridColumns(ModuleName + "." + sRELATED_MODULE, arrSearchFields);
										
										sSQL = "select " + Sql.FormatSelectFields(arrSearchFields)
										     + "  from vw" + sTABLE_NAME + "_" + sRELATED_TABLE + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, sRELATED_MODULE, "list");
											Sql.AppendParameter(cmd, ID, sRELATED_FIELD_NAME);
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												using ( DataTable dtSubPanel = new DataTable() )
												{
													da.Fill(dtSubPanel);
													results.Add(sRELATED_MODULE, RowsToDictionary(sBaseURI, sRELATED_MODULE, dtSubPanel, T10n));
												}
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
				}
			}
			
			string sResponse = json.Serialize(dict);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetCalendar()
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			
			string   ModuleName        = "Activities";
			DateTime dtDATE_START      = FromJsonDate(Request.QueryString["DATE_START"      ]);
			DateTime dtDATE_END        = FromJsonDate(Request.QueryString["DATE_END"        ]);
			Guid     gASSIGNED_USER_ID = Sql.ToGuid  (Request.QueryString["ASSIGNED_USER_ID"]);
			Guid     gTIMEZONE         = Sql.ToGuid  (Session["USER_SETTINGS/TIMEZONE"]);
			string   sCULTURE          = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			L10N     L10n              = new L10N(sCULTURE);
			
			int nACLACCESS = Security.GetUserAccess(ModuleName, "list");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			DataTable dt = new DataTable() ;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                " + ControlChars.CrLf
				     + "  from vwACTIVITIES_List" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, "Calls", "list");
					if ( !Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
						Sql.AppendParameter(cmd, gASSIGNED_USER_ID, "ASSIGNED_USER_ID");
					cmd.CommandText += "   and (   DATE_START >= @DATE_START and DATE_START < @DATE_END" + ControlChars.CrLf;
					cmd.CommandText += "        or DATE_END   >= @DATE_START and DATE_END   < @DATE_END" + ControlChars.CrLf;
					cmd.CommandText += "        or DATE_START <  @DATE_START and DATE_END   > @DATE_END" + ControlChars.CrLf;
					cmd.CommandText += "       )                                                       " + ControlChars.CrLf;
					cmd.CommandText += " order by DATE_START asc, NAME asc                             " + ControlChars.CrLf;
					
					Sql.AddParameter(cmd, "@DATE_START", T10n.ToServerTime(dtDATE_START));
					Sql.AddParameter(cmd, "@DATE_END"  , T10n.ToServerTime(dtDATE_END  ));
					
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dt);
						
						foreach(DataRow row in dt.Rows)
						{
							switch ( Sql.ToString(row["ACTIVITY_TYPE"]) )
							{
								case "Calls"   :  row["STATUS"] = L10n.Term(".activity_dom.Call"   ) + " " + L10n.Term(".call_status_dom."   , row["STATUS"]);  break;
								case "Meetings":  row["STATUS"] = L10n.Term(".activity_dom.Meeting") + " " + L10n.Term(".meeting_status_dom.", row["STATUS"]);  break;
							}
							if ( SplendidInit.bEnableACLFieldSecurity )
							{
								Guid gACTIVITY_ASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
								foreach ( DataColumn col in dt.Columns )
								{
									Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(ModuleName, col.ColumnName, gACTIVITY_ASSIGNED_USER_ID);
									if ( !acl.IsReadable() )
									{
										row[col.ColumnName] = DBNull.Value;
									}
								}
							}
						}
						dt.AcceptChanges();
					}
				}
			}
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath.Replace("/GetCalendar", "/GetModuleItem");
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
			string sResponse = json.Serialize(ToJson(sBaseURI, ModuleName, dt, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		// 06/17/2013 Paul.  Add support for GROUP BY. 
		private DataTable GetTable(string sTABLE_NAME, int nSKIP, int nTOP, string sFILTER, string sORDER_BY, string sGROUP_BY, UniqueStringCollection arrSELECT, Guid[] arrITEMS)
		{
			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			DataTable dt = null;
			try
			{
				// 09/03/2011 Paul.  We should use the cached layout tables instead of a database lookup for performance reasons. 
				// When getting the layout tables, we typically only need the view name, so extract from the filter string. 
				// The Regex match will allow an OData query. 
				if ( Security.IsAuthenticated() )
				{
					string sMATCH_NAME = String.Empty;
					if ( sTABLE_NAME == "DYNAMIC_BUTTONS" )
					{
						sMATCH_NAME = "VIEW_NAME";
						Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
						if ( match.Success )
						{
							string sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
							dt = SplendidCache.DynamicButtons(sVIEW_NAME).Copy();
							if ( dt != null )
							{
								bool bRowsDeleted = false;
								foreach(DataRow row in dt.Rows)
								{
									string sCONTROL_TYPE       = Sql.ToString (row["CONTROL_TYPE"      ]);
									string sMODULE_NAME        = Sql.ToString (row["MODULE_NAME"       ]);
									string sMODULE_ACCESS_TYPE = Sql.ToString (row["MODULE_ACCESS_TYPE"]);
									string sTARGET_NAME        = Sql.ToString (row["TARGET_NAME"       ]);
									string sTARGET_ACCESS_TYPE = Sql.ToString (row["TARGET_ACCESS_TYPE"]);
									bool   bADMIN_ONLY         = Sql.ToBoolean(row["ADMIN_ONLY"        ]);
									
									bool bVisible = (bADMIN_ONLY && Security.IS_ADMIN || !bADMIN_ONLY);
									if ( String.Compare(sCONTROL_TYPE, "Button", true) == 0 || String.Compare(sCONTROL_TYPE, "HyperLink", true) == 0 || String.Compare(sCONTROL_TYPE, "ButtonLink", true) == 0 )
									{
										if ( bVisible && !Sql.IsEmptyString(sMODULE_NAME) && !Sql.IsEmptyString(sMODULE_ACCESS_TYPE) )
										{
											int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, sMODULE_ACCESS_TYPE);
											// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
											//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
											if ( bVisible && !Sql.IsEmptyString(sTARGET_NAME) && !Sql.IsEmptyString(sTARGET_ACCESS_TYPE) )
											{
												nACLACCESS = SplendidCRM.Security.GetUserAccess(sTARGET_NAME, sTARGET_ACCESS_TYPE);
												// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
												//bVisible = (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && ((Security.USER_ID == gASSIGNED_USER_ID) || (!bIsPostBack && rdr == null) || (rdr != null && bShowUnassigned && Sql.IsEmptyGuid(gASSIGNED_USER_ID))));
											}
										}
									}
									if ( !bVisible )
									{
										row.Delete();
										bRowsDeleted = true;
									}
								}
								if ( bRowsDeleted )
									dt.AcceptChanges();
							}
							return dt;
						}
					}
					else if ( sTABLE_NAME == "GRIDVIEWS_COLUMNS" )
					{
						sMATCH_NAME = "GRID_NAME";
						Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
						if ( match.Success )
						{
							string sGRID_NAME = match.Groups[sMATCH_NAME].Value;
							dt = SplendidCache.GridViewColumns(sGRID_NAME);
							// 09/03/2011 Paul.  Apply Field Level Security before sending to the client. 
							if ( dt != null && SplendidInit.bEnableACLFieldSecurity )
							{
								bool bRowsDeleted = false;
								// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
								for ( int i = 0; i < dt.Rows.Count; i++ )
								{
									DataRow row = dt.Rows[i];
									string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD"]);
									string sMODULE_NAME = String.Empty;
									string[] arrGRID_NAME = sGRID_NAME.Split('.');
									if ( arrGRID_NAME.Length > 0 )
									{
										if ( arrGRID_NAME[0] == "ListView" || arrGRID_NAME[0] == "PopupView" || arrGRID_NAME[0] == "Activities" )
											sMODULE_NAME = arrGRID_NAME[0];
										else if ( Sql.ToBoolean(Application["Modules." + arrGRID_NAME[1] + ".Valid"]) )
											sMODULE_NAME = arrGRID_NAME[1];
										else
											sMODULE_NAME = arrGRID_NAME[0];
									}
									bool bIsReadable = true;
									if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
									{
										Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
										bIsReadable  = acl.IsReadable();
									}
									if ( !bIsReadable )
									{
										row.Delete();
										bRowsDeleted = true;
									}
									// 09/03/2011 Paul.  We only need one copy of the SCRIPT field in the first record. 
									if ( i > 0 )
										row["SCRIPT"] = DBNull.Value;
								}
								if ( bRowsDeleted )
									dt.AcceptChanges();
							}
							return dt;
						}
					}
					else if ( sTABLE_NAME == "EDITVIEWS_FIELDS" )
					{
						sMATCH_NAME = "EDIT_NAME";
						Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
						if ( match.Success )
						{
							string sEDIT_NAME = match.Groups[sMATCH_NAME].Value;
							dt = SplendidCache.EditViewFields(sEDIT_NAME);
							// 09/03/2011 Paul.  Apply Field Level Security before sending to the client. 
							if ( dt != null && SplendidInit.bEnableACLFieldSecurity )
							{
								// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
								for ( int i = 0; i < dt.Rows.Count; i++ )
								{
									DataRow row = dt.Rows[i];
									string sFIELD_TYPE    = Sql.ToString (row["FIELD_TYPE"   ]);
									string sDATA_FIELD    = Sql.ToString (row["DATA_FIELD"   ]);
									string sDATA_FORMAT   = Sql.ToString (row["DATA_FORMAT"  ]);
									string sDISPLAY_FIELD = Sql.ToString (row["DISPLAY_FIELD"]);
									string sMODULE_NAME   = String.Empty;
									string[] arrEDIT_NAME = sEDIT_NAME.Split('.');
									if ( arrEDIT_NAME.Length > 0 )
										sMODULE_NAME = arrEDIT_NAME[0];
									bool bIsReadable  = true;
									bool bIsWriteable = true;
									if ( SplendidInit.bEnableACLFieldSecurity )
									{
										// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
										Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
										bIsReadable  = acl.IsReadable();
										// 02/16/2011 Paul.  We should allow a Read-Only field to be searchable, so always allow writing if the name contains Search. 
										bIsWriteable = acl.IsWriteable() || sEDIT_NAME.Contains(".Search");
									}
									if ( !bIsReadable )
									{
										row["FIELD_TYPE"] = "Blank";
									}
									else if ( !bIsWriteable )
									{
										row["FIELD_TYPE"] = "Label";
									}
									// 09/03/2011 Paul.  We only need one copy of the SCRIPT field in the first record. 
									if ( i > 0 )
										row["SCRIPT"] = DBNull.Value;
								}
							}
							return dt;
						}
					}
					else if ( sTABLE_NAME == "DETAILVIEWS_FIELDS" )
					{
						sMATCH_NAME = "DETAIL_NAME";
						Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
						if ( match.Success )
						{
							string sDETAIL_NAME = match.Groups[sMATCH_NAME].Value;
							dt = SplendidCache.DetailViewFields(sDETAIL_NAME);
							// 09/03/2011 Paul.  Apply Field Level Security before sending to the client. 
							if ( dt != null && SplendidInit.bEnableACLFieldSecurity )
							{
								// 09/20/2012 Paul.  We need a SCRIPT field that is form specific. 
								for ( int i = 0; i < dt.Rows.Count; i++ )
								{
									DataRow row = dt.Rows[i];
									string sDATA_FIELD  = Sql.ToString (row["DATA_FIELD"]);
									string sMODULE_NAME = String.Empty;
									string[] arrDETAIL_NAME = sDETAIL_NAME.Split('.');
									if ( arrDETAIL_NAME.Length > 0 )
										sMODULE_NAME = arrDETAIL_NAME[0];
									bool bIsReadable  = true;
									if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sDATA_FIELD) )
									{
										// 09/03/2011 Paul.  Can't apply Owner rights without the item record. 
										Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, sDATA_FIELD, Guid.Empty);
										bIsReadable  = acl.IsReadable();
									}
									if ( !bIsReadable )
									{
										row["FIELD_TYPE"] = "Blank";
									}
									// 09/03/2011 Paul.  We only need one copy of the SCRIPT field in the first record. 
									if ( i > 0 )
										row["SCRIPT"] = DBNull.Value;
								}
							}
							return dt;
						}
					}
					else if ( sTABLE_NAME == "DETAILVIEWS_RELATIONSHIPS" )
					{
						sMATCH_NAME = "DETAIL_NAME";
						Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
						if ( match.Success )
						{
							string sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
							dt = SplendidCache.DetailViewRelationships(sVIEW_NAME).Copy();
							if ( dt != null )
							{
								bool bRowsDeleted = false;
								foreach(DataRow row in dt.Rows)
								{
									string sMODULE_NAME       = Sql.ToString(row["MODULE_NAME" ]);
									string sCONTROL_NAME      = Sql.ToString(row["CONTROL_NAME"]);
									string sMODULE_TABLE_NAME = Sql.ToString(Context.Application["Modules." + sMODULE_NAME + ".TableName"]).ToUpper();
									// 10/09/2012 Paul.  Make sure to filter by modules with REST enabled. 
									using ( DataView vwSYNC_TABLES = new DataView(SplendidCache.RestTables(sMODULE_TABLE_NAME, true)) )
									{
										bool bVisible = (SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "list") >= 0) && vwSYNC_TABLES.Count > 0;
										if ( !bVisible )
										{
											row.Delete();
											bRowsDeleted = true;
										}
									}
								}
								if ( bRowsDeleted )
									dt.AcceptChanges();
							}
							return dt;
						}
					}
					else if ( sTABLE_NAME == "TAB_MENUS" )
					{
						dt = SplendidCache.TabMenu();
						return dt;
					}
					Regex r = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "");
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, false) )
						{
							string sSQL = String.Empty;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME         = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"        ]);
								string sVIEW_NAME           = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"          ]);
								bool   bHAS_CUSTOM          = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM"         ]);
								int    nMODULE_SPECIFIC     = Sql.ToInteger(rowSYNC_TABLE["MODULE_SPECIFIC"    ]);
								string sMODULE_FIELD_NAME   = Sql.ToString (rowSYNC_TABLE["MODULE_FIELD_NAME"  ]);
								bool   bIS_RELATIONSHIP     = Sql.ToBoolean(rowSYNC_TABLE["IS_RELATIONSHIP"    ]);
								string sMODULE_NAME_RELATED = Sql.ToString (rowSYNC_TABLE["MODULE_NAME_RELATED"]);
								string sASSIGNED_FIELD_NAME = Sql.ToString (rowSYNC_TABLE["ASSIGNED_FIELD_NAME"]);
								// 09/28/2011 Paul.  Include the system flag so that we can cache only system tables. 
								bool   bIS_SYSTEM           = Sql.ToBoolean(rowSYNC_TABLE["IS_SYSTEM"          ]);
								// 11/01/2009 Paul.  Protect against SQL Injection. A table name will never have a space character.
								sTABLE_NAME                 = Sql.ToString (rowSYNC_TABLE["TABLE_NAME"         ]);
								sTABLE_NAME        = r.Replace(sTABLE_NAME       , "");
								sVIEW_NAME         = r.Replace(sVIEW_NAME        , "");
								sMODULE_FIELD_NAME = r.Replace(sMODULE_FIELD_NAME, "");
								
								// 09/28/2011 Paul.  Non-system tables should not be cached on the server because they can change at any time. 
								// 10/01/2011 Paul.  We are getting No Response on system tables and no network request is made when online. 
								//if ( !bIS_SYSTEM )
									HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
								
								// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
								if ( arrSELECT != null && arrSELECT.Count > 0 )
								{
									foreach ( string sColumnName in arrSELECT )
									{
										if ( Sql.IsEmptyString(sSQL) )
											sSQL += "select " + sVIEW_NAME + "." + sColumnName + ControlChars.CrLf;
										else
											sSQL += "     , " + sVIEW_NAME + "." + sColumnName + ControlChars.CrLf;
									}
								}
								else
								{
									sSQL = "select " + sVIEW_NAME + ".*" + ControlChars.CrLf;
								}
								// 06/18/2011 Paul.  The REST API tables will use the view properly, so there is no need to join to the CSTM table. 
								sSQL += "  from " + sVIEW_NAME        + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									cmd.CommandTimeout = 0;
									// 10/27/2009 Paul.  Apply the standard filters. 
									// 11/03/2009 Paul.  Relationship tables will not have Team or Assigned fields. 
									if ( bIS_RELATIONSHIP )
									{
										cmd.CommandText += " where 1 = 1" + ControlChars.CrLf;
										// 11/06/2009 Paul.  Use the relationship table to get the module information. 
										DataView vwRelationships = new DataView(SplendidCache.ReportingRelationships(Context.Application));
										vwRelationships.RowFilter = "(JOIN_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'many-to-many') or (RHS_TABLE = '" + sTABLE_NAME + "' and RELATIONSHIP_TYPE = 'one-to-many')";
										if ( vwRelationships.Count > 0 )
										{
											foreach ( DataRowView rowRelationship in vwRelationships )
											{
												string sJOIN_KEY_LHS             = Sql.ToString(rowRelationship["JOIN_KEY_LHS"            ]).ToUpper();
												string sJOIN_KEY_RHS             = Sql.ToString(rowRelationship["JOIN_KEY_RHS"            ]).ToUpper();
												string sLHS_MODULE               = Sql.ToString(rowRelationship["LHS_MODULE"              ]);
												string sRHS_MODULE               = Sql.ToString(rowRelationship["RHS_MODULE"              ]);
												string sLHS_TABLE                = Sql.ToString(rowRelationship["LHS_TABLE"               ]).ToUpper();
												string sRHS_TABLE                = Sql.ToString(rowRelationship["RHS_TABLE"               ]).ToUpper();
												string sLHS_KEY                  = Sql.ToString(rowRelationship["LHS_KEY"                 ]).ToUpper();
												string sRHS_KEY                  = Sql.ToString(rowRelationship["RHS_KEY"                 ]).ToUpper();
												string sRELATIONSHIP_TYPE        = Sql.ToString(rowRelationship["RELATIONSHIP_TYPE"       ]);
												string sRELATIONSHIP_ROLE_COLUMN = Sql.ToString(rowRelationship["RELATIONSHIP_ROLE_COLUMN"]).ToUpper();
												sJOIN_KEY_LHS = r.Replace(sJOIN_KEY_LHS, "");
												sJOIN_KEY_RHS = r.Replace(sJOIN_KEY_RHS, "");
												sLHS_MODULE   = r.Replace(sLHS_MODULE  , "");
												sRHS_MODULE   = r.Replace(sRHS_MODULE  , "");
												sLHS_TABLE    = r.Replace(sLHS_TABLE   , "");
												sRHS_TABLE    = r.Replace(sRHS_TABLE   , "");
												sLHS_KEY      = r.Replace(sLHS_KEY     , "");
												sRHS_KEY      = r.Replace(sRHS_KEY     , "");
												if ( sRELATIONSHIP_TYPE == "many-to-many" )
												{
													cmd.CommandText += "   and " + sJOIN_KEY_LHS + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
													Security.Filter(cmd, sLHS_MODULE, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
													
													// 11/12/2009 Paul.  We don't want to deal with relationships to multiple tables, so just ignore for now. 
													if ( sRELATIONSHIP_ROLE_COLUMN != "RELATED_TYPE" )
													{
														cmd.CommandText += "   and " + sJOIN_KEY_RHS + " in " + ControlChars.CrLf;
														cmd.CommandText += "(select " + sRHS_KEY + " from " + sRHS_TABLE + ControlChars.CrLf;
														Security.Filter(cmd, sRHS_MODULE, "list");
														cmd.CommandText += ")" + ControlChars.CrLf;
													}
												}
												else if ( sRELATIONSHIP_TYPE == "one-to-many" )
												{
													cmd.CommandText += "   and " + sRHS_KEY + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select " + sLHS_KEY + " from " + sLHS_TABLE + ControlChars.CrLf;
													Security.Filter(cmd, sLHS_MODULE, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
										}
										else
										{
											// 11/12/2009 Paul.  EMAIL_IMAGES is a special table that is related to EMAILS or KBDOCUMENTS. 
											if ( sTABLE_NAME == "EMAIL_IMAGES" )
											{
												// 11/12/2009 Paul.  There does not appear to be an easy way to filter the EMAIL_IMAGES table. 
												// For now, just return the EMAIL related images. 
												cmd.CommandText += "   and PARENT_ID in " + ControlChars.CrLf;
												cmd.CommandText += "(select ID from EMAILS" + ControlChars.CrLf;
												Security.Filter(cmd, "Emails", "list");
												cmd.CommandText += "union all" + ControlChars.CrLf;
												cmd.CommandText += "select ID from KBDOCUMENTS" + ControlChars.CrLf;
												Security.Filter(cmd, "KBDocuments", "list");
												cmd.CommandText += ")" + ControlChars.CrLf;
											}
											// 11/06/2009 Paul.  If the relationship is not in the RELATIONSHIPS table, then try and build it manually. 
											// 11/05/2009 Paul.  We cannot use the standard filter on the Teams table (or TeamNotices). 
											else if ( !Sql.IsEmptyString(sMODULE_NAME) && !sMODULE_NAME.StartsWith("Team") )
											{
												// 11/05/2009 Paul.  We could query the foreign key tables to perpare the filters, but that is slow. 
												string sMODULE_TABLE_NAME   = Sql.ToString(Context.Application["Modules." + sMODULE_NAME + ".TableName"]).ToUpper();
												if ( !Sql.IsEmptyString(sMODULE_TABLE_NAME) )
												{
													// 06/04/2011 Paul.  New function to get the singular name. 
													string sMODULE_FIELD_ID = Crm.Modules.SingularTableName(sMODULE_TABLE_NAME) + "_ID";
													
													cmd.CommandText += "   and " + sMODULE_FIELD_ID + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select ID from " + sMODULE_TABLE_NAME + ControlChars.CrLf;
													Security.Filter(cmd, sMODULE_NAME, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
											// 11/05/2009 Paul.  We cannot use the standard filter on the Teams table. 
											if ( !Sql.IsEmptyString(sMODULE_NAME_RELATED) && !sMODULE_NAME_RELATED.StartsWith("Team") )
											{
												string sMODULE_TABLE_RELATED = Sql.ToString(Context.Application["Modules." + sMODULE_NAME_RELATED + ".TableName"]).ToUpper();
												if ( !Sql.IsEmptyString(sMODULE_TABLE_RELATED) )
												{
													// 06/04/2011 Paul.  New function to get the singular name. 
													string sMODULE_RELATED_ID = Crm.Modules.SingularTableName(sMODULE_TABLE_RELATED) + "_ID";
													
													// 11/05/2009 Paul.  Some tables use ASSIGNED_USER_ID as the relationship ID instead of the USER_ID. 
													if ( sMODULE_RELATED_ID == "USER_ID" && !Sql.IsEmptyString(sASSIGNED_FIELD_NAME) )
														sMODULE_RELATED_ID = sASSIGNED_FIELD_NAME;
													
													cmd.CommandText += "   and " + sMODULE_RELATED_ID + " in " + ControlChars.CrLf;
													cmd.CommandText += "(select ID from " + sMODULE_TABLE_RELATED + ControlChars.CrLf;
													Security.Filter(cmd, sMODULE_NAME_RELATED, "list");
													cmd.CommandText += ")" + ControlChars.CrLf;
												}
											}
										}
									}
									else
									{
										// 02/14/2010 Paul.  GetTable should only require read-only access. 
										// We were previously requiring Edit access, but that seems to be a high bar. 
										Security.Filter(cmd, sMODULE_NAME, "view");
									}
									if ( !Sql.IsEmptyString(sMODULE_FIELD_NAME) )
									{
										List<string> lstMODULES = AccessibleModules();
										
										if ( sTABLE_NAME == "MODULES" )
										{
											// 11/27/2009 Paul.  Don't filter the MODULES table. It can cause system tables to get deleted. 
											// 11/28/2009 Paul.  Keep the filter on the Modules table, but add the System Sync Tables to the list. 
											// We should make sure that the clients do not get module records for unnecessary or disabled modules. 
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
											// 10/09/2012 Paul.  We need to make sure to only return modules that are available to REST. 
											cmd.CommandText += "   and MODULE_NAME in (select MODULE_NAME from vwSYSTEM_REST_TABLES)" + ControlChars.CrLf;
										}
										else if ( nMODULE_SPECIFIC == 1 )
										{
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
										}
										else if ( nMODULE_SPECIFIC == 2 )
										{
											// 04/05/2012 Paul.  AppendLikeModules is a special like that assumes that the search is for a module related value 
											Sql.AppendLikeModules(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME);
										}
										else if ( nMODULE_SPECIFIC == 3 )
										{
											cmd.CommandText += "   and ( 1 = 0" + ControlChars.CrLf;
											cmd.CommandText += "         or " + sMODULE_FIELD_NAME + " is null" + ControlChars.CrLf;
											// 11/02/2009 Paul.  There are a number of terms with undefined modules. 
											// ACL, ACLActions, Audit, Config, Dashlets, DocumentRevisions, Export, Merge, Roles, SavedSearch, Teams
											cmd.CommandText += "     ";
											Sql.AppendParameter(cmd, lstMODULES.ToArray(), sMODULE_FIELD_NAME, true);
											cmd.CommandText += "       )" + ControlChars.CrLf;
										}
										// 11/22/2009 Paul.  Make sure to only send the selected user language.  This will dramatically reduce the amount of data. 
										//if ( sTABLE_NAME == "TERMINOLOGY" || sTABLE_NAME == "TERMINOLOGY_HELP" )
										//{
										//	cmd.CommandText += "   and LANG in ('en-US', @LANG)" + ControlChars.CrLf;
										//	string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
										//	Sql.AddParameter(cmd, "@LANG", sCULTURE);
										//}
									}
		
									if ( arrITEMS != null )
									{
										// 11/13/2009 Paul.  If a list of items is provided, then the max records field is ignored. 
										nSKIP = 0;
										nTOP = -1;
										Sql.AppendGuids(cmd, arrITEMS, "ID");
									}
									else if ( sTABLE_NAME == "IMAGES" )
									{
										// 02/14/2010 Paul.  There is no easy way to filter IMAGES table, so we are simply going to fetch 
										// images that the user has created.  Otherwise, images that are accessible to the user will 
										// need to be retrieved by ID.
										Sql.AppendParameter(cmd, Security.USER_ID, "CREATED_BY");
									}
									// 06/18/2011 Paul.  Tables that are filtered by user should have an explicit filter added. 
									if ( sASSIGNED_FIELD_NAME == "USER_ID" )
									{
										Sql.AppendParameter(cmd, Security.USER_ID, "USER_ID");
									}
									if ( !Sql.IsEmptyString(sFILTER) )
									{
										string sSQL_FILTER = ConvertODataFilter(sFILTER, cmd);
										cmd.CommandText += "   and (" + sSQL_FILTER + ")" + ControlChars.CrLf;;
									}
									if ( Sql.IsEmptyString(sORDER_BY) )
									{
										sORDER_BY = " order by " + sVIEW_NAME + ".DATE_MODIFIED_UTC" + ControlChars.CrLf;
									}
									else
									{
										// 06/18/2011 Paul.  Allow a comma in a sort expression. 
										r = new Regex(@"[^A-Za-z0-9_, ]");
										sORDER_BY = " order by " + r.Replace(sORDER_BY, "");
									}
									// 06/17/2013 Paul.  Add support for GROUP BY. 
									if ( !Sql.IsEmptyString(sGROUP_BY) )
									{
										// 06/18/2011 Paul.  Allow a comma in a sort expression. 
										r = new Regex(@"[^A-Za-z0-9_, ]");
										sGROUP_BY = " group by " + r.Replace(sGROUP_BY, "");
									}
									//cmd.CommandText += sORDER_BY;
// 03/20/2012 Paul.  Nolonger need to debug these SQL statements. 
//#if DEBUG
//									SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
//#endif
									
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										// 11/08/2009 Paul.  The table name is required in order to serialize the DataTable. 
										dt = new DataTable(sTABLE_NAME);
										if ( nTOP > 0 )
										{
											if ( nSKIP > 0 )
											{
												int nCurrentPageIndex = nSKIP / nTOP;
												// 06/17/2103 Paul.  We cannot page a group result. 
												Sql.PageResults(cmd, sTABLE_NAME, sORDER_BY, nCurrentPageIndex, nTOP);
												da.Fill(dt);
											}
											else
											{
												// 06/17/2013 Paul.  Add support for GROUP BY. 
												cmd.CommandText += sGROUP_BY + sORDER_BY;
												using ( DataSet ds = new DataSet() )
												{
													ds.Tables.Add(dt);
													da.Fill(ds, 0, nTOP, sTABLE_NAME);
												}
											}
										}
										else
										{
											// 06/17/2013 Paul.  Add support for GROUP BY. 
											cmd.CommandText += sGROUP_BY + sORDER_BY;
											da.Fill(dt);
										}
										// 02/24/2013 Paul.  Manually add the Calendar entries. 
										if ( sTABLE_NAME == "TERMINOLOGY" && (sFILTER.Contains("MODULE_NAME eq 'Calendar'") || sFILTER.Contains("MODULE_NAME = 'Calendar'")) )
										{
											string sLANG  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
											DataRow row = null;
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "YearMonthPattern";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.YearMonthPattern;
											dt.Rows.Add(row);
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "MonthDayPattern";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthDayPattern;
											dt.Rows.Add(row);
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "LongDatePattern";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
											dt.Rows.Add(row);
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "ShortTimePattern";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/TIMEFORMAT"]);
											dt.Rows.Add(row);
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "ShortDatePattern";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/DATEFORMAT"]);
											dt.Rows.Add(row);
											row = dt.NewRow();
											row["LANG"        ] = sLANG;
											row["NAME"        ] = "FirstDayOfWeek";
											row["MODULE_NAME" ] = "Calendar";
											row["DISPLAY_NAME"] = ((int) System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek).ToString();
											dt.Rows.Add(row);
										}
										// 01/18/2010 Paul.  Apply ACL Field Security. 
										// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
										if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
										{
											bool bApplyACL = false;
											bool bASSIGNED_USER_ID_Exists = dt.Columns.Contains("ASSIGNED_USER_ID");
											foreach ( DataRow row in dt.Rows )
											{
												Guid gASSIGNED_USER_ID = Guid.Empty;
												if ( bASSIGNED_USER_ID_Exists )
													gASSIGNED_USER_ID = Sql.ToGuid(row["ASSIGNED_USER_ID"]);
												foreach ( DataColumn col in dt.Columns )
												{
													Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, gASSIGNED_USER_ID);
													if ( !acl.IsReadable() )
													{
														row[col.ColumnName] = DBNull.Value;
														bApplyACL = true;
													}
												}
											}
											if ( bApplyACL )
												dt.AcceptChanges();
										}
										if ( sTABLE_NAME == "USERS" )
										{
											// 05/24/2014 Paul.  Provide a way to customize the list of available field names for the Users table. 
											UniqueStringCollection arrUSERS_FIELDS = new UniqueStringCollection();
											string sUSERS_FIELDS = Sql.ToString(Application["CONFIG.rest.Users.Fields"]);
											sUSERS_FIELDS = sUSERS_FIELDS.Replace(",", " ").Trim();
											if ( Sql.IsEmptyString(sUSERS_FIELDS) )
											{
												arrUSERS_FIELDS.Add("ID"               );
												arrUSERS_FIELDS.Add("DELETED"          );
												arrUSERS_FIELDS.Add("CREATED_BY"       );
												arrUSERS_FIELDS.Add("DATE_ENTERED"     );
												arrUSERS_FIELDS.Add("MODIFIED_USER_ID" );
												arrUSERS_FIELDS.Add("DATE_MODIFIED"    );
												arrUSERS_FIELDS.Add("DATE_MODIFIED_UTC");
												arrUSERS_FIELDS.Add("USER_NAME"        );
												arrUSERS_FIELDS.Add("FIRST_NAME"       );
												arrUSERS_FIELDS.Add("LAST_NAME"        );
												arrUSERS_FIELDS.Add("REPORTS_TO_ID"    );
												arrUSERS_FIELDS.Add("EMAIL1"           );
												arrUSERS_FIELDS.Add("STATUS"           );
												arrUSERS_FIELDS.Add("IS_GROUP"         );
												arrUSERS_FIELDS.Add("PORTAL_ONLY"      );
												arrUSERS_FIELDS.Add("EMPLOYEE_STATUS"  );
											}
											else
											{
												foreach ( string sField in sUSERS_FIELDS.Split(' ') )
												{
													if ( !Sql.IsEmptyString(sField) )
														arrUSERS_FIELDS.Add(sField.ToUpper());
												}
											}
											// 11/12/2009 Paul.  For the USERS table, we are going to limit the data return to the client. 
											foreach ( DataRow row in dt.Rows )
											{
												if ( Sql.ToGuid(row["ID"]) != Security.USER_ID )
												{
													foreach ( DataColumn col in dt.Columns )
													{
														// 11/12/2009 Paul.  Allow auditing fields and basic user info. 
														if ( !arrUSERS_FIELDS.Contains(col.ColumnName) )
														{
															row[col.ColumnName] = DBNull.Value;
														}
													}
												}
											}
											dt.AcceptChanges();
										}
									}
								}
							}
							else
							{
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), sTABLE_NAME + " cannot be accessed.");
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				// 12/01/2012 Paul.  We need a more descriptive error message. 
				//SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				string sMessage = "GetTable(" + sTABLE_NAME + ", " + sFILTER + ", " + sORDER_BY + ") " + ex.Message;
				SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), sMessage);
				throw(new Exception(sMessage));
			}
			return dt;
		}
		#endregion

		#region Update
		[OperationContract]
		// 03/13/2011 Paul.  Must use octet-stream instead of json, outherwise we get the following error. 
		// Incoming message for operation 'CreateRecord' (contract 'AddressService' with namespace 'http://tempuri.org/') contains an unrecognized http body format value 'Json'. 
		// The expected body format value is 'Raw'. This can be because a WebContentTypeMapper has not been configured on the binding. See the documentation of WebContentTypeMapper for more details.
		//xhr.setRequestHeader('content-type', 'application/octet-stream');
		public Guid UpdateModuleTable(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			string sTableName = Sql.ToString(Request.QueryString["TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("The table name must be specified."));
			if ( !Security.IsAuthenticated() )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			// 08/22/2011 Paul.  Add admin control to REST API. 
			string sMODULE_NAME = Sql.ToString(Application["Modules." + sTableName + ".ModuleName"]);
			// 08/22/2011 Paul.  Not all tables will have a module name, such as relationship tables. 
			// Tables will get another security filter later in the code. 
			if ( !Sql.IsEmptyString(sMODULE_NAME) )
			{
				int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "edit");
				if ( !Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".RestEnabled"]) || nACLACCESS < 0 )
				{
					L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
				}
			}
			
			Guid gID = UpdateTable(sTableName, dict);
			return gID;
		}

		[OperationContract]
		public Guid UpdateModule(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			// http://weblogs.asp.net/hajan/archive/2010/07/23/javascriptserializer-dictionary-to-json-serialization-and-deserialization.aspx
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 12/12/2014 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);

			string sModuleName = Sql.ToString(Request.QueryString["ModuleName"]);
			if ( Sql.IsEmptyString(sModuleName) )
				throw(new Exception("The module name must be specified."));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(sModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + sModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			// 11/16/2014 Paul.  We need to continually update the SplendidSession so that it expires along with the ASP.NET Session. 
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			string sTableName = Sql.ToString(Application["Modules." + sModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("Unknown module: " + sModuleName));
			
			Guid gID = UpdateTable(sTableName, dict);
			return gID;
		}

		private Guid UpdateTable(string sTABLE_NAME, Dictionary<string, object> dict)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			Guid gID = Guid.Empty;
			try
			{
				// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
				Guid     gTIMEZONE = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
				TimeZone T10n      = TimeZone.CreateTimeZone(gTIMEZONE);
				// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
				bool bSaveDuplicate   = false;
				bool bSaveConcurrency = false;
				DateTime dtLAST_DATE_MODIFIED = DateTime.MinValue;
				DataTable dtUPDATE = new DataTable(sTABLE_NAME);
				foreach ( string sColumnName in dict.Keys )
				{
					// 03/16/2014 Paul.  Don't include Save Overrides as column names. 
					if ( sColumnName == "SaveDuplicate" )
						bSaveDuplicate = true;
					else if ( sColumnName == "SaveConcurrency" )
						bSaveConcurrency = true;
					else if ( sColumnName == "LAST_DATE_MODIFIED" )
						dtLAST_DATE_MODIFIED = T10n.ToServerTime(FromJsonDate(Sql.ToString(dict[sColumnName])));
					else
						dtUPDATE.Columns.Add(sColumnName);
				}
				DataRow row = dtUPDATE.NewRow();
				dtUPDATE.Rows.Add(row);
				foreach ( string sColumnName in dict.Keys )
				{
					// 09/09/2011 Paul.  Multi-selection list boxes will come in as an ArrayList. 
					if ( dict[sColumnName] is System.Collections.ArrayList )
					{
						System.Collections.ArrayList lst = dict[sColumnName] as System.Collections.ArrayList;
						XmlDocument xml = new XmlDocument();
						xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
						xml.AppendChild(xml.CreateElement("Values"));
						if ( lst.Count > 0 )
						{
							foreach(string item in lst)
							{
								XmlNode xValue = xml.CreateElement("Value");
								xml.DocumentElement.AppendChild(xValue);
								xValue.InnerText = item;
							}
						}
						row[sColumnName] = xml.OuterXml;
					}
					else if ( sColumnName != "SaveDuplicate" && sColumnName != "SaveConcurrency" && sColumnName != "LAST_DATE_MODIFIED" )
					{
						row[sColumnName] = dict[sColumnName];
					}
				}
				//dtResults.Columns.Add("SPLENDID_SYNC_STATUS" , typeof(System.String));
				//dtResults.Columns.Add("SPLENDID_SYNC_MESSAGE", typeof(System.String));
				if ( Security.IsAuthenticated() )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N   L10n     = new L10N(sCULTURE);
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
						{
							string sSQL;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								bool   bHAS_CUSTOM  = Sql.ToBoolean(rowSYNC_TABLE["HAS_CUSTOM" ]);
								// 02/14/2010 Paul.  GetUserAccess requires a non-null sMODULE_NAME. 
								// Lets catch the exception here so that we can throw a meaningful error. 
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									
									// 02/22/2013 Paul.  Make sure the ID column exists before retrieving. It is optional. 
									if ( row.Table.Columns.Contains("ID") )
										gID = Sql.ToGuid(row["ID"]);
									if ( !Sql.IsEmptyGuid(gID) )
									{
										sSQL = "select *"              + ControlChars.CrLf
										     + "  from " + sTABLE_NAME + ControlChars.CrLf
										     + " where 1 = 1"          + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Sql.AppendParameter(cmd, gID, "ID");
											using ( DbDataAdapter da = dbf.CreateDataAdapter() )
											{
												((IDbDataAdapter)da).SelectCommand = cmd;
												// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
												try
												{
													da.Fill(dtCurrent);
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
												if ( dtCurrent.Rows.Count > 0 )
												{
													rowCurrent = dtCurrent.Rows[0];
													// 03/16/2014 Paul.  Throw an exception if the record has been edited since the last load. 
													// 03/16/2014 Paul.  Enable override of concurrency error. 
													if ( Sql.ToBoolean(HttpContext.Current.Application["CONFIG.enable_concurrency_check"])  && !bSaveConcurrency && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
													{
														throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED) + ".ERR_CONCURRENCY_OVERRIDE"));
													}
													bRecordExists = true;
													// 01/18/2010 Paul.  Apply ACL Field Security. 
													if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
													{
														gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
													}
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "edit");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( !bRecordExists || bAccessAllowed )
										{
											// 03/14/2014 Paul.  DUPLICATE_CHECHING_ENABLED enables duplicate checking. 
											HttpApplicationState Application = HttpContext.Current.Application;
											bool bDUPLICATE_CHECHING_ENABLED = Sql.ToBoolean(Application["CONFIG.enable_duplicate_check"]) && Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".DuplicateCheckingEnabled"]) && !bSaveDuplicate;
											if ( bDUPLICATE_CHECHING_ENABLED )
											{
												if ( Utils.DuplicateCheck(Application, con, sMODULE_NAME, gID, row, rowCurrent) > 0 )
												{
													// 03/16/2014 Paul.  Put the error name at the end so that we can detect the event. 
													throw(new Exception(L10n.Term(".ERR_DUPLICATE_EXCEPTION") + ".ERR_DUPLICATE_EXCEPTION"));
												}
											}
											DataTable dtMetadata = SplendidCache.SqlColumns(sTABLE_NAME);
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													bool bEnableTeamManagement  = Crm.Config.enable_team_management();
													bool bRequireTeamManagement = Crm.Config.require_team_management();
													bool bRequireUserAssignment = Crm.Config.require_user_assignment();
													// 06/04/2011 Paul.  Unlike the Sync service, we want to use the stored procedures to update records. 
													// 10/27/2012 Paul.  Relationship tables start with vw. 
													IDbCommand cmdUpdate = null;
													// 11/23/2014 Paul.  NOTE_ATTACHMENTS does not have an _Update procedure.  Fallback to _Insert. 
													try
													{
														// 11/23/2014 Paul.  Table name is converted to upper case. 
														if ( sTABLE_NAME.StartsWith("vw") || sTABLE_NAME.StartsWith("VW") )
															cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME.Substring(2) + "_Update");
														else
															cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
													}
													catch
													{
														if ( sTABLE_NAME.StartsWith("vw") || sTABLE_NAME.StartsWith("VW") )
															cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME.Substring(2) + "_Insert");
														else
															cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Insert");
													}
													cmdUpdate.Transaction = trn;
													foreach(IDbDataParameter par in cmdUpdate.Parameters)
													{
														// 03/27/2010 Paul.  The ParameterName will start with @, so we need to remove it. 
														string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
														if ( sParameterName == "TEAM_ID" && bEnableTeamManagement )
															par.Value = Sql.ToDBGuid(Security.TEAM_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
														else if ( sParameterName == "ASSIGNED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);  // 02/26/2011 Paul.  Make sure to convert Guid.Empty to DBNull. 
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													if ( bRecordExists )
													{
														// 11/11/2009 Paul.  If the record already exists, then the current values are treated as default values. 
														foreach ( DataColumn col in rowCurrent.Table.Columns )
														{
															IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
															// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
															if ( par != null && String.Compare(col.ColumnName, "DATE_MODIFIED_UTC", true) != 0 )
																par.Value = rowCurrent[col.ColumnName];
														}
													}
													
													foreach ( DataColumn col in row.Table.Columns )
													{
														// 01/18/2010 Paul.  Apply ACL Field Security. 
														// 02/01/2010 Paul.  System tables may not have a valid Module name, so Field Security will not apply. 
														bool bIsWriteable = true;
														if ( SplendidInit.bEnableACLFieldSecurity && !Sql.IsEmptyString(sMODULE_NAME) )
														{
															Security.ACL_FIELD_ACCESS acl = Security.GetUserFieldSecurity(sMODULE_NAME, col.ColumnName, Guid.Empty);
															bIsWriteable = acl.IsWriteable();
														}
														if ( bIsWriteable )
														{
															IDbDataParameter par = Sql.FindParameter(cmdUpdate, col.ColumnName);
															// 11/26/2009 Paul.  The UTC modified date should be set to Now. 
															if ( par != null )
															{
																switch ( par.DbType )
																{
																	// 10/08/2011 Paul.  We must use Sql.ToDBDateTime, otherwise we get a an error whe DateTime.MinValue is used. 
																	// SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.
																	// 05/05/2013 Paul.  We need to convert the date to the user's timezone. 
																	case DbType.Date                 :  par.Value = Sql.ToDBDateTime(T10n.ToServerTime(FromJsonDate(Sql.ToString(row[col.ColumnName]))));  break;
																	case DbType.DateTime             :  par.Value = Sql.ToDBDateTime(T10n.ToServerTime(FromJsonDate(Sql.ToString(row[col.ColumnName]))));  break;
																	case DbType.Int16                :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.Int32                :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.Int64                :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.UInt16               :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.UInt32               :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.UInt64               :  par.Value = Sql.ToDBInteger(row[col.ColumnName]);  break;
																	case DbType.Single               :  par.Value = Sql.ToDBFloat  (row[col.ColumnName]);  break;
																	case DbType.Double               :  par.Value = Sql.ToDBFloat  (row[col.ColumnName]);  break;
																	case DbType.Decimal              :  par.Value = Sql.ToDBDecimal(row[col.ColumnName]);  break;
																	case DbType.Currency             :  par.Value = Sql.ToDBDecimal(row[col.ColumnName]);  break;
																	case DbType.Boolean              :  par.Value = Sql.ToDBBoolean(row[col.ColumnName]);  break;
																	case DbType.Guid                 :  par.Value = Sql.ToDBGuid   (row[col.ColumnName]);  break;
																	case DbType.String               :  par.Value = Sql.ToDBString (row[col.ColumnName]);  break;
																	case DbType.StringFixedLength    :  par.Value = Sql.ToDBString (row[col.ColumnName]);  break;
																	case DbType.AnsiString           :  par.Value = Sql.ToDBString (row[col.ColumnName]);  break;
																	case DbType.AnsiStringFixedLength:  par.Value = Sql.ToDBString (row[col.ColumnName]);  break;
																}
															}
														}
													}
													cmdUpdate.ExecuteScalar();
													IDbDataParameter parID = Sql.FindParameter(cmdUpdate, "@ID");
													if ( parID != null )
													{
														gID = Sql.ToGuid(parID.Value);
														if ( bHAS_CUSTOM )
														{
															DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
															SplendidDynamic.UpdateCustomFields(row, trn, gID, sTABLE_NAME, dtCustomFields);
														}
													}
													trn.Commit();
												}
												catch
												{
													trn.Rollback();
													throw;
												}
											}
											// 11/23/2014 Paul.  Attachments require a separate step of inserting the content. 
											if ( sTABLE_NAME == "VWNOTE_ATTACHMENTS" )
											{
												if ( dict.ContainsKey("FILE_DATA") )
												{
													string sFILE_DATA = Sql.ToString(dict["FILE_DATA"]);
													byte[] byFILE_DATA  = Convert.FromBase64String(sFILE_DATA);
													using ( IDbTransaction trn = Sql.BeginTransaction(con) )
													{
														try
														{
															Crm.NoteAttachments.LoadFile(gID, byFILE_DATA, trn);
															trn.Commit();
														}
														catch(Exception ex)
														{
															trn.Rollback();
															SplendidError.SystemError(new StackTrace(true).GetFrame(0),  Utils.ExpandException(ex));
														}
													}
												}
											}
											// 11/18/2014 Paul.  Send a SignalR alert if created. 
											else if ( sTABLE_NAME == "CHAT_MESSAGES" )
											{
												/*
												// 11/19/2014 Paul.  A chat message can include an attachment.  We want to create the attachment prior sending the SignalR alert. 
												string sFILENAME       = String.Empty;
												string sFILE_MIME_TYPE = String.Empty;
												string sFILE_DATA      = String.Empty;
												foreach ( string sColumnName in dict.Keys )
												{
													// 03/16/2014 Paul.  Don't include Save Overrides as column names. 
													if ( sColumnName == "FILE_NAME" )
														sFILENAME = Sql.ToString(dict[sColumnName]);
													else if ( sColumnName == "FILE_TYPE" )
														sFILE_MIME_TYPE = Sql.ToString(dict[sColumnName]);
													else if ( sColumnName == "FILE_DATA" )
														sFILE_DATA = Sql.ToString(dict[sColumnName]);
												}
												if ( !Sql.IsEmptyString(sFILENAME) && !Sql.IsEmptyString(sFILE_DATA) )
												{
													try
													{
														byte[] byFILE_DATA  = Convert.FromBase64String(sFILE_DATA);
														string sFILE_EXT    = Path.GetExtension(sFILENAME);
														long lFileSize      = byFILE_DATA.Length;
														long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
														if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
														{
															throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
														}
														Guid   gTEAM_ID          = Security.TEAM_ID;
														Guid   gASSIGNED_USER_ID = Security.USER_ID;
														Guid   gMODIFIED_USER_ID = Security.USER_ID;
														string sTEAM_SET_LIST    = String.Empty;
														foreach ( DataColumn col in row.Table.Columns )
														{
															switch ( col.ColumnName.ToUpper() )
															{
																case "TEAM_ID"         :  gTEAM_ID          = Sql.ToGuid  (row[col.ColumnName]);  break;
																case "ASSIGNED_USER_ID":  gASSIGNED_USER_ID = Sql.ToGuid  (row[col.ColumnName]);  break;
																case "TEAM_SET_LIST"   :  sTEAM_SET_LIST    = Sql.ToString(row[col.ColumnName]);  break;
															}
														}
														
														using ( IDbTransaction trn = Sql.BeginTransaction(con) )
														{
															try
															{
																Guid gNOTE_ID = Guid.Empty;
																SqlProcs.spNOTES_Update
																	( ref gNOTE_ID
																	, L10n.Term("ChatMessages.LBL_ATTACHMENT") + sFILENAME
																	, "ChatMessages"  // Parent Type
																	, gID             // Parent ID
																	, Guid.Empty
																	, String.Empty
																	, gTEAM_ID
																	, sTEAM_SET_LIST
																	, gASSIGNED_USER_ID
																	, trn
																	);
																Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
																SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, sFILENAME, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
																Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, byFILE_DATA, trn);
																trn.Commit();
															}
															catch(Exception ex)
															{
																trn.Rollback();
																SplendidError.SystemError(new StackTrace(true).GetFrame(0),  Utils.ExpandException(ex));
															}
														}
													}
													catch(Exception ex)
													{
														SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
													}
												}
												*/
												if ( !bRecordExists )
												{
													ChatManager.Instance.NewMessage(gID);
												}
											}
										}
										else
										{
											//DataRow rowError = dtResults.NewRow();
											//dtResults.Rows.Add(rowError);
											//rowError["ID"                   ] = gID;
											//rowError["SPLENDID_SYNC_STATUS" ] = "Access Denied";
											//rowError["SPLENDID_SYNC_MESSAGE"] = L10n.Term("ACL.LBL_NO_ACCESS");
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			return gID;
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void UpdateRelatedItem(string ModuleName, Guid ID, string RelatedModule, Guid RelatedID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			if ( Sql.IsEmptyString(RelatedModule) )
				throw(new Exception("The related module name must be specified."));
			string sRELATED_TABLE = Sql.ToString(Application["Modules." + RelatedModule + ".TableName"]);
			if ( Sql.IsEmptyString(sRELATED_TABLE) )
				throw(new Exception("Unknown module: " + RelatedModule));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			nACLACCESS = Security.GetUserAccess(RelatedModule, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + RelatedModule + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + sRELATED_TABLE;
			string sMODULE_FIELD_NAME  = Crm.Modules.SingularTableName(sTABLE_NAME   ) + "_ID";
			string sRELATED_FIELD_NAME = Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
			// 11/24/2012 Paul.  In the special cases of Accounts Related and Contacts Reports To, we need to correct the field name. 
			if ( sMODULE_FIELD_NAME == "ACCOUNT_ID" && sRELATED_FIELD_NAME == "ACCOUNT_ID" )
			{
				sRELATIONSHIP_TABLE = "ACCOUNTS_MEMBERS";
				sRELATED_FIELD_NAME = "PARENT_ID";
			}
			else if ( sMODULE_FIELD_NAME == "CONTACT_ID" && sRELATED_FIELD_NAME == "CONTACT_ID" )
			{
				sRELATIONSHIP_TABLE = "CONTACTS_DIRECT_REPORTS";
				sRELATED_FIELD_NAME = "REPORTS_TO_ID";
			}
			
			DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, true);
			if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
			{
				sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
				dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, true);
				if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
				{
					L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedModule));
				}
			}
			UpdateRelatedItem(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedID);
		}

		private void UpdateRelatedItem(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, Guid gRELATED_ID)
		{
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N   L10n     = new L10N(sCULTURE);
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
						{
							string sSQL;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "edit");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											// 11/24/2012 Paul.  We do not need to check for RestTable access as that step was already done. 
											IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Update");
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													cmdUpdate.Transaction = trn;
													foreach(IDbDataParameter par in cmdUpdate.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdUpdate, par.ParameterName).ToUpper();
														if ( sParameterName == sMODULE_FIELD_NAME )
															par.Value = gID;
														else if ( sParameterName == sRELATED_FIELD_NAME )
															par.Value = gRELATED_ID;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													cmdUpdate.ExecuteScalar();
													trn.Commit();
												}
												catch
												{
													trn.Rollback();
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}
		#endregion

		#region Delete
		// 3.2 Method Tunneling through POST. 
		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void DeleteModuleItem(string ModuleName, Guid ID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "delete");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			DeleteTableItem(sTABLE_NAME, ID);
		}

		private void DeleteTableItem(string sTABLE_NAME, Guid gID)
		{
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N   L10n     = new L10N(sCULTURE);
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
						{
							string sSQL;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where 1 = 1"          + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "delete");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
													cmdDelete.Transaction = trn;
													foreach(IDbDataParameter par in cmdDelete.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdDelete, par.ParameterName).ToUpper();
														if ( sParameterName == "ID" )
															par.Value = gID;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													cmdDelete.ExecuteScalar();
													trn.Commit();
												}
												catch
												{
													trn.Rollback();
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		[OperationContract]
		[WebInvoke(Method="POST", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public void DeleteRelatedItem(string ModuleName, Guid ID, string RelatedModule, Guid RelatedID)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) )
				throw(new Exception("Unknown module: " + ModuleName));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			int nACLACCESS = Security.GetUserAccess(ModuleName, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + ModuleName + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			
			if ( Sql.IsEmptyString(RelatedModule) )
				throw(new Exception("The related module name must be specified."));
			string sRELATED_TABLE = Sql.ToString(Application["Modules." + RelatedModule + ".TableName"]);
			if ( Sql.IsEmptyString(sRELATED_TABLE) )
				throw(new Exception("Unknown module: " + RelatedModule));
			// 08/22/2011 Paul.  Add admin control to REST API. 
			nACLACCESS = Security.GetUserAccess(RelatedModule, "edit");
			if ( !Security.IsAuthenticated() || !Sql.ToBoolean(Application["Modules." + RelatedModule + ".RestEnabled"]) || nACLACCESS < 0 )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}

			string sRELATIONSHIP_TABLE = sTABLE_NAME + "_" + sRELATED_TABLE;
			string sMODULE_FIELD_NAME  = Crm.Modules.SingularTableName(sTABLE_NAME   ) + "_ID";
			string sRELATED_FIELD_NAME = Crm.Modules.SingularTableName(sRELATED_TABLE) + "_ID";
			// 11/24/2012 Paul.  In the special cases of Accounts Related and Contacts Reports To, we need to correct the field name. 
			if ( sMODULE_FIELD_NAME == "ACCOUNT_ID" && sRELATED_FIELD_NAME == "ACCOUNT_ID" )
			{
				sRELATIONSHIP_TABLE = "ACCOUNTS_MEMBERS";
				sRELATED_FIELD_NAME = "PARENT_ID";
			}
			else if ( sMODULE_FIELD_NAME == "CONTACT_ID" && sRELATED_FIELD_NAME == "CONTACT_ID" )
			{
				sRELATIONSHIP_TABLE = "CONTACTS_DIRECT_REPORTS";
				sRELATED_FIELD_NAME = "REPORTS_TO_ID";
			}
			
			DataTable dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, true);
			if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
			{
				sRELATIONSHIP_TABLE = sRELATED_TABLE + "_" + sTABLE_NAME;
				dtSYNC_TABLES = SplendidCache.RestTables("vw" + sRELATIONSHIP_TABLE, true);
				if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count == 0 )
				{
					L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
					throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS") + " to relationship between modules " + ModuleName + " and " + RelatedModule));
				}
			}
			DeleteRelatedItem(sTABLE_NAME, sRELATIONSHIP_TABLE, sMODULE_FIELD_NAME, ID, sRELATED_FIELD_NAME, RelatedID);
		}

		private void DeleteRelatedItem(string sTABLE_NAME, string sRELATIONSHIP_TABLE, string sMODULE_FIELD_NAME, Guid gID, string sRELATED_FIELD_NAME, Guid gRELATED_ID)
		{
			HttpSessionState     Session     = HttpContext.Current.Session    ;
			try
			{
				if ( Security.IsAuthenticated() )
				{
					string sCULTURE = Sql.ToString (Session["USER_SETTINGS/CULTURE"]);
					L10N   L10n     = new L10N(sCULTURE);
					Regex  r        = new Regex(@"[^A-Za-z0-9_]");
					sTABLE_NAME = r.Replace(sTABLE_NAME, "").ToUpper();
					
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 06/03/2011 Paul.  Cache the Rest Table data. 
						// 11/26/2009 Paul.  System tables cannot be updated. 
						// 06/04/2011 Paul.  For relationships, we first need to check the access rights of the parent record. 
						using ( DataTable dtSYNC_TABLES = SplendidCache.RestTables(sTABLE_NAME, true) )
						{
							string sSQL;
							if ( dtSYNC_TABLES != null && dtSYNC_TABLES.Rows.Count > 0 )
							{
								DataRow rowSYNC_TABLE = dtSYNC_TABLES.Rows[0];
								string sMODULE_NAME = Sql.ToString (rowSYNC_TABLE["MODULE_NAME"]);
								string sVIEW_NAME   = Sql.ToString (rowSYNC_TABLE["VIEW_NAME"  ]);
								if ( Sql.IsEmptyString(sMODULE_NAME) )
								{
									throw(new Exception("sMODULE_NAME should not be empty for table " + sTABLE_NAME));
								}
								
								int nACLACCESS = SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "edit");
								// 11/11/2009 Paul.  First check if the user has access to this module. 
								if ( nACLACCESS >= 0 )
								{
									bool      bRecordExists              = false;
									bool      bAccessAllowed             = false;
									Guid      gLOCAL_ASSIGNED_USER_ID    = Guid.Empty;
									DataRow   rowCurrent                 = null;
									DataTable dtCurrent                  = new DataTable();
									sSQL = "select *"              + ControlChars.CrLf
									     + "  from " + sTABLE_NAME + ControlChars.CrLf
									     + " where DELETED = 0"    + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AppendParameter(cmd, gID, "ID");
										using ( DbDataAdapter da = dbf.CreateDataAdapter() )
										{
											((IDbDataAdapter)da).SelectCommand = cmd;
											// 11/27/2009 Paul.  It may be useful to log the SQL during errors at this location. 
											try
											{
												da.Fill(dtCurrent);
											}
											catch
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
												throw;
											}
											if ( dtCurrent.Rows.Count > 0 )
											{
												rowCurrent = dtCurrent.Rows[0];
												bRecordExists = true;
												// 01/18/2010 Paul.  Apply ACL Field Security. 
												if ( dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
												{
													gLOCAL_ASSIGNED_USER_ID = Sql.ToGuid(rowCurrent["ASSIGNED_USER_ID"]);
												}
											}
										}
									}
									// 06/04/2011 Paul.  We are not ready to handle conflicts. 
									//if ( !bConflicted )
									{
										if ( bRecordExists )
										{
											sSQL = "select count(*)"       + ControlChars.CrLf
											     + "  from " + sTABLE_NAME + ControlChars.CrLf;
											using ( IDbCommand cmd = con.CreateCommand() )
											{
												cmd.CommandText = sSQL;
												Security.Filter(cmd, sMODULE_NAME, "delete");
												Sql.AppendParameter(cmd, gID, "ID");
												try
												{
													if ( Sql.ToInteger(cmd.ExecuteScalar()) > 0 )
													{
														if ( (nACLACCESS > ACL_ACCESS.OWNER) || (nACLACCESS == ACL_ACCESS.OWNER && Security.USER_ID == gLOCAL_ASSIGNED_USER_ID) || !dtCurrent.Columns.Contains("ASSIGNED_USER_ID") )
															bAccessAllowed = true;
													}
												}
												catch
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), Sql.ExpandParameters(cmd));
													throw;
												}
											}
										}
										if ( bAccessAllowed )
										{
											// 11/24/2012 Paul.  We do not need to check for RestTable access as that step was already done. 
											IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sRELATIONSHIP_TABLE + "_Delete");
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													cmdDelete.Transaction = trn;
													foreach(IDbDataParameter par in cmdDelete.Parameters)
													{
														string sParameterName = Sql.ExtractDbName(cmdDelete, par.ParameterName).ToUpper();
														if ( sParameterName == sMODULE_FIELD_NAME )
															par.Value = gID;
														else if ( sParameterName == sRELATED_FIELD_NAME )
															par.Value = gRELATED_ID;
														else if ( sParameterName == "MODIFIED_USER_ID" )
															par.Value = Sql.ToDBGuid(Security.USER_ID);
														else
															par.Value = DBNull.Value;
													}
													cmdDelete.ExecuteScalar();
													trn.Commit();
												}
												catch
												{
													trn.Rollback();
													throw;
												}
											}
										}
										else
										{
											throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
										}
									}
								}
								else
								{
									throw(new Exception(L10n.Term("ACL.LBL_NO_ACCESS")));
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}
		#endregion
	}
}

