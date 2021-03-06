import { In, With, Out } from 'ng-set-state';

export class RadioListState {

    constructor() {
        this.radioName = 'RL-' + Math.random().toString(36).substring(2, 15);
    }

    public static readonly ngInputs = ['items', 'selected', 'titleProperty', 'valueProperty'];

    public static readonly ngOutputs = ['selectedChange','selectedItemChange'];

    @In()
    public readonly items: any[];

    @In()
    public readonly titleProperty: string;

    @In()
    public readonly valueProperty: string;

    @In()
    @Out()
    public readonly selected: Object;

    @Out()
    public readonly selectedItem: any;

    public readonly radioName: string;

    public readonly viewItems: ItemView[];

    @With('items', 'titleProperty', 'valueProperty', 'radioName')
    public static withItems(state: RadioListState): Partial<RadioListState> | null {

        if (!state.items || !state.radioName) {
            return null;
        }

        const newViewItems = state.items.map((i, index) =>  ({
            id: state.radioName + index.toString(),
            value: RadioListState.getProperty(i, state.valueProperty),
            title: RadioListState.getProperty(i, state.titleProperty),
            checked: state.shouldBeChecked(RadioListState.getProperty(i, state.valueProperty)),
            target: i,
        }) as ItemView);

        return {
            viewItems: newViewItems,
            selectedItem: RadioListState.getSelectedItem(newViewItems)
        };
    }

    @With('selected')
    public static withSelected(state: RadioListState): Partial<RadioListState> | null {

        const requireUpdate = (vi: ItemView) => vi.checked && !state.shouldBeChecked(vi.value) || !vi.checked && state.shouldBeChecked(vi.value);

        if (state.viewItems && state.viewItems.some(vi => requireUpdate(vi))) {
            const newViewItems = state.viewItems.map(vi => !requireUpdate(vi)
                ? vi
                : Object.assign(vi,
                     {
                        checked: state.shouldBeChecked(vi.value)
                    } as Partial<ItemView>));
            return {
                viewItems: newViewItems,
                selectedItem: RadioListState.getSelectedItem(newViewItems)
            };
        }
        return null;
    }

    private static getSelectedItem(viewItems: ItemView[]): any {
        if (viewItems) {
            const vi = viewItems.find(i => i.checked);
            if (vi) {
                return vi.target;
            }
        }
        return null;
    }

    private static getProperty(item: Object, propName: string): Object {
        if (!item || !propName) {
            return item;
        }
        return item[propName];
    }

    private shouldBeChecked(itemValue: Object): boolean {
        return this.selected != null && itemValue === this.selected;
    }
}

export type ItemView = {
    readonly id: string,
    readonly value: Object,
    readonly title: string,
    readonly checked: boolean,
    readonly target: any,
};
