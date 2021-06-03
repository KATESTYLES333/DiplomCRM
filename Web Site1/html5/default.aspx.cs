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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM.html5
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : SplendidPage
	{
		public Default()
		{
			this.PreInit += new EventHandler(Default_PreInit);
		}

		protected void Default_PreInit(object sender, EventArgs e)
		{
			// 11/13/2012 Paul.  Can't seem to disable theming, so just force the theme to Six as that is all that is supported. 
			this.Theme = "Six";
		}

		// 01/25/2008 Paul.  This page must be accessible without authentication. 
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			if ( !IsPostBack )
			{
				try
				{
					ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
					ChatManager.RegisterScripts(Context, mgrAjax);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
			if ( !Sql.ToBoolean(Request["debug"]) )
			{
				// 09/04/2013 Paul.  ASP.NET 4.5 is enforcing a rule that the root be an HtmlElement and not an HtmlGenericControl. 
				HtmlContainerControl htmlRoot = FindControl("htmlRoot") as HtmlContainerControl;
				if ( htmlRoot != null )
					htmlRoot.Attributes.Add("manifest", "manifest.aspx");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}


