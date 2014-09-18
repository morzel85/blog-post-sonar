var sonarStats = (function () {
    'use strict';

    return {       
        fillTable: function (angle, distance, startTime, numberOfMessages, numberOfSamples) {                     
            var elapsedSeconds = (new Date() - startTime) / 1000;

            $('#angle').text(angle);
            $('#distance').text(distance);

            $('#numberOfMessages').text(numberOfMessages);
            $('#numberOfSamples').text(numberOfSamples);

            $('#elapsedSeconds').text(elapsedSeconds.toFixed(2));

            if (elapsedSeconds > 0) {
                $('#messagesPerSecond').text((numberOfMessages / elapsedSeconds).toFixed(2));
                $('#samplesPerSecond').text((numberOfSamples / elapsedSeconds).toFixed(2));
            }
        }
    };
}());