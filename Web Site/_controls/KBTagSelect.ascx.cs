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
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for KBTagSelect.
	/// </summary>
	public class KBTagSelect : SplendidControl
	{
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected String          sKBTAG_SET_LIST       ;
		protected Panel           pnlAddReplace         ;
		protected RadioButton     radKBTagSetReplace    ;
		protected RadioButton     radKBTagSetAdd        ;
		protected bool            bShowAddReplace       = false;
		protected bool            bEnabled              = true;
		protected short           nTagIndex             = 12;

		// 12/10/2017 Paul.  Provide a way to set the tab index. 
		public short TabIndex
		{
			get
			{
				return nTagIndex;
			}
			set
			{
				nTagIndex = value;
			}
		}

		public DataTable LineItems
		{
			get { return dtLineItems; }
			set { dtLineItems = value; }
		}

		public bool ShowAddReplace
		{
			get { return bShowAddReplace; }
			set { bShowAddReplace = value; }
		}

		public bool Enabled
		{
			get { return bEnabled; }
			set { bEnabled = value; }
		}

		public string KBTAG_SET_LIST
		{
			get { return String.Empty; }
			set { }
		}

		public bool ADD_KBTAG_SET
		{
			get { return false; }
		}

		public void InitTable()
		{
		}

		public void Clear()
		{
		}

		public void Validate()
		{
		}

		public void Validate(bool bEnabled)
		{
		}

		public void LoadLineItems(Guid gKBDOCUMENT_ID)
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
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

