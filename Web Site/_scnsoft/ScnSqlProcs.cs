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
using System.Data;
using System.Data.Common;
//using Microsoft.VisualBasic;
using System.Xml;

namespace SplendidCRM
{
	/// <summary>
	/// SqlProcs generated from database [SplendidCRM7] on 1/16/2018 12:48:39 AM.
	/// </summary>
	public partial class SqlProcs
	{



        #region spPARTNERS_RESOURCES_Delete
        /// <summary>
        /// spPARTNERS_RESOURCES_Delete
        /// </summary>
        public static void spPARTNERS_RESOURCES_Delete(Guid gPARTNER_ID, Guid gRESOURCE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_RESOURCES_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_RESOURCES_Delete
        /// <summary>
        /// spPARTNERS_RESOURCES_Delete
        /// </summary>
        public static void spPARTNERS_RESOURCES_Delete(Guid gPARTNER_ID, Guid gRESOURCE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_RESOURCES_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_RESOURCES_Delete
        /// <summary>
        /// spPARTNERS_RESOURCES_Delete
        /// </summary>
        public static IDbCommand cmdPARTNERS_RESOURCES_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_RESOURCES_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_RESOURCES_Update
        /// <summary>
        /// spPARTNERS_RESOURCES_Update
        /// </summary>
        public static void spPARTNERS_RESOURCES_Update(Guid gPARTNER_ID, Guid gRESOURCE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_RESOURCES_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_RESOURCES_Update
        /// <summary>
        /// spPARTNERS_RESOURCES_Update
        /// </summary>
        public static void spPARTNERS_RESOURCES_Update(Guid gPARTNER_ID, Guid gRESOURCE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_RESOURCES_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_RESOURCES_Update
        /// <summary>
        /// spPARTNERS_RESOURCES_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_RESOURCES_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_RESOURCES_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_Delete
        /// <summary>
        /// spPARTNERS_Delete
        /// </summary>
        public static void spPARTNERS_Delete(Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_Delete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_Delete
        /// <summary>
        /// spPARTNERS_Delete
        /// </summary>
        public static void spPARTNERS_Delete(Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_Delete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_Delete
        /// <summary>
        /// spPARTNERS_Delete
        /// </summary>
        public static IDbCommand cmdPARTNERS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_Delete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_DOCUMENTS_Delete
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Delete
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_Delete(Guid gPARTNER_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_DOCUMENTS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_DOCUMENTS_Delete
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Delete
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_Delete(Guid gPARTNER_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_DOCUMENTS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_DOCUMENTS_Delete
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Delete
        /// </summary>
        public static IDbCommand cmdPARTNERS_DOCUMENTS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_DOCUMENTS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_DOCUMENTS_GetLatest
        /// <summary>
        /// spPARTNERS_DOCUMENTS_GetLatest
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_GetLatest(Guid gPARTNER_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_DOCUMENTS_GetLatest";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_DOCUMENTS_GetLatest
        /// <summary>
        /// spPARTNERS_DOCUMENTS_GetLatest
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_GetLatest(Guid gPARTNER_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_DOCUMENTS_GetLatest";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_DOCUMENTS_GetLatest
        /// <summary>
        /// spPARTNERS_DOCUMENTS_GetLatest
        /// </summary>
        public static IDbCommand cmdPARTNERS_DOCUMENTS_GetLatest(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_DOCUMENTS_GetLatest";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_DOCUMENTS_Update
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Update
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_Update(Guid gPARTNER_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_DOCUMENTS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_DOCUMENTS_Update
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Update
        /// </summary>
        public static void spPARTNERS_DOCUMENTS_Update(Guid gPARTNER_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_DOCUMENTS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_DOCUMENTS_Update
        /// <summary>
        /// spPARTNERS_DOCUMENTS_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_DOCUMENTS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_DOCUMENTS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        //NOT UPTODATE
        #region spPARTNERS_Import
        /// <summary>
        /// spPARTNERS_Import - 
        /// </summary>
        public static void spPARTNERS_Import(ref Guid gID, Guid gASSIGNED_USER_ID, string sNAME, string sPARTNER_TYPE, Guid gPARENT_ID, string sINDUSTRY, string sANNUAL_REVENUE, string sPHONE_FAX, string sBILLING_ADDRESS_STREET, string sBILLING_ADDRESS_CITY, string sBILLING_ADDRESS_STATE, string sBILLING_ADDRESS_POSTALCODE, string sBILLING_ADDRESS_COUNTRY, string sDESCRIPTION, string sRATING, string sPHONE_OFFICE, string sPHONE_ALTERNATE, string sEMAIL1, string sEMAIL2, string sWEBSITE, string sOWNERSHIP, string sEMPLOYEES, string sSIC_CODE, string sTICKER_SYMBOL, string sSHIPPING_ADDRESS_STREET, string sSHIPPING_ADDRESS_CITY, string sSHIPPING_ADDRESS_STATE, string sSHIPPING_ADDRESS_POSTALCODE, string sSHIPPING_ADDRESS_COUNTRY, string sPARTNER_NUMBER, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bEXCHANGE_FOLDER, DateTime dtDATE_ENTERED, DateTime dtDATE_MODIFIED, string sBILLING_ADDRESS_STREET1, string sBILLING_ADDRESS_STREET2, string sBILLING_ADDRESS_STREET3, string sSHIPPING_ADDRESS_STREET1, string sSHIPPING_ADDRESS_STREET2, string sSHIPPING_ADDRESS_STREET3, string sTAG_SET_NAME, string sNAICS_SET_NAME, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_Import";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                            IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                            IDbDataParameter parANNUAL_REVENUE = Sql.AddParameter(cmd, "@ANNUAL_REVENUE", sANNUAL_REVENUE, 25);
                            IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                            IDbDataParameter parBILLING_ADDRESS_STREET = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET", sBILLING_ADDRESS_STREET, 150);
                            IDbDataParameter parBILLING_ADDRESS_CITY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_CITY", sBILLING_ADDRESS_CITY, 100);
                            IDbDataParameter parBILLING_ADDRESS_STATE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STATE", sBILLING_ADDRESS_STATE, 100);
                            IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", sBILLING_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_COUNTRY", sBILLING_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                            IDbDataParameter parRATING = Sql.AddParameter(cmd, "@RATING", sRATING, 25);
                            IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                            IDbDataParameter parPHONE_ALTERNATE = Sql.AddParameter(cmd, "@PHONE_ALTERNATE", sPHONE_ALTERNATE, 25);
                            IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                            IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                            IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                            IDbDataParameter parOWNERSHIP = Sql.AddParameter(cmd, "@OWNERSHIP", sOWNERSHIP, 100);
                            IDbDataParameter parEMPLOYEES = Sql.AddParameter(cmd, "@EMPLOYEES", sEMPLOYEES, 10);
                            IDbDataParameter parSIC_CODE = Sql.AddParameter(cmd, "@SIC_CODE", sSIC_CODE, 10);
                            IDbDataParameter parTICKER_SYMBOL = Sql.AddParameter(cmd, "@TICKER_SYMBOL", sTICKER_SYMBOL, 10);
                            IDbDataParameter parSHIPPING_ADDRESS_STREET = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET", sSHIPPING_ADDRESS_STREET, 150);
                            IDbDataParameter parSHIPPING_ADDRESS_CITY = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_CITY", sSHIPPING_ADDRESS_CITY, 100);
                            IDbDataParameter parSHIPPING_ADDRESS_STATE = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STATE", sSHIPPING_ADDRESS_STATE, 100);
                            IDbDataParameter parSHIPPING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_POSTALCODE", sSHIPPING_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parSHIPPING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_COUNTRY", sSHIPPING_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parPARTNER_NUMBER = Sql.AddParameter(cmd, "@PARTNER_NUMBER", sPARTNER_NUMBER, 30);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parEXCHANGE_FOLDER = Sql.AddParameter(cmd, "@EXCHANGE_FOLDER", bEXCHANGE_FOLDER);
                            IDbDataParameter parDATE_ENTERED = Sql.AddParameter(cmd, "@DATE_ENTERED", dtDATE_ENTERED);
                            IDbDataParameter parDATE_MODIFIED = Sql.AddParameter(cmd, "@DATE_MODIFIED", dtDATE_MODIFIED);
                            IDbDataParameter parBILLING_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET1", sBILLING_ADDRESS_STREET1, 150);
                            IDbDataParameter parBILLING_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET2", sBILLING_ADDRESS_STREET2, 150);
                            IDbDataParameter parBILLING_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET3", sBILLING_ADDRESS_STREET3, 150);
                            IDbDataParameter parSHIPPING_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET1", sSHIPPING_ADDRESS_STREET1, 150);
                            IDbDataParameter parSHIPPING_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET2", sSHIPPING_ADDRESS_STREET2, 150);
                            IDbDataParameter parSHIPPING_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET3", sSHIPPING_ADDRESS_STREET3, 150);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parNAICS_SET_NAME = Sql.AddParameter(cmd, "@NAICS_SET_NAME", sNAICS_SET_NAME, 4000);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        //NOT UPTODATE
        #region spPARTNERS_Import
        /// <summary>
        /// spPARTNERS_Import
        /// </summary>
        public static void spPARTNERS_Import(ref Guid gID, Guid gASSIGNED_USER_ID, string sNAME, string sPARTNER_TYPE, Guid gPARENT_ID, string sINDUSTRY, string sANNUAL_REVENUE, string sPHONE_FAX, string sBILLING_ADDRESS_STREET, string sBILLING_ADDRESS_CITY, string sBILLING_ADDRESS_STATE, string sBILLING_ADDRESS_POSTALCODE, string sBILLING_ADDRESS_COUNTRY, string sDESCRIPTION, string sRATING, string sPHONE_OFFICE, string sPHONE_ALTERNATE, string sEMAIL1, string sEMAIL2, string sWEBSITE, string sOWNERSHIP, string sEMPLOYEES, string sSIC_CODE, string sTICKER_SYMBOL, string sSHIPPING_ADDRESS_STREET, string sSHIPPING_ADDRESS_CITY, string sSHIPPING_ADDRESS_STATE, string sSHIPPING_ADDRESS_POSTALCODE, string sSHIPPING_ADDRESS_COUNTRY, string sPARTNER_NUMBER, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bEXCHANGE_FOLDER, DateTime dtDATE_ENTERED, DateTime dtDATE_MODIFIED, string sBILLING_ADDRESS_STREET1, string sBILLING_ADDRESS_STREET2, string sBILLING_ADDRESS_STREET3, string sSHIPPING_ADDRESS_STREET1, string sSHIPPING_ADDRESS_STREET2, string sSHIPPING_ADDRESS_STREET3, string sTAG_SET_NAME, string sNAICS_SET_NAME, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_Import";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                IDbDataParameter parANNUAL_REVENUE = Sql.AddParameter(cmd, "@ANNUAL_REVENUE", sANNUAL_REVENUE, 25);
                IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                IDbDataParameter parBILLING_ADDRESS_STREET = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET", sBILLING_ADDRESS_STREET, 150);
                IDbDataParameter parBILLING_ADDRESS_CITY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_CITY", sBILLING_ADDRESS_CITY, 100);
                IDbDataParameter parBILLING_ADDRESS_STATE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STATE", sBILLING_ADDRESS_STATE, 100);
                IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", sBILLING_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_COUNTRY", sBILLING_ADDRESS_COUNTRY, 100);
                IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                IDbDataParameter parRATING = Sql.AddParameter(cmd, "@RATING", sRATING, 25);
                IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                IDbDataParameter parPHONE_ALTERNATE = Sql.AddParameter(cmd, "@PHONE_ALTERNATE", sPHONE_ALTERNATE, 25);
                IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                IDbDataParameter parOWNERSHIP = Sql.AddParameter(cmd, "@OWNERSHIP", sOWNERSHIP, 100);
                IDbDataParameter parEMPLOYEES = Sql.AddParameter(cmd, "@EMPLOYEES", sEMPLOYEES, 10);
                IDbDataParameter parSIC_CODE = Sql.AddParameter(cmd, "@SIC_CODE", sSIC_CODE, 10);
                IDbDataParameter parTICKER_SYMBOL = Sql.AddParameter(cmd, "@TICKER_SYMBOL", sTICKER_SYMBOL, 10);
                IDbDataParameter parSHIPPING_ADDRESS_STREET = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET", sSHIPPING_ADDRESS_STREET, 150);
                IDbDataParameter parSHIPPING_ADDRESS_CITY = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_CITY", sSHIPPING_ADDRESS_CITY, 100);
                IDbDataParameter parSHIPPING_ADDRESS_STATE = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STATE", sSHIPPING_ADDRESS_STATE, 100);
                IDbDataParameter parSHIPPING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_POSTALCODE", sSHIPPING_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parSHIPPING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_COUNTRY", sSHIPPING_ADDRESS_COUNTRY, 100);
                IDbDataParameter parPARTNER_NUMBER = Sql.AddParameter(cmd, "@PARTNER_NUMBER", sPARTNER_NUMBER, 30);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parEXCHANGE_FOLDER = Sql.AddParameter(cmd, "@EXCHANGE_FOLDER", bEXCHANGE_FOLDER);
                IDbDataParameter parDATE_ENTERED = Sql.AddParameter(cmd, "@DATE_ENTERED", dtDATE_ENTERED);
                IDbDataParameter parDATE_MODIFIED = Sql.AddParameter(cmd, "@DATE_MODIFIED", dtDATE_MODIFIED);
                IDbDataParameter parBILLING_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET1", sBILLING_ADDRESS_STREET1, 150);
                IDbDataParameter parBILLING_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET2", sBILLING_ADDRESS_STREET2, 150);
                IDbDataParameter parBILLING_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET3", sBILLING_ADDRESS_STREET3, 150);
                IDbDataParameter parSHIPPING_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET1", sSHIPPING_ADDRESS_STREET1, 150);
                IDbDataParameter parSHIPPING_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET2", sSHIPPING_ADDRESS_STREET2, 150);
                IDbDataParameter parSHIPPING_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@SHIPPING_ADDRESS_STREET3", sSHIPPING_ADDRESS_STREET3, 150);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parNAICS_SET_NAME = Sql.AddParameter(cmd, "@NAICS_SET_NAME", sNAICS_SET_NAME, 4000);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        //NOT UPTODATE
        #region cmdPARTNERS_Import
        /// <summary>
        /// spPARTNERS_Import
        /// </summary>
        public static IDbCommand cmdPARTNERS_Import(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_Import";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 150);
            IDbDataParameter parPARTNER_TYPE = Sql.CreateParameter(cmd, "@PARTNER_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            IDbDataParameter parINDUSTRY = Sql.CreateParameter(cmd, "@INDUSTRY", "string", 25);
            IDbDataParameter parANNUAL_REVENUE = Sql.CreateParameter(cmd, "@ANNUAL_REVENUE", "string", 25);
            IDbDataParameter parPHONE_FAX = Sql.CreateParameter(cmd, "@PHONE_FAX", "string", 25);
            IDbDataParameter parBILLING_ADDRESS_STREET = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STREET", "string", 150);
            IDbDataParameter parBILLING_ADDRESS_CITY = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_CITY", "string", 100);
            IDbDataParameter parBILLING_ADDRESS_STATE = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STATE", "string", 100);
            IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parDESCRIPTION = Sql.CreateParameter(cmd, "@DESCRIPTION", "string", 104857600);
            IDbDataParameter parRATING = Sql.CreateParameter(cmd, "@RATING", "string", 25);
            IDbDataParameter parPHONE_OFFICE = Sql.CreateParameter(cmd, "@PHONE_OFFICE", "string", 25);
            IDbDataParameter parPHONE_ALTERNATE = Sql.CreateParameter(cmd, "@PHONE_ALTERNATE", "string", 25);
            IDbDataParameter parEMAIL1 = Sql.CreateParameter(cmd, "@EMAIL1", "string", 100);
            IDbDataParameter parEMAIL2 = Sql.CreateParameter(cmd, "@EMAIL2", "string", 100);
            IDbDataParameter parWEBSITE = Sql.CreateParameter(cmd, "@WEBSITE", "string", 255);
            IDbDataParameter parOWNERSHIP = Sql.CreateParameter(cmd, "@OWNERSHIP", "string", 100);
            IDbDataParameter parEMPLOYEES = Sql.CreateParameter(cmd, "@EMPLOYEES", "string", 10);
            IDbDataParameter parSIC_CODE = Sql.CreateParameter(cmd, "@SIC_CODE", "string", 10);
            IDbDataParameter parTICKER_SYMBOL = Sql.CreateParameter(cmd, "@TICKER_SYMBOL", "string", 10);
            IDbDataParameter parSHIPPING_ADDRESS_STREET = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_STREET", "string", 150);
            IDbDataParameter parSHIPPING_ADDRESS_CITY = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_CITY", "string", 100);
            IDbDataParameter parSHIPPING_ADDRESS_STATE = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_STATE", "string", 100);
            IDbDataParameter parSHIPPING_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parSHIPPING_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parPARTNER_NUMBER = Sql.CreateParameter(cmd, "@PARTNER_NUMBER", "string", 30);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parEXCHANGE_FOLDER = Sql.CreateParameter(cmd, "@EXCHANGE_FOLDER", "bool", 1);
            IDbDataParameter parDATE_ENTERED = Sql.CreateParameter(cmd, "@DATE_ENTERED", "DateTime", 8);
            IDbDataParameter parDATE_MODIFIED = Sql.CreateParameter(cmd, "@DATE_MODIFIED", "DateTime", 8);
            IDbDataParameter parBILLING_ADDRESS_STREET1 = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STREET1", "string", 150);
            IDbDataParameter parBILLING_ADDRESS_STREET2 = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STREET2", "string", 150);
            IDbDataParameter parBILLING_ADDRESS_STREET3 = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STREET3", "string", 150);
            IDbDataParameter parSHIPPING_ADDRESS_STREET1 = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_STREET1", "string", 150);
            IDbDataParameter parSHIPPING_ADDRESS_STREET2 = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_STREET2", "string", 150);
            IDbDataParameter parSHIPPING_ADDRESS_STREET3 = Sql.CreateParameter(cmd, "@SHIPPING_ADDRESS_STREET3", "string", 150);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parNAICS_SET_NAME = Sql.CreateParameter(cmd, "@NAICS_SET_NAME", "string", 4000);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spPARTNERS_InsRelated
        /// <summary>
        /// spPARTNERS_InsRelated
        /// </summary>
        public static void spPARTNERS_InsRelated(Guid gPARTNER_ID, string sPARENT_TYPE, Guid gPARENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_InsRelated";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_InsRelated
        /// <summary>
        /// spPARTNERS_InsRelated
        /// </summary>
        public static void spPARTNERS_InsRelated(Guid gPARTNER_ID, string sPARENT_TYPE, Guid gPARENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_InsRelated";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_InsRelated
        /// <summary>
        /// spPARTNERS_InsRelated
        /// </summary>
        public static IDbCommand cmdPARTNERS_InsRelated(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_InsRelated";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion


        #region spPARTNERS_MassDelete
        /// <summary>
        /// spPARTNERS_MassDelete
        /// </summary>
        public static void spPARTNERS_MassDelete(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_MassDelete";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_MassDelete
        /// <summary>
        /// spPARTNERS_MassDelete
        /// </summary>
        public static void spPARTNERS_MassDelete(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_MassDelete";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_MassDelete
        /// <summary>
        /// spPARTNERS_MassDelete
        /// </summary>
        public static IDbCommand cmdPARTNERS_MassDelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_MassDelete";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_MassSync
        /// <summary>
        /// spPARTNERS_MassSync
        /// </summary>
        public static void spPARTNERS_MassSync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_MassSync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_MassSync
        /// <summary>
        /// spPARTNERS_MassSync
        /// </summary>
        public static void spPARTNERS_MassSync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_MassSync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_MassSync
        /// <summary>
        /// spPARTNERS_MassSync
        /// </summary>
        public static IDbCommand cmdPARTNERS_MassSync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_MassSync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_MassUnsync
        /// <summary>
        /// spPARTNERS_MassUnsync
        /// </summary>
        public static void spPARTNERS_MassUnsync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_MassUnsync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_MassUnsync
        /// <summary>
        /// spPARTNERS_MassUnsync
        /// </summary>
        public static void spPARTNERS_MassUnsync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_MassUnsync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_MassUnsync
        /// <summary>
        /// spPARTNERS_MassUnsync
        /// </summary>
        public static IDbCommand cmdPARTNERS_MassUnsync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_MassUnsync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_MassUpdate
        /// <summary>
        /// spPARTNERS_MassUpdate
        /// </summary>
        public static void spPARTNERS_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, string sPARTNER_TYPE, string sINDUSTRY, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_MassUpdate";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                            IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_MassUpdate
        /// <summary>
        /// spPARTNERS_MassUpdate
        /// </summary>
        public static void spPARTNERS_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, string sPARTNER_TYPE, string sINDUSTRY, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_MassUpdate";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_MassUpdate
        /// <summary>
        /// spPARTNERS_MassUpdate
        /// </summary>
        public static IDbCommand cmdPARTNERS_MassUpdate(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_MassUpdate";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_TYPE = Sql.CreateParameter(cmd, "@PARTNER_TYPE", "string", 25);
            IDbDataParameter parINDUSTRY = Sql.CreateParameter(cmd, "@INDUSTRY", "string", 25);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parTEAM_SET_ADD = Sql.CreateParameter(cmd, "@TEAM_SET_ADD", "bool", 1);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parTAG_SET_ADD = Sql.CreateParameter(cmd, "@TAG_SET_ADD", "bool", 1);
            return cmd;
        }
        #endregion

        #region spPARTNERS_Merge
        /// <summary>
        /// spPARTNERS_Merge
        /// </summary>
        public static void spPARTNERS_Merge(Guid gID, Guid gMERGE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_Merge";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_Merge
        /// <summary>
        /// spPARTNERS_Merge
        /// </summary>
        public static void spPARTNERS_Merge(Guid gID, Guid gMERGE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_Merge";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_Merge
        /// <summary>
        /// spPARTNERS_Merge
        /// </summary>
        public static IDbCommand cmdPARTNERS_Merge(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_Merge";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parMERGE_ID = Sql.CreateParameter(cmd, "@MERGE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_New
        /// <summary>
        /// spPARTNERS_New
        /// </summary>
        public static void spPARTNERS_New(ref Guid gID, string sNAME, string sPHONE_OFFICE, string sWEBSITE, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_New";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                            IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                            IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spPARTNERS_New
        /// <summary>
        /// spPARTNERS_New
        /// </summary>
        public static void spPARTNERS_New(ref Guid gID, string sNAME, string sPHONE_OFFICE, string sWEBSITE, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_New";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdPARTNERS_New
        /// <summary>
        /// spPARTNERS_New
        /// </summary>
        public static IDbCommand cmdPARTNERS_New(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_New";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 150);
            IDbDataParameter parPHONE_OFFICE = Sql.CreateParameter(cmd, "@PHONE_OFFICE", "string", 25);
            IDbDataParameter parWEBSITE = Sql.CreateParameter(cmd, "@WEBSITE", "string", 255);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spPARTNERS_REQUESTS_Delete
        /// <summary>
        /// spPARTNERS_REQUESTS_Delete
        /// </summary>
        public static void spPARTNERS_REQUESTS_Delete(Guid gPARTNER_ID, Guid gREQUEST_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spPARTNERS_REQUESTS_Delet";
                            else
                                cmd.CommandText = "spPARTNERS_REQUESTS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_REQUESTS_Delete
        /// <summary>
        /// spPARTNERS_REQUESTS_Delete
        /// </summary>
        public static void spPARTNERS_REQUESTS_Delete(Guid gPARTNER_ID, Guid gREQUEST_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spPARTNERS_REQUESTS_Delet";
                else
                    cmd.CommandText = "spPARTNERS_REQUESTS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_REQUESTS_Delete
        /// <summary>
        /// spPARTNERS_REQUESTS_Delete
        /// </summary>
        public static IDbCommand cmdPARTNERS_REQUESTS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spPARTNERS_REQUESTS_Delet";
            else
                cmd.CommandText = "spPARTNERS_REQUESTS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_REQUESTS_Update
        /// <summary>
        /// spPARTNERS_REQUESTS_Update
        /// </summary>
        public static void spPARTNERS_REQUESTS_Update(Guid gPARTNER_ID, Guid gREQUEST_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spPARTNERS_REQUESTS_Updat";
                            else
                                cmd.CommandText = "spPARTNERS_REQUESTS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_REQUESTS_Update
        /// <summary>
        /// spPARTNERS_REQUESTS_Update
        /// </summary>
        public static void spPARTNERS_REQUESTS_Update(Guid gPARTNER_ID, Guid gREQUEST_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spPARTNERS_REQUESTS_Updat";
                else
                    cmd.CommandText = "spPARTNERS_REQUESTS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_REQUESTS_Update
        /// <summary>
        /// spPARTNERS_REQUESTS_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_REQUESTS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spPARTNERS_REQUESTS_Updat";
            else
                cmd.CommandText = "spPARTNERS_REQUESTS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_PARENT_Update
        /// <summary>
        /// spPARTNERS_PARENT_Update
        /// </summary>
        public static void spPARTNERS_PARENT_Update(Guid gPARTNER_ID, Guid gPARENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_PARENT_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_PARENT_Update
        /// <summary>
        /// spPARTNERS_PARENT_Update
        /// </summary>
        public static void spPARTNERS_PARENT_Update(Guid gPARTNER_ID, Guid gPARENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_PARENT_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_PARENT_Update
        /// <summary>
        /// spPARTNERS_PARENT_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_PARENT_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_PARENT_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_STREAM_InsertPost
        /// <summary>
        /// spPARTNERS_STREAM_InsertPost
        /// </summary>
        public static void spPARTNERS_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_STREAM_InsertPost";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                            IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                            IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                            IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_STREAM_InsertPost
        /// <summary>
        /// spPARTNERS_STREAM_InsertPost
        /// </summary>
        public static void spPARTNERS_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_STREAM_InsertPost";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_STREAM_InsertPost
        /// <summary>
        /// spPARTNERS_STREAM_InsertPost
        /// </summary>
        public static IDbCommand cmdPARTNERS_STREAM_InsertPost(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_STREAM_InsertPost";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 104857600);
            IDbDataParameter parRELATED_ID = Sql.CreateParameter(cmd, "@RELATED_ID", "Guid", 16);
            IDbDataParameter parRELATED_MODULE = Sql.CreateParameter(cmd, "@RELATED_MODULE", "string", 25);
            IDbDataParameter parRELATED_NAME = Sql.CreateParameter(cmd, "@RELATED_NAME", "string", 255);
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_Undelete
        /// <summary>
        /// spPARTNERS_Undelete
        /// </summary>
        public static void spPARTNERS_Undelete(Guid gID, string sAUDIT_TOKEN)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_Undelete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_Undelete
        /// <summary>
        /// spPARTNERS_Undelete
        /// </summary>
        public static void spPARTNERS_Undelete(Guid gID, string sAUDIT_TOKEN, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_Undelete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_Undelete
        /// <summary>
        /// spPARTNERS_Undelete
        /// </summary>
        public static IDbCommand cmdPARTNERS_Undelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_Undelete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parAUDIT_TOKEN = Sql.CreateParameter(cmd, "@AUDIT_TOKEN", "ansistring", 255);
            return cmd;
        }
        #endregion

        #region spPARTNERS_Update
        /// <summary>
        /// spPARTNERS_Update
        /// </summary>
        public static void spPARTNERS_Update(ref Guid gID, Guid gASSIGNED_USER_ID, string sNAME, string sPARTNER_TYPE, Guid gPARENT_ID, string sINDUSTRY, string sANNUAL_REVENUE, string sPHONE_FAX, string sBILLING_ADDRESS_STREET, string sBILLING_ADDRESS_CITY, string sBILLING_ADDRESS_STATE, string sBILLING_ADDRESS_POSTALCODE, string sBILLING_ADDRESS_COUNTRY, string sDESCRIPTION, string sRATING, string sPHONE_OFFICE, string sPHONE_ALTERNATE, string sEMAIL1, string sEMAIL2, string sWEBSITE, string sOWNERSHIP, string sEMPLOYEES, string sPARTNER_NUMBER, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bEXCHANGE_FOLDER, string sPICTURE, string sTAG_SET_NAME, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_Update";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                            IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                            IDbDataParameter parANNUAL_REVENUE = Sql.AddParameter(cmd, "@ANNUAL_REVENUE", sANNUAL_REVENUE, 25);
                            IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                            IDbDataParameter parBILLING_ADDRESS_STREET = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET", sBILLING_ADDRESS_STREET, 150);
                            IDbDataParameter parBILLING_ADDRESS_CITY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_CITY", sBILLING_ADDRESS_CITY, 100);
                            IDbDataParameter parBILLING_ADDRESS_STATE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STATE", sBILLING_ADDRESS_STATE, 100);
                            IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", sBILLING_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_COUNTRY", sBILLING_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                            IDbDataParameter parRATING = Sql.AddParameter(cmd, "@RATING", sRATING, 25);
                            IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                            IDbDataParameter parPHONE_ALTERNATE = Sql.AddParameter(cmd, "@PHONE_ALTERNATE", sPHONE_ALTERNATE, 25);
                            IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                            IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                            IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                            IDbDataParameter parOWNERSHIP = Sql.AddParameter(cmd, "@OWNERSHIP", sOWNERSHIP, 100);
                            IDbDataParameter parEMPLOYEES = Sql.AddParameter(cmd, "@EMPLOYEES", sEMPLOYEES, 10);
                            IDbDataParameter parPARTNER_NUMBER = Sql.AddParameter(cmd, "@PARTNER_NUMBER", sPARTNER_NUMBER, 30);
                            IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spPARTNERS_Update
        /// <summary>
        /// spPARTNERS_Update
        /// </summary>
        public static void spPARTNERS_Update(ref Guid gID, Guid gASSIGNED_USER_ID, string sNAME, string sPARTNER_TYPE, Guid gPARENT_ID, string sINDUSTRY, string sANNUAL_REVENUE, string sPHONE_FAX, string sBILLING_ADDRESS_STREET, string sBILLING_ADDRESS_CITY, string sBILLING_ADDRESS_STATE, string sBILLING_ADDRESS_POSTALCODE, string sBILLING_ADDRESS_COUNTRY, string sDESCRIPTION, string sRATING, string sPHONE_OFFICE, string sPHONE_ALTERNATE, string sEMAIL1, string sEMAIL2, string sWEBSITE, string sOWNERSHIP, string sEMPLOYEES, string sPARTNER_NUMBER, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bEXCHANGE_FOLDER, string sPICTURE, string sTAG_SET_NAME, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_Update";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                IDbDataParameter parPARTNER_TYPE = Sql.AddParameter(cmd, "@PARTNER_TYPE", sPARTNER_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                IDbDataParameter parINDUSTRY = Sql.AddParameter(cmd, "@INDUSTRY", sINDUSTRY, 25);
                IDbDataParameter parANNUAL_REVENUE = Sql.AddParameter(cmd, "@ANNUAL_REVENUE", sANNUAL_REVENUE, 25);
                IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                IDbDataParameter parBILLING_ADDRESS_STREET = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STREET", sBILLING_ADDRESS_STREET, 150);
                IDbDataParameter parBILLING_ADDRESS_CITY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_CITY", sBILLING_ADDRESS_CITY, 100);
                IDbDataParameter parBILLING_ADDRESS_STATE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_STATE", sBILLING_ADDRESS_STATE, 100);
                IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", sBILLING_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@BILLING_ADDRESS_COUNTRY", sBILLING_ADDRESS_COUNTRY, 100);
                IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                IDbDataParameter parRATING = Sql.AddParameter(cmd, "@RATING", sRATING, 25);
                IDbDataParameter parPHONE_OFFICE = Sql.AddParameter(cmd, "@PHONE_OFFICE", sPHONE_OFFICE, 25);
                IDbDataParameter parPHONE_ALTERNATE = Sql.AddParameter(cmd, "@PHONE_ALTERNATE", sPHONE_ALTERNATE, 25);
                IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                IDbDataParameter parWEBSITE = Sql.AddParameter(cmd, "@WEBSITE", sWEBSITE, 255);
                IDbDataParameter parOWNERSHIP = Sql.AddParameter(cmd, "@OWNERSHIP", sOWNERSHIP, 100);
                IDbDataParameter parEMPLOYEES = Sql.AddParameter(cmd, "@EMPLOYEES", sEMPLOYEES, 10);
                IDbDataParameter parPARTNER_NUMBER = Sql.AddParameter(cmd, "@PARTNER_NUMBER", sPARTNER_NUMBER, 30);
                IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdPARTNERS_Update
        /// <summary>
        /// spPARTNERS_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_Update";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 150);
            IDbDataParameter parPARTNER_TYPE = Sql.CreateParameter(cmd, "@PARTNER_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            IDbDataParameter parINDUSTRY = Sql.CreateParameter(cmd, "@INDUSTRY", "string", 25);
            IDbDataParameter parANNUAL_REVENUE = Sql.CreateParameter(cmd, "@ANNUAL_REVENUE", "string", 25);
            IDbDataParameter parPHONE_FAX = Sql.CreateParameter(cmd, "@PHONE_FAX", "string", 25);
            IDbDataParameter parBILLING_ADDRESS_STREET = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STREET", "string", 150);
            IDbDataParameter parBILLING_ADDRESS_CITY = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_CITY", "string", 100);
            IDbDataParameter parBILLING_ADDRESS_STATE = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_STATE", "string", 100);
            IDbDataParameter parBILLING_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parBILLING_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@BILLING_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parDESCRIPTION = Sql.CreateParameter(cmd, "@DESCRIPTION", "string", 104857600);
            IDbDataParameter parRATING = Sql.CreateParameter(cmd, "@RATING", "string", 25);
            IDbDataParameter parPHONE_OFFICE = Sql.CreateParameter(cmd, "@PHONE_OFFICE", "string", 25);
            IDbDataParameter parPHONE_ALTERNATE = Sql.CreateParameter(cmd, "@PHONE_ALTERNATE", "string", 25);
            IDbDataParameter parEMAIL1 = Sql.CreateParameter(cmd, "@EMAIL1", "string", 100);
            IDbDataParameter parEMAIL2 = Sql.CreateParameter(cmd, "@EMAIL2", "string", 100);
            IDbDataParameter parWEBSITE = Sql.CreateParameter(cmd, "@WEBSITE", "string", 255);
            IDbDataParameter parOWNERSHIP = Sql.CreateParameter(cmd, "@OWNERSHIP", "string", 100);
            IDbDataParameter parEMPLOYEES = Sql.CreateParameter(cmd, "@EMPLOYEES", "string", 10);
            IDbDataParameter parPARTNER_NUMBER = Sql.CreateParameter(cmd, "@PARTNER_NUMBER", "string", 30);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parEXCHANGE_FOLDER = Sql.CreateParameter(cmd, "@EXCHANGE_FOLDER", "bool", 1);
            IDbDataParameter parPICTURE = Sql.CreateParameter(cmd, "@PICTURE", "string", 104857600);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spPARTNERS_USERS_Delete
        /// <summary>
        /// spPARTNERS_USERS_Delete
        /// </summary>
        public static void spPARTNERS_USERS_Delete(Guid gPARTNER_ID, Guid gUSER_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_USERS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_USERS_Delete
        /// <summary>
        /// spPARTNERS_USERS_Delete
        /// </summary>
        public static void spPARTNERS_USERS_Delete(Guid gPARTNER_ID, Guid gUSER_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_USERS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_USERS_Delete
        /// <summary>
        /// spPARTNERS_USERS_Delete
        /// </summary>
        public static IDbCommand cmdPARTNERS_USERS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_USERS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_USERS_Update
        /// <summary>
        /// spPARTNERS_USERS_Update
        /// </summary>
        public static void spPARTNERS_USERS_Update(Guid gPARTNER_ID, Guid gUSER_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spPARTNERS_USERS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_USERS_Update
        /// <summary>
        /// spPARTNERS_USERS_Update
        /// </summary>
        public static void spPARTNERS_USERS_Update(Guid gPARTNER_ID, Guid gUSER_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spPARTNERS_USERS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_USERS_Update
        /// <summary>
        /// spPARTNERS_USERS_Update
        /// </summary>
        public static IDbCommand cmdPARTNERS_USERS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spPARTNERS_USERS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_RESOURCES_Delete
        /// <summary>
        /// spREQUESTS_RESOURCES_Delete
        /// </summary>
        public static void spREQUESTS_RESOURCES_Delete(Guid gREQUEST_ID, Guid gRESOURCE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_RESOURCES_Delet";
                            else
                                cmd.CommandText = "spREQUESTS_RESOURCES_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_RESOURCES_Delete
        /// <summary>
        /// spREQUESTS_RESOURCES_Delete
        /// </summary>
        public static void spREQUESTS_RESOURCES_Delete(Guid gREQUEST_ID, Guid gRESOURCE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_RESOURCES_Delet";
                else
                    cmd.CommandText = "spREQUESTS_RESOURCES_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_RESOURCES_Delete
        /// <summary>
        /// spREQUESTS_RESOURCES_Delete
        /// </summary>
        public static IDbCommand cmdREQUESTS_RESOURCES_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_RESOURCES_Delet";
            else
                cmd.CommandText = "spREQUESTS_RESOURCES_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_RESOURCES_Update
        /// <summary>
        /// spREQUESTS_RESOURCES_Update
        /// </summary>
        public static void spREQUESTS_RESOURCES_Update(Guid gREQUEST_ID, Guid gRESOURCE_ID, string sRESOURCE_ROLE)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_RESOURCES_Updat";
                            else
                                cmd.CommandText = "spREQUESTS_RESOURCES_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parRESOURCE_ROLE = Sql.AddParameter(cmd, "@RESOURCE_ROLE", sRESOURCE_ROLE, 50);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_RESOURCES_Update
        /// <summary>
        /// spREQUESTS_RESOURCES_Update
        /// </summary>
        public static void spREQUESTS_RESOURCES_Update(Guid gREQUEST_ID, Guid gRESOURCE_ID, string sRESOURCE_ROLE, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_RESOURCES_Updat";
                else
                    cmd.CommandText = "spREQUESTS_RESOURCES_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parRESOURCE_ROLE = Sql.AddParameter(cmd, "@RESOURCE_ROLE", sRESOURCE_ROLE, 50);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_RESOURCES_Update
        /// <summary>
        /// spREQUESTS_RESOURCES_Update
        /// </summary>
        public static IDbCommand cmdREQUESTS_RESOURCES_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_RESOURCES_Updat";
            else
                cmd.CommandText = "spREQUESTS_RESOURCES_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ROLE = Sql.CreateParameter(cmd, "@RESOURCE_ROLE", "string", 50);
            return cmd;
        }
        #endregion

        #region spREQUESTS_Delete
        /// <summary>
        /// spREQUESTS_Delete
        /// </summary>
        public static void spREQUESTS_Delete(Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_Delete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_Delete
        /// <summary>
        /// spREQUESTS_Delete
        /// </summary>
        public static void spREQUESTS_Delete(Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_Delete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_Delete
        /// <summary>
        /// spREQUESTS_Delete
        /// </summary>
        public static IDbCommand cmdREQUESTS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_Delete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_DOCUMENTS_Delete
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Delete
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_Delete(Guid gREQUEST_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_Dele";
                            else
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_DOCUMENTS_Delete
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Delete
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_Delete(Guid gREQUEST_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_Dele";
                else
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_DOCUMENTS_Delete
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Delete
        /// </summary>
        public static IDbCommand cmdREQUESTS_DOCUMENTS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_DOCUMENTS_Dele";
            else
                cmd.CommandText = "spREQUESTS_DOCUMENTS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_DOCUMENTS_GetLatest
        /// <summary>
        /// spREQUESTS_DOCUMENTS_GetLatest
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_GetLatest(Guid gREQUEST_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_GetL";
                            else
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_GetLatest";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_DOCUMENTS_GetLatest
        /// <summary>
        /// spREQUESTS_DOCUMENTS_GetLatest
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_GetLatest(Guid gREQUEST_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_GetL";
                else
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_GetLatest";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_DOCUMENTS_GetLatest
        /// <summary>
        /// spREQUESTS_DOCUMENTS_GetLatest
        /// </summary>
        public static IDbCommand cmdREQUESTS_DOCUMENTS_GetLatest(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_DOCUMENTS_GetL";
            else
                cmd.CommandText = "spREQUESTS_DOCUMENTS_GetLatest";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_DOCUMENTS_Update
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Update
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_Update(Guid gREQUEST_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_Upda";
                            else
                                cmd.CommandText = "spREQUESTS_DOCUMENTS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_DOCUMENTS_Update
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Update
        /// </summary>
        public static void spREQUESTS_DOCUMENTS_Update(Guid gREQUEST_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_Upda";
                else
                    cmd.CommandText = "spREQUESTS_DOCUMENTS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_DOCUMENTS_Update
        /// <summary>
        /// spREQUESTS_DOCUMENTS_Update
        /// </summary>
        public static IDbCommand cmdREQUESTS_DOCUMENTS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_DOCUMENTS_Upda";
            else
                cmd.CommandText = "spREQUESTS_DOCUMENTS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_InsRelated
        /// <summary>
        /// spREQUESTS_InsRelated
        /// </summary>
        public static void spREQUESTS_InsRelated(Guid gREQUEST_ID, string sPARENT_TYPE, Guid gPARENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_InsRelated";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_InsRelated
        /// <summary>
        /// spREQUESTS_InsRelated
        /// </summary>
        public static void spREQUESTS_InsRelated(Guid gREQUEST_ID, string sPARENT_TYPE, Guid gPARENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_InsRelated";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_InsRelated
        /// <summary>
        /// spREQUESTS_InsRelated
        /// </summary>
        public static IDbCommand cmdREQUESTS_InsRelated(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_InsRelated";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_MassDelete
        /// <summary>
        /// spREQUESTS_MassDelete
        /// </summary>
        public static void spREQUESTS_MassDelete(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_MassDelete";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_MassDelete
        /// <summary>
        /// spREQUESTS_MassDelete
        /// </summary>
        public static void spREQUESTS_MassDelete(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_MassDelete";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_MassDelete
        /// <summary>
        /// spREQUESTS_MassDelete
        /// </summary>
        public static IDbCommand cmdREQUESTS_MassDelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_MassDelete";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_MassSync
        /// <summary>
        /// spREQUESTS_MassSync
        /// </summary>
        public static void spREQUESTS_MassSync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_MassSync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_MassSync
        /// <summary>
        /// spREQUESTS_MassSync
        /// </summary>
        public static void spREQUESTS_MassSync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_MassSync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_MassSync
        /// <summary>
        /// spREQUESTS_MassSync
        /// </summary>
        public static IDbCommand cmdREQUESTS_MassSync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_MassSync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_MassUnsync
        /// <summary>
        /// spREQUESTS_MassUnsync
        /// </summary>
        public static void spREQUESTS_MassUnsync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_MassUnsync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_MassUnsync
        /// <summary>
        /// spREQUESTS_MassUnsync
        /// </summary>
        public static void spREQUESTS_MassUnsync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_MassUnsync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_MassUnsync
        /// <summary>
        /// spREQUESTS_MassUnsync
        /// </summary>
        public static IDbCommand cmdREQUESTS_MassUnsync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_MassUnsync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_MassUpdate
        /// <summary>
        /// spREQUESTS_MassUpdate
        /// </summary>
        public static void spREQUESTS_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, string sREQUEST_TYPE, string sLEAD_SOURCE, DateTime dtDATE_CLOSED, string sSALES_STAGE, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_MassUpdate";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parREQUEST_TYPE = Sql.AddParameter(cmd, "@REQUEST_TYPE", sREQUEST_TYPE, 25);
                            IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                            IDbDataParameter parSALES_STAGE = Sql.AddParameter(cmd, "@SALES_STAGE", sSALES_STAGE, 25);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_MassUpdate
        /// <summary>
        /// spREQUESTS_MassUpdate
        /// </summary>
        public static void spREQUESTS_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, string sREQUEST_TYPE, string sLEAD_SOURCE, DateTime dtDATE_CLOSED, string sSALES_STAGE, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_MassUpdate";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parREQUEST_TYPE = Sql.AddParameter(cmd, "@REQUEST_TYPE", sREQUEST_TYPE, 25);
                IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                IDbDataParameter parSALES_STAGE = Sql.AddParameter(cmd, "@SALES_STAGE", sSALES_STAGE, 25);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_MassUpdate
        /// <summary>
        /// spREQUESTS_MassUpdate
        /// </summary>
        public static IDbCommand cmdREQUESTS_MassUpdate(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_MassUpdate";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_TYPE = Sql.CreateParameter(cmd, "@REQUEST_TYPE", "string", 25);
            IDbDataParameter parDATE_CLOSED = Sql.CreateParameter(cmd, "@DATE_CLOSED", "DateTime", 8);
            IDbDataParameter parSALES_STAGE = Sql.CreateParameter(cmd, "@SALES_STAGE", "string", 25);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parTEAM_SET_ADD = Sql.CreateParameter(cmd, "@TEAM_SET_ADD", "bool", 1);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parTAG_SET_ADD = Sql.CreateParameter(cmd, "@TAG_SET_ADD", "bool", 1);
            return cmd;
        }
        #endregion

        #region spREQUESTS_Merge
        /// <summary>
        /// spREQUESTS_Merge
        /// </summary>
        public static void spREQUESTS_Merge(Guid gID, Guid gMERGE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_Merge";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_Merge
        /// <summary>
        /// spREQUESTS_Merge
        /// </summary>
        public static void spREQUESTS_Merge(Guid gID, Guid gMERGE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_Merge";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_Merge
        /// <summary>
        /// spREQUESTS_Merge
        /// </summary>
        public static IDbCommand cmdREQUESTS_Merge(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_Merge";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parMERGE_ID = Sql.CreateParameter(cmd, "@MERGE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_New
        /// <summary>
        /// spREQUESTS_New
        /// </summary>
        public static void spREQUESTS_New(ref Guid gID, Guid gPARTNER_ID, string sNAME, DateTime dtDATE_CLOSED, string sSALES_STAGE, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, Guid gB2C_RESOURCE_ID, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_New";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                            IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                            IDbDataParameter parSALES_STAGE = Sql.AddParameter(cmd, "@SALES_STAGE", sSALES_STAGE, 25);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parB2C_RESOURCE_ID = Sql.AddParameter(cmd, "@B2C_RESOURCE_ID", gB2C_RESOURCE_ID);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spREQUESTS_New
        /// <summary>
        /// spREQUESTS_New
        /// </summary>
        public static void spREQUESTS_New(ref Guid gID, Guid gPARTNER_ID, string sNAME, DateTime dtDATE_CLOSED, string sSALES_STAGE, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, Guid gB2C_RESOURCE_ID, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_New";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                IDbDataParameter parSALES_STAGE = Sql.AddParameter(cmd, "@SALES_STAGE", sSALES_STAGE, 25);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parB2C_RESOURCE_ID = Sql.AddParameter(cmd, "@B2C_RESOURCE_ID", gB2C_RESOURCE_ID);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdREQUESTS_New
        /// <summary>
        /// spREQUESTS_New
        /// </summary>
        public static IDbCommand cmdREQUESTS_New(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_New";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 150);
            IDbDataParameter parDATE_CLOSED = Sql.CreateParameter(cmd, "@DATE_CLOSED", "DateTime", 8);
            IDbDataParameter parSALES_STAGE = Sql.CreateParameter(cmd, "@SALES_STAGE", "string", 25);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parB2C_RESOURCE_ID = Sql.CreateParameter(cmd, "@B2C_RESOURCE_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spREQUESTS_STREAM_InsertPost
        /// <summary>
        /// spREQUESTS_STREAM_InsertPost
        /// </summary>
        public static void spREQUESTS_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spREQUESTS_STREAM_InsertP";
                            else
                                cmd.CommandText = "spREQUESTS_STREAM_InsertPost";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                            IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                            IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                            IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_STREAM_InsertPost
        /// <summary>
        /// spREQUESTS_STREAM_InsertPost
        /// </summary>
        public static void spREQUESTS_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spREQUESTS_STREAM_InsertP";
                else
                    cmd.CommandText = "spREQUESTS_STREAM_InsertPost";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_STREAM_InsertPost
        /// <summary>
        /// spREQUESTS_STREAM_InsertPost
        /// </summary>
        public static IDbCommand cmdREQUESTS_STREAM_InsertPost(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spREQUESTS_STREAM_InsertP";
            else
                cmd.CommandText = "spREQUESTS_STREAM_InsertPost";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 104857600);
            IDbDataParameter parRELATED_ID = Sql.CreateParameter(cmd, "@RELATED_ID", "Guid", 16);
            IDbDataParameter parRELATED_MODULE = Sql.CreateParameter(cmd, "@RELATED_MODULE", "string", 25);
            IDbDataParameter parRELATED_NAME = Sql.CreateParameter(cmd, "@RELATED_NAME", "string", 255);
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_Undelete
        /// <summary>
        /// spREQUESTS_Undelete
        /// </summary>
        public static void spREQUESTS_Undelete(Guid gID, string sAUDIT_TOKEN)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_Undelete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_Undelete
        /// <summary>
        /// spREQUESTS_Undelete
        /// </summary>
        public static void spREQUESTS_Undelete(Guid gID, string sAUDIT_TOKEN, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_Undelete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_Undelete
        /// <summary>
        /// spREQUESTS_Undelete
        /// </summary>
        public static IDbCommand cmdREQUESTS_Undelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_Undelete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parAUDIT_TOKEN = Sql.CreateParameter(cmd, "@AUDIT_TOKEN", "ansistring", 255);
            return cmd;
        }
        #endregion

        #region spREQUESTS_Update
        /// <summary>
        /// spREQUESTS_Update
        /// </summary>
        public static void spREQUESTS_Update(ref Guid gID, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, string sNAME, string sREQUEST_TYPE, DateTime dtDATE_CLOSED, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, string sPARTNER_NAME, Guid gTEAM_ID, string sTEAM_SET_LIST, Guid gCAMPAIGN_ID, bool bEXCHANGE_FOLDER, Guid gB2C_RESOURCE_ID, string sTAG_SET_NAME, string sREQUEST_NUMBER, string sASSIGNED_SET_LIST, Guid sORIGINATOR_ID, string sORIGINATOR, string sRFP, string sCOLLAB, string sDEPARTMENT)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_Update";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                            IDbDataParameter parREQUEST_TYPE = Sql.AddParameter(cmd, "@REQUEST_TYPE", sREQUEST_TYPE, 255);
                            IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                            IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            IDbDataParameter parPARTNER_NAME = Sql.AddParameter(cmd, "@PARTNER_NAME", sPARTNER_NAME, 100);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parCAMPAIGN_ID = Sql.AddParameter(cmd, "@CAMPAIGN_ID", gCAMPAIGN_ID);
                            IDbDataParameter parEXCHANGE_FOLDER = Sql.AddParameter(cmd, "@EXCHANGE_FOLDER", bEXCHANGE_FOLDER);
                            IDbDataParameter parB2C_RESOURCE_ID = Sql.AddParameter(cmd, "@B2C_RESOURCE_ID", gB2C_RESOURCE_ID);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parREQUEST_NUMBER = Sql.AddParameter(cmd, "@REQUEST_NUMBER", sREQUEST_NUMBER, 30);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            IDbDataParameter parORIGINATOR_ID = Sql.AddParameter(cmd, "@ORIGINATOR_ID", sORIGINATOR_ID);
                            IDbDataParameter parORIGINATOR = Sql.AddParameter(cmd, "@ORIGINATOR", sORIGINATOR, 64);
                            IDbDataParameter parRFP = Sql.AddAnsiParam(cmd, "@RFP", sRFP, 256);
                            IDbDataParameter parCOLLAB = Sql.AddAnsiParam(cmd, "@COLLAB", sCOLLAB, 256);
                            IDbDataParameter parDEPARTMENT = Sql.AddAnsiParam(cmd, "@DEPARTMENT", sDEPARTMENT, 64);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spREQUESTS_Update
        /// <summary>
        /// spREQUESTS_Update
        /// </summary>
        public static void spREQUESTS_Update(ref Guid gID, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, string sNAME, string sREQUEST_TYPE, DateTime dtDATE_CLOSED, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, string sPARTNER_NAME, Guid gTEAM_ID, string sTEAM_SET_LIST, Guid gCAMPAIGN_ID, bool bEXCHANGE_FOLDER, Guid gB2C_RESOURCE_ID, string sTAG_SET_NAME, string sREQUEST_NUMBER, string sASSIGNED_SET_LIST, Guid sORIGINATOR_ID, string sORIGINATOR, string sRFP, string sCOLLAB, string sDEPARTMENT, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_Update";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME, 150);
                IDbDataParameter parREQUEST_TYPE = Sql.AddParameter(cmd, "@REQUEST_TYPE", sREQUEST_TYPE, 255);
                IDbDataParameter parDATE_CLOSED = Sql.AddParameter(cmd, "@DATE_CLOSED", dtDATE_CLOSED);
                IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                IDbDataParameter parPARTNER_NAME = Sql.AddParameter(cmd, "@PARTNER_NAME", sPARTNER_NAME, 100);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parCAMPAIGN_ID = Sql.AddParameter(cmd, "@CAMPAIGN_ID", gCAMPAIGN_ID);
                IDbDataParameter parEXCHANGE_FOLDER = Sql.AddParameter(cmd, "@EXCHANGE_FOLDER", bEXCHANGE_FOLDER);
                IDbDataParameter parB2C_RESOURCE_ID = Sql.AddParameter(cmd, "@B2C_RESOURCE_ID", gB2C_RESOURCE_ID);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parREQUEST_NUMBER = Sql.AddParameter(cmd, "@REQUEST_NUMBER", sREQUEST_NUMBER, 30);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                IDbDataParameter parORIGINATOR_ID = Sql.AddParameter(cmd, "@ORIGINATOR_ID", sORIGINATOR_ID);
                IDbDataParameter parORIGINATOR = Sql.AddParameter(cmd, "@ORIGINATOR", sORIGINATOR, 64);
                IDbDataParameter parRFP = Sql.AddAnsiParam(cmd, "@RFP", sRFP, 256);
                IDbDataParameter parCOLLAB = Sql.AddAnsiParam(cmd, "@COLLAB", sCOLLAB, 256);
                IDbDataParameter parDEPARTMENT = Sql.AddAnsiParam(cmd, "@DEPARTMENT", sDEPARTMENT, 64);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdREQUESTS_Update
        /// <summary>
        /// spREQUESTS_Update
        /// </summary>
        public static IDbCommand cmdREQUESTS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_Update";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 150);
            IDbDataParameter parREQUEST_TYPE = Sql.CreateParameter(cmd, "@REQUEST_TYPE", "string", 255);
            IDbDataParameter parDATE_CLOSED = Sql.CreateParameter(cmd, "@DATE_CLOSED", "DateTime", 8);
            IDbDataParameter parDESCRIPTION = Sql.CreateParameter(cmd, "@DESCRIPTION", "string", 104857600);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            IDbDataParameter parPARTNER_NAME = Sql.CreateParameter(cmd, "@PARTNER_NAME", "string", 100);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parCAMPAIGN_ID = Sql.CreateParameter(cmd, "@CAMPAIGN_ID", "Guid", 16);
            IDbDataParameter parEXCHANGE_FOLDER = Sql.CreateParameter(cmd, "@EXCHANGE_FOLDER", "bool", 1);
            IDbDataParameter parB2C_RESOURCE_ID = Sql.CreateParameter(cmd, "@B2C_RESOURCE_ID", "Guid", 16);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parREQUEST_NUMBER = Sql.CreateParameter(cmd, "@REQUEST_NUMBER", "string", 30);
            IDbDataParameter parORIGINATOR_ID = Sql.CreateParameter(cmd, "@ORIGINATOR_ID", "Guid", 16);
            IDbDataParameter parORIGINATOR = Sql.CreateParameter(cmd, "@ORIGINATOR", "string", 64);
            IDbDataParameter parRFP        = Sql.CreateParameter(cmd, "@RFP", "ansistring", 256);
            IDbDataParameter parCOLLAB     = Sql.CreateParameter(cmd, "@COLLAB", "ansistring", 256);
            IDbDataParameter parDEPARTMENT = Sql.CreateParameter(cmd, "@DEPARTMENT", "ansistring", 32);
            IDbDataParameter par = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spREQUESTS_USERS_Delete
        /// <summary>
        /// spREQUESTS_USERS_Delete
        /// </summary>
        public static void spREQUESTS_USERS_Delete(Guid gREQUEST_ID, Guid gUSER_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_USERS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_USERS_Delete
        /// <summary>
        /// spREQUESTS_USERS_Delete
        /// </summary>
        public static void spREQUESTS_USERS_Delete(Guid gREQUEST_ID, Guid gUSER_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_USERS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_USERS_Delete
        /// <summary>
        /// spREQUESTS_USERS_Delete
        /// </summary>
        public static IDbCommand cmdREQUESTS_USERS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_USERS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spREQUESTS_USERS_Update
        /// <summary>
        /// spREQUESTS_USERS_Update
        /// </summary>
        public static void spREQUESTS_USERS_Update(Guid gREQUEST_ID, Guid gUSER_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spREQUESTS_USERS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spREQUESTS_USERS_Update
        /// <summary>
        /// spREQUESTS_USERS_Update
        /// </summary>
        public static void spREQUESTS_USERS_Update(Guid gREQUEST_ID, Guid gUSER_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spREQUESTS_USERS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parREQUEST_ID = Sql.AddParameter(cmd, "@REQUEST_ID", gREQUEST_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdREQUESTS_USERS_Update
        /// <summary>
        /// spREQUESTS_USERS_Update
        /// </summary>
        public static IDbCommand cmdREQUESTS_USERS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spREQUESTS_USERS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parREQUEST_ID = Sql.CreateParameter(cmd, "@REQUEST_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion
        #region spRESOURCES_Delete
        /// <summary>
        /// spRESOURCES_Delete
        /// </summary>
        public static void spRESOURCES_Delete(Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_Delete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_Delete
        /// <summary>
        /// spRESOURCES_Delete
        /// </summary>
        public static void spRESOURCES_Delete(Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_Delete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_Delete
        /// <summary>
        /// spRESOURCES_Delete
        /// </summary>
        public static IDbCommand cmdRESOURCES_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_Delete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_DOCUMENTS_Delete
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Delete
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_Delete(Guid gRESOURCE_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_DOCUMENTS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_DOCUMENTS_Delete
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Delete
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_Delete(Guid gRESOURCE_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_DOCUMENTS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_DOCUMENTS_Delete
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Delete
        /// </summary>
        public static IDbCommand cmdRESOURCES_DOCUMENTS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_DOCUMENTS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_DOCUMENTS_GetLatest
        /// <summary>
        /// spRESOURCES_DOCUMENTS_GetLatest
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_GetLatest(Guid gRESOURCE_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_DOCUMENTS_GetLatest";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_DOCUMENTS_GetLatest
        /// <summary>
        /// spRESOURCES_DOCUMENTS_GetLatest
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_GetLatest(Guid gRESOURCE_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_DOCUMENTS_GetLatest";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_DOCUMENTS_GetLatest
        /// <summary>
        /// spRESOURCES_DOCUMENTS_GetLatest
        /// </summary>
        public static IDbCommand cmdRESOURCES_DOCUMENTS_GetLatest(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_DOCUMENTS_GetLatest";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_DOCUMENTS_Update
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Update
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_Update(Guid gRESOURCE_ID, Guid gDOCUMENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_DOCUMENTS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_DOCUMENTS_Update
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Update
        /// </summary>
        public static void spRESOURCES_DOCUMENTS_Update(Guid gRESOURCE_ID, Guid gDOCUMENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_DOCUMENTS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parDOCUMENT_ID = Sql.AddParameter(cmd, "@DOCUMENT_ID", gDOCUMENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_DOCUMENTS_Update
        /// <summary>
        /// spRESOURCES_DOCUMENTS_Update
        /// </summary>
        public static IDbCommand cmdRESOURCES_DOCUMENTS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_DOCUMENTS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parDOCUMENT_ID = Sql.CreateParameter(cmd, "@DOCUMENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        //OLD
        #region spRESOURCES_Import
        /// <summary>
        /// spRESOURCES_Import
        /// </summary>
        public static void spRESOURCES_Import(ref Guid gID, Guid gASSIGNED_USER_ID, string sSALUTATION, string sFIRST_NAME, string sLAST_NAME, Guid gPARTNER_ID, string sLEAD_SOURCE, string sTITLE, string sDEPARTMENT, Guid gREPORTS_TO_ID, DateTime dtBIRTHDATE, bool bDO_NOT_CALL, string sPHONE_HOME, string sPHONE_MOBILE, string sPHONE_WORK, string sPHONE_OTHER, string sPHONE_FAX, string sEMAIL1, string sEMAIL2, string sASSISTANT, string sASSISTANT_PHONE, bool bEMAIL_OPT_OUT, bool bINVALID_EMAIL, string sPRIMARY_ADDRESS_STREET, string sPRIMARY_ADDRESS_CITY, string sPRIMARY_ADDRESS_STATE, string sPRIMARY_ADDRESS_POSTALCODE, string sPRIMARY_ADDRESS_COUNTRY, string sALT_ADDRESS_STREET, string sALT_ADDRESS_CITY, string sALT_ADDRESS_STATE, string sALT_ADDRESS_POSTALCODE, string sALT_ADDRESS_COUNTRY, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, bool bSYNC_CONTACT, Guid gTEAM_ID, string sPARTNER_NAME, string sTEAM_SET_LIST, DateTime dtDATE_ENTERED, DateTime dtDATE_MODIFIED, string sPRIMARY_ADDRESS_STREET1, string sPRIMARY_ADDRESS_STREET2, string sPRIMARY_ADDRESS_STREET3, string sALT_ADDRESS_STREET1, string sALT_ADDRESS_STREET2, string sALT_ADDRESS_STREET3, string sSMS_OPT_IN, string sTWITTER_SCREEN_NAME, Guid gLEAD_ID, string sTAG_SET_NAME, string sPICTURE, string sRESOURCE_NUMBER, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_Import";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parSALUTATION = Sql.AddParameter(cmd, "@SALUTATION", sSALUTATION, 25);
                            IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                            IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parLEAD_SOURCE = Sql.AddParameter(cmd, "@LEAD_SOURCE", sLEAD_SOURCE, 100);
                            IDbDataParameter parTITLE = Sql.AddParameter(cmd, "@TITLE", sTITLE, 50);
                            IDbDataParameter parDEPARTMENT = Sql.AddParameter(cmd, "@DEPARTMENT", sDEPARTMENT, 100);
                            IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                            IDbDataParameter parBIRTHDATE = Sql.AddParameter(cmd, "@BIRTHDATE", dtBIRTHDATE);
                            IDbDataParameter parDO_NOT_CALL = Sql.AddParameter(cmd, "@DO_NOT_CALL", bDO_NOT_CALL);
                            IDbDataParameter parPHONE_HOME = Sql.AddParameter(cmd, "@PHONE_HOME", sPHONE_HOME, 25);
                            IDbDataParameter parPHONE_MOBILE = Sql.AddParameter(cmd, "@PHONE_MOBILE", sPHONE_MOBILE, 25);
                            IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                            IDbDataParameter parPHONE_OTHER = Sql.AddParameter(cmd, "@PHONE_OTHER", sPHONE_OTHER, 25);
                            IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                            IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                            IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                            IDbDataParameter parASSISTANT = Sql.AddParameter(cmd, "@ASSISTANT", sASSISTANT, 75);
                            IDbDataParameter parASSISTANT_PHONE = Sql.AddParameter(cmd, "@ASSISTANT_PHONE", sASSISTANT_PHONE, 25);
                            IDbDataParameter parEMAIL_OPT_OUT = Sql.AddParameter(cmd, "@EMAIL_OPT_OUT", bEMAIL_OPT_OUT);
                            IDbDataParameter parINVALID_EMAIL = Sql.AddParameter(cmd, "@INVALID_EMAIL", bINVALID_EMAIL);
                            IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET", sPRIMARY_ADDRESS_STREET, 150);
                            IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_CITY", sPRIMARY_ADDRESS_CITY, 100);
                            IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STATE", sPRIMARY_ADDRESS_STATE, 100);
                            IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", sPRIMARY_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", sPRIMARY_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parALT_ADDRESS_STREET = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET", sALT_ADDRESS_STREET, 150);
                            IDbDataParameter parALT_ADDRESS_CITY = Sql.AddParameter(cmd, "@ALT_ADDRESS_CITY", sALT_ADDRESS_CITY, 100);
                            IDbDataParameter parALT_ADDRESS_STATE = Sql.AddParameter(cmd, "@ALT_ADDRESS_STATE", sALT_ADDRESS_STATE, 100);
                            IDbDataParameter parALT_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@ALT_ADDRESS_POSTALCODE", sALT_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parALT_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@ALT_ADDRESS_COUNTRY", sALT_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            IDbDataParameter parSYNC_CONTACT = Sql.AddParameter(cmd, "@SYNC_CONTACT", bSYNC_CONTACT);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parPARTNER_NAME = Sql.AddParameter(cmd, "@PARTNER_NAME", sPARTNER_NAME, 100);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parDATE_ENTERED = Sql.AddParameter(cmd, "@DATE_ENTERED", dtDATE_ENTERED);
                            IDbDataParameter parDATE_MODIFIED = Sql.AddParameter(cmd, "@DATE_MODIFIED", dtDATE_MODIFIED);
                            IDbDataParameter parPRIMARY_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET1", sPRIMARY_ADDRESS_STREET1, 150);
                            IDbDataParameter parPRIMARY_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET2", sPRIMARY_ADDRESS_STREET2, 150);
                            IDbDataParameter parPRIMARY_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET3", sPRIMARY_ADDRESS_STREET3, 150);
                            IDbDataParameter parALT_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET1", sALT_ADDRESS_STREET1, 150);
                            IDbDataParameter parALT_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET2", sALT_ADDRESS_STREET2, 150);
                            IDbDataParameter parALT_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET3", sALT_ADDRESS_STREET3, 150);
                            IDbDataParameter parSMS_OPT_IN = Sql.AddParameter(cmd, "@SMS_OPT_IN", sSMS_OPT_IN, 25);
                            IDbDataParameter parTWITTER_SCREEN_NAME = Sql.AddParameter(cmd, "@TWITTER_SCREEN_NAME", sTWITTER_SCREEN_NAME, 20);
                            IDbDataParameter parLEAD_ID = Sql.AddParameter(cmd, "@LEAD_ID", gLEAD_ID);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                            IDbDataParameter parRESOURCE_NUMBER = Sql.AddParameter(cmd, "@RESOURCE_NUMBER", sRESOURCE_NUMBER, 30);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spRESOURCES_Import
        /// <summary>
        /// spRESOURCES_Import
        /// </summary>
        public static void spRESOURCES_Import(ref Guid gID, Guid gASSIGNED_USER_ID, string sSALUTATION, string sFIRST_NAME, string sLAST_NAME, Guid gPARTNER_ID, string sLEAD_SOURCE, string sTITLE, string sDEPARTMENT, Guid gREPORTS_TO_ID, DateTime dtBIRTHDATE, bool bDO_NOT_CALL, string sPHONE_HOME, string sPHONE_MOBILE, string sPHONE_WORK, string sPHONE_OTHER, string sPHONE_FAX, string sEMAIL1, string sEMAIL2, string sASSISTANT, string sASSISTANT_PHONE, bool bEMAIL_OPT_OUT, bool bINVALID_EMAIL, string sPRIMARY_ADDRESS_STREET, string sPRIMARY_ADDRESS_CITY, string sPRIMARY_ADDRESS_STATE, string sPRIMARY_ADDRESS_POSTALCODE, string sPRIMARY_ADDRESS_COUNTRY, string sALT_ADDRESS_STREET, string sALT_ADDRESS_CITY, string sALT_ADDRESS_STATE, string sALT_ADDRESS_POSTALCODE, string sALT_ADDRESS_COUNTRY, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, bool bSYNC_CONTACT, Guid gTEAM_ID, string sPARTNER_NAME, string sTEAM_SET_LIST, DateTime dtDATE_ENTERED, DateTime dtDATE_MODIFIED, string sPRIMARY_ADDRESS_STREET1, string sPRIMARY_ADDRESS_STREET2, string sPRIMARY_ADDRESS_STREET3, string sALT_ADDRESS_STREET1, string sALT_ADDRESS_STREET2, string sALT_ADDRESS_STREET3, string sSMS_OPT_IN, string sTWITTER_SCREEN_NAME, Guid gLEAD_ID, string sTAG_SET_NAME, string sPICTURE, string sRESOURCE_NUMBER, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_Import";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parSALUTATION = Sql.AddParameter(cmd, "@SALUTATION", sSALUTATION, 25);
                IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parLEAD_SOURCE = Sql.AddParameter(cmd, "@LEAD_SOURCE", sLEAD_SOURCE, 100);
                IDbDataParameter parTITLE = Sql.AddParameter(cmd, "@TITLE", sTITLE, 50);
                IDbDataParameter parDEPARTMENT = Sql.AddParameter(cmd, "@DEPARTMENT", sDEPARTMENT, 100);
                IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                IDbDataParameter parBIRTHDATE = Sql.AddParameter(cmd, "@BIRTHDATE", dtBIRTHDATE);
                IDbDataParameter parDO_NOT_CALL = Sql.AddParameter(cmd, "@DO_NOT_CALL", bDO_NOT_CALL);
                IDbDataParameter parPHONE_HOME = Sql.AddParameter(cmd, "@PHONE_HOME", sPHONE_HOME, 25);
                IDbDataParameter parPHONE_MOBILE = Sql.AddParameter(cmd, "@PHONE_MOBILE", sPHONE_MOBILE, 25);
                IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                IDbDataParameter parPHONE_OTHER = Sql.AddParameter(cmd, "@PHONE_OTHER", sPHONE_OTHER, 25);
                IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                IDbDataParameter parASSISTANT = Sql.AddParameter(cmd, "@ASSISTANT", sASSISTANT, 75);
                IDbDataParameter parASSISTANT_PHONE = Sql.AddParameter(cmd, "@ASSISTANT_PHONE", sASSISTANT_PHONE, 25);
                IDbDataParameter parEMAIL_OPT_OUT = Sql.AddParameter(cmd, "@EMAIL_OPT_OUT", bEMAIL_OPT_OUT);
                IDbDataParameter parINVALID_EMAIL = Sql.AddParameter(cmd, "@INVALID_EMAIL", bINVALID_EMAIL);
                IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET", sPRIMARY_ADDRESS_STREET, 150);
                IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_CITY", sPRIMARY_ADDRESS_CITY, 100);
                IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STATE", sPRIMARY_ADDRESS_STATE, 100);
                IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", sPRIMARY_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", sPRIMARY_ADDRESS_COUNTRY, 100);
                IDbDataParameter parALT_ADDRESS_STREET = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET", sALT_ADDRESS_STREET, 150);
                IDbDataParameter parALT_ADDRESS_CITY = Sql.AddParameter(cmd, "@ALT_ADDRESS_CITY", sALT_ADDRESS_CITY, 100);
                IDbDataParameter parALT_ADDRESS_STATE = Sql.AddParameter(cmd, "@ALT_ADDRESS_STATE", sALT_ADDRESS_STATE, 100);
                IDbDataParameter parALT_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@ALT_ADDRESS_POSTALCODE", sALT_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parALT_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@ALT_ADDRESS_COUNTRY", sALT_ADDRESS_COUNTRY, 100);
                IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                IDbDataParameter parSYNC_CONTACT = Sql.AddParameter(cmd, "@SYNC_CONTACT", bSYNC_CONTACT);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parPARTNER_NAME = Sql.AddParameter(cmd, "@PARTNER_NAME", sPARTNER_NAME, 100);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parDATE_ENTERED = Sql.AddParameter(cmd, "@DATE_ENTERED", dtDATE_ENTERED);
                IDbDataParameter parDATE_MODIFIED = Sql.AddParameter(cmd, "@DATE_MODIFIED", dtDATE_MODIFIED);
                IDbDataParameter parPRIMARY_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET1", sPRIMARY_ADDRESS_STREET1, 150);
                IDbDataParameter parPRIMARY_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET2", sPRIMARY_ADDRESS_STREET2, 150);
                IDbDataParameter parPRIMARY_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET3", sPRIMARY_ADDRESS_STREET3, 150);
                IDbDataParameter parALT_ADDRESS_STREET1 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET1", sALT_ADDRESS_STREET1, 150);
                IDbDataParameter parALT_ADDRESS_STREET2 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET2", sALT_ADDRESS_STREET2, 150);
                IDbDataParameter parALT_ADDRESS_STREET3 = Sql.AddParameter(cmd, "@ALT_ADDRESS_STREET3", sALT_ADDRESS_STREET3, 150);
                IDbDataParameter parSMS_OPT_IN = Sql.AddParameter(cmd, "@SMS_OPT_IN", sSMS_OPT_IN, 25);
                IDbDataParameter parTWITTER_SCREEN_NAME = Sql.AddParameter(cmd, "@TWITTER_SCREEN_NAME", sTWITTER_SCREEN_NAME, 20);
                IDbDataParameter parLEAD_ID = Sql.AddParameter(cmd, "@LEAD_ID", gLEAD_ID);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                IDbDataParameter parRESOURCE_NUMBER = Sql.AddParameter(cmd, "@RESOURCE_NUMBER", sRESOURCE_NUMBER, 30);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdRESOURCES_Import
        /// <summary>
        /// spRESOURCES_Import
        /// </summary>
        public static IDbCommand cmdRESOURCES_Import(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_Import";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parSALUTATION = Sql.CreateParameter(cmd, "@SALUTATION", "string", 25);
            IDbDataParameter parFIRST_NAME = Sql.CreateParameter(cmd, "@FIRST_NAME", "string", 100);
            IDbDataParameter parLAST_NAME = Sql.CreateParameter(cmd, "@LAST_NAME", "string", 100);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parLEAD_SOURCE = Sql.CreateParameter(cmd, "@LEAD_SOURCE", "string", 100);
            IDbDataParameter parTITLE = Sql.CreateParameter(cmd, "@TITLE", "string", 50);
            IDbDataParameter parDEPARTMENT = Sql.CreateParameter(cmd, "@DEPARTMENT", "string", 100);
            IDbDataParameter parREPORTS_TO_ID = Sql.CreateParameter(cmd, "@REPORTS_TO_ID", "Guid", 16);
            IDbDataParameter parBIRTHDATE = Sql.CreateParameter(cmd, "@BIRTHDATE", "DateTime", 8);
            IDbDataParameter parDO_NOT_CALL = Sql.CreateParameter(cmd, "@DO_NOT_CALL", "bool", 1);
            IDbDataParameter parPHONE_HOME = Sql.CreateParameter(cmd, "@PHONE_HOME", "string", 25);
            IDbDataParameter parPHONE_MOBILE = Sql.CreateParameter(cmd, "@PHONE_MOBILE", "string", 25);
            IDbDataParameter parPHONE_WORK = Sql.CreateParameter(cmd, "@PHONE_WORK", "string", 25);
            IDbDataParameter parPHONE_OTHER = Sql.CreateParameter(cmd, "@PHONE_OTHER", "string", 25);
            IDbDataParameter parPHONE_FAX = Sql.CreateParameter(cmd, "@PHONE_FAX", "string", 25);
            IDbDataParameter parEMAIL1 = Sql.CreateParameter(cmd, "@EMAIL1", "string", 100);
            IDbDataParameter parEMAIL2 = Sql.CreateParameter(cmd, "@EMAIL2", "string", 100);
            IDbDataParameter parASSISTANT = Sql.CreateParameter(cmd, "@ASSISTANT", "string", 75);
            IDbDataParameter parASSISTANT_PHONE = Sql.CreateParameter(cmd, "@ASSISTANT_PHONE", "string", 25);
            IDbDataParameter parEMAIL_OPT_OUT = Sql.CreateParameter(cmd, "@EMAIL_OPT_OUT", "bool", 1);
            IDbDataParameter parINVALID_EMAIL = Sql.CreateParameter(cmd, "@INVALID_EMAIL", "bool", 1);
            IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STREET", "string", 150);
            IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_CITY", "string", 100);
            IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STATE", "string", 100);
            IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parALT_ADDRESS_STREET = Sql.CreateParameter(cmd, "@ALT_ADDRESS_STREET", "string", 150);
            IDbDataParameter parALT_ADDRESS_CITY = Sql.CreateParameter(cmd, "@ALT_ADDRESS_CITY", "string", 100);
            IDbDataParameter parALT_ADDRESS_STATE = Sql.CreateParameter(cmd, "@ALT_ADDRESS_STATE", "string", 100);
            IDbDataParameter parALT_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@ALT_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parALT_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@ALT_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parDESCRIPTION = Sql.CreateParameter(cmd, "@DESCRIPTION", "string", 104857600);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            IDbDataParameter parSYNC_CONTACT = Sql.CreateParameter(cmd, "@SYNC_CONTACT", "bool", 1);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parPARTNER_NAME = Sql.CreateParameter(cmd, "@PARTNER_NAME", "string", 100);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parDATE_ENTERED = Sql.CreateParameter(cmd, "@DATE_ENTERED", "DateTime", 8);
            IDbDataParameter parDATE_MODIFIED = Sql.CreateParameter(cmd, "@DATE_MODIFIED", "DateTime", 8);
            IDbDataParameter parPRIMARY_ADDRESS_STREET1 = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STREET1", "string", 150);
            IDbDataParameter parPRIMARY_ADDRESS_STREET2 = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STREET2", "string", 150);
            IDbDataParameter parPRIMARY_ADDRESS_STREET3 = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STREET3", "string", 150);
            IDbDataParameter parALT_ADDRESS_STREET1 = Sql.CreateParameter(cmd, "@ALT_ADDRESS_STREET1", "string", 150);
            IDbDataParameter parALT_ADDRESS_STREET2 = Sql.CreateParameter(cmd, "@ALT_ADDRESS_STREET2", "string", 150);
            IDbDataParameter parALT_ADDRESS_STREET3 = Sql.CreateParameter(cmd, "@ALT_ADDRESS_STREET3", "string", 150);
            IDbDataParameter parSMS_OPT_IN = Sql.CreateParameter(cmd, "@SMS_OPT_IN", "string", 25);
            IDbDataParameter parTWITTER_SCREEN_NAME = Sql.CreateParameter(cmd, "@TWITTER_SCREEN_NAME", "string", 20);
            IDbDataParameter parLEAD_ID = Sql.CreateParameter(cmd, "@LEAD_ID", "Guid", 16);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parPICTURE = Sql.CreateParameter(cmd, "@PICTURE", "string", 104857600);
            IDbDataParameter parRESOURCE_NUMBER = Sql.CreateParameter(cmd, "@RESOURCE_NUMBER", "string", 30);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spRESOURCES_InsRelated
        /// <summary>
        /// spRESOURCES_InsRelated
        /// </summary>
        public static void spRESOURCES_InsRelated(Guid gRESOURCE_ID, string sPARENT_TYPE, Guid gPARENT_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_InsRelated";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_InsRelated
        /// <summary>
        /// spRESOURCES_InsRelated
        /// </summary>
        public static void spRESOURCES_InsRelated(Guid gRESOURCE_ID, string sPARENT_TYPE, Guid gPARENT_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_InsRelated";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_InsRelated
        /// <summary>
        /// spRESOURCES_InsRelated
        /// </summary>
        public static IDbCommand cmdRESOURCES_InsRelated(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_InsRelated";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_MassDelete
        /// <summary>
        /// spRESOURCES_MassDelete
        /// </summary>
        public static void spRESOURCES_MassDelete(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_MassDelete";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_MassDelete
        /// <summary>
        /// spRESOURCES_MassDelete
        /// </summary>
        public static void spRESOURCES_MassDelete(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_MassDelete";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_MassDelete
        /// <summary>
        /// spRESOURCES_MassDelete
        /// </summary>
        public static IDbCommand cmdRESOURCES_MassDelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_MassDelete";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_MassSync
        /// <summary>
        /// spRESOURCES_MassSync
        /// </summary>
        public static void spRESOURCES_MassSync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_MassSync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_MassSync
        /// <summary>
        /// spRESOURCES_MassSync
        /// </summary>
        public static void spRESOURCES_MassSync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_MassSync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_MassSync
        /// <summary>
        /// spRESOURCES_MassSync
        /// </summary>
        public static IDbCommand cmdRESOURCES_MassSync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_MassSync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_MassUnsync
        /// <summary>
        /// spRESOURCES_MassUnsync
        /// </summary>
        public static void spRESOURCES_MassUnsync(string sID_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_MassUnsync";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_MassUnsync
        /// <summary>
        /// spRESOURCES_MassUnsync
        /// </summary>
        public static void spRESOURCES_MassUnsync(string sID_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_MassUnsync";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_MassUnsync
        /// <summary>
        /// spRESOURCES_MassUnsync
        /// </summary>
        public static IDbCommand cmdRESOURCES_MassUnsync(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_MassUnsync";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_MassUpdate
        /// <summary>
        /// spRESOURCES_MassUpdate
        /// </summary>
        public static void spRESOURCES_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, Guid gREPORTS_TO_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_MassUpdate";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_MassUpdate
        /// <summary>
        /// spRESOURCES_MassUpdate
        /// </summary>
        public static void spRESOURCES_MassUpdate(string sID_LIST, Guid gASSIGNED_USER_ID, Guid gPARTNER_ID, Guid gREPORTS_TO_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, bool bTEAM_SET_ADD, string sTAG_SET_NAME, bool bTAG_SET_ADD, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_MassUpdate";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parTEAM_SET_ADD = Sql.AddParameter(cmd, "@TEAM_SET_ADD", bTEAM_SET_ADD);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parTAG_SET_ADD = Sql.AddParameter(cmd, "@TAG_SET_ADD", bTAG_SET_ADD);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_MassUpdate
        /// <summary>
        /// spRESOURCES_MassUpdate
        /// </summary>
        public static IDbCommand cmdRESOURCES_MassUpdate(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_MassUpdate";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parREPORTS_TO_ID = Sql.CreateParameter(cmd, "@REPORTS_TO_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parTEAM_SET_ADD = Sql.CreateParameter(cmd, "@TEAM_SET_ADD", "bool", 1);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parTAG_SET_ADD = Sql.CreateParameter(cmd, "@TAG_SET_ADD", "bool", 1);
            return cmd;
        }
        #endregion

        #region spRESOURCES_Merge
        /// <summary>
        /// spRESOURCES_Merge
        /// </summary>
        public static void spRESOURCES_Merge(Guid gID, Guid gMERGE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_Merge";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_Merge
        /// <summary>
        /// spRESOURCES_Merge
        /// </summary>
        public static void spRESOURCES_Merge(Guid gID, Guid gMERGE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_Merge";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parMERGE_ID = Sql.AddParameter(cmd, "@MERGE_ID", gMERGE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_Merge
        /// <summary>
        /// spRESOURCES_Merge
        /// </summary>
        public static IDbCommand cmdRESOURCES_Merge(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_Merge";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parMERGE_ID = Sql.CreateParameter(cmd, "@MERGE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_New
        /// <summary>
        /// spRESOURCES_New
        /// </summary>
        public static void spRESOURCES_New(ref Guid gID, string sFIRST_NAME, string sLAST_NAME, string sPHONE_WORK, string sEMAIL1, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sASSIGNED_SET_LIST)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_New";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                            IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                            IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                            IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spRESOURCES_New
        /// <summary>
        /// spRESOURCES_New
        /// </summary>
        public static void spRESOURCES_New(ref Guid gID, string sFIRST_NAME, string sLAST_NAME, string sPHONE_WORK, string sEMAIL1, Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sTEAM_SET_LIST, string sASSIGNED_SET_LIST, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_New";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);
                parID.Direction = ParameterDirection.InputOutput;
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdRESOURCES_New
        /// <summary>
        /// spRESOURCES_New
        /// </summary>
        public static IDbCommand cmdRESOURCES_New(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_New";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parFIRST_NAME = Sql.CreateParameter(cmd, "@FIRST_NAME", "string", 100);
            IDbDataParameter parLAST_NAME = Sql.CreateParameter(cmd, "@LAST_NAME", "string", 100);
            IDbDataParameter parPHONE_WORK = Sql.CreateParameter(cmd, "@PHONE_WORK", "string", 25);
            IDbDataParameter parEMAIL1 = Sql.CreateParameter(cmd, "@EMAIL1", "string", 100);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parTEAM_SET_LIST = Sql.CreateParameter(cmd, "@TEAM_SET_LIST", "ansistring", 8000);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);
            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion


        #region spRESOURCES_STREAM_InsertPost
        /// <summary>
        /// spRESOURCES_STREAM_InsertPost
        /// </summary>
        public static void spRESOURCES_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_STREAM_InsertPost";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                            IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                            IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                            IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_STREAM_InsertPost
        /// <summary>
        /// spRESOURCES_STREAM_InsertPost
        /// </summary>
        public static void spRESOURCES_STREAM_InsertPost(Guid gASSIGNED_USER_ID, Guid gTEAM_ID, string sNAME, Guid gRELATED_ID, string sRELATED_MODULE, string sRELATED_NAME, Guid gID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_STREAM_InsertPost";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                IDbDataParameter parNAME = Sql.AddParameter(cmd, "@NAME", sNAME);
                IDbDataParameter parRELATED_ID = Sql.AddParameter(cmd, "@RELATED_ID", gRELATED_ID);
                IDbDataParameter parRELATED_MODULE = Sql.AddParameter(cmd, "@RELATED_MODULE", sRELATED_MODULE, 25);
                IDbDataParameter parRELATED_NAME = Sql.AddParameter(cmd, "@RELATED_NAME", sRELATED_NAME, 255);
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_STREAM_InsertPost
        /// <summary>
        /// spRESOURCES_STREAM_InsertPost
        /// </summary>
        public static IDbCommand cmdRESOURCES_STREAM_InsertPost(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_STREAM_InsertPost";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parTEAM_ID = Sql.CreateParameter(cmd, "@TEAM_ID", "Guid", 16);
            IDbDataParameter parNAME = Sql.CreateParameter(cmd, "@NAME", "string", 104857600);
            IDbDataParameter parRELATED_ID = Sql.CreateParameter(cmd, "@RELATED_ID", "Guid", 16);
            IDbDataParameter parRELATED_MODULE = Sql.CreateParameter(cmd, "@RELATED_MODULE", "string", 25);
            IDbDataParameter parRELATED_NAME = Sql.CreateParameter(cmd, "@RELATED_NAME", "string", 255);
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_Undelete
        /// <summary>
        /// spRESOURCES_Undelete
        /// </summary>
        public static void spRESOURCES_Undelete(Guid gID, string sAUDIT_TOKEN)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_Undelete";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_Undelete
        /// <summary>
        /// spRESOURCES_Undelete
        /// </summary>
        public static void spRESOURCES_Undelete(Guid gID, string sAUDIT_TOKEN, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_Undelete";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parAUDIT_TOKEN = Sql.AddAnsiParam(cmd, "@AUDIT_TOKEN", sAUDIT_TOKEN, 255);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_Undelete
        /// <summary>
        /// spRESOURCES_Undelete
        /// </summary>
        public static IDbCommand cmdRESOURCES_Undelete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_Undelete";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parAUDIT_TOKEN = Sql.CreateParameter(cmd, "@AUDIT_TOKEN", "ansistring", 255);
            return cmd;
        }
        #endregion

        #region spRESOURCES_Update
        /// <summary>
        /// spRESOURCES_Update
        /// </summary>
        public static void spRESOURCES_Update(ref Guid gID, Guid gASSIGNED_USER_ID, string sSALUTATION, string sFIRST_NAME, string sLAST_NAME, Guid gPARTNER_ID, string sTITLE, string sDEPARTMENT, Guid gREPORTS_TO_ID, DateTime dtBIRTHDATE, bool bDO_NOT_CALL, string sPHONE_HOME, string sPHONE_MOBILE, string sPHONE_WORK, string sPHONE_OTHER, string sPHONE_FAX, string sEMAIL1, string sEMAIL2, string sPRIMARY_ADDRESS_STREET, string sPRIMARY_ADDRESS_CITY, string sPRIMARY_ADDRESS_STATE, string sPRIMARY_ADDRESS_POSTALCODE, string sPRIMARY_ADDRESS_COUNTRY, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, bool bSYNC_CONTACT, Guid gTEAM_ID, string sTEAM_SET_LIST, string sPICTURE, bool bEXCHANGE_FOLDER, string sTAG_SET_NAME, string sRESOURCE_NUMBER, string sASSIGNED_SET_LIST, string sSKYPE, string sCVTOOL_LINK, bool bENGAGED, DateTime dtENGAGED_TILL, string sENGAGED_JIRAPROJECT, string sSCNSOFT_ACCOUNT)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_Update";
                            IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                            IDbDataParameter parSALUTATION = Sql.AddParameter(cmd, "@SALUTATION", sSALUTATION, 25);
                            IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                            IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parTITLE = Sql.AddParameter(cmd, "@TITLE", sTITLE, 50);
                            IDbDataParameter parDEPARTMENT = Sql.AddParameter(cmd, "@DEPARTMENT", sDEPARTMENT, 100);
                            IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                            IDbDataParameter parBIRTHDATE = Sql.AddParameter(cmd, "@BIRTHDATE", dtBIRTHDATE);
                            IDbDataParameter parDO_NOT_CALL = Sql.AddParameter(cmd, "@DO_NOT_CALL", bDO_NOT_CALL);
                            IDbDataParameter parPHONE_HOME = Sql.AddParameter(cmd, "@PHONE_HOME", sPHONE_HOME, 25);
                            IDbDataParameter parPHONE_MOBILE = Sql.AddParameter(cmd, "@PHONE_MOBILE", sPHONE_MOBILE, 25);
                            IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                            IDbDataParameter parPHONE_OTHER = Sql.AddParameter(cmd, "@PHONE_OTHER", sPHONE_OTHER, 25);
                            IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                            IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                            IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                            IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET", sPRIMARY_ADDRESS_STREET, 150);
                            IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_CITY", sPRIMARY_ADDRESS_CITY, 100);
                            IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STATE", sPRIMARY_ADDRESS_STATE, 100);
                            IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", sPRIMARY_ADDRESS_POSTALCODE, 20);
                            IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", sPRIMARY_ADDRESS_COUNTRY, 100);
                            IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                            IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                            IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                            IDbDataParameter parSYNC_CONTACT = Sql.AddParameter(cmd, "@SYNC_CONTACT", bSYNC_CONTACT);
                            IDbDataParameter parTEAM_ID = Sql.AddParameter(cmd, "@TEAM_ID", gTEAM_ID);
                            IDbDataParameter parTEAM_SET_LIST = Sql.AddAnsiParam(cmd, "@TEAM_SET_LIST", sTEAM_SET_LIST, 8000);
                            IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                            IDbDataParameter parEXCHANGE_FOLDER = Sql.AddParameter(cmd, "@EXCHANGE_FOLDER", bEXCHANGE_FOLDER);
                            IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                            IDbDataParameter parRESOURCE_NUMBER = Sql.AddParameter(cmd, "@RESOURCE_NUMBER", sRESOURCE_NUMBER, 30);
                            IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);

                            IDbDataParameter parSKYPE = Sql.AddParameter(cmd, "@SKYPE", sSKYPE, 20);
                            IDbDataParameter parCVTOOL_LINK = Sql.AddParameter(cmd, "@CVTOOL_LINK", sCVTOOL_LINK, 255);
                            IDbDataParameter parENGAGED = Sql.AddParameter(cmd, "@ENGAGED", bENGAGED);
                            IDbDataParameter parENGAGED_TILL = Sql.AddParameter(cmd, "@ENGAGED_TILL", dtENGAGED_TILL);
                            IDbDataParameter parENGAGED_JIRAPROJECT = Sql.AddParameter(cmd, "@ENGAGED_JIRAPROJECT", sENGAGED_JIRAPROJECT, 32);
                            IDbDataParameter parSCNSOFT_ACCOUNT = Sql.AddParameter(cmd, "@SCNSOFT_ACCOUNT", sSCNSOFT_ACCOUNT, 32);

                            parID.Direction = ParameterDirection.InputOutput;
                            cmd.ExecuteNonQuery();
                            gID = Sql.ToGuid(parID.Value);
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
        #endregion

        #region spRESOURCES_Update
        /// <summary>
        /// spRESOURCES_Update
        /// </summary>
        public static void spRESOURCES_Update(ref Guid gID, Guid gASSIGNED_USER_ID, string sSALUTATION, string sFIRST_NAME, string sLAST_NAME, Guid gPARTNER_ID, string sTITLE, string sDEPARTMENT, Guid gREPORTS_TO_ID, DateTime dtBIRTHDATE, bool bDO_NOT_CALL, string sPHONE_HOME, string sPHONE_MOBILE, string sPHONE_WORK, string sPHONE_OTHER, string sPHONE_FAX, string sEMAIL1, string sEMAIL2, string sPRIMARY_ADDRESS_STREET, string sPRIMARY_ADDRESS_CITY, string sPRIMARY_ADDRESS_STATE, string sPRIMARY_ADDRESS_POSTALCODE, string sPRIMARY_ADDRESS_COUNTRY, string sDESCRIPTION, string sPARENT_TYPE, Guid gPARENT_ID, bool bSYNC_CONTACT, Guid gTEAM_ID, string sTEAM_SET_LIST, string sPICTURE, bool bEXCHANGE_FOLDER, string sTAG_SET_NAME, string sRESOURCE_NUMBER, string sASSIGNED_SET_LIST, string sSKYPE, string sCVTOOL_LINK, bool bENGAGED, DateTime dtENGAGED_TILL, string sENGAGED_JIRAPROJECT, string sSCNSOFT_ACCOUNT, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_Update";
                IDbDataParameter parID = Sql.AddParameter(cmd, "@ID", gID);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parASSIGNED_USER_ID = Sql.AddParameter(cmd, "@ASSIGNED_USER_ID", gASSIGNED_USER_ID);
                IDbDataParameter parSALUTATION = Sql.AddParameter(cmd, "@SALUTATION", sSALUTATION, 25);
                IDbDataParameter parFIRST_NAME = Sql.AddParameter(cmd, "@FIRST_NAME", sFIRST_NAME, 100);
                IDbDataParameter parLAST_NAME = Sql.AddParameter(cmd, "@LAST_NAME", sLAST_NAME, 100);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parTITLE = Sql.AddParameter(cmd, "@TITLE", sTITLE, 50);
                IDbDataParameter parDEPARTMENT = Sql.AddParameter(cmd, "@DEPARTMENT", sDEPARTMENT, 100);
                IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", gREPORTS_TO_ID);
                IDbDataParameter parBIRTHDATE = Sql.AddParameter(cmd, "@BIRTHDATE", dtBIRTHDATE);
                IDbDataParameter parDO_NOT_CALL = Sql.AddParameter(cmd, "@DO_NOT_CALL", bDO_NOT_CALL);
                IDbDataParameter parPHONE_HOME = Sql.AddParameter(cmd, "@PHONE_HOME", sPHONE_HOME, 25);
                IDbDataParameter parPHONE_MOBILE = Sql.AddParameter(cmd, "@PHONE_MOBILE", sPHONE_MOBILE, 25);
                IDbDataParameter parPHONE_WORK = Sql.AddParameter(cmd, "@PHONE_WORK", sPHONE_WORK, 25);
                IDbDataParameter parPHONE_OTHER = Sql.AddParameter(cmd, "@PHONE_OTHER", sPHONE_OTHER, 25);
                IDbDataParameter parPHONE_FAX = Sql.AddParameter(cmd, "@PHONE_FAX", sPHONE_FAX, 25);
                IDbDataParameter parEMAIL1 = Sql.AddParameter(cmd, "@EMAIL1", sEMAIL1, 100);
                IDbDataParameter parEMAIL2 = Sql.AddParameter(cmd, "@EMAIL2", sEMAIL2, 100);
                IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STREET", sPRIMARY_ADDRESS_STREET, 150);
                IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_CITY", sPRIMARY_ADDRESS_CITY, 100);
                IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_STATE", sPRIMARY_ADDRESS_STATE, 100);
                IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", sPRIMARY_ADDRESS_POSTALCODE, 20);
                IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.AddParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", sPRIMARY_ADDRESS_COUNTRY, 100);
                IDbDataParameter parDESCRIPTION = Sql.AddParameter(cmd, "@DESCRIPTION", sDESCRIPTION);
                IDbDataParameter parPARENT_TYPE = Sql.AddParameter(cmd, "@PARENT_TYPE", sPARENT_TYPE, 25);
                IDbDataParameter parPARENT_ID = Sql.AddParameter(cmd, "@PARENT_ID", gPARENT_ID);
                IDbDataParameter parSYNC_CONTACT = Sql.AddParameter(cmd, "@SYNC_CONTACT", bSYNC_CONTACT);
                IDbDataParameter parPICTURE = Sql.AddParameter(cmd, "@PICTURE", sPICTURE);
                IDbDataParameter parTAG_SET_NAME = Sql.AddParameter(cmd, "@TAG_SET_NAME", sTAG_SET_NAME, 4000);
                IDbDataParameter parRESOURCE_NUMBER = Sql.AddParameter(cmd, "@RESOURCE_NUMBER", sRESOURCE_NUMBER, 30);
                IDbDataParameter parASSIGNED_SET_LIST = Sql.AddAnsiParam(cmd, "@ASSIGNED_SET_LIST", sASSIGNED_SET_LIST, 8000);

                IDbDataParameter parSKYPE = Sql.AddParameter(cmd, "@SKYPE", sSKYPE, 20);
                IDbDataParameter parCVTOOL_LINK = Sql.AddParameter(cmd, "@CVTOOL_LINK", sCVTOOL_LINK, 255);
                IDbDataParameter parENGAGED = Sql.AddParameter(cmd, "@ENGAGED", bENGAGED);
                IDbDataParameter parENGAGED_TILL = Sql.AddParameter(cmd, "@ENGAGED_TILL", dtENGAGED_TILL);
                IDbDataParameter parENGAGED_JIRAPROJECT = Sql.AddParameter(cmd, "@ENGAGED_JIRAPROJECT", sENGAGED_JIRAPROJECT, 32);
                IDbDataParameter parSCNSOFT_ACCOUNT = Sql.AddParameter(cmd, "@SCNSOFT_ACCOUNT", sSCNSOFT_ACCOUNT, 32);

                parID.Direction = ParameterDirection.InputOutput;

                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
                gID = Sql.ToGuid(parID.Value);
            }
        }
        #endregion

        #region cmdRESOURCES_Update
        /// <summary>
        /// spRESOURCES_Update
        /// </summary>
        public static IDbCommand cmdRESOURCES_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_Update";
            IDbDataParameter parID = Sql.CreateParameter(cmd, "@ID", "Guid", 16);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parASSIGNED_USER_ID = Sql.CreateParameter(cmd, "@ASSIGNED_USER_ID", "Guid", 16);
            IDbDataParameter parSALUTATION = Sql.CreateParameter(cmd, "@SALUTATION", "string", 25);
            IDbDataParameter parFIRST_NAME = Sql.CreateParameter(cmd, "@FIRST_NAME", "string", 100);
            IDbDataParameter parLAST_NAME = Sql.CreateParameter(cmd, "@LAST_NAME", "string", 100);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parTITLE = Sql.CreateParameter(cmd, "@TITLE", "string", 50);
            IDbDataParameter parDEPARTMENT = Sql.CreateParameter(cmd, "@DEPARTMENT", "string", 100);
            IDbDataParameter parREPORTS_TO_ID = Sql.AddParameter(cmd, "@REPORTS_TO_ID", "Guid", 16);
            IDbDataParameter parBIRTHDATE = Sql.CreateParameter(cmd, "@BIRTHDATE", "DateTime", 8);
            IDbDataParameter parDO_NOT_CALL = Sql.CreateParameter(cmd, "@DO_NOT_CALL", "bool", 1);
            IDbDataParameter parPHONE_HOME = Sql.CreateParameter(cmd, "@PHONE_HOME", "string", 25);
            IDbDataParameter parPHONE_MOBILE = Sql.CreateParameter(cmd, "@PHONE_MOBILE", "string", 25);
            IDbDataParameter parPHONE_WORK = Sql.CreateParameter(cmd, "@PHONE_WORK", "string", 25);
            IDbDataParameter parPHONE_OTHER = Sql.CreateParameter(cmd, "@PHONE_OTHER", "string", 25);
            IDbDataParameter parPHONE_FAX = Sql.CreateParameter(cmd, "@PHONE_FAX", "string", 25);
            IDbDataParameter parEMAIL1 = Sql.CreateParameter(cmd, "@EMAIL1", "string", 100);
            IDbDataParameter parEMAIL2 = Sql.CreateParameter(cmd, "@EMAIL2", "string", 100);
            IDbDataParameter parPRIMARY_ADDRESS_STREET = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STREET", "string", 150);
            IDbDataParameter parPRIMARY_ADDRESS_CITY = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_CITY", "string", 100);
            IDbDataParameter parPRIMARY_ADDRESS_STATE = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_STATE", "string", 100);
            IDbDataParameter parPRIMARY_ADDRESS_POSTALCODE = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_POSTALCODE", "string", 20);
            IDbDataParameter parPRIMARY_ADDRESS_COUNTRY = Sql.CreateParameter(cmd, "@PRIMARY_ADDRESS_COUNTRY", "string", 100);
            IDbDataParameter parDESCRIPTION = Sql.CreateParameter(cmd, "@DESCRIPTION", "string", 104857600);
            IDbDataParameter parPARENT_TYPE = Sql.CreateParameter(cmd, "@PARENT_TYPE", "string", 25);
            IDbDataParameter parPARENT_ID = Sql.CreateParameter(cmd, "@PARENT_ID", "Guid", 16);
            IDbDataParameter parSYNC_CONTACT = Sql.CreateParameter(cmd, "@SYNC_CONTACT", "bool", 1);
            IDbDataParameter parPICTURE = Sql.CreateParameter(cmd, "@PICTURE", "string", 104857600);
            IDbDataParameter parTAG_SET_NAME = Sql.CreateParameter(cmd, "@TAG_SET_NAME", "string", 4000);
            IDbDataParameter parRESOURCE_NUMBER = Sql.CreateParameter(cmd, "@RESOURCE_NUMBER", "string", 30);
            IDbDataParameter parASSIGNED_SET_LIST = Sql.CreateParameter(cmd, "@ASSIGNED_SET_LIST", "ansistring", 8000);

            IDbDataParameter parSKYPE = Sql.CreateParameter(cmd, "@SKYPE", "string", 20);
            IDbDataParameter parCVTOOL_LINK = Sql.CreateParameter(cmd, "@CVTOOL_LINK", "string", 255);
            IDbDataParameter parENGAGED = Sql.CreateParameter(cmd, "@ENGAGED", "bool", 1);
            IDbDataParameter parENGAGED_TILL = Sql.CreateParameter(cmd, "@ENGAGED_TILL", "DateTime", 8);
            IDbDataParameter parENGAGED_JIRAPROJECT = Sql.CreateParameter(cmd, "@ENGAGED_JIRAPROJECT", "string", 32);
            IDbDataParameter parSCNSOFT_ACCOUNT = Sql.CreateParameter(cmd, "@SCNSOFT_ACCOUNT", "string", 32);

            parID.Direction = ParameterDirection.InputOutput;
            return cmd;
        }
        #endregion

        #region spRESOURCES_USERS_Delete
        /// <summary>
        /// spRESOURCES_USERS_Delete
        /// </summary>
        public static void spRESOURCES_USERS_Delete(Guid gRESOURCE_ID, Guid gUSER_ID, string sSERVICE_NAME)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_USERS_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            IDbDataParameter parSERVICE_NAME = Sql.AddParameter(cmd, "@SERVICE_NAME", sSERVICE_NAME, 25);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_USERS_Delete
        /// <summary>
        /// spRESOURCES_USERS_Delete
        /// </summary>
        public static void spRESOURCES_USERS_Delete(Guid gRESOURCE_ID, Guid gUSER_ID, string sSERVICE_NAME, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_USERS_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                IDbDataParameter parSERVICE_NAME = Sql.AddParameter(cmd, "@SERVICE_NAME", sSERVICE_NAME, 25);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_USERS_Delete
        /// <summary>
        /// spRESOURCES_USERS_Delete
        /// </summary>
        public static IDbCommand cmdRESOURCES_USERS_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_USERS_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            IDbDataParameter parSERVICE_NAME = Sql.CreateParameter(cmd, "@SERVICE_NAME", "string", 25);
            return cmd;
        }
        #endregion

        #region spRESOURCES_USERS_Update
        /// <summary>
        /// spRESOURCES_USERS_Update
        /// </summary>
        public static void spRESOURCES_USERS_Update(Guid gRESOURCE_ID, Guid gUSER_ID, string sSERVICE_NAME)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_USERS_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                            IDbDataParameter parSERVICE_NAME = Sql.AddParameter(cmd, "@SERVICE_NAME", sSERVICE_NAME, 25);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_USERS_Update
        /// <summary>
        /// spRESOURCES_USERS_Update
        /// </summary>
        public static void spRESOURCES_USERS_Update(Guid gRESOURCE_ID, Guid gUSER_ID, string sSERVICE_NAME, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_USERS_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@USER_ID", gUSER_ID);
                IDbDataParameter parSERVICE_NAME = Sql.AddParameter(cmd, "@SERVICE_NAME", sSERVICE_NAME, 25);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_USERS_Update
        /// <summary>
        /// spRESOURCES_USERS_Update
        /// </summary>
        public static IDbCommand cmdRESOURCES_USERS_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_USERS_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@USER_ID", "Guid", 16);
            IDbDataParameter parSERVICE_NAME = Sql.CreateParameter(cmd, "@SERVICE_NAME", "string", 25);
            return cmd;
        }
        #endregion

        #region spRESOURCES_NOTES_Delete
        /// <summary>
        /// spRESOURCES_NOTES_Delete
        /// </summary>
        public static void spRESOURCES_NOTES_Delete(Guid gRESOURCE_ID, Guid gNOTE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_NOTES_Delete";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@NOTE_ID", gNOTE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_NOTES_Delete
        /// <summary>
        /// spRESOURCES_NOTES_Delete
        /// </summary>
        public static void spRESOURCES_NOTES_Delete(Guid gRESOURCE_ID, Guid gNOTE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_NOTES_Delete";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@NOTE_ID", gNOTE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_NOTES_Delete
        /// <summary>
        /// cmdRESOURCES_NOTES_Delete
        /// </summary>
        public static IDbCommand cmdRESOURCES_NOTES_Delete(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "cmdRESOURCES_NOTES_Delete";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@NOTE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spRESOURCES_NOTES_Update
        /// <summary>
        /// spRESOURCES_USERS_Update
        /// </summary>
        public static void spRESOURCES_NOTES_Update(Guid gRESOURCE_ID, Guid gNOTE_ID)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spRESOURCES_NOTES_Update";
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                            IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@NOTE_ID", gNOTE_ID);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spRESOURCES_NOTES_Update
        /// <summary>
        /// spRESOURCES_NOTES_Update
        /// </summary>
        public static void spRESOURCES_NOTES_Update(Guid gRESOURCE_ID, Guid gNOTE_ID, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spRESOURCES_NOTES_Update";
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parRESOURCE_ID = Sql.AddParameter(cmd, "@RESOURCE_ID", gRESOURCE_ID);
                IDbDataParameter parUSER_ID = Sql.AddParameter(cmd, "@NOTE_ID", gNOTE_ID);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdRESOURCES_NOTES_Update
        /// <summary>
        /// spRESOURCES_NOTES_Update
        /// </summary>
        public static IDbCommand cmdRESOURCES_NOTES_Update(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spRESOURCES_NOTES_Update";
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parRESOURCE_ID = Sql.CreateParameter(cmd, "@RESOURCE_ID", "Guid", 16);
            IDbDataParameter parUSER_ID = Sql.CreateParameter(cmd, "@NOTE_ID", "Guid", 16);
            return cmd;
        }
        #endregion

        #region spPARTNERS_RESOURCES_CopyAddress
        /// <summary>
        /// spPARTNERS_RESOURCES_CopyAddress
        /// </summary>
        public static void spPARTNERS_RESOURCES_CopyAddress(string sID_LIST, Guid gPARTNER_ID, string sADDRESS_TYPE)
        {
            DbProviderFactory dbf = DbProviderFactories.GetFactory();
            using (IDbConnection con = dbf.CreateConnection())
            {
                con.Open();
                using (IDbTransaction trn = Sql.BeginTransaction(con))
                {
                    try
                    {
                        using (IDbCommand cmd = con.CreateCommand())
                        {
                            cmd.Transaction = trn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (Sql.IsOracle(cmd))
                                cmd.CommandText = "spACCOUNTS_CONTACTS_CopyAddres";
                            else
                                cmd.CommandText = "spPARTNERS_RESOURCES_CopyAddress";
                            IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                            IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                            IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                            IDbDataParameter parADDRESS_TYPE = Sql.AddParameter(cmd, "@ADDRESS_TYPE", sADDRESS_TYPE, 25);
                            cmd.ExecuteNonQuery();
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
        #endregion

        #region spPARTNERS_RESOURCES_CopyAddress
        /// <summary>
        /// spPARTNERS_RESOURCES_CopyAddress
        /// </summary>
        public static void spPARTNERS_RESOURCES_CopyAddress(string sID_LIST, Guid gPARTNER_ID, string sADDRESS_TYPE, IDbTransaction trn)
        {
            IDbConnection con = trn.Connection;
            using (IDbCommand cmd = con.CreateCommand())
            {
                cmd.Transaction = trn;
                cmd.CommandType = CommandType.StoredProcedure;
                if (Sql.IsOracle(cmd))
                    cmd.CommandText = "spACCOUNTS_CONTACTS_CopyAddres";
                else
                    cmd.CommandText = "spPARTNERS_RESOURCES_CopyAddress";
                IDbDataParameter parID_LIST = Sql.AddAnsiParam(cmd, "@ID_LIST", sID_LIST, 8000);
                IDbDataParameter parMODIFIED_USER_ID = Sql.AddParameter(cmd, "@MODIFIED_USER_ID", Security.USER_ID);
                IDbDataParameter parPARTNER_ID = Sql.AddParameter(cmd, "@PARTNER_ID", gPARTNER_ID);
                IDbDataParameter parADDRESS_TYPE = Sql.AddParameter(cmd, "@ADDRESS_TYPE", sADDRESS_TYPE, 25);
                Sql.Trace(cmd);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region cmdPARTNERS_RESOURCES_CopyAddress
        /// <summary>
        /// spPARTNERS_RESOURCES_CopyAddress
        /// </summary>
        public static IDbCommand cmdPARTNERS_RESOURCES_CopyAddress(IDbConnection con)
        {
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (Sql.IsOracle(cmd))
                cmd.CommandText = "spACCOUNTS_CONTACTS_CopyAddres";
            else
                cmd.CommandText = "spPARTNERS_RESOURCES_CopyAddress";
            IDbDataParameter parID_LIST = Sql.CreateParameter(cmd, "@ID_LIST", "ansistring", 8000);
            IDbDataParameter parMODIFIED_USER_ID = Sql.CreateParameter(cmd, "@MODIFIED_USER_ID", "Guid", 16);
            IDbDataParameter parPARTNER_ID = Sql.CreateParameter(cmd, "@PARTNER_ID", "Guid", 16);
            IDbDataParameter parADDRESS_TYPE = Sql.CreateParameter(cmd, "@ADDRESS_TYPE", "string", 25);
            return cmd;
        }
        #endregion


    }
}

