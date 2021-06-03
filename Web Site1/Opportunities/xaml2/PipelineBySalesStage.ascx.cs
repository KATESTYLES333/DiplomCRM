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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace SplendidCRM.Opportunities.xaml2
{
	/// <summary>
	///		Summary description for PipelineBySalesStage.
	/// </summary>
	public class PipelineBySalesStage : SplendidControl
	{
		protected XmlDocument xml = new XmlDocument();
		protected DataSet     dsMain = new DataSet();
		protected int nCHART_LENGTH = 4;

		public int CHART_LENGTH
		{
			get { return nCHART_LENGTH; }
			set { nCHART_LENGTH = value; }
		}

		#region Properties used when rendering.
		protected int       nGridWidth  = 400;
		protected int       nGridHeight = 226;

		protected string    sGraphData_Title                 ;
		protected string    sGraphData_GraphInfo             ;
		protected Hashtable hashColorLegend                  ;

		protected string    sAxis_yData_defaultAltText       ;

		protected int       nAxis_xData_min                  ;
		protected int       nAxis_xData_max                  ;
		protected int       nAxis_xData_length               = 1;
		protected double    dAxis_xData_section              ;
		protected string    sAxis_xData_prefix               ;
		protected string    sAxis_xData_suffix               ;

		// 09/26/2009 Paul.  Temporarily stop using the SugarCRM color scheme and instead use the default Silverlight Charts color scheme. 
		/*
		protected string    sChartColors_docBorder           ;
		protected string    sChartColors_docBg1              ;
		protected string    sChartColors_docBg2              ;
		protected string    sChartColors_xText               ;
		protected string    sChartColors_yText               ;
		protected string    sChartColors_title               ;
		protected string    sChartColors_misc                ;
		protected string    sChartColors_altBorder           ;
		protected string    sChartColors_altBg               ;
		protected string    sChartColors_altText             ;
		protected string    sChartColors_graphBorder         ;
		protected string    sChartColors_graphBg1            ;
		protected string    sChartColors_graphBg2            ;
		protected string    sChartColors_graphLines          ;
		protected string    sChartColors_graphText           ;
		protected string    sChartColors_graphTextShadow     ;
		protected string    sChartColors_barBorder           ;
		protected string    sChartColors_barBorderHilite     ;
		protected string    sChartColors_legendBorder        ;
		protected string    sChartColors_legendBg1           ;
		protected string    sChartColors_legendBg2           ;
		protected string    sChartColors_legendText          ;
		protected string    sChartColors_legendColorKeyBorder;
		protected string    sChartColors_scrollBar           ;
		protected string    sChartColors_scrollBarBorder     ;
		protected string    sChartColors_scrollBarTrack      ;
		protected string    sChartColors_scrollBarTrackBorder;
		*/
		protected XmlNodeList nlDataRows;
		protected XmlNode     nodeRoot  ;
		protected XmlNode     nodeYData ;
		protected XmlNode     nodeXData ;
		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 09/21/2008 Paul.  Mono does not support Silverlight inline XAML at this time. 
			// 09/22/2008 Paul.  The Mono exception code was moved to enable_silverlight(). 
			if ( !Crm.Config.enable_silverlight() )
			{
				this.Visible = false;
				return;
			}
			// 09/11/2008 Paul.  Silverlight requires that all numeric values need to be in US format. 
			// Reset the culture before binding/rendering so that all the numeric conversions will be automatic. 
			// All currencies should already be converted to text, so there should not be an impact. 
			// 01/11/2009 Paul.  We need to save and restore the previous culture as there are other controls on the page. 
			// 10/13/2010 Paul.  Move the culture fix higher up so that all numeric ToString() operations are in English. 
			System.Globalization.CultureInfo ciCurrent = System.Threading.Thread.CurrentThread.CurrentCulture;
			try
			{
				System.Globalization.CultureInfo ciEnglish = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
				ciEnglish.DateTimeFormat.ShortDatePattern = ciCurrent.DateTimeFormat.ShortDatePattern;
				ciEnglish.DateTimeFormat.ShortTimePattern = ciCurrent.DateTimeFormat.ShortTimePattern;
				ciEnglish.NumberFormat.CurrencySymbol     = ciCurrent.NumberFormat.CurrencySymbol;
				System.Threading.Thread.CurrentThread.CurrentCulture   = ciEnglish;
				System.Threading.Thread.CurrentThread.CurrentUICulture = ciEnglish;
				
				_controls.ChartDatePicker ctlDATE_START = Parent.FindControl("ctlDATE_START") as _controls.ChartDatePicker;
				_controls.ChartDatePicker ctlDATE_END   = Parent.FindControl("ctlDATE_END"  ) as _controls.ChartDatePicker;
				ListBox lstSALES_STAGE      = Parent.FindControl("lstSALES_STAGE"     ) as ListBox;
				ListBox lstASSIGNED_USER_ID = Parent.FindControl("lstASSIGNED_USER_ID") as ListBox;

				// 09/15/2005 Paul.  Values will always be in the query string. 
				DateTime dtDATE_START  = (ctlDATE_START != null) ? ctlDATE_START.Value : DateTime.MinValue;
				DateTime dtDATE_END    = (ctlDATE_END   != null) ? ctlDATE_END.Value   : DateTime.MinValue;
				if ( dtDATE_START == DateTime.MinValue )
				{
					// 09/14/2005 Paul.  SugarCRM uses a max date of 01/01/2100. 
					dtDATE_START = DateTime.Today;
				}
				if ( dtDATE_END == DateTime.MinValue )
				{
					// 09/14/2005 Paul.  SugarCRM uses a max date of 01/01/2100. 
					dtDATE_END = new DateTime(2100, 1, 1);
				}

				string[] arrASSIGNED_USER_ID = (lstASSIGNED_USER_ID != null) ? Sql.ToStringArray(lstASSIGNED_USER_ID) : new string[] { Security.USER_ID.ToString() };
				string[] arrSALES_STAGE      = (lstSALES_STAGE      != null) ? Sql.ToStringArray(lstSALES_STAGE     ) : null;

				// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
				// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
				// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
				xml.XmlResolver = null;
				xml.LoadXml(SplendidCache.XmlFile(Server.MapPath(Session["themeURL"] + "BarChart.xml")));
				nodeRoot        = xml.SelectSingleNode("graphData");
				nodeXData       = xml.CreateElement("xData"      );
				nodeYData       = xml.CreateElement("yData"      );
				XmlNode nodeColorLegend = xml.CreateElement("colorLegend");
				XmlNode nodeGraphInfo   = xml.CreateElement("graphInfo"  );
				XmlNode nodeChartColors = nodeRoot.SelectSingleNode("chartColors");

				nodeRoot.InsertBefore(nodeGraphInfo  , nodeChartColors);
				nodeRoot.InsertBefore(nodeColorLegend, nodeGraphInfo  );
				nodeRoot.InsertBefore(nodeXData      , nodeColorLegend);
				nodeRoot.InsertBefore(nodeYData      , nodeXData      );
				
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "defaultAltText", L10n.Term("Dashboard.LBL_ROLLOVER_DETAILS"));
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "min", "0");
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max", "0");
				if ( nCHART_LENGTH < 4 )
					nCHART_LENGTH = 4;
				else if ( nCHART_LENGTH > 10 )
					nCHART_LENGTH = 10;
				
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "length", nCHART_LENGTH.ToString());
				System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
				// 03/07/2008 Paul.  Use CurrencyPositivePattern to determine location of the CurrencySymbol. 
				switch ( culture.NumberFormat.CurrencyPositivePattern )
				{
					case 0:  // $n
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", culture.NumberFormat.CurrencySymbol);
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", "");
						break;
					case 1:  // n$
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", "");
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", culture.NumberFormat.CurrencySymbol);
						break;
					case 2:  // $ n
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", culture.NumberFormat.CurrencySymbol + " ");
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", "");
						break;
					case 3:  // n $
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", "");
						XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", " " + culture.NumberFormat.CurrencySymbol);
						break;
				}
				
				// 01/31/2008 Paul.  Add space after TO. 
				nodeGraphInfo.InnerText = L10n.Term("Dashboard.LBL_DATE_RANGE") + " " + Sql.ToDateString(T10n.FromServerTime(dtDATE_START)) + " " + L10n.Term("Dashboard.LBL_DATE_RANGE_TO") + " " + Sql.ToDateString(T10n.FromServerTime(dtDATE_END)) + "<BR/>"
				                        + L10n.Term("Dashboard.LBL_OPP_SIZE"  ) + " " + 1.ToString("c0") + L10n.Term("Dashboard.LBL_OPP_THOUSANDS");
				
				Hashtable hashUSER = new Hashtable();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					List<String> arrUSERS = new List<String>();
					// 09/19/2005 Paul.  Prepopulate the user key with all the users specified. 
					// 09/26/2009 Paul.  The AppendGuids function will protected against an empty list. 
					// We want to keep the empty list as we still need to pre-populate the users. 
					//if ( arrASSIGNED_USER_ID != null && arrASSIGNED_USER_ID.Length > 0 )
					{
						sSQL = "select ID          " + ControlChars.CrLf
						     + "     , USER_NAME   " + ControlChars.CrLf
						     + "  from vwUSERS_List" + ControlChars.CrLf
						     + " where 1 = 1       " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Sql.AppendGuids(cmd, arrASSIGNED_USER_ID, "ID");
							cmd.CommandText += " order by USER_NAME" + ControlChars.CrLf;
							using ( IDataReader rdr = cmd.ExecuteReader() )
							{
								while ( rdr.Read() )
								{
									Guid   gUSER_ID   = Sql.ToGuid   (rdr["ID"       ]);
									string sUSER_NAME = Sql.ToString (rdr["USER_NAME"]);
									if ( !hashUSER.ContainsKey(gUSER_ID.ToString()) )
									{
										XmlNode nodeMapping = xml.CreateElement("mapping");
										nodeColorLegend.AppendChild(nodeMapping);
										XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "id"   , gUSER_ID.ToString());
										XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , sUSER_NAME);
										XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "color", SplendidDefaults.generate_graphcolor(gUSER_ID.ToString(), hashUSER.Count));
										hashUSER.Add(gUSER_ID.ToString(), sUSER_NAME);
										// 09/26/2009 Paul.  Reverse the insertion order. 
										arrUSERS.Insert(0, gUSER_ID.ToString());
									}
								}
							}
						}
					}
					// 09/26/2009 Paul.  We need to prepopulate the sales stage, so if none are provided, then build the set in advance. 
					if ( arrSALES_STAGE == null || arrSALES_STAGE.Length == 0 )
					{
						sSQL = "select distinct                                      " + ControlChars.CrLf
						     + "       SALES_STAGE                                   " + ControlChars.CrLf
						     + "     , LIST_ORDER                                    " + ControlChars.CrLf
						     + "  from vwOPPORTUNITIES_Pipeline                      " + ControlChars.CrLf;
						using ( IDbCommand cmd = con.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Security.Filter(cmd, "Opportunities", "list");
							cmd.CommandText += "   and DATE_CLOSED >= @DATE_START" + ControlChars.CrLf;
							cmd.CommandText += "   and DATE_CLOSED <= @DATE_END  " + ControlChars.CrLf;
							// 09/14/2005 Paul.  Use add because <= and >= are not supported. 
							Sql.AddParameter   (cmd, "@DATE_START"      , dtDATE_START      );
							Sql.AddParameter   (cmd, "@DATE_END"        , dtDATE_END        );
							// 09/14/2005 Paul.  Use append because it supports arrays using the IN clause. 
							Sql.AppendGuids    (cmd, arrASSIGNED_USER_ID, "ASSIGNED_USER_ID");
							Sql.AppendParameter(cmd, arrSALES_STAGE     , "SALES_STAGE"     );
							
							cmd.CommandText += " order by LIST_ORDER                                 " + ControlChars.CrLf;
							using ( DbDataAdapter da = dbf.CreateDataAdapter() )
							{
								((IDbDataAdapter)da).SelectCommand = cmd;
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									arrSALES_STAGE = new String[dt.Rows.Count];
									for ( int i = 0; i < dt.Rows.Count; i++ )
									{
										DataRow row = dt.Rows[i];
										arrSALES_STAGE[i] = Sql.ToString(row["SALES_STAGE"]);
									}
								}
							}
						}
					}

					// 09/19/2005 Paul.  Prepopulate the stage rows so that empty rows will appear.  The SQL query will not return empty rows. 
					if ( arrSALES_STAGE != null )
					{
						XmlNode nodeRowPrevious = null;
						foreach(string sSALES_STAGE in arrSALES_STAGE)
						{
							XmlNode nodeRow = xml.CreateElement("dataRow");
							// 09/26/2009 Paul.  Reverse the order of the items. 
							if ( nodeRowPrevious == null )
								nodeYData.AppendChild(nodeRow);
							else
								nodeYData.InsertBefore(nodeRow, nodeRowPrevious);
							nodeRowPrevious = nodeRow;
							// 09/26/2009 Paul.  LBL_NONE is --None--, so create a new term LBL_NONE_VALUE.
							if ( sSALES_STAGE == String.Empty )
								XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , Sql.ToString(L10n.Term(".LBL_NONE_VALUE")));
							else
								XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , Sql.ToString(L10n.Term(".sales_stage_dom.", sSALES_STAGE)));
							XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", "0");
							// 09/26/2009 Paul.  Prepopulate the bars as the StackedColumnSeries requires all values. 
							foreach(string sUSERS in arrUSERS)
							{
								XmlNode nodeBar = xml.CreateElement("bar");
								nodeRow.AppendChild(nodeBar);
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "id", sUSERS);
							}
						}
					}
					sSQL = "select SALES_STAGE                                   " + ControlChars.CrLf
					     + "     , ASSIGNED_USER_ID                              " + ControlChars.CrLf
					     + "     , USER_NAME                                     " + ControlChars.CrLf
					     + "     , LIST_ORDER                                    " + ControlChars.CrLf
					     + "     , sum(AMOUNT_USDOLLAR/1000) as TOTAL            " + ControlChars.CrLf
					     + "     , count(*)                  as OPPORTUNITY_COUNT" + ControlChars.CrLf
					     + "  from vwOPPORTUNITIES_Pipeline                      " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Opportunities", "list");
						cmd.CommandText += "   and DATE_CLOSED >= @DATE_START" + ControlChars.CrLf;
						cmd.CommandText += "   and DATE_CLOSED <= @DATE_END  " + ControlChars.CrLf;
						// 09/14/2005 Paul.  Use add because <= and >= are not supported. 
						Sql.AddParameter   (cmd, "@DATE_START"      , dtDATE_START      );
						Sql.AddParameter   (cmd, "@DATE_END"        , dtDATE_END        );
						// 09/14/2005 Paul.  Use append because it supports arrays using the IN clause. 
						Sql.AppendGuids    (cmd, arrASSIGNED_USER_ID, "ASSIGNED_USER_ID");
						Sql.AppendParameter(cmd, arrSALES_STAGE     , "SALES_STAGE"     );
						
						cmd.CommandText += ""
						     + " group by SALES_STAGE                                " + ControlChars.CrLf
						     + "        , LIST_ORDER                                 " + ControlChars.CrLf
						     + "        , ASSIGNED_USER_ID                           " + ControlChars.CrLf
						     + "        , USER_NAME                                  " + ControlChars.CrLf
						     + " order by LIST_ORDER                                 " + ControlChars.CrLf
						     + "        , USER_NAME                                  " + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							double dMAX_TOTAL      = 0;
							double dPIPELINE_TOTAL = 0;
							while ( rdr.Read() )
							{
								string  sSALES_STAGE       = Sql.ToString (rdr["SALES_STAGE"      ]);
								double  dTOTAL             = Sql.ToDouble (rdr["TOTAL"            ]);
								int     nOPPORTUNITY_COUNT = Sql.ToInteger(rdr["OPPORTUNITY_COUNT"]);
								Guid    gASSIGNED_USER_ID  = Sql.ToGuid   (rdr["ASSIGNED_USER_ID" ]);
								string  sUSER_NAME         = Sql.ToString (rdr["USER_NAME"        ]);
								double  dSTART_OFFSET      = 0.0;
								
								dPIPELINE_TOTAL += dTOTAL;
								if ( dTOTAL > dMAX_TOTAL )
									dMAX_TOTAL = dTOTAL;
								XmlNode nodeRow = nodeYData.SelectSingleNode("dataRow[@title=\'" + Sql.ToString(L10n.Term(".sales_stage_dom.", sSALES_STAGE)).Replace("'", "\'") +"\']");
								if ( nodeRow == null )
								{
									nodeRow = xml.CreateElement("dataRow");
									nodeYData.AppendChild(nodeRow);
									// 09/26/2009 Paul.  LBL_NONE is --None--, so create a new term LBL_NONE_VALUE.
									if ( sSALES_STAGE == String.Empty )
										XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , Sql.ToString(L10n.Term(".LBL_NONE_VALUE")));
									else
										XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , Sql.ToString(L10n.Term(".sales_stage_dom.", sSALES_STAGE)));
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", dTOTAL.ToString("0")   );
								}
								else
								{
									if ( nodeRow.Attributes.GetNamedItem("endLabel") != null )
									{
										double dEND_LABEL = Sql.ToDouble(nodeRow.Attributes.GetNamedItem("endLabel").Value);
										dSTART_OFFSET = dEND_LABEL;
										dEND_LABEL += dTOTAL;
										if ( dEND_LABEL > dMAX_TOTAL )
											dMAX_TOTAL = dEND_LABEL;
										XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", dEND_LABEL.ToString("0")   );
									}
								}
								
								if ( !hashUSER.ContainsKey(gASSIGNED_USER_ID.ToString()) )
								{
									XmlNode nodeMapping = xml.CreateElement("mapping");
									nodeColorLegend.AppendChild(nodeMapping);
									XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "id"   , gASSIGNED_USER_ID.ToString());
									XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , sUSER_NAME);
									XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "color", SplendidDefaults.generate_graphcolor(gASSIGNED_USER_ID.ToString(), hashUSER.Count));
									hashUSER.Add(gASSIGNED_USER_ID.ToString(), sUSER_NAME);
								}
								
								// 09/26/2009 Paul.  The bar should have been prepopulated. 
								XmlNode nodeBar = nodeRow.SelectSingleNode("bar[@id=\'" + gASSIGNED_USER_ID.ToString() + "\']");
								if ( nodeBar == null )
								{
									nodeBar = xml.CreateElement("bar");
									nodeRow.AppendChild(nodeBar);
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "id"         , gASSIGNED_USER_ID.ToString());
								}
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "totalSize"  , dTOTAL.ToString("0"));
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "altText"    , sUSER_NAME + ": " + nOPPORTUNITY_COUNT.ToString() + " " + L10n.Term("Dashboard.LBL_OPPS_WORTH") + " " + dTOTAL.ToString("c0") + L10n.Term("Dashboard.LBL_OPP_THOUSANDS") + " " + L10n.Term("Dashboard.LBL_OPPS_IN_STAGE") + " " + Sql.ToString(L10n.Term(".sales_stage_dom.", sSALES_STAGE)) );
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "url"        , Sql.ToString(Application["rootURL"]) + "Opportunities/default.aspx?SALES_STAGE=" + Server.UrlEncode(sSALES_STAGE) + "&ASSIGNED_USER_ID=" + gASSIGNED_USER_ID.ToString() );
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "startOffset", dSTART_OFFSET.ToString("0"));
								// 05/30/2009 Paul.  We need to be able to track a bar to a specific series for MS Charts. 
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "legendID"   , gASSIGNED_USER_ID.ToString());
							}
							int    nNumLength   = Math.Floor(dMAX_TOTAL).ToString("0").Length - 1;
							double dWhole       = Math.Pow(10, nNumLength);
							double dDecimal     = 1 / dWhole;
							double dMAX_ROUNDED = Math.Ceiling(dMAX_TOTAL * dDecimal) * dWhole;
							
							XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max", dMAX_ROUNDED.ToString("0"));
							// 11/23/2012 Paul.  Add space before value. 
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "title", L10n.Term("Dashboard.LBL_TOTAL_PIPELINE") + " " + dPIPELINE_TOTAL.ToString("c0") + L10n.Term("Dashboard.LBL_OPP_THOUSANDS"));
						}
					}
				}

				nlDataRows = xml.DocumentElement.SelectNodes("yData/dataRow");
				nGridHeight = nlDataRows.Count == 0 ? 226 : 6 + nlDataRows.Count * 20;
				foreach ( XmlNode xRow in nlDataRows )
				{
					foreach ( XmlNode xBar in xRow.SelectNodes("bar") )
					{
						string sTableName = XmlUtil.SelectAttribute(xBar, "id");
						sTableName = sTableName.Replace(" ", "_");
						if ( Sql.IsEmptyString(sTableName) )
							sTableName = "Series_Empty_Table_Name";
						else
							sTableName = "Series_" + sTableName;
						
						DataTable dt = null;
						if ( !dsMain.Tables.Contains(sTableName) )
						{
							dt = dsMain.Tables.Add();
							dt.TableName = sTableName;
							dt.Columns.Add("LABEL"        , Type.GetType("System.String"));
							dt.Columns.Add("VALUE"        , Type.GetType("System.Double"));
							dt.Columns.Add("DISPLAY_VALUE", Type.GetType("System.String"));
							dt.Columns.Add("DESCRIPTION"  , Type.GetType("System.String"));
							dt.Columns.Add("URL"          , Type.GetType("System.String"));
						}
						else
						{
							dt = dsMain.Tables[sTableName];
						}

						DataRow row = dt.NewRow();
						dt.Rows.Add(row);
						row["LABEL"        ] = XmlUtil.SelectAttribute(xRow, "title");
						row["VALUE"        ] = Sql.ToDouble(XmlUtil.SelectAttribute(xBar, "totalSize"));
						row["DISPLAY_VALUE"] = Sql.ToDouble(XmlUtil.SelectAttribute(xBar, "totalSize")).ToString("c0") + L10n.Term("Dashboard.LBL_OPP_THOUSANDS");
						row["DESCRIPTION"  ] = XmlUtil.SelectAttribute(xBar, "altText");
						row["URL"          ] = XmlUtil.SelectAttribute(xBar, "url"    );
					}
				}
				
				sGraphData_Title     = XmlUtil.SelectAttribute (nodeRoot, "title");
				sGraphData_GraphInfo = XmlUtil.SelectSingleNode(nodeRoot, "graphInfo").Replace("<BR/>", " ");

				sAxis_yData_defaultAltText = XmlUtil.SelectAttribute(nodeYData, "defaultAltText");

				nAxis_xData_min            = Sql.ToInteger(XmlUtil.SelectAttribute(nodeXData, "min"   ));
				nAxis_xData_max            = Sql.ToInteger(XmlUtil.SelectAttribute(nodeXData, "max"   ));
				nAxis_xData_length         = Sql.ToInteger(XmlUtil.SelectAttribute(nodeXData, "length"));
				sAxis_xData_prefix         = XmlUtil.SelectAttribute(nodeXData, "prefix");
				sAxis_xData_suffix         = XmlUtil.SelectAttribute(nodeXData, "suffix");
				if ( nAxis_xData_length == 0 )
					nAxis_xData_length = 1;
				if ( nAxis_xData_max == 0 )
					nAxis_xData_max = nGridWidth;
				dAxis_xData_section        = Convert.ToDouble(nAxis_xData_max - nAxis_xData_min) / nAxis_xData_length;

				hashColorLegend = new Hashtable();
				XmlNodeList nlColorLegend = xml.DocumentElement.SelectNodes("colorLegend/mapping");
				foreach ( XmlNode xMapping in nlColorLegend )
				{
					string sTableName = XmlUtil.SelectAttribute(xMapping, "id");
					sTableName = sTableName.Replace(" ", "_");
					if ( Sql.IsEmptyString(sTableName) )
						sTableName = "Series_Empty_Table_Name";
					else
						sTableName = "Series_" + sTableName;
					hashColorLegend.Add(sTableName, XmlUtil.SelectAttribute(xMapping, "color"));
				}

				// 09/26/2009 Paul.  Temporarily stop using the SugarCRM color scheme and instead use the default Silverlight Charts color scheme. 
				/*
				sChartColors_docBorder            = XmlUtil.SelectAttribute(nodeChartColors, "docBorder"           );
				sChartColors_docBg1               = XmlUtil.SelectAttribute(nodeChartColors, "docBg1"              );
				sChartColors_docBg2               = XmlUtil.SelectAttribute(nodeChartColors, "docBg2"              );
				sChartColors_xText                = XmlUtil.SelectAttribute(nodeChartColors, "xText"               );
				sChartColors_yText                = XmlUtil.SelectAttribute(nodeChartColors, "yText"               );
				sChartColors_title                = XmlUtil.SelectAttribute(nodeChartColors, "title"               );
				sChartColors_misc                 = XmlUtil.SelectAttribute(nodeChartColors, "misc"                );
				sChartColors_altBorder            = XmlUtil.SelectAttribute(nodeChartColors, "altBorder"           );
				sChartColors_altBg                = XmlUtil.SelectAttribute(nodeChartColors, "altBg"               );
				sChartColors_altText              = XmlUtil.SelectAttribute(nodeChartColors, "altText"             );
				sChartColors_graphBorder          = XmlUtil.SelectAttribute(nodeChartColors, "graphBorder"         );
				sChartColors_graphBg1             = XmlUtil.SelectAttribute(nodeChartColors, "graphBg1"            );
				sChartColors_graphBg2             = XmlUtil.SelectAttribute(nodeChartColors, "graphBg2"            );
				sChartColors_graphLines           = XmlUtil.SelectAttribute(nodeChartColors, "graphLines"          );
				sChartColors_graphText            = XmlUtil.SelectAttribute(nodeChartColors, "graphText"           );
				sChartColors_graphTextShadow      = XmlUtil.SelectAttribute(nodeChartColors, "graphTextShadow"     );
				sChartColors_barBorder            = XmlUtil.SelectAttribute(nodeChartColors, "barBorder"           );
				sChartColors_barBorderHilite      = XmlUtil.SelectAttribute(nodeChartColors, "barBorderHilite"     );
				sChartColors_legendBorder         = XmlUtil.SelectAttribute(nodeChartColors, "legendBorder"        );
				sChartColors_legendBg1            = XmlUtil.SelectAttribute(nodeChartColors, "legendBg1"           );
				sChartColors_legendBg2            = XmlUtil.SelectAttribute(nodeChartColors, "legendBg2"           );
				sChartColors_legendText           = XmlUtil.SelectAttribute(nodeChartColors, "legendText"          );
				sChartColors_legendColorKeyBorder = XmlUtil.SelectAttribute(nodeChartColors, "legendColorKeyBorder");
				sChartColors_scrollBar            = XmlUtil.SelectAttribute(nodeChartColors, "scrollBar"           );
				sChartColors_scrollBarBorder      = XmlUtil.SelectAttribute(nodeChartColors, "scrollBarBorder"     );
				sChartColors_scrollBarTrack       = XmlUtil.SelectAttribute(nodeChartColors, "scrollBarTrack"      );
				sChartColors_scrollBarTrackBorder = XmlUtil.SelectAttribute(nodeChartColors, "scrollBarTrackBorder");
				*/
				this.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture   = ciCurrent;
				System.Threading.Thread.CurrentThread.CurrentUICulture = ciCurrent;
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
			// 02/11/2008 Paul.  GetCurrent is a better way to get the Ajax manager. 
			ScriptManager mgrAjax = ScriptManager.GetCurrent(this.Page);
			if ( mgrAjax != null && Crm.Config.enable_silverlight() )
			{
				ScriptReference scrSilverlight = new ScriptReference ("~/Include/Silverlight/Silverlight.js");
				mgrAjax.Scripts.Add(scrSilverlight);
			}
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			m_sMODULE = "Opportunities";
		}
		#endregion
	}
}


