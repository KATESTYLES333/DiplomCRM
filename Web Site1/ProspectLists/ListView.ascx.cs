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
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.ExportHeader ctlExportHeader;
		protected _controls.SearchView   ctlSearchView  ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
					// 04/27/2008 Paul.  Sorting has been moved to the database to increase performance. 
					// 03/17/2011 Paul.  We need to treat a comma-separated list of fields as an array. 
					arrSelectFields.AddFields(grdMain.SortColumn);
				}
				// 11/17/2010 Paul.  Populate the hidden Selected field with all IDs. 
				else if ( e.CommandName == "SelectAll" )
				{
					// 05/22/2011 Paul.  When using custom paging, vwMain may not be defined. 
					if ( vwMain == null )
						grdMain.DataBind();
					ctlCheckAll.SelectAll(vwMain, "ID");
					grdMain.DataBind();
				}
				else if ( e.CommandName == "MassUpdate" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						// 10/26/2007 Paul.  Use a stack to run the update in blocks of under 200 IDs. 
						//string sIDs = Utils.ValidateIDs(arrID);
						System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "edit", arrID, "PROSPECT_LISTS");
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
											// 09/11/2007 Paul.  Mass update of teams is now available. 
											// 08/29/2009 Paul.  Add support for dynamic teams. 
											SqlProcs.spPROSPECT_LISTS_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, ctlMassUpdate.PRIMARY_TEAM_ID, ctlMassUpdate.TEAM_SET_LIST, ctlMassUpdate.ADD_TEAM_SET, trn);
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
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "MassDelete" )
				{
					// 11/27/2010 Paul.  Use new selected items. 
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null )
					{
						// 10/26/2007 Paul.  Use a stack to run the update in blocks of under 200 IDs. 
						//string sIDs = Utils.ValidateIDs(arrID);
						System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "delete", arrID, "PROSPECT_LISTS");
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
											SqlProcs.spPROSPECT_LISTS_MassDelete(sIDs, trn);
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
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "Export" )
				{
					// 11/03/2006 Paul.  Apply ACL rules to Export. 
					int nACLACCESS = SplendidCRM.Security.GetUserAccess(m_sMODULE, "export");
					if ( nACLACCESS  >= 0 )
					{
						// 10/05/2009 Paul.  When exporting, we may need to manually bind.  Custom paging should be disabled when exporting all. 
						if ( vwMain == null )
							grdMain.DataBind();
						if ( nACLACCESS == ACL_ACCESS.OWNER )
							vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
						// 11/27/2010 Paul.  Use new selected items. 
						string[] arrID = ctlCheckAll.SelectedItemsArray;
						SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID, grdMain.AllowCustomPaging);
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		// 09/08/2009 Paul.  Add support for custom paging. 
		protected void grdMain_OnSelectMethod(int nCurrentPageIndex, int nPageSize)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
					// 03/31/2012 Paul.  Add support for favorites. 
					cmd.CommandText = "  from vw" + sTABLE_NAME + "_List" + ControlChars.CrLf
					                + "  left outer join vwSUGARFAVORITES                                       " + ControlChars.CrLf
					                + "               on vwSUGARFAVORITES.FAVORITE_RECORD_ID = ID               " + ControlChars.CrLf
					                + "              and vwSUGARFAVORITES.FAVORITE_USER_ID   = @FAVORITE_USER_ID" + ControlChars.CrLf;
					Sql.AddParameter(cmd, "@FAVORITE_USER_ID", Security.USER_ID);
					Security.Filter(cmd, m_sMODULE, "list");
					ctlSearchView.SqlSearchClause(cmd);
					cmd.CommandText = "select " + Sql.FormatSelectFields(arrSelectFields)
					                + cmd.CommandText;
					if ( nPageSize > 0 )
					{
						Sql.PageResults(cmd, sTABLE_NAME, grdMain.OrderByClause(), nCurrentPageIndex, nPageSize);
					}
					else
					{
						cmd.CommandText += grdMain.OrderByClause();
					}
					
					if ( bDebug )
						RegisterClientScriptBlock("SQLPaged", Sql.ClientScriptBlock(cmd));
					
					// 01/13/2010 Paul.  Allow default search to be disabled. 
					if ( PrintView || IsPostBack || SplendidCRM.Crm.Modules.DefaultSearch(m_sMODULE) )
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 11/22/2010 Paul.  Apply Business Rules. 
								this.ApplyGridViewRules(m_sMODULE + "." + LayoutListView, dt);
								
								vwMain = dt.DefaultView;
								grdMain.DataSource = vwMain ;
							}
						}
						ctlExportHeader.Visible = true;
					}
					else
					{
						ctlExportHeader.Visible = false;
					}
					ctlMassUpdate.Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
					ctlCheckAll  .Visible = ctlMassUpdate.Visible;
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 09/08/2009 Paul.  Add support for custom paging in a DataGrid. Custom paging can be enabled / disabled per module. 
				if ( Crm.Config.allow_custom_paging() && Crm.Modules.CustomPaging(m_sMODULE) )
				{
					// 10/05/2009 Paul.  We need to make sure to disable paging when exporting all. 
					// 09/18/2012 Paul.  Disable custom paging if SelectAll was checked. 
					grdMain.AllowCustomPaging = (Request.Form[ctlExportHeader.ExportUniqueID] == null || ctlExportHeader.ExportRange != "All") && !ctlCheckAll.SelectAllChecked;
					grdMain.SelectMethod     += new SelectMethodHandler(grdMain_OnSelectMethod);
					// 11/17/2010 Paul.  Disable Select All when using custom paging. 
					//ctlCheckAll.ShowSelectAll = false;
				}

				if ( this.IsMobile && grdMain.Columns.Count > 0 )
					grdMain.Columns[0].Visible = false;
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						grdMain.OrderByClause("NAME", "asc");
						
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						// 03/31/2012 Paul.  Add support for favorites. 
						cmd.CommandText = "  from vw" + sTABLE_NAME + "_List" + ControlChars.CrLf
						                + "  left outer join vwSUGARFAVORITES                                       " + ControlChars.CrLf
						                + "               on vwSUGARFAVORITES.FAVORITE_RECORD_ID = ID               " + ControlChars.CrLf
						                + "              and vwSUGARFAVORITES.FAVORITE_USER_ID   = @FAVORITE_USER_ID" + ControlChars.CrLf;
						Sql.AddParameter(cmd, "@FAVORITE_USER_ID", Security.USER_ID);
						Security.Filter(cmd, m_sMODULE, "list");
						ctlSearchView.SqlSearchClause(cmd);
						// 09/08/2009 Paul.  Custom paging will always require two queries, the first is to get the total number of rows. 
						if ( grdMain.AllowCustomPaging )
						{
							cmd.CommandText = "select count(*)" + ControlChars.CrLf
							                + cmd.CommandText;
							
							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
							
							// 01/13/2010 Paul.  Allow default search to be disabled. 
							if ( PrintView || IsPostBack || SplendidCRM.Crm.Modules.DefaultSearch(m_sMODULE) )
							{
								grdMain.VirtualItemCount = Sql.ToInteger(cmd.ExecuteScalar());
							}
						}
						else
						{
							// 04/27/2008 Paul.  The fields in the search clause need to be prepended after any Saved Search sort has been determined.
							// 09/08/2009 Paul.  The order by clause is separate as it can change due to SearchViews. 
							// 04/20/2011 Paul.  If there are no fields in the GridView.Export, then return all fields (*). 
							cmd.CommandText = "select " + (Request.Form[ctlExportHeader.ExportUniqueID] != null ? SplendidDynamic.ExportGridColumns(m_sMODULE + ".Export") : Sql.FormatSelectFields(arrSelectFields))
							                + cmd.CommandText
							                + grdMain.OrderByClause();
							
							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
							
							// 01/13/2010 Paul.  Allow default search to be disabled. 
							if ( PrintView || IsPostBack || SplendidCRM.Crm.Modules.DefaultSearch(m_sMODULE) )
							{
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										// 11/22/2010 Paul.  Apply Business Rules. 
										this.ApplyGridViewRules(m_sMODULE + "." + LayoutListView, dt);
										
										vwMain = dt.DefaultView;
										grdMain.DataSource = vwMain ;
									}
								}
								ctlExportHeader.Visible = true;
							}
							else
							{
								ctlExportHeader.Visible = false;
							}
							ctlMassUpdate.Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
							ctlCheckAll  .Visible = ctlMassUpdate.Visible;
						}
					}
				}
				if ( !IsPostBack )
				{
					// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
					//Page.DataBind();
					// 09/08/2009 Paul.  Let the grid handle the differences between normal and custom paging. 
					// 09/08/2009 Paul.  Bind outside of the existing connection so that a second connect would not get created. 
					grdMain.DataBind();
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			ctlSearchView  .Command += new CommandEventHandler(Page_Command);
			ctlExportHeader.Command += new CommandEventHandler(Page_Command);
			ctlMassUpdate  .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll    .Command += new CommandEventHandler(Page_Command);
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			m_sMODULE = "ProspectLists";
			// 02/13/2007 Paul.  ProspectLists should highlight the Campaigns menu. 
			// 03/15/2011 Paul.  Change menu to use main module. 
			SetMenu(m_sMODULE);
			// 02/08/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("NAME");
			// 02/20/2010 Paul.  The ASSIGNED_USER_ID is used in the grid, so we must add it. 
			arrSelectFields.Add("ASSIGNED_USER_ID");
			// 03/31/2012 Paul.  Add support for favorites. 
			arrSelectFields.Add("FAVORITE_RECORD_ID");
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView, arrSelectFields);
			// 05/02/2006 Paul.  Hide the MassUpdate control if the user cannot make changes. 
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;

		}
		#endregion
	}
}


