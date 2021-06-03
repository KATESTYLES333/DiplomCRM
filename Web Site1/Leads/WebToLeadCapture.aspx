<%@ Page language="c#" AutoEventWireup="true" Inherits="SplendidCRM.SplendidPage" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Diagnostics" %>
<head visible="false" runat="server" />
<script runat="server">
		override protected bool AuthenticationRequired()
		{
			return false;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.ExpiresAbsolute = new DateTime(1980, 1, 1, 0, 0, 0, 0);
			
			string sRedirect = Sql.ToString(Request["Redirect"]);
			if ( Request.QueryString.Count == 0 && Request.Form.Count == 0 )
			{
				Response.Write("Missing data.");
				if ( !Sql.IsEmptyString(sRedirect) )
				{
					sRedirect += (sRedirect.Contains("?") ? "&" : "?") + "Error=Missing data.";
					Response.Redirect(sRedirect);
				}
				return;
			}
			// 08/03/2012 Paul.  Provide a way to disable Web Capture. 
			if ( Sql.ToBoolean(Application["WebToLeadCapture.Disabled"]) )
			{
				Response.Write("Web Capture has been disabled.");
				if ( !Sql.IsEmptyString(sRedirect) )
				{
					sRedirect += (sRedirect.Contains("?") ? "&" : "?") + "Error=Web Capture has been disabled.";
					Response.Redirect(sRedirect);
				}
				return;
			}

			try
			{
				string sCUSTOM_MODULE = "LEADS";
				DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					// 10/07/2009 Paul.  We need to create our own global transaction ID to support auditing and workflow on SQL Azure, PostgreSQL, Oracle, DB2 and MySQL. 
					using ( IDbTransaction trn = Sql.BeginTransaction(con) )
					{
						try
						{
							Guid gID = Guid.Empty;
							// 04/26/2012 Paul.  Add ASSISTANT, ASSISTANT_PHONE, BIRTHDATE, WEBSITE. 
							SqlProcs.spLEADS_Update
								( ref gID
								, Sql.ToGuid   (Request["ASSIGNED_USER_ID"          ])
								, Sql.ToString (Request["SALUTATION"                ])
								, Sql.ToString (Request["FIRST_NAME"                ])
								, Sql.ToString (Request["LAST_NAME"                 ])
								, Sql.ToString (Request["TITLE"                     ])
								, Sql.ToString (Request["REFERED_BY"                ])
								, Sql.ToString (Request["LEAD_SOURCE"               ])
								, Sql.ToString (Request["LEAD_SOURCE_DESCRIPTION"   ])
								, Sql.ToString (Request["STATUS"                    ])
								, Sql.ToString (Request["STATUS_DESCRIPTION"        ])
								, Sql.ToString (Request["DEPARTMENT"                ])
								, Guid.Empty  // 06/24/2005. REPORTS_TO_ID is not used in version 3.0. 
								, Sql.ToBoolean(Request["DO_NOT_CALL"               ])
								, Sql.ToString (Request["PHONE_HOME"                ])
								, Sql.ToString (Request["PHONE_MOBILE"              ])
								, Sql.ToString (Request["PHONE_WORK"                ])
								, Sql.ToString (Request["PHONE_OTHER"               ])
								, Sql.ToString (Request["PHONE_FAX"                 ])
								, Sql.ToString (Request["EMAIL1"                    ])
								, Sql.ToString (Request["EMAIL2"                    ])
								, Sql.ToBoolean(Request["EMAIL_OPT_OUT"             ])
								, Sql.ToBoolean(Request["INVALID_EMAIL"             ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_STREET"    ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_CITY"      ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_STATE"     ])
								, Sql.ToString (Request["PRIMARY_ADDRESS_POSTALCODE"])
								, Sql.ToString (Request["PRIMARY_ADDRESS_COUNTRY"   ])
								, Sql.ToString (Request["ALT_ADDRESS_STREET"        ])
								, Sql.ToString (Request["ALT_ADDRESS_CITY"          ])
								, Sql.ToString (Request["ALT_ADDRESS_STATE"         ])
								, Sql.ToString (Request["ALT_ADDRESS_POSTALCODE"    ])
								, Sql.ToString (Request["ALT_ADDRESS_COUNTRY"       ])
								, Sql.ToString (Request["DESCRIPTION"               ])
								, Sql.ToString (Request["ACCOUNT_NAME"              ])
								, Sql.ToGuid   (Request["CAMPAIGN_ID"               ])
								, Sql.ToGuid   (Request["TEAM_ID"                   ])
								, String.Empty // 09/13/2009 Paul.  It does not seem practical to allow TEAM_SET_LIST as a parameter. 
								, Guid.Empty   // 02/20/2010 Paul.  It does not make sense to support CONTACT_ID. 
								, Guid.Empty   // 02/20/2010 Paul.  It does not make sense to support ACCOUNT_ID. 
								, false        // 04/08/2010 Paul.  EXCHANGE_FOLDER is not supported in this context. 
								, Sql.ToDateTime(Request["BIRTHDATE"                ])
								, Sql.ToString  (Request["ASSISTANT"                ])
								, Sql.ToString  (Request["ASSISTANT_PHONE"          ])
								, Sql.ToString  (Request["WEBSITE"                  ])
								// 09/27/2013 Paul.  SMS messages need to be opt-in. 
								, Sql.ToString  (Request["SMS_OPT_IN"               ])
								// 10/22/2013 Paul.  Provide a way to map Tweets to a parent. 
								, Sql.ToString  (Request["TWITTER_SCREEN_NAME"      ])
								, trn
								);
							SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
							trn.Commit();
						}
						catch(Exception ex)
						{
							trn.Rollback();
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							Response.Write(ex.Message);
							return;
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
				return;
			}
			Response.Write("Thank you.");
			if ( !Sql.IsEmptyString(sRedirect) )
			{
				Response.Redirect(sRedirect);
			}
		}

</script>

