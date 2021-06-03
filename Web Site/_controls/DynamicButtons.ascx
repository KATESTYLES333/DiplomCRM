<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DynamicButtons.ascx.cs" Inherits="SplendidCRM._controls.DynamicButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
function ConfirmDelete()
{
	return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>');
    }
</script>
<asp:Table SkinID="tabEditViewButtons" Visible="<%# !PrintView %>" runat="server">
	<asp:TableRow>
		<asp:TableCell ID="tdButtons" Width="10%" Wrap="false">
			<asp:Panel ID="pnlDynamicButtons" CssClass="button-panel" runat="server" />
		</asp:TableCell>
		<asp:TableCell ID="tdError">
			<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell ID="tdRequired" HorizontalAlign="Right" Wrap="false" Visible="false">
			<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
			&nbsp;
			<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>



