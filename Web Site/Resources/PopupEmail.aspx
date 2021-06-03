<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="PopupEmail.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Resources.PopupEmail" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Resources" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

<script type="text/javascript">
function SelectResource(sRESOURCE_ID, sRESOURCE_NAME, sRESOURCE_EMAIL)
{
	if ( window.opener != null && window.opener.ChangeResourceEmail != null )
	{
		window.opener.ChangeResourceEmail(sRESOURCE_ID, sRESOURCE_NAME, sRESOURCE_EMAIL);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Resource cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
	}
}
function Clear()
{
	// 11/20/2005 Paul.  Clear does nothing on SugarCRM 3.5.1. 
	if ( window.opener != null && window.opener.ChangeResourceEmail != null )
	{
		window.opener.ChangeResourceEmail('', '', '');
		window.close();
	}
	else
	{
		alert('Original window has closed.  Resource cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Resources.LBL_LIST_FORM_TITLE" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DynamicButtons" Src="~/_controls/DynamicButtons.ascx" %>
	<SplendidCRM:DynamicButtons ID="ctlDynamicButtons" Runat="Server" />

	<SplendidCRM:SplendidGrid id="grdMain" SkinID="grdPopupView" EnableViewState="true" runat="server">
		<Columns>
			<asp:TemplateColumn HeaderText="Resources.LBL_LIST_NAME" SortExpression="NAME" ItemStyle-Width="40%">
				<ItemTemplate>
					<a name="RESOURCE_ID" id="RESOURCE_ID_<%# DataBinder.Eval(Container.DataItem, "ID") %>" class="listViewTdLinkS1" href="#" onclick="SelectResource('<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "ID"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "NAME"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "EMAIL1"))) %>');"><%# DataBinder.Eval(Container.DataItem, "NAME") %></a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Resources.LBL_LIST_EMAIL_ADDRESS" SortExpression="EMAIL1" ItemStyle-Width="25%">
				<ItemTemplate>
					<a name="RESOURCE_ID_EMAIL" id="RESOURCE_ID_EMAIL_<%# DataBinder.Eval(Container.DataItem, "ID") %>" class="listViewTdLinkS1" href="#" onclick="SelectResource('<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "ID"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "NAME"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "EMAIL1"))) %>');"><%# DataBinder.Eval(Container.DataItem, "EMAIL1") %></a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Resources.LBL_LIST_partner_NAME"  DataField="partner_NAME"     SortExpression="partner_NAME" ItemStyle-Width="35%" />
		</Columns>
	</SplendidCRM:SplendidGrid>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
 
</asp:Content>


