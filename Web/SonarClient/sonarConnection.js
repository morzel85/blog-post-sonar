var sonarConnection = (function () {
    'use strict';

    var sonarHub, startTime, numberOfMessages, numberOfSamples;

    var processSonarData = function (sonarData) {
        numberOfMessages++;
        $.each(sonarData, function (index, item) {
            numberOfSamples++;
            sonarImage.draw(item.Angle, item.Distance);
            sonarStats.fillTable(item.Angle, item.Distance, startTime, numberOfMessages, numberOfSamples);
        });
    };

    return {
        init: function (url) {
            $.connection.hub.url = url;

            sonarHub = $.connection.sonarHub;

            if (sonarHub) {
                sonarHub.client.sonarData = processSonarData;
                               
                startTime = new Date();
                numberOfMessages = 0;
                numberOfSamples = 0;

                $.connection.hub.start();
            } else {
                alert('Sonar hub not found! Are you sure the server is working and URL is set correctly?');
            }
        }
    };
}());