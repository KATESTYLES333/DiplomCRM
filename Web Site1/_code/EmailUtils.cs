/**********************************************************************************************************************
 * SplendidCRM is a Customer Relationship Management program created by SplendidCRM Software, Inc. 
 * Copyright (C) 2005-2011 SplendidCRM Software, Inc. All rights reserved.
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
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Koolwired.Imap;

namespace SplendidCRM
{
	public class EmailUtils
	{
		// 04/23/2010 Paul.  Make the inside flag public so that we can access from the SystemCheck. 
		public static bool bInsideSendQueue        = false;
		public static bool bInsideCheckInbound     = false;
		public static bool bInsideCheckOutbound    = false;
		public static bool bInsideActivityReminder = false;
		public static bool bInsideSmsActivityReminder = false;

		public static string FormatEmailDisplayName(string sFROM_NAME, string sFROM_ADDR)
		{
			string sDISPLAY_NAME = sFROM_NAME;
			if ( !Sql.IsEmptyString(sFROM_ADDR) )
			{
				if ( !Sql.IsEmptyString(sDISPLAY_NAME) )
					sDISPLAY_NAME += " ";
				sDISPLAY_NAME += "<" + sFROM_ADDR + ">";
			}
			return sDISPLAY_NAME;
		}

		public static bool IsValidEmail(string sEmailAddress)
		{
			/*
			http://www.regexlib.com/
			Expression :  ^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$
			Description:  The most complete email validation routine I could come up with. It verifies that: - Only letters, numbers and email acceptable symbols (+, _, -, .) are allowed - No two different symbols may follow each other - Cannot begin with a symbol - Ending domain ...
			Matches    :  [g_s+gav@com.com], [gav@gav.com], [jim@jim.c.dc.ca]
			Non-Matches:  [gs_.gs@com.com], [gav@gav.c], [jim@--c.ca]
			*/
			Regex r = new Regex(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$");
			return r.Match(sEmailAddress).Success;
		}

		// Cross-Site Scripting (XSS) filter. 
		// http://you.gotfoo.org/howto-anti-xss-w-aspnet-and-c/
		public static string XssFilter(string sHTML, string sXSSTags)
		{
			RegexOptions options = RegexOptions.IgnoreCase;
			string nojavascript = ("([a-z]*)[\\x00-\\x20]*=[\\x00-\\x20]*([\\`\\\'\\\\\"]*)[\\x00-\\x20]*j[\\x00-\\x20]*a[\\x00-\\x20]*v[\\x0" + "0-\\x20]*a[\\x00-\\x20]*s[\\x00-\\x20]*c[\\x00-\\x20]*r[\\x00-\\x20]*i[\\x00-\\x20]*p[\\x00-\\x20]*t[\\x00-\\x20]*");
			Regex regex = new Regex(nojavascript, options);
			string sResult = regex.Replace(sHTML, "");
			if ( !Sql.IsEmptyString(sXSSTags) )
			{
				string unwantedTags = "</*(" + sXSSTags + ")[^>]*>"; 
				regex = new Regex(unwantedTags, options);
				sResult = regex.Replace(sResult, "");
			}
			return sResult;
		}

		public static MailAddress SplitMailAddress(string sFullAddress)
		{
			string sName    = String.Empty;
			string sAddress = String.Empty;
			int nStartAddress = sFullAddress.IndexOf('<');
			if ( nStartAddress > 0 )
			{
				sName = sFullAddress.Substring(0, nStartAddress-1);
				sName = sName.Trim();
				sAddress = sFullAddress.Substring(nStartAddress+1);
				int nEndAddress = sAddress.IndexOf('>');
				if ( nEndAddress >= 0 )
					sAddress = sAddress.Substring(0, nEndAddress);
			}
			else
			{
				sAddress = sFullAddress;
			}
			if ( sName != String.Empty )
				return new MailAddress(sAddress, sName);
			else
				return new MailAddress(sAddress);
		}

		// 01/20/2009 Paul.  We need to fill the emails in the correct order, otherwise $AMOUNT_TOTAL_USDOLLAR would not get replaced properly. 
		public static DataView SortedTableColumns(DataTable dt)
		{
			DataTable dtSorted = new DataTable();
			dtSorted.Columns.Add("ColumnName", typeof(System.String));
			dtSorted.Columns.Add("Length"    , typeof(System.Int32 ));
			foreach ( DataColumn col in dt.Columns )
			{
				DataRow row = dtSorted.NewRow();
				row["ColumnName"] = col.ColumnName.ToLower();
				row["Length"    ] = col.ColumnName.Length;
				dtSorted.Rows.Add(row);
			}
			DataView vwSorted = new DataView(dtSorted);
			vwSorted.Sort = "Length desc";
			return vwSorted;
		}

		// 01/20/2009 Paul.  Make sure to locate money fields and treat as such. 
		public static Hashtable CurrencyColumns(DataView vwColumns)
		{
			Hashtable hash = new Hashtable();
			foreach ( DataRowView rowColumn in vwColumns )
			{
				string sColumnName = Sql.ToString(rowColumn["ColumnName"]);
				if ( sColumnName.EndsWith("_usdollar") )
				{
					if ( !hash.ContainsKey(sColumnName) )
						hash.Add(sColumnName.ToUpper(), null);
					sColumnName = sColumnName.Substring(0, sColumnName.Length - 9);
					if ( !hash.ContainsKey(sColumnName) )
						hash.Add(sColumnName.ToUpper(), null);
				}
			}
			return hash;
		}

		public static Hashtable EnumColumns(HttpApplicationState Application, string sMODULE_NAME)
		{
			Hashtable hashEnumsColumns = new Hashtable();
			if ( !Sql.IsEmptyString(sMODULE_NAME) )
			{
				string sMODULE_TABLE = Sql.ToString(Application["Modules." + sMODULE_NAME + ".TableName"]);
				// 10/20/2009 Paul.  We should be using the ReportingFilterColumns and not the WorkflowFilterColumns. 
				DataView vwEnumColumns = new DataView(SplendidCache.ReportingFilterColumns(Application, sMODULE_TABLE));
				foreach ( DataRowView row in vwEnumColumns )
				{
					if ( Sql.ToString(row["CsType"]) == "enum" )
					{
						string sFieldName = Sql.ToString(row["ColumnName"]);
						string sListName  = SplendidCache.ReportingFilterColumnsListName(Application, sMODULE_NAME, sFieldName);
						if ( !Sql.IsEmptyString(sListName) )
						{
							hashEnumsColumns.Add(sFieldName.ToUpper(), sListName);
						}
					}
				}
			}
			return hashEnumsColumns;
		}

		// 06/03/2009 Paul.  We need to perform list replacements. 
		// 01/20/2009 Paul.  We need to fill the emails in the correct order, otherwise $AMOUNT_TOTAL_USDOLLAR would not get replaced properly. 
		public static string FillEmail(HttpApplicationState Application, string sTEMPLATE_BODY, string sMODULE, DataRow row, DataView vwColumns, Hashtable hashCurrencyColumns, Hashtable hashEnumsColumns)
		{
			string sCultureName = SplendidDefaults.Culture(Application);
			string sEMAIL_BODY = sTEMPLATE_BODY;
			// 12/03/2008 Paul.  Make sure that there is something to replace before going through the effort. 
			if ( !Sql.IsEmptyString(sEMAIL_BODY) && row != null )
			{
				sMODULE = sMODULE.ToLower();
				foreach ( DataRowView rowColumn in vwColumns )
				{
					string sColumnName = Sql.ToString(rowColumn["ColumnName"]);
					if ( row[sColumnName] == DBNull.Value )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, String.Empty);
					else if ( hashCurrencyColumns != null && hashCurrencyColumns.ContainsKey(sColumnName.ToUpper()) )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, Sql.ToDecimal(row[sColumnName]).ToString("c"));
					// 06/03/2009 Paul.  The hash is case-significant, so make sure to convert to upper. 
					else if ( hashEnumsColumns != null && hashEnumsColumns.ContainsKey(sColumnName.ToUpper()) )
					{
						string sValue = Sql.ToString(row[sColumnName]);
						string sTerm = Sql.ToString(L10N.Term(Application, sCultureName, "." + Sql.ToString(hashEnumsColumns[sColumnName.ToUpper()]) + ".", sValue));
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, sTerm);
					}
					else
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, Sql.ToString(row[sColumnName]));
				}
			}
			return sEMAIL_BODY;
		}

		// 06/03/2009 Paul.  We need to perform list replacements. 
		public static string FillEmail(HttpApplicationState Application, string sTEMPLATE_BODY, string sMODULE, DataRowView row, DataView vwColumns, Hashtable hashCurrencyColumns, Hashtable hashEnumsColumns)
		{
			string sCultureName = SplendidDefaults.Culture(Application);
			string sEMAIL_BODY = sTEMPLATE_BODY;
			// 12/03/2008 Paul.  Make sure that there is something to replace before going through the effort. 
			if ( !Sql.IsEmptyString(sEMAIL_BODY) && row != null )
			{
				sMODULE = sMODULE.ToLower();
				foreach ( DataRowView rowColumn in vwColumns )
				{
					string sColumnName = Sql.ToString(rowColumn["ColumnName"]);
					if ( row[sColumnName] == DBNull.Value )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, String.Empty);
					else if ( hashCurrencyColumns != null && hashCurrencyColumns.ContainsKey(sColumnName.ToUpper()) )
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, Sql.ToDecimal(row[sColumnName]).ToString("c"));
					// 06/03/2009 Paul.  The hash is case-significant, so make sure to convert to upper. 
					else if ( hashEnumsColumns != null && hashEnumsColumns.ContainsKey(sColumnName.ToUpper()) )
					{
						string sValue = Sql.ToString(row[sColumnName]);
						string sTerm = Sql.ToString(L10N.Term(Application, sCultureName, "." + Sql.ToString(hashEnumsColumns[sColumnName.ToUpper()]) + ".", sValue));
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, sTerm);
					}
					else
						sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sColumnName, Sql.ToString(row[sColumnName]));
				}
			}
			return sEMAIL_BODY;
		}

		// 10/04/2008 Paul.  We use a Hashtable in the workflow engine. 
		// 01/20/2009 Paul.  The workflow engine uses the proper table-based FillEmail function. 
		/*
		public static string FillEmail1(string sTEMPLATE_BODY, string sMODULE, Hashtable hash)
		{
			string sEMAIL_BODY = sTEMPLATE_BODY;
			// 12/03/2008 Paul.  Make sure that there is something to replace before going through the effort. 
			if ( !Sql.IsEmptyString(sEMAIL_BODY) )
			{
				sMODULE = sMODULE.ToLower();
				foreach ( string sKey in hash.Keys )
				{
					sEMAIL_BODY = sEMAIL_BODY.Replace("$" + sMODULE + "_" + sKey.ToLower(), Sql.ToString(hash[sKey]));
				}
			}
			return sEMAIL_BODY;
		}
		*/

		// 06/28/2008 Paul.  The function cannot rely upon the HttpContext to get the application. 
		public static DataTable CampaignTrackers(HttpContext Context, Guid gID)
		{
			DataTable dt = new DataTable();
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select ID                        " + ControlChars.CrLf
					     + "     , TRACKER_NAME              " + ControlChars.CrLf
					     + "     , TRACKER_URL               " + ControlChars.CrLf
					     + "     , IS_OPTOUT                 " + ControlChars.CrLf
					     + "  from vwCAMPAIGNS_CAMPAIGN_TRKRS" + ControlChars.CrLf
					     + " where CAMPAIGN_ID = @CAMPAIGN_ID" + ControlChars.CrLf
					     + " order by DATE_ENTERED asc       " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CAMPAIGN_ID", gID);
		
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				throw(new Exception("CampaignTrackers failed " + gID.ToString(), ex));
			}
			return dt;
		}

		// 06/28/2008 Paul.  The function cannot rely upon the HttpContext to get the application. 
		public static DataTable EmailTemplateAttachments(HttpContext Context, Guid gID)
		{
			DataTable dt = new DataTable();
			// 07/17/2008 Paul.  Add code to protect and track problems.
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *                            " + ControlChars.CrLf
					     + "  from vwEMAIL_TEMPLATES_Attachments" + ControlChars.CrLf
					     + " where EMAIL_TEMPLATE_ID = @EMAIL_TEMPLATE_ID" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@EMAIL_TEMPLATE_ID", gID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				throw(new Exception("EmailTemplateAttachments failed " + gID.ToString(), ex));
			}
			return dt;
		}

		// 07/16/2008 Paul.  We need to pass the Application object so that L10n_Term() can be used. 
		public static string FillTrackers(HttpContext Context, string sBODY_HTML, DataTable dtTrackers, string sSiteURL, Guid gTARGET_TRACKER_KEY)
		{
			// 07/17/2008 Paul.  Add code to protect and track problems.
			try
			{
				if ( dtTrackers != null )
				{
					bool bHAS_OPTOUT_LINKS = false;
					foreach ( DataRow row in dtTrackers.Rows )
					{
						Guid   gTRACKER_ID   = Sql.ToGuid   (row["ID"          ]);
						string sTRACKER_NAME = Sql.ToString (row["TRACKER_NAME"]);
						string sTRACKER_URL  = Sql.ToString (row["TRACKER_URL" ]);
						bool   bIS_OPTOUT    = Sql.ToBoolean(row["IS_OPTOUT"   ]);
						string sTrackerPath  = String.Empty;
						// 04/06/2010 Paul.  Use better logic to allow more flexible RemoveMe URL. 
						if ( bIS_OPTOUT )
						{
							bHAS_OPTOUT_LINKS = true;
							if ( sTRACKER_URL.ToLower().StartsWith("http") )
								sTrackerPath = sTRACKER_URL;
							else
								sTrackerPath = sSiteURL;
							if ( !sTrackerPath.Contains(".aspx") )
							{
								if ( !sTrackerPath.EndsWith("/") )
									sTrackerPath += "/";
								sTrackerPath += "RemoveMe.aspx";
							}
							if ( !sTrackerPath.Contains("?identifier=") )
								sTrackerPath += "?identifier=";
							sTrackerPath += gTARGET_TRACKER_KEY.ToString();
						}
						else
						{
							sTrackerPath  = sSiteURL;
							sTrackerPath += "campaign_trackerv2.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString();
							sTrackerPath += "&track=" + gTRACKER_ID.ToString();
						}
						sBODY_HTML = sBODY_HTML.Replace("{" + sTRACKER_NAME + "}", sTrackerPath);
					}

					// 07/16/2008 Paul.  L10n.Term() is not available here.  Use L10n_Term() instead. 
					if ( !bHAS_OPTOUT_LINKS )
					{
						// 07/30/2008 Paul.  Lookup the default culture. 
						string sCultureName = SplendidDefaults.Culture(Context.Application);
						// 07/30/2008 Paul.  Use a new static version of L10N.Term() that is better about sharing the code. 
						sBODY_HTML += "<br><font size='2'>" + HttpUtility.HtmlEncode(L10N.Term(Context.Application, sCultureName, "EmailMan.TXT_REMOVE_ME")) + "<a href='" + sSiteURL + "RemoveMe.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString() + "'>" + HttpUtility.HtmlEncode(L10N.Term(Context.Application, sCultureName, "EmailMan.TXT_REMOVE_ME_CLICK")) + "</a></font>";
					}
					sBODY_HTML += "<br><img height='1' width='1' src='" + sSiteURL + "image.aspx?identifier=" + gTARGET_TRACKER_KEY.ToString() + "'>";
				}
				else
				{
					SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "dtTrackers should never be NULL.");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				throw(new Exception("FillTrackers failed " + gTARGET_TRACKER_KEY.ToString(), ex));
			}
			return sBODY_HTML;
		}

		public static SmtpClient CreateSmtpClient(HttpApplicationState Application)
		{
			return CreateSmtpClient(Application, null);
		}

		public static SmtpClient CreateSmtpClient(HttpApplicationState Application, NetworkCredential ncUserCredential)
		{
			string sSmtpServer   = Sql.ToString (Application["CONFIG.smtpserver"  ]);
			int    nSmtpPort     = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
			bool   bSmtpAuthReq  = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
			bool   bSmtpSSL      = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
			return CreateSmtpClient(Application, ncUserCredential, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL);
		}

		// 07/19/2010 Paul.  Create a new method so we can provide a way to skip the decryption of the system password. 
		// 07/18/2013 Paul.  Add support for multiple outbound emails. 
		public static SmtpClient CreateSmtpClient(HttpApplicationState Application, NetworkCredential ncUserCredential, string sSmtpServer, int nSmtpPort, bool bSmtpAuthReq, bool bSmtpSSL)
		{
			string sSmtpUser        = Sql.ToString (Application["CONFIG.smtpuser"       ]);
			string sSmtpPassword    = Sql.ToString (Application["CONFIG.smtppass"       ]);
			string sX509Certificate = Sql.ToString (Application["CONFIG.smtpcertificate"]);
			
			if ( Sql.IsEmptyString(sSmtpServer) )
			{
				sSmtpServer   = Sql.ToString (Application["CONFIG.smtpserver"  ]);
				nSmtpPort     = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
				bSmtpAuthReq  = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
				bSmtpSSL      = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
			}

			if ( ncUserCredential == null )
			{
				// 01/12/2008 Paul.  We must decrypt the password before using it. 
				if ( !Sql.IsEmptyString(sSmtpPassword) )
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					sSmtpPassword = Security.DecryptPassword(sSmtpPassword, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
				}
			}
			if ( Sql.IsEmptyString(sSmtpServer) )
				sSmtpServer = "127.0.0.1";
			if ( nSmtpPort == 0 )
				nSmtpPort = 25;

			// 04/17/2006 Paul.  Use config value for SMTP server. 
			// 12/21/2006 Paul.  Allow the use of SMTP servers that require authentication. 
			// 07/21/2013 Paul.  Gmail should use 587 and not 465 with EnableSsl. 
			// http://stackoverflow.com/questions/1082216/gmail-smtp-via-c-sharp-net-errors-on-all-ports
			SmtpClient client = new SmtpClient(sSmtpServer, nSmtpPort);
			client.Timeout = 60 * 1000;
			// 01/12/2008 Paul.  Use SMTP SSL flag to support Gmail. 
			if ( bSmtpSSL )
			{
				client.EnableSsl = true;
				// 11/16/2009 Paul.  One of our Live clients would like to use a client certificate for SMTP. 
				// 07/19/2010 Paul.  We are not going to support user certificates at this time. 
				if ( ncUserCredential == null && !Sql.IsEmptyString(sX509Certificate) )
				{
					try
					{
						X509Certificate cert = HttpRuntime.Cache.Get("SMTP.X509Certificate") as X509Certificate;
						if ( cert == null )
						{
							const string sCertHeader = "-----BEGIN CERTIFICATE-----";
							const string sCertFooter = "-----END CERTIFICATE-----";
							sX509Certificate = sX509Certificate.Trim();
							if (sX509Certificate.StartsWith(sCertHeader) && sX509Certificate.EndsWith(sCertFooter))
							{
								sX509Certificate = sX509Certificate.Substring(sCertHeader.Length, sX509Certificate.Length - sCertHeader.Length - sCertFooter.Length);
								byte[] byPKS8  = Convert.FromBase64String(sX509Certificate.Trim());
								
								cert = new X509Certificate(byPKS8);
							}
							else
							{
								throw(new Exception("Invalid X509 Certificate.  Missing BEGIN CERTIFICATE or END CERTIFICATE."));
							}
							HttpRuntime.Cache.Insert("SMTP.X509Certificate", cert, null, SplendidCache.DefaultCacheExpiration(), System.Web.Caching.Cache.NoSlidingExpiration);
						}
						if ( cert != null )
							client.ClientCertificates.Add(cert);
					}
					catch(Exception ex)
					{
						SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), "Failed to add SMTP certificate to email: " + Utils.ExpandException(ex));
					}
				}
			}
			// 07/19/2010 Paul.  Use the user credentials if provided. 
			if ( ncUserCredential != null )
				client.Credentials = ncUserCredential;
			else if ( bSmtpAuthReq )
				client.Credentials = new NetworkCredential(sSmtpUser, sSmtpPassword);
			else
				client.UseDefaultCredentials = true;
			return client;
		}

		public static void SendTestMessage(HttpApplicationState Application, string sSmtpServer, int nSmtpPort, bool bSmtpAuthReq, bool bSmtpSSL, string sSmtpUser, string sSmtpPassword, string sFromAddress, string sFromName, string sToAddress, string sToName)
		{
			string sX509Certificate = Sql.ToString (Application["CONFIG.smtpcertificate"]);
			// 04/17/2006 Paul.  Use config value for SMTP server. 
			// 12/21/2006 Paul.  Allow the use of SMTP servers that require authentication. 
			// 07/21/2013 Paul.  Gmail should use 587 and not 465 with EnableSsl. 
			// http://stackoverflow.com/questions/1082216/gmail-smtp-via-c-sharp-net-errors-on-all-ports
			SmtpClient client = new SmtpClient(sSmtpServer, nSmtpPort);
			client.Timeout = 60 * 1000;
			// 01/12/2008 Paul.  Use SMTP SSL flag to support Gmail. 
			if ( bSmtpSSL )
			{
				client.EnableSsl = true;
				// 11/16/2009 Paul.  One of our Live clients would like to use a client certificate for SMTP. 
				if ( !Sql.IsEmptyString(sX509Certificate) )
				{
					try
					{
						X509Certificate cert = HttpRuntime.Cache.Get("SMTP.X509Certificate") as X509Certificate;
						if ( cert == null )
						{
							const string sCertHeader = "-----BEGIN CERTIFICATE-----";
							const string sCertFooter = "-----END CERTIFICATE-----";
							sX509Certificate = sX509Certificate.Trim();
							if (sX509Certificate.StartsWith(sCertHeader) && sX509Certificate.EndsWith(sCertFooter))
							{
								sX509Certificate = sX509Certificate.Substring(sCertHeader.Length, sX509Certificate.Length - sCertHeader.Length - sCertFooter.Length);
								byte[] byPKS8  = Convert.FromBase64String(sX509Certificate.Trim());
								
								cert = new X509Certificate(byPKS8);
							}
							else
							{
								throw(new Exception("Invalid X509 Certificate.  Missing BEGIN CERTIFICATE or END CERTIFICATE."));
							}
							HttpRuntime.Cache.Insert("SMTP.X509Certificate", cert, null, SplendidCache.DefaultCacheExpiration(), System.Web.Caching.Cache.NoSlidingExpiration);
						}
						if ( cert != null )
							client.ClientCertificates.Add(cert);
					}
					catch(Exception ex)
					{
						SplendidError.SystemMessage(Application, "Error", new StackTrace(true).GetFrame(0), "Failed to add SMTP certificate to email: " + Utils.ExpandException(ex));
					}
				}
			}
			if ( bSmtpAuthReq )
				client.Credentials = new NetworkCredential(sSmtpUser, sSmtpPassword);
			else
				client.UseDefaultCredentials = true;
			
			MailMessage mail = new MailMessage();
			MailAddress addr = null;
			if ( Sql.IsEmptyString(sFromName) )
				mail.From = new MailAddress(sFromAddress);
			else
				mail.From = new MailAddress(sFromAddress, sFromName);
			if ( Sql.IsEmptyString(sFromName) )
				addr = new MailAddress(sToAddress);
			else
				addr = new MailAddress(sToAddress, sToName);
			mail.To.Add(addr);
			mail.Subject = "SplendidCRM Test Email " + DateTime.Now.ToString();
			mail.Body    = "This is a test.";
			client.Send(mail);
		}

		// 05/19/2008 Paul.  Application is a required parameter so that SendEmail can be called within the scheduler. 
		public static void SendEmail(HttpContext Context, Guid gID, string sFromName, string sFromAddress, ref int nEmailsSent)
		{
			MailMessage mail = new MailMessage();
			DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL ;
				bool bReadyToSend = false;
				DataTable dtContacts  = new DataTable();
				DataTable dtLeads     = new DataTable();
				DataTable dtProspects = new DataTable();
				// 10/05/2007 Paul.  The vwEMAILS_CONTACTS view handles the join and returns all vwCONTACTS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_CONTACTS   " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtContacts);
					}
				}
				// 10/05/2007 Paul.  The vwEMAILS_LEADS view handles the join and returns all vwLEADS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_LEADS      " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtLeads);
					}
				}
				// 10/05/2007 Paul.  The vwEMAILS_PROSPECTS view handles the join and returns all vwPROSPECTS data. 
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_PROSPECTS  " + ControlChars.CrLf
				     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_ID", gID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtProspects);
					}
				}
				DataView vwContacts  = new DataView(dtContacts );
				DataView vwLeads     = new DataView(dtLeads    );
				DataView vwProspects = new DataView(dtProspects);
				// 01/20/2009 Paul.  We need to fill the emails in the correct order, otherwise $AMOUNT_TOTAL_USDOLLAR would not get replaced properly. 
				DataView  vwContactsParentColumns      = SortedTableColumns(dtContacts );
				DataView  vwLeadsParentColumns         = SortedTableColumns(dtLeads    );
				DataView  vwProspectsParentColumns     = SortedTableColumns(dtProspects);
				Hashtable hashContactsCurrencyColumns  = CurrencyColumns(vwContactsParentColumns );
				Hashtable hashLeadsCurrencyColumns     = CurrencyColumns(vwLeadsParentColumns    );
				Hashtable hashProspectsCurrencyColumns = CurrencyColumns(vwProspectsParentColumns);
				string[] arrTo = new string[] {};
				
				// 07/19/2010 Paul.  Each user can have their own email account, but they all will share the same server. 
				string sMAIL_SMTPUSER     = String.Empty;
				string sMAIL_SMTPPASS     = String.Empty;
				string sEXCHANGE_ALIAS    = String.Empty;
				string sEXCHANGE_EMAIL    = String.Empty;
				// 07/18/2013 Paul.  Add support for multiple outbound emails. 
				string sMAIL_SMTPSERVER   = String.Empty;
				int    nMAIL_SMTPPORT     = 0;
				bool   bMAIL_SMTPAUTH_REQ = false;
				bool   bMAIL_SMTPSSL      = false;
				
				// 04/27/2011 Paul.  When sending to multiple TO addresses, we need to reset the subject and body. 
				bool   bEmailChanged = false;
				string sSubject      = String.Empty;
				string sBody         = String.Empty;
				string sBodyHtml     = String.Empty;
				sSQL = "select *                   " + ControlChars.CrLf
				     + "  from vwEMAILS_ReadyToSend" + ControlChars.CrLf
				     + " where ID = @ID            " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							bReadyToSend = true;
							// 05/19/2008 Paul.  Email From information may already be split between FROM_ADDR and FROM_NAME. 
							string sFROM_ADDR   = Sql.ToString(rdr["FROM_ADDR"       ]);
							string sFROM_NAME   = Sql.ToString(rdr["FROM_NAME"       ]);
							string sTo          = Sql.ToString(rdr["TO_ADDRS"        ]);
							string sCC          = Sql.ToString(rdr["CC_ADDRS"        ]);
							string sBcc         = Sql.ToString(rdr["BCC_ADDRS"       ]);
							string sPARENT_TYPE = Sql.ToString(rdr["PARENT_TYPE"     ]);
							Guid   gPARENT_ID   = Sql.ToGuid  (rdr["PARENT_ID"       ]);
							try
							{
								sMAIL_SMTPUSER  = Sql.ToString(rdr["MAIL_SMTPUSER" ]);
								sMAIL_SMTPPASS  = Sql.ToString(rdr["MAIL_SMTPPASS" ]);
								sEXCHANGE_ALIAS = Sql.ToString(rdr["EXCHANGE_ALIAS"]);
								sEXCHANGE_EMAIL = Sql.ToString(rdr["EXCHANGE_EMAIL"]);
							}
							catch(Exception ex)
							{
								SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
							}
							// 07/18/2013 Paul.  Add support for multiple outbound emails. 
							try
							{
								sMAIL_SMTPSERVER   = Sql.ToString (rdr["MAIL_SMTPSERVER"  ]);
								nMAIL_SMTPPORT     = Sql.ToInteger(rdr["MAIL_SMTPPORT"    ]);
								bMAIL_SMTPAUTH_REQ = Sql.ToBoolean(rdr["MAIL_SMTPAUTH_REQ"]);
								bMAIL_SMTPSSL      = Sql.ToBoolean(rdr["MAIL_SMTPSSL"     ]);
							}
							catch(Exception ex)
							{
								SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
							}
							// 04/27/2011 Paul.  When sending to multiple TO addresses, we need to reset the subject and body. 
							sSubject     = Sql.ToString(rdr["NAME"            ]);
							sBody        = Sql.ToString(rdr["DESCRIPTION"     ]);
							sBodyHtml    = Sql.ToString(rdr["DESCRIPTION_HTML"]);

							// 12/19/2006 Paul.  Fill the email with parent data. 
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								// 05/19/2008 Paul.  Application is a required parameter so that SendEmail can be called within the scheduler. 
								DataTable dtParent = Crm.Modules.Parent(Context.Application, sPARENT_TYPE, gPARENT_ID);
								// 01/20/2009 Paul.  We need to fill the emails in the correct order, otherwise $AMOUNT_TOTAL_USDOLLAR would not get replaced properly. 
								DataView  vwParentColumns = SortedTableColumns(dtParent);
								Hashtable hashCurrencyColumns = CurrencyColumns(vwParentColumns);
								// 06/03/2009 Paul.  Allow the lists to be translated. 
								Hashtable hashEnumsColumns = EmailUtils.EnumColumns(Context.Application, sPARENT_TYPE);
								if ( dtParent.Rows.Count > 0 )
								{
									string sFillPrefix = String.Empty;
									switch ( sPARENT_TYPE )
									{
										// 09/03/2008 Paul.  Just fall-through for Accounts and Contacts. 
										//case "Accounts" :  sFillPrefix = "account";  break;
										//case "Contacts" :  sFillPrefix = "contact";  break;
										// 09/03/2008 Paul.  For compatibility with SugarCRM, we will keep the Leads -> contact conversion
										// but we will also apply Leads -> lead. 
										case "Leads"    :
											sFillPrefix = "lead";
											sSubject  = EmailUtils.FillEmail(Context.Application, sSubject , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											// 12/03/2008 Paul.  Also replace the plain-text body. 
											sBody     = EmailUtils.FillEmail(Context.Application, sBody    , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Context.Application, sBodyHtml, sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sFillPrefix = "contact";
											sSubject  = EmailUtils.FillEmail(Context.Application, sSubject , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBody     = EmailUtils.FillEmail(Context.Application, sBody    , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Context.Application, sBodyHtml, sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											break;
										// 09/03/2008 Paul.  For compatibility with SugarCRM, we will keep the Prospects -> contact conversion
										// but we will also apply Prospects -> prospect. 
										case "Prospects":
											sFillPrefix = "prospect";
											sSubject  = EmailUtils.FillEmail(Context.Application, sSubject , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											// 12/03/2008 Paul.  Also replace the plain-text body. 
											sBody     = EmailUtils.FillEmail(Context.Application, sBody    , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Context.Application, sBodyHtml, sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sFillPrefix = "contact";
											sSubject  = EmailUtils.FillEmail(Context.Application, sSubject , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBody     = EmailUtils.FillEmail(Context.Application, sBody    , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Context.Application, sBodyHtml, sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											break;
										default:
											sFillPrefix = sPARENT_TYPE.ToLower();
											if ( sFillPrefix.EndsWith("ies") )
												sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-3) + "y";
											else if ( sFillPrefix.EndsWith("s") )
												sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-1);
											// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
											sSubject  = EmailUtils.FillEmail(Context.Application, sSubject , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											// 12/03/2008 Paul.  Also replace the plain-text body. 
											sBody     = EmailUtils.FillEmail(Context.Application, sBody    , sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Context.Application, sBodyHtml, sFillPrefix, dtParent.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											break;
									}
									// 04/27/2011 Paul.  If the email body changes due to insertions, then update the email. 
									if ( sSubject != Sql.ToString(rdr["NAME"]) || sBody != Sql.ToString(rdr["DESCRIPTION"]) || sBodyHtml != Sql.ToString(rdr["DESCRIPTION_HTML"]) )
										bEmailChanged = true;
								}
							}

							if ( Sql.IsEmptyString(sFROM_ADDR) && !Sql.IsEmptyString(sFromAddress) )
								mail.From = new MailAddress(sFromAddress, sFromName);
							// 05/19/2008 Paul.  Email From information may already be split between FROM_ADDR and FROM_NAME. 
							else if ( !Sql.IsEmptyString(sFROM_ADDR) && !Sql.IsEmptyString(sFROM_NAME) )
								mail.From = new MailAddress(sFROM_ADDR, sFROM_NAME);
							else if ( !Sql.IsEmptyString(sFROM_ADDR) )
								mail.From = EmailUtils.SplitMailAddress(sFROM_ADDR);
							
							// 12/19/2006 Paul.  We are going to send each email in the TO field as a separate email. 
							arrTo = sTo.Split(';');
							/*
							foreach ( string sAddress in arrTo )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.To.Add(SEmailUtils.plitMailAddress(sAddress));
							}
							*/
							string[] arrAddresses = sCC.Split(';');
							foreach ( string sAddress in arrAddresses )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.CC.Add(EmailUtils.SplitMailAddress(sAddress));
							}
							arrAddresses = sBcc.Split(';');
							foreach ( string sAddress in arrAddresses )
							{
								if ( sAddress.Trim() != String.Empty )
									mail.Bcc.Add(EmailUtils.SplitMailAddress(sAddress));
							}
							mail.Subject     = sSubject       ;
							if ( !Sql.IsEmptyString(sBodyHtml) )
							{
								mail.Body         = sBodyHtml;
								// 08/24/2006 Paul.  Set the encoding to UTF8. 
								mail.BodyEncoding = System.Text.Encoding.UTF8;
								mail.IsBodyHtml   = true;
							}
							else
							{
								mail.Body       = sBody    ;
							}
							mail.Headers.Add("X-SplendidCRM-ID", gID.ToString());
						}
						else
						{
							// 05/19/2008 Paul.  It is possible that the email might have already been sent, so just ignore this issue. 
							//throw(new Exception("SendEmail: Email is not ready to send, " + gID.ToString()));
						}
					}
				}

				if ( bReadyToSend )
				{
					// 07/30/2006 Paul.  .NET 2.0 now supports sending mail from a stream, remove the directory stuff. 
					using ( DataTable dtAttachments = new DataTable() )
					{
						sSQL = "select *                   " + ControlChars.CrLf
						     + "  from vwEMAILS_Attachments" + ControlChars.CrLf
						     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@EMAIL_ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								da.Fill(dtAttachments);
							}
						}
						
						try
						{
							if ( dtAttachments.Rows.Count > 0 )
							{
								foreach(DataRow row in dtAttachments.Rows)
								{
									string sFILENAME           = Sql.ToString(row["FILENAME"          ]);
									string sFILE_MIME_TYPE     = Sql.ToString(row["FILE_MIME_TYPE"    ]);
									Guid   gNOTE_ATTACHMENT_ID = Sql.ToGuid  (row["NOTE_ATTACHMENT_ID"]);

									// 07/30/2006 Paul.  We cannot close the streams until the message is sent. 
									MemoryStream mem = new MemoryStream();
									BinaryWriter writer = new BinaryWriter(mem);
									Notes.Attachment.WriteStream(gNOTE_ATTACHMENT_ID, con, writer);
									writer.Flush();
									mem.Seek(0, SeekOrigin.Begin);
									Attachment att = new Attachment(mem, sFILENAME, sFILE_MIME_TYPE);
									// 06/02/2014 Tomi.  Make sure to use UTF8 encoding for the name. 
									att.NameEncoding = System.Text.Encoding.UTF8;
									mail.Attachments.Add(att);
								}
							}
							// 10/04/2008 Paul.  Move SmtpClient code to a shared function. 
							SmtpClient client = null;
							// 07/19/2010 Paul.  If the user has his own login, then send using it. 
							if ( !Sql.IsEmptyString(sMAIL_SMTPUSER) )
							{
								// 07/19/2010 Paul.  Although it is not efficient to decrypt on-the-fly, it will prevent 
								Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Context.Application["CONFIG.InboundEmailKey"]);
								Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Context.Application["CONFIG.InboundEmailIV" ]);
								if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
									sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
								// 07/19/2010 Paul.  We create the credentials object in advance so that we don't waste time decrypting the system password. 
								NetworkCredential ncUserCredentials = new NetworkCredential(sMAIL_SMTPUSER, sMAIL_SMTPPASS);
								// 07/18/2013 Paul.  Add support for multiple outbound emails. 
								client = CreateSmtpClient(Context.Application, ncUserCredentials, sMAIL_SMTPSERVER, nMAIL_SMTPPORT, bMAIL_SMTPAUTH_REQ, bMAIL_SMTPSSL);
							}
							else
							{
								// 07/18/2013 Paul.  Add support for multiple outbound emails. 
								client = CreateSmtpClient(Context.Application, null, sMAIL_SMTPSERVER, nMAIL_SMTPPORT, bMAIL_SMTPAUTH_REQ, bMAIL_SMTPSSL);
							}

							if ( arrTo.Length > 0 )
							{
								for ( int nAddressIndex = 0; nAddressIndex < arrTo.Length; nAddressIndex++ )
								{
									string sAddress = arrTo[nAddressIndex];
									// 04/27/2011 Paul.  When sending to multiple TO addresses, we need to reset the subject and body. 
									bool   bToEmailChanged = false;
									string sToSubject  = sSubject ;
									string sToBody     = sBody    ;
									string sToBodyHtml = sBodyHtml;
									if ( sAddress.Trim() != String.Empty )
									{
										MailAddress addr = EmailUtils.SplitMailAddress(sAddress);
										mail.To.Clear();
										mail.To.Add(addr);
										// 12/19/2006 Paul.  The address can be in any one of three tables.  
										// Try and filter on the minimum number of tables for performance reasons. 
										// 01/25/2014 Paul.  Need to escape the address as it can contain an apostrophe. 
										vwContacts.RowFilter = "EMAIL1 = '" + Sql.EscapeSQL(addr.Address) + "'";
										if ( vwContacts.Count > 0 )
										{
											// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
											sToSubject  = EmailUtils.FillEmail(Context.Application, sToSubject , "contact", vwContacts[0], vwContactsParentColumns, hashContactsCurrencyColumns, null);
											sToBody     = EmailUtils.FillEmail(Context.Application, sToBody    , "contact", vwContacts[0], vwContactsParentColumns, hashContactsCurrencyColumns, null);
											sToBodyHtml = EmailUtils.FillEmail(Context.Application, sToBodyHtml, "contact", vwContacts[0], vwContactsParentColumns, hashContactsCurrencyColumns, null);
										}
										else
										{
											// 01/25/2014 Paul.  Need to escape the address as it can contain an apostrophe. 
											vwLeads.RowFilter = "EMAIL1 = '" + Sql.EscapeSQL(addr.Address) + "'";
											if ( vwLeads.Count > 0 )
											{
												sToSubject  = EmailUtils.FillEmail(Context.Application, sToSubject , "contact", vwLeads[0], vwLeadsParentColumns, hashLeadsCurrencyColumns, null);
												sToBody     = EmailUtils.FillEmail(Context.Application, sToBody    , "contact", vwLeads[0], vwLeadsParentColumns, hashLeadsCurrencyColumns, null);
												sToBodyHtml = EmailUtils.FillEmail(Context.Application, sToBodyHtml, "contact", vwLeads[0], vwLeadsParentColumns, hashLeadsCurrencyColumns, null);
											}
											else
											{
												// 01/25/2014 Paul.  Need to escape the address as it can contain an apostrophe. 
												vwProspects.RowFilter = "EMAIL1 = '" + Sql.EscapeSQL(addr.Address) + "'";
												if ( vwProspects.Count > 0 )
												{
													sToSubject  = EmailUtils.FillEmail(Context.Application, sToSubject , "contact", vwProspects[0], vwProspectsParentColumns, hashProspectsCurrencyColumns, null);
													sToBody     = EmailUtils.FillEmail(Context.Application, sToBody    , "contact", vwProspects[0], vwProspectsParentColumns, hashProspectsCurrencyColumns, null);
													sToBodyHtml = EmailUtils.FillEmail(Context.Application, sToBodyHtml, "contact", vwProspects[0], vwProspectsParentColumns, hashProspectsCurrencyColumns, null);
												}
											}
										}
										if ( sToSubject != sSubject || sToBody != sBody )
											bToEmailChanged = true;
										mail.Subject = sToSubject;
										mail.Body    = !Sql.IsEmptyString(sToBodyHtml) ? sToBodyHtml : sToBody;
										
										client.Send(mail);
										nEmailsSent++;
										// 12/19/2006 Paul.  Clear the CC and BCC after first send so that they only get one email. 
										mail.CC.Clear();
										mail.Bcc.Clear();
										// 04/27/2011 Paul.  If the email body changes due to insertions, then update the email. 
										if ( bEmailChanged || bToEmailChanged )
										{
											// 04/27/2011 Paul.  If there is only one recipient, then update the email directly. 
											if ( arrTo.Length == 1 )
											{
												using ( IDbTransaction trn = Sql.BeginTransaction(con) )
												{
													try
													{
														SqlProcs.spEMAILS_UpdateContent(gID, sSubject, sToBody, sToBodyHtml, trn);
														trn.Commit();
													}
													catch(Exception ex)
													{
														trn.Rollback();
														throw(new Exception(ex.Message, ex.InnerException));
													}
												}
												// 04/27/2011 Paul.  Clear the main changed flag so that the original email only gets updated once. 
												bEmailChanged = false;
											}
											else
											{
												// 04/27/2011 Paul.  If there are multiple To recipients, we still want to update the original email, 
												// but we also want to insert one Archived email per user. 
												if ( bEmailChanged )
												{
													using ( IDbTransaction trn = Sql.BeginTransaction(con) )
													{
														try
														{
															SqlProcs.spEMAILS_UpdateContent(gID, sSubject, sBody, sBodyHtml, trn);
															trn.Commit();
														}
														catch(Exception ex)
														{
															trn.Rollback();
															throw(new Exception(ex.Message, ex.InnerException));
														}
													}
													// 04/27/2011 Paul.  Clear the main changed flag so that the original email only gets updated once. 
													bEmailChanged = false;
												}
												using ( IDbTransaction trn = Sql.BeginTransaction(con) )
												{
													try
													{
														SqlProcs.spEMAILS_ArchiveContent(gID, sSubject, sToBody, sToBodyHtml, (nAddressIndex == 0), trn);
														trn.Commit();
													}
													catch(Exception ex)
													{
														trn.Rollback();
														throw(new Exception(ex.Message, ex.InnerException));
													}
												}
											}
										}
									}
								}
							}
							else if ( mail.CC.Count > 0 || mail.Bcc.Count > 0 )
							{
								// 12/19/2006 Paul.  Still send the email even if there are no TO addresses. 
								client.Send(mail);
								nEmailsSent++;
								// 04/27/2011 Paul.  If the email body changes due to insertions, then update the email. 
								if ( bEmailChanged )
								{
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										try
										{
											SqlProcs.spEMAILS_UpdateContent(gID, sSubject, sBody, sBodyHtml, trn);
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											throw(new Exception(ex.Message, ex.InnerException));
										}
									}
								}
							}
							else
								throw(new Exception("SendEmail: No addresses"));
						}
						finally
						{
							// 07/30/2006 Paul.  Close the streams after the message is sent. 
							foreach ( Attachment att in mail.Attachments )
							{
								if ( att.ContentStream != null )
									att.ContentStream.Close();
							}
						}
					}
				}
			}
		}

		public static void SendQueued(HttpContext Context, Guid gID, Guid gCAMPAIGN_ID, bool bSendNow)
		{
			if ( !bInsideSendQueue )
			{
				bInsideSendQueue = true;
				Hashtable hashTrackers    = new Hashtable();
				Hashtable hashAttachments = new Hashtable();
				Hashtable hashNoteStreams = new Hashtable();
				try
				{
					//SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "SendQueued Begin");

					string sAttachmentLabel = L10N.Term    (Context.Application, "en-US", "Emails.LBL_EMAIL_ATTACHMENT");
					string sFromName        = Sql.ToString (Context.Application["CONFIG.fromname"    ]);
					string sFromAddress     = Sql.ToString (Context.Application["CONFIG.fromaddress" ]);
					int    nEmailsPerRun    = Sql.ToInteger(Context.Application["CONFIG.massemailer_campaign_emails_per_run"]);
					if ( nEmailsPerRun == 0 )
						nEmailsPerRun = 500;
					
					// 10/04/2008 Paul.  Move SmtpClient code to a shared function. 
					SmtpClient client = CreateSmtpClient(Context.Application);

					// 07/16/2008 Paul.  We can't use L10N because it requires a valid Application object. 
					//L10N L10n = new L10N(SplendidDefaults.Culture(Application));
					string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						// 01/12/2008 Paul.  Preview is different in that it does not filter on queue date. 
						if ( !Sql.IsEmptyGuid(gID) )
						{
							sSQL = "select *                 " + ControlChars.CrLf
							     + "  from vwEMAILMAN_Preview" + ControlChars.CrLf
							     + " where 1 = 1             " + ControlChars.CrLf;
						}
						else
						{
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwEMAILMAN_Send" + ControlChars.CrLf
							     + " where 1 = 1          " + ControlChars.CrLf;
						}
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							// 08/22/2011 Paul.  Prevent any timeouts with campaign emails. 
							cmd.CommandTimeout = 0;
							cmd.CommandText = sSQL;
							if ( !Sql.IsEmptyGuid(gID) )
								Sql.AppendParameter(cmd, gID, "ID", false);
							// 01/12/2008 Paul.  Allow filtering by campaign for the Sent Test. 
							else if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
								Sql.AppendParameter(cmd, gCAMPAIGN_ID, "CAMPAIGN_ID", false);
							else if ( !bSendNow )
							{
								// 04/24/2008 Paul.  Fix date range.  Send all emails that are in the past. 
								Sql.AppendParameter(cmd, DateTime.MinValue, DateTime.Now, "SEND_DATE_TIME");
							}
							// 12/20/2007 Paul.  Set the order so that it is predictable. 
							cmd.CommandText += " order by CAMPAIGN_ID, MARKETING_ID, LIST_ID, EMAIL_TEMPLATE_ID, RECIPIENT_EMAIL";
							if ( !bSendNow && nEmailsPerRun > 0 )
							{
								Sql.LimitResults(cmd, nEmailsPerRun);
							}

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
										SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "Processing " + dt.Rows.Count.ToString() + " emails");
									foreach ( DataRow row in dt.Rows )
									{
										gID = Sql.ToGuid(row["ID"]);
										string sRECIPIENT_NAME    = Sql.ToString(row["RECIPIENT_NAME"   ]);
										string sRECIPIENT_EMAIL   = Sql.ToString(row["RECIPIENT_EMAIL"  ]);
										string sSUBJECT           = Sql.ToString(row["SUBJECT"          ]);
										string sBODY_HTML         = Sql.ToString(row["BODY_HTML"        ]);
										string sRELATED_TYPE      = Sql.ToString(row["RELATED_TYPE"     ]);
										Guid   gRELATED_ID        = Sql.ToGuid  (row["RELATED_ID"       ]);
										string sCAMPAIGN_NAME     = Sql.ToString(row["CAMPAIGN_NAME"    ]);
										       gCAMPAIGN_ID       = Sql.ToGuid  (row["CAMPAIGN_ID"      ]);
										//Guid   gMARKETING_ID      = Sql.ToGuid  (row["MARKETING_ID"     ]);
										//Guid   gLIST_ID           = Sql.ToGuid  (row["LIST_ID"          ]);
										// 12/20/2007 Paul.  We will need the email template to get any attachments. 
										Guid   gEMAIL_TEMPLATE_ID = Sql.ToGuid  (row["EMAIL_TEMPLATE_ID"]);
										string sFROM_ADDR         = Sql.ToString(row["EMAIL_MARKETING_FROM_ADDR"    ]);
										string sFROM_NAME         = Sql.ToString(row["EMAIL_MARKETING_FROM_NAME"    ]);
										string sRETURN_PATH       = Sql.ToString(row["EMAIL_MARKETING_RETURN_PATH"  ]);
										// 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
										string sREPLY_TO_ADDR     = Sql.ToString(row["EMAIL_MARKETING_REPLY_TO_ADDR"]);
										string sREPLY_TO_NAME     = Sql.ToString(row["EMAIL_MARKETING_REPLY_TO_NAME"]);
										// 03/30/2013 Paul.  All campaign emails should be created with the template Assigned User and Team ID. 
										Guid gASSIGNED_USER_ID    = Sql.ToGuid(row["ASSIGNED_USER_ID"   ]);
										Guid gTEAM_ID             = Sql.ToGuid(row["TEAM_ID"            ]);
										Guid gTEAM_SET_ID         = Sql.ToGuid(row["TEAM_SET_ID"        ]);
										// 01/20/2008 Paul.  If the from address is not provided by the email campaign, then use the default settings. 
										if ( Sql.IsEmptyString(sFROM_ADDR) )
										{
											sFROM_ADDR = sFromAddress;
											if ( Sql.IsEmptyString(sFROM_NAME) )
												sFROM_NAME = sFromName;
										}
										// 12/20/2007 Paul.  Try and capture invalid emails. 
										MailMessage mail = new MailMessage();
										try
										{
											// 12/27/2007 Paul.  If the From address is invalid, then that should generate a send error, not an invalid email error. 
											// 01/12/2008 Paul.  Populate ReplyTo and Sender using the same values as From. 
											if ( !Sql.IsEmptyString(sFROM_NAME) )
											{
												mail.From    = new MailAddress(sFROM_ADDR, sFROM_NAME);
												mail.Sender  = new MailAddress(sFROM_ADDR, sFROM_NAME);
												// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
												//mail.ReplyTo = new MailAddress(sFROM_ADDR, sFROM_NAME);
											}
											else
											{
												mail.From    = new MailAddress(sFROM_ADDR);
												mail.Sender  = new MailAddress(sFROM_ADDR);
												// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
												//mail.ReplyTo = new MailAddress(sFROM_ADDR);
											}
											// 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
											if ( !Sql.IsEmptyString(sREPLY_TO_ADDR) && !Sql.IsEmptyString(sREPLY_TO_NAME) )
											{
#if DOTNET4
												mail.ReplyToList.Add(new MailAddress(sREPLY_TO_ADDR, sREPLY_TO_NAME));
#else
												#pragma warning disable 618
												mail.ReplyTo = new MailAddress(sREPLY_TO_ADDR, sREPLY_TO_NAME);
												#pragma warning restore 618
#endif
											}
											else if ( !Sql.IsEmptyString(sREPLY_TO_ADDR) )
											{
#if DOTNET4
												mail.ReplyToList.Add(new MailAddress(sREPLY_TO_ADDR));
#else
												#pragma warning disable 618
												mail.ReplyTo = new MailAddress(sREPLY_TO_ADDR);
												#pragma warning restore 618
#endif
											}
										}
										catch(Exception ex)
										{
											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "send error", true, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											continue;
										}
										try
										{
											// 10/11/2008 Paul.  Move email validation to a common area. 
											if ( !EmailUtils.IsValidEmail(sRECIPIENT_EMAIL) )
											{
												// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
												using ( IDbTransaction trn = Sql.BeginTransaction(con) )
												{
													try
													{
														SqlProcs.spEMAILMAN_SendFailed(gID, "invalid email", true, trn);
														trn.Commit();
													}
													catch(Exception ex)
													{
														trn.Rollback();
														throw(new Exception(ex.Message, ex.InnerException));
													}
												}
												continue;
											}
											if ( sRECIPIENT_NAME != String.Empty )
												mail.To.Add(new MailAddress(sRECIPIENT_EMAIL, sRECIPIENT_NAME));
											else
												mail.To.Add(new MailAddress(sRECIPIENT_EMAIL));
										}
										catch(Exception ex)
										{
											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "invalid email", true, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
											continue;
										}
										try
										{
											DataTable dtRelated = Crm.Modules.Parent(Context.Application, sRELATED_TYPE, gRELATED_ID);
											if ( dtRelated.Rows.Count > 0 )
											{
												// 01/20/2009 Paul.  We need to fill the emails in the correct order, otherwise $AMOUNT_TOTAL_USDOLLAR would not get replaced properly. 
												DataView  vwParentColumns = SortedTableColumns(dtRelated);
												Hashtable hashCurrencyColumns = CurrencyColumns(vwParentColumns);
												// 06/03/2009 Paul.  Allow the lists to be translated. 
												Hashtable hashEnumsColumns = EmailUtils.EnumColumns(Context.Application, sRELATED_TYPE);
												// 12/20/2007 Paul.  FillEmail moved to EmailUtils. 
												sSUBJECT   = EmailUtils.FillEmail(Context.Application, sSUBJECT  , "contact", dtRelated.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
												sBODY_HTML = EmailUtils.FillEmail(Context.Application, sBODY_HTML, "contact", dtRelated.Rows[0], vwParentColumns, hashCurrencyColumns, hashEnumsColumns);
											}

											// 12/20/2007 Paul.  We don't watch to cache the trackers for any period of time, just for the particular campaign run. 
											DataTable dtTrackers = hashTrackers[gCAMPAIGN_ID] as DataTable;
											if ( dtTrackers == null )
											{
												// 06/28/2008 Paul.  The function cannot rely upon the HttpContext to get the application. 
												dtTrackers = CampaignTrackers(Context, gCAMPAIGN_ID);
												hashTrackers.Add(gCAMPAIGN_ID, dtTrackers);
											}
											DataTable dtAttachments = hashAttachments[gEMAIL_TEMPLATE_ID] as DataTable;
											if ( dtAttachments == null )
											{
												// 06/28/2008 Paul.  The function cannot rely upon the HttpContext to get the application. 
												dtAttachments = EmailTemplateAttachments(Context, gEMAIL_TEMPLATE_ID);
												hashAttachments.Add(gEMAIL_TEMPLATE_ID, dtAttachments);
											}

											Guid gTARGET_TRACKER_KEY = Guid.NewGuid();
											// 07/16/2008 Paul.  We need to pass the Application object so that L10n_Term() can be used. 
											sBODY_HTML = EmailUtils.FillTrackers(Context, sBODY_HTML, dtTrackers, sSiteURL, gTARGET_TRACKER_KEY);

											// 07/17/2008 Paul.  Add code to protect and track problems.
											if ( dtAttachments != null )
											{
												if ( dtAttachments.Rows.Count > 0 )
												{
													foreach(DataRow rowAttachment in dtAttachments.Rows)
													{
														string sFILENAME           = Sql.ToString(rowAttachment["FILENAME"          ]);
														string sFILE_MIME_TYPE     = Sql.ToString(rowAttachment["FILE_MIME_TYPE"    ]);
														Guid   gNOTE_ATTACHMENT_ID = Sql.ToGuid  (rowAttachment["NOTE_ATTACHMENT_ID"]);
														
														MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
														if ( mem == null )
														{
															// 07/30/2006 Paul.  We cannot close the streams until the message is sent. 
															mem = new MemoryStream();
															BinaryWriter writer = new BinaryWriter(mem);
															Notes.Attachment.WriteStream(gNOTE_ATTACHMENT_ID, con, writer);
															writer.Flush();
															mem.Seek(0, SeekOrigin.Begin);
															hashNoteStreams.Add(gNOTE_ATTACHMENT_ID, mem);
														}
														// 10/21/2010 Paul.  The first user was getting a valid attachment, but all subsequent users were getting a 64 byte attachment. 
														// The solution was to reset the position as the Attachment object was leaving the stream at the end of the memory range. 
														mem.Seek(0, SeekOrigin.Begin);
														
														Attachment att = new Attachment(mem, sFILENAME, sFILE_MIME_TYPE);
														// 06/02/2014 Tomi.  Make sure to use UTF8 encoding for the name. 
														att.NameEncoding = System.Text.Encoding.UTF8;
														mail.Attachments.Add(att);
													}
												}
											}
											else
											{
												SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "dtAttachments should never be NULL.");
											}

											mail.Subject      = sSUBJECT;
											mail.Body         = sBODY_HTML;
											mail.BodyEncoding = System.Text.Encoding.UTF8;
											mail.IsBodyHtml   = true;
											mail.Headers.Add("X-SplendidCRM-ID", gTARGET_TRACKER_KEY.ToString());
											mail.Headers.Add("X-Mailer", "SplendidCRM");
											if ( !Sql.IsEmptyString(sRETURN_PATH) )
											{
												// 12/21/2007 Paul.  Return-Path may not work with Exchange Server any more.
												mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
												mail.Headers.Add("Return-Path", sRETURN_PATH);
											}
											client.Send(mail);

											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													Guid gEMAIL_ID = Guid.Empty;
													// 01/13/2008 Paul.  The email manager is also being used for AutoReplies, so the campaign might not exist. 
													if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
													{
														// 01/13/2008 Paul.  Since the Plug-in saves body in DESCRIPTION, we need to continue to use it as the primary source of data. 
														// 03/30/2013 Paul.  All campaign emails should be created with the template Assigned User and Team ID. 
														SqlProcs.spEMAILS_CampaignRef(ref gEMAIL_ID, sCAMPAIGN_NAME + ": " + sSUBJECT, sRELATED_TYPE, gRELATED_ID, sBODY_HTML, String.Empty, sFROM_ADDR, sFROM_NAME, sRECIPIENT_NAME + " <" + sRECIPIENT_EMAIL + ">", gRELATED_ID.ToString(), sRECIPIENT_NAME, sRECIPIENT_EMAIL, "campaign", "sent", sRELATED_TYPE, gRELATED_ID, gASSIGNED_USER_ID, gTEAM_ID, gTEAM_SET_ID, trn);
														// 03/30/2013 Paul.  Link attachments to campaign emails. 
														if ( dtAttachments != null )
														{
															if ( dtAttachments.Rows.Count > 0 )
															{
																foreach(DataRow rowAttachment in dtAttachments.Rows)
																{
																	string sFILENAME           = Sql.ToString(rowAttachment["FILENAME"          ]);
																	string sFILE_MIME_TYPE     = Sql.ToString(rowAttachment["FILE_MIME_TYPE"    ]);
																	Guid   gNOTE_ATTACHMENT_ID = Sql.ToGuid  (rowAttachment["NOTE_ATTACHMENT_ID"]);
																	Guid gNOTE_ID = Guid.Empty;
																	SqlProcs.spNOTES_LinkAttachment
																		( ref gNOTE_ID
																		, sAttachmentLabel + ": " + sFILENAME
																		, "Emails"      // PARENT_TYPE
																		, gEMAIL_ID     // PARENT_ID
																		, String.Empty  // DESCRIPTION
																		, gASSIGNED_USER_ID
																		, gTEAM_ID
																		, gTEAM_SET_ID
																		, gNOTE_ATTACHMENT_ID
																		, trn
																		);
																}
															}
														}
													}
													SqlProcs.spEMAILMAN_SendSuccessful(gID, gTARGET_TRACKER_KEY, gEMAIL_ID, trn);
													trn.Commit();
												}
												catch(Exception ex)
												{
													trn.Rollback();
													throw(new Exception(ex.Message, ex.InnerException));
												}
											}
										}
										catch(Exception ex)
										{
											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													SqlProcs.spEMAILMAN_SendFailed(gID, "send error", false, trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
										}
										
										// 12/20/2007 Paul.  We need to protect against caching too much. 
										// If the total cached size is > 100M, then close all streams and clear the cache. 
										long lTotalCachedSize = 0;
										foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
										{
											MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
											if ( mem != null )
												lTotalCachedSize += mem.Length;
										}
										// 12/20/2007 Paul.  In an attempt to be efficient, if we are only caching one big file, then don't flush it. 
										if ( hashNoteStreams.Count > 1 && lTotalCachedSize > 100 * 1024 * 1024 )
										{
											foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
											{
												MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
												if ( mem != null )
													mem.Close();
											}
											hashNoteStreams.Clear();
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					// 12/20/2007 Paul.  We should close the streams manually to help with garbage collection. 
					foreach ( Guid gNOTE_ATTACHMENT_ID in hashNoteStreams.Keys )
					{
						MemoryStream mem = hashNoteStreams[gNOTE_ATTACHMENT_ID] as MemoryStream;
						if ( mem != null )
							mem.Close();
					}
					hashTrackers   .Clear();
					hashAttachments.Clear();
					hashNoteStreams.Clear();
					bInsideSendQueue = false;
				}
			}
		}

		public static void CheckInbound(HttpContext Context, Guid gID, bool bBounce)
		{
			if ( !bInsideCheckInbound )
			{
				bInsideCheckInbound = true;
				try
				{
					bool bEMAIL_INBOUND_SAVE_RAW = Sql.ToBoolean(Context.Application["CONFIG.email_inbound_save_raw"]);
					Guid gINBOUND_EMAIL_KEY      = Sql.ToGuid   (Context.Application["CONFIG.InboundEmailKey"       ]);
					Guid gINBOUND_EMAIL_IV       = Sql.ToGuid   (Context.Application["CONFIG.InboundEmailIV"        ]);
					DataView vwINBOUND_EMAILS_Inbound = null;

					// 02/16/2008 Paul.  InboundEmailMonitored needs to be called from the scheduler, so the application must be provided. 
					if ( bBounce )
						vwINBOUND_EMAILS_Inbound = new DataView(SplendidCache.InboundEmailBounce(Context));
					else
						vwINBOUND_EMAILS_Inbound = new DataView(SplendidCache.InboundEmailMonitored(Context));

					if ( !Sql.IsEmptyGuid(gID) )
						vwINBOUND_EMAILS_Inbound.RowFilter = "ID = '" + gID.ToString() + "'";
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						// 05/20/2009 Paul.  When checking for inbound emails, make sure not to filter by deleted, 
						// otherwise the email could get imported again. 
						sSQL = "select count(*)                " + ControlChars.CrLf
						     + "  from vwEMAILS_Inbound        " + ControlChars.CrLf
						     + " where MESSAGE_ID = @MESSAGE_ID" + ControlChars.CrLf;
						using ( IDbCommand cmdExistingEmails = con.CreateCommand() )
						{
							cmdExistingEmails.CommandText = sSQL;
							// 11/01/2010 Paul.  Increase length of MESSAGE_ID to varchar(851) to allow for IMAP value + login + server. 
							IDbDataParameter parMESSAGE_ID = Sql.AddParameter(cmdExistingEmails, "@MESSAGE_ID", String.Empty, 851);
							foreach ( DataRowView rowInbound in vwINBOUND_EMAILS_Inbound )
							{
								// 01/13/2008 Paul.  The MAILBOX_ID is the ID for the INBOUND_EMAIL record. 
								Guid   gMAILBOX_ID     = Sql.ToGuid   (rowInbound["ID"            ]);
								Guid   gGROUP_ID       = Sql.ToGuid   (rowInbound["GROUP_ID"      ]);
								string sMAILBOX_TYPE   = Sql.ToString (rowInbound["MAILBOX_TYPE"  ]);
								string sSERVER_URL     = Sql.ToString (rowInbound["SERVER_URL"    ]);
								string sEMAIL_USER     = Sql.ToString (rowInbound["EMAIL_USER"    ]);
								string sEMAIL_PASSWORD = Sql.ToString (rowInbound["EMAIL_PASSWORD"]);
								int    nPORT           = Sql.ToInteger(rowInbound["PORT"          ]);
								string sSERVICE        = Sql.ToString (rowInbound["SERVICE"       ]);
								bool   bMAILBOX_SSL    = Sql.ToBoolean(rowInbound["MAILBOX_SSL"   ]);
								bool   bMARK_READ      = Sql.ToBoolean(rowInbound["MARK_READ"     ]);
								bool   bONLY_SINCE     = Sql.ToBoolean(rowInbound["ONLY_SINCE"    ]);
								string sMAILBOX        = Sql.ToString (rowInbound["MAILBOX"       ]);
								// 05/24/2014 Paul.  We need to track the Last Email UID in order to support Only Since flag. 
								int    nLAST_EMAIL_UID = 0;
								int    nNEXT_EMAIL_UID = 0;
								try
								{
									nLAST_EMAIL_UID = Sql.ToInteger(rowInbound["LAST_EMAIL_UID"]);
								}
								catch
								{
								}

								// 01/08/2008 Paul.  Decrypt at the last minute to ensure that an unencrypted password is never sent to the browser. 
								sEMAIL_PASSWORD = Security.DecryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);

								// 07/19/2010 Paul.  We now support the IMAP service. 
								if ( String.Compare(sSERVICE, "imap", true) == 0 )
								{
									using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
									{
										connection.Open();
										ImapCommand command = new ImapCommand(connection);
										using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
										{
											authenticate.Login();
											if ( Sql.IsEmptyString(sMAILBOX) )
												sMAILBOX = "INBOX";
											ImapMailbox mailbox = command.Select(sMAILBOX);
											// 05/24/2014 Paul.  We need to track the Last Email UID in order to support Only Since flag. 
											if ( bONLY_SINCE && nLAST_EMAIL_UID > 0 )
												command.Fetch(mailbox, nLAST_EMAIL_UID, -1);
											else
												command.Fetch(mailbox);
											if ( mailbox != null && mailbox.Messages != null )
											{
												for ( int i = 0; i < mailbox.Messages.Count; i++ )
												{
													ImapMailboxMessage email = mailbox.Messages[i];
													// 01/13/2008 Paul.  Bounce processing only applies if sent by the mailer daemon. 
													// 01/13/2008 Paul.  MS Exchange Server uses postmaster. 
													// 01/29/2008 Paul.  We must convert the address to lower case before comparing. 
													string sFromAddress = String.Empty;
													if ( email.From != null && email.From.Address != null )
														sFromAddress = Sql.ToString(email.From.Address);
													sFromAddress = sFromAddress.ToLower();
													bool bMailerDaemon = (sFromAddress.IndexOf("mailer-daemon@") >= 0 || sFromAddress.IndexOf("postmaster@") >= 0);
													// 05/16/2010 Paul.  We need another way to detect bounced messages. 
													if ( email.Subject != null )
													{
														// 07/03/2012 Paul.  There are a number of other possible failure messages. 
														// This same text is used for POP3. 
														if (  email.Subject.StartsWith("Undeliverable:") 
														   || email.Subject.StartsWith("Undeliverable mail:")
														   || email.Subject.StartsWith("DELIVERY FAILURE:")
														   || email.Subject.StartsWith("Delivery Status Notification (Failure)")
														   || email.Subject.StartsWith("Undeliverable Mail Returned to Sender")
														   || email.Subject.StartsWith("Mail System Error - Returned Mail")
														   || email.Subject.StartsWith("Mail delivery failed")
														   || email.Subject.StartsWith("failure notice")
														   )
															bMailerDaemon = true;
													}
													if ( (bBounce && bMailerDaemon) || (!bBounce && !bMailerDaemon) )
													{
														try
														{
															// 01/12/2008 Paul.  Lookup the message to see if we need to import it. 
															// SugarCRM: The uniqueness of a given email message is determined by a concatenationof 2 values, 
															// SugarCRM: the messageID and the delivered-to field.  This allows multiple To: and B/CC: destination 
															// SugarCRM: addresses to be imported by Sugar without violating the true duplicate-email issues.
															// 01/20/2008 Paul.  mm.DeliveredTo can be NULL. 
															// 07/19/2010 Paul.  Since our Imap library does not provide a DeliveryTo, we are just going to fallback to the mailbox ID. 
															// 10/29/2010 Paul.  Instead of using gMAILBOX_ID, it would make more sense to to use the server and user. 
															string sUNIQUE_MESSAGE_ID = email.MessageID + " " + sSERVER_URL + " " + sEMAIL_USER;
															// 11/01/2010 Paul.  Increase length of MESSAGE_ID to varchar(851) to allow for IMAP value + login + server. 
															if ( sUNIQUE_MESSAGE_ID.Length > 851 )
																sUNIQUE_MESSAGE_ID = sUNIQUE_MESSAGE_ID.Substring(0, 851);
															parMESSAGE_ID.Value = sUNIQUE_MESSAGE_ID;
															if ( Sql.ToInteger(cmdExistingEmails.ExecuteScalar()) == 0 )
															{
																// 10/28/2010 Paul.  IMPORTANT BUG FIX.  We need to call FetchBodyStructure instead of FetchBody 
																// because FetchBody will create a new message and add it to the Messages list, thereby creating an endless loop. 
																command.FetchBodyStructure(email);
																// 01/13/2008 Paul.  Pull POP3 logic out of import function so that it can be reused by IMAP4 driver. 
																// 11/18/2008 Paul.  We must use the passed context as the current context is not available in a scheduled task. 
																ImapUtils.ImportInboundEmail(Context, con, command, email, gMAILBOX_ID, sMAILBOX_TYPE, gGROUP_ID, sUNIQUE_MESSAGE_ID);
															}
															// 05/23/2014 Paul.  The Mark Unread flag was not previously used. 
															if ( !bMARK_READ && !email.Flags.Seen )
															{
																command.SetSeen(email.UID, false);
															}
															// 05/24/2014 Paul.  We need to track the Last Email UID in order to support Only Since flag. 
															nNEXT_EMAIL_UID = email.UID;
														}
														catch(Exception ex)
														{
															// 01/29/2008 Paul.  We want to continue even if one email generates an error, otherwise it would block the rest. 
															SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + "; MessageID = " + email.MessageID);
														}
													}
												}
											}
										}
									}
									// 05/24/2014 Paul.  We need to track the Last Email UID in order to support Only Since flag. 
									if ( bONLY_SINCE && nNEXT_EMAIL_UID > nLAST_EMAIL_UID )
									{
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												SqlProcs.spINBOUND_EMAILS_UpdateLastUID(gMAILBOX_ID, nNEXT_EMAIL_UID, trn);
												trn.Commit();
											}
											catch(Exception ex1)
											{
												trn.Rollback();
												throw(new Exception(ex1.Message, ex1.InnerException));
											}
										}
									}
								}
								else if ( String.Compare(sSERVICE, "pop3", true) == 0 )
								{
									Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
									try
									{
										pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
										pop.Connect();
										
										int nTotalEmails = 0;
										int mailboxSize  = 0;
										pop.GetMailboxStats(out nTotalEmails, out mailboxSize);

										List<int> arrEmailIds = new List<int>();
										pop.GetEmailIdList(out arrEmailIds);
										foreach ( int i in arrEmailIds )
										{
											Pop3.RxMailMessage mm = null;
											try
											{
												pop.IsCollectRawEmail = bEMAIL_INBOUND_SAVE_RAW;
												pop.GetHeaders(i, out mm);
												if ( mm != null )
												{
													// 01/13/2008 Paul.  Bounce processing only applies if sent by the mailer daemon. 
													// 01/13/2008 Paul.  MS Exchange Server uses postmaster. 
													// 01/29/2008 Paul.  We must convert the address to lower case before comparing. 
													string sFromAddress = String.Empty;
													if ( mm.From != null && mm.From.Address != null )
														sFromAddress = Sql.ToString(mm.From.Address);
													sFromAddress = sFromAddress.ToLower();
													bool bMailerDaemon = (sFromAddress.IndexOf("mailer-daemon@") >= 0 || sFromAddress.IndexOf("postmaster@") >= 0);
													// 05/16/2010 Paul.  We need another way to detect bounced messages. 
													if ( !bMailerDaemon && mm.Subject != null )
													{
														// 07/20/2011 Paul.  There are a number of other possible failure messages. 
														if (  mm.Subject.StartsWith("Undeliverable:") 
														   || mm.Subject.StartsWith("Undeliverable mail:")
														   || mm.Subject.StartsWith("DELIVERY FAILURE:")
														   || mm.Subject.StartsWith("Delivery Status Notification (Failure)")
														   || mm.Subject.StartsWith("Undeliverable Mail Returned to Sender")
														   || mm.Subject.StartsWith("Mail System Error - Returned Mail")
														   || mm.Subject.StartsWith("Mail delivery failed")
														   || mm.Subject.StartsWith("failure notice")
														   )
															bMailerDaemon = true;
													}
													if ( (bBounce && bMailerDaemon) || (!bBounce && !bMailerDaemon) )
													{
														try
														{
															// 01/12/2008 Paul.  Lookup the message to see if we need to import it. 
															// SugarCRM: The uniqueness of a given email message is determined by a concatenationof 2 values, 
															// SugarCRM: the messageID and the delivered-to field.  This allows multiple To: and B/CC: destination 
															// SugarCRM: addresses to be imported by Sugar without violating the true duplicate-email issues.
															// 01/20/2008 Paul.  mm.DeliveredTo can be NULL. 
															// 07/19/2010 Paul.  If there is no delivery address, then Sugar just uses the guid of the mailbox. 
															// 09/04/2011 Paul.  In order to prevent duplicate emails, we need to use the unique message ID. 
															string sUNIQUE_MESSAGE_ID = mm.MessageId + ((mm.DeliveredTo != null && mm.DeliveredTo.Address != null) ? mm.DeliveredTo.Address : gMAILBOX_ID.ToString());
															parMESSAGE_ID.Value = sUNIQUE_MESSAGE_ID;
															if ( Sql.ToInteger(cmdExistingEmails.ExecuteScalar()) == 0 )
															{
																mm = null;
																pop.GetEmail(i, out mm);
																// 01/13/2008 Paul.  Pull POP3 logic out of import function so that it can be reused by IMAP4 driver. 
																// 11/18/2008 Paul.  We must use the passed context as the current context is not available in a scheduled task. 
																// 07/19/2010 Paul.  Moved ImportInboundEmail to PopUtils. 
																PopUtils.ImportInboundEmail(Context, con, mm, gMAILBOX_ID, sMAILBOX_TYPE, gGROUP_ID, sUNIQUE_MESSAGE_ID);
															}
															if ( !bMARK_READ )
																pop.DeleteEmail(i);
														}
														catch(Exception ex)
														{
															// 01/29/2008 Paul.  We want to continue even if one email generates an error, otherwise it would block the rest. 
															SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex) + "; MessageID = " + mm.MessageId);
														}
													}
												}
											}
											finally
											{
												// 01/13/2008 Paul.  We may need to be more efficient about garbage cleanup as an email can contain a large attachment. 
												mm = null;
											}
										}
									}
									finally
									{
										pop.Disconnect();
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideCheckInbound = false;
				}
			}
		}

		public static void CheckBounced(HttpContext Context, Guid gID)
		{
			CheckInbound(Context, gID, true);
		}

		public static void CheckMonitored(HttpContext Context, Guid gID)
		{
			CheckInbound(Context, gID, false);
		}

		// 05/15/2008 Paul.  Check for outbound emails. 
		public static void SendOutbound(HttpContext Context)
		{
			if ( !bInsideCheckOutbound )
			{
				bInsideCheckOutbound = true;
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						sSQL = "select ID                    " + ControlChars.CrLf
						     + "  from vwEMAILS_ScheduledSend" + ControlChars.CrLf
						     + " order by DATE_MODIFIED      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								// 05/19/2008 Paul.  We cannot use a DataReader as it will block the creation of a transaction. 
								// There is already an open DataReader associated with this Command which must be closed first.
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										int nEmailsSent = 0;
										Guid gID = Sql.ToGuid(row["ID"]);
										try
										{
											EmailUtils.SendEmail(Context, gID, String.Empty, String.Empty, ref nEmailsSent);
											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													SqlProcs.spEMAILS_UpdateStatus(gID, "sent", trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
										}
										catch(Exception ex)
										{
											// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
											using ( IDbTransaction trn = Sql.BeginTransaction(con) )
											{
												try
												{
													if ( nEmailsSent > 0 )
														SqlProcs.spEMAILS_UpdateStatus(gID, "partial", trn);
													else
														SqlProcs.spEMAILS_UpdateStatus(gID, "send_error", trn);
													trn.Commit();
												}
												catch(Exception ex1)
												{
													trn.Rollback();
													throw(new Exception(ex1.Message, ex1.InnerException));
												}
											}
											SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
										}
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideCheckOutbound = false;
				}
			}
		}

		public static string NormalizeDescription(string sDESCRIPTION)
		{
			// 06/04/2010 Paul.  Try and prevent excess blank lines. 
			sDESCRIPTION = sDESCRIPTION.Replace("\r\n"    , "\n");
			sDESCRIPTION = sDESCRIPTION.Replace("\r"      , "\n");
			sDESCRIPTION = sDESCRIPTION.Replace("<br />\n", "\n");
			sDESCRIPTION = sDESCRIPTION.Replace("<br/>\n" , "\n");
			sDESCRIPTION = sDESCRIPTION.Replace("<br>\n"  , "\n");
			sDESCRIPTION = sDESCRIPTION.Replace("\n"      , "<br />\r\n");
			return sDESCRIPTION;
		}

		public static void OnTimer(Object sender)
		{
			// 04/11/2013 Paul.  If multiple apps connect to the same database, make sure that only one is the job server. 
			// This is primarily for load-balanced sites. 
			HttpContext Context = sender as HttpContext;
			int nSplendidReminderServerFlag = Sql.ToInteger(Context.Application["SplendidReminderServerFlag"]);
			if ( nSplendidReminderServerFlag == 0 )
			{
				string sSplendidReminderServer = System.Configuration.ConfigurationManager.AppSettings["SplendidReminderServer"];
				// 09/17/2009 Paul.  If we are running in Azure, then assume that this is the only instance. 
				string sMachineName = sSplendidReminderServer;
				try
				{
					// 09/17/2009 Paul.  Azure does not support MachineName.  Just ignore the error. 
					sMachineName = System.Environment.MachineName;
				}
				catch
				{
				}
				if ( Sql.IsEmptyString(sSplendidReminderServer) || String.Compare(sMachineName, sSplendidReminderServer, true) == 0 )
				{
					nSplendidReminderServerFlag = 1;
					SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), sMachineName + " is a Splendid Reminder Server.");
				}
				else
				{
					nSplendidReminderServerFlag = -1;
					SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), sMachineName + " is not a Splendid Reminder Server.");
				}
				Context.Application["SplendidReminderServerFlag"] = nSplendidReminderServerFlag;
			}
			if ( nSplendidReminderServerFlag > 0 )
			{
				SendActivityReminders(sender);
				SendSmsActivityReminders(sender);
			}
		}

		// 12/25/2012 Paul.  Use a separate timer for email reminders as they are timely and cannot be stuck behind other scheduler tasks. 
		public static void SendActivityReminders(Object sender)
		{
			HttpContext          Context     = sender as HttpContext;
			HttpApplicationState Application = Context.Application;
			if ( !bInsideActivityReminder && !Utils.IsOfflineClient && Sql.ToBoolean(Application["CONFIG.enable_email_reminders"]) && !Sql.IsEmptyString(Application["CONFIG.smtpserver"]) && !Sql.IsEmptyString(Application["CONFIG.smtpuser"]) && !Sql.IsEmptyString(Application["CONFIG.fromaddress"]) )
			{
				bInsideActivityReminder = true;
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						using ( DataTable dt = new DataTable() )
						{
							List<string> lstINVITEE_TYPE = new List<string>();
							lstINVITEE_TYPE.Add("Users");
							// 01/16/2014 Paul.  Allow reminders to contacts and leads to be disabled. 
							bool enable_email_reminders_contacts = Sql.ToBoolean(Application["CONFIG.enable_email_reminders_contacts"]);
							bool enable_email_reminders_leads    = Sql.ToBoolean(Application["CONFIG.enable_email_reminders_leads"   ]);
							if ( enable_email_reminders_contacts )
								lstINVITEE_TYPE.Add("Contacts");
							if ( enable_email_reminders_leads )
								lstINVITEE_TYPE.Add("Leads");
							sSQL = "select *                          " + ControlChars.CrLf
							     + "  from vwACTIVITIES_EmailReminders" + ControlChars.CrLf
							     + " where 1 = 1                      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, lstINVITEE_TYPE.ToArray(), "INVITEE_TYPE");
								cmd.CommandText += " order by DATE_START" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
								}
							}
							if ( dt.Rows.Count > 0 )
							{
								SmtpClient client           = CreateSmtpClient(Application);
								string     sSiteURL         = Crm.Config.SiteURL(Application);
								string     sFromName        = Sql.ToString(Application["CONFIG.fromname"        ]);
								string     sFromAddress     = Sql.ToString(Application["CONFIG.fromaddress"     ]);
								Guid       gDefaultTimezone = Sql.ToGuid  (Application["CONFIG.default_timezone"]);
								string     sDefaultLanguage = Sql.ToString(Application["CONFIG.default_language"]);
								L10N       L10nEN           = new L10N("en-US");
								DataView   vwColumns        = EmailUtils.SortedTableColumns(dt);
								Hashtable  hashEnumsColumns = EmailUtils.EnumColumns(Application, "Calls");
								foreach ( DataRow row in dt.Rows )
								{
									try
									{
										Guid   gID               = Sql.ToGuid  (row["ID"              ]);
										string sACTIVITY_TYPE    = Sql.ToString(row["ACTIVITY_TYPE"   ]);
										Guid   gINVITEE_ID       = Sql.ToGuid  (row["INVITEE_ID"      ]);
										string sINVITEE_TYPE     = Sql.ToString(row["INVITEE_TYPE"    ]);
										string sINVITEE_NAME     =(Sql.ToString(row["FIRST_NAME"      ]) + " " + Sql.ToString(row["LAST_NAME"])).Trim();
										string sINVITEE_EMAIL1   = Sql.ToString(row["EMAIL1"          ]);
										string sINVITEE_LANG     = Sql.ToString(row["LANG"            ]);
										Guid   gTIMEZONE_ID      = Sql.ToGuid  (row["TIMEZONE_ID"     ]);
										Guid   gASSIGNED_USER_ID = Sql.ToGuid  (row["ASSIGNED_USER_ID"]);
										string sASSIGNED_TO      = Sql.ToString(row["ASSIGNED_TO"     ]);
										if ( !Sql.IsEmptyGuid(gTIMEZONE_ID) )
											gTIMEZONE_ID = gDefaultTimezone;
										if ( !Sql.IsEmptyString(sINVITEE_LANG) )
											sINVITEE_LANG = sDefaultLanguage;
										TimeZone T10n = TimeZone.CreateTimeZone(Application, gTIMEZONE_ID);
										row["DATE_START"] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_START"]));
										row["DATE_END"  ] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_END"  ]));
										
										// 12/25/2012 Paul.  The reminder mssages are pulled from the terminology table so that they can be localized. 
										string sSubjectMsg = (sINVITEE_TYPE == "Users" ? "MSG_USER_REMINDER_SUBJECT" : "MSG_CONTACT_REMINDER_SUBJECT");
										string sBodyMsg    = (sINVITEE_TYPE == "Users" ? "MSG_USER_REMINDER_BODY"    : "MSG_CONTACT_REMINDER_BODY"   );
										string sSubject    = L10N.Term(Application, sINVITEE_LANG, sACTIVITY_TYPE + "." + sSubjectMsg);
										string sBodyHtml   = L10N.Term(Application, sINVITEE_LANG, sACTIVITY_TYPE + "." + sBodyMsg   );
										// 12/25/2012 Paul.  First fallback is English. 
										if ( Sql.IsEmptyString(sSubject) )
											sSubject    = L10nEN.Term(sACTIVITY_TYPE + "." + sSubjectMsg);
										if ( Sql.IsEmptyString(sBodyHtml) )
											sBodyHtml   = L10nEN.Term(sACTIVITY_TYPE + "." + sBodyMsg   );
										// 12/25/2012 Paul.  Second fallback is embedded string. 
										if ( Sql.IsEmptyString(sSubject) )
											sSubject = sACTIVITY_TYPE + " Reminder - $activity_name";
										if ( Sql.IsEmptyString(sBodyHtml) )
											sBodyHtml = "$activity_name\n$activity_date_start\n" + (sINVITEE_TYPE == "Users" ? "\n<a href=\"$view_url\">$view_url</a>" : String.Empty);
										
										string sViewURL    = sSiteURL + sACTIVITY_TYPE + "/view.aspx?ID=" + gID.ToString();
										string sEditURL    = sSiteURL + sACTIVITY_TYPE + "/edit.aspx?ID=" + gID.ToString();
										sBodyHtml = sBodyHtml.Replace("$view_url", sViewURL);
										sBodyHtml = sBodyHtml.Replace("$edit_url", sEditURL);
										sBodyHtml = sBodyHtml.Replace("href=\"~/", "href=\"" + sSiteURL);
										sBodyHtml = sBodyHtml.Replace("href=\'~/", "href=\'" + sSiteURL);  // 12/25/2012 Paul.  Also watch for single quote. 
										
										string sFillPrefix = sACTIVITY_TYPE;
										if ( sFillPrefix.EndsWith("s") )
											sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-1);
										sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
										sBodyHtml = EmailUtils.FillEmail(Application, sBodyHtml, sFillPrefix, row, vwColumns, null, hashEnumsColumns);
										if ( sBodyHtml.Contains("$activity_") )
										{
											sFillPrefix = "activity";
											sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
											sBodyHtml = EmailUtils.FillEmail(Application, sBodyHtml, sFillPrefix, row, vwColumns, null, hashEnumsColumns);
										}
										
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												if ( sACTIVITY_TYPE == "Meetings" )
													SqlProcs.spMEETINGS_EmailReminderSent(gID, sINVITEE_TYPE, gINVITEE_ID, trn);
												else if ( sACTIVITY_TYPE == "Calls" )
													SqlProcs.spCALLS_EmailReminderSent(gID, sINVITEE_TYPE, gINVITEE_ID, trn);
												trn.Commit();
											}
											catch(Exception ex)
											{
												trn.Rollback();
												throw(new Exception(ex.Message, ex.InnerException));
											}
										}
										
										MailMessage mail = new MailMessage();
										try
										{
											if ( !Sql.IsEmptyString(sFromAddress) && !Sql.IsEmptyString(sFromName) )
												mail.From = new MailAddress(sFromAddress, sFromName);
											else
												mail.From = new MailAddress(sFromAddress);
											MailAddress addr = new MailAddress(sINVITEE_EMAIL1, sINVITEE_NAME);
											mail.To.Add(addr);
											
											mail.Subject      = sSubject ;
											mail.Body         = sBodyHtml;
											mail.IsBodyHtml   = true;
											mail.BodyEncoding = System.Text.Encoding.UTF8;
											
											/*
											// 12/27/2012 Paul.  The email reminder should not include the ICS file as it would create a second event in Outlook. 
											string sICS = Utils.GenerateVCalendar(row, false);
											string sAttachmentName = sSubject.Trim() + ".ics";
											sAttachmentName = sAttachmentName.Replace('\\', '_');
											sAttachmentName = sAttachmentName.Replace(':' , '_');
											MemoryStream mem = new MemoryStream(UTF8Encoding.Default.GetBytes(sICS));
											Attachment att = new Attachment(mem, sAttachmentName, "text/calendar");
											// 06/02/2014 Tomi.  Make sure to use UTF8 encoding for the name. 
											att.NameEncoding = System.Text.Encoding.UTF8;
											mail.Attachments.Add(att);
											*/
											
											client.Send(mail);
										}
										finally
										{
											// 12/27/2012 Paul.  Close the streams after the message is sent. 
											foreach ( Attachment att in mail.Attachments )
											{
												if ( att.ContentStream != null )
													att.ContentStream.Close();
											}
										}
									}
									catch(Exception ex)
									{
										SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideActivityReminder = false;
				}
			}
		}

		// 12/23/2013 Paul.  Add SMS_REMINDER_TIME. 
		public static void SendSmsActivityReminders(Object sender)
		{
			HttpContext          Context     = sender as HttpContext;
			HttpApplicationState Application = Context.Application;
			if ( !bInsideSmsActivityReminder && !Utils.IsOfflineClient && Sql.ToBoolean(Application["CONFIG.enable_sms_reminders"]) && !Sql.IsEmptyString(Application["CONFIG.Twilio.AccountSID"]) && !Sql.IsEmptyString(Application["CONFIG.Twilio.AuthToken"]) && !Sql.IsEmptyString(Application["CONFIG.Twilio.FromPhone"]) )
			{
				bInsideSmsActivityReminder = true;
				try
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL;
						using ( DataTable dt = new DataTable() )
						{
							List<string> lstINVITEE_TYPE = new List<string>();
							lstINVITEE_TYPE.Add("Users");
							// 01/16/2014 Paul.  Allow reminders to contacts and leads to be disabled. 
							bool enable_sms_reminders_contacts = Sql.ToBoolean(Application["CONFIG.enable_sms_reminders_contacts"]);
							bool enable_sms_reminders_leads    = Sql.ToBoolean(Application["CONFIG.enable_sms_reminders_leads"   ]);
							if ( enable_sms_reminders_contacts )
								lstINVITEE_TYPE.Add("Contacts");
							if ( enable_sms_reminders_leads )
								lstINVITEE_TYPE.Add("Leads");
							sSQL = "select *                          " + ControlChars.CrLf
							     + "  from vwACTIVITIES_SmsReminders  " + ControlChars.CrLf
							     + " where 1 = 1                      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, lstINVITEE_TYPE.ToArray(), "INVITEE_TYPE");
								cmd.CommandText += " order by DATE_START" + ControlChars.CrLf;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
								}
							}
							if ( dt.Rows.Count > 0 )
							{
								string     sFromNumber      = Sql.ToString(Application["CONFIG.Twilio.FromPhone"]);
								Guid       gDefaultTimezone = Sql.ToGuid  (Application["CONFIG.default_timezone"]);
								string     sDefaultLanguage = Sql.ToString(Application["CONFIG.default_language"]);
								L10N       L10nEN           = new L10N("en-US");
								DataView   vwColumns        = EmailUtils.SortedTableColumns(dt);
								Hashtable  hashEnumsColumns = EmailUtils.EnumColumns(Application, "Calls");
								foreach ( DataRow row in dt.Rows )
								{
									try
									{
										Guid   gID               = Sql.ToGuid  (row["ID"              ]);
										string sACTIVITY_TYPE    = Sql.ToString(row["ACTIVITY_TYPE"   ]);
										Guid   gINVITEE_ID       = Sql.ToGuid  (row["INVITEE_ID"      ]);
										string sINVITEE_TYPE     = Sql.ToString(row["INVITEE_TYPE"    ]);
										string sINVITEE_PHONE    = Sql.ToString(row["PHONE_MOBILE"    ]);
										string sINVITEE_LANG     = Sql.ToString(row["LANG"            ]);
										Guid   gTIMEZONE_ID      = Sql.ToGuid  (row["TIMEZONE_ID"     ]);
										Guid   gASSIGNED_USER_ID = Sql.ToGuid  (row["ASSIGNED_USER_ID"]);
										string sASSIGNED_TO      = Sql.ToString(row["ASSIGNED_TO"     ]);
										if ( !Sql.IsEmptyGuid(gTIMEZONE_ID) )
											gTIMEZONE_ID = gDefaultTimezone;
										if ( !Sql.IsEmptyString(sINVITEE_LANG) )
											sINVITEE_LANG = sDefaultLanguage;
										TimeZone T10n = TimeZone.CreateTimeZone(Application, gTIMEZONE_ID);
										row["DATE_START"] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_START"]));
										row["DATE_END"  ] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_END"  ]));
										
										// 12/23/2013 Paul.  The reminder mssages are pulled from the terminology table so that they can be localized. 
										string sSubjectMsg = (sINVITEE_TYPE == "Users" ? "SMS_USER_REMINDER_SUBJECT" : "SMS_CONTACT_REMINDER_SUBJECT");
										string sSubject    = L10N.Term(Application, sINVITEE_LANG, sACTIVITY_TYPE + "." + sSubjectMsg);
										// 12/23/2013 Paul.  First fallback is English. 
										if ( Sql.IsEmptyString(sSubject) )
											sSubject    = L10nEN.Term(sACTIVITY_TYPE + "." + sSubjectMsg);
										// 12/23/2013 Paul.  Second fallback is embedded string. 
										if ( Sql.IsEmptyString(sSubject) )
											sSubject = sACTIVITY_TYPE + " Reminder - $activity_name";
										
										string sFillPrefix = sACTIVITY_TYPE;
										if ( sFillPrefix.EndsWith("s") )
											sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-1);
										sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
										if ( sSubject.Contains("$activity_") )
										{
											sFillPrefix = "activity";
											sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
										}
										
										using ( IDbTransaction trn = Sql.BeginTransaction(con) )
										{
											try
											{
												if ( sACTIVITY_TYPE == "Meetings" )
													SqlProcs.spMEETINGS_SmsReminderSent(gID, sINVITEE_TYPE, gINVITEE_ID, trn);
												else if ( sACTIVITY_TYPE == "Calls" )
													SqlProcs.spCALLS_SmsReminderSent(gID, sINVITEE_TYPE, gINVITEE_ID, trn);
												trn.Commit();
											}
											catch(Exception ex)
											{
												trn.Rollback();
												throw(new Exception(ex.Message, ex.InnerException));
											}
										}
										
										TwilioManager.SendText(Context.Application, sFromNumber, sINVITEE_PHONE, sSubject);
									}
									catch(Exception ex)
									{
										SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					bInsideSmsActivityReminder = false;
				}
			}
		}

		public static void SendActivityInvites(Guid gID)
		{
			HttpContext Context = HttpContext.Current;
			HttpApplicationState Application = Context.Application;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				using ( DataTable dt = new DataTable() )
				{
					sSQL = "select *                       " + ControlChars.CrLf
					     + "  from vwACTIVITIES_Invitees   " + ControlChars.CrLf
					     + " where ID = @ID                " + ControlChars.CrLf
					     + " order by EMAIL1               " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ID", gID);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
					if ( dt.Rows.Count > 0 )
					{
						SmtpClient client           = CreateSmtpClient(Application);
						string     sSiteURL         = Crm.Config.SiteURL(Application);
						bool       bSendFromUser    = Sql.ToBoolean(Application["notify_send_from_assigning_user"]);
						string     sFromName        = Sql.ToString (Application["CONFIG.fromname"                ]);
						string     sFromAddress     = Sql.ToString (Application["CONFIG.fromaddress"             ]);
						Guid       gDefaultTimezone = Sql.ToGuid   (Application["CONFIG.default_timezone"        ]);
						string     sDefaultLanguage = Sql.ToString (Application["CONFIG.default_language"        ]);
						L10N       L10nEN           = new L10N("en-US");
						DataView   vwColumns        = EmailUtils.SortedTableColumns(dt);
						Hashtable  hashEnumsColumns = EmailUtils.EnumColumns(Application, "Calls");
						foreach ( DataRow row in dt.Rows )
						{
							try
							{
								string sACTIVITY_TYPE      = Sql.ToString(row["ACTIVITY_TYPE"     ]);
								Guid   gINVITEE_ID         = Sql.ToGuid  (row["INVITEE_ID"        ]);
								string sINVITEE_TYPE       = Sql.ToString(row["INVITEE_TYPE"      ]);
								string sINVITEE_NAME       =(Sql.ToString(row["FIRST_NAME"        ]) + " " + Sql.ToString(row["LAST_NAME"])).Trim();
								string sINVITEE_EMAIL1     = Sql.ToString(row["EMAIL1"            ]);
								string sINVITEE_LANG       = Sql.ToString(row["LANG"              ]);
								Guid   gTIMEZONE_ID        = Sql.ToGuid  (row["TIMEZONE_ID"       ]);
								Guid   gASSIGNED_USER_ID   = Sql.ToGuid  (row["ASSIGNED_USER_ID"  ]);
								string sASSIGNED_TO        = Sql.ToString(row["ASSIGNED_TO"       ]);
								string sASSIGNED_TO_NAME   = Sql.ToString(row["ASSIGNED_TO_NAME"  ]);
								string sASSIGNED_TO_EMAIL1 = Sql.ToString(row["ASSIGNED_TO_EMAIL1"]);
								if ( !Sql.IsEmptyGuid(gTIMEZONE_ID) )
									gTIMEZONE_ID = gDefaultTimezone;
								if ( !Sql.IsEmptyString(sINVITEE_LANG) )
									sINVITEE_LANG = sDefaultLanguage;
								TimeZone T10n = TimeZone.CreateTimeZone(Application, gTIMEZONE_ID);
								row["DATE_START"] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_START"]));
								row["DATE_END"  ] = T10n.FromServerTime(Sql.ToDateTime(row["DATE_END"  ]));
								
								// 12/25/2012 Paul.  The invitee mssages are pulled from the terminology table so that they can be localized. 
								string sSubjectMsg = (sINVITEE_TYPE == "Users" ? "MSG_USER_INVITEE_SUBJECT" : "MSG_CONTACT_INVITEE_SUBJECT");
								string sBodyMsg    = (sINVITEE_TYPE == "Users" ? "MSG_USER_INVITEE_BODY"    : "MSG_CONTACT_INVITEE_BODY"   );
								string sSubject    = L10N.Term(Application, sINVITEE_LANG, sACTIVITY_TYPE + "." + sSubjectMsg);
								string sBodyHtml   = L10N.Term(Application, sINVITEE_LANG, sACTIVITY_TYPE + "." + sBodyMsg   );
								// 12/25/2012 Paul.  First fallback is English. 
								if ( Sql.IsEmptyString(sSubject) )
									sSubject    = L10nEN.Term(sACTIVITY_TYPE + "." + sSubjectMsg);
								if ( Sql.IsEmptyString(sBodyHtml) )
									sBodyHtml   = L10nEN.Term(sACTIVITY_TYPE + "." + sBodyMsg   );
								// 12/25/2012 Paul.  Second fallback is embedded string. 
								if ( Sql.IsEmptyString(sSubject) )
									sSubject = sACTIVITY_TYPE + " Invitation - $activity_name";
								if ( Sql.IsEmptyString(sBodyHtml) )
									sBodyHtml = "$activity_name\n$activity_date_start\n" + (sINVITEE_TYPE == "Users" ? "\n<a href=\"$view_url\">$view_url</a>" : String.Empty);
								
								string sViewURL    = sSiteURL + sACTIVITY_TYPE + "/view.aspx?ID="          + gID.ToString();
								string sEditURL    = sSiteURL + sACTIVITY_TYPE + "/edit.aspx?ID="          + gID.ToString();
								string sAcceptURL  = sSiteURL + sACTIVITY_TYPE + "/AcceptDecline.aspx?ID=" + gID.ToString() + "&INVITEE_ID=" + gINVITEE_ID.ToString();
								sBodyHtml = sBodyHtml.Replace("$view_url"  , sViewURL  );
								sBodyHtml = sBodyHtml.Replace("$edit_url"  , sEditURL  );
								sBodyHtml = sBodyHtml.Replace("$accept_url", sAcceptURL);
								sBodyHtml = sBodyHtml.Replace("href=\"~/", "href=\"" + sSiteURL);
								sBodyHtml = sBodyHtml.Replace("href=\'~/", "href=\'" + sSiteURL);  // 12/25/2012 Paul.  Also watch for single quote. 
								
								string sFillPrefix = sACTIVITY_TYPE;
								if ( sFillPrefix.EndsWith("s") )
									sFillPrefix = sFillPrefix.Substring(0, sFillPrefix.Length-1);
								sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
								sBodyHtml = EmailUtils.FillEmail(Application, sBodyHtml, sFillPrefix, row, vwColumns, null, hashEnumsColumns);
								if ( sBodyHtml.Contains("$activity_") )
								{
									sFillPrefix = "activity";
									sSubject  = EmailUtils.FillEmail(Application, sSubject , sFillPrefix, row, vwColumns, null, hashEnumsColumns);
									sBodyHtml = EmailUtils.FillEmail(Application, sBodyHtml, sFillPrefix, row, vwColumns, null, hashEnumsColumns);
								}
								
								MailMessage mail = new MailMessage();
								try
								{
									if ( bSendFromUser && !Sql.IsEmptyString(sASSIGNED_TO_EMAIL1) && !Sql.IsEmptyString(sASSIGNED_TO_NAME) )
										mail.From = new MailAddress(sASSIGNED_TO_EMAIL1, sASSIGNED_TO_NAME);
									else if ( bSendFromUser && !Sql.IsEmptyString(sASSIGNED_TO_EMAIL1) )
										mail.From = new MailAddress(sASSIGNED_TO_EMAIL1);
									else if ( !Sql.IsEmptyString(sFromAddress) && !Sql.IsEmptyString(sFromName) )
										mail.From = new MailAddress(sFromAddress, sFromName);
									else
										mail.From = new MailAddress(sFromAddress);
									MailAddress addr = new MailAddress(sINVITEE_EMAIL1, sINVITEE_NAME);
									mail.To.Add(addr);
								
									mail.Subject      = sSubject ;
									mail.Body         = sBodyHtml;
									mail.IsBodyHtml   = true;
									mail.BodyEncoding = System.Text.Encoding.UTF8;
									
									string sICS = Utils.GenerateVCalendar(row, true);
									string sAttachmentName = sSubject.Trim() + ".ics";
									sAttachmentName = sAttachmentName.Replace('\\', '_');
									sAttachmentName = sAttachmentName.Replace(':' , '_');
									MemoryStream mem = new MemoryStream(UTF8Encoding.Default.GetBytes(sICS));
									Attachment att = new Attachment(mem, sAttachmentName, "text/calendar");
									// 06/02/2014 Tomi.  Make sure to use UTF8 encoding for the name. 
									att.NameEncoding = System.Text.Encoding.UTF8;
									mail.Attachments.Add(att);
									
									client.Send(mail);
								}
								finally
								{
									// 12/27/2012 Paul.  Close the streams after the message is sent. 
									foreach ( Attachment att in mail.Attachments )
									{
										if ( att.ContentStream != null )
											att.ContentStream.Close();
									}
								}
							}
							catch(Exception ex)
							{
								SplendidError.SystemMessage("Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
							}
						}
					}
				}
			}
		}
	}
}


