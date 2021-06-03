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

namespace SplendidCRM._controls
{
	/// <summary>
	/// Summary description for Preview.
	/// </summary>
	public class Preview : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;

		protected HiddenField PREVIEW_ID       ;
		protected Guid        gID              ;
		protected HtmlTable   tblMain          ;

		public string Module
		{
			get { return m_sMODULE;  }
			set { m_sMODULE = value; }
		}

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

		public void LoadPreview(Guid gID)
		{
			try
			{
				if ( !Sql.IsEmptyGuid(gID) )
				{
					string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						string sSQL ;
						sSQL = "select *                         " + ControlChars.CrLf
							    + "  from vw" + sTABLE_NAME + "_Edit" + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, m_sMODULE, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);
							con.Open();

							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dtCurrent = new DataTable() )
								{
									da.Fill(dtCurrent);
									if ( dtCurrent.Rows.Count > 0 )
									{
										DataRow rdr = dtCurrent.Rows[0];
										this.ApplyDetailViewPreLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
										
										// 03/09/2014 Paul.  We need to make the table visible as it might remember the previous visibility state. 
										tblMain.Visible = true;
										tblMain.Rows.Clear();
										this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, rdr);
										ctlDynamicButtons.Visible = true;
										ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Sql.ToGuid(rdr["ASSIGNED_USER_ID"]), rdr);
										this.ApplyDetailViewPostLoadEventRules(m_sMODULE + "." + LayoutDetailView, rdr);
									}
									else
									{
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
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
					ctlDynamicButtons.DisableAll();
					this.Visible = false;
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
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			if ( this.IsTrackingViewState )
				gID = Sql.ToGuid(PREVIEW_ID.Value);
			else
				gID = Sql.ToGuid(Request[PREVIEW_ID.UniqueID]);
			LoadPreview(gID);
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
			if ( IsPostBack )
			{
				this.AppendDetailViewFields(m_sMODULE + "." + LayoutDetailView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutDetailView, Guid.Empty, null);
			}
		}
		#endregion
	}
}


