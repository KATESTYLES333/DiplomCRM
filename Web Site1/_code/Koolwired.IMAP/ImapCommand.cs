#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2006, Koolwired Solutions, LLC.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer. 
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution. 
 * Neither the name of Koolwired Solutions, LLC. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission. 
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS
 * AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
 * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
 * OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
 * WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *--------------------------------------------------------------------------*/
#endregion

#region History
/*--------------------------------------------------------------------------
 * Modification History: 
 * Date       Programmer        Description
 * 09/16/06   Keith Kikta       Inital release.
 * 09/27/07   Keith Kikta       Modified decoding of body parts to look for '=='
 *                              instead looking for "[tag] OK"
 * 10/18/08   Frans-J King      Add missing Expunge method.
 * 10/18/08   Bradley Llewellyn Added support for Microsoft Outlooks "Welcome to Outlook" message. 
 *--------------------------------------------------------------------------*/
#endregion

#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Koolwired.Imap
{
	#region Header
	/// <summary>
	/// Represents the ImapCommand object.
	/// </summary>
	#endregion
	public class ImapCommand
	{
		#region private variables
		const string OK_COMPLETE = @"^kw\d+\WOK\W([Ff][Ee][Tt][Cc][Hh]\W|[Ll][Ii][Ss][Tt]\W|)[Cc][Oo][Mm][Pp][Ll][Ee][Tt][Ee]([Dd]|)";
		// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/workitem/8937. 
		//const string OK_SUCCESS = @"kw\d+\WOK [Ss][Uu][Cc][Cc][Ee][Ss][Ss]$";
		const string OK_SUCCESS = @"kw\d+\WOK\W+[Ss][Uu][Cc][Cc][Ee][Ss][Ss]$";
		const string OK_COPY_SUCCESS = @"kw\d+\WOK \[[Cc][Oo][Pp][Yy][Uu][Ii][Dd] \d+ \d+ \d+\] \([Ss][Uu][Cc][Cc][Ee][Ss][Ss]\)$";
		const string FETCH_NOT_OK = @"^kw\d+\W(NO|BAD)";
		const string MULTI_LINE_MESSAGE = @"^.*\{\d+\}$";
		const string EXPUNGE_COMPLETE = @"^kw\d+\WOK\W([Ee][Xx][Pp][Uu][Nn][Gg][Ee]\W|)[Cc][Oo][Mm][Pp][Ll][Ee][Tt][Ee]([Dd]|)";
		const string FETCH_COMPLETE = @"^kw\d+\WOK\W([Ff][Ee][Tt][Cc][Hh]\W|)[Cc][Oo][Mm][Pp][Ll][Ee][Tt][Ee]";
		// 07/17/2010 Paul.  We always want to return UID. 
		const string ALL_UID  = "(UID FLAGS INTERNALDATE RFC822.SIZE ENVELOPE)";
		const string FULL_UID = "(UID FLAGS INTERNALDATE RFC822.SIZE ENVELOPE BODY)";
		ImapConnect _connection;
		char _folderdelim = '.';
		#endregion

		#region protected properties
		/// <summary>
		/// Sets the ImapConnect to use in this instance.
		/// </summary>
		public ImapConnect Connection
		{
			set { _connection = value; }
			private get { return _connection; }
		}
		#endregion

		#region constructor
		/// <summary>
		/// Initalizes an instance of the ImapCommand object.
		/// </summary>
		public ImapCommand() { }
		/// <summary>
		/// Initalizes an instance of the ImapCommand object.
		/// </summary>
		/// <param name="connection">A ImapConnect object representing the connection to use in this instance.</param>
		public ImapCommand(ImapConnect connection)
		{
			this.Connection = connection;
		}
		#endregion

		#region public enumerators
		/// <summary>
		/// Properties that messages can be sorted on.
		/// </summary>
		public enum SortMethod
		{
			/// <summary>
			/// No property.
			/// </summary>
			NONE, 
			/// <summary>
			/// Sort on arrival (received).
			/// </summary>
			ARRIVAL, 
			/// <summary>
			/// Sort on CC addresses.
			/// </summary>
			CC, 
			/// <summary>
			/// Sort on sent date.
			/// </summary>
			DATE, 
			/// <summary>
			/// Sort on the From address.
			/// </summary>
			FROM, 
			/// <summary>
			/// Sort on message size.
			/// </summary>
			SIZE, 
			/// <summary>
			/// Sort on message subject.
			/// </summary>
			SUBJECT
		}
		/// <summary>
		/// Options for indicating the direction of the sort.
		/// </summary>
		public enum SortOrder {
			/// <summary>
			/// Sorts the messages ascending.
			/// </summary>
			ASC,
			/// <summary>
			/// Sorts the messages descending.
			/// </summary>
			DESC
		}
		#endregion

		#region public methods
		/// <summary>
		/// Examines a mailbox.
		/// </summary>
		/// <param name="mailbox">The name of the mailbox to examine.</param>
		/// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
		public ImapMailbox Examine(string mailbox) {
			ImapMailbox Mailbox = null;
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write("EXAMINE \"" + mailbox + "\"\r\n");
			Mailbox = ParseMailbox(mailbox);
			return Mailbox;
		}
		/// <summary>
		/// Selects a mailbox to perform commands on.
		/// </summary>
		/// <param name="mailbox">The name of the mailbox to select.</param>
		/// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
		public ImapMailbox Select(string mailbox)
		{
			ImapMailbox Mailbox = null;
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write("SELECT \"" + mailbox + "\"\r\n");
			Mailbox = ParseMailbox(mailbox);
			return Mailbox;
		}
		/// <summary>
		/// Obtains a sorted collection of messages from a mailbox.
		/// </summary>
		/// <param name="sort">A value of type SortMethod.</param>
		/// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
		/// <param name="records">An interger value containing the number of messages to return.</param>
		/// <param name="page">An integer value representing the page to display.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		public ImapMailbox Sort(SortMethod sort, SortOrder order, int records, int page)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			// 07/17/2010 Paul.  We always want to return UID. 
			Connection.Write(string.Format("SORT ({0}{1}) US-ASCII " + ALL_UID + "\r\n", OrderToString(order), SortToString(sort)));
			string response = Connection.Read();
			if (response.StartsWith("*"))
			{
				Connection.Read();
				MatchCollection matches = Regex.Matches(response, @"\d+");
				if (matches.Count > 0)
				{
					int[] ids;
					if ((page + 1) * records > matches.Count)
					{
						page = matches.Count / records;
						ids = new int[matches.Count % records];
					}
					else
						ids = new int[records];
					for (int i = page * records; i < matches.Count && i < (page + 1) * records; i++)
						ids[i - page * records] = Convert.ToInt16(matches[i].Value);
					return Fetch(ids);
				}
			}
			return new ImapMailbox();

		}
		/// <summary>
		/// Obtains a sorted collection of messages from a mailbox.
		/// </summary>
		/// <param name="sort">A value of type SortMethod.</param>
		/// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		public ImapMailbox Sort(SortMethod sort, SortOrder order)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			// 07/17/2010 Paul.  We always want to return UID. 
			Connection.Write(string.Format("SORT ({0}{1}) US-ASCII " + ALL_UID + "\r\n", OrderToString(order), SortToString(sort)));
			string response = Connection.Read();
			if (response.StartsWith("*")) {
				Connection.Read();
				MatchCollection matches = Regex.Matches(response, @"\d+");
				if (matches.Count > 0) {
					int[] ids = new int[matches.Count];
					for (int i = 0; i < matches.Count; i++)
						ids[i] = Convert.ToInt16(matches[i].Value);
					return Fetch(ids);
				}
			}
			return new ImapMailbox();
		}
		/// <summary>
		/// Obtains message from a mailbox.
		/// </summary>
		/// <param name="begin">The first message to retreive.</param>
		/// <param name="end">The last message to retreive.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		/// 07/17/2010 Paul.  With our switch to UID, this function does not make sense. 
		/*
		public ImapMailbox Fetch(int begin, int end)
		{
			ImapMailbox Mailbox = new ImapMailbox();
			return Fetch(Mailbox, begin, end);
		}
		*/
		/// <summary>
		/// Obtains messages from a mailbox.
		/// </summary>
		/// <param name="messages">A interger array of message ids.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		public ImapMailbox Fetch(int[] messages)
		{
			ImapMailbox Mailbox = new ImapMailbox();
			return Fetch(Mailbox, messages);
		}
		/// <summary>
		/// Retreives message headers for a message.
		/// </summary>
		/// <param name="message">A integer representing the message id.</param>
		/// <returns>Returns a ImapMailboxMessage object.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UIDs. 
		public string[] FetchHeaders(int uid)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write(string.Format("UID FETCH {0} BODY.PEEK[HEADER]\r\n", uid));
			return ParseHeaders();
		}
		/// <summary>
		/// Retreives message with body.
		/// </summary>
		/// <param name="message">A integer representing the message id.</param>
		/// <returns>Returns a ImapMailboxMessage object.</returns>
		/// 07/17/2010 Paul.  New function to fetch the Body. 
		public ImapMailboxMessage FetchBody(ImapMailbox Mailbox, int uid)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write(string.Format("UID FETCH {0} " + FULL_UID + "\r\n", uid));
			ParseMessages(ref Mailbox);
			if ( Mailbox.Messages != null && Mailbox.Messages.Count > 0 )
			{
				ImapMailboxMessage Message = Mailbox.Messages[0];
				FetchBodyStructure(Message);
				return Message;
			}
			return null;
		}
		/// <summary>
		/// Retreives the bodystructure of a message.
		/// </summary>
		/// <param name="message">A ImapMailboxMessage object.</param>
		/// <returns>Returns an ImapMailboxMessage object.</returns>
		// 07/30/2010 Paul.  There are fixes for Microsoft Exchange. 
		// http://imapnet.codeplex.com/Thread/View.aspx?ThreadId=46935&ANCHOR
		public ImapMailboxMessage FetchBodyStructure(ImapMailboxMessage message)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			/// 07/17/2010 Paul.  We are changing to the use of UIDs. 
			Connection.Write(string.Format("UID FETCH {0} BODYSTRUCTURE\r\n", message.UID));
			//string response = Connection.Read();
			StringBuilder sb = new StringBuilder();
			string sTemp = String.Empty;
			do
			{
				sb.Append(sTemp);
				sTemp = Connection.Read();
			}
			// 07/30/2010 Paul.  Gmail returns OK Success and MS Exchange returns Fetch Complete. 
			while ( !Regex.IsMatch(sTemp, FETCH_COMPLETE) && !Regex.IsMatch(sTemp, OK_SUCCESS) );
			
			string sReponse = sb.ToString();
			if ( sReponse.StartsWith("*") )
			{
				sReponse = sReponse.Substring(sReponse.IndexOf(" (", sReponse.IndexOf("BODYSTRUCTURE")));
				message.Errors = sReponse;
				sReponse = sReponse.Trim().Substring(0, sReponse.Trim().Length - 1);
				sReponse = AddQuotes(sReponse);
				message.BodyParts = BodyPartSplit(sReponse);
				//response = Connection.Read();
				for (int i = 0; i < message.BodyParts.Count && i < 2; i++)
				{
					if (message.BodyParts[i].ContentType.MediaType.ToLower() == "text/html")
					{
						message.HasHTML = true;
						message.HTML = i;
					}
					else if (message.BodyParts[i].ContentType.MediaType.ToLower() == "text/plain")
					{
						// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/workitem/9034?ProjectName=imapnet.
						message.HasText = true;
						message.Text = i;
					}
				}
				return message;
			}
			else
				throw new ImapCommandInvalidMessageNumber("No UID found for message number " + message.UID);
		}

		/// <summary>
		/// Replaces {5}abcde with "abcde". The number 5 denotes the length of the string to be enclosed in quotes.
		/// </summary>
		/// <param name="s">A string.</param>
		/// <returns>A string.</returns>
		private string AddQuotes(string s)
		{
			string result = String.Empty;
			for ( int i = 0; i < s.Length; i++ )
			{
				char c = s[i];
				if ( c == '{' )
				{
					int t = i + 1;
					int length = 0;
					while ( t < s.Length && s[t] >= '0' && s[t] <= '9' )
					{
						length = 10 * length + Convert.ToInt32(s[t].ToString());
						t++;
					}
					if ( t < s.Length && s[t] == '}' )
					{
						result += "\"" + s.Substring(t + 1, length) + "\"";
						i = t + length;
					}
					else
						result += c;
				}
				else
					result += c;
			}
			return result;
		}

		/// <summary>
		/// Retreives the content of a particular body part.
		/// </summary>
		/// <param name="message">A ImapMailboxMessage object.</param>
		/// <param name="part">A numeric value representing the body part in the message.</param>
		/// <returns>Returns an ImapMailboxMessage object.</returns>
		public ImapMailboxMessage FetchBodyPart(ImapMailboxMessage message, int part)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			/// 07/17/2010 Paul.  We are changing to the use of UIDs. 
			Connection.Write(string.Format("UID FETCH {0} BODY[{1}]\r\n", message.UID, message.BodyParts[part].BodyPart));
			string response = Connection.Read();
			if ( response.StartsWith("*") )
				message.BodyParts[part].Data = ParseBodyPart(message.UID, message.BodyParts[part].ContentEncoding, message.BodyParts[part].Encoding);
			else
				throw(new Exception(response));
			return message;
		}
		/// <summary>
		/// Obtains message from a mailbox.
		/// </summary>
		/// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
		/// <param name="begin">The first message to retreive.</param>
		/// <param name="end">The last message to retreive.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		/// 07/17/2010 Paul.  With our switch to UID, this function does not make sense. 
		/// 05/24/2014 Paul.  We need this function to allow searching since last search. 
		public ImapMailbox Fetch(ImapMailbox Mailbox, int begin, int end)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			// 07/17/2010 Paul.  We always want to return UID. 
			if ( end == -1 )
				Connection.Write(string.Format("FETCH {0}:* " + ALL_UID + "\r\n", begin, end));
			else
				Connection.Write(string.Format("FETCH {0}:{1} " + ALL_UID + "\r\n", begin, end));
			ParseMessages(ref Mailbox);
			return Mailbox;
		}
		/// <summary>
		/// Obtains messages from a mailbox.
		/// </summary>
		/// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
		/// <param name="messages">A interger array of message ids.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UIDs. 
		public ImapMailbox Fetch(ImapMailbox Mailbox, int[] messages)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			string messagelist = string.Empty;
			for (int i = 0; i < messages.Length; i++)
				messagelist += (i == 0) ? messages[i].ToString() : "," + messages[i];
			// 07/17/2010 Paul.  We always want to return UID. 
			Connection.Write(string.Format("UID FETCH {0} " + ALL_UID + "\r\n", messagelist));
			ParseMessages(ref Mailbox);
			return Mailbox;
		}
		/// <summary>
		/// Obtains message from a mailbox.
		/// </summary>
		/// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
		/// <returns>Returns a ImapMailbox object containing the messages.</returns>
		public ImapMailbox Fetch(ImapMailbox Mailbox)
		{
			return Fetch(Mailbox, false);
		}
		// 11/06/2010 Paul.  Provide a way to get the full headers. 
		public ImapMailbox Fetch(ImapMailbox Mailbox, bool bFULL)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			// 07/17/2010 Paul.  We always want to return UID. 
			if ( bFULL )
				Connection.Write(string.Format("FETCH 1:* " + FULL_UID + "\r\n"));
			else
				Connection.Write(string.Format("FETCH 1:* " + ALL_UID + "\r\n"));
			ParseMessages(ref Mailbox);
			return Mailbox;
		}
		/// <summary>
		/// Converts a message number to the server message UID.
		/// </summary>
		/// <param name="messageNumber">The message number to convert.</param>
		/// <returns>Returns the server UID of the message.</returns>
		// 07/17/2010 Paul.  We always return the UID, so there is no need for the FetchUID function. 
		// By removing the function, we avoid confusion. 
		/*
		public int FetchUID(int messageNumber)
		{
			Connection.Write(string.Format("FETCH {0} UID\r\n", messageNumber));
			string response = Connection.Read();
			int uid = 0;
			if (response.StartsWith("*"))
			{
				Match match = Regex.Match(response, @"\(UID (\d+)\)");
				uid = Convert.ToInt32(match.Groups[1].ToString());
				Connection.Read();
				return uid;
			}
			else
				throw new ImapCommandInvalidMessageNumber("No UID found for message number" + messageNumber);
		}
		*/
		/// <summary>
		/// Expunges a mailbox
		/// </summary>
		public void Expunge()
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write("EXPUNGE\r\n");

			string response;
			do
			{
				response = Connection.Read();
			} while (!(IsResponseEnd(response) || Regex.IsMatch(response, EXPUNGE_COMPLETE)));
		}
		/// <summary>
		/// Copies some messages from the current mailbox to another mailbox.
		/// </summary>
		/// <param name="destinationMailbox">The name of the destination mailbox.</param>
		/// <param name="begin">ID of the first message to copy.</param>
		/// <param name="end">ID of the last message to copy.</param>
		/// <returns>void</returns>
		public void Copy(string destinationMailbox, int begin, int end)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			string cmd = string.Format("COPY {0}:{1} \"{2}\"\r\n", begin, end, destinationMailbox);
			Connection.Write(cmd);
			string response;
			do
				response = Connection.Read();
			while (!(IsResponseEnd(response)));
		}
		/// <summary>
		/// Creates a folder
		/// </summary>
		/// <param name="name">A string representing the folder name.</param>
		public void Create(string name)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write(string.Format("CREATE {0}\r\n", name));
			string response;
			do
				response = Connection.Read();
			while (!(IsResponseEnd(response)));
		}
		/// <summary>
		/// Creates a folder
		/// </summary>
		/// <param name="dest">A string representing folder to create the new folder in.</param>
		/// <param name="name">A string representing the folder name.</param>
		public void Create(string dest, string name)
		{
			Create(string.Format("{0}{1}{2}", dest, _folderdelim, name));
		}
		/// <summary>
		/// Deletes a folder
		/// </summary>
		/// <param name="name">A string representing the folder name.</param>
		public void Delete(string name)
		{
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write(string.Format("DELETE {0}\r\n", name));
			string response;
			do
				response = Connection.Read();
			while (!(IsResponseEnd(response)));
		}
		/// <summary>
		/// Deletes a folder
		/// </summary>
		/// <param name="dest">A string representing folder containing the folder to delete.</param>
		/// <param name="name">A string representing the folder name.</param>
		public void Delete(string dest, string name)
		{
			Delete(string.Format("{0}{1}{2}", dest, _folderdelim, name));
		}
		/// <summary>
		/// Gets a list of folders.
		/// </summary>
		/// <returns>Returns a list of folders.</returns>
		public ImapFolder List()
		{
			List<string> folders = new List<string>();
			string response = string.Empty;
			if (!(Connection.ConnectionState == ConnectionState.Open))
				NoOpenConnection();
			Connection.Write("LIST \"\" *\r\n");
			while(!IsResponseEnd(response = Connection.Read()))
				folders.Add(response);
			return ImapFolder.ParseFolder(folders);
		}
		/// <summary>
		/// Sets the seen flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetSeen(int uid, bool value)
		{
			return SetFlag(uid, @"\Seen", value);
		}
		/// <summary>
		/// Sets the answered flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetAnswered(int uid, bool value)
		{
			return SetFlag(uid, @"\Answered", value);
		}
		/// <summary>
		/// Sets the flagged flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetFlagged(int uid, bool value)
		{
			return SetFlag(uid, @"\Flagged", value);
		}
		/// <summary>
		/// Sets the deleted flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetDeleted(int uid, bool value)
		{
			return SetFlag(uid, @"\Deleted", value);
		}
		/// <summary>
		/// Sets the draft flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetDraft(int uid, bool value)
		{
			return SetFlag(uid, @"\Draft", value);
		}
		/// <summary>
		/// Sets the recent flag of a message.
		/// </summary>
		/// <param name="messageNumber">The position of the message on the server.</param>
		/// <param name="value">A boolean value to set or unset the flag.</param>
		/// <returns>Returns true if the command succeded.</returns>
		/// 07/17/2010 Paul.  We are changing to the use of UID. 
		public bool SetRecent(int uid, bool value)
		{
			return SetFlag(uid, @"\Recent", value);
		}
		#endregion

		#region private methods

		bool SetFlag(int uid, string flag, bool append)
		{
			string method = null;
			if (append)
				method = "+flags";
			else
				method = "-flags";
			Connection.Write(string.Format("UID STORE {0} {1} ({2})\r\n", uid.ToString(), method, flag));
			string response = Connection.Read();
			if (response.StartsWith("*"))
			{
				Connection.Read();
				return true;
			}
			else
				return false;
		}
		// 07/05/2010 Paul.  Fix problem with Copy command as mentioned in http://imapnet.codeplex.com/Thread/View.aspx?ThreadId=46938. 
		bool IsResponseEnd(string response)
		{
			return (Regex.IsMatch(response, OK_COMPLETE) ||
					Regex.IsMatch(response, FETCH_NOT_OK) ||
					Regex.IsMatch(response, OK_SUCCESS) ||
					//Fixed because of Copy command response 
					Regex.IsMatch(response, OK_COPY_SUCCESS) 
				);
		}
		bool IsMultiline(string response)
		{
			return Regex.IsMatch(response, MULTI_LINE_MESSAGE);
		}
		List<string> LoadMessages()
		{
			List<string> responses = new List<string>();
			StringBuilder sb = new StringBuilder();
			do
			{
				sb = sb.Remove(0, sb.Length);
				do
				{
					if (sb.Length > 0)
						sb.Remove(sb.ToString().LastIndexOf("{"), sb.Length - sb.ToString().LastIndexOf("{"));
					sb.Append(Connection.Read());
				} while (IsMultiline(sb.ToString()));
				responses.Add(sb.ToString());
			} while (!IsResponseEnd(sb.ToString()));
			return responses;
		}
		// 07/17/2010 Paul.  Parsing the headers is very simple, just verify that the first line 
		// indicates that the results are the headers then loop until an OK is found. 
		string[] ParseHeaders()
		{
			List<string> responses = LoadMessages();
			List<string> headers = new List<string>();
			for ( int i = 0; i < responses.Count; i++ )
			{
				string response = responses[i];
				if ( i == 0 )
				{
					// 07/17/2010 Paul.  The first line should look like:
					// * 1 FETCH (BODY[HEADER] Return-Path: <splendidcrm@gmail.com>
					if ( response.Contains("BODY[HEADER]") )
						continue;
					else
						break;
				}
				// 07/17/2010 Paul.  The headers should end with a blank line. 
				if ( response == String.Empty || Regex.IsMatch(response, OK_COMPLETE) || Regex.IsMatch(response, OK_SUCCESS) )
					break;
				if ( response.StartsWith(" ") && headers.Count > 0 )
					headers[headers.Count - 1] += "\r\n" + response;
				else
					headers.Add(response);
			}
			return headers.ToArray();
		}

		void ParseMessages(ref ImapMailbox Mailbox)
		{
			List<string> responses = LoadMessages();
			if (Mailbox.Messages == null)
				Mailbox.Messages = new List<ImapMailboxMessage>();
			// 07/17/2010 Paul.  We need to make sure that errors are seen by the user, 
			// so throw the exception and expect it to be caught. 
			if ( responses.Count > 0 )
			{
				if ( !responses[0].StartsWith("*") && !Regex.IsMatch(responses[0], OK_SUCCESS) )
					throw(new Exception(responses[0]));
			}
			for (int i = 0; i < responses.Count; i++)
			{
				try
				{
					if (!responses[i].StartsWith("*"))
						continue;
					ImapMailboxMessage Message = new ImapMailboxMessage();
					Message.Flags = new ImapMessageFlags();
					Message.Addresses = new ImapAddressCollection();
					Match match;
					if ((match = Regex.Match(responses[i], @"\* (\d*)")).Success)
						Message.ID = Convert.ToInt32(match.Groups[1].ToString());
					// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/Thread/View.aspx?ThreadId=64633. 
					//if ((match = Regex.Match(responses[i], @"\(FLAGS \(([^\)]*)\)")).Success)
					if ((match = Regex.Match(responses[i], @"\sFLAGS \((.*?)\)\s")).Success)
						Message.Flags.ParseFlags(match.Groups[1].ToString());
					if ((match = Regex.Match(responses[i], @"INTERNALDATE ""([^""]+)""")).Success)
						Message.Received = DateTime.Parse(match.Groups[1].ToString());
					if ((match = Regex.Match(responses[i], @"UID (\d+)")).Success)
						Message.UID = Convert.ToInt32(match.Groups[1].ToString());
					if ((match = Regex.Match(responses[i], @"RFC822.SIZE (\d+)")).Success)
						Message.Size = Convert.ToInt32(match.Groups[1].ToString());
					if ((match = Regex.Match(responses[i], @"ENVELOPE")).Success)
						responses[i] = responses[i].Remove(0, match.Index + match.Length);
					if ((match = Regex.Match(responses[i], @"\(""(?:\w{3}\, )?([^""]+)""")).Success)
					{
						Match subMatch;
						subMatch = Regex.Match(match.Groups[1].ToString(), @"([\-\+]\d{4}.*|NIL.*)"); //(-\d{4}|-\d{4}[^""]+|NIL)
						DateTime d;
						DateTime.TryParse(match.Groups[1].ToString().Remove(subMatch.Index), out d);
						Message.Sent = d;
						Message.TimeZone = subMatch.Groups[1].ToString();
						responses[i] = responses[i].Remove(0, match.Index + match.Length);
					}
					string TOKEN = "((";
					int TOKEN_OFFSET = responses[i].Length;
					if (responses[i].Contains("(("))
						TOKEN_OFFSET = 0;
					else
					{
						TOKEN = "\" NIL";
						TOKEN_OFFSET = 2;
					}
					Message.Subject = responses[i].Substring(0, responses[i].IndexOf(TOKEN) + TOKEN_OFFSET).Trim();
					if (Message.Subject == "NIL")
						Message.Subject = null;
					else if ((match = Regex.Match(Message.Subject, "^\"(.*)\"$")).Success)
						Message.Subject = match.Groups[1].ToString();
					Message.Subject = ImapDecode.Decode(Message.Subject);
					if (responses[i].Contains("(("))
						responses[i] = responses[i].Remove(0, responses[i].Substring(0, responses[i].IndexOf("((")).Length);
					else
					{
						if (Message.Subject == string.Empty) // this is squirrely dont hate me
							Message.Subject = responses[i];
						responses[i] = string.Empty;
					}
					// 07/17/2010 Paul.  Gmail returns the message ID with a single ). 
					// It seems odd to parse the MessageID and the Reference before the addresses when they both appear after the addresses. 
					// 07/17/2010 Paul.  The code parses the MessageID and Reference before the addresses so that it can truncate all data after the two fields. 
					// The catch is that the BODY may appear after the MessageID. Lets capture the data after the MessageID and process as body if possible. 
					string sBODY = String.Empty;
					//if ((match = Regex.Match(responses[i], @"""<([^>]+)>""\)\)")).Success)
					if ((match = Regex.Match(responses[i], @"""<([^>]+)>""\)")).Success)
					{
						Message.MessageID = match.Groups[1].ToString();
						sBODY = responses[i].Substring(match.Index + match.Length);
						responses[i] = responses[i].Remove(match.Index).Trim();
					}
					if (responses[i].EndsWith("NIL"))
						responses[i] = responses[i].Remove(responses[i].Length - 3);
					else
					{
						match = Regex.Match(responses[i], @"""<([^>]+)>""");
						Message.Reference = match.Groups[1].ToString();
					}
					try
					{
						Message.Addresses = Message.Addresses.ParseAddresses(responses[i]);
					}
					catch(Exception ex)
					{
						Message.Errors = responses[i] + ex.ToString();
					}
					// 07/17/2010 Paul.  Fetching the BODY takes significantly long, and the format does not match that of an expected BodyPart. 
					// 11/06/2010 Paul.  We want to get the parse the full body so that we can show the Attachment icon. 
					// We may want to revisit this code in the future. 
					try
					{
						if ( (match = Regex.Match(sBODY, @"BODY")).Success )
						{
							sBODY = sBODY.Remove(0, match.Index + match.Length);
							Message.BodyParts = BodyPartSplit(sBODY.Trim());
						}
					}
					catch(Exception ex)
					{
						Message.Errors = sBODY + "\r\n" + ex.ToString();
					}
					Mailbox.Messages.Add(Message);
				}
				catch (Exception ex) // for debugging
				{
					throw ex;
				}
			}
		}

		string ParseBodyPart(int nUID, Imap.BodyPartEncoding encoding)
		{
			return ParseBodyPart(nUID, encoding, null);
		}

		string ParseBodyPart(int nUID, Imap.BodyPartEncoding encoding, Encoding en)
		{
			string response;
			string sUIDEnd = " UID " + nUID.ToString() + " FLAGS ";
			StringBuilder sb = new StringBuilder("");
			do
			{
				response = Connection.Read();
				if (Regex.IsMatch(response, OK_COMPLETE) || Regex.IsMatch(response, OK_SUCCESS))
					break;
				// 07/30/2010 Paul.  Gmail is returning " UID 5343 FLAGS (\\Seen))" after the Base64 string.  Don't append this. 
				if ( response.StartsWith(sUIDEnd) )
					continue;
				else if ( encoding == Imap.BodyPartEncoding.BASE64 )
					sb.Append(response);
				else if (encoding == Imap.BodyPartEncoding.QUOTEDPRINTABLE)
					if (response.EndsWith("=") || response.EndsWith(")"))
						sb.Append(response.Substring(0, response.Length - 1));
					else
						sb.AppendLine(response);
				else
					sb.AppendLine(response);
			} while (true);
			//} while (!(response.EndsWith("==") || response == ")"));
			if (sb.ToString().Trim().EndsWith(")"))
				sb = sb.Remove(sb.ToString().LastIndexOf(")"), 1);
			if (encoding != BodyPartEncoding.BASE64)
				return ImapDecode.Decode(sb.ToString(), en);
			return sb.ToString();
		}

		ImapMailbox ParseMailbox(string mailbox)
		{
			ImapMailbox Mailbox = null;
			string response = Connection.Read();
			if (response.StartsWith("*"))
			{
				Mailbox = new ImapMailbox(mailbox);
				Mailbox.Flags = new ImapMessageFlags();
				do
				{
					Match match;
					if ((match = Regex.Match(response, @"(\d+) EXISTS")).Success)
						Mailbox.Exist = Convert.ToInt32(match.Groups[1].ToString());
					else if ((match = Regex.Match(response, @"(\d+) RECENT")).Success)
						Mailbox.Recent = Convert.ToInt32(match.Groups[1].ToString());
					else if ((match = Regex.Match(response, @" FLAGS \((.*?)\)")).Success)
						Mailbox.Flags.ParseFlags(match.Groups[1].ToString());
					response = Connection.Read();
				} while (response.StartsWith("*"));
				if ((response.StartsWith("OK") || response.Substring(7, 2) == "OK") && (response.ToUpper().Contains("READ/WRITE") || response.ToUpper().Contains("READ-WRITE")))
					Mailbox.ReadWrite = true;
			}
			return Mailbox;
		}

		ImapMessageBodyPartList BodyPartSplit(string response)
		{
			ImapMessageBodyPartList Parts = new ImapMessageBodyPartList();
			int i = 0;
			int index = 1;
			int count = 0;
			// 05/07/2014 Paul.  Part values with parentheses will confuse the parsing. 
			bool bInsideQuotes = false;
			bool bInsideEscape = false;
			do
			{
				int next = index;
				do
				{
					// 05/07/2014 Paul.  Part values with parentheses will confuse the parsing. 
					// We need to include quotes and escapes in the parsing so that we can avoid counting parentheses in the content. 
					if ( bInsideEscape )
					{
						bInsideEscape = false;
						continue;
					}
					else if ( bInsideQuotes )
					{
						// 05/07/2014 Paul.  If we are inside quotes, then there is not much we can do except wait for the end quote. 
						if ( response[next] == '\\' )
						{
							// 05/07/2014 Paul.  We only allow escapes inside quotes. 
							bInsideEscape = true;
						}
						else if ( response[next] == '\"' )
						{
							bInsideQuotes = false;
						}
					}
					else if ( response[next] == '\"' )
					{
						bInsideQuotes = true;
					}
					else
					{
						if ( next >= response.Length )
						{
							break;
						}
						if (response[next] == '(')
							i++;
						else if (response[next] == ')')
							i--;
					}
					next++;
				} while (i > 0 || response[next - 1] != ')');
				if (i >= 0 && response[index] == '(')
				{
					count++;
					// Parse nested body parts
					if (response.Substring(index, next - index).StartsWith("(("))
					{
						// 10/11/2012 Clif.  Adding the statement "next ? Index" to the code allowed it to proceed. 
						// http://imapnet.codeplex.com/workitem/9064
						ImapMessageBodyPartList temp = BodyPartSplit(response.Substring(index, next - index));
						for (int j = 0; j < temp.Count; j++)
						{
							temp[j].BodyPart = count.ToString() + "." + temp[j].BodyPart;
							Parts.Add(temp[j]);
						}
					}
					else
					{
						ImapMessageBodyPart Part = new ImapMessageBodyPart(response.Substring(index, next - index));
						Part.BodyPart = count.ToString();
						Parts.Add(Part);
					}
				}
				else if(Parts.Count == 0)
				{
					ImapMessageBodyPart Part = new ImapMessageBodyPart(response);
					Part.BodyPart = "1";
					Parts.Add(Part);
				}
				index = next;
			} while (i >= 0);
			return Parts;
		}

		string SortToString(SortMethod sort)
		{
			switch (sort)
			{
				case SortMethod.ARRIVAL: return "ARRIVAL";
				case SortMethod.CC: return "CC";
				case SortMethod.DATE: return "DATE";
				case SortMethod.FROM: return "FROM";
				case SortMethod.SIZE: return "SIZE";
				case SortMethod.SUBJECT: return "SUBJECT";
				default: return string.Empty;
			}
		}

		string OrderToString(SortOrder order)
		{
			if (order == SortOrder.DESC)
				return "REVERSE ";
			return string.Empty;
		}

		void NoOpenConnection()
		{
			throw new ImapConnectionException("Connection must be open before commands can be performed.");
		}
		#endregion
	}
}
