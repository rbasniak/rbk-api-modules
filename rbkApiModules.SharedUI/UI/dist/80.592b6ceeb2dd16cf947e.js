(self.webpackChunkrbk_shared_ui=self.webpackChunkrbk_shared_ui||[]).push([[80],{7080:(e,t,n)=>{"use strict";n.r(t),n.d(t,{AdminModule:()=>y});var o=n(2561),a=n(1116),r=n(1039),c=n(6726),s=n(8203),i=n(8751),l=n(4762),u=n(3623),p=n(6905),d=n(5366),f=n(2995);let m=(()=>{let e=class{constructor(e){this.store=e,this.formConfig=null}ngOnInit(){this.formConfig={behaviors:{flattenResponse:!0,avoidFocusOnLoad:!0},groups:[{name:null,showName:!0,children:[{propertyName:"searchText",type:r.rc7.TEXT,name:"Matching text",defaultValue:"",template:{extraLarge:{row:"col-12"}}}],template:{extraLarge:{row:"col-12"}}}]}}execute(e){const t=e.getData().data;this.store.dispatch(new p.n.DeleteBasedOnPathText(t.searchText))}};return e.\u0275fac=function(t){return new(t||e)(d.Y36(f.yh))},e.\u0275cmp=d.Xpm({type:e,selectors:[["app-admin-page"]],decls:6,vars:1,consts:[["header","DELETE ENTRIES MATCHING TEXT"],[1,"p-d-flex","p-ai-center"],[3,"config"],["form",""],[1,"mt-6"],["pButton","","type","button","label","RUN","icon","fas fa-play","iconPos","left",3,"click"]],template:function(e,t){if(1&e){const e=d.EpF();d.TgZ(0,"p-panel",0),d.TgZ(1,"div",1),d._UZ(2,"smz-form-group",2,3),d.TgZ(4,"div",4),d.TgZ(5,"button",5),d.NdJ("click",function(){d.CHM(e);const n=d.MAs(3);return t.execute(n)}),d.qZA(),d.qZA(),d.qZA(),d.qZA()}2&e&&(d.xp6(2),d.Q6J("config",t.formConfig))},directives:[s.s,r.HK$,c.Hq],styles:[""],encapsulation:2}),(0,l.gn)([(0,r.zGJ)("Are you sure you want to continue? Deleted data cannot be recovered","Confirmation",!0)],e.prototype,"execute",null),e=(0,l.gn)([(0,u.c)()],e),e})();var h=n(1945),g=n(3464);const T=[{path:"",children:[{path:g.R3+"/"+g.no,component:m,data:{layout:{mode:"menu-only",contentPadding:"2em"},title:"Admin",appArea:"admin",clearReusableRoutes:!0}}]}];let y=(()=>{class e{}return e.\u0275fac=function(t){return new(t||e)},e.\u0275mod=d.oAB({type:e}),e.\u0275inj=d.cJS({providers:[],imports:[[a.ez,o.Bz.forChild(T),c.hJ,r.Z4r,s.Q,r.P$2,h.V,r.Yel,i.d]]}),e})()}}]);