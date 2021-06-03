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
using System.Diagnostics;
using System.Globalization;

namespace SplendidCRM.Calendar.html5
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		public DateTimeFormatInfo DateTimeFormat
		{
			get { return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat; }
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_MODULE_TITLE"));
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//ScriptReference scrJQuery         = new ScriptReference ("~/html5/jQuery/jquery-1.8.2.min.js"      );
				//ScriptReference scrJQueryUI       = new ScriptReference ("~/html5/jQuery/jquery-ui-1.9.1.custom.js");
				ScriptReference scrTimePicker     = new ScriptReference ("~/html5/jQuery/jquery-ui-timepicker-addon.js");
				// 02/22/2013 Paul.  Can't use min FullCalendar as we have customized the code. 
				ScriptReference scrFullCalendar   = new ScriptReference ("~/html5/FullCalendar/fullcalendar.js"    );
				ScriptReference scrGoogleCalUtil  = new ScriptReference ("~/html5/FullCalendar/gcal.js"            );
				// 08/28/2013 Paul.  json2.js now registered in the master pages. 
				//ScriptReference scrJSON           = new ScriptReference ("~/html5/JSON.js"                         );
				ScriptReference scrCalendarViewUI = new ScriptReference ("~/html5/SplendidUI/CalendarViewUI.js"    );
				ScriptReference scrUtility        = new ScriptReference ("~/html5/Utility.js"                      );
				ScriptReference scrFormatting     = new ScriptReference ("~/html5/SplendidUI/Formatting.js"        );
				ScriptReference scrSQL            = new ScriptReference ("~/html5/SplendidUI/Sql.js"               );
				
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//HtmlLink cssJQuery = new HtmlLink();
				//cssJQuery.Attributes.Add("href" , "~/html5/jQuery/jquery-ui-1.9.1.custom.css");
				//cssJQuery.Attributes.Add("type" , "text/css"  );
				//cssJQuery.Attributes.Add("rel"  , "stylesheet");
				//Page.Header.Controls.Add(cssJQuery);
				
				HtmlLink cssFullCalendar = new HtmlLink();
				cssFullCalendar.Attributes.Add("href" , "~/html5/FullCalendar/fullcalendar.css");
				cssFullCalendar.Attributes.Add("type" , "text/css"  );
				cssFullCalendar.Attributes.Add("rel"  , "stylesheet");
				Page.Header.Controls.Add(cssFullCalendar);
				
				HtmlLink cssFullCalendarPrint = new HtmlLink();
				cssFullCalendarPrint.Attributes.Add("href" , "~/html5/FullCalendar/fullcalendar.print.css");
				cssFullCalendarPrint.Attributes.Add("type" , "text/css"  );
				cssFullCalendarPrint.Attributes.Add("rel"  , "stylesheet");
				cssFullCalendarPrint.Attributes.Add("media", "print"     );
				Page.Header.Controls.Add(cssFullCalendarPrint);
				
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//if ( !mgrAjax.Scripts.Contains(scrJQuery        ) ) mgrAjax.Scripts.Add(scrJQuery        );
				//if ( !mgrAjax.Scripts.Contains(scrJQueryUI      ) ) mgrAjax.Scripts.Add(scrJQueryUI      );
				if ( !mgrAjax.Scripts.Contains(scrTimePicker    ) ) mgrAjax.Scripts.Add(scrTimePicker    );
				if ( !mgrAjax.Scripts.Contains(scrFullCalendar  ) ) mgrAjax.Scripts.Add(scrFullCalendar  );
				if ( !mgrAjax.Scripts.Contains(scrGoogleCalUtil ) ) mgrAjax.Scripts.Add(scrGoogleCalUtil );
				// 08/28/2013 Paul.  json2.js now registered in the master pages. 
				//if ( !mgrAjax.Scripts.Contains(scrJSON          ) ) mgrAjax.Scripts.Add(scrJSON          );
				if ( !mgrAjax.Scripts.Contains(scrCalendarViewUI) ) mgrAjax.Scripts.Add(scrCalendarViewUI);
				if ( !mgrAjax.Scripts.Contains(scrUtility       ) ) mgrAjax.Scripts.Add(scrUtility       );
				if ( !mgrAjax.Scripts.Contains(scrFormatting    ) ) mgrAjax.Scripts.Add(scrFormatting    );
				if ( !mgrAjax.Scripts.Contains(scrSQL           ) ) mgrAjax.Scripts.Add(scrSQL           );
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
			m_sMODULE = "Calendar";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}


