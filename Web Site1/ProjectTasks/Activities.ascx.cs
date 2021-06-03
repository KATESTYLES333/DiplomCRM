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

namespace SplendidCRM.ProjectTasks
{
	/// <summary>
	///		Summary description for Activities.
	/// </summary>
	public class Activities : SplendidControl
	{
		protected _controls.DynamicButtons ctlDynamicButtonsOpen   ;
		protected _controls.DynamicButtons ctlDynamicButtonsHistory;
		protected _controls.SearchView     ctlSearchViewOpen       ;
		protected _controls.SearchView     ctlSearchViewHistory    ;
		protected UniqueStringCollection arrSelectFields;
		protected Guid          gID            ;
		protected DataView      vwOpen         ;
		protected SplendidGrid  grdOpen        ;
		protected DataView      vwHistory      ;
		protected SplendidGrid  grdHistory     ;
		// 02/21/2010 Paul.  Controls to manage inline create. 
		protected Panel           pnlNewRecordInlineTask   ;
		protected Panel           pnlNewRecordInlineCall   ;
		protected Panel           pnlNewRecordInlineMeeting;
		protected Panel           pnlNewRecordInlineNote   ;
		protected SplendidCRM.Tasks.NewRecord    ctlNewRecordTask   ;
		protected SplendidCRM.Calls.NewRecord    ctlNewRecordCall   ;
		protected SplendidCRM.Meetings.NewRecord ctlNewRecordMeeting;
		protected SplendidCRM.Notes.NewRecord    ctlNewRecordNote   ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				switch ( e.CommandName )
				{
					case "Tasks.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/Tasks/edit.aspx?PARENT_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInlineTask.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtonsOpen.HideAll();
						}
						break;
					case "Calls.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/Calls/edit.aspx?PARENT_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInlineCall.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtonsOpen.HideAll();
						}
						break;
					case "Meetings.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/Meetings/edit.aspx?PARENT_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInlineMeeting.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtonsOpen.HideAll();
						}
						break;
					case "Notes.Create":
						if ( this.IsMobile || Sql.ToBoolean(Application["CONFIG.disable_editview_inline"]) )
							Response.Redirect("~/Notes/edit.aspx?PARENT_ID=" + gID.ToString());
						else
						{
							pnlNewRecordInlineNote.Style.Add(HtmlTextWriterStyle.Display, "inline");
							ctlDynamicButtonsHistory.HideAll();
						}
						break;
					case "Emails.Compose":
						Response.Redirect("~/Emails/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "Emails.Archive":
						// 10/14/2010 Paul.  Needed to set the type to Archived. 
						Response.Redirect("~/Emails/edit.aspx?TYPE=archived&PARENT_ID=" + gID.ToString());
						break;
					case "Activities.Delete":
					{
						Guid gACTIVITY_ID = Sql.ToGuid(e.CommandArgument);
						SqlProcs.spACTIVITIES_Delete(gACTIVITY_ID);
						// 08/30/2006 Paul.  We need to redirect so that the activities list will reflect the deleted item. 
						//Response.Redirect("view.aspx?ID=" + gID.ToString());
						// 05/16/2008 Paul.  Instead of redirecting, just rebind the grid and AJAX will repaint. 
						BindGrid();
						break;
					}
					case "NewRecord.Cancel":
						pnlNewRecordInlineTask.Style.Add(HtmlTextWriterStyle.Display, "none");
						pnlNewRecordInlineCall.Style.Add(HtmlTextWriterStyle.Display, "none");
						pnlNewRecordInlineMeeting.Style.Add(HtmlTextWriterStyle.Display, "none");
						pnlNewRecordInlineNote.Style.Add(HtmlTextWriterStyle.Display, "none");
						ctlDynamicButtonsOpen.ShowAll();
						ctlDynamicButtonsHistory.ShowAll();
						break;
					case "NewRecord.FullForm":
						// 04/26/2010 Paul.  We cannot use the m_sMODULE here because the folder is plural. 
						Response.Redirect("~/ProjectTasks/edit.aspx?PARENT_ID=" + gID.ToString());
						break;
					case "NewRecord":
						//BindGrid();
						// 02/21/2010 Paul.  Redirect instead of rebind so that the NewRecord form will get cleared. 
						Response.Redirect(Request.RawUrl);
						break;
					// 06/21/2010 Paul.  Add support for SearchView events. Need to rebind inside the clear event. 
					case "Activities.SearchOpen":
						ctlSearchViewOpen.Visible = !ctlSearchViewOpen.Visible;
						break;
					case "Activities.SearchHistory":
						ctlSearchViewHistory.Visible = !ctlSearchViewHistory.Visible;
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
				ctlDynamicButtonsOpen.ErrorText = ex.Message;
			}
		}

		protected void BindGrid()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				// 04/26/2008 Paul.  Build the list of fields to use in the select clause.
				// 10/13/2012 Paul.  Create a separate activites view for the HTML5 Offline Client. 
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from " + Sql.MetadataName(con, "vwPROJECT_TASKS_ACTIVITIES_OPEN") + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
					// 12/07/2006 Paul.  This view has an alternate assigned id. 
					// 08/30/2009 Paul.  Activity views need to use a special activities team filter. 
					Security.Filter(cmd, m_sMODULE, "list", "ACTIVITY_ASSIGNED_USER_ID", true);
					Sql.AppendParameter(cmd, gID, "PROJECT_TASK_ID");
					//Sql.AppendParameter(cmd, true, "IS_OPEN", false);
					// 06/21/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchViewOpen.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdOpen.OrderByClause("DATE_DUE", "desc");

					if ( bDebug )
						RegisterClientScriptBlock("vwPROJECT_TASKS_ACTIVITIES_OPEN", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules(m_sMODULE + ".Activities.Open", dt);
								// 11/26/2005 Paul.  Convert the term here so that sorting will apply. 
								foreach(DataRow row in dt.Rows)
								{
									// 11/26/2005 Paul.  Status is translated differently for each type. 
									switch ( Sql.ToString(row["ACTIVITY_TYPE"]) )
									{
										// 07/15/2006 Paul.  Translation of Call status remains here because it is more complex than the standard list translation. 
										case "Calls"   :  row["STATUS"] = L10n.Term(".call_direction_dom.", row["DIRECTION"]) + " " + L10n.Term(".call_status_dom.", row["STATUS"]);  break;
										// 03/27/2008 Paul.  Correct the Call, Meeting and Task label to use the activity_dom list. 
										//case "Meetings":  row["STATUS"] = L10n.Term(".activity_dom.Meeting") + " " + L10n.Term(".meeting_status_dom.", row["STATUS"]);  break;
										//case "Tasks"   :  row["STATUS"] = L10n.Term(".activity_dom.Task"   ) + " " + L10n.Term(".task_status_dom."   , row["STATUS"]);  break;
									}
								}
								vwOpen = new DataView(dt);
								grdOpen.DataSource = vwOpen ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Bind but don't apply sort unless not postback. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								// 05/16/2008 Paul.  We must always rebind otherwise a postback from an alternate subpanel will cause this grid be incomplete.
								grdOpen.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtonsOpen.ErrorText = ex.Message;
					}
				}
				// 04/26/2008 Paul.  Use a separate query for the history so that it can be sorted separately. 
				// 10/13/2012 Paul.  Create a separate activites view for the HTML5 Offline Client. 
				sSQL = "select " + Sql.FormatSelectFields(arrSelectFields)
				     + "  from " + Sql.MetadataName(con, "vwPROJECT_TASKS_ACTIVITIES_HISTORY") + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 11/27/2006 Paul.  Make sure to filter relationship data based on team access rights. 
					// 12/07/2006 Paul.  This view has an alternate assigned id. 
					// 08/30/2009 Paul.  Activity views need to use a special activities team filter. 
					Security.Filter(cmd, m_sMODULE, "list", "ACTIVITY_ASSIGNED_USER_ID", true);
					Sql.AppendParameter(cmd, gID, "PROJECT_TASK_ID");
					//Sql.AppendParameter(cmd, false, "IS_OPEN", false);
					// 06/21/2010 Paul.  Allow searching of the subpanel. 
					ctlSearchViewHistory.SqlSearchClause(cmd);
					// 04/26/2008 Paul.  Move Last Sort to the database.
					cmd.CommandText += grdHistory.OrderByClause("DATE_MODIFIED", "desc");

					if ( bDebug )
						RegisterClientScriptBlock("vwPROJECT_TASKS_ACTIVITIES_HISTORY", Sql.ClientScriptBlock(cmd));

					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								// 03/07/2013 Paul.  Apply business rules to subpanel. 
								this.ApplyGridViewRules(m_sMODULE + ".Activities.History", dt);
								// 11/26/2005 Paul.  Convert the term here so that sorting will apply. 
								foreach(DataRow row in dt.Rows)
								{
									// 11/26/2005 Paul.  Status is translated differently for each type. 
									switch ( Sql.ToString(row["ACTIVITY_TYPE"]) )
									{
										// 07/15/2006 Paul.  Translation of Call status remains here because it is more complex than the standard list translation. 
										case "Calls"   :  row["STATUS"] = L10n.Term(".call_direction_dom.", row["DIRECTION"]) + " " + L10n.Term(".call_status_dom.", row["STATUS"]);  break;
										// 03/27/2008 Paul.  Correct the Call, Meeting and Task label to use the activity_dom list. 
										//case "Meetings":  row["STATUS"] = L10n.Term(".activity_dom.Meeting") + " " + L10n.Term(".meeting_status_dom.", row["STATUS"]);  break;
										//case "Tasks"   :  row["STATUS"] = L10n.Term(".activity_dom.Task"   ) + " " + L10n.Term(".task_status_dom."   , row["STATUS"]);  break;
									}
								}
								vwHistory = new DataView(dt);
								grdHistory.DataSource = vwHistory ;
								// 09/05/2005 Paul.  LinkButton controls will not fire an event unless the the grid is bound. 
								// 04/25/2008 Paul.  Bind but don't apply sort unless not postback. 
								// 04/26/2008 Paul.  Move Last Sort to the database.
								// 05/16/2008 Paul.  We must always rebind otherwise a postback from an alternate subpanel will cause this grid be incomplete.
								grdHistory.DataBind();
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlDynamicButtonsHistory.ErrorText = ex.Message;
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			gID = Sql.ToGuid(Request["ID"]);
			BindGrid();
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				// 04/28/2008 Paul.  Make use of dynamic buttons. 
				Guid gASSIGNED_USER_ID = Sql.ToGuid(Page.Items["ASSIGNED_USER_ID"]);
				ctlDynamicButtonsOpen   .AppendButtons(m_sMODULE + ".Activities.Open"   , gASSIGNED_USER_ID, gID);
				ctlDynamicButtonsHistory.AppendButtons(m_sMODULE + ".Activities.History", gASSIGNED_USER_ID, gID);
				// 02/21/2010 Paul.  The parent needs to be initialized when the page first loads. 
				ctlNewRecordTask   .PARENT_ID = gID;
				ctlNewRecordCall   .PARENT_ID = gID;
				ctlNewRecordMeeting.PARENT_ID = gID;
				ctlNewRecordNote   .PARENT_ID = gID;
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
			ctlDynamicButtonsOpen   .Command += new CommandEventHandler(Page_Command);
			ctlDynamicButtonsHistory.Command += new CommandEventHandler(Page_Command);
			// 06/21/2010 Paul.  We need to connect the SearchView command handler, otherwise it will throw an exception. 
			ctlSearchViewOpen       .Command += new CommandEventHandler(Page_Command);
			ctlSearchViewHistory    .Command += new CommandEventHandler(Page_Command);
			ctlNewRecordTask        .Command += new CommandEventHandler(Page_Command);
			ctlNewRecordCall        .Command += new CommandEventHandler(Page_Command);
			ctlNewRecordMeeting     .Command += new CommandEventHandler(Page_Command);
			ctlNewRecordNote        .Command += new CommandEventHandler(Page_Command);
			// 08/30/2009 Paul.  Specify the base module here and switch to Calls when applying ACL in the Filter method. 
			m_sMODULE = "ProjectTask";
			// 04/26/2008 Paul.  We need to build a list of the fields used by the search clause. 
			arrSelectFields = new UniqueStringCollection();
			arrSelectFields.Add("DATE_MODIFIED"            );
			arrSelectFields.Add("ACTIVITY_ID"              );
			arrSelectFields.Add("ACTIVITY_TYPE"            );
			arrSelectFields.Add("ACTIVITY_ASSIGNED_USER_ID");
			arrSelectFields.Add("IS_OPEN"                  );
			arrSelectFields.Add("DATE_DUE"                 );
			arrSelectFields.Add("STATUS"                   );
			arrSelectFields.Add("DIRECTION"                );
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			// 07/15/2006 Paul.  Fix name.  ProjectTask is singular so that it will match the module name. 
			this.AppendGridColumns(grdOpen   , m_sMODULE + ".Activities.Open"   , arrSelectFields);
			this.AppendGridColumns(grdHistory, m_sMODULE + ".Activities.History", arrSelectFields);
			if ( IsPostBack )
			{
				ctlDynamicButtonsOpen   .AppendButtons(m_sMODULE + ".Activities.Open"   , Guid.Empty, Guid.Empty);
				ctlDynamicButtonsHistory.AppendButtons(m_sMODULE + ".Activities.History", Guid.Empty, Guid.Empty);
			}
		}
		#endregion
	}
}


