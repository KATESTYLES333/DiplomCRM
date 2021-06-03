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
using System.Net;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Globalization;
using System.Threading;

namespace SplendidCRM._devtools
{
	#region AdmAuthentication
	// http://msdn.microsoft.com/en-us/library/hh454950.aspx
	[DataContract]
	public class AdmAccessToken
	{
		[DataMember]
		public string access_token { get; set; }
		[DataMember]
		public string token_type { get; set; }
		[DataMember]
		public string expires_in { get; set; }
		[DataMember]
		public string scope { get; set; }
	}

	public class AdmAuthentication
	{
		public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
		private string clientId;
		private string clientSecret;
		private string request;
		private AdmAccessToken token;
		private Timer accessTokenRenewer;

		//Access token expires every 10 minutes. Renew it every 9 minutes only.
		private const int RefreshTokenDuration = 9;

		public AdmAuthentication(string clientId, string clientSecret)
		{
			this.clientId = clientId;
			this.clientSecret = clientSecret;
			//If clientid or client secret has special characters, encode before sending request
			this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
			this.token = HttpPost(DatamarketAccessUri, this.request);
			//renew the token every specfied minutes
			accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback), this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
		}

		public AdmAccessToken GetAccessToken()
		{
			return this.token;
		}

		private void RenewAccessToken()
		{
			AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
			//swap the new token with old one
			//Note: the swap is thread unsafe
			this.token = newAccessToken;
			Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this.clientId, this.token.access_token));
		}

		private void OnTokenExpiredCallback(object stateInfo)
		{
			try
			{
				RenewAccessToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
			}
			finally
			{
				try
				{
					accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
				}
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
				}
			}
		}

		private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
		{
			//Prepare OAuth request 
			WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;
			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}
			using (WebResponse webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream
				AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
				return token;
			}
		}
	}
	#endregion

	/// <summary>
	/// Summary description for Translate.
	/// </summary>
	public class Translate : System.Web.UI.Page
	{
		protected DataTable dtMain;
		protected string    sLang ;

		void Page_Load(object sender, System.EventArgs e)
		{
			dtMain = new DataTable();
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;

			sLang = Sql.ToString(Request["Lang"]);
			if ( sLang == "en-US" )
				return;
			if ( !Sql.IsEmptyString(sLang) )
			{
				AdmAccessToken admToken = null;
				// Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
				// Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx)
				string sTranslatorClientID     = Sql.ToString(Application["CONFIG.MicrosoftTranslator.ClientID"    ]);
				string sTranslatorClientSecret = Sql.ToString(Application["CONFIG.MicrosoftTranslator.ClientSecret"]);
				if ( Sql.IsEmptyString(sTranslatorClientID) || Sql.IsEmptyString(sTranslatorClientSecret) )
				{
					Response.Write("<font color=red>Missing Microsoft Translator configuration settings.</font><br />"+ ControlChars.CrLf);
					return;
				}
				AdmAuthentication admAuth = new AdmAuthentication(sTranslatorClientID, sTranslatorClientSecret);
				try
				{
					admToken = admAuth.GetAccessToken();
				}
				catch (WebException ex)
				{
					string sResponse = string.Empty;
					using ( HttpWebResponse response = (HttpWebResponse)ex.Response )
					{
						using ( Stream stm = response.GetResponseStream() )
						{
							using ( StreamReader rdr = new StreamReader(stm, System.Text.Encoding.ASCII) )
							{
								sResponse = rdr.ReadToEnd();
							}
						}
					}
					Response.Write("<font color=red>Http status code=" + ex.Status + ", error message=" + sResponse + "</font><br />"+ ControlChars.CrLf);
					return;
				}
				catch (Exception ex)
				{
					Response.Write("<font color=red>" + ex.Message + "</font><br />"+ ControlChars.CrLf);
					return;
				}
				System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
				
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					// 05/24/2008 Paul.  Use an outer join so that we only translate missing terms. 
					sSQL = "select ENGLISH.NAME                                     " + ControlChars.CrLf
					     + "     , ENGLISH.MODULE_NAME                              " + ControlChars.CrLf
					     + "     , ENGLISH.LIST_NAME                                " + ControlChars.CrLf
					     + "     , ENGLISH.LIST_ORDER                               " + ControlChars.CrLf
					     + "     , ENGLISH.DISPLAY_NAME                             " + ControlChars.CrLf
					     + "  from            vwTERMINOLOGY             ENGLISH     " + ControlChars.CrLf
					     + "  left outer join vwTERMINOLOGY             TRANSLATED  " + ControlChars.CrLf
					     + "               on TRANSLATED.NAME         = ENGLISH.NAME" + ControlChars.CrLf
					     + "              and lower(TRANSLATED.LANG) = lower(@LANG) " + ControlChars.CrLf
					     + "              and (TRANSLATED.MODULE_NAME = ENGLISH.MODULE_NAME or TRANSLATED.MODULE_NAME is null and ENGLISH.MODULE_NAME is null)" + ControlChars.CrLf
					     + "              and (TRANSLATED.LIST_NAME   = ENGLISH.LIST_NAME   or TRANSLATED.LIST_NAME   is null and ENGLISH.LIST_NAME   is null)" + ControlChars.CrLf
					     + " where lower(ENGLISH.LANG) = lower('en-US')             " + ControlChars.CrLf
					     + "   and TRANSLATED.ID is null                            " + ControlChars.CrLf
					     + " order by ENGLISH.MODULE_NAME, ENGLISH.LIST_NAME, ENGLISH.LIST_ORDER, ENGLISH.NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 03/06/2006 Paul.  Oracle is case sensitive, and we modify the case of L10n.NAME to be lower. 
						Sql.AddParameter(cmd, "@LANG", sLang);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dtMain);
							}
						}
					}
				}
				CultureInfo culture = new CultureInfo(sLang);
				if ( culture == null )
					throw(new Exception("Unknown language: " + sLang));
				SqlProcs.spLANGUAGES_InsertOnly(sLang, culture.LCID, true, culture.NativeName, culture.DisplayName);

				int nErrors = 0;
				JavaScriptSerializer json = new JavaScriptSerializer();
				// 05/18/2008 Paul.  Increase timeout to support slower machines. 
				Server.ScriptTimeout = 600;
				Response.Buffer = false;
				for ( int i = 0 ; i < dtMain.Rows.Count && Response.IsClientConnected && nErrors < 20; i++ )
				{
					DataRow row = dtMain.Rows[i];
					string sNAME         = Sql.ToString (row["NAME"        ]);
					string sMODULE_NAME  = Sql.ToString (row["MODULE_NAME" ]);
					string sLIST_NAME    = Sql.ToString (row["LIST_NAME"   ]);
					Int32  nLIST_ORDER   = Sql.ToInteger(row["LIST_ORDER"  ]);
					string sDISPLAY_NAME = Sql.ToString (row["DISPLAY_NAME"]);
					string sLANG         = sLang;
					// 02/02/2009 Paul.  Some languages use 10 characters. 
					if ( sLANG.Length == 5 )
						sLANG = sLang.Substring(0, 2).ToLower() + "-" + sLang.Substring(3, 2).ToUpper();

					try
					{
						// 05/18/2008 Paul.  No need to translate empty strings or single characters. 
						if ( sDISPLAY_NAME.Length > 1 )
						{
							string sToLang = sLang.Split('-')[0];
							// 05/18/2008 Paul.  The only time we use the 5-character code is when requesting Chinese. 
							// 08/01/2013 Paul.  Microsoft Translator uses zh-CHS for Chinese (Simplified) and zh-CHT for Chinese (Traditional). 
							if ( sLANG == "zh-CN" )
								sToLang = "zh-CHS";
							else if ( sLANG == "zh-TW" )
								sToLang = "zh-CHT";
							// 05/18/2008 Paul.  Not sure why Google uses NO. 
							// 08/01/2013 Paul.  Microsoft Translator also uses NO. 
							if ( sLANG == "nb-NO" || sLANG == "nn-NO" )
								sToLang = "no";
							//if ( sLANG == "fil-PH" )
							//	sToLang = "tl";
							string sURL = "http://api.microsofttranslator.com/v2/Http.svc/Translate?contentType=" + HttpUtility.UrlEncode("text/plain") + "&text=" + HttpUtility.UrlEncode(sDISPLAY_NAME) + "&from=en" + "&to=" + sToLang;
							HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(sURL);
							objRequest.Headers.Add("Authorization", "Bearer " + admToken.access_token);
							objRequest.KeepAlive         = false;
							objRequest.AllowAutoRedirect = false;
							objRequest.Timeout           = 15000;  //15 seconds
							objRequest.Method            = "GET";

							// 01/11/2011 Paul.  Make sure to dispose of the response object as soon as possible. 
							using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
							{
								if ( objResponse != null )
								{
									if ( objResponse.StatusCode == HttpStatusCode.OK || objResponse.StatusCode == HttpStatusCode.Found )
									{
										using ( Stream stream = objResponse.GetResponseStream() )
										{
											// <string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">translated text</string>
											string sTranslation = (string) dcs.ReadObject(stream);
											Response.Write(sTranslation + "<br />"+ ControlChars.CrLf);
											sDISPLAY_NAME = sTranslation;
											SqlProcs.spTERMINOLOGY_InsertOnly(sNAME, sLANG, sMODULE_NAME, sLIST_NAME, nLIST_ORDER, sDISPLAY_NAME);
										}
									}
									else
									{
										nErrors++;
										Response.Write("<font color=red>" + objResponse.StatusCode + " " + objResponse.StatusDescription + " (" + sMODULE_NAME + "." + sNAME + ")" + "</font><br />"+ ControlChars.CrLf);
									}
								}
							}
						}
						else
						{
							SqlProcs.spTERMINOLOGY_InsertOnly(sNAME, sLANG, sMODULE_NAME, sLIST_NAME, nLIST_ORDER, sDISPLAY_NAME);
						}
					}
					catch(Exception ex)
					{
						nErrors++;
						Response.Write("<font color=red>" + ex.Message + "</font><br />"+ ControlChars.CrLf);
					}
				}
				dtMain = new DataTable();
				// 05/18/2008 Paul.  Cannot redirect when response buffering is off. 
				//Response.Redirect("Terminology.aspx");
				if ( nErrors == 0 )
					Response.Write("<script type=\"text/javascript\">window.location.href='Terminology.aspx';</script>");
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}

