import { StateTracking } from 'ng-set-state';
import { TabPaneComponent } from './tab-pane.component';

@StateTracking({includeAllPredefinedFields: true})
export class TabsState{

  selectedPane: TabPaneComponent|null = null;

  previewPane: TabPaneComponent|null = null;

  inkPosition: [number, number]|null = null;
}
