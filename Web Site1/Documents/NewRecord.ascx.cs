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
using System.IO;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM.Documents
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
		// 05/05/2010 Paul.  We need a common way to access the parent from the Toolbar. 

		// 04/20/2010 Paul.  Add functions to allow this control to be used as part of an InlineEdit operation. 
		public override bool IsEmpty()
		{
			string sNAME = new DynamicControl(this, "DOCUMENT_NAME").Text;
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
			
			Guid   gASSIGNED_USER_ID = new DynamicControl(this, "ASSIGNED_USER_ID").ID;
			Guid   gTEAM_ID          = new DynamicControl(this, "TEAM_ID"         ).ID;
			if ( Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
				gASSIGNED_USER_ID = Security.USER_ID;
			if ( Sql.IsEmptyGuid(gTEAM_ID) )
				gTEAM_ID = Security.TEAM_ID;
			
			HtmlInputFile fileCONTENT = FindControl("CONTENT_File") as HtmlInputFile;
			HttpPostedFile pstCONTENT  = null;
			if ( fileCONTENT != null )
				pstCONTENT = fileCONTENT.PostedFile;
			if ( pstCONTENT != null )
			{
				long lFileSize      = pstCONTENT.ContentLength;
				long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
				if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
				{
					throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
				}
			}
			// 05/15/2011 Paul.  We need to include the Master and Secondary so that the user selects the correct template. 
			// 04/02/2012 Paul.  Add ASSIGNED_USER_ID. 
			SqlProcs.spDOCUMENTS_Update
				( ref gID
				, new DynamicControl(this, "DOCUMENT_NAME"      ).Text
				, new DynamicControl(this, "ACTIVE_DATE"        ).DateValue
				, new DynamicControl(this, "EXP_DATE"           ).DateValue
				, new DynamicControl(this, "CATEGORY_ID"        ).SelectedValue
				, new DynamicControl(this, "SUBCATEGORY_ID"     ).SelectedValue
				, new DynamicControl(this, "STATUS_ID"          ).SelectedValue
				, new DynamicControl(this, "DESCRIPTION"        ).Text
				, new DynamicControl(this, "MAIL_MERGE_DOCUMENT").Checked
				, new DynamicControl(this, "RELATED_DOC_ID"     ).ID
				, new DynamicControl(this, "RELATED_DOC_REV_ID" ).ID
				, new DynamicControl(this, "IS_TEMPLATE"        ).Checked
				, new DynamicControl(this, "TEMPLATE_TYPE"      ).Text
				, gTEAM_ID
				, new DynamicControl(this, "TEAM_SET_LIST"      ).Text
				, new DynamicControl(this, "PRIMARY_MODULE"     ).Text
				, new DynamicControl(this, "SECONDARY_MODULE"   ).Text
				, gASSIGNED_USER_ID
				, trn
				);
			if ( pstCONTENT != null )
			{
				if ( pstCONTENT.FileName.Length > 0 )
				{
					string sFILENAME       = Path.GetFileName (pstCONTENT.FileName);
					string sFILE_EXT       = Path.GetExtension(sFILENAME);
					string sFILE_MIME_TYPE = pstCONTENT.ContentType;
			
					Guid gRevisionID = Guid.Empty;
					SqlProcs.spDOCUMENT_REVISIONS_Insert
						( ref gRevisionID
						, gID
						, new DynamicControl(this, "REVISION").Text
						, "Document Created"
						, sFILENAME
						, sFILE_EXT
						, sFILE_MIME_TYPE
						, trn
						);
					// 04/24/2011 Paul.  Move LoadFile() to Crm.DocumentRevisions. 
					Crm.DocumentRevisions.LoadFile(gRevisionID, pstCONTENT.InputStream, trn);
				}
			}
			SplendidDynamic.UpdateCustomFields(this, trn, gID, sTABLE_NAME, dtCustomFields);
			//SqlProcs.spDOCUMENTS_InsRelated(gID, sPARENT_TYPE, gPARENT_ID, trn);
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
									Guid   gPARENT_ID   = this.PARENT_ID  ;
									String sPARENT_TYPE = this.PARENT_TYPE;
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
					this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
					// 03/05/2011 Paul.  Automatically populate the report name based on the file name. Need to do this on a PostBack. 
					HtmlInputFile CONTENT = FindControl("CONTENT_File") as HtmlInputFile;
					if ( CONTENT != null )
						CONTENT.Attributes.Add("onchange", "FileNameChanged(this)");
					// 03/05/2011 Paul.  Initialize Publish Date to Today. 
					new DynamicControl(this, "ACTIVE_DATE").DateValue = DateTime.Today;
					// 06/04/2010 Paul.  Notify the parent that the fields have been loaded. 
					if ( EditViewLoad != null )
						EditViewLoad(this, null);
					
					// 02/21/2010 Paul.  When the Full Form buttons are used, we don't want the panel to have margins. 
					if ( bShowFullForm || bShowCancel || sEditView != "NewRecord" )
					{
						pnlMain.CssClass = "";
						pnlEdit.CssClass = "tabForm";
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
			// 07/15/2010 Paul.  There is a documented issue with a FileUpload/HtmlInputFile control inside an UpdatePanel. 
			// The solution is to set the postback control that submits the file to be a PostBackTrigger for the panel. 
			ScriptManager mgr = ScriptManager.GetCurrent(this.Page);
			if ( mgr != null )
			{
				Button btnNewRecord = ctlDynamicButtons.FindButton("NewRecord");
				if ( btnNewRecord != null )
					mgr.RegisterPostBackControl(btnNewRecord);
				btnNewRecord = ctlFooterButtons.FindButton("NewRecord");
				if ( btnNewRecord != null )
					mgr.RegisterPostBackControl(btnNewRecord);
			}

			m_sMODULE = "Documents";
			// 05/06/2010 Paul.  Use a special Page flag to override the default IsPostBack behavior. 
			bool bIsPostBack = this.IsPostBack && !NotPostBack;
			if ( bIsPostBack )
			{
				this.AppendEditViewFields(m_sMODULE + "." + sEditView, tblMain, null, ctlFooterButtons.ButtonClientID("NewRecord"));
				// 03/05/2011 Paul.  Automatically populate the report name based on the file name. Need to do this on a PostBack. 
				HtmlInputFile CONTENT = FindControl("CONTENT_File") as HtmlInputFile;
				if ( CONTENT != null )
					CONTENT.Attributes.Add("onchange", "FileNameChanged(this)");
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


