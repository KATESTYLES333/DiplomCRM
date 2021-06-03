<%@ Page language="c#" EnableTheming="false" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.html5.Default" %>
<!--
/* Copyright (C) 2011-2015 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */
-->
<!DOCTYPE HTML>
<html id="htmlRoot" runat="server">
<head runat="server">
<title><%# L10n.Term(".LBL_BROWSER_TITLE") %></title>
<meta name="apple-mobile-web-app-capable"          content="yes"   Visible='<%# Request.UserAgent.Contains("iPad;") %>' runat="server" />
<meta name="apple-mobile-web-app-status-bar-style" content="black" Visible='<%# Request.UserAgent.Contains("iPad;") %>' runat="server" />

<link type="text/css" rel="stylesheet" href="jQuery/jquery-ui-1.9.1.custom.css" />
<link type="text/css" rel="stylesheet" href="jQuery/contextMenu-1.2.0.css" />
<link type="text/css" rel="stylesheet" href="FullCalendar/fullcalendar.css" />
<link type="text/css" rel="stylesheet" href="fonts/font-awesome.css" />
<link type="text/css" rel="stylesheet" href="mobile.css" />

<script type="text/javascript" src="jQuery/jquery-1.9.1.min.js"></script>
<script type="text/javascript" src="jQuery/jquery-ui-1.9.1.custom.js"></script>
<script type="text/javascript" src="jQuery/jquery-ui-timepicker-addon.js"></script>
<script type="text/javascript" src="jQuery/jquery.paging.min.js"></script>
<script type="text/javascript" src="jQuery/contextMenu-1.2.0.js"></script>
<script type="text/javascript" src="FullCalendar/fullcalendar.js"></script>
<script type="text/javascript" src="FullCalendar/gcal.js"></script>
<script type="text/javascript" src="JSON.js"></script>
<script type="text/javascript" src="Math.uuid.js"></script>
<script type="text/javascript" src="utility.js"></script>
<script type="text/javascript" src="sha1.js"></script>
<script type="text/javascript" src="SplendidScripts/SplendidStorage.js"></script>
<script type="text/javascript" src="SplendidScripts/Credentials.js"></script>
<script type="text/javascript" src="SplendidScripts/SplendidRequest.js"></script>
<script type="text/javascript" src="SplendidScripts/SystemCacheRequest.js"></script>
<script type="text/javascript" src="SplendidScripts/SplendidCache.js"></script>
<script type="text/javascript" src="SplendidScripts/Application.js"></script>
<script type="text/javascript" src="SplendidScripts/Login.js"></script>
<script type="text/javascript" src="SplendidScripts/Logout.js"></script>
<script type="text/javascript" src="SplendidScripts/Terminology.js"></script>
<script type="text/javascript" src="SplendidScripts/DetailViewRelationships.js"></script>
<script type="text/javascript" src="SplendidScripts/TabMenu.js"></script>
<script type="text/javascript" src="SplendidScripts/ListView.js"></script>
<script type="text/javascript" src="SplendidScripts/DetailView.js"></script>
<script type="text/javascript" src="SplendidScripts/EditView.js"></script>
<script type="text/javascript" src="SplendidScripts/DynamicButtons.js"></script>
<script type="text/javascript" src="SplendidScripts/ModuleUpdate.js"></script>
<script type="text/javascript" src="SplendidScripts/AutoComplete.js"></script>
<script type="text/javascript" src="SplendidScripts/Options.js"></script>
<script type="text/javascript" src="SplendidScripts/CalendarView.js"></script>

<script type="text/javascript" src="SplendidUI/chrome.js"></script>
<script type="text/javascript" src="SplendidUI/SplendidErrorUI.js"></script>
<script type="text/javascript" src="SplendidUI/SearchBuilder.js"></script>
<script type="text/javascript" src="SplendidUI/Sql.js"></script>
<script type="text/javascript" src="SplendidUI/Crm.js"></script>
<script type="text/javascript" src="SplendidUI/Formatting.js"></script>
<script type="text/javascript" src="SplendidUI/TerminologyUI.js"></script>
<script type="text/javascript" src="SplendidUI/TabMenuUI.js"></script>
<script type="text/javascript" src="SplendidUI/TabMenuUI_Six.js"></script>
<script type="text/javascript" src="SplendidUI/TabMenuUI_Atlantic.js"></script>
<script type="text/javascript" src="SplendidUI/TabMenuUI_Mobile.js"></script>
<script type="text/javascript" src="SplendidUI/ListViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/PopupViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/DetailViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/EditViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/SearchViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/SplendidInitUI.js"></script>
<script type="text/javascript" src="SplendidUI/DynamicButtonsUI.js"></script>
<script type="text/javascript" src="SplendidUI/DetailViewRelationshipsUI.js"></script>
<script type="text/javascript" src="SplendidUI/SelectionUI.js"></script>
<script type="text/javascript" src="SplendidUI/LoginViewUI.js"></script>
<script type="text/javascript" src="SplendidUI/ArchiveEmailUI.js"></script>
<script type="text/javascript" src="SplendidUI/CalendarViewUI.js"></script>

<script type="text/javascript" src="SignalR/jquery.signalR.min.js"></script>
<script type="text/javascript" src="SignalR/server.js"></script>
<script type="text/javascript" src="SignalR/connection.start.js"></script>
<script type="text/javascript" src="SplendidUI/ChatDashboardUI.js"></script>
<script type="text/javascript" src="<%# Application["scriptURL"] %>ModulePopupScripts.aspx?LastModified=<%# Server.UrlEncode(Sql.ToString(Application["Modules.LastModified"])) + "&UserID=" + Security.USER_ID.ToString() %>"></script>

<script type="text/javascript" src="default.js"></script>
<%@ Register TagPrefix="SplendidCRM" Tagname="LoadSplendid" Src="LoadSplendid.ascx" %>
<SplendidCRM:LoadSplendid ID="ctlLoadSplendid" Runat="Server" />
</head>
<link type="text/css" rel="stylesheet" href="Atlantic.css" />
<body>
<div id="ctlAtlanticToolbar"></div>
<div id="ctlHeader"></div>
<div width="100%" style="background-color: White">
	<div id="ctlTabMenu"></div>
	
	<div style="padding-left: 10px; padding-right: 10px; padding-bottom: 5px;">
		<div id="divMainLayoutPanel_Header"></div>
		
		<div id="divMainActionsPanel"></div>
		
		<div id="divMainLayoutPanel"></div>
	</div>
	<div id="divFooterCopyright" align="center" class="copyRight">
		Copyright &copy; 2005-2015 <a id="lnkSplendidCRM" href="http://www.splendidcrm.com" target="_blank" class="copyRightLink">SplendidCRM Software, Inc.</a> All Rights Reserved.<br />
	</div>
</div>
</body>
</html>

