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

namespace SplendidCRM.Cases
{
	/// <summary>
	///		Summary description for Contacts.
	/// </summary>
	public class Contacts : SubPanelControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.SearchView     ctlSearchView    ;
		protected UniqueStringCollection arrSelectFields;
		protected Guid            gID            ;
		protected DataView        vwMain         ;
		protected SplendidGrid    grdMain        ;
		protected HtmlInputHidden txtCONTACT_ID  ;
		// 02/21/2010 Paul.  Controls to manage inline create. 
		protected Button          btnCreateInline   ;
		protected Panel           pnlNewRecordInline;
		// 02/21/2010 Paul.  Make sure to point to the Contacts.NewRecord. 
		protected SplendidCRM.Contacts.NewRecord ctlNewRecord   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					//case "Contacts.Create":
					//	Response.Redirect("~/Contacts/edit.aspx?PARENT_ID=" + gID.ToString());
					//	break;
					case "Contacts.Edit":
					{
						Guid gCONTACT_ID = Sql.ToGuid(e.CommandArgument);
						Response.Redirect("~/Contacts/edit.aspx?ID=" + gCONTACT_ID.ToString());
						break;
					}
					case "Contacts.Remove":
					{
						Guid gCONTACT_ID = Sql.ToGuid(e.CommandArgument);
						if ( bEditView )
						{
							this.DeleteEditViewRelationship(gCONTACT_ID);
						}
						else
						{
							SqlProcs.spCONTACTS_CASES_Delete(gCONTACT_ID, gID);
						}
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					// 02/21/2010 Paul.  Handle new events that hide and show the NewRecord panel. 
					//case "NewRecord.Show":
					case "Contacts.Create":
						// 02/21/2010 Paul.  We are not going to allow inline create on a mobile device. 
						// 02/22/2010 Paul.  We should have a way to turn-off inline editing as it is a performance issue. 
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
						{
							// 12/27/2012 Paul.  The contacts popup should filter on the account. 
							Guid gPARENT_ID = ctlNewRecord.ACCOUNT_ID;
							if ( Sql.IsEmptyGuid(gPARENT_ID) )
								gPARENT_ID = gID;
							Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gPARENT_ID.ToString());
						}
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
					{
						// 12/27/2012 Paul.  The contacts popup should filter on the account. 
						Guid gPARENT_ID = ctlNewRecord.ACCOUNT_ID;
						if ( Sql.IsEmptyGuid(gPARENT_ID) )
							gPARENT_ID = gID;
						Response.Redirect("~/" + m_sMODULE + "/edit.aspx?PARENT_ID=" + gPARENT_ID.ToString());
						break;
					}
					case "NewRecord":
						//BindGrid();
						// 02/21/2010 Paul.  Redirect instead of rebind so that the NewRecord form will get cleared. 
						Response.Redirect(Request.RawUrl);
						break;
					// 06/20/2010 Paul.  Add support for SearchView events. Need to rebind inside the clear event. 
					case "Contacts.Search":
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
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
					if ( bEditView && IsPostBack && arrUPDATED.Count > 0 )
					{
						arrSelectFields.Remove("CONTACT_ID"            );
						arrSelectFields.Remove("CONTACT_NAME"          );
						arrSelectFields.Remove("CASE_ID"               );
						arrSelectFields.Remove("CASE_NAME"             );
						arrSelectFields.Remove("CASE_ASSIGNED_USER_ID" );
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                        as CONTACT_ID           " + ControlChars.CrLf
						     + "     , NAME                      as CONTACT_NAME         " + ControlChars.CrLf
						     + "     , @CASE_ID                  as CASE_ID              " + ControlChars.CrLf
						     + "     , @CASE_NAME                as CASE_NAME            " + ControlChars.CrLf
						     + "     , @CASE_ASSIGNED_USER_ID    as CASE_ASSIGNED_USER_ID" + ControlChars.CrLf
						     + "  from vwCONTACTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@CASE_ID"              , gID);
						Sql.AddParameter(cmd, "@CASE_NAME"            , Sql.ToString(Page.Items["NAME"            ]));
						Sql.AddParameter(cmd, "@CASE_ASSIGNED_USER_ID", Sql.ToGuid  (Page.Items["ASSIGNED_USER_ID"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "  from vwCASES_CONTACTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, gID, "CASE_ID");
					}
					// 06/20/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchView.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					// 10/14/2012 Paul.  Change default sort to Contact Name. 
					cmd.CommandText += grdMain.OrderByClause("CONTACT_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwCASES_CONTACTS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules("Cases." + m_sMODULE, dt);
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Enable sorting of sub panel. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								grdMain.DataBind();
								// 01/27/2010 Paul.  In EditView mode, we need a list of existing relationships. 
								if ( bEditView && !IsPostBack )
								{
									this.CreateEditViewRelationships(dt, "CONTACT_ID");
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
					SqlProcs.spCONTACTS_CASES_Delete(gDELETE_ID, gPARENT_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spCONTACTS_CASES_Update(gUPDATE_ID, gPARENT_ID, String.Empty, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			Guid gCONTACT_ID = Sql.ToGuid(txtCONTACT_ID.Value);
			if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
			{
				try
				{
					if ( bEditView )
					{
						this.UpdateEditViewRelationship(gCONTACT_ID);
					}
					else
					{
						SqlProcs.spCONTACTS_CASES_Update(gCONTACT_ID, gID, String.Empty);
					}
					// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
					//Response.Redirect("view.aspx?ID=" + gID.ToString());
					// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
					txtCONTACT_ID.Value = String.Empty;
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
				ctlDynamicButtons.AppendButtons("Cases." + m_sMODULE, gASSIGNED_USER_ID, gID);
				// 02/21/2010 Paul.  The parent needs to be initialized when the page first loads. 
				ctlNewRecord.PARENT_ID = gID;
				// 03/28/2010 Paul.  We need to specify the parent type in order for the parent to be assigned inside the update procedure. 
				ctlNewRecord.PARENT_TYPE = "Cases";
				// 12/27/2012 Paul.  The contacts popup should filter on the account. 
				ctlNewRecord.ACCOUNT_ID = Sql.ToGuid(Page.Items["ACCOUNT_ID"]);
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
			m_sMODULE = "Contacts";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_ENTERED"         );
			arrSelectFields.Add("CONTACT_ID"           );
			arrSelectFields.Add("ASSIGNED_USER_ID"     );
			arrSelectFields.Add("CASE_ASSIGNED_USER_ID");
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			this.AppendGridColumns(grdMain, "Cases." + m_sMODULE, arrSelectFields);
			// 04/28/2008 Paul.  Make use of dynamic buttons. 
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("Cases." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}


