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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Accounts
{
	/// <summary>
	///		Summary description for Bugs.
	/// </summary>
	public class Bugs : SubPanelControl
	{
		// 06/03/2015 Paul.  Combine ListHeader and DynamicButtons. 
		protected _controls.SubPanelButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView    ;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected HtmlInputHidden txtBUG_ID      ;
		// 02/21/2010 Paul.  Controls to manage inline create. 
		protected Button          btnCreateInline   ;
		protected Panel           pnlNewRecordInline;
		// 02/21/2010 Paul.  Make sure to point to the Contacts.NewRecord. 
		protected SplendidCRM.Bugs.NewRecord ctlNewRecord   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					//case "Bugs.Create":
					//	Response.Redirect("~/Bugs/edit.aspx?PARENT_ID=" + gID.ToString());
					//	break;
					case "Bugs.Edit":
					{
						Guid gBUG_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Bugs/edit.aspx?ID=" + gBUG_ID.ToString());
						break;
					}
					case "Bugs.Remove":
					{
						Guid gBUG_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gBUG_ID);
						}
						else
						{
							SqlProcs.spACCOUNTS_BUGS_Delete(gID, gBUG_ID);
						}
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					// 02/21/2010 Paul.  Handle new events that hide and show the NewRecord panel. 
					//case "NewRecord.Show":
					case "Bugs.Create":
						// 02/21/2010 Paul.  We are not going to allow inline create on a mobile device. 
						// 02/22/2010 Paul.  We should have a way to turn-off inline editing as it is a performance issue. 
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gID.ToString());
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
						Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "NewRecord":
						//BindGrid();
						// 02/21/2010 Paul.  Redirect instead of rebind so that the NewRecord form will get cleared. 
						Response.Redirect(Request.RawUrl);
						break;
					// 06/20/2010 Paul.  Add support for SearchView events. Need to rebind inside the clear event. 
					case "Bugs.Search":
						ctlSearchView.Visible = !ctlSearchView.Visible;
						break;
					case "Search":
						break;
					case "Clear":
						BindGrid();
						break;
					case "SortGrid":
						break;
					// 06/07/2015 Paul.  Add support for Preview button. 
					case "Preview":
						if ( Page.Master is SplendidMaster )
						{
							CommandEventArgs ePreview = new CommandEventArgs(e.CommandName, new PreviewData(m_sMODULE, Sql.ToGuid(e.CommandArgument)));
							(Page.Master as SplendidMaster).Page_Command(sender, ePreview);
						}
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
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
					if ( bEditView && IsPostBack && arrUPDATED.Count > 0 )
					{
						arrSelectFields.Remove("BUG_ID"                  );
						arrSelectFields.Remove("BUG_NAME"                );
						arrSelectFields.Remove("ACCOUNT_ID"              );
						arrSelectFields.Remove("ACCOUNT_NAME"            );
						arrSelectFields.Remove("ACCOUNT_ASSIGNED_USER_ID");
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                        as BUG_ID                  " + ControlChars.CrLf
						     + "     , NAME                      as BUG_NAME                " + ControlChars.CrLf
						     + "     , @ACCOUNT_ID               as ACCOUNT_ID              " + ControlChars.CrLf
						     + "     , @ACCOUNT_NAME             as ACCOUNT_NAME            " + ControlChars.CrLf
						     + "     , @ACCOUNT_ASSIGNED_USER_ID as ACCOUNT_ASSIGNED_USER_ID" + ControlChars.CrLf
						     + "  from vwBUGS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@ACCOUNT_ID"              , gID);
						Sql.AddParameter(cmd, "@ACCOUNT_NAME"            , Sql.ToString(Page.Items["NAME"            ]));
						Sql.AddParameter(cmd, "@ACCOUNT_ASSIGNED_USER_ID", Sql.ToGuid  (Page.Items["ASSIGNED_USER_ID"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
						// 10/05/2017 Paul.  Add Archive relationship view. 
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
						     + "  from " + m_sVIEW_NAME + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
						// 10/08/2017 Paul.  Add Archive access right. 
						Security.Filter(cmd, m_sMODULE, (ArchiveViewEnabled() ? "archive" : "list"));
						Sql.AppendParameter(cmd, gID, "ACCOUNT_ID");
					}
					// 06/20/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchView.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdMain.OrderByClause("DATE_ENTERED", "desc");

					if ( bDebug )
						RegisterClientScriptBlock("vwACCOUNTS_BUGS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								// 10/05/2017 Paul.  Add Archive relationship view. 
								this.ApplyGridViewRules("Accounts." + this.LayoutListView, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Enable sorting of sub panel. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								// 05/16/2008 Paul.  We must always rebind otherwise a postback from an alternate subpanel will cause this grid be incomplete.
								grdMain.DataBind();
								// 01/27/2010 Paul.  In EditView mode, we need a list of existing relationships. 
								if ( bEditView && !IsPostBack )
								{
									this.CreateEditViewRelationships(dt, "BUG_ID");
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
			}
		}

		// 01/27/2010 Paul.  This method is only calld when in EditMode. 
		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			UniqueGuidCollection arrDELETED = this.GetDeletedEditViewRelationships();
			foreach ( Guid gDELETE_ID in arrDELETED )
			{
				if ( !Sql.IsEmptyGuid(gDELETE_ID) )
					SqlProcs.spACCOUNTS_BUGS_Delete(gPARENT_ID, gDELETE_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spACCOUNTS_BUGS_Update(gPARENT_ID, gUPDATE_ID, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			Guid gBUG_ID = Sql.ToGuid(txtBUG_ID.Value);
			if ( !Sql.IsEmptyGuid(gBUG_ID) )
			{
				try
				{
					if ( bEditView )
					{
						this.UpdateEditViewRelationship(gBUG_ID);
					}
					else
					{
						SqlProcs.spACCOUNTS_BUGS_Update(gID, gBUG_ID);
					}
					// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
					//Response.Redirect("view.aspx?ID=" + gID.ToString());
					// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
					txtBUG_ID.Value = String.Empty;
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			try
			{
				BindGrid();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons.ErrorText = ex.Message;
			}

			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				// 04/28/2008 Paul.  Make use of dynamic buttons. 
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				// 10/05/2017 Paul.  Add Archive relationship view. 
				ctlDynamicButtons.AppendButtons("Accounts." + this.LayoutListView, gASSIGNED_USER_ID, gID);
				// 02/21/2010 Paul.  The parent needs to be initialized when the page first loads. 
				ctlNewRecord.PARENT_ID    = gID;
				ctlNewRecord.PARENT_TYPE = "Accounts";
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
			// 06/20/2010 Paul.  We need to connect the SearchView command handler, otherwise it will throw an exception. 
			ctlSearchView.Command     += new CommandEventHandler(Page_Command);
			m_sMODULE = "Bugs";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED"            );
			arrSelectFields.Add("BUG_ID"                  );
			arrSelectFields.Add("ASSIGNED_USER_ID"        );
			arrSelectFields.Add("ACCOUNT_ASSIGNED_USER_ID");
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			// 06/07/2015 Paul.  Must include Page_Command in order for Preview to fire. 
			// 10/05/2017 Paul.  Add Archive relationship view. 
			m_sVIEW_NAME = "vwACCOUNTS_BUGS";
			if ( ArchiveViewExists() )
				m_sVIEW_NAME = m_sVIEW_NAME + "_ARCHIVE";
			this.LayoutListView = m_sMODULE + (ArchiveView() ? ".ArchiveView" : String.Empty);
			this.AppendGridColumns(grdMain, "Accounts." + this.LayoutListView, arrSelectFields, Page_Command);
			// 04/28/2008 Paul.  Make use of dynamic buttons. 
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Accounts." + this.LayoutListView, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}


