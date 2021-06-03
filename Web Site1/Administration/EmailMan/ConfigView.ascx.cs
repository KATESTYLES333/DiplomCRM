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
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Xml;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace SplendidCRM.Administration.EmailMan
{
	/// <summary>
	///		Summary description for ConfigView.
	/// </summary>
	public class ConfigView : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;

		private const string sEMPTY_PASSWORD = "**********";
		protected TextBox     NOTIFY_FROMNAME       ;
		protected TextBox     NOTIFY_FROMADDRESS    ;
		// 11/18/2010 Paul.  NOTIFY_ON is not used. 
		protected CheckBox    NOTIFY_SEND_FROM_ASSIGNING_USER;
		protected Label       MAIL_SENDTYPE         ;
		protected TextBox     MAIL_SMTPSERVER       ;
		protected TextBox     MAIL_SMTPPORT         ;
		protected CheckBox    MAIL_SMTPAUTH_REQ     ;
		protected CheckBox    MAIL_SMTPSSL          ;
		protected TextBox     MAIL_SMTPUSER         ;
		protected TextBox     MAIL_SMTPPASS         ;

		protected RequiredFieldValidator reqNOTIFY_FROMNAME   ;
		protected RequiredFieldValidator reqNOTIFY_FROMADDRESS;
		protected RequiredFieldValidator reqMAIL_SMTPSERVER   ;
		protected RequiredFieldValidator reqMAIL_SMTPPORT     ;
		protected RequiredFieldValidator reqMAIL_SMTPUSER     ;
		protected RequiredFieldValidator reqMAIL_SMTPPASS     ;

		protected string sDangerousTags = "html|meta|body|base|form|style|applet|object|script|embed|xml|frameset|iframe|frame|blink|link|ilayer|layer|import|xmp|bgsound";
		protected string sOutlookTags   = "base|form|style|applet|object|script|embed|frameset|iframe|frame|link|ilayer|layer|import|xmp";
		protected CheckBox    EMAIL_INBOUND_SAVE_RAW   ;
		protected CheckBox    SECURITY_TOGGLE_ALL      ;
		protected CheckBox    SECURITY_OUTLOOK_DEFAULTS;
		protected Table       tblSECURITY_TAGS         ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				reqNOTIFY_FROMNAME   .Enabled = true;
				reqNOTIFY_FROMADDRESS.Enabled = true;
				reqMAIL_SMTPSERVER   .Enabled = true;
				reqMAIL_SMTPPORT     .Enabled = true;
				// 10/27/2008 Paul.  Allow the authentication to be optional. 
				reqMAIL_SMTPUSER     .Enabled = MAIL_SMTPAUTH_REQ.Checked;
				reqMAIL_SMTPPASS     .Enabled = MAIL_SMTPAUTH_REQ.Checked;
				reqNOTIFY_FROMNAME   .Validate();
				reqNOTIFY_FROMADDRESS.Validate();
				reqMAIL_SMTPSERVER   .Validate();
				reqMAIL_SMTPPORT     .Validate();
				reqMAIL_SMTPUSER     .Validate();
				reqMAIL_SMTPPASS     .Validate();
				if ( Page.IsValid )
				{
					try
					{
						// 01/08/2008 Paul.  If the encryption key does not exist, then we must create it and we must save it back to the database. 
						// 01/08/2008 Paul.  SugarCRM uses blowfish for the inbound email encryption, but we will not since .NET 2.0 does not support blowfish natively. 
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						string sMAIL_SMTPPASS = MAIL_SMTPPASS.Text;
						// 07/08/2010 Paul.  We want to save the password for later use. 
						if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
						{
							sMAIL_SMTPPASS = Sql.ToString(ViewState["smtppass"]);
						}
						else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sMAIL_SMTPPASS )
								throw(new Exception("Decryption failed"));
							sMAIL_SMTPPASS = sENCRYPTED_EMAIL_PASSWORD;
							MAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						
						int nMAIL_SMTPPORT = Sql.ToInteger(MAIL_SMTPPORT.Text);
						Application["CONFIG.fromname"                       ] = NOTIFY_FROMNAME                .Text;
						Application["CONFIG.fromaddress"                    ] = NOTIFY_FROMADDRESS             .Text;
						Application["CONFIG.notify_send_from_assigning_user"] = NOTIFY_SEND_FROM_ASSIGNING_USER.Checked;
						Application["CONFIG.mail_sendtype"                  ] = MAIL_SENDTYPE                  .Text;
						Application["CONFIG.smtpserver"                     ] = MAIL_SMTPSERVER                .Text;
						Application["CONFIG.smtpport"                       ] = nMAIL_SMTPPORT                 .ToString();
						Application["CONFIG.smtpuser"                       ] = MAIL_SMTPUSER                  .Text;
						Application["CONFIG.smtppass"                       ] = sMAIL_SMTPPASS;
						Application["CONFIG.smtpauth_req"                   ] = MAIL_SMTPAUTH_REQ              .Checked;
						Application["CONFIG.smtpssl"                        ] = MAIL_SMTPSSL                   .Checked;
						Application["CONFIG.email_inbound_save_raw"         ] = EMAIL_INBOUND_SAVE_RAW         .Checked;

						StringBuilder sbEMAIL_XSS = new StringBuilder();
						foreach ( string sTag in sDangerousTags.Split('|') )
						{
							CheckBox chk = FindControl("SECURITY_" + sTag.ToUpper()) as CheckBox;
							if ( chk.Checked )
							{
								if ( sbEMAIL_XSS.Length > 0 ) sbEMAIL_XSS.Append("|");
								sbEMAIL_XSS.Append(sTag);
							}
						}
						Application["CONFIG.email_xss"] = sbEMAIL_XSS.ToString();

						SqlProcs.spCONFIG_Update("notify", "fromname"                , Sql.ToString(Application["CONFIG.fromname"                ]));
						SqlProcs.spCONFIG_Update("notify", "fromaddress"             , Sql.ToString(Application["CONFIG.fromaddress"             ]));
						SqlProcs.spCONFIG_Update("notify", "send_from_assigning_user", Sql.ToString(Application["CONFIG.send_from_assigning_user"]));
						SqlProcs.spCONFIG_Update("mail"  , "smtpserver"              , Sql.ToString(Application["CONFIG.smtpserver"              ]));
						SqlProcs.spCONFIG_Update("mail"  , "smtpport"                , Sql.ToString(Application["CONFIG.smtpport"                ]));
						SqlProcs.spCONFIG_Update("mail"  , "smtpuser"                , Sql.ToString(Application["CONFIG.smtpuser"                ]));
						SqlProcs.spCONFIG_Update("mail"  , "smtppass"                , Sql.ToString(Application["CONFIG.smtppass"                ]));
						SqlProcs.spCONFIG_Update("mail"  , "smtpauth_req"            , Sql.ToString(Application["CONFIG.smtpauth_req"            ]));
						SqlProcs.spCONFIG_Update("mail"  , "smtpssl"                 , Sql.ToString(Application["CONFIG.smtpssl"                 ]));
						SqlProcs.spCONFIG_Update("mail"  , "email_inbound_save_raw"  , Sql.ToString(Application["CONFIG.email_inbound_save_raw"  ]));
						SqlProcs.spCONFIG_Update("mail"  , "email_xss"               , Sql.ToString(Application["CONFIG.email_xss"               ]));
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("../default.aspx");
				}
			}
			else if ( e.CommandName == "Test" )
			{
				reqNOTIFY_FROMNAME   .Enabled = true;
				reqNOTIFY_FROMADDRESS.Enabled = true;
				reqMAIL_SMTPSERVER   .Enabled = true;
				reqMAIL_SMTPPORT     .Enabled = true;
				// 10/27/2008 Paul.  Allow the authentication to be optional. 
				reqMAIL_SMTPUSER     .Enabled = MAIL_SMTPAUTH_REQ.Checked;
				reqMAIL_SMTPPASS     .Enabled = MAIL_SMTPAUTH_REQ.Checked;
				reqNOTIFY_FROMNAME   .Validate();
				reqNOTIFY_FROMADDRESS.Validate();
				reqMAIL_SMTPSERVER   .Validate();
				reqMAIL_SMTPPORT     .Validate();
				reqMAIL_SMTPUSER     .Validate();
				reqMAIL_SMTPPASS     .Validate();
				if ( Page.IsValid )
				{
					try
					{
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						string sMAIL_SMTPPASS = MAIL_SMTPPASS.Text;
						// 07/08/2010 Paul.  We want to save the password for later use. 
						if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
						{
							sMAIL_SMTPPASS = Sql.ToString(ViewState["smtppass"]);
							if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
								sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							ViewState["smtppass"] = sENCRYPTED_EMAIL_PASSWORD;
							MAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						string sSmtpServer      = MAIL_SMTPSERVER  .Text;
						int    nSmtpPort        = Sql.ToInteger(MAIL_SMTPPORT.Text);
						bool   bSmtpAuthReq     = MAIL_SMTPAUTH_REQ.Checked;
						bool   bSmtpSSL         = MAIL_SMTPSSL     .Checked;
						string sSmtpUser        = MAIL_SMTPUSER    .Text;
						string sSmtpPassword    = sMAIL_SMTPPASS;
						if ( Sql.IsEmptyString(sSmtpServer) )
						{
							sSmtpServer = "127.0.0.1";
							MAIL_SMTPSERVER.Text = sSmtpServer;
						}
						if ( nSmtpPort == 0 )
						{
							nSmtpPort = 25;
							MAIL_SMTPPORT.Text = nSmtpPort.ToString();
						}
						EmailUtils.SendTestMessage(Application, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, NOTIFY_FROMADDRESS.Text, NOTIFY_FROMNAME.Text, NOTIFY_FROMADDRESS.Text, NOTIFY_FROMNAME.Text);
						ctlDynamicButtons.ErrorText = "Send was successful.";
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
						return;
					}
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				Response.Redirect("../default.aspx");
			}
		}

		private void BuildSecurityTags()
		{
			// 01/21/2008 Paul.  We must always build the table, but on postback, we must do it inside init. 
			StringDictionary dictEMAIL_XSS = new StringDictionary();
			if ( !IsPostBack )
			{
				string sEMAIL_XSS = Sql.ToString(Application["CONFIG.email_xss"]);
				sEMAIL_XSS = sEMAIL_XSS.ToLower();
				foreach ( string sTag in sEMAIL_XSS.Split('|') )
				{
					dictEMAIL_XSS.Add(sTag, sTag);
				}
			}

			int nTagIndex = 0;
			TableRow tr = null;
			foreach ( string sTag in sDangerousTags.Split('|') )
			{
				if ( nTagIndex % 2 == 0 )
				{
					tr = new TableRow();
					tblSECURITY_TAGS.Rows.Add(tr);
				}
				TableCell td1 = new TableCell();
				CheckBox  chk = new CheckBox();
				tr.Cells.Add(td1);
				td1.Controls.Add(chk);
				td1.VerticalAlign = VerticalAlign.Bottom;
				chk.ID   = "SECURITY_" + sTag.ToUpper();
				chk.Text = "&lt;" + sTag + "&gt;";
				chk.CssClass = "checkbox";

				if ( !IsPostBack )
					chk.Checked = dictEMAIL_XSS.ContainsKey(sTag);

				TableCell td2 = new TableCell();
				Label     lbl = new Label();
				tr.Cells.Add(td2);
				td2.Controls.Add(lbl);
				td2.VerticalAlign = VerticalAlign.Bottom;
				if ( !IsPostBack )
					lbl.Text = L10n.Term("EmailMan.LBL_SECURITY_" + sTag.ToUpper());
				nTagIndex++;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("EmailMan.LBL_CAMPAIGN_EMAIL_SETTINGS"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			try
			{
				reqNOTIFY_FROMNAME   .DataBind();
				reqNOTIFY_FROMADDRESS.DataBind();
				reqMAIL_SMTPSERVER   .DataBind();
				reqMAIL_SMTPPORT     .DataBind();
				reqMAIL_SMTPUSER     .DataBind();
				reqMAIL_SMTPPASS     .DataBind();
				if ( !IsPostBack )
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_KEY) )
					{
						gINBOUND_EMAIL_KEY = Guid.NewGuid();
						SqlProcs.spCONFIG_Update("mail", "InboundEmailKey", gINBOUND_EMAIL_KEY.ToString());
						Application["CONFIG.InboundEmailKey"] = gINBOUND_EMAIL_KEY;
					}
					Guid gINBOUND_EMAIL_IV = Sql.ToGuid(Application["CONFIG.InboundEmailIV"]);
					if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_IV) )
					{
						gINBOUND_EMAIL_IV = Guid.NewGuid();
						SqlProcs.spCONFIG_Update("mail", "InboundEmailIV", gINBOUND_EMAIL_IV.ToString());
						Application["CONFIG.InboundEmailIV"] = gINBOUND_EMAIL_IV;
					}
					NOTIFY_FROMNAME                .Text    = Sql.ToString (Application["CONFIG.fromname"                       ]);
					NOTIFY_FROMADDRESS             .Text    = Sql.ToString (Application["CONFIG.fromaddress"                    ]);
					NOTIFY_SEND_FROM_ASSIGNING_USER.Checked = Sql.ToBoolean(Application["CONFIG.notify_send_from_assigning_user"]);
					MAIL_SENDTYPE                  .Text    = Sql.ToString (Application["CONFIG.mail_sendtype"                  ]);
					MAIL_SMTPSERVER                .Text    = Sql.ToString (Application["CONFIG.smtpserver"                     ]);
					MAIL_SMTPPORT                  .Text    = Sql.ToString (Application["CONFIG.smtpport"                       ]);
					MAIL_SMTPUSER                  .Text    = Sql.ToString (Application["CONFIG.smtpuser"                       ]);
					//MAIL_SMTPPASS                  .Text    = Sql.ToString (Application["CONFIG.smtppass"                       ]);
					MAIL_SMTPAUTH_REQ              .Checked = Sql.ToBoolean(Application["CONFIG.smtpauth_req"                   ]);
					MAIL_SMTPSSL                   .Checked = Sql.ToBoolean(Application["CONFIG.smtpssl"                        ]);
					// 01/20/2008 Paul.  We are going to deviate from SugarCRM and associate the Preserve text with save raw. 
					EMAIL_INBOUND_SAVE_RAW.Checked = Sql.ToBoolean(Application["CONFIG.email_inbound_save_raw"]);

					NOTIFY_SEND_FROM_ASSIGNING_USER.Checked = false;
					MAIL_SENDTYPE                  .Text    = "SMTP";
					// 10/27/2008 Paul. Allow the authentication to be optional. 
					//MAIL_SMTPAUTH_REQ              .Checked = true ;

					string sMAIL_SMTPPASS = Sql.ToString (Application["CONFIG.smtppass"]);
					// 01/08/2008 Paul.  Don't display the password. 
					// 01/08/2008 Paul.  Browsers don't display passwords. 
					// 07/08/2010 Paul.  We want to save the password for later use. 
					if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
					{
						//MAIL_SMTPPASS.Text = sEMPTY_PASSWORD;
						MAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
						ViewState["smtppass"] = sMAIL_SMTPPASS;
					}
					// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
					BuildSecurityTags();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
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
			// 05/20/2007 Paul.  The m_sMODULE field must be set in order to allow default export handling. 
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "EmailMan";
			if ( IsPostBack )
			{
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".ConfigView", Guid.Empty, null);
				BuildSecurityTags();
			}
		}
		#endregion
	}
}


