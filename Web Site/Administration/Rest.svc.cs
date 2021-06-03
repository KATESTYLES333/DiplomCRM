/*
 * Copyright (C) 2016 SplendidCRM Software, Inc. All Rights Reserved. 
 *
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License 
 * Agreement, or other written agreement between you and SplendidCRM ("License"). By installing or 
 * using this file, you have unconditionally agreed to the terms and conditions of the License, 
 * including but not limited to restrictions on the number of users therein, and you may not use this 
 * file except in compliance with the License. 
 * 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and 
 * trademarks, in and to the contents of this file.  You will not link to or in any way combine the 
 * contents of this file or any derivatives with any Open Source Code in any manner that would require 
 * the contents of this file to be made available to any third party. 
 * 
 * IN NO EVENT SHALL SPLENDIDCRM BE RESPONSIBLE FOR ANY DAMAGES OF ANY KIND, INCLUDING ANY DIRECT, 
 * SPECIAL, PUNITIVE, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGES.  Other limitations of liability 
 * and disclaimers set forth in the License. 
 * 
 */
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace SplendidCRM.Administration
{
	[ServiceContract]
	[ServiceBehavior(IncludeExceptionDetailInFaults=true)]
	[AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
	public class Rest
	{
		public class ViewNode
		{
			public string ViewName   ;
			public string DisplayName;
			public string LayoutType ;
		}

		public class ModuleNode
		{
			public string       ModuleName   ;
			public string       DisplayName  ;
			public bool         IsAdmin      ;
			public List<object> EditViews    ;
			public List<object> Search       ;
			public List<object> DetailViews  ;
			public List<object> ListViews    ;
			public List<object> SubPanels    ;
			public List<object> Relationships;
			public List<object> Terminology  ;

			public ModuleNode()
			{
				this.EditViews     = new List<object>();
				this.Search        = new List<object>();
				this.DetailViews   = new List<object>();
				this.ListViews     = new List<object>();
				this.SubPanels     = new List<object>();
				this.Relationships = new List<object>();
				this.Terminology   = new List<object>();
			}
		}

		public class LayoutField
		{
			public string ColumnName        ;
			public string ColumnType        ;
			public string CsType            ;
			public int    length            ;
			public string FIELD_TYPE        ;
			public string DATA_LABEL        ;
			public string DATA_FIELD        ;
			public string MODULE_TYPE       ;
			public string LIST_NAME         ;
			public string DATA_FORMAT       ;
			public int    FORMAT_MAX_LENGTH ;
			public string URL_FIELD         ;
			public string URL_FORMAT        ;
			public string COLUMN_TYPE       ;
			public string HEADER_TEXT       ;
			public string SORT_EXPRESSION   ;
			public string URL_ASSIGNED_FIELD;
		}

		#region Get
		public static Stream ToJsonStream(Dictionary<string, object> d)
		{
			JavaScriptSerializer json = new JavaScriptSerializer();
			// 05/05/2013 Paul.  No reason to limit the Json result. 
			json.MaxJsonLength = int.MaxValue;
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAdminLayoutModules()
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpApplicationState Application = HttpContext.Current.Application;
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			Dictionary<string, ModuleNode> dict = new Dictionary<string, ModuleNode>();
			List<ModuleNode> lstModules = new List<ModuleNode>();
			
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				DataTable dtLANGUAGES = new DataTable();
				sSQL = "select NAME                                   " + ControlChars.CrLf
				     + "     , DISPLAY_NAME                           " + ControlChars.CrLf
				     + "  from vwLANGUAGES                            " + ControlChars.CrLf
				     + " where ACTIVE = 1                             " + ControlChars.CrLf
				     + "   and NAME not in ('en-AU', 'en-GB', 'en-CA')" + ControlChars.CrLf
				     + " order by NAME                                " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						da.Fill(dtLANGUAGES);
					}
				}
				// 04/06/2016 Paul.  Exclude disabled modules. 
				Dictionary<string, bool> dictAllModules = new Dictionary<string,bool>();
				sSQL = "select MODULE_NAME                            " + ControlChars.CrLf
				     + "     , MODULE_ENABLED                         " + ControlChars.CrLf
				     + "  from vwMODULES_Edit                         " + ControlChars.CrLf
				     + " order by MODULE_NAME                         " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							foreach ( DataRow row in dt.Rows )
							{
								string sMODULE_NAME    = Sql.ToString (row["MODULE_NAME"   ]);
								bool   bMODULE_ENABLED = Sql.ToBoolean(row["MODULE_ENABLED"]);
								if ( !dictAllModules.ContainsKey(sMODULE_NAME) )
									dictAllModules[sMODULE_NAME] = bMODULE_ENABLED;
							}
						}
					}
				}

				sSQL = "select NAME                                   " + ControlChars.CrLf
				     + "     , MODULE_NAME                            " + ControlChars.CrLf
				     + "     , 'EditView'               as LAYOUT_TYPE" + ControlChars.CrLf
				     + "  from vwEDITVIEWS                            " + ControlChars.CrLf
				     + "union all                                     " + ControlChars.CrLf
				     + "select NAME                                   " + ControlChars.CrLf
				     + "     , MODULE_NAME                            " + ControlChars.CrLf
				     + "     , 'DetailView'             as LAYOUT_TYPE" + ControlChars.CrLf
				     + "  from vwDETAILVIEWS                          " + ControlChars.CrLf
				     + "union all                                     " + ControlChars.CrLf
				     + "select NAME                                   " + ControlChars.CrLf
				     + "     , MODULE_NAME                            " + ControlChars.CrLf
				     + "     , 'ListView'               as LAYOUT_TYPE" + ControlChars.CrLf
				     + "  from vwGRIDVIEWS                            " + ControlChars.CrLf
				     + "union all                                     " + ControlChars.CrLf
				     + "select distinct DETAIL_NAME            as NAME" + ControlChars.CrLf
				     + "     , DETAIL_NAME              as MODULE_NAME" + ControlChars.CrLf
				     + "     , 'DetailViewRelationship'               " + ControlChars.CrLf
				     + "  from vwDETAILVIEWS_RELATIONSHIPS_La         " + ControlChars.CrLf
				     + "union all                                     " + ControlChars.CrLf
				     + "select distinct EDIT_NAME              as NAME" + ControlChars.CrLf
				     + "     , EDIT_NAME               as  MODULE_NAME" + ControlChars.CrLf
				     + "     , 'EditViewRelationship'                 " + ControlChars.CrLf
				     + "  from " + Sql.MetadataName(con, "vwEDITVIEWS_RELATIONSHIPS_Layout") + ControlChars.CrLf
				     + " order by MODULE_NAME, NAME                   " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							ViewNode view = null;
							ModuleNode dictMODULE = new ModuleNode();
							lstModules.Add(dictMODULE);
							dictMODULE.ModuleName  = String.Empty;
							dictMODULE.IsAdmin     = false;
							dictMODULE.DisplayName = "Global";
							foreach ( DataRow rowLang in dtLANGUAGES.Rows )
							{
								view = new ViewNode();
								view.ViewName    = Sql.ToString(rowLang["NAME"]);
								view.LayoutType  = "Terminology";
								view.DisplayName = Sql.ToString(rowLang["DISPLAY_NAME"]);
								dictMODULE.Terminology.Add(view);
							}
							foreach ( DataRow row in dt.Rows )
							{
								string sNAME        = Sql.ToString(row["NAME"       ]);
								string sMODULE_NAME = Sql.ToString(row["MODULE_NAME"]);
								string sLAYOUT_TYPE = Sql.ToString(row["LAYOUT_TYPE"]);
								if ( sLAYOUT_TYPE == "DetailViewRelationship" || sLAYOUT_TYPE == "EditViewRelationship" )
								{
									string[] arrMODULE_NAME = sMODULE_NAME.Split('.');
									sMODULE_NAME = arrMODULE_NAME[0];
								}
								// 04/06/2016 Paul.  Exclude disabled modules. 
								if ( dictAllModules.ContainsKey(sMODULE_NAME) && !dictAllModules[sMODULE_NAME] )
									continue;
								try
								{
									if ( !dict.ContainsKey(sMODULE_NAME) )
									{
										dictMODULE = new ModuleNode();
										dict.Add(sMODULE_NAME, dictMODULE);
										lstModules.Add(dictMODULE);
										dictMODULE.ModuleName  = sMODULE_NAME;
										dictMODULE.IsAdmin     = Sql.ToBoolean(Application["Modules." + sMODULE_NAME + ".IsAdmin"]);
										dictMODULE.DisplayName = L10n.Term(".moduleList." + sMODULE_NAME);
										if ( dictMODULE.DisplayName.StartsWith(".moduleList.") )
											dictMODULE.DisplayName = sMODULE_NAME;
										
										foreach ( DataRow rowLang in dtLANGUAGES.Rows )
										{
											view = new ViewNode();
											view.ViewName    = Sql.ToString(rowLang["NAME"]);
											view.LayoutType  = "Terminology";
											view.DisplayName = Sql.ToString(rowLang["DISPLAY_NAME"]);
											dictMODULE.Terminology.Add(view);
										}
									}
									else
									{
										dictMODULE = dict[sMODULE_NAME] as ModuleNode;
									}
									view = new ViewNode();
									view.ViewName    = sNAME       ;
									view.LayoutType  = sLAYOUT_TYPE;
									view.DisplayName = sNAME       ;
									if ( sNAME.StartsWith(sMODULE_NAME + ".") )
										view.DisplayName = sNAME.Substring(sMODULE_NAME.Length + 1);
									switch ( sLAYOUT_TYPE )
									{
										case "EditView"  :
											if ( sNAME.Contains(".Search") )
												dictMODULE.Search.Add(view);
											else
												dictMODULE.EditViews.Add(view);
											break;
										case "DetailView":
											dictMODULE.DetailViews.Add(view);
											break;
										case "ListView"  :
											// 10/19/2017 Paul.  ArchiveView should be part of the main list. 
											if ( sNAME.Contains(".ArchiveView") || sNAME.Contains(".ListView") || sNAME.Contains(".PopupView") || sNAME.EndsWith(".Export") || sNAME.Contains("." + sMODULE_NAME) )
												dictMODULE.ListViews.Add(view);
											else
												dictMODULE.SubPanels.Add(view);
											break;
										case "DetailViewRelationship":
											dictMODULE.Relationships.Add(view);
											break;
										case "EditViewRelationship":
											dictMODULE.Relationships.Add(view);
											break;
									}
								}
								catch(Exception ex)
								{
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								}
							}
						}
					}
				}
			}
			
			Dictionary<string, object> d = new Dictionary<string, object>();
			d.Add("d", lstModules);
			JavaScriptSerializer json = new JavaScriptSerializer();
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}
		
		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
		public Stream GetAdminLayoutModuleFields(string ModuleName, string LayoutType, string LayoutName)
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			if ( Sql.IsEmptyString(ModuleName) )
				throw(new Exception("The module name must be specified."));
			string sTABLE_NAME = Sql.ToString(Application["Modules." + ModuleName + ".TableName"]);
			string sVIEW_NAME  = "vw" + sTABLE_NAME;
			// 04/06/2016 Paul.  Some modules do not have a table name, but are still valid. 
			bool   bValid      = Sql.ToBoolean(Application["Modules." + ModuleName + ".Valid"]);
			if ( Sql.IsEmptyString(sTABLE_NAME) && !bValid )
				throw(new Exception("Unknown module: " + ModuleName));
			
			List<LayoutField> lstFields = new List<LayoutField>();
			if ( LayoutType != "EditView" && LayoutType != "DetailView" && LayoutType != "ListView" )
			{
				LayoutType = "EditView";
			}
			// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
			if ( Sql.IsEmptyString(LayoutName) )
			{
				LayoutName = ModuleName + "." + LayoutType;
			}
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL;
				DataTable dtDefaultView = new DataTable();
				if ( LayoutType == "EditView" )
				{
					sSQL = "select *                        " + ControlChars.CrLf
					     + "  from vwEDITVIEWS_FIELDS       " + ControlChars.CrLf
					     + " where EDIT_NAME = @LAYOUT_NAME " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = 1         " + ControlChars.CrLf
					     + " order by FIELD_INDEX           " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dtDefaultView);
							if ( dtDefaultView.Rows.Count == 0 )
							{
								sSQL = "select *                        " + ControlChars.CrLf
								     + "  from vwEDITVIEWS_FIELDS       " + ControlChars.CrLf
								     + " where EDIT_NAME = @LAYOUT_NAME " + ControlChars.CrLf
								     + "   and DEFAULT_VIEW = 0         " + ControlChars.CrLf
								     + " order by FIELD_INDEX           " + ControlChars.CrLf;
								da.Fill(dtDefaultView);
							}
						}
					}
					// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
					sSQL = "select VIEW_NAME          " + ControlChars.CrLf
					     + "  from vwEDITVIEWS        " + ControlChars.CrLf
					     + " where NAME = @LAYOUT_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						sVIEW_NAME = Sql.ToString(cmd.ExecuteScalar());
						if ( Sql.IsEmptyString(sVIEW_NAME) )
							sVIEW_NAME  = "vw" + sTABLE_NAME + "_Edit";
					}
				}
				else if ( LayoutType == "DetailView" )
				{
					sSQL = "select *                         " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS_FIELDS      " + ControlChars.CrLf
					     + " where DETAIL_NAME = @LAYOUT_NAME" + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = 1          " + ControlChars.CrLf
					     + " order by FIELD_INDEX            " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dtDefaultView);
							if ( dtDefaultView.Rows.Count == 0 )
							{
								sSQL = "select *                         " + ControlChars.CrLf
								     + "  from vwDETAILVIEWS_FIELDS      " + ControlChars.CrLf
								     + " where DETAIL_NAME = @LAYOUT_NAME" + ControlChars.CrLf
								     + "   and DEFAULT_VIEW = 0          " + ControlChars.CrLf
								     + " order by FIELD_INDEX            " + ControlChars.CrLf;
								da.Fill(dtDefaultView);
							}
						}
					}
					// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
					sSQL = "select VIEW_NAME          " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS      " + ControlChars.CrLf
					     + " where NAME = @LAYOUT_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						sVIEW_NAME = Sql.ToString(cmd.ExecuteScalar());
						if ( Sql.IsEmptyString(sVIEW_NAME) )
							sVIEW_NAME  = "vw" + sTABLE_NAME + "_Edit";
					}
				}
				else if ( LayoutType == "ListView" )
				{
					sSQL = "select *                        " + ControlChars.CrLf
					     + "  from vwGRIDVIEWS_COLUMNS      " + ControlChars.CrLf
					     + " where GRID_NAME = @LAYOUT_NAME " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = 1         " + ControlChars.CrLf
					     + " order by COLUMN_INDEX          " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dtDefaultView);
							if ( dtDefaultView.Rows.Count == 0 )
							{
								sSQL = "select *                        " + ControlChars.CrLf
								     + "  from vwGRIDVIEWS_COLUMNS      " + ControlChars.CrLf
								     + " where GRID_NAME = @LAYOUT_NAME " + ControlChars.CrLf
								     + "   and DEFAULT_VIEW = 0         " + ControlChars.CrLf
								     + " order by COLUMN_INDEX          " + ControlChars.CrLf;
								da.Fill(dtDefaultView);
							}
						}
					}
					// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
					sSQL = "select VIEW_NAME          " + ControlChars.CrLf
					     + "  from vwGRIDVIEWS        " + ControlChars.CrLf
					     + " where NAME = @LAYOUT_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@LAYOUT_NAME", LayoutName);
						sVIEW_NAME = Sql.ToString(cmd.ExecuteScalar());
						if ( Sql.IsEmptyString(sVIEW_NAME) )
							sVIEW_NAME  = "vw" + sTABLE_NAME + "_List";
					}
				}
				DataView vwDefaultView = new DataView(dtDefaultView);
				
				// 05/09/2016 Paul.  DetailView and ListView needs access to all fields, not just those being updated in the stored procedure. 
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
					if ( LayoutType == "EditView" && !LayoutName.Contains(".Search") )
					{
						// 08/30/2017 Paul.  Provide a way to show all fields and not just editable fields.  This will allow read-only fields to be added to an EditView. 
						if ( Sql.ToBoolean (Application["CONFIG.LayoutEditor.EditView.AllFields"]) )
						{
							sSQL = "select *                        " + ControlChars.CrLf
							     + "  from vwSqlColumns             " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME " + ControlChars.CrLf
							     + " order by ColumnName            " + ControlChars.CrLf;
							cmd.CommandText = sSQL;
							if ( !Sql.IsEmptyString(sVIEW_NAME) )
							{
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sVIEW_NAME));
							}
							else
							{
								Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sTABLE_NAME));
							}
						}
						else
						{
							sSQL = "select *                        " + ControlChars.CrLf
							     + "  from vwSqlColumns             " + ControlChars.CrLf
							     + " where ObjectName = @OBJECTNAME " + ControlChars.CrLf
							     + "   and ObjectType = 'P'         " + ControlChars.CrLf
							     + " union all                      " + ControlChars.CrLf
							     + "select *                        " + ControlChars.CrLf
							     + "  from vwSqlColumns             " + ControlChars.CrLf
							     + " where ObjectName = @CUSTOMNAME " + ControlChars.CrLf
							     + "   and ObjectType = 'U'         " + ControlChars.CrLf;
							// 02/20/2016 Paul.  Oracle does not allow order by after union all. 
							// 03/20/2016 Paul.  We still want an alphabetical sort. 
							if ( Sql.IsOracle(con) )
							{
								sSQL = "select *" + ControlChars.CrLf
								     + " from (" + sSQL + ControlChars.CrLf
								     + "      ) vwSqlColumns" + ControlChars.CrLf
								     + " order by ColumnName" + ControlChars.CrLf;
							}
							else
							{
								sSQL += " order by ColumnName" + ControlChars.CrLf;
							}
							cmd.CommandText = sSQL;
							// 05/09/2016 Paul.  DetailView and ListView needs access to all fields, not just those being updated in the stored procedure. 
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "sp" + sTABLE_NAME + "_Update"));
							Sql.AddParameter(cmd, "@CUSTOMNAME", Sql.MetadataName(cmd, sTABLE_NAME + "_CSTM"));
						}
					}
					else
					{
						sSQL = "select *" + ControlChars.CrLf
						     + "  from vwSqlColumns             " + ControlChars.CrLf
						     + " where ObjectName = @OBJECTNAME " + ControlChars.CrLf
						     + " order by ColumnName            " + ControlChars.CrLf;
						cmd.CommandText = sSQL;
						// 10/19/2016 Paul.  Specify the LayoutName so that we can search the fields added in a _List view. 
						if ( !Sql.IsEmptyString(sVIEW_NAME) )
						{
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sVIEW_NAME));
						}
						else if ( LayoutType == "ListView" )
						{
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sTABLE_NAME + "_List"));
						}
						else
						{
							Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, "vw" + sTABLE_NAME));
						}
					}
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							if ( !dt.Columns.Contains("FIELD_TYPE"        ) ) dt.Columns.Add("FIELD_TYPE"        , typeof(System.String));
							// 02/20/2016 Paul.  EditView columns. 
							if ( !dt.Columns.Contains("DATA_LABEL"        ) ) dt.Columns.Add("DATA_LABEL"        , typeof(System.String));
							if ( !dt.Columns.Contains("DATA_FIELD"        ) ) dt.Columns.Add("DATA_FIELD"        , typeof(System.String));
							if ( !dt.Columns.Contains("DISPLAY_FIELD"     ) ) dt.Columns.Add("DISPLAY_FIELD"     , typeof(System.String));
							if ( !dt.Columns.Contains("MODULE_TYPE"       ) ) dt.Columns.Add("MODULE_TYPE"       , typeof(System.String));
							if ( !dt.Columns.Contains("LIST_NAME"         ) ) dt.Columns.Add("LIST_NAME"         , typeof(System.String));
							if ( !dt.Columns.Contains("DATA_FORMAT"       ) ) dt.Columns.Add("DATA_FORMAT"       , typeof(System.String));
							if ( !dt.Columns.Contains("FORMAT_MAX_LENGTH" ) ) dt.Columns.Add("FORMAT_MAX_LENGTH" , typeof(System.Int32 ));
							// 02/15/2016 Paul.  DetailView columns. 
							if ( !dt.Columns.Contains("URL_FIELD"         ) ) dt.Columns.Add("URL_FIELD"         , typeof(System.String));
							if ( !dt.Columns.Contains("URL_FORMAT"        ) ) dt.Columns.Add("URL_FORMAT"        , typeof(System.String));
							// 02/15/2016 Paul.  ListView columns. 
							if ( !dt.Columns.Contains("COLUMN_TYPE"       ) ) dt.Columns.Add("COLUMN_TYPE"       , typeof(System.String));
							if ( !dt.Columns.Contains("HEADER_TEXT"       ) ) dt.Columns.Add("HEADER_TEXT"       , typeof(System.String));
							if ( !dt.Columns.Contains("SORT_EXPRESSION"   ) ) dt.Columns.Add("SORT_EXPRESSION"   , typeof(System.String));
							if ( !dt.Columns.Contains("URL_ASSIGNED_FIELD") ) dt.Columns.Add("URL_ASSIGNED_FIELD", typeof(System.String));
							foreach ( DataRow row in dt.Rows )
							{
								string sColumnName = Sql.ToString (row["ColumnName"]);
								if ( sColumnName.StartsWith("@") )
									sColumnName = sColumnName.Replace("@", String.Empty);
								else if ( sColumnName.StartsWith("ID_") && Sql.IsOracle(cmd) )
									sColumnName = sColumnName.Substring(3);
								// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
								// 12/08/2017 Paul.  We want to see the ID on the export. 
								if ( (sColumnName == "ID" && !LayoutName.Contains(".Export")) || sColumnName == "ID_C" || sColumnName == "MODIFIED_USER_ID" || sColumnName == "TEAM_SET_LIST" || sColumnName == "ASSIGNED_SET_LIST" )
								{
									row.Delete();
									continue;
								}
								row["ColumnName" ] = sColumnName ;
								row["DATA_LABEL" ] = Utils.BuildTermName(ModuleName, sColumnName);
								row["DATA_FIELD" ] = sColumnName;
								if ( LayoutType == "EditView" )
								{
									row["FIELD_TYPE" ] = "TextBox";
									vwDefaultView.RowFilter = "DATA_FIELD = '" + sColumnName + "'";
									if ( vwDefaultView.Count > 0 )
									{
										row["FIELD_TYPE"       ] = Sql.ToString (vwDefaultView[0]["FIELD_TYPE"       ]);
										row["DISPLAY_FIELD"    ] = Sql.ToString (vwDefaultView[0]["DISPLAY_FIELD"    ]);
										row["LIST_NAME"        ] = Sql.ToString (vwDefaultView[0]["LIST_NAME"        ]);
										row["DATA_FORMAT"      ] = Sql.ToString (vwDefaultView[0]["DATA_FORMAT"      ]);
										row["FORMAT_MAX_LENGTH"] = Sql.ToInteger(vwDefaultView[0]["FORMAT_MAX_LENGTH"]);
										row["MODULE_TYPE"      ] = Sql.ToString (vwDefaultView[0]["MODULE_TYPE"      ]);
									}
								}
								else if ( LayoutType == "DetailView" )
								{
									row["FIELD_TYPE" ] = "String";
									row["DATA_FORMAT"] = "{0}";
									vwDefaultView.RowFilter = "DATA_FIELD = '" + sColumnName + "'";
									if ( vwDefaultView.Count > 0 )
									{
										row["FIELD_TYPE" ] = Sql.ToString(vwDefaultView[0]["FIELD_TYPE" ]);
										row["LIST_NAME"  ] = Sql.ToString(vwDefaultView[0]["LIST_NAME"  ]);
										row["DATA_FORMAT"] = Sql.ToString(vwDefaultView[0]["DATA_FORMAT"]);
										row["URL_FIELD"  ] = Sql.ToString(vwDefaultView[0]["URL_FIELD"  ]);
										row["URL_FORMAT" ] = Sql.ToString(vwDefaultView[0]["URL_FORMAT" ]);
										row["MODULE_TYPE"] = Sql.ToString(vwDefaultView[0]["MODULE_TYPE"]);
									}
								}
								else if ( LayoutType == "ListView" )
								{
									row["COLUMN_TYPE"    ] = "BoundColumn";
									row["DATA_FORMAT"    ] = String.Empty;
									row["SORT_EXPRESSION"] = sColumnName;
									row["HEADER_TEXT"    ] = Utils.BuildTermName(ModuleName, sColumnName).Replace(".LBL_", ".LBL_LIST_");
									vwDefaultView.RowFilter = "DATA_FIELD = '" + sColumnName + "'";
									if ( vwDefaultView.Count > 0 )
									{
										row["COLUMN_TYPE"       ] = Sql.ToString(vwDefaultView[0]["COLUMN_TYPE"       ]);
										row["DATA_FORMAT"       ] = Sql.ToString(vwDefaultView[0]["DATA_FORMAT"       ]);
										row["HEADER_TEXT"       ] = Sql.ToString(vwDefaultView[0]["HEADER_TEXT"       ]);
										row["SORT_EXPRESSION"   ] = Sql.ToString(vwDefaultView[0]["SORT_EXPRESSION"   ]);
										row["LIST_NAME"         ] = Sql.ToString(vwDefaultView[0]["LIST_NAME"         ]);
										row["URL_FIELD"         ] = Sql.ToString(vwDefaultView[0]["URL_FIELD"         ]);
										row["URL_FORMAT"        ] = Sql.ToString(vwDefaultView[0]["URL_FORMAT"        ]);
										row["MODULE_TYPE"       ] = Sql.ToString(vwDefaultView[0]["MODULE_TYPE"       ]);
										row["URL_ASSIGNED_FIELD"] = Sql.ToString(vwDefaultView[0]["URL_ASSIGNED_FIELD"]);
									}
								}
							}
							dt.AcceptChanges();
							DataView vw = new DataView(dt);
							vw.Sort = "DATA_FIELD asc";
							foreach ( DataRow row in dt.Rows )
							{
								LayoutField lay = new LayoutField();
								lay.ColumnName         = Sql.ToString (row["ColumnName"        ]);
								lay.ColumnType         = Sql.ToString (row["ColumnType"        ]);
								lay.CsType             = Sql.ToString (row["CsType"            ]);
								lay.length             = Sql.ToInteger(row["length"            ]);
								lay.FIELD_TYPE         = Sql.ToString (row["FIELD_TYPE"        ]);
								lay.DATA_LABEL         = Sql.ToString (row["DATA_LABEL"        ]);
								lay.DATA_FIELD         = Sql.ToString (row["DATA_FIELD"        ]);
								lay.MODULE_TYPE        = Sql.ToString (row["MODULE_TYPE"       ]);
								lay.LIST_NAME          = Sql.ToString (row["LIST_NAME"         ]);
								lay.DATA_FORMAT        = Sql.ToString (row["DATA_FORMAT"       ]);
								if ( lay.CsType == "string" )
									lay.FORMAT_MAX_LENGTH  = Sql.ToInteger(row["FORMAT_MAX_LENGTH" ]);
								lay.URL_FIELD          = Sql.ToString (row["URL_FIELD"         ]);
								lay.URL_FORMAT         = Sql.ToString (row["URL_FORMAT"        ]);
								lay.COLUMN_TYPE        = Sql.ToString (row["COLUMN_TYPE"       ]);
								lay.HEADER_TEXT        = Sql.ToString (row["HEADER_TEXT"       ]);
								lay.SORT_EXPRESSION    = Sql.ToString (row["SORT_EXPRESSION"   ]);
								lay.URL_ASSIGNED_FIELD = Sql.ToString (row["URL_ASSIGNED_FIELD"]);
								lstFields.Add(lay);
							}
						}
					}
				}
			}
			
			Dictionary<string, object> d = new Dictionary<string, object>();
			d.Add("d", lstFields);
			JavaScriptSerializer json = new JavaScriptSerializer();
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAdminLayoutRelationshipFields(string TableName, string ModuleName)
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			if ( Sql.IsEmptyString(TableName) )
				throw(new Exception("The table name must be specified."));
			if ( !TableName.StartsWith("vw") )
				throw(new Exception("The table name is not in the correct format."));
			Regex r = new Regex(@"[^A-Za-z0-9_]");
			string sTABLE_NAME = r.Replace(TableName, "");
			
			List<LayoutField> lstFields = new List<LayoutField>();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL;
				sSQL = "select *                        " + ControlChars.CrLf
				     + "  from vwSqlColumns             " + ControlChars.CrLf
				     + " where ObjectName = @OBJECTNAME " + ControlChars.CrLf
				     + "   and ObjectType = 'V'         " + ControlChars.CrLf
				     + " order by ColumnName            " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					// 02/20/2016 Paul.  Make sure to use upper case for Oracle. 
					Sql.AddParameter(cmd, "@OBJECTNAME", Sql.MetadataName(cmd, sTABLE_NAME));
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							dt.Columns.Add("FIELD_TYPE"        , typeof(System.String));
							dt.Columns.Add("DATA_LABEL"        , typeof(System.String));
							dt.Columns.Add("DATA_FIELD"        , typeof(System.String));
							foreach ( DataRow row in dt.Rows )
							{
								string sColumnName = Sql.ToString (row["ColumnName"]);
								row["ColumnName" ] = sColumnName ;
								row["DATA_LABEL" ] = Utils.BuildTermName(ModuleName, sColumnName);
								row["DATA_FIELD" ] = sColumnName;
							}
							DataView vw = new DataView(dt);
							vw.Sort = "DATA_FIELD asc";

							foreach ( DataRow row in dt.Rows )
							{
								LayoutField lay = new LayoutField();
								lay.ColumnName  = Sql.ToString (row["ColumnName" ]);
								lay.ColumnType  = Sql.ToString (row["ColumnType" ]);
								lay.CsType      = Sql.ToString (row["CsType"     ]);
								lay.length      = Sql.ToInteger(row["length"     ]);
								lay.DATA_LABEL  = Sql.ToString (row["DATA_LABEL" ]);
								lay.DATA_FIELD  = Sql.ToString (row["DATA_FIELD" ]);
								lstFields.Add(lay);
							}
						}
					}
				}
			}
			
			Dictionary<string, object> d = new Dictionary<string, object>();
			d.Add("d", lstFields);
			JavaScriptSerializer json = new JavaScriptSerializer();
			string sResponse = json.Serialize(d);
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAdminLayoutTerminologyLists()
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			
			L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			Dictionary<string, object> d       = new Dictionary<string, object>();
			Dictionary<string, object> results = new Dictionary<string, object>();
			Dictionary<string, object> objs    = new Dictionary<string, object>();
			results.Add("results", objs);
			d.Add("d", results);
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					List<string> vwTERMINOLOGY_PickList = new List<string>();
					sSQL = "select LIST_NAME             " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY_PickList" + ControlChars.CrLf
					     + " order by LIST_NAME          " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sNAME = Sql.ToString(row["LIST_NAME"]);
									vwTERMINOLOGY_PickList.Add(sNAME);
								}
							}
						}
					}
					for ( int i = 0; i < SplendidCache.CustomCaches.Count; i++ )
					{
						vwTERMINOLOGY_PickList.Add(SplendidCache.CustomCaches[i].Name);
					}
					objs.Add("vwTERMINOLOGY_PickList", vwTERMINOLOGY_PickList);
					
					List<string> FIELD_VALIDATORS = new List<string>();
					sSQL = "select ID                " + ControlChars.CrLf
					     + "     , NAME              " + ControlChars.CrLf
					     + "  from vwFIELD_VALIDATORS" + ControlChars.CrLf
					     + " order by NAME           " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sNAME = Sql.ToString(row["ID"]);
									FIELD_VALIDATORS.Add(sNAME);
								}
							}
						}
					}
					objs.Add("FIELD_VALIDATORS", FIELD_VALIDATORS);
					
					DataTable dtModules = SplendidCache.ModulesPopups(Context);
					DataView vwModules = new DataView(dtModules);
					vwModules.RowFilter = "HAS_POPUP = 1";
					List<string> MODULE_TYPES = new List<string>();
					foreach ( DataRowView row in vwModules )
					{
						string sNAME = Sql.ToString(row["MODULE_NAME"]);
						MODULE_TYPES.Add(sNAME);
					}
					objs.Add("MODULE_TYPES", MODULE_TYPES);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			// 04/21/2017 Paul.  Count should be returend as a number. 
			d.Add("__count", objs.Count);
			return ToJsonStream(d);
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAdminLayoutTerminology()
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpContext          Context     = HttpContext.Current;
			HttpSessionState     Session     = HttpContext.Current.Session;
			HttpApplicationState Application = HttpContext.Current.Application;
			
			L10N L10n = new L10N(Session["USER_SETTINGS/CULTURE"] as string);
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			Dictionary<string, object>       d       = new Dictionary<string, object>();
			Dictionary<string, object>       results = new Dictionary<string, object>();
			List<Dictionary<string, object>> objs    = new List<Dictionary<string, object>>();
			results.Add("results", objs);
			d.Add("d", results);
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select NAME                         " + ControlChars.CrLf
					     + "     , MODULE_NAME                  " + ControlChars.CrLf
					     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY                " + ControlChars.CrLf
					     + " where MODULE_NAME in ('DynamicLayout', 'BusinessRules')" + ControlChars.CrLf
					     + "   and LANG = @LANG                 " + ControlChars.CrLf
					     + " order by NAME                      " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						string sLANG  = Sql.ToString(Session["USER_SETTINGS/CULTURE" ]);
						Sql.AddParameter(cmd, "@LANG", sLANG);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									Dictionary<string, object> drow = new Dictionary<string, object>();
									drow.Add("NAME"        , Sql.ToString(row["NAME"        ]));
									drow.Add("LIST_NAME"   , null                             );
									drow.Add("MODULE_NAME" , Sql.ToString(row["MODULE_NAME" ]));
									drow.Add("DISPLAY_NAME", Sql.ToString(row["DISPLAY_NAME"]));
									objs.Add(drow);
								}
							}
						}
					}
					sSQL = "select MODULE_NAME                  " + ControlChars.CrLf
					     + "     , DISPLAY_NAME                 " + ControlChars.CrLf
					     + "  from vwMODULES                    " + ControlChars.CrLf
					     + " order by MODULE_NAME               " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									string sMODULE_NAME  = Sql.ToString(row["MODULE_NAME" ]);
									string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
									Dictionary<string, object> drow = new Dictionary<string, object>();
									drow.Add("NAME"        , "LBL_MODULE_NAME" );
									drow.Add("LIST_NAME"   , null              );
									drow.Add("MODULE_NAME" , sMODULE_NAME      );
									drow.Add("DISPLAY_NAME", L10n.Term(sMODULE_NAME + ".LBL_MODULE_NAME"));
									objs.Add(drow);
									drow = new Dictionary<string, object>();
									drow.Add("NAME"        , "LBL_NEW_FORM_TITLE" );
									drow.Add("LIST_NAME"   , null              );
									drow.Add("MODULE_NAME" , sMODULE_NAME      );
									drow.Add("DISPLAY_NAME", L10n.Term(sMODULE_NAME + ".LBL_NEW_FORM_TITLE"));
									objs.Add(drow);
									if ( sMODULE_NAME == "Activities" )
									{
										drow = new Dictionary<string, object>();
										drow.Add("NAME"        , "LBL_HISTORY"        );
										drow.Add("LIST_NAME"   , null                 );
										drow.Add("MODULE_NAME" , sMODULE_NAME         );
										drow.Add("DISPLAY_NAME", L10n.Term(sMODULE_NAME + ".LBL_HISTORY"));
										objs.Add(drow);
										drow = new Dictionary<string, object>();
										drow.Add("NAME"        , "LBL_OPEN_ACTIVITIES");
										drow.Add("LIST_NAME"   , null                 );
										drow.Add("MODULE_NAME" , sMODULE_NAME         );
										drow.Add("DISPLAY_NAME", L10n.Term(sMODULE_NAME + ".LBL_OPEN_ACTIVITIES"));
										objs.Add(drow);
									}
									if ( sDISPLAY_NAME.StartsWith(".moduleList.") )
									{
										drow = new Dictionary<string, object>();
										drow.Add("NAME"        , sDISPLAY_NAME.Replace(".moduleList.", ""));
										drow.Add("LIST_NAME"   , "moduleList" );
										drow.Add("MODULE_NAME" , null         );
										drow.Add("DISPLAY_NAME", sDISPLAY_NAME);
										objs.Add(drow);
									}
								}
							}
						}
					}
					sSQL = "select ID                " + ControlChars.CrLf
					     + "     , NAME              " + ControlChars.CrLf
					     + "  from vwFIELD_VALIDATORS" + ControlChars.CrLf
					     + " order by NAME           " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								foreach ( DataRow row in dt.Rows )
								{
									Dictionary<string, object> drow = new Dictionary<string, object>();
									drow.Add("NAME"        , Sql.ToString(row["ID"  ]));
									drow.Add("LIST_NAME"   , "FIELD_VALIDATORS"       );
									drow.Add("MODULE_NAME" , null                     );
									drow.Add("DISPLAY_NAME", Sql.ToString(row["NAME"]));
									objs.Add(drow);
								}
							}
						}
					}
				}
				objs.Add(CreateGlobalTerm(L10n, "LBL_SELECT_BUTTON_LABEL"     ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_CLEAR_BUTTON_LABEL"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_SAVE_BUTTON_LABEL"       ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_CANCEL_BUTTON_LABEL"     ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_EXPORT_BUTTON_LABEL"     ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DEFAULTS_BUTTON_LABEL"   ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DELETE"                  ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ID"                      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DELETED"                 ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_CREATED_BY"              ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_CREATED_BY_ID"           ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_CREATED_BY_NAME"         ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DATE_ENTERED"            ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_MODIFIED_USER_ID"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DATE_MODIFIED"           ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_DATE_MODIFIED_UTC"       ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_MODIFIED_BY"             ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_MODIFIED_BY_ID"          ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_MODIFIED_BY_NAME"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_USER_ID"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_TO"             ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_TO_NAME"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_TEAM_ID"                 ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_TEAM_NAME"               ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_TEAM_SET_ID"             ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_TEAM_SET_NAME"           ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_TEAM_SET_LIST"           ));
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_SET_ID"         ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_SET_NAME"       ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ASSIGNED_SET_LIST"       ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ID_C"                    ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LAST_ACTIVITY_DATE"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_ID"                 ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_DELETED"            ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_CREATED_BY"         ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_CREATED_BY_ID"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_CREATED_BY_NAME"    ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_DATE_ENTERED"       ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_MODIFIED_USER_ID"   ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_DATE_MODIFIED"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_DATE_MODIFIED_UTC"  ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_MODIFIED_BY"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_MODIFIED_BY_ID"     ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_MODIFIED_BY_NAME"   ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_ASSIGNED_USER_ID"   ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_ASSIGNED_TO"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_ASSIGNED_TO_NAME"   ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TEAM_ID"            ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TEAM_NAME"          ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TEAM_SET_ID"        ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TEAM_SET_NAME"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TEAM_SET_LIST"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_ID_C"               ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_LAST_ACTIVITY_DATE" ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_ACTIVITY_STREAM"         ));
				// 05/13/2016 Paul.  LBL_TAG_SET_NAME should be global. 
				objs.Add(CreateGlobalTerm(L10n, "LBL_TAG_SET_NAME"            ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_TAG_SET_NAME"       ));
				// 08/20/2016 Paul.  PENDING_PROCESS_ID should be a global term. 
				objs.Add(CreateGlobalTerm(L10n, "LBL_PENDING_PROCESS_ID"      ));
				objs.Add(CreateGlobalTerm(L10n, "LBL_LIST_PENDING_PROCESS_ID" ));
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
			// 04/21/2017 Paul.  Count should be returend as a number. 
			d.Add("__count", objs.Count);
			return ToJsonStream(d);
		}

		private Dictionary<string, object> CreateGlobalTerm(L10N L10n, string sTerm)
		{
			Dictionary<string, object> drow = new Dictionary<string, object>();
			drow.Add("NAME"        , sTerm       );
			drow.Add("LIST_NAME"   , null        );
			drow.Add("MODULE_NAME" , String.Empty);
			drow.Add("DISPLAY_NAME", L10n.Term("." + sTerm));
			return drow;
		}

		public class RecompileStatus
		{
			public string   StartDate       ;
			public bool     Restart         ;
			public int      CurrentPass     ;
			public int      TotalPasses     ;
			public int      CurrentView     ;
			public int      TotalViews      ;
			public string   CurrentViewName ;
			public int      ElapseSeconds   ;
			public int      ViewsPerSecond  ;
			public int      RemainingSeconds;
		}

		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public RecompileStatus GetRecompileStatus()
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpApplicationState Application = HttpContext.Current.Application;
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			RecompileStatus oStatus = null;
			if ( !Sql.IsEmptyString(Application["System.Recompile.StartDate"]) )
			{
				oStatus = new RecompileStatus();
				oStatus.StartDate       = Sql.ToString  (Application["System.Recompile.StartDate"      ]);
				oStatus.Restart         = Sql.ToBoolean (Application["System.Recompile.Restart"        ]);
				oStatus.CurrentPass     = Sql.ToInteger (Application["System.Recompile.CurrentPass"    ]);
				oStatus.TotalPasses     = Sql.ToInteger (Application["System.Recompile.TotalPasses"    ]);
				oStatus.CurrentView     = Sql.ToInteger (Application["System.Recompile.CurrentView"    ]);
				oStatus.TotalViews      = Sql.ToInteger (Application["System.Recompile.TotalViews"     ]);
				oStatus.CurrentViewName = Sql.ToString  (Application["System.Recompile.CurrentViewName"]);
				
				DateTime dtStartDate = Sql.ToDateTime(Application["System.Recompile.StartDate"]);
				TimeSpan ts = DateTime.Now - dtStartDate;
				oStatus.ElapseSeconds = Convert.ToInt32(ts.TotalSeconds);
				if ( oStatus.ElapseSeconds > 0 )
				{
					oStatus.ViewsPerSecond   = ((oStatus.CurrentPass - 1) * oStatus.TotalViews + oStatus.CurrentView) / oStatus.ElapseSeconds;
					if ( oStatus.ViewsPerSecond > 0 )
						oStatus.RemainingSeconds = (oStatus.TotalPasses * oStatus.TotalViews - ((oStatus.CurrentPass - 1) * oStatus.TotalViews + oStatus.CurrentView)) / oStatus.ViewsPerSecond;
				}
			}
			return oStatus;
		}

		// 03/13/2016 Paul.  We need a special version that of the module get so that we can avoid any CRM caching. 
		[OperationContract]
		[WebInvoke(Method="GET", BodyStyle=WebMessageBodyStyle.WrappedRequest, RequestFormat=WebMessageFormat.Json, ResponseFormat=WebMessageFormat.Json)]
		public Stream GetAdminTable(string TableName)
		{
			HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
			WebOperationContext.Current.OutgoingResponse.Headers.Add("Pragma", "no-cache");
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			TableName = TableName.ToUpper();
			int    nSKIP     = Sql.ToInteger(Request.QueryString["$skip"   ]);
			int    nTOP      = Sql.ToInteger(Request.QueryString["$top"    ]);
			string sFILTER   = Sql.ToString (Request.QueryString["$filter" ]);
			string sORDER_BY = Sql.ToString (Request.QueryString["$orderby"]);
			// 06/17/2013 Paul.  Add support for GROUP BY. 
			string sGROUP_BY = Sql.ToString (Request.QueryString["$groupby"]);
			// 08/03/2011 Paul.  We need a way to filter the columns so that we can be efficient. 
			string sSELECT   = Sql.ToString (Request.QueryString["$select" ]);
			
			L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			DataTable dt = new DataTable();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL          = String.Empty;
				string sVIEW_NAME    = String.Empty;
				string sDEFAULT_VIEW = String.Empty;
				string sMATCH_NAME   = "DEFAULT_VIEW";
				Match match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
				if ( match.Success )
					sDEFAULT_VIEW = match.Groups[sMATCH_NAME].Value;
				if ( TableName == "DYNAMIC_BUTTONS" )
				{
					sMATCH_NAME = "VIEW_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                           " + ControlChars.CrLf
					     + "  from vwDYNAMIC_BUTTONS           " + ControlChars.CrLf
					     + " where VIEW_NAME    = @VIEW_NAME   " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = @DEFAULT_VIEW" + ControlChars.CrLf
					     + " order by CONTROL_INDEX            " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						Sql.AddParameter(cmd, "@DEFAULT_VIEW", Sql.ToBoolean(sDEFAULT_VIEW));
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "EDITVIEWS_FIELDS" )
				{
					sMATCH_NAME = "EDIT_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                           " + ControlChars.CrLf
					     + "  from vwEDITVIEWS_FIELDS          " + ControlChars.CrLf
					     + " where EDIT_NAME    = @VIEW_NAME   " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = @DEFAULT_VIEW" + ControlChars.CrLf
					     + " order by FIELD_INDEX              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						Sql.AddParameter(cmd, "@DEFAULT_VIEW", Sql.ToBoolean(sDEFAULT_VIEW));
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "DETAILVIEWS_FIELDS" )
				{
					sMATCH_NAME = "DETAIL_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                           " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS_FIELDS        " + ControlChars.CrLf
					     + " where DETAIL_NAME  = @VIEW_NAME   " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = @DEFAULT_VIEW" + ControlChars.CrLf
					     + " order by FIELD_INDEX              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						Sql.AddParameter(cmd, "@DEFAULT_VIEW", Sql.ToBoolean(sDEFAULT_VIEW));
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "GRIDVIEWS_COLUMNS" )
				{
					sMATCH_NAME = "GRID_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                           " + ControlChars.CrLf
					     + "  from vwGRIDVIEWS_COLUMNS         " + ControlChars.CrLf
					     + " where GRID_NAME    = @VIEW_NAME   " + ControlChars.CrLf
					     + "   and DEFAULT_VIEW = @DEFAULT_VIEW" + ControlChars.CrLf
					     + " order by COLUMN_INDEX             " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						Sql.AddParameter(cmd, "@DEFAULT_VIEW", Sql.ToBoolean(sDEFAULT_VIEW));
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "EDITVIEWS" )
				{
					sMATCH_NAME = "NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *            " + ControlChars.CrLf
					     + "  from vwEDITVIEWS  " + ControlChars.CrLf
					     + " where NAME = @NAME " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sVIEW_NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dt = new DataTable();
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "DETAILVIEWS" )
				{
					sMATCH_NAME = "NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *            " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS" + ControlChars.CrLf
					     + " where NAME = @NAME " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sVIEW_NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dt = new DataTable();
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "GRIDVIEWS" )
				{
					sMATCH_NAME = "NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *            " + ControlChars.CrLf
					     + "  from vwGRIDVIEWS  " + ControlChars.CrLf
					     + " where NAME = @NAME " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@NAME", sVIEW_NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dt = new DataTable();
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "EDITVIEWS_RELATIONSHIPS" )
				{
					sMATCH_NAME = "EDIT_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                                                       " + ControlChars.CrLf
					     + "  from vwEDITVIEWS_RELATIONSHIPS_Layout                        " + ControlChars.CrLf
					     + " where EDIT_NAME    = @VIEW_NAME                               " + ControlChars.CrLf
					     + " order by RELATIONSHIP_ENABLED, RELATIONSHIP_ORDER, MODULE_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else if ( TableName == "DETAILVIEWS_RELATIONSHIPS" )
				{
					sMATCH_NAME = "DETAIL_NAME";
					match = Regex.Match(sFILTER, "\\b" + sMATCH_NAME + "\\s*(=|eq)\\s*\'(?<" + sMATCH_NAME + ">([^(\'|\\s)]*))", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
					if ( match.Success )
						sVIEW_NAME = match.Groups[sMATCH_NAME].Value;
					sSQL = "select *                                                       " + ControlChars.CrLf
					     + "  from vwDETAILVIEWS_RELATIONSHIPS_La                          " + ControlChars.CrLf
					     + " where DETAIL_NAME  = @VIEW_NAME                               " + ControlChars.CrLf
					     + " order by RELATIONSHIP_ENABLED, RELATIONSHIP_ORDER, MODULE_NAME" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AddParameter(cmd, "@VIEW_NAME"   , sVIEW_NAME);
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							da.Fill(dt);
						}
					}
				}
				else
				{
					throw(new Exception("Unsupported table: " + TableName));
				}
			}
			
			string sBaseURI = Request.Url.Scheme + "://" + Request.Url.Host + Request.Url.AbsolutePath;
			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = int.MaxValue;
			
			Guid     gTIMEZONE         = Sql.ToGuid  (HttpContext.Current.Session["USER_SETTINGS/TIMEZONE"]);
			TimeZone T10n              = TimeZone.CreateTimeZone(gTIMEZONE);
			string sResponse = json.Serialize(SplendidCRM.Rest.ToJson(sBaseURI, String.Empty, dt, T10n));
			byte[] byResponse = Encoding.UTF8.GetBytes(sResponse);
			return new MemoryStream(byResponse);
		}

		#endregion

		#region Update
		[OperationContract]
		public void UpdateAdminLayout(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			string sTableName = Sql.ToString(Request.QueryString["TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("The table name must be specified."));
			
			// 02/20/2016 Paul.  Module name is included in ViewName, or is blank when updating globals. 
			string sViewName = Sql.ToString(Request.QueryString["ViewName"]);
			if ( Sql.IsEmptyString(sViewName) && sTableName != "TERMINOLOGY" )
				throw(new Exception("The layout view name must be specified."));
			
			string sRequest = String.Empty;
			using ( StreamReader stmRequest = new StreamReader(input, System.Text.Encoding.UTF8) )
			{
				sRequest = stmRequest.ReadToEnd();
			}
			JavaScriptSerializer json = new JavaScriptSerializer();
			json.MaxJsonLength = int.MaxValue;
			Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(sRequest);
			
			switch ( sTableName )
			{
				case "EDITVIEWS_FIELDS"         :  UpdateAdminLayoutTable("EDITVIEWS"  , "EDITVIEWS_FIELDS"         , "EDIT_NAME"  , "FIELD_INDEX"       , "FIELD_TYPE" , true , sViewName, dict);  SplendidCache.ClearEditView  (sViewName);  break;
				case "DETAILVIEWS_FIELDS"       :  UpdateAdminLayoutTable("DETAILVIEWS", "DETAILVIEWS_FIELDS"       , "DETAIL_NAME", "FIELD_INDEX"       , "FIELD_TYPE" , true , sViewName, dict);  SplendidCache.ClearDetailView(sViewName);  break;
				case "GRIDVIEWS_COLUMNS"        :  UpdateAdminLayoutTable("GRIDVIEWS"  , "GRIDVIEWS_COLUMNS"        , "GRID_NAME"  , "COLUMN_INDEX"      , "COLUMN_TYPE", false, sViewName, dict);  SplendidCache.ClearGridView  (sViewName);  break;
				case "DETAILVIEWS_RELATIONSHIPS":  UpdateAdminTable("DETAILVIEWS_RELATIONSHIPS", "DETAIL_NAME", sViewName, dict);  SplendidCache.ClearDetailViewRelationships(sViewName);  break;
				case "EDITVIEWS_RELATIONSHIPS"  :  UpdateAdminTable("EDITVIEWS_RELATIONSHIPS"  , "EDIT_NAME"  , sViewName, dict);  SplendidCache.ClearEditViewRelationships  (sViewName);  break;
				case "TERMINOLOGY"              :  UpdateAdminTable("TERMINOLOGY"              , "MODULE_NAME", sViewName, dict);  ReloadTerminology(HttpContext.Current, sViewName);  break;
				default:  throw(new Exception("Unsupported table: " + sTableName));
			}
		}

		private void ReloadTerminology(HttpContext Context, string sMODULE_NAME)
		{
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory(Context.Application);
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select NAME                " + ControlChars.CrLf
					     + "     , LANG                " + ControlChars.CrLf
					     + "     , MODULE_NAME         " + ControlChars.CrLf
					     + "     , DISPLAY_NAME        " + ControlChars.CrLf
					     + "  from vwTERMINOLOGY       " + ControlChars.CrLf
					     + " where LIST_NAME is null   " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( Sql.IsEmptyString(sMODULE_NAME) )
							sSQL += "   and MODULE_NAME is null " + ControlChars.CrLf;
						else
							Sql.AppendParameter(cmd, sMODULE_NAME, "MODULE_NAME");
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							while ( rdr.Read() )
							{
								string sLANG         = Sql.ToString(rdr["LANG"        ]);
								string sNAME         = Sql.ToString(rdr["NAME"        ]);
								string sDISPLAY_NAME = Sql.ToString(rdr["DISPLAY_NAME"]);
								L10N.SetTerm(Context.Application, sLANG, sMODULE_NAME, sNAME, sDISPLAY_NAME);
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemMessage(Context, "Error", new StackTrace(true).GetFrame(0), ex);
				//HttpContext.Current.Response.Write(ex.Message);
			}
		}

		private void ClearLayoutTable(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sVIEW_NAME)
		{
			IDbConnection con = trn.Connection;
			IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
			cmdDelete.Transaction = trn;
			Sql.SetParameter(cmdDelete, "MODIFIED_USER_ID", Security.USER_ID);
			
			string sSQL = String.Empty;
			sSQL = "select ID"                    + ControlChars.CrLf
			     + "  from vw" + sTABLE_NAME      + ControlChars.CrLf
			     + " where " + sLAYOUT_NAME_FIELD + " = @" + sLAYOUT_NAME_FIELD + ControlChars.CrLf
			     + "   and DEFAULT_VIEW = 0"      + ControlChars.CrLf;
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = sSQL;
				cmd.Transaction = trn;
				Sql.AddParameter(cmd, "@" + sLAYOUT_NAME_FIELD, sVIEW_NAME);
				using ( DbDataAdapter da = dbf.CreateDataAdapter() )
				{
					((IDbDataAdapter)da).SelectCommand = cmd;
					using ( DataTable dt = new DataTable() )
					{
						da.Fill(dt);
						foreach ( DataRow row in dt.Rows )
						{
							Guid gID = Sql.ToGuid(row["ID"]);
							Sql.SetParameter(cmdDelete, "ID", gID);
							cmdDelete.ExecuteNonQuery();
						}
					}
				}
			}
		}

		private void UpdateLayoutEvents(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sVIEW_NAME, Dictionary<string, object> dict)
		{
			if ( dict.ContainsKey(sTABLE_NAME) )
			{
				IDbConnection con = trn.Connection;
				IDbCommand cmdUpdateEvents = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_UpdateEvents");
				cmdUpdateEvents.Transaction = trn;
				
				Sql.SetParameter(cmdUpdateEvents, "NAME", sVIEW_NAME);
				Sql.SetParameter(cmdUpdateEvents, "MODIFIED_USER_ID", Security.USER_ID);
				
				Dictionary<string, object> dictTable = dict[sTABLE_NAME] as Dictionary<string, object>;
				foreach ( string sFieldName in dictTable.Keys )
				{
					if ( sFieldName != "NAME" && sFieldName != "MODIFIED_USER_ID" )
					{
						IDbDataParameter par = Sql.FindParameter(cmdUpdateEvents, sFieldName);
						if ( par != null )
						{
							switch ( par.DbType )
							{
								case DbType.Guid    :  par.Value = Sql.ToDBGuid    (dictTable[sFieldName]);  break;
								case DbType.Int16   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Int32   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Int64   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Double  :  par.Value = Sql.ToDBFloat   (dictTable[sFieldName]);  break;
								case DbType.Decimal :  par.Value = Sql.ToDBDecimal (dictTable[sFieldName]);  break;
								case DbType.Byte    :  par.Value = Sql.ToDBBoolean (dictTable[sFieldName]);  break;
								case DbType.DateTime:  par.Value = Sql.ToDBDateTime(dictTable[sFieldName]);  break;
								default             :  par.Value = Sql.ToDBString  (dictTable[sFieldName]);  break;
							}
						}
					}
				}
				cmdUpdateEvents.ExecuteNonQuery();
			}
		}

		// 05/04/2016 Paul.  A copied view will need the root EDITVIEWS, DETAILVIEWS or GRIDVIEWS record. 
		private void CreateParentTable(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sVIEW_NAME, Dictionary<string, object> dict)
		{
			if ( dict.ContainsKey(sTABLE_NAME) )
			{
				IDbConnection con = trn.Connection;
				IDbCommand cmdInsertOnly = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_InsertOnly");
				cmdInsertOnly.Transaction = trn;
				Sql.SetParameter(cmdInsertOnly, "NAME", sVIEW_NAME);
				Sql.SetParameter(cmdInsertOnly, "MODIFIED_USER_ID", Security.USER_ID);
				
				Dictionary<string, object> dictTable = dict[sTABLE_NAME] as Dictionary<string, object>;
				foreach ( string sFieldName in dictTable.Keys )
				{
					if ( sFieldName != "NAME" && sFieldName != "MODIFIED_USER_ID" )
					{
						IDbDataParameter par = Sql.FindParameter(cmdInsertOnly, sFieldName);
						if ( par != null )
						{
							switch ( par.DbType )
							{
								case DbType.Guid    :  par.Value = Sql.ToDBGuid    (dictTable[sFieldName]);  break;
								case DbType.Int16   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Int32   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Int64   :  par.Value = Sql.ToDBInteger (dictTable[sFieldName]);  break;
								case DbType.Double  :  par.Value = Sql.ToDBFloat   (dictTable[sFieldName]);  break;
								case DbType.Decimal :  par.Value = Sql.ToDBDecimal (dictTable[sFieldName]);  break;
								case DbType.Byte    :  par.Value = Sql.ToDBBoolean (dictTable[sFieldName]);  break;
								case DbType.DateTime:  par.Value = Sql.ToDBDateTime(dictTable[sFieldName]);  break;
								default             :  par.Value = Sql.ToDBString  (dictTable[sFieldName]);  break;
							}
						}
					}
				}
				cmdInsertOnly.ExecuteNonQuery();
			}
		}

		private void UpdateLayoutTable(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sLAYOUT_INDEX_FIELD, string sVIEW_NAME, Dictionary<string, object> dict)
		{
			if ( dict.ContainsKey(sTABLE_NAME) )
			{
				IDbConnection con = trn.Connection;
				IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
				cmdUpdate.Transaction = trn;
				IDbDataParameter parMODIFIED_USER_ID = Sql.FindParameter(cmdUpdate, "@MODIFIED_USER_ID");
				
				System.Collections.ArrayList lst = dict[sTABLE_NAME] as System.Collections.ArrayList;
				for ( int i = 0; i < lst.Count; i++ )
				{
					foreach(IDbDataParameter par in cmdUpdate.Parameters)
					{
						par.Value = DBNull.Value;
					}
					if ( parMODIFIED_USER_ID != null )
						parMODIFIED_USER_ID.Value = Security.USER_ID;
					
					Dictionary<string, object> dictRow = lst[i] as Dictionary<string, object>;
					Sql.SetParameter(cmdUpdate, "ID"               , Guid.Empty);
					Sql.SetParameter(cmdUpdate, sLAYOUT_NAME_FIELD , sVIEW_NAME);
					foreach ( string sFieldName in dictRow.Keys )
					{
						if ( sFieldName != sLAYOUT_NAME_FIELD && sFieldName != "ID" && sFieldName != "MODIFIED_USER_ID" )
						{
							IDbDataParameter par = Sql.FindParameter(cmdUpdate, sFieldName);
							if ( par != null )
							{
								switch ( par.DbType )
								{
									case DbType.Guid    :  par.Value = Sql.ToDBGuid    (dictRow[sFieldName]);  break;
									case DbType.Int16   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
									case DbType.Int32   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
									case DbType.Int64   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
									case DbType.Double  :  par.Value = Sql.ToDBFloat   (dictRow[sFieldName]);  break;
									case DbType.Decimal :  par.Value = Sql.ToDBDecimal (dictRow[sFieldName]);  break;
									case DbType.Byte    :  par.Value = Sql.ToDBBoolean (dictRow[sFieldName]);  break;
									case DbType.DateTime:  par.Value = Sql.ToDBDateTime(dictRow[sFieldName]);  break;
									default             :  par.Value = Sql.ToDBString  (dictRow[sFieldName]);  break;
								}
							}
						}
					}
					cmdUpdate.ExecuteNonQuery();
				}
			}
		}

		private void CheckDuplicates(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sLAYOUT_TYPE_FIELD, string sVIEW_NAME)
		{
			IDbConnection con = trn.Connection;

			string sSQL = String.Empty;
			sSQL = "select DATA_FIELD"                                          + ControlChars.CrLf
			     + "  from vw" + sTABLE_NAME                                    + ControlChars.CrLf
			     + " where DATA_FIELD is not null"                              + ControlChars.CrLf
			     + "   and " + sLAYOUT_NAME_FIELD + " = @" + sLAYOUT_NAME_FIELD + ControlChars.CrLf
			     + "   and " + sLAYOUT_TYPE_FIELD + " <> 'JavaScript'"          + ControlChars.CrLf
			     + "   and DEFAULT_VIEW = 0"                                    + ControlChars.CrLf
			     + " group by DATA_FIELD"                                       + ControlChars.CrLf
			     + " having count(*) > 1"                                       + ControlChars.CrLf
			     + " order by DATA_FIELD"                                       + ControlChars.CrLf;
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = sSQL;
				cmd.Transaction = trn;
				Sql.AddParameter(cmd, "@" + sLAYOUT_NAME_FIELD, sVIEW_NAME);
				using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
				{
					StringBuilder sbDuplicateFields = new StringBuilder();
					while ( rdr.Read() )
					{
						if ( sbDuplicateFields.Length > 0 )
							sbDuplicateFields.Append(", ");
						sbDuplicateFields.Append(Sql.ToString(rdr["DATA_FIELD"]));
					}
					if ( sbDuplicateFields.Length > 0 )
					{
						throw(new Exception("Duplicate fields: " + sbDuplicateFields.ToString()));
					}
				}
			}
		}

		private void UpdateAdminLayoutTable(string sPARENT_TABLE, string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sLAYOUT_INDEX_FIELD, string sLAYOUT_TYPE_FIELD, bool bCheckDuplicates, string sVIEW_NAME, Dictionary<string, object> dict)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							ClearLayoutTable  (dbf, trn, sTABLE_NAME  , sLAYOUT_NAME_FIELD, sVIEW_NAME);
							// 05/04/2016 Paul.  A copied view will need the root EDITVIEWS, DETAILVIEWS or GRIDVIEWS record. 
							CreateParentTable (dbf, trn, sPARENT_TABLE  , sVIEW_NAME, dict);
							UpdateLayoutTable (dbf, trn, sTABLE_NAME  , sLAYOUT_NAME_FIELD, sLAYOUT_INDEX_FIELD, sVIEW_NAME, dict);
							if ( !Sql.IsEmptyString(sPARENT_TABLE) )
								UpdateLayoutEvents(dbf, trn, sPARENT_TABLE, sVIEW_NAME, dict);
							if ( bCheckDuplicates )
								CheckDuplicates(dbf, trn, sTABLE_NAME, sLAYOUT_NAME_FIELD, sLAYOUT_TYPE_FIELD, sVIEW_NAME);
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception("Failed to update, transaction aborted; " + ex.Message, ex));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}
		#endregion

		#region Delete
		[OperationContract]
		public void DeleteAdminLayout(Stream input)
		{
			HttpApplicationState Application = HttpContext.Current.Application;
			HttpRequest          Request     = HttpContext.Current.Request    ;
			
			if ( !Security.IsAuthenticated() || !Security.IS_ADMIN )
			{
				L10N L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				throw(new Exception(L10n.Term("ACL.LBL_INSUFFICIENT_ACCESS")));
			}
			SplendidSession.CreateSession(HttpContext.Current.Session);
			
			string sTableName = Sql.ToString(Request.QueryString["TableName"]);
			if ( Sql.IsEmptyString(sTableName) )
				throw(new Exception("The table name must be specified."));
			
			// 02/20/2016 Paul.  Module name is included in ViewName, or is blank when updating globals. 
			string sViewName = Sql.ToString(Request.QueryString["ViewName"]);
			if ( Sql.IsEmptyString(sViewName) && sTableName != "TERMINOLOGY" )
				throw(new Exception("The layout view name must be specified."));
			
			switch ( sTableName )
			{
				case "EDITVIEWS_FIELDS"         :  DeleteAdminLayoutTable("EDITVIEWS"  , "EDITVIEWS_FIELDS"  , "EDIT_NAME"  , sViewName);  SplendidCache.ClearEditView  (sViewName);  break;
				case "DETAILVIEWS_FIELDS"       :  DeleteAdminLayoutTable("DETAILVIEWS", "DETAILVIEWS_FIELDS", "DETAIL_NAME", sViewName);  SplendidCache.ClearDetailView(sViewName);  break;
				case "GRIDVIEWS_COLUMNS"        :  DeleteAdminLayoutTable("GRIDVIEWS"  , "GRIDVIEWS_COLUMNS" , "GRID_NAME"  , sViewName);  SplendidCache.ClearGridView  (sViewName);  break;
				default:  throw(new Exception("Unsupported table: " + sTableName));
			}
		}

		private void DeleteParentTable(DbProviderFactory dbf, IDbTransaction trn, string sTABLE_NAME, string sLAYOUT_NAME)
		{
			IDbConnection con = trn.Connection;
			IDbCommand cmdDelete = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Delete");
			cmdDelete.Transaction = trn;
			Sql.SetParameter(cmdDelete, "MODIFIED_USER_ID", Security.USER_ID);
			
			string sSQL = String.Empty;
			sSQL = "select ID"               + ControlChars.CrLf
			     + "  from vw" + sTABLE_NAME + ControlChars.CrLf
			     + " where NAME = @NAME"     + ControlChars.CrLf;
			using ( IDbCommand cmd = con.CreateCommand() )
			{
				cmd.CommandText = sSQL;
				cmd.Transaction = trn;
				Sql.AddParameter(cmd, "@NAME", sLAYOUT_NAME);
				using ( DbDataAdapter da = dbf.CreateDataAdapter() )
				{
					((IDbDataAdapter)da).SelectCommand = cmd;
					using ( DataTable dt = new DataTable() )
					{
						da.Fill(dt);
						foreach ( DataRow row in dt.Rows )
						{
							Guid gID = Sql.ToGuid(row["ID"]);
							Sql.SetParameter(cmdDelete, "ID", gID);
							cmdDelete.ExecuteNonQuery();
						}
					}
				}
			}
		}

		private void DeleteAdminLayoutTable(string sPARENT_TABLE, string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sVIEW_NAME)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							ClearLayoutTable(dbf, trn, sTABLE_NAME, sLAYOUT_NAME_FIELD, sVIEW_NAME);
							DeleteParentTable(dbf, trn, sPARENT_TABLE, sVIEW_NAME);
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception("Failed to update, transaction aborted; " + ex.Message, ex));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}

		private void UpdateAdminTable(string sTABLE_NAME, string sLAYOUT_NAME_FIELD, string sVIEW_NAME, Dictionary<string, object> dict)
		{
			HttpSessionState Session = HttpContext.Current.Session;
			try
			{
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							if ( dict.ContainsKey(sTABLE_NAME) )
							{
								IDbCommand cmdUpdate = SqlProcs.Factory(con, "sp" + sTABLE_NAME + "_Update");
								cmdUpdate.Transaction = trn;
								IDbDataParameter parMODIFIED_USER_ID = Sql.FindParameter(cmdUpdate, "@MODIFIED_USER_ID");
								
								System.Collections.ArrayList lst = dict[sTABLE_NAME] as System.Collections.ArrayList;
								for ( int i = 0; i < lst.Count; i++ )
								{
									foreach(IDbDataParameter par in cmdUpdate.Parameters)
									{
										par.Value = DBNull.Value;
									}
									if ( parMODIFIED_USER_ID != null )
										parMODIFIED_USER_ID.Value = Security.USER_ID;
									
									Dictionary<string, object> dictRow = lst[i] as Dictionary<string, object>;
									Sql.SetParameter(cmdUpdate, sLAYOUT_NAME_FIELD , sVIEW_NAME);
									foreach ( string sFieldName in dictRow.Keys )
									{
										if ( sFieldName != sLAYOUT_NAME_FIELD && sFieldName != "MODIFIED_USER_ID" )
										{
											IDbDataParameter par = Sql.FindParameter(cmdUpdate, sFieldName);
											if ( par != null )
											{
												switch ( par.DbType )
												{
													case DbType.Guid    :  par.Value = Sql.ToDBGuid    (dictRow[sFieldName]);  break;
													case DbType.Int16   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
													case DbType.Int32   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
													case DbType.Int64   :  par.Value = Sql.ToDBInteger (dictRow[sFieldName]);  break;
													case DbType.Double  :  par.Value = Sql.ToDBFloat   (dictRow[sFieldName]);  break;
													case DbType.Decimal :  par.Value = Sql.ToDBDecimal (dictRow[sFieldName]);  break;
													case DbType.Byte    :  par.Value = Sql.ToDBBoolean (dictRow[sFieldName]);  break;
													case DbType.DateTime:  par.Value = Sql.ToDBDateTime(dictRow[sFieldName]);  break;
													default             :  par.Value = Sql.ToDBString  (dictRow[sFieldName]);  break;
												}
											}
										}
									}
									cmdUpdate.ExecuteNonQuery();
								}
							}
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							throw(new Exception("Failed to update, transaction aborted; " + ex.Message, ex));
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				throw;
			}
		}
		#endregion
	}
}

