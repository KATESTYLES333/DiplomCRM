<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="view.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Terminology.View" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailView" Src="DetailView.ascx" %>
	<SplendidCRM:DetailView ID="ctlDetailView" Runat="Server" />
	<asp:Label ID="lblAccessError" Text='<%# L10n.Term(".LBL_UNAUTH_ADMIN") %>' Visible='<%# !ctlDetailView.Visible %>' CssClass="error" EnableViewState="false" Runat="Server" />
</asp:Content>


