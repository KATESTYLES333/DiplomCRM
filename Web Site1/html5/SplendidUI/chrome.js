/* Copyright (C) 2011-2012 SplendidCRM Software, Inc. All Rights Reserved. 
 * Any use of the contents of this file are subject to the SplendidCRM Professional Source Code License Agreement, or other written agreement between you and SplendidCRM ("License"). 
 * SplendidCRM owns all proprietary rights, including all copyrights, patents, trade secrets, and trademarks, in and to the contents of this file.  You will not link to or in any way 
 * combine the contents of this file or any derivatives with any Open Source Code in any manner that would require the contents of this file to be made available to any third party. 
 */

var backgroundPage = new Object();
var chrome         = new Object();
chrome.extension   = new Object();
chrome.extension.getBackgroundPage = function()
{
	return backgroundPage;
}

try
{
	// Storage.js
	backgroundPage.SplendidStorage                        = SplendidStorage;
	// Login.js
	backgroundPage.GetUserID                              = GetUserID;
	backgroundPage.GetUserName                            = GetUserName;
	backgroundPage.GetTeamID                              = GetTeamID;
	backgroundPage.GetTeamName                            = GetTeamName;
	backgroundPage.GetUserLanguage                        = GetUserLanguage;
	backgroundPage.GetUserProfile                         = GetUserProfile;
	backgroundPage.IsAuthenticated                        = IsAuthenticated;
	backgroundPage.Login                                  = Login;
	backgroundPage.AuthenticatedMethod                    = AuthenticatedMethod;
	backgroundPage.IsOnline                               = IsOnline;
	// Credentials.js
	backgroundPage.GetIsOffline                           = GetIsOffline;
	backgroundPage.GetEnableOffline                       = GetEnableOffline;
	// 12/09/2014 Paul.  Remote Server is on the background page of the browser extensions. 
	backgroundPage.RemoteServer                           = RemoteServer;
	// SplendidCache.js
	backgroundPage.SplendidCache                          = SplendidCache;
	// AutoComplete.js
	backgroundPage.AutoComplete_ModuleMethod              = AutoComplete_ModuleMethod;
	// Logout.js
	backgroundPage.Logout                                 = Logout;
	// Terminology.js
	backgroundPage.Terminology_SetTerm                    = Terminology_SetTerm;
	backgroundPage.Terminology_SetListTerm                = Terminology_SetListTerm;
	backgroundPage.Terminology_LoadGlobal                 = Terminology_LoadGlobal;
	backgroundPage.Terminology_LoadList                   = Terminology_LoadList;
	backgroundPage.Terminology_LoadModule                 = Terminology_LoadModule;
	backgroundPage.Terminology_LoadCustomList             = Terminology_LoadCustomList;
	backgroundPage.Terminology_LoadAllLists               = Terminology_LoadAllLists;
	backgroundPage.Terminology_LoadAllTerms               = Terminology_LoadAllTerms;
	// Application.js
	backgroundPage.Application_Modules                    = Application_Modules;
	backgroundPage.Application_Config                     = Application_Config;
	backgroundPage.Application_Teams                      = Application_Teams;
	// TabMenu.js
	backgroundPage.TabMenu_Load                           = TabMenu_Load;
	// ListView.js
	backgroundPage.ListView_LoadTable                     = ListView_LoadTable;
	backgroundPage.ListView_LoadModule                    = ListView_LoadModule;
	backgroundPage.ListView_LoadLayout                    = ListView_LoadLayout;
	backgroundPage.ListView_LoadAllLayouts                = ListView_LoadAllLayouts;
	// DetailView.js
	backgroundPage.DetailView_LoadItem                    = DetailView_LoadItem;
	backgroundPage.DetailView_LoadLayout                  = DetailView_LoadLayout;
	backgroundPage.DetailView_LoadAllLayouts              = DetailView_LoadAllLayouts;
	// EditView.js
	backgroundPage.EditView_LoadItem                      = EditView_LoadItem;
	backgroundPage.EditView_LoadLayout                    = EditView_LoadLayout;
	backgroundPage.EditView_LoadAllLayouts                = EditView_LoadAllLayouts;
	// DetailViewRelationships.js
	backgroundPage.DetailViewRelationships_LoadLayout     = DetailViewRelationships_LoadLayout;
	backgroundPage.DetailViewRelationships_LoadAllLayouts = DetailViewRelationships_LoadAllLayouts;
	// DynamicButtons.js
	backgroundPage.DynamicButtons_LoadLayout              = DynamicButtons_LoadLayout;
	backgroundPage.DynamicButtons_LoadAllLayouts          = DynamicButtons_LoadAllLayouts;
	// ModuleUpdate.js
	backgroundPage.DeleteModuleItem                       = DeleteModuleItem;
	backgroundPage.UpdateModule                           = UpdateModule;
	backgroundPage.UpdateModuleTable                      = UpdateModuleTable;
	backgroundPage.DeleteRelatedItem                      = DeleteRelatedItem;
	backgroundPage.UpdateRelatedItem                      = UpdateRelatedItem;
	// CalendarView.js
	backgroundPage.CalendarView_GetCalendar               = CalendarView_GetCalendar;
}
catch(e)
{
	alert('chrome.js ' + e.message);
}


