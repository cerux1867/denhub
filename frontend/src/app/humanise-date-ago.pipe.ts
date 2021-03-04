import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'humaniseDateAgo' })
export class HumaniseDateAgo implements PipeTransform {
    /**
     * Takes a value in miliseconds and converts it to a humanised format. Example: 3000 becomes '3 seconds'.
     * @param value value to humanise in miliseconds
     */
    transform(value: number) {
        if (value) {
            const seconds = Math.floor((+ new Date() - +new Date(value)) / 1000);
            if (seconds < 29) {
                return 'Just now'
            }
            const intervals = [
                {
                    name: 'year',
                    value: 31536000
                },
                {
                    name: 'month',
                    value: 2592000
                },
                {
                    name: 'week',
                    value: 604800
                },
                {
                    name: 'day',
                    value: 86400
                },
                {
                    name: 'hour',
                    value: 3600
                },
                {
                    name: 'minute',
                    value: 60
                },
                {
                    name: 'second',
                    value: 1
                }
            ];
            let counter;
            for (const i of intervals) {
                counter = Math.floor(seconds / i.value);
                if (counter > 0) {
                    if (counter === 1) {
                        return counter + ' ' + i.name + ' ago';
                    } else {
                        return counter + ' ' + i.name + 's ago';
                    }
                }
            }
        }

        return value;
    }

}