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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using Koolwired.Imap;

namespace SplendidCRM.Administration.InboundEmail
{
	/// <summary>
	///		Summary description for Mailbox.
	/// </summary>
	public class Mailbox : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected Guid            gID            ;
		protected DataTable       dtMain         ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected Label           lblError       ;
		protected StringBuilder   sbTrace        ;

		protected void Pop3Trace(string sText)
		{
			sbTrace.AppendLine(sText);
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				sbTrace = new StringBuilder();
				if ( e.CommandName == "Mailbox.CheckBounce" )
				{
					EmailUtils.CheckInbound(HttpContext.Current, gID, true);
				}
				else if ( e.CommandName == "Mailbox.CheckInbound" )
				{
					EmailUtils.CheckInbound(HttpContext.Current, gID, false);
				}
				if ( e.CommandName == "Mailbox.CheckMail" || e.CommandName == "Mailbox.CheckBounce" || e.CommandName == "Mailbox.CheckInbound" )
				{
					string sSERVER_URL     = Sql.ToString (ViewState["SERVER_URL"    ]);
					string sEMAIL_USER     = Sql.ToString (ViewState["EMAIL_USER"    ]);
					string sEMAIL_PASSWORD = Sql.ToString (ViewState["EMAIL_PASSWORD"]);
					int    nPORT           = Sql.ToInteger(ViewState["PORT"          ]);
					string sSERVICE        = Sql.ToString (ViewState["SERVICE"       ]);
					bool   bMAILBOX_SSL    = Sql.ToBoolean(ViewState["MAILBOX_SSL"   ]);
					string sMAILBOX        = Sql.ToString (ViewState["MAILBOX"       ]);

					// 01/08/2008 Paul.  Decrypt at the last minute to ensure that an unencrypted password is never sent to the browser. 
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					sEMAIL_PASSWORD = Security.DecryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);

					dtMain = new DataTable();
					dtMain.Columns.Add("From"        , typeof(System.String  ));
					dtMain.Columns.Add("Sender"      , typeof(System.String  ));
					dtMain.Columns.Add("ReplyTo"     , typeof(System.String  ));
					dtMain.Columns.Add("To"          , typeof(System.String  ));
					dtMain.Columns.Add("CC"          , typeof(System.String  ));
					dtMain.Columns.Add("Bcc"         , typeof(System.String  ));
					dtMain.Columns.Add("Subject"     , typeof(System.String  ));
					dtMain.Columns.Add("DeliveryDate", typeof(System.DateTime));
					dtMain.Columns.Add("Priority"    , typeof(System.String  ));
					dtMain.Columns.Add("Size"        , typeof(System.Int32   ));
					dtMain.Columns.Add("ContentID"   , typeof(System.String  ));
					dtMain.Columns.Add("MessageID"   , typeof(System.String  ));
					dtMain.Columns.Add("Headers"     , typeof(System.String  ));
					//dtMain.Columns.Add("Body"        , typeof(System.String  ));

					// 10/28/2010 Paul.  Add support for IMAP. 
					if ( String.Compare(sSERVICE, "imap", true) == 0 )
					{
						try
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
									command.Fetch(mailbox);
									if ( mailbox != null && mailbox.Messages != null )
									{
										// 04/21/2011 Paul.  Limit the messages to 200 to prevent a huge loop. 
										for ( int i = 0; i < mailbox.Messages.Count && i < 200; i++ )
										{
											// 04/21/2011 Paul.  Reverse the index so that the newest are first. 
											ImapMailboxMessage email = mailbox.Messages[mailbox.Messages.Count - i - 1];
											
											// 10/28/2010 Paul.  IMPORTANT BUG FIX.  We need to call FetchBodyStructure instead of FetchBody 
											// because FetchBody will create a new message and add it to the Messages list, thereby creating an endless loop. 
											//command.FetchBodyStructure(email);
											
											string[] arrHeaders = command.FetchHeaders(email.UID);
											StringBuilder sbTO_ADDRS  = new StringBuilder();
											StringBuilder sbCC_ADDRS  = new StringBuilder();
											StringBuilder sbBCC_ADDRS = new StringBuilder();
											if ( email.To != null )
											{
												foreach ( ImapAddress addr in email.To )
												{
													// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
													sbTO_ADDRS.Append((sbTO_ADDRS.Length > 0) ? "; " : String.Empty);
													sbTO_ADDRS.Append(addr.ToString());
												}
											}
											if ( email.CC != null )
											{
												foreach ( ImapAddress addr in email.CC )
												{
													// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
													sbCC_ADDRS.Append((sbCC_ADDRS.Length > 0) ? "; " : String.Empty);
													sbCC_ADDRS.Append(addr.ToString());
												}
											}
											if ( email.BCC != null )
											{
												foreach ( ImapAddress addr in email.BCC )
												{
													// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
													sbBCC_ADDRS.Append((sbBCC_ADDRS.Length > 0) ? "; " : String.Empty);
													sbBCC_ADDRS.Append(addr.ToString());
												}
											}
											
											DataRow row = dtMain.NewRow();
											dtMain.Rows.Add(row);
											row["From"        ] = Server.HtmlEncode(((email.From   != null) ? email.From  .ToString() : String.Empty));
											row["Sender"      ] = Server.HtmlEncode(((email.Sender != null) ? email.Sender.ToString() : String.Empty));
											// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
											//row["ReplyTo"     ] = Server.HtmlEncode(Sql.ToString(mm.ReplyTo));
											row["ReplyTo"     ] = DBNull.Value;
											row["To"          ] = Server.HtmlEncode(sbTO_ADDRS .ToString());
											row["CC"          ] = Server.HtmlEncode(sbCC_ADDRS .ToString());
											row["Bcc"         ] = Server.HtmlEncode(sbBCC_ADDRS.ToString());
											row["Subject"     ] = Server.HtmlEncode(email.Subject);
											// 01/23/2008 Paul.  DateTime in the email is in universal time. 
											row["DeliveryDate"] = email.Received.ToLocalTime();
											row["Priority"    ] = DBNull.Value   ;
											row["Size"        ] = email.Size     ;
											row["ContentId"   ] = DBNull.Value   ;
											row["MessageId"   ] = email.MessageID;
											row["Headers"     ] = "<pre>" + Server.HtmlEncode(String.Join(ControlChars.CrLf, arrHeaders)) + "</pre>";
											//row["Body"        ] = mm.Body        ;
										}
									}
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlDynamicButtons.ErrorText = ex.Message;
						}
					}
					else if ( String.Compare(sSERVICE, "pop3", true) == 0 )
					{
						Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
						try
						{
							pop.Trace += new Pop3.TraceHandler(this.Pop3Trace);
							pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
							pop.Connect();
							
							int nTotalEmails = 0;
							int mailboxSize  = 0;
							pop.GetMailboxStats(out nTotalEmails, out mailboxSize);

							List<int> arrEmailIds = new List<int>();
							pop.GetEmailIdList(out arrEmailIds);
							// 04/21/2011 Paul.  Limit the messages to 200 to prevent a huge loop. 
							// 08/30/2011 Paul.  The i index is not the message ID.  Go back to using foreach to iterate through the list. 
							arrEmailIds.Reverse();
							if ( arrEmailIds.Count > 200 )
								arrEmailIds.RemoveRange(200, arrEmailIds.Count - 200);
							foreach ( int i in arrEmailIds )
							{
								// 04/21/2011 Paul.  Reverse the index so that the newest are first. 
								int nEmailSize = pop.GetEmailSize(i);
								if ( nEmailSize < 10 * 1024 * 1024 )
								{
									Pop3.RxMailMessage mm = null;
#if DEBUG
									pop.IsCollectRawEmail = true;
#endif
									pop.GetHeaders(i, out mm);
									if ( mm == null )
									{
										sbTrace.AppendLine("Email " + i.ToString() + " cannot be displayed.");
									}
									else
									{
										DataRow row = dtMain.NewRow();
										dtMain.Rows.Add(row);
										row["From"        ] = Server.HtmlEncode(Sql.ToString(mm.From   ));
										row["Sender"      ] = Server.HtmlEncode(Sql.ToString(mm.Sender ));
										// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
										//row["ReplyTo"     ] = Server.HtmlEncode(Sql.ToString(mm.ReplyTo));
										row["ReplyTo"     ] = DBNull.Value;
										row["To"          ] = Server.HtmlEncode(Sql.ToString(mm.To     ));
										row["CC"          ] = Server.HtmlEncode(Sql.ToString(mm.CC     ));
										row["Bcc"         ] = Server.HtmlEncode(Sql.ToString(mm.Bcc    ));
										row["Subject"     ] = Server.HtmlEncode(mm.Subject);
										// 01/23/2008 Paul.  DateTime in the email is in universal time. 
										row["DeliveryDate"] = T10n.FromUniversalTime(mm.DeliveryDate);
										row["Priority"    ] = mm.Priority.ToString();
										row["Size"        ] = nEmailSize     ;
										row["ContentId"   ] = mm.ContentId   ;
										row["MessageId"   ] = mm.MessageId   ;
										row["Headers"     ] = "<pre>" + Server.HtmlEncode(mm.RawContent) + "</pre>";
										//row["Body"        ] = mm.Body        ;
									}
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlDynamicButtons.ErrorText = ex.Message;
						}
						finally
						{
							pop.Disconnect();
						}
					}
					ViewState["Inbox"] = dtMain;
					vwMain = new DataView(dtMain);
					grdMain.DataSource = vwMain;
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
			finally
			{
#if DEBUG
				RegisterClientScriptBlock("Pop3Trace", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript(sbTrace.ToString()) + "';</script>");
#endif
				}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;
			
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + ".Mailbox", Guid.Empty, Guid.Empty);

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL;
						sSQL = "select *                    " + ControlChars.CrLf
						     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
						     + " where ID = @ID             " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("vwINBOUND_EMAILS_Edit", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									string sSERVER_URL     = Sql.ToString (rdr["SERVER_URL"    ]);
									string sEMAIL_USER     = Sql.ToString (rdr["EMAIL_USER"    ]);
									string sEMAIL_PASSWORD = Sql.ToString (rdr["EMAIL_PASSWORD"]);
									int    nPORT           = Sql.ToInteger(rdr["PORT"          ]);
									string sSERVICE        = Sql.ToString (rdr["SERVICE"       ]);
									bool   bMAILBOX_SSL    = Sql.ToBoolean(rdr["MAILBOX_SSL"   ]);
									string sMAILBOX        = Sql.ToString (rdr["MAILBOX"       ]);
									
									ViewState["SERVER_URL"    ] = sSERVER_URL    ;
									ViewState["EMAIL_USER"    ] = sEMAIL_USER    ;
									ViewState["EMAIL_PASSWORD"] = sEMAIL_PASSWORD;
									ViewState["PORT"          ] = nPORT          ;
									ViewState["SERVICE"       ] = sSERVICE       ;
									ViewState["MAILBOX_SSL"   ] = bMAILBOX_SSL   ;
									// 04/21/2011 Paul.  We need the mailbox for Imap tests. 
									ViewState["MAILBOX"       ] = sMAILBOX       ;
								}
							}
						}
					}
				}
				else
				{
					if ( ViewState["Inbox"] != null )
					{
						dtMain = ViewState["Inbox"] as DataTable;
					}
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "InboundEmail";
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons(m_sMODULE + ".Mailbox", Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}


