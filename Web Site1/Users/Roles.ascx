<%@ Control CodeBehind="Roles.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Users.Roles" %>
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
function ChangeRole(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtROLE_ID.ClientID   %>').value = sPARENT_ID  ;
	document.forms[0].submit();
}
function RoleMultiSelect()
{
	return window.open('../Administration/ACLRoles/PopupMultiSelect.aspx', 'RolePopup', '<%= SplendidCRM.Crm.Config.PopupWindowOptions() %>');
}
</script>
<input ID="txtROLE_ID" type="hidden" Runat="server" />
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader SubPanel="divUsersRoles" Title="Roles.LBL_MODULE_NAME" Runat="Server" />

<div id="divUsersRoles" style='<%= "display:" + (CookieValue("divUsersRoles") != "1" ? "inline" : "none") %>'>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdSubPanelView" AllowPaging="<%# !PrintView %>" EnableViewState="true" runat="server">
		<Columns>
			<asp:BoundColumn     HeaderText="ACLRoles.LBL_LIST_NAME"        DataField="ROLE_NAME"     ItemStyle-Width="30%" />
			<asp:BoundColumn     HeaderText="ACLRoles.LBL_LIST_DESCRIPTION" DataField="DESCRIPTION"   ItemStyle-Width="60%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<div visible='<%# SplendidCRM.Security.AdminUserAccess("Users", "edit") >= 0 %>' runat="server">
						<asp:ImageButton CommandName="Roles.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" SkinID="edit_inline" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
						<asp:LinkButton  CommandName="Roles.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term(".LNK_EDIT") %></asp:LinkButton>
						&nbsp;
						<asp:ImageButton CommandName="Roles.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' SkinID="delete_inline" Runat="server" />
						<asp:LinkButton  CommandName="Roles.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>


