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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Calls
{
	/// <summary>
	///		Summary description for Leads.
	/// </summary>
	public class Leads : SubPanelControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView    ;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected HtmlInputHidden txtLEAD_ID     ;
		protected Button          btnCreateInline   ;
		protected Panel           pnlNewRecordInline;
		protected SplendidCRM.Leads.NewRecord ctlNewRecord   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					//case "Leads.Create":
					//	Response.Redirect("~/Leads/edit.aspx?CALL_ID=" + gID.ToString());
					//	break;
					case "Leads.Edit":
					{
						Guid gLEAD_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Leads/edit.aspx?ID=" + gLEAD_ID.ToString());
						break;
					}
					case "Leads.Remove":
					{
						Guid gLEAD_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spCALLS_LEADS_Delete(gID, gLEAD_ID);
						BindGrid();
						break;
					}
					case "Leads.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/" + m_sMODULE + "/edit.aspx?CALL_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtons.HideAll();
						}
						break;
					case "NewRecord.Cancel":
						pnlNewRecordInline.Style.Add(HtmlTextWriterStyle.Display, "none");
						ctlDynamicButtons.ShowAll();
						break;
					case "NewRecord.FullForm":
						Response.Redirect("~/" + m_sMODULE + "/edit.aspx?CALL_ID=" + gID.ToString());
						break;
					case "NewRecord":
						Response.Redirect(Request.RawUrl);
						break;
					case "Leads.Search":
						ctlSearchView.Visible = !ctlSearchView.Visible;
						break;
					case "Search":
						break;
					case "Clear":
						BindGrid();
						break;
					case "SortGrid":
						break;
					default:
						throw(new Exception("Unknown command: " + e.CommandName));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}
		}

		protected void BindGrid()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from vwCALLS_LEADS" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Security.Filter(cmd, m_sMODULE, "list");
					Sql.AppendParameter(cmd, gID, "CALL_ID");
					ctlSearchView.SqlSearchClause(cmd);
					// 10/14/2012 Paul.  Change default sort to Lead Name. 
					cmd.CommandText += grdMain.OrderByClause("LEAD_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwCALLS_LEADS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules("Calls." + m_sMODULE, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								grdMain.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtons.ErrorText = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			Guid gLEAD_ID = Sql.ToGuid(txtLEAD_ID.Value);
			if ( !Sql.IsEmptyGuid(gLEAD_ID) )
			{
				try
				{
					SqlProcs.spCALLS_LEADS_Update(gID, gLEAD_ID, false, String.Empty);
					txtLEAD_ID.Value = String.Empty;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			BindGrid();

			if ( !IsPostBack )
			{
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtons.AppendButtons("Calls." + m_sMODULE, gASSIGNED_USER_ID, gID);
				ctlNewRecord.CALL_ID = gID;
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
			ctlNewRecord.Command      += new CommandEventHandler(Page_Command);
			ctlSearchView.Command     += new CommandEventHandler(Page_Command);
			m_sMODULE = "Leads";
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED"         );
			arrSelectFields.Add("LEAD_ID"              );
			arrSelectFields.Add("ASSIGNED_USER_ID"     );
			arrSelectFields.Add("CALL_ASSIGNED_USER_ID");
			this.AppendGridColumns(grdMain, "Calls." + m_sMODULE, arrSelectFields);
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Calls." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

