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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Bugs
{
	/// <summary>
	///		Summary description for MassUpdate.
	/// </summary>
	public class MassUpdate : SplendidCRM.MassUpdate
	{
		// 11/10/2010 Paul.  Convert MassUpdate to dynamic buttons. 
		protected _controls.DynamicButtons ctlDynamicButtons;

		protected DropDownList lstSTATUS          ;
		protected DropDownList lstPRIORITY        ;
		protected DropDownList lstRESOLUTION      ;
		protected DropDownList lstTYPE            ;
		protected DropDownList lstSOURCE          ;
		protected DropDownList lstPRODUCT_CATEGORY;
		public    CommandEventHandler Command ;
		protected _controls.TeamAssignedMassUpdate ctlTeamAssignedMassUpdate;

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

		public string STATUS
		{
			get
			{
				return lstSTATUS.SelectedValue;
			}
		}

		public string PRIORITY
		{
			get
			{
				return lstPRIORITY.SelectedValue;
			}
		}

		public string RESOLUTION
		{
			get
			{
				return lstRESOLUTION.SelectedValue;
			}
		}

		public string TYPE
		{
			get
			{
				return lstTYPE.SelectedValue;
			}
		}

		public string SOURCE
		{
			get
			{
				return lstSOURCE.SelectedValue;
			}
		}

		public string PRODUCT_CATEGORY
		{
			get
			{
				return lstPRODUCT_CATEGORY.SelectedValue;
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

					lstSTATUS          .DataSource = SplendidCache.List("bug_status_dom");
					lstSTATUS          .DataBind();
					lstSTATUS          .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstPRIORITY        .DataSource = SplendidCache.List("bug_priority_dom");
					lstPRIORITY        .DataBind();
					lstPRIORITY        .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstRESOLUTION      .DataSource = SplendidCache.List("bug_resolution_dom");
					lstRESOLUTION      .DataBind();
					lstRESOLUTION      .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstTYPE            .DataSource = SplendidCache.List("bug_type_dom");
					lstTYPE            .DataBind();
					lstTYPE            .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstSOURCE          .DataSource = SplendidCache.List("source_dom");
					lstSOURCE          .DataBind();
					lstSOURCE          .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					lstPRODUCT_CATEGORY.DataSource = SplendidCache.List("product_category_dom");
					lstPRODUCT_CATEGORY.DataBind();
					lstPRODUCT_CATEGORY.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
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
			m_sMODULE = "Bugs";
			ctlDynamicButtons.AppendButtons(m_sMODULE + ".MassUpdate", Guid.Empty, null);
			ctlDynamicButtons.ShowButton("Sync"  , SplendidCRM.Crm.Modules.ExchangeFolders(m_sMODULE) && Security.HasExchangeAlias());
			ctlDynamicButtons.ShowButton("Unsync", SplendidCRM.Crm.Modules.ExchangeFolders(m_sMODULE) && Security.HasExchangeAlias());
		}
		#endregion
	}
}


