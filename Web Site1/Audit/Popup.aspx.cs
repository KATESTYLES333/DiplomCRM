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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Audit
{
	/// <summary>
	/// Summary description for Popup.
	/// </summary>
	public class Popup : SplendidPopup
	{
		protected Guid          gID            ;
		protected string        sModule        ;
		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblTitle       ;
		protected Label         lblError       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private DataTable BuildChangesTable(DataTable dtAudit)
		{
			DataTable dtChanges = new DataTable();
			DataColumn colFIELD_NAME   = new DataColumn("FIELD_NAME"  , typeof(System.String  ));
			DataColumn colBEFORE_VALUE = new DataColumn("BEFORE_VALUE", typeof(System.String  ));
			DataColumn colAFTER_VALUE  = new DataColumn("AFTER_VALUE" , typeof(System.String  ));
			DataColumn colCREATED_BY   = new DataColumn("CREATED_BY"  , typeof(System.String  ));
			DataColumn colDATE_CREATED = new DataColumn("DATE_CREATED", typeof(System.DateTime));
			dtChanges.Columns.Add(colFIELD_NAME  );
			dtChanges.Columns.Add(colBEFORE_VALUE);
			dtChanges.Columns.Add(colAFTER_VALUE );
			dtChanges.Columns.Add(colCREATED_BY  );
			dtChanges.Columns.Add(colDATE_CREATED);
			if ( dtAudit.Rows.Count > 0 )
			{
				StringDictionary dict = new StringDictionary();
				dict.Add("AUDIT_ACTION"      , String.Empty);
				dict.Add("AUDIT_DATE"        , String.Empty);
				dict.Add("AUDIT_COLUMNS"     , String.Empty);
				dict.Add("CSTM_AUDIT_COLUMNS", String.Empty);
				dict.Add("ID"                , String.Empty);
				dict.Add("ID_C"              , String.Empty);
				dict.Add("DELETED"           , String.Empty);
				dict.Add("CREATED_BY"        , String.Empty);
				dict.Add("DATE_ENTERED"      , String.Empty);
				dict.Add("MODIFIED_USER_ID"  , String.Empty);
				dict.Add("DATE_MODIFIED"     , String.Empty);
				// 09/17/2009 Paul.  No need to audit the UTC date. 
				dict.Add("DATE_MODIFIED_UTC" , String.Empty);

				DataRow rowLast = dtAudit.Rows[0];
				for ( int i = 1; i < dtAudit.Rows.Count; i++ )
				{
					DataRow row = dtAudit.Rows[i];
					foreach ( DataColumn col in row.Table.Columns )
					{
						if ( !dict.ContainsKey(col.ColumnName) )
						{
							if ( Sql.ToString(rowLast[col.ColumnName]) != Sql.ToString(row[col.ColumnName]) )
							{
								DataRow rowChange = dtChanges.NewRow();
								dtChanges.Rows.Add(rowChange);
								// 09/16/2009 Paul.  Localize the field name. 
								rowChange["FIELD_NAME"  ] = Utils.TableColumnName(L10n, sModule, col.ColumnName);
								rowChange["CREATED_BY"  ] = SplendidCache.AssignedUser(Sql.ToGuid(row["MODIFIED_USER_ID"]));
								// 06/15/2009 Van.  The change date was not being converted to the time zone of the current user. 
								rowChange["DATE_CREATED"] = T10n.FromServerTime(row["AUDIT_DATE"]);
								rowChange["BEFORE_VALUE"] = rowLast[col.ColumnName];
								rowChange["AFTER_VALUE" ] = row    [col.ColumnName];
								// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
								if ( rowChange["BEFORE_VALUE"] != DBNull.Value )
								{
									if ( rowChange["BEFORE_VALUE"].GetType() == typeof(System.String) )
										rowChange["BEFORE_VALUE"] = HttpUtility.HtmlEncode(Sql.ToString(rowChange["BEFORE_VALUE"]));
								}
								if ( rowChange["AFTER_VALUE"] != DBNull.Value )
								{
									if ( rowChange["AFTER_VALUE"].GetType() == typeof(System.String) )
										rowChange["AFTER_VALUE"] = HttpUtility.HtmlEncode(Sql.ToString(rowChange["AFTER_VALUE"]));
								}
							}
						}
					}
					rowLast = row;
				}
			}
			return dtChanges;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				gID     = Sql.ToGuid  (Request["ID"    ]);
				sModule = Sql.ToString(Request["Module"]);
				string sTableName = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
				// 05/04/2008 Paul.  Protect against SQL Injection. A table name will never have a space character.
				sTableName = sTableName.Replace(" ", "");
				if ( !Sql.IsEmptyGuid(gID) && !Sql.IsEmptyString(sModule) && !Sql.IsEmptyString(sTableName) )
				{
					// 12/30/2007 Paul.  The first query should be used just to determine if access is allowed. 
					bool bAccessAllowed = false;
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vw" + sTableName + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, sModule, "view");
							Sql.AppendParameter(cmd, gID, "ID", false);

							using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdr.Read() )
								{
									bAccessAllowed = true;
									string sNAME = String.Empty;
									try
									{
										// 12/30/2007 Paul.  The name field might not be called NAME.
										// For now, just ignore the issue. 
										sNAME = Sql.ToString(rdr["NAME"]);
									}
									catch
									{
									}
									// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
									lblTitle.Text = L10n.Term(".moduleList." + sModule) + ": " + HttpUtility.HtmlEncode(sNAME);
									SetPageTitle(L10n.Term(".moduleList." + sModule) + " - " + sNAME);
								}
							}
						}
						if ( bAccessAllowed )
						{
							StringBuilder sb = new StringBuilder();
							DataTable dtTableColumns  = new DataTable();
							DataTable dtCustomColumns = new DataTable();
							// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
							// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTableName));
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtTableColumns);
								}
							}
							// 02/29/2008 Niall.  Some SQL Server 2005 installations require matching case for the parameters. 
							// Since we force the parameter to be uppercase, we must also make it uppercase in the command text. 
							sSQL = "select ColumnName              " + ControlChars.CrLf
							     + "  from vwSqlColumns            " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME" + ControlChars.CrLf
							     + " order by colid                " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 09/02/2008 Paul.  Standardize the case of metadata tables to uppercase.  PostgreSQL defaults to lowercase. 
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTableName + "_CSTM"));
							
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCustomColumns);
								}
							}
							string sAuditName       = sTableName + "_AUDIT";
							string sCustomAuditName = sTableName + "_CSTM_AUDIT";
							sb.AppendLine("select " + sAuditName       + ".AUDIT_ACTION  as AUDIT_ACTION      ");
							sb.AppendLine("     , " + sAuditName       + ".AUDIT_DATE    as AUDIT_DATE        ");
							sb.AppendLine("     , " + sAuditName       + ".AUDIT_COLUMNS as AUDIT_COLUMNS     ");
							sb.AppendLine("     , " + sCustomAuditName + ".AUDIT_COLUMNS as CSTM_AUDIT_COLUMNS");
							foreach ( DataRow row in dtTableColumns.Rows )
							{
								sb.AppendLine("     , " + sAuditName + "." + Sql.ToString(row["ColumnName"]));
							}
							foreach ( DataRow row in dtCustomColumns.Rows )
							{
								sb.AppendLine("     , " + sCustomAuditName + "." + Sql.ToString(row["ColumnName"]));
							}
							sb.AppendLine("  from            " + sAuditName);
							sb.AppendLine("  left outer join " + sCustomAuditName);
							sb.AppendLine("               on " + sCustomAuditName + ".ID_C        = " + sAuditName + ".ID         ");
							sb.AppendLine("              and " + sCustomAuditName + ".AUDIT_TOKEN = " + sAuditName + ".AUDIT_TOKEN");
							sb.AppendLine(" where " + sAuditName + ".ID = @ID");
							sb.AppendLine(" order by " + sAuditName + ".AUDIT_VERSION asc");
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sb.ToString();
								Sql.AddParameter(cmd, "@ID", gID);

								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SQLCode", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										DataTable dtChanges = BuildChangesTable(dt);
										vwMain = new DataView(dtChanges);
										// 06/03/2009 Paul.  We will not import the SugarCRM history, but we should still display it. 
										if ( Sql.ToBoolean(Application["CONFIG.append_sugarcrm_history"]) )
										{
											try
											{
												cmd.Parameters.Clear();
												using ( DataTable dtSugarCRM = new DataTable() )
												{
													string sSugarAuditName = sAuditName.ToUpper() + "_SUGARCRM";
													sSQL = "select " + sSugarAuditName + ".DATE_CREATED       " + ControlChars.CrLf
													     + "     , USERS.USER_NAME      as CREATED_BY         " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".FIELD_NAME         " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".BEFORE_VALUE_STRING" + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".AFTER_VALUE_STRING " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".BEFORE_VALUE_TEXT  " + ControlChars.CrLf
													     + "     , " + sSugarAuditName + ".AFTER_VALUE_TEXT   " + ControlChars.CrLf
													     + "  from      " + sSugarAuditName                     + ControlChars.CrLf
													     + " inner join USERS                                 " + ControlChars.CrLf
													     + "         on USERS.ID      = " + sSugarAuditName + ".CREATED_BY" + ControlChars.CrLf
													     + "        and USERS.DELETED = 0                     " + ControlChars.CrLf
													     + " where " + sSugarAuditName + ".PARENT_ID = @ID    " + ControlChars.CrLf
													     + " order by " + sSugarAuditName + ".DATE_CREATED    " + ControlChars.CrLf;
													cmd.CommandText = sSQL;
													Sql.AddParameter(cmd, "@ID", gID);
													if ( bDebug )
														Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "SugarSQLCode", Sql.ClientScriptBlock(cmd));

													da.Fill(dtSugarCRM);
													foreach ( DataRow rowSugar in dtSugarCRM.Rows )
													{
														DataRow rowMerge = dtChanges.NewRow();
														rowMerge["DATE_CREATED"] = Sql.ToString(rowSugar["DATE_CREATED"       ]);
														rowMerge["CREATED_BY"  ] = Sql.ToString(rowSugar["CREATED_BY"         ]);
														rowMerge["FIELD_NAME"  ] = Sql.ToString(rowSugar["FIELD_NAME"         ]);
														rowMerge["BEFORE_VALUE"] = Sql.ToString(rowSugar["BEFORE_VALUE_STRING"]) + Sql.ToString(rowSugar["BEFORE_VALUE_TEXT"]);
														rowMerge["AFTER_VALUE" ] = Sql.ToString(rowSugar["AFTER_VALUE_STRING" ]) + Sql.ToString(rowSugar["AFTER_VALUE_TEXT" ]);
														dtChanges.Rows.Add(rowMerge);
													}
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
										}
										vwMain = new DataView(dtChanges);
										vwMain.Sort = "DATE_CREATED desc, FIELD_NAME asc";
										grdMain.DataSource = vwMain ;
										if ( !IsPostBack )
										{
											grdMain.DataBind();
										}
									}
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  The primary data binding will now only occur in the ASPX pages so that this is only one per cycle. 
				// 03/11/2008 Paul.  Move the primary binding to SplendidPage. 
				//Page DataBind();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}


