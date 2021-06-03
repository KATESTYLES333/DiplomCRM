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
using System.Text;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Diagnostics;

namespace SplendidCRM
{
	#region Create Item Templates
	public class CreateItemTemplateTranslated : ITemplate
	{
		protected string sDATA_FIELD ;
		
		public CreateItemTemplateTranslated(string sDATA_FIELD)
		{
			this.sDATA_FIELD  = sDATA_FIELD ;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					lbl.Text = L10n.Term(Sql.ToString(row[sDATA_FIELD]));
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateLiteral : ITemplate
	{
		protected string sDATA_FIELD ;
		protected string sDATA_FORMAT;
		protected string sMODULE_TYPE;
		
		// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
		public CreateItemTemplateLiteral(string sDATA_FIELD, string sDATA_FORMAT, string sMODULE_TYPE)
		{
			this.sDATA_FIELD  = sDATA_FIELD ;
			this.sDATA_FORMAT = sDATA_FORMAT;
			this.sMODULE_TYPE = sMODULE_TYPE;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					switch ( sDATA_FORMAT )
					{
						case "DateTime":
						{
							// 03/30/2007 Paul.  T10n should never be NULL. 
							TimeZone T10n = HttpContext.Current.Items["T10n"] as TimeZone;
							if ( T10n != null )
								lbl.Text = Sql.ToString(T10n.FromServerTime(row[sDATA_FIELD]));
							break;
						}
						case "Date":
						{
							// 03/30/2007 Paul.  T10n should never be NULL. 
							TimeZone T10n = HttpContext.Current.Items["T10n"] as TimeZone;
							if ( T10n != null )
								lbl.Text = Sql.ToDateString(T10n.FromServerTime(row[sDATA_FIELD]));
							break;
						}
						case "Currency":
						{
							// 03/30/2007 Paul.  Move init of C10n to minimize use. 
							// 05/09/2006 Paul.  Convert the currency values before displaying. 
							// The UI culture should already be set to format the currency. 
							Currency C10n = HttpContext.Current.Items["C10n"] as Currency;
							Decimal d = C10n.ToCurrency(Convert.ToDecimal(row[sDATA_FIELD]));
							// 03/07/2008 Paul.  Lets stop showing the cents in the grids. 
							// 05/14/2008 Paul.  Some companies might want to show the cents.  This is more common when used as an ordering system. 
							string sCurrencyFormat = Sql.ToString(HttpContext.Current.Application["CONFIG.currency_format"]);
							lbl.Text = d.ToString(sCurrencyFormat);
							break;
						}
						case "MultiLine":
						{
							string sDATA = Sql.ToString(row[sDATA_FIELD]);
							// 05/20/2009 Paul.  We need a way to preserve CRLF in description fields. 
							// 06/04/2010 Paul.  Try and prevent excess blank lines. 
							sDATA = EmailUtils.NormalizeDescription(sDATA);
							lbl.Text = sDATA;
							break;
						}
						default:
							// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
							// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
							if ( Sql.IsEmptyString(sMODULE_TYPE) )
							{
								if ( sDATA_FORMAT.Contains("{") )
									lbl.Text = HttpUtility.HtmlEncode(String.Format(sDATA_FORMAT, Sql.ToString(row[sDATA_FIELD])));
								else
									lbl.Text = HttpUtility.HtmlEncode(Sql.ToString(row[sDATA_FIELD]));
							}
							else
								lbl.Text = HttpUtility.HtmlEncode(Crm.Modules.ItemName(HttpContext.Current.Application, sMODULE_TYPE, row[sDATA_FIELD]));
							break;
					}
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateLiteralList : ITemplate
	{
		protected string sDATA_FIELD  ;
		protected string sLIST_NAME   ;
		protected string sPARENT_FIELD;
		
		public CreateItemTemplateLiteralList(string sDATA_FIELD, string sLIST_NAME, string sPARENT_FIELD)
		{
			this.sDATA_FIELD   = sDATA_FIELD ;
			this.sLIST_NAME    = sLIST_NAME  ;
			this.sPARENT_FIELD = sPARENT_FIELD;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( L10n != null )
				{
					if ( row[sDATA_FIELD] != DBNull.Value )
					{
						// 10/09/2010 Paul.  Add PARENT_FIELD so that we can establish dependent listboxes. 
						if ( !Sql.IsEmptyString(sPARENT_FIELD) )
						{
							sLIST_NAME = Sql.ToString(row[sPARENT_FIELD]);
						}
						if ( !Sql.IsEmptyString(sLIST_NAME) )
						{
							string sList = sLIST_NAME;
							// 08/10/2008 Paul.  Use an array to define the custom caches so that list is in the Cache module. 
							// This should reduce the number of times that we have to edit the SplendidDynamic module. 
							bool bCustomCache = false;
							lbl.Text = SplendidCache.CustomList(sLIST_NAME, Sql.ToString(row[sDATA_FIELD]), ref bCustomCache);
							if ( bCustomCache )
								return;
							// 01/18/2007 Paul.  If AssignedUser list, then use the cached value to find the value. 
							if ( sLIST_NAME == "AssignedUser" )
							{
								lbl.Text = SplendidCache.AssignedUser(Sql.ToGuid(row[sDATA_FIELD]));
							}
							// 12/05/2005 Paul.  The activity status needs to be dynamically converted to the correct list. 
							else if ( sLIST_NAME == "activity_status" )
							{
								string sACTIVITY_TYPE = String.Empty;
								try
								{
									sACTIVITY_TYPE = Sql.ToString(row["ACTIVITY_TYPE"]);
									switch ( sACTIVITY_TYPE )
									{
										case "Tasks"   :  sList = "task_status_dom"   ;  break;
										case "Meetings":  sList = "meeting_status_dom";  break;
										case "Calls"   :
											// 07/15/2006 Paul.  Call status is translated externally. 
											lbl.Text = Sql.ToString(row[sDATA_FIELD]);
											return;
											//sList = "call_status_dom"   ;  break;
										case "Notes"   :
											// 07/15/2006 Paul.  Note Status is not normally as it does not have a status. 
											lbl.Text = L10n.Term(".activity_dom.Note");
											return;
										// 06/15/2006 Paul.  This list name for email_status does not follow the standard. 
										case "Emails"  :  sList = "dom_email_status"  ;  break;
										// 09/26/2013 Paul.  Add Sms Messages status. 
										case "SmsMessages":  sList = "dom_sms_status"  ;  break;
										// 04/21/2006 Paul.  If the activity does not have a status (such as a Note), then use activity_dom. 
										default        :  sList = "activity_dom"      ;  break;
									}
								}
								catch(Exception ex)
								{
									SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
								}
								lbl.Text = Sql.ToString(L10n.Term("." + sList + ".", row[sDATA_FIELD]));
							}
							// 02/12/2008 Paul.  If the list contains XML, then treat as a multi-selection. 
							else if ( Sql.ToString(row[sDATA_FIELD]).StartsWith("<?xml") )
							{
								StringBuilder sb = new StringBuilder();
								XmlDocument xml = new XmlDocument();
								// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
								// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
								// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
								xml.XmlResolver = null;
								xml.LoadXml(Sql.ToString(row[sDATA_FIELD]));
								XmlNodeList nlValues = xml.DocumentElement.SelectNodes("Value");
								foreach ( XmlNode xValue in nlValues )
								{
									if ( sb.Length > 0 )
										sb.Append(", ");
									sb.Append(L10n.Term("." + sLIST_NAME + ".", xValue.InnerText));
								}
								lbl.Text = sb.ToString();
							}
							else
							{
								lbl.Text = Sql.ToString(L10n.Term("." + sList + ".", row[sDATA_FIELD]));
							}
						}
					}
				}
				else
				{
					lbl.Text = Sql.ToString(row[sDATA_FIELD]);
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateHyperLink : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sURL_TARGET;
		protected string sCSSCLASS  ;
		protected string sURL_MODULE;
		protected string sURL_ASSIGNED_FIELD;
		protected string sMODULE_TYPE;
		
		// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
		public CreateItemTemplateHyperLink(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sURL_TARGET, string sCSSCLASS, string sURL_MODULE, string sURL_ASSIGNED_FIELD, string sMODULE_TYPE)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sURL_TARGET = sURL_TARGET;
			this.sCSSCLASS   = sCSSCLASS  ;
			this.sURL_MODULE = sURL_MODULE;
			this.sURL_ASSIGNED_FIELD = sURL_ASSIGNED_FIELD;
			this.sMODULE_TYPE = sMODULE_TYPE;
		}
		public void InstantiateIn(Control objContainer)
		{
			HyperLink lnk = new HyperLink();
			lnk.Target   = sURL_TARGET;
			lnk.CssClass = sCSSCLASS  ;
			lnk.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lnk);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			HyperLink lnk = (HyperLink)sender;
			DataGridItem objContainer = (DataGridItem) lnk.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					// 04/27/2006 Paul.  We need the module in order to determine if access is allowed. 
					Guid gASSIGNED_USER_ID = Guid.Empty;
					string sMODULE_NAME = sURL_MODULE;
					if ( row.DataView.Table.Columns.Contains(sURL_ASSIGNED_FIELD) )
					{
						gASSIGNED_USER_ID = Sql.ToGuid(row[sURL_ASSIGNED_FIELD]);
					}
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						// 01/02/2008 Paul.  It is very important that we allow the text field to be set even when the URL_FIELD is NULL. Otherwise, nothing would be displayed.  
						// When displaying Products, we allow the display of order line items, but they are not linkable. 
						if ( row[sDATA_FIELD] != DBNull.Value )
						{
							// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
							// 09/15/2014 Paul.  Prevent Cross-Site Scripting by HTML encoding the data. 
							if ( Sql.IsEmptyString(sMODULE_TYPE) )
								lnk.Text = HttpUtility.HtmlEncode(Sql.ToString(row[sDATA_FIELD]));
							else
								lnk.Text = HttpUtility.HtmlEncode(Crm.Modules.ItemName(HttpContext.Current.Application, sMODULE_TYPE, row[sDATA_FIELD]));
							
							// 02/17/2010 Paul.  sURL_FIELD may not be defined, mostlikely when sMODULE_TYPE is defined. 
							if ( !Sql.IsEmptyString(sMODULE_TYPE) || (!Sql.IsEmptyString(sURL_FIELD) && (row[sURL_FIELD] != DBNull.Value)) )
							{
								bool bAllowed = false;
								// 04/27/2006 Paul.  Only provide the URL if access is allowed.
								// 08/28/2006 Paul.  The URL_FIELD might not always be a GUID.  iFrame uses a URL in this field. 
								// 08/28/2006 Paul.  MODULE_NAME is not always available.  In those cases, assume access is allowed. 
								// 02/17/2010 Paul.  sURL_FIELD may not be defined. 
								string sURL_FIELD_VALUE = String.Empty;
								if ( !Sql.IsEmptyString(sURL_FIELD) )
									sURL_FIELD_VALUE = Sql.ToString(row[sURL_FIELD]);
								int nACLACCESS = ACL_ACCESS.ALL;
								if ( !Sql.IsEmptyString(sMODULE_NAME) )
									nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "view");
								// 05/02/2006 Paul.  Admin has full access. 
								if ( Security.IS_ADMIN )
									bAllowed = true;
								else if ( nACLACCESS == ACL_ACCESS.OWNER )
								{
									// 05/02/2006 Paul.  Owner can only view if USER_ID matches the assigned user id. 
									// 05/02/2006 Paul.  This role also prevents the user from seeing unassigned items.  
									// This may or may not be a good thing. 
									if ( gASSIGNED_USER_ID == Security.USER_ID || Security.IS_ADMIN )
										bAllowed = true;
								}
								// 05/02/2006 Paul.  Allow access if the item is not assigned to anyone. 
								else if ( nACLACCESS >= 0 || Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
									bAllowed = true;
								if ( bAllowed )
								{
									if ( Sql.IsEmptyString(sMODULE_TYPE) )
									{
										lnk.NavigateUrl = String.Format(sURL_FORMAT, sURL_FIELD_VALUE.ToString());
									}
									else
									{
										if ( !Sql.IsEmptyString(sURL_FORMAT) )
										{
											lnk.NavigateUrl = String.Format(sURL_FORMAT, sURL_FIELD_VALUE.ToString());
										}
										else
										{
											// 02/18/2010 Paul.  Get the Module Relative Path so that Project and Project Task will be properly handled. 
											// In these cases, the type is singular and the path is plural. 
											string sRELATIVE_PATH = Sql.ToString(HttpContext.Current.Application["Modules." + sMODULE_TYPE + ".RelativePath"]);
											if ( Sql.IsEmptyString(sRELATIVE_PATH) )
												sRELATIVE_PATH = "~/" + sMODULE_TYPE + "/";
											lnk.NavigateUrl = sRELATIVE_PATH + "view.aspx?ID=" + Sql.ToString(row[sDATA_FIELD]);
										}
									}
								}
							}
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	// 07/26/2007 Paul.  PopupViews have special requirements.  They need an OnClick action that takes more than one parameter. 
	public class CreateItemTemplateHyperLinkOnClick : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sURL_TARGET;
		protected string sCSSCLASS  ;
		protected string sURL_MODULE;
		protected string sURL_ASSIGNED_FIELD;
		protected string sMODULE_TYPE;
		
		// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
		public CreateItemTemplateHyperLinkOnClick(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sURL_TARGET, string sCSSCLASS, string sURL_MODULE, string sURL_ASSIGNED_FIELD, string sMODULE_TYPE)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sURL_TARGET = sURL_TARGET;
			this.sCSSCLASS   = sCSSCLASS  ;
			this.sURL_MODULE = sURL_MODULE;
			this.sURL_ASSIGNED_FIELD = sURL_ASSIGNED_FIELD;
			this.sMODULE_TYPE = sMODULE_TYPE;
		}
		public void InstantiateIn(Control objContainer)
		{
			HyperLink lnk = new HyperLink();
			lnk.Target   = sURL_TARGET;
			lnk.CssClass = sCSSCLASS  ;
			lnk.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lnk);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			HyperLink lnk = (HyperLink)sender;
			DataGridItem objContainer = (DataGridItem) lnk.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					// 04/27/2006 Paul.  We need the module in order to determine if access is allowed. 
					Guid gASSIGNED_USER_ID = Guid.Empty;
					string sMODULE_NAME = sURL_MODULE;
					if ( row.DataView.Table.Columns.Contains(sURL_ASSIGNED_FIELD) )
					{
						gASSIGNED_USER_ID = Sql.ToGuid(row[sURL_ASSIGNED_FIELD]);
					}
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						if ( row[sDATA_FIELD] != DBNull.Value )
						{
							// 02/16/2010 Paul.  Add MODULE_TYPE so that we can lookup custom field IDs. 
							if ( Sql.IsEmptyString(sMODULE_TYPE) )
								lnk.Text = Sql.ToString(row[sDATA_FIELD]);
							else
								lnk.Text = Crm.Modules.ItemName(HttpContext.Current.Application, sMODULE_TYPE, row[sDATA_FIELD]);
							
							bool bAllowed = false;
							string[] arrURL_FIELD = sURL_FIELD.Split(' ');
							object[] objURL_FIELD = new object[arrURL_FIELD.Length];
							for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
							{
								if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
								{
									// 07/26/2007 Paul.  Make sure to escape the javascript string. 
									if ( row[arrURL_FIELD[i]] != DBNull.Value )
										objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(row[arrURL_FIELD[i]]));
									else
										objURL_FIELD[i] = String.Empty;
								}
							}

							int nACLACCESS = ACL_ACCESS.ALL;
							if ( !Sql.IsEmptyString(sMODULE_NAME) )
								nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "view");
							// 05/02/2006 Paul.  Admin has full access. 
							if ( Security.IS_ADMIN )
								bAllowed = true;
							else if ( nACLACCESS == ACL_ACCESS.OWNER )
							{
								// 05/02/2006 Paul.  Owner can only view if USER_ID matches the assigned user id. 
								// 05/02/2006 Paul.  This role also prevents the user from seeing unassigned items.  
								// This may or may not be a good thing. 
								if ( gASSIGNED_USER_ID == Security.USER_ID || Security.IS_ADMIN )
									bAllowed = true;
							}
							// 05/02/2006 Paul.  Allow access if the item is not assigned to anyone. 
							else if ( nACLACCESS >= 0 || Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
								bAllowed = true;
							if ( bAllowed )
							{
								// 01/20/2010 Paul.  If the site root is specified, then don't use onclick. 
								if ( sURL_FORMAT.StartsWith("~/") )
								{
									lnk.NavigateUrl = String.Format(sURL_FORMAT, objURL_FIELD);
								}
								else
								{
									lnk.NavigateUrl = "#";
									lnk.Attributes.Add("onclick", String.Format(sURL_FORMAT, objURL_FIELD));
								}
							}
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	public class CreateItemTemplateJavaScript: ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sURL_TARGET;
		
		public CreateItemTemplateJavaScript(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sURL_TARGET)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sURL_TARGET = sURL_TARGET;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
					// 08/02/2010 Paul.  The DATA_FIELD is not required. 
					//if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						//if ( row[sDATA_FIELD] != DBNull.Value )
						{
							string[] arrURL_FIELD = sURL_FIELD.Split(' ');
							object[] objURL_FIELD = new object[arrURL_FIELD.Length];
							for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
							{
								if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
								{
									// 08/02/2010 Paul.  In our application of Field Level Security, we will hide fields by replacing with "."
									if ( arrURL_FIELD[i].Contains(".") )
										objURL_FIELD[i] = (arrURL_FIELD[i] == "." || L10n == null ) ? String.Empty : L10n.Term(arrURL_FIELD[i]);
									// 07/26/2007 Paul.  Make sure to escape the javascript string. 
									else if ( row.DataView.Table.Columns.Contains(arrURL_FIELD[i]) && row[arrURL_FIELD[i]] != DBNull.Value )
										objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(row[arrURL_FIELD[i]]));
									else
										objURL_FIELD[i] = String.Empty;
								}
							}
							// 12/03/2009 Paul.  LinkedIn Company Profile requires a span tag to insert the link.
							lbl.Text = "<span id=\"" + String.Format(sURL_TARGET, objURL_FIELD) + "\"></span>";
							lbl.Text += "<script type=\"text/javascript\"> " + String.Format(sURL_FORMAT, objURL_FIELD) + "</script>";
						}
					}
					//else
					//	SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	// 02/26/2014 Paul.  Add Preview button. 
	public class CreateItemTemplateJavaScriptImage: ITemplate
	{
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sIMAGE_SKIN;
		
		public CreateItemTemplateJavaScriptImage(string sURL_FIELD, string sURL_FORMAT, string sIMAGE_SKIN)
		{
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sIMAGE_SKIN = sIMAGE_SKIN;
		}
		public void InstantiateIn(Control objContainer)
		{
			Image img = new Image();
			img.SkinID = sIMAGE_SKIN;
			img.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(img);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Image img = (Image)sender;
			DataGridItem objContainer = (DataGridItem) img.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
					string[] arrURL_FIELD = sURL_FIELD.Split(' ');
					object[] objURL_FIELD = new object[arrURL_FIELD.Length];
					for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
					{
						if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
						{
							if ( arrURL_FIELD[i].Contains(".") )
								objURL_FIELD[i] = (arrURL_FIELD[i] == "." || L10n == null ) ? String.Empty : L10n.Term(arrURL_FIELD[i]);
							else if ( row.DataView.Table.Columns.Contains(arrURL_FIELD[i]) && row[arrURL_FIELD[i]] != DBNull.Value )
								objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(row[arrURL_FIELD[i]]));
							else
								objURL_FIELD[i] = String.Empty;
						}
					}
					img.Attributes.Add("onclick", String.Format(sURL_FORMAT, objURL_FIELD));
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	// 03/01/2014 Paul.  Add Preview button. 
	public class CreateItemTemplateImageButton: ITemplate
	{
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sIMAGE_SKIN;
		protected string sCSS_CLASS ;
		protected CommandEventHandler Page_Command;
		
		public CreateItemTemplateImageButton(string sURL_FIELD, string sURL_FORMAT, string sIMAGE_SKIN, string sCSS_CLASS, CommandEventHandler Page_Command)
		{
			this.sURL_FIELD   = sURL_FIELD  ;
			this.sURL_FORMAT  = sURL_FORMAT ;
			this.sIMAGE_SKIN  = sIMAGE_SKIN ;
			this.sCSS_CLASS   = sCSS_CLASS  ;
			this.Page_Command = Page_Command;

		}
		public void InstantiateIn(Control objContainer)
		{
			ImageButton img = new ImageButton();
			img.SkinID      = sIMAGE_SKIN;
			img.CssClass    = sCSS_CLASS ;
			img.CommandName = sURL_FORMAT;
			L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
			if ( L10n != null )
				img.ToolTip = L10n.Term(".LBL_" + sURL_FORMAT.ToUpper());
			if ( Page_Command != null )
				img.Command += new CommandEventHandler(Page_Command);
			img.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(img);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			ImageButton img = (ImageButton)sender;
			DataGridItem objContainer = (DataGridItem) img.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					if ( row.DataView.Table.Columns.Contains(sURL_FIELD) && row[sURL_FIELD] != DBNull.Value )
						img.CommandArgument = Sql.ToString(row[sURL_FIELD]);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	// 08/02/2010 Paul.  Add ability to create hover. 
	public class CreateItemTemplateHover: ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sIMAGE_SKIN;
		
		public CreateItemTemplateHover(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sIMAGE_SKIN)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sIMAGE_SKIN = sIMAGE_SKIN;
		}
		public void InstantiateIn(Control objContainer)
		{
			Image imgInfo = new Image();
			imgInfo.ID = Guid.NewGuid().ToString().Replace("-", "_");
			imgInfo.SkinID = sIMAGE_SKIN;
			objContainer.Controls.Add(imgInfo);
			
			Panel pnlHover = new Panel();
			pnlHover.ID = Guid.NewGuid().ToString().Replace("-", "_");
			// 08/02/2010 Paul.  Set the initial visibility to hidden to prevent flicker. 
			// 08/02/2010 Paul.  Need both display: none; visibility: hidden;
			pnlHover.Attributes.Add("class", "PanelHoverHidden");
			objContainer.Controls.Add(pnlHover);
			
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			pnlHover.Controls.Add(lit);
			
			AjaxControlToolkit.HoverMenuExtender hov = new AjaxControlToolkit.HoverMenuExtender();
			hov.TargetControlID = imgInfo.ID;
			hov.PopupControlID  = pnlHover.ID;
			hov.PopupPosition   = AjaxControlToolkit.HoverMenuPopupPosition.Left;
			hov.PopDelay        =  250;  // Delay popup remains visible after mouse leaves. 
			hov.HoverDelay      =  250;  // Delay before the popup displays.
			hov.OffsetX         =  17;
			hov.OffsetY         =  17;
			objContainer.Controls.Add(hov);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					L10N     L10n = HttpContext.Current.Items["L10n"] as L10N;
					TimeZone T10n = HttpContext.Current.Items["T10n"] as TimeZone;
					Currency C10n = HttpContext.Current.Items["C10n"] as Currency;
					//if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						//if ( row[sDATA_FIELD] != DBNull.Value )
						{
							string[] arrURL_FIELD = sURL_FIELD.Split(' ');
							object[] objURL_FIELD = new object[arrURL_FIELD.Length];
							for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
							{
								if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
								{
									// 08/02/2010 Paul.  In our application of Field Level Security, we will hide fields by replacing with "."
									if ( arrURL_FIELD[i].Contains(".") )
										objURL_FIELD[i] = (arrURL_FIELD[i] == "." || L10n == null) ? String.Empty : L10n.Term(arrURL_FIELD[i]);
									// 08/02/2010 Paul.  HTML encode the data. 
									// 08/02/2010 Paul.  Avoid HTML encoding the data so that currency can be formatted. 
									else if ( row.DataView.Table.Columns.Contains(arrURL_FIELD[i]) && row[arrURL_FIELD[i]] != DBNull.Value )
									{
										object oValue = row[arrURL_FIELD[i]];
										if ( oValue.GetType() == typeof(System.DateTime) && T10n != null )
											objURL_FIELD[i] = T10n.FromServerTime(oValue);
										else if ( arrURL_FIELD[i].EndsWith("_USDOLLAR") && C10n != null )
											objURL_FIELD[i] = C10n.ToCurrency(Convert.ToDecimal(oValue)).ToString("c");
										else
											objURL_FIELD[i] = oValue;
									}
									else
										objURL_FIELD[i] = String.Empty;
								}
							}
							lbl.Text = String.Format(sURL_FORMAT, objURL_FIELD);
						}
					}
					//else
					//	SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	public class CreateItemTemplateImage : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FORMAT;
		protected string sCSSCLASS  ;
		
		// 08/15/2014 Paul.  Show the URL_FORMAT for Images so that we can point to the EmailImages URL. 
		public CreateItemTemplateImage(string sDATA_FIELD, string sURL_FORMAT, string sCSSCLASS)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sCSSCLASS   = sCSSCLASS  ;
		}
		public void InstantiateIn(Control objContainer)
		{
			Image img = new Image();
			img.CssClass = sCSSCLASS  ;
			img.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(img);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Image img = (Image)sender;
			DataGridItem objContainer = (DataGridItem) img.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						if ( row[sDATA_FIELD] != DBNull.Value && row[sDATA_FIELD] != DBNull.Value )
						{
							// 08/15/2014 Paul.  Show the URL_FORMAT for Images so that we can point to the EmailImages URL. 
							if ( Sql.IsEmptyString(sURL_FORMAT) )
								img.ImageUrl = "~/Images/Image.aspx?ID=" + Sql.ToString(row[sDATA_FIELD]);
							else
								img.ImageUrl = sURL_FORMAT + Sql.ToString(row[sDATA_FIELD]);
						}
						else
						{
							// 04/13/2006 Paul.  Don't show the image control if there is no data to show. 
							img.Visible = false;
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}
	#endregion

	public delegate void SelectMethodHandler(int nCurrentPageIndex, int nPageSize);

	/// <summary>
	/// Summary description for SplendidGrid.
	/// </summary>
	public class SplendidGrid : System.Web.UI.WebControls.DataGrid
	{
		protected bool bTranslated = false;

		public SelectMethodHandler SelectMethod;

		public SplendidGrid()
		{
			// http://www2.msdnaa.net/content/?ID=1267
			ItemCreated      += new DataGridItemEventHandler       (OnItemCreated     );
			PageIndexChanged += new DataGridPageChangedEventHandler(OnPageIndexChanged);
			SortCommand      += new DataGridSortCommandEventHandler(OnSort            );
			// 04/07/2008 Paul.  The DataGrid default for AutoGenerateColumns is true, 
			// but we want the default to be false for SplendidGrid.
			this.AutoGenerateColumns = false;

			// 04/09/2008 Paul.  Move the PageSize to the constructor so that it can be overridden. 
			// When we do move to the constructor, we have to clear the value from the skin, otherwise it will override the config value. 
			int nPageSize = Sql.ToInteger(HttpContext.Current.Application["CONFIG.list_max_entries_per_page"]);
			if ( nPageSize > 0 )
			{
				this.PageSize = nPageSize;
			}
		}

		// 11/12/2005 Paul.  Not sure why, but Unified Search/Project List is not translating. 
		public void L10nTranslate()
		{
			if ( !bTranslated )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				PagerStyle.PrevPageText = L10n.Term(PagerStyle.PrevPageText);
				PagerStyle.NextPageText = L10n.Term(PagerStyle.NextPageText);
				foreach(DataGridColumn col in Columns)
				{
					col.HeaderText = L10n.Term(col.HeaderText);
					// 02/25/2008 Paul.  The main term may be used in the grid header. 
					// Instead of always requiring a separate list term, just remove the trailing colon. 
					if ( col.HeaderText.EndsWith(":") )
					{
						col.HeaderText = col.HeaderText.Substring(0, col.HeaderText.Length-1);
					}
				}
				bTranslated = true;
			}
		}

		public string InputCheckbox(bool bShowCheck, string sCheckboxName, Guid gID, HiddenField hidSelectedItems)
		{
			string sInput = String.Empty;
			if ( bShowCheck )
			{
				string sChecked = String.Empty;
				string sID = Sql.ToString(gID);
				if ( hidSelectedItems != null && !Sql.IsEmptyGuid(gID) && hidSelectedItems.Value.Contains(sID) )
					sChecked = "checked";
				sInput = "<input name=\"" + sCheckboxName + "\" class=\"checkbox\" type=\"checkbox\" value=\"" + sID + "\" onclick=\"SplendidGrid_ToggleCheckbox(this)\" " + sChecked + " />";
			}
			return sInput;
		}

		// 12/15/2011 Paul.  The ID can be text. 
		public string InputCheckbox(bool bShowCheck, string sCheckboxName, string sID, HiddenField hidSelectedItems)
		{
			string sInput = String.Empty;
			if ( bShowCheck )
			{
				string sChecked = String.Empty;
				if ( hidSelectedItems != null && !Sql.IsEmptyString(sID) && hidSelectedItems.Value.Contains(sID) )
					sChecked = "checked";
				sInput = "<input name=\"" + sCheckboxName + "\" class=\"checkbox\" type=\"checkbox\" value=\"" + HttpUtility.HtmlEncode(sID) + "\" onclick=\"SplendidGrid_ToggleCheckbox(this)\" " + sChecked + " />";
			}
			return sInput;
		}

		protected void OnItemCreated(object sender, DataGridItemEventArgs e)
		{
			// 08/21/2006 Lawrence Zamorano.  Add the record count to the pager control. 
			// 08/21/2006 Paul.  Enhance to include page range. 
			// 02/20/2008 Paul.  Only add page numbers if using the NextPrev mode. 
			if ( e.Item.ItemType == ListItemType.Pager && this.PagerStyle.Mode == PagerMode.NextPrev )
			{
				TableCell pgr = e.Item.Controls[0] as TableCell; 
				DataView vw = this.DataSource as DataView;
				if ( vw != null && vw.Count > 0 )
				{
					// 08/21/2006 Paul.  Grab references to the Prev and Next controls while we know their indexes. 
					// 08/21/2006 Paul.  The previous and next controls will either be a LinkButton if active, or a Label if inactive. 
					LinkButton lnkPrev = pgr.Controls[0] as LinkButton;
					LinkButton lnkNext = pgr.Controls[2] as LinkButton;
					Label      lblPrev = pgr.Controls[0] as Label;
					Label      lblNext = pgr.Controls[2] as Label;
					
					L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
					string sOf = L10n.Term(".LBL_LIST_OF");
					int nPageStart = this.CurrentPageIndex * this.PageSize + 1;
					int nPageEnd   = Math.Min((this.CurrentPageIndex+1) * this.PageSize, vw.Count);
					LiteralControl litPageRange = new LiteralControl();
					litPageRange.Text = String.Format("&nbsp; <span class=\"pageNumbers\">({0} - {1} {2} {3})</span> ", nPageStart, nPageEnd, sOf, vw.Count);
					// 09/08/2009 Paul.  For custom paging, use the VirtualItemCount instead of the actual data count. 
					if ( base.AllowCustomPaging )
					{
						nPageEnd   = Math.Min((this.CurrentPageIndex+1) * this.PageSize, base.VirtualItemCount);
						litPageRange.Text = String.Format("&nbsp; <span class=\"pageNumbers\">({0} - {1} {2} {3})</span> ", nPageStart, nPageEnd, sOf, base.VirtualItemCount);
					}
					pgr.Controls.AddAt(1, litPageRange);

					string sThemeURL = Sql.ToString(HttpContext.Current.Session["themeURL"]);
					if ( lblPrev != null )
					{
						// 05/08/2010 Paul.  Increase the size of the image to match the Six Theme.  Will need to modify the existing Sugar theme to match the size. 
						lblPrev.Text = "<img src=\"" + sThemeURL + "images/previous_off.gif" + "\" border=\"0\" height=\"18\" width=\"18\" /><span class=\"pageNumbers\">&nbsp;" + lblPrev.Text + "</span>";
					}
					if ( lnkPrev != null )
					{
						// 05/08/2010 Paul.  Increase the size of the image to match the Six Theme.  Will need to modify the existing Sugar theme to match the size. 
						lnkPrev.Text = "<img src=\"" + sThemeURL + "images/previous.gif" + "\" border=\"0\" height=\"18\" width=\"18\" /><span class=\"pageNumbers\">&nbsp;" + lnkPrev.Text + "</span>";
						//LinkButton lnkStart = new LinkButton();
						//lnkStart.CommandArgument = "1";
						//lnkStart.CommandName = "Page";
						//lnkStart.Text = "<img src=\"" + sThemeURL + "images/start.gif" + "\" border=\"0\" height=\"10\" width=\"11\" />&nbsp;" + L10n.Term(".LNK_LIST_START") + "&nbsp;";
						//pgr.Controls.AddAt(0, lnkStart);
					}
					if ( lblNext != null )
					{
						// 05/08/2010 Paul.  Increase the size of the image to match the Six Theme.  Will need to modify the existing Sugar theme to match the size. 
						lblNext.Text = "<span class=\"pageNumbers\">" + lblNext.Text + "&nbsp;</span><img src=\"" + sThemeURL + "images/next_off.gif" + "\" border=\"0\" height=\"18\" width=\"18\" />";
					}
					if ( lnkNext != null )
					{
						// 05/08/2010 Paul.  Increase the size of the image to match the Six Theme.  Will need to modify the existing Sugar theme to match the size. 
						lnkNext.Text = "<span class=\"pageNumbers\">" + lnkNext.Text + "&nbsp;</span><img src=\"" + sThemeURL + "images/next.gif" + "\" border=\"0\" height=\"18\" width=\"18\" />";
						//LinkButton lnkEnd = new LinkButton();
						//lnkEnd.CommandArgument = this.PageCount.ToString();
						//lnkEnd.CommandName = "Page";
						//lnkEnd.Text = "&nbsp;" + L10n.Term(".LNK_LIST_END") + "&nbsp;<img src=\"" + sThemeURL + "images/end.gif" + "\" border=\"0\" height=\"10\" width=\"11\" />";
						//pgr.Controls.Add(lnkEnd);
					}
				}
			}
			else if ( e.Item.ItemType == ListItemType.Header )
			{
				// 06/09/2006 Paul.  Move the translation to overridden DataBind. 
				//L10nTranslate();
				// 11/21/2005 Paul.  The header cells should never wrap, the background image was not designed to wrap. 
				foreach(TableCell cell in e.Item.Cells)
				{
					cell.Wrap = false;
				}
				HttpSessionState Session = HttpContext.Current.Session;
				// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
				string sLastSortColumn = (string)ViewState[this.ID + ".LastSortColumn"];
				string sLastSortOrder  = (string)ViewState[this.ID + ".LastSortOrder" ];
				// 08/28/2006 Paul.  We need to watch for overflow.  This has occurred when a grid was created with no columns. 
				for(int i = 0 ; i < e.Item.Controls.Count && i < this.Columns.Count ; i++ )
				{
					// 11/13/2005 Paul.  If sorting is not enabled, this code will cause the header text to disappear. 
					if ( this.AllowSorting && !Sql.IsEmptyString(Columns[i].SortExpression) )
					{
						Image img = new Image();
						// 04/09/2008 Paul.  Use SkinID. 
						if ( Columns[i].SortExpression == sLastSortColumn )
						{
							// 09/01/2010 Paul.  Sugar has ascending pointing up and descending pointing down. 
							if ( sLastSortOrder == "asc" )
								img.SkinID = "arrow_up";
							else
								img.SkinID = "arrow_down";
						}
						else
						{
							img.SkinID = "arrow";
						}
						Literal lit = new Literal();
						lit.Text = "&nbsp;";
						e.Item.Cells[i].Controls.Add(lit);
						e.Item.Cells[i].Controls.Add(img);
					}
				}
			}
			else if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				// 09/05/2005 Paul.  Reducing viewstate data in a table can be done at the row level. 
				// This will provide a major performance benefit while not loosing the ability to sort a grid. 
				// http://authors.aspalliance.com/jimross/Articles/DatagridDietPartTwo.aspx
				// 10/13/2005 Paul.  Can't disable the content otherwise the data is not retained during certain postback operations. 
				//e.Item.EnableViewState = false;
			}
		}

		protected void OnPageIndexChanged(Object sender, DataGridPageChangedEventArgs e)
		{
			// Set CurrentPageIndex to the page the user clicked.
			base.CurrentPageIndex = e.NewPageIndex;
			ApplySort();
			try
			{
				// 09/08/2009 Paul.  The database will not throw an exception of the specified rows are 
				// out of range, so we need to manually catch this event. 
				if ( base.AllowCustomPaging && SelectMethod != null )
				{
					// 10/12/2009 Paul.  We need the page count to be rounded up. 
					int nPageCount = (int) Math.Ceiling((double) base.VirtualItemCount / base.PageSize);
					if ( base.CurrentPageIndex >= nPageCount )
					{
						base.CurrentPageIndex = 0;
					}
				}
				DataBind();
			}
			catch
			{
				// 11/28/2008 tomg.  If the user specifies a search criteria, then clicks Next instead of search, 
				// the page count may be less than the next index. Catch this error and move to the top of the list. 
				if ( e.NewPageIndex >= base.PageCount )
				{
					base.CurrentPageIndex = 0;
					DataBind();
				}
				else
				{
					throw;
				}
			}
		}

		protected void OnSort(object sender, DataGridSortCommandEventArgs e)
		{
			string sNewSortColumn  = e.SortExpression.ToString();
			string sNewSortOrder   = "asc"; // default
			// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
			string sLastSortColumn = (string)ViewState[this.ID + ".LastSortColumn"];
			string sLastSortOrder  = (string)ViewState[this.ID + ".LastSortOrder" ];
			if ( sNewSortColumn.Equals(sLastSortColumn) && sLastSortOrder.Equals("asc") )
			{
				sNewSortOrder= "desc";
			}
			ViewState[this.ID + ".LastSortColumn"] = sNewSortColumn;
			ViewState[this.ID + ".LastSortOrder" ] = sNewSortOrder;

			ApplySort();
			EditItemIndex     = -1;
			CurrentPageIndex  = 0; // goto first page
			DataBind();
		}

		// 06/09/2006 Paul.  Now that we have removed all the data binding code in controls, 
		// we are back to having a problem with the translations.  
		public override void DataBind()
		{
			L10nTranslate();
			// 09/08/2009 Paul.  If we are using custom paging, send the binding event. 
			if ( base.AllowCustomPaging && SelectMethod != null )
			{
				bool bPrintView = Sql.ToBoolean(Context.Items["PrintView"]);
				// 09/08/2009 Paul.  In PrintView, we disable paging, so if this flag is disabled, then show all records. 
				if ( bPrintView )
					SelectMethod(0, -1);
				else
					SelectMethod(base.CurrentPageIndex, base.PageSize);
			}
			base.DataBind();
			// 10/04/2006 Paul.  Use the config value to set the page size. 
			// We need to do this inside DataBind as AllowPaging is typically updated in the ASCX, and only applies when bound. 
			// 04/09/2008 Paul.  Move the PageSize to the constructor so that it can be overridden. 
			// When we do move to the constructor, we have to clear the value from the skin, otherwise it will override the config value. 
			/*
			if ( this.AllowPaging )
			{
				int nPageSize = Sql.ToInteger(Page.Application["CONFIG.list_max_entries_per_page"]);
				if ( nPageSize > 0 )
				{
					this.PageSize = nPageSize;
				}
			}
			*/
		}

		// 04/27/2008 Paul.  A ListView will need to set and build the order clause in two setps 
		// so that the SavedSearch sort value can be taken into account. 
		public string OrderByClause()
		{
			if ( Sql.IsEmptyString(this.SortColumn) )
				return String.Empty;
			return " order by " + this.SortColumn + " " + this.SortOrder + ControlChars.CrLf;
		}

		// 04/26/2008 Paul.  Move Last Sort to the database.
		public string OrderByClause(string sSortColumn, string sSortOrder)
		{
			string sOrderBy = String.Empty;
			if ( !this.Page.IsPostBack )
			{
				this.SortColumn = sSortColumn;
				this.SortOrder  = sSortOrder ;
			}
			else
			{
				sSortColumn = this.SortColumn;
				sSortOrder  = this.SortOrder ;
			}
			// 10/23/2010 Paul.  Prevent invalid SQL if SortColumn is not specified. 
			if ( !Sql.IsEmptyString(sSortColumn) )
				sOrderBy = " order by " + sSortColumn + " " + sSortOrder + ControlChars.CrLf;
			return sOrderBy;
		}

		public void ApplySort()
		{
			// 09/08/2009 Paul.  We can't use the default handling when using custom paging. 
			if ( !base.AllowCustomPaging || SelectMethod == null )
			{
				// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
				string sLastSortColumn = (string)ViewState[this.ID + ".LastSortColumn"];
				string sLastSortOrder  = (string)ViewState[this.ID + ".LastSortOrder" ];
				DataView vw = (DataView) DataSource ;
				if ( vw != null && !Sql.IsEmptyString(sLastSortColumn) )
				{
					// 04/20/2008 Paul.  We need to make sure that the table contains the sort column. 
					// We have a separate problem of ensuring that the sort column in a saved search is in the fields list. 
					if ( vw.Table.Columns.Contains(sLastSortColumn) )
						vw.Sort = sLastSortColumn + " " + sLastSortOrder;
				}
				// 11/12/2005 Paul.  Not sure why, but Unified Search/Project List is not translating. 
				// 06/09/2006 Paul.  Now that we have overridden DataBind, there is no need to translate here. 
				//L10nTranslate();
			}
		}

		// 12/14/2007 Paul.  We need an easy way to capture the sort event from the SearchView. 
		public void SetSortFields(string[] arrSort)
		{
			if ( arrSort != null )
			{
				if ( arrSort.Length == 2 )
				{
					if ( !Sql.IsEmptyString(arrSort[0]) && !Sql.IsEmptyString(arrSort[1]) )
					{
						SortColumn = arrSort[0];
						SortOrder  = arrSort[1];
					}
				}
			}
			else
			{
				// 12/17/2007 Paul.  Clear the sort when NULL provided. 
				// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
				ViewState.Remove(this.ID + ".LastSortColumn");
				ViewState.Remove(this.ID + ".LastSortOrder" );
			}
		}

		public string SortColumn
		{
			get
			{
				// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
				return Sql.ToString(ViewState[this.ID + ".LastSortColumn"]);
			}
			set
			{
				ViewState[this.ID + ".LastSortColumn"] = value;
			}
		}

		public string SortOrder
		{
			get
			{
				// 10/23/2010 Paul.  Use the ID to ensure uniqueness when multiple SplendidGrids are used on a page. 
				return Sql.ToString(ViewState[this.ID + ".LastSortOrder"]);
			}
			set
			{
				ViewState[this.ID + ".LastSortOrder"] = value;
			}
		}

		// 11/19/2007 Paul.  Restore old name to reduce upgrade pains. 
		public void DynamicColumns(string sGRID_NAME)
		{
			SplendidDynamic.AppendGridColumns(sGRID_NAME, this);
		}

		public void AppendGridColumns(string sGRID_NAME)
		{
			AppendGridColumns(sGRID_NAME, null);
		}

		// 02/08/2008 Paul.  We need to build a list of the fields used by the dynamic grid. 
		public void AppendGridColumns(string sGRID_NAME, UniqueStringCollection arrSelectFields)
		{
			SplendidDynamic.AppendGridColumns(sGRID_NAME, this, arrSelectFields, null);
		}

		// 03/01/2014 Paul.  Add Preview button. 
		public void AppendGridColumns(string sGRID_NAME, UniqueStringCollection arrSelectFields, CommandEventHandler Page_Command)
		{
			SplendidDynamic.AppendGridColumns(sGRID_NAME, this, arrSelectFields, Page_Command);
		}
	}

	/// <summary>
	/// Summary description for DynamicImage.
	/// </summary>
	public class DynamicImage : System.Web.UI.UserControl
	{
		protected string sImageSkinID;
		protected string sAlternateText;

		public string ImageSkinID
		{
			get { return sImageSkinID; }
			set { sImageSkinID = value; }
		}

		public string AlternateText
		{
			get { return sAlternateText; }
			set { sAlternateText = value; }
		}

		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			if ( !Sql.IsEmptyString(sImageSkinID) )
			{
				// 06/10/2009 Paul.  Clear before adding, otherwise if multiple Page.DataBind() calls will create multiple images. 
				this.Controls.Clear();

				Image img = new Image();
				img.SkinID        = sImageSkinID;
				// 08/18/2010 Paul.  IE8 does not support alt any more, so we need to use ToolTip instead. 
				img.ToolTip       = sAlternateText;
				this.Controls.Add(img);
				// 04/09/2008 Paul.  The skin is being applied properly, but the Root Path Reference is not. 
				// Manually resolve the ~/ using the Page object. 
				img.ImageUrl = Page.ResolveUrl(img.ImageUrl);
			}
		}
	}

}



