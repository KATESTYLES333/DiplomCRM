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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

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

	// 07/20/2017 Paul.  New method of accessing translator. 
	// https://www.microsoft.com/en-us/translator/getstarted.aspx
	// https://github.com/MicrosoftTranslator/HTTP-Code-Samples
	#region AzureAuthToken
	/// <summary>
	/// Client to call Cognitive Services Azure Auth Token service in order to get an access token.
	/// Exposes asynchronous as well as synchronous methods.
	/// </summary>
	public class AzureAuthToken
	{
		/// URL of the token service
		private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");

		/// Name of header used to pass the subscription key to the token service
		private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

		/// After obtaining a valid token, this class will cache it for this duration.
		/// Use a duration of 5 minutes, which is less than the actual token lifetime of 10 minutes.
		private static readonly TimeSpan TokenCacheDuration = new TimeSpan(0, 5, 0);

		/// Cache the value of the last valid token obtained from the token service.
		private string _storedTokenValue = string.Empty;

		/// When the last valid token was obtained.
		private DateTime _storedTokenTime = DateTime.MinValue;

		/// Gets the subscription key.
		public string SubscriptionKey { get; set; }

		/// Gets the HTTP status code for the most recent request to the token service.
		public HttpStatusCode RequestStatusCode { get; private set; }

		/// <summary>
		/// Creates a client to obtain an access token.
		/// </summary>
		/// <param name="key">Subscription key to use to get an authentication token.</param>
		public AzureAuthToken(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(key, "A subscription key is required");
			}

			this.SubscriptionKey = key;
			this.RequestStatusCode = HttpStatusCode.InternalServerError;
		}

		/// <summary>
		/// Gets a token for the specified subscription.
		/// </summary>
		/// <returns>The encoded JWT token prefixed with the string "Bearer ".</returns>
		/// <remarks>
		/// This method uses a cache to limit the number of request to the token service.
		/// A fresh token can be re-used during its lifetime of 10 minutes. After a successful
		/// request to the token service, this method caches the access token. Subsequent 
		/// invocations of the method return the cached token for the next 5 minutes. After
		/// 5 minutes, a new token is fetched from the token service and the cache is updated.
		/// </remarks>
		public async Task<string> GetAccessTokenAsync()
		{
			if (string.IsNullOrWhiteSpace(this.SubscriptionKey))
			{
				return string.Empty;
			}

			// Re-use the cached token if there is one.
			if ((DateTime.Now - _storedTokenTime) < TokenCacheDuration)
			{
				return _storedTokenValue;
			}

			using (var client = new HttpClient())
			using (var request = new HttpRequestMessage())
			{
				request.Method = HttpMethod.Post;
				request.RequestUri = ServiceUrl;
				request.Content = new StringContent(string.Empty);
				request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, this.SubscriptionKey);
				client.Timeout = TimeSpan.FromSeconds(2);
				var response = await client.SendAsync(request);
				this.RequestStatusCode = response.StatusCode;
				response.EnsureSuccessStatusCode();
				var token = await response.Content.ReadAsStringAsync();
				_storedTokenTime = DateTime.Now;
				_storedTokenValue = "Bearer " + token;
				return _storedTokenValue;
			}
		}

		/// <summary>
		/// Gets a token for the specified subscription. Synchronous version.
		/// Use of async version preferred
		/// </summary>
		/// <returns>The encoded JWT token prefixed with the string "Bearer ".</returns>
		/// <remarks>
		/// This method uses a cache to limit the number of request to the token service.
		/// A fresh token can be re-used during its lifetime of 10 minutes. After a successful
		/// request to the token service, this method caches the access token. Subsequent 
		/// invocations of the method return the cached token for the next 5 minutes. After
		/// 5 minutes, a new token is fetched from the token service and the cache is updated.
		/// </remarks>
		public string GetAccessToken()
		{
			// Re-use the cached token if there is one.
			if ((DateTime.Now - _storedTokenTime) < TokenCacheDuration)
			{
				return _storedTokenValue;
			}

			string accessToken = null;
			var task = Task.Run(async () =>
			{
				accessToken = await this.GetAccessTokenAsync();
			});

			while (!task.IsCompleted)
			{
				System.Threading.Thread.Yield();
			}
			if (task.IsFaulted)
			{
				throw task.Exception;
			}
			if (task.IsCanceled)
			{
				throw new Exception("Timeout obtaining access token.");
			}
			return accessToken;
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
			// 05/18/2008 Paul.  Increase timeout to support slower machines. 
			Server.ScriptTimeout = 600;
			Response.Buffer = false;
			dtMain = new DataTable();
			// 01/11/2006 Paul.  Only a developer/administrator should see this. 
			if ( !SplendidCRM.Security.IS_ADMIN )
				return;

			sLang = Sql.ToString(Request["Lang"]);
			if ( sLang == "en-US" )
				return;
#if DEBUG
			if ( Sql.IsEmptyString(sLang) )
				sLang = "all";
#endif
			if ( !Sql.IsEmptyString(sLang) )
			{
				/*
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
				*/
				// 07/20/2017 Paul.  New method of accessing translator. 
				string admToken = String.Empty;
				string sTranslatorKey = Sql.ToString(Application["CONFIG.MicrosoftTranslator.Key"]);
				AzureAuthToken authTokenSource = new AzureAuthToken(sTranslatorKey.Trim());
				try
				{
					admToken = authTokenSource.GetAccessToken();
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
#if FALSE
				if ( true )
				{
					string sURL = "https://api.microsofttranslator.com/V2/Http.svc/GetLanguagesForTranslate";
					HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(sURL);
					admToken = authTokenSource.GetAccessToken();
					objRequest.Headers.Add("Authorization", admToken);
					objRequest.KeepAlive         = false;
					objRequest.AllowAutoRedirect = false;
					objRequest.Timeout           = 15000;  //15 seconds
					objRequest.Method            = "GET";
					StringBuilder sbLanguages = new StringBuilder();
					using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
					{
						if ( objResponse != null )
						{
							if ( objResponse.StatusCode == HttpStatusCode.OK || objResponse.StatusCode == HttpStatusCode.Found )
							{
								using ( StreamReader readStream = new StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.UTF8) )
								{
									string sResponse = readStream.ReadToEnd();
									Debug.WriteLine(sResponse);
									
									XmlDocument xml = new XmlDocument();
									xml.LoadXml(sResponse);
									foreach ( XmlNode xNode in xml.DocumentElement.ChildNodes )
									{
										Debug.WriteLine(xNode.InnerText);
										if ( sbLanguages.Length > 0 )
											sbLanguages.Append(",");
										sbLanguages.Append(xNode.InnerText);
									}
								}
							}
							else
							{
								Response.Write("<font color=red>" + objResponse.StatusCode + " " + objResponse.StatusDescription + "</font><br />"+ ControlChars.CrLf);
							}
						}
					}
					sURL = "https://api.microsofttranslator.com/V2/Http.svc/GetLanguageNames?locale=en-US&languageCodes=" + HttpUtility.UrlEncode(sbLanguages.ToString());
					objRequest = (HttpWebRequest) WebRequest.Create(sURL);
					admToken = authTokenSource.GetAccessToken();
					objRequest.Headers.Add("Authorization", admToken);
					objRequest.KeepAlive         = false;
					objRequest.AllowAutoRedirect = false;
					objRequest.Timeout           = 15000;  //15 seconds
					objRequest.Method            = "GET";
					using ( HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse() )
					{
						if ( objResponse != null )
						{
							if ( objResponse.StatusCode == HttpStatusCode.OK || objResponse.StatusCode == HttpStatusCode.Found )
							{
								using ( StreamReader readStream = new StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.UTF8) )
								{
									string sResponse = readStream.ReadToEnd();
									Debug.WriteLine(sResponse);
								}
							}
							else
							{
								Response.Write("<font color=red>" + objResponse.StatusCode + " " + objResponse.StatusDescription + "</font><br />"+ ControlChars.CrLf);
							}
						}
					}
					return;
				}
#endif
				// 07/22/2017 Paul.  Provide a way to perform all translations. 
				List<string> arrLang = new List<string>();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				if ( sLang == "all" )
				{
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select NAME       " + ControlChars.CrLf
						     + "  from vwLANGUAGES" + ControlChars.CrLf
						     + " order by NAME    " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows )
									{
										sLang = Sql.ToString(row["NAME"]);
										arrLang.Add(sLang);
									}
								}
							}
						}
					}
				}
				else
				{
					arrLang.Add(sLang);
				}
				int nErrors = 0;
				JavaScriptSerializer json = new JavaScriptSerializer();
				for ( int j = 0; j < arrLang.Count && Response.IsClientConnected && nErrors < 20; j++ )
				{
					sLang = arrLang[j];
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						// 05/24/2008 Paul.  Use an outer join so that we only translate missing terms. 
						// 07/20/2017 Paul.  Match NAME and NAME null. 
						sSQL = "select ENGLISH.NAME                                     " + ControlChars.CrLf
						     + "     , ENGLISH.MODULE_NAME                              " + ControlChars.CrLf
						     + "     , ENGLISH.LIST_NAME                                " + ControlChars.CrLf
						     + "     , ENGLISH.LIST_ORDER                               " + ControlChars.CrLf
						     + "     , ENGLISH.DISPLAY_NAME                             " + ControlChars.CrLf
						     + "  from            vwTERMINOLOGY             ENGLISH     " + ControlChars.CrLf
						     + "  left outer join vwTERMINOLOGY             TRANSLATED  " + ControlChars.CrLf
						     + "               on lower(TRANSLATED.LANG) = lower(@LANG) " + ControlChars.CrLf
						     + "              and (TRANSLATED.NAME        = ENGLISH.NAME        or TRANSLATED.NAME        is null and ENGLISH.NAME        is null)" + ControlChars.CrLf
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
									dtMain = new DataTable();
									da.Fill(dtMain);
								}
							}
						}
					}
					CultureInfo culture = null;
					try
					{
						// 07/21/2017 Paul.  sr-SP-Latn is an invalid culture identifier
						culture = new CultureInfo(sLang);
					}
					catch
					{
					}
					if ( culture == null )
					{
						Response.Write("Unknown language: " + sLang + "<br />"+ ControlChars.CrLf);
						continue;  // throw(new Exception("Unknown language: " + sLang));
					}
					SqlProcs.spLANGUAGES_InsertOnly(sLang, culture.LCID, true, culture.NativeName, culture.DisplayName);

					for ( int i = 0 ; i < dtMain.Rows.Count && Response.IsClientConnected && nErrors < 20; i++ )
					{
#if DEBUG
						if ( i >= 10 )
							break;
#endif
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
								// 07/21/2017 paul.  Exceptions to the 2-leter. 
								if ( sLANG == "bs-Latn" )
									sToLang = sLANG;
								if ( sLANG == "sr-Cyrl" )
									sToLang = sLANG;
								if ( sLANG == "sr-Latn" )
									sToLang = sLANG;
								
								// 08/02/2017 Paul.  Some terms are not translated.  These stream labels are used to allow customization but they should not be translated. 
								if ( sNAME == "LBL_STREAM_FIELDS_CREATED" || sNAME == "LBL_STREAM_FIELDS_UPDATED" )
								{
									SqlProcs.spTERMINOLOGY_InsertOnly(sNAME, sLANG, sMODULE_NAME, sLIST_NAME, nLIST_ORDER, sDISPLAY_NAME);
									continue;
								}
								
								string sURL = "https://api.microsofttranslator.com/V2/Http.svc/Translate?contentType=" + HttpUtility.UrlEncode("text/plain") + "&text=" + HttpUtility.UrlEncode(sDISPLAY_NAME) + "&from=en" + "&to=" + sToLang;
								HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(sURL);
								// 07/20/2017 Paul.  Get new token if it has expired. 
								admToken = authTokenSource.GetAccessToken();
								objRequest.Headers.Add("Authorization", admToken);
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
												Response.Write(sToLang + ": " + sTranslation + "<br />"+ ControlChars.CrLf);
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
				}
				dtMain = new DataTable();
				// 05/18/2008 Paul.  Cannot redirect when response buffering is off. 
				//Response.Redirect("Terminology.aspx");
#if !DEBUG
				if ( nErrors == 0 )
					Response.Write("<script type=\"text/javascript\">window.location.href='Terminology.aspx';</script>");
#endif
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

