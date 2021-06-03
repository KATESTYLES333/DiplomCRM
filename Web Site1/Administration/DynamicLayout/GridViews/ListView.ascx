<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.DynamicLayout.GridViews.ListView" %>
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
<SplendidCRM:InlineScript runat="server">
<script type="text/javascript">
var sDynamicLayoutModule = '<%= ViewState["MODULE_NAME"] %>';
function PreLoadEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "PRE_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function PostLoadEventPopup()
{
	return ModulePopup('BusinessRules', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_ID").ClientID %>', '<%= new SplendidCRM.DynamicControl(this, "POST_LOAD_EVENT_NAME").ClientID %>', 'Module=' + sDynamicLayoutModule, false, null);
}
function LayoutDragOver(event, nDropIndex)
{
	// 08/08/2013 Paul.  IE does not support preventDefault. 
	// http://stackoverflow.com/questions/1000597/event-preventdefault-function-not-working-in-ie
	event.preventDefault ? event.preventDefault() : event.returnValue = false;
}
function LayoutDropIndex(event, nDropIndex)
{
	// 08/08/2013 Paul.  IE does not support preventDefault. 
	event.preventDefault ? event.preventDefault() : event.returnValue = false;
	var hidDragStartIndex = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "hidDragStartIndex").ClientID %>');
	var hidDragEndIndex   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "hidDragEndIndex"  ).ClientID %>');
	var btnDragComplete   = document.getElementById('<%= new SplendidCRM.DynamicControl(this, "btnDragComplete"   ).ClientID %>');
	hidDragStartIndex.value = event.dataTransfer.getData('Text');
	hidDragEndIndex.value   = nDropIndex;
	if ( hidDragStartIndex.value != hidDragEndIndex.value )
	{
		btnDragComplete.click();
	}
}
</script>
</SplendidCRM:InlineScript>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="DynamicLayout.LBL_GRID_VIEW_LAYOUT" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<asp:HiddenField ID="hidDragStartIndex" runat="server" />
	<asp:HiddenField ID="hidDragEndIndex"   runat="server" />
	<asp:Button      ID="btnDragComplete" CommandName="Layout.DragIndex" OnCommand="Page_Command" style="display:none" runat="server" />

	<asp:Table Width="100%" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="200px" VerticalAlign="Top">
				<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="../_controls/SearchBasic.ascx" %>
				<SplendidCRM:SearchBasic ID="ctlSearch" ViewTableName="vwGRIDVIEWS_Layout" ViewFieldName="GRID_NAME" Runat="Server" />
			</asp:TableCell>
			<asp:TableCell VerticalAlign="Top">
				<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
				<SplendidCRM:ListHeader ID="ctlListHeader" Runat="Server" />
				
				<%@ Register TagPrefix="SplendidCRM" Tagname="LayoutButtons" Src="../_controls/LayoutButtons.ascx" %>
				<SplendidCRM:LayoutButtons ID="ctlLayoutButtons" Visible="<%# !PrintView %>" Runat="Server" />

				<asp:Table ID="tblViewEventsPanel" Width="100%" CellPadding="0" CellSpacing="0" CssClass="" runat="server">
					<asp:TableRow>
						<asp:TableCell>
							<table ID="tblViewEvents" class="tabEditView" runat="server">
							</table>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>

				<input type="hidden" id="txtFieldState" runat="server" />
				<table ID="tblMain" width="100%" border="0" cellspacing="0" cellpadding="0" class="" runat="server">
				</table>
				
				<br />
				<%@ Register TagPrefix="SplendidCRM" Tagname="NewRecord" Src="~/Administration/DynamicLayout/GridViews/NewRecord.ascx" %>
				<SplendidCRM:NewRecord ID="ctlNewRecord" Visible="false" Runat="Server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>


