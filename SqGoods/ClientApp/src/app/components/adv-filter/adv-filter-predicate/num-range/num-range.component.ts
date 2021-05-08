import { ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ComponentState, ComponentStateDiff, initializeStateTracking, With } from 'ng-set-state';

type State = ComponentState<NumRangeComponent>;
type NewState = ComponentStateDiff<NumRangeComponent>;

export type NumRange = {from: number|null, to: number|null};

export function isNumRange(value: any): value is NumRange{
  if (value == null){
    return false;
  }
  const from = (value as NumRange).from;
  const to = (value as NumRange).to;

  if (typeof from === 'undefined' || (from != null && typeof from !== 'number')){
    return false;
  }
  if (typeof to === 'undefined' || (to != null && typeof to !== 'number')){
    return false;
  }
  return true;
}

export function toNumRange(from: number|null, to: number|null): NumRange{
  return {from, to};
}

@Component({
  selector: 'app-num-range',
  templateUrl: './num-range.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NumRangeComponent {

  @Input('value')
  valueIn: NumRange|null = null;

  value: NumRange|null = null;

  @Output()
  readonly valueChange = new EventEmitter<NumRange>();

  fromStr: string|null = null;
  toStr: string|null = null;

  constructor(cd: ChangeDetectorRef){
    initializeStateTracking(this, {
      onStateApplied: () => cd.detectChanges(),
      includeAllPredefinedFields: true
    });
  }

  @With('valueIn')
  static onValueIn(s: State): NewState{

    if (s.valueIn == null){
      return {
        value: null,
        fromStr: null,
        toStr: null
      };
    }
    return {
      value: s.valueIn,
      fromStr: s.valueIn.from?.toString(),
      toStr: s.valueIn.to?.toString()
    };
  }

  @With('fromStr', 'toStr')
  static onTextChange(s: State): NewState{

    let from: number| null = null;
    let to: number| null = null;

    if (s.fromStr != null){
      from = Number.parseInt(s.fromStr);
      if (Number.isNaN(from)){
        return {
          value: null
        };
      }
    }

    if (s.toStr != null){
      to = Number.parseInt(s.toStr);
      if (Number.isNaN(to)){
        return {
          value: null
        };
      }
    }

    return {
      value: {from, to}
    };
  }

  onFromChange(input: EventTarget | null): void {
    let str: string|null = (input as HTMLInputElement).value;

    if (str != null){
      str = str.trim();
      if (str === ''){
        str = null;
      }
    }
    this.fromStr = str;
  }

  onToChange(input: EventTarget | null): void {
    let str: string|null = (input as HTMLInputElement).value;

    if (str != null){
      str = str.trim();
      if (str === ''){
        str = null;
      }
    }
    this.toStr = str;
  }
}
