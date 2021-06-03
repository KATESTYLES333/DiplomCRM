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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Net.Mail;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for LoginView.
	/// </summary>
	public class LoginView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;

		protected Label           lblError                        ;
		protected TextBox         txtUSER_NAME                    ;
		protected TextBox         txtPASSWORD                     ;
		protected Table           tblUser                         ;
		protected TableRow        trError                         ;
		protected HyperLink       lnkWorkOnline                   ;
		protected HyperLink       lnkHTML5Client                  ;
		
		protected TextBox         txtFORGOT_USER_NAME             ;
		protected TextBox         txtFORGOT_EMAIL                 ;
		protected Panel           pnlForgotPassword               ;
		protected Table           tblForgotUser                   ;
		protected TableRow        trForgotError                   ;
		protected Label           lblForgotError                  ;
		protected FacebookLogin   ctlFacebookLogin                ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Login" || e.CommandName == "Login.Facebook" )
			{
				trForgotError.Visible = false;
				lblForgotError.Text = String.Empty;
				if ( Page.IsValid )
				{
					bool bValidUser  = false;
					try
					{
						if ( Security.IsWindowsAuthentication() )
						{
							bValidUser = true;
						}
						else
						{
							// 03/19/2011 Paul.  If the facebook user has been authenticated, then all we will have is the user name. 
							if ( e.CommandName == "Login.Facebook" )
							{
								string sAppID     = Sql.ToString(Application["CONFIG.facebook.AppID"    ]);
								string sAppSecret = Sql.ToString(Application["CONFIG.facebook.AppSecret"]);
								
								FacebookUtils fbUtils = new FacebookUtils(sAppID, sAppSecret, Request.Cookies);
								if ( fbUtils.FacebookValuesExist )
								{
									fbUtils.ParseCookie();
									if ( !fbUtils.IsValidSignature() )
									{
										throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_SIGNATURE")));
									}
									bValidUser = SplendidInit.FacebookLoginUser(fbUtils.UID);
									if ( !bValidUser  )
									{
										throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_LOGIN")));
									}
								}
								else
								{
									throw(new Exception(L10n.Term("Users.ERR_FACEBOOK_COOKIE")));
								}
							}
							else
							{
								// 02/20/2011 Paul.  Skip the login if the user has been locked. 
								// 04/16/2013 Paul.  Throw an exception so that we can track lockout count failures in the error log. 
								if ( SplendidInit.LoginFailures(Application, txtUSER_NAME.Text) >= Crm.Password.LoginLockoutCount(Application) )
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
								bValidUser = SplendidInit.LoginUser(txtUSER_NAME.Text, txtPASSWORD.Text, String.Empty, String.Empty);
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						trError.Visible = true;
						lblError.Text = ex.Message;
						return;
					}
					// 09/12/2006 Paul.  Move redirect outside try/catch to avoid catching "Thread was being aborted" exception. 
					if ( bValidUser )
					{
						// 02/22/2011 Paul.  The login redirect is also needed after the change password. 
						LoginRedirect();
						return;
					}
					else
					{
						trError.Visible = true;
						lblError.Text = L10n.Term("Users.ERR_INVALID_PASSWORD");
					}
				}
			}
			else if ( e.CommandName == "ForgotPassword" )
			{
				trError.Visible = false;
				lblError.Text = String.Empty;
				pnlForgotPassword.Style.Remove("display");
				try
				{
					txtFORGOT_USER_NAME.Text = txtFORGOT_USER_NAME.Text.Trim();
					txtFORGOT_EMAIL    .Text = txtFORGOT_EMAIL    .Text.Trim();
					if ( !Security.IsWindowsAuthentication() )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory(Application);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL;
							sSQL = "select *                            " + ControlChars.CrLf
							     + "  from vwUSERS                      " + ControlChars.CrLf
							     + " where lower(USER_NAME) = @USER_NAME" + ControlChars.CrLf
							     + "   and lower(EMAIL1   ) = @EMAIL1   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@USER_NAME", txtFORGOT_USER_NAME.Text.ToLower());
								Sql.AddParameter(cmd, "@EMAIL1"   , txtFORGOT_EMAIL    .Text.ToLower());
								using ( IDataReader rdr = cmd.ExecuteReader() )
								{
									//string sApplicationPath = Sql.ToString(Application["rootURL"]);
									Guid gUSER_LOGIN_ID = Guid.Empty;
									if ( rdr.Read() )
									{
										MailMessage mail = new MailMessage();
										string sFromName     = Sql.ToString (Application["CONFIG.fromname"    ]);
										string sFromAddress  = Sql.ToString (Application["CONFIG.fromaddress" ]);
										if ( !Sql.IsEmptyString(sFromAddress) && !Sql.IsEmptyString(sFromName) )
											mail.From = new MailAddress(sFromAddress, sFromName);
										else
											mail.From = new MailAddress(sFromAddress);
										mail.To.Add(new MailAddress(txtFORGOT_EMAIL.Text));
										// 10/05/2008 Paul.  If there are no recipients, then exit early. 
										if ( mail.To.Count == 0 && mail.CC.Count == 0 && mail.Bcc.Count == 0 )
											return;
										
										Guid gPASSWORD_ID = Guid.Empty;
										SqlProcs.spUSERS_PASSWORD_LINK_InsertOnly(ref gPASSWORD_ID, txtFORGOT_USER_NAME.Text);
										
										string sSiteURL   = Crm.Config.SiteURL(Application);
										string sResetURL  = sSiteURL + "Users/ChangePassword.aspx?ID=" + gPASSWORD_ID.ToString();
										string sSubject   = L10n.Term("Users.LBL_RESET_PASSWORD_SUBJECT");
										if ( Sql.IsEmptyString(sSubject) )
											sSubject = "Reset your password";
										string sBodyHtml  = L10n.Term("Users.LBL_RESET_PASSWORD_BODY");
										if ( Sql.IsEmptyString(sBodyHtml) )
										{
											sBodyHtml += "<p>A password reset was requested.</p>\n";
											sBodyHtml += "<p>Please click the following link to reset your password:</p>\n";
											sBodyHtml += "<p><a href=\"{0}\">{0}</a></p>\n";
										}
										if ( sBodyHtml.IndexOf("{0}") < 0 )
										{
											sBodyHtml += "<p><a href=\"{0}\">{0}</a></p>\n";
										}
										sBodyHtml = String.Format(sBodyHtml, sResetURL);
										mail.Subject      = sSubject ;
										mail.Body         = sBodyHtml;
										mail.IsBodyHtml   = true;
										mail.BodyEncoding = System.Text.Encoding.UTF8;
										
										SmtpClient client = EmailUtils.CreateSmtpClient(Application);
										client.Send(mail);
										trForgotError.Visible = true;
										lblForgotError.Text = L10n.Term("Users.LBL_RESET_PASSWORD_STATUS");
									}
									else
									{
										trForgotError.Visible = true;
										lblForgotError.Text = L10n.Term("Users.ERR_INVALID_FORGOT_PASSWORD");
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					trForgotError.Visible = true;
					lblForgotError.Text = ex.Message;
					return;
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 12/27/2008 Paul.  Set the page title so that a bookmark will not default to "Login". 
			// 08/18/2011 Paul.  Make sure to use the terminology table for the browser title. 
			SetPageTitle(L10n.Term(".LBL_BROWSER_TITLE"));
			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				if ( !IsPostBack )
				{
					string sDefaultUserName = Sql.ToString(Application["CONFIG.default_user_name"]);
					string sDefaultPassword = Sql.ToString(Application["CONFIG.default_password" ]);
					string sDefaultTheme    = Sql.ToString(Application["CONFIG.default_theme"    ]);
					string sDefaultLanguage = Sql.ToString(Application["CONFIG.default_language" ]);
					txtUSER_NAME.Text = sDefaultUserName;
					txtPASSWORD.Text  = sDefaultPassword;

					// 11/19/2009 Paul.  File IO is expensive, so cache the results of the Exists test. 
					// 08/25/2013 Paul.  File IO is slow, so cache existance test. 
					lnkWorkOnline .Visible = Utils.CachedFileExists(Context, "~/Users/ClientLogin.aspx");
					lnkHTML5Client.Visible = Utils.CachedFileExists(Context, "~/html5/default.aspx"    );
					// 03/19/2011 Paul.  Facebook login does not make sense on the offline client. 
					ctlFacebookLogin.Visible = !lnkWorkOnline.Visible && Sql.ToBoolean(Application["CONFIG.facebook.EnableLogin"]) && !Sql.IsEmptyString(Application["CONFIG.facebook.AppID"]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlFacebookLogin.Command += new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}


