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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Emails
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.ModuleHeader   ctlModuleHeader ;
		protected _controls.DynamicButtons ctlDynamicButtons;

		protected Guid        gID              ;
		protected HtmlTable   tblMain          ;
		protected PlaceHolder plcSubPanel      ;
		protected Repeater    ctlAttachments   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spEMAILS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				// 11/28/2005 Paul.  We must always populate the table, otherwise it will disappear during event processing. 
				// 03/19/2008 Paul.  Place AppendDetailViewFields inside OnInit to avoid having to re-populate the data. 
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *            " + ControlChars.CrLf
							     + "  from vwEMAILS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "view");
								Sql.AppendParameter(cmd, gID, "ID", false);
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
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyDetailViewPreLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
											
											ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
											
											// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
											this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											string sDESCRIPTION = Sql.ToString(rdr["DESCRIPTION"]);
											// 12/03/2008 Paul.  The plain-text description may not contain anything.  If HTML exists, then always use it. 
											string sDESCRIPTION_HTML = Sql.ToString(rdr["DESCRIPTION_HTML"]);
											if ( !Sql.IsEmptyString(sDESCRIPTION_HTML) )
												sDESCRIPTION = sDESCRIPTION_HTML;

											// 05/23/2010 Paul.  We only need to do the replacements if this the message is not HTML. 
											// 05/23/2010 Paul.  XssFilter will remove <html>, so we have to check first. 
											if ( !(sDESCRIPTION.IndexOf("<html", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<body", StringComparison.CurrentCultureIgnoreCase) >= 0 || sDESCRIPTION.IndexOf("<br", StringComparison.CurrentCultureIgnoreCase) >= 0) )
											{
												// 01/20/2008 Paul.  There is probably a regular expression filter that would do the following replacement better. 
												// 06/04/2010 Paul.  Try and prevent excess blank lines. 
												sDESCRIPTION = EmailUtils.NormalizeDescription(sDESCRIPTION);
											}
											sDESCRIPTION = EmailUtils.XssFilter(sDESCRIPTION, Sql.ToString(Application["CONFIG.email_xss"]));
											new DynamicControl(this, "DESCRIPTION").Text = sDESCRIPTION;
											
											// 05/23/2010 Paul.  Lets also filter the subject. 
											string sSUBJECT = Sql.ToString(rdr["NAME"]);
											sSUBJECT = EmailUtils.XssFilter(sSUBJECT, Sql.ToString(Application["CONFIG.email_xss"]));
											new DynamicControl(this, "NAME").Text = sSUBJECT;
											
											// 11/17/2005 Paul.  Archived emails allow editing of the Date & Time Sent. 
											string sEMAIL_TYPE = Sql.ToString(rdr["TYPE"]).ToLower();
											ctlModuleHeader.EnableModuleLabel = false;
											switch ( sEMAIL_TYPE )
											{
												case "archived":
													// 09/26/2013 Paul.  Format the header as a link. 
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_ARCHIVED_MODULE_NAME") + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													// 04/12/2011 Paul.  Should be able to forward an Archived email. 
													Response.Redirect("inbound.aspx?ID=" + gID.ToString());
													break;
												case "inbound":
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_INBOUND_TITLE") + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													Response.Redirect("inbound.aspx?ID=" + gID.ToString());
													break;
												case "out":
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													// 01/21/2006 Paul.  Sent emails cannot be edited or duplicated. 
													// 12/20/2006 Paul.  Messages have type "out" when they are ready to send. 
													ctlDynamicButtons.ShowButton("Edit"     , false);
													ctlDynamicButtons.ShowButton("Duplicate", false);
													break;
												case "sent":
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													// 12/20/2006 Paul.  Sent emails cannot be edited or duplicated. 
													ctlDynamicButtons.ShowButton("Edit"     , false);
													ctlDynamicButtons.ShowButton("Duplicate", false);
													// 04/12/2011 Paul.  Should be able to forward an Sent email. 
													Response.Redirect("inbound.aspx?ID=" + gID.ToString());
													break;
												case "campaign":
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													// 12/20/2006 Paul.  Sent emails cannot be edited or duplicated. 
													ctlDynamicButtons.ShowButton("Edit"     , false);
													ctlDynamicButtons.ShowButton("Duplicate", false);
													break;
												default:
													sEMAIL_TYPE = "draft";
													ctlModuleHeader.Title = "<a href=\"default.aspx\">" + L10n.Term("Emails.LBL_COMPOSE_MODULE_NAME" ) + "</a><span class=\"pointer\">&raquo;</span>" + ctlModuleHeader.Title;
													ctlDynamicButtons.ShowButton("Duplicate", false);
													// 01/21/2006 Paul.  Draft messages go directly to edit mode. 
													Response.Redirect("edit.aspx?ID=" + gID.ToString());
													break;
											}
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 04/28/2008 Paul.  We will need the ASSIGNED_USER_ID in the sub-panels. 
											Page.Items["ASSIGNED_USER_ID"] = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyDetailViewPostLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't show its sub panel either. 
											plcSubPanel.Visible = false;
											
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
							sSQL = "select *                   " + ControlChars.CrLf
							     + "  from vwEMAILS_Attachments" + ControlChars.CrLf
							     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwEMAILS_Attachments", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										ctlAttachments.DataSource = dt.DefaultView;
										ctlAttachments.DataBind();
									}
								}
							}
						}
					}
					else
					{
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						ctlDynamicButtons.DisableAll();
						//ctlDynamicButtons.ErrorText = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + "ID";
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Emails";
			// 02/13/2007 Paul.  Emails should highlight the Activities menu. 
			// 05/26/2007 Paul.  We are display the emails tab, so we must highlight the tab. 
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
				this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

