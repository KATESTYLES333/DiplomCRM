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
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.InboundEmail
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader   ctlModuleHeader  ;
		protected _controls.DynamicButtons ctlDynamicButtons;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.DynamicButtons ctlFooterButtons ;

		private const string sEMPTY_PASSWORD = "**********";
		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;
		protected HtmlTable       tblOptions                   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );
					this.ValidateEditViewFields(m_sMODULE + ".EditOptions");

					if ( Page.IsValid )
					{
						// 07/19/2010 Paul.  Add support for IMAP. 
						/*
						DropDownList SERVICE = FindControl("SERVICE") as DropDownList;
						if ( SERVICE != null )
						{
							if ( SERVICE.SelectedValue == "imap" )
							{
								ctlDynamicButtons.ErrorText += "POP3 is the only supported service at this time. ";
								return;
							}
						}
						*/
						/*
						// 02/12/2008 Paul.  Start to allow other inbound email features. 
						DropDownList MAILBOX_TYPE = FindControl("MAILBOX_TYPE") as DropDownList;
						if ( MAILBOX_TYPE != null )
						{
							if ( MAILBOX_TYPE.SelectedValue != "bounce" )
							{
								ctlDynamicButtons.ErrorText += "Bounce handling is the only supported action at this time. ";
								return;
							}
						}
						*/
					}
					if ( Page.IsValid )
					{
						// 01/08/2008 Paul.  If the encryption key does not exist, then we must create it and we must save it back to the database. 
						// 01/08/2008 Paul.  SugarCRM uses blowfish for the inbound email encryption, but we will not since .NET 2.0 does not support blowfish natively. 
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						string sEMAIL_PASSWORD = sEMPTY_PASSWORD;
						TextBox EMAIL_PASSWORD = FindControl("EMAIL_PASSWORD") as TextBox;
						if ( EMAIL_PASSWORD != null )
							sEMAIL_PASSWORD = EMAIL_PASSWORD.Text;
						// 07/08/2010 Paul.  We want to save the password for later use. 
						if ( sEMAIL_PASSWORD == sEMPTY_PASSWORD )
						{
							sEMAIL_PASSWORD = Sql.ToString(ViewState["smtppass"]);
						}
						else
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sEMAIL_PASSWORD )
								throw(new Exception("Decryption failed"));
							sEMAIL_PASSWORD = sENCRYPTED_EMAIL_PASSWORD;
							if ( EMAIL_PASSWORD != null )
								EMAIL_PASSWORD.Attributes.Add("value", sEMAIL_PASSWORD);
						}

						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *                    " + ControlChars.CrLf
								     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
								     + " where ID = @ID             " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
											// It is possible that the record exists, but that ACL rules prevent it from being selected. 
											gID = Guid.Empty;
										}
									}
								}
							}
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									// 04/12/2011 Paul.  Stop forcing the mailbox to be INBOX. 
									// 04/19/2011 Paul.  Add IS_PERSONAL to exclude EmailClient inbound from being included in monitored list. 
									// 01/23/2013 Paul.  Add REPLY_TO_NAME and REPLY_TO_ADDR. 
									SqlProcs.spINBOUND_EMAILS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "NAME"          ).Text
										, new DynamicControl(this, rowCurrent, "STATUS"        ).SelectedValue
										, new DynamicControl(this, rowCurrent, "SERVER_URL"    ).Text
										, new DynamicControl(this, rowCurrent, "EMAIL_USER"    ).Text
										, sEMAIL_PASSWORD
										, new DynamicControl(this, rowCurrent, "PORT"          ).IntegerValue
										, new DynamicControl(this, rowCurrent, "MAILBOX_SSL"   ).Checked
										, new DynamicControl(this, rowCurrent, "SERVICE"       ).SelectedValue
										, new DynamicControl(this, rowCurrent, "MAILBOX"       ).Text
										, new DynamicControl(this, rowCurrent, "MARK_READ"     ).Checked
										, new DynamicControl(this, rowCurrent, "ONLY_SINCE"    ).Checked
										, new DynamicControl(this, rowCurrent, "MAILBOX_TYPE"  ).SelectedValue
										, new DynamicControl(this, rowCurrent, "TEMPLATE_ID"   ).ID
										, new DynamicControl(this, rowCurrent, "GROUP_ID"      ).ID
										, new DynamicControl(this, rowCurrent, "FROM_NAME"     ).Text
										, new DynamicControl(this, rowCurrent, "FROM_ADDR"     ).Text
										, new DynamicControl(this, rowCurrent, "FILTER_DOMAIN" ).Text
										, new DynamicControl(this, rowCurrent, "IS_PERSONAL"   ).Checked
										, new DynamicControl(this, rowCurrent, "REPLY_TO_NAME" ).Text
										, new DynamicControl(this, rowCurrent, "REPLY_TO_ADDR" ).Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
						}
						SplendidCache.ClearEmailGroups();
						SplendidCache.ClearInboundEmails();
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Test" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );
					this.ValidateEditViewFields(m_sMODULE + ".EditOptions");

					if ( Page.IsValid )
					{
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
						string sEMAIL_PASSWORD = sEMPTY_PASSWORD;
						TextBox EMAIL_PASSWORD = FindControl("EMAIL_PASSWORD") as TextBox;
						if ( EMAIL_PASSWORD != null )
							sEMAIL_PASSWORD = EMAIL_PASSWORD.Text;
						// 07/08/2010 Paul.  We want to save the password for later use. 
						if ( sEMAIL_PASSWORD == sEMPTY_PASSWORD )
						{
							sEMAIL_PASSWORD = Sql.ToString(ViewState["smtppass"]);
							if ( !Sql.IsEmptyString(sEMAIL_PASSWORD) )
								sEMAIL_PASSWORD = Security.DecryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sEMAIL_PASSWORD )
								throw(new Exception("Decryption failed"));
							if ( EMAIL_PASSWORD != null )
								EMAIL_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						string sSERVICE     = new DynamicControl(this, "SERVICE"       ).SelectedValue;
						string sSERVER_URL  = new DynamicControl(this, "SERVER_URL"    ).Text;
						int    nPORT        = new DynamicControl(this, "PORT"          ).IntegerValue;
						bool   bMAILBOX_SSL = new DynamicControl(this, "MAILBOX_SSL"   ).Checked;
						string sEMAIL_USER  = new DynamicControl(this, "EMAIL_USER"    ).Text;
						string sMAILBOX     = new DynamicControl(this, "MAILBOX"       ).Text;
						StringBuilder sbErrors = new StringBuilder();
						if ( String.Compare(sSERVICE, "pop3", true) == 0 )
						{
							PopUtils.Validate(Context, sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD, sbErrors);
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
						}
						else if ( String.Compare(sSERVICE, "imap", true) == 0 )
						{
							ImapUtils.Validate(Context, sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD, sMAILBOX, sbErrors);
							ctlDynamicButtons.ErrorText = sbErrors.ToString();
						}
						else
						{
							throw(new Exception("Unknown/unsupported mail service: " + sSERVICE));
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
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
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                    " + ControlChars.CrLf
							     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
							     + " where ID = @ID             " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView   , tblMain   , rdr);
											this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, rdr);
											// 01/08/2008 Paul.  Don't display the password. 
											// 01/08/2008 Paul.  Browsers don't display passwords. 
											// 07/08/2010 Paul.  We want to save the password for later use. 
											string sEMAIL_PASSWORD = Sql.ToString(rdr["EMAIL_PASSWORD"]);
											if ( !Sql.IsEmptyString(sEMAIL_PASSWORD) )
											{
												ViewState["smtppass"] = sEMAIL_PASSWORD;
												TextBox EMAIL_PASSWORD = FindControl("EMAIL_PASSWORD") as TextBox;
												if ( EMAIL_PASSWORD != null )
												{
													//txtEMAIL_PASSWORD.Text = sEMPTY_PASSWORD;
													EMAIL_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
												}
											}
											// 04/19/2011 Paul.  A personal email can only have two types, None and Bounce. 
											DropDownList GROUP_ID = FindControl("GROUP_ID") as DropDownList;
											if ( Sql.ToBoolean(rdr["IS_PERSONAL"]) )
											{
												DropDownList MAILBOX_TYPE = FindControl("MAILBOX_TYPE") as DropDownList;
												if ( MAILBOX_TYPE != null )
												{
													MAILBOX_TYPE.Items.Clear();
													MAILBOX_TYPE.Items.Add(new ListItem(L10n.Term(".LBL_NONE"), ""));
													MAILBOX_TYPE.Items.Add(new ListItem(Sql.ToString(L10n.Term(".dom_mailbox_type.", "bounce")), "bounce"));
													try
													{
														Utils.SetSelectedValue(MAILBOX_TYPE, Sql.ToString(rdr["MAILBOX_TYPE"]));
													}
													catch
													{
													}
												}
												if ( GROUP_ID != null )
												{
													GROUP_ID.Visible = false;
												}
												Literal GROUP_ID_LABEL = FindControl("GROUP_ID_LABEL") as Literal;
												if ( GROUP_ID_LABEL != null )
												{
													GROUP_ID_LABEL.Visible = false;
													// 04/19/2011 Paul.  We also want to hide the required field indicator. 
													if ( GROUP_ID_LABEL.Parent.Controls.Count == 2 )
													{
														GROUP_ID_LABEL.Parent.Controls[0].Visible = false;
														GROUP_ID_LABEL.Parent.Controls[1].Visible = false;
													}
												}
											}
											else
											{
												if ( GROUP_ID != null )
												{
													GROUP_ID.Items.Insert(0, new ListItem(L10n.Term("InboundEmail.LBL_CREATE_NEW_GROUP"), ""));
												}
											}
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView   , tblMain   , null);
						this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, null);

						// 03/18/2008 Paul.  The default value of bounce should only apply to new record. 
						DropDownList MAILBOX_TYPE = FindControl("MAILBOX_TYPE") as DropDownList;
						if ( MAILBOX_TYPE != null )
						{
							try
							{
								// 08/19/2010 Paul.  Check the list before assigning the value. 
								Utils.SetSelectedValue(MAILBOX_TYPE, "bounce");
							}
							catch
							{
							}
						}
						DropDownList GROUP_ID = FindControl("GROUP_ID") as DropDownList;
						if ( GROUP_ID != null )
						{
							GROUP_ID.Items.Insert(0, new ListItem(L10n.Term("InboundEmail.LBL_CREATE_NEW_GROUP"), ""));
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "InboundEmail";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView   , tblMain   , null);
				this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}


