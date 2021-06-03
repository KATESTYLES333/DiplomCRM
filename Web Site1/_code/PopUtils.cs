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
using System.Net.Mail;
using System.Net.Mime;

namespace SplendidCRM
{
	public class PopUtils
	{
		public static bool Validate(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, StringBuilder sbErrors)
		{
			bool bValid = false;
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				//pop.Trace += new Pop3.TraceHandler(this.Pop3Trace);
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.Connect();
				
				int nTotalEmails = 0;
				int mailboxSize  = 0;
				pop.GetMailboxStats(out nTotalEmails, out mailboxSize);
				sbErrors.AppendLine("Connection successful. " + nTotalEmails.ToString() + " items in Inbox" + "<br />");
				bValid = true;
			}
			catch(Exception ex)
			{
				sbErrors.AppendLine(ex.Message);
			}
			finally
			{
				pop.Disconnect();
			}
			return bValid;
		}

		public static XmlDocument GetFolderTree(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
			xml.AppendChild(xml.CreateElement("Folders"));
			XmlUtil.SetSingleNodeAttribute(xml, xml.DocumentElement, "DisplayName", "Mailbox - " + sEMAIL_USER);
			
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.Connect();
				
				int nTotalEmails = 0;
				int nUnreadCount = 0;
				int mailboxSize  = 0;
				string sMAILBOX = "INBOX";
				pop.GetMailboxStats(out nTotalEmails, out mailboxSize);

				XmlElement xChild = xml.CreateElement("Folder");
				xml.DocumentElement.AppendChild(xChild);
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "Id"         , sMAILBOX);
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "TotalCount" , nTotalEmails.ToString());
				XmlUtil.SetSingleNodeAttribute(xml, xChild, "UnreadCount", nUnreadCount.ToString());
				if ( nUnreadCount > 0 )
					XmlUtil.SetSingleNodeAttribute(xml, xChild, "DisplayName", "<b>" + sMAILBOX + "</b> <font color=blue>(" + nUnreadCount.ToString() + ")</font>");
				else
					XmlUtil.SetSingleNodeAttribute(xml, xChild, "DisplayName", sMAILBOX       );
			}
			finally
			{
				pop.Disconnect();
			}
			return xml;
		}

		public static void GetFolderCount(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, ref int nTotalCount, ref int nUnreadCount)
		{
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.Connect();
				
				int nTotalEmails = 0;
				int mailboxSize  = 0;  // 07/18/2010 Paul.  The mailbox size is not useful. 
				pop.GetMailboxStats(out nTotalEmails, out mailboxSize);
				nTotalCount  = nTotalEmails;
				nUnreadCount = 0;
			}
			finally
			{
				pop.Disconnect();
			}
		}

		private static string[] GetHeadersArray(string sRawContent)
		{
			List<string> headers = new List<string>();
			if ( !Sql.IsEmptyString(sRawContent) )
			{
				using ( TextReader reader = new StringReader(sRawContent) )
				{
					string sLine = null;
					while ( (sLine = reader.ReadLine()) != null )
					{
						if ( sLine.Length == 0 )
							break;
						if ( sLine.StartsWith(" ") && headers.Count > 0 )
							headers[headers.Count - 1] += "\r\n" + sLine;
						else
							headers.Add(sLine);
					}
				}
			}
			return headers.ToArray();
		}

		// 07/18/2010 Paul.  Return the headers so that we don't have to fetch them again in the GetMessage method. 
		private static int FindMessageByMessageID(Pop3.Pop3MimeClient pop, string sMessageID, ref string sHeaders)
		{
			List<int> arrEmailIds = new List<int>();
			pop.GetEmailIdList(out arrEmailIds);
			foreach ( int i in arrEmailIds )
			{
				Pop3.RxMailMessage email = null;
				pop.GetHeaders(i, out email);
				if ( email != null )
				{
					bool bNoMessageID = true;
					string[] arrHeaders = GetHeadersArray(email.RawContent);
					for ( int j = 0; j < arrHeaders.Length; j++ )
					{
						if ( arrHeaders[j].StartsWith("Message-ID:", StringComparison.InvariantCultureIgnoreCase) )
						{
							bNoMessageID = false;
							string[] arrNameValue = arrHeaders[j].Split(':');
							if ( arrNameValue[1].Trim() == sMessageID )
							{
								sHeaders = email.RawContent;
								return i;
							}
						}
					}
					// 07/18/2010 Paul.  If there is no Message ID, then we will need to make one. 
					// 11/06/2010 Paul.  Should use ToString and not IsEmptyString. 
					if ( bNoMessageID && sMessageID == (email.DeliveryDate.ToString() + " " + Sql.ToString(email.From.Address)) )
					{
						sHeaders = email.RawContent;
						return i;
					}
				}
			}
			return -1;
		}

		public static void DeleteMessage(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sUNIQUE_ID)
		{
			// 07/18/2010 Paul.  The POP3 Message Number is meaningless as it is only valid for the connection, and we disconnect immediately. 
			// We will use the MessageID as the primary key and we will need to lookup the Message Number in order to delete it. 
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.IsCollectRawEmail = true;
				pop.Connect();
				
				string sHeaders = String.Empty;
				int nMessageNumber = FindMessageByMessageID(pop, sUNIQUE_ID, ref sHeaders);
				if ( nMessageNumber >= 0 )
				{
					pop.DeleteEmail(nMessageNumber);
				}
			}
			finally
			{
				pop.Disconnect();
			}
		}

		public static DataTable GetMessage(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sUNIQUE_ID)
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
			
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.IsCollectRawEmail = true;
				pop.Connect();
				
				string sHeaders = String.Empty;
				int nMessageNumber = FindMessageByMessageID(pop, sUNIQUE_ID, ref sHeaders);
				if ( nMessageNumber >= 0 )
				{
					string[] arrHeaders = GetHeadersArray(sHeaders);
					XmlDocument xmlInternetHeaders = new XmlDocument();
					xmlInternetHeaders.AppendChild(xmlInternetHeaders.CreateElement("Headers"));
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
					
					Pop3.RxMailMessage email = null;
					pop.GetEmail(nMessageNumber, out email);
					if ( email != null )
					{
						double dSize = 0;
						string sSize = String.Empty;
						if ( dSize < 1024 )
							sSize = dSize.ToString() + " B";
						else if ( dSize < 1024 * 1024 )
							sSize = Math.Floor(dSize / 1024).ToString() + " KB";
						else
							sSize = Math.Floor(dSize / (1024 * 1024)).ToString() + " MB";

						row["ID"                    ] = Guid.NewGuid().ToString().Replace('-', '_');
						if ( !Sql.IsEmptyString(email.MessageId) )
						{
							row["UNIQUE_ID"             ] = email.MessageId                  ;
							row["MESSAGE_ID"            ] = email.MessageId                  ;
						}
						else
						{
							// 07/18/2010 Paul.  If there is no Message ID, then we will need to make one. 
							// 11/06/2010 Paul.  Should use ToString and not IsEmptyString. 
							row["UNIQUE_ID"             ] = email.DeliveryDate.ToString() + " " + Sql.ToString(email.From.Address);
						}
						row["SIZE"                  ] = dSize                            ;
						row["SIZE_STRING"           ] = sSize                            ;
						row["IS_READ"               ] = false                            ;
						if ( email.To != null )
							row["TO_ADDRS"              ] = email.To                         ;
						if ( email.CC != null )
							row["CC_ADDRS"              ] = email.CC                         ;
						row["NAME"                  ] = email.Subject                    ;
						if ( email.DeliveryDate != null )
						{
							row["DATE_MODIFIED"         ] = email.DeliveryDate.ToLocalTime()     ;
							row["DATE_ENTERED"          ] = email.DeliveryDate.ToLocalTime()     ;
							row["DATE_START"            ] = email.DeliveryDate.ToLocalTime()     ;
						}
						else
						{
							row["DATE_MODIFIED"         ] = DateTime.Now;
							row["DATE_ENTERED"          ] = DateTime.Now;
							row["DATE_START"            ] = DateTime.Now;
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
							foreach ( MailAddress addr in email.To )
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
							foreach ( MailAddress addr in email.CC )
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
						
						StringBuilder sbBodyPlain = new StringBuilder();
						StringBuilder sbBodyHtml  = new StringBuilder();
						BuildBody(sbBodyPlain, sbBodyHtml, email);
						row["DESCRIPTION"     ] = sbBodyPlain.ToString();
						row["DESCRIPTION_HTML"] = sbBodyHtml .ToString();
						if ( email.Attachments != null && email.Attachments.Count > 0 )
						{
							row["ATTACHMENTS"] = GetAttachments(email);
						}
					}
				}
			}
			finally
			{
				pop.Disconnect();
			}
			return dt;
		}

		// 11/06/2010 Paul.  Return the Attachments so that we can show embedded images or download the attachments. 
		public static string GetAttachments(Pop3.RxMailMessage email)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			xml.AppendChild(xml.CreateElement("Attachments"));
			for ( int i = 0; i < email.Attachments.Count; i++ )
			{
				Attachment part = email.Attachments[i];
				XmlNode xAttachment = xml.CreateElement("Attachment");
				xml.DocumentElement.AppendChild(xAttachment);
				XmlUtil.SetSingleNode(xml, xAttachment, "ID"                , i.ToString()                   );
				XmlUtil.SetSingleNode(xml, xAttachment, "Name"              , part.Name                      );
				if ( part.ContentDisposition != null )
				{
					XmlUtil.SetSingleNode(xml, xAttachment, "IsInline"          , part.ContentDisposition.Inline.ToString());
					XmlUtil.SetSingleNode(xml, xAttachment, "FileName"          , part.ContentDisposition.FileName         );
				}
				XmlUtil.SetSingleNode(xml, xAttachment, "Size"              , part.ContentStream.Length.ToString()           );
				XmlUtil.SetSingleNode(xml, xAttachment, "MediaType"         , part.ContentType.MediaType     );
				XmlUtil.SetSingleNode(xml, xAttachment, "CharSet"           , part.ContentType.CharSet       );
				XmlUtil.SetSingleNode(xml, xAttachment, "ContentType"       , part.ContentType.ToString()    );
				XmlUtil.SetSingleNode(xml, xAttachment, "ContentID"         , part.ContentId                 );
				//XmlUtil.SetSingleNode(xml, xAttachment, "ContentDescription", part.ContentDescription        );
				XmlUtil.SetSingleNode(xml, xAttachment, "ContentEncoding"   , part.TransferEncoding.ToString());
				//XmlUtil.SetSingleNode(xml, xAttachment, "ContentMD5"        , part.ContentMD5                );
				//XmlUtil.SetSingleNode(xml, xAttachment, "ContentLanguage"   , part.ContentLanguage           );
				//XmlUtil.SetSingleNode(xml, xAttachment, "Disposition"       , part.Disposition               );
				//XmlUtil.SetSingleNode(xml, xAttachment, "Boundary"          , part.Boundary                  );
				//XmlUtil.SetSingleNode(xml, xAttachment, "Location"          , part.Location                  );
				//XmlUtil.SetSingleNode(xml, xAttachment, "LastModifiedTime"  , part.LastModifiedTime.ToLocalTime().ToString());
			}
			return xml.OuterXml;
		}

		public static byte[] GetAttachmentData(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, string sUNIQUE_ID, int nATTACHMENT_ID, ref string sFILENAME, ref string sCONTENT_TYPE, ref bool bINLINE)
		{
			byte[] byDataBinary = null;
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.IsCollectRawEmail = true;
				pop.Connect();
				
				string sHeaders = String.Empty;
				int nMessageNumber = FindMessageByMessageID(pop, sUNIQUE_ID, ref sHeaders);
				if ( nMessageNumber >= 0 )
				{
					Pop3.RxMailMessage email = null;
					pop.GetEmail(nMessageNumber, out email);
					if ( email != null )
					{
						if ( email.Attachments != null && email.Attachments.Count > 0 )
						{
							if ( nATTACHMENT_ID < email.Attachments.Count )
							{
								Attachment att = email.Attachments[nATTACHMENT_ID];
								bINLINE       = false;
								sCONTENT_TYPE = att.ContentType.MediaType;
								sFILENAME     = Path.GetFileName (att.Name);
								if ( att.ContentDisposition != null )
								{
									bINLINE = att.ContentDisposition.Inline;
									if ( Sql.IsEmptyString(sFILENAME) && att.ContentDisposition.FileName != null )
										sFILENAME = Path.GetFileName (att.ContentDisposition.FileName);
								}
								byDataBinary = new byte[att.ContentStream.Length];
								att.ContentStream.Read(byDataBinary, 0, (int) att.ContentStream.Length);
							}
						}
					}
				}
			}
			finally
			{
				pop.Disconnect();
			}
			return byDataBinary;
		}

		public static DataTable GetFolderMessages(HttpContext Context, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD)
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
			
			Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
			try
			{
				pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
				pop.IsCollectRawEmail = true;
				pop.Connect();
				
				List<int> arrEmailIds = new List<int>();
				pop.GetEmailIdList(out arrEmailIds);
				foreach ( int i in arrEmailIds )
				{
					Pop3.RxMailMessage email = null;
					pop.GetHeaders(i, out email);
					if ( email != null )
					{
						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						double dSize = 0;
						string sSize = String.Empty;
						if ( dSize < 1024 )
							sSize = dSize.ToString() + " B";
						else if ( dSize < 1024 * 1024 )
							sSize = Math.Floor(dSize / 1024).ToString() + " KB";
						else
							sSize = Math.Floor(dSize / (1024 * 1024)).ToString() + " MB";

						row["ID"                    ] = Guid.NewGuid().ToString().Replace('-', '_');
						// 07/18/2010 Paul.  The POP3 Message Number is meaningless as it is only valid for the connection, and we disconnect immediately. 
						// We will use the MessageID as the primary key and we will need to lookup the Message Number in order to delete it. 
						if ( !Sql.IsEmptyString(email.MessageId) )
						{
							row["UNIQUE_ID"             ] = email.MessageId                  ;
							row["MESSAGE_ID"            ] = email.MessageId                  ;
						}
						else
						{
							// 07/18/2010 Paul.  If there is no Message ID, then we will need to make one. 
							// 11/06/2010 Paul.  Should use ToString and not IsEmptyString. 
							row["UNIQUE_ID"             ] = email.DeliveryDate.ToString() + " " + Sql.ToString(email.From.Address);
						}
						row["SIZE"                  ] = dSize                            ;
						row["SIZE_STRING"           ] = sSize                            ;
						row["IS_READ"               ] = true                             ;  // We do not know the Read state, so bold nothing. 
						if ( email.To != null )
							row["TO_ADDRS"              ] = email.To.ToString()              ;
						if ( email.CC != null )
							row["CC_ADDRS"              ] = email.CC.ToString()              ;
						row["NAME"                  ] = email.Subject                    ;
						if ( email.DeliveryDate != null )
						{
							row["DATE_MODIFIED"         ] = email.DeliveryDate.ToLocalTime()     ;
							row["DATE_ENTERED"          ] = email.DeliveryDate.ToLocalTime()     ;
							row["DATE_START"            ] = email.DeliveryDate.ToLocalTime()     ;
						}
						else
						{
							row["DATE_MODIFIED"         ] = DateTime.Now;
							row["DATE_ENTERED"          ] = DateTime.Now;
							row["DATE_START"            ] = DateTime.Now;
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
					}
				}
			}
			finally
			{
				pop.Disconnect();
			}
			return dt;
		}

		public static Guid ImportMessage(HttpContext Context, HttpSessionState Session, string sPARENT_TYPE, Guid gPARENT_ID, string sSERVER_URL, int nPORT, bool bMAILBOX_SSL, string sEMAIL_USER, string sEMAIL_PASSWORD, Guid gUSER_ID, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sUNIQUE_ID)
		{
			Guid gEMAIL_ID = Guid.Empty;
			HttpApplicationState Application = Context.Application;
			long   lUploadMaxSize  = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
			string sCULTURE        = L10N.NormalizeCulture(Sql.ToString(Session["USER_SETTINGS/CULTURE"]));
			
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				
				string sSQL = String.Empty;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					Pop3.Pop3MimeClient pop = new Pop3.Pop3MimeClient(sSERVER_URL, nPORT, bMAILBOX_SSL, sEMAIL_USER, sEMAIL_PASSWORD);
					try
					{
						pop.ReadTimeout = 60 * 1000; //give pop server 60 seconds to answer
						pop.IsCollectRawEmail = true;
						pop.Connect();
						
						string sHeaders = String.Empty;
						int nMessageNumber = FindMessageByMessageID(pop, sUNIQUE_ID, ref sHeaders);
						if ( nMessageNumber >= 0 )
						{
							Pop3.RxMailMessage email = null;
							bool bLoadSuccessful = false;
							try
							{
								pop.GetEmail(nMessageNumber, out email);
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
								DateTime dtREMOTE_DATE_MODIFIED_UTC = email.DeliveryDate;
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
								// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
								/*
								if ( email.ReplyTo != null )
								{
									sREPLY_TO_NAME = email.ReplyTo.DisplayName;
									sREPLY_TO_ADDR = email.ReplyTo.Address    ;
								}
								*/
								
								StringBuilder sbTO_ADDRS_IDS    = new StringBuilder();
								StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
								StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
								if ( email.To != null && email.To.Count > 0 )
								{
									foreach ( MailAddress addr in email.To )
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
									foreach ( MailAddress addr in email.CC )
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
								if ( email.Bcc != null && email.Bcc.Count > 0 )
								{
									foreach ( MailAddress addr in email.Bcc )
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
								StringBuilder sbBodyPlain = new StringBuilder();
								StringBuilder sbBodyHtml  = new StringBuilder();
								BuildBody(sbBodyPlain, sbBodyHtml, email);
								string sDESCRIPTION      =  sbBodyPlain.ToString();
								string sDESCRIPTION_HTML =  sbBodyHtml .ToString();
								
								// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
								string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
								string sFileURL = sSiteURL + "Images/EmailImage.aspx?ID=";
								
								using ( IDbTransaction trn = Sql.BeginTransaction(con) )
								{
									try
									{
										// 11/06/2010 Paul.  Set ID so that it can be used with inline images. 
										gEMAIL_ID = Guid.NewGuid();
										foreach ( Attachment att in email.Attachments )
										{
											bool   bINLINE        = false;
											string sFILENAME       = Path.GetFileName (att.Name);
											string sFILE_EXT       = Path.GetExtension(sFILENAME);
											string sFILE_MIME_TYPE = att.ContentType.MediaType;
											if ( att.ContentDisposition != null )
											{
												bINLINE = att.ContentDisposition.Inline;
												if ( Sql.IsEmptyString(sFILENAME) && att.ContentDisposition.FileName != null )
												{
													sFILENAME       = Path.GetFileName (att.ContentDisposition.FileName);
													sFILE_EXT       = Path.GetExtension(sFILENAME);
												}
											}
											
											// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
											if ( bINLINE )
											{
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
												Crm.EmailImages.LoadFile(gIMAGE_ID, att.ContentStream, trn);
												// 11/06/2010 Paul.  Now replace the ContentId with the new Image URL. 
												// 05/06/2014 Paul.  The ContentID might be NULL. 
												string sContentID = Sql.ToString(att.ContentId);
												if ( sContentID.StartsWith("<") && sContentID.EndsWith(">") )
													sContentID = sContentID.Substring(1, sContentID.Length - 2);
												sDESCRIPTION_HTML = sDESCRIPTION_HTML.Replace("cid:" + sContentID, sFileURL + gIMAGE_ID.ToString());
											}
										}
										SqlProcs.spEMAILS_Update
											( ref gEMAIL_ID
											, gASSIGNED_USER_ID
											, email.Subject
											, email.DeliveryDate.ToLocalTime()
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
											, email.MessageId  // MESSAGE_ID
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
										foreach ( Attachment att in email.Attachments )
										{
											bool   bINLINE   = false;
											string sFILENAME = Path.GetFileName (att.Name);
											// 01/14/2010 Eric.  Hotmail does not use the Name field for the filename. 
											if ( att.ContentDisposition != null )
											{
												bINLINE = att.ContentDisposition.Inline;
												if ( Sql.IsEmptyString(sFILENAME) && att.ContentDisposition.FileName != null )
													sFILENAME = Path.GetFileName (att.ContentDisposition.FileName);
											}
											string sFILE_EXT       = Path.GetExtension(sFILENAME);
											string sFILE_MIME_TYPE = att.ContentType.MediaType;
										
											// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
											if ( !bINLINE )
											{
												long lFileSize = att.ContentStream.Length;
												if ( (lUploadMaxSize == 0) || (lFileSize <= lUploadMaxSize) )
												{
													Guid gNOTE_ID = Guid.Empty;
													// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
													SqlProcs.spNOTES_Update
														( ref gNOTE_ID
														, L10N.Term(Context.Application, sCULTURE, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
														, "Emails"   // Parent Type
														, gEMAIL_ID  // Parent ID
														, Guid.Empty
														, Sql.ToString(att.ContentId)  // 05/06/2014 Paul.  The ContentID might be NULL. 
														, gTEAM_ID       // TEAM_ID
														, sTEAM_SET_LIST // TEAM_SET_LIST
														, gASSIGNED_USER_ID
														, trn
														);

													Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
													SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, att.Name, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
													// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
													Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, att.ContentStream, trn);
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
						else
						{
							string sError = "Error loading email for " + sEMAIL_USER + ", " + sUNIQUE_ID + "." + ControlChars.CrLf;
							throw(new Exception(sError));
						}
					}
					finally
					{
						pop.Disconnect();
					}
				}
			}
			return gEMAIL_ID;
		}

		// 01/26/2013 Paul.  The SplendidCRM email header can be found in the body of a bounced message, so consider it as a tracker key. 
		private static string[] arrTrackers = new string[] { "/RemoveMe.aspx?identifier=", "/campaign_trackerv2.aspx?identifier=", "/image.aspx?identifier=", "X-SplendidCRM-ID: " };

		public static Guid FindTargetTrackerKey(Pop3.RxMailMessage mm)
		{
			Guid gTARGET_TRACKER_KEY = Sql.ToGuid(mm.Headers["x-splendidcrm-id"]);
			if ( Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
			{
				// 01/13/2008 Paul.  Now look for a RemoveMe tracker, or any of the other expected trackers. 
				if ( mm.Body != null )
				{
					foreach ( string sTracker in arrTrackers )
					{
						int nStartTracker = mm.Body.IndexOf(sTracker);
						if ( nStartTracker > 0 )
						{
							nStartTracker += sTracker.Length;
							gTARGET_TRACKER_KEY = Sql.ToGuid(mm.Body.Substring(nStartTracker, 36));
							if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
								return gTARGET_TRACKER_KEY;
						}
					}
				}
				foreach ( AlternateView vw in mm.AlternateViews )
				{
					Encoding enc = new System.Text.ASCIIEncoding();
					switch ( vw.ContentType.CharSet )
					{
						case "UTF-8" :  enc = new System.Text.UTF8Encoding   ();  break;
						case "UTF-16":  enc = new System.Text.UnicodeEncoding();  break;
						case "UTF-32":  enc = new System.Text.UTF32Encoding  ();  break;
					}
					// 02/16/2010 Paul.  Not sure why, but we noticed a problem with reading a view. 
					// We had expected all views to be able to be opened, but now we test first. 
					if ( vw.ContentStream.CanRead )
					{
						// 06/12/2008 Paul.  We need to seek to the beginning since we may have used the stream before. 
						vw.ContentStream.Seek(0, SeekOrigin.Begin);
						StreamReader rdr = new StreamReader(vw.ContentStream, enc);
						string sBody = rdr.ReadToEnd();
						if ( vw.TransferEncoding == TransferEncoding.QuotedPrintable )
							sBody = Pop3.QuotedPrintable.Decode(sBody);
						else if ( vw.TransferEncoding == TransferEncoding.Base64 )
						{
							// 05/16/2010 paul.  We have seen the encoding being marked as Base64, but the body being in text. 
							try
							{
								sBody = XmlUtil.FromBase64String(sBody);
							}
							catch
							{
							}
						}
						foreach ( string sTracker in arrTrackers )
						{
							int nStartTracker = sBody.IndexOf(sTracker);
							if ( nStartTracker > 0 )
							{
								nStartTracker += sTracker.Length;
								gTARGET_TRACKER_KEY = Sql.ToGuid(sBody.Substring(nStartTracker, 36));
								if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
									return gTARGET_TRACKER_KEY;
							}
						}
					}
				}
				// 01/20/2008 Paul.  In a bounce, the server messages will be stored in entities. 
				foreach ( Pop3.RxMailMessage ent in mm.Entities )
				{
					// text/plain
					// message/delivery-status
					// message/rfc822
					if ( ent.ContentType.MediaType == "text/plain" || ent.ContentType.MediaType == "message/rfc822" )
					{
						gTARGET_TRACKER_KEY = FindTargetTrackerKey(ent);
						if ( !Sql.IsEmptyGuid(gTARGET_TRACKER_KEY) )
							return gTARGET_TRACKER_KEY;
					}
				}
			}
			return gTARGET_TRACKER_KEY;
		}

		public static void BuildBody(StringBuilder sbBodyPlain, StringBuilder sbBodyHtml, Pop3.RxMailMessage mm)
		{
			if ( mm.Body != null )
			{
				if ( mm.ContentType.MediaType == "text/html" )
					sbBodyHtml.Append(mm.Body);
				// 06/18/2008 Paul.  If there are no alternate views, then the body should be treated as plain text. 
				else if ( mm.ContentType.MediaType == "text/plain" || mm.AlternateViews == null || mm.AlternateViews.Count == 0 )
					sbBodyPlain.Append(mm.Body);
			}
			// 06/13/2008 Paul.  Only use AlternateViews if this is a multipart message. 
			if ( mm.ContentType.MediaType.StartsWith("multipart/") )
			{
				foreach ( AlternateView vw in mm.AlternateViews )
				{
					Encoding enc = new System.Text.ASCIIEncoding();
					switch ( vw.ContentType.CharSet )
					{
						case "UTF-8" :  enc = new System.Text.UTF8Encoding   ();  break;
						case "UTF-16":  enc = new System.Text.UnicodeEncoding();  break;
						case "UTF-32":  enc = new System.Text.UTF32Encoding  ();  break;
					}
					// 02/16/2010 Paul.  Not sure why, but we noticed a problem with reading a view. 
					// We had expected all views to be able to be opened, but now we test first. 
					if ( vw.ContentStream.CanRead )
					{
						// 06/12/2008 Paul.  We need to seek to the beginning since we may have used the stream before. 
						vw.ContentStream.Seek(0, SeekOrigin.Begin);
						StreamReader rdr = new StreamReader(vw.ContentStream, enc);
						string sBody = rdr.ReadToEnd();
						if ( vw.TransferEncoding == TransferEncoding.QuotedPrintable )
							sBody = Pop3.QuotedPrintable.Decode(sBody);
						else if ( vw.TransferEncoding == TransferEncoding.Base64 )
						{
							// 05/18/2010 paul.  We have seen the encoding being marked as Base64, but the body being in text. 
							try
							{
								sBody = XmlUtil.FromBase64String(sBody);
							}
							catch
							{
							}
						}
						if ( vw.ContentType.MediaType == "text/html" )
							sbBodyHtml.Append(sBody);
						// 06/18/2008 Paul.  If the text is not HTML, then treat as plain text. 
						else // if ( vw.ContentType.MediaType == "text/plain" )
							sbBodyPlain.Append(sBody);
					}
				}
			}
			foreach ( Pop3.RxMailMessage ent in mm.Entities )
			{
				// text/plain
				// message/delivery-status
				// message/rfc822
				// multipart/alternative
				if ( ent.ContentType.MediaType.StartsWith("text/") || ent.ContentType.MediaType.StartsWith("multipart/") || ent.ContentType.MediaType.StartsWith("message/") )
				{
					BuildBody(sbBodyPlain, sbBodyHtml, ent);
				}
			}
		}

		// 07/19/2010 Paul.  Moved ImportInboundEmail to PopUtils. 
		// 09/04/2011 Paul.  In order to prevent duplicate emails, we need to use the unique message ID. 
		public static Guid ImportInboundEmail(HttpContext Context, IDbConnection con, Pop3.RxMailMessage mm, Guid gMAILBOX_ID, string sINTENT, Guid gGROUP_ID, string sUNIQUE_MESSAGE_ID)
		{
			// 09/04/2011 Paul.  Return the email ID so that we can use this method with the Chrome Extension. 
			Guid gEMAIL_ID = Guid.Empty;
			// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
			using ( IDbTransaction trn = Sql.BeginTransaction(con) )
			{
				try
				{
					string sEMAIL_TYPE = "inbound";
					string sSTATUS     = "unread";
					// 07/30/2008 Paul.  Lookup the default culture. 
					string sCultureName = SplendidDefaults.Culture(Context.Application);

					StringBuilder sbTO_ADDRS        = new StringBuilder();
					StringBuilder sbTO_ADDRS_NAMES  = new StringBuilder();
					StringBuilder sbTO_ADDRS_EMAILS = new StringBuilder();
					if ( mm.To != null )
					{
						foreach ( MailAddress addr in mm.To )
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
					if ( mm.CC != null )
					{
						foreach ( MailAddress addr in mm.CC )
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
					if ( mm.Bcc != null )
					{
						foreach ( MailAddress addr in mm.Bcc )
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

					// 01/13/2008 Paul.  First look for our special header. 
					// Our special header will only exist if the email is a bounce. 
					Guid gTARGET_TRACKER_KEY = Guid.Empty;
					// 01/13/2008 Paul.  The header will always be in lower case. 
					gTARGET_TRACKER_KEY = FindTargetTrackerKey(mm);
					// 01/20/2008 Paul.  mm.DeliveredTo can be NULL. 
					// 01/20/2008 Paul.  Filter the XSS tags before inserting the email. 
					// 01/23/2008 Paul.  DateTime in the email is in universal time. 
					
					// 06/12/2008 Paul.  The body needs to be built from all the entities using a recursive function. 
					StringBuilder sbBodyPlain = new StringBuilder();
					StringBuilder sbBodyHtml  = new StringBuilder();
					BuildBody(sbBodyPlain, sbBodyHtml, mm);

					string sSAFE_BODY_PLAIN = EmailUtils.XssFilter(sbBodyPlain.ToString(), Sql.ToString(Context.Application["CONFIG.email_xss"]));
					string sSAFE_BODY_HTML  = EmailUtils.XssFilter(sbBodyHtml .ToString(), Sql.ToString(Context.Application["CONFIG.email_xss"]));

					// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
					string sSiteURL = Utils.MassEmailerSiteURL(Context.Application);
					string sFileURL = sSiteURL + "Images/EmailImage.aspx?ID=";
					
					// 11/06/2010 Paul.  Set ID so that it can be used with inline images. 
					gEMAIL_ID = Guid.NewGuid();
					foreach ( Attachment att in mm.Attachments )
					{
						bool   bINLINE        = false;
						string sFILENAME       = Path.GetFileName (att.Name);
						string sFILE_EXT       = Path.GetExtension(sFILENAME);
						string sFILE_MIME_TYPE = att.ContentType.MediaType;
						if ( att.ContentDisposition != null )
						{
							bINLINE = att.ContentDisposition.Inline;
							if ( Sql.IsEmptyString(sFILENAME) && att.ContentDisposition.FileName != null )
							{
								sFILENAME       = Path.GetFileName (att.ContentDisposition.FileName);
								sFILE_EXT       = Path.GetExtension(sFILENAME);
							}
						}
						
						// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
						if ( bINLINE )
						{
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
							Crm.EmailImages.LoadFile(gIMAGE_ID, att.ContentStream, trn);
							// 11/06/2010 Paul.  Now replace the ContentId with the new Image URL. 
							// 05/06/2014 Paul.  The ContentID might be NULL. 
							string sContentID = Sql.ToString(att.ContentId);
							if ( sContentID.StartsWith("<") && sContentID.EndsWith(">") )
								sContentID = sContentID.Substring(1, sContentID.Length - 2);
							sSAFE_BODY_HTML = sSAFE_BODY_HTML.Replace("cid:" + sContentID, sFileURL + gIMAGE_ID.ToString());
						}
					}
					SqlProcs.spEMAILS_InsertInbound
						( ref gEMAIL_ID
						, gGROUP_ID
						, mm.Subject
						, mm.DeliveryDate.ToLocalTime()
						, sSAFE_BODY_PLAIN
						, sSAFE_BODY_HTML
						, ((mm.From != null) ? mm.From.Address    : String.Empty)
						, ((mm.From != null) ? mm.From.ToString() : String.Empty)
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
						// 09/04/2011 Paul.  In order to prevent duplicate emails, we need to use the unique message ID. 
						, sUNIQUE_MESSAGE_ID  // mm.MessageId + ((mm.DeliveredTo != null && mm.DeliveredTo.Address != null) ? mm.DeliveredTo.Address : String.Empty)
						// 07/24/2010 Paul.  ReplyTo is obsolete in .NET 4.0. 
						, String.Empty // ((mm.ReplyTo != null) ? mm.ReplyTo.ToString() : String.Empty)
						, String.Empty // ((mm.ReplyTo != null) ? mm.ReplyTo.Address    : String.Empty)
						, sINTENT
						, gMAILBOX_ID
						, gTARGET_TRACKER_KEY
						// 05/28/2008 Paul.  RawContent might be NULL. 
						, ((mm.RawContent != null) ? mm.RawContent : String.Empty)
						, trn
						);
					
					// 01/20/2008 Paul.  In a bounce, the server messages will be stored in entities. 
					// 06/12/2008 Paul.  Entities should be in the body of the message. 
					/*
					foreach ( Pop3.RxMailMessage ent in mm.Entities )
					{
						// text/plain
						// message/delivery-status
						// message/rfc822
						// 01/20/2008 Paul.  Most server status reports will not have a subject, so use the first 300 characters, but take out the CRLF. 
						// 01/21/2008 Paul.  Substring will throw an exception if request exceeds length. 
						if ( Sql.IsEmptyString(ent.Subject) && !Sql.IsEmptyString(ent.Body) )
							ent.Subject = ent.Body.Substring(0, Math.Min(ent.Body.Length, 300)).Replace("\r\n", " ");
						// 06/12/2008 Paul.  Some entities will have not subject and no body. 
						if ( !Sql.IsEmptyString(ent.Subject) )
						{
							Guid gNOTE_ID = Guid.Empty;
							// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
							SqlProcs.spNOTES_Update
								( ref gNOTE_ID
								, mm.ContentType.MediaType + ": " + ent.Subject
								, "Emails"   // Parent Type
								, gEMAIL_ID  // Parent ID
								, Guid.Empty
								, ent.Body
								, Guid.Empty // TEAM_ID
								, gGROUP_ID
								, trn
								);
						}
					}
					*/
					foreach ( Attachment att in mm.Attachments )
					{
						// 01/13/2008 Paul.  We may need to convert the encoding to UTF8. 
						// att.NameEncoding;
						bool   bINLINE        = false;
						string sFILENAME       = Path.GetFileName (att.Name);
						string sFILE_EXT       = Path.GetExtension(sFILENAME);
						string sFILE_MIME_TYPE = att.ContentType.MediaType;
						// 01/14/2010 Eric.  Hotmail does not use the Name field for the filename. 
						if ( att.ContentDisposition != null )
						{
							bINLINE = att.ContentDisposition.Inline;
							if ( Sql.IsEmptyString(sFILENAME) && att.ContentDisposition.FileName != null )
							{
								sFILENAME       = Path.GetFileName (att.ContentDisposition.FileName);
								sFILE_EXT       = Path.GetExtension(sFILENAME);
							}
						}
						
						// 11/06/2010 Paul.  Inline images are stored in the EMAIL_IMAGES table and not the NOTE_ATTACHMENTS table. 
						if ( !bINLINE )
						{
							// 07/30/2008 Paul.  Use a new static version of L10N.Term() that is better about sharing the code. 
							Guid gNOTE_ID = Guid.Empty;
							// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
							SqlProcs.spNOTES_Update
								( ref gNOTE_ID
								, L10N.Term(Context.Application, sCultureName, "Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
								, "Emails"   // Parent Type
								, gEMAIL_ID  // Parent ID
								, Guid.Empty
								, Sql.ToString(att.ContentId)  // 05/06/2014 Paul.  The ContentID might be NULL. 
								, Guid.Empty        // TEAM_ID
								, String.Empty      // TEAM_SET_LIST
								, gGROUP_ID
								, trn
								);

							Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
							// 01/20/2006 Paul.  Must include in transaction
							SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, att.Name, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
							// 11/06/2010 Paul.  Move LoadFile() to Crm.NoteAttachments. 
							Crm.NoteAttachments.LoadFile(gNOTE_ATTACHMENT_ID, att.ContentStream, trn);
						}
					}
					trn.Commit();
				}
				catch(Exception ex)
				{
					trn.Rollback();
					SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
				}
			}
			return gEMAIL_ID;
		}
	}
}


