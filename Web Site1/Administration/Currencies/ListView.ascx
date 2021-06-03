<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Currencies.ListView" %>
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Currencies.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="true" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="Currencies.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="20%" ItemStyle-CssClass="listViewTdLinkS1">
				<ItemTemplate>
					<div style="DISPLAY: <%# Sql.ToString(DataBinder.Eval(Container.DataItem, "ISO4217")) == "USD" ? "inline" : "none" %>">
						<%# DataBinder.Eval(Container.DataItem, "NAME") %>
					</div>
					<div style="DISPLAY: <%# Sql.ToString(DataBinder.Eval(Container.DataItem, "ISO4217")) != "USD" ? "inline" : "none" %>">
						<asp:HyperLink Enabled='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' Text='<%# DataBinder.Eval(Container.DataItem, "NAME") %>' NavigateUrl='<%# "default.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ID") %>' CssClass="listViewTdLinkS1" Runat="server" />
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn    HeaderText="Currencies.LBL_LIST_ISO4217" DataField="ISO4217"         SortExpression="ISO4217"         ItemStyle-Width="10%" />
			<asp:BoundColumn    HeaderText="Currencies.LBL_LIST_SYMBOL"  DataField="SYMBOL"          SortExpression="SYMBOL"          ItemStyle-Width="10%" />
			<asp:BoundColumn    HeaderText="Currencies.LBL_LIST_RATE"    DataField="CONVERSION_RATE" SortExpression="CONVERSION_RATE" ItemStyle-Width="10%" />
			<asp:BoundColumn    HeaderText="Currencies.LBL_LIST_STATUS"  DataField="STATUS"          SortExpression="STATUS"          ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText=".LBL_LIST_DEFAULT" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<div style="DISPLAY: <%# String.Compare(Sql.ToString(DataBinder.Eval(Container.DataItem, "ID")), Sql.ToString(Application["CONFIG.default_currency"]), true) == 0 ? "inline" : "none" %>">
						<%= L10n.Term(".LBL_YES") %>
					</div>
					<div style="DISPLAY: <%# String.Compare(Sql.ToString(DataBinder.Eval(Container.DataItem, "ID")), Sql.ToString(Application["CONFIG.default_currency"]), true) != 0 ? "inline" : "none" %>">
						<%= L10n.Term(".LBL_NO") %>&nbsp;
						(<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' CommandName="Currencies.MakeDefault" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_MAKE_DEFAULT") %>' Runat="server" />)
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<div style="DISPLAY: <%# Sql.ToString(DataBinder.Eval(Container.DataItem, "ISO4217")) != "USD" ? "inline" : "none" %>">
						<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
							<asp:ImageButton Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Currencies.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Currencies.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
						</span>
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>


