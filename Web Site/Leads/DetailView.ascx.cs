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
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Leads
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		protected Guid          gID                    ;
		protected HtmlTable     tblMain                ;
		protected PlaceHolder   plcSubPanel            ;
		protected HtmlTable     tblConverted           ;
		protected HtmlTableCell tdConvertedContact     ;
		protected HtmlTableCell tdConvertedAccount     ;
		protected HtmlTableCell tdConvertedOpportunity ;
		protected HyperLink     lnkConvertedContact    ;
		protected HyperLink     lnkConvertedAccount    ;
		protected HyperLink     lnkConvertedOpportunity;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Convert" )
				{
					Response.Redirect("convert.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spLEADS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
				// 10/10/2017 Paul.  Add Archive access right. 
				else if ( e.CommandName == "Archive.MoveData" )
				{
					ArchiveUtils archive = new ArchiveUtils(Context);
					ctlDynamicButtons.ErrorText = archive.MoveData(m_sMODULE, gID);
					if ( Sql.IsEmptyString(ctlDynamicButtons.ErrorText) )
						Response.Redirect("view.aspx?ID=" + gID.ToString() + "&ArchiveView=1");
				}
				else if ( e.CommandName == "Archive.RecoverData" )
				{
					ArchiveUtils archive = new ArchiveUtils(Context);
					ctlDynamicButtons.ErrorText = archive.RecoverData(m_sMODULE, gID);
					if ( Sql.IsEmptyString(ctlDynamicButtons.ErrorText) )
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
				else
				{
					throw(new Exception("Unknown command: " + e.CommandName));
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
							// 04/03/2010 Paul.  Add EXCHANGE_FOLDER. 
							// 10/08/2017 Paul.  Add Archive access right. 
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							sSQL = "select " + m_sVIEW_NAME + ".*"                                     + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "view", m_sVIEW_NAME)
							     + "     , (case when vwLEADS_USERS.LEAD_ID is null then 0 else 1 end) as EXCHANGE_FOLDER" + ControlChars.CrLf
							     + "  from            " + m_sVIEW_NAME                                 + ControlChars.CrLf
							     + "  left outer join vwLEADS_USERS                                  " + ControlChars.CrLf
							     + "               on vwLEADS_USERS.LEAD_ID = " + m_sVIEW_NAME + ".ID" + ControlChars.CrLf
							     + "              and vwLEADS_USERS.USER_ID = @SYNC_USER_ID          " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@SYNC_USER_ID", Security.USER_ID);
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								// 10/08/2017 Paul.  Add Archive access right. 
								Security.Filter(cmd, m_sMODULE, (ArchiveViewExists() ? "archive" : "view"));
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
										// 10/31/2017 Paul.  Provide a way to inject Record level ACL. 
										if ( dtCurrent.Rows.Count > 0 && (SplendidCRM.Security.GetRecordAccess(dtCurrent.Rows[0], m_sMODULE, "view", "ASSIGNED_USER_ID") >= 0) )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyDetailViewPreLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
											
											// 10/20/2010 Paul.  Salutation needed to be translated.  Salutation may be empty. 
											string sSALUTATION = Sql.ToString(rdr["SALUTATION"]);
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = (!Sql.IsEmptyString(sSALUTATION) ? L10n.Term(".salutation_dom." + sSALUTATION) + " " : String.Empty) + Sql.ToString(rdr["FIRST_NAME"]) + " " + Sql.ToString(rdr["LAST_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											// 10/10/2017 Paul.  Don't update tracker in ArchiveView. 
											if ( !ArchiveView() )
												Utils.UpdateTracker(Page, m_sMODULE, gID, ctlDynamicButtons.Title);
											ViewState["ctlDynamicButtons.Title"] = ctlDynamicButtons.Title;
											
											// 01/31/2006 Paul.  Provide links to converted records. 
											// 04/24/2006 Paul.  Fix links.  The ID was being displayed instead of the name. 
											if ( rdr["CONTACT_ID"] != DBNull.Value && rdr["CONVERTED_CONTACT_NAME"] != DBNull.Value )
											{
												// 07/27/2006 Paul.  Field should be CONVERTED_CONTACT_NAME. 
												lnkConvertedContact.Text        = Sql.ToString(rdr["CONVERTED_CONTACT_NAME"]);
												// 07/27/2006 Paul.  Field should be CONTACT_ID. 
												lnkConvertedContact.NavigateUrl = "~/Contacts/view.aspx?ID=" + Sql.ToString(rdr["CONTACT_ID"]);
											}
											if ( rdr["ACCOUNT_ID"] != DBNull.Value && rdr["CONVERTED_ACCOUNT_NAME"] != DBNull.Value )
											{
												// 07/27/2006 Paul.  Field should be CONVERTED_ACCOUNT_NAME. 
												lnkConvertedAccount.Text        = Sql.ToString(rdr["CONVERTED_ACCOUNT_NAME"]);
												// 07/27/2006 Paul.  Field should be ACCOUNT_ID. 
												lnkConvertedAccount.NavigateUrl = "~/Accounts/view.aspx?ID=" + Sql.ToString(rdr["ACCOUNT_ID"]);
											}
											// 08/08/2015 Paul.  Separate relationship for Leads/Opportunities. 
											if ( rdr["CONVERTED_OPPORTUNITY_ID"] != DBNull.Value && rdr["CONVERTED_OPPORTUNITY_NAME"] != DBNull.Value )
											{
												lnkConvertedOpportunity.Text        = Sql.ToString(rdr["CONVERTED_OPPORTUNITY_NAME"]);
												lnkConvertedOpportunity.NavigateUrl = "~/Opportunities/view.aspx?ID=" + Sql.ToString(rdr["CONVERTED_OPPORTUNITY_ID"]);
											}
											bool bValidContact     = !Sql.IsEmptyString(lnkConvertedContact    .NavigateUrl);
											bool bValidAccount     = !Sql.IsEmptyString(lnkConvertedAccount    .NavigateUrl);
											bool bValidOpportunity = !Sql.IsEmptyString(lnkConvertedOpportunity.NavigateUrl);
											tdConvertedContact     .Visible = bValidContact    ;
											tdConvertedAccount     .Visible = bValidAccount    ;
											tdConvertedOpportunity .Visible = bValidOpportunity;
											// 04/24/2006 Paul.  Can't use tdConvertedContact.Visible to determine if we should show the table 
											// because an invisible table will force tdConvertedLead.Visible to always be false. 
											tblConverted           .Visible = bValidContact || bValidAccount || bValidOpportunity;
											
											// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
											this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 04/28/2008 Paul.  We will need the ASSIGNED_USER_ID in the sub-panels. 
											Page.Items["ASSIGNED_USER_ID"] = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
											// 10/10/2017 Paul.  Add Archive access right. 
											int nACLACCESS_Archive = Security.GetUserAccess(m_sMODULE, "archive");
											ctlDynamicButtons.ShowButton("Archive.MoveData"   , (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) && !ArchiveView() && ArchiveEnabled());
											ctlDynamicButtons.ShowButton("Archive.RecoverData", (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) &&  ArchiveView() && ArchiveEnabled());
											// 10/31/2017 Paul.  Provide a way to inject Record level ACL. 
											ctlDynamicButtons.ShowButton("Duplicate", (SplendidCRM.Security.GetRecordAccess(rdr, m_sMODULE, "edit"  , "ASSIGNED_USER_ID") >= 0) && ctlDynamicButtons.IsButtonVisible("Duplicate"));
											ctlDynamicButtons.ShowButton("Edit"     , (SplendidCRM.Security.GetRecordAccess(rdr, m_sMODULE, "edit"  , "ASSIGNED_USER_ID") >= 0) && ctlDynamicButtons.IsButtonVisible("Edit"     ));
											ctlDynamicButtons.ShowButton("Delete"   , (SplendidCRM.Security.GetRecordAccess(rdr, m_sMODULE, "delete", "ASSIGNED_USER_ID") >= 0) && ctlDynamicButtons.IsButtonVisible("Delete"   ));
											// 08/06/2015 paul.  Separate method to include process buttons so that we don't do a process query for all the non-detailview buttons. 
											ctlDynamicButtons.AppendProcessButtons(rdr);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyDetailViewPostLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
										}
										else
										{
											// 11/25/2006 Paul.  If item is not visible, then don't show its sub panel either. 
											plcSubPanel.Visible = false;
											
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
											// 08/06/2015 paul.  Separate method to include process buttons so that we don't do a process query for all the non-detailview buttons. 
											ctlDynamicButtons.AppendProcessButtons(null);
											ctlDynamicButtons.DisableAll();
											ctlDynamicButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
										}
									}
								}
							}
						}
					}
					else
					{
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
						// 08/06/2015 paul.  Separate method to include process buttons so that we don't do a process query for all the non-detailview buttons. 
						ctlDynamicButtons.AppendProcessButtons(null);
						ctlDynamicButtons.DisableAll();
						//ctlDynamicButtons.ErrorText = L10n.Term(".ERR_MISSING_REQUIRED_FIELDS") + "ID";
					}
				}
				else
				{
					// 06/07/2015 Paul.  Seven theme DetailView.master uses an UpdatePanel, so we need to recall the title. 
					ctlDynamicButtons.Title = Sql.ToString(ViewState["ctlDynamicButtons.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
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
			m_sMODULE = "Leads";
			// 10/08/2017 Paul.  Add Archive access right. 
			string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
			m_sVIEW_NAME = "vw" + sTABLE_NAME;
			if ( ArchiveViewExists() )
				m_sVIEW_NAME += "_ARCHIVE";
			else
				m_sVIEW_NAME += "_Edit";
			this.LayoutDetailView = (ArchiveViewExists() ? "ArchiveView" : "DetailView");
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
				this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
				// 08/06/2015 paul.  Separate method to include process buttons so that we don't do a process query for all the non-detailview buttons. 
				ctlDynamicButtons.AppendProcessButtons(null);
			}
		}
		#endregion
	}
}


