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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Requests
{
	/// <summary>
	///		Summary description for MassUpdate.
	/// </summary>
	public class MassUpdate : SplendidCRM.MassUpdate
	{
		// 11/10/2010 Paul.  Convert MassUpdate to dynamic buttons. 
		// 06/06/2015 Paul.  MassUpdateButtons combines ListHeader and DynamicButtons. 
		protected _controls.MassUpdateButtons ctlDynamicButtons;

		protected _controls.DatePicker ctlDATE_CLOSED;
		protected HiddenField     txtPARTNER_ID      ;
		protected TextBox         txtPARTNER_NAME    ;
		protected DropDownList    lstREQUEST_TYPE;
		protected DropDownList    lstLEAD_SOURCE     ;
		protected DropDownList    lstSALES_STAGE     ;
		public    CommandEventHandler Command ;
		protected _controls.TeamAssignedMassUpdate ctlTeamAssignedMassUpdate;
		// 05/13/2016 Paul.  Add Tags module. 
		protected _controls.TagMassUpdate          ctlTagMassUpdate;

		public Guid ASSIGNED_USER_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.ASSIGNED_USER;
			}
		}

		public Guid PRIMARY_TEAM_ID
		{
			get
			{
				return ctlTeamAssignedMassUpdate.PRIMARY_TEAM_ID;
			}
		}

		// 08/29/2009 Paul. Add support for dynamic teams. 
		public string TEAM_SET_LIST
		{
			get
			{
				return ctlTeamAssignedMassUpdate.TEAM_SET_LIST;
			}
		}

		public bool ADD_TEAM_SET
		{
			get
			{
				return ctlTeamAssignedMassUpdate.ADD_TEAM_SET;
			}
		}

		public DateTime DATE_CLOSED
		{
			get
			{
				// 07/09/2006 Paul.  Move the date conversion out of the MassUpdate control. 
				return ctlDATE_CLOSED.Value;
			}
		}

		public Guid PARTNER_ID
		{
			get
			{
				return Sql.ToGuid(txtPARTNER_ID.Value);
			}
		}

		public string REQUEST_TYPE
		{
			get
			{
				return lstREQUEST_TYPE.SelectedValue;
			}
		}

		public string LEAD_SOURCE
		{
			get
			{
				return lstLEAD_SOURCE.SelectedValue;
			}
		}

		public string SALES_STAGE
		{
			get
			{
				return lstSALES_STAGE.SelectedValue;
			}
		}

		// 05/13/2016 Paul.  Add Tags module. 
		public string TAG_SET_NAME
		{
			get
			{
				return ctlTagMassUpdate.TAG_SET_NAME;
			}
		}

		public bool ADD_TAG_SET
		{
			get
			{
				return ctlTagMassUpdate.ADD_TAG_SET;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// Command is handled by the parent. 
			if ( Command != null )
				Command(this, e) ;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( !IsPostBack )
				{
					// 06/02/2006 Paul.  Buttons should be hidden if the user does not have access. 
					int nACLACCESS_Delete = Security.GetUserAccess(m_sMODULE, "delete");
					int nACLACCESS_Edit   = Security.GetUserAccess(m_sMODULE, "edit"  );
					ctlDynamicButtons.ShowButton("MassUpdate", nACLACCESS_Edit   >= 0);
					ctlDynamicButtons.ShowButton("MassDelete", nACLACCESS_Delete >= 0);
					// 09/26/2017 Paul.  Add Archive access right. 
					int nACLACCESS_Archive = Security.GetUserAccess(m_sMODULE, "archive");
					ctlDynamicButtons.ShowButton("Archive.MoveData"   , (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) && !ArchiveView() && ArchiveEnabled());
					ctlDynamicButtons.ShowButton("Archive.RecoverData", (nACLACCESS_Archive >= ACL_ACCESS.ARCHIVE || Security.IS_ADMIN) &&  ArchiveView() && ArchiveEnabled());

					// 05/01/2007 Paul.  Fix list name for REQUEST_TYPE. 
					lstREQUEST_TYPE.DataSource = SplendidCache.List("request_type_dom");
					lstREQUEST_TYPE.DataBind();
					lstREQUEST_TYPE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstLEAD_SOURCE     .DataSource = SplendidCache.List("lead_source_dom");
					lstLEAD_SOURCE     .DataBind();
					lstLEAD_SOURCE     .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstSALES_STAGE     .DataSource = SplendidCache.List("sales_stage_dom");
					lstSALES_STAGE     .DataBind();
					lstSALES_STAGE     .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
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
			m_sMODULE = "Requests";
			// 09/26/2017 Paul.  Add Archive access right. 
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".MassUpdate" + (ArchiveView() ? ".ArchiveView" : String.Empty), Guid.Empty, null);
			ctlDynamicButtons.ShowButton("Sync"  , SplendidCRM.Crm.Modules.ExchangeFolders(m_sMODULE) && Security.HasExchangeAlias());
			ctlDynamicButtons.ShowButton("Unsync", SplendidCRM.Crm.Modules.ExchangeFolders(m_sMODULE) && Security.HasExchangeAlias());
		}
		#endregion
	}
}


