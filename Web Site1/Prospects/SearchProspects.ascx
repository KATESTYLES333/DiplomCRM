<%@ Control CodeBehind="SearchProspects.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Prospects.SearchProspects" %>
<script runat="server">
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
</script>
<script type="text/javascript">
nUnifiedSearchVisibleCount++;
</script>
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divSearchProspects" ID="ctlListHeader" Title="Prospects.LBL_LIST_FORM_TITLE" Runat="Server" />

<div id="divSearchProspects" style='<%= "display:" + (CookieValue("divSearchProspects") != "1" ? "inline" : "none") %>'>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdListView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate>
					<asp:Panel Visible='<%# bShowCheckboxColumn %>' runat="server">
						<input name="chkMain" class="checkbox" type="checkbox" value="<%# Sql.ToString(Eval("ID")) + "|" + m_sMODULE %>" />
					</asp:Panel>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit", Sql.ToGuid(Eval("ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + m_sMODULE + "/edit.aspx?id=" + Eval("ID") %>' ToolTip='<%# L10n.Term(".LNK_EDIT") %>' Runat="server">
						<asp:Image SkinID="edit_inline" Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>


