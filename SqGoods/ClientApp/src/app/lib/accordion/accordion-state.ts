import { StateTracking } from 'ng-set-state';
import { AccordionPaneComponent } from './accordion-pane.component';

@StateTracking({includeAllPredefinedFields: true})
export class AccordionState {

  selectedPane: AccordionPaneComponent|null = null;

  firstPane: AccordionPaneComponent|null = null;
}
