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
using System.Reflection;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for EffiProzClientFactory.
	/// </summary>
	public class EffiProzClientFactory : DbProviderFactory
	{
		public EffiProzClientFactory(string sConnectionString)
			: base( sConnectionString
			      , "EffiProz"
			      , "System.Data.EffiProz.EfzConnection" 
			      , "System.Data.EffiProz.EfzCommand"    
			      , "System.Data.EffiProz.EfzDataAdapter"
			      , "System.Data.EffiProz.EfzParameter"  
			      , "System.Data.EffiProz.EfzCommandBuilder"
			      )
		{
		}

		// 12/12/2010 Paul.  We need an easy way to get to the ClearAllPools static method. 
		public static void ClearAllPools()
		{
			#pragma warning disable 618
			Assembly    m_asmSqlClient      = Assembly.LoadWithPartialName("EffiProz");
			System.Type m_typSqlConnection  = m_asmSqlClient.GetType("System.Data.EffiProz.EfzConnection" );
			#pragma warning restore 618

			// 12/12/2010 Paul.  This is the recommended approach to clearing the pools. 
			m_typSqlConnection.InvokeMember("ClearAllPools", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public, null, null, null);
		}
	}
}

