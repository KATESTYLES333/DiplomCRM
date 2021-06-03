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
using System.Collections.Generic;
using System.Diagnostics;

namespace SplendidCRM.Administration.Undelete
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected SearchBasic            ctlSearchBasic ;
		protected _controls.CheckAll     ctlCheckAll    ;

		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;

		public class UndeleteModule
		{
			private HttpContext Context          ;
			private string      sMODULE_NAME     ;
			private string[]    arrID            ;
			private Guid        gMODIFIED_USER_ID;
			
			public UndeleteModule(HttpContext Context, string sMODULE_NAME, string[] arrID, Guid gMODIFIED_USER_ID)
			{
				this.Context           = Context          ;
				this.sMODULE_NAME      = sMODULE_NAME     ;
				this.arrID             = arrID            ;
				this.gMODIFIED_USER_ID = gMODIFIED_USER_ID;
			}
			
			public void Start()
			{
				try
				{
					SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "Undelete Start: " + DateTime.Now.ToString() );
					if ( this.arrID != null && this.arrID.Length > 0 )
					{
						List<Guid> garrID = new List<Guid>();
						foreach ( string sID in this.arrID )
						{
							garrID.Add(Sql.ToGuid(sID));
						}
						DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sTABLE_NAME = Crm.Modules.TableName(Context.Application, this.sMODULE_NAME);
							string sAUDIT_NAME = "vw" + sTABLE_NAME + "_AUDIT";
							IDbCommand spUndelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Undelete");
							
							DataTable dt = new DataTable();
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								string sSQL;
								sSQL = "select ID               " + ControlChars.CrLf
								     + "     , AUDIT_TOKEN      " + ControlChars.CrLf
								     + "  from " + sAUDIT_NAME    + ControlChars.CrLf
								     + " where AUDIT_ACTION = -1" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AppendParameter(cmd, garrID.ToArray(), "AUDIT_ID");
								cmd.CommandText += " order by AUDIT_DATE" + ControlChars.CrLf;
								
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dt);
								}
							}
							
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								spUndelete.Transaction = trn;
								try
								{
									IDbDataParameter parID               = Sql.FindParameter(spUndelete, "@ID"              );
									IDbDataParameter parMODIFIED_USER_ID = Sql.FindParameter(spUndelete, "@MODIFIED_USER_ID");
									IDbDataParameter parAUDIT_TOKEN      = Sql.FindParameter(spUndelete, "@AUDIT_TOKEN"     );
									parMODIFIED_USER_ID.Value = this.gMODIFIED_USER_ID;
									foreach ( DataRow row in dt.Rows )
									{
										Guid   gID          = Sql.ToGuid  (row["ID"         ]);
										string sAUDIT_TOKEN = Sql.ToString(row["AUDIT_TOKEN"]);
										parID         .Value = gID;
										parAUDIT_TOKEN.Value = Sql.ToDBString(sAUDIT_TOKEN);
										spUndelete.ExecuteNonQuery();
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
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
				finally
				{
					SplendidError.SystemMessage(Context, "Warning", new StackTrace(true).GetFrame(0), "Undelete End: " + DateTime.Now.ToString() );
				}
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					grdMain.CurrentPageIndex = 0;
					grdMain.DataBind();
				}
				else if ( e.CommandName == "Clear" )
				{
					grdMain.CurrentPageIndex = 0;
					ctlSearchBasic.ClearForm();
					Bind(false);
				}
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
				else if ( e.CommandName == "SelectAll" )
				{
					if ( vwMain == null )
						grdMain.DataBind();
					ctlCheckAll.SelectAll(vwMain, "AUDIT_ID");
					grdMain.DataBind();
				}
				else if ( e.CommandName == "Undelete" )
				{
					string[] arrID = ctlCheckAll.SelectedItemsArray;
					if ( arrID != null && arrID.Length > 0 )
					{
						// 08/10/2013 Paul.  Perform a test lookup of the delete procedure.  An exception will be thrown if it does not exist. 
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sTABLE_NAME = Crm.Modules.TableName(ctlSearchBasic.MODULE_NAME);
							// 08/07/2013 Paul.  Use the factory early so that the exception will be display to the user. 
							SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Undelete");
						}
						
						UndeleteModule undelete = new UndeleteModule(this.Context, ctlSearchBasic.MODULE_NAME, arrID, Security.USER_ID);
						if ( ctlSearchBasic.BackgroundOperation )
						{
							System.Threading.Thread t = new System.Threading.Thread(undelete.Start);
							t.Start();
							lblError.Text = L10n.Term("Undelete.LBL_UNDELETING");
							ctlCheckAll.ClearAll();
						}
						else
						{
							undelete.Start();
							string sStatus = L10n.Term("Undelete.LBL_UNDELETE_COMPLETE");
							string sModuleDisplayName = (arrID.Length == 1) ? L10n.Term(".moduleListSingular." + ctlSearchBasic.MODULE_NAME) : L10n.Term(".moduleList." + ctlSearchBasic.MODULE_NAME);
							lblError.Text = String.Format(sStatus, arrID.Length, sModuleDisplayName);
							ctlCheckAll.ClearAll();
							// 01/24/2015 Paul.  Rebind after updating. 
							Bind(true);
						}
						
						grdMain.CurrentPageIndex = 0;
						grdMain.DataBind();
					}
					else
					{
						lblError.Text = L10n.Term("Undelete.LBL_NOTHING_SELECTED");
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		protected void grdMain_OnSelectMethod(int nCurrentPageIndex, int nPageSize)
		{
			if ( !Sql.IsEmptyString(ctlSearchBasic.MODULE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						string sTABLE_NAME = Crm.Modules.TableName(ctlSearchBasic.MODULE_NAME);
						string sAUDIT_NAME = "vw" + sTABLE_NAME + "_AUDIT";
						string sSQL;
						// 08/06/2013 Paul.  We need to join to the main table to make sure that the record is still deleted as the undelete operation may have already run. 
						sSQL = "select AUDIT_ID         " + ControlChars.CrLf
						     + "     , AUDIT_DATE       " + ControlChars.CrLf
						     + "     , AUDIT_TOKEN      " + ControlChars.CrLf
						     + "     , ID               " + ControlChars.CrLf
						     + "     , NAME             " + ControlChars.CrLf
						     + "     , MODIFIED_BY      " + ControlChars.CrLf
						     + "     , MODIFIED_USER_ID " + ControlChars.CrLf
						     + "  from " + sAUDIT_NAME    + ControlChars.CrLf
						     + " where AUDIT_ACTION = -1" + ControlChars.CrLf
						     + "   and ID in (select ID from " + sTABLE_NAME + " where DELETED = 1 and " + sTABLE_NAME + ".ID = " + sAUDIT_NAME + ".ID)" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						ctlSearchBasic.SqlSearchClause(cmd);
						if ( nPageSize > 0 )
						{
							Sql.PageResults(cmd, sAUDIT_NAME, grdMain.OrderByClause(), nCurrentPageIndex, nPageSize);
						}
						else
						{
							cmd.CommandText += grdMain.OrderByClause();
						}
						
						if ( bDebug )
							RegisterClientScriptBlock("SQLPaged", Sql.ClientScriptBlock(cmd));
						
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
					}
				}
			}
		}

		private void Bind(bool bIsPostBack)
		{
			if ( !Sql.IsEmptyString(ctlSearchBasic.MODULE_NAME) )
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						
						string sTABLE_NAME = Crm.Modules.TableName(ctlSearchBasic.MODULE_NAME);
						string sAUDIT_NAME = "vw" + sTABLE_NAME + "_AUDIT";
						string sSQL;
						// 08/06/2013 Paul.  We need to join to the main table to make sure that the record is still deleted as the undelete operation may have already run. 
						sSQL = "  from " + sAUDIT_NAME    + ControlChars.CrLf
						     + " where AUDIT_ACTION = -1" + ControlChars.CrLf
						     + "   and ID in (select ID from " + sTABLE_NAME + " where DELETED = 1 and " + sTABLE_NAME + ".ID = " + sAUDIT_NAME + ".ID)" + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						ctlSearchBasic.SqlSearchClause(cmd);
						
						// 01/24/2015 Paul.  We need to make sure not to use custom paging when selecting all. 
						if ( grdMain.AllowCustomPaging )
						{
							cmd.CommandText = "select count(*)" + ControlChars.CrLf
							                + cmd.CommandText;
							
							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
							
							grdMain.VirtualItemCount = Sql.ToInteger(cmd.ExecuteScalar());
						}
						else
						{
							cmd.CommandText = "select AUDIT_ID         " + ControlChars.CrLf
							                + "     , AUDIT_DATE       " + ControlChars.CrLf
							                + "     , AUDIT_TOKEN      " + ControlChars.CrLf
							                + "     , ID               " + ControlChars.CrLf
							                + "     , NAME             " + ControlChars.CrLf
							                + "     , MODIFIED_BY      " + ControlChars.CrLf
							                + "     , MODIFIED_USER_ID " + ControlChars.CrLf
							                + cmd.CommandText;
							
							if ( bDebug )
								RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));
							
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
						}
					}
				}
			}
			if ( !bIsPostBack )
			{
				grdMain.DataBind();
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Undelete.LBL_MODULE_TITLE"));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				// 01/24/2015 Paul.  We need to make sure not to use custom paging when selecting all. 
				grdMain.AllowCustomPaging = !ctlCheckAll.SelectAllChecked;
				grdMain.SelectMethod     += new SelectMethodHandler(grdMain_OnSelectMethod);
				//ctlCheckAll.ShowSelectAll = false;
				if ( !IsPostBack )
					grdMain.OrderByClause("AUDIT_DATE", "desc");
				
				Bind(IsPostBack);
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
			ctlSearchBasic .Command += new CommandEventHandler(Page_Command);
			ctlCheckAll    .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Undelete";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}


