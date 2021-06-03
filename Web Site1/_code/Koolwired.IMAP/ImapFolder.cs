#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2006-2009, Koolwired Solutions, LLC.
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
 * 03/14/2009 Keith Kikta     Inital release. 
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
    /// Represents the ImapFolder class.
    /// </summary>
    #endregion
    public class ImapFolder : ImapFolderNode
    {
        #region Private Variables
        const string PARSE_FOLDER = @"[^\(]*\(\\(?<children>[^\)]+)\)\s\""(?<seperator>[^\""]*)\""\s\""(?<folder>[^\""]*)\""";
        // 07/05/2010 Paul.  Proposed fix for hMailServer http://imapnet.codeplex.com/workitem/7875.
        // Will not implement the fix until it can be fully regression tested. 
        // const string PARSE_FOLDER = @"[^\(]*\(\\(?<children>[^\)]+)\)\s\""(?<seperator>[^\""]*)\""\s(?<folder>[^\""]*)";

        // 07/30/2010 Paul.  Move the delimiter to the node. 
        //char _delimiter;
        #endregion

        /*
        #region Public Properties
        /// <summary>
        /// Gets or sets a string that is used as the folder delimiter.
        /// </summary>
        public char Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }
        #endregion
        */

        #region Constructor
        /// <summary>
        /// Creates an instance of the imap folder.
        /// </summary>
        //public ImapFolder() { }
        #endregion

        #region Internal Methods
        internal static void SkipWhitespace(ref int nStart, ref string sLine)
        {
            while ( nStart < sLine.Length && Char.IsWhiteSpace(sLine[nStart]) )
                nStart++;
        }

        internal static ImapFolder ParseFolder(List<string> folders)
        {
            ImapFolder folder = new ImapFolder();
            for (int i = 0; i < folders.Count; i++)
            {
                /*
                Match match = Regex.Match(folders[i], PARSE_FOLDER);
                folder.Delimiter = match.Groups["seperator"].Value[0];
                string[] parts = match.Groups["folder"].Value.Split(folder.Delimiter);
                ImapFolderNode node = folder;
                for (int j = 0; j < parts.Length; j++)
                    if (node.Children.HasNode(parts[j]))
                        node = node.Children[parts[j]];
                    else
                        node = node.Children.Add(new ImapFolderNode(parts[j]));
                */
                // 07/30/2010 Paul.  Regular expressions do not work well with the escaping possible in a name. 
                if ( folders[i].StartsWith("*") )
                {
                    string sLine = folders[i];
                    int nStart = 1;
                    SkipWhitespace(ref nStart, ref sLine);
                    int nEnd = nStart;
                    while ( nEnd < sLine.Length && !Char.IsWhiteSpace(sLine[nEnd]) )
                        nEnd++;
                    if ( String.Compare(sLine.Substring(nStart, nEnd - nStart), "LIST", true) == 0 )
                    {
                        nStart = nEnd;
                        SkipWhitespace(ref nStart, ref sLine);
                        string sFolderName = String.Empty;
                        char   chDelimiter  = '/';
                        if ( nStart < sLine.Length && sLine[nStart] == '(' )
                        {
                            nStart++;
                            nEnd = nStart;
                            while ( nEnd < sLine.Length && sLine[nEnd] != ')' )
                                nEnd++;
                            string sFolderFlags = sLine.Substring(nStart, nEnd - nStart);
                            nStart = nEnd + 1;
                            SkipWhitespace(ref nStart, ref sLine);
                            nEnd = nStart;
                            while ( nEnd < sLine.Length && !Char.IsWhiteSpace(sLine[nEnd]) )
                                nEnd++;
                            string sDelimiter = sLine.Substring(nStart, nEnd - nStart);
                            if ( sDelimiter.Length >= 3 && sDelimiter.StartsWith("\"") && sDelimiter.EndsWith("\"") )
                                chDelimiter = sDelimiter[1];
                            else if ( sDelimiter.Length > 0 )
                                chDelimiter = sDelimiter[0];
                            nStart = nEnd;
                            SkipWhitespace(ref nStart, ref sLine);
                            // 07/30/2010 Paul.  A folder can contain an embedded quote or double quote, so treat the rest of the line as the folder name. 
                            sFolderName = sLine.Substring(nStart);
                            // 07/30/2010 Paul.  If a folder name contains a backslash, then there will be a number in curly brackets
                            // and the name will be on the following line.  The number is the length of the folder name. 
                            // * LIST (\HasNoChildren) "/" {31}
                            // "Contacts (Old's \ "Personal")"
                            if ( sFolderName.StartsWith("{") && sFolderName.EndsWith("}") )
                            {
                                // 07/30/2010 Paul.  Before using the next line as a string, first make sure that it is not a new LIST response. 
                                if ( (i + 1) < folders.Count && folders[i+1][0] != '*'  )
                                {
                                    /*
                                    string sFolderNameLength = sFolderName.Substring(1, sFolderName.Length - 2);
                                    int nFolderNameLength = 0;
                                    if ( Int32.TryParse(sFolderNameLength, out nFolderNameLength) && nFolderNameLength > 0 )
                                    {
                                    }
                                    */
                                    // 07/30/2010 Paul.  Instead of computing the length of the next string, just use the entire next line. 
                                    i++;
                                    sFolderName = folders[i];
                                    nStart = 0;
                                }
                            }
                            sFolderName = sFolderName.TrimEnd();
                            if ( sFolderName.Length > 3 && sFolderName.StartsWith("\"") && sFolderName.EndsWith("\"") )
                                sFolderName = sFolderName.Substring(1, sFolderName.Length - 2);
                        }
                        if ( !String.IsNullOrEmpty(sFolderName) )
                        {
                            string[] arrParts = sFolderName.Split(chDelimiter);
                            ImapFolderNode node = folder;
                            for ( int j = 0; j < arrParts.Length; j++ )
                            {
                                if ( node.Children.HasNode(arrParts[j]) )
                                    node = node.Children[arrParts[j]];
                                else
                                    node = node.Children.Add(new ImapFolderNode(arrParts[j], chDelimiter));
                            }
                        }
                    }
                }
            }
            return folder;
        }
        #endregion
    }
}

