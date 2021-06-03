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
using System.IO;
using System.Web;
using System.Threading;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SplendidCRM 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private Timer tSchedulerManager = null;
		private Timer tEmailManager     = null;

		public void InitSchedulerManager()
		{
			if ( tSchedulerManager == null )
			{
				// 05/19/2008 Paul.  The timer will fire every 5 minutes.  If decreased to 1 minute, then vwSCHEDULERS_Run must be modified to round to 1 minute. 
				// 10/30/2008 Paul.  The time now requires the Context be passed. 
				tSchedulerManager = new Timer(SchedulerUtils.OnTimer, this.Context, new TimeSpan(0, 1, 0), new TimeSpan(0, 5, 0));
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Scheduler Manager timer has been activated.");
			}
		}

		// 12/25/2012 Paul.  Use a separate timer for email reminders as they are timely and cannot be stuck behind other scheduler tasks. 
		public void InitEmailManager()
		{
			if ( tEmailManager == null )
			{
				tEmailManager = new Timer(EmailUtils.OnTimer, this.Context, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
				SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), "The Email Manager timer has been activated.");
			}
		}

		public Global()
		{
			InitializeComponent();
		}

		protected void Application_OnError(Object sender, EventArgs e)
		{
			//SplendidInit.Application_OnError();
		}
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			// 11/04/2008 Paul.  IIS7 does not provide access to Request object, so we cannot determine the database connection from the URL. 
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
			SplendidInit.InitSession(this.Context);
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			// 11/04/2008 Paul.  IIS7 does not provide access to Request object from Application_Start. Move code to Application_BeginRequest. 
			if ( Application.Count == 0 )
			{
				SplendidInit.InitApp(this.Context);
				WorkflowInit.StartRuntime(this.Application);
				InitSchedulerManager();
				InitEmailManager();
				// 08/28/2013 Paul.  Add support for Twilio and SignalR. 
				TwilioManager.InitApp(this.Context);
				// 11/10/2014 Paul.  Add ChatChannels support. 
				ChatManager.InitApp(this.Context);
				SignalRUtils.InitApp();
			}

			// 12/29/2005 Paul.  vCalendar support is not going to be easy.
			// Outlook will automatically use FrontPage extensions to place the file. 
			// When connecting to a Apache server, it will make HTTP GET/PUT requests. 
			/*
			string sPath = HttpContext.Current.Request.Path.ToLower();
			Regex regex = new Regex("/vcal_server/(\\w+)", RegexOptions.IgnoreCase);
			MatchCollection matches = regex.Matches(sPath);
			//if ( sPath.IndexOf("/vcal_server/") >= 0 )
			if ( matches.Count > 0 )
			{
				//sPath = sPath.Replace("/vcal_server/", "/vcal_server.aspx?");
				sPath = "~/vcal_server.aspx?" + matches[0].Groups[1].ToString();
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), sPath);
				HttpContext.Current.RewritePath(sPath);
			}
			*/
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AcquireRequestState(Object sender, EventArgs e)
		{
			// 03/04/2007 Paul.  The Session will be NULL during web service calls. 
			// We noticed this problem when AJAX failed in ScriptResource.axd. 
			if ( HttpContext.Current.Session != null )
			{
				// 02/28/2007 Paul.  Although Application_AuthenticateRequest might seem like the best place for this code,
				// we have to wait until the Session variables have been initialized to determine if the user has been authenticated. 
				if ( !Sql.IsEmptyString(HttpContext.Current.Session["USER_NAME"]) )
				{
					// 02/28/2007 Paul.  WebParts requires a valid User identity.  
					// We must store the USER_NAME as this will be the lookup key when updating preferences. 
					if ( !HttpContext.Current.User.Identity.IsAuthenticated )
						HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(Security.USER_NAME, "Forms"), null);
				}
			}
		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{
			// 03/02/2008 Paul.  Log the logout. 
			try
			{
				Guid gUSER_LOGIN_ID = Security.USER_LOGIN_ID;
				if ( !Sql.IsEmptyGuid(gUSER_LOGIN_ID) )
					SqlProcs.spUSERS_LOGINS_Logout(gUSER_LOGIN_ID);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}

			// 10/29/2006 Paul.  Delete temp files. 
			foreach ( string sKey in Session.Keys )
			{
				if ( sKey.StartsWith("TempFile.") )
				{
					string sTempFileName = Sql.ToString(Session[sKey]);
					string sTempPathName = Path.Combine(Path.GetTempPath(), sTempFileName);
					if ( File.Exists(sTempPathName) )
					{
						try
						{
							File.Delete(sTempPathName);
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), "Could not delete temp file: " + sTempPathName + ControlChars.CrLf + ex.Message);
						}
					}
				}
			}
		}

		protected void Application_End(Object sender, EventArgs e)
		{
			if ( tSchedulerManager != null )
				tSchedulerManager.Dispose();
			if ( tEmailManager != null )
				tEmailManager.Dispose();
			WorkflowInit.StopRuntime(this.Application);
			SplendidInit.StopApp(this.Context);
		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}


