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
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

namespace SplendidCRM
{
	/// <summary>
	/// Summary description for SurveyUtil.
	/// </summary>
	public class ChartUtil
	{
		public static void RegisterScripts(Page Page)
		{
			try
			{
				AjaxControlToolkit.ToolkitScriptManager mgrAjax = ScriptManager.GetCurrent(Page) as AjaxControlToolkit.ToolkitScriptManager;
#if DEBUG
				mgrAjax.CombineScripts = false;
#endif
				ScriptReference scrExcanvas                = new ScriptReference ("~/Include/jqPlot/excanvas.min.js"                              );
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//ScriptReference scrJQuery                  = new ScriptReference ("~/Include/jqPlot/jquery.min.js"                                );
				ScriptReference scrJQueryJQplot            = new ScriptReference ("~/Include/jqPlot/jquery.jqplot.min.js"                         );
				ScriptReference scrBarRenderer             = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.barRenderer.min.js"            );
				ScriptReference scrBezierCurveRenderer     = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.BezierCurveRenderer.min.js"    );
				ScriptReference scrBlockRenderer           = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.blockRenderer.min.js"          );
				ScriptReference scrBubbleRenderer          = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.bubbleRenderer.min.js"         );
				ScriptReference scrCanvasAxisLabelRenderer = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.min.js");
				ScriptReference scrCanvasAxisTickRenderer  = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.canvasAxisTickRenderer.min.js" );
				ScriptReference scrCanvasOverlay           = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.canvasOverlay.min.js"          );
				ScriptReference scrCanvasTextRenderer      = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.canvasTextRenderer.min.js"     );
				ScriptReference scrCategoryAxisRenderer    = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.categoryAxisRenderer.min.js"   );
				ScriptReference scrCiParser                = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.ciParser.min.js"               );
				ScriptReference scrCursor                  = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.cursor.min.js"                 );
				ScriptReference scrDateAxisRenderer        = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.dateAxisRenderer.min.js"       );
				ScriptReference scrDonutRenderer           = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.donutRenderer.min.js"          );
				ScriptReference scrDragable                = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.dragable.min.js"               );
				// 06/21/2013 Paul.  Reverse is not a property. 
				// 01/22/2015 Paul.  Missing min from enhancedLegendRenderer. 
				ScriptReference scrEnhancedLegendRenderer  = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.enhancedLegendRenderer.min.js" );
				ScriptReference scrFunnelRenderer          = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.funnelRenderer.min.js"         );
				ScriptReference scrHighlighter             = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.highlighter.min.js"            );
				ScriptReference scrJson2                   = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.json2.min.js"                  );
				ScriptReference scrLogAxisRenderer         = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.logAxisRenderer.min.js"        );
				ScriptReference scrMekkoAxisRenderer       = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.mekkoAxisRenderer.min.js"      );
				ScriptReference scrMekkoRenderer           = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.mekkoRenderer.min.js"          );
				ScriptReference scrMeterGaugeRenderer      = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.meterGaugeRenderer.min.js"     );
				ScriptReference scrOhlcRenderer            = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.ohlcRenderer.min.js"           );
				ScriptReference scrPieRenderer             = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.pieRenderer.min.js"            );
				ScriptReference scrPointLabels             = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.pointLabels.min.js"            );
				ScriptReference scrPyramidAxisRenderer     = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.pyramidAxisRenderer.min.js"    );
				ScriptReference scrPyramidGridRenderer     = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.pyramidGridRenderer.min.js"    );
				ScriptReference scrPyramidRenderer         = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.pyramidRenderer.min.js"        );
				ScriptReference scrTrendline               = new ScriptReference ("~/Include/jqPlot/plugins/jqplot.trendline.min.js"              );
				
				HtmlLink cssJQplot = new HtmlLink();
				cssJQplot.Attributes.Add("href" , "~/Include/jqPlot/jquery.jqplot.min.css");
				cssJQplot.Attributes.Add("type" , "text/css");
				cssJQplot.Attributes.Add("rel"  , "stylesheet");
				Page.Header.Controls.Add(cssJQplot);
				
				HttpRequest Request = HttpContext.Current.Request;
				bool bOldIE = (Request.UserAgent.IndexOf("MSIE 6.0") > 0) || (Request.UserAgent.IndexOf("MSIE 7.0") > 0) || (Request.UserAgent.IndexOf("MSIE 8.0") > 0);
				if ( bOldIE && !mgrAjax.Scripts.Contains(scrExcanvas     ) ) mgrAjax.Scripts.Add(scrExcanvas               );
				// 08/25/2013 Paul.  jQuery now registered in the master pages. 
				//if ( !mgrAjax.Scripts.Contains(scrJQuery                 ) ) mgrAjax.Scripts.Add(scrJQuery                 );
				if ( !mgrAjax.Scripts.Contains(scrJQueryJQplot           ) ) mgrAjax.Scripts.Add(scrJQueryJQplot           );
				if ( !mgrAjax.Scripts.Contains(scrBarRenderer            ) ) mgrAjax.Scripts.Add(scrBarRenderer            );
				if ( !mgrAjax.Scripts.Contains(scrBezierCurveRenderer    ) ) mgrAjax.Scripts.Add(scrBezierCurveRenderer    );
				if ( !mgrAjax.Scripts.Contains(scrBlockRenderer          ) ) mgrAjax.Scripts.Add(scrBlockRenderer          );
				if ( !mgrAjax.Scripts.Contains(scrBubbleRenderer         ) ) mgrAjax.Scripts.Add(scrBubbleRenderer         );
				if ( !mgrAjax.Scripts.Contains(scrCanvasAxisLabelRenderer) ) mgrAjax.Scripts.Add(scrCanvasAxisLabelRenderer);
				if ( !mgrAjax.Scripts.Contains(scrCanvasAxisTickRenderer ) ) mgrAjax.Scripts.Add(scrCanvasAxisTickRenderer );
				if ( !mgrAjax.Scripts.Contains(scrCanvasOverlay          ) ) mgrAjax.Scripts.Add(scrCanvasOverlay          );
				if ( !mgrAjax.Scripts.Contains(scrCanvasTextRenderer     ) ) mgrAjax.Scripts.Add(scrCanvasTextRenderer     );
				if ( !mgrAjax.Scripts.Contains(scrCategoryAxisRenderer   ) ) mgrAjax.Scripts.Add(scrCategoryAxisRenderer   );
				if ( !mgrAjax.Scripts.Contains(scrCiParser               ) ) mgrAjax.Scripts.Add(scrCiParser               );
				if ( !mgrAjax.Scripts.Contains(scrCursor                 ) ) mgrAjax.Scripts.Add(scrCursor                 );
				if ( !mgrAjax.Scripts.Contains(scrDateAxisRenderer       ) ) mgrAjax.Scripts.Add(scrDateAxisRenderer       );
				if ( !mgrAjax.Scripts.Contains(scrDonutRenderer          ) ) mgrAjax.Scripts.Add(scrDonutRenderer          );
				if ( !mgrAjax.Scripts.Contains(scrDragable               ) ) mgrAjax.Scripts.Add(scrDragable               );
				if ( !mgrAjax.Scripts.Contains(scrEnhancedLegendRenderer ) ) mgrAjax.Scripts.Add(scrEnhancedLegendRenderer );
				if ( !mgrAjax.Scripts.Contains(scrFunnelRenderer         ) ) mgrAjax.Scripts.Add(scrFunnelRenderer         );
				if ( !mgrAjax.Scripts.Contains(scrHighlighter            ) ) mgrAjax.Scripts.Add(scrHighlighter            );
				if ( !mgrAjax.Scripts.Contains(scrJson2                  ) ) mgrAjax.Scripts.Add(scrJson2                  );
				if ( !mgrAjax.Scripts.Contains(scrLogAxisRenderer        ) ) mgrAjax.Scripts.Add(scrLogAxisRenderer        );
				if ( !mgrAjax.Scripts.Contains(scrMekkoAxisRenderer      ) ) mgrAjax.Scripts.Add(scrMekkoAxisRenderer      );
				if ( !mgrAjax.Scripts.Contains(scrMekkoRenderer          ) ) mgrAjax.Scripts.Add(scrMekkoRenderer          );
				if ( !mgrAjax.Scripts.Contains(scrMeterGaugeRenderer     ) ) mgrAjax.Scripts.Add(scrMeterGaugeRenderer     );
				if ( !mgrAjax.Scripts.Contains(scrOhlcRenderer           ) ) mgrAjax.Scripts.Add(scrOhlcRenderer           );
				if ( !mgrAjax.Scripts.Contains(scrPieRenderer            ) ) mgrAjax.Scripts.Add(scrPieRenderer            );
				if ( !mgrAjax.Scripts.Contains(scrPointLabels            ) ) mgrAjax.Scripts.Add(scrPointLabels            );
				if ( !mgrAjax.Scripts.Contains(scrPyramidAxisRenderer    ) ) mgrAjax.Scripts.Add(scrPyramidAxisRenderer    );
				if ( !mgrAjax.Scripts.Contains(scrPyramidGridRenderer    ) ) mgrAjax.Scripts.Add(scrPyramidGridRenderer    );
				if ( !mgrAjax.Scripts.Contains(scrPyramidRenderer        ) ) mgrAjax.Scripts.Add(scrPyramidRenderer        );
				if ( !mgrAjax.Scripts.Contains(scrTrendline              ) ) mgrAjax.Scripts.Add(scrTrendline              );
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}
	}
}


