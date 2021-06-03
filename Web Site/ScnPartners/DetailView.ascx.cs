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

namespace SplendidCRM.ScnPartners
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlDynamicButtons;

		protected Guid          gID              ;
		protected HtmlTable     tblMain          ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Edit" )
				{
					Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spACCOUNTS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
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
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select " + m_sVIEW_NAME + ".*"                                           + ControlChars.CrLf
							     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "view", m_sVIEW_NAME)
							     + "  from            " + m_sVIEW_NAME                                       + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
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
											// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
											ctlDynamicButtons.Title = Sql.ToString(rdr["MODULE_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlDynamicButtons.Title);
											
											this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
											
											Page.Items["ASSIGNED_USER_ID"] = Guid.Empty;
											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);

											ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, rdr);


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
			m_sMODULE = "ScnPartners";
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


