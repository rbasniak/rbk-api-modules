(window.webpackJsonp=window.webpackJsonp||[]).push([[4],{"Nl/z":function(t,e,n){"use strict";n.r(e),n.d(e,"HomeModule",function(){return f});var o=n("tyNb"),i=n("ofXK"),c=n("mrSG"),s=n("VfN6"),r=n("fXoL"),a=n("kgK3"),l=n("AytR"),p=n("tk/3");let u=(()=>{class t extends a.e{constructor(t){super(),this.http=t,this.endpoint=`${window.location.origin}/api/analytics`,l.a.production||(this.endpoint=`${l.a.serverUrl}/api/analytics`),console.log(window.location),console.log(this.endpoint)}test(){return this.http.get(`${this.endpoint}/test`,this.generateDefaultHeaders({}))}}return t.\u0275fac=function(e){return new(e||t)(r.Zb(p.b))},t.\u0275prov=r.Lb({token:t,factory:t.\u0275fac,providedIn:"root"}),t})();var b=n("jIHw");function d(t,e){if(1&t&&(r.Vb(0,"p"),r.Kc(1),r.Ub()),2&t){const t=e.$implicit;r.Eb(1),r.Lc(t)}}const h=[{path:"",children:[{path:"",component:(()=>{let t=class{constructor(t){this.service=t,this.items=[]}ngOnInit(){}test(){this.service.test().subscribe(t=>{console.log(t),this.items=t})}};return t.\u0275fac=function(e){return new(e||t)(r.Pb(u))},t.\u0275cmp=r.Jb({type:t,selectors:[["app-home"]],decls:4,vars:1,consts:[["pButton","","type","button","label","Click",3,"click"],[4,"ngFor","ngForOf"]],template:function(t,e){1&t&&(r.Vb(0,"h1"),r.Kc(1,"Home!"),r.Ub(),r.Vb(2,"button",0),r.dc("click",function(){return e.test()}),r.Ub(),r.Ic(3,d,2,1,"p",1)),2&t&&(r.Eb(3),r.nc("ngForOf",e.items))},directives:[b.b,i.k],styles:[".home_page-container[_ngcontent-%COMP%]{position:absolute;top:0;bottom:0;left:0;right:0}"]}),t=Object(c.b)([Object(s.a)()],t),t})(),data:{layout:{mode:"layout",contentPadding:"2em"},title:"Home",appArea:"home",clearReusableRoutes:!0,requiredStates:[]}}]}];let f=(()=>{class t{}return t.\u0275fac=function(e){return new(e||t)},t.\u0275mod=r.Nb({type:t}),t.\u0275inj=r.Mb({providers:[],imports:[[i.c,o.n.forChild(h),b.c]]}),t})()}}]);