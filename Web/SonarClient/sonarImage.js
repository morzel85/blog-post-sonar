var sonarImage = (function () {
    'use strict';

    var maxDistance = 100;
    var canvas, context;

    var fadeSonarLines = function () {
        var imageData = context.getImageData(0, 0, canvas.width, canvas.height),
            pixels = imageData.data,
            fadeStep = 1,
            green,
            fadedGreen;

        for (var i = 0; i < pixels.length; i += 4) {
            green = pixels[i + 1];

            fadedGreen = green - fadeStep;
            pixels[i + 1] = fadedGreen;
        }

        context.putImageData(imageData, 0, 0);
    };

    return {
        init: function (canvasId) {
            canvas = document.getElementById(canvasId);
            context = canvas.getContext('2d');

            context.lineWidth = 2;
            context.strokeStyle = '#00FF00';
            context.fillStyle = "#000000";

            context.fillRect(0, 0, canvas.width, canvas.height);

            context.translate(canvas.width / 2, 0);
            context.scale(2, 2);
        },

        draw: function (angle, distance) {
            context.save();

            context.rotate((90 - angle) * Math.PI / 180);
            context.beginPath();
            context.moveTo(0, 0);
            context.lineTo(0, distance || maxDistance); // Treat 0 as above range
            context.stroke();

            context.restore();

            fadeSonarLines();
        }
    };
}());