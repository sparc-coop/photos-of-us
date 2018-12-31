import * as angular from 'angular';
import * as api from '../../Services/PhotosOfUs.API';

angular.module('app').controller('RandomPhotoCtrl',
    ['$scope', 'RandomPhotoClient', ($scope, RandomPhotoClient: api.RandomPhotoClient) => {

    function getRandomInt(min: number, max: number) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    }

    // let area = document.getElementsByClassName("main-landing")[0];
    $scope.getRandomPosition = (element) => {
        const x = 800 - element.clientHeight;
        const y = document.body.offsetWidth - element.clientWidth;
        // var randomX = Math.floor(Math.random() * x);
        // var randomY = Math.floor(Math.random() * y);
        const coords = [getRandomInt(0, x), getRandomInt(650, 900)];
        return coords;
    };

    // $scope.loadImages = (ph) => {
    //    var img = document.createElement('img');
    //    img.setAttribute("src", ph.Url);
    //    img.setAttribute("style", "position:absolute; border-radius: 20px; max-width: 400px; max-height: 400px; margin: 50px;");
    //    var xy = $scope.getRandomPosition(img);
    //    img.style.top = xy[0] + 'px';
    //    img.style.left = xy[1] + 'px';

    //    var element = document.getElementById("main-landing");
    //    element.appendChild(img);
    // };

    $scope.photoList = [];

    $scope.getRandomPhotos = (numPhotos: number) => {
        let counter = 0;
        const interval = setInterval(function () {
            counter += 1;
            if (counter === numPhotos) {
                clearInterval(interval);
            }

            RandomPhotoClient.get().then(photo => $scope.photoList.push(photo));
        }, 0);
    };
}]);
