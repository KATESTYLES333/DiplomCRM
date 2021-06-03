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

namespace SplendidCRM.Contacts
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.ModuleHeader  ctlModuleHeader ;
		protected _controls.DynamicButtons ctlDynamicButtons;

		protected Guid        gID              ;
		protected HtmlTable   tblMain          ;
		protected PlaceHolder plcSubPanel;

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
					SqlProcs.spCONTACTS_Delete(gID);
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
							// 02/09/2006 Paul.  SugarCRM uses the CONTACTS_USERS table to allow each user to 
							// choose the contacts they want sync'd with Outlook. 
							// 02/09/2006 Paul.  Need to allow SYNC_USER_ID to be NULL, 
							// otherwise we will not get any results if the contact is not sync'd. 
							// 03/06/2006 Paul.  The join to CONTACTS_USERS must occur external to the view. 
							// This is the only way to ensure that the record is always returned, with the sync flag set. 
							// 04/20/2006 Paul.  Use vwCONTACTS_USERS to prevent an access denied error for non-admin connections. 
							// 04/23/2006 Paul.  Bug fix.  vwCONTACTS_USERS does not have an ID, use CONTACT_ID instead. 
							sSQL = "select vwCONTACTS_Edit.*                                                        " + ControlChars.CrLf
							     + "     , (case when vwCONTACTS_USERS.CONTACT_ID is null then 0 else 1 end) as SYNC_CONTACT" + ControlChars.CrLf
							     + "  from            vwCONTACTS_Edit                                               " + ControlChars.CrLf
							     + "  left outer join vwCONTACTS_USERS                                              " + ControlChars.CrLf
							     + "               on vwCONTACTS_USERS.CONTACT_ID = vwCONTACTS_Edit.ID              " + ControlChars.CrLf
							     + "              and vwCONTACTS_USERS.USER_ID    = @SYNC_USER_ID                   " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@SYNC_USER_ID", Security.USER_ID);
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "view");
								cmd.CommandText += "   and vwCONTACTS_Edit.ID = @ID" + ControlChars.CrLf;
								Sql.AddParameter(cmd, "@ID"          , gID             );
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
											
											// 10/20/2010 Paul.  Salutation needed to be translated.  Salutation may be empty. 
											string sSALUTATION = Sql.ToString(rdr["SALUTATION"]);
											ctlModuleHeader.Title = (!Sql.IsEmptyString(sSALUTATION) ? L10n.Term(".salutation_dom." + sSALUTATION) + " " : String.Empty) + Sql.ToString(rdr["FIRST_NAME"]) + " " + Sql.ToString(rdr["LAST_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
											
											// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
											this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr, new CommandEventHandler(Page_Command));
											// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 04/28/2008 Paul.  We will need the ASSIGNED_USER_ID in the sub-panels. 
											Page.Items["ASSIGNED_USER_ID"] = Sql.ToGuid(rdr["ASSIGNED_USER_ID"]);
											// 09/08/2010 Paul.  Prefill the Account for the Opportunity. 
											Page.Items["ACCOUNT_ID"      ] = Sql.ToGuid(rdr["ACCOUNT_ID"      ]);
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
			m_sMODULE = "Contacts";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 02/13/2013 Paul.  Move relationship append so that it can be controlled by business rules. 
				this.AppendDetailViewRelationships(m_sMODULE + "." + LayoutDetailView, plcSubPanel);
				// 12/11/2008 Paul.  Add the event handler to the detail view generator. 
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null, new CommandEventHandler(Page_Command));
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}


