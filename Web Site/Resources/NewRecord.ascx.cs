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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Resources
{
	/// <summary>
	///		Summary description for New.
	/// </summary>
	public class NewRecord : NewRecordControl
	{
		protected _controls.DynamicButtons ctlDynamicButtons;
		protected _controls.DynamicButtons ctlFooterButtons ;
		protected _controls.HeaderLeft     ctlHeaderLeft    ;

		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected Label           lblError                        ;
		protected Panel           pnlMain                         ;
		protected Panel           pnlEdit                         ;

		// 05/06/2010 Paul.  We need a common way to attach a command from the Toolbar. 

		public Guid PARTNER_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gPARTNER_ID = Sql.ToGuid(ViewState["PARTNER_ID"]);
				if ( Sql.IsEmptyGuid(gPARTNER_ID) )
					gPARTNER_ID = Sql.ToGuid(Request["PARTNER_ID"]);
				return gPARTNER_ID;
			}
			set
			{
				ViewState["PARTNER_ID"] = value;
			}
		}

		// 08/07/2015 Paul.  Add Leads/Resources relationship. 
		public Guid LEAD_ID
		{
			get
			{
				// 02/21/2010 Paul.  An EditView Inline will use the ViewState, and a NewRecord Inline will use the Request. 
				Guid gLEAD_ID = Sql.ToGuid(ViewState["LEAD_ID"]);
				if ( Sql.IsEmptyGuid(gLEAD_ID) )
					gLEAD_ID = Sql.ToGuid(Request["LEAD_ID"]);
				return gLEAD_ID;
			}
			set
			{
				ViewState["LEAD_ID"] = value;
			}
		}

		// 03/13/2014 Paul.  Make sure to pass Reports To from Direct Reports subpanel. 
		public Guid REPORTS_TO_ID
		{
			get
			{
				Guid gREPORTS_TO_ID = Sql.ToGuid(ViewState["REPORTS_TO_ID"]);
				if ( Sql.IsEmptyGuid(gREPORTS_TO_ID) )
					gREPORTS_TO_ID = Sql.ToGuid(Request["REPORTS_TO_ID"]);
				return gREPORTS_TO_ID;
			}
			set
			{
				ViewState["REPORTS_TO_ID"] = value;
			}
		}

		// 05/05/2010 Paul.  We need a common way to access the parent from the Toolbar. 

		public Guid CALL_ID
		{
			get { return Sql.ToGuid(ViewState["CALL_ID"]); }
			set { ViewState["CALL_ID"] = value; }
		}

		public Guid MEETING_ID
		{
			get { return Sql.ToGuid(ViewState["MEETING_ID"]); }
			set { ViewState["MEETING_ID"] = value; }
		}

		// 04/20/2010 Paul.  Add functions to allow this control to be used as part of an InlineEdit operation. 
		public override bool IsEmpty()
		{
            //TD: LAST_NAME can be empty!
			string sNAME = new DynamicControl(this, "FIRST_NAME").Text;
			return Sql.IsEmptyString(sNAME);
		}

		public override void ValidateEditViewFields()
		{
			if ( !IsEmpty() )
			{
				this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
			}
		}

		public override void Save(Guid gPARENT_ID, string sPARENT_TYPE, IDbTransaction trn)
		{
			if ( IsEmpty() )
				return;
			
			string    sTABLE_NAME    = Crm.Modules.TableName(m_sMODULE);
			DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sTABLE_NAME);
			
			Guid gASSIGNED_USER_ID = new DynamicControl(this, "ASSIGNED_USER_ID").ID;
			Guid gTEAM_ID          = new DynamicControl(this, "TEAM_ID"         ).ID;
			Guid gPARTNER_ID       = new DynamicControl(this, "PARTNER_ID"      ).ID;
			Guid gCALL_ID          = this.CALL_ID   ;
			Guid gMEETING_ID       = this.MEETING_ID;
			if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
				gASSIGNED_USER_ID = Security.USER_ID;
			if ( Sql.IsEmptyGuid(gTEAM_ID) )
				gTEAM_ID = Security.TEAM_ID;
			if ( Sql.IsEmptyGuid(gPARTNER_ID) )
				gPARTNER_ID = this.PARTNER_ID;
			if ( sPARENT_TYPE == "Partners" && !Sql.IsEmptyGuid(gPARENT_ID) )
				gPARTNER_ID = gPARENT_ID;
			// 03/13/2014 Paul.  Make sure to pass Reports To from Direct Reports subpanel. 
			Guid gREPORTS_TO_ID    = new DynamicControl(this, "REPORTS_TO_ID"   ).ID;
			if ( Sql.IsEmptyGuid(gREPORTS_TO_ID) )
				gREPORTS_TO_ID = this.REPORTS_TO_ID;


			SqlProcs.spRESOURCES_Update
				( ref gID
				, gASSIGNED_USER_ID
				, new DynamicControl(this, "SALUTATION"                ).SelectedValue
				, new DynamicControl(this, "FIRST_NAME"                ).Text
				, new DynamicControl(this, "LAST_NAME"                 ).Text
				, gPARTNER_ID
				, new DynamicControl(this, "TITLE"                     ).Text
				, new DynamicControl(this, "DEPARTMENT"                ).Text
				, gREPORTS_TO_ID
				, new DynamicControl(this, "BIRTHDATE"                 ).DateValue
				, new DynamicControl(this, "DO_NOT_CALL"               ).Checked
				, new DynamicControl(this, "PHONE_HOME"                ).Text
				, new DynamicControl(this, "PHONE_MOBILE"              ).Text
				, new DynamicControl(this, "PHONE_WORK"                ).Text
				, new DynamicControl(this, "PHONE_OTHER"               ).Text
				, new DynamicControl(this, "PHONE_FAX"                 ).Text
				, new DynamicControl(this, "EMAIL1"                    ).Text
				, new DynamicControl(this, "EMAIL2"                    ).Text
				, new DynamicControl(this, "PRIMARY_ADDRESS_STREET"    ).Text
				, new DynamicControl(this, "PRIMARY_ADDRESS_CITY"      ).Text
				, new DynamicControl(this, "PRIMARY_ADDRESS_STATE"     ).Text
				, new DynamicControl(this, "PRIMARY_ADDRESS_POSTALCODE").Text
				, new DynamicControl(this, "PRIMARY_ADDRESS_COUNTRY"   ).Text
				, new DynamicControl(this, "DESCRIPTION"               ).Text
				, sPARENT_TYPE
				, gPARENT_ID
				, new DynamicControl(this, "SYNC_CONTACT"              ).Checked
				, gTEAM_ID
				, new DynamicControl(this, "TEAM_SET_LIST"             ).Text
				// 08/07/2015 Paul.  Add picture. 
				, new DynamicControl(this, "PICTURE"                   ).Text
				// 09/27/2015 Paul.  Separate SYNC_CONTACT and EXCHANGE_FOLDER. 
				, new DynamicControl(this, "EXCHANGE_FOLDER"           ).Checked
				// 05/12/2016 Paul.  Add Tags module. 
				, new DynamicControl(this, "TAG_SET_NAME"              ).Text
				// 06/20/2017 Paul.  Add number fields to Resources, Leads, Prospects, Requests and Campaigns. 
				, new DynamicControl(this, "RESOURCE_NUMBER"            ).Text
				// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
				, new DynamicControl(this, "ASSIGNED_SET_LIST"         ).Text
                , new DynamicControl(this, "SKYPE").Text
                , new DynamicControl(this, "CVTOOL_LINK").Text
                , new DynamicControl(this, "ENGAGED").Checked
                , new DynamicControl(this, "ENGAGED_TILL").DateValue
                , new DynamicControl(this, "ENGAGED_JIRAPROJECT").Text
                , new DynamicControl(this, "SCNSOFT_ACCOUNT").Text
                , trn
				);
			SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
			
			// 04/20/2010 Paul.  For those procedures that do not include a PARENT_TYPE, 
			// we need a new relationship procedure. 
			SqlProcs.spRESOURCES_InsRelated(gID, sPARENT_TYPE, gPARENT_ID, trn);
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "NewRecord" )
				{
					// 06/20/2009 Paul.  Use a Dynamic View that is nearly idential to the EditView version. 
					this.ValidateEditViewFields(m_sMODULE + "." + sEditView);
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewValidationEventRules(m_sMODULE + "." + sEditView);
					if ( Page.IsValid )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							con.Open();
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							this.ApplyEditViewPreSaveEventRules(m_sMODULE + "." + sEditView, null);
							
							// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
							using ( IDbTransaction trn = Sql.BeginTransaction(con) )
							{
								try
								{
									Guid   gPARENT_ID   = new DynamicControl(this, "PARENT_ID"  ).ID;
									// 02/04/2011 Paul.  We gave the PARENT_TYPE a unique name, but we need to update all EditViews and NewRecords. 
									string sPARENT_TYPE = new DynamicControl(this, "PARENT_ID_PARENT_TYPE").SelectedValue;
									if ( Sql.IsEmptyGuid(gPARENT_ID) )
										gPARENT_ID = this.PARENT_ID;
									// 07/14/2010 Paul.  We should be checking the sPARENT_TYPE value and not the ViewState value. 
									if ( Sql.IsEmptyString(sPARENT_TYPE) )
										sPARENT_TYPE = this.PARENT_TYPE;
									Save(gPARENT_ID, sPARENT_TYPE, trn);
									trn.Commit();
								}
								catch(Exception ex)
								{
									trn.Rollback();
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
									if ( bShowFullForm || bShowCancel )
										ctlFooterButtons.ErrorText = ex.Message;
									else
										lblError.Text = ex.Message;
									return;
								}
							}
							// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
							// 12/10/2012 Paul.  Provide access to the item data. 
							DataRow rowCurrent = Crm.Modules.ItemEdit(m_sMODULE, gID);
							this.ApplyEditViewPostSaveEventRules(m_sMODULE + "." + sEditView, rowCurrent);
						}
						if ( !Sql.IsEmptyString(RulesRedirectURL) )
							Response.Redirect(RulesRedirectURL);
						// 02/21/2010 Paul.  An error should not forward the command so that the error remains. 
						// In case of success, send the command so that the page can be rebuilt. 
						// 06/02/2010 Paul.  We need a way to pass the ID up the command chain. 
						else if ( Command != null )
							Command(sender, new CommandEventArgs(e.CommandName, gID.ToString()));
						else if ( !Sql.IsEmptyGuid(gID) )
							Response.Redirect("~/" + m_sMODULE + "/view.aspx?ID=" + gID.ToString());
					}
				}
				else if ( Command != null )
				{
					Command(sender, e);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
					lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// 06/04/2006 Paul.  NewRecord should not be displayed if the user does not have edit rights. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
				bool bIsPostBack = this.IsPostBack && !NotPostBack;
				if ( !bIsPostBack )
				{
					// 05/06/2010 Paul.  When the control is created out-of-band, we need to manually bind the controls. 
					if ( NotPostBack )
						this.DataBind();
					// 02/21/2010 Paul.  When used in a SubPanel, this line does not get executed because 
					// the Page_Load happens after the user as clicked Create, which is a PostBack event. 
					this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
					// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
					if ( EditViewLoad != null )
						EditViewLoad(this, null);
					
					// 02/21/2010 Paul.  When the Full Form buttons are used, we don't want the panel to have margins. 
					if ( bShowFullForm || bShowCancel || sEditView != "NewRecord" )
					{
						pnlMain.CssClass = "";
						pnlEdit.CssClass = "tabForm";
						
						Guid gPARENT_ID = this.PARTNER_ID;
						// 05/05/2010 Paul.  The Toolbar will only set the Parent, so we need to populate with this value. 
						// 04/03/2013 Paul.  Simplify the logic so that there is only one code path to initialize the partner data. 
						if ( Sql.IsEmptyGuid(gPARENT_ID) )
							gPARENT_ID = this.PARENT_ID;
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
							string sMODULE           = String.Empty;
							string sPARENT_TYPE      = String.Empty;
							string sPARENT_NAME      = String.Empty;
							Guid   gASSIGNED_USER_ID = Guid.Empty;
							string sASSIGNED_TO      = String.Empty;
							string sASSIGNED_TO_NAME = String.Empty;
							Guid   gTEAM_ID          = Guid.Empty;
							string sTEAM_NAME        = String.Empty;
							Guid   gTEAM_SET_ID      = Guid.Empty;
							// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
							Guid   gASSIGNED_SET_ID  = Guid.Empty;
							SqlProcs.spPARENT_GetWithTeam(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME, ref gASSIGNED_USER_ID, ref sASSIGNED_TO, ref sASSIGNED_TO_NAME, ref gTEAM_ID, ref sTEAM_NAME, ref gTEAM_SET_ID, ref gASSIGNED_SET_ID);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								if ( sPARENT_TYPE == "Partners" )
								{
									new DynamicControl(this, "PARTNER_ID"  ).ID   = gPARENT_ID;
									new DynamicControl(this, "PARTNER_NAME").Text = sPARENT_NAME;
									// 04/14/2016 Paul.  New spPARENT_GetWithTeam procedure so that we can inherit Assigned To and Team values. 
									if ( Sql.ToBoolean(Application["CONFIG.inherit_assigned_user"]) )
									{
										new DynamicControl(this, "ASSIGNED_USER_ID").ID   = gASSIGNED_USER_ID;
										new DynamicControl(this, "ASSIGNED_TO"     ).Text = sASSIGNED_TO     ;
										new DynamicControl(this, "ASSIGNED_TO_NAME").Text = sASSIGNED_TO_NAME;
										// 11/30/2017 Paul.  Add ASSIGNED_SET_ID for Dynamic User Assignment. 
										if ( Crm.Config.enable_dynamic_assignment() )
										{
											SplendidCRM._controls.UserSelect ctlUserSelect = FindControl("ASSIGNED_SET_NAME") as SplendidCRM._controls.UserSelect;
											if ( ctlUserSelect != null )
												ctlUserSelect.LoadLineItems(gASSIGNED_SET_ID, true, true);
										}
									}
									if ( Sql.ToBoolean(Application["CONFIG.inherit_team"]) )
									{
										new DynamicControl(this, "TEAM_ID"  ).ID   = gTEAM_ID  ;
										new DynamicControl(this, "TEAM_NAME").Text = sTEAM_NAME;
										SplendidCRM._controls.TeamSelect ctlTeamSelect = FindControl("TEAM_SET_NAME") as SplendidCRM._controls.TeamSelect;
										if ( ctlTeamSelect != null )
											ctlTeamSelect.LoadLineItems(gTEAM_SET_ID, true, true);
									}
									// 10/07/2010 Paul.  Populate with full address information. 
									DbProviderFactory dbf = DbProviderFactories.GetFactory();
									using ( IDbConnection con = dbf.CreateConnection() )
									{
										string sSQL ;
										sSQL = "select *              " + ControlChars.CrLf
										     + "  from vwPartners_Edit" + ControlChars.CrLf;
										using ( IDbCommand cmd = con.CreateCommand() )
										{
											cmd.CommandText = sSQL;
											Security.Filter(cmd, "Partners", "view");
											Sql.AppendParameter(cmd, gPARENT_ID, "ID", false);
											con.Open();

											if ( bDebug )
												RegisterClientScriptBlock("vwPartners_Edit", Sql.ClientScriptBlock(cmd));

											using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
											{
												if ( rdr.Read() )
												{
													new DynamicControl(this, "PRIMARY_ADDRESS_STREET"     ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STREET"     ]);
													new DynamicControl(this, "PRIMARY_ADDRESS_CITY"       ).Text = Sql.ToString(rdr["BILLING_ADDRESS_CITY"       ]);
													new DynamicControl(this, "PRIMARY_ADDRESS_STATE"      ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STATE"      ]);
													new DynamicControl(this, "PRIMARY_ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["BILLING_ADDRESS_POSTALCODE" ]);
													new DynamicControl(this, "PRIMARY_ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["BILLING_ADDRESS_COUNTRY"    ]);
													// 10/26/2010 Paul.  Fix spelling of SHIPPING. 
													new DynamicControl(this, "ALT_ADDRESS_STREET"     ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STREET"     ]);
													new DynamicControl(this, "ALT_ADDRESS_CITY"       ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_CITY"       ]);
													new DynamicControl(this, "ALT_ADDRESS_STATE"      ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STATE"      ]);
													new DynamicControl(this, "ALT_ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_POSTALCODE" ]);
													new DynamicControl(this, "ALT_ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_COUNTRY"    ]);
													// 04/03/2013 Paul.  A customer suggested that we copy phone numbers. 
													new DynamicControl(this, "PHONE_WORK"             ).Text = Sql.ToString(rdr["PHONE_OFFICE"                ]);
													new DynamicControl(this, "PHONE_FAX"              ).Text = Sql.ToString(rdr["PHONE_FAX"                   ]);
													new DynamicControl(this, "PHONE_OTHER"            ).Text = Sql.ToString(rdr["PHONE_ALTERNATE"             ]);
												}
											}
										}
									}
								}
							}
						}
					}
					// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
					this.ApplyEditViewNewEventRules(m_sMODULE + "." + sEditView);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				if ( bShowFullForm || bShowCancel )
					ctlFooterButtons.ErrorText = ex.Message;
				else
					lblError.Text = ex.Message;
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
			ctlDynamicButtons.Command += new CommandEventHandler(Page_Command);
			ctlFooterButtons .Command += new CommandEventHandler(Page_Command);

			ctlDynamicButtons.AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			ctlFooterButtons .AppendButtons("NewRecord." + (bShowFullForm ? "FullForm" : (bShowCancel ? "WithCancel" : "SaveOnly")), Guid.Empty, Guid.Empty);
			m_sMODULE = "Resources";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( bIsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
				// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
				if ( EditViewLoad != null )
					EditViewLoad(this, null);
				// 10/20/2011 Paul.  Apply Business Rules to NewRecord. 
				Page.Validators.Add(new RulesValidator(this));
			}
		}
		#endregion
	}
}


