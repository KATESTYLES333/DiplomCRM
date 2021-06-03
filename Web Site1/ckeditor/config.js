/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	// http://docs.cksource.com/CKEditor_3.x/Developers_Guide/Toolbar
	// 09/18/2011 Paul.  Custom toolbar for SplendidCRM. 
	config.toolbar = 'SplendidCRM';
	config.toolbar_SplendidCRM =
	[
		{ name: 'document'   , items : [ 'Source'] },
		{ name: 'editing'    , items : [ 'Find','Replace','-','SelectAll', 'Undo','Redo',,'SpellChecker', 'Scayt' ] },
		{ name: 'paragraph'  , items : [ 'NumberedList','BulletedList','-','Outdent','Indent','-','Blockquote','CreateDiv','-','JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock','-','BidiLtr','BidiRtl' ] },
		{ name: 'insert'     , items : [ 'Link','Image','Table','HorizontalRule','Smiley','SpecialChar' ] },
		'/',
		{ name: 'basicstyles', items : [ 'Bold','Italic','Underline','Strike','Subscript','Superscript','-','RemoveFormat' ] },
		{ name: 'styles'     , items : [ 'Styles','Format','Font','FontSize' ] },
		{ name: 'colors'     , items : [ 'TextColor','BGColor' ] },
		{ name: 'tools'      , items : [ 'Maximize', 'ShowBlocks','-','About' ] }
	];
	config.toolbar_None =
	[
		[]
	];
};
