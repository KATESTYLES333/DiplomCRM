<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Dropdown.ListView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Dropdown" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Dropdown.LBL_LIST_FORM_TITLE" Visible="false" Runat="Server" />
	
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
				<asp:HiddenField ID="txtINDEX" Runat="server" />
				<asp:Button ID="btnINDEX_MOVE" style="display: none" runat="server" />
				<asp:Label ID="lblError" CssClass="error" EnableViewState="false" Runat="server" />
			</asp:Panel>
			
			<SplendidCRM:SplendidGrid id="grdMain" AllowPaging="false" AllowSorting="false" EnableViewState="true" ShowFooter='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' runat="server">
				<Columns>
					<asp:TemplateColumn ItemStyle-CssClass="dragHandle">
						<ItemTemplate><asp:Image SkinID="blank" Width="14px" runat="server" /></ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Dropdown.LBL_KEY"   ItemStyle-Width="29%">
						<ItemTemplate><%# Eval("NAME") %></ItemTemplate>
						<EditItemTemplate><asp:TextBox ID="txtNAME" Text='<%# Eval("NAME") %>' runat="server" /></EditItemTemplate>
						<FooterTemplate><asp:TextBox ID="txtNAME" Text='<%# Eval("NAME") %>' runat="server" /></FooterTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Dropdown.LBL_VALUE" ItemStyle-Width="50%">
						<ItemTemplate><%# Server.HtmlEncode(Eval("DISPLAY_NAME") as string) %></ItemTemplate>
						<EditItemTemplate><asp:TextBox ID="txtDISPLAY_NAME" Text='<%# Eval("DISPLAY_NAME") %>' size="40" runat="server" /></EditItemTemplate>
						<FooterTemplate><asp:TextBox ID="txtDISPLAY_NAME" Text='<%# Eval("DISPLAY_NAME") %>' size="40" runat="server" /></FooterTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" Visible="false">
						<ItemTemplate>
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveUp"   CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP") %>' SkinID="uparrow_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveUp"   CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveDown" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' SkinID="downarrow_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Dropdown.MoveDown" CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
						<ItemTemplate><%# Eval("LIST_ORDER") %></ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Update"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' SkinID="accept_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Update"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Cancel"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' SkinID="decline_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex == grdMain.EditItemIndex %>' CommandName="Cancel"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Edit"     CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' SkinID="edit_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit"  ) >= 0 %>' CommandName="Edit"     CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Delete"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' SkinID="delete_inline" Runat="server" />
							<asp:LinkButton  Visible='<%# Container.ItemIndex != grdMain.EditItemIndex && SplendidCRM.Security.AdminUserAccess(m_sMODULE, "delete") >= 0 %>' CommandName="Delete"   CommandArgument='<%# Eval("ID") %>' CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
						</ItemTemplate>
						<FooterTemplate>
							<asp:Button ID="btnInsert" CommandName="Insert" Text='<%# L10n.Term(".LBL_ADD_BUTTON_LABEL") %>' Visible='<%# SplendidCRM.Security.AdminUserAccess(m_sMODULE, "edit") >= 0 %>' runat="server" />
						</FooterTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
			
			<SplendidCRM:InlineScript runat="server">
				<script type="text/javascript">
				// http://www.isocra.com/2008/02/table-drag-and-drop-jquery-plugin/
				$(document).ready(function()
				{
					$("#<%= grdMain.ClientID %>").tableDnD
					({
						dragHandle: "dragHandle",
						onDragClass: "jQueryDragBorder",
						onDragStart: function(table, row)
						{
							var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
							txtINDEX.value = (row.parentNode.rowIndex-1);
						},
						onDrop: function(table, row)
						{
							var txtINDEX = document.getElementById('<%= txtINDEX.ClientID %>');
							txtINDEX.value += ',' + (row.rowIndex-1); 
							document.getElementById('<%= btnINDEX_MOVE.ClientID %>').click();
						}
					});
					$("#<%= grdMain.ClientID %> tr").hover
					(
						function()
						{
							if ( !$(this).hasClass("nodrag") )
								$(this.cells[0]).addClass('jQueryDragHandle');
						},
						function()
						{
							if ( !$(this).hasClass("nodrag") )
								$(this.cells[0]).removeClass('jQueryDragHandle');
						}
					);
				});
				</script>
			</SplendidCRM:InlineScript>
		</ContentTemplate>
	</asp:UpdatePanel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>


