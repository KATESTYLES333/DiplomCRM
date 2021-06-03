#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2007, Koolwired Solutions, LLC.
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
 * 06/05/07   Keith Kikta     Applied patch for attachments that do not contain charset.
 * 01/07/08   Keith Kikta     Added complex regular expressions to parse headers.
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
	/// Represents an Imap Body Part
	/// </summary>
	#endregion
	public class ImapMessageBodyPart
	{
		#region constants
		// RFC3501 - INTERNET MESSAGE ACCESS PROTOCOL - VERSION 4rev1
		// http://www.faqs.org/rfcs/rfc3501.html
		/* 11/05/2010 Paul.  Using regular expressions is just too flawed.  Replace with standard parsing. 
		// Regular Expression Tester
		// http://derekslager.com/blog/posts/2007/09/a-better-dotnet-regular-expression-tester.ashx
		// 11/04/2010 Paul.  Need the expression to be more flexible by using .+ before the end-of-line $. 
		const string non_attach = "^\\("
		                        + "(?<type>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<subtype>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<attr>(\\([^\\)]*\\)|NIL))\\s"
		                        + "(?<id>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<desc>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<encoding>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<size>(\\d+|NIL))\\s"
		                        + "(?<lines>(\\d+|NIL))\\s"
		                        + "(?<md5>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<disposition>(\\([^\\)]*\\)|NIL))\\s"
		                        + "(?<lang>(\"[^\"]*\"|NIL))\\"
		                        + ").+$";
		// const string attachment = "^\\((?<type>(\"[^\"]*\"|NIL))\\s(?<subtype>(\"[^\"]*\"|NIL))\\s(?<attr>(\\([^\\)]*\\)|NIL))\\s(?<id>(\"[^\"]*\"|NIL))\\s(?<desc>(\"[^\"]*\"|NIL))\\s(?<encoding>(\"[^\"]*\"|NIL))\\s(?<size>(\\d+|NIL))\\s((?<data>(.*))\\s|)(?<lines>(\"[^\"]*\"|NIL))\\s(?<disposition>((?>\\((?<LEVEL>)|\\)(?<-LEVEL>)|(?!\\(|\\)).)+(?(LEVEL)(?!))|NIL))\\s(?<lang>(\"[^\"]*\"|NIL))\\)$";
		// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/Thread/View.aspx?ThreadId=77775. 
		// 11/04/2010 Paul.  Need the expression to be more flexible by using .+ before the end-of-line $. 
		// ("text" "plain" ("charset" "Windows-1252") NIL NIL "quoted-printable" 2618 86 NIL NIL "en-US" NIL) UID 37770
		const string attachment = "^\\("
		                        + "(?<type>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<subtype>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<attr>(\\((\"[^\"]*\"(\\s)?)*\\)|NIL))\\s"
		                        + "(?<id>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<desc>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<encoding>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<size>(\\d+|NIL))\\s"
		                        + "((?<data>(.*))\\s|)"
		                        + "(?<lines>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<disposition>(\\([^\\)]*\\)|NIL))\\s"  // 11/04/2010 Paul.  The disposition with levels was not working.  It was taking over the language part. 
		                        + "(?<lang>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<location>(\"[^\"]*\"|NIL))\\"  // // 11/04/2010 Paul.  Now that we fixed the disposition, we need to add the location. 
		                        + ").+$";
		// 11/04/2010 Paul.  A multipart will not have the data part. 
		// ("multipart" "signed" ("protocol" "application/x-pkcs7-signature" "micalg" "sha1" "boundary" "----FA491F5971E3845EA6756AEABEAF0260") NIL NIL "7BIT" -1 NIL NIL "de-DE" NIL) UID 37927
		const string embedded   = "^\\("
		                        + "(?<type>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<subtype>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<attr>(\\((\"[^\"]*\"(\\s)?)*\\)|NIL))\\s"
		                        + "(?<id>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<desc>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<encoding>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<size>(\\d+|-1|NIL))\\s"   // 11/04/2010 Paul.  Allow the size to be -1. 
		                        + "(?<lines>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<disposition>(\\([^\\)]*\\)|NIL))\\s" // 11/04/2010 Paul.  The disposition with levels was not working.  It was taking over the language part. 
		                        + "(?<lang>(\"[^\"]*\"|NIL))\\s"
		                        + "(?<location>(\"[^\"]*\"|NIL))\\"  // // 11/04/2010 Paul.  Now that we fixed the disposition, we need to add the location. 
		                        + ").+$";
		*/
		#endregion

		#region private variables
		System.Net.Mime.ContentType _ContentType = new System.Net.Mime.ContentType();
		bool _attachment;
		string _contentid;
		string _contentdescription;
		BodyPartEncoding _encoding;
		long _size;
		long _lines;
		string _hash;
		int index = 0;
		string _data;
		string _language;
		string _bodytype;
		string _filename;
		string _bodypart;
		string _disposition;
		bool   _inline;
		string _boundary;
		string _location;
		DateTime _modifieddate;
		#endregion

		#region public properties
		/// <summary>
		/// Gets or sets a boolean value representing if the body part is an attachment.
		/// </summary>
		public bool Attachment
		{
			set { _attachment = value; }
			get { return _attachment; }
		}
		/// <summary>
		/// Gets or sets the message data (Encoded)
		/// </summary>
		public string Data
		{
			set { _data = value; }
			get { return _data; }
		}
		/// <summary>
		/// Gets the message data in binary format.
		/// </summary>
		public byte[] DataBinary
		{
			get { return Convert.FromBase64String(_data); }
		}
		/// <summary>
		/// Gets the text that identfies the body part of the message for retevial from the server.
		/// </summary>
		public string BodyPart
		{
			internal set { _bodypart = value; }
			get { return _bodypart; }
		}
		/// <summary>
		/// Gets or sets the file name of an attachment.
		/// </summary>
		public string FileName 
		{
			set { _filename = value; }
			get { return _filename; }
		}
		/// <summary>
		/// Gets or sets the content type of an body type.
		/// </summary>
		public System.Net.Mime.ContentType ContentType
		{
			set { _ContentType = value; }
			get { return _ContentType; }
		}
		/// <summary>
		/// Gets or sets the content ID.
		/// </summary>
		public string ContentID
		{
			set { _contentid = value; }
			get { return _contentid; }
		}
		/// <summary>
		/// Gets or sets the content description.
		/// </summary>
		public string ContentDescription
		{
			set { _contentdescription = value; }
			get { return _contentdescription; }
		}
		/// <summary>
		/// Gets or sets the encoding of body part.
		/// </summary>
		public BodyPartEncoding ContentEncoding
		{
			set { _encoding = value; }
			get { return _encoding; }
		}
		/// <summary>
		/// Gets the encoding type of a message.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				try
				{
					return System.Text.Encoding.GetEncoding(this.ContentType.CharSet.Split('/')[1]);
				}
				// 05/23/2014 Paul.  Catch all exceptions, not just Null Reference. 
				// A jpeg attachment was coming in as "charset" "binary" and throwing an exception. 
				// 'binary' is not a supported encoding name.
				catch// (NullReferenceException)
				{
					return Encoding.Default;
				}
			}
		}
		/// <summary>
		/// Gets or sets the size of body part.
		/// </summary>
		public long Size
		{
			set { _size = value; }
			get { return _size; }
		}
		/// <summary>
		/// Gets or sets the number of lines in a body part.
		/// </summary>
		public long Lines
		{
			set { _lines = value; }
			get { return _lines; }
		}
		/// <summary>
		/// Gets or sets the MD5 hash of a body part.
		/// </summary>
		public string ContentMD5
		{
			set { _hash = value; }
			get { return _hash; }
		}
		/// <summary>
		/// Gets or sets the content language of a body part.
		/// </summary>
		public string ContentLanguage
		{
			set { _language = value; }
			get { return _language; }
		}
		/// <summary>
		/// Gets or sets the body type of a body part.
		/// </summary>
		public string BodyType 
		{ 
			set { _bodytype = value; }
			get { return _bodytype; }
		}
		/// <summary>
		/// Gets or sets the content disposition.
		/// </summary>
		public string Disposition
		{
			set { _disposition = value; }
			get { return _disposition; }
		}
		/// <summary>
		/// Gets or sets the content inline.
		/// </summary>
		public bool Inline
		{
			set { _inline = value; }
			get { return _inline; }
		}
		// 11/04/2010 Paul.  Add support for embedded boundary. 
		/// <summary>
		/// Gets or sets the content boundary.
		/// </summary>
		public string Boundary
		{
			set { _boundary = value; }
			get { return _boundary; }
		}
		// 11/04/2010 Paul.  Add support for location. 
		/// <summary>
		/// Gets or sets the content location.
		/// </summary>
		public string Location
		{
			set { _location = value; }
			get { return _location; }
		}
		// 11/06/2010 Paul.  Add support for LastModifiedTime. 
		/// <summary>
		/// Gets or sets the content boundary.
		/// </summary>
		public DateTime LastModifiedTime
		{
			set { _modifieddate = value; }
			get { return _modifieddate; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Creates an instance of the IampMessageBodyPart class.
		/// </summary>
		/// <param name="data">A string containing the message headers for a body part.</param>
		public ImapMessageBodyPart(string data)
		{
			Match match;
			// 11/05/2010 Paul.  Experiment with just using our new code as the regular expression approach does not handle embedded rfc822 messages. 
			/*
			if ((match = Regex.Match(data, non_attach, RegexOptions.ExplicitCapture)).Success)
			{
				this.Attachment = false;
				this.ContentType.MediaType = string.Format("{0}/{1}", match.Groups["type"].Value.Replace("\"", ""), match.Groups["subtype"].Value.Replace("\"", ""));
				ParseCharacterSet(ParseNIL(match.Groups["attr"].Value));
				this.ContentID          = ParseNIL(match.Groups["id"].Value);
				this.ContentDescription = ParseNIL(match.Groups["desc"].Value);
				this.ContentEncoding    = ParseEncoding(ParseNIL(match.Groups["encoding"].Value));
				this.Size               = Convert.ToInt64(ParseNIL(match.Groups["size"].Value));
				this.Lines              = Convert.ToInt64(ParseNIL(match.Groups["lines"].Value));
				this.ContentMD5         = ParseNIL(match.Groups["md5"].Value);
				this.Disposition        = ParseNIL(match.Groups["disposition"].Value);
				this.ContentLanguage    = ParseNIL(match.Groups["lang"].Value);
			}
			else if ((match = Regex.Match(data, attachment, RegexOptions.ExplicitCapture)).Success)
			{
				this.Attachment = true;
				this.ContentType.MediaType = string.Format("{0}/{1}", match.Groups["type"].Value.Replace("\"", ""), match.Groups["subtype"].Value.Replace("\"", ""));
				ParseFileName(ParseNIL(match.Groups["attr"].Value));
				this.ContentID          = ParseNIL(match.Groups["id"].Value);
				this.ContentDescription = ParseNIL(match.Groups["desc"].Value);
				this.ContentEncoding    = ParseEncoding(ParseNIL(match.Groups["encoding"].Value));
				this.Size               = Convert.ToInt64(ParseNIL(match.Groups["size"].Value));
				this.Lines              = Convert.ToInt64(ParseNIL(match.Groups["lines"].Value));
				this.Disposition        = ParseNIL(match.Groups["disposition"].Value);
				this.ContentLanguage    = ParseNIL(match.Groups["lang"].Value);
				this.Location           = ParseNIL(match.Groups["location"].Value);
			}
			// 11/04/2010 Paul.  A embedded will not have the data part. 
			else if ((match = Regex.Match(data, embedded, RegexOptions.ExplicitCapture)).Success)
			{
				this.Attachment = true;
				this.ContentType.MediaType = string.Format("{0}/{1}", match.Groups["type"].Value.Replace("\"", ""), match.Groups["subtype"].Value.Replace("\"", ""));
				ParseBoundary(ParseNIL(match.Groups["attr"].Value));
				this.ContentID          = ParseNIL(match.Groups["id"].Value);
				this.ContentDescription = ParseNIL(match.Groups["desc"].Value);
				this.ContentEncoding    = ParseEncoding(ParseNIL(match.Groups["encoding"].Value));
				this.Size               = Convert.ToInt64(ParseNIL(match.Groups["size"].Value));
				this.Lines              = Convert.ToInt64(ParseNIL(match.Groups["lines"].Value));
				this.Disposition        = ParseNIL(match.Groups["disposition"].Value);
				this.ContentLanguage    = ParseNIL(match.Groups["lang"].Value);
				this.Location           = ParseNIL(match.Groups["location"].Value);
			}
			else
			*/
			{
				//throw new Exception("Invalid format could not parse body part headers. <br />\n" + data + ";<br />\n");
				// 11/05/2010 Paul.  Instead of throwing an exception, we need to do everything possible to read the structure. 
				if ( !data.StartsWith("(") )
					throw new Exception("Invalid format, block structure should start with open parenthesis. <br />\n" + data + ";<br />\n");
				
				// http://www.faqs.org/rfcs/rfc3501.html
				Queue<string> qParts = new Queue<string>();
				int  nStartBlock   = -1   ;
				int  nStartQuote   = -1   ;
				int  nStartText    = -1   ;
				int  nNestedBlocks =  0   ;
				bool bInsideQuotes = false;
				bool bInsideEscape = false;
				string sValue = String.Empty;
				// 11/05/2010 Paul.  When starting, skip past the first parenthesis. 
				for ( int i = 1; i < data.Length; i++ )
				{
					if ( bInsideEscape )
					{
						bInsideEscape = false;
						continue;
					}
					else if ( bInsideQuotes )
					{
						// 11/05/2010 Paul.  If we are inside quotes, then there is not much we can do except wait for the end quote. 
						if ( data[i] == '\\' )
						{
							// 11/05/2010 Paul.  We only allow escapes inside quotes. 
							bInsideEscape = true;
						}
						else if ( data[i] == '\"' )
						{
							bInsideQuotes = false;
							// 11/05/2010 Paul.  The parenthesis has a higher priority than the quote, so just ignore any quotes while inside parentheses. 
							if ( nNestedBlocks == 0 && nStartQuote >= 0 )
							{
								sValue = data.Substring(nStartQuote, i - nStartQuote);
								qParts.Enqueue(sValue);
								nStartQuote = -1;
							}
						}
					}
					else if ( data[i] == '\"' )
					{
						bInsideQuotes = true;
						// 11/05/2010 Paul.  The parenthesis has a higher priority than the quote, so just ignore any quotes while inside parentheses. 
						if ( nNestedBlocks == 0 && nStartQuote < 0 )
						{
							nStartQuote = i + 1;  // Skip past the opening quote. 
						}
					}
					else if ( data[i] == '(' )
					{
						// 11/05/2010 Paul.  Parenthesis ends any text. 
						if ( nStartText >= 0 )
						{
							sValue = data.Substring(nStartText, i - nStartText);
							qParts.Enqueue(sValue);
							nStartText = -1;
						}
						if ( nStartBlock < 0 )
							nStartBlock = i + 1;  // Skip past the opening parenthesis. 
						nNestedBlocks++;
					}
					else if ( data[i] == ')' )
					{
						// 11/05/2010 Paul.  Parenthesis ends any text. A text block should not never be withing a nested block. 
						if ( nStartText >= 0 )
						{
							sValue = data.Substring(nStartText, i - nStartText);
							qParts.Enqueue(sValue);
							nStartText = -1;
						}
						if ( nNestedBlocks == 0 )
						{
							// 11/05/2010 Paul.  If we are not inside a nested block, then this must be the end of the body structure. 
							break;
						}
						nNestedBlocks--;
						if ( nNestedBlocks == 0 )
						{
							// 11/05/2010 Paul.  If this is the end of a nested block, then grab the entire block and treat as a single part. 
							if ( nStartBlock >= 0 )
							{
								sValue = data.Substring(nStartBlock, i - nStartBlock);
								qParts.Enqueue(sValue);
								nStartBlock = -1;
							}
						}
					}
					else if ( nNestedBlocks > 0 )
					{
						continue;
					}
					else
					{
						if ( Char.IsWhiteSpace(data[i]) )
						{
							// 11/05/2010 Paul.  Whitespace ends any text. 
							if ( nStartText >= 0 )
							{
								sValue = data.Substring(nStartText, i - nStartText);
								qParts.Enqueue(sValue);
								nStartText = -1;
							}
						}
						else
						{
							if ( nStartText < 0 )
							{
								// 11/05/2010 Paul.  If this is not a whitespace, then start the beginning of a text string. 
								nStartText = i;
							}
						}
					}
				}
				if ( qParts.Count > 0 )
				{
					string sType    = String.Empty;
					string sSubtype = "*";
					// The basic fields of a non-multipart body part are in the following order:
					// body type
					//   A string giving the content media type name as defined in [MIME-IMB].
					if ( qParts.Count > 0 )
						sType = qParts.Dequeue();
					
					// body subtype
					//   A string giving the content subtype name as defined in [MIME-IMB].
					if ( qParts.Count > 0 )
						sSubtype = qParts.Dequeue();
					this.ContentType.MediaType = sType + "/" + sSubtype;
					
					// ("multipart" "signed" ("protocol" "application/x-pkcs7-signature" "micalg" "sha1" "boundary" "----A9F5C32C0E9CD6C356BAA5D68BDD8269") NIL NIL "7BIT" -1 NIL NIL "en-US" NIL) UID 37924
					if ( sType == "multipart" )
					{
						this.Attachment = true;
						// Extension data follows the multipart subtype.  Extension data
						// is never returned with the BODY fetch, but can be returned with
						// a BODYSTRUCTURE fetch.  Extension data, if present, MUST be in
						// the defined order.  The extension data of a multipart body part
						// are in the following order:
						
						// body parameter parenthesized list
						//   A parenthesized list of attribute/value pairs [e.g., ("foo"
						//   "bar" "baz" "rag") where "bar" is the value of "foo" and
						//   "rag" is the value of "baz"] as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
						{
							string sParameters = ParseNIL(qParts.Dequeue());
							if ( !String.IsNullOrEmpty(sParameters) )
							{
								match = Regex.Match(sParameters, "\"boundary\"\\s\"(?<boundary>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
								if ( match.Success )
								{
									this.FileName = match.Groups["boundary"].Value;
								}
							}
						}
						
						// body id
						//   A string giving the content id as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentID = ParseNIL(qParts.Dequeue());
						
						// body description
						//   A string giving the content description as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentDescription = ParseNIL(qParts.Dequeue());
						
						// body encoding
						//   A string giving the content transfer encoding as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentEncoding = ParseEncoding(ParseNIL(qParts.Dequeue()));
						
						// body size
						//   A number giving the size of the body in octets.  Note that
						//   this size is the size in its transfer encoding and not the
						//   resulting size after any decoding.
						if ( qParts.Count > 0 )
						{
							try
							{
								this.Size = Convert.ToInt64(ParseNIL(qParts.Dequeue()));
							}
							catch
							{
							}
						}
						
						// body MD5
						//   A string giving the body MD5 value as defined in [MD5].
						if ( qParts.Count > 0 )
							this.ContentMD5 = ParseNIL(qParts.Dequeue());
						
						// body disposition
						//   A parenthesized list with the same content and function as
						//   the body disposition for a multipart body part.
						if ( qParts.Count > 0 )
						{
							this.Disposition = ParseNIL(qParts.Dequeue());
							ParseDisposition(this.Disposition);
						}
						
						// body language
						//   A string or parenthesized list giving the body language
						//   value as defined in [LANGUAGE-TAGS].
						if ( qParts.Count > 0 )
							this.ContentLanguage = ParseNIL(qParts.Dequeue());
						
						// body location
						//   A string list giving the body content URI as defined in [LOCATION].
						if ( qParts.Count > 0 )
							this.Location = ParseNIL(qParts.Dequeue());
					}
					// A body type of type MESSAGE and subtype RFC822 contains,
					// immediately after the basic fields, the envelope structure,
					// body structure, and size in text lines of the encapsulated message.
					else if ( String.Compare(sType, "message", true) == 0 && String.Compare(sSubtype, "rfc822", true) == 0 )
					{
						this.Attachment = true;
						// body parameter parenthesized list
						//   A parenthesized list of attribute/value pairs [e.g., ("foo"
						//   "bar" "baz" "rag") where "bar" is the value of "foo" and
						//   "rag" is the value of "baz"] as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
						{
							string sParameters = ParseNIL(qParts.Dequeue());
							if ( !String.IsNullOrEmpty(sParameters) )
							{
								match = Regex.Match(sParameters, "\"charset\"\\s\"(?<set>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
								if ( match.Success )
								{
									this.ContentType.CharSet = string.Format("charset/{0}", match.Groups["set"].Value);
								}
								match = Regex.Match(sParameters, "\"name\"\\s\"(?<file>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
								if ( match.Success )
								{
									this.FileName = match.Groups["file"].Value;
									// 11/05/2010 Paul.  If a name is defined, then I think we can assume that this is an attachment. 
									this.Attachment = true;
								}
							}
						}
						
						// body id
						//   A string giving the content id as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentID = ParseNIL(qParts.Dequeue());
						
						// body description
						//   A string giving the content description as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentDescription = ParseNIL(qParts.Dequeue());
						
						// body encoding
						//   A string giving the content transfer encoding as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
						{
							this.ContentEncoding = ParseEncoding(ParseNIL(qParts.Dequeue()));
						}
						
						// body size
						//   A number giving the size of the body in octets.  Note that
						//   this size is the size in its transfer encoding and not the
						//   resulting size after any decoding.
						if ( qParts.Count > 0 )
						{
							try
							{
								this.Size = Convert.ToInt64(ParseNIL(qParts.Dequeue()));
							}
							catch
							{
							}
						}
						string sRfc822Envelope = String.Empty;
						string sRfc822Body     = String.Empty;
						long   nRfc822Lines    = 0;
						if ( qParts.Count > 0 )
							sRfc822Envelope = qParts.Dequeue();
						if ( qParts.Count > 0 )
							sRfc822Body = qParts.Dequeue();
						if ( qParts.Count > 0 )
						{
							try
							{
								nRfc822Lines = Convert.ToInt64(ParseNIL(qParts.Dequeue()));
							}
							catch
							{
							}
						}
					}
					else
					{
						this.Attachment = false;
						// body parameter parenthesized list
						//   A parenthesized list of attribute/value pairs [e.g., ("foo"
						//   "bar" "baz" "rag") where "bar" is the value of "foo" and
						//   "rag" is the value of "baz"] as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
						{
							string sParameters = ParseNIL(qParts.Dequeue());
							if ( !String.IsNullOrEmpty(sParameters) )
							{
								match = Regex.Match(sParameters, "\"charset\"\\s\"(?<set>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
								if ( match.Success )
								{
									this.ContentType.CharSet = string.Format("charset/{0}", match.Groups["set"].Value);
								}
								match = Regex.Match(sParameters, "\"name\"\\s\"(?<file>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
								if ( match.Success )
								{
									this.FileName = match.Groups["file"].Value;
									// 11/05/2010 Paul.  If a name is defined, then I think we can assume that this is an attachment. 
									this.Attachment = true;
								}
							}
						}
						
						// body id
						//   A string giving the content id as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentID = ParseNIL(qParts.Dequeue());
						
						// body description
						//   A string giving the content description as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
							this.ContentDescription = ParseNIL(qParts.Dequeue());
						
						// body encoding
						//   A string giving the content transfer encoding as defined in [MIME-IMB].
						if ( qParts.Count > 0 )
						{
							this.ContentEncoding = ParseEncoding(ParseNIL(qParts.Dequeue()));
						}
						
						// body size
						//   A number giving the size of the body in octets.  Note that
						//   this size is the size in its transfer encoding and not the
						//   resulting size after any decoding.
						if ( qParts.Count > 0 )
						{
							try
							{
								this.Size = Convert.ToInt64(ParseNIL(qParts.Dequeue()));
							}
							catch
							{
							}
						}
						
						if ( String.Compare(sType, "text", true) == 0 )
						{
							// A body type of type TEXT contains, immediately after the basic
							// fields, the size of the body in text lines.  Note that this
							// size is the size in its content transfer encoding and not the
							// resulting size after any decoding.
							if ( qParts.Count > 0 )
							{
								try
								{
									this.Lines = Convert.ToInt64(ParseNIL(qParts.Dequeue()));
								}
								catch
								{
								}
							}
						}
						else
						{
							if ( qParts.Count > 0 )
							{
							}
						}
						// body MD5
						//   A string giving the body MD5 value as defined in [MD5].
						if ( qParts.Count > 0 )
							this.ContentMD5 = ParseNIL(qParts.Dequeue());
						
						// body disposition
						//   A parenthesized list with the same content and function as
						//   the body disposition for a multipart body part.
						if ( qParts.Count > 0 )
						{
							this.Disposition = ParseNIL(qParts.Dequeue());
							ParseDisposition(this.Disposition);
						}
						
						// body language
						//   A string or parenthesized list giving the body language
						//   value as defined in [LANGUAGE-TAGS].
						if ( qParts.Count > 0 )
							this.ContentLanguage = ParseNIL(qParts.Dequeue());
						
						// body location
						//   A string list giving the body content URI as defined in [LOCATION].
						if ( qParts.Count > 0 )
							this.Location = ParseNIL(qParts.Dequeue());
					}
				}
			}
		}
		#endregion

		#region private methods
		private void ParseContentType(string data)
		{
			string[] part = new string[2];
			part[0] = data.Substring(data.IndexOf("\"") + 1, data.IndexOf("\"", data.IndexOf("\"") + 1) - (data.IndexOf("\"") + 1));
			part[1] = data.Substring(data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1, data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1) - (data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1));
			this.ContentType.MediaType = string.Format("{0}/{1}", part[0], part[1]);
			index = data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"", data.IndexOf("\"") + 1) + 1) + 1) + 1;
		}
		private void ParseCharacterSet(string data)
		{
			if (data != null)
			{
				// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/workitem/9036. 
				Match match = Regex.Match(data, "\"charset\"\\s\"(?<set>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if (match.Success)
					this.ContentType.CharSet = string.Format("charset/{0}", match.Groups["set"].Value);
			}
		}
		private void ParseFileName(string data)
		{
			if (data != null)
			{
				// 07/05/2010 Paul.  Bug fix http://imapnet.codeplex.com/workitem/9036. 
				Match match = Regex.Match(data, "\"name\"\\s\"(?<file>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if (match.Success)
					this.FileName = match.Groups["file"].Value;
			}
		}
		// 11/04/2010 Paul.  Add support for embedded boundary. 
		private void ParseBoundary(string data)
		{
			if (data != null)
			{
				Match match = Regex.Match(data, "\"boundary\"\\s\"(?<boundary>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if (match.Success)
					this.FileName = match.Groups["boundary"].Value;
			}
		}
		private void ParseDisposition(string data)
		{
			Match match = null;
			if ( !String.IsNullOrEmpty(data) )
			{
				match = Regex.Match(data, "\"inline\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if ( match.Success )
				{
					this.Inline = true;
				}
				match = Regex.Match(data, "\"filename\"\\s\"(?<file>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if ( match.Success && (String.IsNullOrEmpty(this.FileName) || !this.Attachment) )
				{
					this.FileName = match.Groups["file"].Value;
					// 11/05/2010 Paul.  If a name is defined, then I think we can assume that this is an attachment. 
					this.Attachment = true;
				}
				match = Regex.Match(data, "\"modification-date\"\\s\"(?<date>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if ( match.Success )
				{
					try
					{
						this.LastModifiedTime = DateTime.Parse(match.Groups["date"].Value);
					}
					catch
					{
					}
				}
				match = Regex.Match(data, "\"size\"\\s\"(?<size>([^\"]*))\"", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if ( match.Success )
				{
					this.Size = Convert.ToInt64(match.Groups["size"].Value);
				}
			}
		}
		private string ParseNIL(string data)
		{
			if (data.Trim() == "NIL")
				return null;
			return data;
		}
		private BodyPartEncoding ParseEncoding(string data)
		{
			if (data == null)
				return BodyPartEncoding.UNKNOWN;
			data = data.Replace("\"", "").ToUpper();
			switch (data.Substring(0,1))
			{
				case "7": return BodyPartEncoding.UTF7;
				case "8":
					if (data.Substring(1).CompareTo("BIT") == 0)
						return BodyPartEncoding.UNKNOWN;
					return BodyPartEncoding.UTF8;
				case "B":
					if (data.CompareTo("BASE64") == 0)
						return BodyPartEncoding.BASE64;
					else if (data.CompareTo("BINARY") == 0)
						return BodyPartEncoding.NONE;
					else
						return BodyPartEncoding.UNKNOWN;
				case "Q":
					if (data.CompareTo("QUOTED-PRINTABLE") == 0)
						return BodyPartEncoding.QUOTEDPRINTABLE;
					return BodyPartEncoding.UNKNOWN;
				default:
					return BodyPartEncoding.UNKNOWN;
			}
		}
		#endregion

		// 07/30/2010 Paul.  We need a function that will decode the Base64 if that is the encoding. 
		public string DataString()
		{
			string sData = _data;
			if ( _encoding == BodyPartEncoding.BASE64 )
			{
				try
				{
					byte[] by = Convert.FromBase64String(_data);
					sData = Encoding.GetString(by);
				}
				catch
				{
				}
			}
			return sData;
		}

		// 11/04/2010 Paul.  We need to make sure to access the IMAP data properly. 
		// Using part.DataBinary only works when the data is Base64. 
		public byte[] GetBytes()
		{
			byte[] byDataBinary = null;
			if ( _encoding == BodyPartEncoding.BASE64 )
			{
				try
				{
					if ( _data != null )
						byDataBinary = Convert.FromBase64String(_data);
				}
				catch
				{
				}
			}
			else if ( _encoding == BodyPartEncoding.UTF8 )
				byDataBinary = System.Text.Encoding.UTF8.GetBytes(_data);
			else if ( _encoding == BodyPartEncoding.UTF7 )
				byDataBinary = System.Text.Encoding.UTF7.GetBytes(_data);
			else if ( _encoding == BodyPartEncoding.QUOTEDPRINTABLE )
				byDataBinary = System.Text.Encoding.ASCII.GetBytes(_data);
			else
				byDataBinary = this.Encoding.GetBytes(_data);
			return byDataBinary;
		}
	}
}

