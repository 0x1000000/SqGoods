import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input } from '@angular/core';
import { ActivatedRoute, Router, Routes, UrlSegment } from '@angular/router';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';

type TabData = {
  label: string,
  href: string,
  component: any
};

type State = ComponentState<RouterTabsComponent>;
type NewState = ComponentStateDiff<RouterTabsComponent>;

@Component({
  selector: 'sq-app-router-tabs',
  templateUrl: './router-tabs.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RouterTabsComponent {

  constructor(public router: Router, cd: ChangeDetectorRef, activatedRoute: ActivatedRoute) {
    initializeStateTracking(this, {onStateApplied: () => cd.detectChanges()});
    const self = this;
    activatedRoute.url.subscribe(u => {
      let diff = false;
      if (self.path != null && self.path.length === u.length){
        for (let i = 0; i < self.path.length; i++){
          if (self.path[i].path !== u[i].path){
            diff = true;
            break;
          }
        }
      }
      else{
        diff = true;
      }

      if (diff){
        self.path = u;
      }
    });
  }

  @Input()
  path: UrlSegment[]|null = null;

  tabs: TabData[] = [];

  selectedTabId = '';

  activatedComponent: any = null;

  @With('path', 'router')
  static buildTabs(state: State): NewState {
    let tabs: TabData[];

    if (state.path == null){
      tabs = [];
    }
    else{
      const paths = state.path;
      const currentRoutes = getCurrentRoutes(state.router.config, paths, 0);

      if (currentRoutes != null){
        tabs = currentRoutes
          .filter(c => c.data?.title && c.component)
          .map(r => ({
            label: r.data!.title,
            href: state.path + '/' + r.path!,
            component: r.component!}));
      }
      else{
        tabs = [];
      }
    }

    return {tabs};

    function getCurrentRoutes(routes: Routes, paths: UrlSegment[], offset: number): Routes| null{
      if (paths.length - offset < 1 || paths[offset].path === ''){
        return routes;
      }
      const segment = paths[offset];

      const res = routes.find(r => r.path === segment.path);

      if (res == null || res.children == null){
        return null;
      }
      return getCurrentRoutes(res.children, paths, offset + 1);
    }
  }

  @With('tabs', 'activatedComponent')
  static setSelectedTabId(state: State): NewState {
    if (state.tabs != null && state.activatedComponent != null){
      const t = state.tabs.find(ti => ti.component === state.activatedComponent.constructor);
      if (t) {
        return {selectedTabId: t.href};
      }
    }
    return {selectedTabId: ''};
  }

  onLinkClick(url: string): void{
    this.router.navigateByUrl(url);
  }
}
