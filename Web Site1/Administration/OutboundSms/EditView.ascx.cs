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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Administration.OutboundSms
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader   ctlModuleHeader  ;
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;
		protected HtmlTable       tblOptions                   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );

					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								string sSQL ;
								sSQL = "select *                  " + ControlChars.CrLf
								     + "  from vwOUTBOUND_SMS_Edit" + ControlChars.CrLf
								     + " where ID = @ID           " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											gID = Guid.Empty;
										}
									}
								}
							}
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									SqlProcs.spOUTBOUND_SMS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "NAME"       ).Text
										, new DynamicControl(this, rowCurrent, "USER_ID"    ).ID
										, new DynamicControl(this, rowCurrent, "FROM_NUMBER").Text
										, trn
										);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons.ErrorText = ex.Message;
									return;
								}
							}
						}
						SplendidCache.ClearOutboundSms();
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Test" )
			{
				try
				{
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );

					if ( Page.IsValid )
					{
						//EmailUtils.SendTestMessage(Application, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, sFromAddress, sFromName, sFromAddress, sFromName);
						ctlDynamicButtons.ErrorText = "Send was successful.";
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			this.Visible = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				Parent.DataBind();
				return;
			}

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
					ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                  " + ControlChars.CrLf
							     + "  from vwOUTBOUND_SMS_Edit" + ControlChars.CrLf
							     + " where ID = @ID           " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
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
											ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
											ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
											
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, rdr);
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
					}
				}
				else
				{
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
				}
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "OutboundSms";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain, null);
				ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
			}
		}
		#endregion
	}
}

