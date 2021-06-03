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
using System.Web;
using System.Web.UI.WebControls;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for ExportHeader.
	/// </summary>
	public class ExportHeader : SplendidControl
	{
		public CommandEventHandler Command ;
		protected string       sModule         = String.Empty;
		protected string       sTitle          = String.Empty;
		protected DropDownList lstEXPORT_RANGE ;
		protected DropDownList lstEXPORT_FORMAT;
		protected Button       btnExport       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( Command != null )
				Command(this, e) ;
		}

		// 02/08/2008 Paul.  We need to determine if the export button has been clicked inside Page_Load. 
		public string ExportUniqueID
		{
			get
			{
				return btnExport.UniqueID;
			}
		}

		public string Module
		{
			get
			{
				return sModule;
			}
			set
			{
				sModule = value;
			}
		}

		public string Title
		{
			get
			{
				return sTitle;
			}
			set
			{
				sTitle = value;
			}
		}

		public string ExportRange
		{
			get
			{
				return lstEXPORT_RANGE.SelectedValue;
			}
		}

		public string ExportFormat
		{
			get
			{
				return lstEXPORT_FORMAT.SelectedValue;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				lstEXPORT_RANGE.Items.Add(new ListItem(L10n.Term(".LBL_LISTVIEW_OPTION_ENTIRE"  ), "All"     ));
				lstEXPORT_RANGE.Items.Add(new ListItem(L10n.Term(".LBL_LISTVIEW_OPTION_CURRENT" ), "Page"    ));
				lstEXPORT_RANGE.Items.Add(new ListItem(L10n.Term(".LBL_LISTVIEW_OPTION_SELECTED"), "Selected"));
				
				lstEXPORT_FORMAT.Items.Add(new ListItem(L10n.Term("Import.LBL_XML_SPREADSHEET"  ), "Excel"   ));
				lstEXPORT_FORMAT.Items.Add(new ListItem(L10n.Term("Import.LBL_XML"              ), "xml"     ));
				lstEXPORT_FORMAT.Items.Add(new ListItem(L10n.Term("Import.LBL_CUSTOM_CSV"       ), "csv"     ));
				lstEXPORT_FORMAT.Items.Add(new ListItem(L10n.Term("Import.LBL_CUSTOM_TAB"       ), "tab"     ));
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
		}
		#endregion
	}
}


