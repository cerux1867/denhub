import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'humaniseViews' })
export class HumaniseViewsPipe implements PipeTransform {
    /**
     * Takes an integer representing the amount of views on a VoD and converts it to a more human-friendly format. Example 300000 -> 300K.
     * @param value Number of views
     */
    transform(value: number) {
        let humanisedValue = 0;
        let sizeDenominator = "";

        if (value > 1000) {
            humanisedValue = value / 1000;
            sizeDenominator = "K";
        } else if (value > 1000000) {
            humanisedValue = value / 1000000;
            sizeDenominator = "M";
        }

        const fractionDigits =  humanisedValue % 1 < 0.1 ? 0 : 1;

        return `${humanisedValue.toLocaleString(undefined, {
            minimumFractionDigits: fractionDigits,
            maximumFractionDigits: fractionDigits
        })}${sizeDenominator}`;
    }

}