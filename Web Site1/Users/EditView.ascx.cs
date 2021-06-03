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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader   ctlModuleHeader  ;
		protected _controls.EditButtons    ctlDynamicButtons;
		// 01/13/2010 Paul.  Add footer buttons. 
		protected _controls.EditButtons    ctlFooterButtons ;
		protected Users.FacebookButtons    ctlFacebookButtons;

		private const string sEMPTY_PASSWORD = "******";
		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		//protected HtmlTable       tblAddress                      ;
		protected HtmlTable       tblMailOptions                  ;

		protected TextBox         txtFIRST_NAME                   ;
		protected TextBox         txtLAST_NAME                    ;
		protected TextBox         txtUSER_NAME                    ;
		protected TextBox         txtPASSWORD                     ;
		protected TableCell       tdPASSWORD_Label                ;
		protected TableCell       tdPASSWORD_Field                ;
		// 11/21/2014 Paul.  Add User Picture. 
		protected HiddenField     PICTURE                         ;
		protected HtmlImage       imgPICTURE                      ;

		protected DropDownList    lstSTATUS                       ;
		// user_settings
		protected CheckBox        chkIS_ADMIN                     ;
		// 03/16/2010 Paul.  Add IS_ADMIN_DELEGATE. 
		protected CheckBox        chkIS_ADMIN_DELEGATE            ;
		protected CheckBox        chkPORTAL_ONLY                  ;
		protected CheckBox        chkRECEIVE_NOTIFICATIONS        ;
		// 03/04/2011 Paul.  We need to allow the admin to set the flag to force a password change. 
		protected CheckBox        chkSYSTEM_GENERATED_PASSWORD    ;
		protected DropDownList    lstTHEME                        ;
		protected DropDownList    lstLANGUAGE                     ;
		protected DropDownList    lstDATE_FORMAT                  ;
		protected DropDownList    lstTIME_FORMAT                  ;
		protected DropDownList    lstTIMEZONE                     ;
		protected CheckBox        chkSAVE_QUERY                   ;
		// 02/26/2010 Paul.  Allow users to configure use of tabs. 
		protected CheckBox        chkGROUP_TABS                   ;
		protected CheckBox        chkSUBPANEL_TABS                ;
		protected DropDownList    lstCURRENCY                     ;

		// 05/06/2009 Paul.  Add DEFAULT_TEAM to support SugarCRM migration. 
		protected TableCell       tdDEFAULT_TEAM_Label            ;
		protected TableCell       tdDEFAULT_TEAM_Field            ;
		protected bool            bMyAccount                      ;
		protected RequiredFieldValidator reqLAST_NAME;
		protected RequiredFieldValidator reqUSER_NAME;
		
		protected Button          btnSmtpTest                     ;
		// 03/25/2011 Paul.  Add support for Google Apps. 
		protected Table           tblGoogleAppsPanel              ;
		protected HtmlTable       tblGoogleAppsOptions            ;
		protected Button          btnGoogleAppsTest               ;
		// 12/13/2011 Paul.  Add support for Apple iCloud. 
		protected Table           tblICloudPanel                  ;
		protected HtmlTable       tblICloudOptions                ;
		protected Button          btnICloudTest                   ;

		public bool MyAccount
		{
			get
			{
				return bMyAccount;
			}
			set
			{
				bMyAccount = value;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid   gPARENT_ID   = Sql.ToGuid(Request["PARENT_ID"]);
			string sMODULE      = String.Empty;
			string sPARENT_TYPE = String.Empty;
			string sPARENT_NAME = String.Empty;
			try
			{
				SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// The only possible error is a connection failure, so just ignore all errors. 
				gPARENT_ID = Guid.Empty;
			}
			// 03/15/2014 Paul.  Enable override of concurrency error. 
			if ( e.CommandName == "Save" || e.CommandName == "SaveConcurrency" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + "." + LayoutEditView   );
					this.ValidateEditViewFields(m_sMODULE + ".EditAddress");
					// 11/10/2010 Paul.  Apply Business Rules. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + LayoutEditView   );
					this.ApplyEditViewValidationEventRules(m_sMODULE + ".EditAddress");
					if ( Page.IsValid )
					{
						string sUSER_PREFERENCES = String.Empty;
						string sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
						string sGOOGLEAPPS_PASSWORD = Sql.ToString(ViewState["GOOGLEAPPS_PASSWORD"]);
						string sICLOUD_PASSWORD     = Sql.ToString(ViewState["ICLOUD_PASSWORD"    ]);
						//XmlDocument xml = new XmlDocument();
						// 01/20/2015 Paul.  Disable XmlResolver to prevent XML XXE. 
						// https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Processing
						// http://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
						//xml.XmlResolver = null;
						try
						{
							//12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
							/*
							try
							{
								sUSER_PREFERENCES = Sql.ToString(ViewState["USER_PREFERENCES"]);
								xml.LoadXml(sUSER_PREFERENCES);
							}
							catch(Exception ex)
							{
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
								xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
								xml.AppendChild(xml.CreateElement("USER_PREFERENCE"));
							}
							// user_settings
							// 08/12/2009 Paul.  A customer wants the ability to turn off the saved searches, both globally and on a per user basis. 
							XmlUtil.SetSingleNode(xml, "save_query"          , chkSAVE_QUERY.Checked ? "true" : "false");
							// 02/26/2010 Paul.  Allow users to configure use of tabs. 
							XmlUtil.SetSingleNode(xml, "group_tabs"          , chkGROUP_TABS.Checked ? "true" : "false");
							XmlUtil.SetSingleNode(xml, "subpanel_tabs"       , chkSUBPANEL_TABS.Checked ? "true" : "false");
							XmlUtil.SetSingleNode(xml, "culture"             , lstLANGUAGE.SelectedValue             );
							XmlUtil.SetSingleNode(xml, "theme"               , lstTHEME.SelectedValue                );
							XmlUtil.SetSingleNode(xml, "dateformat"          , lstDATE_FORMAT.SelectedValue          );
							XmlUtil.SetSingleNode(xml, "timeformat"          , lstTIME_FORMAT.SelectedValue          );
							XmlUtil.SetSingleNode(xml, "timezone"            , lstTIMEZONE.SelectedValue             );
							XmlUtil.SetSingleNode(xml, "currency_id"         , lstCURRENCY.SelectedValue             );
							*/
							// 02/29/2008 Paul.  The config value should only be used as an override.  We should default to the .NET culture value. 
							//CultureInfo culture = CultureInfo.CreateSpecificCulture(lstLANGUAGE.SelectedValue);
							// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
							//XmlUtil.SetSingleNode(xml, "reminder_time"       , chkSHOULD_REMIND.Checked ? lstREMINDER_TIME.SelectedValue : "0" );
							// mail_options
							
							Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
							Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
							// 08/06/2005 Paul.  Password might be our empty value. 
							TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
							if ( txtMAIL_SMTPPASS != null )
							{
								// 08/05/2006 Paul.  Allow the password to be cleared. 
								// 07/08/2010 Paul.  We want to save the password for later use. 
								sMAIL_SMTPPASS = txtMAIL_SMTPPASS.Text;
								if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
								{
									sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
								}
								else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
								{
									string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
									if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sMAIL_SMTPPASS )
										throw(new Exception("Decryption failed"));
									sMAIL_SMTPPASS = sENCRYPTED_EMAIL_PASSWORD;
									ViewState["mail_smtppass"] = sMAIL_SMTPPASS;
									txtMAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
								}
							}
							TextBox txtGOOGLEAPPS_PASSWORD = FindControl("GOOGLEAPPS_PASSWORD") as TextBox;
							if ( txtGOOGLEAPPS_PASSWORD != null )
							{
								// 08/05/2006 Paul.  Allow the password to be cleared. 
								// 07/08/2010 Paul.  We want to save the password for later use. 
								sGOOGLEAPPS_PASSWORD = txtGOOGLEAPPS_PASSWORD.Text;
								if ( sGOOGLEAPPS_PASSWORD == sEMPTY_PASSWORD )
								{
									sGOOGLEAPPS_PASSWORD = Sql.ToString(ViewState["GOOGLEAPPS_PASSWORD"]);
								}
								else if ( !Sql.IsEmptyString(sGOOGLEAPPS_PASSWORD) )
								{
									string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sGOOGLEAPPS_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
									if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sGOOGLEAPPS_PASSWORD )
										throw(new Exception("Decryption failed"));
									sGOOGLEAPPS_PASSWORD = sENCRYPTED_EMAIL_PASSWORD;
									ViewState["GOOGLEAPPS_PASSWORD"] = sGOOGLEAPPS_PASSWORD;
									txtGOOGLEAPPS_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
								}
							}
							// 12/13/2011 Paul.  Add support for Apple iCloud. 
							TextBox txtICLOUD_PASSWORD = FindControl("ICLOUD_PASSWORD") as TextBox;
							if ( txtICLOUD_PASSWORD != null )
							{
								// 08/05/2006 Paul.  Allow the password to be cleared. 
								// 07/08/2010 Paul.  We want to save the password for later use. 
								sICLOUD_PASSWORD = txtICLOUD_PASSWORD.Text;
								if ( sICLOUD_PASSWORD == sEMPTY_PASSWORD )
								{
									sICLOUD_PASSWORD = Sql.ToString(ViewState["ICLOUD_PASSWORD"]);
								}
								else if ( !Sql.IsEmptyString(sICLOUD_PASSWORD) )
								{
									string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sICLOUD_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
									if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sICLOUD_PASSWORD )
										throw(new Exception("Decryption failed"));
									sICLOUD_PASSWORD = sENCRYPTED_EMAIL_PASSWORD;
									ViewState["ICLOUD_PASSWORD"] = sICLOUD_PASSWORD;
									txtICLOUD_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
								}
							}
							
							// 07/08/2010 Paul.  The user must share the global mail server, so all we need here is the user name and password. 
							//XmlUtil.SetSingleNode(xml, "mail_fromname"       , new DynamicControl(this, "MAIL_FROMNAME"    ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_fromaddress"    , new DynamicControl(this, "MAIL_FROMADDRESS" ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_smtpserver"     , new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_smtpport"       , new DynamicControl(this, "MAIL_SMTPPORT"    ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_sendtype"       , new DynamicControl(this, "MAIL_SENDTYPE"    ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_smtpauth_req"   , new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked ? "true" : "false");
							// 07/09/2010 Paul.  Move the SMTP values from USER_PREFERENCES to the main table to make it easier to access. 
							//XmlUtil.SetSingleNode(xml, "mail_smtpuser"       , new DynamicControl(this, "MAIL_SMTPUSER"    ).Text   );
							//XmlUtil.SetSingleNode(xml, "mail_smtppass"       , sMAIL_SMTPPASS);
							
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						}
						//12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
						//if ( Sql.ToBoolean(Application["CONFIG.XML_UserPreferences"]) )
						//	sUSER_PREFERENCES = xml.OuterXml;
						//else
						//	sUSER_PREFERENCES = XmlUtil.ConvertToPHP(xml.DocumentElement);
						
						// 12/06/2005 Paul.  Need to prevent duplicate users. 
						string sUSER_NAME = txtUSER_NAME.Text.Trim();
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						try
						{
							// 11/10/2006 Paul.  If the decimal and group separate match, then .NET will not be able to parse decimals. 
							// The exception "Input string was not in a correct format." is thrown. 
							if ( !Sql.IsEmptyString(sUSER_NAME) )
							{
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									string sSQL ;
									sSQL = "select USER_NAME             " + ControlChars.CrLf
									     + "  from vwUSERS               " + ControlChars.CrLf
									     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME);
										if ( !Sql.IsEmptyGuid(gID) )
										{
											// 12/06/2005 Paul.  Only include the ID if it is not null as we cannot compare NULL to anything. 
											cmd.CommandText += "   and ID <> @ID" + ControlChars.CrLf;
											Sql.AddParameter(cmd, "@ID", gID);
										}
										con.Open();
										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												string sMESSAGE = String.Empty;
												sMESSAGE = String.Format(L10n.Term("Users.ERR_USER_NAME_EXISTS_1") + "{0}" + L10n.Term("Users.ERR_USER_NAME_EXISTS_2"), sUSER_NAME);
												throw(new Exception(sMESSAGE));
											}
										}
									}
								}
							}
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlDynamicButtons .ErrorText = ex.Message;
							ctlFacebookButtons.ErrorText = ex.Message;
							return;
						}

						// 09/09/2009 Paul.  Use the new function to get the table name. 
						string sTABLE_NAME = Crm.Modules.TableName(m_sMODULE);
						DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							string sSQL ;
							// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
							DataRow   rowCurrent = null;
							DataTable dtCurrent  = new DataTable();
							if ( !Sql.IsEmptyGuid(gID) )
							{
								sSQL = "select *           " + ControlChars.CrLf
								     + "  from vwUSERS_Edit" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Security.Filter(cmd, m_sMODULE, "edit");
									Sql.AppendParameter(cmd, gID, "ID", false);
									using ( DbDataAdapter da = dbf.CreateDataAdapter() )
									{
										((IDbDataAdapter)da).SelectCommand = cmd;
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											rowCurrent = dtCurrent.Rows[0];
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											DateTime dtLAST_DATE_MODIFIED = Sql.ToDateTime(ViewState["LAST_DATE_MODIFIED"]);
											// 03/15/2014 Paul.  Enable override of concurrency error. 
											if ( Sql.ToBoolean(Application["CONFIG.enable_concurrency_check"])  && (e.CommandName != "SaveConcurrency") && dtLAST_DATE_MODIFIED != DateTime.MinValue && Sql.ToDateTime(rowCurrent["DATE_MODIFIED"]) > dtLAST_DATE_MODIFIED )
											{
												// 03/15/2014 Paul.  Dynamic Buttons is not used in this area. 
												//ctlDynamicButtons.ShowButton("SaveConcurrency", true);
												//ctlFooterButtons .ShowButton("SaveConcurrency", true);
												throw(new Exception(String.Format(L10n.Term(".ERR_CONCURRENCY_OVERRIDE"), dtLAST_DATE_MODIFIED)));
											}
										}
										else
										{
											// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
											// It is possible that the record exists, but that ACL rules prevent it from being selected. 
											gID = Guid.Empty;
										}
									}
								}
							}

							// 11/10/2010 Paul.  Apply Business Rules. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + LayoutEditView   , rowCurrent);
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + ".EditAddress", rowCurrent);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									bool bNewUser = Sql.IsEmptyGuid(gID);
									// 04/24/2006 Paul.  Upgrade to SugarCRM 4.2 Schema. 
									// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
									// 05/06/2009 Paul.  Add DEFAULT_TEAM to support SugarCRM migration. 
									SqlProcs.spUSERS_Update
										( ref gID
										, sUSER_NAME
										, txtFIRST_NAME.Text
										, txtLAST_NAME .Text
										, new DynamicControl(this, rowCurrent, "REPORTS_TO_ID"     ).ID
										, (Security.IS_ADMIN ? chkIS_ADMIN.Checked : Sql.ToBoolean(ViewState["IS_ADMIN"]) )
										, chkRECEIVE_NOTIFICATIONS.Checked
										, new DynamicControl(this, rowCurrent, "DESCRIPTION"       ).Text
										, new DynamicControl(this, rowCurrent, "TITLE"             ).Text
										, new DynamicControl(this, rowCurrent, "DEPARTMENT"        ).Text
										, new DynamicControl(this, rowCurrent, "PHONE_HOME"        ).Text
										, new DynamicControl(this, rowCurrent, "PHONE_MOBILE"      ).Text
										, new DynamicControl(this, rowCurrent, "PHONE_WORK"        ).Text
										, new DynamicControl(this, rowCurrent, "PHONE_OTHER"       ).Text
										, new DynamicControl(this, rowCurrent, "PHONE_FAX"         ).Text
										, new DynamicControl(this, rowCurrent, "EMAIL1"            ).Text
										, new DynamicControl(this, rowCurrent, "EMAIL2"            ).Text
										, lstSTATUS.SelectedValue
										, new DynamicControl(this, rowCurrent, "ADDRESS_STREET"    ).Text
										, new DynamicControl(this, rowCurrent, "ADDRESS_CITY"      ).Text
										, new DynamicControl(this, rowCurrent, "ADDRESS_STATE"     ).Text
										, new DynamicControl(this, rowCurrent, "ADDRESS_POSTALCODE").Text
										, new DynamicControl(this, rowCurrent, "ADDRESS_COUNTRY"   ).Text
										, sUSER_PREFERENCES
										, chkPORTAL_ONLY.Checked
										, new DynamicControl(this, rowCurrent, "EMPLOYEE_STATUS"   ).SelectedValue
										, new DynamicControl(this, rowCurrent, "MESSENGER_ID"      ).Text
										, new DynamicControl(this, rowCurrent, "MESSENGER_TYPE"    ).SelectedValue
										, sMODULE
										, gPARENT_ID
										, new DynamicControl(this, rowCurrent, "IS_GROUP"          ).Checked
										, new DynamicControl(this, rowCurrent, "DEFAULT_TEAM"      ).ID
										// 03/16/2010 Paul.  Add IS_ADMIN_DELEGATE. 
										, ((SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0) ? chkIS_ADMIN_DELEGATE.Checked : Sql.ToBoolean(ViewState["IS_ADMIN_DELEGATE"]) )
										, new DynamicControl(this, rowCurrent, "MAIL_SMTPUSER"     ).Text
										, sMAIL_SMTPPASS
										// 03/04/2011 Paul.  We need to allow the admin to set the flag to force a password change. 
										, chkSYSTEM_GENERATED_PASSWORD.Checked
										// 03/25/2011 Paul.  Add support for Google Apps. 
										, new DynamicControl(this, "GOOGLEAPPS_SYNC_CONTACTS"      ).Checked
										, new DynamicControl(this, "GOOGLEAPPS_SYNC_CALENDAR"      ).Checked
										, new DynamicControl(this, "GOOGLEAPPS_USERNAME"           ).Text
										, sGOOGLEAPPS_PASSWORD
										, new DynamicControl(this, "FACEBOOK_ID"                   ).Text
										// 12/13/2011 Paul.  Add support for Apple iCloud. 
										, new DynamicControl(this, "ICLOUD_SYNC_CONTACTS"          ).Checked
										, new DynamicControl(this, "ICLOUD_SYNC_CALENDAR"          ).Checked
										, new DynamicControl(this, "ICLOUD_USERNAME"               ).Text
										, sICLOUD_PASSWORD
										// 12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
										, lstTHEME.SelectedValue
										, lstDATE_FORMAT.SelectedValue
										, lstTIME_FORMAT.SelectedValue
										, lstLANGUAGE.SelectedValue
										, Sql.ToGuid(lstCURRENCY.SelectedValue)
										, Sql.ToGuid(lstTIMEZONE.SelectedValue)
										, chkSAVE_QUERY.Checked
										, chkGROUP_TABS.Checked
										, chkSUBPANEL_TABS.Checked
										// 09/20/2013 Paul.  Move EXTENSION to the main table. 
										, new DynamicControl(this, "EXTENSION"                     ).Text
										// 09/27/2013 Paul.  SMS messages need to be opt-in. 
										, new DynamicControl(this, "SMS_OPT_IN"                    ).SelectedValue
										// 11/21/2014 Paul.  Add User Picture. 
										, new DynamicControl(this, rowCurrent, "PICTURE"           ).Text
										, trn
										);
									SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
									// 11/27/2009 Paul.  The password field only exists if this is a new user. 
									if ( tdPASSWORD_Label.Visible && (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0) )
									{
										txtPASSWORD.Text = txtPASSWORD.Text.Trim();
										if ( !Sql.IsEmptyString(txtPASSWORD.Text) )
											SqlProcs.spUSERS_PasswordUpdate(gID, Security.HashPassword(txtPASSWORD.Text), trn);
									}

									// 11/11/2008 Paul.  Display an error message if max users has been exceeded. 
									// 02/09/2009 Paul.  We need to check the ActiveUsers in the middle of the transaction. 
									// This is so that a user can be disabled without throwing the max users exception. 
									int nActiveUsers = 0;
									sSQL = "select count(*)          " + ControlChars.CrLf
									     + "  from vwUSERS_List      " + ControlChars.CrLf
									     + " where STATUS = N'Active'" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.Transaction = trn;
										cmd.CommandText = sSQL;
										nActiveUsers = Sql.ToInteger(cmd.ExecuteScalar());
									}
									int nMaxUsers = Sql.ToInteger(Crm.Config.Value("max_users"));
									if ( nMaxUsers > 0 && nActiveUsers > nMaxUsers )
										throw(new Exception(L10n.Term("Users.ERR_MAX_USERS")));

									trn.Commit();
									// 07/18/2013 Paul.  Add support for multiple outbound emails. 
									SplendidCache.ClearOutboundMail();
									// 09/09/2006 Paul.  Refresh cached user information. 
									if ( bNewUser )
										SplendidCache.ClearUsers();
									// 08/27/2005 Paul. Reload session with user preferences. 
									// 08/30/2005 Paul. Only reload preferences the user is editing his own profile. 
									// We want to allow an administrator to update other user profiles. 
									if ( Security.USER_ID == gID )
										SplendidInit.LoadUserPreferences(gID, lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
									// 09/05/2013 Paul.  Use the Application as a cache for the Asterisk extension as we can correct by editing a user. 
									// 09/20/2013 Paul.  Move EXTENSION to the main table. 
									string sEXTENSION = new DynamicControl(this, "EXTENSION").Text;
									if ( !Sql.IsEmptyString(sEXTENSION) )
									{
										Application["Users.EXTENSION." + sEXTENSION + ".USER_ID"] = gID;
										Application["Users.EXTENSION." + sEXTENSION + ".TEAM_ID"] = new DynamicControl(this, rowCurrent, "DEFAULT_TEAM").ID;
									}
									string sPREV_EXTENSION = Sql.ToString(ViewState["EXTENSION"]);
									if ( sEXTENSION != sPREV_EXTENSION && !Sql.IsEmptyString(sPREV_EXTENSION) )
									{
										Application.Remove("Users.EXTENSION." + sPREV_EXTENSION + ".USER_ID");
										Application.Remove("Users.EXTENSION." + sPREV_EXTENSION + ".TEAM_ID");
									}
									// 12/06/2013 Paul.  Update the devices being monitored if the extension has changed. 
									if ( sEXTENSION != sPREV_EXTENSION )
									{
										AvayaManager.Instance.MonitorDevices();
									}
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									ctlDynamicButtons .ErrorText = ex.Message;
									ctlFacebookButtons.ErrorText = ex.Message;
									return;
								}
							}
							// 08/26/2010 Paul.  Add new record to tracker. 
							sSQL = "select FULL_NAME   " + ControlChars.CrLf
							     + "  from vwUSERS_Edit" + ControlChars.CrLf
							     + " where ID = @ID    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								string sNAME = Sql.ToString(cmd.ExecuteScalar());
								// 03/08/2012 Paul.  Add ACTION to the tracker table so that we can create quick user activity reports. 
								SqlProcs.spTRACKER_Update
									( Security.USER_ID
									, m_sMODULE
									, gID
									, sNAME
									, "save"
									);
							}
							// 11/10/2010 Paul.  Apply Business Rules. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + LayoutEditView   , rowCurrent);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + ".EditAddress", rowCurrent);
						}
						
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						else if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
						else if ( bMyAccount )
							Response.Redirect("MyAccount.aspx");
						else
							Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons .ErrorText = ex.Message;
					ctlFacebookButtons.ErrorText = ex.Message;
				}
			}
			// 07/08/2010 Paul.  Provide the ability to test the email settings. 
			else if ( e.CommandName == "Smtp.Test" )
			{
				try
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					TextBox txtEMAIL1        = FindControl("EMAIL1"       ) as TextBox;
					TextBox txtMAIL_SMTPUSER = FindControl("MAIL_SMTPUSER") as TextBox;
					TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
					if ( txtEMAIL1 != null && txtMAIL_SMTPUSER != null && txtMAIL_SMTPPASS != null )
					{
						string sMAIL_SMTPPASS = txtMAIL_SMTPPASS.Text;
						if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
						{
							sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
							if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
								sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							ViewState["mail_smtppass"] = sENCRYPTED_EMAIL_PASSWORD;
							txtMAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						string sSmtpServer      = Sql.ToString (Application["CONFIG.smtpserver"  ]);
						int    nSmtpPort        = Sql.ToInteger(Application["CONFIG.smtpport"    ]);
						bool   bSmtpAuthReq     = Sql.ToBoolean(Application["CONFIG.smtpauth_req"]);
						bool   bSmtpSSL         = Sql.ToBoolean(Application["CONFIG.smtpssl"     ]);
						string sSmtpUser        = txtMAIL_SMTPUSER    .Text;
						string sSmtpPassword    = sMAIL_SMTPPASS;
						string sFromName        = (txtFIRST_NAME.Text + " " + txtLAST_NAME.Text).Trim();
						string sFromAddress     = txtEMAIL1.Text;
						EmailUtils.SendTestMessage(Application, sSmtpServer, nSmtpPort, bSmtpAuthReq, bSmtpSSL, sSmtpUser, sSmtpPassword, sFromAddress, sFromName, sFromAddress, sFromName);
						ctlDynamicButtons .ErrorText = L10n.Term("Users.LBL_SEND_SUCCESSFUL");
						ctlFacebookButtons.ErrorText = L10n.Term("Users.LBL_SEND_SUCCESSFUL");
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons .ErrorText = ex.Message;
					ctlFacebookButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "GoogleApps.Test" )
			{
				try
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					TextBox txtMAIL_SMTPUSER       = FindControl("MAIL_SMTPUSER"      ) as TextBox;
					TextBox txtMAIL_SMTPPASS       = FindControl("MAIL_SMTPPASS"      ) as TextBox;
					TextBox txtGOOGLEAPPS_USERNAME = FindControl("GOOGLEAPPS_USERNAME") as TextBox;
					TextBox txtGOOGLEAPPS_PASSWORD = FindControl("GOOGLEAPPS_PASSWORD") as TextBox;
					if ( txtGOOGLEAPPS_USERNAME != null && txtGOOGLEAPPS_PASSWORD != null )
					{
						string sGOOGLEAPPS_USERNAME = txtGOOGLEAPPS_USERNAME.Text;
						string sGOOGLEAPPS_PASSWORD = txtGOOGLEAPPS_PASSWORD.Text;
						if ( sGOOGLEAPPS_PASSWORD == sEMPTY_PASSWORD )
						{
							sGOOGLEAPPS_PASSWORD = Sql.ToString(ViewState["GOOGLEAPPS_PASSWORD"]);
							if ( !Sql.IsEmptyString(sGOOGLEAPPS_PASSWORD) )
								sGOOGLEAPPS_PASSWORD = Security.DecryptPassword(sGOOGLEAPPS_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else if ( !Sql.IsEmptyString(sGOOGLEAPPS_PASSWORD) )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sGOOGLEAPPS_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							ViewState["GOOGLEAPPS_PASSWORD"] = sENCRYPTED_EMAIL_PASSWORD;
							txtGOOGLEAPPS_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						// 03/25/2011 Paul.  Use SMTP values if the Google values have not been provided. 
						if ( Sql.IsEmptyString(sGOOGLEAPPS_USERNAME) && txtMAIL_SMTPUSER != null )
						{
							sGOOGLEAPPS_USERNAME = txtMAIL_SMTPUSER.Text;
						}
						if ( Sql.IsEmptyString(sGOOGLEAPPS_PASSWORD) && txtMAIL_SMTPPASS != null )
						{
							string sMAIL_SMTPPASS = txtMAIL_SMTPPASS.Text;
							if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
							{
								sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
								if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
									sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							}
							else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
							{
								string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
								ViewState["mail_smtppass"] = sENCRYPTED_EMAIL_PASSWORD;
								txtMAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
							}
							sGOOGLEAPPS_PASSWORD = sMAIL_SMTPPASS;
						}
						
						StringBuilder sbErrors = new StringBuilder();
						GoogleSync.ValidateGoogleApps(Application, txtGOOGLEAPPS_USERNAME.Text, sGOOGLEAPPS_PASSWORD, sbErrors);
						if ( sbErrors.Length > 0 )
						{
							ctlDynamicButtons .ErrorText = sbErrors.ToString();
							ctlFacebookButtons.ErrorText = sbErrors.ToString();
						}
						else
						{
							ctlDynamicButtons .ErrorText = L10n.Term("Users.LBL_GOOGLEAPPS_TEST_SUCCESSFUL");
							ctlFacebookButtons.ErrorText = L10n.Term("Users.LBL_GOOGLEAPPS_TEST_SUCCESSFUL");
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons .ErrorText = ex.Message;
					ctlFacebookButtons.ErrorText = ex.Message;
				}
			}
			// 12/13/2011 Paul.  Add support for Apple iCloud. 
			else if ( e.CommandName == "iCloud.Test" )
			{
				try
				{
					Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
					Guid gINBOUND_EMAIL_IV  = Sql.ToGuid(Application["CONFIG.InboundEmailIV" ]);
					TextBox txtMAIL_SMTPUSER       = FindControl("MAIL_SMTPUSER"      ) as TextBox;
					TextBox txtMAIL_SMTPPASS       = FindControl("MAIL_SMTPPASS"      ) as TextBox;
					TextBox txtICLOUD_USERNAME = FindControl("ICLOUD_USERNAME") as TextBox;
					TextBox txtICLOUD_PASSWORD = FindControl("ICLOUD_PASSWORD") as TextBox;
					if ( txtICLOUD_USERNAME != null && txtICLOUD_PASSWORD != null )
					{
						string sICLOUD_USERNAME = txtICLOUD_USERNAME.Text;
						string sICLOUD_PASSWORD = txtICLOUD_PASSWORD.Text;
						if ( sICLOUD_PASSWORD == sEMPTY_PASSWORD )
						{
							sICLOUD_PASSWORD = Sql.ToString(ViewState["ICLOUD_PASSWORD"]);
							if ( !Sql.IsEmptyString(sICLOUD_PASSWORD) )
								sICLOUD_PASSWORD = Security.DecryptPassword(sICLOUD_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
						}
						else if ( !Sql.IsEmptyString(sICLOUD_PASSWORD) )
						{
							string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sICLOUD_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							ViewState["ICLOUD_PASSWORD"] = sENCRYPTED_EMAIL_PASSWORD;
							txtICLOUD_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
						}
						// 03/25/2011 Paul.  Use SMTP values if the Google values have not been provided. 
						if ( Sql.IsEmptyString(sICLOUD_USERNAME) && txtMAIL_SMTPUSER != null )
						{
							sICLOUD_USERNAME = txtMAIL_SMTPUSER.Text;
						}
						if ( Sql.IsEmptyString(sICLOUD_PASSWORD) && txtMAIL_SMTPPASS != null )
						{
							string sMAIL_SMTPPASS = txtMAIL_SMTPPASS.Text;
							if ( sMAIL_SMTPPASS == sEMPTY_PASSWORD )
							{
								sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
								if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
									sMAIL_SMTPPASS = Security.DecryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
							}
							else if ( !Sql.IsEmptyString(sMAIL_SMTPPASS) )
							{
								string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sMAIL_SMTPPASS, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
								ViewState["mail_smtppass"] = sENCRYPTED_EMAIL_PASSWORD;
								txtMAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
							}
							sICLOUD_PASSWORD = sMAIL_SMTPPASS;
						}
						
						StringBuilder sbErrors = new StringBuilder();
						iCloudSync.Validate_iCloud(Application, txtICLOUD_USERNAME.Text, sICLOUD_PASSWORD, sbErrors);
						if ( sbErrors.Length > 0 )
						{
							ctlDynamicButtons .ErrorText = sbErrors.ToString();
							ctlFacebookButtons.ErrorText = sbErrors.ToString();
						}
						else
						{
							ctlDynamicButtons .ErrorText = L10n.Term("Users.LBL_ICLOUD_TEST_SUCCESSFUL");
							ctlFacebookButtons.ErrorText = L10n.Term("Users.LBL_ICLOUD_TEST_SUCCESSFUL");
						}
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlDynamicButtons .ErrorText = ex.Message;
					ctlFacebookButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				else if ( bMyAccount )
					Response.Redirect("MyAccount.aspx");
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void lstLANGUAGE_Changed(Object sender, EventArgs e)
		{
			if ( lstLANGUAGE.SelectedValue.Length > 0 )
			{
				CultureInfo oldCulture   = Thread.CurrentThread.CurrentCulture   ;
				CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture ;
				Thread.CurrentThread.CurrentCulture   = CultureInfo.CreateSpecificCulture(lstLANGUAGE.SelectedValue);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(lstLANGUAGE.SelectedValue);

				DateTime dtNow = T10n.FromServerTime(DateTime.Now);
				DateTimeFormatInfo oDateInfo   = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				NumberFormatInfo   oNumberInfo = Thread.CurrentThread.CurrentCulture.NumberFormat  ;

				String[] aDateTimePatterns = oDateInfo.GetAllDateTimePatterns();

				lstDATE_FORMAT.Items.Clear();
				lstTIME_FORMAT.Items.Clear();
				foreach ( string sPattern in aDateTimePatterns )
				{
					// 11/12/2005 Paul.  Only allow patterns that have a full year. 
					// 10/15/2013 Paul.  Allow 2-digit year. 
					if ( sPattern.IndexOf("yy") >= 0 && sPattern.IndexOf("dd") >= 0 && sPattern.IndexOf("mm") <  0 )
						lstDATE_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
					if ( sPattern.IndexOf("yy") <  0 && sPattern.IndexOf("mm") >= 0 )
						lstTIME_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
				}
				Thread.CurrentThread.CurrentCulture = oldCulture  ;
				Thread.CurrentThread.CurrentCulture = oldUICulture;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 07/11/2006 Paul.  Users must be able to view and edit their own settings. 
			// 03/10/2010 Paul.  Apply full ACL security rules. 
			this.Visible = bMyAccount || (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
			{
				// 03/17/2010 Paul.  We need to rebind the parent in order to get the error message to display. 
				Parent.DataBind();
				return;
			}

			reqUSER_NAME.DataBind();
			reqLAST_NAME.DataBind();
			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( bMyAccount )
				{
					gID = Security.USER_ID;
				}
				// 07/12/2006 Paul.  Status can only be edited by an administrator. 
				lstSTATUS.Enabled = false;
				// 12/06/2005 Paul.  A user can only edit his own user name if Windows Authentication is off. 
				if ( SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 )
				{
					// 12/06/2005 Paul.  An administrator can always edit the user name.  This is to allow him to pre-add any NTLM users. 
					txtUSER_NAME.Enabled = true;
					lstSTATUS.Enabled = true;
				}
				else if ( gID == Security.USER_ID )
				{
					// 12/06/2005 Paul.  If editing yourself, then you can only edit if not NTLM. 
					// txtUSER_NAME.Enabled = !Security.IsWindowsAuthentication();
					// 11/26/2006 Paul.  A user cannot edit their own user name. This is a job for the admin. 
					txtUSER_NAME.Enabled = false;
				}
				else
				{
					// 12/06/2005 Paul.  If not an administrator and not editing yourself, then the name cannot be edited. 
					txtUSER_NAME.Enabled = false;
				}

				if ( !IsPostBack )
				{
					// 'date_formats' => array('Y-m-d'=>'2006-12-23', 'm-d-Y'=>'12-23-2006', 'Y/m/d'=>'2006/12/23', 'm/d/Y'=>'12/23/2006')
					// 'time_formats' => array('H:i'=>'23:00', 'h:ia'=>'11:00pm', 'h:iA'=>'11:00PM', 'H.i'=>'23.00', 'h.ia'=>'11.00pm', 'h.iA'=>'11.00PM' )
					lstSTATUS         .DataSource = SplendidCache.List("user_status_dom");
					lstSTATUS         .DataBind();
					// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
					//lstREMINDER_TIME  .DataSource = SplendidCache.List("reminder_time_dom");
					//lstREMINDER_TIME  .DataBind();
					lstTIMEZONE       .DataSource = SplendidCache.TimezonesListbox();
					lstTIMEZONE       .DataBind();
					lstCURRENCY       .DataSource = SplendidCache.Currencies();
					lstCURRENCY       .DataBind();

					lstLANGUAGE.DataSource = SplendidCache.Languages();
					lstLANGUAGE.DataBind();
					lstLANGUAGE_Changed(null, null);
					lstTHEME.DataSource = SplendidCache.Themes();
					lstTHEME.DataBind();
					// 05/06/2009 Paul.  Add DEFAULT_TEAM to support SugarCRM migration. 
					tdDEFAULT_TEAM_Label.Visible = Crm.Config.enable_team_management();
					tdDEFAULT_TEAM_Field.Visible = tdDEFAULT_TEAM_Label.Visible;
					// 03/19/2011 Paul.  Facebook button should not be visible on an offline client. 
					ctlFacebookButtons.Visible = !PrintView && !Utils.IsOfflineClient && Sql.ToBoolean(Application["CONFIG.facebook.EnableLogin"]) && !Sql.IsEmptyString(Application["CONFIG.facebook.AppID"]);
					ctlDynamicButtons .Visible = !PrintView && !ctlFacebookButtons.Visible;

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwUSERS_Edit" + ControlChars.CrLf
							     + " where ID = @ID    " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								// 11/22/2010 Paul.  Convert data reader to data table for Rules Wizard. 
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dtCurrent = new DataTable() )
									{
										da.Fill(dtCurrent);
										if ( dtCurrent.Rows.Count > 0 )
										{
											DataRow rdr = dtCurrent.Rows[0];
											// 11/11/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPreLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
											
											ctlModuleHeader.Title = Sql.ToString(rdr["FULL_NAME"]);
											SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title + " (" + Sql.ToString(rdr["USER_NAME"]) + ")");
											Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
											ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
											
											this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , rdr);
											// 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
											//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , rdr);
											// 08/05/2006 Paul.  Use the dynamic grid to create the fields, but populate manually. 
											this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);
											// 03/25/2011 Paul.  Add support for Google Apps. The fields will be manually populated to prevent the password from getting to the browser. 
											this.AppendEditViewFields(m_sMODULE + ".EditGoogleAppsOptions", tblGoogleAppsOptions, null);
											tblGoogleAppsPanel.Visible = (tblGoogleAppsOptions.Rows.Count > 1) && Sql.ToBoolean(Context.Application["CONFIG.GoogleApps.Enabled"]);
											// 12/13/2011 Paul.  Add support for Apple iCloud. The fields will be manually populated to prevent the password from getting to the browser. 
											this.AppendEditViewFields(m_sMODULE + ".EditICloudOptions", tblICloudOptions, null);
											tblICloudPanel.Visible = (tblICloudOptions.Rows.Count > 1) && Sql.ToBoolean(Context.Application["CONFIG.iCloud.Enabled"]);
											
											// 03/28/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
											// 11/29/2008 Paul   Dynamic buttons don't work well for user admin. 
											//ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, rdr);
											//ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, rdr);

											// 01/20/2008 Paul.  The mail options panel is manually populated. 
											new DynamicControl(this, "EMAIL1").Text = Sql.ToString (rdr["EMAIL1"]);
											new DynamicControl(this, "EMAIL2").Text = Sql.ToString (rdr["EMAIL2"]);
											// 05/06/2009 Paul.  Add DEFAULT_TEAM to support SugarCRM migration. 
											new DynamicControl(this, "DEFAULT_TEAM"     ).Text = Sql.ToString (rdr["DEFAULT_TEAM"     ]);
											new DynamicControl(this, "DEFAULT_TEAM_NAME").Text = Sql.ToString (rdr["DEFAULT_TEAM_NAME"]);

											// main
											txtUSER_NAME            .Text    = Sql.ToString (rdr["USER_NAME"            ]);
											txtFIRST_NAME           .Text    = Sql.ToString (rdr["FIRST_NAME"           ]);
											txtLAST_NAME            .Text    = Sql.ToString (rdr["LAST_NAME"            ]);
											// 11/21/2014 Paul.  Add User Picture. 
											PICTURE                 .Value   = Sql.ToString (rdr["PICTURE"              ]);
											imgPICTURE              .Src     = Sql.ToString (rdr["PICTURE"              ]);
											// user_settings
											chkIS_ADMIN             .Checked = Sql.ToBoolean(rdr["IS_ADMIN"             ]);
											// 03/16/2010 Paul.  Add IS_ADMIN_DELEGATE. 
											chkIS_ADMIN_DELEGATE    .Checked = Sql.ToBoolean(rdr["IS_ADMIN_DELEGATE"    ]);
											chkPORTAL_ONLY          .Checked = Sql.ToBoolean(rdr["PORTAL_ONLY"          ]);
											chkRECEIVE_NOTIFICATIONS.Checked = Sql.ToBoolean(rdr["RECEIVE_NOTIFICATIONS"]);
											try
											{
												// 03/04/2011 Paul.  We need to allow the admin to set the flag to force a password change. 
												chkSYSTEM_GENERATED_PASSWORD.Checked = Sql.ToBoolean(rdr["SYSTEM_GENERATED_PASSWORD"]);
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), "SYSTEM_GENERATED_PASSWORD is not defined. " + ex.Message);
											}
											// 12/04/2005 Paul.  Only allow the admin flag to be changed if the current user is an admin. 
											chkIS_ADMIN.Enabled = Security.IS_ADMIN;
											// 03/16/2010 Paul.  Add IS_ADMIN_DELEGATE. 
											chkIS_ADMIN_DELEGATE.Enabled = (SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0);
											// 12/04/2005 Paul.  Save admin flag in ViewState to prevent hacking. 
											ViewState["IS_ADMIN"] = Sql.ToBoolean(rdr["IS_ADMIN"]);
											ViewState["IS_ADMIN_DELEGATE"] = Sql.ToBoolean(rdr["IS_ADMIN_DELEGATE"]);

											try
											{
												// 08/19/2010 Paul.  Check the list before assigning the value. 
												Utils.SetSelectedValue(lstSTATUS, Sql.ToString (rdr["STATUS"               ]));
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											
											//12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
											/*
											string sUSER_PREFERENCES = Sql.ToString(rdr["USER_PREFERENCES"]);
											if ( !Sql.IsEmptyString(sUSER_PREFERENCES) )
											{
												XmlDocument xml = SplendidInit.InitUserPreferences(sUSER_PREFERENCES);
												try
												{
													ViewState["USER_PREFERENCES"] = xml.OuterXml;
													// user_settings
													// 08/12/2009 Paul.  A customer wants the ability to turn off the saved searches, both globally and on a per user basis. 
													chkSAVE_QUERY.Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "save_query"));
													// 02/26/2010 Paul.  Allow users to configure use of tabs. 
													chkGROUP_TABS.Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "group_tabs"));
													chkSUBPANEL_TABS.Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "subpanel_tabs"));
													try
													{
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetSelectedValue(lstLANGUAGE, L10N.NormalizeCulture(XmlUtil.SelectSingleNode(xml, "culture")));
														lstLANGUAGE_Changed(null, null);
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													try
													{
														// 02/29/2008 Paul.  Theme was not being set properly.  We were setting the language to the theme. 
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetSelectedValue(lstTHEME, XmlUtil.SelectSingleNode(xml, "theme"));
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													try
													{
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetSelectedValue(lstDATE_FORMAT, XmlUtil.SelectSingleNode(xml, "dateformat"));
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													try
													{
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetSelectedValue(lstTIME_FORMAT, XmlUtil.SelectSingleNode(xml, "timeformat"));
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													try
													{
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetValue(lstTIMEZONE, XmlUtil.SelectSingleNode(xml, "timezone"));
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													try
													{
														// 08/19/2010 Paul.  Check the list before assigning the value. 
														Utils.SetValue(lstCURRENCY, XmlUtil.SelectSingleNode(xml, "currency_id"));
													}
													catch(Exception ex)
													{
														SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
													}
													
													// mail_options
													// 07/08/2010 Paul.  The user must share the global mail server, so all we need here is the user name and password. 
													//new DynamicControl(this, "MAIL_FROMNAME"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromname"        );
													//new DynamicControl(this, "MAIL_FROMADDRESS" ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromaddress"     );
													//new DynamicControl(this, "MAIL_SENDTYPE"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_sendtype"        );
													//new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpserver"      );
													//new DynamicControl(this, "MAIL_SMTPPORT"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpport"        );
													//new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "mail_smtpauth_req"    ));
												}
												catch(Exception ex)
												{
													SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
												}
											}
											*/
											//12/15/2012 Paul.  Move USER_PREFERENCES to separate fields for easier access on Surface RT. 
											try
											{
												// user_settings
												// 08/12/2009 Paul.  A customer wants the ability to turn off the saved searches, both globally and on a per user basis. 
												chkSAVE_QUERY   .Checked = Sql.ToBoolean(Sql.ToString(rdr["SAVE_QUERY"   ]));
												// 02/26/2010 Paul.  Allow users to configure use of tabs. 
												chkGROUP_TABS   .Checked = Sql.ToBoolean(Sql.ToString(rdr["GROUP_TABS"   ]));
												chkSUBPANEL_TABS.Checked = Sql.ToBoolean(Sql.ToString(rdr["SUBPANEL_TABS"]));
												try
												{
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetSelectedValue(lstLANGUAGE, L10N.NormalizeCulture(Sql.ToString(rdr["LANG"])));
													lstLANGUAGE_Changed(null, null);
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													// 02/29/2008 Paul.  Theme was not being set properly.  We were setting the language to the theme. 
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetSelectedValue(lstTHEME, Sql.ToString(rdr["THEME"]));
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetSelectedValue(lstDATE_FORMAT, Sql.ToString(rdr["DATE_FORMAT"]));
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetSelectedValue(lstTIME_FORMAT, Sql.ToString(rdr["TIME_FORMAT"]));
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetValue(lstTIMEZONE, Sql.ToString(rdr["TIMEZONE_ID"]));
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													// 08/19/2010 Paul.  Check the list before assigning the value. 
													Utils.SetValue(lstCURRENCY, Sql.ToString(rdr["CURRENCY_ID"]));
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
											// 07/09/2010 Paul.  Move the SMTP values from USER_PREFERENCES to the main table to make it easier to access. 
											try
											{
												// 07/09/2010 Paul.  Move the SMTP values from USER_PREFERENCES to the main table to make it easier to access. 
												new DynamicControl(this, "MAIL_SMTPUSER"    ).Text    =               Sql.ToString (rdr["MAIL_SMTPUSER"               ]);
												//new DynamicControl(this, "MAIL_SMTPPASS"    ).Text    =               Sql.ToString (rdr["MAIL_SMTPPASS"               ]);
												
												string sMAIL_SMTPPASS = Sql.ToString (rdr["MAIL_SMTPPASS"]);
												ViewState["mail_smtppass"] = sMAIL_SMTPPASS;
												// 08/06/2005 Paul.  Never return password to user. 
												TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
												btnSmtpTest.Visible = (txtMAIL_SMTPPASS != null);
												if ( txtMAIL_SMTPPASS != null && !Sql.IsEmptyString(sMAIL_SMTPPASS) )
												{
													// txtMAIL_SMTPPASS.Text = sEMPTY_PASSWORD;
													txtMAIL_SMTPPASS.Attributes.Add("value", sEMPTY_PASSWORD);
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											// 03/25/2011 Paul.  Add support for Google Apps. 
											try
											{
												new DynamicControl(this, "GOOGLEAPPS_SYNC_CONTACTS").Checked = Sql.ToBoolean(rdr["GOOGLEAPPS_SYNC_CONTACTS"]);
												new DynamicControl(this, "GOOGLEAPPS_SYNC_CALENDAR").Checked = Sql.ToBoolean(rdr["GOOGLEAPPS_SYNC_CALENDAR"]);
												new DynamicControl(this, "GOOGLEAPPS_USERNAME"     ).Text    = Sql.ToString (rdr["GOOGLEAPPS_USERNAME"     ]);
												//new DynamicControl(this, "GOOGLEAPPS_PASSWORD"     ).Text    = Sql.ToString (rdr["GOOGLEAPPS_PASSWORD"     ]);
												
												string sGOOGLEAPPS_PASSWORD = Sql.ToString (rdr["GOOGLEAPPS_PASSWORD"]);
												ViewState["GOOGLEAPPS_PASSWORD"] = sGOOGLEAPPS_PASSWORD;
												// 03/25/2011 Paul.  Never return password to user. 
												TextBox txtGOOGLEAPPS_PASSWORD = FindControl("GOOGLEAPPS_PASSWORD") as TextBox;
												if ( txtGOOGLEAPPS_PASSWORD != null && !Sql.IsEmptyString(sGOOGLEAPPS_PASSWORD) )
												{
													txtGOOGLEAPPS_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											// 12/13/2011 Paul.  Add support for Apple iCloud. 
											try
											{
												new DynamicControl(this, "ICLOUD_SYNC_CONTACTS").Checked = Sql.ToBoolean(rdr["ICLOUD_SYNC_CONTACTS"]);
												new DynamicControl(this, "ICLOUD_SYNC_CALENDAR").Checked = Sql.ToBoolean(rdr["ICLOUD_SYNC_CALENDAR"]);
												new DynamicControl(this, "ICLOUD_USERNAME"     ).Text    = Sql.ToString (rdr["ICLOUD_USERNAME"     ]);
												//new DynamicControl(this, "ICLOUD_PASSWORD"     ).Text    = Sql.ToString (rdr["ICLOUD_PASSWORD"     ]);
												
												string sICLOUD_PASSWORD = Sql.ToString (rdr["ICLOUD_PASSWORD"]);
												ViewState["ICLOUD_PASSWORD"] = sICLOUD_PASSWORD;
												// 12/13/2011 Paul.  Never return password to user. 
												TextBox txtICLOUD_PASSWORD = FindControl("ICLOUD_PASSWORD") as TextBox;
												if ( txtICLOUD_PASSWORD != null && !Sql.IsEmptyString(sICLOUD_PASSWORD) )
												{
													txtICLOUD_PASSWORD.Attributes.Add("value", sEMPTY_PASSWORD);
												}
											}
											catch(Exception ex)
											{
												SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
											}
											// 12/09/2008 Paul.  Throw an exception if the record has been edited since the last load. 
											ViewState["LAST_DATE_MODIFIED"] = Sql.ToDateTime(rdr["DATE_MODIFIED"]);
											// 09/05/2013 Paul.  Use the Application as a cache for the Asterisk extension as we can correct by editing a user. 
											// 09/20/2013 Paul.  Move EXTENSION to the main table. 
											ViewState["EXTENSION"] = Sql.ToString(rdr["EXTENSION"]);
											// 11/10/2010 Paul.  Apply Business Rules. 
											this.ApplyEditViewPostLoadEventRules(m_sMODULE + "." + LayoutEditView, rdr);
										}
									}
								}
							}
						}
					}
					else
					{
						// 11/27/2009 Paul.  The password field only exists if this is a new user. 
						tdPASSWORD_Label.Visible = true;
						tdPASSWORD_Field.Visible = true;

						// 11/11/2008 Paul.  Display an error message if max users has been exceeded. 
						int nActiveUsers = Crm.Users.ActiveUsers();
						int nMaxUsers = Sql.ToInteger(Crm.Config.Value("max_users"));
						if ( nMaxUsers > 0 && nActiveUsers > nMaxUsers )
						{
							ctlDynamicButtons .ErrorText = L10n.Term("Users.ERR_MAX_USERS");
							ctlFacebookButtons.ErrorText = L10n.Term("Users.ERR_MAX_USERS");
						}

						this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , null);
						// 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
						//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
						this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);
						// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
						// 11/29/2008 Paul   Dynamic buttons don't work well for user admin. 
						//ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						//ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
						TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
						btnSmtpTest.Visible = (txtMAIL_SMTPPASS != null);

						try
						{
							// 08/19/2010 Paul.  Check the list before assigning the value. 
							Utils.SetSelectedValue(this.lstTHEME, SplendidDefaults.Theme());
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							// 11/27/2009 Paul.  The default language should pull from Config. 
							// 11/27/2009 Paul.  Make sure to normalize as the default format may be 'en_us'. 
							string sDefault = L10N.NormalizeCulture(Sql.ToString(Application["CONFIG.default_language"]));
							if ( Sql.IsEmptyString(sDefault) )
								sDefault = "en-US";
							// 08/19/2010 Paul.  Check the list before assigning the value. 
							Utils.SetSelectedValue(lstLANGUAGE, sDefault);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							lstLANGUAGE_Changed(null, null);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						// 11/27/2009 Paul.  Make sure that the language is first as it can change the date and time formats. 
						try
						{
							// 11/27/2009 Paul.  The default date format should pull from Config. 
							string sDefault = Sql.ToString(Application["CONFIG.default_date_format"]);
							if ( !Sql.IsEmptyString(sDefault) )
								// 08/19/2010 Paul.  Check the list before assigning the value. 
								Utils.SetSelectedValue(this.lstDATE_FORMAT, sDefault);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							// 11/27/2009 Paul.  The default time format should pull from Config. 
							string sDefault = Sql.ToString(Application["CONFIG.default_time_format"]);
							if ( !Sql.IsEmptyString(sDefault) )
								// 08/19/2010 Paul.  Check the list before assigning the value. 
								Utils.SetSelectedValue(this.lstTIME_FORMAT, sDefault);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							// 11/27/2009 Paul.  The default time zone should pull from Config. 
							string sDefault = Sql.ToString(Application["CONFIG.default_timezone"]);
							if ( !Sql.IsEmptyString(sDefault) )
								// 08/19/2010 Paul.  Check the list before assigning the value. 
								Utils.SetValue(this.lstTIMEZONE, sDefault.ToLower());
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							// 11/27/2009 Paul.  The default currency should pull from Config. 
							string sDefault = Sql.ToString(Application["CONFIG.default_currency"]);
							if ( !Sql.IsEmptyString(sDefault) )
								// 08/19/2010 Paul.  Check the list before assigning the value. 
								Utils.SetValue(this.lstCURRENCY, sDefault.ToLower());
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						// 02/27/2010 Paul.  For a new user, the Group Tabs and SubPanel Tabs flags may not have been saved, so manually include. 
						// 02/11/2010 Paul.  Set the default value for Save Query. 
						chkSAVE_QUERY   .Checked = Sql.ToBoolean(Application["CONFIG.save_query"   ]) || Sql.ToBoolean(Session["USER_SETTINGS/SAVE_QUERY"      ]);
						// 02/26/2010 Paul.  Allow users to configure use of tabs. 
						chkGROUP_TABS   .Checked = Sql.ToBoolean(Application["CONFIG.group_tabs"   ]) || Sql.ToBoolean(Session["USER_SETTINGS/GROUP_TABS"      ]);
						chkSUBPANEL_TABS.Checked = Sql.ToBoolean(Application["CONFIG.subpanel_tabs"]) || Sql.ToBoolean(Session["USER_SETTINGS/SUBPANEL_TABS"   ]);
						// 11/10/2010 Paul.  Apply Business Rules. 
						this.ApplyEditViewNewEventRules(m_sMODULE + "." + LayoutEditView);
						this.ApplyEditViewNewEventRules(m_sMODULE + ".EditMailOptions");
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlDynamicButtons .ErrorText = ex.Message;
				ctlFacebookButtons.ErrorText = ex.Message;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlDynamicButtons .Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons  .Command += new CommandEventHandler(Page_Command);
			ctlFacebookButtons.Command += new CommandEventHandler(Page_Command);
			m_sMODULE = "Users";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + "." + LayoutEditView, tblMain       , null);
				// 07/08/2010 Paul.  Move Users.EditAddress fields to Users.EditView
				//this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
				this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);
				// 03/20/2008 Paul.  Dynamic buttons need to be recreated in order for events to fire. 
				// 11/29/2008 Paul   Dynamic buttons don't work well for user admin. 
				//ctlDynamicButtons.AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				//ctlFooterButtons .AppendButtons(m_sMODULE + "." + LayoutEditView, Guid.Empty, null);
				// 11/10/2010 Paul.  Make sure to add the RulesValidator early in the pipeline. 
				// 03/25/2011 Paul.  Add support for Google Apps. 
				this.AppendEditViewFields(m_sMODULE + ".EditGoogleAppsOptions", tblGoogleAppsOptions, null);
				tblGoogleAppsPanel.Visible = (tblGoogleAppsOptions.Rows.Count > 1) && Sql.ToBoolean(Context.Application["CONFIG.GoogleApps.Enabled"]);
				// 12/13/2011 Paul.  Add support for Apple iCloud. 
				this.AppendEditViewFields(m_sMODULE + ".EditICloudOptions", tblICloudOptions, null);
				tblICloudPanel.Visible = (tblICloudOptions.Rows.Count > 1) && Sql.ToBoolean(Context.Application["CONFIG.iCloud.Enabled"]);
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}


