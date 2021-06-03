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

namespace SplendidCRM.Activities
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		// 06/05/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected _controls.HeaderButtons ctlModuleHeader;
		protected _controls.ExportHeader ctlExportHeader;
		protected _controls.SearchView   ctlSearchView  ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected UniqueStringCollection arrSelectFields;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected Calls.MassUpdate    ctlMassUpdate  ;
		// 06/06/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
		protected Panel         pnlMassUpdateSeven;
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
				// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
				else if ( e.CommandName == "ToggleMassUpdate" )
				{
					pnlMassUpdateSeven.Visible = !pnlMassUpdateSeven.Visible;
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
						// 07/06/2017 Paul.  If there is still no view, then there was an error in the select. 
						if ( vwMain != null )
						{
							if ( nACLACCESS == ACL_ACCESS.OWNER )
								vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
							// 11/27/2010 Paul.  Use new selected items. 
							string[] arrID = ctlCheckAll.SelectedItemsArray;
							SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID, grdMain.AllowCustomPaging);
						}
						else
						{
							lblError.Text += ControlChars.CrLf + "vwMain is null.";
						}
					}
				}
				// 03/08/2014 Paul.  Add support for Preview button. 
				else
				{
					grdMain.DataBind();
					if ( Page.Master is SplendidMaster )
						(Page.Master as SplendidMaster).Page_Command(sender, e);
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
					// 03/15/2016 Paul.  We don't use the _List view here because it has an old meaning. 
					// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
					m_sVIEW_NAME = "vwACTIVITIES";
					cmd.CommandText = "  from " + m_sVIEW_NAME + ControlChars.CrLf;
					Security.Filter(cmd, m_sMODULE, "list");
					ctlSearchView.SqlSearchClause(cmd);
					// 09/23/2015 Paul.  Paginated results still need to specify export fields. 
					// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
					cmd.CommandText = "select " + (Request.Form[ctlExportHeader.ExportUniqueID] != null ? SplendidDynamic.ExportGridColumns(m_sMODULE + ".Export", arrSelectFields) : Sql.FormatSelectFields(arrSelectFields))
					                + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
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
					// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
					ctlCheckAll  .Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Activities.LBL_LIST_FORM_TITLE"));
			try
			{
				// 09/08/2009 Paul.  Add support for custom paging in a DataGrid. Custom paging can be enabled / disabled per module. 
				if ( Crm.Config.allow_custom_paging() && Crm.Modules.CustomPaging(m_sMODULE) )
				{
					// 10/05/2009 Paul.  We need to make sure to disable paging when exporting all. 
					// 09/18/2012 Paul.  Disable custom paging if SelectAll was checked. 
					// 01/24/2018 Paul.  Disable custom paging if Selected Records is used as selection can cross pages. 
					grdMain.AllowCustomPaging = (Request.Form[ctlExportHeader.ExportUniqueID] == null || ctlExportHeader.ExportRange == "Page") && !ctlCheckAll.SelectAllChecked;
					grdMain.SelectMethod     += new SelectMethodHandler(grdMain_OnSelectMethod);
					// 11/17/2010 Paul.  Disable Select All when using custom paging. 
					//ctlCheckAll.ShowSelectAll = false;
				}

				// 11/15/2007 Paul.  Hide the checkbox column on a mobile browser. 
				if ( this.IsMobile && grdMain.Columns.Count > 0 )
					grdMain.Columns[0].Visible = false;
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						grdMain.OrderByClause("DATE_MODIFIED", "desc");
						// 03/15/2016 Paul.  We don't use the _List view here because it has an old meaning. 
						// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
						m_sVIEW_NAME = "vwACTIVITIES";
						cmd.CommandText = "  from " + m_sVIEW_NAME + ControlChars.CrLf;
						Security.Filter(cmd, m_sMODULE, "list");
						ctlSearchView.SqlSearchClause(cmd);
						if ( !IsPostBack )
						{
							Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								string sMODULE      = String.Empty;
								string sPARENT_TYPE = String.Empty;
								string sPARENT_NAME = String.Empty;
								SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
								if ( !Sql.IsEmptyGuid(gPARENT_ID) )
								{
									if ( sPARENT_TYPE != "Project" && sPARENT_TYPE != "ProjectTask" )
										sPARENT_TYPE = sMODULE;
									new DynamicControl(ctlSearchView, "PARENT_ID"  ).ID   = gPARENT_ID;
									new DynamicControl(ctlSearchView, "PARENT_NAME").Text = sPARENT_NAME;
									new DynamicControl(ctlSearchView, "PARENT_ID_PARENT_TYPE").SelectedValue = sPARENT_TYPE;
									Sql.AppendParameter(cmd, gPARENT_ID, "PARENT_ID");
								}
							}
						}
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
							// 09/23/2015 Paul.  Need to include the data grid fields as it will be bound using the same data set. 
							// 11/01/2017 Paul.  Use a module-based flag so that Record Level Security is only enabled when needed. 
							cmd.CommandText = "select " + (Request.Form[ctlExportHeader.ExportUniqueID] != null ? SplendidDynamic.ExportGridColumns(m_sMODULE + ".Export", arrSelectFields) : Sql.FormatSelectFields(arrSelectFields))
							                + Sql.AppendRecordLevelSecurityField(m_sMODULE, "edit", m_sVIEW_NAME)
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
							// 06/06/2015 Paul.  Change standard MassUpdate command to a command to toggle visibility. 
							ctlCheckAll  .Visible = ctlExportHeader.Visible && !PrintView && SplendidCRM.Crm.Modules.MassUpdate(m_sMODULE);
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
			// 03/15/2016 Paul.  Make better use of the activities list by showing all. 
			m_sMODULE = "Activities";
			SetMenu("Activities");
			// 02/08/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("ID"              );
			arrSelectFields.Add("NAME"            );
			arrSelectFields.Add("DATE_MODIFIED"   );
			arrSelectFields.Add("ACTIVITY_TYPE"   );
			arrSelectFields.Add("ASSIGNED_USER_ID");
			// 03/08/2014 Paul.  Add support for Preview button. 
			this.AppendGridColumns(grdMain, m_sMODULE + "." + LayoutListView, arrSelectFields, Page_Command);
			// 05/02/2006 Paul.  Hide the MassUpdate control if the user cannot make changes. 
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;
			
			// 05/31/2015 Paul.  Combine ModuleHeader and DynamicButtons. 
			if ( SplendidDynamic.StackedLayout(Page.Theme) )
			{
				ctlModuleHeader.Command += new CommandEventHandler(Page_Command);
				ctlModuleHeader.AppendButtons(m_sMODULE + "." + LayoutListView, Guid.Empty, null);
				// 06/05/2015 Paul.  Move MassUpdate buttons to the SplendidGrid. 
				grdMain.IsMobile       = this.IsMobile;
				grdMain.MassUpdateView = m_sMODULE + ".MassUpdate";
				grdMain.Command       += new CommandEventHandler(Page_Command);
				if ( !IsPostBack )
					pnlMassUpdateSeven.Visible = false;
			}
		}
		#endregion
	}
}


