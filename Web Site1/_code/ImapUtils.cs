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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Diagnostics;
using Koolwired.Imap;

namespace SplendidCRM
{
	public class ImapUtils
	{
		public static bool Validate(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID, StringBuilder sbErrors)
		{
			bool bValid = false;
			try
			{
				if ( Sql.IsEmptyString(sFOLDER_ID) )
					sFOLDER_ID = "INBOX";
				using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
				{
					connection.Open();
					ImapCommand command = new ImapCommand(connection);
					using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
					{
						authenticate.Login();
						ImapMailbox mailbox = command.Examine(sFOLDER_ID);
						sbErrors.AppendLine("Connection successful. " + mailbox.Exist.ToString() + " items in Inbox" + "<br />");
						bValid = true;
					}
				}
			}
			catch(Exception ex)
			{
				sbErrors.AppendLine(ex.Message);
			}
			return bValid;
		}

		private static void UpdateFolderTreeNodeCounts(ImapCommand command, XmlNode xFolder)
		{
			foreach ( XmlNode xChild in xFolder.ChildNodes )
			{
				int nTotalCount  = 0;
				int nUnreadCount = 0;
				string sMailbox = XmlUtil.GetNamedItem(xChild, "Id"  );
				string sName    = XmlUtil.GetNamedItem(xChild, "Name");
				ImapMailbox mailbox = command.Examine(sMailbox);
				// 07/17/2010 Paul.  The [Gmail] folder will not return a mailbox. 
				if ( mailbox != null )
				{
					nTotalCount  = mailbox.Exist;
					nUnreadCount = mailbox.Recent;
				}

				XmlUtil.SetSingleNodeAttribute(xFolder.OwnerDocument, xChild, "TotalCount" , nTotalCount .ToString());
				XmlUtil.SetSingleNodeAttribute(xFolder.OwnerDocument, xChild, "UnreadCount", nUnreadCount.ToString());
				if ( nUnreadCount > 0 )
					XmlUtil.SetSingleNodeAttribute(xFolder.OwnerDocument, xChild, "DisplayName", "<b>" + sName + "</b> <font color=blue>(" + nUnreadCount.ToString() + ")</font>");
				else
					XmlUtil.SetSingleNodeAttribute(xFolder.OwnerDocument, xChild, "DisplayName", sName);
			}
		}

		public static void UpdateFolderTreeNodeCounts(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, XmlNode xFolder)
		{
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					UpdateFolderTreeNodeCounts(command, xFolder);
				}
			}
		}

		private static void GetFolderTreeFromResults(ImapCommand command, XmlNode xParent, ImapFolderNode fResults)
		{
			XmlDocument xml = xParent.OwnerDocument;
			foreach (ImapFolderNode fld in fResults.Children )
			{
				XmlElement xChild = xml.CreateElement("Folder");
				xParent.AppendChild(xChild);
				
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "Id"         , fld.ToString());
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "TotalCount" , "0"      );
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "UnreadCount", "0"      );
				// 07/30/2010 Paul.  We need to separate the Name from the DisplayName due to the formatting differences. 
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "Name"       , fld.Value);
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "DisplayName", fld.Value);
				if ( fld.Children.Count > 0 )
				{
					GetFolderTreeFromResults(command, xChild, fld);
				}
			}
		}

		public static XmlDocument GetFolderTree(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
			xml.AppendChild(xml.CreateElement("Folders"));
			XmlUtil.SetSingleNodeAttribute(xml, xml.DocumentElement, "Id"         , String.Empty              );
			XmlUtil.SetSingleNodeAttribute(xml, xml.DocumentElement, "DisplayName", "Mailbox - " + sEMAIL_USER);
			
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					ImapFolder root = command.List();
					GetFolderTreeFromResults(command, xml.DocumentElement, root);
					UpdateFolderTreeNodeCounts(command, xml.DocumentElement);
				}
			}
			return xml;
		}

		public static void GetFolderCount(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID, ref int nTotalCount, ref int nUnreadCount)
		{
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					ImapMailbox mailbox = command.Examine(sFOLDER_ID);
					nTotalCount  = mailbox.Exist;
					nUnreadCount = mailbox.Recent;
					
				}
			}
		}

		public static void DeleteMessage(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID, string sUNIQUE_ID)
		{
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					command.Select(sFOLDER_ID);
					command.SetDeleted(Sql.ToInteger(sUNIQUE_ID), true);
				}
			}
		}

		public static DataTable GetMessage(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID, string sUNIQUE_ID)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ID"              , typeof(System.String  ));
			dt.Columns.Add("UNIQUE_ID"       , typeof(System.String  ));
			dt.Columns.Add("NAME"            , typeof(System.String  ));
			dt.Columns.Add("DATE_START"      , typeof(System.DateTime));
			dt.Columns.Add("FROM"            , typeof(System.String  ));
			dt.Columns.Add("FROM_ADDR"       , typeof(System.String  ));
			dt.Columns.Add("FROM_NAME"       , typeof(System.String  ));
			dt.Columns.Add("TO_ADDRS"        , typeof(System.String  ));
			dt.Columns.Add("CC_ADDRS"        , typeof(System.String  ));
			dt.Columns.Add("TYPE"            , typeof(System.String  ));
			dt.Columns.Add("STATUS"          , typeof(System.String  ));
			dt.Columns.Add("MESSAGE_ID"      , typeof(System.String  ));
			dt.Columns.Add("REPLY_TO_NAME"   , typeof(System.String  ));
			dt.Columns.Add("REPLY_TO_ADDR"   , typeof(System.String  ));
			dt.Columns.Add("DATE_ENTERED"    , typeof(System.DateTime));
			dt.Columns.Add("DATE_MODIFIED"   , typeof(System.DateTime));
			dt.Columns.Add("DESCRIPTION"     , typeof(System.String  ));
			dt.Columns.Add("DESCRIPTION_HTML", typeof(System.String  ));
			dt.Columns.Add("INTERNET_HEADERS", typeof(System.String  ));
			dt.Columns.Add("SIZE"            , typeof(System.Int32   ));
			dt.Columns.Add("SIZE_STRING"     , typeof(System.String  ));
			dt.Columns.Add("HAS_ATTACHMENTS" , typeof(System.Boolean ));
			dt.Columns.Add("IS_READ"         , typeof(System.Boolean ));
			dt.Columns.Add("CATEGORIES"      , typeof(System.String  ));
			dt.Columns.Add("ATTACHMENTS"     , typeof(System.String  ));
			DataRow row = dt.NewRow();
			dt.Rows.Add(row);
			
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					if ( Sql.IsEmptyString(sFOLDER_ID) )
						sFOLDER_ID = "INBOX";
					
					ImapMailbox mailbox = command.Select(sFOLDER_ID);
					int nUNIQUE_ID = Sql.ToInteger(sUNIQUE_ID);
					ImapMailboxMessage email = command.FetchBody(mailbox, nUNIQUE_ID);
					if ( email != null )
					{
						double dSize = email.Size;
						string sSize = String.Empty;
						if ( dSize < 1024 )
							sSize = dSize.ToString() + " B";
						else if ( dSize < 1024 * 1024 )
							sSize = Math.Floor(dSize / 1024).ToString() + " KB";
						else
							sSize = Math.Floor(dSize / (1024 * 1024)).ToString() + " MB";

						row["ID"                    ] = Guid.NewGuid().ToString().Replace('-', '_');
						row["UNIQUE_ID"             ] = email.UID                        ;
						row["SIZE"                  ] = email.Size                       ;
						row["SIZE_STRING"           ] = sSize                            ;
						row["IS_READ"               ] = email.Flags.Seen                 ;
						if ( email.To != null )
							row["TO_ADDRS"              ] = email.To                         ;
						if ( email.CC != null )
							row["CC_ADDRS"              ] = email.CC                         ;
						row["NAME"                  ] = email.Subject                    ;
						if ( !Sql.IsEmptyString(email.MessageID) )
							row["MESSAGE_ID"            ] = email.MessageID                  ;
						else
							row["MESSAGE_ID"            ] = email.Reference                  ;
						if ( email.Received != null )
						{
							row["DATE_MODIFIED"         ] = email.Received.ToLocalTime()     ;
							row["DATE_ENTERED"          ] = email.Received.ToLocalTime()     ;
							row["DATE_START"            ] = email.Received.ToLocalTime()     ;
						}
						else
						{
							row["DATE_MODIFIED"         ] = email.Sent.ToLocalTime()         ;
							row["DATE_ENTERED"          ] = email.Sent.ToLocalTime()         ;
							row["DATE_START"            ] = email.Sent.ToLocalTime()         ;
						}
						if ( email.From != null )
						{
							string sFrom = String.Empty;
							if ( Sql.IsEmptyString(email.From.Address) )
								sFrom = email.From.DisplayName;
							else if ( Sql.IsEmptyString(email.From.DisplayName) )
								sFrom = email.From.Address;
							else
								sFrom = String.Format("{0} <{1}>", email.From.DisplayName, email.From.Address);
							row["FROM"      ] = sFrom                 ;
							row["FROM_ADDR" ] = email.From.Address    ;
							row["FROM_NAME" ] = email.From.DisplayName;
						}
						if ( email.To != null && email.To.Count > 0 )
						{
							StringBuilder sbTO_ADDRS = new StringBuilder();
							foreach ( ImapAddress addr in email.To )
							{
								if ( sbTO_ADDRS.Length > 0 ) sbTO_ADDRS.Append(';');
								if ( Sql.IsEmptyString(addr.Address) )
									sbTO_ADDRS.Append(addr.DisplayName);
								else if ( Sql.IsEmptyString(addr.DisplayName) )
									sbTO_ADDRS.Append(addr.Address);
								else
									sbTO_ADDRS.Append(String.Format("{0} <{1}>", addr.DisplayName, addr.Address));
							}
							row["TO_ADDRS"] = sbTO_ADDRS.ToString();
						}
						if ( email.CC != null && email.CC.Count > 0 )
						{
							StringBuilder sbCC_ADDRS = new StringBuilder();
							foreach ( ImapAddress addr in email.CC )
							{
								if ( sbCC_ADDRS.Length > 0 ) sbCC_ADDRS.Append(';');
								if ( Sql.IsEmptyString(addr.Address) )
									sbCC_ADDRS.Append(addr.DisplayName);
								else if ( Sql.IsEmptyString(addr.DisplayName) )
									sbCC_ADDRS.Append(addr.Address);
								else
									sbCC_ADDRS.Append(String.Format("{0} <{1}>", addr.DisplayName, addr.Address));
							}
							row["CC_ADDRS"] = sbCC_ADDRS.ToString();
						}
						
						XmlDocument xmlInternetHeaders = new XmlDocument();
						xmlInternetHeaders.AppendChild(xmlInternetHeaders.CreateElement("Headers"));
						string[] arrHeaders = command.FetchHeaders(email.UID);
						for ( int i = 0; i < arrHeaders.Length; i++ )
						{
							XmlElement xHeader = xmlInternetHeaders.CreateElement("Header");
							xmlInternetHeaders.DocumentElement.AppendChild(xHeader);
							XmlElement xName  = xmlInternetHeaders.CreateElement("Name" );
							XmlElement xValue = xmlInternetHeaders.CreateElement("Value");
							xHeader.AppendChild(xName );
							xHeader.AppendChild(xValue);
							int nSeparator = arrHeaders[i].IndexOf(':');
							string sName  = String.Empty;
							string sValue = arrHeaders[i];
							if ( nSeparator >= 0 )
							{
								sName  = arrHeaders[i].Substring(0, nSeparator);
								sValue = arrHeaders[i].Substring(nSeparator + 1);
							}
							xName .InnerText = sName ;
							xValue.InnerText = sValue;
						}
						row["INTERNET_HEADERS"] = xmlInternetHeaders.OuterXml;
						if ( email.HasText )
						{
							command.FetchBodyPart(email, email.Text);
							row["DESCRIPTION"     ] = email.BodyParts[email.Text].DataString();
						}
						if ( email.HasHTML )
						{
							command.FetchBodyPart(email, email.HTML);
							row["DESCRIPTION_HTML"] = email.BodyParts[email.HTML].DataString();
						}
						if ( email.BodyParts != null )
						{
							foreach ( ImapMessageBodyPart part in email.BodyParts )
							{
								// 11/06/2010 Paul.  We do want to include the Inline images in this attachment flag. 
								if ( part.Attachment )
								{
									row["HAS_ATTACHMENTS"] = true;
									break;
								}
							}
							if ( Sql.ToBoolean(row["HAS_ATTACHMENTS"]) )
								row["ATTACHMENTS"] = GetAttachments(email);
						}
					}
				}
			}
			return dt;
		}

		// 11/06/2010 Paul.  Return the Attachments so that we can show embedded images or download the attachments. 
		public static string GetAttachments(ImapMailboxMessage email)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			xml.AppendChild(xml.CreateElement("Attachments"));
			for ( int i = 0; i < email.BodyParts.Count; i++ )
			{
				ImapMessageBodyPart part = email.BodyParts[i];
				if ( part.Attachment )
				{
					XmlNode xAttachment = xml.CreateElement("Attachment");
					xml.DocumentElement.AppendChild(xAttachment);
					XmlUtil.SetSingleNode(xml, xAttachment, "ID"                , i.ToString()                   );
					XmlUtil.SetSingleNode(xml, xAttachment, "Name"              , part.FileName                  );
					XmlUtil.SetSingleNode(xml, xAttachment, "IsInline"          , part.Inline.ToString()         );
					XmlUtil.SetSingleNode(xml, xAttachment, "FileName"          , part.FileName                  );
					XmlUtil.SetSingleNode(xml, xAttachment, "Size"              , part.Size.ToString()           );
					XmlUtil.SetSingleNode(xml, xAttachment, "MediaType"         , part.ContentType.MediaType     );
					XmlUtil.SetSingleNode(xml, xAttachment, "CharSet"           , part.ContentType.CharSet       );
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentType"       , part.ContentType.ToString()    );
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentID"         , Sql.ToString(part.ContentID)   );  // 05/06/2014 Paul.  The ContentID might be NULL. 
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentDescription", part.ContentDescription        );
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentEncoding"   , part.ContentEncoding.ToString());
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentMD5"        , part.ContentMD5                );
					XmlUtil.SetSingleNode(xml, xAttachment, "ContentLanguage"   , part.ContentLanguage           );
					XmlUtil.SetSingleNode(xml, xAttachment, "Disposition"       , part.Disposition               );
					XmlUtil.SetSingleNode(xml, xAttachment, "Boundary"          , part.Boundary                  );
					XmlUtil.SetSingleNode(xml, xAttachment, "Location"          , part.Location                  );
					XmlUtil.SetSingleNode(xml, xAttachment, "LastModifiedTime"  , part.LastModifiedTime.ToLocalTime().ToString());
				}
			}
			return xml.OuterXml;
		}

		public static byte[] GetAttachmentData(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID, string sUNIQUE_ID, int nATTACHMENT_ID, ref string sFILENAME, ref string sCONTENT_TYPE, ref bool bINLINE)
		{
			byte[] byDataBinary = null;
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					if ( Sql.IsEmptyString(sFOLDER_ID) )
						sFOLDER_ID = "INBOX";
					
					ImapMailbox mailbox = command.Select(sFOLDER_ID);
					int nUNIQUE_ID = Sql.ToInteger(sUNIQUE_ID);
					ImapMailboxMessage email = null;
					bool bLoadSuccessful = false;
					try
					{
						email = command.FetchBody(mailbox, nUNIQUE_ID);
						bLoadSuccessful = true;
					}
					catch
					{
					}
					if ( email != null && bLoadSuccessful )
					{
						if ( email.BodyParts != null && nATTACHMENT_ID < email.BodyParts.Count )
						{
							ImapMessageBodyPart part = email.BodyParts[nATTACHMENT_ID];
							if ( part.Attachment )
							{
								bINLINE       = part.Inline;
								sCONTENT_TYPE = part.ContentType.MediaType;
								sFILENAME     = Path.GetFileName(part.FileName);
								if ( Sql.IsEmptyString(sFILENAME) )
									sFILENAME = part.Disposition;
								command.FetchBodyPart(email, nATTACHMENT_ID);
								byDataBinary = part.GetBytes();
							}
						}
					}
				}
			}
			return byDataBinary;
		}

		public static DataTable GetFolderMessages(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sFOLDER_ID)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ID"              , typeof(System.String  ));
			dt.Columns.Add("UNIQUE_ID"       , typeof(System.String  ));
			dt.Columns.Add("NAME"            , typeof(System.String  ));
			dt.Columns.Add("DATE_START"      , typeof(System.DateTime));
			dt.Columns.Add("FROM"            , typeof(System.String  ));
			dt.Columns.Add("FROM_ADDR"       , typeof(System.String  ));
			dt.Columns.Add("FROM_NAME"       , typeof(System.String  ));
			dt.Columns.Add("TO_ADDRS"        , typeof(System.String  ));
			dt.Columns.Add("CC_ADDRS"        , typeof(System.String  ));
			dt.Columns.Add("TYPE"            , typeof(System.String  ));
			dt.Columns.Add("STATUS"          , typeof(System.String  ));
			dt.Columns.Add("MESSAGE_ID"      , typeof(System.String  ));
			dt.Columns.Add("REPLY_TO_NAME"   , typeof(System.String  ));
			dt.Columns.Add("REPLY_TO_ADDR"   , typeof(System.String  ));
			dt.Columns.Add("DATE_ENTERED"    , typeof(System.DateTime));
			dt.Columns.Add("DATE_MODIFIED"   , typeof(System.DateTime));
			dt.Columns.Add("DESCRIPTION"     , typeof(System.String  ));
			dt.Columns.Add("INTERNET_HEADERS", typeof(System.String  ));
			dt.Columns.Add("SIZE"            , typeof(System.Int32   ));
			dt.Columns.Add("SIZE_STRING"     , typeof(System.String  ));
			dt.Columns.Add("HAS_ATTACHMENTS" , typeof(System.Boolean ));
			dt.Columns.Add("IS_READ"         , typeof(System.Boolean ));
			dt.Columns.Add("CATEGORIES"      , typeof(System.String  ));
			
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					if ( Sql.IsEmptyString(sFOLDER_ID) )
						sFOLDER_ID = "INBOX";
					
					ImapMailbox mailbox = command.Select(sFOLDER_ID);
					// 11/06/2010 Paul.  Instead of using ALL_UID, we should use FULL_UID so that we get the body parts. 
					command.Fetch(mailbox, true);
					if ( mailbox != null && mailbox.Messages != null )
					{
						foreach ( ImapMailboxMessage email in mailbox.Messages )
						{
							DataRow row = dt.NewRow();
							dt.Rows.Add(row);
							double dSize = email.Size;
							string sSize = String.Empty;
							if ( dSize < 1024 )
								sSize = dSize.ToString() + " B";
							else if ( dSize < 1024 * 1024 )
								sSize = Math.Floor(dSize / 1024).ToString() + " KB";
							else
								sSize = Math.Floor(dSize / (1024 * 1024)).ToString() + " MB";

							row["ID"                    ] = Guid.NewGuid().ToString().Replace('-', '_');
							row["UNIQUE_ID"             ] = email.UID                        ;
							row["SIZE"                  ] = email.Size                       ;
							row["SIZE_STRING"           ] = sSize                            ;
							row["IS_READ"               ] = email.Flags.Seen                 ;
							if ( email.To != null )
								row["TO_ADDRS"              ] = email.To                         ;
							if ( email.CC != null )
								row["CC_ADDRS"              ] = email.CC                         ;
							row["NAME"                  ] = email.Subject                    ;
							if ( !Sql.IsEmptyString(email.MessageID) )
								row["MESSAGE_ID"            ] = email.MessageID                  ;
							else
								row["MESSAGE_ID"            ] = email.Reference                  ;
							if ( email.Received != null )
							{
								row["DATE_MODIFIED"         ] = email.Received.ToLocalTime()     ;
								row["DATE_ENTERED"          ] = email.Received.ToLocalTime()     ;
								row["DATE_START"            ] = email.Received.ToLocalTime()     ;
							}
							else
							{
								row["DATE_MODIFIED"         ] = email.Sent.ToLocalTime()         ;
								row["DATE_ENTERED"          ] = email.Sent.ToLocalTime()         ;
								row["DATE_START"            ] = email.Sent.ToLocalTime()         ;
							}
							if ( email.From != null )
							{
								string sFrom = String.Empty;
								if ( Sql.IsEmptyString(email.From.Address) )
									sFrom = email.From.DisplayName;
								else if ( Sql.IsEmptyString(email.From.DisplayName) )
									sFrom = email.From.Address;
								else
									sFrom = String.Format("{0} &lt;{1}&gt;", email.From.DisplayName, email.From.Address);
								row["FROM"      ] = sFrom                 ;
								row["FROM_ADDR" ] = email.From.Address    ;
								row["FROM_NAME" ] = email.From.DisplayName;
							}
							// 07/17/2010 Paul.  Instead of using ALL_UID, we should use FULL_UID so that we get the body parts. 
							if ( email.BodyParts != null )
							{
								foreach ( ImapMessageBodyPart part in email.BodyParts )
								{
									// 11/06/2010 Paul.  Don't include inline images as attachments here. 
									if ( part.Attachment && !part.Inline )
									{
										row["HAS_ATTACHMENTS"] = true;
										break;
									}
								}
							}
						}
					}
				}
			}
			return dt;
		}

		public static Guid ImportMessage(HttpContext Context, HttpSessionState Session, string sPARENT_TYPE, Guid gPARENT_ID, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, Guid gUSER_ID, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sFOLDER_ID, string sUNIQUE_ID)
		{
			Guid gEMAIL_ID = Guid.Empty;
			HttpApplicationState Application = Context.Application;
			long   lUploadMaxSize  = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
			string sCULTURE        = L10N.NormalizeCulture(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			
			using ( ImapConnect connection = new ImapConnect(sSERVER_URL, nPORT, bMAILBOX_SSL) )
			{
				connection.Open();
				ImapCommand command = new ImapCommand(connection);
				using ( ImapAuthenticate authenticate = new ImapAuthenticate(connection, sEMAIL_USER, sEMAIL_PASSWORD) )
				{
					authenticate.Login();
					if ( Sql.IsEmptyString(sFOLDER_ID) )
						sFOLDER_ID = "INBOX";
					
					ImapMailbox mailbox = command.Select(sFOLDER_ID);
					int nUNIQUE_ID = Sql.ToInteger(sUNIQUE_ID);
					ImapMailboxMessage email = null;
					bool bLoadSuccessful = false;
					try
					{
						email = command.FetchBody(mailbox, nUNIQUE_ID);
						bLoadSuccessful = true;
					}
					catch(Exception ex)
					{
						string sError = "Error loading email for " + sEMAIL_USER + ", " + sUNIQUE_ID + "." + ControlChars.CrLf;
						sError += Utils.ExpandException(ex) + ControlChars.CrLf;
						SyncError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), sError);
					}
					if ( email != null && bLoadSuccessful )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							
							string sSQL = String.Empty;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								DateTime dtREMOTE_DATE_MODIFIED_UTC = email.Received;
								// 11/06/2010 Paul.  Use simple ToLocalTime() instead of the convert function as it is prone to throwing errors. 
								// The conversion could not be completed because the supplied DateTime did not have the Kind property set correctly. 
								// For example, when the Kind property is DateTimeKind.Local, the source time zone must be TimeZoneInfo.Local.
								//DateTime dtREMOTE_DATE_MODIFIED     = TimeZoneInfo.ConvertTimeFromUtc(dtREMOTE_DATE_MODIFIED_UTC, TimeZoneInfo.Local);
								DateTime dtREMOTE_DATE_MODIFIED     = dtREMOTE_DATE_MODIFIED_UTC.ToLocalTime();
								
								cmd.Parameters.Clear();
								// 04/22/2010 Paul.  Always lookup the Contact. 
								Guid   gCONTACT_ID     = Guid.Empty;
								Guid   gSENDER_USER_ID = gUSER_ID;
								sSQL = "select ID                      " + ControlChars.CrLf
								     + "  from vwCONTACTS              " + ControlChars.CrLf
								     + " where EMAIL1 = @CONTACT_EMAIL1" + ControlChars.CrLf;
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@CONTACT_EMAIL1", email.From.Address);
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										gCONTACT_ID = Sql.ToGuid(rdr["ID"]);
									}
								}
								
								string sREPLY_TO_NAME = String.Empty;
								string sREPLY_TO_ADDR = String.Empty;
								if ( email.ReplyTo != null )
								{
									sREPLY_TO_NAME = email.ReplyTo.DisplayName;
									sREPLY_TO_ADDR = email.ReplyTo.Address    ;
								}
								
								StringBuilder sbTO_ADDRS_IDS    = new StringBuilder();
								StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
								StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
								if ( email.To != null && email.To.Count > 0 )
								{
									foreach ( ImapAddress addr in email.To )
									{
										if ( sbTO_ADDRS_NAMES .Length > 0 ) sbTO_ADDRS_NAMES .Append(';');
										if ( sbTO_ADDRS_EMAILS.Length > 0 ) sbTO_ADDRS_EMAILS.Append(';');
										sbTO_ADDRS_NAMES .Append(addr.DisplayName);
										sbTO_ADDRS_EMAILS.Append(addr.Address    );
										// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
										// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
										Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
										if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
										{
											if ( sbTO_ADDRS_IDS.Length > 0 )
												sbTO_ADDRS_IDS.Append(';');
											sbTO_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
										}
									}
								}
								StringBuilder sbCC_ADDRS_IDS    = new StringBuilder();
								StringBuilder sbCC_ADDRS_NAMES  = new StringBuilder();
								StringBuilder sbCC_ADDRS_EMAILS = new StringBuilder();
								if ( email.CC != null && email.CC.Count > 0 )
								{
									foreach ( ImapAddress addr in email.CC )
									{
										if ( sbCC_ADDRS_NAMES .Length > 0 ) sbCC_ADDRS_NAMES .Append(';');
										if ( sbCC_ADDRS_EMAILS.Length > 0 ) sbCC_ADDRS_EMAILS.Append(';');
										sbCC_ADDRS_NAMES .Append(addr.DisplayName);
										sbCC_ADDRS_EMAILS.Append(addr.Address    );
										// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
										// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
										Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
										if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
										{
											if ( sbCC_ADDRS_IDS.Length > 0 )
												sbCC_ADDRS_IDS.Append(';');
											sbCC_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
										}
									}
								}
								StringBuilder sbBCC_ADDRS_IDS    = new StringBuilder();
								StringBuilder sbBCC_ADDRS_NAMES  = new StringBuilder();
								StringBuilder sbBCC_ADDRS_EMAILS = new StringBuilder();
								if ( email.BCC != null && email.BCC.Count > 0 )
								{
									foreach ( ImapAddress addr in email.BCC )
									{
										if ( sbBCC_ADDRS_NAMES .Length > 0 ) sbBCC_ADDRS_NAMES .Append(';');
										if ( sbBCC_ADDRS_EMAILS.Length > 0 ) sbBCC_ADDRS_EMAILS.Append(';');
										sbBCC_ADDRS_NAMES .Append(addr.DisplayName);
										sbBCC_ADDRS_EMAILS.Append(addr.Address    );
										// 07/18/2010 Paul.  Exchange, Imap and Pop3 utils will all use this method to lookup a contact by email. 
										// 08/30/2010 Paul.  The previous method only returned Contacts, where as this new method returns Contacts, Leads and Prospects. 
										Guid gRECIPIENT_ID = Crm.Emails.RecipientByEmail(con, addr.Address);
										if ( !Sql.IsEmptyGuid(gRECIPIENT_ID) )
										{
											if ( sbBCC_ADDRS_IDS.Length > 0 )
												sbBCC_ADDRS_IDS.Append(';');
											sbBCC_ADDRS_IDS.Append(gRECIPIENT_ID.ToString());
										}
									}
								}
								string[] arrHeaders = command.FetchHeaders(email.UID);
								string sDESCRIPTION      = String.Empty;
								string sDESCRIPTION_HTML = String.Empty;
								if ( email.HasText )
								{
									command.FetchBodyPart(email, email.Text);
									sDESCRIPTION = email.BodyParts[email.Text].DataString();
								}
								if ( email.HasHTML )
								{
									command.FetchBodyPart(email, email.HTML);
									sDESCRIPTION_HTML = email.BodyParts[email.HTML].DataString();
								}
								// 11/04/2010 Paul.  Fetch the body parts outside the SQL transaction. 
								bool bHAS_ATTACHMENTS = false;
								if ( email.BodyParts != null )
								{
									for ( int i = 0; i < email.BodyParts.Count; i++ )
									{
										ImapMessageBodyPart part = email.BodyParts[i];
										if ( part.Attachment )
										{
											bHAS_ATTACHMENTS = true;
											command.FetchBodyPart(email, i);
										}
									}
								}
								
								// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
								string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
								string sFileURL = sSiteURL + "Images/EmailImage.aspx?ID=";
								
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										// 11/06/2010 Paul.  Set ID so that it can be used with inline images. 
										gEMAIL_ID = Guid.NewGuid();
										for ( int i = 0; i < email.BodyParts.Count; i++ )
										{
											ImapMessageBodyPart part = email.BodyParts[i];
											// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
											if ( part.Attachment && part.Inline )
											{
												// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
												// Using part.DataBinary only works when the data is Base64. 
												byte[] byDataBinary = part.GetBytes();
												if ( byDataBinary != null )
												{
													// 01/13/2008 Paul.  We may need to convert the encoding to UTF8. 
													// att.NameEncoding;
													string sFILENAME       = String.Empty;
													string sFILE_EXT       = String.Empty;
													string sFILE_MIME_TYPE = part.ContentType.MediaType;
													// 11/04/2010 Paul.  Path.GetExtension() can throw an exception of it includes bad characters. 
													// This issue was first encountered with the invalid parsing of the Imap body parts. 
													try
													{
														sFILENAME       = Path.GetFileName (part.FileName);
														if ( Sql.IsEmptyString(sFILENAME) )
															sFILENAME = part.Disposition;
														sFILE_EXT       = Path.GetExtension(sFILENAME);
													}
													catch
													{
													}
												
													Guid gIMAGE_ID = Guid.Empty;
													SqlProcs.spEMAIL_IMAGES_Insert
														( ref gIMAGE_ID
														, gEMAIL_ID
														, sFILENAME
														, sFILE_EXT
														, sFILE_MIME_TYPE
														, trn
														);
													// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
													Crm.EmailImages.LoadFile(gIMAGE_ID, byDataBinary, trn);
													// 11/06/2010 Paul.  Now replace the ContentId with the new Image URL. 
													// 05/06/2014 Paul.  The ContentID might be NULL. 
													string sContentID = Sql.ToString(part.ContentID);
													if ( sContentID.StartsWith("<") && sContentID.EndsWith(">") )
														sContentID = sContentID.Substring(1, sContentID.Length - 2);
													sDESCRIPTION_HTML = sDESCRIPTION_HTML.Replace("cid:" + sContentID, sFileURL + gIMAGE_ID.ToString());
												}
											}
										}
										SqlProcs.spEMAILS_Update
											( ref gEMAIL_ID
											, gASSIGNED_USER_ID
											, email.Subject
											, email.Received.ToLocalTime()
											, sPARENT_TYPE
											, gPARENT_ID
											, sDESCRIPTION
											, sDESCRIPTION_HTML
											, email.From.DisplayName
											, email.From.Address
											, (email.To != null) ? email.To.ToString() : String.Empty
											, (email.CC != null) ? email.CC.ToString() : String.Empty
											, String.Empty
											, sbTO_ADDRS_IDS    .ToString()
											, sbTO_ADDRS_NAMES  .ToString()
											, sbTO_ADDRS_EMAILS .ToString()
											, sbCC_ADDRS_IDS    .ToString()
											, sbCC_ADDRS_NAMES  .ToString()
											, sbCC_ADDRS_EMAILS .ToString()
											, sbBCC_ADDRS_IDS   .ToString()
											, sbBCC_ADDRS_NAMES .ToString()
											, sbBCC_ADDRS_EMAILS.ToString()
											, "archived"
											, email.MessageID // MESSAGE_ID
											, sREPLY_TO_NAME
											, sREPLY_TO_ADDR
											, String.Empty  // INTENT
											, Guid.Empty    // MAILBOX_ID
											, gTEAM_ID
											, sTEAM_SET_LIST
											, trn
											);
										
										// 08/31/2010 Paul.  The EMAILS_SYNC table was renamed to EMAIL_CLIENT_SYNC to prevent conflict with Offline Client sync tables. 
										SqlProcs.spEMAIL_CLIENT_SYNC_Update(gUSER_ID, gEMAIL_ID, sUNIQUE_ID, sPARENT_TYPE, gPARENT_ID, dtREMOTE_DATE_MODIFIED, dtREMOTE_DATE_MODIFIED_UTC, trn);
										// 04/01/2010 Paul.  Always add the current user to the email. 
										SqlProcs.spEMAILS_USERS_Update(gEMAIL_ID, gUSER_ID, trn);
										// 04/01/2010 Paul.  Always lookup and assign the contact. 
										if ( !Sql.IsEmptyGuid(gCONTACT_ID) )
										{
											SqlProcs.spEMAILS_CONTACTS_Update(gEMAIL_ID, gCONTACT_ID, trn);
										}
										if ( bHAS_ATTACHMENTS )
										{
											// 03/31/2010 Paul.  Web do not need to load the attachments separately. 
											// email.Load(new PropertySet(ItemSchema.Attachments));
											for ( int i = 0; i < email.BodyParts.Count; i++ )
											{
												ImapMessageBodyPart part = email.BodyParts[i];
												// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
												if ( part.Attachment && !part.Inline )
												{
													// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
													// Using part.DataBinary only works when the data is Base64. 
													byte[] byDataBinary = part.GetBytes();
													if ( byDataBinary != null )
													{
														// 04/01/2010 Paul.  file.Size is only available on Exchange 2010. 
														long lFileSize = byDataBinary.Length;  // file.Size;
														if ( (lUploadMaxSize == 0) || (lFileSize <= lUploadMaxSize) )
														{
															string sFILENAME       = String.Empty;
															string sFILE_EXT       = String.Empty;
															string sFILE_MIME_TYPE = part.ContentType.MediaType;
															// 11/04/2010 Paul.  Path.GetExtension() can throw an exception of it includes bad characters. 
															// This issue was first encountered with the invalid parsing of the Imap body parts. 
															try
															{
																sFILENAME       = Path.GetFileName (part.FileName);
																if ( Sql.IsEmptyString(sFILENAME) )
																	sFILENAME = part.Disposition;
																sFILE_EXT       = Path.GetExtension(sFILENAME);
															}
															catch
															{
															}
															
															Guid gNOTE_ID = Guid.Empty;
															// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
															SqlProcs.spNOTES_Update
																( ref gNOTE_ID
																, L10N.Term(Application, sCULTURE, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
																, "Emails"   // Parent Type
																, gEMAIL_ID  // Parent ID
																, Guid.Empty
																, String.Empty
																, gTEAM_ID       // TEAM_ID
																, sTEAM_SET_LIST // TEAM_SET_LIST
																, gASSIGNED_USER_ID
																, trn
																);
															
															Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
															SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, part.FileName, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
															//SqlProcs.spNOTES_ATTACHMENT_Update(gNOTE_ATTACHMENT_ID, byDataBinary, trn);
															// 11/06/2010 Paul.  Use our streamable function. 
															// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
															Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, byDataBinary, trn);
														}
													}
												}
											}
										}
										trn.Commit();
									}
									catch
									{
										trn.Rollback();
										throw;
									}
								}
							}
						}
					}
				}
			}
			return gEMAIL_ID;
		}

		// 01/26/2013 Paul.  The SplendidCRM email header can be found in the body of a bounced message, so consider it as a tracker key. 
		private static string[] arrTrackers = new string[] { "/RemoveMe.aspx?identifier=", "/campaign_trackerv2.aspx?identifier=", "/image.aspx?identifier=", "X-SplendidCRM-ID: " };

		// 11/04/2010 Paul.  Fetch the body parts before the call to FindTargetTrackerKey. 
		public static Guid FindTargetTrackerKey(ImapMailboxMessage email, string[] arrHeaders, string sDESCRIPTION, string sDESCRIPTION_HTML)
		{
			Guid gTARGET_TRACKER_KEY = Guid.Empty;
			foreach ( string sHeader in arrHeaders )
			{
				if ( sHeader.StartsWith("x-splendidcrm-id:", StringComparison.CurrentCultureIgnoreCase) )
				{
					gTARGET_TRACKER_KEY = Sql.ToGuid(sHeader.Split(':')[1]);
				}
			}
			if ( Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
			{
				// 01/13/2008 Paul.  Now look for a RemoveMe tracker, or any of the other expected trackers. 
				if ( sDESCRIPTION != null )
				{
					foreach ( string sTracker in arrTrackers )
					{
						int nStartTracker = sDESCRIPTION.IndexOf(sTracker);
						if ( nStartTracker > 0 )
						{
							nStartTracker += sTracker.Length;
							gTARGET_TRACKER_KEY = Sql.ToGuid(sDESCRIPTION.Substring(nStartTracker, 36));
							if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
								return gTARGET_TRACKER_KEY;
						}
					}
				}
				if ( sDESCRIPTION_HTML != null )
				{
					foreach ( string sTracker in arrTrackers )
					{
						int nStartTracker = sDESCRIPTION_HTML.IndexOf(sTracker);
						if ( nStartTracker > 0 )
						{
							nStartTracker += sTracker.Length;
							gTARGET_TRACKER_KEY = Sql.ToGuid(sDESCRIPTION_HTML.Substring(nStartTracker, 36));
							if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
								return gTARGET_TRACKER_KEY;
						}
					}
				}
				// 01/20/2008 Paul.  In a bounce, the server messages will be stored in entities. 
				for ( int i = 0; i < email.BodyParts.Count; i++ )
				{
					// text/plain
					// message/delivery-status
					// message/rfc822
					ImapMessageBodyPart part = email.BodyParts[i];
					if ( part.Attachment && (part.ContentType != null) && (part.ContentType.MediaType == "text/plain" || part.ContentType.MediaType == "message/rfc822") )
					{
						// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
						// Using part.DataBinary only works when the data is Base64. 
						string sAttachmentBody = part.DataString();
						foreach ( string sTracker in arrTrackers )
						{
							int nStartTracker = sAttachmentBody.IndexOf(sTracker);
							if ( nStartTracker > 0 )
							{
								nStartTracker += sTracker.Length;
								gTARGET_TRACKER_KEY = Sql.ToGuid(sAttachmentBody.Substring(nStartTracker, 36));
								if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
									return gTARGET_TRACKER_KEY;
							}
						}
					}
				}
			}
			return gTARGET_TRACKER_KEY;
		}

		// 07/19/2010 Paul.  Moved ImportInboundEmail to ImapUtils. 
		public static void ImportInboundEmail(HttpContext Context, IDbConnection con, ImapCommand command, ImapMailboxMessage email, Guid gMAILBOX_ID, string sINTENT, Guid gGROUP_ID, string sUNIQUE_MESSAGE_ID)
		{
			try
			{
				// 11/04/2010 Paul.  Pull the IMAP calls out of the  SQL Transaction. 
				string[] arrHeaders = command.FetchHeaders(email.UID);
				string sDESCRIPTION      = String.Empty;
				string sDESCRIPTION_HTML = String.Empty;
				bool   bHAS_ATTACHMENTS  = false;
				if ( email.HasText )
				{
					command.FetchBodyPart(email, email.Text);
					sDESCRIPTION = email.BodyParts[email.Text].DataString();
				}
				if ( email.HasHTML )
				{
					command.FetchBodyPart(email, email.HTML);
					sDESCRIPTION_HTML = email.BodyParts[email.HTML].DataString();
				}
				// 01/13/2008 Paul.  First look for our special header. 
				// Our special header will only exist if the email is a bounce. 
				Guid gTARGET_TRACKER_KEY = Guid.Empty;
				// 01/13/2008 Paul.  The header will always be in lower case. 
				// 01/20/2008 Paul.  email.DeliveredTo can be NULL. 
				// 01/20/2008 Paul.  Filter the XSS tags before inserting the email. 
				// 01/23/2008 Paul.  DateTime in the email is in universal time. 
				
				// 11/04/2010 Paul.  Fetch the body parts before the call to FindTargetTrackerKey. 
				if ( email.BodyParts != null )
				{
					for ( int i = 0; i < email.BodyParts.Count; i++ )
					{
						ImapMessageBodyPart part = email.BodyParts[i];
						if ( part.Attachment )
						{
							bHAS_ATTACHMENTS = true;
							command.FetchBodyPart(email, i);
						}
					}
				}
				string sSAFE_BODY_PLAIN = EmailUtils.XssFilter(sDESCRIPTION     , Sql.ToString(Context.Application["CONFIG.email_xss"]));
				string sSAFE_BODY_HTML  = EmailUtils.XssFilter(sDESCRIPTION_HTML, Sql.ToString(Context.Application["CONFIG.email_xss"]));
				gTARGET_TRACKER_KEY = FindTargetTrackerKey(email, arrHeaders, sDESCRIPTION, sDESCRIPTION_HTML);
				
				string sEMAIL_TYPE = "inbound";
				string sSTATUS     = "unread";
				// 07/30/2008 Paul.  Lookup the default culture. 
				string sCultureName = SplendidDefaults.Culture(Context.Application);

				StringBuilder sbTO_ADDRS        = new StringBuilder();
				StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
				StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
				if ( email.To != null )
				{
					foreach ( ImapAddress addr in email.To )
					{
						// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
						sbTO_ADDRS.Append((sbTO_ADDRS.Length > 0) ? "; " : String.Empty);
						sbTO_ADDRS.Append(addr.ToString());

						sbTO_ADDRS_NAMES.Append((sbTO_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
						string sDisplayName = addr.DisplayName;
						if ( sDisplayName.StartsWith("\"") && sDisplayName.EndsWith("\"") || sDisplayName.StartsWith("\'") && sDisplayName.EndsWith("\'") )
							sDisplayName = sDisplayName.Substring(1, sDisplayName.Length-2);
						sbTO_ADDRS_NAMES.Append(!Sql.IsEmptyString(sDisplayName) ? sDisplayName : addr.Address);

						sbTO_ADDRS_EMAILS.Append((sbTO_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
						sbTO_ADDRS_EMAILS.Append(addr.Address);
					}
				}

				StringBuilder sbCC_ADDRS        = new StringBuilder();
				StringBuilder sbCC_ADDRS_NAMES  = new StringBuilder();
				StringBuilder sbCC_ADDRS_EMAILS = new StringBuilder();
				if ( email.CC != null )
				{
					foreach ( ImapAddress addr in email.CC )
					{
						// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
						sbCC_ADDRS.Append((sbCC_ADDRS.Length > 0) ? "; " : String.Empty);
						sbCC_ADDRS.Append(addr.ToString());

						sbCC_ADDRS_NAMES.Append((sbCC_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
						string sDisplayName = addr.DisplayName;
						if ( sDisplayName.StartsWith("\"") && sDisplayName.EndsWith("\"") || sDisplayName.StartsWith("\'") && sDisplayName.EndsWith("\'") )
							sDisplayName = sDisplayName.Substring(1, sDisplayName.Length-2);
						sbCC_ADDRS_NAMES.Append(!Sql.IsEmptyString(sDisplayName) ? sDisplayName : addr.Address);

						sbCC_ADDRS_EMAILS.Append((sbCC_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
						sbCC_ADDRS_EMAILS.Append(addr.Address);
					}
				}

				StringBuilder sbBCC_ADDRS        = new StringBuilder();
				StringBuilder sbBCC_ADDRS_NAMES  = new StringBuilder();
				StringBuilder sbBCC_ADDRS_EMAILS = new StringBuilder();
				if ( email.BCC != null )
				{
					foreach ( ImapAddress addr in email.BCC )
					{
						// 01/13/2008 Paul.  SugarCRM uses commas, but we prefer semicolons. 
						sbBCC_ADDRS.Append((sbBCC_ADDRS.Length > 0) ? "; " : String.Empty);
						sbBCC_ADDRS.Append(addr.ToString());

						sbBCC_ADDRS_NAMES.Append((sbBCC_ADDRS_NAMES.Length > 0) ? "; " : String.Empty);
						string sDisplayName = addr.DisplayName;
						if ( sDisplayName.StartsWith("\"") && sDisplayName.EndsWith("\"") || sDisplayName.StartsWith("\'") && sDisplayName.EndsWith("\'") )
							sDisplayName = sDisplayName.Substring(1, sDisplayName.Length-2);
						sbBCC_ADDRS_NAMES.Append(!Sql.IsEmptyString(sDisplayName) ? sDisplayName : addr.Address);

						sbBCC_ADDRS_EMAILS.Append((sbBCC_ADDRS_EMAILS.Length > 0) ? "; " : String.Empty);
						sbBCC_ADDRS_EMAILS.Append(addr.Address);
					}
				}

				// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
				string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
				string sFileURL = sSiteURL + "Images/EmailImage.aspx?ID=";
				
				using ( IDbTransaction trn = Sql.BeginTransaction(con) )
				{
					try
					{
						// 11/06/2010 Paul.  Set ID so that it can be used with inline images. 
						Guid gEMAIL_ID = Guid.NewGuid();
						for ( int i = 0; i < email.BodyParts.Count; i++ )
						{
							ImapMessageBodyPart part = email.BodyParts[i];
							// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
							if ( part.Attachment && part.Inline )
							{
								// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
								// Using part.DataBinary only works when the data is Base64. 
								byte[] byDataBinary = part.GetBytes();
								if ( byDataBinary != null )
								{
									// 01/13/2008 Paul.  We may need to convert the encoding to UTF8. 
									// att.NameEncoding;
									string sFILENAME       = String.Empty;
									string sFILE_EXT       = String.Empty;
									string sFILE_MIME_TYPE = part.ContentType.MediaType;
									// 11/04/2010 Paul.  Path.GetExtension() can throw an exception of it includes bad characters. 
									// This issue was first encountered with the invalid parsing of the Imap body parts. 
									try
									{
										sFILENAME       = Path.GetFileName (part.FileName);
										if ( Sql.IsEmptyString(sFILENAME) )
											sFILENAME = part.Disposition;
										sFILE_EXT       = Path.GetExtension(sFILENAME);
									}
									catch
									{
									}
								
									Guid gIMAGE_ID = Guid.Empty;
									SqlProcs.spEMAIL_IMAGES_Insert
										( ref gIMAGE_ID
										, gEMAIL_ID
										, sFILENAME
										, sFILE_EXT
										, sFILE_MIME_TYPE
										, trn
										);
									// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
									Crm.EmailImages.LoadFile(gIMAGE_ID, byDataBinary, trn);
									// 11/06/2010 Paul.  Now replace the ContentId with the new Image URL. 
									// 05/06/2014 Paul.  The ContentID might be NULL. 
									string sContentID = Sql.ToString(part.ContentID);
									if ( sContentID.StartsWith("<") && sContentID.EndsWith(">") )
										sContentID = sContentID.Substring(1, sContentID.Length - 2);
									sSAFE_BODY_HTML = sSAFE_BODY_HTML.Replace("cid:" + sContentID, sFileURL + gIMAGE_ID.ToString());
								}
							}
						}
						SqlProcs.spEMAILS_InsertInbound
							( ref gEMAIL_ID
							, gGROUP_ID
							, email.Subject
							, email.Received.ToLocalTime()
							, sSAFE_BODY_PLAIN
							, sSAFE_BODY_HTML 
							, ((email.From != null) ? email.From.Address    : String.Empty)
							, ((email.From != null) ? email.From.ToString() : String.Empty)
							, sbTO_ADDRS.ToString()
							, sbCC_ADDRS.ToString()
							, sbBCC_ADDRS.ToString()
							, sbTO_ADDRS_NAMES  .ToString()
							, sbTO_ADDRS_EMAILS .ToString()
							, sbCC_ADDRS_NAMES  .ToString()
							, sbCC_ADDRS_EMAILS .ToString()
							, sbBCC_ADDRS_NAMES .ToString()
							, sbBCC_ADDRS_EMAILS.ToString()
							, sEMAIL_TYPE
							, sSTATUS
							// 10/29/2010 Paul.  Since our Imap library does not provide a DeliveryTo, we are just going to fallback to the mailbox ID. 
							// This fixes a bug whereby every message retrieved was treated as new. 
							// 10/29/2010 Paul.  Instead of using gMAILBOX_ID, it would make more sense to to use the server and user. 
							, sUNIQUE_MESSAGE_ID // MESSAGE_ID
							, ((email.ReplyTo != null) ? email.ReplyTo.ToString() : String.Empty)
							, ((email.ReplyTo != null) ? email.ReplyTo.Address    : String.Empty)
							, sINTENT
							, gMAILBOX_ID
							, gTARGET_TRACKER_KEY
							, String.Join(ControlChars.CrLf, arrHeaders)
							, trn
							);
						
						if ( bHAS_ATTACHMENTS )
						{
							for ( int i = 0; i < email.BodyParts.Count; i++ )
							{
								ImapMessageBodyPart part = email.BodyParts[i];
								// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
								if ( part.Attachment && !part.Inline )
								{
									// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
									// Using part.DataBinary only works when the data is Base64. 
									byte[] byDataBinary = part.GetBytes();
									if ( byDataBinary != null )
									{
										// 01/13/2008 Paul.  We may need to convert the encoding to UTF8. 
										// att.NameEncoding;
										string sFILENAME       = String.Empty;
										string sFILE_EXT       = String.Empty;
										string sFILE_MIME_TYPE = part.ContentType.MediaType;
										// 11/04/2010 Paul.  Path.GetExtension() can throw an exception of it includes bad characters. 
										// This issue was first encountered with the invalid parsing of the Imap body parts. 
										try
										{
											sFILENAME       = Path.GetFileName (part.FileName);
											if ( Sql.IsEmptyString(sFILENAME) )
												sFILENAME = part.Disposition;
											sFILE_EXT       = Path.GetExtension(sFILENAME);
										}
										catch
										{
										}
									
										// 07/30/2008 Paul.  Use a new static version of L10N.Term() that is better about sharing the code. 
										Guid gNOTE_ID = Guid.Empty;
										// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
										SqlProcs.spNOTES_Update
											( ref gNOTE_ID
											, L10N.Term(Context.Application, sCultureName, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
											, "Emails"   // Parent Type
											, gEMAIL_ID  // Parent ID
											, Guid.Empty
											, Sql.ToString(part.ContentID)  // 05/06/2014 Paul.  The ContentID might be NULL. 
											, Guid.Empty        // TEAM_ID
											, String.Empty      // TEAM_SET_LIST
											, gGROUP_ID
											, trn
											);

										Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
										// 01/20/2006 Paul.  Must include in transaction
										SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, part.FileName, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
										// 11/22/2014 Paul.  Don't need to call spNOTES_ATTACHMENT_Update when also using LoadFile(). 
										//SqlProcs.spNOTES_ATTACHMENT_Update(gNOTE_ATTACHMENT_ID, byDataBinary, trn);
										// 11/06/2010 Paul.  Use our streamable function. 
										// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
										Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, byDataBinary, trn);
									}
								}
							}
						}
						trn.Commit();
					}
					catch
					{
						trn.Rollback();
						throw;
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


