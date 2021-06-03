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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Themes.Atlantic
{
	public partial class DefaultView : System.Web.UI.MasterPage
	{
		protected L10N         L10n;

		protected HtmlTable          tblLoginHeader;
		
		public bool PrintView
		{
			get
			{
				bool bPrintView = Sql.ToBoolean(Context.Items["PrintView"]);
				return bPrintView;
			}
		}
		public L10N GetL10n()
		{
			// 08/30/2005 Paul.  Attempt to get the L10n & T10n objects from the parent page. 
			// If that fails, then just create them because they are required. 
			if ( L10n == null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				// A port to DNN prompted this approach. 
				L10n = Context.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					string sCULTURE  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
					L10n = new L10N(sCULTURE);
				}
			}
			return L10n;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				try
				{
					// http://www.i18nguy.com/temp/rtl.html
					// 09/04/2013 Paul.  ASP.NET 4.5 is enforcing a rule that the root be an HtmlElement and not an HtmlGenericControl. 
					HtmlContainerControl htmlRoot = FindControl("htmlRoot") as HtmlContainerControl;
					if ( htmlRoot != null )
					{
						if ( L10n.IsLanguageRTL() )
						{
							htmlRoot.Attributes.Add("dir", "rtl");
						}
					}
				}
				catch
				{
				}
			}
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			// 08/30/2013 Paul.  Move jQuery registration to Utils class. 
			Utils.RegisterJQuery(Page, mgrAjax);
			// 08/25/2013 Paul.  Register Asterisk and Signal-R scripts. 
			// 09/20/2013 Paul.  Move EXTENSION to the main table. 
			if ( !Sql.IsEmptyString(Session["EXTENSION"]) )
			{
				AsteriskManager.RegisterScripts(Context, mgrAjax);
				// 12/03/2013 Paul.  Add support for Avaya. 
				AvayaManager.RegisterScripts(Context, mgrAjax);
			}
			// 09/27/2013 Paul.  SMS messages need to be opt-in. 
			if ( Sql.ToString(Session["SMS_OPT_IN"]) == "yes" )
				TwilioManager.RegisterScripts(Context, mgrAjax);
		}

		#region Web Form Designer generated code
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			GetL10n();
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}
		#endregion
	}
}


