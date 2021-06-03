<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConvertViewPartner.ascx.cs" Inherits="SplendidCRM.Leads.ConvertViewPartner" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script runat="server">
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
</script>
<script type="text/javascript">
	function PartnerPopup()
	{
		return ModulePopup('Partners', '<%= txtSELECT_partner_ID.ClientID %>', '<%= txtSELECT_partner_NAME.ClientID %>', null, false, null);
	}
	function ToggleCreatePartner()
	{
		var divCreatePartner = document.getElementById('divCreatePartner');
		var divSelectPartner = document.getElementById('divSelectPartner');
		if( divCreatePartner.style.display == 'none' )
		{
			divCreatePartner.style.display = 'inline';
			divSelectPartner.style.display = 'none'  ;
			// 12/15/2013 Paul.  Stop clearing to allow creation toggle. 
			//ClearSelectPartner();
		}
		else
		{
			divCreatePartner.style.display = 'none'  ;
			divSelectPartner.style.display = 'inline';
		}
	}
	function ClearSelectPartner()
	{
		// 03/04/2009 Paul.  Must use ClientID to access the controls. 
		// 07/27/2010 Paul.  Add the ability to submit after clear. 
		ClearModuleType('Partners', '<%= txtSELECT_partner_ID.ClientID %>', '<%= txtSELECT_partner_NAME.ClientID %>', false);
		return false;
	}
</script>
<div id="divConvertViewPartner">
	<h5 CssClass="dataLabel" style="display:<%= sBusinessMode == "B2C" ? "none" : "inline" %>">
		<asp:CheckBox ID="chkCreatePartner" CssClass="checkbox" Runat="server" />
		<%= L10n.Term("Leads.LNK_NEW_ACCOUNT") %>
		&nbsp;<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</h5>
	<div id="divSelectPartner" style="display:<%= (sBusinessMode == "B2C" || chkCreatePartner.Checked) ? "none" : "inline" %>">
		<b><%= L10n.Term(".LBL_OR") %></b>
		<b><%= L10n.Term("Leads.LNK_SELECT_ACCOUNT") %></b>&nbsp;
		<asp:TextBox ID="txtSELECT_partner_NAME" ReadOnly="True" Runat="server" />
		<input ID="txtSELECT_partner_ID" type="hidden" runat="server" />
		<input ID="btnChangeSelectPartner" type="button" CssClass="button" onclick="return PartnerPopup();"       title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
		<input ID="btnClearSelectPartner"  type="button" CssClass="button" onclick="return ClearSelectPartner();" title="<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>" AccessKey="<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>" value="<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>" />
		<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqSELECT_partner_ID" ControlToValidate="txtSELECT_partner_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
	</div>
	<div id="divCreatePartner" style="display:<%= chkCreatePartner.Checked ? "inline" : "none" %>">
		<table ID="tblMain" class="tabEditView" runat="server">
		</table>
		
		<div id="divCreatePartnerNoteLink">
			&nbsp;<asp:CheckBox ID="chkCreateNote" CssClass="checkbox" Runat="server" />
			&nbsp;<%= L10n.Term("Leads.LNK_NEW_NOTE") %>
		</div>
		<div id="divCreatePartnerNote" style="display:<%= chkCreateNote.Checked ? "inline" : "none" %>">
			<p></p>
			<%@ Register TagPrefix="SplendidCRM" Tagname="ConvertViewNote" Src="ConvertViewNote.ascx" %>
			<SplendidCRM:ConvertViewNote ID="ctlConvertViewNote" Runat="Server" />
		</div>
	</div>
</div>

