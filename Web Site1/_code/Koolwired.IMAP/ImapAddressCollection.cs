#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2006-2007, Koolwired Solutions, LLC.
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
 * Date       Programmer      Description
 * 09/16/06   Keith Kikta     Inital release.
 *--------------------------------------------------------------------------*/
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Koolwired.Imap
{
	#region Header
	/// <summary>
	/// Represents a collection of addresses.
	/// </summary>
	#endregion
	public class ImapAddressCollection
	{
		#region private variables
		private ImapAddress _from;
		private ImapAddress _sender;
		private ImapAddress _replyto;
		private ImapAddressList _to;
		private ImapAddressList _cc;
		private ImapAddressList _bcc;
		#endregion

		#region Constructor
		/// <summary>
		/// Creates an instance of the ImapAddressCollection class.
		/// </summary>
		public ImapAddressCollection()
		{
			To = new ImapAddressList();
			CC = new ImapAddressList();
			BCC = new ImapAddressList();
		}
		#endregion

		#region public properties
		/// <summary>
		/// Represents the list of addresses in the To field.
		/// </summary>
		public ImapAddressList To
		{
			get { return _to; }
			set { _to = value; }
		}
		/// <summary>
		/// Represents the list of addresses in the CC field.
		/// </summary>
		public ImapAddressList CC
		{
			get { return _cc; }
			set { _cc = value; }
		}
		/// <summary>
		/// Represents the list of addresses in the BCC field.
		/// </summary>
		public ImapAddressList BCC
		{
			get { return _bcc; }
			set { _bcc = value; }
		}
		/// <summary>
		/// Represents the address in the Sender field.
		/// </summary>
		public ImapAddress Sender
		{
			get { return _sender; }
			set { _sender = value; }
		}
		/// <summary>
		/// Represents the address in the ReplyTo field.
		/// </summary>
		public ImapAddress ReplyTo
		{
			get { return _replyto; }
			set { _replyto = value; }
		}
		/// <summary>
		/// Represents the address in the From field.
		/// </summary>
		public ImapAddress From
		{
			get { return _from; }
			set { _from = value; }
		}
		#endregion

		#region private methods
		// 07/19/2010 Paul.  Although it seems logical to assume that all strings are quoted, 
		// my lack of experience with the IMAP protocal will not allow me to make that assumption. 
		// So, assume the string can be quoted or not. 
		private string ExtractNextString(ref string sInput)
		{
			string sToken = String.Empty;
			int nStart = 0;
			int nCurrent = 0;
			// 07/19/2010 Paul.  
			while ( nCurrent < sInput.Length && Char.IsWhiteSpace(sInput[nCurrent]) )
				nCurrent++;
			if ( nCurrent < sInput.Length )
			{
				nStart = nCurrent;
				if ( sInput[nCurrent] == '\"' )
				{
					nStart++;
					bool bInsideEscape = false;
					for ( nCurrent = nStart; nCurrent < sInput.Length; nCurrent++ )
					{
						if ( bInsideEscape )
						{
							bInsideEscape = false;
							continue;
						}
						else if ( sInput[nCurrent] == '\\' )
						{
							bInsideEscape = true;
						}
						else if ( sInput[nCurrent] == '\"' )
						{
							break;
						}
						
					}
					sToken = sInput.Substring(nStart, nCurrent - nStart);
					// 07/19/2010 Paul.  Watch out for a missing ending quote. 
					if ( nCurrent < sInput.Length )
						nCurrent++;
				}
				else
				{
					// 07/19/2010 Paul.  The first character is not a quote, so the terminator will be the space character. 
					while ( nCurrent < sInput.Length && !Char.IsWhiteSpace(sInput[nCurrent]) )
						nCurrent++;
					sToken = sInput.Substring(nStart, nCurrent - nStart);
				}
			}
			sInput = sInput.Substring(nCurrent);
			return sToken;
		}

		private ImapAddressList CreateCollection(string AddressList)
		{
			// 07/19/2010 Paul.  Using a regular expression is not a reliable way to parse the address list. 
			// There have been several attempts to fix it, but the quick solution is to manually parse the list. 
			// string AddressList = "("SplendidCRM" NIL "bounce" "splendidcrm.com") ("splendidcrm@gmail.com" NIL "splendidcrm" "gmail.com")";
			// 07/19/2010 Paul.  We also need to escape a quote or any other character. 
			// ("splendid\"crm@gmail.com" NIL "splendidcrm" "gmail.com")
			// http://stackoverflow.com/questions/249791/regexp-for-quoted-string-with-escaping-quotes
			List<string> lstAddressBlocks = new List<string>();
			int nStartAddressBlock = -1;
			bool bInsideQuotes = false;
			bool bInsideEscape = false;
			string sValue = String.Empty;
			for ( int i = 0; i < AddressList.Length; i++ )
			{
				if ( bInsideEscape )
				{
					bInsideEscape = false;
					continue;
				}
				else if ( AddressList[i] == '(' && !bInsideQuotes && nStartAddressBlock < 0 )
				{
					nStartAddressBlock = i + 1;  // Skip past the opening paren. 
				}
				else if ( AddressList[i] == '\"' && !bInsideQuotes )
				{
					bInsideQuotes = true;
				}
				else if ( AddressList[i] == '\"' && bInsideQuotes )
				{
					bInsideQuotes = false;
				}
				else if ( AddressList[i] == '\\' && bInsideQuotes )
				{
					bInsideEscape = true;
				}
				else if ( AddressList[i] == ')' && !bInsideQuotes && nStartAddressBlock >= 0 )
				{
					sValue = AddressList.Substring(nStartAddressBlock, i - nStartAddressBlock);
					lstAddressBlocks.Add(sValue);
					nStartAddressBlock = -1;
				}
			}
			
			AddressList = Regex.Replace(AddressList, "\\\\\"", "'");
			AddressList = Regex.Replace(AddressList, @"\.\.", ".");
			//if (AddressList.Contains("Carleton"))
			//    throw new Exception();
			ImapAddressList Addresses = new ImapAddressList();
			// MatchCollection matches = Regex.Matches(AddressList, @"\(((?>\((?<LEVEL>)|\)(?<-LEVEL>)|(?! \( | \) ).)+(?(LEVEL)(?!)))\)$");
			// 07/05/2010 Paul.  Bug fixes http://imapnet.codeplex.com/Thread/View.aspx?ThreadId=60842. 
			// 07/17/2010 Paul.  The previous bug fix failed.  Restore previous code. 
			// The previous attempt to allow parenthesis in the display name caused lots of failures. 
			// 07/19/2010 Paul.  Instead of using Regex, use the preparsed address blocks. 
			//foreach (Match match in matches)
			for ( int i = 0; i < lstAddressBlocks.Count; i++ )
			{
				StringBuilder displayName = new StringBuilder("");
				StringBuilder email = new StringBuilder("");
				// 07/17/2010 Paul.  The previous bug fix failed.  Restore previous code. 
				//string value = match.Groups[1].ToString().Trim();
				string value = lstAddressBlocks[i];
				// Display Name
				if (!(value.StartsWith("NIL")))
				{
					Match sub;
					if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success) {
						value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
					}
					/*
					// 07/19/2010 Paul.  We need a better expression that will allow for a quote within a quoted string. 
					// http://tech.efreedom.com/Question/1-2148587/Regex-Quoted-String-with-Escaped-Quotes-in-C%23
					const string sRegexQuotedString = @"""[^""\\]*(?:\\.[^""\\]*)*""";
					//sub = Regex.Match(value, @"""([^""]+)""");
					//displayName.Append(sub.Groups[1].ToString());
					sub = Regex.Match(value, sRegexQuotedString);
					// 07/19/2010 Paul.  I'm sure there is a Regex way to exclude the quotes in the result, 
					// but since I don't know it, I'm just going to cheat and do it manually. 
					string sDisplayName = sub.Groups[0].ToString();
					displayName.Append(sDisplayName.Substring(1, sDisplayName.Length-2));
					value = value.Substring(sub.Length).Trim();
					*/
					// 07/19/2010 Paul.  Regular expressions are too error-prone, so manually parse the next token. 
					string sNext = ExtractNextString(ref value);
					displayName.Append(sNext);
					value = value.Trim();
				}
				else
					value = value.Substring(3).Trim();
				// Display Name Extended
				if (!(value.StartsWith("NIL")))
				{
					Match sub;
					if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
					{
						value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
					}
					//sub = Regex.Match(value, @"""([^""]+)""");
					//displayName.Append(" " + sub.Groups[1].ToString());
					//value = value.Substring(sub.Length).Trim();
					// 07/19/2010 Paul.  Regular expressions are too error-prone, so manually parse the next token. 
					string sNext = ExtractNextString(ref value);
					displayName.Append(" " + sNext);
					value = value.Trim();
				}
				else
					value = value.Substring(3).Trim();
				// Email Prefix
				if (!(value.StartsWith("NIL")))
				{
					Match sub;
					if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
					{
						value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
					}
					sub = Regex.Match(value, @"""([^""]+)""");
					email.Append(sub.Groups[1].ToString());
					value = value.Substring(sub.Length).Trim();
				}
				else
					value = value.Substring(3).Trim();
				// Email Suffix
				if (!(value.StartsWith("NIL")))
				{
					Match sub;
					if ((sub = Regex.Match(value, @"^{(\d+)}(.*)")).Success)
					{
						value = "\"" + sub.Groups[2].Value.Insert(Convert.ToInt32(sub.Groups[1].Value), "\"");
					}
					sub = Regex.Match(value, @"""([^""]+)""");
					email.Append("@" + sub.Groups[1].ToString());
					value = value.Substring(sub.Length).Trim();
				}
				else
					value = value.Substring(3).Trim();
				if (email.ToString().IndexOf('@') < 0)
					email = new StringBuilder("undisclosed-recipients@localhost");
				ImapAddress address = new ImapAddress(email.ToString(), ImapDecode.Decode(displayName.ToString()));
				Addresses.Add(address);
				//return Addresses; // not sure how this got here.
			}
			return Addresses;
		}
		#endregion

		#region internal methods
		internal ImapAddressCollection ParseAddresses(string addresses)
		{
			string addressList;
			addresses = addresses.Trim();
			// From address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.From = CreateCollection(addressList)[0];
			}
			// Sender address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.Sender = CreateCollection(addressList)[0];
			}
			// ReplyTo address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.ReplyTo = CreateCollection(addressList)[0];
			}
			// To address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.To = CreateCollection(addressList);
			}
			// CC address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.CC = CreateCollection(addressList);
			}
			// BCC address
			if (addresses.StartsWith("NIL"))
				addresses = addresses.Remove(0, 3).Trim();
			else
			{
				addressList = addresses.Remove(addresses.IndexOf("))") + 2);
				// Remove the current address from the string
				addresses = addresses.Substring(addressList.Length).Trim();
				// Remove leading and trailing ()
				addressList = addressList.Substring(1, addressList.Length - 2);
				this.BCC = CreateCollection(addressList);
			}
			return this;
		}
		#endregion
	}
}

