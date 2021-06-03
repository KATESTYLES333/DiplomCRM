/*
 *contextMenu.js v 1.2.2
 *Author: Sudhanshu Yadav
 *s-yadav.github.com
 *Copyright (c) 2013 Sudhanshu Yadav.
 *Dual licensed under the MIT and GPL licenses
 */
;(function(e,t,n,r){"use strict";e.fn.contextMenu=function(t,n,r){if(!i[t]){r=n;n=t;t="popup"}else if(n){if(!(n instanceof Array||typeof n==="string"||n.nodeType||n.jquery)){r=n;n=null}}if(n instanceof Array&&t!="update"){t="menu"}var o=r;if(t!="update"){r=s.optionOtimizer(t,r);o=e.extend({},e.fn.contextMenu.defaults,r);if(!o.baseTrigger){o.baseTrigger=this}}i[t].call(this,n,o);return this};e.fn.contextMenu.defaults={triggerOn:"click",displayAround:"cursor",mouseClick:"left",verAdjust:0,horAdjust:0,top:"auto",left:"auto",closeOther:true,containment:t,winEventClose:true,sizeStyle:"auto",position:"auto",closeOnClick:true,onOpen:function(e,t){},afterOpen:function(e,t){},onClose:function(e,t){}};var i={menu:function(t,n){var r=e(this);t=s.createMenuList(r,t,n);s.contextMenuBind.call(this,t,n,"menu")},popup:function(t,n){e(t).addClass("iw-contextMenu");s.contextMenuBind.call(this,t,n,"popup")},update:function(t,n){var r=this;this.each(function(){var i=e(this),o=i.data("iw-menuData");if(!o){r.contextMenu("refresh");o=i.data("iw-menuData")}var u=o.menu;if(typeof t==="object"){for(var a=0;a<t.length;a++){var f=t[a].name,l=t[a].disable,c=u.children("li").filter(function(){return e(this).contents().filter(function(){return this.nodeType==3}).text()==f}),h=t[a].subMenu;if(l=="true"){c.addClass("iw-mDisable")}else{c.removeClass("iw-mDisable")}if(h){c.contextMenu("update",h)}}}s.onOff(u);o.option=e.extend({},o.option,n);i.data("iw-menuData",o);var p=o.option.triggerOn;if(n){if(p!=n.triggerOn){i.unbind(".contextMenu");i.bind(p+".contextMenu",s.eventHandler)}}})},refresh:function(){var t=this.filter(function(){return!!e(this).data("iw-menuData")}).data("iw-menuData"),n=this.filter(function(){return!e(this).data("iw-menuData")});t.option.baseTrigger=this;s.contextMenuBind.call(n,t.menuSelector,t.option)},close:function(){var e=this.data("iw-menuData");if(e){s.closeContextMenu(e.option,this,e.menu,null)}},value:function(e){var t=this.data("iw-menuData");if(t[e]){return t[e]}else if(t.option){return t.option[e]}return null},destroy:function(){this.each(function(){var t=e(this),n=t.data("iw-menuData").menuId,r=e(".iw-contextMenu[menuId="+n+"]"),i=r.data("iw-menuData");if(!i)return;if(i.noTrigger==1){if(r.hasClass("iw-created")){r.remove()}else{r.removeClass("iw-contextMenu "+n).removeAttr("menuId").removeData("iw-menuData");r.find("li.iw-mTrigger").contextMenu("destroy")}}else{i.noTrigger--;r.data("iw-menuData",i)}t.unbind(".contextMenu").removeClass("iw-mTrigger").removeData("iw-menuData")})}};var s={contextMenuBind:function(t,n,r){var i=this,o=e(t),u=o.data("iw-menuData");if(o.length==0){o=i.find(t);if(o.length==0){return}}if(r=="menu"){s.menuHover(o)}var a=n.baseTrigger;if(!u){var f;if(!a.data("iw-menuData")){f=Math.ceil(Math.random()*1e5);a.data("iw-menuData",{menuId:f})}else{f=a.data("iw-menuData").menuId}var l=o.clone();l.appendTo("body");u={menuId:f,menuWidth:l.outerWidth(true),menuHeight:l.outerHeight(true),noTrigger:1,trigger:i};o.data("iw-menuData",u).attr("menuId",f);l.remove()}else{u.noTrigger++;o.data("iw-menuData",u)}i.addClass("iw-mTrigger").data("iw-menuData",{menuId:u.menuId,option:n,menu:o,menuSelector:t,method:r});var c;if(n.triggerOn=="hover"){c="mouseenter";if(a.index(i)!=-1){a.add(o).bind("mouseleave.contextMenu",function(t){if(e(t.relatedTarget).closest(".iw-contextMenu").length==0){e('.iw-contextMenu[menuId="'+u.menuId+'"]').hide(100)}})}}else{c=n.triggerOn}i.delegate("input,a,.needs-click","click",function(e){e.stopImmediatePropagation()});i.bind(c+".contextMenu",s.eventHandler);o.bind("click mouseenter",function(e){e.stopPropagation()});o.delegate("li","click",function(e){if(n.closeOnClick)s.closeContextMenu(n,i,o,e)})},eventHandler:function(r){r.preventDefault();var i=e(this),o=i.data("iw-menuData"),u=o.menu,a=u.data("iw-menuData"),f=o.option,l=f.containment,c={trigger:i,menu:u},h=l==t,p=f.baseTrigger.index(i)==-1;if(!p&&f.closeOther){e(".iw-contextMenu").css("display","none")}u.find(".iw-mSelected").removeClass("iw-mSelected");f.onOpen.call(this,c,r);var d=e(l),v=d.innerHeight(),m=d.innerWidth(),g=0,y=0,b=a.menuHeight,w=a.menuWidth,E,S,x=0,T=0,N,C,k=E=parseInt(f.verAdjust),L=S=parseInt(f.horAdjust);if(!h){g=d.offset().top;y=d.offset().left;if(d.css("position")=="static"){d.css("position","relative")}}if(f.sizeStyle=="auto"){b=Math.min(b,v);w=Math.min(w,m);w=w+20}if(f.displayAround=="cursor"){x=h?r.clientX:r.clientX+e(t).scrollLeft()-y;T=h?r.clientY:r.clientY+e(t).scrollTop()-g;N=T+b;C=x+w;if(N>v){if(T-b<0){if(N-v<b-T){T=v-b;E=-1*E}else{T=0;E=0}}else{T=T-b;E=-1*E}}if(C>m){if(x-w<0){if(C-m<w-x){x=m-w;S=-1*S}else{x=0;S=0}}else{x=x-w;S=-1*S}}}else if(f.displayAround=="trigger"){var A=i.outerHeight(true),O=i.outerWidth(true),M=h?i.offset().left-d.scrollLeft():i.offset().left-y,_=h?i.offset().top-d.scrollTop():i.offset().top-g,D=O;x=M+O;T=_;N=T+b;C=x+w;if(N>v){if(T-b<0){if(N-v<b-T){T=v-b;E=-1*E}else{T=0;E=0}}else{T=T-b+A;E=-1*E}}if(C>m){if(x-w<0){if(C-m<w-x){x=m-w;S=-1*S;D=-O}else{x=0;S=0;D=0}}else{x=x-w-O;S=-1*S;D=-O}}if(f.position=="top"){b=Math.min(a.menuHeight,_);T=_-b;E=k;x=x-D}else if(f.position=="left"){w=Math.min(a.menuWidth,M);x=M-w;S=L}else if(f.position=="bottom"){b=Math.min(a.menuHeight,v-_-A);T=_+A;E=k;x=x-D}else if(f.position=="right"){w=Math.min(a.menuWidth,m-M-O);x=M+O;S=L}}var P=u.outerWidth(true)-u.width(),H=u.outerHeight(true)-u.height();var B={position:h||p?"fixed":"absolute",display:"inline-block",height:"",width:"","overflow-y":b!=a.menuHeight?"auto":"hidden","overflow-x":w!=a.menuWidth?"auto":"hidden"};if(f.sizeStyle=="auto"){B.height=b-H+"px";B.width=w-P+"px"}if(f.left!="auto"){x=s.getPxSize(f.left,m)}if(f.top!="auto"){T=s.getPxSize(f.top,v)}if(!h){var j=i.offsetParent().offset();if(p){x=x+y-e(t).scrollLeft();T=T+g-e(t).scrollTop()}else{x=x-(y-j.left);T=T-(g-j.top)}}B.left=x+S+"px";B.top=T+E+"px";u.css(B);f.afterOpen.call(this,c,r);if(i.closest(".iw-contextMenu").length==0){e(".iw-curMenu").removeClass("iw-curMenu");u.addClass("iw-curMenu")}var F={trigger:i,menu:u,option:f,method:o.method};e("html").unbind("click",s.clickEvent).click(F,s.clickEvent);e(n).unbind("keydown",s.keyEvent).keydown(F,s.keyEvent);if(f.winEventClose){e(t).bind("scroll resize",F,s.scrollEvent)}},scrollEvent:function(e){s.closeContextMenu(e.data.option,e.data.trigger,e.data.menu,e)},clickEvent:function(t){var n=t.data.trigger.get(0);if(n!==t.target&&e(t.target).closest(".iw-contextMenu").length==0){s.closeContextMenu(t.data.option,t.data.trigger,t.data.menu,t)}},keyEvent:function(t){t.preventDefault();var n=t.data.menu,r=t.data.option,i=t.keyCode;if(i==27){s.closeContextMenu(r,t.data.trigger,n,t)}if(t.data.method=="menu"){var o=e(".iw-curMenu"),u=o.children("li:not(.iw-mDisable)"),a=u.filter(".iw-mSelected"),f=u.index(a),l=function(e){a.removeClass("iw-mSelected");e.addClass("iw-mSelected")},c=function(){l(u.filter(":first"))},h=function(){l(u.filter(":last"))},p=function(){l(u.filter(":eq("+(f+1)+")"))},d=function(){l(u.filter(":eq("+(f-1)+")"))},v=function(){var e=a.data("iw-menuData");if(e){a.triggerHandler("mouseenter.contextMenu");var t=e.menu;t.addClass("iw-curMenu");o.removeClass("iw-curMenu");o=t;u=o.children("li:not(.iw-mDisable)");a=u.filter(".iw-mSelected");c()}},m=function(){var e=o.data("iw-menuData").trigger;var t=e.closest(".iw-contextMenu");if(t.length!=0){o.removeClass("iw-curMenu").css("display","none");t.addClass("iw-curMenu")}};switch(i){case 13:a.click();break;case 40:f==u.length-1||a.length==0?c():p();break;case 38:f==0||a.length==0?h():d();break;case 33:c();break;case 34:h();break;case 37:m();break;case 39:v();break}}},closeContextMenu:function(r,i,o,u){e(n).unbind("keydown",s.keyEvent);e("html").unbind("click",s.clickEvent);e(t).unbind("scroll resize",s.scrollEvent);e(".iw-contextMenu").hide();e(n).focus();r.onClose.call(this,{trigger:i,menu:o},u)},getPxSize:function(e,t){if(!isNaN(e)){return e}if(e.indexOf("%")!=-1){return parseInt(e)*t/100}else{return parseInt(e)}},menuHover:function(t){t.children("li").bind("mouseenter",function(n){e(".iw-curMenu").removeClass("iw-curMenu");t.addClass("iw-curMenu");var r=t.find("li.iw-mSelected"),i=r.find(".iw-contextMenu");if(i.length!=0&&r[0]!=this){i.hide(100)}r.removeClass("iw-mSelected");e(this).addClass("iw-mSelected")})},createMenuList:function(n,r,i){var o=i.baseTrigger,u=Math.floor(Math.random()*1e4);if(typeof r=="object"&&!r.nodeType&&!r.jquery){var a=e('<ul class="iw-contextMenu iw-created iw-cm-menu" id="iw-contextMenu'+u+'"></ul>');for(var f=0;f<r.length;f++){var l=r[f],c=l.name,h=l.fun,p=l.subMenu,d=l.img||"",v=l.title||"",m=l.className||"",g=l.disable,y=e('<li title="'+v+'" class="'+m+'">'+c+"</li>");if(d){y.prepend('<img src="'+d+'" align="absmiddle" class="iw-mIcon" />')}if(g=="true"){y.addClass("iw-mDisable")}y.bind("click",h);a.append(y);if(p){y.append('<div class="iw-cm-arrow-right" />');s.subMenu(y,p,o,i)}}if(o.index(n[0])==-1){n.append(a)}else{var b=i.containment==t?"body":i.containment;e(b).append(a)}s.onOff(e("#iw-contextMenu"+u));return"#iw-contextMenu"+u}else if(e(r).length!=0){var w=e(r);w.removeClass("iw-contextMenuCurrent").addClass("iw-contextMenu iw-cm-menu iw-contextMenu"+u).attr("menuId","iw-contextMenu"+u).css("display","none");w.find("ul").each(function(t,n){var r=e(this),u=r.parent("li");u.append('<div class="iw-cm-arrow-right" />');r.addClass("iw-contextMenuCurrent");s.subMenu(u,".iw-contextMenuCurrent",o,i)});s.onOff(e(".iw-contextMenu"+u));return".iw-contextMenu"+u}},subMenu:function(e,t,n,r){e.contextMenu("menu",t,{triggerOn:"hover",displayAround:"trigger",position:"auto",baseTrigger:n,containment:r.containment})},onOff:function(t){t.find(".iw-mOverlay").remove();t.find(".iw-mDisable").each(function(){var t=e(this);t.append('<div class="iw-mOverlay"/>');t.find(".iw-mOverlay").bind("click mouseenter",function(e){e.stopPropagation()})})},optionOtimizer:function(t,n){if(!n){return}if(t=="menu"){if(!n.mouseClick){n.mouseClick="right"}}if(n.mouseClick=="right"&&n.triggerOn=="click"){n.triggerOn="contextmenu"}if(e.inArray(n.triggerOn,["hover","mouseenter","mouseover","mouseleave","mouseout","focusin","focusout"])!=-1){n.displayAround="trigger"}return n}}})(jQuery,window,document);