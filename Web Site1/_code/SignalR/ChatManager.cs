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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for ChatManager.
	/// </summary>
	public class ChatManager
	{
		#region Properties
		private HttpContext                Context ;
		private IHubConnectionContext      Clients { get; set; }

		// Singleton instance
		private static ChatManager _instance = null;

		public static ChatManager Instance
		{
			get { return _instance; }
		}
		#endregion

		#region Initialization
		public static void InitApp(HttpContext Context)
		{
			_instance = new ChatManager(Context, GlobalHost.ConnectionManager.GetHubContext<ChatManagerHub>().Clients);
		}

		public static void RegisterScripts(HttpContext Context, ScriptManager mgrAjax)
		{
			if ( mgrAjax != null )
			{
				if ( Utils.CachedFileExists(Context, "~/Include/javascript/ChatManagerHubJS.aspx") )
				{
					SignalRUtils.RegisterSignalR(mgrAjax);
					// 11/25/2014 Paul.  There is no reason to have teh Chat hub code in a file separate from ChatDashboardUI.js. 
					//ScriptReference scrChatManagerHub = new ScriptReference("~/Include/javascript/ChatManagerHubJS.aspx?" + Sql.ToString(Context.Application["SplendidVersion"]) + "_" + Sql.ToString(Context.Session["USER_SETTINGS/CULTURE"]));
					//if ( !mgrAjax.Scripts.Contains(scrChatManagerHub) )
					//	mgrAjax.Scripts.Add(scrChatManagerHub);
				}
			}
		}
		#endregion

		private object NullID(Guid gID)
		{
			return Sql.IsEmptyGuid(gID) ? null : gID.ToString();
		}

		private ChatManager(HttpContext Context, IHubConnectionContext clients)
		{
			this.Context = Context;
			this.Clients = clients;
		}

		// http://msdn.microsoft.com/en-us/library/system.datetime.ticks.aspx
		private static long UnixTicks(DateTime dt)
		{
			return (dt.Ticks - 621355968000000000) / 10000;
		}

		private static string ToJsonDate(object dt)
		{
			return "\\/Date(" + UnixTicks(Sql.ToDateTime(dt)).ToString() + ")\\/";
		}

		public void NewMessage(Guid gID)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(this.Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					if ( !Sql.IsEmptyGuid(gID) )
					{
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vwCHAT_MESSAGES" + ControlChars.CrLf
						     + " where ID = @ID       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AddParameter(cmd, "@ID", gID);
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									if ( dt.Rows.Count > 0 )
									{
										DataRow row = dt.Rows[0];
										Guid     gCHAT_CHANNEL_ID    = Sql.ToGuid    (row["CHAT_CHANNEL_ID"   ]);
										string   sNAME               = Sql.ToString  (row["NAME"              ]);
										string   sDESCRIPTION        = Sql.ToString  (row["DESCRIPTION"       ]);
										DateTime dtDATE_ENTERED      = Sql.ToDateTime(row["DATE_ENTERED"      ]);
										Guid     gCREATED_BY_ID      = Sql.ToGuid    (row["CREATED_BY_ID"     ]);
										string   sCREATED_BY         = Sql.ToString  (row["CREATED_BY"        ]);
										string   sCREATED_BY_PICTURE = Sql.ToString  (row["CREATED_BY_PICTURE"]);
										Guid     gPARENT_ID          = Sql.ToGuid    (row["PARENT_ID"         ]);
										string   sPARENT_TYPE        = Sql.ToString  (row["PARENT_TYPE"       ]);
										string   sPARENT_NAME        = Sql.ToString  (row["PARENT_NAME"       ]);
										Guid     gNOTE_ATTACHMENT_ID = Sql.ToGuid    (row["NOTE_ATTACHMENT_ID"]);
										string   sFILENAME           = Sql.ToString  (row["FILENAME"          ]);
										string   sFILE_EXT           = Sql.ToString  (row["FILE_EXT"          ]);
										string   sFILE_MIME_TYPE     = Sql.ToString  (row["FILE_MIME_TYPE"    ]);
										long     lFILE_SIZE          = Sql.ToLong    (row["FILE_SIZE"         ]);
										bool     bATTACHMENT_READY   = Sql.ToBoolean (row["ATTACHMENT_READY"  ]);
										
										Guid     gTIMEZONE        = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
										TimeZone T10n             = TimeZone.CreateTimeZone(gTIMEZONE);
										string   sDATE_ENTERED    = ToJsonDate(T10n.FromServerTime(dtDATE_ENTERED));
										Clients.Group(gCHAT_CHANNEL_ID.ToString()).newMessage(gCHAT_CHANNEL_ID, gID, sNAME, sDESCRIPTION, sDATE_ENTERED, NullID(gPARENT_ID), sPARENT_TYPE, sPARENT_NAME, NullID(gCREATED_BY_ID), sCREATED_BY, sCREATED_BY_PICTURE, NullID(gNOTE_ATTACHMENT_ID), sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, lFILE_SIZE, bATTACHMENT_READY);
										//Clients.All.allMessage(gCHAT_CHANNEL_ID, gID, sDESCRIPTION, dtDATE_ENTERED, gUSER_ID, sCREATED_BY, NullID(gPARENT_ID), sPARENT_TYPE);
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
			}
		}
	}
}


