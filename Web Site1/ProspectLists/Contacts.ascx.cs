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

namespace SplendidCRM.ProspectLists
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
					// 09/06/2012 Paul.  Add import buttons. 
					case "Contacts.Import":
					{
						Response.Redirect("~/Contacts/import.aspx?PROSPECT_LIST_ID=" + gID.ToString());
						break;
					}
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
							SqlProcs.spPROSPECT_LISTS_CONTACTS_Delete(gID, gCONTACT_ID);
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
						arrSelectFields.Remove("CONTACT_ID"               );
						arrSelectFields.Remove("CONTACT_NAME"             );
						arrSelectFields.Remove("PROSPECT_LIST_ID"         );
						arrSelectFields.Remove("PROSPECT_LIST_NAME"       );
						arrSelectFields.Remove("PROSPECT_ASSIGNED_USER_ID");
						arrSelectFields.Remove("PROSPECT_DYNAMIC_LIST"    );
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "     , ID                         as CONTACT_ID               " + ControlChars.CrLf
						     + "     , NAME                       as CONTACT_NAME             " + ControlChars.CrLf
						     + "     , @PROSPECT_LIST_ID          as PROSPECT_LIST_ID         " + ControlChars.CrLf
						     + "     , @PROSPECT_LIST_NAME        as PROSPECT_LIST_NAME       " + ControlChars.CrLf
						     + "     , @PROSPECT_ASSIGNED_USER_ID as PROSPECT_ASSIGNED_USER_ID" + ControlChars.CrLf
						     + "     , @PROSPECT_DYNAMIC_LIST     as PROSPECT_DYNAMIC_LIST    " + ControlChars.CrLf
						     + "  from vwCONTACTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@PROSPECT_LIST_ID"         , gID);
						Sql.AddParameter(cmd, "@PROSPECT_LIST_NAME"       , Sql.ToString (Page.Items["NAME"                 ]));
						Sql.AddParameter(cmd, "@PROSPECT_ASSIGNED_USER_ID", Sql.ToGuid   (Page.Items["ASSIGNED_USER_ID"     ]));
						Sql.AddParameter(cmd, "@PROSPECT_DYNAMIC_LIST"    , Sql.ToBoolean(Page.Items["PROSPECT_DYNAMIC_LIST"]));
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, arrUPDATED.ToArray(), "ID");
					}
					else
					{
						// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
						sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
						     + "  from vwPROSPECT_LISTS_CONTACTS" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
						Security.Filter(cmd, m_sMODULE, "list");
						Sql.AppendParameter(cmd, gID, "PROSPECT_LIST_ID");
					}
					// 06/20/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchView.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					// 10/14/2012 Paul.  Change default sort to Contact Name. 
					cmd.CommandText += grdMain.OrderByClause("CONTACT_NAME", "asc");

					if ( bDebug )
						RegisterClientScriptBlock("vwPROSPECT_LISTS_CONTACTS", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules("ProspectLists." + m_sMODULE, dt);
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
					SqlProcs.spPROSPECT_LISTS_CONTACTS_Delete(gPARENT_ID, gDELETE_ID, trn);
			}

			UniqueGuidCollection arrUPDATED = this.GetUpdatedEditViewRelationships();
			foreach ( Guid gUPDATE_ID in arrUPDATED )
			{
				if ( !Sql.IsEmptyGuid(gUPDATE_ID) )
					SqlProcs.spPROSPECT_LISTS_CONTACTS_Update(gPARENT_ID, gUPDATE_ID, trn);
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			if ( !Sql.IsEmptyString(txtCONTACT_ID.Value) )
			{
				try
				{
					// 10/04/2006 Paul.  ValidateIDs can throw an exception. 
					string[] arrID = txtCONTACT_ID.Value.Split(',');
					if ( arrID != null )
					{
						if ( bEditView )
						{
							this.UpdateEditViewRelationship(arrID);
						}
						else
						{
							// 06/11/2009 Paul.  Use a stack to run the update in blocks of under 200 IDs. 
							//string sIDs = Utils.ValidateIDs(arrID);
							System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "list", arrID, "CONTACTS");
							if ( stk.Count > 0 )
							{
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									con.Open();
									// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
									using ( IDbTransaction trn = Sql.BeginTransaction(con) )
									{
										try
										{
											while ( stk.Count > 0 )
											{
												string sIDs = Utils.BuildMassIDs(stk);
												SqlProcs.spPROSPECT_LISTS_CONTACTS_MassUpdate(sIDs, gID, trn);
											}
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											throw(new Exception(ex.Message, ex.InnerException));
										}
									}
								}
								// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
								//Response.Redirect("view.aspx?ID=" + gID.ToString());
								// 05/16/2008 Paul.  If we are not going to redirect,then we must clear the value. 
								txtCONTACT_ID.Value = String.Empty;
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
				ctlDynamicButtons.AppendButtons("ProspectLists." + m_sMODULE, gASSIGNED_USER_ID, gID);
				// 01/10/2010 Paul.  Disable the Create and Select buttons if this is a dynamic list. 
				bool bDYNAMIC_LIST = Sql.ToBoolean(Page.Items["DYNAMIC_LIST"]);
				if ( bDYNAMIC_LIST )
					ctlDynamicButtons.DisableAll();
				// 02/21/2010 Paul.  The parent needs to be initialized when the page first loads. 
				ctlNewRecord.PARENT_ID = gID;
				// 03/28/2010 Paul.  We need to specify the parent type in order for the parent to be assigned inside the update procedure. 
				ctlNewRecord.PARENT_TYPE = "ProspectLists";
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
			arrSelectFields.Add("DATE_ENTERED"             );
			arrSelectFields.Add("CONTACT_ID"               );
			arrSelectFields.Add("ASSIGNED_USER_ID"         );
			arrSelectFields.Add("PROSPECT_ASSIGNED_USER_ID");
			// 01/10/2010 Paul.  New field that is used to determine if the Remove link is displayed. 
			arrSelectFields.Add("PROSPECT_DYNAMIC_LIST"    );
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			this.AppendGridColumns(grdMain, "ProspectLists." + m_sMODULE, arrSelectFields);
			// 04/28/2008 Paul.  Make use of dynamic buttons. 
			if ( IsPostBack )
				ctlDynamicButtons.AppendButtons("ProspectLists." + m_sMODULE, Guid.Empty, Guid.Empty);
		}
		#endregion
	}
}

